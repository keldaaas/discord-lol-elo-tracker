using DiscordLoLEloTracker.Models;

namespace DiscordLoLEloTracker.SaveData;

public record SaveData
{
    public string FileName = "SaveData.json";
    public List<Team> Teams { get; init; } = [];
    public List<Matchup> Matchups { get; init; } = [];
}