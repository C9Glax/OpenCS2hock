namespace OpenCS2hock;

public struct Settings
{
    public OpenShockSettings OpenShockSettings = new()
    {
        Endpoint = "https://api.shocklink.net",
        ApiKey = "",
        Shockers = Array.Empty<string>()
    };

    public short FixedIntensity = 30;
    public short FixedDuration = 1000;

    public Range IntensityRange = new ()
    {
        Min = 0,
        Max = 100
    };

    public Range DurationRange = new()
    {
        Min = 1000,
        Max = 2000
    };

    public Actions Actions = new()
    {
        OnKill = "Nothing",
        OnDeath = "Shock",
        OnRoundStart = "Vibrate",
        OnRoundEnd = "Nothing",
        OnRoundWin = "Beep",
        OnRoundLoss = "Nothing"
    };

    public Settings()
    {
        
    }

    internal Shocker.ControlAction StringToAction(string str)
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

public struct Actions
{
    public string OnKill, OnDeath, OnRoundStart, OnRoundEnd, OnRoundWin, OnRoundLoss;
}