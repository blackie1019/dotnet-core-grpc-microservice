using System;

namespace MockSite.Common.Core.Utilities
{
    public static class StringExtensions
    {
        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
        }

        public static bool EqualsIgnoreCase(this string str, string compare)
        {
            return str.Equals(compare, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}