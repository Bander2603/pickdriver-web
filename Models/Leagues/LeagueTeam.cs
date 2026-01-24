namespace PickDriverWeb.Models.Leagues;

public sealed class LeagueTeam
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LeagueRef League { get; set; } = new();
    public List<TeamMember> Members { get; set; } = new();
}

public sealed class LeagueRef
{
    public int Id { get; set; }
}

public sealed class TeamMember
{
    public int Id { get; set; }
    public TeamMemberUser User { get; set; } = new();
    public TeamMemberTeam Team { get; set; } = new();
}

public sealed class TeamMemberUser
{
    public int Id { get; set; }
}

public sealed class TeamMemberTeam
{
    public int Id { get; set; }
}
