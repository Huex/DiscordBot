using Discord.Commands;
using DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DiscordBot.Core.Packets.Settings
{
    public class SettingsPacket : IPacket
    {
        public DiscordEventsHandlers EventHandlers { get; }

        public CommandsHandler GuildCommandsHandler { get; }

        public CommandsHandler DirectCommandsHandler { get; }

        public SettingsPacket()
        {
            GuildCommandsHandler = new CommandsHandler()
            {
                Modules = new Collection<Type>()
                {
                    typeof(SettingsModule)
                }
            };
            EventHandlers = new DiscordEventsHandlers();
            DirectCommandsHandler = new CommandsHandler();
        }
    }
}
