using DiscordLoLEloTracker.Models.Enums;

namespace DiscordLoLEloTracker.Models;

public class Matchup
{
    public int Id { get; set; }
    public Guid TeamOneId { get; set; }
    public Guid TeamTwoId { get; set; }
    public DateOnly? Date { get; set; }
    public GameResult GameResult { get; set; }
}