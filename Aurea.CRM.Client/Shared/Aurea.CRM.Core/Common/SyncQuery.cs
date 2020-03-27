// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncQuery.cs" company="Aurea Software Gmbh">
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
//   The synchronization query
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    ///  The synchronization query
    /// </summary>
    public class UPSyncQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncQuery"/> class.
        /// </summary>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        public UPSyncQuery(ICRMDataStore dataStore)
        {
            this.DataStore = dataStore;
        }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Synchronizes the with query definition.
        /// </summary>
        /// <param name="queries">
        /// The queries.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SyncWithQueryDefinition(List<object> queries)
        {
            return 0;
        }
    }
}
