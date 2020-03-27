// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSelectCatalogFilter.cs" company="Aurea Software Gmbh">
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
//   The UPMSelectCatalogFilter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Filters
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UPMSelectCatalogFilter
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Filters.UPMCatalogFilter" />
    public class UPMSelectCatalogFilter : UPMCatalogFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSelectCatalogFilter"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSelectCatalogFilter(IIdentifier identifier)
            : base(identifier, false)
        {
        }

        /// <summary>
        /// Gets or sets the CRM result.
        /// </summary>
        /// <value>
        /// The CRM result.
        /// </value>
        public UPCRMResult CrmResult { get; set; }

        /// <summary>
        /// Gets or sets the result row dictionary.
        /// </summary>
        /// <value>
        /// The result row dictionary.
        /// </value>
        public Dictionary<string, UPCRMResultRow> ResultRowDictionary { get; set; }

        /// <summary>
        /// Gets or sets the filter parameters.
        /// </summary>
        /// <value>
        /// The filter parameters.
        /// </value>
        public Dictionary<string, object> FilterParameters { get; set; }
    }
}
