﻿using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
                UpdateCommandsAsync(Config).ConfigureAwait(true);
            }
        }

        private async Task UpdateCommandsAsync(CommandConfig config)
        {
            foreach (var module in Commands.Modules)
            {
                var needed = new List<string>(config.Modules);
                if (!needed.Contains(module.Name))
                {
                    await Commands.RemoveModuleAsync(module);
                }
            }
        }

        private static string GetNameFromModule(Type module)
        {
            var attributes = new List<CustomAttributeData>(module.GetCustomAttributesData());
            var needed = attributes?.Find(p => p.AttributeType == typeof(NameAttribute));
            return (string)needed?.ConstructorArguments[0].Value;
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
}