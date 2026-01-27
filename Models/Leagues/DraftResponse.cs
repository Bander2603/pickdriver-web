using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class DraftResponse
{
    public string Status { get; set; } = string.Empty;

    public int CurrentPickIndex { get; set; }

    [JsonPropertyName("nextUserID")]
    public int? NextUserId { get; set; }

    [JsonPropertyName("bannedDriverIDs")]
    public List<int> BannedDriverIds { get; set; } = new();

    [JsonPropertyName("pickedDriverIDs")]
    public List<int> PickedDriverIds { get; set; } = new();

    public bool YourTurn { get; set; }

    public DateTimeOffset? YourDeadline { get; set; }
}
