using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class CommandHandler : ILogEntity
    {
        private readonly DiscordSocketClient _discord;
        private CommandConfig _config;

        public event Action<ulong, CommandConfig> ConfigUpdated;
        public event Func<LogMessage, Task> Log;

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
                UpdateCommandsAsync(value).ConfigureAwait(true);
                _config = RemoveUnavailableModules(value);
                RaiseConfiUpdatedEvent();
            }
        }

        #region Log
        protected void RaiseLog(LogSeverity severity, string message, Exception exception = null)
        {
            RaiseLog(new LogMessage(severity, GetType().Name, message, exception));
        }

        protected void RaiseLog(LogMessage message)
        {
            Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(LogMessage message)
        {
            await Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(Discord.LogMessage message)
        {
            await Log?.Invoke(new LogMessage(message));
        }
        #endregion

        private void RaiseConfiUpdatedEvent()
        {
            RaiseLog(LogSeverity.Debug, $"Command config updated {Config.Source.ToString().ToLower()} {Config.Name}");
            ConfigUpdated?.Invoke(_config.Id, _config);
        }

        private async Task UpdateCommandsAsync(CommandConfig config)
        {
            foreach (var module in Commands.Modules)
            {
                if (!config.Modules.Contains(module.Name))
                {
                    await Commands.RemoveModuleAsync(module);
                }
            }
        }

        private CommandConfig RemoveUnavailableModules(CommandConfig config)
        {
            var commands = Commands.Modules.ToList();
            var needed = config.Modules.ToList();
            needed.RemoveAll((c) =>
            {
                return !commands.Exists(p => p.Name == c);
            });
            return new CommandConfig(config.Source, config.Name, config.Id, config.Prefix, needed);
        }

        private static string GetNameFromModule(Type module)
        {
            var attributes = new List<CustomAttributeData>(module.GetCustomAttributesData());
            var needed = attributes?.Find(p => p.AttributeType == typeof(NameAttribute));
            return (string)needed?.ConstructorArguments[0].Value;
        }

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
            await HandleCommand(message as SocketUserMessage).ConfigureAwait(true);
        }

        private async Task HandleCommand(SocketUserMessage msg)
        {
            if (msg != null)
            {
                int prefixInt = Config.Prefix.Length - 1;
                if (msg.HasMentionPrefix(_discord.CurrentUser, ref prefixInt) || msg.HasStringPrefix(Config.Prefix, ref prefixInt))
                {
                    await ProcessCommandAsync(prefixInt, msg).ConfigureAwait(true);
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
                await Task.Delay(500).ConfigureAwait(true);
                await message.AddReactionAsync(new Discord.Emoji("⁉"));

            }
            else
            {
                await Task.Delay(500).ConfigureAwait(true);
                await message.AddReactionAsync(new Discord.Emoji("✅"));
            }
        }
    }
}