// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimelineSearch.cs" company="Aurea Software Gmbh">
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
//   Timeline Search
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.UIModel;

    /// <summary>
    /// Timeline Search Class
    /// </summary>
    public class TimelineSearch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineSearch"/> class.
        /// </summary>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="preparedSearch">The prepared search.</param>
        public TimelineSearch(UPContainerMetaInfo crmQuery, UPSearchPageModelControllerPreparedSearch preparedSearch)
        {
            this.CrmQuery = crmQuery;
            this.PreparedSearch = preparedSearch;
            this.TimelineInfoArea = this.PreparedSearch.TimelineConfiguration;
            string configName = !string.IsNullOrEmpty(this.TimelineInfoArea.ConfigName) ? this.TimelineInfoArea.ConfigName : this.TimelineInfoArea.InfoAreaId;
            SearchAndList searchAndList = ConfigurationUnitStore.DefaultStore.SearchAndListByName(configName);
            this.FieldGroupName = searchAndList != null ? searchAndList.FieldGroupName : configName;
        }

        /// <summary>
        /// Gets the CRM query.
        /// </summary>
        /// <value>
        /// The CRM query.
        /// </value>
        public UPContainerMetaInfo CrmQuery { get; private set; }

        /// <summary>
        /// Gets the timeline information area.
        /// </summary>
        /// <value>
        /// The timeline information area.
        /// </value>
        public ConfigTimelineInfoArea TimelineInfoArea { get; private set; }

        /// <summary>
        /// Gets the prepared search.
        /// </summary>
        /// <value>
        /// The prepared search.
        /// </value>
        public UPSearchPageModelControllerPreparedSearch PreparedSearch { get; private set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public UPCRMResult Result { get; set; }

        /// <summary>
        /// Gets the name of the field group.
        /// </summary>
        /// <value>
        /// The name of the field group.
        /// </value>
        public string FieldGroupName { get; private set; }

        /// <summary>
        /// Matchings the criteria for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public ConfigTimelineCriteria MatchingCriteriaForRow(UPCRMResultRow row)
        {
            foreach (ConfigTimelineCriteria crit in this.TimelineInfoArea.ColorCriteria)
            {
                string value = row.RawValueForFieldIdInfoAreaIdLinkId(crit.FieldId, this.TimelineInfoArea.InfoAreaId, this.TimelineInfoArea.LinkId);
                if (!string.IsNullOrEmpty(value))
                {
                    bool checkresult = UPCRMField.ResultForValue(value, crit.CompareOperator, crit.CompareValue, crit.CompareValueTo, false, false, false);
                    if (checkresult)
                    {
                        return crit;
                    }
                }
            }

            return null;
        }
    }
}
