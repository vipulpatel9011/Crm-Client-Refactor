// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDataSet.cs" company="Aurea Software Gmbh">
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
//   Sync dataset
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Common
{
    using System;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Defines the data set to sync
    /// </summary>
    public class UPSyncDataSet
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSet"/> class.
        /// </summary>
        /// <param name="dataSetName">
        /// Name of the data set.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp.
        /// </param>
        public UPSyncDataSet(string dataSetName, string timestamp)
        {
            this.DataSetName = dataSetName;
            this.Timestamp = timestamp;
            this.FullSync = string.IsNullOrEmpty(timestamp);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSet"/> class.
        /// </summary>
        /// <param name="dataSetName">
        /// Name of the data set.
        /// </param>
        public UPSyncDataSet(string dataSetName)
            : this(dataSetName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncDataSet"/> class.
        /// </summary>
        /// <param name="dataSetName">
        /// Name of the data set.
        /// </param>
        /// <param name="incremental">
        /// if set to <c>true</c> [incremental].
        /// </param>
        /// <param name="crmStore">
        /// The CRM store.
        /// </param>
        public UPSyncDataSet(string dataSetName, bool incremental, ICRMDataStore crmStore)
            : this(dataSetName, incremental ? crmStore.LastSyncOfDataset(dataSetName) : null)
        {
        }

        /// <summary>
        /// Gets the name of the data set.
        /// </summary>
        /// <value>
        /// The name of the data set.
        /// </value>
        public string DataSetName { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets a value indicating whether [full synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [full synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool FullSync { get; private set; }

        /// <summary>
        /// Gets or sets the record count.
        /// </summary>
        /// <value>
        /// The record count.
        /// </value>
        public int RecordCount { get; set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; }
    }
}
