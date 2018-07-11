using Discord.WebSocket;

namespace DiscordBot.Core
{
    public abstract class PacketBase : LogEntity
    {
        public DiscordSocketClient Discord { get; private set; }
        public DiscordEventsHandlers EventsHandlers { get; } = new DiscordEventsHandlers();
        public CommandBundle GuildCommands { get; } = new CommandBundle();
        public CommandBundle DMCommands { get; } = new CommandBundle();

        internal void SetDiscordSocket(DiscordSocketClient discord)
        {
            Discord = discord;
        }
    }
}
