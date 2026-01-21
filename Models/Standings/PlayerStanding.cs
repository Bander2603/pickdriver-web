namespace PickDriverWeb.Models.Standings;

public sealed class PlayerStanding
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public double TotalPoints { get; set; }
    public int? TeamId { get; set; }
    public double TotalDeviation { get; set; }
}
