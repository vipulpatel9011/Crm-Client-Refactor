// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmUndoRequest.cs" company="Aurea Software Gmbh">
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
//   The CRM Undo Request class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Undo Request implementation
    /// </summary>
    public class CrmUndoRequest
    {
        /// <summary>
        /// The undo record dictionary
        /// </summary>
        protected Dictionary<string, UPCRMUndoRecord> undoRecordDictionary;

        /// <summary>
        /// The undo records
        /// </summary>
        protected List<UPCRMUndoRecord> undoRecords;

        /// <summary>
        /// Gets the CRM records.
        /// </summary>
        /// <value>
        /// The CRM records.
        /// </value>
        public List<UPCRMRecord> CrmRecords { get; private set; }

        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public int RequestId { get; private set; }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmUndoRequest"/> class.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="records">The records.</param>
        public CrmUndoRequest(int requestId, List<UPCRMRecord> records)
        {
            this.RequestId = requestId;
            this.CrmRecords = records;
            this.DataStore = UPCRMDataStore.DefaultStore;
            foreach (UPCRMRecord record in this.CrmRecords)
            {
                this.AddRecord(record);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmUndoRequest"/> class.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        private CrmUndoRequest(int requestId)
        {
            this.RequestId = requestId;
            this.DataStore = UPCRMDataStore.DefaultStore;
            this.Load();
        }

        /// <summary>
        /// Creates the specified request identifier.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <returns></returns>
        public static CrmUndoRequest Create(int requestId)
        {
            var request = new CrmUndoRequest(requestId);
            return request.Load() == 0 ? request : null;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        private int Load()
        {
            CRMDatabase database = this.DataStore.DatabaseInstance;
            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            int ret = recordSet.Execute("SELECT infoareaid, recordid, rollbackinfo FROM rollbackinfo WHERE requestnr = ?", this.RequestId);
            if (ret != 0)
            {
                return ret;
            }

            int rowCount = recordSet.GetRowCount();
            for (int i = 0; i < rowCount; i++)
            {
                DatabaseRow row = recordSet.GetRow(i);
                string recordIdentification = $"{row.GetColumn(0)}.{row.GetColumn(1)}";
                var rollbackinfo = row.GetColumn(2);
                this.AddRecordIdentificationRollbackInfo(recordIdentification, rollbackinfo);
            }

            return 0;
        }

        /// <summary>
        /// Checks the before cache save.
        /// </summary>
        /// <returns></returns>
        public int CheckBeforeCacheSave()
        {
            CRMDatabase database = this.DataStore.DatabaseInstance;
            int ret = 0;
            int count = this.undoRecords.Count;
            for (int i = 0; ret == 0 && i < count; i++)
            {
                UPCRMUndoRecord undoRecord = this.undoRecords[i];
                ret = undoRecord.CheckBeforeCacheSaveWithDatabase(database);
            }

            return ret;
        }

        /// <summary>
        /// Checks the after cache save.
        /// </summary>
        /// <returns></returns>
        public int CheckAfterCacheSave()
        {
            CRMDatabase database = this.DataStore.DatabaseInstance;
            int ret = 0;
            int count = this.undoRecords.Count;
            for (int i = 0; ret == 0 && i < count; i++)
            {
                UPCRMUndoRecord undoRecord = this.undoRecords[i];
                ret = undoRecord.CheckAfterCacheSave(database);
            }

            return ret;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            if (this.undoRecords.Count == 0)
            {
                return -1;
            }

            CRMDatabase database = this.DataStore.DatabaseInstance;
            database.BeginTransaction();
            DatabaseStatement statement = new DatabaseStatement(database);
            if (!statement.Prepare("DELETE FROM rollbackinfo WHERE requestnr = ?"))
            {
                return -1;
            }

            statement.Bind(this.RequestId);
            int ret = statement.Execute();
            if (ret != 0)
            {
                return ret;
            }

            statement = new DatabaseStatement(database);
            if (!statement.Prepare("INSERT INTO rollbackinfo (requestnr, infoareaid, recordid, rollbackinfo) VALUES (?,?,?,?)"))
            {
                return -1;
            }

            int recordCount = this.undoRecords.Count;
            for (int i = 0; ret == 0 && i < recordCount; i++)
            {
                UPCRMUndoRecord undoRecord = this.undoRecords[i];
                statement.Reset();
                statement.Bind(1, this.RequestId);
                string infoAreaId = undoRecord.RecordIdentification.InfoAreaId();
                string recordId = undoRecord.RecordIdentification.RecordId();
                string rollbackInfo = undoRecord.RollbackInfo;
                if (string.IsNullOrEmpty(infoAreaId) || string.IsNullOrEmpty(recordId) || string.IsNullOrEmpty(rollbackInfo))
                {
                    continue;
                }

                statement.Bind(2, infoAreaId);
                statement.Bind(3, recordId);
                statement.Bind(4, rollbackInfo);
                ret = statement.Execute();
            }

            if (ret == 0)
            {
                database.Commit();
            }
            else
            {
                database.Rollback();
            }

            return ret;
        }

        /// <summary>
        /// Undoes the request.
        /// </summary>
        /// <returns></returns>
        public int UndoRequest()
        {
            if (this.undoRecords == null || this.undoRecords.Count == 0)
            {
                return -1;
            }

            CRMDatabase database = this.DataStore.DatabaseInstance;
            database.BeginTransaction();
            int ret = 0;
            foreach (UPCRMUndoRecord record in this.undoRecords)
            {
                ret = record.UndoWithTransaction(database);
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
                database.Rollback();
            }

            return ret;
        }

        /// <summary>
        /// Adds the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public UPCRMUndoRecord AddRecord(UPCRMRecord record)
        {
            UPCRMUndoRecord undoRecord = this.undoRecordDictionary.ValueOrDefault(record.RecordIdentification);
            if (undoRecord == null)
            {
                undoRecord = new UPCRMUndoRecord(record, this);
                if (this.undoRecordDictionary == null)
                {
                    this.undoRecordDictionary = new Dictionary<string, UPCRMUndoRecord> { { undoRecord.RecordIdentification, undoRecord } };
                    this.undoRecords = new List<UPCRMUndoRecord> { undoRecord };
                }
                else
                {
                    this.undoRecordDictionary[undoRecord.RecordIdentification] = undoRecord;
                    this.undoRecords.Add(undoRecord);
                }
            }
            else
            {
                undoRecord.ApplyChangesFromRecord(record);
            }

            return undoRecord;
        }

        /// <summary>
        /// Adds the record identification rollback information.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="rollbackInfo">The rollback information.</param>
        /// <returns></returns>
        public UPCRMUndoRecord AddRecordIdentificationRollbackInfo(string recordIdentification, string rollbackInfo)
        {
            UPCRMUndoRecord undoRecord = new UPCRMUndoRecord(recordIdentification, rollbackInfo.JsonDictionaryFromString(), this);
            if (this.undoRecordDictionary == null)
            {
                this.undoRecordDictionary = new Dictionary<string, UPCRMUndoRecord> { { undoRecord.RecordIdentification, undoRecord } };
                this.undoRecords = new List<UPCRMUndoRecord> { undoRecord };
            }
            else
            {
                this.undoRecordDictionary.SetObjectForKey(undoRecord, undoRecord.RecordIdentification);
                this.undoRecords.Add(undoRecord);
            }

            return undoRecord;
        }
    }
}
