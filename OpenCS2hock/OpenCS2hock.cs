namespace OpenCS2hock;

public class OpenCS2hock
{
    private GSIServer GSIServer { get; init; }
    private List<Shocker> _shockers = new();
    private readonly Settings _settings;

    public OpenCS2hock(string? settingsPath = null)
    {
        _settings = Installer.GetSettings(settingsPath);
        Installer.InstallGsi();
        this.GSIServer = new GSIServer(3000);
        this.GSIServer.OnMessage += OnGSIMessage;

        Thread runningThread = new(() =>
        {
            while (GSIServer.IsRunning)
                Thread.Sleep(10);
        });
        runningThread.Start();
    }

    private void OnGSIMessage(string content)
    {
        string fileName = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.ToLongTimeString().Replace(':','.')}.json");
        File.WriteAllText(fileName, content);
        Console.WriteLine(fileName);
    }
}