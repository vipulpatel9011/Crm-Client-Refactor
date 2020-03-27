// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordTemplate.cs" company="Aurea Software Gmbh">
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
//   Implements record template
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;
    using System.Text;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implements record template
    /// </summary>
    public class RecordTemplate
    {
        /// <summary>
        /// The field ids.
        /// </summary>
        private readonly FieldIdType[] fieldIds;

        /// <summary>
        /// The is sync.
        /// </summary>
        private readonly bool isSync;

        /// <summary>
        /// The link field names.
        /// </summary>
        private readonly List<string> linkFieldNames;

        /// <summary>
        /// The delete statement.
        /// </summary>
        private DatabaseStatement deleteStatement;

        /// <summary>
        /// The delete statement string.
        /// </summary>
        private string deleteStatementString;

        /// <summary>
        /// The exists statement.
        /// </summary>
        private DatabaseQuery existsStatement;

        /// <summary>
        /// The exists statement string.
        /// </summary>
        private string existsStatementString;

        /// <summary>
        /// The insert statement.
        /// </summary>
        private DatabaseStatement insertStatement;

        /// <summary>
        /// The insert statement string.
        /// </summary>
        private string insertStatementString;

        /// <summary>
        /// The select statement.
        /// </summary>
        private DatabaseRecordSet selectStatement;

        /// <summary>
        /// The select statement string.
        /// </summary>
        private string selectStatementString;

        /// <summary>
        /// The tableinfo.
        /// </summary>
        private TableInfo tableinfo;

        /// <summary>
        /// The update statement.
        /// </summary>
        private DatabaseStatement updateStatement;

        /// <summary>
        /// The update statement string.
        /// </summary>
        private string updateStatementString;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordTemplate"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldIdCount">
        /// The field identifier count.
        /// </param>
        /// <param name="fieldids">
        /// The fieldids.
        /// </param>
        public RecordTemplate(CRMDatabase database, string infoAreaId, int fieldIdCount, FieldIdType[] fieldids)
            : this(database, infoAreaId, fieldIdCount, fieldids, 0, null)
        {
            this.isSync = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordTemplate"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="isSync">
        /// if set to <c>true</c> [is synchronize].
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldIdCount">
        /// The field identifier count.
        /// </param>
        /// <param name="fieldids">
        /// The fieldids.
        /// </param>
        /// <param name="linkFieldNameCount">
        /// The link field name count.
        /// </param>
        /// <param name="linkFieldNames">
        /// The link field names.
        /// </param>
        /// <param name="includeLookupForNew">
        /// if set to <c>true</c> [include lookup for new].
        /// </param>
        /// <param name="includeLookupForUpdate">
        /// if set to <c>true</c> [include lookup for update].
        /// </param>
        public RecordTemplate(
            CRMDatabase database,
            bool isSync,
            string infoAreaId,
            int fieldIdCount,
            FieldIdType[] fieldids,
            int linkFieldNameCount,
            List<string> linkFieldNames,
            bool includeLookupForNew,
            bool includeLookupForUpdate)
            : this(database, infoAreaId, fieldIdCount, fieldids, linkFieldNameCount, linkFieldNames)
        {
            this.isSync = isSync;
            this.IncludeLookupForNew = includeLookupForNew;
            this.IncludeLookupForUpdate = includeLookupForUpdate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordTemplate"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldIdCount">
        /// The field identifier count.
        /// </param>
        /// <param name="fieldids">
        /// The fieldids.
        /// </param>
        /// <param name="linkFieldNameCount">
        /// The link field name count.
        /// </param>
        /// <param name="linkFieldNames">
        /// The link field names.
        /// </param>
        public RecordTemplate(
            CRMDatabase database,
            string infoAreaId,
            int fieldIdCount,
            FieldIdType[] fieldids,
            int linkFieldNameCount,
            List<string> linkFieldNames)
        {
            this.Database = database;
            this.InfoAreaId = infoAreaId;
            this.FieldIdCount = fieldIdCount;
            this.fieldIds = fieldids;

            this.LinkFieldNameCount = linkFieldNameCount;
            if (linkFieldNames != null && linkFieldNameCount > 0)
            {
                this.linkFieldNames = new List<string>();
                for (var i = 0; i < linkFieldNameCount; i++)
                {
                    this.linkFieldNames.Add(linkFieldNames[i]);
                }
            }
            else
            {
                this.linkFieldNames = null;
            }

            this.insertStatementString = null;
            this.updateStatementString = null;
            this.selectStatementString = null;
            this.existsStatementString = null;
            this.deleteStatementString = null;
            this.tableinfo = null;
            this.insertStatement = null;
            this.updateStatement = null;
            this.selectStatement = null;
            this.existsStatement = null;
            this.deleteStatement = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordTemplate"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public RecordTemplate(CRMDatabase database, string infoAreaId)
            : this(database, infoAreaId, 0, null, 0, null)
        {
            this.isSync = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordTemplate"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public RecordTemplate(RecordTemplate source)
            : this(
                source.Database,
                source.InfoAreaId,
                source.FieldIdCount,
                source.fieldIds,
                source.LinkFieldNameCount,
                source.linkFieldNames)
        {
            this.isSync = source.isSync;
            this.IncludeLookupForNew = source.IncludeLookupForNew;
            this.IncludeLookupForUpdate = source.IncludeLookupForUpdate;
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <returns></returns>
        public CRMDatabase Database { get; }

        /// <summary>
        /// Gets the field identifier count.
        /// </summary>
        /// <value>
        /// The field identifier count.
        /// </value>
        public int FieldIdCount { get; }

        /// <summary>
        /// Gets a value indicating whether [include lookup for new].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include lookup for new]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeLookupForNew { get; }

        /// <summary>
        /// Gets a value indicating whether [include lookup for update].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include lookup for update]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeLookupForUpdate { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; }

        /// <summary>
        /// Gets the link field name count.
        /// </summary>
        /// <value>
        /// The link field name count.
        /// </value>
        public int LinkFieldNameCount { get; }

        /// <summary>
        /// Gets the delete statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement GetDeleteStatement()
        {
            if (this.deleteStatement != null)
            {
                return this.deleteStatement;
            }

            this.deleteStatement = new DatabaseStatement(this.Database);
            if (!this.deleteStatement.Prepare(this.GetDeleteStatementString()))
            {
                this.deleteStatement = null;
            }

            return this.deleteStatement;
        }

        /// <summary>
        /// Gets the delete statement string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDeleteStatementString()
        {
            if (!string.IsNullOrEmpty(this.deleteStatementString))
            {
                return this.deleteStatementString;
            }

            if (this.tableinfo == null)
            {
                this.tableinfo = this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
            }

            if (this.tableinfo == null)
            {
                return null;
            }

            var buffer = new StringBuilder();
            buffer.Append("DELETE FROM ");
            buffer.Append(this.tableinfo.DatabaseTableName);
            buffer.Append(" WHERE ");
            buffer.Append(this.GetFieldName(FieldIdType.RECORDID));
            buffer.Append(" = ?");
            this.deleteStatementString = buffer.ToString();

            return this.deleteStatementString;
        }

        /// <summary>
        /// Gets the exists query.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseQuery"/>.
        /// </returns>
        public DatabaseQuery GetExistsQuery()
        {
            if (this.existsStatement != null)
            {
                return this.existsStatement;
            }

            this.existsStatement = new DatabaseQuery(this.Database);
            if (!this.existsStatement.Prepare(this.GetExistsStatementString()))
            {
                this.existsStatement = null;
            }

            return this.existsStatement;
        }

        /// <summary>
        /// Gets the exists statement string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetExistsStatementString()
        {
            if (!string.IsNullOrEmpty(this.existsStatementString))
            {
                return this.existsStatementString;
            }

            if (this.tableinfo == null)
            {
                this.tableinfo = this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
            }

            if (this.tableinfo == null)
            {
                return null;
            }

            var buffer = new StringBuilder();
            buffer.Append("SELECT ");
            buffer.Append(this.GetFieldName(FieldIdType.RECORDID));
            buffer.Append(" FROM ");
            buffer.Append(this.tableinfo.DatabaseTableName);
            buffer.Append(" WHERE ");
            buffer.Append(this.GetFieldName(FieldIdType.RECORDID));
            buffer.Append(" = ?");
            this.existsStatementString = buffer.ToString();

            return this.existsStatementString;
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetFieldIndex(int index)
        {
            return index <= this.FieldIdCount ? (int)this.fieldIds[index] : -999;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfo(FieldIdType fieldId)
        {
            if (this.tableinfo == null)
            {
                this.tableinfo = this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
            }

            return this.tableinfo?.GetFieldInfo((int)fieldId);
        }

        /// <summary>
        /// Gets the index of the field information by.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfoByIndex(int index)
        {
            return this.GetFieldInfo((FieldIdType)this.GetFieldIndex(index));
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFieldName(FieldIdType fieldId)
        {
            switch (fieldId)
            {
                case FieldIdType.INFOAREAID:
                    return "title";
                case FieldIdType.RECORDID:
                    return "recid";
                case FieldIdType.UPDDATE:
                    return "upd";
                case FieldIdType.SYNCDATE:
                    return "sync";
                case FieldIdType.LOOKUP:
                    return "lookup";
                default:
                    return $"F{fieldId}";
            }
        }

        /// <summary>
        /// Gets the index of the field name by.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFieldNameByIndex(int index)
        {
            return this.GetFieldName((FieldIdType)this.GetFieldIndex(index));
        }

        /// <summary>
        /// Gets the field position.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetFieldPos(int fieldId)
        {
            int i;
            for (i = 0; i < this.FieldIdCount; i++)
            {
                if ((int)this.fieldIds[i] == fieldId)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the insert statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement GetInsertStatement()
        {
            if (this.insertStatement != null)
            {
                return this.insertStatement;
            }

            this.insertStatement = new DatabaseStatement(this.Database);
            if (!this.insertStatement.Prepare(this.GetInsertStatementString()))
            {
                this.insertStatement = null;
            }

            return this.insertStatement;
        }

        /// <summary>
        /// Gets the insert statement string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetInsertStatementString()
        {
            if (!string.IsNullOrEmpty(this.insertStatementString))
            {
                return this.insertStatementString;
            }

            int i;
            if (this.tableinfo == null)
            {
                this.tableinfo = this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
            }

            if (this.tableinfo == null)
            {
                return null;
            }

            var buffer = new StringBuilder();
            buffer.Append("INSERT INTO ");
            buffer.Append(this.tableinfo.DatabaseTableName);
            buffer.Append("(");
            buffer.Append(this.GetFieldName(this.isSync ? FieldIdType.SYNCDATE : FieldIdType.UPDDATE));
            buffer.Append(",");
            buffer.Append(this.GetFieldName(FieldIdType.RECORDID));
            buffer.Append(",");
            buffer.Append(this.GetFieldName(FieldIdType.INFOAREAID));

            if (this.IncludeLookupForNew)
            {
                buffer.Append(",");
                buffer.Append(this.GetFieldName(FieldIdType.LOOKUP));
            }

            for (i = 0; i < this.FieldIdCount; i++)
            {
                buffer.Append(",");
                buffer.Append(this.GetFieldName(this.fieldIds[i]));
            }

            for (i = 0; i < this.LinkFieldNameCount; i++)
            {
                buffer.Append(",");
                buffer.Append(this.linkFieldNames[i]);
            }

            buffer.Append(") VALUES (");
            buffer.Append("datetime('now')");

            int parameterCount;
            parameterCount = this.FieldIdCount + 2;
            if (this.IncludeLookupForNew)
            {
                parameterCount++;
            }

            for (i = 0; i < parameterCount; i++)
            {
                buffer.Append(",?");
            }

            for (i = 0; i < this.LinkFieldNameCount; i++)
            {
                buffer.Append(",?");
            }

            buffer.Append(")");
            this.insertStatementString = buffer.ToString();

            return this.insertStatementString;
        }

        /// <summary>
        /// Gets the index of the link name by.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetLinkNameByIndex(int index)
        {
            return index < this.LinkFieldNameCount ? this.linkFieldNames[index] : null;
        }

        /// <summary>
        /// Gets the root information area identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetRootInfoAreaId()
        {
            var tableInfo = this.GetTableInfo();
            return tableInfo != null ? tableInfo.RootInfoAreaId : this.InfoAreaId;
        }

        /// <summary>
        /// Gets the select query.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseRecordSet"/>.
        /// </returns>
        public DatabaseRecordSet GetSelectQuery()
        {
            if (this.selectStatement != null)
            {
                return this.selectStatement;
            }

            this.selectStatement = new DatabaseRecordSet(this.Database);
            if (!this.selectStatement.Query.Prepare(this.GetSelectStatementString()))
            {
                this.selectStatement = null;
            }

            return this.selectStatement;
        }

        /// <summary>
        /// Gets the select statement string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetSelectStatementString()
        {
            if (!string.IsNullOrEmpty(this.selectStatementString))
            {
                return this.selectStatementString;
            }

            if (this.tableinfo == null)
            {
                this.tableinfo = this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
            }

            if (this.tableinfo == null)
            {
                return null;
            }

            var buffer = new StringBuilder();
            buffer.Append("SELECT ");
            int i;

            for (i = 0; i < this.FieldIdCount; i++)
            {
                if (i > 0)
                {
                    buffer.Append(",");
                }

                buffer.Append(this.GetFieldName(this.fieldIds[i]));
            }

            for (i = 0; i < this.LinkFieldNameCount; i++)
            {
                if (i > 0 || this.FieldIdCount > 0)
                {
                    buffer.Append(",");
                }

                buffer.Append(this.linkFieldNames[i]);
            }

            if (this.FieldIdCount > 0 || this.LinkFieldNameCount > 0)
            {
                buffer.Append(",");
            }

            buffer.Append(this.GetFieldName(FieldIdType.INFOAREAID));
            buffer.Append(" FROM ");
            buffer.Append(this.tableinfo.DatabaseTableName);
            buffer.Append(" WHERE ");
            buffer.Append(this.GetFieldName(FieldIdType.RECORDID));
            buffer.Append(" = ?");
            this.selectStatementString = buffer.ToString();

            return this.selectStatementString;
        }

        /// <summary>
        /// Gets the table information.
        /// </summary>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo GetTableInfo()
        {
            return this.tableinfo ?? this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
        }

        /// <summary>
        /// Gets the update statement.
        /// </summary>
        /// <returns>
        /// The <see cref="DatabaseStatement"/>.
        /// </returns>
        public DatabaseStatement GetUpdateStatement()
        {
            if (this.updateStatement != null)
            {
                return this.updateStatement;
            }

            this.updateStatement = new DatabaseStatement(this.Database);
            if (!this.updateStatement.Prepare(this.GetUpdateStatementString()))
            {
                this.updateStatement = null;
            }

            return this.updateStatement;
        }

        /// <summary>
        /// Gets the update statement string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetUpdateStatementString()
        {
            if (!string.IsNullOrEmpty(this.updateStatementString))
            {
                return this.updateStatementString;
            }

            if (this.tableinfo == null)
            {
                this.tableinfo = this.Database.GetTableInfoByInfoArea(this.InfoAreaId);
            }

            if (this.tableinfo == null)
            {
                return null;
            }

            var buffer = new StringBuilder();
            buffer.Append("UPDATE ");
            buffer.Append(this.tableinfo.DatabaseTableName);
            buffer.Append(" SET ");
            if (this.isSync)
            {
                buffer.Append(this.GetFieldName(FieldIdType.SYNCDATE));
                buffer.Append(" = datetime('now'), ");
                buffer.Append(this.GetFieldName(FieldIdType.UPDDATE));
                buffer.Append(" = ''");
            }
            else
            {
                buffer.Append(this.GetFieldName(FieldIdType.UPDDATE));
                buffer.Append(" = datetime('now')");
            }

            int i;
            for (i = 0; i < this.FieldIdCount; i++)
            {
                buffer.Append(",");
                buffer.Append(this.GetFieldName(this.fieldIds[i]));
                buffer.Append("= ?");
            }

            for (i = 0; i < this.LinkFieldNameCount; i++)
            {
                buffer.Append(",");
                buffer.Append(this.linkFieldNames[i]);
                buffer.Append("= ?");
            }

            buffer.Append(",");
            buffer.Append(this.GetFieldName(FieldIdType.INFOAREAID));
            buffer.Append("= ? ");

            if (this.IncludeLookupForUpdate)
            {
                buffer.Append(",");
                buffer.Append(this.GetFieldName(FieldIdType.LOOKUP));
                buffer.Append(" = ?");
            }

            buffer.Append(" WHERE ");
            buffer.Append(this.GetFieldName(FieldIdType.RECORDID));
            buffer.Append(" = ?");
            this.updateStatementString = buffer.ToString();

            return this.updateStatementString;
        }
    }
}
