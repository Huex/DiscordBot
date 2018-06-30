using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using DiscordBot.Services;

namespace DiscordBot
{
    public class GuildDataManager : BotServiceBase
    {
        public readonly Encoding TextEncoding = Encoding.UTF8;

        public GuildConfig Config { get; private set; }

        public GuildDataManager(GuildConfig config)
        {
            Config = config;
        }

        public void SyncConfigWithLocalFile()
        {
            GuildConfig localGuildConfig;
            try
            {
                RaiseLog(LogSeverity.Debug, "Read guild config from file");
                localGuildConfig = FileDataManager.ReadGuildConfig(Config.Id);
            }
            catch (FileNotFoundException ex)
            {
                RaiseLog(LogSeverity.Verbose, null, ex);
                RaiseLog(LogSeverity.Verbose, "Write default guild config to file");
                FileDataManager.WriteGuildConfig(Config);
                return;
            }

            GuildConfigBuilder guildConfigBuilder = new GuildConfigBuilder(Config)
            {
                Modules = new Collection<string>(new List<string>(localGuildConfig.Modules)),
                Prefix = localGuildConfig.Prefix
            };

            Config = guildConfigBuilder.Build();
            RaiseLog(LogSeverity.Debug, "Write guild config to file");
            FileDataManager.WriteGuildConfig(Config);
            RaiseLog(LogSeverity.Info, $"Guild config synced with file ({Config.Name})");
        }

        internal void SetGuildPrefix(string prefix)
        {
            SyncConfigWithLocalFile(); ////!!!!!!!!АЛЯРМ
            GuildConfigBuilder config = new GuildConfigBuilder(Config);
            config.Prefix = prefix;
            Config = config.Build();
            RaiseLog(LogSeverity.Debug, "Write guild config to file");
            FileDataManager.WriteGuildConfig(Config); /////!!!!!!!!!АЛЯРМ
        }

        //private void WriteGuildConfig(string path, GuildConfig guildConfig)
        //{
        //    FileDataManager.WriteGuildConfig(path, guildConfig);
        //    RaiseLog(LogSeverity.Info, $"Guild config are written {path}");
        //}

        //private GuildConfig ReadGuildConfig(string path)
        //{
        //    RaiseLog(LogSeverity.Debug, $"Read guild config {path}");
        //    return FileDataManager.ReadGuildConfig(path);
        //}
    }
}
