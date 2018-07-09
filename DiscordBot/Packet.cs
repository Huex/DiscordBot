using Discord.WebSocket;

namespace DiscordBot.Core
{
    public abstract class Packet : LogEntity
    {
        internal DiscordSocketClient Discord { get; set; }
        public DiscordEventsHandlers EventsHandlers { get; } = new DiscordEventsHandlers();
        public CommandBundle GuildCommands { get; } = new CommandBundle();
        public CommandBundle DMCommands { get; } = new CommandBundle();
    }
}
