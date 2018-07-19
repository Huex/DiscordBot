using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Packets.Settings
{
    [Name("Settings")]
    [Summary("Module for bot configuration")]
    public class SettingsModule : ModuleBase
    {
        private readonly SettingsService _settings;

        private ulong _commandHandlerId => 
            (Context.Channel is IDMChannel) && (Context.Guild == null) ? (Context.Channel as IDMChannel).Recipient.Id : Context.Guild.Id;
        private IPacketCommandService _commandService => _settings.CommandServicesProvider.CommandServices[_commandHandlerId];
        private CommandConfig _commandConfig => _settings.CommandConfigsProvider.CommandConfigs[_commandHandlerId];

        public SettingsModule(SettingsService settings)
        {
            _settings = settings;
        }

        [Name("Prefix"), Command("prefix"), Alias("pref")]
        [Summary("Sets the prefix for the bot commands on the current server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PrefixAsync(string prefix)
        {
            
            _settings.SetPrefix(_commandHandlerId, prefix);
            var message = await ReplyAsync($"Prefix `{prefix}` was set.");
            new Timer((s) => message.DeleteAsync(), null, 5000, Timeout.Infinite);
        }

        [Name("Prefix"), Command("prefix"), Alias("pref")]
        [Summary("Displays the current prefix on the server")]
        public async Task PrefixAsync()
        {
            await ReplyAsync($"{Context.User.Mention} current prefix: `{_commandConfig.Prefix}`");
        }

        [Name("Modules"), Command("modules"), Alias("mod")]
        [Summary("Displays the active modules on the server")]
        public async Task GetModulesAsync()
        {
            var fields = new List<EmbedFieldBuilder>();
            foreach (var module in _commandService.Modules)
            {
                fields.Add(new EmbedFieldBuilder
                {
                    Name = module.Name,
                    Value = $"{module.Summary}\n{module.Remarks}"
                });
            }
            await ReplyAsync("", false, new EmbedBuilder
            {
                Title = "Active modules:",
                Fields = fields
            });
        }

        [Name("Commands"), Command("commands"), Alias("com")]
        [Summary("Displays the commands of module")]
        public async Task GetCommandsAsync(string moduleName)
        {
            var module = new List<ModuleInfo>(_commandService.Modules).Find(m => m.Name.ToLower() == moduleName.ToLower());
            if(module != null)
            {
                var fields = new List<EmbedFieldBuilder>();
                foreach(var command in module.Commands)
                {
                    bool skip = false;
                    foreach(var perm in command.Preconditions)
                    {
                        var res = await perm.CheckPermissions(Context, command, null);
                        if (res.Error.HasValue)
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (!skip)
                    {
                        var parameters = command.Parameters.Count != 0 ? $"<{String.Join(" ", command.Parameters)}>" : "";
                        var aliases = new List<string>(command.Aliases);
                        aliases.RemoveAt(0);
                        fields.Add(new EmbedFieldBuilder
                        {
                            Name = $"{command.Aliases[0]} {parameters}",
                            Value = $"Aliases: `{String.Join("` `", aliases)}`\n{command.Summary}"
                        });
                    }             
                }
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title =  $"{module.Name} for {Context.User.Username}#{Context.User.Discriminator}",
                    Fields = fields
                });
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention} module `{moduleName}` not exists");
            }
        }

        [Name("Commands"), Command("commands"), Alias("com")]
        [Summary("Displays the active commands on the server")]
        public async Task GetCommandsAsync()
        {
            foreach (var module in _commandService.Modules)
            {
                await GetCommandsAsync(module.Name);
            }
        }

        [Name("Ping"), Command("ping"), Alias("p")]
        [Summary("Displays ping")]
        public async Task PingAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();   
            var mes = await ReplyAsync($"Pong!");
            sw.Stop();
            await mes.ModifyAsync((p) =>
             {
                 p.Content = $"Ping: {sw.ElapsedMilliseconds}ms";
             });
        }
    }
}
