using DiscordBot.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace DiscordBot.Services
{
    public class DataManager : ServiceBase
    {
        private IDataProvider _data;

        public DataManager(IDataProvider data)
        {
            _data = data;
        }

        internal void SetGuildPrefix(ulong id, string prefix)
        {
            CommandConfigBuilder config = new CommandConfigBuilder(ReadGuildConfig(id))
            {
                Prefix = prefix
            };
            RaiseLog(LogSeverity.Debug, "Write guild config to file");
            WriteGuildConfig(config.Build());
        }

        private void WriteGuildConfig(CommandConfig guildConfig)
        {
            _data.SetGuildConfig(guildConfig);
            RaiseLog(LogSeverity.Info, $"Guild config are written id = {guildConfig.Id}");
        }

        private CommandConfig ReadGuildConfig(ulong id)
        {
            RaiseLog(LogSeverity.Debug, $"Read guild config id = {id}");
            return _data.GetGuildConfig(id);
        }
    }
}
