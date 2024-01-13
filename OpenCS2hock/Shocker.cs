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
        if(shockerId is null)
            foreach (string shocker in _shockerIds)
            {
                int intensity = _intensity.GetValue();
                int duration = _duration.GetValue();
                ControlInternal(action, shocker, intensity, duration);
                Console.WriteLine($"{shocker} {action} {intensity} {duration}");
            }
        else
        {
            int intensity = _intensity.GetValue();
            int duration = _duration.GetValue();
            ControlInternal(action, shockerId, intensity, duration);
            Console.WriteLine($"{shockerId} {action} {intensity} {duration}");
        }
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