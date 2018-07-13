using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface ICommandConfigsProvider : ICommandConfigsModOnlyProvider
    {
        void AddCommandConfig(CommandConfig config);
    }
}
