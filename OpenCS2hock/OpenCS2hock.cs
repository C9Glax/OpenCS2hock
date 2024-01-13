namespace OpenCS2hock;

public class OpenCS2hock
{
    private GSIServer GSIServer { get; init; }
    public OpenCS2hock()
    {
        GSIServer gsiServer;
        Installer.InstallGsi();
        this.GSIServer = new GSIServer(3000);
        
        this.GSIServer.Dispose();
    }
}