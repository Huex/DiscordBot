using DiscordBot.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot.Services
{
    public class FileManager
    {
        public static readonly Encoding TextEncoding = Encoding.UTF8;

        public const string FILE_TYPE = "json";
        public const string SERVICE_KATALOGNAME = "data";
        public const string PATH_SEPARATOR = "/";
        public const string FILE_TYPE_SEPARATOR = ".";

        protected static T ReadJson<T>(string path)
        {
            return (T)JsonConvert.DeserializeObject(File.ReadAllText(path, TextEncoding), typeof(T));
        }

        protected static void WriteJson<T>(string path, T obj)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ContractResolver = new IgnoreParentPropertiesResolver(true) }), TextEncoding);
        }
    }
}
