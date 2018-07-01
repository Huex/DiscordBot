using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    [Name("Settings")]
    public class SettingsModule : ModuleBase
    {
      //  private GuildDataManager _data;

        //public SettingsModule(GuildDataManager data)
        //{
        //    _data = data;
        //}

        //[Name("Prefix"), Command("prefix"), Alias("pref")]
        //[Summary("Sets the prefix for the bot commands on the current server")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        //public async Task PrefixAsync(string prefix)
        //{
        //    _data.SetGuildPrefix(prefix);
        //    var message = await ReplyAsync($"Prefix `{prefix}` was set.");
        //    new Timer((s) => message.DeleteAsync(), null, 5000, Timeout.Infinite);
        //}

        //[Name("Prefix"), Command("prefix"), Alias("pref")]
        //[Summary("Displays the current prefix on the server")]
        //public async Task PrefixAsync()
        //{
        //    await ReplyAsync($"{Context.User.Mention} current prefix: `{_data.Config.Prefix}`");
        //}

        public SettingsModule()
        {

        }

        [Name("Ping"), Command("ping"), Alias("p")]
        [Summary("Displays ping")]
        public async Task PingAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();   
            var mes = await ReplyAsync($"ты пидор");
//            await Context.Channel.GetMessageAsync(mes.Id);
            sw.Stop();
            await mes.ModifyAsync((p) =>
             {
                 p.Content = $"Ping: {sw.ElapsedMilliseconds}ms";
             });
        }
    }
}
