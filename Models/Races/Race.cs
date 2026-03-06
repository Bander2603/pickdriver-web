using System.Text.Json.Serialization;

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
    public RaceMedia? Media { get; set; }
}

public sealed class RaceMedia
{
    [JsonPropertyName("countryFlagURL")]
    public string? CountryFlagURL { get; set; }

    [JsonPropertyName("circuitURL")]
    public string? CircuitURL { get; set; }

    [JsonPropertyName("circuitSimpleURL")]
    public string? CircuitSimpleURL { get; set; }
}

public sealed class CircuitData
{
    [JsonPropertyName("laps")]
    public int? Laps { get; set; }
    [JsonPropertyName("first_gp")]
    public int? FirstGp { get; set; }
    [JsonPropertyName("race_distance")]
    public double? RaceDistance { get; set; }
    [JsonPropertyName("circuit_length")]
    public double? CircuitLength { get; set; }
    [JsonPropertyName("lap_record_time")]
    public string? LapRecordTime { get; set; }
    [JsonPropertyName("lap_record_driver")]
    public string? LapRecordDriver { get; set; }
}
