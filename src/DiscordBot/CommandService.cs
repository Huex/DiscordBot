using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot
{
    public class CommandService : Discord.Commands.CommandService, IPacketCommandService
    {
        public CommandService() : base() { }

        public CommandService(CommandServiceConfig config) : base(config) { }

        public new event Func<LogMessage, Task> Log
        {
            add
            {
                base.Log += async (arg) =>
                {
                    await value(new LogMessage(arg));
                };
            }
            remove
            {
                base.Log -= async (arg) =>
                {
                    await value(new LogMessage(arg));
                };
            }
        }
    }
}
