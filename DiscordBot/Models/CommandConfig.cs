using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiscordBot
{
    public struct CommandConfig : IEquatable<CommandConfig>
    {
        public CommandConfig(CommandSource source, string name, ulong id, string prefix, Collection<string> modules)
        {
            Source = source;
            Name = name;
            Id = id;
            Prefix = prefix;
            _modules = modules;
        }

        private readonly Collection<string> _modules;

        public CommandSource Source { get; }
        public string Name { get; }
        public ulong Id { get; }
        public string Prefix { get; }

        public IReadOnlyCollection<string> Modules => _modules;

        public bool Equals(CommandConfig other)
        {
            return (Name == other.Name &&
                Id == other.Id &&
                Prefix == other.Prefix &&
                Modules == other.Modules);
        }
    }
}
