using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public interface IPacket
    {
        DiscordEventsHandlers EventHandlers { get; }
        CommandsHandler GuildCommandsHandler { get; }
        CommandsHandler DirectCommandsHandler { get; }
    }
}
