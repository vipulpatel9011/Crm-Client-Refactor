// <copyright file="MAdvancedSearchOrganizer.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Advanced Search Organizer
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMOrganizer" />
    public class UPMAdvancedSearchOrganizer : UPMOrganizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMAdvancedSearchOrganizer"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMAdvancedSearchOrganizer(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the detail searches.
        /// </summary>
        /// <value>
        /// The detail searches.
        /// </value>
        public List<UPMDetailSearch> DetailSearches { get; set; }
    }
}
