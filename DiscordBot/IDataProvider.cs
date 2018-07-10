using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface IDataProvider
    {

        void SetGuildConfig(CommandConfig guildConfig);

        CommandConfig GetGuildConfig(ulong guildId);
    }
}
