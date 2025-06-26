using System.Text.Json.Serialization;

namespace MagicDeckStats.Models;

public class BGStatsExport
{
    [JsonPropertyName("games")]
    public List<Game> Games { get; set; } = new();

    [JsonPropertyName("plays")]
    public List<Play> Plays { get; set; } = new();

    [JsonPropertyName("players")]
    public List<Player> Players { get; set; } = new();
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
    public List<PlayerScore> PlayerScores { get; set; } = new();
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
}

// Simplified models for the parsed data we want to store
public class MagicPlay
{
    public DateTime PlayDate { get; set; }
    public int Duration { get; set; }
    public int Rounds { get; set; }
    public string Deck { get; set; } = string.Empty;
    public List<MagicPlayerScore> PlayerScores { get; set; } = new();
}

public class MagicPlayerScore
{
    public object? Score { get; set; }
    public bool IsWinner { get; set; }
    public bool IsNewPlayer { get; set; }
    public bool IsStartPlayer { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string Deck { get; set; } = string.Empty;
} 