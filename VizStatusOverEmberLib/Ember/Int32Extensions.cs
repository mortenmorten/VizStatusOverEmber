namespace VizStatusOverEmberLib.Ember
{
    internal static class Int32Extensions
    {
        public static bool HasBits(this int flags, int bits)
        {
            return (flags & bits) == bits;
        }
    }
}