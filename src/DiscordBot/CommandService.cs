using Discord.Commands;
using System;
using System.Threading.Tasks;

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
