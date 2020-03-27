// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalendarPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Calendar Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Calendar View Type
    /// </summary>
    public enum UPCalendarViewType
    {
        Undefine = 0,
        Day = 1,
        Week = 2,
        Month = 3,
        List = 4,
        Year = 5,
        Time = 6,
    }

    /// <summary>
    /// Entity Type
    /// </summary>
    public enum EKEntityType
    {
        /// <summary>
        /// Event
        /// </summary>
        Event,

        /// <summary>
        /// Reminder
        /// </summary>
        Reminder
    }

    /// <summary>
    /// Calendar Page Model Controller
    /// </summary>
    /// <seealso cref="UPStandardSearchPageModelController" />
    /// <seealso cref="ICalendarViewControllerDataProvider" />
    public class UPCalendarPageModelController : UPStandardSearchPageModelController, ICalendarViewControllerDataProvider
    {
        private const string DefaultViewTypeDay = "DAY";
        private const string DefaultViewTypeList = "LIST";
        private const string DefaultViewTypeMonth = "MONTH";
        private const string DefaultViewTypeTime = "TIME";
        private const string DefaultViewTypeWeek = "WEEK";
        private const string DefaultViewTypeYear = "YEAR";

        private const string KeyAdditionalCalendarConfigs = "AdditionalCalendarConfigs";
        private const string KeyCalendarIncludeSystemCalendar = "CalendarIncludeSystemCalendar";
        private const string KeyCalendarShowLastEditedDate = "CalendarShowLastEditedDate";
        private const string KeyDefaultViewType = "DefaultViewType";
        private const string KeyIncludeSystemCalendar = "IncludeSystemCalendar";
        private const string KeyNewAppointmentAction = "NewAppointmentAction";
        private const string KeyLinkRecord = "LinkRecord";
        private const string KeyRepFilter = "RepFilter";
        private const string KeyRepFilterCurrentRepActive = "RepFilterCurrentRepActive";

        private UPCalendarViewType viewType;
        private bool jumpBackToEditedValue;
        private int currentPreparedSearchIndex;
        private bool pendingOperation;
        private List<UPCalendarSearch> calendarSearches;
        private UPCalendarSearch currentSearch;
        private DateTime? filterFromDate;
        private DateTime? filterToDate;
        private bool noIpadCalendar;

        private static readonly object Lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCalendarPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public UPCalendarPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Gets the iPad calendar filter.
        /// </summary>
        public UPMCatalogFilter IPadCalendarFilter { get; protected set; }

        /// <summary>
        /// Gets the rep filter.
        /// </summary>
        public UPMFilter RepFilter { get; protected set; }

        /// <summary>
        /// Gets the root information area configuration.
        /// </summary>
        /// <value>
        /// The root information area configuration.
        /// </value>
        public UPCalendarPageInfoArea RootInfoAreaConfig { get; private set; }

        /// <summary>
        /// Gets the additional prepared searches.
        /// </summary>
        /// <value>
        /// The additional prepared searches.
        /// </value>
        public List<UPSearchPageModelControllerPreparedSearch> AdditionalPreparedSearches { get; private set; }

        /// <summary>
        /// Gets the additional information area configs.
        /// </summary>
        /// <value>
        /// The additional information area configs.
        /// </value>
        public List<UPCalendarPageInfoArea> AdditionalInfoAreaConfigs { get; private set; }

        /// <summary>
        /// Gets main prepared search
        /// </summary>
        public override UPSearchPageModelControllerPreparedSearch MainPreparedSearch => new UPSearchPageModelControllerPreparedSearch(this.RootInfoAreaConfig);

        /// <summary>
        /// Gets the calendar page.
        /// </summary>
        public UPMCalendarPage CalendarPage => (UPMCalendarPage)this.SearchPage;

        /// <summary>
        /// Creates the page instance.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public override UPMSearchPage CreatePageInstance(IIdentifier identifier)
        {
            return new UPMCalendarPage(identifier);
        }

        /// <summary>
        /// The create new calendar item for date with button.
        /// </summary>
        /// <param name="date">The _date.</param>
        /// <param name="button">The button.</param>
        public void CreateNewCalendarItemForDate(DateTime date, UPConfigButton button)
        {
            var dateFormatter = UPCRMTimeZone.Current.ClientDataDateFormatter;
            var timeFormatter = UPCRMTimeZone.Current.ClientDataTimeFormatter;

            if (button != null)
            {
                ViewReference oldViewReference = button.ViewReference;
                string cv = oldViewReference.ContextValueForKey("AdditionalParameters");
                cv = cv.Replace("$Date$", dateFormatter.StringFromDate(date));
                cv = cv.Replace("$Time$", timeFormatter.StringFromDate(date));
                ViewReference newViewReference = new ViewReference(oldViewReference, null, cv, "AdditionalParameters");
                newViewReference = newViewReference.ViewReferenceWith(this.LinkRecordIdentification);
                this.ParentOrganizerModelController.PerformActionWithViewReference(newViewReference);
            }
        }

        /// <summary>
        /// Builds page details
        /// </summary>
        public override void BuildPageDetails()
        {
            SearchPage.AvailableViewTypes = GetAvailableViewTypes();
            SearchPage.DefaultViewType = SearchPageViewType.CalendarMonth;
            var defaultViewType = ViewReference.ContextValueForKey(KeyDefaultViewType) ?? DefaultViewTypeMonth;
            SetSearchPageDefaultViewType(defaultViewType);
            RootInfoAreaConfig = new UPCalendarPageInfoArea(ViewReference, null);

            SetAppointmentActionsAndAdditionalInfoAreaConfig();

            base.BuildPageDetails();

            SetAdditionalPreparedSearches();

            SetCalendarPageProperties();

            if (CalendarPage.IncludeSystemCalendar)
            {
                SetIPadCalendarFilters();
            }
        }

        /// <summary>
        /// Returns filter name for index
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns>
        ///   <see cref="string" />
        /// </returns>
        protected override string AlternateFilterNameForIndex(int i)
        {
            if (i < 1 || this.AdditionalInfoAreaConfigs == null)
            {
                return null;
            }

            foreach (UPCalendarPageInfoArea pageInfoArea in this.AdditionalInfoAreaConfigs)
            {
                if (pageInfoArea.FilterArray.Count >= i)
                {
                    var filter = pageInfoArea.FilterArray[i - 1];
                    return filter?.UnitName;
                }
            }

            return null;
        }

        /// <summary>
        /// Switches to detail.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public override async void SwitchToDetail(object sender)
        {
            ResultRowCalendarItem resultRow = (ResultRowCalendarItem)sender;
            UPCalendarPageInfoArea calendarInfoArea = resultRow.CalendarSearch?.PreparedSearch.CalendarPageInfoArea;

            if (calendarInfoArea?.ShowRecordViewReference != null)
            {
                if (resultRow.OnlineData)
                {
                    bool connectedToServer = ServerSession.CurrentSession.ConnectedToServer;
                    if (!connectedToServer)
                    {
                        await SimpleIoc.Default.GetInstance<IDialogService>()
                            .ShowMessage(LocalizedString.TextErrorMessageNoInternet, LocalizedString.TextErrorTitleNoInternet);
                        return;
                    }
                }

                this.CurrentDetailRow = resultRow;
                RecordIdentifier identifier = (RecordIdentifier)resultRow.Identifier;
                UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(
                    calendarInfoArea.ShowRecordViewReference.ViewReferenceWith(identifier.RecordIdentification));
                organizerModelController.OnlineData = resultRow.OnlineData;
                organizerModelController.ShouldShowTabsForSingleTab = true;
                this.EnableSwipingForOrganizerInfoAreaId(organizerModelController, identifier);
                this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
                return;
            }

            base.SwitchToDetail(sender);
        }

        /// <summary>
        /// Performs search
        /// </summary>
        /// <param name="sender">Sender object</param>
        public override void Search(object sender)
        {
            if (!this.pendingOperation)
            {
                this.currentPreparedSearchIndex = 0;
                this.calendarSearches = new List<UPCalendarSearch>();
                if (this.CalendarPage.IncludeSystemCalendar && this.CalendarPage.ShowSystemCalendar)
                {
                    List<UPConfigQueryCondition> dateFilter = null;
                    var configFilters = UPMFilter.ActiveFiltersForFilters(this.SearchPage.AvailableFilters);
                    foreach (UPConfigFilter configFilter in configFilters)
                    {
                        var conditionsWithDate = configFilter.FlatConditionsWith("Date");
                        if (conditionsWithDate.Count > 0)
                        {
                            if (dateFilter == null)
                            {
                                dateFilter = new List<UPConfigQueryCondition>();
                            }

                            dateFilter.AddRange(conditionsWithDate);
                        }
                    }

                    this.noIpadCalendar = false;
                    this.filterFromDate = null;
                    this.filterToDate = null;

                    if (dateFilter != null && dateFilter.Count > 0)
                    {
                        foreach (UPConfigQueryCondition queryCondition in dateFilter)
                        {
                            if (queryCondition.SubConditions.Count > 0 || queryCondition.FieldValues.Count != 1 || string.IsNullOrEmpty(queryCondition.FirstValue))
                            {
                                continue;
                            }

                            DateTime fieldValueDate = queryCondition.FirstValue.ReplaceDateVariables().DateFromCrmValue().GetValueOrDefault();
                            if (queryCondition.CompareOperator == ">")
                            {
                                fieldValueDate = fieldValueDate.EndDateForDate();
                            }

                            if (queryCondition.CompareOperator == "=" || queryCondition.CompareOperator == ">=" || queryCondition.CompareOperator == ">")
                            {
                                if (this.filterFromDate == null)
                                {
                                    this.filterFromDate = fieldValueDate;
                                }
                                else
                                {
                                    if (this.filterFromDate > fieldValueDate)
                                    {
                                        this.noIpadCalendar = true;
                                    }
                                    else
                                    {
                                        this.filterFromDate = fieldValueDate;
                                    }
                                }
                            }

                            if (queryCondition.CompareOperator == "=" || queryCondition.CompareOperator == "<=" || queryCondition.CompareOperator == "<")
                            {
                                if (queryCondition.CompareOperator != "<")
                                {
                                    fieldValueDate = fieldValueDate.EndDateForDate();
                                }

                                if (this.filterToDate == null)
                                {
                                    this.filterToDate = fieldValueDate;
                                }
                                else
                                {
                                    if (this.filterFromDate > fieldValueDate)
                                    {
                                        this.noIpadCalendar = true;
                                    }
                                    else
                                    {
                                        this.filterToDate = fieldValueDate;
                                    }
                                }
                            }
                        }
                    }
                }

                this.NextOperation();
            }
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="origSearchPage">The original search page.</param>
        /// <returns></returns>
        public override Page UpdatedElementForPage(UPMSearchPage origSearchPage)
        {
            Dictionary<string, string> editedValuesDict = UPAppProcessContext.CurrentContext.ContextValueForKey(
                UPAppProcessContext.AppContextEditFieldValueTransferResponse,
                this.ParentOrganizerModelController?.NavControllerId ?? 0) as Dictionary<string, string>;

            if (editedValuesDict != null)
            {
                string dateString = editedValuesDict["UPCalenderStartDate"];
                editedValuesDict.Remove("UPCalenderStartDate");

                if (!string.IsNullOrEmpty(dateString))
                {
                    DateTime startDate = dateString.DateFromCrmValue().GetValueOrDefault();
                    this.InformAboutDidChangeTopLevelElement(this.CalendarPage, this.CalendarPage, null,
                        UPChangeHints.ChangeHintsWithHintDetailHints("setDate", new Dictionary<string, object> { { "startDate", startDate } }));
                }
                else
                {
                    this.Search((UPMSearchPage)this.Page);
                }
            }
            else
            {
                this.Search((UPMSearchPage)this.Page);
            }

            return this.Page;
        }

        // JIRE # 195 fix : Remove multiple search operation when opening calendar view.
        public override void UpdateElementForCurrentChanges(List<IIdentifier> changes)
        {

        }

        /// <summary>
        /// Updateds the element for result row.
        /// </summary>
        /// <param name="origResultRow">The original result row.</param>
        /// <returns></returns>
        public override UPMResultRow UpdatedElementForResultRow(UPMResultRow origResultRow)
        {
            var rowCalendarItem = origResultRow as ResultRowCalendarItem;
            if (rowCalendarItem != null)
            {
                if (rowCalendarItem.IPadCalendarItem != null)
                {
                    rowCalendarItem.UpdateEKEventValues();
                    return rowCalendarItem;
                }
            }

            return base.UpdatedElementForResultRow(origResultRow);
        }

        /// <summary>
        /// The register add start date.
        /// </summary>
        /// <param name="date">The date.</param>
        public void RegisterAddStartDate(DateTime date)
        {
            Dictionary<string, string> addParams = new Dictionary<string, string>
            {
                { "$Date$", date.ToString("yyyyMMdd") },
                { "$Time$", date.ToString("HHmm") }
            };

            this.ParentOrganizerModelController?.PageContextValueforKey(addParams, "AdditionalParametersOverride");
            this.CalendarPage.Invalid = true;
        }

        /// <summary>
        /// Informs about change of top level element.
        /// </summary>
        /// <param name="oldTopLevelElement">The old top level element.</param>
        /// <param name="newTopLevelElement">The new top level element.</param>
        /// <param name="changedIdentifiers">The changed identifiers.</param>
        /// <param name="changeHints">The change hints.</param>
        public override void InformAboutDidChangeTopLevelElement(ITopLevelElement oldTopLevelElement,
            ITopLevelElement newTopLevelElement, List<IIdentifier> changedIdentifiers, UPChangeHints changeHints)
        {
            UPMCalendarPage page = (UPMCalendarPage)newTopLevelElement;
            if (this.CalendarPage.CalendarFromDate != null && this.CalendarPage.CalendarToDate != null)
            {
                page.CalendarItems = this.CalendarItemsFromSearchResult();
            }

            page.PopoverTitle = this.GetPopoverTitle();
            base.InformAboutDidChangeTopLevelElement(oldTopLevelElement, newTopLevelElement, changedIdentifiers, changeHints);
        }

        /// <summary>
        /// Retrieves active filters for given search page
        /// </summary>
        /// <param name="searchPage">Search page</param>
        /// <returns>
        ///   <see cref="List{UPConfigFilter}" />
        /// </returns>
        protected override List<UPConfigFilter> ActiveFiltersForSearchPage(UPMSearchPage searchPage)
        {
            List<UPConfigFilter> currentActiveFilters = new List<UPConfigFilter>();
            foreach (UPMFilter filter in this.SearchPage.AvailableFilters)
            {
                if (filter.Active)
                {
                    if (filter.Equals(this.RepFilter))
                    {
                        UPConfigFilter currentRepFilter = filter.ConfigFilter();
                        if (currentRepFilter != null)
                        {
                            currentActiveFilters.Add(currentRepFilter);
                        }
                    }
                    else
                    {
                        if (this.FilterIndexMapping.ContainsKey(filter.Name))
                        {
                            int filterNumber = this.FilterIndexMapping[filter.Name];
                            UPConfigFilter currentFilter = this.currentSearch.FilterIndexMapping[filterNumber.ToString()];
                            if (currentFilter != null)
                            {
                                currentFilter = UPMFilter.ConfigFilterFromFilterAndConfigFilter(filter, currentFilter);
                                if (currentFilter != null)
                                {
                                    currentActiveFilters.Add(currentFilter);
                                }
                            }
                        }
                    }
                }
            }

            return currentActiveFilters;
        }

        /// <summary>
        /// Builds the calendar page.
        /// </summary>
        /// <param name="searchPage">The search page.</param>
        private void BuildCalendarPage(UPMSearchPage searchPage)
        {
            lock (Lock)
            {
                foreach (UPCalendarSearch calendarSearch in this.calendarSearches)
                {
                    calendarSearch.ResultContext = new UPCoreMappingResultContext(
                        calendarSearch.Result,
                        calendarSearch.PreparedSearch.CombinedControl,
                        calendarSearch.PreparedSearch.ListFieldControl.NumberOfFields)
                    {
                        ExpandMapper = calendarSearch.PreparedSearch.ExpandChecker
                    };
                    this.SectionContexts[calendarSearch.PreparedSearch.InfoAreaId] = calendarSearch.ResultContext;
                }

                string sortSequence = this.ViewReference.ContextValueForKey("SortSequence");
                bool isDescending = sortSequence == null || sortSequence == "DESC";
                List<ICalendarItem> availableResults = this.CalendarItemsFromSearchResult();
                availableResults.Sort(!isDescending || this.viewType == UPCalendarViewType.Day);
                ((UPMCalendarPage)searchPage).DescSort = isDescending;

                //if (this.viewType == UPCalendarViewType.Month || this.viewType == UPCalendarViewType.Week || this.viewType == UPCalendarViewType.Day)

                if (searchPage.DefaultViewType == SearchPageViewType.CalendarMonth || searchPage.DefaultViewType == SearchPageViewType.CalendarWeek || searchPage.DefaultViewType == SearchPageViewType.CalendarDay)
                {
                    UPMResultSection resultSection = new UPMResultSection(
                        StringIdentifier.IdentifierWithStringId("Result_Section_1"),
                        new UPMResultRowProviderForCalendarResult(availableResults, this, this.ResultContext));

                    searchPage.AddResultSection(resultSection);
                    return;
                }

                searchPage.HideSectionIndex = this.ResultContext?.SectionFieldComplete ?? false;

                List<UPMResultSection> resultSections = availableResults.ResultSectionsForSortedData(this.ResultContext, this);
                searchPage.RemoveAllChildren();
                foreach (UPMResultSection resultSection in resultSections)
                {
                    searchPage.AddResultSection(resultSection);
                }
            }
        }

        private void NextOperation()
        {
            if (this.currentPreparedSearchIndex == 0)
            {
                var currentSearch = new UPCalendarSearch(this.PreparedSearch);
                this.calendarSearches.Add(currentSearch);
                this.currentSearch = currentSearch;
                base.Search(this.SearchPage);
            }
            else if (this.currentPreparedSearchIndex < this.AdditionalPreparedSearches?.Count + 1)
            {
                this.Search();
            }
            else
            {
                this.FinishedLoading();
            }
        }

        private void Search()
        {
            var preparedSearch = this.AdditionalPreparedSearches[this.currentPreparedSearchIndex - 1];
            var crmQuery = preparedSearch.CrmQueryForValue(this.SearchPage.SearchText, null, this.FullTextSearch);
            this.currentSearch = new UPCalendarSearch(preparedSearch)
            {
                CrmQuery = crmQuery
            };
            this.calendarSearches.Add(this.currentSearch);

            if (this.MaxResults > 0)
            {
                crmQuery.MaxResults = this.MaxResults;
            }

            var currentActiveFilters = this.GetCurrentActiveFilters();

            if (!string.IsNullOrWhiteSpace(this.LinkRecordIdentification) && !preparedSearch.CalendarPageInfoArea.IgnoreLink)
            {
                crmQuery.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
            }

            foreach (var filter in currentActiveFilters)
            {
                crmQuery.ApplyFilter(filter);
            }

            this.PrepareQuery(crmQuery, this.currentSearch);
            if (this.SearchPage.SearchType == SearchPageSearchType.OnlineSearch)
            {
                this.CurrentSearchOperation = crmQuery.Find(UPRequestOption.Online, this);
            }
            else
            {
                if (this.InitialSearch)
                {
                    this.CurrentSearchOperation = crmQuery.Find(this.InitialRequestOption, this);
                    if (this.CurrentSearchOperation != null)
                    {
                        this.InitialSearch = false;
                        if (this.CurrentSearchOperation is RemoteSearchOperation)
                        {
                            this.SearchPage.InitiallyOnline = true;
                            this.SearchPage.SearchType = SearchPageSearchType.OnlineSearch;
                        }
                    }
                    else
                    {
                        this.SearchPage.Invalid = false;
                        this.SearchPage.ResultState = SearchPageResultState.OnlyOnline;
                        this.SearchPage.Status = null;
                        this.InformAboutDidChangeTopLevelElement(this.SearchPage, this.SearchPage, null, null);
                    }
                }
                else
                {
                    this.SearchPage.InitiallyOnline = false;
                    this.CurrentSearchOperation = crmQuery.Find(
                        this.SearchPage.SearchType == SearchPageSearchType.OnlineSearch
                        ? UPRequestOption.Online
                        : UPRequestOption.Offline, this);
                }
            }
        }

        private IList<UPConfigFilter> GetCurrentActiveFilters()
        {
            var currentActiveFilters = new List<UPConfigFilter>();
            foreach (var filter in this.SearchPage.AvailableFilters)
            {
                if (filter.Active)
                {
                    if (filter.Equals(this.RepFilter) && this.currentSearch.RepFilter != null)
                    {
                        var currentRepFilter = UPMFilter.ConfigFilterFromFilterAndConfigFilter(filter, this.currentSearch.RepFilter);
                        if (currentRepFilter != null)
                        {
                            currentActiveFilters.Add(currentRepFilter);
                        }
                    }
                    else
                    {
                        if (this.FilterIndexMapping.ContainsKey(filter.Name))
                        {
                            int filterNumber = this.FilterIndexMapping[filter.Name];
                            if (this.currentSearch.FilterIndexMapping.Count >= filterNumber)
                            {
                                var currentFilter = this.currentSearch.FilterIndexMapping[filterNumber.ToString()];
                                if (currentFilter != null)
                                {
                                    currentFilter = UPMFilter.ConfigFilterFromFilterAndConfigFilter(filter, currentFilter);
                                    if (currentFilter != null)
                                    {
                                        currentActiveFilters.Add(currentFilter);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return currentActiveFilters;
        }

        private void FinishedLoading()
        {
            this.pendingOperation = false;
            List<IIdentifier> identifiersList = new List<IIdentifier>
            {
                new RecordIdentifier(this.RootInfoAreaConfig.InfoAreaId, null)
            };

            if (this.AdditionalInfoAreaConfigs != null)
            {
                identifiersList.AddRange(this.AdditionalInfoAreaConfigs.Select(addInfoArea => new RecordIdentifier(addInfoArea.InfoAreaId, null)));
            }

            UPMSearchPage newPage = this.CreatePageInstance(new MultipleIdentifier(identifiersList));
            newPage.CopyDataFrom(this.SearchPage);
            newPage.Invalid = false;
            this.BuildCalendarPage(newPage);

            UPMAction switchToDetailAction = new UPMAction(null);
            switchToDetailAction.IconName = "arrow.png";
            switchToDetailAction.SetTargetAction(this, this.SwitchToDetail);
            newPage.RowAction = switchToDetailAction;

            UPMAction searchAction = new UPMAction(null);
            searchAction.SetTargetAction(this, this.Search);
            newPage.SearchAction = searchAction;

            UPCalendarSearch calendarSearch = this.calendarSearches.Count > 0 ? this.calendarSearches[0] : null;
            newPage.ResultState = this.CountResultState(calendarSearch?.Result);
            UPMSearchPage oldSearchPage = this.SearchPage;
            this.TopLevelElement = newPage;
            this.InformAboutDidChangeTopLevelElement(oldSearchPage, newPage, null, null);
        }

        /// <summary>
        /// Responsible for preparing query by given <see cref="UPContainerMetaInfo" />
        /// </summary>
        /// <param name="crmQuery">Metainfo data</param>
        protected override void PrepareQuery(UPContainerMetaInfo crmQuery)
        {
            if (this.calendarSearches.Count > 0)
            {
                this.PrepareQuery(crmQuery, this.calendarSearches[0]);
            }
        }

        /// <summary>
        /// Prepares the query calendar search.
        /// </summary>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="calendarSearch">The calendar search.</param>
        public void PrepareQuery(UPContainerMetaInfo crmQuery, UPCalendarSearch calendarSearch)
        {
            // Add fields for table caption
            UPSearchPageModelControllerPreparedSearch preparedSearch = calendarSearch.PreparedSearch;
            UPConfigTableCaption tableCaption = this.TableCaptionForCalendarItemsWithInfoAreaId(preparedSearch.InfoAreaId);

            if (tableCaption != null)
            {
                calendarSearch.CalendarTableCaptionResultFieldArray = tableCaption.AddTableCaptionFieldsToCrmQuery(crmQuery);
            }

            string fromDate = this.CalendarPage.CalendarFromDate?.CrmValueFromDate();
            string toDate = this.CalendarPage.CalendarToDate?.CrmValueFromDate();

            // Add date condition
            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                return;
            }

            FieldControl searchControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("Search", preparedSearch.SearchConfiguration.FieldGroupName);
            Dictionary<string, UPConfigFieldControlField> functionNameFieldMapping = searchControl.FunctionNames();
            UPConfigFieldControlField fromField = functionNameFieldMapping.ValueOrDefault("Date");
            UPConfigFieldControlField toField = functionNameFieldMapping.ValueOrDefault("EndDate");

            // Register edit information request
            if (fromField == null || toField == null)
            {
                FieldControl listControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", preparedSearch.SearchConfiguration.FieldGroupName);
                Dictionary<string, UPConfigFieldControlField> functionNameFieldMappingList = listControl.FunctionNames();

                if (fromField == null)
                {
                    fromField = functionNameFieldMappingList.ValueOrDefault("Date");
                }

                if (toField == null)
                {
                    toField = functionNameFieldMappingList.ValueOrDefault("EndDate");
                }
            }

            if (fromField != null && this.jumpBackToEditedValue)
            {
                Dictionary<string, object> editInfoFields = UPAppProcessContext.CurrentContext.GetOrCreateDictionaryContextValueForKey(
                    UPAppProcessContext.AppContextEditFieldValueTransferRequest, this.ParentOrganizerModelController?.NavControllerId ?? 0);

                editInfoFields["UPCalenderStartDate"] = fromField.Identification;
            }

            if (toField == null)
            {
                toField = fromField;
            }

            UPInfoAreaCondition fromCondition = new UPInfoAreaConditionLeaf(fromField.InfoAreaId, fromField.FieldId, "<=", toDate);
            UPInfoAreaCondition toCondition = new UPInfoAreaConditionLeaf(toField.InfoAreaId, toField.FieldId, ">=", fromDate);
            UPInfoAreaCondition fromCondition1 = new UPInfoAreaConditionLeaf(fromField.InfoAreaId, fromField.FieldId, ">=", fromDate);
            UPInfoAreaCondition fromCondition2 = new UPInfoAreaConditionLeaf(fromField.InfoAreaId, fromField.FieldId, "<=", toDate);

            UPInfoAreaCondition infoAreaCondition = fromCondition.InfoAreaConditionByAppendingAndCondition(toCondition);
            UPInfoAreaCondition infoAreaCondition2 = fromCondition1.InfoAreaConditionByAppendingAndCondition(fromCondition2);

            UPInfoAreaCondition combinedCondition = infoAreaCondition.InfoAreaConditionByAppendingOrCondition(infoAreaCondition2);

            crmQuery.RootInfoAreaMetaInfo.AddCondition(combinedCondition);
        }

        /// <summary>
        /// Tables the caption for calendar items with information area identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public UPConfigTableCaption TableCaptionForCalendarItemsWithInfoAreaId(string infoAreaId)
        {
            IConfigurationUnitStore unitStore = ConfigurationUnitStore.DefaultStore;
            var tableCaption = unitStore.TableCaptionByName($"{infoAreaId}.Calendar") ??
                               unitStore.TableCaptionByName(infoAreaId);
            return tableCaption;
        }

        /// <summary>
        /// CRMs the row for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPCRMResultRow CrmRowForRow(UPMResultRow row)
        {
            ResultRowCalendarItem resultRow = (ResultRowCalendarItem)row;
            return resultRow.CrmValue ? base.CrmRowForRow(row) : null;
        }

        /// <summary>
        /// Results the context for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPCoreMappingResultContext ResultContextForRow(UPMResultRow row)
        {
            var identifier = row.Identifier as RecordIdentifier;
            return identifier != null ? this.SectionContexts[identifier.InfoAreaId] : null;
        }

        /// <summary>
        /// Results the row at index offset.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override UPMResultRow ResultRowAtIndexOffset(int index)
        {
            if (this.CurrentDetailRow != null)
            {
                if (this.CurrentSectionIndex < this.SearchPage.Children.Count)
                {
                    UPMResultSection section = this.SearchPage.ResultSectionAtIndex(this.CurrentSectionIndex);
                    UPMResultRow row = this.CurrentDetailRow;
                    int currentRowIndex = this.IndexOfRowInSection(row, section);
                    if (currentRowIndex == -1)
                    {
                        return null;
                    }

                    int sectionIndex = this.CurrentSectionIndex;
                    if (index == 0)
                    {
                        return this.SearchPage.ResultSectionAtIndex(sectionIndex).ResultRowAtIndex(currentRowIndex);
                    }

                    while (index < 0)
                    {
                        currentRowIndex--;
                        if (currentRowIndex < 0)
                        {
                            sectionIndex--;
                            if (sectionIndex < 0)
                            {
                                return null;
                            }

                            currentRowIndex += this.SearchPage.ResultSectionAtIndex(sectionIndex).NumberOfResultRows;
                        }

                        if (sectionIndex < 0)
                        {
                            return null;
                        }

                        ResultRowCalendarItem resultRow = (ResultRowCalendarItem)this.SearchPage.ResultSectionAtIndex(sectionIndex).ResultRowAtIndex(currentRowIndex);
                        if (resultRow.CrmValue)
                        {
                            index++;
                        }

                        if (index == 0)
                        {
                            return resultRow;
                        }

                        while (index > 0)
                        {
                            currentRowIndex++;
                            if (currentRowIndex >= this.SearchPage.ResultSectionAtIndex(sectionIndex).NumberOfResultRows)
                            {
                                sectionIndex++;
                                if (sectionIndex >= this.SearchPage.NumberOfResultSections)
                                {
                                    return null;
                                }

                                currentRowIndex = 0;
                            }

                            resultRow = (ResultRowCalendarItem)this.SearchPage.ResultSectionAtIndex(sectionIndex).ResultRowAtIndex(currentRowIndex);
                            if (resultRow.CrmValue)
                            {
                                index--;
                            }

                            if (index == 0)
                            {
                                return resultRow;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Performs the edit action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PerformEditAction(object sender)
        {
            var dict = (Dictionary<string, object>)sender;
            UPMOrganizerAction action = (UPMOrganizerAction)dict["UPMOrganizerAction"];
            UPMResultRow resultRow = (UPMResultRow)dict["UPMOrganizerActionSender"];
            RecordIdentifier identifier = (RecordIdentifier)resultRow.Identifier;
            UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(action.ViewReference.ViewReferenceWith(identifier.RecordIdentification));
            this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }

        /// <summary>
        /// Calendars the items from search result.
        /// </summary>
        /// <returns></returns>
        public List<ICalendarItem> CalendarItemsFromSearchResult()
        {
            bool enableEdit = !ConfigurationUnitStore.DefaultStore?.ConfigValueIsSet("Calendar.NoPopupEdit") ?? false;
            DateTime? from = this.CalendarPage.CalendarFromDate;
            DateTime? to = this.CalendarPage.CalendarToDate;
            UPMAction goToAction = new UPMAction(StringIdentifier.IdentifierWithStringId("action"));
            goToAction.SetTargetAction(this, this.SwitchToDetail);
            goToAction.LabelText = LocalizedString.TextShowRecord;
            List<ICalendarItem> calendarItems = new List<ICalendarItem>();

            foreach (UPCalendarSearch calendarSearch in this.calendarSearches)
            {
                string currentInfoAreaId = calendarSearch.PreparedSearch.InfoAreaId;
                UPCoreMappingResultContext currentResultContext = calendarSearch.ResultContext;
                UPConfigTableCaption tableCaption = this.TableCaptionForCalendarItemsWithInfoAreaId(currentInfoAreaId);
                int fieldId = currentResultContext?.FieldControl.FieldWithFunction("Type")?.FieldId ?? 0;
                UPMOrganizerAction editAction = null;
                if (enableEdit && calendarSearch.PreparedSearch.CalendarPageInfoArea.EditRecordViewReference != null)
                {
                    editAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action"))
                    {
                        ViewReference = calendarSearch.PreparedSearch.CalendarPageInfoArea.EditRecordViewReference,
                        LabelText = LocalizedString.TextEdit
                    };
                    editAction.SetTargetAction(this, this.PerformEditAction);
                }

                UPConfigCatalogAttributes attributes = ConfigurationUnitStore.DefaultStore.CatalogAttributesForInfoAreaIdFieldId(currentInfoAreaId, fieldId);
                int count = currentResultContext?.Result?.RowCount ?? 0;
                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)currentResultContext.Result.ResultRowAtIndex(i);
                    UPConfigExpand expand = null;
                    AureaColor color = null;

                    if (currentResultContext.ExpandMapper != null)
                    {
                        expand = currentResultContext.ExpandMapper.ExpandForResultRow(row);
                    }

                    if (expand != null && (currentResultContext.ExpandMapper.AlternateExpands?.Count > 0 || this.SearchPageMode.HasFlag(SearchPageMode.ShowColorOnDefault)))
                    {
                        var infoAreaColorKey = expand.ColorKey;
                        if (!string.IsNullOrEmpty(infoAreaColorKey))
                        {
                            color = AureaColor.ColorWithString(infoAreaColorKey);
                        }
                    }

                    ICalendarItem calendarItem = new ResultRowCalendarItem(row, currentResultContext,
                        new RecordIdentifier(row.RootRecordIdentification), tableCaption, calendarSearch.CalendarTableCaptionResultFieldArray, attributes, color);
                    ((ResultRowCalendarItem)calendarItem).StyleId = calendarSearch.PreparedSearch.ConfigName;

                    ((ResultRowCalendarItem)calendarItem).CalendarSearch = calendarSearch;
                    calendarItem.GoToAction = goToAction;
                    calendarItem.EditAction = editAction;
                    calendarItems.Add(calendarItem);
                }
            }

            if (!this.noIpadCalendar)
            {
                if (from != null)
                {
                    if (this.filterFromDate != null && from < this.filterFromDate)
                    {
                        from = this.filterFromDate.Value;
                    }
                }
                else
                {
                    from = this.filterFromDate;
                }

                if (to != null)
                {
                    if (this.filterToDate != null && to > this.filterToDate)
                    {
                        to = this.filterToDate;
                    }
                }
                else
                {
                    to = this.filterToDate;
                }

                if (from == null || to == null || from < to)
                {
                    // calendarItems.AddRange(this.EventsFromLocalCalendarFromToSearchText(from.Value, to.Value, this.CalendarPage.SearchText));
                }
            }

            if (calendarItems.Count > 0)
            {
                this.SearchPage.ResultState = SearchPageResultState.Ok;
            }

            return calendarItems;
        }

        /// <summary>
        /// The sender needs calendar item details.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="item">The item.</param>
        public void SenderNeedsCalendarItemDetails(/*UPCalendarViewController */ object sender, ICalendarItem item)
        {
            if (item.CrmResultRow == null && item.ResultRow != null)
            {
                return;
            }

            UPMResultRow resultRow = new UPMResultRow(new RecordIdentifier(item.CrmResultRow.RootRecordIdentification));
            item.ResultRow = resultRow;
            resultRow.Invalid = true;
            resultRow.DataValid = true;

            UPMCalendarPopoverGroup popoverGroup = new UPMCalendarPopoverGroup(item.ResultRow.Identifier)
            {
                Invalid = true,
                Context = item
            };
            resultRow.AddDetailGroup(popoverGroup);
        }

        /// <summary>
        /// Eventses from local calendar from to search text.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns></returns>
        public List<ICalendarItem> EventsFromLocalCalendarFromToSearchText(DateTime from, DateTime to, string searchText)
        {
            List<ICalendarItem> calendarItems = new List<ICalendarItem>();
            if (this.CalendarPage.IncludeSystemCalendar && this.CalendarPage.ShowSystemCalendar)
            {
                // ArrayList localCalendarItems = ResultRowCalendarItem.EventsFromLocalCalendarFromToSearchTextCalenderIdentifiers(from, to, this.CalendarPage.SearchText, iPadCalendarFilter.RawValues);
                //    if (localCalendarItems.Count)
                //    {
                //        calendarItems.AddRange(localCalendarItems);
                //    }
            }

            return calendarItems;
        }

        /// <summary>
        /// Adds the field to drop down group.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="group">The group.</param>
        /// <param name="configField">The configuration field.</param>
        public override void AddFieldToDropDownGroup(UPMField field, UPMGroup group, UPConfigFieldControlField configField)
        {
            if (this.viewType == UPCalendarViewType.Day)
            {
                var list = new List<string> { "Date", "Time", "EndDate", "EndTime", "PersonLabel", "CompanyLabel", "Status", "RepLabel", "RepId", "Type" };

                if (!field.Hidden && (string.IsNullOrEmpty(configField.Function) || !list.Contains(configField.Function)))
                {
                    group.AddField(field);
                }
            }
            else
            {
                base.AddFieldToDropDownGroup(field, group, configField);
            }
        }

        /// <summary>
        /// Adds the dropdown groups for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="rowContext">The row context.</param>
        /// <param name="expand">The expand.</param>
        public override void AddDropdownGroupsForResultRow(UPMResultRow resultRow, UPCoreMappingResultRowContext rowContext, UPConfigExpand expand)
        {
            if (this.viewType == UPCalendarViewType.Day)
            {
                UPCoreMappingResultContext resultContext = rowContext.Context;
                if (resultContext.DropdownFields.Count > 0)
                {
                    int detailFieldCount = resultContext.DropdownFields.Count;
                    UPCRMResultRow row = rowContext.Row;
                    string recordId = row.RecordIdentificationAtIndex(0);
                    if (detailFieldCount > 0)
                    {
                        var detailGroupCol = new UPMGroup(FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(this.InfoAreaId, recordId, "DetailGroupDayView"));
                        detailGroupCol.Invalid = true;
                        resultRow.AddDetailGroup(detailGroupCol);
                    }

                    this.AddRowActions(expand, resultRow, recordId);
                }
            }
            else
            {
                base.AddDropdownGroupsForResultRow(resultRow, rowContext, expand);
            }
        }

        private string GetPopoverTitle()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string sourceCopyFieldGroupName = this.ViewReference.ContextValueForKey("CalendarPopOverConfig");
            FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", sourceCopyFieldGroupName);

            if (sourceFieldControl == null)
            {
                SearchAndList searchAndList = configStore.SearchAndListByName(this.ConfigName);
                if (searchAndList != null)
                {
                    sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", searchAndList.FieldGroupName) ??
                                         configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
                }
            }

            if (sourceFieldControl == null)
            {
                sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", this.ConfigName) ??
                                     configStore.FieldControlByNameFromGroup("List", this.ConfigName);
            }

            FieldControlTab tabConfig = sourceFieldControl.TabAtIndex(0);
            return tabConfig.Label;
        }

        /// <summary>
        /// Sets the type of the current calendar view.
        /// </summary>
        /// <param name="_viewType">Type of the view.</param>
        public void SetCurrentCalendarViewType(UPCalendarViewType _viewType)
        {
            this.Page.Invalid = true;
            this.viewType = _viewType;
        }

        /// <summary>
        /// Updates the element for calendar group.
        /// </summary>
        /// <param name="origDetailGroup">The original detail group.</param>
        /// <returns></returns>
        public override UPMCalendarPopoverGroup UpdateElementForCalendarGroup(UPMCalendarPopoverGroup origDetailGroup)
        {
            if (origDetailGroup.Invalid)
            {
                ViewReference viewReference = null;
                string configName = string.Empty;

                UPCalendarSearch calendarSearch = origDetailGroup.Context.CalendarSearch;
                if (calendarSearch != null)
                {
                    viewReference = calendarSearch.PreparedSearch.CalendarPageInfoArea.ViewReference;
                    configName = viewReference.ContextValueForKey("ConfigName");
                }

                if (viewReference == null)
                {
                    viewReference = this.ViewReference;
                    configName = this.ConfigName;
                }

                string sourceCopyFieldGroupName = viewReference.ContextValueForKey("CalendarPopOverConfig");
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", sourceCopyFieldGroupName);
                if (sourceFieldControl == null)
                {
                    SearchAndList searchAndList = configStore.SearchAndListByName(configName);
                    if (searchAndList != null)
                    {
                        sourceFieldControl = configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
                        if (sourceFieldControl != null)
                        {
                            sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", searchAndList.FieldGroupName);
                        }
                    }
                }

                if (sourceFieldControl != null)
                {
                    sourceFieldControl = configStore.FieldControlByNameFromGroup("List", configName);
                }

                if (sourceFieldControl != null)
                {
                    sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", configName);
                }

                this.Loader = new UPCalendarPopoverLoader(this);
                this.Loader.LoadElementForCalendarGroupFieldControl(origDetailGroup, sourceFieldControl);
            }

            return origDetailGroup;
        }

        /// <summary>
        /// Search operation did fail with error
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="error">Error</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.pendingOperation = false;
            base.SearchOperationDidFailWithError(operation, error);
        }

        /// <summary>
        /// Search operation did finish with result
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="result">Result</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (this.PageErrorDisplayed)
            {
                this.ReportError(null, true);
            }

            this.CurrentSearchOperation = null;
            if (result == null)
            {
                this.calendarSearches = null;
                this.pendingOperation = false;
                this.FinishedLoading();
            }

            this.currentSearch.Result = result;
            ++this.currentPreparedSearchIndex;

            this.NextOperation();
        }

        /// <summary>
        /// Gets called when result row provider created row from data row
        /// </summary>
        /// <param name="resultRowProvider">Result row provider</param>
        /// <param name="resultRow">Result row</param>
        /// <param name="dataRow">Data row</param>
        public override void ResultRowProviderDidCreateRowFromDataRow(UPResultRowProvider resultRowProvider, UPMResultRow resultRow, UPCRMResultRow dataRow)
        {
            UPCoreMappingResultContext resultContext = this.ResultContextForRow(resultRow);
            resultContext.RowDictionary[resultRow.Key] = new UPCoreMappingResultRowContext(dataRow, resultContext);
        }

        /// <summary>
        /// Returns Available View Types
        /// </summary>
        /// <returns>
        /// List of <see cref="SearchPageViewType"/>
        /// </returns>
        private List<SearchPageViewType> GetAvailableViewTypes()
        {
            return new List<SearchPageViewType>
            {
                SearchPageViewType.CalendarDay,
                SearchPageViewType.CalendarWeek,
                SearchPageViewType.CalendarMonth,
                SearchPageViewType.CalendarList,
                SearchPageViewType.CalendarTime,
                SearchPageViewType.CalendarYear
            };
        }

        /// <summary>
        /// Sets Search Page's default view type
        /// </summary>
        /// <param name="defaultViewType">
        /// View Type
        /// </param>
        private void SetSearchPageDefaultViewType(string defaultViewType)
        {
            switch (defaultViewType.ToUpper())
            {
                case DefaultViewTypeDay:
                    SearchPage.DefaultViewType = SearchPageViewType.CalendarDay;
                    break;

                case DefaultViewTypeWeek:
                    SearchPage.DefaultViewType = SearchPageViewType.CalendarWeek;
                    break;

                case DefaultViewTypeMonth:
                    SearchPage.DefaultViewType = SearchPageViewType.CalendarMonth;
                    break;

                case DefaultViewTypeList:
                    SearchPage.DefaultViewType = SearchPageViewType.CalendarList;
                    break;

                case DefaultViewTypeTime:
                    SearchPage.DefaultViewType = SearchPageViewType.CalendarTime;
                    break;

                case DefaultViewTypeYear:
                    SearchPage.DefaultViewType = SearchPageViewType.CalendarYear;
                    break;
            }
        }

        /// <summary>
        /// Sets Appointment Actions and Additional Info Area Config
        /// </summary>
        private void SetAppointmentActionsAndAdditionalInfoAreaConfig()
        {
            var actions = new List<UPConfigButton>();

            var newAppointmentAction = ViewReference.ContextValueForKey(KeyNewAppointmentAction);
            if (!string.IsNullOrWhiteSpace(newAppointmentAction))
            {
                actions.Add(ConfigurationUnitStore.DefaultStore.ButtonByName(newAppointmentAction));
            }

            var additionalConfigs = ViewReference.ContextValueForKey(KeyAdditionalCalendarConfigs);
            if (RootInfoAreaConfig != null && !string.IsNullOrWhiteSpace(additionalConfigs))
            {
                var addtionalConfigNames = additionalConfigs.Split(',');
                var configStore = ConfigurationUnitStore.DefaultStore;
                var linkRecordIdentification = ViewReference.ContextValueForKey(KeyLinkRecord);
                var additionalInfoAreaConfigs = (List<UPCalendarPageInfoArea>)null;

                foreach (var menuName in addtionalConfigNames)
                {
                    var menu = configStore.MenuByName(menuName);
                    var viewReference = menu.ViewReference.ViewReferenceWith(linkRecordIdentification);
                    var additionalAppointmentAction = viewReference.ContextValueForKey(KeyNewAppointmentAction);
                    if (!string.IsNullOrWhiteSpace(additionalAppointmentAction))
                    {
                        actions.Add(configStore.ButtonByName(additionalAppointmentAction));
                    }

                    var pageInfoArea = new UPCalendarPageInfoArea(viewReference, RootInfoAreaConfig);
                    if (additionalInfoAreaConfigs == null)
                    {
                        additionalInfoAreaConfigs = new List<UPCalendarPageInfoArea>();
                    }

                    additionalInfoAreaConfigs.Add(pageInfoArea);
                }

                AdditionalInfoAreaConfigs = additionalInfoAreaConfigs;
            }

            CalendarPage.AddAppointmentActions = actions;
        }

        /// <summary>
        /// Sets Additional Prepared Searches
        /// </summary>
        private void SetAdditionalPreparedSearches()
        {
            if (AdditionalInfoAreaConfigs != null)
            {
                var additionalPreparedSearches = (List<UPSearchPageModelControllerPreparedSearch>)null;

                foreach (var pageInfoArea in AdditionalInfoAreaConfigs)
                {
                    var preparedSearch = new UPSearchPageModelControllerPreparedSearch(pageInfoArea);
                    if (additionalPreparedSearches == null)
                    {
                        additionalPreparedSearches = new List<UPSearchPageModelControllerPreparedSearch>();
                    }

                    additionalPreparedSearches.Add(preparedSearch);
                }

                AdditionalPreparedSearches = additionalPreparedSearches;
            }
        }

        /// <summary>
        /// Sets calendar page properties like IncludeSystemCalendar, ShowSystemCalendar etc
        /// </summary>
        private void SetCalendarPageProperties()
        {
            var calendarIncludeSystemCalendar = ConfigurationUnitStore.DefaultStore
                .ConfigValueIsSetDefaultValue(KeyCalendarIncludeSystemCalendar, true);

            var includeSystemCalendar = ViewReference.ContextValueForKey(KeyIncludeSystemCalendar)
                .ToBoolWithDefaultValue(true);

            CalendarPage.IncludeSystemCalendar = includeSystemCalendar && calendarIncludeSystemCalendar;
            CalendarPage.ShowSystemCalendar = false;
            CalendarPage.PopoverTitle = GetPopoverTitle();

            jumpBackToEditedValue = ConfigurationUnitStore.DefaultStore
                .ConfigValueIsSetDefaultValue(KeyCalendarShowLastEditedDate, true);

            var repFilterName = ViewReference.ContextValueForKey(KeyRepFilter);

            if (!string.IsNullOrWhiteSpace(repFilterName))
            {
                RepFilter = UPMFilter.FilterForName(repFilterName);
            }

            if (RepFilter != null)
            {
                RepFilter.Visible = false;
                var availableFilters = CalendarPage.AvailableFilters;
                if (!availableFilters.Contains(RepFilter))
                {
                    availableFilters.Add(RepFilter);
                }
                else
                {
                    RepFilter = availableFilters[availableFilters.IndexOf(RepFilter)];
                }

                var currentRepActive = ViewReference.ContextValueForKey(KeyRepFilterCurrentRepActive)
                    .ToBoolWithDefaultValue(false);

                if (RepFilter != null && currentRepActive)
                {
                    RepFilter.Active = true;
                    RepFilter.SetDefaultRawValues(new List<object> { UPCRMDataStore.DefaultStore.Reps.CurrentRepId });
                    CalendarPage.ShowSystemCalendar = true;
                }
            }
            else
            {
                CalendarPage.ShowSystemCalendar = true;
            }
        }

        /// <summary>
        /// Sets IPad Calendar Filters (only applicable if #porting macro is on)
        /// </summary>
        private void SetIPadCalendarFilters()
        {
#if PORTING
            dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);
            ResultRowCalendarItem.initializeEventStore(delegate (bool granted, NSError error)
            {
                if (!granted)
                {
                    CalendarPage.IncludeSystemCalendar = false;
                }
                else
                {
                    NSNotificationCenter.DefaultCenter().RemoveObserverNameTheObject(this, EKEventStoreChangedNotification, null);
                    NSNotificationCenter.DefaultCenter().AddObserverSelectorNameTheObject(this, @selector(eventStoreUpdated:), EKEventStoreChangedNotification, null);
                    ResultRowCalendarItem.RefreshEventStore();
                    bool showSystemCalendarFilter = viewReference.ContextValueForKey("ShowSystemCalendarFilter").ToBoolWithDefaultValue(false);
                    NSDictionary calendars = ResultRowCalendarItem.IPadCalendars();
                    if (calendars.Count > 1 && showSystemCalendarFilter == true)
                    {
                        iPadCalendarFilter = new UPMCatalogFilter(UPMStringIdentifier.IdentifierWithStringId("iPadCalender"));
                        iPadCalendarFilter.DisplayName = upText_iPadCalendar;
                        NSMutableDictionary availableCatalogValues = new NSMutableDictionary();
                        foreach (var calendarIdentifier in calendars.AllKeys)
                        {
                            availableCatalogValues.SetObjectForKey(new UPMCatalogFilterValue(calendars.ObjectForKey(calendarIdentifier), ResultRowCalendarItem.IPadCalendarColor(calendarIdentifier)), calendarIdentifier);
                        }
                        iPadCalendarFilter.ExplicitCatalogValues = availableCatalogValues;
                        NSDictionary dic = UPPListService.PropertiesForNameRootpath(UPPListiPadCalender, null);
                        if (dic != null)
                        {
                            ArrayList calenderIdentifiers = new ArrayList();
                            foreach (var key in dic.AllKeys)
                            {
                                if (calendars.AllKeys.ContainsObject(key) == true)
                                {
                                    calenderIdentifiers.Add(key);
                                }

                            }
                            iPadCalendarFilter.SetDefaultRawValues(calenderIdentifiers);
                        }
                        else
                        {
                            iPadCalendarFilter.SetDefaultRawValues(calendars.AllKeys);
                        }

                    }

                }

                dispatch_semaphore_signal(semaphore);
            });
            dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
#endif
        }
    }
}
