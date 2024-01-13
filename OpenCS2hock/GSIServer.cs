using System.Net;
using System.Text;

namespace OpenCS2hock;

public class GSIServer
{
    private HttpListener HttpListener { get; init; }
    public delegate void OnMessageEventHandler(string content);
    public event OnMessageEventHandler? OnMessage;

    private bool _keepRunning = true;
    public bool IsRunning { get; private set; }

    public GSIServer(int port)
    {
        HttpListener = new HttpListener();
        HttpListener.Prefixes.Add($"http://127.0.0.1:{port}/");
        HttpListener.Start();

        Thread connectionListener = new (HandleConnection);
        connectionListener.Start();

        IsRunning = true;
    }

    private async void HandleConnection()
    {
        while (_keepRunning)
        {
            HttpListenerContext context = await HttpListener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            
            Console.WriteLine($"[{request.HttpMethod}] {request.Url} - {request.UserAgent}");

            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
            context.Response.OutputStream.Write(Encoding.UTF8.GetBytes(responseMessage.ToString()));

            StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding);
            string content = await reader.ReadToEndAsync();
            Console.WriteLine(content);
            OnMessage?.Invoke(content);
        }
        HttpListener.Close();
        IsRunning = false;
    }

    internal void Dispose()
    {
        _keepRunning = false;
    }
}