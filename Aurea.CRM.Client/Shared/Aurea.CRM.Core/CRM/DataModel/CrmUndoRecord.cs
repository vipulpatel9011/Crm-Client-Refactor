// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmUndoRecord.cs" company="Aurea Software Gmbh">
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
//   The CRM Undo record class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using DAL;
    using Extensions;

    /// <summary>
    /// Undo Record Implementation
    /// </summary>
    public class UPCRMUndoRecord
    {
        /// <summary>
        /// Gets or sets the field dictionary
        /// </summary>
        protected Dictionary<string, UPCRMUndoField> FieldDictionary { get; set; }

        /// <summary>
        /// Gets the json dictionary.
        /// </summary>
        /// <value>
        /// The json dictionary.
        /// </value>
        public Dictionary<string, object> JsonDictionary
        {
            get
            {
                List<object> fieldArray = new List<object>();

                if (this.FieldDictionary != null)
                {
                    fieldArray.AddRange(this.FieldDictionary.Values.Select(field => field.JsonArray));
                }

                return new Dictionary<string, object>
                {
                    { "mode", this.Mode },
                    { "undo", this.UndoOperation },
                    { "fields", fieldArray }
                };
            }
        }

        /// <summary>
        /// Gets the rollback information.
        /// </summary>
        /// <value>
        /// The rollback information.
        /// </value>
        public string RollbackInfo => StringExtensions.StringFromObject(this.JsonDictionary);

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the undo request.
        /// </summary>
        /// <value>
        /// The undo request.
        /// </value>
        public CrmUndoRequest UndoRequest { get; private set; }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public string Mode { get; private set; }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Gets the undo operation.
        /// </summary>
        /// <value>
        /// The undo operation.
        /// </value>
        public string UndoOperation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMUndoRecord"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="undoRequest">The undo request.</param>
        public UPCRMUndoRecord(string recordIdentification, string mode, CrmUndoRequest undoRequest)
        {
            this.RecordIdentification = recordIdentification;
            this.UndoRequest = undoRequest;
            this.Mode = string.IsNullOrEmpty(mode) ? "Update" : mode;
            this.FieldDictionary = new Dictionary<string, UPCRMUndoField>();
            this.UndoOperation = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMUndoRecord"/> class.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="undoRequest">The undo request.</param>
        public UPCRMUndoRecord(UPCRMRecord record, CrmUndoRequest undoRequest)
        {
            this.RecordIdentification = record.RecordIdentification;
            this.UndoRequest = undoRequest;
            this.Mode = !string.IsNullOrEmpty(record.Mode) ? record.Mode : record.IsNew ? "New" : "Update";
            this.Record = record;
            this.UndoOperation = string.Empty;
            this.FieldDictionary = new Dictionary<string, UPCRMUndoField>();
            this.ApplyChangesFromRecord(this.Record);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMUndoRecord"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="rollbackInfo">The rollback information.</param>
        /// <param name="undoRequest">The undo request.</param>
        public UPCRMUndoRecord(string recordIdentification, Dictionary<string, object> rollbackInfo, CrmUndoRequest undoRequest)
            : this(recordIdentification, rollbackInfo["mode"] as string, undoRequest)
        {
            List<object> fields = rollbackInfo["fields"] as List<object>;
            this.FieldDictionary = new Dictionary<string, UPCRMUndoField>();

            foreach (List<object> fieldInfo in fields)
            {
                UPCRMUndoField undoField = new UPCRMUndoField(fieldInfo);
                this.AddFieldValue(undoField);
            }
        }

        /// <summary>
        /// Adds the field value.
        /// </summary>
        /// <param name="field">The field.</param>
        private void AddFieldValue(UPCRMUndoField field)
        {
            if (field == null)
            {
                return;
            }

            if (this.FieldDictionary == null)
            {
                this.FieldDictionary = new Dictionary<string, UPCRMUndoField>();
            }

            if (field.FieldName != null)
            {
                this.FieldDictionary[field.FieldName] = field;
            }
        }

        /// <summary>
        /// Applies the changes from record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void ApplyChangesFromRecord(UPCRMRecord record)
        {
            UPCRMTableInfo tableInfo = this.UndoRequest.DataStore.TableInfoForInfoArea(this.RecordIdentification.InfoAreaId());

            if (record.FieldValues != null)
            {
                foreach (UPCRMFieldValue value in record.FieldValues)
                {
                    UPCRMFieldInfo fieldInfo = tableInfo.FieldInfoForFieldId(value.FieldId);
                    if (fieldInfo != null)
                    {
                        UPCRMUndoField undoField = new UPCRMUndoField(fieldInfo.DatabaseFieldName, value.Value, null);
                        this.AddFieldValue(undoField);
                    }
                }
            }

            if (record.Links != null)
            {
                foreach (UPCRMLink link in record.Links)
                {
                    UPCRMLinkInfo linkInfo = tableInfo.LinkInfoForTargetInfoAreaIdLinkId(link.InfoAreaId, link.LinkId);
                    if (linkInfo?.HasColumn ?? false)
                    {
                        UPCRMUndoField undoField = new UPCRMUndoField(linkInfo.LinkFieldName, link.RecordId, null);
                        this.AddFieldValue(undoField);
                        string infoAreaColumnName = linkInfo.InfoAreaLinkFieldName;
                        if (!string.IsNullOrEmpty(infoAreaColumnName))
                        {
                            undoField = new UPCRMUndoField(infoAreaColumnName, link.InfoAreaId, null);
                            this.AddFieldValue(undoField);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks the delete before cache save with database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        private int CheckDeleteBeforeCacheSaveWithDatabase(DatabaseBase database)
        {
            UPCRMTableInfo tableInfo = this.UndoRequest.DataStore.TableInfoForInfoArea(this.RecordIdentification.InfoAreaId());
            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("SELECT ");
            bool first = true;
            List<string> allColumns = tableInfo.AllNames();
            foreach (string columnName in allColumns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    selectStatement.Append(", ");
                }

                selectStatement.Append(columnName);
            }

            selectStatement.Append($" FROM {tableInfo.DatabaseTableName} WHERE recid = ?");
            CRMDatabase db = this.UndoRequest.DataStore.DatabaseInstance;
            DatabaseRecordSet rs = new DatabaseRecordSet(db);
            if (!rs.Query.Prepare(selectStatement.ToString()))
            {
                return -1;
            }

            rs.Query.Bind(1, this.RecordIdentification.RecordId());
            int ret = rs.Execute();
            if (ret != 0)
            {
                return ret;
            }

            int rc = rs.GetRowCount();
            if (rc == 0)
            {
                this.Mode = "DeleteIgnore";
                this.UndoOperation = "Ignore";
                return -1;
            }

            if (rc > 1)
            {
                return -1;
            }

            this.UndoOperation = "Insert";
            int colIndex = 0;
            DatabaseRow row = rs.GetRow(0);
            foreach (string col in allColumns)
            {
                string v = row.GetColumn(colIndex++);
                if (v != null)
                {
                    this.AddFieldValue(new UPCRMUndoField(col, null, v));
                }
            }

            return 0;
        }

        /// <summary>
        /// Checks the update before cache save with database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        private int CheckUpdateBeforeCacheSaveWithDatabase(DatabaseBase database)
        {
            UPCRMTableInfo tableInfo = this.UndoRequest.DataStore.TableInfoForInfoArea(this.RecordIdentification.InfoAreaId());
            StringBuilder selectStatement = new StringBuilder();
            selectStatement.Append("SELECT ");
            bool first = true;

            List<string> allColumns = this.FieldDictionary.Keys.ToList();
            foreach (string columnName in allColumns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    selectStatement.Append(", ");
                }

                selectStatement.Append(columnName);
            }

            if (first)
            {
                selectStatement.Append("recid");
            }

            selectStatement.Append($" FROM {tableInfo.DatabaseTableName} WHERE recid = ?");
            CRMDatabase db = this.UndoRequest.DataStore.DatabaseInstance;
            DatabaseRecordSet rs = new DatabaseRecordSet(db);
            if (!rs.Query.Prepare(selectStatement.ToString()))
            {
                return -1;
            }

            rs.Query.Bind(1, this.RecordIdentification.RecordId());
            int ret = rs.Execute();
            if (ret != 0)
            {
                return ret;
            }

            int rc = rs.GetRowCount();
            if (rc == 0)
            {
                this.Mode = "UpdateNew";
                this.UndoOperation = "Delete";
                return 0;
            }

            if (rc > 1)
            {
                return -1;
            }

            this.UndoOperation = "Update";
            int colIndex = 0;
            DatabaseRow row = rs.GetRow(0);
            foreach (string col in allColumns)
            {
                string v = row.GetColumn(colIndex++);
                if (v != null)
                {
                    UPCRMUndoField undoField = this.FieldDictionary[col];
                    undoField.OldValue = v;
                }
            }

            return 0;
        }

        /// <summary>
        /// Checks the before cache save with database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        public int CheckBeforeCacheSaveWithDatabase(DatabaseBase database)
        {
            if (this.Mode.StartsWith("Delete"))
            {
                return this.CheckDeleteBeforeCacheSaveWithDatabase(database);
            }

            if (this.Mode.StartsWith("Update"))
            {
                return this.CheckUpdateBeforeCacheSaveWithDatabase(database);
            }

            return 0;
        }

        /// <summary>
        /// Checks the after cache save.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        public int CheckAfterCacheSave(DatabaseBase database)
        {
            if (this.Mode.StartsWith("New") && this.Record != null)
            {
                this.RecordIdentification = this.Record.RecordIdentification;
                this.UndoOperation = "Delete";
            }

            return 0;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"record {this.RecordIdentification} ({this.UndoOperation}): {this.FieldDictionary.Values}";
        }

        /// <summary>
        /// Undoes the by inserting with transaction.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        private int UndoByInsertingWithTransaction(DatabaseBase database)
        {
            StringBuilder statementString = new StringBuilder();
            StringBuilder parameters = new StringBuilder();
            statementString.Append($"INSERT INTO CRM_{this.RecordIdentification.InfoAreaId()} (");
            bool first = true;

            foreach (UPCRMUndoField undoField in this.FieldDictionary.Values)
            {
                if (string.IsNullOrEmpty(undoField.OldValue))
                {
                    continue;
                }

                if (first)
                {
                    first = false;
                    parameters.Append("?");
                }
                else
                {
                    statementString.Append(", ");
                    parameters.Append(", ?");
                }

                statementString.Append(undoField.FieldName);
            }

            if (first)
            {
                return 0;
            }

            statementString.Append($") VALUES ({parameters})");
            DatabaseStatement statement = new DatabaseStatement(database);
            if (!statement.Prepare(statementString.ToString()))
            {
                return -1;
            }

            int parameterCount = 0;
            foreach (UPCRMUndoField undoField in this.FieldDictionary.Values)
            {
                if (string.IsNullOrEmpty(undoField.OldValue))
                {
                    continue;
                }

                statement.Bind(parameterCount++, undoField.OldValue);
            }

            return statement.Execute();
        }

        /// <summary>
        /// Undoes the by updating with transaction.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        private int UndoByUpdatingWithTransaction(DatabaseBase database)
        {
            StringBuilder statementString = new StringBuilder();
            if (this.FieldDictionary.Count == 0)
            {
                return 0;
            }

            statementString.Append($"UPDATE CRM_{this.RecordIdentification.InfoAreaId()} SET ");
            bool first = true;
            foreach (UPCRMUndoField undoField in this.FieldDictionary.Values)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    statementString.Append(" = ?, ");
                }

                statementString.Append(undoField.FieldName);
            }

            if (first)
            {
                return 0;
            }

            statementString.Append(" = ? WHERE recid = ?");
            DatabaseStatement statement = new DatabaseStatement(database);
            if (!statement.Prepare(statementString.ToString()))
            {
                return -1;
            }

            int parameterCount = 0;
            foreach (UPCRMUndoField undoField in this.FieldDictionary.Values)
            {
                statement.Bind(parameterCount++, undoField.OldValue);
            }

            statement.Bind(parameterCount, this.RecordIdentification.RecordId());
            return statement.Execute();
        }

        /// <summary>
        /// Undoes the by deleting with transaction.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        private int UndoByDeletingWithTransaction(DatabaseBase database)
        {
            DatabaseStatement statement = new DatabaseStatement(database);
            string statementString = $"DELETE FROM CRM_{this.RecordIdentification.InfoAreaId()} WHERE recid = ?";
            if (statement.Prepare(statementString))
            {
                string recordId = this.RecordIdentification.RecordId();
                statement.Bind(recordId);
                return statement.Execute();
            }

            return -1;
        }

        /// <summary>
        /// Undoes the with transaction.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success, else error number</returns>
        public int UndoWithTransaction(DatabaseBase database)
        {
            int ret = 0;
            switch (this.UndoOperation)
            {
                case "Insert":
                    ret = this.UndoByInsertingWithTransaction(database);
                    if (ret == 0)
                    {
                        this.UndoOperation = "DoneInsert";
                    }

                    break;

                case "Delete":
                    ret = this.UndoByDeletingWithTransaction(database);
                    if (ret == 0)
                    {
                        this.UndoOperation = "DoneDelete";
                    }

                    break;

                case "Update":
                    ret = this.UndoByUpdatingWithTransaction(database);
                    if (ret == 0)
                    {
                        this.UndoOperation = "DoneUpdate";
                    }

                    break;
            }

            return ret;
        }
    }
}
