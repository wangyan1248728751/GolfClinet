using System;

namespace Golf
{
    public static class WebUtility
    {
        public static string EscapeString(string stringToEscape)
        {
            return Uri.EscapeDataString(stringToEscape);
        }

        public static string UnescapeString(string stringToUnescape)
        {
            return Uri.UnescapeDataString(stringToUnescape);
        }
    }
}
