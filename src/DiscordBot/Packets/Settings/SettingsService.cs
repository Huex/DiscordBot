using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Packets.Settings
{
    public class SettingsService
    {
        public ICommandConfigsModOnlyProvider CommandProvider { get; set; }

        internal void SetPrefix(ulong id, string prefix)
        {
            CommandProvider.UpdateCommandConfig(id, prefix, CommandProvider.CommandConfigs[id].Modules);
        }

        internal string GetPrefix(ulong id)
        {
            return CommandProvider.CommandConfigs[id].Prefix;
        }

        internal List<string> GetModules(ulong id)
        {
            return new List<string>(CommandProvider.CommandConfigs[id].Modules);
        }
    }
}
