using System.Security.Cryptography;
using System.Text.Json;
using DiscordLoLEloTracker.Models;

namespace DiscordLoLEloTracker.SaveData;

public static class SaveDataLoader
{
    public static SaveData? SaveData;
    private const string SaveDataFileName = "SaveData.json";
    
    public static async Task Save()
    {
        var filePath = Path.Combine(
            new DirectoryInfo(Directory.GetCurrentDirectory())
                .Parent?
                .Parent?
                .FullName ?? throw new InvalidOperationException("Cannot go 2 levels up."),
            SaveDataFileName);

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, SaveData);
    }
    
    public static async Task Load()
    {
        var filePath = Path.Combine(
            new DirectoryInfo(Directory.GetCurrentDirectory())
                .Parent?
                .Parent?
                .FullName ?? throw new InvalidOperationException("Cannot go 2 levels up."),
            SaveDataFileName);

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found. Creating new one...");

            SaveData = new SaveData();
            
            return;
        }
        
        await using var stream = File.OpenRead(filePath);
        var saveData = await JsonSerializer.DeserializeAsync<SaveData>(stream);
        if (saveData != null)
        {
            SaveData = saveData;
            return;
        }
        
        Console.WriteLine("Couldn't read save data :c Creating new one with different name...");
            
        SaveData = new SaveData
        {
            FileName = "SaveData" + RandomNumberGenerator.GetInt32(100000, 1000000) + ".json"
        };
    }
}