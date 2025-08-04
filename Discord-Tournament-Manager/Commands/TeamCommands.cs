using System.Diagnostics.CodeAnalysis;
using DiscordLoLEloTracker.Models;
using DiscordLoLEloTracker.SaveData;
using NetCord.Services.ApplicationCommands;

namespace DiscordLoLEloTracker.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class TeamCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("add", "Adds a new team")]
    public static async Task<string> AddTeam(string name, string role)
    {
        if (SaveDataLoader.SaveData!.Teams.Select(x => x.Name).Contains(name))
            return "Team already exists.";

        SaveDataLoader.SaveData.Teams.Add(new Team
        {
            Id = Guid.NewGuid(),
            Name = name,
            DiscordRoleId = role
        });

        await SaveDataLoader.Save();
        
        return $"Created new team '{name}'.";
    }
    
    [SlashCommand("remove", "Remove a team")]
    public static async Task<string> RemoveTeam(string name)
    {
        if (!SaveDataLoader.SaveData!.Teams.Select(x => x.Name).Contains(name))
            return "There is not such team.";
        
        SaveDataLoader.SaveData.Teams.RemoveAll(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        await SaveDataLoader.Save();
        
        return $"Removed '{name}'.";
    }
}