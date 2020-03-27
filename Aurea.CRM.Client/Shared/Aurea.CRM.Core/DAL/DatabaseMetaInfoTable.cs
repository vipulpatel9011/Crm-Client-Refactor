// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseMetaInfoTable.cs" company="Aurea Software Gmbh">
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
//   Database meta info table definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Database meta info table definition
    /// </summary>
    public class DatabaseMetaInfoTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseMetaInfoTable"/> class.
        /// </summary>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        public DatabaseMetaInfoTable(string tableName)
        {
            this.TableName = tableName;
            this.Unsorted = false;

            // _fieldAlloc = _fieldCount = 0;
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<DatabaseMetaInfoField> Fields { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DatabaseMetaInfoTable"/> is unsorted.
        /// </summary>
        /// <value>
        /// <c>true</c> if unsorted; otherwise, <c>false</c>.
        /// </value>
        public bool Unsorted { get; private set; }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="fieldName">
        /// Name of the field.
        /// </param>
        /// <param name="fieldType">
        /// Type of the field.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseMetaInfoField"/>.
        /// </returns>
        public DatabaseMetaInfoField AddField(string fieldName, string fieldType)
        {
            return this.AddFieldWithOwnership(new DatabaseMetaInfoField(fieldName, fieldType));
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseMetaInfoField"/>.
        /// </returns>
        public DatabaseMetaInfoField AddField(DatabaseMetaInfoField field)
        {
            return field == null ? null : this.AddFieldWithOwnership(field.CreateCopy());
        }

        /// <summary>
        /// Adds the field with ownership.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseMetaInfoField"/>.
        /// </returns>
        public DatabaseMetaInfoField AddFieldWithOwnership(DatabaseMetaInfoField field)
        {
            if (this.Fields == null)
            {
                this.Fields = new List<DatabaseMetaInfoField>();
            }

            this.Fields.Add(field);
            return field;
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="fieldName">
        /// Name of the field.
        /// </param>
        /// <returns>
        /// The <see cref="DatabaseMetaInfoField"/>.
        /// </returns>
        public DatabaseMetaInfoField GetField(string fieldName)
        {
            return this.Unsorted ? null : this.Fields.FirstOrDefault(f => string.Equals(f.FieldName, fieldName));
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public void Sort()
        {
            if (!this.Unsorted)
            {
                return;
            }

            this.Fields?.Sort((f1, f2) => string.CompareOrdinal(f1?.FieldName, f2?.FieldName));
            this.Unsorted = false;
        }
    }
}
