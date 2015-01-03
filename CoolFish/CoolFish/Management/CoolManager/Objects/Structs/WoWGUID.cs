namespace CoolFishNS.Management.CoolManager.Objects.Structs
{
    public struct WoWGUID
    {
        public ulong firstHalf;
        public ulong secondHalf;

        public override string ToString()
        {
            return "0x" + secondHalf.ToString("X") + firstHalf.ToString("X");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is WoWGUID))
            {
                return false;
            }
            var converted = (WoWGUID) obj;
            return converted.firstHalf.Equals(firstHalf) && converted.secondHalf.Equals(secondHalf);
        }
    }
}