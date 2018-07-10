using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Utils
{
    public class NotifyDictonary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>
    {
        public NotifyDictonary() : base() { }
        NotifyDictonary(int capacity) : base(capacity) { }

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
