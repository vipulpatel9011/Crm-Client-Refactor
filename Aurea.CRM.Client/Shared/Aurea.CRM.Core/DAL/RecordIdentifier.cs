// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordIdentifier.cs" company="Aurea Software Gmbh">
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
//   The record identifier.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    /// <summary>
    /// The record identifier.
    /// </summary>
    public class RecordIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdentifier"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        public RecordIdentifier(string infoAreaId, string recordId)
        {
            this.InfoAreaId = infoAreaId;
            this.RecordId = recordId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdentifier"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public RecordIdentifier(string infoAreaId)
            : this(infoAreaId, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdentifier"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public RecordIdentifier(RecordIdentifier source)
            : this(source?.InfoAreaId, source?.RecordId)
        {
        }

        /// <summary>
        /// Gets or sets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; set; }

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordId { get; set; }
    }
}
