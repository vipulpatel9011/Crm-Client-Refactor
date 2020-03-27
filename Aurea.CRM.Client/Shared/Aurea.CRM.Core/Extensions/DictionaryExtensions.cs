// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Extension methods related to dictionaries
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Extensions
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods related to dictionaries
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Appends the specified values to the source dictionary.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="replaceExisting">
        /// if set to <c>true</c> [replace existing] with same key.
        /// </param>
        /// <returns>
        /// appended dictionary
        /// </returns>
        public static Dictionary<TKey, TValue> Append<TKey, TValue>(
            this Dictionary<TKey, TValue> target,
            Dictionary<TKey, TValue> values,
            bool replaceExisting = false)
        {
            if (target == null || values == null || !values.Any())
            {
                return target;
            }

            foreach (var value in values)
            {
                if (!target.ContainsKey(value.Key))
                {
                    target.Add(value.Key, value.Value);
                }
                else if (replaceExisting)
                {
                    target[value.Key] = value.Value;
                }
            }

            return target;
        }

        /// <summary>
        /// Compares two dictionaries for equality
        /// </summary>
        /// <typeparam name="TKey">Key Type</typeparam>
        /// <typeparam name="TValue">Value Type</typeparam>
        /// <param name="first">First Dictionary</param>
        /// <param name="second">Second Dictionary</param>
        /// <param name="valueComparer">Value comparer, default is null</param>
        /// <returns>True if dictionary objects are equal</returns>
        public static bool IsEqualToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second, IEqualityComparer<TValue> valueComparer = null)
        {
            if (first == second)
            {
                return true;
            }

            if ((first == null) || (second == null))
            {
                return false;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;

            foreach (var kvp in first)
            {
                TValue secondValue;
                if (!second.TryGetValue(kvp.Key, out secondValue))
                {
                    return false;
                }

                if (!valueComparer.Equals(kvp.Value, secondValue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes the specified keys from the source dictionary.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// Modified dictionary
        /// </returns>
        public static Dictionary<TKey, TValue> RemoveObjectsForKeys<TKey, TValue>(
            this Dictionary<TKey, TValue> target,
            IEnumerable<TKey> keys)
        {
            if (target == null || keys == null || !keys.Any())
            {
                return target;
            }

            foreach (var key in keys)
            {
                if (!target.ContainsKey(key))
                {
                    target.Remove(key);
                }
            }

            return target;
        }

        /// <summary>
        /// Sets the object for key.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TValue"/>.
        /// </returns>
        public static TValue SetObjectForKey<TKey, TValue>(this Dictionary<TKey, TValue> target, TValue value, TKey key)
        {
            if (target == null)
            {
                return default(TValue);
            }

            target[key] = value;
            return value;
        }

        /// <summary>
        /// Sorteds the keys from key array.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="keyArray">
        /// The key array.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<TKey> SortedKeysFromKeyArray<TKey, TValue>(
            this Dictionary<TKey, TValue> target,
            List<TKey> keyArray) where TValue : IComparable
        {
            keyArray?.Sort(
                (obj1, obj2) =>
                    {
                        var v1 = target.ValueOrDefault(obj1);
                        var v2 = target.ValueOrDefault(obj2);
                        if (v2 == null)
                        {
                            if (v1 == null)
                            {
                                return 0;
                            }

                            return -1;
                        }

                        return v1 == null ? 1 : v1.CompareTo(v2);
                    });

            return keyArray;
        }

        /// <summary>
        /// Converts given String,String Dictionary to a query string.
        /// </summary>
        /// <param name="dict">
        /// Dictionary to convert
        /// </param>
        /// <returns>
        /// Querystring
        /// </returns>
        public static string ToQueryString(this Dictionary<string, string> dict)
        {
            var array = (from key in dict.Keys
                         from value in new[] { dict[key] }
                         select $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}").ToArray();
            return "?" + string.Join("&", array);
        }

        /// <summary>
        /// Compare the given value against the value store at given key.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of the value.
        /// </typeparam>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="checkValue">
        /// The check value.
        /// </param>
        /// <returns>
        /// true if matches;else false
        /// </returns>
        public static bool ValueEquals<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, TValue checkValue)
        {
            return target != null && target.ContainsKey(key) && !Equals(key, null) && Equals(target[key], checkValue);
        }

        /// <summary>
        /// Safely get the value of a given key entry
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// the value if exists; else the dfault
        /// </returns>
        public static TValue ValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key)
        {
            return target != null && key != null && target.ContainsKey(key) ? target[key] : default(TValue);
        }

        /// <summary>
        /// Formats dictionary key value pairs for output
        /// </summary>
        /// <param name="target">Dictionary to format</param>
        /// <typeparam name="TKey">key</typeparam>
        /// <typeparam name="TValue">value</typeparam>
        /// <returns>Formatted string</returns>
        public static string StringFormat<TKey, TValue>(this Dictionary<TKey, TValue> target)
        {
            if (target == null || target.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(";", target.Select(x => $"({x.Key}, {x.Value})").ToArray());
        }

        /// <summary>
        /// ToDictionary
        /// </summary>        
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            return dictionary;
        }
    }
}
