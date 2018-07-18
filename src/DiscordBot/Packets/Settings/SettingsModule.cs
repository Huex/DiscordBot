using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Packets.Settings
{
    [Name("Settings")]
    public class SettingsModule : ModuleBase
    {
        private readonly SettingsService _settings;

        private ulong CommandHandlerId => 
            (Context.Channel is IDMChannel) && (Context.Guild == null) ? (Context.Channel as IDMChannel).Recipient.Id : Context.Guild.Id;

        public SettingsModule(SettingsService settings)
        {
            _settings = settings;
        }

        [Name("Prefix"), Command("prefix"), Alias("pref")]
        [Summary("Sets the prefix for the bot commands on the current server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PrefixAsync(string prefix)
        {
            
            _settings.SetPrefix(CommandHandlerId, prefix);
            var message = await ReplyAsync($"Prefix `{prefix}` was set.");
            new Timer((s) => message.DeleteAsync(), null, 5000, Timeout.Infinite);
        }

        [Name("Prefix"), Command("prefix"), Alias("pref")]
        [Summary("Displays the current prefix on the server")]
        public async Task PrefixAsync()
        {
            await ReplyAsync($"{Context.User.Mention} current prefix: `{_settings.GetPrefix(CommandHandlerId)}`");
        }

        [Name("Modules"), Command("modules"), Alias("mod")]
        [Summary("Displays the active modules on the server")]
        public async Task GetModulesAsync()
        {
            await ReplyAsync($"{Context.User.Mention} active modules:\n`{String.Join("\n", _settings.GetModules(CommandHandlerId).ToArray())}`");
        }

        [Name("Ping"), Command("ping"), Alias("p")]
        [Summary("Displays ping")]
        public async Task PingAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();   
            var mes = await ReplyAsync($"ты пидор");
            sw.Stop();
            await mes.ModifyAsync((p) =>
             {
                 p.Content = $"Ping: {sw.ElapsedMilliseconds}ms";
             });
        }
    }
}
