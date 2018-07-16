using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface ICommandConfigsProvider : ICommandConfigsModOnlyProvider
    {
        void UpdateCommandConfig(ulong id, CommandConfig config);
        void AddCommandConfig(CommandConfig config);
        void DeleteCommandConfig(ulong id);
    }
}
