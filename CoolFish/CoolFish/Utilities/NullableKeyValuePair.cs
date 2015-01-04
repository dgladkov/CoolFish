using System;

namespace CoolFishNS.Utilities
{
    [Serializable]
    public class NullableKeyValuePair<TK, TV, TVV>
    {
        public NullableKeyValuePair(TK key, TV value, TVV valueTwo)
        {
            Key = key;
            Value = value;
            ValueTwo = valueTwo;
        }

        public TK Key { get; private set; }
        public TV Value { get; private set; }

        public TVV ValueTwo { get; private set; }

        public override string ToString()
        {
            return Key == null ? "null" : Key.ToString();
        }
    }
}