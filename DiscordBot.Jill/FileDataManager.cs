using DiscordBot.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace DiscordBot.Jill
{
    public class FileDataManager
    {
        public static readonly Encoding TextEncoding = Encoding.UTF8;
        public static readonly string FILE_TYPE = "json";
        private static readonly string GUILDS_PATH = "commander/";

        private static string GetGuildPath(ulong guildId) => GUILDS_PATH + guildId + "." + FILE_TYPE;

        internal static void WriteGuildConfig(CommandConfig config) => WriteCommandConfig(GetGuildPath(config.Id), config);

        internal static CommandConfig ReadCommandConfig(ulong id) => ReadCommanderConfig(GetGuildPath(id));

        internal static BotConfig ReadBotConfig(string path)
        {
            return (BotConfig)JsonConvert.DeserializeObject(File.ReadAllText(path, TextEncoding), typeof(BotConfig));
        }

        internal static void WriteBotConfig(string path, BotConfig botSettings)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(botSettings, new JsonSerializerSettings { ContractResolver = new IgnoreParentPropertiesResolver(true)} ), TextEncoding);
        }

        internal static void WriteCommandConfig(string path, CommandConfig config)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(config), TextEncoding);
        }

        internal static CommandConfig ReadCommanderConfig(string path)
        {
            return ((CommandConfigBuilder)JsonConvert.DeserializeObject(File.ReadAllText(path, TextEncoding), typeof(CommandConfigBuilder))).Build();
        }

        internal static Dictionary<ulong, CommandConfig> ReadAllCommandConfigs()
        {
            var commandsConfigs = new Dictionary<ulong, CommandConfig>();

            var filesPaths = Directory.GetFiles(GUILDS_PATH);
            foreach (var filePath in filesPaths)
            {
                var config = (CommandConfigBuilder)JsonConvert.DeserializeObject(File.ReadAllText(filePath, TextEncoding), typeof(CommandConfigBuilder));
                if (config != null)
                {
                    commandsConfigs.TryAdd(config.Id, config.Build());
                }
            }
            return commandsConfigs;

        }




        //private static List<GuildSettings> _guildsSettings;

        //internal static BotSettings BotSettings { get; private set; }

        //internal ReadOnlyCollection<GuildSettings> GuildsSettings => new ReadOnlyCollection<GuildSettings>(_guildsSettings);

        //public void LoadGuildsSettings()
        //{
        //    var guildsSettings = new List<GuildSettings>();
        //    try
        //    {
        //        var filesPaths = Directory.GetFiles(BotSettings.GuildsSettingsDirectoryPath);
        //        foreach (var filePath in filesPaths)
        //        {
        //            var serverSettings = (GuildSettings)JsonConvert.DeserializeObject(File.ReadAllText(filePath, _textEncoding), typeof(GuildSettings));
        //            if (serverSettings == null)
        //            {
        //                RaiseLog(LogSeverity.Warning, $"Can not read guild settings. Path={filePath}");
        //            }
        //            else
        //            {
        //                guildsSettings.Add(serverSettings);
        //            }
        //        }
        //        _guildsSettings = guildsSettings;
        //        RaiseLog(LogSeverity.Info, $"Guilds settings readed. Path={BotSettings.GuildsSettingsDirectoryPath}");
        //    }
        //    catch (Exception exp)
        //    {
        //        RaiseLog(LogSeverity.Critical, "Error of reading guilds settings", exp);
        //    }
        //}

        //public void AddGuildSettings(GuildSettings guildSettings)
        //{
        //    if(!GuildSettingsExist(guildSettings.GuildId))
        //    {
        //        _guildsSettings.Add(guildSettings);
        //        WriteGuildSettings(guildSettings);
        //        RaiseLog(LogSeverity.Verbose, $"Added new guild settings. GuildId={guildSettings.GuildId}");
        //    }
        //    else
        //    {
        //        RaiseLog(LogSeverity.Warning, $"Guild settings already exist. GuildId={guildSettings.GuildId}");
        //    }
        //}

        //internal void SetServerPrefix(ulong guildId, string prefix)
        //{
        //    var neededServer = _guildsSettings.Find(s => s.GuildId == guildId);
        //    if (neededServer != null)
        //    {
        //        neededServer.Prefix = prefix;
        //        UpdateGuildSettings(neededServer.GuildId, neededServer);
        //    }
        //    else
        //    {
        //        RaiseLog(LogSeverity.Warning, $"Guild settings not exist. GuildId={guildId}");
        //    }
        //}

        //internal void UpdateGuildSettings(ulong GuildId, GuildSettings serverSettings)
        //{
        //    try
        //    {
        //        var neededServer = _guildsSettings.Find(s => s.GuildId == serverSettings.GuildId);
        //        if (neededServer != null)
        //        {
        //            _guildsSettings[_guildsSettings.IndexOf(neededServer)] = serverSettings;
        //            RaiseLog(LogSeverity.Verbose, $"Guild settings update. GuildId={serverSettings.GuildId}");
        //        }
        //        else
        //        {
        //            RaiseLog(LogSeverity.Warning, $"Guild settings not exist. GuildId={serverSettings.GuildId}");
        //            return;
        //        }
        //        WriteGuildSettings(serverSettings);
        //    }
        //    catch (Exception exp)
        //    {
        //        RaiseLog(LogSeverity.Error, $"Guild settings FAIL update. GuildId={serverSettings.GuildId}", exp);
        //    }
        //}

        //internal bool GuildSettingsExist(ulong guildId)
        //{
        //    return _guildsSettings.Find(g => g.GuildId == guildId) != null;
        //}

        //internal GuildSettings GetGuildSettings(ulong guildId)
        //{
        //    return _guildsSettings.Find(g => g.GuildId == guildId);
        //}

        //private void WriteGuildSettings(GuildSettings settings)
        //{
        //    try
        //    {
        //        File.WriteAllText(BotSettings.GuildsSettingsDirectoryPath + settings.GuildId + ".json", JsonConvert.SerializeObject(settings), _textEncoding);
        //        RaiseLog(LogSeverity.Info, $"Guild settings successfully write. GuildId={settings.GuildId}");
        //    }
        //    catch (Exception exp)
        //    {
        //        RaiseLog(LogSeverity.Error, $"Guild settings FAIL write. GuildId={settings.GuildId}", exp);
        //    }
        //}
    }
}
