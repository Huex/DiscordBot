using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utils
{
    public class NotifyDictonary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>
    {
        public NotifyDictonary() : base() { }
        public NotifyDictonary(int capacity) : base(capacity) { }
        public NotifyDictonary(IDictionary<Tkey, Tvalue> dic) : base(dic) { }

        public event Action<Tkey> ValueUpdated;

        private void OnValueUpdated
            (Tkey key)
        {
            ValueUpdated?.Invoke(key);
        }

        public new Tvalue this[Tkey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
                OnValueUpdated(key);
            }
        }
    }
}
