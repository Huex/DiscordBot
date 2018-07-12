using DiscordBot.Core;
using DiscordBot.Services;

namespace DiscordBot.Packets.Settings
{
    public class SettingsPacket : PacketBase
    {
        public SettingsPacket()
        {
            DMModules.Add(typeof(SettingsModule));
            GuildModules.Add(typeof(SettingsModule));
        }
    }
}
