namespace OpenCS2hock;

public abstract class Shocker
{
    protected readonly HttpClient HttpClient;
    protected readonly string ApiKey,Endpoint;
    private readonly string[] _shockerIds;
    private readonly ConfiguredInteger _intensity, _duration;

    public enum ControlAction { Beep, Vibrate, Shock, Nothing }

    public void Control(ControlAction action, string? shockerId = null)
    {
        int intensity = _intensity.GetValue();
        int duration = _duration.GetValue();
        Console.WriteLine($"{action} {intensity} {duration}");
        if(shockerId is null)
            foreach (string shocker in _shockerIds)
                ControlInternal(action, shocker, intensity, duration);
        else
            ControlInternal(action, shockerId, intensity, duration);
    }

    protected abstract void ControlInternal(ControlAction action, string shockerId, int intensity, int duration);

    protected Shocker(string endpoint, string apiKey, string[] shockerIds, ConfiguredInteger intensity, ConfiguredInteger duration)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;
        this.HttpClient = new HttpClient();
        this._shockerIds = shockerIds;
        this._intensity = intensity;
        this._duration = duration;
    }
}