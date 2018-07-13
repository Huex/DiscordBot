﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot
{
    public interface ICommandConfigsProvider
    {
        void UpdateCommandConfig(ulong id, CommandConfig config);
        void CreateCommandConfig(CommandConfig config);
        CommandConfig? GetCommandConfigIfExsist(ulong id);
        ICollection<CommandConfig> GetCommandConfigs();
    }
}