// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordCopyStep.cs" company="Aurea Software Gmbh">
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
//   The UPRecordCopyStep
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPRecordCopyStep
    /// </summary>
    public class UPRecordCopyStep
    {
        /// <summary>
        /// Gets or sets the source record identification.
        /// </summary>
        /// <value>
        /// The source record identification.
        /// </value>
        public string SourceRecordIdentification { get; set; }

        /// <summary>
        /// Gets or sets the destination record.
        /// </summary>
        /// <value>
        /// The destination record.
        /// </value>
        public UPCRMRecord DestinationRecord { get; set; }

        /// <summary>
        /// Gets or sets the query table.
        /// </summary>
        /// <value>
        /// The query table.
        /// </value>
        public UPConfigQueryTable QueryTable { get; set; }

        /// <summary>
        /// Gets or sets the search and list configuration.
        /// </summary>
        /// <value>
        /// The search and list configuration.
        /// </value>
        public SearchAndList SearchAndListConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; set; }
    }
}
