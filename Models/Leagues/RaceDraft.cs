using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class RaceDraft
{
    public int Id { get; set; }
    public LeagueRef League { get; set; } = new();

    [JsonPropertyName("raceID")]
    public int RaceId { get; set; }

    public List<int> PickOrder { get; set; } = new();

    [JsonPropertyName("pickedDriverIDs")]
    public List<int?> PickedDriverIds { get; set; } = new();

    [JsonPropertyName("bannedDriverIDs")]
    public List<int> BannedDriverIds { get; set; } = new();

    [JsonPropertyName("bannedDriverIDsByPickIndex")]
    public List<int?> BannedDriverIdsByPickIndex { get; set; } = new();

    public int CurrentPickIndex { get; set; }

    [JsonPropertyName("mirrorPicks")]
    public bool MirrorPicks { get; set; }

    public string Status { get; set; } = string.Empty;
}

public sealed class DraftDeadline
{
    [JsonPropertyName("raceID")]
    public int RaceId { get; set; }
    [JsonPropertyName("leagueID")]
    public int LeagueId { get; set; }
    public DateTimeOffset? FirstHalfDeadline { get; set; }
    public DateTimeOffset? SecondHalfDeadline { get; set; }
}
