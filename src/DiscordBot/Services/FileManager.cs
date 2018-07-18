using DiscordBot.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class FileManager : ILogEntity
    {
        public static readonly Encoding TextEncoding = Encoding.UTF8;

        public const string FILE_TYPE = "json";
        public const string ROOT_KATALOGNAME = "data";
        public const string PATH_SEPARATOR = "/";
        public const string FILE_TYPE_SEPARATOR = ".";

        public event Func<LogMessage, Task> Log;

        protected static T ReadJson<T>(string path)
        {
            return (T)JsonConvert.DeserializeObject(File.ReadAllText(path, TextEncoding), typeof(T));
        }

        protected static void WriteJson<T>(string path, T obj)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ContractResolver = new IgnoreParentPropertiesResolver(true) }), TextEncoding);
        }

        protected string GetKatalogPath(string katalogName)
        {
            return ROOT_KATALOGNAME + PATH_SEPARATOR + katalogName;
        }

        #region Log
        protected void RaiseLog(LogSeverity severity, string message, Exception exception = null)
        {
            RaiseLog(new LogMessage(severity, GetType().Name, message, exception));
        }

        protected void RaiseLog(LogMessage message)
        {
            Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(LogMessage message)
        {
            await Log?.Invoke(message);
        }

        protected async Task RaiseLogAsync(Discord.LogMessage message)
        {
            await Log?.Invoke(new LogMessage(message));
        }
        #endregion
    }
}
