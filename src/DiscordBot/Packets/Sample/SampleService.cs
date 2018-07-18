using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Packets.Sample
{
    public class SampleService : ServiceBase
    {
        public async Task ToLogAsync(string messgage)
        {
            await Logger.RaiseAsync(new LogMessage(LogSeverity.Critical, this.GetType().Name, $"THIS IS VERY IMPORTATN -> {messgage}"));
        }

        public void WriteToFile(string message)
        {
            File.WriteAllText("SampleFile.txt", message);
        }

        public async Task<string> GetFromFileAsync()
        {
            string res;
            try
            {
                res = File.ReadAllText("SampleFile.txt");
            }
            catch(Exception ex)
            {
                await Logger.RaiseAsync(LogSeverity.Warning, ex.Message);
                return null;
            }
            return res;
        }
    }
}
