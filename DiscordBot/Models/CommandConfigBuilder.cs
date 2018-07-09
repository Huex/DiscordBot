﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiscordBot
{
    public class CommandConfigBuilder
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public Collection<string> Modules { get; set; }

        public CommandConfigBuilder() { }

        public CommandConfigBuilder(CommandConfig guildConfig) : 
            this(guildConfig.Name, guildConfig.Id, guildConfig.Prefix, 
                new Collection<string>(new List<string>(guildConfig.Modules))) { }

        public CommandConfigBuilder(string name, ulong id, string prefix, Collection<string> modules) :
            this()
        {
            Name = name;
            Id = id;
            Prefix = prefix;
            Modules = modules;
        }

        public CommandConfig Build()
        {
            return new CommandConfig(Name, Id, Prefix, Modules);
        }
    }
}
