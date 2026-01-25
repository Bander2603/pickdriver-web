using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Standings;

public sealed class PickHistory
{
    [JsonPropertyName("race_name")]
    public string RaceName { get; set; } = string.Empty;

    public int Round { get; set; }

    [JsonPropertyName("pick_position")]
    public int PickPosition { get; set; }

    [JsonPropertyName("driver_name")]
    public string DriverName { get; set; } = string.Empty;

    public double Points { get; set; }

    [JsonPropertyName("expected_points")]
    public double? ExpectedPoints { get; set; }

    public double? Deviation { get; set; }
}
