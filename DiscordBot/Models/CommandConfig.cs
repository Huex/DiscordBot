using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiscordBot
{
    public struct CommandConfig : IEquatable<CommandConfig>
    {
        public CommandConfig(string name, ulong id, string prefix, Collection<string> modules)
        {
            Name = name;
            Id = id;
            Prefix = prefix;
            _modules = modules;
        }

        private readonly Collection<string> _modules;

        public string Name { get; }
        public ulong Id { get; }
        public string Prefix { get; }

        public IReadOnlyCollection<string> Modules
        {
            get
            {
                return _modules;
            }
        }

        public bool Equals(CommandConfig other)
        {
            return (Name == other.Name &&
                Id == other.Id &&
                Prefix == other.Prefix &&
                Modules == other.Modules);
        }
    }
}
