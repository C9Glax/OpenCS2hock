using CShocker.Shockers.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenCS2hock;

public struct Configuration
{
    public LogLevel LogLevel = LogLevel.Information;

    public List<Shocker> Shockers = new();

    public List<ShockerAction> ShockerActions = new ();

    public Configuration()
    {
        
    }

    public override string ToString()
    {
        return $"Loglevel: {Enum.GetName(typeof(LogLevel), LogLevel)}\n" +
               $"Shockers: {string.Join("\n---", Shockers)}\n" +
               $"Actions: {string.Join("\n---", ShockerActions)}";
    }
    
    internal static Configuration GetConfigurationFromFile(string? path = null, ILogger? logger = null)
    {
        string settingsFilePath = path ?? "config.json";
        if (!File.Exists(settingsFilePath))
            Setup.Run().SaveConfiguration();
          
        Configuration c = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(settingsFilePath), new CShocker.Shockers.ShockerJsonConverter());
        foreach (Shocker cShocker in c.Shockers)
            cShocker.SetLogger(logger);
        return c;
    }

    internal void SaveConfiguration(string? path = null)
    {
        string settingsFilePath = path ?? "config.json";
        File.WriteAllText(settingsFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}