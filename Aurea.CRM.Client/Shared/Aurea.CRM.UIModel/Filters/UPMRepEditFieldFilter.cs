// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMRepEditFieldFilter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm rep edit field filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Filters
{
    using System.Collections.Generic;

    /// <summary>
    /// The upm rep edit field filter.
    /// </summary>
    public class UPMRepEditFieldFilter
    {
        /// <summary>
        /// The sub filter.
        /// </summary>
        protected List<UPMRepEditFieldFilter> subFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMRepEditFieldFilter"/> class.
        /// </summary>
        /// <param name="repOrgGroup">
        /// The rep org group.
        /// </param>
        public UPMRepEditFieldFilter(UPMRepPossibleValue repOrgGroup)
        {
            this.RepOrgGroup = repOrgGroup;
            this.Active = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets the rep org group.
        /// </summary>
        public UPMRepPossibleValue RepOrgGroup { get; private set; }

        /// <summary>
        /// The sub filter.
        /// </summary>
        public List<UPMRepEditFieldFilter> SubFilter => this.subFilter;

        /// <summary>
        /// The add sub filter.
        /// </summary>
        /// <param name="repOrgGroup">
        /// The rep org group.
        /// </param>
        /// <returns>
        /// The <see cref="UPMRepEditFieldFilter"/>.
        /// </returns>
        public UPMRepEditFieldFilter AddSubFilter(UPMRepPossibleValue repOrgGroup)
        {
            if (repOrgGroup == null)
            {
                return null;
            }

            if (this.subFilter == null)
            {
                this.subFilter = new List<UPMRepEditFieldFilter>();
            }

            UPMRepEditFieldFilter subRepEditFieldFilter = new UPMRepEditFieldFilter(repOrgGroup);
            if (this.subFilter.Contains(subRepEditFieldFilter) == false)
            {
                this.subFilter.Add(subRepEditFieldFilter);
            }
            else
            {
                subRepEditFieldFilter = this.subFilter[this.subFilter.IndexOf(subRepEditFieldFilter)];
            }

            return subRepEditFieldFilter;
        }

        /// <summary>
        /// The is equal.
        /// </summary>
        /// <param name="theObject">
        /// The the object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEqual(object theObject)
        {
            return this.RepOrgGroup.RepId.Equals(((UPMRepEditFieldFilter)theObject).RepOrgGroup.RepId);
        }
    }
}
