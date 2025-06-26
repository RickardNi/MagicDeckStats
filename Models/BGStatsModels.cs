using System.Text.Json.Serialization;

namespace MagicDeckStats.Models;

public class BGStatsExport
{
    [JsonPropertyName("games")]
    public List<Game> Games { get; set; } = new();

    [JsonPropertyName("plays")]
    public List<Play> Plays { get; set; } = new();
}

public class Game
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("bggName")]
    public string? BggName { get; set; }
}

public class Play
{
    [JsonPropertyName("playDate")]
    public string PlayDate { get; set; } = string.Empty;

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

public class PlayerScore
{
    [JsonPropertyName("score")]
    public object? Score { get; set; }

    [JsonPropertyName("winner")]
    public bool IsWinner { get; set; }

    [JsonPropertyName("newPlayer")]
    public bool NewPlayer { get; set; }

    [JsonPropertyName("startPlayer")]
    public bool StartPlayer { get; set; }

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
    public string Board { get; set; } = string.Empty;
    public List<MagicPlayerScore> PlayerScores { get; set; } = new();
}

public class MagicPlayerScore
{
    public object? Score { get; set; }
    public bool IsWinner { get; set; }
    public bool NewPlayer { get; set; }
    public bool StartPlayer { get; set; }
    public int PlayerRefId { get; set; }
    public string Deck { get; set; } = string.Empty;
} 