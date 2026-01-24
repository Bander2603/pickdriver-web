using System.Text.Json.Serialization;

namespace PickDriverWeb.Models.Leagues;

public sealed class LeaguePublic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("invite_code")]
    public string InviteCode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("initial_race_round")]
    public int? InitialRaceRound { get; set; }

    [JsonPropertyName("owner_id")]
    public int OwnerId { get; set; }

    [JsonPropertyName("max_players")]
    public int MaxPlayers { get; set; }

    [JsonPropertyName("teams_enabled")]
    public bool TeamsEnabled { get; set; }

    [JsonPropertyName("bans_enabled")]
    public bool BansEnabled { get; set; }

    [JsonPropertyName("mirror_picks_enabled")]
    public bool MirrorPicksEnabled { get; set; }
}
