// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEFilter.cs" company="Aurea Software Gmbh">
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
//   UPSEFilter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Utilities;

    /// <summary>
    /// UPSEFilter
    /// </summary>
    public class UPSEFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEFilter"/> class.
        /// </summary>
        /// <param name="crmFilter">The CRM filter.</param>
        public UPSEFilter(UPConfigFilter crmFilter)
        {
            this.CrmFilter = crmFilter;
            this.Name = crmFilter.UnitName;
            this.InfoAreaId = crmFilter.InfoAreaId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEFilter"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="specialFilterName">Name of the special filter.</param>
        public UPSEFilter(string infoAreaId, string specialFilterName)    // Any one can be passed
        {
            this.InfoAreaId = infoAreaId;
            this.Name = infoAreaId;
            this.SpecialFilterName = specialFilterName;
            this.Name = specialFilterName;
        }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label
        {
            get
            {
                if (this.CrmFilter != null)
                {
                    return !string.IsNullOrEmpty(this.CrmFilter.DisplayName) ? this.CrmFilter.DisplayName : this.CrmFilter.UnitName;
                }

                return this.IsListingFilter ? LocalizedString.TextProcessFilterListings : LocalizedString.TextProcessFilterAll;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is listing filter.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is listing filter; otherwise, <c>false</c>.
        /// </value>
        public bool IsListingFilter => this.SpecialFilterName == "Listings";

        /// <summary>
        /// Gets the CRM filter.
        /// </summary>
        /// <value>
        /// The CRM filter.
        /// </value>
        public UPConfigFilter CrmFilter { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the name of the special filter.
        /// </summary>
        /// <value>
        /// The name of the special filter.
        /// </value>
        public string SpecialFilterName { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Applies the filter on source query parameters.
        /// </summary>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public bool ApplyFilterOnSourceQueryParameters(UPContainerMetaInfo crmQuery, Dictionary<string, object> parameters)
        {
            if (this.CrmFilter != null)
            {
                if (parameters.Count > 0)
                {
                    UPConfigFilter replacedFilter = this.CrmFilter.
                        FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(parameters));

                    crmQuery.ApplyFilter(replacedFilter);
                }
                else
                {
                    crmQuery.ApplyFilter(this.CrmFilter);
                }
            }

            return true;
        }

        /// <summary>
        /// Applies the filter on source query.
        /// </summary>
        /// <param name="crmQuery">The CRM query.</param>
        /// <returns></returns>
        public bool ApplyFilterOnSourceQuery(UPContainerMetaInfo crmQuery)
        {
            return this.ApplyFilterOnSourceQueryParameters(crmQuery, null);
        }

        /// <summary>
        /// Filters from name.
        /// </summary>
        /// <param name="filterName">Name of the filter.</param>
        /// <param name="initialValues">The initial values.</param>
        /// <returns></returns>
        public static UPSEFilter FilterFromName(string filterName, Dictionary<string, object> initialValues = null)
        {
            UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            if (filter != null)
            {
                return new UPSEFilter(filter);
            }

            int index = filterName.IndexOf(":All");
            if (index >= 0)
            {
                string infoAreaId = filterName.Substring(0, index > 0 ? index - 1 : 0);     //TODO Verify
                return new UPSEFilter(infoAreaId, null);
            }

            return filterName.Contains(":Listing") ? ListingFilter() : null;
        }

        /// <summary>
        /// Alls the positions filter.
        /// </summary>
        /// <returns></returns>
        public static UPSEFilter AllPositionsFilter()
        {
            return new UPSEFilter(null, "AllPositions");
        }

        /// <summary>
        /// Errors the positions filter.
        /// </summary>
        /// <returns></returns>
        public static UPSEFilter ErrorPositionsFilter()
        {
            return new UPSEFilter(null, "Errors");
        }

        /// <summary>
        /// Singles the article filter.
        /// </summary>
        /// <returns></returns>
        public static UPSEFilter SingleArticleFilter()
        {
            return new UPSEFilter(null, "SingleArticle");
        }

        /// <summary>
        /// Listings the filter.
        /// </summary>
        /// <returns></returns>
        public static UPSEFilter ListingFilter()
        {
            return new UPSEFilter(null, "Listings");
        }
    }
}
