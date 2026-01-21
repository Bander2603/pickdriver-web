namespace PickDriverWeb.Models.Standings;

public sealed class TeamStanding
{
    public int TeamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Points { get; set; }
}
