// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchPageModelControllerPreparedSearch.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The up search page model controller prepared search.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// The up search page model controller prepared search.
    /// </summary>
    public class UPSearchPageModelControllerPreparedSearch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchPageModelControllerPreparedSearch"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The info area id.
        /// </param>
        /// <param name="quickSearchEntries">
        /// The quick search entries.
        /// </param>
        public UPSearchPageModelControllerPreparedSearch(string infoAreaId, List<QuickSearchEntry> quickSearchEntries)
        {
            this.InfoAreaId = infoAreaId;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.DetailAction = configStore.MenuByName(@"SHOWRECORD");

            this.ListFieldControl = configStore.FieldControlByNameFromGroup(@"List", this.InfoAreaId);
            this.DropdownFieldControl = configStore.FieldControlByNameFromGroup(@"MiniDetails", this.InfoAreaId);

            this.CombinedControl = this.DropdownFieldControl == null
                                       ? this.ListFieldControl
                                       : new FieldControl(
                                             new List<FieldControl> { this.ListFieldControl, this.DropdownFieldControl });

            this.QuickSearchEntries = quickSearchEntries;
            this.ExpandSettings = configStore.ExpandByName(this.InfoAreaId);
            this.ReplaceCaseSensitiveCharacters = configStore.ConfigValueIsSet(@"Search.ReplaceCaseSensitiveCharacters");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchPageModelControllerPreparedSearch"/> class.
        /// </summary>
        /// <param name="timelineInfoAreaConfiguration">
        /// The timeline info area configuration.
        /// </param>
        public UPSearchPageModelControllerPreparedSearch(ConfigTimelineInfoArea timelineInfoAreaConfiguration)
        {
            this.InfoAreaId = timelineInfoAreaConfiguration.InfoAreaId;
            this.ConfigName = timelineInfoAreaConfiguration.ConfigName;
            this.TimelineConfiguration = timelineInfoAreaConfiguration;

            if (string.IsNullOrEmpty(this.ConfigName))
            {
                this.ConfigName = this.InfoAreaId;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            this.SearchConfiguration = configStore.SearchAndListByName(this.ConfigName);

            string fieldGroupName;

            if (this.SearchConfiguration != null)
            {
                if (this.SearchConfiguration.DefaultAction != null)
                {
                    this.DetailAction = configStore.MenuByName(this.SearchConfiguration.DefaultAction);
                }

                if (this.DetailAction == null)
                {
                    InfoArea infoAreaConfig = configStore.InfoAreaConfigById(this.InfoAreaId);

                    if (!string.IsNullOrEmpty(infoAreaConfig.DefaultAction))
                    {
                        this.DetailAction = configStore.MenuByName(infoAreaConfig.DefaultAction);
                    }
                }

                fieldGroupName = this.SearchConfiguration.FieldGroupName;
            }
            else
            {
                fieldGroupName = this.ConfigName;
            }

            this.FilterName = timelineInfoAreaConfiguration.FilterName;

            this.ListFieldControl = configStore.FieldControlByNameFromGroup(@"List", fieldGroupName);

            if (this.ListFieldControl != null)
            {
                this.DropdownFieldControl = configStore.FieldControlByNameFromGroup(@"MiniDetails", fieldGroupName);

                if (this.DropdownFieldControl == null)
                {
                    this.DropdownFieldControl = configStore.FieldControlByNameFromGroup(@"Details", fieldGroupName);

                    if (this.DropdownFieldControl != null)
                    {
                        this.DropdownFieldControl = this.DropdownFieldControl.FieldControlWithSingleTab(0);
                    }
                }

                this.CombinedControl = this.DropdownFieldControl == null
                                           ? this.ListFieldControl
                                           : new FieldControl(
                                                 new List<FieldControl>
                                                     {
                                                         this.ListFieldControl,
                                                         this.DropdownFieldControl
                                                     });
            }

            if (this.TimelineConfiguration.ColorCriteria.Count > 0)
            {
                Dictionary<string, UPCRMField> additionalFieldDictionary = null;

                foreach (ConfigTimelineCriteria criteria in this.TimelineConfiguration.ColorCriteria)
                {
                    if (criteria.FieldId < 0)
                    {
                        continue;
                    }

                    UPCRMField field = new UPCRMField(criteria.FieldId, this.TimelineConfiguration.InfoAreaId);

                    string key = field.FieldId.ToString();

                    if (additionalFieldDictionary == null)
                    {
                        additionalFieldDictionary = new Dictionary<string, UPCRMField>();
                    }

                    if (!additionalFieldDictionary.ContainsKey(key))
                    {
                        additionalFieldDictionary.Add(key, field);
                    }
                }

                this.AdditionalOutputFields = additionalFieldDictionary.Values.Select(x => x).ToList();
            }

            if (this.DetailAction == null)
            {
                this.DetailAction = configStore.MenuByName(@"SHOWRECORD");
            }

            this.ReplaceCaseSensitiveCharacters = configStore.ConfigValueIsSet(@"Search.ReplaceCaseSensitiveCharacters");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchPageModelControllerPreparedSearch"/> class.
        /// </summary>
        /// <param name="searchPageModelController">
        /// The search page.
        /// </param>
        public UPSearchPageModelControllerPreparedSearch(/*UPSearchPageModelController*/ dynamic searchPageModelController)
        {
            this.Initialize(searchPageModelController.InfoAreaId, searchPageModelController.ConfigName, searchPageModelController.FilterName);

            this.FilterParameter = searchPageModelController.ViewReference.ParamsDictionary();
            this.FilterObject = searchPageModelController.FilterObject;
            this.DetailActionSwitchFilterName =
                searchPageModelController.ViewReference.ContextValueForKey(@"DetailsActionSwitchFilterName");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchPageModelControllerPreparedSearch"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The info area id.
        /// </param>
        /// <param name="configName">
        /// The config name.
        /// </param>
        /// <param name="filterName">
        /// The filter name.
        /// </param>
        public UPSearchPageModelControllerPreparedSearch(string infoAreaId, string configName, string filterName)
        {
            Initialize(infoAreaId, configName, filterName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSearchPageModelControllerPreparedSearch"/> class.
        /// </summary>
        /// <param name="pageInfoArea">
        /// The page info area.
        /// </param>
        public UPSearchPageModelControllerPreparedSearch(UPCalendarPageInfoArea pageInfoArea)
            : this(pageInfoArea.InfoAreaId, pageInfoArea.SearchAndList?.UnitName, pageInfoArea.Filter?.UnitName)
        {
            this.CalendarPageInfoArea = pageInfoArea;

            if (this.CalendarPageInfoArea.ShowRecordViewReference == null)
            {
                this.CalendarPageInfoArea.ShowRecordViewReference = this.DetailAction.ViewReference;
            }
        }

        /// <summary>
        /// Gets the additional output fields.
        /// </summary>
        public List<UPCRMField> AdditionalOutputFields { get; private set; }

        /// <summary>
        /// Gets the calendar page info area.
        /// </summary>
        public UPCalendarPageInfoArea CalendarPageInfoArea { get; private set; }

        /// <summary>
        /// Gets the combined control.
        /// </summary>
        public FieldControl CombinedControl { get; private set; }

        /// <summary>
        /// Gets the config name.
        /// </summary>
        public string ConfigName { get; private set; }

        /// <summary>
        /// Gets the detail action.
        /// </summary>
        public Menu DetailAction { get; private set; }

        /// <summary>
        /// Gets the detail action switch filter.
        /// </summary>
        public UPConfigFilter DetailActionSwitchFilter { get; private set; }

        /// <summary>
        /// Gets the detail action switch filter name.
        /// </summary>
        public string DetailActionSwitchFilterName { get; private set; }

        /// <summary>
        /// Gets the dropdown field control.
        /// </summary>
        public FieldControl DropdownFieldControl { get; private set; }

        /// <summary>
        /// Gets the expand checker.
        /// </summary>
        public UPConfigExpand ExpandChecker { get; private set; }

        /// <summary>
        /// Gets the expand settings.
        /// </summary>
        public UPConfigExpand ExpandSettings { get; private set; }

        /// <summary>
        /// Gets the filter based decision.
        /// </summary>
        public UPCRMFilterBasedDecision FilterBasedDecision { get; private set; }

        /// <summary>
        /// Gets the filter name.
        /// </summary>
        public string FilterName { get; private set; }

        /// <summary>
        /// Gets the filter object.
        /// </summary>
        public UPConfigFilter FilterObject { get; private set; }

        /// <summary>
        /// Gets or sets the filter parameter.
        /// </summary>
        public Dictionary<string, object> FilterParameter { get; set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets or sets the link id.
        /// </summary>
        public int LinkId { get; set; }

        /// <summary>
        /// Gets or sets the link record identification.
        /// </summary>
        public string LinkRecordIdentification { get; set; }

        /// <summary>
        /// Gets the list field control.
        /// </summary>
        public FieldControl ListFieldControl { get; private set; }

        /// <summary>
        /// Gets the multi search crm fields.
        /// </summary>
        public List<UPCRMField> MultiSearchCRMFields { get; private set; }

        /// <summary>
        /// Gets the quick search entries.
        /// </summary>
        public List<QuickSearchEntry> QuickSearchEntries { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether replace case sensitive characters.
        /// </summary>
        public bool ReplaceCaseSensitiveCharacters { get; set; }

        /// <summary>
        /// Gets the search configuration.
        /// </summary>
        public SearchAndList SearchConfiguration { get; private set; }

        /// <summary>
        /// Gets the search crm fields.
        /// </summary>
        public List<UPCRMField> SearchCRMFields { get; private set; }

        /// <summary>
        /// Gets the search field control.
        /// </summary>
        public FieldControl SearchFieldControl { get; private set; }

        /// <summary>
        /// Gets the timeline configuration.
        /// </summary>
        public ConfigTimelineInfoArea TimelineConfiguration { get; }

        /// <summary>
        /// The crm query for value.
        /// </summary>
        /// <param name="searchValue">
        /// The search value.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <param name="fullTextSearch">
        /// The full text search.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerMetaInfo"/>.
        /// </returns>
        public UPContainerMetaInfo CrmQueryForValue(
            string searchValue,
            List<UPConfigFilter> filters,
            bool fullTextSearch)
        {
            if (this.CombinedControl == null)
            {
                return null;
            }

            if (this.SearchCRMFields == null && this.MultiSearchCRMFields == null
                && this.SearchFieldControl?.Tabs?.Count > 0)
            {
                int tabCount = this.SearchFieldControl.Tabs.Count;

                List<UPCRMField> fieldArray = null;
                List<UPCRMField> multiFieldArray = null;

                for (int i = 0; i < tabCount; i++)
                {
                    FieldControlTab tab = this.SearchFieldControl.TabAtIndex(i);
                    if (tab.Fields == null || tab.Fields.Count == 0)
                    {
                        continue;
                    }

                    if (string.Compare(tab.Type, @"MULTI", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        UPCRMField multiField = tab.FieldAtIndex(0).Field;

                        if (multiFieldArray == null)
                        {
                            multiFieldArray = new List<UPCRMField>();
                        }

                        multiFieldArray.Add(multiField);
                    }
                    else
                    {
                        if (fieldArray == null)
                        {
                            fieldArray = new List<UPCRMField>();
                        }

                        fieldArray.AddRange(tab.AllCRMFields());
                    }
                }

                this.SearchCRMFields = fieldArray;
                this.MultiSearchCRMFields = multiFieldArray;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPContainerMetaInfo container = new UPContainerMetaInfo(this.CombinedControl);

            if (!string.IsNullOrEmpty(this.DetailActionSwitchFilterName))
            {
                this.DetailActionSwitchFilter = ConfigurationUnitStore.DefaultStore.FilterByName(this.DetailActionSwitchFilterName);

                if (this.DetailActionSwitchFilter != null)
                {
                    this.DetailActionSwitchFilter =
                        this.DetailActionSwitchFilter.FilterByApplyingValueDictionaryDefaults(
                            this.FilterParameter,
                            true);
                }

                if (this.DetailActionSwitchFilter != null)
                {
                    this.FilterBasedDecision = new UPCRMFilterBasedDecision(this.DetailActionSwitchFilter);
                }
            }

            container.ReplaceCaseSensitiveCharacters = this.ReplaceCaseSensitiveCharacters;

            List<UPCRMField> additionalFields = null;

            if (this.ExpandSettings != null)
            {
                Dictionary<string, UPCRMField> alternateExpandFields =
                    this.ExpandSettings.FieldsForAlternateExpands(true);
                List<UPCRMField> _additionalFields = this.AdditionalOutputFields != null
                                                         ? new List<UPCRMField>(this.AdditionalOutputFields)
                                                         : new List<UPCRMField>();

                if (alternateExpandFields != null)
                {
                    _additionalFields.AddRange(alternateExpandFields.Values.Where(field => container.ContainsField(field) == null));
                }

                additionalFields = _additionalFields;
            }
            else if (this.AdditionalOutputFields?.Count > 0)
            {
                additionalFields = this.AdditionalOutputFields;
            }

            if (additionalFields != null && additionalFields.Count > 0)
            {
                container.AddCrmFields(additionalFields);
            }

            var checkFilterFields = this.FilterBasedDecision?.FieldDictionary.Values.Select(x => x).ToList();

            if (checkFilterFields?.Count > 0)
            {
                container.AddCrmFields(checkFilterFields);
                this.FilterBasedDecision.UseCrmQuery(container);
            }

            if (this.ExpandSettings != null)
            {
                this.ExpandChecker = this.ExpandSettings.ExpandCheckerForCrmQuery(container);
            }

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                container.SetLinkRecordIdentification(this.LinkRecordIdentification);
            }

            if (this.SearchFieldControl != null)
            {
                container.SetSearchConditionsFor(
                    searchValue,
                    this.SearchCRMFields,
                    this.MultiSearchCRMFields,
                    fullTextSearch);
            }
            else if (this.QuickSearchEntries != null)
            {
                if (!string.IsNullOrEmpty(searchValue))
                {
                    List<UPCRMField> crmFields = this.QuickSearchEntries.Select(entry => entry.CrmField).ToList();

                    container.SetSearchConditionsFor(searchValue, crmFields, fullTextSearch);
                }
            }

            if (!string.IsNullOrEmpty(this.SearchConfiguration?.FilterName))
            {
                UPConfigFilter filter = configStore.FilterByName(this.SearchConfiguration.FilterName);

                if (filter != null)
                {
                    filter = filter.FilterByApplyingDefaultReplacements();
                    filter = filter.FilterByApplyingValueDictionary(this.FilterParameter);
                    container.ApplyFilter(filter);
                }
            }

            if (this.FilterObject != null)
            {
                container.ApplyFilter(this.FilterObject);
            }
            else if (!string.IsNullOrEmpty(this.FilterName))
            {
                UPConfigFilter filter = configStore.FilterByName(this.FilterName);
                if (filter != null)
                {
                    filter = filter.FilterByApplyingDefaultReplacements();
                    filter = filter.FilterByApplyingValueDictionary(this.FilterParameter);
                    container.ApplyFilter(filter);
                }
            }

            if (filters != null)
            {
                foreach (UPConfigFilter filter in filters)
                {
                    if (this.FilterParameter != null)
                    {
                        container.ApplyFilterWithReplacementDictionary(filter, this.FilterParameter);
                    }
                    else
                    {
                        container.ApplyFilter(filter);
                    }
                }
            }

            return container;
        }

        /// <summary>
        /// The set filter parameter.
        /// </summary>
        /// <param name="filterParameter">
        /// The filter parameter.
        /// </param>
        public void SetFilterParameter(Dictionary<string, object> filterParameter)
        {
            this.FilterParameter = filterParameter;
        }

        private void Initialize(string infoAreaId, string configName, string filterName)
        {
            this.InfoAreaId = infoAreaId;
            this.ConfigName = configName;

            if (string.IsNullOrEmpty(this.ConfigName))
            {
                this.ConfigName = this.InfoAreaId;
            }

            this.FilterName = filterName;

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            this.SearchConfiguration = configStore.SearchAndListByName(this.ConfigName);

            if (this.SearchConfiguration == null)
            {
                FieldControl fieldControl = configStore.FieldControlByNameFromGroup(@"List", this.ConfigName);
                if (fieldControl != null)
                {
                    this.SearchConfiguration = new SearchAndList(fieldControl);
                }
            }

            if (this.InfoAreaId == null)
            {
                this.InfoAreaId = this.SearchConfiguration != null
                                      ? this.SearchConfiguration.InfoAreaId
                                      : this.ConfigName;
            }

            this.ExpandSettings = configStore.ExpandByName(this.ConfigName) ?? configStore.ExpandByName(this.InfoAreaId);

            if (!string.IsNullOrEmpty(this.SearchConfiguration?.DefaultAction))
            {
                this.DetailAction = configStore.MenuByName(this.SearchConfiguration.DefaultAction);
            }

            if (this.DetailAction == null)
            {
                InfoArea infoAreaConfig = configStore.InfoAreaConfigById(this.InfoAreaId);

                if (!string.IsNullOrEmpty(infoAreaConfig?.DefaultAction))
                {
                    this.DetailAction = configStore.MenuByName(infoAreaConfig.DefaultAction);
                }
            }

            if (this.DetailAction == null)
            {
                this.DetailAction = configStore.MenuByName(@"SHOWRECORD");
            }

            if (this.SearchConfiguration != null)
            {
                this.ListFieldControl = configStore.FieldControlByNameFromGroup(
                    @"List",
                    this.SearchConfiguration.FieldGroupName);

                if (this.ListFieldControl != null)
                {
                    this.SearchFieldControl = configStore.FieldControlByNameFromGroup(
                        @"Search",
                        this.SearchConfiguration.FieldGroupName);
                    this.DropdownFieldControl = configStore.FieldControlByNameFromGroup(
                        @"MiniDetails",
                        this.SearchConfiguration.FieldGroupName);

                    if (this.DropdownFieldControl == null)
                    {
                        this.DropdownFieldControl = configStore.FieldControlByNameFromGroup(
                            @"Details",
                            this.SearchConfiguration.FieldGroupName);

                        this.DropdownFieldControl = this.DropdownFieldControl?.FieldControlWithSingleTab(0);
                    }

                    this.CombinedControl = this.DropdownFieldControl == null
                                               ? this.ListFieldControl
                                               : new FieldControl(
                                                     new List<FieldControl>
                                                         {
                                                             this.ListFieldControl,
                                                             this.DropdownFieldControl
                                                         });
                }
            }

            this.ReplaceCaseSensitiveCharacters = configStore.ConfigValueIsSet(@"Search.ReplaceCaseSensitiveCharacters");
        }
    }
}
