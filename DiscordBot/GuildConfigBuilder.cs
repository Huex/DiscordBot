using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiscordBot
{
    public class GuildConfigBuilder
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public ulong OwnerId { get; set; }
        public string Owner { get; set; }
        public Collection<string> Modules { get; set; }

        public GuildConfigBuilder() { }

        public GuildConfigBuilder(GuildConfig guildConfig) : this(guildConfig.Name, guildConfig.Id, guildConfig.Prefix, guildConfig.OwnerId, guildConfig.Owner, new Collection<string>(new List<string>(guildConfig.Modules))) { }

        public GuildConfigBuilder(string name, ulong id, string prefix, ulong ownerId, string owner, Collection<string> modules) : this()
        {
            Name = name;
            Id = id;
            Prefix = prefix;
            OwnerId = ownerId;
            Owner = owner;
            Modules = modules;
        }

        public GuildConfig Build()
        {
            return new GuildConfig(Name, Id, Prefix, OwnerId, Owner, Modules);
        }
    }
}
