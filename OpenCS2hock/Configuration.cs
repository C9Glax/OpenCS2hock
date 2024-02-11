using CShocker.Devices.Abstract;
using CShocker.Devices.Additional;
using CShocker.Shockers.Abstract;
using CShocker.Shockers.Additional;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenCS2hock;

public struct Configuration
{
    public LogLevel LogLevel = LogLevel.Information;

    public List<ShockerAction> ShockerActions { get; init; }

    public List<Api> Apis { get; init; }

    public Dictionary<int, Shocker> Shockers { get; init; }

    public Configuration()
    {
        ShockerActions = new ();
        Apis = new();
        Shockers = new();
    }

    public override string ToString()
    {
        return $"Loglevel: {Enum.GetName(typeof(LogLevel), LogLevel)}\n" +
               $"Apis:\n{string.Join("\n---\n", Apis)}\n" +
               $"Shockers:\n{string.Join("\n---\n", Shockers)}\n" +
               $"Actions:\n{string.Join("\n---\n", ShockerActions)}\n";
    }
    
    internal static Configuration GetConfigurationFromFile(string? path = null, ILogger? logger = null)
    {
        string settingsFilePath = path ?? "config.json";
        Configuration c;
        if (!File.Exists(settingsFilePath))
            c = Setup.Run().SaveConfiguration();
        else
            c = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(settingsFilePath), new ApiJsonConverter(), new ShockerJsonConverter());
        if (!c.ConfigurationValid())
            throw new Exception("Configuration validation failed.");
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