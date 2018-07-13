using DiscordBot.Core;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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

            DiscordInitialized += SubscribeOnDiscordEvents;// во время создания this обьекта Discord еще не существует, 
                                                           // он создается когда DiscordBot проделает все манипуляции и передаст его методом PacketBase.InitPacket
                                                           // я еще не понял как можно по другому *thinkong*
        }

        private Task Discord_Ready()
        {
            RaiseLog(new LogMessage(LogSeverity.Critical, "Я ИЗ ЧЕЧНИ", "ДАРОВА))"));
            return Task.CompletedTask;
        }

        private void SubscribeOnDiscordEvents()
        {
            Discord.Ready += Discord_Ready;          
        }
    }
}
