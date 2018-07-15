using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface ICommandConfigsModOnlyProvider
    {
        void UpdateCommandConfig(ulong id, string prefix, IEnumerable<string> modules);
        IReadOnlyDictionary<ulong, CommandConfig> CommandConfigs { get; }
    }
}
