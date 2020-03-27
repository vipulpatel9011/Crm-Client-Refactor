// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSearchPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//      Max Menezes
// </author>
// <summary>
//   The Multi Search Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// The Multi Search Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Search.GlobalSearchPageModelController" />
    public class UPMultiSearchPageModelController : GlobalSearchPageModelController
    {
        /// <summary>
        /// The used filters
        /// </summary>
        private Dictionary<string, UPMFilter> usedFilters;

        /// <summary>
        /// The available searches
        /// </summary>
        private List<UPSearchPageModelControllerPreparedSearch> availableSearches;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMultiSearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPMultiSearchPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Creates the page instance.
        /// </summary>
        /// <returns></returns>
        public override UPMSearchPage CreatePageInstance()
        {
            this.InfoAreaId = this.ViewReference.ContextValueForKey("InfoArea");
            this.ConfigName = this.ViewReference.ContextValueForKey("Config1Name");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string searchTypeString = this.ViewReference.ContextValueForKey("InitialSearchType");
            SearchPageSearchType searchType = Convert.ToInt32(searchTypeString) == 2 ? SearchPageSearchType.OnlineSearch : SearchPageSearchType.OfflineSearch;

            if (string.IsNullOrEmpty(this.InfoAreaId) && this.ViewReference.ContextValueForKey("Modus") == "MultiSearch")
            {
                if (string.IsNullOrEmpty(this.ConfigName))
                {
                    this.ConfigName = "default";
                }
            }

            List<IIdentifier> identifiers = new List<IIdentifier>();
            this.usedFilters = new Dictionary<string, UPMFilter>();
            List<UPMFilter> filters = new List<UPMFilter>();
            string geoFilter1 = this.ViewReference.ContextValueForKey("Config1Filter");
            UPMFilter filter = UPMFilter.FilterForName(geoFilter1);
            SearchAndList searchConfiguration = configStore.SearchAndListByName(this.ConfigName);
            var infoAreaId = searchConfiguration != null ? searchConfiguration.InfoAreaId : this.ConfigName;

            if (filter != null)
            {
                filter.Active = true;
                this.usedFilters[this.ConfigName] = filter;
                filters.Add(filter);
            }

            identifiers.Add(new RecordIdentifier(infoAreaId, null));
            for (int i = 2; i < 99; i++)
            {
                string configNameKey = $"Config{i}Name";
                string configName2 = this.ViewReference.ContextValueForKey(configNameKey);
                if (!string.IsNullOrEmpty(configName2))
                {
                    string configFilterKey = $"Config{i}Filter";
                    string geoFilter2 = this.ViewReference.ContextValueForKey(configFilterKey);
                    UPMFilter filter2 = UPMFilter.FilterForName(geoFilter2);
                    SearchAndList searchConfiguration2 = configStore.SearchAndListByName(configName2);
                    if (filter2 != null)
                    {
                        filter2.Active = true;
                        this.usedFilters[configName2] = filter2;
                        filters.Add(filter2);
                    }

                    infoAreaId = (searchConfiguration2 != null) ? searchConfiguration2.InfoAreaId : configName2;
                    identifiers.Add(new RecordIdentifier(infoAreaId, null));
                }
                else
                {
                    break;
                }
            }

            MultipleIdentifier multipleIdentifier = new MultipleIdentifier(identifiers);
            UPMSearchPage page = new UPMSearchPage(multipleIdentifier)
            {
                SearchType = searchType,
                AvailableFilters = filters,
                Style = UPMTableStyle.UPMStandardTableStyle,
                AvailableOnlineSearch = !this.ViewReference.ContextValueIsSet("hideOnlineOfflineButton"),
                InitiallyOnline = searchType == SearchPageSearchType.OnlineSearch,
                HideSearchBar = true,
                ViewType = SearchPageViewType.List
            };

            return page;
        }

        /// <summary>
        /// Builds the page details.
        /// </summary>
        public override void BuildPageDetails()
        {
            base.BuildPageDetails();
            this.SearchPage.Invalid = true;
        }

        /// <summary>
        /// Creates the container meta information with value prepared search.
        /// </summary>
        /// <param name="searchValue">The search value.</param>
        /// <param name="preparedSearch">The prepared search.</param>
        /// <returns></returns>
        public override UPContainerMetaInfo CreateContainerMetaInfoWithValuePreparedSearch(string searchValue, UPSearchPageModelControllerPreparedSearch preparedSearch)
        {
            List<UPConfigFilter> configFilter = null;

            if (this.usedFilters.ContainsKey(preparedSearch.SearchConfiguration.UnitName))
            {
                UPMFilter vFilter = this.usedFilters[preparedSearch.SearchConfiguration.UnitName];
                configFilter = UPMFilter.ActiveFiltersForFilters(new List<UPMFilter> { vFilter });
            }

            UPContainerMetaInfo container = preparedSearch.CrmQueryForValue(searchValue, configFilter, false);
            return container;
        }

        /// <summary>
        /// Searches the specified sender.
        /// </summary>
        /// <param name="searchPage">The search page.</param>
        public override void Search(object searchPage)
        {
            if (this.PreparedSearches != null && this.availableSearches != null)
            {
                this.PreparedSearches.Clear();
                foreach (UPSearchPageModelControllerPreparedSearch search in this.availableSearches)
                {
                    UPMFilter vFilter = this.usedFilters[search.SearchConfiguration.UnitName];
                    if (vFilter.Active)
                    {
                        this.PreparedSearches.Add(search);
                    }
                }
            }

            base.Search(searchPage);
        }

        /// <summary>
        /// Builds the prepared searches.
        /// </summary>
        /// <param name="forServer">if set to <c>true</c> [for server].</param>
        public override void BuildPreparedSearches(bool forServer)
        {
            this.PreparedSearches = new List<UPSearchPageModelControllerPreparedSearch>();
            this.availableSearches = new List<UPSearchPageModelControllerPreparedSearch>();
            UPSearchPageModelControllerPreparedSearch prepareSearch = new UPSearchPageModelControllerPreparedSearch(null, this.ConfigName, null);
            this.PreparedSearches.Add(prepareSearch);
            this.availableSearches.Add(prepareSearch);

            for (int i = 2; i < 99; i++)
            {
                string configNameKey = $"Config{i}Name";
                string configName2 = this.ViewReference.ContextValueForKey(configNameKey);
                if (!string.IsNullOrEmpty(configName2))
                {
                    UPSearchPageModelControllerPreparedSearch vPrepareSearch = new UPSearchPageModelControllerPreparedSearch(null, configName2, null);
                    this.PreparedSearches.Add(vPrepareSearch);
                    this.availableSearches.Add(vPrepareSearch);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
