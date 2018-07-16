using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot.Services
{
    public class FileConfigManager : FileManager
    {
        private readonly List<string> _configs = new List<string>();
        private string KatalogPath => GetKatalogPath(KatalogName);

        public string KatalogName { get; }

        public IReadOnlyList<string> ConfigNames => _configs;

        public FileConfigManager(PacketBase packet) : this(packet.GetType().Name.ToLower()) { }

        public FileConfigManager(string katalogName)
        {
            KatalogName = katalogName;
            CreateKatalogIfNotExsist();
            var pathsFiles = Directory.GetFiles(KatalogPath);
            foreach(var path in pathsFiles)
            {
                if (path.EndsWith(FILE_TYPE))
                {
                    string res = path.Remove(path.Length - FILE_TYPE.Length - FILE_TYPE_SEPARATOR.Length);
                    res = res.Remove(0, KatalogPath.Length + 1);
                    _configs.Add(res);
                }                
            }
        }

        private void CreateKatalogIfNotExsist()
        {
            if (!Directory.Exists(KatalogPath))
            {
                Directory.CreateDirectory(KatalogPath);
            }
        }
    }
}
