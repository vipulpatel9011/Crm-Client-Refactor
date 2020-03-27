// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineStorage.cs" company="Aurea Software Gmbh">
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
//   The Offline Storage class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.Utilities;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Offline Request Type
    /// </summary>
    public enum OfflineRequestType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Records
        /// </summary>
        Records,

        /// <summary>
        /// Settings
        /// </summary>
        Settings,

        /// <summary>
        /// Document upload
        /// </summary>
        DocumentUpload
    }

    /// <summary>
    /// Offline Request Process
    /// </summary>
    public enum OfflineRequestProcess
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Edit record
        /// </summary>
        EditRecord,

        /// <summary>
        /// Serial entry
        /// </summary>
        SerialEntry,

        /// <summary>
        /// Serial entry order
        /// </summary>
        SerialEntryOrder,

        /// <summary>
        /// Serial entry POS
        /// </summary>
        SerialEntryPOS,

        /// <summary>
        /// Characteristics
        /// </summary>
        Characteristics,

        /// <summary>
        /// Objectives
        /// </summary>
        Objectives,

        /// <summary>
        /// Document upload
        /// </summary>
        DocumentUpload,

        /// <summary>
        /// Questionnaire
        /// </summary>
        Questionnaire,

        /// <summary>
        /// Copy records
        /// </summary>
        CopyRecords,

        /// <summary>
        /// Delete record
        /// </summary>
        DeleteRecord,

        /// <summary>
        /// Modify record
        /// </summary>
        ModifyRecord
    }

    /// <summary>
    /// Offline Storage Implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    public class UPOfflineStorage : IOfflineStorage
    {
        private UPOfflineRequest blockingRequest;
        private string requestControlKey;
        private bool blockOnlineRecordRequest;
        private ILogger Logger;

        private string databaseFilename;
        private string baseDirectoryPath;
        private int nextId;
        private List<UPOfflineRequest> offlineRequests;
        private int nextRequestIndex;
        private bool syncIsActive;
        //protected ArrayList serverSyncs;
        //protected int nextServerSync;
        private bool trustCachedNumberOfRequestsWithErrors;
        private int numberOfRequestsWithErrors;
        private bool trustCachedNumberOfRequests;
        private int numberOfRequests;
        private bool noCachedRequestNumbers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineStorage"/> class.
        /// </summary>
        /// <param name="baseDirectoryPath">The base directory path.</param>
        /// <param name="configStore">The configuration store.</param>
        public UPOfflineStorage(string baseDirectoryPath, IConfigurationUnitStore configStore)
        {
            this.Logger = SimpleIoc.Default.GetInstance<ILogger>();
            this.baseDirectoryPath = baseDirectoryPath;
            this.databaseFilename = this.GetOfflineStoragePath("offlineDB.sql");
            this.Database = OfflineDatabase.Create(this.databaseFilename);
            this.StoreBeforeRequest = !configStore.ConfigValueIsSet("Disable.85260");
            this.nextId = this.MaxRequestIdFromDatabase();
            this.noCachedRequestNumbers = configStore.ConfigValueIsSet("Disable.79679");
            this.UnblockUpSyncRequests();
        }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPOfflineStorageSyncDelegate TheDelegate { get; set; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public IDatabase Database { get; private set; }

        /// <summary>
        /// Gets or sets the blocking request.
        /// </summary>
        /// <value>
        /// The blocking request.
        /// </value>
        public UPOfflineRequest BlockingRequest
        {
            get
            {
                return this.blockingRequest;
            }

            set
            {
                if (value != null)
                {
                    this.Logger.LogDebug($"UPSync: set blocking request {value.RequestNr} ({value})", LogFlag.LogUpSync);

                    value.BlockExecution = true;
                    this.blockingRequest = value;
                }
                else if (this.blockingRequest != null)
                {
                    this.Logger.LogDebug("UPSync: unblock", LogFlag.LogUpSync);
                    this.blockingRequest.BlockExecution = false;
                    this.blockingRequest = null;
                }
            }
        }

        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <value>
        /// The next identifier.
        /// </value>
        public int NextId => ++this.nextId;

        /// <summary>
        /// Gets or sets the request control key.
        /// </summary>
        /// <value>
        /// The request control key.
        /// </value>
        public string RequestControlKey
        {
            get
            {
                if (this.requestControlKey == null)
                {
                    this.requestControlKey = string.Empty;
                    DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
                    int ret = recordSet.Execute("SELECT requestkey FROM requestControl");
                    if (ret == 0)
                    {
                        if (recordSet.GetRowCount() == 1)
                        {
                            this.requestControlKey = recordSet.GetRow(0).GetColumn(0);
                        }
                    }
                }

                return this.requestControlKey;
            }

            set
            {
                if (value != this.RequestControlKey)
                {
                    this.Database.BeginTransaction();
                    this.Logger.LogDebug($"UPSync-setRequestControlKey {value}", LogFlag.LogUpSync);

                    int ret = this.Database.Execute("DELETE FROM requestControl", null);
                    if (ret == 0)
                    {
                        DatabaseStatement statement = new DatabaseStatement(this.Database);
                        if (statement.Prepare("INSERT INTO requestControl (requestkey, nextrequestnumber) VALUES (?,1)"))
                        {
                            statement.Bind(value);
                            ret = statement.Execute();
                        }
                    }

                    if (ret == 0)
                    {
                        this.Database.Commit();
                        this.requestControlKey = value;
                    }
                    else
                    {
                        this.Database.Rollback();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [block online record request].
        /// </summary>
        /// <value>
        /// <c>true</c> if [block online record request]; otherwise, <c>false</c>.
        /// </value>
        public bool BlockOnlineRecordRequest
        {
            get
            {
                return this.blockOnlineRecordRequest;
            }

            set
            {
                this.blockOnlineRecordRequest = value && !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("System.TimeoutDoNotBlockOnlineRequests");
            }
        }

        /// <summary>
        /// Gets a value indicating whether [store before request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [store before request]; otherwise, <c>false</c>.
        /// </value>
        public bool StoreBeforeRequest { get; private set; }

        /// <summary>
        /// Gets the offline storage path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public string GetOfflineStoragePath(string fileName)
        {
            return Path.Combine(this.baseDirectoryPath, fileName);
        }

        /// <summary>
        /// Gets the default storage.
        /// </summary>
        /// <value>
        /// The default storage.
        /// </value>
        public static IOfflineStorage DefaultStorage => ServerSession.CurrentSession?.OfflineStorage;

        /// <summary>
        /// Maximums the request identifier from database.
        /// </summary>
        /// <returns></returns>
        public int MaxRequestIdFromDatabase()
        {
            int maxRequestId;
            DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
            int ret = recordSet.Execute("SELECT MAX(requestnr) FROM requests");
            if (ret < 0)
            {
                return ret;
            }

            maxRequestId = recordSet.GetRowCount() == 0 ? 0 : recordSet.GetRow(0).GetColumnInt(0);

            recordSet = new DatabaseRecordSet(this.Database);
            ret = recordSet.Execute("SELECT MAX(requestnr) FROM records");
            if (ret < 0)
            {
                return maxRequestId;
            }

            if (recordSet.GetRowCount() > 0)
            {
                int recordMaxRequest = recordSet.GetRow(0).GetColumnInt(0);
                if (recordMaxRequest > maxRequestId)
                {
                    maxRequestId = recordMaxRequest;
                }
            }

            return maxRequestId;
        }

        /// <summary>
        /// Deallocs this instance.
        /// </summary>
        void Dealloc()
        {
            this.Database = null;
        }

        /// <summary>
        /// Traces the table.
        /// </summary>
        /// <param name="name">The name.</param>
        public void TraceTable(string name)
        {
            //DatabaseRecordSet recordSet = new DatabaseRecordSet(Database);
            //char[] buf = new char[8192];
            //sprintf(buf, "SELECT * FROM %s", name);
            //int ret = recordSet.Execute(buf);
            //if (ret == 0)
            //{
            //    uint i, j;
            //    Console.WriteLine("TABLE: %@", name);
            //    buf[0] = 0;
            //    for (i = 0; i < recordSet.GetColumnCount(); i++)
            //    {
            //        if (!i) strcpy(buf, recordSet.GetColumnName(0));
            //        else sprintf(buf, "%s;%s", buf, recordSet.GetColumnName(i));
            //    }

            //    Console.WriteLine("%s", buf);
            //    for (i = 0; i < recordSet.GetRowCount(); i++)
            //    {
            //        const DatabaseRow row = recordSet.GetRow(i);
            //        buf[0] = 0;
            //        for (j = 0; j < recordSet.GetColumnCount(); j++)
            //        {
            //            if (!j) strcpy(buf, row.GetColumn(0));
            //            else sprintf(buf, "%s;%.60s", buf, row.GetColumn(j));
            //        }

            //        Console.WriteLine("%s", buf);
            //    }
            //}
        }

        /// <summary>
        /// Traces the statement parameters.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="parameters">The parameters.</param>
        public void TraceStatementParameters(string statement, List<object> parameters)
        {
            //DatabaseRecordSet recordSet = new DatabaseRecordSet(Database);
            //char[] buf = new char[8192];
            //char next = buf;
            //next += sprintf(buf, "%s", statement.UTF8String()) + 1;
            //const char[] parameterArray = new char[20];
            //uint parameterCount = parameters.Count;
            //if (parameterCount > 0)
            //{
            //    for (uint i = 0; i < parameterCount; i++)
            //    {
            //        parameterArray[i] = next;
            //        next += sprintf(next, "%s", ((string)parameters[i]).UTF8String()) + 1;
            //    }
            //}

            //int ret = recordSet.Execute(buf, (unsigned int)parameterCount, parameterArray);
            //if (ret == 0)
            //{
            //    uint i, j;
            //    Console.WriteLine("TRACESTATEMENT: %@", statement);
            //    buf[0] = 0;
            //    for (i = 0; i < recordSet.GetColumnCount(); i++)
            //    {
            //        if (!i) strcpy(buf, recordSet.GetColumnName(0));
            //        else sprintf(buf, "%s;%s", buf, recordSet.GetColumnName(i));
            //    }

            //    Console.WriteLine("%s", buf);
            //    for (i = 0; i < recordSet.GetRowCount(); i++)
            //    {
            //        const DatabaseRow row = recordSet.GetRow(i);
            //        buf[0] = 0;
            //        for (j = 0; j < recordSet.GetColumnCount(); j++)
            //        {
            //            if (!j) strcpy(buf, row.GetColumn(0));
            //            else sprintf(buf, "%s;%.60s", buf, row.GetColumn(j));
            //        }

            //        Console.WriteLine("%s", buf);
            //    }
            //}
        }

        public string ResultToStringForStatement(string databaseStatement)
        {
            DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
            int ret = recordSet.Execute(databaseStatement);
            StringBuilder resultString = new StringBuilder();
            if (ret == 0)
            {
                for (int i = 0; i < recordSet.GetColumnCount(); i++)
                {
                    resultString.Append(i == 0 ? recordSet.GetColumnName(0) : $";{recordSet.GetColumnName(i)}");
                }

                resultString.AppendLine();
                for (int i = 0; i < recordSet.GetRowCount(); i++)
                {
                    DatabaseRow row = recordSet.GetRow(i);
                    for (int j = 0; j < recordSet.GetColumnCount(); j++)
                    {
                        resultString.Append(j == 0 ? row.GetColumn(0) : $";{row.GetColumn(j)}");
                    }

                    resultString.AppendLine();
                }
            }

            return resultString.ToString();
        }

        /// <summary>
        /// Synchronizes the trace with setting.
        /// </summary>
        /// <param name="traceSetting">The trace setting.</param>
        /// <returns></returns>
        public string SyncTraceWithSetting(string traceSetting)
        {
            return this.ResultToStringForStatement("SELECT * FROM synchistory");
        }

        /// <summary>
        /// Empties the database.
        /// </summary>
        public void EmptyDB()
        {
            this.Database.EmptyTable("recordlinks");
            this.Database.EmptyTable("recordfields");
            this.Database.EmptyTable("records");
            this.Database.EmptyTable("requests");
            this.Database.EmptyTable("documentuploads");
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        public void BeginTransaction()
        {
            this.Database.BeginTransaction();
        }

        public int SaveDocumentUpload(byte[] data, int requestNr, string fileName,
            string mimeType, string recordIdentification, int fieldId, IDatabase database)
        {
            DatabaseStatement statement = new DatabaseStatement(database);
            UPRecordIdentificationMapper mapper = ServerSession.CurrentSession.RecordIdentificationMapper;
            string origRecordIdentification = recordIdentification;
            recordIdentification = mapper.MappedRecordIdentification(recordIdentification);
            if (recordIdentification == origRecordIdentification)
            {
                this.Logger.LogDebug(
                    $"UPSync-saveDocupload request={requestNr}, length={data.Length}, file={fileName}, recordId={recordIdentification}",
                    LogFlag.LogUpSync);
            }
            else
            {
                this.Logger.LogDebug(
                    $"UPSync-saveDocupload request={requestNr}, length={data.Length}, file={fileName}, recordId={recordIdentification}, original={origRecordIdentification}",
                    LogFlag.LogUpSync);
            }

            if (statement.Prepare("INSERT INTO documentuploads (requestnr, data, filename, mimetype, infoareaid, recordid, fieldid) VALUES (?,?,?,?,?,?,?)"))
            {
                string timepart = DateTime.UtcNow.TimeIntervalSinceReferenceDate().ToString().Replace(":", string.Empty).Replace(".", string.Empty);
                string extendedFileName = $"{timepart}_{requestNr}_{fileName}";
                string localPath = this.GetOfflineStoragePath(extendedFileName);
                string databaseValue = $"file://{extendedFileName}";
                SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.SaveFile(localPath, data);
                this.Logger.LogDebug($"Sync request {requestNr}: document {localPath} saved ({data.Length} bytes)", LogFlag.LogUpSync);
                statement.Bind(1, requestNr);
                statement.Bind(2, databaseValue);
                statement.Bind(3, fileName);
                statement.Bind(4, mimeType);
                statement.Bind(5, recordIdentification.InfoAreaId());
                statement.Bind(6, recordIdentification.RecordId());
                statement.Bind(7, fieldId);
                return statement.Execute();
            }

            return -1;
        }

        /// <summary>
        /// Saves the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="recordnr">The recordnr.</param>
        /// <param name="database">The database.</param>
        /// <returns>0 if successfull, 1 otherwise</returns>
        public int SaveRecord(UPCRMRecord record, int requestNr, int recordnr, IDatabase database)
        {
            var statement = new DatabaseStatement(database);
            var recordMapper = ServerSession.CurrentSession.RecordIdentificationMapper;
            var recordIdentification = recordMapper.MappedRecordIdentification(record.RecordIdentification);
            var recordId = recordIdentification.RecordId();

            if (recordIdentification == record.RecordIdentification)
            {
                this.Logger.LogDebug($"UPSync-saveRecord request={requestNr}, recordnr={recordnr}, recordId={recordIdentification}, mode={record.Mode}", LogFlag.LogUpSync);
            }
            else
            {
                this.Logger.LogDebug($"UPSync-saveRecord request={requestNr}, recordnr={recordnr}, recordId={recordIdentification}, mode={record.Mode}, original={record.RecordIdentification}", LogFlag.LogUpSync);
            }

            string recordIdBuffer = null;
            var result = InsertRecord(record, recordId, requestNr, recordnr, statement, out recordIdBuffer);

            result = this.InsertRecordFields(record, requestNr, recordnr, result);

            result = InsertRecordLinks(record, recordMapper, requestNr, recordnr, database, result);

            if (result == 0 && (string.IsNullOrWhiteSpace(recordId) || recordId.StartsWith("newid:") || recordId == "new"))
            {
                record.RecordId = recordIdBuffer;
            }

            return result;
        }

        /// <summary>
        /// Numbers the of uncommitted requests.
        /// </summary>
        /// <returns></returns>
        public int NumberOfUncommittedRequests()
        {
            if (!this.noCachedRequestNumbers && this.trustCachedNumberOfRequests)
            {
                return this.numberOfRequests;
            }

            DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
            int ret = recordSet.Execute("SELECT count(*) FROM requests");
            if (ret == 0)
            {
                ret = recordSet.GetRowCount() != 0 ? recordSet.GetRow(0).GetColumnInt(0) : 0;

                this.numberOfRequests = ret;
                this.trustCachedNumberOfRequests = true;
            }

            return ret;
        }

        /// <summary>
        /// Numbers the of requests with errors.
        /// </summary>
        /// <returns></returns>
        public int NumberOfRequestsWithErrors()
        {
            int _numberOfRequestsWithErrors;
            if (!this.noCachedRequestNumbers && this.trustCachedNumberOfRequestsWithErrors)
            {
                _numberOfRequestsWithErrors = this.numberOfRequestsWithErrors;
            }
            else
            {
                DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
                int ret = recordSet.Execute("SELECT count(*) FROM requests WHERE error IS NOT NULL");
                if (ret == 0)
                {
                    _numberOfRequestsWithErrors = recordSet.GetRowCount() != 0 ? recordSet.GetRow(0).GetColumnInt(0) : 0;

                    this.numberOfRequestsWithErrors = _numberOfRequestsWithErrors;
                    this.trustCachedNumberOfRequestsWithErrors = true;
                }
                else
                {
                    _numberOfRequestsWithErrors = 0;
                }
            }

            if (this.blockingRequest?.RequestNr > 0 && _numberOfRequestsWithErrors > 0)
            {
                return _numberOfRequestsWithErrors - 1;
            }

            return _numberOfRequestsWithErrors;
        }

        /// <summary>
        /// Clears the cached request numbers.
        /// </summary>
        public void ClearCachedRequestNumbers()
        {
            this.trustCachedNumberOfRequests = false;
            this.trustCachedNumberOfRequestsWithErrors = false;
        }

        public void ExecuteNextRequest()
        {
            if (this.offlineRequests == null)
            {
                return;
            }

            while (this.nextRequestIndex < this.offlineRequests.Count)
            {
                var recentMostRequest = this.offlineRequests[this.offlineRequests.Count - this.nextRequestIndex - 1];
                UPOfflineRequest request = recentMostRequest;
                request.LoadFromOfflineStorage();
                if (!request.CanSync())
                {
                    ++this.nextRequestIndex;
                    this.Logger.LogDebug($"UPSync: request {request.RequestNr} cannot be synced ({request})", LogFlag.LogUpSync);
                    continue;
                }

                if (request.NeedsWLANForSync && ServerSession.CurrentSession.ConnectionWatchDog.LastServerReachabilityStatus != ReachabilityStatus.ReachableViaWiFi)
                {
                    ++this.nextRequestIndex;
                    this.Logger.LogDebug($"UPSync: request {request.RequestNr} cannot be synced because no WLAN available ({request})", LogFlag.LogUpSync);
                    continue;
                }

                ++this.nextRequestIndex;
                this.TheDelegate?.OfflineStorageDidProceedToStepNumberOfSteps(this, this.nextRequestIndex, this.offlineRequests.Count);

                this.Logger.LogDebug($"UPSync: syncing request {request.RequestNr} ({request})", LogFlag.LogUpSync);

                if (request.StartSync(this))
                {
                    continue;
                }
            }

            if (this.TheDelegate != null)
            {
                this.offlineRequests = null;
                UPOfflineStorageSyncDelegate _delegate = this.TheDelegate;
                this.TheDelegate = null;
                this.syncIsActive = false;
                if (this.blockingRequest?.RequestNr <= 0)
                {
                    this.blockOnlineRecordRequest = false;
                }
                this.Logger.LogDebug("UPSync finished.", LogFlag.LogUpSync);
                _delegate.OfflineStorageDidFinishWithResult(this, null);
            }
        }

        /// <summary>
        /// Clears all errors.
        /// </summary>
        /// <returns>0 if success, else SQL error nuumber</returns>
        public int ClearAllErrors()
        {
            this.Logger.LogDebug("UPSync: removing all errors", LogFlag.LogUpSync);
            DatabaseStatement statement = new DatabaseStatement(this.Database);
            int ret = statement.Execute("UPDATE requests SET error = NULL, errorcode = 0, errorstack = NULL");
            this.ClearCachedRequestNumbers();
            this.offlineRequests = null;
            return ret;
        }

        /// <summary>
        /// Synchronizes the specified delegate.
        /// </summary>
        /// <param name="_delegate">The delegate.</param>
        /// <returns>true sync was done, false if sync is already in progress</returns>
        public bool Sync(UPOfflineStorageSyncDelegate _delegate)
        {
            lock (this)
            {
                if (this.syncIsActive)
                {
                    return false;
                }

                this.syncIsActive = true;
                this.TheDelegate = _delegate;
            }

            this.offlineRequests = this.OfflineRequests;
            this.nextRequestIndex = 0;
            this.ExecuteNextRequest();
            return true;
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(
            UPOfflineRequest request,
            object data,
            bool online,
            object context,
            Dictionary<string,
            object> result)
        {
            this.ExecuteNextRequest();
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            if (error.IsConnectionOfflineError())
            {
                if (this.TheDelegate != null)
                {
                    this.offlineRequests = null;
                    UPOfflineStorageSyncDelegate _delegate = this.TheDelegate;
                    this.TheDelegate = null;
                    this.syncIsActive = false;
                    _delegate.OfflineStorageDidFailWithError(this, error);
                }

                return;
            }

            this.ExecuteNextRequest();
        }

        /// <summary>
        /// Offlines the request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Requests the type of the with nr type process.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="processType">Type of the process.</param>
        /// <returns></returns>
        public UPOfflineRequest RequestWithNrTypeProcessType(int requestNr, OfflineRequestType requestType, OfflineRequestProcess processType)
        {
            return UPOfflineRequest.RequestWithIdTypeProcesstype(requestNr, requestType, processType);
        }

        /// <summary>
        /// Requests from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPOfflineRequest RequestFromResultRow(DatabaseRow row)
        {
            var col = row.GetColumnInt(0);
            var col1 = row.GetColumn(1);
            var col2 = row.GetColumn(2);

            if (col1 == null || col2 == null)
            {
                return null;
            }

            return this.RequestWithNrTypeProcessType(col,
                (OfflineRequestType)Enum.Parse(typeof(OfflineRequestType), col1),
                (OfflineRequestProcess)Enum.Parse(typeof(OfflineRequestProcess), col2));
        }

        /// <summary>
        /// Unblocks up synchronize requests.
        /// </summary>
        public void UnblockUpSyncRequests()
        {
            DatabaseStatement pStatement = new DatabaseStatement(this.Database);
            pStatement.Execute("UPDATE requests SET error = NULL WHERE error = 'blocked'");
            this.ClearCachedRequestNumbers();
        }

        /// <summary>
        /// Requests the with nr.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        /// <returns></returns>
        public UPOfflineRequest RequestWithNr(int requestNr)
        {
            DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
            if (recordSet.Query.Prepare("SELECT requestnr, requesttype, processtype FROM requests WHERE requestnr = ?"))
            {
                recordSet.Query.Bind(1, requestNr);
                int ret = recordSet.Execute();

                if (ret != 0 || recordSet.GetRowCount() == 0)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            UPOfflineRequest request = this.RequestFromResultRow(recordSet.GetRow(0));
            return request;
        }

        /// <summary>
        /// Offlines the requests.
        /// </summary>
        /// <returns></returns>
        public List<UPOfflineRequest> OfflineRequests
        {
            get
            {
                DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
                int ret = recordSet.Execute("SELECT requestnr, requesttype, processtype FROM requests ORDER BY requestnr");
                if (ret != 0)
                {
                    return null;
                }

                int rowCount = recordSet.GetRowCount();
                List<UPOfflineRequest> offlineRequestArray = new List<UPOfflineRequest>(rowCount);
                for (int i = 0; i < rowCount; i++)
                {
                    offlineRequestArray.Add(this.RequestFromResultRow(recordSet.GetRow(i)));
                }

                return offlineRequestArray;
            }
        }

        /// <summary>
        /// Gets the conflict requests.
        /// </summary>
        /// <value>
        /// The conflict requests.
        /// </value>
        public List<UPOfflineRequest> ConflictRequests
        {
            get
            {
                DatabaseRecordSet recordSet = new DatabaseRecordSet(this.Database);
                int ret = recordSet.Execute("SELECT requestnr, requesttype, processtype FROM requests WHERE error IS NOT NULL ORDER BY requestnr");
                if (ret != 0)
                {
                    return null;
                }

                int rowCount = recordSet.GetRowCount();
                List<UPOfflineRequest> offlineRequestArray = new List<UPOfflineRequest>(rowCount);
                for (int i = 0; i < rowCount; i++)
                {
                    offlineRequestArray.Add(this.RequestFromResultRow(recordSet.GetRow(i)));
                }

                return offlineRequestArray;
            }
        }

        /// <summary>
        /// Offlines the request XML.
        /// </summary>
        /// <returns></returns>
        public string OfflineRequestXml()
        {
            List<UPOfflineRequest> conflictRequests = this.ConflictRequests;
            UPXmlMemoryWriter writer = new UPXmlMemoryWriter();
            writer.WriteElementStart("Requests");
            if (conflictRequests != null)
            {
                foreach (UPOfflineRequest request in conflictRequests)
                {
                    request.Serialize(writer);
                }
            }

            writer.WriteElementEnd();
            return writer.XmlContentString();
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="recreate">if set to <c>true</c> [recreate].</param>
        /// <returns></returns>
        public bool DeleteDatabase(bool recreate)
        {
            //NSFileManager fileManager = NSFileManager.DefaultManager();
            //if (fileManager.FileExistsAtPath(this._databaseFilename))
            //{
            //    if (_database)
            //    {
            //        _database = null;
            //    }

            //    fileManager.RemoveItemAtPathError(this._databaseFilename, null);
            //    if (recreate)
            //    {
            //        Database db = new Database(this._databaseFilename.UTF8String(), log_output);
            //        _database = db;
            //    }
            //}

            return true;
        }

        //private static int log_output(const void db, const char text)
        //{
        //    if (text)
        //    {
        //        //if (UPLogSettings.LogStatements())
        //        //{
        //        //    DDLogCSQL("%08lX: %s", (unsigned long)db, text);

        //        //    #ifdef DEBUG
        //        //        Console.WriteLine("%08lX: %s", (unsigned long)db, text);
        //        //    #endif

        //        //    return 1;
        //        //}

        //        return 0;
        //    }
        //    else
        //    {
        //        return UPLogSettings.LogStatements();
        //    }
        //}

        private static int InsertRecordLinks(UPCRMRecord record, UPRecordIdentificationMapper recordMapper, int requestNr, int recordnr, IDatabase database, int result)
        {
            var links = record.Links;
            if (result == 0 && links?.Count > 0)
            {
                var statement = new DatabaseStatement(database);
                if (statement.Prepare("INSERT INTO recordlinks (requestnr, recordnr, infoareaid, linkid, recordid) VALUES (?,?,?,?,?)"))
                {
                    foreach (var link in links)
                    {
                        statement.Reset();
                        statement.Bind(1, requestNr);
                        statement.Bind(2, recordnr);
                        var linkRecordIdentification = recordMapper.MappedRecordIdentification(link.RecordIdentification);
                        statement.Bind(3, link.InfoAreaId);
                        statement.Bind(4, link.LinkId);
                        statement.Bind(5, linkRecordIdentification.RecordId());
                        result = statement.Execute();
                        if (result != 0)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private static int InsertRecord(UPCRMRecord record, string recordId, int requestNr, int recordnr, DatabaseStatement statement, out string recordIdBuffer)
        {
            recordIdBuffer = null;
            var ret = 0;
            var sql = "INSERT INTO records (requestnr, recordnr, infoareaid, recordid, mode, options) VALUES (?,?,?,?,?,?)";
            if (statement.Prepare(sql))
            {
                string mode = record.Mode;
                if (string.IsNullOrWhiteSpace(recordId) || recordId.StartsWith("newid:") || recordId == "new")
                {
                    if (!string.IsNullOrWhiteSpace(mode) && mode.StartsWith("Sync"))
                    {
                        if (!string.IsNullOrWhiteSpace(recordId))
                        {
                            recordIdBuffer = recordId;
                        }
                    }
                    else
                    {
                        recordIdBuffer = $"new{requestNr:X8}{recordnr:X4}";
                        record.OfflineRecordNumber = ((requestNr & 65535) << 16) + recordnr;
                        record.OfflineStationNumber = Convert.ToInt32(ConfigurationUnitStore.DefaultStore.ConfigValueDefaultValue("System.OfflineStationNumber", "0"));
                        if (string.IsNullOrWhiteSpace(mode) || !mode.StartsWith("New"))
                        {
                            mode = "New";
                        }
                    }
                }
                else
                {
                    recordIdBuffer = recordId;
                }

                statement.Bind(1, requestNr);
                statement.Bind(2, recordnr);
                statement.Bind(3, record.InfoAreaId);
                statement.Bind(4, recordIdBuffer);
                statement.Bind(5, mode);
                var optionArray = record.OptionArray();
                if (optionArray == null)
                {
                    statement.Bind(6, null);
                }
                else
                {
                    var optionArrayString = StringExtensions.StringFromObject(optionArray);
                    statement.Bind(6, optionArrayString);
                }

                ret = statement.Execute();
            }

            return ret;
        }

        private int InsertRecordFields(UPCRMRecord record, int requestNr, int recordnr, int result)
        {
            var fieldValues = record.FieldValues;
            if (result == 0 && fieldValues?.Count > 0)
            {
                var statement = new DatabaseStatement(this.Database);
                if (statement.Prepare("INSERT INTO recordfields (requestnr, recordnr, fieldid, oldvalue, newvalue, offline) VALUES (?,?,?,?,?,?)"))
                {
                    foreach (var fieldValue in fieldValues)
                    {
                        statement.Reset();
                        statement.Bind(1, requestNr);
                        statement.Bind(2, recordnr);
                        statement.Bind(3, fieldValue.FieldId);
                        statement.Bind(4, fieldValue.OldValue);
                        statement.Bind(5, fieldValue.Value);
                        statement.Bind(6, fieldValue.OnlyOffline ? 1 : 0);

                        result = statement.Execute();
                        if (result != 0)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public void SetSyncIsActive(bool value)
        {
            this.syncIsActive = value;
        }
    }
}
