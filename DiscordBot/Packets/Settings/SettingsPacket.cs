using DiscordBot.Core;
using DiscordBot.Services;

namespace DiscordBot.Packets.Settings
{
    public class SettingsPacket : PacketBase
    {
        public SettingsPacket()
        {
            GuildCommands.Modules.Add(typeof(SettingsModule));
            DMCommands.Modules.Add(typeof(SettingsModule));
        }
    }
}
