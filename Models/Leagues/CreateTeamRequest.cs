using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class CreateTeamRequest
{
    [JsonPropertyName("league_id")]
    public int LeagueId { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("user_ids")]
    public List<int> UserIds { get; set; } = new();
}
