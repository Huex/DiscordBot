using DiscordBot.Core;
using DiscordBot.Packets.Settings;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBot.Jill
{
    public class Program
    {
        private Core.DiscordBot _bot;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Task Log(LogMessage logMessage)
        {
            if (_bot.Config.LogLevel < logMessage.Severity)
            {
                return Task.CompletedTask;
            }
            var cc = Console.ForegroundColor;
            switch (logMessage.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.ForegroundColor = cc;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} {logMessage.Source}: {logMessage.Message} {logMessage.Exception?.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            var botSettings = FileDataManager.ReadBotConfig("settings.json");
            //var botSettings1 = new BotConfig();
            //botSettings1.DefaultGuildCommandConfig = new CommandConfigBuilder();
            //botSettings1.DefaultGuildCommandConfig.Modules = new Collection<string>();
            //botSettings1.DefaultGuildCommandConfig.Modules.Add("dasd");
            //FileDataManager.WriteBotConfig("settings2.json", botSettings1);
            _bot = new Core.DiscordBot(botSettings, new Collection<Packet>
            {
                new SettingsPacket()
            });
            _bot.Log += Log;
            await _bot.StartAsync();
            await Task.Delay(-1);
        }
    }
}
