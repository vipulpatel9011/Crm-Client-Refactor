// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CRMDatabase.cs" company="Aurea Software Gmbh">
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
//   Implements primary CRM database access
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM
{
    using System;
    using System.Text;
    using Aurea.CRM.Core.DAL;

    /// <summary>
    /// Implements prmary CRM database access
    /// </summary>
    /// <seealso cref="DatabaseBase" />
    public class CRMDatabase : DatabaseBase
    {
        private const string FieldArrayFieldIndices = "arrayfieldindices";
        private const string FieldFormat = "format";
        private const string FieldHasLookup = "haslookup";
        private const string FieldInfoAreaId = "infoareaid";
        private const string FieldRights = "rights";
        private const string FieldSourceFieldId = "sourceFieldId";
        private const string FieldSourceValue = "sourceValue";
        private const string FieldUseLinkFields = "useLinkFields";
        private const string TableDataModel = "datamodel";
        private const string TableFieldInfo = "fieldinfo";
        private const string TableLinkInfo = "linkinfo";
        private const string TableLinkFields = "linkfields";
        private const string TableQueryResultTable = "queryresulttable";
        private const string TableSyncInfo = "syncinfo";
        private const string TableTableInfo = "tableInfo";

        /// <summary>
        /// The lock object used when accesing the data base
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// The data model version.
        /// </summary>
        private string dataModelVersion;

        /// <summary>
        /// The supports timezones.
        /// </summary>
        private bool supportsTimezones;

        /// <summary>
        /// The time zone name.
        /// </summary>
        private string timeZoneName;

        /// <summary>
        /// The utc offset.
        /// </summary>
        private int utcOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="CRMDatabase"/> class.
        /// </summary>
        /// <param name="isUpdateCrm">
        /// if set to <c>true</c> [is update CRM].
        /// </param>
        /// <param name="databaseFileName">
        /// Name of the database file.
        /// </param>
        private CRMDatabase(bool isUpdateCrm, string databaseFileName)
            : base(databaseFileName)
        {
            this.IsUpdateCrm = isUpdateCrm;

            this.EnsureDDL();
            this.DataModel = new DAL.DataModel(this);
            this.DataModel.Load();
        }

        /// <summary>
        /// Gets the data model.
        /// </summary>
        /// <value>
        /// The data model.
        /// </value>
        public DAL.DataModel DataModel { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [fixed cat sort by sort information and code].
        /// </summary>
        /// <value>
        /// <c>true</c> if [fixed cat sort by sort information and code]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedCatSortBySortInfoAndCode { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has lookup.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has lookup; otherwise, <c>false</c>.
        /// </value>
        public bool HasLookup => true;

        /// <summary>
        /// Gets a value indicating whether this instance is update CRM.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is update CRM; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateCrm { get; }

        /// <summary>
        /// Creates the specified is update CRM.
        /// </summary>
        /// <param name="isUpdateCrm">
        /// if set to <c>true</c> [is update CRM].
        /// </param>
        /// <param name="databaseFileName">
        /// Name of the database file.
        /// </param>
        /// <returns>
        /// The <see cref="CRMDatabase"/>.
        /// </returns>
        public static CRMDatabase Create(bool isUpdateCrm, string databaseFileName)
        {
            return new CRMDatabase(isUpdateCrm, databaseFileName);
        }

        /// <summary>
        /// Creates the insert field information statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement CreateInsertFieldInfoStatement()
        {
            var statement = new DatabaseStatement(this);
            var ret =
                statement.Prepare(
                    "INSERT INTO fieldinfo (infoareaid, fieldid, xmlName, name, fieldtype, fieldlen, cat, ucat, attributes, repMode, rights, format, arrayfieldindices) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)");
            return ret ? statement : null;
        }

        /// <summary>
        /// Creates the insert fix cat statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement CreateInsertFixCatStatement()
        {
            var statement = new DatabaseStatement(this);
            var ret = statement.Prepare("INSERT INTO fixcatinfo (catnr) VALUES (?)");
            return ret ? statement : null;
        }

        /// <summary>
        /// Creates the insert link field information statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement CreateInsertLinkFieldInfoStatement()
        {
            var statement = new DatabaseStatement(this);
            var ret =
                statement.Prepare(
                    "INSERT INTO linkfields (infoareaid, targetinfoareaid, linkid, nr, sourceFieldId, destFieldId, sourceValue, destValue) VALUES (?,?,?,?,?,?,?,?)");
            return ret ? statement : null;
        }

        /// <summary>
        /// Creates the insert link information statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement CreateInsertLinkInfoStatement()
        {
            var statement = new DatabaseStatement(this);
            var ret =
                statement.Prepare(
                    "INSERT INTO linkinfo (infoareaid, targetinfoareaid, linkid, relationtype, reverseLinkId, sourceFieldId, destFieldId, useLinkFields) VALUES (?,?,?,?,?,?,?,?)");
            return ret ? statement : null;
        }

        /// <summary>
        /// Creates the insert table information statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement CreateInsertTableInfoStatement()
        {
            var statement = new DatabaseStatement(this);
            var ret = statement.Prepare("INSERT INTO tableinfo (infoareaid, rootinfoareaid, name) VALUES (?, ?, ?)");
            return ret ? statement : null;
        }

        /// <summary>
        /// Creates the insert variable cat statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement CreateInsertVarCatStatement()
        {
            var statement = new DatabaseStatement(this);
            var ret = statement.Prepare("INSERT INTO varcatinfo (catnr, parentcatnr) VALUES (?,?)");
            return ret ? statement : null;
        }

        /// <summary>
        /// Ensures the existance of the database.
        /// </summary>
        /// <returns>
        /// true if exists;else false
        /// </returns>
        public override bool EnsureDDL()
        {
            var ret = !this.ExistsTable("tableinfo")
                ? this.CreateTables()
                : this.AlterTables();

            if (ret == 0 && !this.ExistsTable("rollbackinfo"))
            {
                ret = this.CreateTableRollbackInfo();
            }

            if (ret == 0)
            {
                return true;
            }

            this.SetVersionTimezoneUtcOffsetProperties();

            return true;
        }

        /// <summary>
        /// Gets the last synchronize of dataset.
        /// </summary>
        /// <param name="datasetname">
        /// The datasetname.
        /// </param>
        /// <param name="timestamp">
        /// The timestamp.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetLastSyncOfDataset(string datasetname, out string timestamp)
        {
            var recordSet = new DatabaseRecordSet(this);
            if (recordSet.Execute($"SELECT synctimestamp FROM syncinfo WHERE datasetname = '{datasetname}'", 1) == 0
                && recordSet.GetRowCount() > 0)
            {
                var row = recordSet.GetRow(0);
                timestamp = row.GetColumn(0);
                if (!string.IsNullOrEmpty(timestamp))
                {
                    return 1;
                }
            }

            timestamp = null;
            return 0;
        }

        /// <summary>
        /// Gets the table information by information area.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo GetTableInfoByInfoArea(string infoAreaId)
        {
            return this.DataModel?.InternalGetTableInfo(infoAreaId);
        }

        /// <summary>
        /// Determines whether [has lookup records] [the specified information area identifier].
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasLookupRecords(string infoAreaId)
        {
            return this.DataModel == null || this.DataModel.HasLookupRecords(infoAreaId);
        }

        /// <summary>
        /// Reports the synchronize.
        /// </summary>
        /// <param name="datasetname">
        /// The datasetname.
        /// </param>
        /// <param name="recordCount">
        /// The record count.
        /// </param>
        /// <param name="fullSyncTimestamp">
        /// The full synchronize timestamp.
        /// </param>
        /// <param name="lastSyncTimestamp">
        /// The last synchronize timestamp.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReportSync(
            string datasetname,
            int recordCount,
            string fullSyncTimestamp,
            string lastSyncTimestamp,
            string infoAreaId)
        {
            this.BeginTransaction();
            var ret = 0;
            var parameters = new object[5];
            parameters[0] = datasetname;

            if (string.IsNullOrEmpty(lastSyncTimestamp))
            {
                lastSyncTimestamp = fullSyncTimestamp;
            }

            var recordCountBuffer = $"{recordCount}";

            if (this.ExistsRow($"SELECT * FROM syncinfo WHERE datasetname = '{datasetname}'"))
            {
                if (!string.IsNullOrEmpty(fullSyncTimestamp))
                {
                    parameters[0] = recordCountBuffer;
                    parameters[1] = fullSyncTimestamp;
                    parameters[2] = lastSyncTimestamp;
                    parameters[3] = infoAreaId;
                    parameters[4] = datasetname;
                    ret =
                        this.Execute(
                            "UPDATE syncinfo SET recordcount = ?, fullsynctimestamp = ?, synctimestamp = ?, infoareaid = ? WHERE datasetname = ?",
                            parameters);
                }
                else
                {
                    parameters[0] = lastSyncTimestamp;
                    parameters[1] = datasetname;
                    ret = this.Execute("UPDATE syncinfo SET synctimestamp = ? WHERE datasetname = ?", parameters);
                }
            }
            else
            {
                parameters[0] = datasetname;
                parameters[1] = recordCountBuffer;
                parameters[2] = fullSyncTimestamp;
                parameters[3] = lastSyncTimestamp;
                parameters[4] = infoAreaId;
                ret =
                    this.Execute(
                        "INSERT INTO syncinfo (datasetname, recordcount, fullsynctimestamp, synctimestamp, infoareaid) VALUES (?,?,?,?,?)",
                        parameters);
            }

            this.Commit();
            return ret;
        }

        /// <summary>
        /// Resets the data model.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ResetDataModel()
        {
            if (this.DataModel != null)
            {
                // delete _dataModel; // DANGEROUS
            }

            this.DataModel = new DAL.DataModel(this);
            return this.DataModel.Load();
        }

        /// <summary>
        /// Sets the has lookup records.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetHasLookupRecords(string infoAreaId)
        {
            var statement = new DatabaseStatement(this);
            var ret = statement.Prepare("UPDATE tableinfo SET haslookup = 1 WHERE infoareaid = ?");
            if (ret)
            {
                statement.Bind(infoAreaId);
            }

            statement.ExecuteNonQuery();

            this.DataModel?.SetHasLookupRecords(infoAreaId);

            return true;
        }

        /// <summary>
        /// Writes the field information.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int WriteFieldInfo(DatabaseStatement statement, FieldInfo fieldInfo)
        {
            if (statement == null)
            {
                return 0;
            }

            try
            {
                int arrayFieldCount;
                statement.Reset();
                statement.Bind(1, fieldInfo.InfoAreaId);
                statement.Bind(2, fieldInfo.FieldId);
                statement.Bind(3, fieldInfo.XmlName);
                statement.Bind(4, fieldInfo.Name);
                statement.Bind(5, $"{fieldInfo.FieldType}");
                statement.Bind(6, fieldInfo.FieldLen);
                statement.Bind(7, fieldInfo.Cat);
                statement.Bind(8, fieldInfo.UCat);
                statement.Bind(9, fieldInfo.Attributes);

                var repMode = fieldInfo.RepMode;
                statement.Bind(10, !string.IsNullOrEmpty(repMode) ? repMode : null);

                statement.Bind(11, fieldInfo.Rights);
                statement.Bind(12, fieldInfo.Format);

                arrayFieldCount = fieldInfo.ArrayFieldCount;

                if (arrayFieldCount > 0)
                {
                    var curpos = string.Empty;
                    for (var i = 0; i < arrayFieldCount; i++)
                    {
                        if (i == 0)
                        {
                            curpos += $"{fieldInfo.ArrayFieldIndices[i]}";
                        }
                        else
                        {
                            curpos += $",{fieldInfo.ArrayFieldIndices[i]}";
                        }
                    }

                    statement.Bind(13, curpos);
                }
                else
                {
                    statement.Bind(13, null);
                }

                statement.ExecuteNonQuery();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Writes the fix cat information.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <param name="catalogInfo">
        /// The catalog information.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int WriteFixCatInfo(DatabaseStatement statement, CatalogInfo catalogInfo)
        {
            if (statement == null)
            {
                return 0;
            }

            try
            {
                statement.Reset();
                statement.Bind(1, catalogInfo.CatalogNr);
                statement.ExecuteNonQuery();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Writes the link information.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <param name="linkInfo">
        /// The link information.
        /// </param>
        /// <param name="linkFieldStatement">
        /// The link field statement.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int WriteLinkInfo(DatabaseStatement statement, LinkInfo linkInfo, DatabaseStatement linkFieldStatement)
        {
            if (statement == null)
            {
                return 0;
            }

            try
            {
                statement.Reset();
                statement.Bind(1, linkInfo.InfoAreaId);
                statement.Bind(2, linkInfo.TargetInfoAreaId);
                statement.Bind(3, linkInfo.LinkId);
                statement.Bind(4, linkInfo.RelationType);
                statement.Bind(5, linkInfo.ReverseLinkId);
                statement.Bind(6, linkInfo.SourceFieldId);
                statement.Bind(7, linkInfo.DestFieldId);
                statement.Bind(8, linkInfo.LinkFlag);

                statement.ExecuteNonQuery();

                var linkFieldCount = linkInfo.LinkFieldCount;
                for (var i = 0; i < linkFieldCount; i++)
                {
                    linkFieldStatement.Reset();
                    linkFieldStatement.Bind(1, linkInfo.InfoAreaId);
                    linkFieldStatement.Bind(2, linkInfo.TargetInfoAreaId);
                    linkFieldStatement.Bind(3, linkInfo.LinkId);
                    linkFieldStatement.Bind(4, i);
                    linkFieldStatement.Bind(5, linkInfo.GetSourceFieldIdWithIndex(i));
                    linkFieldStatement.Bind(6, linkInfo.GetDestinationFieldIdWithIndex(i));
                    linkFieldStatement.Bind(7, linkInfo.GetSourceValueWithIndex(i));
                    linkFieldStatement.Bind(8, linkInfo.GetDestinationValueWithIndex(i));
                    linkFieldStatement.ExecuteNonQuery();
                }

                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        /// <summary>
        /// Writes the table information.
        /// </summary>
        /// <param name="tableInfo">
        /// The table information.
        /// </param>
        /// <param name="tableInsertStatement">
        /// The table insert statement.
        /// </param>
        /// <param name="fieldInsertStatement">
        /// The field insert statement.
        /// </param>
        /// <param name="linkInsertStatement">
        /// The link insert statement.
        /// </param>
        /// <param name="linkFieldInsertStatement">
        /// The link field insert statement.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int WriteTableInfo(
            TableInfo tableInfo,
            DatabaseStatement tableInsertStatement,
            DatabaseStatement fieldInsertStatement,
            DatabaseStatement linkInsertStatement,
            DatabaseStatement linkFieldInsertStatement)
        {
            tableInsertStatement.Reset();
            tableInsertStatement.Bind(1, tableInfo.InfoAreaId);
            tableInsertStatement.Bind(2, tableInfo.RootInfoAreaId);
            tableInsertStatement.Bind(3, tableInfo.Name);
            tableInsertStatement.ExecuteNonQuery();

            var count = tableInfo.FieldCount;
            for (var i = 0; i < count; i++)
            {
                var fieldInfo = tableInfo.GetFieldInfoByIndex(i);
                this.WriteFieldInfo(fieldInsertStatement, fieldInfo);
            }

            count = tableInfo.LinkCount;
            for (var i = 0; i < count; i++)
            {
                var linkInfo = tableInfo.GetLink(i);
                this.WriteLinkInfo(linkInsertStatement, linkInfo, linkFieldInsertStatement);
            }

            return 0;
        }

        /// <summary>
        /// Writes the time zone information.
        /// </summary>
        /// <param name="timezoneName">
        /// Name of the timezone.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool WriteTimeZoneInformation(string timezoneName, int offset)
        {
            var statement = new DatabaseStatement(this);
            var ret = statement.Prepare("UPDATE datamodel SET timezone = ?, utctimeoffset = ?");
            if (!ret)
            {
                if (!string.IsNullOrEmpty(timezoneName))
                {
                    statement.Bind(timezoneName);
                    statement.Bind(offset);
                }
                else
                {
                    statement.Bind(null);
                    statement.Bind(0);
                }
            }

            ret = statement.ExecuteNonQuery() != 0;
            return ret;
        }

        /// <summary>
        /// Writes the variable cat information.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        /// <param name="catalogInfo">
        /// The catalog information.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int WriteVarCatInfo(DatabaseStatement statement, CatalogInfo catalogInfo)
        {
            if (statement == null)
            {
                return 0;
            }

            try
            {
                statement.Reset();
                statement.Bind(1, catalogInfo.CatalogNr);
                statement.Bind(2, catalogInfo.IsDependent ? (object)catalogInfo.ParentCatalogNr : null);
                statement.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Initializes this database.
        /// typically used to create the database tables, etc...
        /// </summary>
        protected override void Init()
        {
        }

        /// <summary>
        /// Provide the lock object.
        /// </summary>
        /// <returns>
        /// the lock object
        /// </returns>
        protected override object LockObject()
        {
            return Locker;
        }

        /// <summary>
        /// Creates Database Tables
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTables()
        {
            var ret = 0;
            ret = CreateTableDataModel();
            if (ret == 0)
            {
                ret = CreateTableTableInfo();
            }

            if (ret == 0)
            {
                ret = CreateTableFieldInfo();
            }

            if (ret == 0)
            {
                ret = CreateTableLinkInfo();
            }

            if (ret == 0)
            {
                ret = CreateTableLinkFields(true);
            }

            if (ret == 0)
            {
                ret = CreateTableVarCatInfo();
            }

            if (ret == 0)
            {
                ret = CreateTableFixCatInfo();
            }

            if (ret == 0)
            {
                ret = CreateTableQueryResult();
            }

            if (ret == 0)
            {
                ret = CreateTableQueryResultTable();
            }

            if (ret == 0)
            {
                ret = CreateTableQueryResultField();
            }

            if (ret == 0)
            {
                ret = CreateTableSyncInfo();
            }

            if (ret == 0)
            {
                ret = InsertDefaultDataModelRecords();
            }

            return ret;
        }

        /// <summary>
        /// Alters Database Tables
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int AlterTables()
        {
            var ret = AlterTableDataModel();
            if (ret == 0)
            {
                ret = AlterTableTableInfo();
            }

            if (ret == 0)
            {
                ret = AlterTableLinkInfo();
            }

            if (ret == 0)
            {
                ret = AlterTableLinkFields();
            }

            if (ret == 0)
            {
                ret = AlterTableFieldInfo();
            }

            if (ret == 0 && !ExistsTable(TableQueryResultTable))
            {
                ret = CreateTableQueryResultTable();
                if (ret == 0)
                {
                    ret = CreateTableQueryResultField();
                }
            }

            if (ret == 0)
            {
                if (!ExistsTable(TableSyncInfo))
                {
                    ret = CreateTableSyncInfo();
                }
                else
                {
                    ret = AlterTableSyncInfo();
                }
            }

            if (ret == 0 && !ExistsTable(TableLinkFields))
            {
                ret = CreateTableLinkFields(!false);
            }

            return ret;
        }

        /// <summary>
        /// Creata Table Rollback Info
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableRollbackInfo()
        {
            var query = new StringBuilder("CREATE TABLE rollbackinfo (")
                .Append("requestnr INT,")
                .Append("infoareaid TEXT,")
                .Append("recordid TEXT,")
                .Append("rollbackinfo TEXT)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Sets Version, Timezone, UtcOffset property of <see cref="CRMDatabase"/>
        /// </summary>
        private void SetVersionTimezoneUtcOffsetProperties()
        {
            var recordSet = new DatabaseRecordSet(this);
            recordSet.Query.Prepare("SELECT version, timezone, utctimeoffset FROM datamodel");
            var ret = recordSet.Execute(1);

            if (ret > 0 && recordSet.GetRowCount() > 0)
            {
                var row = recordSet.GetRow(0);
                dataModelVersion = row?.GetColumn(0);

                if (row?.GetColumn(1) != null)
                {
                    supportsTimezones = true;
                    utcOffset = row.GetColumnInt(2);
                    if (!row.IsNull(1))
                    {
                        timeZoneName = row.GetColumn(1);
                    }
                }
                else
                {
                    supportsTimezones = false;
                    utcOffset = 0;
                }
            }
        }

        /// <summary>
        /// Creates Table Data Model
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableDataModel()
        {
            return Execute("CREATE TABLE datamodel (version TEXT, timezone TEXT, utctimeoffset INTEGER)");
        }

        /// <summary>
        /// Creates Table TableInfo
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableTableInfo()
        {
            return Execute("CREATE TABLE tableinfo (infoareaid TEXT, rootinfoareaid TEXT, name TEXT, haslookup INTEGER)");
        }

        /// <summary>
        /// Creates Table FieldInfo
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableFieldInfo()
        {
            var query = new StringBuilder("CREATE TABLE fieldinfo (")
                .Append("infoareaid TEXT,")
                .Append("fieldid INTEGER,")
                .Append("xmlname TEXT,")
                .Append("name TEXT,")
                .Append("fieldtype CHAR,")
                .Append("fieldlen INTEGER,")
                .Append("cat INTEGER,")
                .Append("ucat INTEGER,")
                .Append("attributes INTEGER,")
                .Append("repMode TEXT,")
                .Append("rights INTEGER,")
                .Append("format INTEGER,")
                .Append("arrayfieldindices TEXT)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Creates Table LinkInfo
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableLinkInfo()
        {
            var query = new StringBuilder("CREATE TABLE linkinfo (")
                .Append("infoareaid TEXT,")
                .Append("targetinfoareaid TEXT,")
                .Append("linkid INTEGER,")
                .Append("relationtype INTEGER,")
                .Append("reverseLinkId INTEGER,")
                .Append("sourceFieldId INTEGER,")
                .Append("destFieldId INTEGER,")
                .Append("useLinkFields INTEGER)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Creates Table LinkFields
        /// </summary>
        /// <param name="appendValueFields">
        /// Add source/dest value fields or not
        /// </param>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableLinkFields(bool appendValueFields)
        {
            var query = new StringBuilder("CREATE TABLE linkfields (")
                .Append("infoareaid TEXT,")
                .Append("targetinfoareaid TEXT,")
                .Append("linkid INTEGER,")
                .Append("nr INTEGER,")
                .Append("sourceFieldId INTEGER,");

            if (appendValueFields)
            {
                query.Append("destFieldId INTEGER,")
                    .Append("sourceValue TEXT,")
                    .Append("destValue TEXT)");
            }
            else
            {
                query.Append("targetFieldId INTEGER)");
            }

            return Execute(query.ToString());
        }

        /// <summary>
        /// Create Table VarCarInfo
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableVarCatInfo()
        {
            return Execute("CREATE TABLE varcatinfo (catnr INTEGER, parentcatnr INTEGER)");
        }

        /// <summary>
        /// Create TAble FixCatInfo
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableFixCatInfo()
        {
            return Execute("CREATE TABLE fixcatinfo (catnr INTEGER)");
        }

        /// <summary>
        /// Create Table QueryResult
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableQueryResult()
        {
            return Execute("CREATE TABLE queryresult (queryid INTEGER, name TEXT, definition TEXT, sync TEXT)");
        }

        /// <summary>
        /// Create Table QueryResultTable
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableQueryResultTable()
        {
            var query = new StringBuilder("CREATE TABLE queryresulttable (")
                .Append("queryid INTEGER,")
                .Append("parenttablenr INTEGER,")
                .Append("tablenr INTEGER,")
                .Append("relation TEXT,")
                .Append("infoareaid TEXT,")
                .Append("alias TEXT)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Create Table QueryResultField
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableQueryResultField()
        {
            return Execute("CREATE TABLE queryresultfield (queryid INTEGER, tablenr INTEGER, fieldnr INTEGER, fieldid INTEGER)");
        }

        /// <summary>
        /// Create Table SyncInfo
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int CreateTableSyncInfo()
        {
            var query = new StringBuilder("CREATE TABLE syncinfo (")
                .Append("datasetname TEXT, ")
                .Append("recordcount INTEGER, ")
                .Append("fullsynctimestamp TEXT, ")
                .Append("synctimestamp TEXT, ")
                .Append("infoareaid TEXT)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Insert default rows in DataModel Tables
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int InsertDefaultDataModelRecords()
        {
            return Execute("INSERT INTO datamodel(version) VALUES('2.0')");
        }

        /// <summary>
        /// Alters DataModel Table
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int AlterTableDataModel()
        {
            var ret = 0;
            var tableInfoMetaInfo = CreateTableMetaInfo(TableDataModel);
            if (tableInfoMetaInfo != null && tableInfoMetaInfo.GetField("timezone") == null)
            {
                ret = Execute("ALTER TABLE datamodel ADD COLUMN timezone TEXT");
                if (ret == 0)
                {
                    ret = Execute("ALTER TABLE datamodel ADD COLUMN utctimeoffset INTEGER");
                }
            }

            return ret;
        }

        /// <summary>
        /// Alters TableInfo Table
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int AlterTableTableInfo()
        {
            var ret = 0;
            var tableInfoMetaInfo = CreateTableMetaInfo(TableTableInfo);
            if (tableInfoMetaInfo != null)
            {
                if (tableInfoMetaInfo.GetField(FieldHasLookup) == null)
                {
                    ret = Execute("ALTER TABLE tableinfo ADD COLUMN haslookup INTEGER");
                }
            }

            return ret;
        }

        /// <summary>
        /// Alter LinkInfo Table
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int AlterTableLinkInfo()
        {
            var ret = 0;
            var linkInfoMetaInfo = CreateTableMetaInfo(TableLinkInfo);
            if (linkInfoMetaInfo != null)
            {
                if (linkInfoMetaInfo.GetField(FieldSourceFieldId) == null)
                {
                    ret = Execute("ALTER TABLE linkinfo ADD COLUMN sourceFieldId INTEGER");
                    if (ret == 0)
                    {
                        ret = Execute("ALTER TABLE linkinfo ADD COLUMN destFieldId INTEGER");
                    }
                }

                if (linkInfoMetaInfo.GetField(FieldUseLinkFields) == null)
                {
                    ret = Execute("ALTER TABLE linkinfo ADD COLUMN useLinkFields INTEGER");
                }
            }

            return ret;
        }

        /// <summary>
        /// Alters LinkFields Table
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int AlterTableLinkFields()
        {
            var ret = 0;
            var linkInfoMetaInfo = CreateTableMetaInfo(TableLinkFields);
            if (linkInfoMetaInfo.GetField(FieldSourceValue) == null)
            {
                ret = Execute("ALTER TABLE linkfields ADD COLUMN sourceValue TEXT");
                if (ret == 0)
                {
                    ret = Execute("ALTER TABLE linkfields ADD COLUMN destValue TEXT");
                }
            }

            return ret;
        }

        /// <summary>
        /// Alters FieldInfo Table
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>
        private int AlterTableFieldInfo()
        {
            var ret = 0;
            var fieldInfoMetaInfo = CreateTableMetaInfo(TableFieldInfo);
            if (fieldInfoMetaInfo != null)
            {
                if (fieldInfoMetaInfo.GetField(FieldRights) == null)
                {
                    ret = Execute("ALTER TABLE fieldinfo ADD COLUMN rights INTEGER");
                }

                if (fieldInfoMetaInfo.GetField(FieldFormat) == null)
                {
                    ret = Execute("ALTER TABLE fieldinfo ADD COLUMN format INTEGER");
                }

                if (fieldInfoMetaInfo.GetField(FieldArrayFieldIndices) == null)
                {
                    ret = Execute("ALTER TABLE fieldinfo ADD COLUMN arrayfieldindices TEXT");
                }
            }

            return ret;
        }

        /// <summary>
        /// Alters SyncInfo Table
        /// </summary>
        /// <returns>
        /// Success/fail result
        /// </returns>/// <returns></returns>
        private int AlterTableSyncInfo()
        {
            var ret = 0;
            var fieldInfoMetaInfo = CreateTableMetaInfo(TableSyncInfo);
            if (fieldInfoMetaInfo.GetField(FieldInfoAreaId) == null)
            {
                ret = Execute("ALTER TABLE syncinfo ADD COLUMN infoareaid TEXT");
                if (ret == 0)
                {
                    ret = Execute("UPDATE syncinfo SET infoareaid = datasetname");
                }
            }

            return ret;
        }
    }
}
