using System;
using System.Collections.ObjectModel;

namespace DiscordBot.Core
{
    public class CommandBundle
    {
        public Collection<ServiceBase> Services { get; set; } = new Collection<ServiceBase>();
        public Collection<Type> Modules { get; set; } = new Collection<Type>();
    }
}