// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineDatabase.cs" company="Aurea Software Gmbh">
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
//   Implements offline database access
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Text;
    using Aurea.CRM.Core.DAL;

    /// <summary>
    /// Offline database class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.DatabaseBase" />
    public class OfflineDatabase : DatabaseBase
    {
        private const string ColumnAppVersion = "appversion";
        private const string ColumnBaseError = "baseError";
        private const string ColumnDraft = "draft";
        private const string ColumnErrorCode = "errorcode";
        private const string ColumnErrorStack = "errorstack";
        private const string ColumnFollowUpRoot = "followuproot";
        private const string ColumnGroupRequestNr = "grouprequestnr";
        private const string ColumnOffline = "offline";
        private const string ColumnOptions = "options";
        private const string ColumnProcessType = "processtype";
        private const string ColumnRecordNr = "recordnr";
        private const string ColumnRelatedInfo = "relatedinfo";
        private const string ColumnResponse = "response";
        private const string ColumnServerRequestNumber = "serverRequestNumber";
        private const string ColumnServerTime = "servertime";
        private const string ColumnSessionId = "sessionid";
        private const string ColumnTitleLine = "titleLine";
        private const string ColumnTranslationKey = "translationkey";

        private const string DataTypeInt = "INT";
        private const string DataTypeText = "TEXT";

        private const string TableDDLVersion = "ddlVersion";
        private const string TableRequests = "requests";
        private const string TableRecords = "records";
        private const string TableRecordFields = "recordfields";
        private const string TableRecordLinks = "recordlinks";

        /// <summary>
        /// The lock object used when accesing the data base
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// The current instance
        /// </summary>
        private static WeakReference currentInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBase"/> class.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database.
        /// </param>
        /// <param name="recreate">
        /// if set to <c>true</c> recreate the target by delete existing.
        /// </param>
        protected OfflineDatabase(string databaseName, bool recreate = false)
            : base(databaseName, recreate)
        {
            if (!this.EnsureDDL())
            {
                throw new Exception("EnsureDDL failed.");
            }
        }

        /// <summary>
        /// Creates the specified is update CRM.
        /// </summary>
        /// <param name="databaseFileName">Name of the database file.</param>
        /// <returns>THe instance of Offline Database</returns>
        public static OfflineDatabase Create(string databaseFileName)
        {
            if (currentInstance != null && currentInstance.IsAlive)
            {
                var db = currentInstance.Target as OfflineDatabase;
                if (db != null && db.DatabasePath == databaseFileName)
                {
                    return db;
                }
            }

            var newInstance = new OfflineDatabase(databaseFileName);
            currentInstance = new WeakReference(newInstance);

            return newInstance;
        }

        /// <summary>
        /// Ensures the existance of the database.
        /// </summary>
        /// <returns>true if exists;else false</returns>
        public override bool EnsureDDL()
        {
            var ret = 0;
            if (!ExistsTable(TableDDLVersion))
            {
                ret = ManageTableDDLVersion();
            }

            ret = !ExistsTable(TableRequests)
                ? CreateTableRequests()
                : AlterTables();

            if (ret == 0)
            {
                ret = ManageTableRecords();
            }

            if (ret == 0 && !ExistsIndex("records_recordid"))
            {
                ret = CreateIndexRecordsReocrdId();
            }

            if (!ExistsTable("recordfields") && ret == 0)
            {
                ret = CreateTableRecordFields();
            }

            if (!ExistsTable("recordlinks") && ret == 0)
            {
                ret = CreateTableRecordLinks();
            }

            if (!ExistsTable("synchistory"))
            {
                ret = CreateTableSyncHistory();
            }

            if (ret == 0 && !ExistsIndex("recordlinks_recordid"))
            {
                ret = CreateIndexRecordLinksRecordId();
            }

            if (!ExistsTable("documentuploads") && ret == 0)
            {
                ret = CreateTableDocumentUploads();
            }

            if (!ExistsTable("requestcontrol") && ret == 0)
            {
                ret = CreateTableRequestControl();
            }

            return ret == 0;
        }

        /// <summary>
        /// Resets the OfflineDatabase
        /// </summary>
        public void Reset()
        {
            currentInstance = null;
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
        /// <returns>the lock object</returns>
        protected override object LockObject()
        {
            return Locker;
        }

        /// <summary>
        /// Alter Table caller
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int AlterTables()
        {
            var ret = 0;
            DatabaseMetaInfoTable requestInfoMetaInfo = CreateTableMetaInfo(TableRequests);
            if (requestInfoMetaInfo != null)
            {
                if (requestInfoMetaInfo.GetField(ColumnErrorCode) == null)
                {
                    ret = AlterTableRequests();
                }

                if (requestInfoMetaInfo.GetField(ColumnProcessType) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnProcessType, DataTypeText);
                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRecords, ColumnRecordNr, DataTypeInt);
                    }

                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRecordFields, ColumnRecordNr, DataTypeInt);
                    }

                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRecordLinks, ColumnRecordNr, DataTypeInt);
                    }
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnResponse) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnResponse, DataTypeText);
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnTitleLine) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnTitleLine, DataTypeText);
                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRequests, "detailsLine", DataTypeText);
                    }

                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRequests, "imageName", DataTypeText);
                    }
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnServerRequestNumber) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnServerRequestNumber, DataTypeInt);
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnServerTime) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnServerTime, DataTypeText);
                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRequests, ColumnSessionId, DataTypeText);
                    }

                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRequests, ColumnFollowUpRoot, DataTypeInt);
                    }
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnTranslationKey) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnTranslationKey, DataTypeText);
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnRelatedInfo) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnRelatedInfo, DataTypeText);
                }

                if (ret == 0 && requestInfoMetaInfo.GetField(ColumnBaseError) == null)
                {
                    ret = AlterTablesAddColumn(TableRequests, ColumnBaseError, DataTypeInt);
                    if (ret == 0)
                    {
                        ret = AlterTablesAddColumn(TableRequests, ColumnAppVersion, DataTypeText);
                    }
                }

                if (ret == 0)
                {
                    requestInfoMetaInfo = CreateTableMetaInfo(TableRecordFields);
                    if (requestInfoMetaInfo != null)
                    {
                        if (requestInfoMetaInfo.GetField(ColumnOffline) == null)
                        {
                            ret = AlterTablesAddColumn(TableRecordFields, ColumnOffline, DataTypeInt);
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Alters Database Table Results
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int AlterTableRequests()
        {
            var ret = AlterTablesAddColumn(TableRequests, ColumnErrorCode, DataTypeInt);
            if (ret == 0)
            {
                ret = AlterTablesAddColumn(TableRequests, ColumnErrorStack, DataTypeText);
            }

            if (ret == 0)
            {
                ret = AlterTablesAddColumn(TableRequests, ColumnGroupRequestNr, DataTypeInt);
            }

            if (ret == 0)
            {
                ret = UpdateColumnValueInTable(TableRequests, ColumnGroupRequestNr, "-1");
            }

            if (ret == 0)
            {
                ret = AlterTablesAddColumn(TableRequests, ColumnDraft, DataTypeInt);
            }

            return ret;
        }

        /// <summary>
        /// Updates value in table.
        /// </summary>
        /// <param name="tableName">target table</param>
        /// <param name="columnName">target column</param>
        /// <param name="value">value to be updated</param>
        /// <returns>Success/Fail</returns>
        private int UpdateColumnValueInTable(string tableName, string columnName, string value)
        {
            return Execute($"UPDATE {tableName} SET {columnName} = {value}");
        }

        /// <summary>
        /// Alter Table Add column Script Generator
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <param name="columnName">Column to be added</param>
        /// <param name="dataType"> Column DataType</param>
        /// <returns>Success/Fail</returns>
        private int AlterTablesAddColumn(string tableName, string columnName, string dataType)
        {
            return Execute($"ALTER TABLE {tableName} ADD COLUMN {columnName} {dataType}");
        }

        /// <summary>
        /// Create Index Recordlinks_recordid
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateIndexRecordLinksRecordId()
        {
            return Execute("CREATE INDEX recordlinks_recordid ON recordlinks (recordid,infoareaid)");
        }

        /// <summary>
        /// Create Index Records_RecordId
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateIndexRecordsReocrdId()
        {
            return Execute("CREATE INDEX records_recordid ON records (recordId,infoareaid)");
        }

        /// <summary>
        /// Create Table DDLVersion
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableDDLVersion()
        {
            return Execute("CREATE TABLE ddlVersion (version TEXT, createdate TEXT)");
        }

        /// <summary>
        /// Create Table Requests
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableRequests()
        {
            var query = new StringBuilder("CREATE TABLE requests (")
                .Append("requestnr INTEGER,")
                .Append("syncdate TEXT,")
                .Append("requesttype TEXT,")
                .Append("json TEXT,")
                .Append("error TEXT,")
                .Append("errorcode INT,")
                .Append("errorstack TEXT,")
                .Append("grouprequestnr INTEGER,")
                .Append("draft INTEGER,")
                .Append("processtype TEXT,")
                .Append("response TEXT,")
                .Append("titleLine TET,")
                .Append("detailsLine TEXT,")
                .Append("imageName TEXT,")
                .Append("serverRequestNumber INTEGER,")
                .Append("servertime TEXT,")
                .Append("sessionid TEXT,")
                .Append("followuproot INTEGER,")
                .Append("translationkey TEXT,")
                .Append("relatedinfo TEXT,")
                .Append("baseError INTEGER,")
                .Append("appversion TEXT)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Creates Table RequestControl
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableRequestControl()
        {
            return Execute("CREATE TABLE requestcontrol (requestkey TEXT, nextRequestNumber INTEGER)");
        }

        /// <summary>
        /// Creates Table RecordFields
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableRecordFields()
        {
            var query = new StringBuilder("CREATE TABLE recordfields ( ")
                .Append("requestnr INTEGER,")
                .Append("recordnr INTEGER,")
                .Append("fieldid INTEGER,")
                .Append("oldvalue TEXT,")
                .Append("newvalue TEXT,")
                .Append("offline INTEGER)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Creates Table RecordLinks
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableRecordLinks()
        {
            return Execute("CREATE TABLE recordlinks (requestnr INTEGER, recordnr INTEGER, infoareaid TEXT, linkid INTEGER, recordid TEXT)");
        }

        /// <summary>
        /// Creates Table SyncHistory
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableSyncHistory()
        {
            var query = new StringBuilder("CREATE TABLE synchistory ( ")
                .Append("startdate TEXT,")
                .Append("synctype TEXT,")
                .Append("details TEXT,")
                .Append("runtimeserver INTEGER,")
                .Append("runtimetransport INTEGER,")
                .Append("runtimeclient INTEGER,")
                .Append("packagecount INTEGER,")
                .Append("responsesize INTEGER,")
                .Append("recordcount INTEGER,")
                .Append("errortext TEXT,")
                .Append("errordetails TEXT)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Creates Table DocumentUploads
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int CreateTableDocumentUploads()
        {
            var query = new StringBuilder("CREATE TABLE documentuploads ( ")
                .Append("requestnr INTEGER,")
                .Append("data TEXT,")
                .Append("filename TEXT,")
                .Append("mimetype TEXT,")
                .Append("infoareaid TEXT,")
                .Append("recordid TEXT,")
                .Append("fieldid INTEGER)");
            return Execute(query.ToString());
        }

        /// <summary>
        /// Inserts default values in DDLVersion Tables
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int InsertDDLVersionDefaultValues()
        {
            return Execute("INSERT INTO ddlVersion (version, createdate) VALUES ('1.0', datetime('now'))");
        }

        /// <summary>
        /// Manages Table DDLVersion Operations
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int ManageTableDDLVersion()
        {
            var ret = CreateTableDDLVersion();
            if (ret == 0)
            {
                ret = InsertDDLVersionDefaultValues();
            }

            return ret;
        }

        /// <summary>
        /// Manages Table Records Operations
        /// </summary>
        /// <returns>Success/Fail</returns>
        private int ManageTableRecords()
        {
            var ret = 0;
            if (!ExistsTable(TableRecords))
            {
                ret = Execute("CREATE TABLE records (requestnr INTEGER, recordnr INTEGER, infoareaid TEXT, recordid TEXT, mode TEXT, options TEXT)");
            }
            else
            {
                DatabaseMetaInfoTable requestInfoMetaInfo = CreateTableMetaInfo(TableRecords);
                if (requestInfoMetaInfo.GetField(ColumnOptions) == null)
                {
                    ret = AlterTablesAddColumn(TableRecords, ColumnOptions, DataTypeText);
                }
            }

            return ret;
        }
    }
}
