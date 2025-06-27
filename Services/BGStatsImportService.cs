using MagicDeckStats.Models;
using System.Text.Json;

namespace MagicDeckStats.Services;

public interface IBGStatsImportService
{
    Task<List<Play>> GetMagicPlaysAsync();
    Task<bool> ImportDataAsync(string jsonContent);
    Task<BGStatsExport?> GetCurrentDataAsync();
}

public class BGStatsImportService(HttpClient httpClient, ILogger<BGStatsImportService> logger) : IBGStatsImportService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<BGStatsImportService> _logger = logger;
    private BGStatsExport? _cachedData;
    private int _magicGameId = -1;
    private readonly SemaphoreSlim _dataLoadSemaphore = new(1, 1);
    private readonly SemaphoreSlim _playsLoadSemaphore = new(1, 1);

    public async Task<List<Play>> GetMagicPlaysAsync()
    {
        await _playsLoadSemaphore.WaitAsync();
        try
        {
            if (_cachedData == null)
                await LoadBGStatsDataAsync();

            if (_cachedData == null || _cachedData.Plays == null)
            {
                _logger.LogWarning("No valid BGStats data available");
                return [];
            }

            _logger.LogInformation("Loaded {PlayCount} plays from BGStats export", _cachedData.Plays.Count);

            if (_magicGameId == -1)
            {
                _logger.LogWarning("Magic: The Gathering game not found in BGStats export");
                return [];
            }

            // Create a lookup dictionary for player names
            var playerNamesById = _cachedData.Players?.ToDictionary(p => p.Id, p => p.Name) ?? [];

            foreach (var play in _cachedData.Plays)
            {
                try
                {
                    // Set PlayerName for each PlayerScore and clean deck names
                    foreach (var playerScore in play.PlayerScores ?? [])
                    {
                        if (playerNamesById.TryGetValue(playerScore.PlayerRefId, out var name))
                            playerScore.PlayerName = name;
                        else
                        {
                            _logger.LogError("Player with {PlayerRedId} not found", playerScore.PlayerRefId);
                            playerScore.PlayerName = "Unknown Player";
                        }

                        playerScore.Deck = CleanDeckPrefix(playerScore.Deck);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing play with date: {PlayDate}", play.Date);
                }
            }

            _logger.LogInformation("Successfully parsed {ParsedPlayCount} Magic: The Gathering plays", _cachedData.Plays.Count);
            return _cachedData.Plays;
        }
        finally
        {
            _playsLoadSemaphore.Release();
        }
    }

    public async Task<bool> ImportDataAsync(string jsonContent)
    {
        try
        {
            _logger.LogInformation("Importing new BGStats data...");
            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var importedData = JsonSerializer.Deserialize<BGStatsExport>(jsonContent, jsonOptions);

            if (importedData == null)
            {
                _logger.LogError("Failed to deserialize imported BGStats data");
                return false;
            }

            // Clear cached data to force reload
            await _dataLoadSemaphore.WaitAsync();
            try
            {
                _cachedData = importedData;
                _magicGameId = -1; // Clear magic game ID to force recalculation

                SetMagicGameId();
                PurgeIrrelevantData();

                _logger.LogInformation("Successfully imported BGStats data with {GameCount} games, {PlayCount} plays, {PlayerCount} players",
                    importedData.Games.Count, importedData.Plays.Count, importedData.Players.Count);
                return true;
            }
            finally
            {
                _dataLoadSemaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing BGStats data");
            return false;
        }
    }

    public async Task<BGStatsExport?> GetCurrentDataAsync()
    {
        return await LoadBGStatsDataAsync();
    }

    private static string CleanDeckPrefix(string deckName)
    {
        if (string.IsNullOrWhiteSpace(deckName))
            return deckName;

        const string battleDeckPrefix = "[Battle Deck]";

        if (deckName.StartsWith(battleDeckPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var cleanedName = deckName[battleDeckPrefix.Length..].Trim();
            return string.IsNullOrWhiteSpace(cleanedName) ? deckName : cleanedName;
        }

        return deckName;
    }

    private async Task<BGStatsExport> LoadBGStatsDataAsync()
    {
        _logger.LogInformation("LoadBGStatsDataAsync - Thread ID: {ThreadId}", Environment.CurrentManagedThreadId);

        if (_cachedData != null)
        {
            _logger.LogInformation("Returning cached BGStats data (Plays: {PlayCount})", 
                _cachedData.Plays.Count);
            return _cachedData;
        }

        await _dataLoadSemaphore.WaitAsync();
        try
        {
            // Double-check pattern after acquiring the semaphore
            if (_cachedData != null)
            {
                _logger.LogInformation("Returning cached BGStats data after semaphore wait (Plays: {PlayCount})", 
                    _cachedData.Plays.Count);
                return _cachedData;
            }

            _logger.LogInformation("Loading BGStats export data... (Thread ID: {ThreadId})", Environment.CurrentManagedThreadId);
            var loadStartTime = DateTime.UtcNow;
            
            // Try to load data from either BGStatsExport.json or SampleData.json
            string jsonContent;
            string dataSource;
            
            try
            {
                var result = await LoadJsonContentAsync();
                jsonContent = result.jsonContent;
                dataSource = result.dataSource;
            }
            catch (HttpRequestException)
            {
                _logger.LogError("No valid JSON content could be loaded from any file");
                return CreateEmptyBGStatsExport();
            }

            // Parse the JSON content
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            try
            {
                _cachedData = JsonSerializer.Deserialize<BGStatsExport>(jsonContent, jsonOptions);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to parse JSON from {DataSource}. The file may be corrupted or not in the expected format.", dataSource);
                return CreateEmptyBGStatsExport();
            }

            if (_cachedData == null)
            {
                _logger.LogError("Failed to deserialize BGStats export data from {DataSource}", dataSource);
                return CreateEmptyBGStatsExport();
            }

            SetMagicGameId();
            PurgeIrrelevantData();

            var loadDuration = DateTime.UtcNow - loadStartTime;
            _logger.LogInformation("Successfully loaded {DataSource} with {PlayCount} plays in {LoadDuration}ms",
                dataSource, _cachedData.Plays.Count, loadDuration.TotalMilliseconds);

            return _cachedData;
        }
        finally
        {
            _dataLoadSemaphore.Release();
        }
    }

    private static BGStatsExport CreateEmptyBGStatsExport()
    {
        return new BGStatsExport
        {
            Games = [],
            Plays = [],
            Players = []
        };
    }

    private void SetMagicGameId()
    {
        if (_cachedData == null)
        {
            _magicGameId = -1;
            return;
        }

        var magicGame = _cachedData.Games.FirstOrDefault(g => g.Name.Equals("Magic: The Gathering", StringComparison.OrdinalIgnoreCase));

        if (magicGame == null)
        {
            _logger.LogWarning("Magic: The Gathering game not found!");
            _magicGameId = -1;
            return;
        }

        _magicGameId = magicGame.Id;
        _logger.LogInformation("Found Magic: The Gathering game with ID: {GameId}", _magicGameId);
    }

    private void PurgeIrrelevantData()
    {
        if (_cachedData == null)
            return;

        _logger.LogInformation("Purging irrelevant data from BGStats export...");

        // Keep only the Magic: The Gathering game
        var magicGame = _cachedData.Games.FirstOrDefault(g => g.Name.Equals("Magic: The Gathering", StringComparison.OrdinalIgnoreCase));

        if (magicGame != null)
            _cachedData.Games = [magicGame];
        else
            _cachedData.Games.Clear();

        // Keep only plays that reference the Magic game
        var originalPlayCount = _cachedData.Plays.Count;
        _cachedData.Plays = [.. _cachedData.Plays.Where(p => p.GameRefId == _magicGameId)];
        _logger.LogInformation("Removed {RemovedPlayCount} non-Magic plays, kept {KeptPlayCount} Magic plays", 
            originalPlayCount - _cachedData.Plays.Count, _cachedData.Plays.Count);

        // Temporary filter: only include Battle Decks variants
        _cachedData.Plays = [.. _cachedData.Plays.Where(p => p.Variant.Contains("Battle Decks") && !p.Variant.Contains("Two-Headed Giant"))];

        // Keep only players that are referenced in the remaining plays
        var playerIdsInPlays = _cachedData.Plays
            .SelectMany(p => p.PlayerScores)
            .Select(ps => ps.PlayerRefId)
            .Distinct()
            .ToHashSet();

        var originalPlayerCount = _cachedData.Players.Count;
        _cachedData.Players = [.. _cachedData.Players.Where(p => playerIdsInPlays.Contains(p.Id))];
        _logger.LogInformation("Removed {RemovedPlayerCount} unreferenced players, kept {KeptPlayerCount} referenced players",
            originalPlayerCount - _cachedData.Players.Count, _cachedData.Players.Count);

        _logger.LogInformation("Data purging completed. Final counts - Games: {GameCount}, Plays: {PlayCount}, Players: {PlayerCount}",
            _cachedData.Games.Count, _cachedData.Plays.Count, _cachedData.Players.Count);
    }

    private async Task<(string jsonContent, string dataSource)> LoadJsonContentAsync()
    {
        var files = new[]
        {
            ("sample-data/BGStatsExport.json", "BGStatsExport.json"),
            ("sample-data/SampleData.json", "SampleData.json")
        };

        foreach (var (filePath, dataSource) in files)
        {
            var result = await TryLoadFileAsync(filePath, dataSource);
            if (result.success)
                return (result.content, result.dataSource);
        }

        _logger.LogError("Failed to load both BGStatsExport.json and SampleData.json");
        throw new HttpRequestException("No valid data files could be loaded");
    }

    private async Task<(bool success, string content, string dataSource)> TryLoadFileAsync(string filePath, string dataSource)
    {
        try
        {
            var content = await _httpClient.GetStringAsync(filePath);
            _logger.LogInformation("Successfully loaded {DataSource} (Content length: {ContentLength})", dataSource, content.Length);
            
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("{DataSource} is empty", dataSource);
                return (false, string.Empty, dataSource);
            }

            return (true, content, dataSource);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to load {DataSource}", dataSource);
            return (false, string.Empty, dataSource);
        }
    }
}