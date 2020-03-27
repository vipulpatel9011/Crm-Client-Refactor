// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortField.cs" company="Aurea Software Gmbh">
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
//   CRM sort field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Query
{
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// CRM sort field
    /// </summary>
    /// <seealso cref="UPCRMField" />
    public class UPCRMSortField : UPCRMField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMSortField"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="ascending">
        /// if set to <c>true</c> [ascending].
        /// </param>
        public UPCRMSortField(int fieldId, string infoAreaId, bool ascending)
            : this(fieldId, infoAreaId, -1, ascending)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMSortField"/> class.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="ascending">
        /// if set to <c>true</c> [ascending].
        /// </param>
        public UPCRMSortField(UPCRMField field, bool ascending)
            : this(field.FieldId, field.InfoAreaId, field.LinkId, ascending)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMSortField"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="ascending">
        /// if set to <c>true</c> [ascending].
        /// </param>
        public UPCRMSortField(int fieldId, string infoAreaId, int linkId, bool ascending)
            : base(fieldId, infoAreaId, linkId)
        {
            this.Ascending = ascending;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMSortField"/> is ascending.
        /// </summary>
        /// <value>
        /// <c>true</c> if ascending; otherwise, <c>false</c>.
        /// </value>
        public bool Ascending { get; private set; }
    }
}
