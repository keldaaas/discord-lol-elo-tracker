using DiscordTournamentManager.Models.Enums;

namespace DiscordTournamentManager.Models;

public class Matchup
{
    public int Id { get; set; }
    public Guid TeamOneId { get; init; }
    public Guid TeamTwoId { get; init; }
    public DateTime? Date { get; set; }
    public Guid? RequestingTeamId { get; set; }
    public bool Confirmed { get; set; }
    public GameResult GameResult { get; set; }
}