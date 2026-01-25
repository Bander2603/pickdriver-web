using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Standings;

public sealed class PlayerStanding
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("total_points")]
    public double TotalPoints { get; set; }
    [JsonPropertyName("team_id")]
    public int? TeamId { get; set; }
    [JsonPropertyName("total_deviation")]
    public double TotalDeviation { get; set; }
}
