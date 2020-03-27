// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JObjectExtensions.cs" company="Aurea Software Gmbh">
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
//   Defines extensions related to Json objects
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines extensions related to Json objects
    /// </summary>
    public static class JObjectExtensions
    {
        /// <summary>
        /// Parses the array.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="objectFactory">
        /// The object factory.
        /// </param>
        /// <param name="collectionFactory">
        /// The collection factory.
        /// </param>
        /// <returns>
        /// a collection
        /// </returns>
        public static ICollection<object> ParseArray(
            this JArray instance,
            Func<IDictionary<string, object>> objectFactory,
            Func<ICollection<object>> collectionFactory)
        {
            var result = collectionFactory != null ? collectionFactory() : new List<object>();

            foreach (var token in instance)
            {
                result.Add(ParseToken(token, objectFactory, collectionFactory));
            }

            return result;
        }

        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <typeparam name="TObject">
        /// The type of the object.
        /// </typeparam>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="TObject"/>.
        /// </returns>
        public static TObject ParseObject<TObject>(this JObject instance)
            where TObject : IDictionary<string, object>, new()
        {
            return ParseObject<TObject, List<object>>(instance);
        }

        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <typeparam name="TObject">
        /// The type of the object.
        /// </typeparam>
        /// <typeparam name="TCollection">
        /// The type of the collection.
        /// </typeparam>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="TObject"/>.
        /// </returns>
        public static TObject ParseObject<TObject, TCollection>(this JObject instance)
            where TObject : IDictionary<string, object>, new() where TCollection : ICollection<object>, new()
        {
            return (TObject)instance?.ParseObject(() => new TObject(), () => new TCollection());
        }

        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="objectFactory">
        /// The object factory.
        /// </param>
        /// <param name="collectionFactory">
        /// The collection factory.
        /// </param>
        /// <returns>
        /// the parsed dictionary
        /// </returns>
        public static IDictionary<string, object> ParseObject(
            this JObject instance,
            Func<IDictionary<string, object>> objectFactory,
            Func<ICollection<object>> collectionFactory)
        {
            var result = objectFactory != null ? objectFactory() : new Dictionary<string, object>();

            foreach (var token in instance)
            {
                result[token.Key] = ParseToken(token.Value, objectFactory, collectionFactory);
            }

            return result;
        }

        /// <summary>
        /// Parses the token.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="objectFactory">
        /// The object factory.
        /// </param>
        /// <param name="collectionFactory">
        /// The collection factory.
        /// </param>
        /// <returns>
        /// the native data type value
        /// </returns>
        public static dynamic ParseToken(
            this JToken token,
            Func<IDictionary<string, object>> objectFactory,
            Func<ICollection<object>> collectionFactory)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return ParseObject(token.ToObject<JObject>(), objectFactory, collectionFactory);
                case JTokenType.Array:
                    return ParseArray(token.ToObject<JArray>(), objectFactory, collectionFactory);
                case JTokenType.Integer:
                    return token.Value<int>();
                case JTokenType.Float:
                    return token.Value<float>();
                case JTokenType.Boolean:
                    return token.Value<bool>();
                case JTokenType.String:
                    return token.ToString();
                case JTokenType.Null:
                    return null;

                /* The following types are not parsed; instead will return the string value
                by using.ToString().
                case JTokenType.TimeSpan:
                case JTokenType.Date:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Property:
                case JTokenType.Constructor:
                case JTokenType.None:
                case JTokenType.Undefined:
                case JTokenType.Comment:
                */
                default:
                    return token.ToString();
            }
        }

        /// <summary>
        /// Converts the string representation of a number to an integer.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ToInt(this object source)
        {
            if (source is JValue)
            {
                source = ((JValue)source)?.Value;
            }

            if (source == null)
            {
                return 0;
            }

            if (source is long)
            {
                return Convert.ToInt32((long)source);
            }

            if (source is int)
            {
                return (int)source;
            }

            if (source is bool)
            {
                return (bool)source ? 1 : 0;
            }

            var val = 0;
            int.TryParse(source.ToString(), out val);
            return val;
        }

        /// <summary>
        /// Converts the string representation of a number to a double.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double ToDouble(this object source)
        {
            if (source == null)
            {
                return 0;
            }

            if (source is long)
            {
                return Convert.ToDouble((long)source, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (source is float)
            {
                return Convert.ToDouble((float)source, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (source is int)
            {
                return Convert.ToDouble((int)source, System.Globalization.CultureInfo.InvariantCulture);
            }

            var val = 0.0;
            double.TryParse(source.ToString(), out val);
            return val;
        }

        /// <summary>
        /// Converts the string representation of a number to an unsigned long.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static ulong ToUInt64(this object source)
        {
            if (source == null)
            {
                return 0;
            }

            if (source is int)
            {
                return Convert.ToUInt64((int)source);
            }

            if (source is ulong)
            {
                return (ulong)source;
            }

            ulong val = 0;
            ulong.TryParse(source.ToString(), out val);
            return val;
        }
    }
}
