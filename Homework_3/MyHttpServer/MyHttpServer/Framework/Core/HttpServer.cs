using System.Net;
using System.Text;
using System.Text.Json;

namespace MyHttpServer;

public class HttpServer
{
    private HttpListener server;
    private bool _isListening;
    public Dictionary<string, string> _mimeTypes;
    
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
        if (File.Exists("mime-types.json"))
        {
            string typesJson = File.ReadAllText("mime-types.json");
            _mimeTypes = JsonSerializer.Deserialize<Dictionary<string, string>>(typesJson);
        }
        Listen();
    }

    private string GetContentType(string extension)
    {
        if (_mimeTypes.TryGetValue(extension.ToLower(), out string contentType))
        {
            return contentType;
        }
        else return "text/html";
    }
    
    private async Task Listen()
    {
        while (_isListening)
        {
            try
            {
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                
                string filePath = Directory.GetCurrentDirectory() + "/static" + request.Url.LocalPath;

                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    response.StatusCode=404;
                    filePath = Directory.GetCurrentDirectory() + "/static/404.html";
                    response.ContentType = "text/html";
                }
                else
                {
                    response.ContentType = GetContentType(fileInfo.Extension);
                }
                
                byte[] buffer = await File.ReadAllBytesAsync(filePath);

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