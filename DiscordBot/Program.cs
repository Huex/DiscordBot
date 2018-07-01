using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DiscordBot.Modules;
using DiscordBot.Services;
using DiscordBot.Core;
using DiscordBot.Core.Packets.Settings;

namespace DiscordBot
{
    public class Program
    {
        private Bot _bot;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Task Log(Discord.LogMessage logMessage)
        {
            Log(new LogMessage(logMessage));
            return Task.CompletedTask;
        }

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
            _bot = new Bot(botSettings);
            _bot.AddPacket(new CommandHandlerPacket(_bot));
            _bot.AddPacket(new SettingsPacket());           
            _bot.Log += Log;
            await _bot.LoginAsync();
            await _bot.StartAsync();

            await Task.Delay(-1);
        }
    }
}