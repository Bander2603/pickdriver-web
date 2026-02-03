using PickDriverWeb.Models.Auth;
using PickDriverWeb.Models.Drivers;
using PickDriverWeb.Models.Leagues;
using PickDriverWeb.Models.Races;
using PickDriverWeb.Models.Standings;
using PickDriverWeb.Models.Teams;

namespace PickDriverWeb.Services;

internal static class MockApiData
{
    public static UserPublic DemoUser => new()
    {
        Id = 1,
        Username = "demo",
        Email = "demo@example.com",
        EmailVerified = true
    };

    public static UserPublic TeammateUser => new()
    {
        Id = 2,
        Username = "teammate",
        Email = "teammate@example.com",
        EmailVerified = true
    };

    public static UserPublic RivalUser => new()
    {
        Id = 3,
        Username = "rival",
        Email = "rival@example.com",
        EmailVerified = true
    };

    public static UserPublic RookieUser => new()
    {
        Id = 4,
        Username = "rookie",
        Email = "rookie@example.com",
        EmailVerified = true
    };

    public static LeaguePublic DemoLeague => new()
    {
        Id = 10,
        Name = "Demo League",
        InviteCode = "DEMO123",
        Status = "active",
        InitialRaceRound = 1,
        OwnerId = DemoUser.Id,
        MaxPlayers = 4,
        TeamsEnabled = true,
        BansEnabled = true,
        MirrorPicksEnabled = true
    };

    public static List<LeaguePublic> Leagues => new()
    {
        DemoLeague
    };

    public static List<UserPublic> LeagueMembers => new()
    {
        DemoUser,
        TeammateUser,
        RivalUser,
        RookieUser
    };

    public static List<F1Team> F1Teams => new()
    {
        new F1Team { Id = 1, Name = "Apex GP", Color = "#1e90ff" },
        new F1Team { Id = 2, Name = "Redline Racing", Color = "#ff3b30" }
    };

    public static List<Driver> Drivers => new()
    {
        new Driver
        {
            Id = 1,
            SeasonId = 2026,
            TeamId = 1,
            FirstName = "Alex",
            LastName = "Taylor",
            Country = "UK",
            DriverNumber = 7,
            Active = true,
            DriverCode = "TAY"
        },
        new Driver
        {
            Id = 2,
            SeasonId = 2026,
            TeamId = 2,
            FirstName = "Maya",
            LastName = "Rivera",
            Country = "ES",
            DriverNumber = 22,
            Active = true,
            DriverCode = "RIV"
        }
    };

    public static List<Race> Races
    {
        get
        {
            var now = DateTimeOffset.UtcNow;
            return new List<Race>
            {
                new()
                {
                    Id = 1,
                    SeasonId = 2026,
                    Round = 1,
                    Name = "Demo Grand Prix",
                    CircuitName = "Demo Circuit",
                    Country = "USA",
                    CountryCode = "US",
                    Sprint = false,
                    Completed = false,
                    Fp1Time = now.AddDays(5),
                    QualifyingTime = now.AddDays(6),
                    RaceTime = now.AddDays(7),
                    CircuitData = new CircuitData
                    {
                        Laps = 58,
                        FirstGp = 2020,
                        RaceDistance = 305.0,
                        CircuitLength = 5.2,
                        LapRecordTime = "1:32.000",
                        LapRecordDriver = "A. Taylor"
                    }
                },
                new()
                {
                    Id = 2,
                    SeasonId = 2026,
                    Round = 2,
                    Name = "City Grand Prix",
                    CircuitName = "Harbor Street Circuit",
                    Country = "ES",
                    CountryCode = "ES",
                    Sprint = true,
                    Completed = false,
                    Fp1Time = now.AddDays(12),
                    QualifyingTime = now.AddDays(13),
                    RaceTime = now.AddDays(14)
                }
            };
        }
    }

    public static Race CurrentRace => Races[0];

    public static List<Race> UpcomingRaces => Races;

    public static List<DriverStanding> DriverStandings => new()
    {
        new DriverStanding
        {
            DriverId = 1,
            FirstName = "Alex",
            LastName = "Taylor",
            DriverCode = "TAY",
            Points = 25,
            TeamId = 1,
            TeamName = "Apex GP",
            TeamColor = "#1e90ff"
        },
        new DriverStanding
        {
            DriverId = 2,
            FirstName = "Maya",
            LastName = "Rivera",
            DriverCode = "RIV",
            Points = 18,
            TeamId = 2,
            TeamName = "Redline Racing",
            TeamColor = "#ff3b30"
        }
    };

    public static List<TeamStanding> TeamStandings => new()
    {
        new TeamStanding { TeamId = 1, Name = "Apex GP", Color = "#1e90ff", Points = 25 },
        new TeamStanding { TeamId = 2, Name = "Redline Racing", Color = "#ff3b30", Points = 18 }
    };

    public static List<PlayerStanding> PlayerStandings => new()
    {
        new PlayerStanding { UserId = DemoUser.Id, Username = DemoUser.Username, TotalPoints = 42, TeamId = 1, TotalDeviation = 1.2 },
        new PlayerStanding { UserId = TeammateUser.Id, Username = TeammateUser.Username, TotalPoints = 38, TeamId = 1, TotalDeviation = 1.8 },
        new PlayerStanding { UserId = RivalUser.Id, Username = RivalUser.Username, TotalPoints = 35, TeamId = 2, TotalDeviation = 2.1 },
        new PlayerStanding { UserId = RookieUser.Id, Username = RookieUser.Username, TotalPoints = 30, TeamId = 2, TotalDeviation = 2.6 }
    };

    public static List<PlayerTeamStanding> PlayerTeamStandings => new()
    {
        new PlayerTeamStanding { TeamId = 1, Name = "Apex GP", TotalPoints = 80, TotalDeviation = 3.0 },
        new PlayerTeamStanding { TeamId = 2, Name = "Redline Racing", TotalPoints = 65, TotalDeviation = 4.7 }
    };

    public static List<LeagueTeam> LeagueTeams => new()
    {
        new LeagueTeam
        {
            Id = 1,
            Name = "Apex Squad",
            League = new LeagueRef { Id = DemoLeague.Id },
            Members = new List<TeamMember>
            {
                new()
                {
                    Id = 1,
                    User = new TeamMemberUser { Id = DemoUser.Id },
                    Team = new TeamMemberTeam { Id = 1 }
                },
                new()
                {
                    Id = 2,
                    User = new TeamMemberUser { Id = TeammateUser.Id },
                    Team = new TeamMemberTeam { Id = 1 }
                }
            }
        },
        new LeagueTeam
        {
            Id = 2,
            Name = "Redline Crew",
            League = new LeagueRef { Id = DemoLeague.Id },
            Members = new List<TeamMember>
            {
                new()
                {
                    Id = 3,
                    User = new TeamMemberUser { Id = RivalUser.Id },
                    Team = new TeamMemberTeam { Id = 2 }
                },
                new()
                {
                    Id = 4,
                    User = new TeamMemberUser { Id = RookieUser.Id },
                    Team = new TeamMemberTeam { Id = 2 }
                }
            }
        }
    };

    public static RaceDraft Draft => new()
    {
        Id = 100,
        League = new LeagueRef { Id = DemoLeague.Id },
        RaceId = CurrentRace.Id,
        PickOrder = new List<int>
        {
            DemoUser.Id,
            TeammateUser.Id,
            RivalUser.Id,
            RookieUser.Id,
            RookieUser.Id,
            RivalUser.Id,
            TeammateUser.Id,
            DemoUser.Id
        },
        PickedDriverIds = new List<int?> { null, null, null, null, null, null, null, null },
        BannedDriverIds = new List<int>(),
        BannedDriverIdsByPickIndex = new List<int?> { null, null, null, null, null, null, null, null },
        CurrentPickIndex = 0,
        MirrorPicks = true,
        Status = "active"
    };

    public static DraftDeadline DraftDeadlines => new()
    {
        RaceId = CurrentRace.Id,
        LeagueId = DemoLeague.Id,
        FirstHalfDeadline = DateTimeOffset.UtcNow.AddDays(4),
        SecondHalfDeadline = DateTimeOffset.UtcNow.AddDays(5)
    };

    public static AutopickSettings AutopickSettings => new()
    {
        DriverIds = new List<int> { 1, 2 }
    };

    public static DraftResponse DraftResponse => new()
    {
        Status = "active",
        CurrentPickIndex = 1,
        NextUserId = RivalUser.Id,
        BannedDriverIds = new List<int>(),
        PickedDriverIds = new List<int> { 1 },
        YourTurn = false,
        YourDeadline = DateTimeOffset.UtcNow.AddMinutes(30)
    };
}
