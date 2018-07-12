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
            this.GuildCommands.Services.AddSingleton(myService);
            this.GuildCommands.Modules.Add(typeof(SampleModule));
            this.DMCommands.Modules.Add(typeof(SampleModule));
            this.DMCommands.Services.AddSingleton(myService);
        }
    }
}
