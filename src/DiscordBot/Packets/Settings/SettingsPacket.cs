using DiscordBot.Core;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Packets.Settings
{
    public class SettingsPacket : PacketBase
    {
        public SettingsPacket()
        {
            var service = new SettingsService();
            Services.AddSingleton(service);
            DMModules.Add(typeof(SettingsModule));
            GuildModules.Add(typeof(SettingsModule));
            Initialized += () =>
            {
                service.CommandProvider = CommandConfigsProvider;
            };
        }
    }
}
