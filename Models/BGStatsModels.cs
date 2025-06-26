using System.Text.Json.Serialization;

namespace MagicDeckStats.Models;

public class BGStatsExport
{
    [JsonPropertyName("games")]
    public List<Game> Games { get; set; } = [];

    [JsonPropertyName("plays")]
    public List<Play> Plays { get; set; } = [];

    [JsonPropertyName("players")]
    public List<Player> Players { get; set; } = [];
}

public class Game
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class Play
{
    [JsonPropertyName("playDate")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("durationMin")]
    public int DurationInMinutes { get; set; }

    [JsonPropertyName("rounds")]
    public int Rounds { get; set; }

    [JsonPropertyName("board")]
    public string Variant { get; set; } = string.Empty;

    [JsonPropertyName("gameRefId")]
    public int GameRefId { get; set; }

    [JsonPropertyName("playerScores")]
    public List<PlayerScore> PlayerScores { get; set; } = [];
}

public class Player
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class PlayerScore
{
    [JsonPropertyName("score")]
    public object? Score { get; set; }

    [JsonPropertyName("winner")]
    public bool IsWinner { get; set; }

    [JsonPropertyName("newPlayer")]
    public bool IsNewPlayer { get; set; }

    [JsonPropertyName("startPlayer")]
    public bool IsStartPlayer { get; set; }

    [JsonPropertyName("playerRefId")]
    public int PlayerRefId { get; set; }

    [JsonPropertyName("role")]
    public string Deck { get; set; } = string.Empty;

    // Add PlayerName for display purposes (not from JSON)
    [JsonIgnore]
    public string PlayerName { get; set; } = string.Empty;
}