namespace PickDriverWeb.Models.Leagues;

public sealed class CreateLeagueRequest
{
    public string Name { get; set; } = string.Empty;
    public int MaxPlayers { get; set; }
    public bool TeamsEnabled { get; set; }
    public bool BansEnabled { get; set; }
    public bool MirrorEnabled { get; set; }
}
