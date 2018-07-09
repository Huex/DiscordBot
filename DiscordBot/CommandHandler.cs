using Discord.Commands;
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

        public IServiceProvider Services { get; set; }
        public CommandService Commands { get; set; }
        public string Prefix { get; set; }
        public ulong Id { get; }

        public CommandHandler(DiscordSocketClient discord, IServiceProvider services, CommandService commands, string prefix, ulong id)
        {
            _discord = discord;
            Commands.Log += RaiseLogAsync;
            Services = services;
            Prefix = prefix;
            Id = id;
        }

        public async Task HandleMessage(SocketMessage message)
        {
            await HandleCommand(message as SocketUserMessage);
        }

        private async Task HandleCommand(SocketUserMessage msg)
        {
            if (msg != null)
            {
                int prefixInt = Prefix.Length - 1;
                if (msg.HasStringPrefix(Prefix, ref prefixInt) || msg.HasMentionPrefix(_discord.CurrentUser, ref prefixInt))
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
            }
            else
            {
                RaiseLog(LogSeverity.Critical, "БЛЯ");
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
