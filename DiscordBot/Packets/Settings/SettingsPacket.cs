using DiscordBot.Core;
using DiscordBot.Services;

namespace DiscordBot.Packets.Settings
{
    public class SettingsPacket : Packet
    {
        public SettingsPacket()
        {
            GuildCommands.Modules.Add(typeof(SettingsModule));
            DMCommands.Modules.Add(typeof(SettingsModule));
        }
    }
}
