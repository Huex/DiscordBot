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
            SampleService myService = new SampleService();
            Services.AddSingleton(myService);
            GuildModules.Add(typeof(SampleModule));
            DMModules.Add(typeof(SampleModule));
        }
    }
}
