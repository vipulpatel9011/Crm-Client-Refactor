// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuerySortField.cs" company="Aurea Software Gmbh">
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
    /// Query sort field configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.UPConfigQueryField" />
    public class UPConfigQuerySortField : UPConfigQueryField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQuerySortField"/> class.
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
        public UPConfigQuerySortField(int fieldIndex, UPConfigQueryTable queryTable, int flags)
            : base(fieldIndex, queryTable, flags)
        {
            this.Descending = (this.Flags & 1) != 0;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPConfigQuerySortField"/> is descending.
        /// </summary>
        /// <value>
        /// <c>true</c> if descending; otherwise, <c>false</c>.
        /// </value>
        public bool Descending { get; private set; }
    }
}
