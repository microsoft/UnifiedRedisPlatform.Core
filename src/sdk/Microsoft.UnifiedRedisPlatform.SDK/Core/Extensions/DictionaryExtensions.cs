using System.Linq;
using System.Collections.Generic;

namespace Microsoft.UnifiedRedisPlatform.Core.Extensions
{
    internal static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            return default;
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, bool overwriteExisting = true)
        {
            if (dictionary.ContainsKey(key) && overwriteExisting)
                dictionary[key] = value;
            else if (!dictionary.ContainsKey(key))
                dictionary.Add(key, value);
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> otherDictionary)
        {
            if (otherDictionary.Any())
            {
                foreach (var pair in otherDictionary)
                {
                    dictionary.Add(pair);
                }
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;

            dictionary.Add(key, value);
        }

        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
                return true;
            }

            return false;
        }
    }
}
