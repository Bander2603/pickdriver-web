namespace PickDriverWeb.Options;

public sealed class ApiOptions
{
    public string BaseUrl { get; set; } = "https://api.example.com/api/";
    public bool UseMock { get; set; }
}
