using System.Text.Json;
using MagicDeckStats.Models;

namespace MagicDeckStats.Services;

public interface IBGStatsImportService
{
    Task<List<MagicPlay>> GetMagicPlaysAsync();
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

    public async Task<List<MagicPlay>> GetMagicPlaysAsync()
    {
        var magicGameId = await GetMagicGameIdAsync();

        if (!magicGameId.HasValue)
        {
            _logger.LogWarning("Cannot get Magic plays without finding the game ID");
            return [];
        }

        var data = await LoadBGStatsDataAsync();
        _logger.LogInformation("Loaded {PlayCount} plays from BGStats export", data.Plays.Count);
        
        var magicPlays = data.Plays.Where(p => p.GameRefId == magicGameId.Value).ToList();
        _logger.LogInformation("Found {MagicPlayCount} plays for Magic: The Gathering (GameRefId: {GameRefId})", 
            magicPlays.Count, magicGameId.Value);
        
        // Create a lookup dictionary for player names
        var playerLookup = data.Players.ToDictionary(p => p.Id, p => p.Name);
        
        var result = new List<MagicPlay>();

        foreach (var play in magicPlays)
        {
            try
            {
                var magicPlay = new MagicPlay
                {
                    PlayDate = DateTime.Parse(play.Date),
                    Duration = play.DurationInMinutes,
                    Rounds = play.Rounds,
                    Deck = play.Variant,
                    PlayerScores = play.PlayerScores.Select(ps => new MagicPlayerScore
                    {
                        Score = ps.Score,
                        IsWinner = ps.IsWinner,
                        IsNewPlayer = ps.IsNewPlayer,
                        IsStartPlayer = ps.IsStartPlayer,
                        PlayerName = "Unknown Player", // Since we removed PlayerRefId, we can't look up the name
                        Deck = ps.Deck
                    }).ToList()
                };

                result.Add(magicPlay);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing play with date: {PlayDate}", play.Date);
            }
        }

        _logger.LogInformation("Successfully parsed {ParsedPlayCount} Magic: The Gathering plays", result.Count);
        return result;
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