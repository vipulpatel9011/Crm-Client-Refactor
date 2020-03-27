// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardSearchPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Standard Search Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Standard Search Page Model Controller class implementation
    /// </summary>
    public class UPStandardSearchPageModelController : SearchPageModelController, IResultRowProviderDelegate, UPCRMLinkReaderDelegate
    {
        private const string FilterNameKey = "FilterName";
        private const string SectionsKey = "Sections";
        private const string FullTextSearchKey = "FullTextSearch";
        private const string FalseCaseKey = "FullTextSearch";
        private const string SearchKey = "Search";
        private const string TabTypeMULTIKey = "MULTI";
        private const string EnabledFilterKey = "EnabledFilter";
        private const string AdditionalFilterKey = "AdditionalFilter";
        private const string HideOnlineOfflineButtonKey = "hideOnlineOfflineButton";
        private const int MaxAvalaibleFilterCount = 5;

        protected UPSearchPageModelControllerPreparedSearch PreparedSearch;
        protected UPCoreMappingResultContext ResultContext;
        protected UPCRMLinkReader LinkReader;
        protected bool PageErrorDisplayed;
        protected bool InitialSearch;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPStandardSearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public UPStandardSearchPageModelController(ViewReference viewReference)
        : base(viewReference)
        {
        }

        /// <summary>
        /// Occurs when [search finished event].
        /// </summary>
        public event EventHandler SearchFinishedEvent;

        /// <summary>
        /// Gets parent link string
        /// </summary>
        public string ParentLinkString { get; private set; }

        /// <summary>
        /// Gets link record identification
        /// </summary>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets link id
        /// </summary>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether full text search is enabled
        /// </summary>
        public bool FullTextSearch { get; private set; }

        /// <summary>
        /// Gets sections
        /// </summary>
        public string Sections { get; private set; }

        /// <summary>
        /// Gets filter index mapping
        /// </summary>
        public Dictionary<string, int> FilterIndexMapping { get; private set; }

        /// <summary>
        /// Gets main prepared search
        /// </summary>
        public virtual UPSearchPageModelControllerPreparedSearch MainPreparedSearch => new UPSearchPageModelControllerPreparedSearch(this);

        /// <summary>
        /// Gets result contexts
        /// </summary>
        public override List<UPCoreMappingResultContext> ResultContexts => this.ResultContext != null
            ? new List<UPCoreMappingResultContext> { this.ResultContext } : null;

        /// <summary>
        /// Gets details action
        /// </summary>
        public override ViewReference DetailsAction
        {
            get
            {
                if (this.PreparedSearch.FilterBasedDecision != null && this.CurrentDetailRow != null)
                {
                    UPCoreMappingResultRowContext rowContext = this.ResultContext.RowDictionary[this.CurrentDetailRow.Key];

                    if (rowContext.Row != null)
                    {
                        var viewReference = this.PreparedSearch.FilterBasedDecision.ViewReferenceForResultRow(rowContext.Row);
                        if (viewReference != null)
                        {
                            return viewReference;
                        }
                    }
                }

                return this.PreparedSearch.DetailAction != null ? this.PreparedSearch.DetailAction.ViewReference : base.DetailsAction;
            }
        }

        /// <summary>
        /// Reset method, resets model controller with given reason
        /// </summary>
        /// <param name="reason">Reset reason</param>
        public override void ResetWithReason(ModelControllerResetReason reason)
        {
            base.ResetWithReason(reason);

            // Only if not visible
            if (this.ModelControllerDelegate != null)
            {
                this.ResultContext = null;
            }
        }

        /// <summary>
        /// Starts test search
        /// </summary>
        /// <param name="searchPage">Search page</param>
        public void StartTestSearch(UPMSearchPage searchPage)
        {
            this.Search(searchPage);
        }

        /// <summary>
        /// Performs search
        /// </summary>
        /// <param name="sender">Sender object</param>
        public override void Search(object sender)
        {
            var searchPage = (UPMSearchPage)sender;
            if (this.LinkReader != null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.ParentLinkString) && !string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                this.LinkReader = new UPCRMLinkReader(this.LinkRecordIdentification, this.ParentLinkString, UPRequestOption.FastestAvailable, this);
                if (this.LinkReader.Start())
                {
                    return;
                }

                this.LinkReader = null;
                this.ParentLinkString = null;
            }

            this.CurrentSearchOperation?.Cancel();

            string value = null;
            if (!string.IsNullOrEmpty(searchPage.SearchText))
            {
                value = searchPage.SearchText;
            }

            var configFilters = this.ActiveFiltersForSearchPage(searchPage);

            if (string.IsNullOrEmpty(value) && configFilters.Count == 0)
            {
                var contextValue = this.ViewReference.ContextValueForKey(@"SearchOptions");
                var range = contextValue?.IndexOf(@"NoEmptySearch");

                if (range > 0)
                {
                    this.SearchOperationDidFinishWithResult(null, null);
                    return;
                }
            }

            this.StartStandardSearchWithValue(value, configFilters);
        }

        /// <summary>
        /// Adds row actions
        /// </summary>
        /// <param name="expand">Expand</param>
        /// <param name="resultRow">Result row</param>
        /// <param name="recordId">Record id</param>
        public override void AddRowActions(UPConfigExpand expand, UPMResultRow resultRow, string recordId)
        {
            base.AddRowActions(expand, resultRow, recordId);

            if (this.PreparedSearch.FilterBasedDecision != null)
            {
                var rowContext = this.ResultContext.RowDictionary[resultRow.Key];
                var actionButtons = this.PreparedSearch.FilterBasedDecision.ButtonsForResultRow(rowContext.Row);

                if (actionButtons.Count > 0)
                {
                    foreach (UPConfigButton buttonDef in actionButtons)
                    {
                        string iconName = string.Empty;
                        string buttonName = buttonDef.UnitName;

                        if (buttonDef.ViewReference != null)
                        {
                            var action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId($"action.{buttonName}"));

                            var viewReference = buttonDef.ViewReference.ViewReferenceWith(recordId);
                            viewReference = viewReference.ViewReferenceWith(new Dictionary<string, object> { { ".fromPopup", 1 } });
                            action.ViewReference = viewReference;

                            if (!string.IsNullOrEmpty(iconName))
                            {
                                action.IconName = iconName;
                            }

                            action.LabelText = buttonDef.Label;
                            resultRow.AddDetailAction(action);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates page instance
        /// </summary>
        /// <returns><see cref="UPMSearchPage"/></returns>
        public override UPMSearchPage CreatePageInstance()
        {
            this.LinkRecordIdentification = this.ViewReference.ContextValueForKey(@"RecordId");
            if (string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                this.LinkRecordIdentification = this.ViewReference.ContextValueForKey(@"LinkRecord");
            }

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification) && this.LinkRecordIdentification.IsRecordIdentification())
            {
                this.LinkRecordIdentification = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(this.LinkRecordIdentification);
                string linkIdObject = this.ViewReference.ContextValueForKey(@"LinkId");
                this.LinkId = !string.IsNullOrEmpty(linkIdObject) ? Convert.ToInt32(linkIdObject) : 0;
            }
            else
            {
                this.LinkRecordIdentification = null;
                this.LinkId = -1;
            }

            this.ParentLinkString = this.ViewReference.ContextValueForKey(@"ParentLink");

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                var infoAreaIdentifier = new RecordIdentifier(this.InfoAreaId, null);
                var multiIdentifier = new MultipleIdentifier(new List<IIdentifier> { infoAreaIdentifier, new RecordIdentifier(this.LinkRecordIdentification) });
                return this.CreatePageInstance(multiIdentifier);
            }

            return this.CreatePageInstance(new RecordIdentifier(this.InfoAreaId, null));
        }

        /// <summary>
        /// Builds page details
        /// </summary>
        public override void BuildPageDetails()
        {
            base.BuildPageDetails();
            if (string.IsNullOrWhiteSpace(ConfigName))
            {
                ConfigName = InfoAreaId;
            }

            InitialSearch = true;
            Sections = ViewReference.ContextValueForKey(SectionsKey);

            filterObject = null; // TODO: CRM-59214 - 2018-08-31 - (UPConfigFilter)ViewReference.ContextValueForKey(@"FilterObject");
            if (filterObject == null)
            {
                filterName = ViewReference.ContextValueForKey(FilterNameKey);
            }

            var hasOfflineData = UPCRMDataStore.DefaultStore.HasOfflineData(InfoAreaId);
            if (!hasOfflineData)
            {
                SearchPage.InitiallyOnline = true;
                SearchPage.SearchType = SearchPageSearchType.OnlineSearch;
            }

            var fullTextSearchString = ViewReference.ContextValueForKey(FullTextSearchKey);
            FullTextSearch = string.IsNullOrWhiteSpace(fullTextSearchString) && fullTextSearchString != FalseCaseKey;

            if (PreparedSearch == null)
            {
                PreparedSearch = MainPreparedSearch;
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            var searchHeader = configStore.HeaderByNameFromGroup(SearchKey, PreparedSearch.SearchConfiguration?.HeaderGroupName);

            SearchPage.LabelText = searchHeader != null
                ? searchHeader.Label
                : LocalizedString.Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsConfigHeaderMissing);

            var searchLabel = GetSearchLabel(configStore);
            if (!string.IsNullOrWhiteSpace(searchLabel))
            {
                SearchPage.SearchPlaceholder = searchLabel;
            }
            else
            {
                SearchPage.HideTextSearch = true;
            }

            var enabledFilters = GetEnabledFields();
            var availableFilters = new List<string>();
            var filterMapping = new Dictionary<string, int>();
            PopulateAvailableFiltersAndMapping(availableFilters, filterMapping);

            FilterIndexMapping = filterMapping;
            AddFilters(availableFilters, enabledFilters);

            SearchPage.AvailableOnlineSearch = !ViewReference.ContextValueIsSet(HideOnlineOfflineButtonKey);
        }

        /// <summary>
        /// Search operation did fail with error
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="error">Error</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.CurrentSearchOperation?.Cancel();
            this.CurrentSearchOperation = null;
            this.PageErrorDisplayed = true;

            if (this.TestDelegate != null)
            {
                this.TestDelegate.ModelControllerReturnedError(this, error);
                return;
            }

            this.ReportError(error, true);
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
            else if (operation != this.CurrentSearchOperation)
            {
                return;
            }

            this.CurrentSearchOperation = null;
            var oldSearchPage = this.SearchPage;
            var newPage = this.CreatePageInstance(this.Page.Identifier);
            this.TopLevelElement = newPage;
            newPage.CopyDataFrom(oldSearchPage);

            newPage.Invalid = false;
            this.BuildStandardSearchPage(newPage, result);

            // Hardcoded Detail Action
            UPMAction switchToDetailAction = new UPMAction(null);
            switchToDetailAction.IconName = @"arrow.png";
            switchToDetailAction.SetTargetAction(this, this.SwitchToDetail);
            newPage.RowAction = switchToDetailAction;

            UPMAction searchAction = new UPMAction(null);
            searchAction.SetTargetAction(this, this.Search);
            newPage.SearchAction = searchAction;
            newPage.ResultState = this.CountResultState(result);
            this.TopLevelElement = newPage;

            if (this.TestDelegate != null)
            {
                this.TestDelegate.ModelControllerReturnedPage(this, newPage);
                return;
            }

            if (this.ModelControllerDelegate == null)
            {
                this.SearchFinished();
            }
            else
            {
                this.ModelControllerDelegate.ModelControllerDidChange(this, oldSearchPage, newPage, null, null);
            }
        }

        /// <summary>
        /// Search operation did finish with count
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="count">Count</param>
        public override void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
        }

        /// <summary>
        /// Gets called when result row provider created row from data row
        /// </summary>
        /// <param name="resultRowProvider">Result row provider</param>
        /// <param name="resultRow">Result row</param>
        /// <param name="dataRow">Data row</param>
        public virtual void ResultRowProviderDidCreateRowFromDataRow(UPResultRowProvider resultRowProvider, UPMResultRow resultRow, UPCRMResultRow dataRow)
        {
            this.ResultContext.RowDictionary[resultRow.Key] = new UPCoreMappingResultRowContext(dataRow, this.ResultContext);
        }

        /// <summary>
        /// Link reader did finish with result
        /// </summary>
        /// <param name="linkReader">Link reader</param>
        /// <param name="result">Result</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            this.LinkRecordIdentification = linkReader.DestinationRecordIdentification;
            this.ParentLinkString = string.Empty;
            this.LinkReader = null;
            this.Search((UPMSearchPage)this.Page);
        }

        /// <summary>
        /// Link reader did finish with error
        /// </summary>
        /// <param name="linkReader">Link reader</param>
        /// <param name="error">Error</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.CurrentSearchOperation.Cancel();
            this.CurrentSearchOperation = null;
            this.ReportError(error, false);
        }

        /// <summary>
        /// Search operation did finish with results
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="results">Results</param>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
        }

        /// <summary>
        /// Search operation did finish with counts
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="counts">Results</param>
        public override void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
        }

        /// <summary>
        /// Responsible for preparing query by given <see cref="UPContainerMetaInfo"/>
        /// </summary>
        /// <param name="crmQuery">Metainfo data</param>
        protected virtual void PrepareQuery(UPContainerMetaInfo crmQuery)
        {
            // do nothing; to be overridden
        }

        /// <summary>
        /// Returns filter name for index
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns><see cref="string"/></returns>
        protected virtual string AlternateFilterNameForIndex(int i)
        {
            return null;
        }

        /// <summary>
        /// Retrieves active filters for given search page
        /// </summary>
        /// <param name="searchPage">Search page</param>
        /// <returns><see cref="List{UPConfigFilter}"/></returns>
        protected virtual List<UPConfigFilter> ActiveFiltersForSearchPage(UPMSearchPage searchPage)
        {
            return UPMFilter.ActiveFiltersForFilters(searchPage.AvailableFilters);
        }

        /// <summary>
        /// Builds standard search page with search page and result
        /// </summary>
        /// <param name="searchPage">Search page</param>
        /// <param name="result">Result</param>
        private void BuildStandardSearchPage(UPMSearchPage searchPage, UPCRMResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var count = result.RowCount;

            this.ResultContext = new UPCoreMappingResultContext(
                result,
                this.PreparedSearch.CombinedControl,
                this.PreparedSearch.ListFieldControl.NumberOfFields)
            {
                ExpandMapper = this.PreparedSearch.ExpandChecker
            };
            this.SectionContexts[this.InfoAreaId] = this.ResultContext;

            var hasSections = !string.IsNullOrWhiteSpace(this.Sections);
            if (!hasSections && this.ResultContext.SectionField != null)
            {
                hasSections = true;
            }

            if (this.Sections == @"false")
            {
                hasSections = false;
            }

            var optimizeForSpeed = false;
            if (result.RowCount < 200)
            {
                // optimizeForSpeed can handle multipleRows with the same id. (PVCS 80132)
                optimizeForSpeed = this.SearchPageMode == SearchPageMode.ForceOptimizeForSpeed;
            }

            this.BuildStandardSearchPage(searchPage, result, count, hasSections, optimizeForSpeed);
        }

        private void BuildStandardSearchPage(
            UPMSearchPage searchPage,
            UPCRMResult result,
            int count,
            bool hasSections,
            bool optimizeForSpeed)
        {
            if (searchPage == null)
            {
                throw new ArgumentNullException(nameof(searchPage));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionDictionary = new Dictionary<string, UPMResultSection>();
            var resultSection = this.GetResultSection(searchPage, result, hasSections, optimizeForSpeed, out bool doContinue);
            if (!doContinue)
            {
                return;
            }

            // Section with # which groups the uncommon characters
            UPMResultSection lastSection = null;

            for (var i = 0; i < count; i++)
            {
                var dataRow = (UPCRMResultRow)result.ResultRowAtIndex(i);

                if (hasSections)
                {
                    var currentSectionKey = this.SectionKeyRawValueForRow(dataRow, this.ResultContext);
                    var isLastSection = currentSectionKey == @"?";
                    if (isLastSection)
                    {
                        lastSection = resultSection;
                    }
                    else
                    {
                        resultSection = sectionDictionary.ValueOrDefault(currentSectionKey);
                    }

                    if (resultSection == null)
                    {
                        resultSection = this.CreateResultSection(optimizeForSpeed, currentSectionKey, dataRow);

                        if (isLastSection)
                        {
                            lastSection = resultSection;
                        }
                        else
                        {
                            this.SearchPage.AddResultSection(resultSection);
                            sectionDictionary[currentSectionKey] = resultSection;
                        }
                    }
                }

                this.AddResultRow(optimizeForSpeed, resultSection, dataRow);
            }

            if (lastSection != null)
            {
                searchPage.AddResultSection(lastSection);
            }
        }

        private UPMResultSection GetResultSection(
            UPMSearchPage searchPage,
            UPCRMResult result,
            bool hasSections,
            bool optimizeForSpeed,
            out bool doContinue)
        {
            if (searchPage == null)
            {
                throw new ArgumentNullException(nameof(searchPage));
            }

            doContinue = true;

            UPMResultSection resultSection = null;
            if (!hasSections)
            {
                if (optimizeForSpeed)
                {
                    resultSection = new UPMResultSection(
                        StringIdentifier.IdentifierWithStringId(@"Result_Section_1"),
                        new UPResultRowProviderForCRMResult(result, this));
                    this.SearchPage.AddResultSection(resultSection);
                    doContinue = false;
                }

                resultSection = new UPMResultSection(StringIdentifier.IdentifierWithStringId(@"Result_Section_1"));
                this.SearchPage.AddResultSection(resultSection);
            }
            else
            {
                // Date or Catalog Field --> Kein Section Index
                searchPage.HideSectionIndex = this.ResultContext.SectionFieldComplete;
            }

            return resultSection;
        }

        private UPMResultSection CreateResultSection(bool optimizeForSpeed, string currentSectionKey, UPCRMResultRow dataRow)
        {
            UPMResultSection resultSection;
            if (optimizeForSpeed)
            {
                resultSection = new UPMResultSection(
                    StringIdentifier.IdentifierWithStringId($"Result_Section_{currentSectionKey}"),
                    new UPResultRowFromCRMResultRows(this));
            }
            else
            {
                resultSection = new UPMResultSection(
                    StringIdentifier.IdentifierWithStringId($"Result_Section_{currentSectionKey}"));
            }

            resultSection.SectionField = new UPMField(StringIdentifier.IdentifierWithStringId(@"SectionLabel"));
            resultSection.SectionIndexKey = this.SectionKeyForRow(dataRow, this.ResultContext);
            resultSection.SectionField.FieldValue = resultSection.SectionIndexKey;

            return resultSection;
        }

        private void AddResultRow(bool optimizeForSpeed, UPMResultSection resultSection, UPCRMResultRow dataRow)
        {
            if (resultSection == null)
            {
                throw new ArgumentNullException(nameof(resultSection));
            }

            if (dataRow == null)
            {
                throw new ArgumentNullException(nameof(dataRow));
            }

            if (optimizeForSpeed)
            {
                ((UPResultRowFromCRMResultRows)resultSection.ResultRowProvider).AddRow(dataRow);
            }
            else
            {
                var identifier = new RecordIdentifier(this.InfoAreaId, dataRow.RecordIdAtIndex(0));
                var resultRow = new UPMResultRow(identifier);
                this.ResultContext.RowDictionary[resultRow.Key] = new UPCoreMappingResultRowContext(dataRow, this.ResultContext);
                resultRow.Invalid = true;
                resultRow.DataValid = true;
                resultSection?.AddResultRow(resultRow);
            }
        }

        /// <summary>
        /// Adds filters
        /// </summary>
        /// <param name="availableFilters">Available filters</param>
        /// <param name="enabledFilters">Enabled filters</param>
        private void AddFilters(List<string> availableFilters, List<string> enabledFilters)
        {
            var upmFilterArray = new List<UPMFilter>();

            foreach (string filterNameItem in availableFilters)
            {
                if (!string.IsNullOrEmpty(filterNameItem))
                {
                    var filter = UPMFilter.FilterForName(filterNameItem, this.PreparedSearch.FilterParameter);

                    if (filter != null)
                    {
                        if (enabledFilters != null && enabledFilters.Contains(filterNameItem))
                        {
                            if (filter.FilterType == UPMFilterType.NoParam || filter.RawValues?.Count > 0)
                            {
                                filter.DefaultEnabled = true;
                                filter.Active = true;
                            }
                        }

                        upmFilterArray.Add(filter);
                    }
                }
            }

            this.SearchPage.AvailableFilters = upmFilterArray;
        }

        /// <summary>
        /// Section key raw value for row
        /// </summary>
        /// <param name="dataRow">Data row</param>
        /// <param name="context">Context</param>
        /// <returns><see cref="string"/></returns>
        private string SectionKeyRawValueForRow(UPCRMResultRow dataRow, UPCoreMappingResultContext context)
        {
            var sectionIndex = dataRow.RawValueAtIndex(context.SectionField?.TabIndependentFieldIndex ?? 0);
            return this.CheckSectionIndex(sectionIndex, context);
        }

        /// <summary>
        /// Section key for row
        /// </summary>
        /// <param name="dataRow">Data row</param>
        /// <param name="context">Context</param>
        /// <returns><see cref="string"/></returns>
        private string SectionKeyForRow(UPCRMResultRow dataRow, UPCoreMappingResultContext context)
        {
            var sectionIndex = dataRow.ValueAtIndex(context.SectionField?.TabIndependentFieldIndex ?? 0);
            return this.CheckSectionIndex(sectionIndex, context);
        }

        /// <summary>
        /// Check section index
        /// </summary>
        /// <param name="sectionIndex">Section index</param>
        /// <param name="context">Context</param>
        /// <returns><see cref="string"/></returns>
        private string CheckSectionIndex(string sectionIndex, UPCoreMappingResultContext context)
        {
            if (string.IsNullOrEmpty(sectionIndex))
            {
                return @"?";
            }
            else if (!context.SectionFieldComplete)
            {
#if PORTING
                NSRange range = [sectionIndex rangeOfCharacterFromSet:[NSCharacterSet alphanumericCharacterSet]];
                if (range.length > 0 && range.location == 0)
                {
                    string vRet = [[[sectionIndex decomposedStringWithCanonicalMapping] substringToIndex: 1] uppercaseString];
                    if ([vRet characterAtIndex: 0] > 255)
                    {
                        return @"?";
                    }
                    else
                    {
                        return vRet;
                    }
                }
#endif
                return @"?";
            }

            return sectionIndex;
        }

        /// <summary>
        /// Start standard search with value
        /// </summary>
        /// <param name="searchValue">Search value</param>
        /// <param name="filters">Filters</param>
        private void StartStandardSearchWithValue(string searchValue, List<UPConfigFilter> filters)
        {
            if (this.PreparedSearch == null)
            {
                this.PreparedSearch = new UPSearchPageModelControllerPreparedSearch(this);
            }

            if (this.PreparedSearch.CombinedControl == null)
            {
                return; // dont crash but do nothing if false list exists
            }

            var crmQuery = this.PreparedSearch.CrmQueryForValue(searchValue, filters, true);

            if (this.MaxResults > 0)
            {
                crmQuery.MaxResults = this.MaxResults;
            }

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                crmQuery.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
            }

            this.PrepareQuery(crmQuery);

            // Override request Option if no offline Data Available
            if (this.SearchPage.SearchType == SearchPageSearchType.OnlineSearch)
            {
                this.CurrentSearchOperation = crmQuery.Find(UPRequestOption.Online, this);
                if (this.CurrentSearchOperation == null)
                {
                    this.SearchPage.Invalid = false;
                    this.SearchPage.ResultState = SearchPageResultState.OnlyOnline;
                    this.SearchPage.Status = null;

                    if (this.TestDelegate != null)
                    {
                        this.TestDelegate.ModelControllerReturnedError(this, new Exception(@"online not supported"));
                        return;
                    }
                }
            }
            else
            {
                if (this.InitialSearch)
                {
                    // null Result --> online search but offline
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
                        // Offline Error
                        this.SearchPage.Invalid = false;
                        this.SearchPage.ResultState = SearchPageResultState.OnlyOnline;
                        this.SearchPage.Status = null;
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

        private void SearchFinished()
        {
            this.SearchFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns List of Enabled Fields
        /// </summary>
        /// <returns>
        /// List&lt;string&gt;
        /// </returns>
        private List<string> GetEnabledFields()
        {
            var enabledFilters = new List<string>();
            var enabledFilterList = ViewReference.ContextValueForKey(EnabledFilterKey);
            if (enabledFilterList != null)
            {
                var enabledFilterNames = enabledFilterList.Split(',');
                enabledFilters.AddRange(enabledFilterNames.Select(filter => filter.Trim()));
            }

            return enabledFilters;
        }

        /// <summary>
        /// Populates AvailableFilter List and FilterMapping Dictionary
        /// </summary>
        /// <param name="availableFilters">
        /// List&lt;string&gt;
        /// </param>
        /// <param name="filterMapping">
        /// Dictionary&lt;string,int&gt;
        /// </param>
        private void PopulateAvailableFiltersAndMapping(List<string> availableFilters, Dictionary<string, int> filterMapping)
        {
            for (var filterNo = 1; filterNo <= MaxAvalaibleFilterCount; filterNo++)
            {
                var availableFilterName = ViewReference.ContextValueForKey($"Filter{filterNo}");

                if (string.IsNullOrWhiteSpace(availableFilterName))
                {
                    availableFilterName = AlternateFilterNameForIndex(filterNo);
                }

                if (!string.IsNullOrWhiteSpace(availableFilterName))
                {
                    availableFilters.Add(availableFilterName);
                    filterMapping[availableFilterName] = filterNo;
                }
            }

            var additionalFilterAvailableFilterName = ViewReference.ContextValueForKey(AdditionalFilterKey);
            if (!string.IsNullOrWhiteSpace(additionalFilterAvailableFilterName))
            {
                var filterParts = additionalFilterAvailableFilterName.Split(';');
                int filterIndex = 6;
                foreach (string filterPart in filterParts)
                {
                    string availableFilterName = filterPart;
                    if (string.IsNullOrWhiteSpace(availableFilterName))
                    {
                        availableFilterName = AlternateFilterNameForIndex(filterIndex);
                    }

                    if (!string.IsNullOrWhiteSpace(availableFilterName))
                    {
                        availableFilters.Add(availableFilterName);
                        filterMapping[availableFilterName] = filterIndex;
                    }

                    ++filterIndex;
                }
            }
        }

        /// <summary>
        /// Gets search label from configuration store.
        /// </summary>
        /// <param name="configStore">
        /// The configuration store to hold search label.
        /// </param>
        /// <returns>
        /// The search label.
        /// </returns>
        private string GetSearchLabel(IConfigurationUnitStore configStore)
        {
            var searchLabel = string.Empty;
            FieldControl searchControl = configStore.FieldControlByNameFromGroup(SearchKey, PreparedSearch.SearchConfiguration?.FieldGroupName);
            if (searchControl != null && searchControl.Tabs != null)
            {
                foreach (FieldControlTab tab in searchControl.Tabs)
                {
                    if (string.Compare(tab.Type, TabTypeMULTIKey, StringComparison.OrdinalIgnoreCase) == 0 && tab.NumberOfFields > 1)
                    {
                        bool ignoreField = true;
                        foreach (UPConfigFieldControlField field in tab.Fields)
                        {
                            if (ignoreField)
                            {
                                ignoreField = false;
                                continue;
                            }

                            searchLabel = string.IsNullOrWhiteSpace(searchLabel) ? field.Label : $"{searchLabel} | {field.Label}";
                        }
                    }
                    else
                    {
                        if (tab.Fields != null)
                        {
                            foreach (UPConfigFieldControlField field in tab.Fields)
                            {
                                searchLabel = string.IsNullOrWhiteSpace(searchLabel) ? field.Label : $"{searchLabel} | {field.Label}";
                            }
                        }
                    }
                }
            }

            return searchLabel;
        }
    }
}
