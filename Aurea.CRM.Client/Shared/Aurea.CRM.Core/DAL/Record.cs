// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Record.cs" company="Aurea Software Gmbh">
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
//   Implements record definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System;
    using System.Text;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implements record definition
    /// </summary>
    public class Record
    {
        /// <summary>
        /// The ident.
        /// </summary>
        private readonly RecordIdentifier ident;

        /// <summary>
        /// The lookup record.
        /// </summary>
        private bool lookupRecord;

        /// <summary>
        /// The result row.
        /// </summary>
        private DatabaseRow resultRow;

        /// <summary>
        /// The value count.
        /// </summary>
        private int valueCount;

        /// <summary>
        /// The values.
        /// </summary>
        private string[] values;

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="ident">
        /// The ident.
        /// </param>
        public Record(RecordIdentifier ident)
        {
            this.ident = new RecordIdentifier(ident);
            this.Loaded = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public Record(string infoAreaId)
        {
            this.ident = new RecordIdentifier(infoAreaId);
            this.Loaded = false;
            this.SetVirtualInfoAreaId(infoAreaId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Record"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        public Record(string infoAreaId, string recordId)
        {
            this.ident = new RecordIdentifier(infoAreaId, recordId);
            this.valueCount = 0;
            this.Loaded = false;
            this.SetVirtualInfoAreaId(infoAreaId);
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.ident?.InfoAreaId;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Record"/> is loaded.
        /// </summary>
        /// <value>
        /// <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool Loaded { get; private set; }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordId => this.ident?.RecordId;

        /// <summary>
        /// Gets the record template.
        /// </summary>
        /// <returns></returns>
        public RecordTemplate RecordTemplate { get; private set; }

        /// <summary>
        /// Gets the virtual information area identifier.
        /// </summary>
        /// <value>
        /// The virtual information area identifier.
        /// </value>
        public string VirtualInfoAreaId { get; private set; }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Delete()
        {
            if (this.ident == null)
            {
                return -2;
            }

            var statement = this.RecordTemplate.GetDeleteStatement();
            if (statement != null)
            {
                var recid = this.ident.RecordId;
                return statement.Execute(recid);
            }

            return -1;
        }

        /// <summary>
        /// Existses this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Exists()
        {
            if (this.ident == null)
            {
                return false;
            }

            var query = this.RecordTemplate.GetExistsQuery();
            if (query == null)
            {
                return false;
            }

            var recid = this.ident.RecordId;
            var exists = query.ExistsRow(recid);
            return exists;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="pos">
        /// The position.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetValue(int pos)
        {
            if (pos >= this.valueCount)
            {
                return null;
            }

            return !string.IsNullOrEmpty(this.values?[pos]) ? this.values[pos] : this.resultRow?[pos];
        }

        /// <summary>
        /// Inserts this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Insert()
        {
            if (this.ident == null)
            {
                return 1;
            }

            var statement = this.RecordTemplate.GetInsertStatement();
            if (statement == null)
            {
                return 1;
            }

            var parameters = new object[this.valueCount + 3];
            var parameterOffset = 0;

            parameters[parameterOffset++] = this.ident.RecordId;
            parameters[parameterOffset++] = this.VirtualInfoAreaId;

            if (this.RecordTemplate.IncludeLookupForNew)
            {
                parameters[parameterOffset++] = this.lookupRecord ? "1" : "0";
            }

            for (var i = 0; i < this.valueCount; i++)
            {
                parameters[parameterOffset + i] = this.values[i];
            }

            var ret = 0;
            try
            {
                statement.Execute(parameters);
            }
            catch
            {
                ret = 1;
            }

            if (ret == 0)
            {
                this.Loaded = true;
            }

            return ret;
        }

        /// <summary>
        /// Inserts the or update.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int InsertOrUpdate()
        {
            return this.Exists() ? this.Update() : this.Insert();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Load()
        {
            if (this.ident == null)
            {
                return -1;
            }

            var query = this.RecordTemplate.GetSelectQuery();
            if (query == null)
            {
                return -1;
            }

            query.Query.Bind(1, this.ident.RecordId);
            var ret = query.Execute(0);

            if (ret > 0)
            {
                this.resultRow = query.TakeRowOwnership(0);
                this.Loaded = true;
            }

            return ret;
        }

        /// <summary>
        /// Prints this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Print()
        {
            var builder = new StringBuilder();
            if (this.ident != null)
            {
                builder.Append($"Ident = {this.ident?.InfoAreaId}.{this.ident.RecordId}");
            }

            if (this.RecordTemplate == null)
            {
                return builder.ToString();
            }

            var fieldCount = this.RecordTemplate.FieldIdCount;
            for (var i = 0; i < fieldCount; i++)
            {
                builder.Append(
                    $"field #{this.RecordTemplate.GetFieldIndex(i):D5}: {this.GetValue(i)}{Environment.NewLine}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Sets the lookup record.
        /// </summary>
        /// <param name="isLookup">
        /// if set to <c>true</c> [is lookup].
        /// </param>
        public void SetLookupRecord(bool isLookup)
        {
            this.lookupRecord = isLookup;
        }

        /// <summary>
        /// Sets the template.
        /// </summary>
        /// <param name="recordTemplate">
        /// The record template.
        /// </param>
        public void SetTemplate(RecordTemplate recordTemplate)
        {
            this.ReplaceRecordTemplate(recordTemplate, true);
        }

        /// <summary>
        /// Sets the template.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="fieldIdCount">
        /// The field identifier count.
        /// </param>
        /// <param name="fieldids">
        /// The fieldids.
        /// </param>
        public void SetTemplate(CRMDatabase database, int fieldIdCount, FieldIdType[] fieldids)
        {
            this.ReplaceRecordTemplate(
                new RecordTemplate(database, this.ident?.InfoAreaId, fieldIdCount, fieldids),
                true);
        }

        /// <summary>
        /// Sets the template weak.
        /// </summary>
        /// <param name="recordTemplate">
        /// The record template.
        /// </param>
        public void SetTemplateWeak(RecordTemplate recordTemplate)
        {
            this.ReplaceRecordTemplate(recordTemplate, false);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="pos">
        /// The position.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SetValue(int pos, string value)
        {
            if (pos >= this.valueCount)
            {
                return;
            }

            if (this.values == null)
            {
                this.values = new string[this.valueCount];
            }

            this.values[pos] = value;
        }

        /// <summary>
        /// Sets the virtual information area identifier.
        /// </summary>
        /// <param name="virtualInfoAreaId">
        /// The virtual information area identifier.
        /// </param>
        public void SetVirtualInfoAreaId(string virtualInfoAreaId)
        {
            this.VirtualInfoAreaId = virtualInfoAreaId;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Update()
        {
            if (this.ident == null)
            {
                return -1;
            }

            var statement = this.RecordTemplate.GetUpdateStatement();
            if (statement == null)
            {
                return -1;
            }

            var parameters = new object[this.valueCount + 3];
            var parameterOffset = this.valueCount;
            for (var i = 0; i < this.valueCount; i++)
            {
                parameters[i] = this.values[i];
            }

            parameters[parameterOffset++] = this.VirtualInfoAreaId;
            if (this.RecordTemplate.IncludeLookupForUpdate)
            {
                parameters[parameterOffset++] = this.lookupRecord ? "1" : "0";
            }

            parameters[parameterOffset] = this.ident.RecordId;
            int ret = statement.Execute(parameters);
            if (ret != 0)
            {
                this.Loaded = true;
            }

            return ret;
        }

        /// <summary>
        /// Replaces the record template.
        /// </summary>
        /// <param name="recordTemplate">
        /// The record template.
        /// </param>
        /// <param name="ownership">
        /// if set to <c>true</c> [ownership].
        /// </param>
        private void ReplaceRecordTemplate(RecordTemplate recordTemplate, bool ownership)
        {
            this.RecordTemplate = recordTemplate;

            if (this.RecordTemplate != null)
            {
                this.valueCount = recordTemplate.FieldIdCount + recordTemplate.LinkFieldNameCount;
            }
        }
    }
}
