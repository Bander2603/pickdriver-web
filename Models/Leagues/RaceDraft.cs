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

    [JsonPropertyName("bannedByUserIDsByPickIndex")]
    public List<int?> BannedByUserIdsByPickIndex { get; set; } = new();

    [JsonPropertyName("bansUsedByUserID")]
    public Dictionary<string, int> BansUsedByUserID { get; set; } = new();

    [JsonPropertyName("bansUsedByTeamID")]
    public Dictionary<string, int> BansUsedByTeamID { get; set; } = new();

    public int BanLimitPerActor { get; set; }

    public int CurrentPickIndex { get; set; }

    [JsonPropertyName("mirrorPicks")]
    public bool MirrorPicks { get; set; }

    public string Status { get; set; } = string.Empty;

    public string GameplayVersion { get; set; } = "legacy";

    public string? ResolutionState { get; set; }

    public int? ResolutionRevision { get; set; }

    public DateTimeOffset? SubmissionDeadline { get; set; }

    public DateTimeOffset? BanWindowClosesAt { get; set; }
}

public sealed class DraftDeadline
{
    [JsonPropertyName("raceID")]
    public int RaceId { get; set; }
    [JsonPropertyName("leagueID")]
    public int LeagueId { get; set; }
    public DateTimeOffset? FirstHalfDeadline { get; set; }
    public DateTimeOffset? SecondHalfDeadline { get; set; }
    public string? GameplayVersion { get; set; }
    public DateTimeOffset? SubmissionDeadline { get; set; }
    public DateTimeOffset? BanWindowClosesAt { get; set; }
}
