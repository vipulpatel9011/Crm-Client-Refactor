// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSyncReport.cs" company="Aurea Software Gmbh">
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
//   The Sync Report class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Net;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Sync Report
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Session.IRemoteDataTracking" />
    public class UPSyncReport : IRemoteDataTracking
    {
        private long rowId;
        private TimeSpan curTick;
        private TimeSpan requestStartTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncReport"/> class.
        /// </summary>
        /// <param name="syncType">Type of the synchronize.</param>
        /// <param name="details">The details.</param>
        /// <exception cref="Exception">Insert Database failed.</exception>
        public UPSyncReport(string syncType, string details = null)
        {
            this.StartDate = DateTime.Now;
            this.SyncType = syncType;
            this.Details = details;

            if (this.InsertDatabase() != 0)
            {
                throw new Exception("Insert Database failed.");
            }
        }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Gets the type of the synchronize.
        /// </summary>
        /// <value>
        /// The type of the synchronize.
        /// </value>
        public string SyncType { get; private set; }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public string Details { get; private set; }

        /// <summary>
        /// Gets the client runtime.
        /// </summary>
        /// <value>
        /// The runtime client.
        /// </value>
        public TimeSpan RuntimeClient { get; private set; }

        /// <summary>
        /// Gets the runtime server.
        /// </summary>
        /// <value>
        /// The runtime server.
        /// </value>
        public TimeSpan RuntimeServer { get; private set; }

        /// <summary>
        /// Gets the runtime transport.
        /// </summary>
        /// <value>
        /// The runtime transport.
        /// </value>
        public TimeSpan RuntimeTransport { get; private set; }

        /// <summary>
        /// Gets the reported error.
        /// </summary>
        /// <value>
        /// The reported error.
        /// </value>
        public Exception ReportedError { get; private set; }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <value>
        /// The record count.
        /// </value>
        public long RecordCount { get; private set; }

        /// <summary>
        /// Gets the size of the response.
        /// </summary>
        /// <value>
        /// The size of the response.
        /// </value>
        public long ResponseSize { get; private set; }

        /// <summary>
        /// Gets the package count.
        /// </summary>
        /// <value>
        /// The package count.
        /// </value>
        public long PackageCount { get; private set; }

        /// <summary>
        /// Clients the finished with record count.
        /// </summary>
        /// <param name="_recordCount">The record count.</param>
        public void ClientFinishedWithRecordCount(int _recordCount)
        {
            TimeSpan endTick = DateTime.UtcNow.TimeIntervalSince1970();
            this.RuntimeClient = endTick - this.curTick;
            this.curTick = endTick;
            this.RecordCount = _recordCount;
            IDatabase database = UPOfflineStorage.DefaultStorage.Database;
            if (database == null)
            {
                return;
            }

            DatabaseStatement statement = new DatabaseStatement(database);
            if (statement.Prepare("UPDATE synchistory SET runtimeclient = ?, recordCount = ? WHERE rowid = ?"))
            {
                statement.Bind(1, this.RuntimeClient);
                statement.Bind(2, this.RecordCount);
                statement.Bind(3, this.rowId);
                statement.Execute();
            }
        }

        /// <summary>
        /// The Request loaded event.
        /// </summary>
        /// <param name="request">The request.</param>
        public void RequestLoaded(HttpWebRequest request)
        {
            this.curTick = DateTime.UtcNow.TimeIntervalSince1970();
            this.requestStartTick = this.curTick;
        }

        /// <summary>
        /// The Responses received event.
        /// </summary>
        /// <param name="response">The response.</param>
        public void ResponseReceived(HttpWebResponse response)
        {
            TimeSpan endTick = DateTime.UtcNow.TimeIntervalSince1970();
            this.RuntimeServer = endTick - this.requestStartTick;
            this.curTick = endTick;
            IDatabase database = UPOfflineStorage.DefaultStorage.Database;
            if (database == null)
            {
                return;
            }

            DatabaseStatement statement = new DatabaseStatement(database);
            if (statement.Prepare("UPDATE synchistory SET runtimeServer = ? WHERE rowid = ?"))
            {
                statement.Bind(1, this.RuntimeServer);
                statement.Bind(2, this.rowId);
                statement.Execute();
            }
        }

        /// <summary>
        /// On Data received.
        /// </summary>
        /// <param name="data">The data.</param>
        public void DataReceived(byte[] data)
        {
            this.PackageCount++;
            this.ResponseSize += data.Length;
        }

        /// <summary>
        /// On Data receive finished.
        /// </summary>
        public void DataFinished()
        {
            TimeSpan endTick = DateTime.UtcNow.TimeIntervalSince1970();
            this.RuntimeTransport = endTick - this.curTick;
            this.curTick = endTick;
            IDatabase database = UPOfflineStorage.DefaultStorage.Database;
            if (database == null)
            {
                return;
            }

            DatabaseStatement statement = new DatabaseStatement(database);
            if (statement.Prepare("UPDATE synchistory SET runtimetransport = ?, packagecount = ?, responsesize = ? WHERE rowid = ?"))
            {
                statement.Bind(1, this.RuntimeTransport);
                statement.Bind(2, this.PackageCount);
                statement.Bind(3, this.ResponseSize);
                statement.Bind(4, this.rowId);
                statement.Execute();
            }
        }

        /// <summary>
        /// Errors the received.
        /// </summary>
        /// <param name="_error">The error.</param>
        public void ErrorReceived(Exception _error)
        {
            TimeSpan endTick = DateTime.UtcNow.TimeIntervalSince1970();
            this.RuntimeTransport = endTick - this.curTick;
            this.curTick = endTick;
            IDatabase database = UPOfflineStorage.DefaultStorage.Database;
            if (database != null)
            {
                return;
            }

            DatabaseStatement statement = new DatabaseStatement(database);
            this.ReportedError = _error;
            if (statement.Prepare("UPDATE synchistory SET runtimetransport = ?, packagecount = ?, responsesize = ?, errortext = ?, errordetails = ? WHERE rowid = ?"))
            {
                statement.Bind(1, this.RuntimeTransport);
                statement.Bind(2, this.PackageCount);
                statement.Bind(3, this.ResponseSize);
                statement.Bind(4, this.ReportedError.Message);
                statement.Bind(5, this.ReportedError.Source);
                statement.Bind(6, this.rowId);
                statement.Execute();
            }
        }

        /// <summary>
        /// On request cancelation.
        /// </summary>
        public void Canceled()
        {
        }

        /// <summary>
        /// Inserts record in the database.
        /// </summary>
        /// <returns></returns>
        int InsertDatabase()
        {
            IDatabase database = UPOfflineStorage.DefaultStorage.Database;
            if (database == null)
            {
                return 1;
            }

            DatabaseStatement insertStatement = new DatabaseStatement(database);
            int ret = 0;
            if (insertStatement.Prepare("INSERT INTO synchistory (startdate, synctype, details) VALUES (datetime('now'),?,?)"))
            {
                insertStatement.Bind(1, this.SyncType);
                insertStatement.Bind(2, this.Details);

                ret = insertStatement.Execute();
                if (ret == 0)
                {
                    this.rowId = database.ExecuteScalar<int>("SELECT MAX(rowid) FROM synchistory");
                }
            }

            return ret;
        }
    }
}