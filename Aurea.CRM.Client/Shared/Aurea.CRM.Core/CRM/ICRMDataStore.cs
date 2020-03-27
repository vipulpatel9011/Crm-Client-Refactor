// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICRMDataStore.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.CRM
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.DAL;

    /// <summary>
    /// Interface for local CRM data store functionality
    /// </summary>
    public interface ICRMDataStore
    {
        /// <summary>
        /// Gets the reps.
        /// </summary>
        /// <value>
        /// The reps.
        /// </value>
        UPCRMReps Reps { get; }

        /// <summary>
        /// Gets the virtual information area cache.
        /// </summary>
        /// <value>
        /// The virtual information area cache.
        /// </value>
        UPVirtualInfoAreaCache VirtualInfoAreaCache { get; }

        /// <summary>
        /// Gets the base directory path.
        /// </summary>
        /// <value>
        /// The base directory path.
        /// </value>
        string BaseDirectoryPath { get; }

        /// <summary>
        /// Gets the database instance.
        /// </summary>
        /// <value>
        /// The database instance.
        /// </value>
        CRMDatabase DatabaseInstance { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMDataStore"/> is disable86326.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable86326; otherwise, <c>false</c>.
        /// </value>
        bool Disable86326 { get; }

        /// <summary>
        /// Adds the virtual links.
        /// </summary>
        void AddVirtualLinks();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="recreate">
        /// if set to <c>true</c> [recreate].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool DeleteDatabase(bool recreate);

        /// <summary>
        /// Renames the database to default.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool RenameDatabaseToDefault();

        /// <summary>
        /// Updates the DDL.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool UpdateDDL();

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
        bool SetTimeZoneUtcDifference(string timezone, int utcDifference);

        /// <summary>
        /// Times the name of the zone.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string TimeZoneName();

        /// <summary>
        /// Stores the data model.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int StoreDataModel(DAL.DataModel dataModel);

        /// <summary>
        /// Resets the data model.
        /// </summary>
        void ResetDataModel();

        /// <summary>
        /// Determines whether [is valid information area] [the specified information area identifier].
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsValidInfoArea(string infoAreaId);

        /// <summary>
        /// Tables the information for information area.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMTableInfo"/>.
        /// </returns>
        UPCRMTableInfo TableInfoForInfoArea(string infoAreaId);

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
        UPCRMFieldInfo FieldInfoForInfoAreaFieldId(string infoAreaId, int fieldId);

        /// <summary>
        /// Fields the information for field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldInfo"/>.
        /// </returns>
        UPCRMFieldInfo FieldInfoForField(UPCRMField field);

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
        UPCRMLinkInfo LinkInfoForInfoAreaTargetInfoAreaLinkId(
            string infoAreaId,
            string targetInfoAreaId,
            int linkId);

        /// <summary>
        /// Idents the link for information area.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkInfo"/>.
        /// </returns>
        UPCRMLinkInfo IdentLinkForInfoArea(string infoAreaId);

        /// <summary>
        /// Catalogs for variable catalog identifier.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalog"/>.
        /// </returns>
        UPCatalog CatalogForVariableCatalogId(int catNo);

        /// <summary>
        /// Catalogs for fixed catalog identifier.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalog"/>.
        /// </returns>
        UPCatalog CatalogForFixedCatalogId(int catNo);

        /// <summary>
        /// Catalogs for CRM field.
        /// </summary>
        /// <param name="crmField">
        /// The CRM field.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalog"/>.
        /// </returns>
        UPCatalog CatalogForCrmField(UPCRMField crmField);

        /// <summary>
        /// Records the exists offline.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool RecordExistsOffline(string recordIdentification);

        /// <summary>
        /// Replaces the record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string ReplaceRecordIdentification(string recordIdentification);

        /// <summary>
        /// Deletes the record with identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int DeleteRecordWithIdentification(string recordIdentification);

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
        int DeleteRecordWithIdentificationRollbackRequestNr(string recordIdentification, int requestNr);

        /// <summary>
        /// Creates the index for.
        /// </summary>
        /// <param name="crmField">
        /// The CRM field.
        /// </param>
        /// <param name="namePrefix">
        /// The name prefix.
        /// </param>
        void CreateIndexFor(UPCRMField crmField, string namePrefix);

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
        void CreateIndexFor(string infoAreaId, List<object> columns, string prefix);

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="undo">if set to <c>true</c> [undo].</param>
        /// <returns>Record</returns>
        DAL.Record CreateRecord(UPCRMRecord record, bool undo);

        /// <summary>
        /// Creates the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>Returns the newly created record</returns>
        Record CreateRecord(UPCRMRecord record);

        /// <summary>
        /// Saves the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>Returns error code for the operation or 0 for success</returns>
        int SaveRecord(UPCRMRecord record);

        /// <summary>
        /// Saves the record rollback request nr.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="requestNr">The request nr.</param>
        /// <returns>Returns error code for the operation or 0 for success</returns>
        int SaveRecordRollbackRequestNr(UPCRMRecord record, int requestNr);

        /// <summary>
        /// Undoes the record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns>Returns error code for the operation or 0 for success</returns>
        int UndoRecord(UPCRMRecord record);

        /// <summary>
        /// Updates the participants tables for record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="database">The database.</param>
        /// <returns>Returns error code for the operation or 0 for success</returns>
        int UpdateParticipantsTablesForRecord(Record record, DatabaseBase database);

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
        int ReportSyncWithDatasetRecordCountTimestampFullSyncInfoAreaId(
            string datasetName,
            int recordCount,
            string timestamp,
            bool fullSync,
            string infoAreaId);

        /// <summary>
        /// Lasts the synchronize of dataset.
        /// </summary>
        /// <param name="datasetName">
        /// Name of the dataset.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string LastSyncOfDataset(string datasetName);

        /// <summary>
        /// Virtuals the information area identifier for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string VirtualInfoAreaIdForRecordIdentification(string recordIdentification);

        /// <summary>
        /// Determines whether [has offline data] [the specified information area identifier].
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool HasOfflineData(string infoAreaId);

        /// <summary>
        /// Roots the information area identifier for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string RootInfoAreaIdForInfoAreaId(string infoAreaId);

        /// <summary>
        /// Roots the physical information area identifier for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string RootPhysicalInfoAreaIdForInfoAreaId(string infoAreaId);

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
        string CatalogValueForVariableCatalogIdCode(int catNo, int code);

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
        string CatalogValueForFixedCatalogIdCode(int catNo, int code);

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
        string ExtKeyForVariableCatalogIdCode(int catNo, int code);

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
        string CatalogValueForVariableCatalogIdRawValue(int catNo, string rawValue);

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
        string CatalogValueForFixedCatalogIdRawValue(int catNo, string rawValue);

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
        string ExtKeyForVariableCatalogIdRawValue(int catNo, string rawValue);

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
        bool InfoAreaIdLinkIdIsParentOfInfoAreaId(string infoAreaId, int linkId, string childInfoAreaId);

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
        bool UpdateLinksFromRecordIdToRecordId(List<UPCRMLinksToUpdate> linkInfos, string fromRecordId, string toRecordId);

        /// <summary>
        /// Analyzes the database.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int AnalyzeDB();
    }
}
