// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMDocumentPage.cs" company="Aurea Software Gmbh">
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
//   The Document Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Filters;

    /// <summary>
    /// Document Page
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMDocumentPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMDocumentPage(IIdentifier identifier) 
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the type of the search.
        /// </summary>
        /// <value>
        /// The type of the search.
        /// </value>
        public SearchPageSearchType SearchType { get; set; }

        /// <summary>
        /// Gets or sets the search action.
        /// </summary>
        /// <value>
        /// The search action.
        /// </value>
        public UPMAction SearchAction { get; set; }

        /// <summary>
        /// Gets or sets the search placeholder.
        /// </summary>
        /// <value>
        /// The search placeholder.
        /// </value>
        public string SearchPlaceholder { get; set; }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        /// <value>
        /// The search text.
        /// </value>
        public string SearchText { get; set; }

        /// <summary>
        /// Gets or sets the available filters.
        /// </summary>
        /// <value>
        /// The available filters.
        /// </value>
        public List<UPMFilter> AvailableFilters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [available grouping].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [available grouping]; otherwise, <c>false</c>.
        /// </value>
        public bool AvailableGrouping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [available online search].
        /// </summary>
        /// <value>
        /// <c>true</c> if [available online search]; otherwise, <c>false</c>.
        /// </value>
        public bool AvailableOnlineSearch { get; set; }
    }
}
