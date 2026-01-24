namespace PickDriverWeb.Models.Drivers;

public sealed class Driver
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int TeamId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int DriverNumber { get; set; }
    public bool Active { get; set; }
    public string DriverCode { get; set; } = string.Empty;
}
