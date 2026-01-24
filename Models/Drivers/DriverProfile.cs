namespace PickDriverWeb.Models.Drivers;

public sealed class DriverProfile
{
    public int Id { get; set; }
    public string DriverCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
