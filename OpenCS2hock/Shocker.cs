namespace OpenCS2hock;

public abstract class Shocker
{
    protected readonly HttpClient HttpClient;
    protected readonly string ApiKey,Endpoint;
    protected readonly string[] ShockerIds;
    protected readonly ConfiguredInteger Intensity, Duration;

    public enum ControlAction { Beep, Vibrate, Shock, Nothing }

    public abstract void Control(ControlAction action, string? shockerId = null);

    protected Shocker(string endpoint, string apiKey, string[] shockerIds, ConfiguredInteger intensity, ConfiguredInteger duration)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;
        this.HttpClient = new HttpClient();
        this.ShockerIds = shockerIds;
        this.Intensity = intensity;
        this.Duration = duration;
    }
}