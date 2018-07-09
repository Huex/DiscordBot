using DiscordBot.Core;

namespace DiscordBot.Packets.Settings
{
    public class SettingsPacket : Packet
    {
        public SettingsPacket()
        {
            GuildCommands.Modules.Add(typeof(SettingsModule));
        }
    }
}
