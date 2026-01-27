using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class DraftPickRequest
{
    [JsonPropertyName("driverID")]
    public int DriverId { get; set; }
}

public sealed class DraftBanRequest
{
    [JsonPropertyName("targetUserID")]
    public int TargetUserId { get; set; }

    [JsonPropertyName("driverID")]
    public int DriverId { get; set; }
}
