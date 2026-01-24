using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Standings;

public sealed class DriverStanding
{
    [JsonPropertyName("driver_id")]
    public int DriverId { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("driver_code")]
    public string DriverCode { get; set; } = string.Empty;

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("team_id")]
    public int TeamId { get; set; }

    [JsonPropertyName("team_name")]
    public string TeamName { get; set; } = string.Empty;

    [JsonPropertyName("team_color")]
    public string TeamColor { get; set; } = string.Empty;
}
