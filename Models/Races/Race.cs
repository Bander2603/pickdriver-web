namespace PickDriverWeb.Models.Races;

public sealed class Race
{
    public int Id { get; set; }
    public int SeasonId { get; set; }
    public int Round { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CircuitName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool Sprint { get; set; }
    public bool Completed { get; set; }
    public DateTimeOffset? Fp1Time { get; set; }
    public DateTimeOffset? Fp2Time { get; set; }
    public DateTimeOffset? Fp3Time { get; set; }
    public DateTimeOffset? QualifyingTime { get; set; }
    public DateTimeOffset? SprintTime { get; set; }
    public DateTimeOffset? SprintQualifyingTime { get; set; }
    public DateTimeOffset? RaceTime { get; set; }
    public CircuitData? CircuitData { get; set; }
}

public sealed class CircuitData
{
    public int? Laps { get; set; }
    public int? FirstGp { get; set; }
    public double? RaceDistance { get; set; }
    public double? CircuitLength { get; set; }
    public string? LapRecordTime { get; set; }
    public string? LapRecordDriver { get; set; }
}
