using System.Net;
using System.Text;

namespace MyHttpServer;

public class HttpServer
{
    private HttpListener server;
    
    private bool _isListening;
    
    public HttpServer(Settings settings)
    {
        server = new HttpListener();
        server.Prefixes.Add($"http://{settings.Server.Host}:{settings.Server.Port}/{settings.Server.Path}");
    }
    
    
    public void Start()
    {
        server.Start();
        _isListening = true;
        Console.WriteLine("Сервер запущен");
        Listen();
    }

    private async Task Listen()
    {
        while (_isListening)
        {
            try
            {
                var context = await server.GetContextAsync();

                HttpListenerResponse response = context.Response;

                string htmlFileText = File.ReadAllText("index.html");
                byte[] buffer = Encoding.UTF8.GetBytes(htmlFileText);

                response.ContentLength64 = buffer.Length;

                using Stream output = response.OutputStream;
                await output.WriteAsync(buffer);
                await output.FlushAsync();
            
                Console.WriteLine("Запрос обработан");
            }
            catch (HttpListenerException)
            {
            }
        }
    }

    public void Stop()
    {
        _isListening = false;
        server.Stop();
        Console.WriteLine("Сервер выключен");
    }
}