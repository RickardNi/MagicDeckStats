using MagicDeckStats.Models;
using System.Text.Json;

namespace MagicDeckStats.Services;

public interface IBGStatsImportService
{
    Task<List<Play>> GetMagicPlaysAsync();
}

public class BGStatsImportService(HttpClient httpClient, ILogger<BGStatsImportService> logger) : IBGStatsImportService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<BGStatsImportService> _logger = logger;
    private BGStatsExport? _cachedData;

    public async Task<List<Play>> GetMagicPlaysAsync()
    {
        var data = await LoadBGStatsDataAsync();
        _logger.LogInformation("Loaded {PlayCount} plays from BGStats export", data.Plays.Count);

        var magicGame = data.Games.FirstOrDefault(g => g.Name.Equals("Magic: The Gathering", StringComparison.OrdinalIgnoreCase));

        if (magicGame == null)
        {
            _logger.LogWarning("Magic: The Gathering game not found in BGStats export");
            return [];
        }

        var magicGameId = magicGame.Id;
        _logger.LogInformation("Found Magic: The Gathering game with ID: {GameId}", magicGameId);

        var plays = data.Plays.Where(p => p.GameRefId == magicGameId).ToList();
        _logger.LogInformation("Found {MagicPlayCount} plays for Magic: The Gathering (GameRefId: {GameRefId})",
            plays.Count, magicGameId);

        // Temporary filter: only include Battle Decks variants
        plays = [.. plays.Where(p => p.Variant.Contains("Battle Decks") && !p.Variant.Contains("Two-Headed Giant"))];

        // Create a lookup dictionary for player names
        var playerNamesById = data.Players.ToDictionary(p => p.Id, p => p.Name);

        foreach (var play in plays)
        {
            try
            {
                // Set PlayerName for each PlayerScore and clean deck names
                foreach (var playerScore in play.PlayerScores)
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

        _logger.LogInformation("Successfully parsed {ParsedPlayCount} Magic: The Gathering plays", plays.Count);
        return plays;
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
        if (_cachedData != null)
            return _cachedData;

        try
        {
            _logger.LogInformation("Loading BGStats export data...");
            var jsonContent = await _httpClient.GetStringAsync("sample-data/BGStatsExport.json");

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