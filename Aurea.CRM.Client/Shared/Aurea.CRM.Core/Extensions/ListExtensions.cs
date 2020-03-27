// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   List Extensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// List Extensions
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Reverses the enumerator.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Items in reverse order</returns>
        public static IEnumerable<TSource> ReverseEnumerator<TSource>(this IList<TSource> source)
        {
            for (var i = source.Count - 1; i >= 0; --i)
            {
                yield return source[i];
            }
        }

        /// <summary>
        /// Searches in the haystack array for the given needle using the default equality operator and returns the index at which the needle starts.
        /// </summary>
        /// <typeparam name="T">Type of the arrays.</typeparam>
        /// <param name="haystack">Sequence to operate on.</param>
        /// <param name="needle">Sequence to search for.</param>
        /// <returns>Index of the needle within the haystack or -1 if the needle isn't contained.</returns>
        public static IEnumerable<int> IndexOf<T>(this T[] haystack, T[] needle)
        {
            if ((needle != null) && (haystack.Length >= needle.Length))
            {
                for (int l = 0; l < haystack.Length - needle.Length + 1; l++)
                {
                    if (!needle.Where((data, index) => !haystack[l + index].Equals(data)).Any())
                    {
                        yield return l;
                    }
                }
            }
        }

        public static T PopFirst<T>(this IList<T> t)
        {
            T element = t[0];
            t.RemoveAt(0);
            return element;
        }

        public static T Peek<T>(this IList<T> t)
        {
            T element = t[t.Count - 1];           
            return element;
        }

        public static T PeekFirst<T>(this IList<T> t)
        {
            T element = t[0];
            return element;
        }

        public static void PushFirst<T>(this IList<T> t, T element)
        {
            t.Insert(0, element);
        }

        public static T Pop<T>(this IList<T> t)
        {
            T element = t[t.Count - 1];
            t.RemoveAt(t.Count - 1);
            return element;
        }

        public static void Push<T>(this IList<T> t, T element)
        {
            t.Add(element);
        }
    }
}
