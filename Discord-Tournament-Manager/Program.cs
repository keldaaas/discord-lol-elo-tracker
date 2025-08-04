using DiscordLoLEloTracker.SaveData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = Host.CreateApplicationBuilder();

builder.Configuration.AddUserSecrets<Program>();

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);
host.UseGatewayHandlers();

await SaveDataLoader.Load();

await host.RunAsync();
