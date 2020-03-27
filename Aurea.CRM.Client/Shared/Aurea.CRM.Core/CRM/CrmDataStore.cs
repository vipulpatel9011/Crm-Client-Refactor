// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmDataStore.cs" company="Aurea Software Gmbh">
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
//   Implements the local CRM data store functionality
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.Sync;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Implements the local CRM data store functionality
    /// </summary>
    public class UPCRMDataStore : ICRMDataStore
    {
        /// <summary>
        /// The default database name.
        /// </summary>
        public const string DefaultDatabaseName = "crmData";

        /// <summary>
        /// The database filename.
        /// </summary>
        private readonly string databaseFilename;

        /// <summary>
        /// The is update crm.
        /// </summary>
        private readonly bool isUpdateCrm;

        /// <summary>
        /// The offline data for info area id.
        /// </summary>
        private Dictionary<string, object> offlineDataForInfoAreaId;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMDataStore"/> class.
        /// </summary>
        /// <param name="baseDirectoryPath">
        /// The base directory path.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="recreate">
        /// if set to <c>true</c> [recreate].
        /// </param>
        /// <param name="isUpdateCrm">
        /// if set to <c>true</c> [is update CRM].
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        public UPCRMDataStore(
            string baseDirectoryPath,
            string fileName,
            bool recreate,
            bool isUpdateCrm,
            IConfigurationUnitStore configStore)
        {
            this.BaseDirectoryPath = baseDirectoryPath;
            this.isUpdateCrm = isUpdateCrm;
            this.databaseFilename = Path.Combine(this.BaseDirectoryPath, fileName);
            if (recreate)
            {
                var platformService = SimpleIoc.Default.GetInstance<IPlatformService>();
                if (platformService.StorageProvider.FileExists(this.databaseFilename))
                {
                    Exception error = null;

                    platformService.StorageProvider.TryDelete(databaseFilename, out error);
                    if (error != null)
                    {
                        Logger.LogError(error);
                    }
                }
            }

            var db = CRMDatabase.Create(isUpdateCrm, this.databaseFilename);
            this.DatabaseInstance = db;
            if (configStore.ConfigValueIsSetDefaultValue("System.DisplayFixCatBySortInfo", true))
            {
                this.DatabaseInstance.FixedCatSortBySortInfoAndCode = true;
            }

            if (recreate && !this.UpdateDDL())
            {
                return;
            }

            this.AddVirtualLinks();
            this.Reps = new UPCRMReps();

            this.VirtualInfoAreaCache = new UPVirtualInfoAreaCache();
            this.Disable86326 = configStore.ConfigValueIsSet("Disable.86326");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMDataStore"/> class.
        /// </summary>
        /// <param name="baseDirectoryPath">
        /// The base directory path.
        /// </param>
        /// <param name="isUpdateCRM">
        /// if set to <c>true</c> [is update CRM].
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        public UPCRMDataStore(string baseDirectoryPath, bool isUpdateCRM, IConfigurationUnitStore configStore)
            : this(baseDirectoryPath, DefaultDatabaseName, false, isUpdateCRM, configStore)
        {
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the reps.
        /// </summary>
        /// <value>
        /// The reps.
        /// </value>
        public UPCRMReps Reps { get; private set; }

        /// <summary>
        /// Gets the virtual information area cache.
        /// </summary>
        /// <value>
        /// The virtual information area cache.
        /// </value>
        public UPVirtualInfoAreaCache VirtualInfoAreaCache { get; private set; }

        /// <summary>
        /// Gets the base directory path.
        /// </summary>
        /// <value>
        /// The base directory path.
        /// </value>
        public string BaseDirectoryPath { get; private set; }

        /// <summary>
        /// Gets the database instance.
        /// </summary>
        /// <value>
        /// The database instance.
        /// </value>
        public CRMDatabase DatabaseInstance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMDataStore"/> is disable86326.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable86326; otherwise, <c>false</c>.
        /// </value>
        public bool Disable86326 { get; private set; }

        /// <summary>
        /// Adds the virtual links.
        /// </summary>
        public void AddVirtualLinks()
        {
            var database = this.DatabaseInstance;
            var dataModel = database.DataModel;
            var linkInfo = new VirtualLinkInfo(dataModel, "MA", "KP", 0, VirtualLinkType.MoveFromSource, "MB", 0, 0);
            dataModel.AddVirtualLinkWithOwnership(linkInfo);

            linkInfo = new VirtualLinkInfo(dataModel, "MA", "FI", 0, VirtualLinkType.MoveFromSource, "MB", 0, 0);
            dataModel.AddVirtualLinkWithOwnership(linkInfo);

            linkInfo = new VirtualLinkInfo(dataModel, "FI", "MA", 0, VirtualLinkType.MoveFromTarget, "MB", 0, 0);
            dataModel.AddVirtualLinkWithOwnership(linkInfo);

            linkInfo = new VirtualLinkInfo(dataModel, "KP", "MA", 0, VirtualLinkType.MoveFromTarget, "MB", 0, 0);
            dataModel.AddVirtualLinkWithOwnership(linkInfo);

            if (this.isUpdateCrm)
            {
                linkInfo = new VirtualLinkInfo(dataModel, "MA", "PE", 0, VirtualLinkType.MoveFromSource, "MB", 0, 0);
                dataModel.AddVirtualLinkWithOwnership(linkInfo);

                linkInfo = new VirtualLinkInfo(dataModel, "MA", "CP", 0, VirtualLinkType.MoveFromSource, "MB", 0, 0);
                dataModel.AddVirtualLinkWithOwnership(linkInfo);

                linkInfo = new VirtualLinkInfo(dataModel, "PE", "MA", 0, VirtualLinkType.MoveFromTarget, "MB", 0, 0);
                dataModel.AddVirtualLinkWithOwnership(linkInfo);

                linkInfo = new VirtualLinkInfo(dataModel, "CP", "MA", 0, VirtualLinkType.MoveFromTarget, "MB", 0, 0);
                dataModel.AddVirtualLinkWithOwnership(linkInfo);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.DatabaseInstance != null)
            {
                this.DatabaseInstance.Dispose();
                this.DatabaseInstance = null;
            }
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="recreate">
        /// if set to <c>true</c> [recreate].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DeleteDatabase(bool recreate)
        {
            var platform = SimpleIoc.Default.GetInstance<IPlatformService>();

            if (!platform.StorageProvider.FileExists(this.databaseFilename))
            {
                return false;
            }

            if (this.DatabaseInstance != null)
            {
                this.DatabaseInstance.Dispose();
                this.DatabaseInstance = null;
            }

            Exception error;
            platform.StorageProvider.TryDelete(this.databaseFilename, out error);
            if (!recreate)
            {
                return true;
            }

            this.DatabaseInstance = CRMDatabase.Create(this.isUpdateCrm, this.databaseFilename);
            if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("System.DisplayFixCatBySortInfo", true))
            {
                this.DatabaseInstance.FixedCatSortBySortInfoAndCode = true;
            }

            this.Reps = new UPCRMReps();
            return true;
        }

        /// <summary>
        /// Renames the database to default.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RenameDatabaseToDefault()
        {
            if (this.DatabaseInstance != null)
            {
                this.DatabaseInstance.Dispose();
                this.DatabaseInstance = null;
            }

            var defaultDatabaseFilename = Path.Combine(this.BaseDirectoryPath, DefaultDatabaseName);
            if (Equals(defaultDatabaseFilename, this.databaseFilename))
            {
                return true;
            }

            var platform = SimpleIoc.Default.GetInstance<IPlatformService>();

            Exception error;
            platform.StorageProvider.TryMove(this.databaseFilename, defaultDatabaseFilename, out error);
            return error == null;
        }

        /// <summary>
        /// Updates the DDL.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdateDDL()
        {
            return this.DatabaseInstance.EnsureDDL();
        }

        /// <summary>
        /// Sets the time zone UTC difference.
        /// </summary>
        /// <param name="timezone">
        /// The timezone.
        /// </param>
        /// <param name="utcDifference">
        /// The UTC difference.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetTimeZoneUtcDifference(string timezone, int utcDifference)
        {
            return this.DatabaseInstance.WriteTimeZoneInformation(timezone, utcDifference);
        }

        /// <summary>
        /// Times the name of the zone.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TimeZoneName()
        {
#if PORTING
            string timeZone = DatabaseInstance.GetTimeZoneName();
            if (string.IsNullOrEmpty(timeZone))
            {
                return null;
            }

            return timeZone;
#else
            return null;
#endif
        }

        /// <summary>
        /// Stores the data model.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int StoreDataModel(DAL.DataModel dataModel)
        {
            dataModel.SetCatalogInfoFromDataModel();
            dataModel.Save();
            dataModel.EnsureDDL();
            return this.DatabaseInstance.ResetDataModel();
        }

        /// <summary>
        /// Resets the data model.
        /// </summary>
        public void ResetDataModel()
        {
            this.DatabaseInstance?.ResetDataModel();
        }

        /// <summary>
        /// Gets the default store.
        /// </summary>
        /// <value>
        /// The default store.
        /// </value>
        public static ICRMDataStore DefaultStore => ServerSession.CurrentSession?.CrmDataStore;

        /// <summary>
        /// Determines whether [is valid information area] [the specified information area identifier].
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValidInfoArea(string infoAreaId)
        {
            return this.DatabaseInstance?.GetTableInfo(infoAreaId) != null;
        }

        /// <summary>
        /// Tables the information for information area.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMTableInfo"/>.
        /// </returns>
        public UPCRMTableInfo TableInfoForInfoArea(string infoAreaId)
        {
            return new UPCRMTableInfo(infoAreaId, this);
        }

        /// <summary>
        /// Fields the information for information area field identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldInfo"/>.
        /// </returns>
        public UPCRMFieldInfo FieldInfoForInfoAreaFieldId(string infoAreaId, int fieldId)
        {
            if (infoAreaId == null || fieldId < 0)
            {
                return null;
            }

            return UPCRMFieldInfo.Create(fieldId, infoAreaId, this);
        }

        /// <summary>
        /// Fields the information for field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldInfo"/>.
        /// </returns>
        public UPCRMFieldInfo FieldInfoForField(UPCRMField field)
        {
            return UPCRMFieldInfo.Create(field, this);
        }

        /// <summary>
        /// Links the information for information area target information area link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="targetInfoAreaId">
        /// The target information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkInfo"/>.
        /// </returns>
        public UPCRMLinkInfo LinkInfoForInfoAreaTargetInfoAreaLinkId(
            string infoAreaId,
            string targetInfoAreaId,
            int linkId)
        {
            return new UPCRMLinkInfo(infoAreaId, targetInfoAreaId, linkId, this);
        }

        /// <summary>
        /// Idents the link for information area.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkInfo"/>.
        /// </returns>
        public UPCRMLinkInfo IdentLinkForInfoArea(string infoAreaId)
        {
            return this.LinkInfoForInfoAreaTargetInfoAreaLinkId(infoAreaId, infoAreaId, -1);
        }

        /// <summary>
        /// Catalogs for variable catalog identifier.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalog"/>.
        /// </returns>
        public UPCatalog CatalogForVariableCatalogId(int catNo)
        {
            return new UPCatalog(catNo, this.DatabaseInstance, false);
        }

        /// <summary>
        /// Catalogs for fixed catalog identifier.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalog"/>.
        /// </returns>
        public UPCatalog CatalogForFixedCatalogId(int catNo)
        {
            return new UPCatalog(catNo, this.DatabaseInstance, true);
        }

        /// <summary>
        /// Catalogs for CRM field.
        /// </summary>
        /// <param name="crmField">
        /// The CRM field.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalog"/>.
        /// </returns>
        public UPCatalog CatalogForCrmField(UPCRMField crmField)
        {
            var fieldInfo = this.FieldInfoForField(crmField);
            if (fieldInfo == null)
            {
                return null;
            }

            if (fieldInfo.FieldType == "K")
            {
                return this.CatalogForVariableCatalogId(fieldInfo.CatNo);
            }

            if (fieldInfo.FieldType == "X")
            {
                return this.CatalogForFixedCatalogId(fieldInfo.CatNo);
            }

            return null;
        }

        /// <summary>
        /// Records the exists offline.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RecordExistsOffline(string recordIdentification)
        {
            if (recordIdentification == null)
            {
                return false;
            }

            var crmQuery = new UPContainerMetaInfo(null, recordIdentification.InfoAreaId());
            crmQuery.SetLinkRecordIdentification(recordIdentification);
            var result = crmQuery.Find();
            return result?.RowCount > 0;
        }

        /// <summary>
        /// Replaces the record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ReplaceRecordIdentification(string recordIdentification)
        {
            return recordIdentification == "ID.$curRep" ? this.Reps.CurrentRepRecordIdentification : recordIdentification;
        }

        /// <summary>
        /// Deletes the record with identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteRecordWithIdentification(string recordIdentification)
        {
            if (string.IsNullOrEmpty(recordIdentification))
            {
                return 0;
            }

            return this.DeleteRecordWithIdentificationRollbackRequestNr(recordIdentification, -1);
        }

        /// <summary>
        /// Deletes the record with identification rollback request nr.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="requestNr">
        /// The request nr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteRecordWithIdentificationRollbackRequestNr(string recordIdentification, int requestNr)
        {
            var infoAreaId = recordIdentification.InfoAreaId();
            var recordId = recordIdentification.RecordId();

            var tableInfo = this.DatabaseInstance.GetTableInfoByInfoArea(infoAreaId);
            if (tableInfo == null)
            {
                return -1;
            }

            var deleteStatement = $"DELETE FROM {tableInfo.DatabaseTableName} WHERE {tableInfo.RecordIdFieldName} = ?";
            int ret = this.DatabaseInstance.Execute(deleteStatement, recordId);
            if (!this.Disable86326)
            {
                var count = tableInfo.GetParticipantsFieldCount();
                if (ret == 0 && count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        var pt = tableInfo.GetDatabaseTableNameForParticipantsField(i);
                        deleteStatement = $"DELETE FROM {pt} WHERE recid = ?";
                        ret = this.DatabaseInstance.Execute(deleteStatement, recordId);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Creates the index for.
        /// </summary>
        /// <param name="crmField">
        /// The CRM field.
        /// </param>
        /// <param name="namePrefix">
        /// The name prefix.
        /// </param>
        public void CreateIndexFor(UPCRMField crmField, string namePrefix)
        {
            var infoAreaId = crmField.InfoAreaId;
            var tableInfo = this.DatabaseInstance.GetTableInfoByInfoArea(infoAreaId);
            if (tableInfo == null)
            {
                return;
            }

            var databaseTableName = tableInfo.DatabaseTableName;
            var databaseFieldName = tableInfo.GetFieldName((int)crmField.FieldId);
            string databaseIndexName = $"INDEX_{namePrefix}_{databaseTableName}_{databaseFieldName}";
            if (this.DatabaseInstance.ExistsIndex(databaseIndexName))
            {
                return;
            }

            Logger.LogDebug($"Creating index for quicksearch definition table {databaseTableName}, field {databaseFieldName}", LogFlag.LogStatements);

            var createIndexStatement = $"CREATE INDEX {databaseIndexName} ON {databaseTableName}({databaseFieldName}) ";
            this.DatabaseInstance.Execute(createIndexStatement);
        }

        /// <summary>
        /// Creates the index for.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="columns">
        /// The columns.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        public void CreateIndexFor(string infoAreaId, List<object> columns, string prefix)
        {
            var databaseTableName = $"CRM_{infoAreaId}";
            var indexName = $"INDEX_{prefix}_{databaseTableName}_";
            if (columns == null || columns.Count <= 0)
            {
                return;
            }

            var firstAdded = false;
            var columnsSql = string.Empty;

            foreach (string columnName in columns)
            {
                var desc = false;
                var normColumnName = columnName;
                if (columnName.StartsWith("d"))
                {
                    desc = true;
                    normColumnName = columnName.Substring(1);
                }

                columnsSql = firstAdded ? $"{columnsSql}, {normColumnName}" : $"{normColumnName}";

                if (desc)
                {
                    columnsSql = $"{columnsSql} DESC";
                }

                indexName = $"{indexName}_{columnName}";
                firstAdded = true;
            }

            // DDLogInfo("Creating custom index for table: %@ columns: %@", databaseTableName, columnsSQL);
            Logger.LogInfo($"Creating custom index for table: {databaseTableName} columns: {columnsSql}");
            var createIndexStatement = $"CREATE INDEX {indexName} ON {databaseTableName}({columnsSql})";
            var database = this.DatabaseInstance;
            database.Execute(createIndexStatement);
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="undo">if set to <c>true</c> [undo].</param>
        /// <returns>Record</returns>
        public DAL.Record CreateRecord(UPCRMRecord record, bool undo)
        {
            string infoAreaId = record.InfoAreaId;
            List<FieldIdType> fieldIds;
            List<string> linkFieldNames;
            bool checkDataModel = true;

            TableInfo tableInfo = this.DatabaseInstance.GetTableInfoByInfoArea(infoAreaId);
            if (tableInfo == null)
            {
                return null;
            }

            int statNoField = 0, lnrField = 0;
            bool saveOfflineStationNumber = false;
            if (!undo && record.OfflineRecordNumber > 0 && record.OfflineStationNumber > 0)
            {
                statNoField = tableInfo.GetStatNoFieldId();
                if (statNoField >= 0)
                {
                    lnrField = tableInfo.GetLNrFieldId();
                    if (lnrField >= 0 && tableInfo.GetFieldInfo(statNoField) != null && tableInfo.GetFieldInfo(lnrField) != null)
                    {
                        saveOfflineStationNumber = true;
                    }
                }
            }

            List<UPCRMFieldValue> fieldValues = record.FieldValues;
            int fieldCount = fieldValues?.Count ?? 0;
            int linkCount = record.Links?.Count ?? 0;
            if (undo)
            {
                linkCount = 0;
            }

            List<string> recordFieldValues = new List<string>();
            if (fieldCount == 0)
            {
                fieldIds = null;
            }
            else
            {
                fieldIds = new List<FieldIdType>();
                for (int i = 0; i < fieldCount; i++)
                {
                    UPCRMFieldValue fieldValue = fieldValues[i];
                    FieldInfo fieldInfo = tableInfo.GetFieldInfo(fieldValue.FieldId);
                    if (!checkDataModel || fieldInfo != null)
                    {
                        int currentFieldId = fieldValue.FieldId;
                        fieldIds.Add((FieldIdType)currentFieldId);
                        recordFieldValues.Add(undo ? fieldValue.OldValue : fieldValue.Value);

                        if (saveOfflineStationNumber && (currentFieldId == statNoField || currentFieldId == lnrField))
                        {
                            saveOfflineStationNumber = false;
                        }
                    }
                }

                if (saveOfflineStationNumber)
                {
                    fieldIds.Add((FieldIdType)statNoField);
                    recordFieldValues.Add(record.OfflineStationNumber.ToString());
                    fieldIds.Add((FieldIdType)lnrField);
                    recordFieldValues.Add(record.OfflineRecordNumber.ToString());
                }
            }

            if (record.Links != null && (record.Links.Count == 0 || undo))
            {
                linkFieldNames = null;
                linkCount = 0;
            }
            else
            {
                linkFieldNames = new List<string>();
                for (int i = 0; i < linkCount; i++)
                {
                    UPCRMLink link = record.Links[i];
                    LinkInfo linkInfo = tableInfo.GetLink(link.LinkFieldName()) ??
                                        tableInfo.GetLink(link.InfoAreaId, link.LinkId);

                    if (linkInfo != null && linkInfo.HasColumn)
                    {
                        if (linkInfo.IsGeneric)
                        {
                            string targetInfoAreaId = this.RootPhysicalInfoAreaIdForInfoAreaId(link.InfoAreaId);
                            recordFieldValues.Add(targetInfoAreaId);
                            linkFieldNames.Add(linkInfo.InfoAreaColumnName);
                        }

                        recordFieldValues.Add(link.RecordId);
                        linkFieldNames.Add(linkInfo.ColumnName);
                    }
                }
            }

            var recordTemplate = new RecordTemplate(
                this.DatabaseInstance,
                false,
                infoAreaId,
                fieldIds?.Count ?? 0,
                fieldIds?.ToArray(),
                linkFieldNames.Count,
                linkFieldNames,
                false,
                false);

            Record rec = new Record(infoAreaId, record.RecordId);
            rec.SetTemplate(recordTemplate);
            for (var i = 0; i < recordFieldValues.Count; i++)
            {
                rec.SetValue(i, recordFieldValues[i]);
            }

            return rec;
        }

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public Record CreateRecord(UPCRMRecord record)
        {
            return this.CreateRecord(record, false);
        }

        /// <summary>
        /// Saves the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public int SaveRecord(UPCRMRecord record)
        {
            return this.SaveRecordRollbackRequestNr(record, -1);
        }

        /// <summary>
        /// Saves the record rollback request nr.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="requestNr">The request nr.</param>
        /// <returns></returns>
        public int SaveRecordRollbackRequestNr(UPCRMRecord record, int requestNr)
        {
            CRMDatabase database = this.DatabaseInstance;
            Record rec = this.CreateRecord(record);

            if (rec == null)
            {
                return -1;
            }

            database.BeginTransaction();
            int ret = rec.InsertOrUpdate();
            if (ret == 0 && !this.Disable86326)
            {
                this.UpdateParticipantsTablesForRecord(rec, database);
            }

            database.Commit();
            return ret;
        }

        /// <summary>
        /// Undoes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        public int UndoRecord(UPCRMRecord record)
        {
            CRMDatabase database = this.DatabaseInstance;
            Record rec = this.CreateRecord(record, true);
            if (rec == null)
            {
                return -1;
            }

            database.BeginTransaction();
            int ret = rec.Update();
            if (ret == 0 && !this.Disable86326)
            {
                this.UpdateParticipantsTablesForRecord(rec, database);
            }

            database.Commit();
            return ret;
        }

        /// <summary>
        /// Updates the participants tables for record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public int UpdateParticipantsTablesForRecord(Record record, DatabaseBase database)
        {
            var recordTemplate = record.RecordTemplate;
            var tableInfo = recordTemplate.GetTableInfo();
            int participantsFieldCount = tableInfo.GetParticipantsFieldCount();
            if (participantsFieldCount == 0)
            {
                return 0;
            }

            for (int i = 0; i < participantsFieldCount; i++)
            {
                FieldInfo participantsFieldInfo = tableInfo.ParticipantsFieldInfos[i];
                var fieldPos = recordTemplate.GetFieldPos(participantsFieldInfo.FieldId);
                if (fieldPos < 0)
                {
                    continue;
                }

                UPCRMRecordParticipantWriter pw = new UPCRMRecordParticipantWriter(record.InfoAreaId, participantsFieldInfo, database);
                pw.UpdateValueForRecordId(record.GetValue(fieldPos), record.RecordId);
            }

            return 0;
        }

        /// <summary>
        /// Reports the synchronize with dataset record count timestamp full synchronize information area identifier.
        /// </summary>
        /// <param name="datasetName">
        /// Name of the dataset.
        /// </param>
        /// <param name="recordCount">
        /// The record count.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp.
        /// </param>
        /// <param name="fullSync">
        /// if set to <c>true</c> [full synchronize].
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReportSyncWithDatasetRecordCountTimestampFullSyncInfoAreaId(
            string datasetName,
            int recordCount,
            string timestamp,
            bool fullSync,
            string infoAreaId)
        {
            var database = this.DatabaseInstance;
            var cTimestamp = !string.IsNullOrEmpty(timestamp) ? timestamp : null;
            var cInfoAreaId = !string.IsNullOrEmpty(infoAreaId) ? infoAreaId : null;
            return database.ReportSync(
                datasetName,
                recordCount,
                fullSync ? cTimestamp : null,
                cTimestamp,
                cInfoAreaId);
        }

        /// <summary>
        /// Lasts the synchronize of dataset.
        /// </summary>
        /// <param name="datasetName">
        /// Name of the dataset.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string LastSyncOfDataset(string datasetName)
        {
            string timestampBuffer = null;
            this.DatabaseInstance?.GetLastSyncOfDataset(datasetName, out timestampBuffer);
            return timestampBuffer;
        }

        /// <summary>
        /// Requests the option from string the default.
        /// </summary>
        /// <param name="requestOptionString">
        /// The request option string.
        /// </param>
        /// <param name="theDefaultRequestOption">
        /// The default request option.
        /// </param>
        /// <returns>
        /// The <see cref="UPRequestOption"/>.
        /// </returns>
        public static UPRequestOption RequestOptionFromString(string requestOptionString, UPRequestOption theDefaultRequestOption)
        {
            switch (requestOptionString)
            {
                case "Offline":
                    return UPRequestOption.Offline;

                case "Online":
                    return UPRequestOption.Online;

                case "Best":
                    return UPRequestOption.BestAvailable;

                case "Fastest":
                    return UPRequestOption.FastestAvailable;

                default:
                    return theDefaultRequestOption;
            }
        }

        /// <summary>
        /// Strings from request option.
        /// </summary>
        /// <param name="requestOption">
        /// The request option.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string StringFromRequestOption(UPRequestOption requestOption)
        {
            switch (requestOption)
            {
                case UPRequestOption.Offline:
                    return "Offline";

                case UPRequestOption.Online:
                    return "Online";

                case UPRequestOption.BestAvailable:
                    return "Best";

                case UPRequestOption.FastestAvailable:
                    return "Fastest";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Virtuals the information area identifier for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string VirtualInfoAreaIdForRecordIdentification(string recordIdentification)
        {
            var tableInfo = this.TableInfoForInfoArea(recordIdentification?.InfoAreaId());
            if (tableInfo == null)
            {
                return recordIdentification?.InfoAreaId();
            }

            var virtualInfoAreaId = tableInfo.VirtualInfoAreaIdForRecordId(recordIdentification.RecordId());
            if (virtualInfoAreaId != null)
            {
                return virtualInfoAreaId;
            }

            virtualInfoAreaId = this.VirtualInfoAreaCache.VirtualInfoAreaIdForRecordIdentification(recordIdentification)
                                ?? recordIdentification.InfoAreaId();

            return virtualInfoAreaId;
        }

        /// <summary>
        /// Determines whether [has offline data] [the specified information area identifier].
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasOfflineData(string infoAreaId)
        {
            if (string.IsNullOrEmpty(infoAreaId))
            {
                return false;
            }

            var num = this.offlineDataForInfoAreaId.ValueOrDefault(infoAreaId);
            if (num != null)
            {
                return (bool)num;
            }

            var tableInfo = this.TableInfoForInfoArea(infoAreaId);
            if (tableInfo != null)
            {
                var hasOfflineData = tableInfo.HasOfflineData;
                if (this.offlineDataForInfoAreaId == null)
                {
                    this.offlineDataForInfoAreaId = new Dictionary<string, object> { { infoAreaId, hasOfflineData } };
                }
                else
                {
                    this.offlineDataForInfoAreaId.SetObjectForKey(hasOfflineData, infoAreaId);
                }

                return hasOfflineData;
            }

            return false;
        }

        /// <summary>
        /// Roots the information area identifier for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RootInfoAreaIdForInfoAreaId(string infoAreaId)
        {
            var tableInfo = this.TableInfoForInfoArea(infoAreaId);
            return tableInfo != null ? tableInfo.RootInfoAreaId() : infoAreaId;
        }

        /// <summary>
        /// Roots the physical information area identifier for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RootPhysicalInfoAreaIdForInfoAreaId(string infoAreaId)
        {
            var tableInfo = this.TableInfoForInfoArea(infoAreaId);
            return tableInfo != null ? tableInfo.RootPhysicalInfoAreaId() : infoAreaId;
        }

        /// <summary>
        /// Catalogs the value for variable catalog identifier code.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CatalogValueForVariableCatalogIdCode(int catNo, int code)
        {
            var dataModel = this.DatabaseInstance?.DataModel;
            if (dataModel == null)
            {
                return null;
            }

            var catalogInfo = dataModel.GetVarCat(catNo);
            var value = catalogInfo?.GetCatalogText(code);
            return value;
        }

        /// <summary>
        /// Catalogs the value for fixed catalog identifier code.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CatalogValueForFixedCatalogIdCode(int catNo, int code)
        {
            var dataModel = this.DatabaseInstance?.DataModel;
            if (dataModel == null)
            {
                return null;
            }

            var catalogInfo = dataModel.GetFixCat(catNo);
            var value = catalogInfo?.GetCatalogText(code);
            return value;
        }

        /// <summary>
        /// Exts the key for variable catalog identifier code.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExtKeyForVariableCatalogIdCode(int catNo, int code)
        {
            var database = this.DatabaseInstance;
            var dataModel = database.DataModel;
            if (dataModel == null)
            {
                return null;
            }

            var catalogInfo = dataModel.GetVarCat(catNo);
            var value = catalogInfo?.GetExternalKey(code);
            return value;
        }

        /// <summary>
        /// Catalogs the value for variable catalog identifier raw value.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CatalogValueForVariableCatalogIdRawValue(int catNo, string rawValue)
        {
            int code;
            int.TryParse(rawValue, out code);
            return code > 0 ? this.CatalogValueForVariableCatalogIdCode(catNo, code) : null;
        }

        /// <summary>
        /// Catalogs the value for fixed catalog identifier raw value.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CatalogValueForFixedCatalogIdRawValue(int catNo, string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                return null;
            }

            int code;
            return int.TryParse(rawValue, out code) ? this.CatalogValueForFixedCatalogIdCode(catNo, code) : null;
        }

        /// <summary>
        /// Exts the key for variable catalog identifier raw value.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExtKeyForVariableCatalogIdRawValue(int catNo, string rawValue)
        {
            int code;
            int.TryParse(rawValue, out code);
            return code > 0 ? this.ExtKeyForVariableCatalogIdCode(catNo, code) : null;
        }

        /// <summary>
        /// Informations the area identifier link identifier is parent of information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="childInfoAreaId">
        /// The child information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool InfoAreaIdLinkIdIsParentOfInfoAreaId(string infoAreaId, int linkId, string childInfoAreaId)
        {
            var tableInfo = this.TableInfoForInfoArea(infoAreaId);
            return tableInfo?.ParentOfInfoAreaIdLinkId(childInfoAreaId, linkId) ?? false;
        }

        /// <summary>
        /// Updates the links from record identifier to record identifier.
        /// </summary>
        /// <param name="linkInfos">
        /// The link infos.
        /// </param>
        /// <param name="fromRecordId">
        /// From record identifier.
        /// </param>
        /// <param name="toRecordId">
        /// To record identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdateLinksFromRecordIdToRecordId(List<UPCRMLinksToUpdate> linkInfos, string fromRecordId, string toRecordId)
        {
            var database = this.DatabaseInstance;
            database.BeginTransaction();
            var ret = false;
            foreach (UPCRMLinksToUpdate linkToUpdate in linkInfos)
            {
                if (linkToUpdate.UpdateFromRecordIdToRecordId(fromRecordId, toRecordId))
                {
                    ret = true;
                }
            }

            database.Commit();
            return ret;
        }

        /// <summary>
        /// Analyzes the database.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AnalyzeDB()
        {
            var database = this.DatabaseInstance;
            var statement = new DatabaseStatement(database);
            var ret = statement.Execute("ANALYZE");
            return ret;
        }
    }

    /// <summary>
    /// CRM links to update
    /// </summary>
    public class UPCRMLinksToUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinksToUpdate"/> class.
        /// </summary>
        /// <param name="linkInfo">
        /// The link information.
        /// </param>
        /// <param name="db">
        /// The database.
        /// </param>
        public UPCRMLinksToUpdate(UPCRMLinkInfo linkInfo, IDatabase db)
        {
            this.LinkInfo = linkInfo;
            this.Database = db;
            this.RecordIds = new List<object>();
        }

        /// <summary>
        /// Gets the link information.
        /// </summary>
        /// <value>
        /// The link information.
        /// </value>
        public UPCRMLinkInfo LinkInfo { get; private set; }

        /// <summary>
        /// Gets the record ids.
        /// </summary>
        /// <value>
        /// The record ids.
        /// </value>
        public List<object> RecordIds { get; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        private IDatabase Database { get; set; }

        /// <summary>
        /// Adds the record identifier.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        public void AddRecordId(string recordId)
        {
            this.RecordIds.Add(recordId);
        }

        /// <summary>
        /// Updates from record identifier to record identifier transaction.
        /// </summary>
        /// <param name="fromRecordId">
        /// From record identifier.
        /// </param>
        /// <param name="toRecordId">
        /// To record identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdateFromRecordIdToRecordId(string fromRecordId, string toRecordId)
        {
            if (!this.LinkInfo.HasColumn)
            {
                return false;
            }

            string statement;
            bool ret;
            var updateStatement = new DatabaseStatement(this.Database);
            var linkInfo = this.LinkInfo.LinkInfo;
            if (linkInfo.IsGeneric)
            {
                statement =
                    $"UPDATE CRM_{linkInfo.InfoAreaId} SET {linkInfo.ColumnName} = ? WHERE recid = ? AND {linkInfo.ColumnName} = ? AND {linkInfo.InfoAreaColumnName} = ?";
                ret = updateStatement.Prepare(statement);
            }
            else
            {
                statement = $"UPDATE CRM_{linkInfo.InfoAreaId} SET {linkInfo.ColumnName} = ? WHERE recid = ? AND {linkInfo.ColumnName}= ?";
                ret = updateStatement.Prepare(statement);
            }

            if (!ret)
            {
                return false;
            }

            foreach (string recId in this.RecordIds)
            {
                updateStatement.Reset();
                updateStatement.Bind(1, toRecordId);
                updateStatement.Bind(2, recId);
                updateStatement.Bind(3, fromRecordId);
                if (linkInfo.IsGeneric)
                {
                    updateStatement.Bind(4, linkInfo.TargetInfoAreaId);
                }

                ret = updateStatement.Execute() > 0;
            }

            return !ret;
        }
    }
}
