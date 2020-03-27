// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCalendarSearch.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The up calendar search.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.UIModel.Contexts;

    /// <summary>
    /// The up calendar search.
    /// </summary>
    public class UPCalendarSearch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCalendarSearch"/> class.
        /// </summary>
        /// <param name="preparedSearch">
        /// The prepared search.
        /// </param>
        public UPCalendarSearch(UPSearchPageModelControllerPreparedSearch preparedSearch)
        {
            this.PreparedSearch = preparedSearch;
            ViewReference viewRef = preparedSearch.CalendarPageInfoArea.ViewReference;

            if (viewRef != null)
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                string repFilterName = viewRef.ContextValueForKey("RepFilter");
                this.RepFilter = configStore.FilterByName(repFilterName);
                var filterIndexMapping = new Dictionary<string, UPConfigFilter>();

                for (int i = 1; i < 6; i++)
                {
                    string filterName = viewRef.ContextValueForKey($"Filter{i}");
                    if (!string.IsNullOrEmpty(filterName))
                    {
                        UPConfigFilter filter = configStore.FilterByName(filterName);
                        if (filter != null)
                        {
                            filterIndexMapping[i.ToString()] = filter;
                        }
                    }
                }

                string addIndex = viewRef.ContextValueForKey("AdditionalFilter");
                if (!string.IsNullOrEmpty(addIndex))
                {
                    int filterIndex = 6;
                    var parts = addIndex.Split(';');

                    foreach (string part in parts)
                    {
                        if (part.Length > 0)
                        {
                            UPConfigFilter filter = configStore.FilterByName(part);
                            if (filter != null)
                            {
                                filterIndexMapping[filterIndex.ToString()] = filter;
                            }
                        }

                        ++filterIndex;
                    }
                }

                this.FilterIndexMapping = filterIndexMapping;
            }
        }

        /// <summary>
        /// Gets or sets the calendar table caption result field array.
        /// </summary>
        public List<UPContainerFieldMetaInfo> CalendarTableCaptionResultFieldArray { get; set; }

        /// <summary>
        /// Gets or sets the crm query.
        /// </summary>
        public UPContainerMetaInfo CrmQuery { get; set; }

        /// <summary>
        /// Gets the filter index mapping.
        /// </summary>
        public Dictionary<string, UPConfigFilter> FilterIndexMapping { get; private set; }

        /// <summary>
        /// Gets the prepared search.
        /// </summary>
        public UPSearchPageModelControllerPreparedSearch PreparedSearch { get; private set; }

        /// <summary>
        /// Gets the rep filter.
        /// </summary>
        public UPConfigFilter RepFilter { get; private set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public UPCRMResult Result { get; set; }

        /// <summary>
        /// Gets or sets the result context.
        /// </summary>
        public UPCoreMappingResultContext ResultContext { get; set; }
    }
}
