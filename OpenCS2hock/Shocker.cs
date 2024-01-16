using Microsoft.Extensions.Logging;

namespace OpenCS2hock;

internal abstract class Shocker
{
    protected readonly HttpClient HttpClient;
    protected readonly string ApiKey,Endpoint;
    private readonly string[] _shockerIds;
    private readonly ConfiguredInteger _intensity, _duration;
    protected readonly Logger? Logger;

    internal enum ControlAction { Beep, Vibrate, Shock, Nothing }

    internal void Control(ControlAction action, string? shockerId = null, int? intensity = null, int? duration = null)
    {
        int i = intensity ?? _intensity.GetValue();
        int d = duration ?? _duration.GetValue();
        this.Logger?.Log(LogLevel.Information, $"{action} {(intensity is not null ? "Overwrite " : "")}{i} {(duration is not null ? "Overwrite " : "")}{d}");
        if (action is ControlAction.Nothing)
            return;
        if(shockerId is null)
            foreach (string shocker in _shockerIds)
                ControlInternal(action, shocker, i, d);
        else
            ControlInternal(action, shockerId, i, d);
    }

    protected abstract void ControlInternal(ControlAction action, string shockerId, int intensity, int duration);

    protected Shocker(string endpoint, string apiKey, string[] shockerIds, ConfiguredInteger intensity, ConfiguredInteger duration, Logger? logger = null)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;
        this.HttpClient = new HttpClient();
        this._shockerIds = shockerIds;
        this._intensity = intensity;
        this._duration = duration;
        this.Logger = logger;
    }
}