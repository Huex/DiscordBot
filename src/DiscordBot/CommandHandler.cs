using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public class CommandHandler : ILogEntity
    {
        private readonly DiscordClient _discord;
        private CommandConfig _config;

        protected LogRaiser Logger;

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
                RaiseConfigUpdatedEventAsync().ConfigureAwait(true);
            }
        }

        public CommandHandler(DiscordClient discord, IServiceProvider services, CommandService commands, CommandConfig config)
        {       
            _discord = discord ?? throw new ArgumentNullException(nameof(discord));
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = new LogRaiser(GetType().Name, async (msg) => await Log?.Invoke(msg));
            Commands.Log += Logger.RaiseAsync;

            Config = config;
        }

        public async Task HandleMessage(SocketMessage message)
        {
            await HandleCommand(message as SocketUserMessage).ConfigureAwait(true);
        }

        private async Task RaiseConfigUpdatedEventAsync()
        {
            await Logger.RaiseAsync(LogSeverity.Debug, 
                $"Command config updated {Config.Source.ToString().ToLower()} {Config.Name}");

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

        private async Task HandleCommand(SocketUserMessage msg)
        {
            if (msg != null)
            {
                int prefixInt = Config.Prefix.Length - 1;
                if (msg.HasMentionPrefix(_discord.CurrentUser, ref prefixInt) 
                    || msg.HasStringPrefix(Config.Prefix, ref prefixInt))
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
                await message.Channel.SendMessageAsync($"Message:`{message.Content}`\n{result.ErrorReason}");
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}