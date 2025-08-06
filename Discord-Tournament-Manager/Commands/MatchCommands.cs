using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DiscordTournamentManager.Models;
using DiscordTournamentManager.Models.Enums;
using DiscordTournamentManager.SaveData;
using NetCord;
using NetCord.Services.ApplicationCommands;
using Team = DiscordTournamentManager.Models.Team;

namespace DiscordTournamentManager.Commands;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public partial class MatchCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("generate", "Generiert die jeweiligen Matchups", DefaultGuildUserPermissions = Permissions.ManageEvents)]
    public static async Task<string> GenerateMatchups()
    {
        if (SaveDataLoader.SaveData!.Matchups.Count != 0)
            return "Es existieren bereits Matchups.";
        
        if (SaveDataLoader.SaveData.Teams.Where(x => !x.Deleted).ToList().Count < 2)
            return $"Es existieren nicht genügend Teams ({SaveDataLoader.SaveData.Teams.Count}).";

        var teams = SaveDataLoader.SaveData.Teams.ToList();
        
        for (var i = 0; i < teams.Count - 1; i++)
        {
            for (var j = i + 1; j < teams.Count; j++)
            {
                SaveDataLoader.SaveData.Matchups.Add(new Matchup
                {
                    Id = i + j,
                    TeamOneId = teams[i].Id,
                    TeamTwoId = teams[j].Id,
                    GameResult = GameResult.Pending
                });
            }
        }
        
        await SaveDataLoader.Save();
        
        return $"'{SaveDataLoader.SaveData.Matchups.Count}' Matchups wurden erstellt.";
    }
    
    [SlashCommand("matchups", "Listet alle Matchups auf")]
    public static string ListMatchups()
    {
        var matchups = SaveDataLoader.SaveData!.Matchups;
        var teamDict = SaveDataLoader.SaveData.Teams.Where(x => !x.Deleted).ToDictionary(x => x.Id, x => x.Name);
        
        return matchups.Count == 0 
            ? "Es wurden keine Matchups erstellt." 
            : $"{string.Join('\n', matchups.Select(x => $"{teamDict[x.TeamOneId]} vs. {teamDict[x.TeamTwoId]} ({x.Id})").ToList())}";
    }
    
    [SlashCommand("suggest", "Macht einen Terminvorschlag zu einem bestimmten Spieltag")]
    public async Task<string> SuggestDate(int matchupId, DateTime date)
    {
        var matchup = SaveDataLoader.SaveData!.Matchups.SingleOrDefault(x => x.Id == matchupId);
        var teamDict = SaveDataLoader.SaveData.Teams.Where(x => !x.Deleted).ToDictionary(x => x.Id, x => x);

        if (matchup == null)
            return $"Es existiert kein Matchup mit der Id '{matchupId}'.";

        if (matchup.Confirmed)
            return $"Der Spieltermin '{matchup.Date}' wurde bereits bestätigt und kann nicht geändert werden.";

        var guildUser = (await Context.Guild?.FindUserAsync(Context.User.Username, 1)!)[0];
        var isTeamOneMember = guildUser.RoleIds.Contains(ExtractRoleIdAsULong(teamDict, matchup.TeamOneId));
        var isTeamTwoMember = guildUser.RoleIds.Contains(ExtractRoleIdAsULong(teamDict, matchup.TeamTwoId));
        if (isTeamOneMember && isTeamTwoMember)
            return "Du bist Mitglied in beiden Teams. " +
                   "Daher kann nicht evaluiert werden, für welches Team du den Vorschlag machen möchtest.";

        if (isTeamTwoMember == false && isTeamOneMember == false)
            return $"Um einen Spieltermin vorzuschlagen, musst du in einem der beiden Teams " +
                   $"('{teamDict[matchup.TeamOneId].Name}' oder '{teamDict[matchup.TeamTwoId].Name}') Mitglied sein.";
        
        if (date < DateTime.Now)
            return "Der Terminvorschlag darf nicht in der Vergangenheit liegen.";
        
        matchup.Date = date;
        matchup.RequestingTeamId = isTeamOneMember ? teamDict[matchup.TeamOneId].Id : teamDict[matchup.TeamTwoId].Id;

        return $"Der Termin {date} wurde vorgeschlagen.";
    }

    [GeneratedRegex(@"\D")]
    private static partial Regex MyRegex();

    private static ulong ExtractRoleIdAsULong(Dictionary<Guid, Team> teamDict, Guid teamId)
    {
        var teamRoleAsString = teamDict[teamId].DiscordRoleId;
        
        return ulong.Parse(MyRegex().Replace(teamRoleAsString, ""));
    }
}