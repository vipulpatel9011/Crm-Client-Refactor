// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordSync.cs" company="Aurea Software Gmbh">
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
//   CRM record sync info
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Sync
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// CRM record sync info
    /// </summary>
    public class UPCRMRecordSync
    {
        private const bool EnterpriseFlag = true;
        private const int GenericLinkId = 126;

        /// <summary>
        /// The field ids.
        /// </summary>
        private List<int> fieldIds;

        /// <summary>
        /// The links.
        /// </summary>
        private List<object> links;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordSync"/> class.
        /// </summary>
        /// <param name="infoAreaDef">
        /// The information area definition.
        /// </param>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        public UPCRMRecordSync(List<object> infoAreaDef, ICRMDataStore dataStore)
        {
            this.InitFromSyncDefArray(infoAreaDef, dataStore);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordSync"/> class.
        /// </summary>
        /// <param name="recordSetDef">
        /// The record set definition.
        /// </param>
        /// <param name="dataStore">
        /// The _data store.
        /// </param>
        public UPCRMRecordSync(Dictionary<string, object> recordSetDef, ICRMDataStore dataStore)
        {
            var infoAreasDef = (recordSetDef.ValueOrDefault("metainfo") as JArray)?.ToObject<List<object>>() ?? recordSetDef.ValueOrDefault("metainfo") as List<object>;
            if (infoAreasDef != null)
            {
                if (infoAreasDef[0] is string)
                {
                    this.InitFromSyncDefArray(infoAreasDef, dataStore);
                }
                else
                {
                    List<object> defArray = (infoAreasDef[0] as JArray)?.ToObject<List<object>>() ?? infoAreasDef[0] as List<object>;
                    this.InitFromSyncDefArray(defArray, dataStore);
                }
            }

            this.IsFullSync = true;
            this.Timestamp = recordSetDef.ValueOrDefault("fullSyncTimestamp") as string;
            if (string.IsNullOrEmpty(this.Timestamp))
            {
                this.IsFullSync = false;
                this.Timestamp = recordSetDef.ValueOrDefault("syncTimestamp") as string;
            }

            this.DataSetName = recordSetDef.ValueOrDefault("datasetName") as string;
            this.LookupOnly = this.DataSetName?.IndexOf("(Lookup)", StringComparison.Ordinal) > 0 || this.LookupOnly;

            this.Rows = (recordSetDef.ValueOrDefault("rows") as JArray)?.ToObject<List<object>>() ?? recordSetDef.ValueOrDefault("rows") as List<object>;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordSync"/> class.
        /// </summary>
        /// <param name="recordSetDef">
        /// The record set definition.
        /// </param>
        /// <param name="dataStore">
        /// The _data store.
        /// </param>
        public UPCRMRecordSync(SyncRecord recordSetDef, ICRMDataStore dataStore)
        {
            var infoAreasDef = recordSetDef.metainfo;
            if (infoAreasDef != null)
            {
                if (infoAreasDef[0] is string)
                {
                    this.InitFromSyncDefArray(infoAreasDef[0], dataStore);
                }
                else
                {
                    List<object> defArray = infoAreasDef[0];
                    this.InitFromSyncDefArray(defArray, dataStore);
                }
            }

            this.IsFullSync = true;
            this.Timestamp = recordSetDef.fullSyncTimestamp;
            if (string.IsNullOrEmpty(this.Timestamp))
            {
                this.IsFullSync = false;
                this.Timestamp = recordSetDef.syncTimestamp;
            }

            this.DataSetName = recordSetDef.datasetName;
            this.LookupOnly = this.DataSetName?.IndexOf("(Lookup)", StringComparison.Ordinal) > 0 || this.LookupOnly;

            this.Rows = recordSetDef.rows;
        }


        /// <summary>
        /// Gets the name of the data set.
        /// </summary>
        /// <value>
        /// The name of the data set.
        /// </value>
        public string DataSetName { get; private set; }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is full synchronize.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is full synchronize; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullSync { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [lookup only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [lookup only]; otherwise, <c>false</c>.
        /// </value>
        public bool LookupOnly { get; private set; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<object> Rows { get; private set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get; private set; }

        /// <summary>
        /// Synchronizes the record set definitions.
        /// </summary>
        /// <param name="recordSetDefinitions">
        /// The record set definitions.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public static UPCRMRecordSyncResult SyncRecordSetDefinitions(List<object> recordSetDefinitions)
        {
            return SyncRecordSetDefinitionsCrmDataStore(recordSetDefinitions, UPCRMDataStore.DefaultStore);
        }

        /// <summary>
        /// Synchronizes the record set definitions CRM data store.
        /// </summary>
        /// <param name="recordSetDefinitions">
        /// The record set definitions.
        /// </param>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        /// <param name="isEnterprise">
        /// Is enterprise.</param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public static UPCRMRecordSyncResult SyncRecordSetDefinitionsCrmDataStore(
            List<object> recordSetDefinitions,
            ICRMDataStore dataStore, bool isEnterprise = false)
        {
            UPCRMRecordSyncResult result = null;
            foreach (var defObject in recordSetDefinitions)
            {
                //TODO check if defObject can actually ever be a JObject. I didn't touch the cast below for now just to be safe
                var def = (defObject as JObject)?.ToObject<Dictionary<string, object>>() ?? defObject as Dictionary<string, object>;

                if (def == null)
                {
                    continue;
                }

                var syncTemplate = new UPCRMRecordSync(def, dataStore);
                var currentResult = syncTemplate.SyncRecords(isEnterprise);
                result = currentResult.ResultByAppendingResult(result);
                if (!result.Successful)
                {
                    return result;
                }
            }

            return result ?? UPCRMRecordSyncResult.SuccessResultWithRecordCount(0);
        }

        /// <summary>
        /// Synchronizes the record set definitions CRM data store.
        /// </summary>
        /// <param name="recordSetDefinitions">
        /// The record set definitions.
        /// </param>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        /// <param name="isEnterprise">
        /// Is enterprise.</param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public static UPCRMRecordSyncResult SyncRecordSetDefinitionsCrmDataStore(
            List<SyncRecord> recordSetDefinitions,
            ICRMDataStore dataStore, bool isEnterprise = false)
        {
            UPCRMRecordSyncResult result = null;
            foreach (var defObject in recordSetDefinitions)
            {
                var syncTemplate = new UPCRMRecordSync(defObject, dataStore);
                var currentResult = syncTemplate.SyncRecords(isEnterprise);
                result = currentResult.ResultByAppendingResult(result);
                if (!result.Successful)
                {
                    return result;
                }
            }

            return result ?? UPCRMRecordSyncResult.SuccessResultWithRecordCount(0);
        }

        /// <summary>
        /// Synchronizes the records.
        /// </summary>
        /// <param name="isEnterprise">
        /// Is enterprise
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public UPCRMRecordSyncResult SyncRecords(bool isEnterprise)
        {
            return this.SyncRecords(this.Rows, isEnterprise);
        }

        /// <summary>
        /// Synchronizes the records.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <param name="isEnterprise">
        /// Is enterprise
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public UPCRMRecordSyncResult SyncRecords(List<object> rows, bool isEnterprise)
        {
            var database = this.DataStore.DatabaseInstance;
            database.BeginTransaction();

            var syncResult = isEnterprise ? this.SyncRecordsTransactionEnterprise(rows) : this.SyncRecordsTransaction(rows);
            if (syncResult.Successful)
            {
                database.Commit();

                if (!string.IsNullOrEmpty(this.Timestamp))
                {
                    this.DataStore.ReportSyncWithDatasetRecordCountTimestampFullSyncInfoAreaId(
                        this.DataSetName,
                        syncResult.RecordCount,
                        this.Timestamp,
                        this.IsFullSync,
                        this.InfoAreaId);
                }
            }
            else
            {
                database.Rollback();
            }

            return syncResult;
        }

        /// <summary>
        /// Synchronizes the records transaction.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public UPCRMRecordSyncResult SyncRecordsTransactionEnterprise(List<object> rows)
        {
            return this.SyncRecordsTransaction(rows, EnterpriseFlag);
        }

        /// <summary>
        /// Synchronizes the records transaction.
        /// </summary>
        /// <param name="rows">
        /// The rows.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public UPCRMRecordSyncResult SyncRecordsTransaction(List<object> rows)
        {
            return this.SyncRecordsTransaction(rows, !EnterpriseFlag);
        }

        private static void AddOrUpdateRecords(List<Record> recordsList, List<UPCRMRecordParticipantWriter> participantsWriter, out int syncRecordCount, out int ret)
        {
            ret = 0;
            syncRecordCount = 0;

            foreach (var record in recordsList)
            {
                if (!record.Exists())
                {
                    record.Insert();
                    if (participantsWriter != null)
                    {
                        foreach (UPCRMRecordParticipantWriter participantWriter in participantsWriter)
                        {
                            ret = participantWriter.AddValueForRecordId(record.GetValue(participantWriter.ResultIndex), record.RecordId);
                            if (ret != 0)
                            {
                                break;
                            }
                        }

                        if (ret != 0)
                        {
                            participantsWriter = null;
                            ret = 0; // ignore Participants sync error
                        }
                    }
                }
                else
                {
                    record.Update();
                    if (participantsWriter != null)
                    {
                        foreach (UPCRMRecordParticipantWriter participantWriter in participantsWriter)
                        {
                            ret = participantWriter.UpdateValueForRecordId(record.GetValue(participantWriter.ResultIndex), record.RecordId);
                            if (ret != 0)
                            {
                                break;
                            }
                        }

                        if (ret != 0)
                        {
                            participantsWriter = null;
                            ret = 0;  // ignore Participants sync error
                        }
                    }
                }

                ++syncRecordCount;
            }
        }

        private UPCRMRecordSyncResult SyncRecordsTransaction(List<object> rows, bool isEnterprise)
        {
            if (rows == null || rows.Count == 0)
            {
                return UPCRMRecordSyncResult.SuccessResultWithRecordCount(0);
            }

            var database = this.DataStore.DatabaseInstance;
            var tableInfo = database.GetTableInfoByInfoArea(this.InfoAreaId);
            if (tableInfo == null)
            {
                // dont crash if client database does not exist
                return UPCRMRecordSyncResult.SuccessResultWithRecordCount(0);
            }

            int fieldCount;
            FieldIdType[] intFieldIds = null;
            int[] fieldMap = null;
            var participantsWriter = this.AddParticipiantWriters(database, tableInfo, this.fieldIds.Count, out fieldCount, out intFieldIds, out fieldMap);

            int linkCount, linkFieldCount;
            string[] cLinkFields;
            int[] linkMap = this.CreateLinkMap(tableInfo, out linkCount, out linkFieldCount, out cLinkFields);

            var recordTemplate = this.CreateRecordTemplate(database, fieldCount, intFieldIds, linkFieldCount, cLinkFields);
            var recordsList = this.CreateRecords(rows, isEnterprise, this.fieldIds.Count, fieldMap, linkCount, linkMap, recordTemplate);

            var syncRecordCount = 0;
            int ret = 0;
            AddOrUpdateRecords(recordsList, participantsWriter, out syncRecordCount, out ret);

            if (this.LookupOnly)
            {
                database.SetHasLookupRecords(this.InfoAreaId);
            }

            return ret > 0 ?
             UPCRMRecordSyncResult.ResultWithErrorCode(ret) :
             UPCRMRecordSyncResult.SuccessResultWithRecordCount(syncRecordCount);
        }

        private List<Record> CreateRecords(List<object> rows, bool isEnterprise, int sourceFieldCount, int[] fieldMap, int linkCount, int[] linkMap, RecordTemplate recordTemplate)
        {
            var recordsList = new List<Record>();

            Parallel.ForEach(rows, (recordDefJArray) =>
            {
                var recordDefArray = isEnterprise ?
                ((JArray)recordDefJArray).ToObject<List<object>>() :
                (recordDefJArray as JArray)?.ToObject<List<object>>() ?? recordDefJArray as List<object>;

                var recordDef = isEnterprise ?
                (recordDefArray[0] as JArray)?.ToObject<List<object>>() :
                recordDefArray[0] is string ?
                    recordDefArray :
                    (recordDefArray[0] as JArray)?.ToObject<List<object>>() ?? recordDefArray[0] as List<object>;

                var recordInfoAreaId = recordDef?[0] as string;
                var recordId = recordDef?[1] as string;
                var recordExists = recordDef != null && recordDef.Count > 4 ? recordDef[4].ToInt() : 1;

                var record = new Record(recordTemplate.InfoAreaId, recordId);
                record.SetTemplateWeak(recordTemplate);
                if (recordExists == 0)
                {
                    record.Delete();
                }
                else
                {
                    record.SetLookupRecord(this.LookupOnly);
                    if (!string.IsNullOrWhiteSpace(recordInfoAreaId))
                    {
                        record.SetVirtualInfoAreaId(recordInfoAreaId);
                    }

                    var fieldValues = isEnterprise ?
                    (recordDef[2] as JArray)?.ToObject<List<object>>() :
                    (recordDef[2] as JArray)?.ToObject<List<object>>() ?? recordDef[2] as List<object>;

                    for (var sourceFieldlndex = 0; sourceFieldlndex < sourceFieldCount; sourceFieldlndex++)
                    {
                        if (fieldMap[sourceFieldlndex] >= 0)
                        {
                            record.SetValue(fieldMap[sourceFieldlndex], (string)fieldValues[sourceFieldlndex]);
                        }
                    }

                    if (linkCount > 0)
                    {
                        var linkValues = isEnterprise ? (recordDef[3] as JArray)?.ToObject<List<object>>() : (recordDef[3] as JArray)?.ToObject<List<object>>() ?? recordDef[3] as List<object>;

                        var fieldOffset = recordTemplate.FieldIdCount;
                        for (var linkIndex = 0; linkIndex < linkCount; linkIndex++)
                        {
                            if (linkMap[linkIndex] >= 0)
                            {
                                record.SetValue(linkMap[linkIndex] + fieldOffset, (string)linkValues[linkIndex]);
                            }
                        }
                    }

                    lock (recordsList)
                    {
                        recordsList.Add(record);
                    }
                }
            });

            return recordsList;
        }

        private int[] CreateLinkMap(TableInfo tableInfo, out int linkCount, out int linkFieldCount, out string[] cLinkFields)
        {
            var sourceLinkCount = this.links?.Count ?? 0;
            linkCount = 0;
            linkFieldCount = 0;
            int[] linkMap = null;
            cLinkFields = null;

            if (sourceLinkCount > 0)
            {
                var hadGeneric = false;
                cLinkFields = new string[sourceLinkCount + 1];
                linkMap = new int[sourceLinkCount + 1];
                linkFieldCount = 0;

                for (var sourceLinkIndex = 0; sourceLinkIndex < sourceLinkCount; sourceLinkIndex++)
                {
                    var linkFieldName = (string)this.links?[sourceLinkIndex];
                    var linkInfo = tableInfo.GetLink(linkFieldName);
                    if (linkInfo != null && linkInfo.HasColumn)
                    {
                        if (linkInfo.LinkId == GenericLinkId)
                        {
                            if (hadGeneric)
                            {
                                linkMap[linkCount++] = -1;
                            }
                            else
                            {
                                cLinkFields[linkFieldCount] = linkInfo.InfoAreaColumnName;
                                linkMap[linkCount++] = linkFieldCount++;
                                cLinkFields[linkFieldCount] = linkInfo.ColumnName;
                                linkMap[linkCount++] = linkFieldCount++;
                                hadGeneric = true;
                            }
                        }
                        else
                        {
                            cLinkFields[linkFieldCount] = linkFieldName;
                            linkMap[linkCount++] = linkFieldCount++;
                        }
                    }
                    else
                    {
                        linkMap[linkCount++] = -1;
                    }
                }
            }

            return linkMap;
        }

        private RecordTemplate CreateRecordTemplate(CRMDatabase database, int fieldCount, FieldIdType[] intFieldIds, int linkFieldCount, string[] cLinkFields)
        {
            var includeLookupForNew = false;
            var includeLookupForUpdate = false;

            if (database.HasLookup)
            {
                includeLookupForNew = this.LookupOnly;
                includeLookupForUpdate = !this.LookupOnly;
            }
            else
            {
                includeLookupForNew = false;
                includeLookupForUpdate = false;
            }

            var recordTemplate = new RecordTemplate(
                database,
                true,
                this.InfoAreaId,
                fieldCount,
                intFieldIds,
                linkFieldCount,
                cLinkFields?.ToList(),
                includeLookupForNew,
                includeLookupForUpdate);

            return recordTemplate;
        }

        private List<UPCRMRecordParticipantWriter> AddParticipiantWriters(CRMDatabase database, TableInfo tableInfo, int sourceFieldCount, out int fieldCount, out FieldIdType[] intFieldIds, out int[] fieldMap)
        {
            intFieldIds = null;
            fieldMap = null;
            fieldCount = 0;

            List<UPCRMRecordParticipantWriter> participantsWriter = null;

            if (sourceFieldCount > 0)
            {
                intFieldIds = new FieldIdType[sourceFieldCount];
                fieldMap = new int[sourceFieldCount];
                fieldCount = 0;
                for (var sourceFieldIndex = 0; sourceFieldIndex < sourceFieldCount; sourceFieldIndex++)
                {
                    var sourceFieldId = this.fieldIds[sourceFieldIndex];
                    var fieldInfo = tableInfo.GetFieldInfo(sourceFieldId);
                    if (fieldInfo != null)
                    {
                        intFieldIds[fieldCount] = (FieldIdType)sourceFieldId;
                        if (fieldInfo.IsParticipantsField)
                        {
                            var writer = new UPCRMRecordParticipantWriter(this.InfoAreaId, fieldInfo, database)
                            {
                                ResultIndex = sourceFieldIndex
                            };
                            if (participantsWriter == null)
                            {
                                participantsWriter = new List<UPCRMRecordParticipantWriter>();
                            }
                            participantsWriter.Add(writer);
                        }

                        fieldMap[sourceFieldIndex] = fieldCount++;
                    }
                    else
                    {
                        fieldMap[sourceFieldIndex] = -1;
                    }
                }
            }

            return participantsWriter;
        }

        /// <summary>
        /// Initializes from synchronize definition array.
        /// </summary>
        /// <param name="infoAreaDef">
        /// The information area definition.
        /// </param>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        private void InitFromSyncDefArray(List<object> infoAreaDef, ICRMDataStore dataStore)
        {
            this.DataStore = dataStore;
            this.InfoAreaId = (string)infoAreaDef[0];
            this.fieldIds = (infoAreaDef[1] as JArray)?.ToObject<List<int>>() ?? (infoAreaDef[1] as List<object>)?.Cast<int>().ToList();
            this.links = (infoAreaDef[2] as JArray)?.ToObject<List<object>>() ?? infoAreaDef[2] as List<object>;
            this.IsFullSync = false;
            this.Timestamp = null;
            this.DataSetName = null;
        }
    }
}
