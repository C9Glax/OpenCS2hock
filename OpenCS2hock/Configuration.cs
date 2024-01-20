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
        Configuration c;
        if (!File.Exists(settingsFilePath))
            c = Setup.Run().SaveConfiguration();
        else
            c = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(settingsFilePath), new CShocker.Shockers.ShockerJsonConverter());
        if (!c.ConfigurationValid())
            throw new Exception("Configuration validation failed.");
        foreach (Shocker cShocker in c.Shockers)
            cShocker.SetLogger(logger);
        return c;
    }

    internal Configuration SaveConfiguration(string? path = null)
    {
        string settingsFilePath = path ?? "config.json";
        File.WriteAllText(settingsFilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        return this;
    }

    private bool ConfigurationValid()
    {
        return true; //TODO Check values
    }
}