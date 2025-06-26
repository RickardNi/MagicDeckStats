using System.Text.Json;
using MagicDeckStats.Models;

namespace MagicDeckStats.Services;

public interface IBGStatsImportService
{
    Task<List<MagicPlay>> GetMagicPlaysAsync();
    Task<int?> GetMagicGameIdAsync();
}

public class BGStatsImportService : IBGStatsImportService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BGStatsImportService> _logger;
    private BGStatsExport? _cachedData;
    private int? _magicGameId;

    public BGStatsImportService(HttpClient httpClient, ILogger<BGStatsImportService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int?> GetMagicGameIdAsync()
    {
        if (_magicGameId.HasValue)
            return _magicGameId.Value;

        var data = await LoadBGStatsDataAsync();
        
        _logger.LogInformation("Loaded {GameCount} games from BGStats export", data.Games.Count);
        
        // Log the first few games to see what we're getting
        foreach (var game in data.Games.Take(5))
        {
            _logger.LogInformation("Game: ID={GameId}, Name='{GameName}', BggName='{BggName}'", 
                game.Id, game.Name, game.BggName ?? "null");
        }
        
        var magicGame = data.Games.FirstOrDefault(g => 
            g.Name.Equals("Magic: The Gathering", StringComparison.OrdinalIgnoreCase) ||
            (g.BggName != null && g.BggName.Equals("Magic: The Gathering", StringComparison.OrdinalIgnoreCase)));

        if (magicGame != null)
        {
            _magicGameId = magicGame.Id;
            _logger.LogInformation("Found Magic: The Gathering game with ID: {GameId}", _magicGameId.Value);
        }
        else
        {
            _logger.LogWarning("Magic: The Gathering game not found in BGStats export");
            _logger.LogWarning("Available games: {GameNames}", 
                string.Join(", ", data.Games.Select(g => $"'{g.Name}'")));
        }

        return _magicGameId;
    }

    public async Task<List<MagicPlay>> GetMagicPlaysAsync()
    {
        var magicGameId = await GetMagicGameIdAsync();
        if (!magicGameId.HasValue)
        {
            _logger.LogWarning("Cannot get Magic plays without finding the game ID");
            return new List<MagicPlay>();
        }

        var data = await LoadBGStatsDataAsync();
        _logger.LogInformation("Loaded {PlayCount} plays from BGStats export", data.Plays.Count);
        
        var magicPlays = data.Plays.Where(p => p.GameRefId == magicGameId.Value).ToList();
        _logger.LogInformation("Found {MagicPlayCount} plays for Magic: The Gathering (GameRefId: {GameRefId})", 
            magicPlays.Count, magicGameId.Value);
        
        var result = new List<MagicPlay>();

        foreach (var play in magicPlays)
        {
            try
            {
                var magicPlay = new MagicPlay
                {
                    PlayDate = DateTime.Parse(play.PlayDate),
                    Duration = play.DurationInMinutes,
                    Rounds = play.Rounds,
                    Board = play.Variant,
                    PlayerScores = play.PlayerScores.Select(ps => new MagicPlayerScore
                    {
                        Score = ps.Score,
                        IsWinner = ps.IsWinner,
                        NewPlayer = ps.NewPlayer,
                        StartPlayer = ps.StartPlayer,
                        PlayerRefId = ps.PlayerRefId,
                        Deck = ps.Deck
                    }).ToList()
                };

                result.Add(magicPlay);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing play with date: {PlayDate}", play.PlayDate);
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
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            _cachedData = JsonSerializer.Deserialize<BGStatsExport>(jsonContent, options);
            
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