using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class UpdateTeamRequest
{
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("user_ids")]
    public List<int> UserIds { get; set; } = new();
}
