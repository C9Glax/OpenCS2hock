using System.Net;
using System.Text;

namespace OpenCS2hock;

public class GSIServer
{
    private HttpListener HttpListener { get; init; }

    public delegate void OnMessageEventHandler(string content);

    public event OnMessageEventHandler? OnMessage;

    private bool keepRunning = true;

    public GSIServer(int port)
    {
        HttpListener = new HttpListener();
        HttpListener.Prefixes.Add($"http://127.0.0.1:{port}");
        HttpListener.Start();

        Task connectionListener = HandleConnection();
        connectionListener.GetAwaiter().GetResult();
        
        HttpListener.Close();
    }

    private async Task HandleConnection()
    {
        while (keepRunning)
        {
            HttpListenerContext context = await HttpListener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            
            Console.WriteLine($"[{request.HttpMethod}] {request.Url} - {request.UserAgent}");

            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
            context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(responseMessage.ToString()));
            await context.Response.OutputStream.DisposeAsync();

            StreamReader reader = new StreamReader(request.InputStream);
            OnMessage?.Invoke(await reader.ReadToEndAsync());
            reader.Close();
            await request.InputStream.DisposeAsync();
        }
    }

    internal void Dispose()
    {
        keepRunning = false;
    }
}