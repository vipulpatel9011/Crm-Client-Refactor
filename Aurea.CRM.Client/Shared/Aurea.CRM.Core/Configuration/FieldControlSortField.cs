// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldControlSortField.cs" company="Aurea Software Gmbh">
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
//   Control sort field configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Control sort field configurations
    /// </summary>
    public class FieldControlSortField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldControlSortField"/> class.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public FieldControlSortField(List<object> def, string infoAreaId)
        {
            this.InfoAreaId = infoAreaId;
            this.FieldIndex = JObjectExtensions.ToInt(def[0]);
            this.Ascending = JObjectExtensions.ToInt(def[1]) == 0;
            if (def.Count() > 2)
            {
                this.InfoAreaId = (string)def[2];
            }
            else
            {
                this.InfoAreaId = infoAreaId;
            }

            this.LinkId = -1;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FieldControlSortField"/> is ascending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ascending; otherwise, <c>false</c>.
        /// </value>
        public bool Ascending { get; private set; }

        /// <summary>
        /// Gets the CRM sort field.
        /// </summary>
        /// <value>
        /// The CRM sort field.
        /// </value>
        public UPCRMSortField CrmSortField
            => new UPCRMSortField(this.FieldIndex, this.InfoAreaId, this.LinkId, this.Ascending);

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <value>
        /// The index of the field.
        /// </value>
        public int FieldIndex { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sortKey = this.Ascending ? "ASC" : "DESC";
            return $"{this.InfoAreaId}.{this.FieldIndex} {sortKey}";
        }
    }
}
