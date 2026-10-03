using System;
using System.Net;
using System.Text;
using System.Text.Json;
using MyHttpServer;

Settings settings = new Settings();
if (File.Exists("settings.json"))
{
    string settingsJson = File.ReadAllText("settings.json");
    settings = JsonSerializer.Deserialize<Settings>(settingsJson);
}


HttpServer server = new HttpServer(settings);
server.Start();

while (true)
{
    string? command =  Console.ReadLine();
    if (command != null && command.Equals("shutdown"))
    {
        server.Stop();
        break;
    }
}