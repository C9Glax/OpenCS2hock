using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenCS2hock;

public struct Settings
{
    public LogLevel LogLevel = LogLevel.Information;
    public OpenShockSettings OpenShockSettings = new()
    {
        Endpoint = "https://api.shocklink.net",
        ApiKey = "",
        Shockers = Array.Empty<string>()
    };

    public Range IntensityRange = new ()
    {
        Min = 20,
        Max = 60
    };

    public Range DurationRange = new()
    {
        Min = 1000,
        Max = 1000
    };

    public Dictionary<string, string> Actions = new()
    {
        {"OnKill", "Nothing"},
        {"OnDeath", "Shock"},
        {"OnRoundStart", "Vibrate"},
        {"OnRoundEnd", "Nothing"},
        {"OnRoundWin", "Beep"},
        {"OnRoundLoss", "Nothing"},
        {"OnDamageTaken", "Vibrate"}
    };

    public Settings()
    {
        
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    internal static Shocker.ControlAction StringToAction(string str)
    {
        return str.ToLower() switch
        {
            "shock" => Shocker.ControlAction.Shock,
            "vibrate" => Shocker.ControlAction.Vibrate,
            "beep" => Shocker.ControlAction.Beep,
            _ => Shocker.ControlAction.Nothing
        };
    }
}

public struct OpenShockSettings
{
    public string Endpoint, ApiKey;
    public string[] Shockers;
}

public struct Range
{
    public short Min, Max;
}