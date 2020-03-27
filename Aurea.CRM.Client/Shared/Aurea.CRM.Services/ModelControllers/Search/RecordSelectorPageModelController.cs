// <copyright file="RecordSelectorPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel.Filters;

    /// <summary>
    /// Record selector page model controller class implementation
    /// </summary>
    public class UPRecordSelectorPageModelController : UPStandardSearchPageModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelectorPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// View Reference parameter
        /// </param>
        public UPRecordSelectorPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Creates and adds filters based on given filter name list
        /// </summary>
        /// <param name="availableFilters">List of available filter names</param>
        public void AddFilters(IEnumerable<string> availableFilters)
        {
            var upmFilterArray = new List<UPMFilter>();
            foreach (string filterNameItem in availableFilters)
            {
                if (!string.IsNullOrEmpty(filterNameItem))
                {
                    UPMFilter filter = UPMFilter.FilterForName(filterNameItem);
                    if (filter != null && filter.FilterType == UPMFilterType.NoParam)
                    {
                        upmFilterArray.Add(filter);
                    }
                }
            }

            this.SearchPage.AvailableFilters = upmFilterArray;
        }
    }
}
