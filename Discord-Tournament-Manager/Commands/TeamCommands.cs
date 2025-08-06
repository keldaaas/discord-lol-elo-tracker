using System.Diagnostics.CodeAnalysis;
using DiscordTournamentManager.SaveData;
using NetCord;
using NetCord.Services.ApplicationCommands;
using Team = DiscordTournamentManager.Models.Team;

namespace DiscordTournamentManager.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class TeamCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("list", "Listet alle registrierten Teams auf")]
    public static string ListTeams()
    {
        var teams = SaveDataLoader.SaveData!.Teams.Where(x => !x.Deleted).ToList();
        
        return teams.Count == 0 
            ? "Es wurden keine Teams hinzugefügt.." 
            : $"{string.Join('\n', teams.Select(x => $"{x.Name} ({x.Id})").ToList())}";
    }

    [SlashCommand("add", "Fügt ein neues Team hinzu", DefaultGuildUserPermissions = Permissions.ManageEvents)]
    public static async Task<string> AddTeam(string name, string role)
    {
        if (SaveDataLoader.SaveData!.Teams.Where(x => !x.Deleted).Select(x => x.Name).Contains(name))
            return "Team wurde schon hinzugefügt.";

        SaveDataLoader.SaveData.Teams.Add(new Team
        {
            Id = Guid.NewGuid(),
            Name = name,
            DiscordRoleId = role
        });

        await SaveDataLoader.Save();
        
        return $"Neues Team '{name}' hinzugefügt.";
    }
    
    [SlashCommand("remove", "Entfernt ein Team", DefaultGuildUserPermissions = Permissions.ManageEvents)]
    public static async Task<string> RemoveTeam(string name)
    {
        if (!SaveDataLoader.SaveData!.Teams.Where(x => !x.Deleted).Select(x => x.Name).Contains(name))
            return $"Es gibt kein registriertes Team mit dem Namen '{name}'.";
        
        foreach (var team in SaveDataLoader.SaveData.Teams.Where(team => team.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            team.Deleted = true;
        }
        
        await SaveDataLoader.Save();
        
        return $"'{name}' wurde entfernt.";
    }
}