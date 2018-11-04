using DiscordBot.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DiscordBot.Packets.Sample
{
    public class SamplePacket : PacketBase
    {
        public SamplePacket()
        {
            SampleService myService = new SampleService(); // <- модулям будет передаваться именно этот объект, не измененный
            Services.AddSingleton(myService);              // если myService нужно как-то конфигурировать, то делай это в этом классе, 
            GuildModules.Add(typeof(SampleModule));        // или монжно еще выше по архитектурке, т.е. сюда в конструкторе передавать чото :^)
            DMModules.Add(typeof(SampleModule));

            Initialized += SubscribeOnDiscordEvents;        // во время создания this обьекта Discord еще не существует, 
                                                            // он создается когда DiscordBot проделает все манипуляции и передаст его методом PacketBase.InitPacket
                                                            // я еще не понял как можно по другому *thinkong*
                                                            // так же и передается ConfigsProvider, пока Discord не будет рэди, комманд конфиги серверов, будут пустыми (точнее Counts = 0)
        }

        private async Task ShowCommandConfigsAsync()
        {
            string configs = "";
            foreach (var config in CommandConfigsProvider.CommandConfigs.Values)
            {
                configs += config.Name + " ";
            }
            await Logger.RaiseAsync(new LogMessage(LogSeverity.Critical, "тип лог", $"Есть такие конфиги: {configs}"));
        }

        private async Task Discord_ReadyAsync()
        {
            await Logger.RaiseAsync(new LogMessage(LogSeverity.Critical, "тип лог", "ДАРОВА))"));
        }

        private void SubscribeOnDiscordEvents()
        {
            Discord.Ready += Discord_ReadyAsync;
            Discord.Ready += ShowCommandConfigsAsync;
        }
    }
}
