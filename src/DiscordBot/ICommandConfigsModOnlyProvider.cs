using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface ICommandConfigsModOnlyProvider
    {
        IReadOnlyCollection<CommandConfig> Configs { get; }
        void UpdateCommandConfig(ulong id, CommandConfig config);
    }
}
