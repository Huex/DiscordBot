using DiscordBot.Core;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
        }
    }
}
