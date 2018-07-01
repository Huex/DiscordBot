using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class CommandHandlerPacket : BotServiceBase, IPacket
    {
        public DiscordEventsHandlers EventHandlers { get; }

        public CommandsHandler GuildCommandsHandler { get; }

        public CommandsHandler DirectCommandsHandler { get; }

        private readonly string DefaultPrefix = "hx!";
        private readonly List<CommandHandler> _guilds = new List<CommandHandler>();
        private readonly ServiceCollection _map = new ServiceCollection();
        private readonly Bot _bot;
        

        public CommandHandlerPacket(Bot bot)
        {
            _bot = bot;
            EventHandlers = new DiscordEventsHandlers
            {
                GuildAvailable = GuildAvailable,
                MessageReceived = HandleMessage
            };
        }

        private readonly Collection<Type> _avalaibleModules = new Collection<Type>
        {
            typeof(SettingsModule)
        };

        private readonly Collection<Type> _defaultGuildModules = new Collection<Type>
        {
            typeof(SettingsModule)
        };

        private Task GuildAvailable(SocketGuild guild)
        {
            Collection<Type> modules = new Collection<Type>();
            foreach(var packet in _bot.Packets)
            {
                if(packet.GuildCommandsHandler != null)
                {
                    foreach (var module in packet.GuildCommandsHandler.Modules)
                    {
                        modules.Add(module);
                    }
                }               
            }
            var guildWorker = new CommandHandler(_bot._discord, _map, modules, DefaultPrefix, guild.Id);
            guildWorker.Log += (p) =>
            {
                RaiseLog(p);
                return Task.CompletedTask;
            };
            _guilds.Add(guildWorker);
            return Task.CompletedTask;
        }

        private static string GetNameFromModule(Type module)
        {
            var attributes = new List<CustomAttributeData>(module.GetCustomAttributesData());
            var needed = attributes?.Find(p => p.AttributeType == typeof(NameAttribute));
            return (string)needed?.ConstructorArguments[0].Value;
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

        private GuildConfig GetDefaultGuildConfig(SocketGuild guild)
        {
            return new GuildConfigBuilder
            {
                Id = guild.Id,
                Name = guild.Name,
                OwnerId = guild.Owner.Id,
                Owner = guild.Owner.Username + "#" + guild.Owner.Discriminator,
                Prefix = DefaultPrefix,
                Modules = GetNamesOfModules(_defaultGuildModules)
            }.Build();
        }

        private Task HandleMessage(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg) || msg.Source != MessageSource.User)
            {
                return Task.CompletedTask;
            }
            var guild = (new SocketCommandContext(_bot._discord, msg)).Guild;
            if (guild != null)
            {
                CommandHandler guildWorker = _guilds.Find(p => p.GuildId == guild.Id);
                guildWorker?.HandleMessage(arg).ConfigureAwait(false);
            }
            else
            {
                msg.Channel.SendMessageAsync("чо?");
            }
            return Task.CompletedTask;
        }

    }
}
