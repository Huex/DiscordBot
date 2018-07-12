using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;

namespace DiscordBot.Core
{
    public class CommandBundle
    {
        public ServiceCollection Services { get; set; } = new ServiceCollection();
        public Collection<Type> Modules { get; set; } = new Collection<Type>();
    }
}