namespace PickDriverWeb.Models.Standings;

public sealed class DriverStanding
{
    public int DriverId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DriverCode { get; set; } = string.Empty;
    public int Points { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string TeamColor { get; set; } = string.Empty;
}
