using System;
using System.Collections.Generic;

namespace RoverSim.BlazorClient
{
    public static class QueryExtensions
    {
        public static Int32 GetPositive(this Dictionary<String, String> query, String key, Int32 fallback)
        {
            Int32 value = GetValue(query, key, fallback);
            if (value < 1)
                return fallback;

            return value;
        }

        public static Int32 GetNonNegative(this Dictionary<String, String> query, String key, Int32 fallback)
        {
            Int32 value = GetValue(query, key, fallback);
            if (value < 0)
                return fallback;

            return value;
        }

        public static Int32 GetValue(this Dictionary<String, String> query, String key, Int32 fallback)
        {
            String str = GetValue(query, key, null);
            if (!Int32.TryParse(str, out Int32 value))
                return fallback;

            return value;
        }

        public static String GetValue(this Dictionary<String, String> query, String key, String fallback)
        {
            if (query == null || !query.TryGetValue(key, out String value))
                return fallback;

            return value;
        }
    }
}
