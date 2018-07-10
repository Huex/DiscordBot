﻿using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class CommandHandler : LogEntity
    {
        private readonly DiscordSocketClient _discord;
        private CommandConfig _config;

        public IServiceProvider Services { get; set; }
        public CommandService Commands { get; set; }
        public CommandConfig Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
                ConfigUpdated?.Invoke(_config.Id, _config);
            }
        }

        public event Action<ulong, CommandConfig> ConfigUpdated;

        public CommandHandler(DiscordSocketClient discord, IServiceProvider services, CommandService commands, CommandConfig config)
        {
            _discord = discord ?? throw new ArgumentNullException(nameof(discord));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Commands.Log += RaiseLogAsync;
            Config = config;
        }

        public async Task HandleMessage(SocketMessage message)
        {
            await HandleCommand(message as SocketUserMessage);
        }

        private async Task HandleCommand(SocketUserMessage msg)
        {
            if (msg != null)
            {
                int prefixInt = Config.Prefix.Length - 1;
                if (msg.HasStringPrefix(Config.Prefix, ref prefixInt) || msg.HasMentionPrefix(_discord.CurrentUser, ref prefixInt))
                {
                    await ProcessCommandAsync(prefixInt, msg);
                }
            }
        }

        private async Task ProcessCommandAsync(int prefixInt, SocketUserMessage message)
        {
            var context = new SocketCommandContext(_discord, message);
            await message.Channel.TriggerTypingAsync();
            var result = await Commands.ExecuteAsync(context, prefixInt, Services);
            if (!result.IsSuccess)
            {
                await message.Channel.SendMessageAsync(result.ErrorReason);
                await Task.Delay(500);
                await message.AddReactionAsync(new Discord.Emoji("⁉"));
                
            }
            else
            {
                await Task.Delay(500);
                await message.AddReactionAsync(new Discord.Emoji("✅"));
            }
        }
    }

    //public class GuildWorker : BotServiceBase
    //{
    //    public GuildConfig Config => _data.Config;

    //    private ServiceCollection _map;

    //    private readonly DiscordSocketClient _discord;
    //    private readonly CommandService _commands;
    //    private readonly GuildDataManager _data;
    //    private readonly Collection<Type> _availableModules;

    //    public GuildWorker(DiscordSocketClient discord, ServiceCollection map, Collection<Type> modules, GuildConfig config)
    //    {
    //        _discord = discord;
    //        _availableModules = modules;
    //        _commands = new CommandService();
    //        _commands.Log += (p) =>
    //        {
    //            RaiseLog(new LogMessage(p));
    //            return Task.CompletedTask;
    //        };
    //        _data = new GuildDataManager(config);
    //        _data.Log += (p) =>
    //        {
    //            RaiseLog(p);
    //            return Task.CompletedTask;
    //        };
    //        _map = map;
    //        _map.AddSingleton(_data);
    //        _map.AddSingleton(_commands);
    //    }

    //    public void SyncConfig()
    //    {
    //        _data.SyncConfigWithLocalFile();
    //        UpdateModulesByConfig();
    //    }

    //    public void UpdateModulesByConfig()
    //    {
    //        foreach (var moduleType in _availableModules)
    //        {
    //            if (moduleType.IsSubclassOf(typeof(ModuleBase)))
    //            {
    //                var neededAttributes = new List<CustomAttributeData>(moduleType.GetCustomAttributesData());
    //                var need = neededAttributes?.Find(x => x.Constructor.DeclaringType == typeof(NameAttribute));
    //                var value = need?.ConstructorArguments[0].Value;
    //                if (value != null)
    //                {
    //                    if (_data.Config.Modules.Contains((string)value))
    //                    {
    //                        _commands.AddModuleAsync(moduleType);
    //                    }
    //                }
    //            }
    //        }
    //        _map.Remove(_map.FirstOrDefault(d => d.ServiceType == _commands.GetType()));
    //        _map.AddSingleton(_commands);
    //    }

    //    public async Task HandleMessage(SocketMessage message)
    //    {
    //        await HandleCommand(message as SocketUserMessage);
    //    }

    //    private async Task HandleCommand(SocketUserMessage msg)
    //    {
    //        if (msg == null)
    //        {
    //            return;
    //        }
    //        var prefix = _data.Config.Prefix;
    //        int prefixInt = prefix.Length - 1;
    //        if (msg.HasStringPrefix(prefix, ref prefixInt) || msg.HasMentionPrefix(_discord.CurrentUser, ref prefixInt))
    //        {
    //            await ProcessCommandAsync(prefixInt, msg);
    //        }
    //    }

    //    private async Task ProcessCommandAsync(int prefixInt, SocketUserMessage message)
    //    {
    //        var context = new SocketCommandContext(_discord, message);

    //        await message.Channel.TriggerTypingAsync();
    //        var result = await _commands.ExecuteAsync(context, prefixInt, _map.BuildServiceProvider());
    //        if (!result.IsSuccess)
    //        {
    //            await message.Channel.SendMessageAsync(result.ErrorReason);
    //        }
    //    }
    //}
}
