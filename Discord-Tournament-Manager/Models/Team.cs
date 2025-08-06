namespace DiscordTournamentManager.Models;

public class Team
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string DiscordRoleId { get; init; }
    public bool Deleted { get; set; }
}