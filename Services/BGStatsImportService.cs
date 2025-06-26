using MagicDeckStats.Models;
using System.Text.Json;

namespace MagicDeckStats.Services;

public interface IBGStatsImportService
{
    Task<List<Play>> GetMagicPlaysAsync();
    Task<int?> GetMagicGameIdAsync();
}

public class BGStatsImportService(HttpClient httpClient, ILogger<BGStatsImportService> logger) : IBGStatsImportService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<BGStatsImportService> _logger = logger;
    private BGStatsExport? _cachedData;
    private int? _magicGameId;

    public async Task<int?> GetMagicGameIdAsync()
    {
        if (_magicGameId.HasValue)
            return _magicGameId.Value;

        var data = await LoadBGStatsDataAsync();

        _logger.LogInformation("Loaded {GameCount} games from BGStats export", data.Games.Count);

        var magicGame = data.Games.FirstOrDefault(g => g.Name.Equals("Magic: The Gathering", StringComparison.OrdinalIgnoreCase));

        if (magicGame != null)
        {
            _magicGameId = magicGame.Id;
            _logger.LogInformation("Found Magic: The Gathering game with ID: {GameId}", _magicGameId.Value);
        }
        else
        {
            _logger.LogWarning("Magic: The Gathering game not found in BGStats export");
        }

        return _magicGameId;
    }

    public async Task<List<Play>> GetMagicPlaysAsync()
    {
        var magicGameId = await GetMagicGameIdAsync();

        if (!magicGameId.HasValue)
        {
            _logger.LogWarning("Cannot get Magic plays without finding the game ID");
            return [];
        }

        var data = await LoadBGStatsDataAsync();
        _logger.LogInformation("Loaded {PlayCount} plays from BGStats export", data.Plays.Count);

        var plays = data.Plays.Where(p => p.GameRefId == magicGameId.Value).ToList();
        _logger.LogInformation("Found {MagicPlayCount} plays for Magic: The Gathering (GameRefId: {GameRefId})",
            plays.Count, magicGameId.Value);

        // Temporary filter: only include Battle Decks variants
        plays = [.. plays.Where(p => p.Variant.Contains("Battle Decks") && !p.Variant.Contains("Two-Headed Giant"))];

        // Create a lookup dictionary for player names
        var playerNamesById = data.Players.ToDictionary(p => p.Id, p => p.Name);

        foreach (var play in plays)
        {
            try
            {
                // Set PlayerName for each PlayerScore
                foreach (var playerScore in play.PlayerScores)
                {
                    if (playerNamesById.TryGetValue(playerScore.PlayerRefId, out var name))
                        playerScore.PlayerName = name;
                    else
                    {
                        _logger.LogError("Player with {PlayerRedId} not found", playerScore.PlayerRefId);
                        playerScore.PlayerName = "Unknown Player";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing play with date: {PlayDate}", play.Date);
            }
        }

        _logger.LogInformation("Successfully parsed {ParsedPlayCount} Magic: The Gathering plays", plays.Count);
        return plays;
    }

    private async Task<BGStatsExport> LoadBGStatsDataAsync()
    {
        if (_cachedData != null)
            return _cachedData;

        try
        {
            _logger.LogInformation("Loading BGStats export data...");
            var jsonContent = await _httpClient.GetStringAsync("sample-data/BGStatsExport.json");
            _logger.LogInformation("Loaded JSON content, length: {ContentLength} characters", jsonContent.Length);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            _cachedData = JsonSerializer.Deserialize<BGStatsExport>(jsonContent, jsonOptions);

            if (_cachedData == null)
            {
                _logger.LogError("Failed to deserialize BGStats export data");
                return new BGStatsExport();
            }

            _logger.LogInformation("Successfully loaded BGStats export with {GameCount} games and {PlayCount} plays",
                _cachedData.Games.Count, _cachedData.Plays.Count);

            return _cachedData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading BGStats export data");
            return new BGStatsExport();
        }
    }
}