using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class JoinLeagueRequest
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
