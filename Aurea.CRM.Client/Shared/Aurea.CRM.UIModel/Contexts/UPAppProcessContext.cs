// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPAppProcessContext.cs" company="Aurea Software Gmbh">
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
//   The App Process Context
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// App Process Context
    /// </summary>
    public class UPAppProcessContext
    {
        private static UPAppProcessContext defaultContext;

        /// <summary>
        /// The context dictionary
        /// </summary>
        private readonly Dictionary<int, Dictionary<string, object>> contextDict;

        /// <summary>
        /// The application context edit field value transfer request
        /// </summary>
        public const string AppContextEditFieldValueTransferRequest = "AppContextEditFieldValueTransferRequest";

        /// <summary>
        /// The application context edit field value transfer response
        /// </summary>
        public const string AppContextEditFieldValueTransferResponse = "AppContextEditFieldValueTransferResponse";

        /// <summary>
        /// Initializes a new instance of the <see cref="UPAppProcessContext"/> class.
        /// </summary>
        public UPAppProcessContext()
        {
            this.contextDict = new Dictionary<int, Dictionary<string, object>>();
        }

        /// <summary>
        /// Gets the current context.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        public static UPAppProcessContext CurrentContext => defaultContext ?? (defaultContext = new UPAppProcessContext());

        /// <summary>
        /// Releases the instance.
        /// </summary>
        public static void ReleaseInstance()
        {
            defaultContext = null;
        }

        /// <summary>
        /// Sets the context value for key.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        /// <param name="nvaControllerKey">The nva controller key.</param>
        public void SetContextValueForKey(object value, string key, int nvaControllerKey)
        {
            lock (this)
            {
                this.DictForNavControllerKey(nvaControllerKey)[key] = value;
            }
        }

        /// <summary>
        /// Contexts the value for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="nvaControllerKey">The nva controller key.</param>
        /// <returns></returns>
        public object ContextValueForKey(string key, int nvaControllerKey)
        {
            lock (this)
            {
                return this.DictForNavControllerKey(nvaControllerKey).ValueOrDefault(key);
            }
        }

        /// <summary>
        /// Gets the or create dictionary context value for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="nvaControllerKey">The nva controller key.</param>
        /// <returns></returns>
        public Dictionary<string, object> GetOrCreateDictionaryContextValueForKey(string key, int nvaControllerKey)
        {
            lock (this)
            {
                Dictionary<string, object> dictionary = this.DictForNavControllerKey(nvaControllerKey).ValueOrDefault(key) as Dictionary<string, object>;
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, object>();
                    this.SetContextValueForKey(dictionary, key, nvaControllerKey);
                }

                return dictionary;
            }
        }

        /// <summary>
        /// Removes all for nav controler identifier.
        /// </summary>
        /// <param name="navControllerKey">The nav controller key.</param>
        public void RemoveAllForNavControlerId(int navControllerKey)
        {
            lock (this)
            {
                this.contextDict.Remove(navControllerKey);
            }
        }

        /// <summary>
        /// Removes all for all nav controller.
        /// </summary>
        public void RemoveAllForAllNavController()
        {
            lock (this)
            {
                this.contextDict.Clear();
            }
        }

        /// <summary>
        /// Dictionaries for nav controller key.
        /// </summary>
        /// <param name="nvaControllerKey">The nva controller key.</param>
        /// <returns></returns>
        public Dictionary<string, object> DictForNavControllerKey(int nvaControllerKey)
        {
            Dictionary<string, object> dictionary = this.contextDict.ValueOrDefault(nvaControllerKey);
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, object>();
                this.contextDict[nvaControllerKey] = dictionary;
            }

            return dictionary;
        }
    }
}
