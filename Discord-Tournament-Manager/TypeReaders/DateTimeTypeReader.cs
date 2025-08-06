using System.Globalization;
using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace DiscordTournamentManager.TypeReaders;

public class DateTimeTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context,
        SlashCommandParameter<TContext> parameter,
        ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
        => new(DateTime.TryParseExact(value, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var result)
            ? TypeReaderResult.Success(result)
            : TypeReaderResult.Fail("Das Datum muss dem Format 'dd.MM.yyyy HH:mm' entsprechen."));

    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.String;
}