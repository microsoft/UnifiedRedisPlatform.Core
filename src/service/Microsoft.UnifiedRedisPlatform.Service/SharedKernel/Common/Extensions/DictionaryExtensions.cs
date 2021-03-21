//namespace System.Collections.Generic
//{
//    /// <summary>
//    /// Extends and adds additional functionalities to the System.Collections.Generic.Dictionary class
//    /// </summary>
//    public static class DictionaryExtensions
//    {
//        /// <summary>
//        /// Adds or updates (if exists) a key value pair in the Dictionary.
//        /// </summary>
//        /// <typeparam name="TKey">Type of the key</typeparam>
//        /// <typeparam name="TValue">Type of the value</typeparam>
//        /// <param name="dictionary">Current dictionary</param>
//        /// <param name="key">Key</param>
//        /// <param name="value">Value</param>
//        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
//        {
//            if (dictionary.ContainsKey(key))
//                dictionary[key] = value;
//            else
//                dictionary.Add(key, value);
//        }

//        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
//        {
//            if (dictionary.TryGetValue(key, out TValue value))
//                return value;
//            return defaultValue;
//        }
//    }
//}
