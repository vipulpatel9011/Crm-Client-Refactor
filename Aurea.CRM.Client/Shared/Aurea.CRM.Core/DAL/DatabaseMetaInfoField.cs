// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseMetaInfoField.cs" company="Aurea Software Gmbh">
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
//   Data base meta info field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    /// <summary>
    /// Data base meta info field
    /// </summary>
    public class DatabaseMetaInfoField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseMetaInfoField"/> class.
        /// </summary>
        /// <param name="fieldName">
        /// Name of the field.
        /// </param>
        /// <param name="fieldType">
        /// Type of the field.
        /// </param>
        public DatabaseMetaInfoField(string fieldName, string fieldType)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public string FieldType { get; }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns>An instance of <see cref="DatabaseMetaInfoField"/></returns>
        public DatabaseMetaInfoField CreateCopy()
        {
            return new DatabaseMetaInfoField(this.FieldName, this.FieldType);
        }
    }
}
