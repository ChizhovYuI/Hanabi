using System;

namespace Hanabi
{
    internal static class Extensions
    {
        internal static int ParseToInt(this string arg)
        {
            int number;
            if (int.TryParse(arg, out number))
                return number;
            throw new ArgumentException();
        }

        internal static Color ParseToColor(this string arg)
        {
            Color color;
            if (Enum.TryParse(arg, out color))
                return color;
            throw new ArgumentException();
        }
    }
}