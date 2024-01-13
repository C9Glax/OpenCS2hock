namespace OpenCS2hock;

public class OpenCS2hock
{
    private GSIServer GSIServer { get; init; }
    private readonly CS2MessageHandler _cs2MessageHandler;
    private readonly List<Shocker> _shockers;
    private readonly Settings _settings;

    public OpenCS2hock(string? settingsPath = null)
    {
        _settings = Installer.GetSettings(settingsPath);
        this._shockers = Installer.GetShockers(_settings);
        Installer.InstallGsi();
        
        this._cs2MessageHandler = new CS2MessageHandler();
        
        this.GSIServer = new GSIServer(3000);
        this.GSIServer.OnMessage += OnGSIMessage;

        Thread runningThread = new(() =>
        {
            while (GSIServer.IsRunning)
                Thread.Sleep(10);
        });
        runningThread.Start();
    }

    private void SetupEventHandlers()
    {
        foreach (Shocker shocker in this._shockers)
        {
            foreach (KeyValuePair<string, string> kv in _settings.Actions)
            {
                switch (kv.Key)
                {
                    case "OnKill":
                        this._cs2MessageHandler.OnKill += () => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnDeath":
                        this._cs2MessageHandler.OnDeath += () => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundStart":
                        this._cs2MessageHandler.OnRoundStart += () => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundEnd":
                        this._cs2MessageHandler.OnRoundEnd += () => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundLoss":
                        this._cs2MessageHandler.OnRoundLoss += () => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                    case "OnRoundWin":
                        this._cs2MessageHandler.OnRoundWin += () => shocker.Control(Settings.StringToAction(kv.Value));
                        break;
                }
            }
        }
    }

    private void OnGSIMessage(string content)
    {
        string fileName = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.ToLongTimeString().Replace(':','.')}.json");
        File.WriteAllText(fileName, content);
        Console.WriteLine(fileName);
        _cs2MessageHandler.HandleCS2Message(content);
    }
}