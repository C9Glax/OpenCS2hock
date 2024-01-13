namespace OpenCS2hock;

public abstract class Shocker
{
    protected readonly HttpClient HttpClient;
    protected readonly string ApiKey;
    protected readonly string Endpoint;
    protected string[] ShockerIds;

    public enum ControlAction { Beep, Vibrate, Shock, Nothing }

    public abstract void Control(ControlAction action, byte intensity, short duration, string? shockerId = null);

    protected Shocker(string endpoint, string apiKey, string[] shockerIds)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;
        this.HttpClient = new HttpClient();
        this.ShockerIds = shockerIds;
    }
}