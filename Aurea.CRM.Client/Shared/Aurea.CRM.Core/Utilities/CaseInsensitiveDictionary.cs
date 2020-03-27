// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseInsensitiveDictionary.cs" company="Aurea Software Gmbh">
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
//   Implements a case insensitive dictionary
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implements a case insensitive dictionary
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the value.
    /// </typeparam>
    public class CaseInsensitiveDictionary<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaseInsensitiveDictionary{TValue}"/> class.
        /// </summary>
        /// <param name="internalStore">
        /// The internal store.
        /// </param>
        public CaseInsensitiveDictionary(Dictionary<KeyPair, TValue> internalStore)
        {
            this.InternalStore = internalStore;
        }

        /// <summary>
        /// Gets the internal store.
        /// </summary>
        /// <value>
        /// The internal store.
        /// </value>
        protected Dictionary<KeyPair, TValue> InternalStore { get; }

        /// <summary>
        /// Gets or sets the <see cref="TValue"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="TValue"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[string key]
        {
            get
            {
                var existingKey = this.InternalStore.Keys.FirstOrDefault(k => k.CaseIgnoreKey == key.ToLower());
                return existingKey == null ? default(TValue) : this.InternalStore[existingKey];
            }

            set
            {
                var existingKey = this.InternalStore.Keys.FirstOrDefault(k => k.CaseIgnoreKey == key.ToLower())
                                  ?? new KeyPair { OriginalKey = key };

                this.InternalStore[existingKey] = value;
            }
        }

        /// <summary>
        /// Key pair implementation to be used as the key
        /// </summary>
        public class KeyPair
        {
            /// <summary>
            /// Gets the case ignore key.
            /// </summary>
            /// <value>
            /// The case ignore key.
            /// </value>
            public string CaseIgnoreKey => this.OriginalKey?.ToLower();

            /// <summary>
            /// Gets or sets the original key.
            /// </summary>
            /// <value>
            /// The original key.
            /// </value>
            public string OriginalKey { get; set; }
        }
    }
}
