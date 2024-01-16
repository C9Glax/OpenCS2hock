using Microsoft.Extensions.Logging;

namespace OpenCS2hock;

public class OpenCS2hock
{
    private readonly CS2GSI.CS2GSI _cs2GSI;
    private readonly List<Shocker> _shockers;
    private readonly Settings _settings;
    private readonly Logger? _logger;

    public OpenCS2hock(string? settingsPath = null, Logger? logger = null)
    {
        this._logger = logger;
        this._logger?.Log(LogLevel.Information, "Starting OpenCS2hock...");
        this._logger?.Log(LogLevel.Information, "Loading Settings...");
        _settings = Installer.GetSettings(settingsPath);
        this._logger?.Log(LogLevel.Information, $"Loglevel set to {_settings.LogLevel}");
        this._logger?.UpdateLogLevel(_settings.LogLevel);
        this._logger?.Log(LogLevel.Information, _settings.ToString());
        this._logger?.Log(LogLevel.Information, "Setting up Shockers...");
        this._shockers = Installer.GetShockers(_settings, logger);
        this._cs2GSI = new CS2GSI.CS2GSI(_logger);
        this.SetupEventHandlers();
        while(this._cs2GSI.IsRunning)
            Thread.Sleep(10);
    }

    private void SetupEventHandlers()
    {
        this._logger?.Log(LogLevel.Information, "Setting up EventHandlers...");
        foreach (Shocker shocker in this._shockers)
        {
            foreach (KeyValuePair<string, string> kv in _settings.Actions)
            {
                switch (kv.Key)
                {
                    case "OnKill":
                        this._cs2GSI.OnKill += (cs2EventArgs) => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnDeath":
                        this._cs2GSI.OnDeath += (cs2EventArgs) => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundStart":
                        this._cs2GSI.OnRoundStart += (cs2EventArgs) => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundEnd":
                        this._cs2GSI.OnRoundOver += (cs2EventArgs) => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundLoss":
                        this._cs2GSI.OnRoundLoss += (cs2EventArgs) => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundWin":
                        this._cs2GSI.OnRoundWin += (cs2EventArgs) => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnDamageTaken":
                        this._cs2GSI.OnDamageTaken += (cs2EventArgs) =>
                            shocker.Control(Settings.StringToAction(kv.Value),
                                intensity: MapInt(cs2EventArgs.ValueAsOrDefault<int>(), 0, 100,
                                    _settings.IntensityRange.Min, _settings.IntensityRange.Max));
                        break;
                }
            }
        }
    }

    private int MapInt(int input, int fromLow, int fromHigh, int toLow, int toHigh) 
    {
        int mappedValue = (input - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
        return mappedValue;
    }
}