using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Modules;
using DiscordBot.Services;

namespace DiscordBot
{
    public class GuildWorker : BotServiceBase
    {
        ///добавить запись о гуилд на диск здесь
        public bool IsWork { get; private set; }

        public GuildConfig Config => _data.Config;

        private ServiceCollection _map;

        private readonly Random _random = new Random();
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly GuildDataManager _data;
        private readonly Collection<Type> _availableModules;

        public GuildWorker(DiscordSocketClient discord, ServiceCollection map, Collection<Type> modules, GuildConfig config)
        {
            _discord = discord;
            _availableModules = modules;
            _commands = new CommandService();
            _commands.Log += (p) =>
            {
                RaiseLog(new LogMessage(p));
                return Task.CompletedTask;
            };
            _data = new GuildDataManager(config);
            _data.Log += (p) =>
            {
                RaiseLog(p);
                return Task.CompletedTask;
            };
            _map = map;
            _map.AddSingleton(_data);
            _map.AddSingleton(_commands);
        }

        public void SyncConfig()
        {
            _data.SyncConfigWithLocalFile();
            InitModulesByConfig();
        }

        public void InitModulesByConfig()
        {
            foreach (var moduleType in _availableModules)
            {
                if (moduleType.IsSubclassOf(typeof(ModuleBase)))
                {
                    var neededAttributes = new List<CustomAttributeData>(moduleType.GetCustomAttributesData());
                    var need = neededAttributes?.Find(x => x.Constructor.DeclaringType == typeof(NameAttribute));
                    var value = need?.ConstructorArguments[0].Value;
                    if (value != null)
                    {
                        if (_data.Config.Modules.Contains((string)value))
                        {
                            _commands.AddModuleAsync(moduleType);
                        }
                    }
                }
            }
            _map.Remove(_map.FirstOrDefault(d => d.ServiceType == _commands.GetType()));
            _map.AddSingleton(_commands);
        }

        public async Task HandleMessage(SocketMessage message)
        {
            await HandleCommand(message as SocketUserMessage);
        }

        private async Task HandleCommand(SocketUserMessage msg)
        {
            if (msg == null)
            {
                return;
            }
            var prefix = _data.Config.Prefix;
            int prefixInt = prefix.Length - 1;
            if (msg.HasStringPrefix(prefix, ref prefixInt) || msg.HasMentionPrefix(_discord.CurrentUser, ref prefixInt))
            {
                await ProcessCommandAsync(prefixInt, msg);
            }
        }

        private async Task ProcessCommandAsync(int prefixInt, SocketUserMessage message)
        {
            var context = new SocketCommandContext(_discord, message);

            await message.Channel.TriggerTypingAsync();
            var result = await _commands.ExecuteAsync(context, prefixInt, _map.BuildServiceProvider());
            if (!result.IsSuccess)
            {
                List<GuildEmote> emotes = new List<GuildEmote>(context.Guild.Emotes);
                if (emotes.Count != 0)
                {
                    var emote = emotes[_random.Next(0, emotes.Count - 1)];
                    await message.Channel.SendMessageAsync($"<:{emote.Name}:{emote.Id}>");
                    await Task.Delay(500);
                    await message.Channel.SendMessageAsync($"{message.Author.Mention} чо?");
                }
                else
                {
                    await message.AddReactionAsync(new Emoji("❔"));
                }
            }
        }
    }
}
