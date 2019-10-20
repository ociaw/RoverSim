using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace RoverSim.BlazorClient
{
    public static class QueryExtensions
    {
        public static Int32 GetFirstPositive(this Dictionary<String, StringValues> query, String key, Int32 fallback)
        {
            Int32 value = GetFirstValue(query, key, fallback);
            if (value < 1)
                return fallback;

            return value;
        }

        public static Int32 GetFirstNonNegative(this Dictionary<String, StringValues> query, String key, Int32 fallback)
        {
            Int32 value = GetFirstValue(query, key, fallback);
            if (value < 0)
                return fallback;

            return value;
        }

        public static Int32 GetFirstValue(this Dictionary<String, StringValues> query, String key, Int32 fallback)
        {
            String str = GetFirstValue(query, key, null);
            if (!Int32.TryParse(str, out Int32 value))
                return fallback;

            return value;
        }

        public static String GetFirstValue(this Dictionary<String, StringValues> query, String key, String fallback)
        {
            if (!query.TryGetValue(key, out StringValues value) || value.Count == 0)
                return fallback;

            return value[0];
        }
    }
}
