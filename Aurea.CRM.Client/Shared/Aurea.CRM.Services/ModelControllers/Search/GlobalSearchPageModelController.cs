// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSearchPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Global Search Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Global Search Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Search.SearchPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class GlobalSearchPageModelController : SearchPageModelController, ISearchOperationHandler
    {
        /// <summary>
        /// The prepared searches
        /// </summary>
        protected List<UPSearchPageModelControllerPreparedSearch> PreparedSearches;

        /// <summary>
        /// Gets a value indicating whether [full text search].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [full text search]; otherwise, <c>false</c>.
        /// </value>
        public bool FullTextSearch { get; private set; }

        /// <summary>
        /// Gets the minimum length of the search text.
        /// </summary>
        /// <value>
        /// The minimum length of the search text.
        /// </value>
        public int MinSearchTextLength { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public GlobalSearchPageModelController(ViewReference viewReference)
        : base(viewReference)
        {
            this.SectionContexts = new Dictionary<string, UPCoreMappingResultContext>();
        }

        /// <summary>
        /// Creates the page instance.
        /// </summary>
        /// <returns></returns>
        public override UPMSearchPage CreatePageInstance()
        {
            this.InfoAreaId = this.ViewReference.ContextValueForKey("InfoArea");
            this.ConfigName = this.ViewReference.ContextValueForKey("ConfigName");
            string searchTypeString = this.ViewReference.ContextValueForKey("InitialSearchType");
            SearchPageSearchType searchType = SearchPageSearchType.OfflineSearch;
            string fullTextSearchString = this.ViewReference.ContextValueForKey("FullTextSearch");
            this.FullTextSearch = !(string.IsNullOrEmpty(fullTextSearchString) || fullTextSearchString == "false");

            this.MinSearchTextLength = Convert.ToInt32(this.ViewReference.ContextValueForKey("MinSearchTextLength"));

            if (this.MinSearchTextLength == 0)
            {
                this.MinSearchTextLength = 1;
            }

            if (!string.IsNullOrEmpty(searchTypeString))
            {
                searchType = (SearchPageSearchType)Convert.ToInt32(searchTypeString);
            }

            if (string.IsNullOrEmpty(this.InfoAreaId) && this.ViewReference.ContextValueForKey("Modus") == "GlobalSearch")
            {
                if (string.IsNullOrEmpty(this.ConfigName))
                {
                    this.ConfigName = "default";
                }
            }

            List<IIdentifier> identifiers = new List<IIdentifier>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            QuickSearch quickSearch = configStore.QuickSearchByName(this.ConfigName);
            int infoAreaCount = quickSearch?.NumberOfInfoAreas ?? 0;
            for (int i = 0; i < infoAreaCount; i++)
            {
                string currentInfoAreaId = quickSearch.InfoAreaIdAtIndex(i);
                identifiers.Add(new RecordIdentifier(currentInfoAreaId, null));
            }

            MultipleIdentifier multipleIdentifier = new MultipleIdentifier(identifiers);
            UPMSearchPage page = new UPMSearchPage(multipleIdentifier);
            page.SearchType = searchType;
            page.AvailableOnlineSearch = !this.ViewReference.ContextValueIsSet("hideOnlineOfflineButton");
            page.Style = UPMTableStyle.UPMGlobalSearchTableStyle;
            if (searchType == SearchPageSearchType.OnlineSearch)
            {
                page.InitiallyOnline = true;
            }

            return page;
        }

        public override void ResetWithReason(ModelControllerResetReason reason)
        {
            base.ResetWithReason(reason);
            this.PreparedSearches = null;
        }
        
        /// <summary>
        /// Builds the page details.
        /// </summary>
        public override void BuildPageDetails()
        {
            base.BuildPageDetails();
            this.SearchPage.Invalid = false;
        }

        /// <summary>
        /// Builds the prepared searches.
        /// </summary>
        /// <param name="forServer">if set to <c>true</c> [for server].</param>
        public virtual void BuildPreparedSearches(bool forServer)
        {
            this.PreparedSearches = new List<UPSearchPageModelControllerPreparedSearch>();
            var configStore = ConfigurationUnitStore.DefaultStore;
            var quickSearch = configStore.QuickSearchByName("default");
            var infoAreaCount = quickSearch?.NumberOfInfoAreas ?? 0;
            for (var i = 0; i < infoAreaCount; i++)
            {
                var currentInfoAreaId = quickSearch.InfoAreaIdAtIndex(i);
                var vPrepareSearch = new UPSearchPageModelControllerPreparedSearch(currentInfoAreaId, quickSearch.EntriesForInfoAreaId(currentInfoAreaId));
                this.PreparedSearches.Add(vPrepareSearch);
            }
        }

        /// <summary>
        /// Results the section for search result.
        /// </summary>
        /// <param name="preparedSearch">The prepared search.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public virtual UPMResultSection ResultSectionForSearchResult(UPSearchPageModelControllerPreparedSearch preparedSearch, UPCRMResult result)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var resultContext = new UPCoreMappingResultContext(result, preparedSearch.CombinedControl, preparedSearch.ListFieldControl.NumberOfFields);
            this.SectionContexts.SetObjectForKey(resultContext, preparedSearch.InfoAreaId);
            var resultSection = new UPMResultSection(StringIdentifier.IdentifierWithStringId($"Search_{preparedSearch.InfoAreaId}"));
            var infoAreaConfig = configStore.InfoAreaConfigById(preparedSearch.InfoAreaId);
            if (infoAreaConfig != null)
            {
                var colorKey = infoAreaConfig.ColorKey;
                if (!string.IsNullOrEmpty(colorKey))
                {
                    resultSection.BarColor = AureaColor.ColorWithString(colorKey);
                }

                var imageName = infoAreaConfig.ImageName;
                if (!string.IsNullOrEmpty(imageName))
                {
                    var fileResource = configStore.ResourceByName(imageName);
                    if (fileResource != null)
                    {
                        resultSection.GlobalSearchIconName = fileResource.FileName;
                    }
                }
            }

            resultSection.SectionField = new UPMField(StringIdentifier.IdentifierWithStringId("SectionLabel"));
            var sectionName = infoAreaConfig?.PluralName;
            if (string.IsNullOrEmpty(sectionName))
            {
                var tableInfo = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(preparedSearch.InfoAreaId);
                sectionName = tableInfo != null ? tableInfo.Label : preparedSearch.InfoAreaId;
            }

            resultSection.SectionField.FieldValue = sectionName;
            var count = result.RowCount;
            for (var j = 0; j < count; j++)
            {
                var dataRow = result.ResultRowAtIndex(j) as UPCRMResultRow;
                var identifier = new RecordIdentifier(preparedSearch.InfoAreaId, dataRow.RecordIdAtIndex(0));
                var resultRow = new UPMResultRow(identifier);
                resultContext.RowDictionary.Add(resultRow.Key, new UPCoreMappingResultRowContext(dataRow, resultContext));
                resultContext.ExpandMapper = preparedSearch.ExpandSettings;
                resultRow.Invalid = true;
                resultRow.DataValid = true;
                resultSection.AddResultRow(resultRow);
            }

            return resultSection;
        }

        /// <summary>
        /// Creates the container meta information with value prepared search.
        /// </summary>
        /// <param name="searchValue">The search value.</param>
        /// <param name="preparedSearch">The prepared search.</param>
        /// <returns></returns>
        public virtual UPContainerMetaInfo CreateContainerMetaInfoWithValuePreparedSearch(string searchValue, UPSearchPageModelControllerPreparedSearch preparedSearch)
        {
            UPContainerMetaInfo container = preparedSearch.CrmQueryForValue(searchValue, null, this.FullTextSearch);
            return container;
        }

        /// <summary>
        /// Starts the search operation with search value.
        /// </summary>
        /// <param name="searchOperation">The search operation.</param>
        /// <param name="searchValue">The search value.</param>
        public void StartSearchOperationWithSearchValue(ISearchOperation searchOperation, string searchValue)
        {
            if ((searchValue?.Length ?? 0) < this.MinSearchTextLength && this.ViewReference.ContextValueForKey("Modus") == "GlobalSearch")
            {
                this.CreateNewSearchPageWithEmptyResult();
                return;
            }

            if (this.PreparedSearches == null)
            {
                this.BuildPreparedSearches(false);
            }

            var operation = searchOperation as Operation;
            var searchCount = this.PreparedSearches?.Count ?? 0;
            for (var i = 0; i < searchCount; i++)
            {
                var preparedSearch = this.PreparedSearches[i];
                var container = this.CreateContainerMetaInfoWithValuePreparedSearch(searchValue, preparedSearch);
                searchOperation.AddContainerMetaInfo(container);
            }

            ServerSession.CurrentSession.ExecuteRequest(operation);
            this.CurrentSearchOperation = operation;
        }

        /// <summary>
        /// Results the context for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPCoreMappingResultContext ResultContextForRow(UPMResultRow row)
        {
            var identifier = (RecordIdentifier)row.Identifier;
            return this.SectionContexts[identifier.InfoAreaId];
        }

        /// <summary>
        /// Searches the specified search page.
        /// </summary>
        /// <param name="_searchPage">The search page.</param>
        public override void Search(object _searchPage)
        {
            var searchPage = (UPMSearchPage)_searchPage;
            searchPage.Invalid = false;
            this.SectionContexts = new Dictionary<string, UPCoreMappingResultContext>();
            string value = null;
            if (!string.IsNullOrEmpty(searchPage.SearchText))
            {
                value = searchPage.SearchText;
            }

            ISearchOperation operation = searchPage.SearchType == SearchPageSearchType.OfflineSearch
                ? (ISearchOperation)new LocalGlobalSearchOperation(this)
                : new RemoteSearchOperation(this);

            this.StartSearchOperationWithSearchValue(operation, value);
        }

        /// <summary>
        /// Creates the new search page with empty result.
        /// </summary>
        public void CreateNewSearchPageWithEmptyResult()
        {
            this.CreateNewSearchPageWithResult(new List<UPCRMResult>());
        }

        /// <summary>
        /// Creates the new search page with result.
        /// </summary>
        /// <param name="results">The results.</param>
        public virtual void CreateNewSearchPageWithResult(List<UPCRMResult> results)
        {
            UPMSearchPage newPage = new UPMSearchPage(this.Page.Identifier);
            newPage.CopyDataFrom(this.SearchPage);
            newPage.Invalid = false;
            int searchCount = this.PreparedSearches?.Count ?? 0;

            for (int i = 0; i < searchCount && i < results.Count; i++)
            {
                UPSearchPageModelControllerPreparedSearch preparedSearch = this.PreparedSearches[i];
                if (results[i] != null)
                {
                    UPCRMResult result = results[i];
                    if (result.RowCount > 0)
                    {
                        UPMResultSection section = this.ResultSectionForSearchResult(preparedSearch, result);
                        if (section != null)
                        {
                            newPage.AddResultSection(section);
                        }
                    }
                }
            }

            // Hardcoded Detail Action
            UPMAction switchToDetailAction = new UPMAction(null);
            switchToDetailAction.IconName = "arrow.png";
            switchToDetailAction.SetTargetAction(this, this.SwitchToDetail);
            newPage.RowAction = switchToDetailAction;

            UPMAction searchAction = new UPMAction(null);
            searchAction.SetTargetAction(this, this.Search);
            newPage.SearchAction = searchAction;

            UPMSearchPage oldSearchPage = this.SearchPage;
            this.TopLevelElement = newPage;
            SearchPageResultState state = SearchPageResultState.OfflineNoLokalData;

            foreach (UPCRMResult result in results)
            {
                if (result != null && result.RowCount > 0)
                {
                    state = SearchPageResultState.Ok;
                }
            }

            newPage.ResultState = state;
            this.InformAboutDidChangeTopLevelElement(oldSearchPage, newPage, null, null);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.CurrentSearchOperation = null;
            this.CreateNewSearchPageWithResult(null);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.CurrentSearchOperation = null;
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            this.CurrentSearchOperation = null;
            this.CreateNewSearchPageWithResult(results);
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public override void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public override void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
