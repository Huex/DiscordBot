﻿using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VKDiscordBot.Services;

namespace VKDiscordBot.Modules
{
    [Name("Settings")]
    public class SettingsModule : ModuleBase
    {
        private DataManager _data;

        public SettingsModule(DataManager data)
        {
            _data = data;
        }

        [Name("Prefix"), Command("prefix"), Alias("pref")]
        [Summary("Sets the prefix for the bot commands on the current server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PrefixAsync(string prefix)
        {
            _data.SetServerPrefix(Context.Guild, prefix);
            var message = await ReplyAsync("", false, new EmbedBuilder
            {
                Color = Color.Green,
                Description = $"Prefix `{prefix}` installed",
            });
            new Timer((s) => message.DeleteAsync(), null, _data.BotSettings.WaitingBeforeDeleteMessage, Timeout.Infinite);
        }

        [Name("Prefix"), Command("prefix"), Alias("pref")]
        [Summary("Displays the current prefix on the server")]
        public async Task PrefixAsync()
        {
            await ReplyAsync("", false, new EmbedBuilder
            {
                Color = Color.Blue,
                Description = $"Current prefix: `{_data.GetGuildPrefix(Context.Guild)}`",
            });
        }
    }
}
