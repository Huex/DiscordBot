using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Services
{
    public class FileCommanderConfigManager : FileManager, ICommandConfigsProvider
    {
        private readonly Dictionary<ulong, CommandConfig> _commandConfigs = new Dictionary<ulong, CommandConfig>();

        private string KatalogPath => SERVICE_KATALOGNAME + PATH_SEPARATOR + COMMANDCONFIGS_KATALOGNAME;

        public const string COMMANDCONFIGS_KATALOGNAME = "commands";

        public IReadOnlyDictionary<ulong, CommandConfig> CommandConfigs => _commandConfigs;

        public void AddCommandConfig(CommandConfig config)
        {
            throw new NotImplementedException();
        }

        public void DeleteCommandConfig(ulong id)
        {
            throw new NotImplementedException();
        }

        public void UpdateCommandConfig(ulong id, CommandConfig config)
        {
            throw new NotImplementedException();
        }

        public void UpdateCommandConfig(ulong id, string prefix, IEnumerable<string> modules)
        {
            throw new NotImplementedException();
        }
    }
}
