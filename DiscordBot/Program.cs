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

namespace DiscordBot
{
    public class Program
    {
        private readonly List<GuildWorker> _guilds = new List<GuildWorker>();
        private readonly ServiceCollection _map = new ServiceCollection();

        private readonly Collection<Type> _modules = new Collection<Type>
        {
            typeof(SettingsModule)
        };

        private readonly Collection<Type> _defaultGuildModules = new Collection<Type>
        {
            typeof(SettingsModule)
        };

        private static DiscordSocketClient _discord;
        private static BotConfig _botSettings;

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private Task Log(Discord.LogMessage logMessage)
        {
            Log(new LogMessage(logMessage));
            return Task.CompletedTask;
        }

        private Task Log(LogMessage logMessage)
        {
            if (_botSettings.LogLevel < logMessage.Severity)
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

        public static Collection<string> GetNamesOfModules(Collection<Type> modules)
        {
            var modulesNames = new Collection<string>();
            foreach (var module in modules)
            {
                if (module.IsSubclassOf(typeof(ModuleBase)))
                {
                    modulesNames.Add(GetNameFromModule(module));
                }          
            }
            return modulesNames;
        }

        private static string GetNameFromModule(Type module)
        {
            var attributes = new List<CustomAttributeData>(module.GetCustomAttributesData());
            var needed = attributes?.Find(p => p.AttributeType == typeof(NameAttribute));
            return (string)needed?.ConstructorArguments[0].Value;
        }

        private async Task MainAsync()
        {
            _botSettings = FileDataManager.ReadBotConfig("settings.json");

            InitDiscordSocket();
            InitEventHandlers();

            await _discord.LoginAsync(TokenType.Bot, _botSettings.Token);
            await _discord.StartAsync();
            await Task.Delay(-1);
        }

        private void InitDiscordSocket()
        {
            _discord = new DiscordSocketClient(_botSettings.DiscordSocket);
            _discord.Log += Log;
        }

        private void InitEventHandlers()
        {
            _discord.MessageReceived += HandleMessage;
            _discord.GuildAvailable += GuildAvailable;
            //_discord.GuildUpdated += GuildUpdated;
            //_discord.Ready += ClientReady;
        }

        private Task HandleMessage(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg) || msg.Source != MessageSource.User)
            {
                return Task.CompletedTask;
            }
            var guild = (new SocketCommandContext(_discord, msg)).Guild;
            if (guild != null)
            {
                GuildWorker guildWorker = _guilds.Find(p => p.Config.Id == guild.Id);
                guildWorker?.HandleMessage(arg).ConfigureAwait(false);
            }
            return Task.CompletedTask;
        }

        //public LocalBotConfig DefaultLocalBotSettings { get; } = new LocalBotConfig
        //{
        //    DefaultPrefix = "hx!",
        //    LogLevel = LogSeverity.Debug,
        //    DiscordSocketLogLevel = Discord.LogSeverity.Debug,
        //    Token = "",
        //    ConnectionTimeout = 300,
        //    MessageCacheSize = 0,
        //    LargeThreshold = 250,
        //    AlwaysDownloadUsers = false,
        //    HandlerTimeout = 3000
        //};

        private Task GuildAvailable(SocketGuild guild)
        {
            var guildWorker = new GuildWorker(_discord, _map, _modules, GetDefaultGuildConfig(guild));
            guildWorker.Log += Log;
            guildWorker.SyncConfig();
            _guilds.Add(guildWorker);       
            return Task.CompletedTask;
        }

        private GuildConfig GetDefaultGuildConfig(SocketGuild guild)
        {
            return new GuildConfigBuilder
            {
                Id = guild.Id,
                Name = guild.Name,
                OwnerId = guild.Owner.Id,
                Owner = guild.Owner.Username + "#" + guild.Owner.Discriminator,
                Prefix = _botSettings.DefaultPrefix,
                Modules = GetNamesOfModules(_defaultGuildModules)
            }.Build();
        }
    }
}