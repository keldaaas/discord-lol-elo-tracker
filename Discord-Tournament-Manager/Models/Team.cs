namespace DiscordLoLEloTracker.Models;

public class Team
{
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required string DiscordRoleId { get; set; }
}