using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiscordBot
{
    public struct GuildConfig
    {
        public GuildConfig(string name, ulong id, string prefix, ulong ownerId, string owner, Collection<string> modules)
        {
            Name = name;
            Id = id;
            Prefix = prefix;
            OwnerId = ownerId;
            Owner = owner;
            _modules = modules;
        }

        private readonly Collection<string> _modules;

        public string Name { get; }
        public ulong Id { get; }
        public string Prefix { get; }
        public ulong OwnerId { get; }
        public string Owner { get; }

        public IReadOnlyCollection<string> Modules
        {
            get
            {
                return _modules;
            }
        }
    }
}
