// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineRecordRequest.cs" company="Aurea Software Gmbh">
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
//   The Offline Record Request class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.Utilities;

    /// <summary>
    /// Offline Record Request class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRequest" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Data.IChangeDataRequestHandler" />
    public class UPOfflineRecordRequest : UPOfflineRequest, IChangeDataRequestHandler
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRecordRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineRecordRequest(int requestNr = -1)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRecordRequest"/> class.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineRecordRequest(UPOfflineRequestMode requestMode, ViewReference viewReference)
            : base(requestMode, viewReference)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRecordRequest"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineRecordRequest(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// The server requests
        /// </summary>
        private List<UPChangeDataServerOperation> ServerRequests;

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public List<UPCRMRecord> Records { get; protected set; }

        /// <summary>
        /// Gets or sets the records by record identifier.
        /// </summary>
        /// <value>
        /// The records by record identifier.
        /// </value>
        public Dictionary<string, UPCRMRecord> RecordsByRecordId { get; protected set; }

        /// <summary>
        /// Gets or sets the depending on request numbers.
        /// </summary>
        /// <value>
        /// The depending on request numbers.
        /// </value>
        public List<int> DependingOnRequestNumbers { get; protected set; }

        /// <summary>
        /// Gets the next record nr.
        /// </summary>
        /// <value>
        /// The next record nr.
        /// </value>
        public int NextRecordNr { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always set implicit links].
        /// </summary>
        /// <value>
        /// <c>true</c> if [always set implicit links]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysSetImplicitLinks { get; set; }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public override OfflineRequestType RequestType => OfflineRequestType.Records;

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.Unknown;

        /// <summary>
        /// Gets a value indicating whether this instance has XML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has XML; otherwise, <c>false</c>.
        /// </value>
        public override bool HasXml => true;

        /// <summary>
        /// Replaces the record with record in dictionary.
        /// </summary>
        /// <param name="oldrecord">The oldrecord.</param>
        /// <param name="record">The record.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        bool ReplaceRecordWithRecordInDictionary(UPCRMRecord oldrecord, UPCRMRecord record, Dictionary<int, UPCRMRecord> dictionary)
        {
            List<int> keys = dictionary.Keys.Where(currentKey => dictionary[currentKey] == oldrecord).ToList();

            foreach (int key in keys)
            {
                dictionary[key] = record;
            }

            return keys.Count > 0;
        }

        /// <summary>
        /// Merges the records with information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public virtual bool MergeRecordsWithInfoAreaId(string infoAreaId)
        {
            return false;
        }

        /// <summary>
        /// Loads from database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>
        /// True if success, else false
        /// </returns>
        public override bool LoadFromDatabase(IDatabase database)
        {
            if (!base.LoadFromDatabase(database))
            {
                return false;
            }

            var recordsByNr = new Dictionary<int, UPCRMRecord>();
            var recordSet = new DatabaseRecordSet(database);
            var sql =
                "SELECT recordnr, infoareaid, recordid, mode, options FROM records WHERE requestnr = ? ORDER BY recordnr";
            var result = 0;

            if (recordSet.Query.Prepare(sql))
            {
                recordSet.Query.Bind(1, this.RequestNr);
                result = recordSet.Execute();
                if (result == 0)
                {
                    var count = recordSet.GetRowCount();
                    this.Records = new List<UPCRMRecord>(count);
                    this.RecordsByRecordId = new Dictionary<string, UPCRMRecord>(count);

                    for (var index = 0; index < count; index++)
                    {
                        this.ProcessRecord(recordSet, index, recordsByNr);
                    }
                }
            }

            if (result == 0)
            {
                result = this.AddRecordValue(database, result, recordsByNr);
            }

            if (result == 0)
            {
                result = this.AddRecordLinks(database, result, recordsByNr);
            }

            if (result == 0)
            {
                result = this.FillDependingOnRequest(database);
            }

            return result == 0;
        }

        private void ProcessRecord(DatabaseRecordSet recordSet, int i, Dictionary<int, UPCRMRecord> recordsByNr)
        {
            var row = recordSet.GetRow(i);
            var recordNr = row.GetColumnInt(0);
            var infoAreaId = row.GetColumn(1);
            var recordId = row.GetColumn(2);
            var mode = row.GetColumn(3);
            var options = row.IsNull(4) ? null : row.GetColumn(4);

            if (!string.IsNullOrWhiteSpace(recordId))
            {
                var recordIdentification = infoAreaId.InfoAreaIdRecordId(recordId);
                var record = new UPCRMRecord(recordIdentification, mode, options);
                var existingRecord = this.RecordsByRecordId.ValueOrDefault(recordIdentification);
                if (existingRecord != null && (existingRecord.JustSync() || this.MergeRecordsWithInfoAreaId(infoAreaId)))
                {
                    var mergedRecord = existingRecord.RecordByMergingWithRecord(record);
                    if (mergedRecord != existingRecord)
                    {
                        this.Records.Remove(existingRecord);
                        this.Records.Add(mergedRecord);
                        this.RecordsByRecordId[recordIdentification] = mergedRecord;
                        this.ReplaceRecordWithRecordInDictionary(existingRecord, mergedRecord, recordsByNr);
                    }

                    recordsByNr[recordNr] = mergedRecord;
                }
                else
                {
                    this.Records.Add(record);
                    this.RecordsByRecordId[recordIdentification] = record;
                    recordsByNr[recordNr] = record;
                }
            }
            else
            {
                var record = new UPCRMRecord(infoAreaId, string.Empty)
                {
                    Mode = mode
                };
                this.Records.Add(record);
                recordsByNr[recordNr] = record;
            }
        }

        private int AddRecordValue(IDatabase database, int result, Dictionary<int, UPCRMRecord> recordsByNr)
        {
            var recordSet = new DatabaseRecordSet(database);
            const string sql =
                "SELECT recordnr, fieldid, oldvalue, newvalue, offline FROM recordfields WHERE requestnr = ? ORDER BY recordnr, fieldid";
            if (recordSet.Query.Prepare(sql))
            {
                recordSet.Query.Bind(1, this.RequestNr);
                result = recordSet.Execute();

                if (result == 0)
                {
                    var count = recordSet.GetRowCount();
                    for (var index = 0; index < count; index++)
                    {
                        var row = recordSet.GetRow(index);
                        var recordNr = row.GetColumnInt(0);
                        var fieldId = row.GetColumnInt(1);
                        var offline = row.GetColumnInt(4, 0);
                        var oldValue = row.IsNull(2) ? null : row.GetColumn(2);
                        var newValue = row.IsNull(3) ? null : row.GetColumn(3);

                        var record = recordsByNr.ValueOrDefault(recordNr);
                        record?.AddValue(
                            new UPCRMFieldValue(newValue, oldValue, record.InfoAreaId, fieldId, offline != 0));
                    }
                }
            }

            return result;
        }

        private int AddRecordLinks(IDatabase database, int result, Dictionary<int, UPCRMRecord> recordsByNr)
        {
            var recordSet = new DatabaseRecordSet(database);
            const string sql =
                "SELECT recordnr, infoareaid, linkid, recordid FROM recordlinks WHERE requestnr = ? ORDER BY recordnr";
            if (recordSet.Query.Prepare(sql))
            {
                recordSet.Query.Bind(1, this.RequestNr);
                result = recordSet.Execute();
                if (result == 0)
                {
                    var count = recordSet.GetRowCount();
                    for (var index = 0; index < count; index++)
                    {
                        var row = recordSet.GetRow(index);
                        var recordNr = row.GetColumnInt(0);
                        var infoAreaId = row.GetColumn(1);
                        var linkId = row.GetColumnInt(2);
                        var recordId = row.GetColumn(3);
                        var record = recordsByNr.ValueOrDefault(recordNr);
                        record?.AddLink(new UPCRMLink(infoAreaId, recordId, linkId));
                    }
                }
            }

            return result;
        }

        private int FillDependingOnRequest(IDatabase database)
        {
            var recordSet = new DatabaseRecordSet(database);
            const string sql = 
                @"SELECT r.requestnr FROM requests r WHERE r.requestnr < ? AND 
                (EXISTS (SELECT * FROM records rd WHERE rd.requestnr = r.requestnr AND rd.recordid IN 
                (SELECT recordid FROM records WHERE records.requestnr = ? AND (records.mode IS NULL OR records.mode <> 'Sync'))) 
                OR EXISTS (SELECT * FROM records WHERE records.requestnr = r.requestnr AND records.mode <> 'Update' 
                AND (records.mode IS NULL OR records.mode <> 'Sync') AND records.recordid IN 
                (SELECT recordlinks.recordid FROM recordlinks WHERE recordlinks.requestnr = ?))) ORDER BY r.requestnr";

            recordSet.Query.Prepare(sql);
            recordSet.Query.Bind(1, this.RequestNr);
            recordSet.Query.Bind(2, this.RequestNr);
            recordSet.Query.Bind(3, this.RequestNr);
            var result = recordSet.Execute();
            var count = recordSet.GetRowCount();
            if (count > 0)
            {
                this.DependingOnRequestNumbers = new List<int>();
                for (var index = 0; index < count; index++)
                {
                    var row = recordSet.GetRow(index);
                    this.DependingOnRequestNumbers.Add(row.GetColumnInt(0));
                }
            }
            else
            {
                this.DependingOnRequestNumbers = null;
            }

            return result;
        }

        /// <summary>
        /// Adds the record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void AddRecord(UPCRMRecord record)
        {
            UPCRMRecord existingRecord = null;
            if (record.RecordIdentification != null && this.RecordsByRecordId != null)
            {
                existingRecord = this.RecordsByRecordId.ContainsKey(record.RecordIdentification) ? this.RecordsByRecordId[record.RecordIdentification] : null;
            }

            record.ApplyTimeZone(this.TimeZone, this.DataStore);
            if (existingRecord != null && !existingRecord.JustSync() && !this.MergeRecordsWithInfoAreaId(record.InfoAreaId))
            {
                existingRecord = null;
            }

            int offlineStationNumber = Convert.ToInt32(this.Configuration.ConfigValueDefaultValue("System.OfflineStationNumber", "0"));
            if (existingRecord != null)
            {
                UPCRMRecord mergedRecord = existingRecord.RecordByMergingWithRecord(record);
                if (mergedRecord != record)
                {
                    this.Records.Remove(existingRecord);
                    if (this.AlwaysSetImplicitLinks)
                    {
                        UPCRMRecordImplicitOfflineFieldSetter fieldSetter = new UPCRMRecordImplicitOfflineFieldSetter(mergedRecord, offlineStationNumber);
                        fieldSetter.SetImplicitFields(false);
                    }

                    this.Records.Add(mergedRecord);
                    this.RecordsByRecordId.SetObjectForKey(mergedRecord, mergedRecord.RecordIdentification);
                }
            }
            else
            {
                if (this.AlwaysSetImplicitLinks)
                {
                    UPCRMRecordImplicitOfflineFieldSetter fieldSetter = new UPCRMRecordImplicitOfflineFieldSetter(record, offlineStationNumber);
                    fieldSetter.SetImplicitFields(false);
                }

                if (this.Records == null)
                {
                    this.Records = new List<UPCRMRecord> { record };
                    this.RecordsByRecordId = new Dictionary<string, UPCRMRecord>();
                    if (record.RecordIdentification != null)
                    {
                        this.RecordsByRecordId.Add(record.RecordIdentification, record);
                    }
                }
                else
                {
                    this.Records.Add(record);
                    if (record.RecordIdentification != null)
                    {
                        this.RecordsByRecordId[record.RecordIdentification] = record;
                    }
                }
            }
        }

        /// <summary>
        /// Undoes the request.
        /// </summary>
        /// <returns>0, if success, else error number</returns>
        public int UndoRequest()
        {
            return this.UndoRecordsInCache(this.Records, this.DataStore);
        }

        /// <summary>
        /// Undoes the records in cache.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="dataStore">The data store.</param>
        /// <returns>0, if success, else error number</returns>
        protected int UndoRecordsInCache(List<UPCRMRecord> records, ICRMDataStore dataStore)
        {
            int ret = 0;
            if (this.RequestNr >= 0)
            {
                CrmUndoRequest undoRequest = CrmUndoRequest.Create(this.RequestNr);
                if (undoRequest != null)
                {
                    ret = undoRequest.UndoRequest();
                    if (ret == 0)
                    {
                        return 0;
                    }
                }
            }

            foreach (UPCRMRecord record in records)
            {
                ret = record.HandleUndoChangeRecordLocalRequestNr(dataStore, this.RequestNr);
                if (ret != 0)
                {
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Recordses to cache with rollback.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="dataStore">The data store.</param>
        /// <param name="withRollback">if set to <c>true</c> [with rollback].</param>
        /// <returns>0, if success, else error number</returns>
        public int RecordsToCacheWithRollback(List<UPCRMRecord> records, ICRMDataStore dataStore, bool withRollback)
        {
            int ret = 0;
            CrmUndoRequest undoRequest = null;
            if (withRollback)
            {
                undoRequest = new CrmUndoRequest(this.RequestNr, records);
                ret = undoRequest.CheckBeforeCacheSave();
            }

            int offlineStationNumber = Convert.ToInt32(this.Configuration.ConfigValueDefaultValue("System.OfflineStationNumber", "0"));
            foreach (UPCRMRecord record in records)
            {
                if (!this.AlwaysSetImplicitLinks)
                {
                    UPCRMRecordImplicitOfflineFieldSetter fieldSetter = new UPCRMRecordImplicitOfflineFieldSetter(record, offlineStationNumber);
                    fieldSetter.SetImplicitFields(true);
                }

                ret = record.HandleChangeRecordLocalRequestNr(dataStore, withRollback ? this.RequestNr : -1);
                if (ret != 0)
                {
                    break;
                }
            }

            if (ret == 0 && undoRequest != null)
            {
                ret = undoRequest.CheckAfterCacheSave();
                if (ret == 0)
                {
                    undoRequest.Save();
                }
            }

            return ret;
        }

        /// <summary>
        /// Stores the records.
        /// </summary>
        /// <returns>0, if success, else error number</returns>
        public int StoreRecords()
        {
            return this.StoreRecords(this.Records);
        }

        /// <summary>
        /// Stores the records.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <returns>0, if success, else error number</returns>
        public int StoreRecords(List<UPCRMRecord> records)
        {
            IDatabase database = this.Storage.Database;

            if (!database.IsInTransaction)
            {
                database.BeginTransaction();
            }

            int ret = 0;
            int storeNextRecordNr = this.NextRecordNr;
            if (this.RequestNr == -1)
            {
                if (!this.CreateRequest(database))
                {
                    database.Rollback();
                    return 1;
                }

                this.NextRecordNr = 1;
            }
            else if (!this.HasRequestHeaderForRequestId(this.RequestNr, database))
            {
                if (this.SaveRequestRoot(database) != this.RequestNr)
                {
                    database.Rollback();
                    return 1;
                }
            }

            foreach (UPCRMRecord record in records)
            {
                if (string.IsNullOrEmpty(record.RecordId) && record.Mode == "Sync")
                {
                    record.RecordId = $"newid:{this.NextRecordNr}-{this.RequestNr}";
                }

                ret = this.Storage.SaveRecord(record, this.RequestNr, this.NextRecordNr++, database);
                if (ret != 0)
                {
                    break;
                }
            }

            if (ret == 0)
            {
                database.Commit();
            }
            else
            {
                this.NextRecordNr = storeNextRecordNr;
                database.Rollback();
            }

            return ret;
        }

        /// <summary>
        /// Changes the record request with delegate records.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="records">The records.</param>
        /// <returns></returns>
        private UPChangeDataServerOperationOffline ChangeRecordRequestWithDelegateRecords(UPOfflineRequestDelegate theDelegate, List<UPCRMRecord> records)
        {
            if (this.ServerRequestNumber < 0)
            {
                this.AttachNextServerRequestNumber();
            }

            UPChangeDataServerOperationOffline request = new UPChangeDataServerOperationOffline(records,
                this.Storage.RequestControlKey, this.ServerRequestNumber, this)
            {
                OfflineDelegate = theDelegate,
                RequestMode = this.RequestMode,
                ErrorTranslationKey = this.ErrorTranslationKey
            };

            return request;
        }

        /// <summary>
        /// Starts the change record request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void StartChangeRecordRequest(UPChangeDataServerOperationOffline request)
        {
            if (this.ServerRequests == null)
            {
                this.ServerRequests = new List<UPChangeDataServerOperation>();
            }

            this.ServerRequests.Add(request);

            ServerSession.CurrentSession?.ExecuteRequest(request);
        }

        /// <summary>
        /// Starts the synchronize.
        /// </summary>
        /// <param name="_delegate">The delegate.</param>
        /// <param name="alwaysPerform">if set to <c>true</c> [always perform].</param>
        /// <returns>True, if success, else false</returns>
        public override bool StartSync(UPOfflineRequestDelegate _delegate, bool alwaysPerform)
        {
            if (this.RequestNr < 0 || (!this.ApplicationRequest && !this.Loaded && !this.LoadFromOfflineStorage()))
            {
                return false;
            }

            UPChangeDataServerOperationOffline request = this.ChangeRecordRequestWithDelegateRecords(_delegate, this.Records);
            if (request != null)
            {
                request.AlwaysPerform = alwaysPerform;
                this.StartChangeRecordRequest(request);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts the request.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="records">The records.</param>
        /// <param name="context">The context.</param>
        /// <param name="_theDelegate">The delegate.</param>
        /// <returns>True, if success, else false</returns>
        public bool StartRequest(UPOfflineRequestMode requestMode, List<UPCRMRecord> records, object context, UPOfflineRequestDelegate _theDelegate)
        {
            if (this.AlwaysSetImplicitLinks)
            {
                int offlineStationNumber = Convert.ToInt32(this.Configuration.ConfigValueDefaultValue("System.OfflineStationNumber", "0"));
                foreach (UPCRMRecord rec in records)
                {
                    UPCRMRecordImplicitOfflineFieldSetter fieldSetter = new UPCRMRecordImplicitOfflineFieldSetter(rec, offlineStationNumber);
                    fieldSetter.SetImplicitFields(false);
                }
            }

            UPCRMTimeZone timeZone = this.TimeZone;
            if (timeZone?.NeedsTimeZoneAdjustment ?? false)
            {
                foreach (UPCRMRecord rec in records)
                {
                    rec.ApplyTimeZone(timeZone, this.DataStore);
                }
            }

            if (this.RequestMode == UPOfflineRequestMode.Offline || this.Storage.BlockOnlineRecordRequest)
            {
                if (requestMode == UPOfflineRequestMode.OnlineOnly)
                {
                    return false;
                }

                requestMode = UPOfflineRequestMode.Offline;
            }
            else
            {
                this.requestMode = requestMode;
            }

            if (this.StoreBeforeRequest || requestMode == UPOfflineRequestMode.Offline)
            {
                if (this.RequestNr < 0)     // request from application, dont report sync conflict.
                {
                    this.ApplicationRequest = true;
                }

                if (this.StoreRecords(records) == 0)
                {
                    this.RecordsToCacheWithRollback(records, UPCRMDataStore.DefaultStore, true);
                    if (requestMode == UPOfflineRequestMode.Offline)
                    {
                        _theDelegate.OfflineRequestDidFinishWithResult(this, records, false, context, null);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            UPChangeDataServerOperationOffline changeRecordRequest = this.ChangeRecordRequestWithDelegateRecords(_theDelegate, records);
            changeRecordRequest.Context = context;
            this.StartChangeRecordRequest(changeRecordRequest);
            return true;
        }

        /// <summary>
        /// Called when [pre start multi request].
        /// </summary>
        public override void OnPreStartMultiRequest()
        {
            if (this.StoreBeforeRequest)
            {
                this.ApplicationRequest = true;
                var database = this.Storage.Database;
                database.BeginTransaction();

                if (this.DeleteRecordsFromRequest(database) != 0 && this.StoreRecords(this.Records) == 0)
                {
                    database.Commit();
                    this.RecordsToCacheWithRollback(this.Records, UPCRMDataStore.DefaultStore, true);
                }
                else
                {
                    database.Rollback();
                }
            }
        }

        /// <summary>
        /// Redoes the record operations.
        /// </summary>
        public override void RedoRecordOperations()
        {
            this.RecordsToCacheWithRollback(this.Records, UPCRMDataStore.DefaultStore, false);
        }

        /// <summary>
        /// Starts the request.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="records">The records.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public bool StartRequest(UPOfflineRequestMode requestMode, List<UPCRMRecord> records, UPOfflineRequestDelegate theDelegate)
        {
            return this.StartRequest(requestMode, records, null, theDelegate);
        }

        /// <summary>
        /// Starts the request.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="_theDelegate">The delegate.</param>
        /// <returns>True, if success, else false</returns>
        public override bool StartRequest(UPOfflineRequestMode requestMode, UPOfflineRequestDelegate _theDelegate)
        {
            return this.StartRequest(requestMode, this.Records, _theDelegate);
        }

        /// <summary>
        /// Deletes the records from request.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        private int DeleteRecordsFromRequest(IDatabase database)
        {
            int ret = database.Execute("DELETE FROM recordlinks WHERE requestnr = ?", this.RequestNr);
            if (ret == 0)
            {
                ret = database.Execute("DELETE FROM recordfields WHERE requestnr = ?", this.RequestNr);
            }

            if (ret == 0)
            {
                ret = database.Execute("DELETE FROM records WHERE requestnr = ?", this.RequestNr);
            }

            return ret;
        }

        /// <summary>
        /// Deletes the request children.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        protected override int DeleteRequestChildren(IDatabase database)
        {
            int ret = base.DeleteRequestChildren(database);
            if (ret == 0)
            {
                ret = this.DeleteRecordsFromRequest(database);
            }

            return ret;
        }

        /// <summary>
        /// Reports the successful synchronize.
        /// </summary>
        /// <returns>0, if success, else error number</returns>
        public override int ReportSuccessfulSync()
        {
            return this.ReportSuccessfulSync(null);
        }

        /// <summary>
        /// Reports the successful synchronize.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <returns>0, if success, else error number</returns>
        public int ReportSuccessfulSync(List<UPCRMRecord> records)
        {
            IDatabase database = this.Storage.Database;
            bool isTransaction = false;
            bool hasDependentRequests = this.DependentRequestNumbers()?.Count > 0;
            UPRecordIdentificationMapper recordIdentificationMapper = null;
            int ret = 0;
            List<string> offlineOnlyRecords = this.OfflineRecordsForRequest();
            var dataStore = this.DataStore;
            foreach (UPCRMRecord record in records)
            {
                if (record.OriginalRecordIdentification == record.RecordIdentification)
                {
                    continue;
                }

                if (recordIdentificationMapper == null)
                {
                    recordIdentificationMapper = ServerSession.CurrentSession.RecordIdentificationMapper;
                }

                recordIdentificationMapper.AddNewRecordIdentification(record.RecordIdentification, record.OriginalRecordIdentification);
                if (!isTransaction)
                {
                    database.BeginTransaction();
                    isTransaction = true;
                }

                this.ChangeCachedLinkRecordIdentification(record.RecordIdentification, record.OriginalRecordIdentification, database);
                string recordId = record.OriginalRecordIdentification.RecordId();

                ret = database.Execute(
                    "UPDATE recordlinks SET recordid = ? WHERE infoareaid = ? AND recordid = ? AND requestnr > ?",
                    record.RecordId, record.InfoAreaId, recordId, this.RequestNr);

                if (ret == 0)
                {
                    ret = database.Execute(
                        "UPDATE records SET recordid = ? WHERE infoareaid = ? AND recordid = ? AND requestnr > ?",
                        record.RecordId, record.InfoAreaId, recordId, this.RequestNr);
                }

                if (ret == 0)
                {
                    ret = database.Execute(
                        "UPDATE documentuploads SET recordid = ? WHERE infoareaid = ? AND recordid = ? AND requestnr > ?",
                        record.RecordId, record.InfoAreaId, recordId, this.RequestNr);
                }

                dataStore.DeleteRecordWithIdentification(record.OriginalRecordIdentification);
                if (ret != 0)
                {
                    break;
                }
            }

            if (offlineOnlyRecords != null)
            {
                foreach (string offlineRecordIdentification in offlineOnlyRecords)
                {
                    dataStore.DeleteRecordWithIdentification(offlineRecordIdentification);
                }
            }

            foreach (UPCRMRecord record in records)
            {
                this.RedoSubsequentRequestsForRecordIdentification(record.RecordIdentification, database);
            }

            if (isTransaction)
            {
                database.Commit();
            }

            ret = base.ReportSuccessfulSync();
            if (ret != 0)
            {
                return ret;
            }

            if (hasDependentRequests)
            {
                ServerSession.CurrentSession.SyncManager.PerformUpSync();
            }

            return ret;
        }

        /// <summary>
        /// Changes the cached link record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="oldRecordIdentification">The old record identification.</param>
        /// <param name="database">The database.</param>
        /// <returns>True, if success, else false</returns>
        private bool ChangeCachedLinkRecordIdentification(string recordIdentification, string oldRecordIdentification, IDatabase database)
        {
            if (ServerSession.CurrentSession.ValueIsSet("Disable.83898"))
            {
                return false;
            }
            Logger.LogDebug($"UPSync: Change Links in Cache-DB for request {this.RequestNr} ({oldRecordIdentification}-> {recordIdentification})", LogFlag.LogUpSync);

            string sql = @"SELECT DISTINCT records.infoareaid, recordlinks.linkid, records.recordid FROM records, recordlinks 
                           WHERE records.requestnr = recordlinks.requestnr AND records.recordnr = recordlinks.recordnr 
                           AND records.requestnr > ? AND recordlinks.infoareaid = ? AND recordlinks.recordid = ? 
                           ORDER BY records.infoareaid, recordlinks.linkid";

            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            if (!recordSet.Query.Prepare(sql))
            {
                return false;
            }

            recordSet.Query.Bind(1, this.RequestNr);
            recordSet.Query.Bind(2, oldRecordIdentification.InfoAreaId());
            recordSet.Query.Bind(3, oldRecordIdentification.RecordId());
            int ret = recordSet.Execute();
            if (ret != 0)
            {
                return false;
            }

            int count = recordSet.GetRowCount();
            if (count == 0)
            {
                return true;
            }

            Logger.LogDebug(
                $"UPSync: Change Links for recordIdentification {oldRecordIdentification} to {recordIdentification}",
                LogFlag.LogUpSync);

            List<UPCRMLinksToUpdate> crmLinks = new List<UPCRMLinksToUpdate>();
            var dataStore = this.DataStore;
            string lastRecordInfoArea = null;
            int lastLinkId = 0;
            UPCRMLinksToUpdate linksToUpdate = null;
            for (int i = 0; i < count; i++)
            {
                DatabaseRow row = recordSet.GetRow(i);
                string recordInfoAreaBuffer = row.GetColumn(0);
                int linkId = row.GetColumnInt(1, 0);
                if (lastRecordInfoArea == null || linkId != lastLinkId || lastRecordInfoArea != recordInfoAreaBuffer)
                {
                    linksToUpdate = null;
                    lastRecordInfoArea = recordInfoAreaBuffer;
                    lastLinkId = linkId;
                    UPCRMTableInfo tableInfo = dataStore.TableInfoForInfoArea(recordInfoAreaBuffer);
                    if (tableInfo != null)
                    {
                        UPCRMLinkInfo linkInfo = tableInfo.LinkInfoForTargetInfoAreaIdLinkId(oldRecordIdentification.InfoAreaId(), linkId);
                        if (linkInfo == null)
                        {
                            string physicalInfoAreaId = UPCRMDataStore.DefaultStore.RootInfoAreaIdForInfoAreaId(oldRecordIdentification.InfoAreaId());
                            if (!string.IsNullOrEmpty(physicalInfoAreaId) && physicalInfoAreaId != oldRecordIdentification.InfoAreaId())
                            {
                                linkInfo = tableInfo.LinkInfoForTargetInfoAreaIdLinkId(physicalInfoAreaId, linkId);
                            }
                        }

                        if (linkInfo != null && linkInfo.HasColumn)
                        {
                            linksToUpdate = new UPCRMLinksToUpdate(linkInfo, database);
                            crmLinks.Add(linksToUpdate);
                            Logger.LogDebug($"UPSync: occurrences found in link {linkInfo}", LogFlag.LogUpSync);
                        }
                        else
                        {
                            var logMessage = (linkInfo != null)
                                ? $"UpSync: occurrences found for link {linkInfo} without field"
                                : $"UpSync: occurrences found for invalid link {recordInfoAreaBuffer}->{lastRecordInfoArea}/{linkId}";
                            Logger.LogDebug(logMessage, LogFlag.LogUpSync);
                        }
                    }
                    else
                    {
                        Logger.LogDebug($"UpSync: occurrences found for invalid infoareaid {recordInfoAreaBuffer}/{linkId}", LogFlag.LogUpSync);
                    }
                }

                linksToUpdate?.AddRecordId(row.GetColumn(2));
            }

            if (crmLinks.Count > 0)
            {
                return dataStore.UpdateLinksFromRecordIdToRecordId(crmLinks, oldRecordIdentification.RecordId(), recordIdentification.RecordId());
            }

            return true;
        }

        /// <summary>
        /// Determines whether [has request header for request identifier] [the specified request identifier].
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="database">The database.</param>
        /// <returns>
        ///   <c>true</c> if [has request header for request identifier] [the specified request identifier]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRequestHeaderForRequestId(int requestId, IDatabase database)
        {
            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            if (recordSet.Query.Prepare("SELECT requestnr FROM requests WHERE requestnr = ?"))
            {
                recordSet.Query.Bind(1, requestId);

                int ret = recordSet.Execute();
                if (ret == 0)
                {
                    return recordSet.GetRowCount() > 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Redoes the subsequent requests for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="database">The database.</param>
        /// <returns>True, if success, else false</returns>
        private bool RedoSubsequentRequestsForRecordIdentification(string recordIdentification, IDatabase database)
        {
            bool openedTransaction = false;
            if (ServerSession.CurrentSession.ValueIsSet("Disable.83896"))
            {
                return false;
            }

            var recordSet = database != null ? new DatabaseRecordSet(database) : new DatabaseRecordSet(this.Storage.Database);

            string sql = @"SELECT r.requestnr FROM requests r WHERE r.requestnr > ? AND error IS NULL AND EXISTS 
                          (SELECT * FROM records WHERE records.requestnr = r.requestnr AND infoareaid = ? AND recordid = ?) 
                          ORDER BY r.requestnr";

            if (!recordSet.Query.Prepare(sql))
            {
                return false;
            }

            recordSet.Query.Bind(1, this.RequestNr);
            recordSet.Query.Bind(2, recordIdentification.InfoAreaId());
            recordSet.Query.Bind(3, recordIdentification.RecordId());
            int ret = recordSet.Execute();

            if (ret == 0)
            {
                int count = recordSet.GetRowCount();
                if (count == 0)
                {
                    return true;
                }

                if (database == null)
                {
                    database = this.Storage.Database;
                    database.BeginTransaction();
                    openedTransaction = true;
                }

                var dataStore = count > 0 ? UPCRMDataStore.DefaultStore : null;
                for (int i = 0; i < count; i++)
                {
                    DatabaseRow row = recordSet.GetRow(i);
                    int requestNr = row.GetColumnInt(0);
                    Logger.LogDebug("UPSync: Redo changes of request {requestNr} for record {recordIdentification} of request {this.RequestNr}", LogFlag.LogUpSync);
                    UPOfflineRecordRequest recordRequest = new UPOfflineRecordRequest(requestNr);
                    recordRequest.LoadFromDatabase(database);
                    foreach (UPCRMRecord r in recordRequest.Records)
                    {
                        Logger.LogDebug($"UPSync: Redo: {r}", LogFlag.LogUpSync);
                        if (r.RecordIdentification == recordIdentification)
                        {
                            r.HandleChangeRecordLocalRequestNr(dataStore, -1);
                        }
                    }
                }

                if (openedTransaction)
                {
                    database.Commit();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Changes the data request did finish with result.
        /// </summary>
        /// <param name="request">The sender.</param>
        /// <param name="result">The result.</param>
        public void ChangeDataRequestDidFinishWithResult(UPChangeDataServerOperation request, Dictionary<string, object> result)
        {
            string requestControlKey = result.ValueOrDefault("RequestControlKey") as string;
            if (!string.IsNullOrEmpty(requestControlKey))
            {
                this.Storage.RequestControlKey = requestControlKey;
            }

            this.ClearServerRequestNumber();
            if (this.RequestNr >= 0)
            {
                this.ReportSuccessfulSync(request.Records);
            }

            var offlineRequest = (UPChangeDataServerOperationOffline)request;
            offlineRequest.OfflineDelegate?.OfflineRequestDidFinishWithResult(this, offlineRequest.Records, true, offlineRequest.Context, result);

            this.ServerRequests.Remove(offlineRequest);
        }

        /// <summary>
        /// Change the data request did fail with error handler.
        /// </summary>
        /// <param name="request">The sender.</param>
        /// <param name="error">The error.</param>
        public void ChangeDataRequestDidFailWithError(UPChangeDataServerOperation request, Exception error)
        {
            //TODO:IMPORTANT 
            //if (error.Code() == -24)
            //{
            //    this.UndoRecordsInCache(request.Records, UPCRMDataStore.DefaultStore);
            //    this.DeleteRequest(false);
            //    this.ServerRequests.Remove(request);
            //    return;
            //}

            var offlineRequest = (UPChangeDataServerOperationOffline)request;

            if (offlineRequest.RequestMode != UPOfflineRequestMode.OnlineOnly && error.IsConnectionOfflineError())
            {
                if (this.RequestNr < 0 || this.ApplicationRequest)
                {
                    Logger.LogDebug($"UPSync: storing request {request.RequestNumber}", LogFlag.LogUpSync);
                    bool alreadyStored = this.ApplicationRequest && this.RequestNr >= 0;
                    if (alreadyStored || this.StoreRecords(request.Records) != 0)
                    {
                        if (!alreadyStored)
                        {
                            this.RecordsToCacheWithRollback(request.Records, this.DataStore, true);
                        }

                        this.requestMode = UPOfflineRequestMode.Offline;
                        this.Storage.BlockOnlineRecordRequest = true;
                        offlineRequest.OfflineDelegate?.OfflineRequestDidFinishWithResult(this, request.Records, false, offlineRequest.Context, null);
                    }
                    else
                    {
                        offlineRequest.OfflineDelegate?.OfflineRequestDidFailWithError(this, request.Records, offlineRequest.Context, error);
                    }
                }
            }
            else
            {
                if (!error.IsConnectionOfflineError() && string.IsNullOrEmpty(this.Error) && this.RequestNr >= 0 && !this.ApplicationRequest)
                {
                    this.ReportSyncError(error);
                    this.UndoRecordsInCache(request.Records, this.DataStore);
                }
                else if (this.ApplicationRequest)
                {
                    this.StoreError(error);
                    this.UndoRecordsInCache(request.Records, this.DataStore);
                }

                offlineRequest.OfflineDelegate?.OfflineRequestDidFailWithError(this, request.Records, offlineRequest.Context, error);
            }

            this.ServerRequests.Remove(request);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"requestnr={this.RequestNr}, recordCount={this.Records?.Count}, requestType={this.RequestType}, processType={this.ProcessType}\n";
        }

        /// <summary>
        /// Gets the first record.
        /// </summary>
        /// <value>
        /// The first record.
        /// </value>
        public UPCRMRecord FirstRecord => this.Records?.Count > 0 ? this.Records[0] : null;

        /// <summary>
        /// Recordses the with information area link identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <returns></returns>
        public List<UPCRMRecord> RecordsWithInfoAreaLinkId(string infoAreaId, int linkId)
        {
            if (this.Records == null)
            {
                return null;
            }

            List<UPCRMRecord> recordsWithInfoArea = null;
            foreach (UPCRMRecord record in this.Records)
            {
                if (record.InfoAreaId == infoAreaId)
                {
                    if (linkId >= -1)
                    {
                        if (record.Links.Count < 1)
                        {
                            continue;
                        }

                        UPCRMLink firstLink = record.Links[0];
                        if (firstLink.LinkId != linkId && (firstLink.LinkId != 0 || linkId != -1) && (firstLink.LinkId != -1 || linkId != 0))
                        {
                            continue;
                        }
                    }

                    if (recordsWithInfoArea == null)
                    {
                        recordsWithInfoArea = new List<UPCRMRecord>();
                    }

                    recordsWithInfoArea.Add(record);
                }
            }

            return recordsWithInfoArea;
        }

        /// <summary>
        /// Recordses the with information area.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public List<UPCRMRecord> RecordsWithInfoArea(string infoAreaId)
        {
            return this.RecordsWithInfoAreaLinkId(infoAreaId, -2);
        }

        /// <summary>
        /// Dependents the request numbers.
        /// </summary>
        /// <returns></returns>
        public override List<int> DependentRequestNumbers()
        {
            IDatabase database = this.Storage.Database;
            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            string sql = @"SELECT r.requestnr FROM requests r WHERE r.requestnr > ? 
                           AND (EXISTS (SELECT * FROM records rd WHERE rd.requestnr = r.requestnr AND rd.recordid IN 
                                (SELECT recordid FROM records WHERE records.requestnr = ? AND (records.mode IS NULL OR records.mode <> 'Sync'))) 
                           OR EXISTS (SELECT * FROM recordlinks rl WHERE rl.requestnr = r.requestnr AND rl.recordid IN 
                                (SELECT recordid FROM records WHERE records.requestnr = ? AND records.mode<>'Update' AND records.mode<>'Sync'))) 
                           ORDER BY r.requestnr";

            if (!recordSet.Query.Prepare(sql))
            {
                return null;
            }

            recordSet.Query.Bind(1, this.RequestNr);
            recordSet.Query.Bind(2, this.RequestNr);
            recordSet.Query.Bind(3, this.RequestNr);

            int ret = recordSet.Execute();
            if (ret != 0 || recordSet.GetRowCount() > 0)
            {
                return null;
            }

            int count = recordSet.GetRowCount();
            List<int> array = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                DatabaseRow row = recordSet.GetRow(i);
                array.Add(row.GetColumnInt(0));
            }

            return array;
        }

        /// <summary>
        /// Gets the identifying record identification.
        /// </summary>
        /// <value>
        /// The identifying record identification.
        /// </value>
        public override string IdentifyingRecordIdentification
        {
            get
            {
                if (!this.Loaded)
                {
                    this.LoadFromOfflineStorage();
                }

                return this.FirstRecord.OriginalRecordIdentification;
            }
        }

        /// <summary>
        /// Records the structure.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecordWithHierarchy> RecordStructure()
        {
            List<UPCRMRecordWithHierarchy> rootRecords = new List<UPCRMRecordWithHierarchy>();
            Dictionary<string, UPCRMRecordWithHierarchy> recordDictionary = new Dictionary<string, UPCRMRecordWithHierarchy>();
            foreach (UPCRMRecord sourceRecord in this.Records)
            {
                if (sourceRecord.Mode.StartsWith("Sync"))
                {
                    continue;
                }

                bool isRootRecord = true;
                UPCRMRecordWithHierarchy record = new UPCRMRecordWithHierarchy(sourceRecord);
                foreach (UPCRMLink link in record.Links)
                {
                    UPCRMRecordWithHierarchy parentRecord = recordDictionary.ValueOrDefault(link.Record.RecordIdentification);
                    if (parentRecord == null)
                    {
                        parentRecord = new UPCRMRecordWithHierarchy(link.Record);
                        recordDictionary.SetObjectForKey(parentRecord, parentRecord.RecordIdentification);
                    }
                    else if (isRootRecord && !parentRecord.OnlyLink)
                    {
                        isRootRecord = false;
                    }

                    parentRecord.AddChild(record);
                }

                UPCRMRecordWithHierarchy existingRecord = recordDictionary.ValueOrDefault(record.RecordIdentification);
                if (existingRecord == null || (existingRecord.OnlyLink && isRootRecord))
                {
                    recordDictionary.SetObjectForKey(record, record.RecordIdentification);
                    if (isRootRecord)
                    {
                        rootRecords.Add(record);
                    }
                }
            }

            return rootRecords;
        }

        /// <summary>
        /// Firsts the record with information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public UPCRMRecord FirstRecordWithInfoAreaId(string infoAreaId)
        {
            return this.Records.FirstOrDefault(current => current.InfoAreaId == infoAreaId);
        }

        /// <summary>
        /// Serializes the details.
        /// </summary>
        /// <param name="writer">The writer.</param>
        private void SerializeDetails(UPSerializer writer)
        {
            writer.WriteElementStart("Records");
            foreach (UPCRMRecord record in this.Records)
            {
                record.Serialize(writer);
            }

            writer.WriteElementEnd();
        }
    }
}
