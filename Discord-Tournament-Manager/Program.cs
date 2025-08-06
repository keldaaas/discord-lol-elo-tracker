using DiscordTournamentManager.SaveData;
using DiscordTournamentManager.TypeReaders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;

var builder = Host.CreateApplicationBuilder();

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands(x => x.TypeReaders.Add(typeof(DateTime), new DateTimeTypeReader<ApplicationCommandContext>()));

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);
host.UseGatewayHandlers();

await SaveDataLoader.Load();

await host.RunAsync();
