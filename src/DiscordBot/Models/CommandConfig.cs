using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiscordBot
{
    public struct CommandConfig : IEquatable<CommandConfig>
    {
        public CommandConfig(CommandSource source, string name, ulong id, string prefix, IEnumerable<string> modules)
        {
            Source = source;
            Name = name;
            Id = id;
            Prefix = prefix;
            _modules = new List<string>(modules);
        }

        private readonly List<string> _modules;

        public CommandSource Source { get; }
        public string Name { get; }
        public ulong Id { get; }
        public string Prefix { get; }

        public IReadOnlyList<string> Modules => _modules;

        public bool Equals(CommandConfig other)
        {
            return Equals(this, other);
        }

        private static bool Equals(CommandConfig config1, CommandConfig config2)
        {
            return (config1.Name == config2.Name &&
                config1.Id == config2.Id &&
                config1.Prefix == config2.Prefix &&
                config1.Source == config2.Source &&
                config1.Modules == config2.Modules);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CommandConfig config1, CommandConfig config2)
        {
            return Equals(config1, config2);
        }

        public static bool operator !=(CommandConfig config1, CommandConfig config2)
        {
            return !Equals(config1, config2);
        }
    }
}
