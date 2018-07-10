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

        public override int GetHashCode()
        {
            var hashCode = 1286316124;
            hashCode = hashCode * -1521134295 + EqualityComparer<Collection<string>>.Default.GetHashCode(_modules);
            hashCode = hashCode * -1521134295 + Source.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Prefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<IReadOnlyCollection<string>>.Default.GetHashCode(Modules);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return base.ToString();
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
