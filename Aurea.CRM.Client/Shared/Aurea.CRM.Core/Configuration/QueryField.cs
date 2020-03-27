// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryField.cs" company="Aurea Software Gmbh">
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
//   Query configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    /// <summary>
    /// Query field configurations
    /// </summary>
    public class UPConfigQueryField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryField"/> class.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <param name="queryTable">
        /// The query table.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        public UPConfigQueryField(int fieldIndex, UPConfigQueryTable queryTable, int flags)
        {
            this.FieldIndex = fieldIndex;
            this.QueryTable = queryTable;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <value>
        /// The index of the field.
        /// </value>
        public int FieldIndex { get; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public int Flags { get; }

        /// <summary>
        /// Gets the query table.
        /// </summary>
        /// <value>
        /// The query table.
        /// </value>
        public UPConfigQueryTable QueryTable { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"field={this.QueryTable.Key}.{this.FieldIndex}";
        }
    }
}
