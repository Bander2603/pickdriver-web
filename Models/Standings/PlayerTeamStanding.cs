using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Standings;

public sealed class PlayerTeamStanding
{
    [JsonPropertyName("team_id")]
    public int TeamId { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("total_points")]
    public double TotalPoints { get; set; }

    [JsonPropertyName("total_deviation")]
    public double TotalDeviation { get; set; }
}
