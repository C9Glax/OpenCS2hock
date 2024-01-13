namespace OpenCS2hock;

public abstract class Shocker
{
    public string ApiKey, Endpoint;
    public enum ControlAction { Beep, Vibrate, Shock }
    public abstract void Control(ControlAction action, byte intensity, short duration);

    public Shocker(string endpoint, string apiKey)
    {
        this.Endpoint = endpoint;
        this.ApiKey = apiKey;
    }
}