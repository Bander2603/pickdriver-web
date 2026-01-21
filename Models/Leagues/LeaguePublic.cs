namespace PickDriverWeb.Models.Leagues;

public sealed class LeaguePublic
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? InitialRaceRound { get; set; }
    public int OwnerId { get; set; }
    public int MaxPlayers { get; set; }
    public bool TeamsEnabled { get; set; }
    public bool BansEnabled { get; set; }
    public bool MirrorPicksEnabled { get; set; }
}
