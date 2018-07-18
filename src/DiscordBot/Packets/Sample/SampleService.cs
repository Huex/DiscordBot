using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordBot.Packets.Sample
{
    public class SampleService : ServiceBase
    {
        public void ToLog(string messgage)
        {
            RaiseLog(new LogMessage(LogSeverity.Critical, this.GetType().Name, $"THIS IS VERY IMPORTATN -> {messgage}"));
        }

        public void WriteToFile(string message)
        {
            File.WriteAllText("SampleFile.txt", message);
        }

        public string GetFromFile()
        {
            string res;
            try
            {
                res = File.ReadAllText("SampleFile.txt");
            }
            catch(Exception ex)
            {
                RaiseLog(LogSeverity.Warning, ex.Message);
                return null;
            }
            return res;
        }
    }
}
