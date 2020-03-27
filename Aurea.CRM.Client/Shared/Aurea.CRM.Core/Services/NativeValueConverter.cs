// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeValueConverter.cs" company="Aurea Software Gmbh">
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
//   A JsonConverter parse a given json string into native data types.
//   The implementation is simmilar to NSJSONSerialization on iOS.
//   The top level object will be either a List or dictionary
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A JsonConverter parse a given json string into native data types.
    /// The implementation is simmilar to NSJSONSerialization on iOS.
    /// The top level object will be either a List or dictionary
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class NativeValueConverter : JsonConverter
    {
        /// <summary>
        /// The new collection factory
        /// </summary>
        private readonly Func<ICollection<object>> newCollection;

        /// <summary>
        /// The new object factory
        /// </summary>
        private readonly Func<IDictionary<string, object>> newObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeValueConverter"/> class.
        /// </summary>
        /// <param name="collectionFactory">
        /// The collection factory.
        /// </param>
        /// <param name="objectFactory">
        /// The object factory.
        /// </param>
        public NativeValueConverter(
            Func<ICollection<object>> collectionFactory,
            Func<IDictionary<string, object>> objectFactory)
        {
            this.newCollection = collectionFactory;
            this.newObject = objectFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeValueConverter"/> class.
        /// </summary>
        public NativeValueConverter()
            : this(() => new List<object>(), () => new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanRead => true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWrite => false;

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">
        /// Type of the object.
        /// </param>
        /// <returns>
        /// true
        /// </returns>
        public override bool CanConvert(Type objectType) => true;

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="objectType">
        /// Type of the object.
        /// </param>
        /// <param name="existingValue">
        /// The existing value.
        /// </param>
        /// <param name="serializer">
        /// The serializer.
        /// </param>
        /// <returns>
        /// the parsed value
        /// </returns>
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            try
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        return JObject.Load(reader)?.ParseObject(this.newObject, this.newCollection);
                    case JsonToken.StartArray:
                        return JArray.Load(reader)?.ParseArray(this.newObject, this.newCollection);
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                var logProvider = SimpleIoc.Default.GetInstance<ILogger>();
                logProvider?.LogError(ex);

                return null;
            }
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="serializer">
        /// The serializer.
        /// </param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
