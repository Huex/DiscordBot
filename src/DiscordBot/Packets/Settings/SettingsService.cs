using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Packets.Settings
{
    public class SettingsService
    {
        public ICommandConfigsModOnlyProvider CommandConfigsProvider { get; set; }

        public ICommandServicesReadOnlyProvider CommandServicesProvider { get; set; }

        internal void SetPrefix(ulong id, string prefix)
        {
            CommandConfigsProvider.UpdateCommandConfig(id, prefix, CommandConfigsProvider.CommandConfigs[id].Modules);
        }

        internal string GetPrefix(ulong id)
        {
            return CommandConfigsProvider.CommandConfigs[id].Prefix;
        }

        internal List<string> GetModules(ulong id)
        {
            return new List<string>(CommandConfigsProvider.CommandConfigs[id].Modules);
        }
    }
}
