using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class AutopickSettings
{
    [JsonPropertyName("driverIDs")]
    public List<int> DriverIds { get; set; } = new();
}
