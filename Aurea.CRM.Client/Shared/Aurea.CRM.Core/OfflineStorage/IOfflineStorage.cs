// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOfflineStorage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Jakub Majewski
// </author>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OfflineStorage
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.DAL;

    /// <summary>
    ///   The Offline Storage interface
    /// </summary>
    public interface IOfflineStorage : UPOfflineRequestDelegate
    {
        /// <summary>
        /// Gets the offline storage path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Path to offline storage</returns>
        string GetOfflineStoragePath(string fileName);

        /// <summary>
        /// Maximums the request identifier from database.
        /// </summary>
        /// <returns>Max ID from DB</returns>
        int MaxRequestIdFromDatabase();

        /// <summary>
        /// Traces the table.
        /// </summary>
        /// <param name="name">The name.</param>
        void TraceTable(string name);

        /// <summary>
        /// Traces the statement parameters.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        void TraceStatementParameters(string statement, List<object> parameters);

        string ResultToStringForStatement(string databaseStatement);

        /// <summary>
        /// Synchronizes the trace with setting.
        /// </summary>
        /// <param name="traceSetting">The trace setting.</param>
        /// <returns>Trace setting value</returns>
        string SyncTraceWithSetting(string traceSetting);

        /// <summary>
        /// Empties the database.
        /// </summary>
        void EmptyDB();

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Saves the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="recordnr">The recordnr.</param>
        /// <param name="database">The database.</param>
        /// <returns>Newly saved record</returns>
        int SaveRecord(UPCRMRecord record, int requestNr, int recordnr, IDatabase database);

        /// <summary>
        /// Numbers the of uncommitted requests.
        /// </summary>
        /// <returns>Number of uncommited requests</returns>
        int NumberOfUncommittedRequests();

        /// <summary>
        /// Numbers the of requests with errors.
        /// </summary>
        /// <returns>Number of requests with errors</returns>
        int NumberOfRequestsWithErrors();

        /// <summary>
        /// Clears the cached request numbers.
        /// </summary>
        void ClearCachedRequestNumbers();

        void ExecuteNextRequest();

        /// <summary>
        /// Clears all errors.
        /// </summary>
        /// <returns>Number of cleared errors</returns>
        int ClearAllErrors();

        /// <summary>
        /// Synchronizes the specified delegate.
        /// </summary>
        /// <param name="_delegate">The delegate.</param>
        /// <returns>If sync was successful</returns>
        bool Sync(UPOfflineStorageSyncDelegate _delegate);

        void SetSyncIsActive(bool value);

        /// <summary>
        /// Requests the type of the with nr type process.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="processType">Type of the process.</param>
        /// <returns>Reqeuest with specified number and type</returns>
        UPOfflineRequest RequestWithNrTypeProcessType(int requestNr, OfflineRequestType requestType, OfflineRequestProcess processType);

        /// <summary>
        /// Requests from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>Request with specified result row</returns>
        UPOfflineRequest RequestFromResultRow(DatabaseRow row);

        /// <summary>
        /// Unblocks up synchronize requests.
        /// </summary>
        void UnblockUpSyncRequests();

        /// <summary>
        /// Requests the with nr.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        /// <returns>Request with specified nr</returns>
        UPOfflineRequest RequestWithNr(int requestNr);

        /// <summary>
        /// Offlines the request XML.
        /// </summary>
        /// <returns>XML of the request</returns>
        string OfflineRequestXml();

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="recreate">if set to <c>true</c> [recreate].</param>
        /// <returns>If the operation was successful</returns>
        bool DeleteDatabase(bool recreate);

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        UPOfflineStorageSyncDelegate TheDelegate { get; set; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        IDatabase Database { get; }

        /// <summary>
        /// Gets or sets the blocking request.
        /// </summary>
        /// <value>
        /// The blocking request.
        /// </value>
        UPOfflineRequest BlockingRequest { get; set; }

        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <value>
        /// The next identifier.
        /// </value>
        int NextId { get; }

        /// <summary>
        /// Gets or sets the request control key.
        /// </summary>
        /// <value>
        /// The request control key.
        /// </value>
        string RequestControlKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [block online record request].
        /// </summary>
        /// <value>
        /// <c>true</c> if [block online record request]; otherwise, <c>false</c>.
        /// </value>
        bool BlockOnlineRecordRequest { get; set; }

        /// <summary>
        /// Gets a value indicating whether [store before request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [store before request]; otherwise, <c>false</c>.
        /// </value>
        bool StoreBeforeRequest { get; }

        /// <summary>
        /// Offlines the requests.
        /// </summary>
        /// <returns>All offline requests</returns>
        List<UPOfflineRequest> OfflineRequests { get; }

        /// <summary>
        /// Gets the conflict requests.
        /// </summary>
        /// <value>
        /// The conflict requests.
        /// </value>
        List<UPOfflineRequest> ConflictRequests { get; }

        int SaveDocumentUpload(byte[] data, int requestNr, string fileName,
            string mimeType, string recordIdentification, int fieldId, IDatabase database);
    }
}