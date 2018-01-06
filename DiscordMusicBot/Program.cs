using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.IO;

namespace DiscordMusicBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetSpotifyTrackInfo());
            try
            {
                DiscordSocketClient _client = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    // Set only general info to be logged.
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 1000
                });

                _client.Log += message =>
                {
                    Console.WriteLine(message.ToString());
                    // Represents a completed task for methods that have to return Task but don't do anything asychronously.
                    return Task.CompletedTask;
                };

                JObject config = JObject.Parse(File.ReadAllText(@".\config.json"));
                string token = (string)config["token"];

                _client.LoginAsync(TokenType.User, token);
                _client.StartAsync();
                string songName = "";
                while(true)
                {
                    songName = GetSpotifyTrackInfo();
                    Console.Title = "Listening to: " + songName;
                    _client.SetGameAsync(GetSpotifyTrackInfo(), null, StreamType.NotStreaming + 2);
                    Thread.Sleep(10000);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }



            Console.ReadLine();
        }

        public static string GetSpotifyTrackInfo()
        {
            var proc = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));

            if (proc == null)
            {
                return "Spotify is not running!";
            }

            if (string.Equals(proc.MainWindowTitle, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                return "Paused";
            }
            return proc.MainWindowTitle;
        }

    }
}
