// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Document Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Documents
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Document Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class DocumentPageModelController : UPPageModelController
    {
        private string recordIdentification;
        private Operation currentSearchOperation;
        private UPSearchPageModelControllerPreparedSearch preparedSearch;
        private bool fullTextSearch;
        private bool hideOnlineButton;
        private string filterName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public DocumentPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.BuildPage();
        }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            UPMDocumentPage page = this.Page != null
                ? new UPMDocumentPage(this.Page.Identifier)
                : new UPMDocumentPage(new RecordIdentifier(this.InfoAreaId, null));

            UPMAction searchAction = new UPMAction(null);
            searchAction.SetTargetAction(this, this.Search);
            page.SearchAction = searchAction;
            return page;
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            var page = element as Page;
            return page != null ? this.UpdatedElementForPage((Page)element) : null;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.currentSearchOperation = null;
            this.ReportError(error, false);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.currentSearchOperation = null;
            UPMDocumentPage newPage = (UPMDocumentPage)this.InstantiatePage();
            newPage.Invalid = false;
            newPage.SearchType = ((UPMDocumentPage)this.Page).SearchType;
            newPage.SearchPlaceholder = ((UPMDocumentPage)this.Page).SearchPlaceholder;
            newPage.AvailableOnlineSearch = ((UPMDocumentPage)this.Page).AvailableOnlineSearch;
            this.FillPageWithDocumentsResult(newPage, result);
            Page oldPage = this.Page;
            this.TopLevelElement = newPage;
            this.ModelControllerDelegate.ModelControllerDidChange(this, oldPage, newPage, null, null);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
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

        private void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.StringValue = LocalizedString.TextLoadingData;
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        private void AddFilters(List<string> availableFilters)
        {
            List<UPMFilter> upmFilterArray = new List<UPMFilter>();
            foreach (string filterNameItem in availableFilters)
            {
                if (!string.IsNullOrEmpty(filterNameItem))
                {
                    UPMFilter filter = UPMFilter.FilterForName(filterNameItem);
                    if (filter != null)
                    {
                        upmFilterArray.Add(filter);
                    }
                }
            }

            ((UPMDocumentPage)this.Page).AvailableFilters = upmFilterArray;
        }

        private void BuildPage()
        {
            this.recordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.InfoAreaId = this.recordIdentification.InfoAreaId();
            this.ConfigName = this.ViewReference.ContextValueForKey("ConfigName");
            this.filterName = this.ViewReference.ContextValueForKey("FilterName");
            string hideOnlineButtonString = this.ViewReference.ContextValueForKey("hideOnlineOfflineButton");
            this.hideOnlineButton = hideOnlineButtonString == "true";
            string fullTextSearchString = this.ViewReference.ContextValueForKey("FullTextSearch");
            this.fullTextSearch = !(string.IsNullOrEmpty(fullTextSearchString) || fullTextSearchString == "false");
            UPMDocumentPage page = (UPMDocumentPage)this.InstantiatePage();
            page.SearchType = SearchPageSearchType.OfflineSearch;
            page.LabelText = LocalizedString.TextProcessDocuments;
            page.Invalid = true;
            page.AvailableOnlineSearch = !this.hideOnlineButton;
            string headerName = this.ViewReference.ContextValueForKey("HeaderName");
            UPConfigHeader header = null;
            if (!string.IsNullOrEmpty(headerName))
            {
                header = ConfigurationUnitStore.DefaultStore.HeaderByName(headerName);
            }

            if (header != null)
            {
                page.LabelText = header.Label;
            }

            this.TopLevelElement = page;
            List<string> availableFilters = new List<string>();
            for (int i = 1; i <= 5; i++)
            {
                string availableFilterName = this.ViewReference.ContextValueForKey($"Filter{i}");
                if (!string.IsNullOrEmpty(availableFilterName))
                {
                    availableFilters.Add(availableFilterName);
                }
            }

            this.AddFilters(availableFilters);
            this.ApplyLoadingStatusOnPage(page);
        }

        private void Search(object _docPage)
        {
            UPMDocumentPage docPage = (UPMDocumentPage)_docPage;
            this.currentSearchOperation?.Cancel();
            if (this.preparedSearch == null)
            {
                this.preparedSearch = new UPSearchPageModelControllerPreparedSearch(this.InfoAreaId, this.ConfigName, this.filterName);
            }

            if (this.preparedSearch.CombinedControl == null)
            {
                this.SearchOperationDidFinishWithResult(null, null);
                return;     // dont crash but do nothing if no list exists
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            FieldControl searchControl = configStore.FieldControlByNameFromGroup("Search", this.preparedSearch.SearchConfiguration.FieldGroupName);
            if (searchControl != null)
            {
                string searchLabel = string.Empty;
                int count = searchControl.NumberOfFields;
                for (int i = 0; i < count; i++)
                {
                    searchLabel = i == 0 ? searchControl.FieldAtIndex(i).Label : $"{searchLabel} | {searchControl.FieldAtIndex(i).Label}";
                }

                docPage.SearchPlaceholder = searchLabel;
            }

            if (string.IsNullOrEmpty(docPage.SearchText))
            {
                int range = this.ViewReference.ContextValueForKey("SearchOptions")?.IndexOf("NoEmptySearch") ?? 0;
                if (range >= 0)
                {
                    this.SearchOperationDidFinishWithResult(null, null);
                    return;
                }
            }

            List<UPConfigFilter> configFilters = UPMFilter.ActiveFiltersForFilters(docPage.AvailableFilters);
            UPContainerMetaInfo containerMetaInfo = this.preparedSearch.CrmQueryForValue(docPage.SearchText, configFilters, this.fullTextSearch);
            if (this.recordIdentification.IsRecordIdentification())
            {
                containerMetaInfo.SetLinkRecordIdentification(this.recordIdentification, 126, 127);
            }

            if (docPage.SearchType == SearchPageSearchType.OfflineSearch)
            {
                this.currentSearchOperation = containerMetaInfo.Find(UPRequestOption.Offline, this);
            }
            else if (docPage.SearchType == SearchPageSearchType.OnlineSearch)
            {
                this.currentSearchOperation = containerMetaInfo.Find(this);
            }
            else
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError($"Unknown search type: {docPage.SearchType}");
            }
        }

        private Page UpdatedElementForPage(Page page)
        {
            this.Search((UPMDocumentPage)page);
            return (Page)this.TopLevelElement;
        }

        private bool FillPageWithDocumentsResult(UPMDocumentPage page, UPCRMResult result)
        {
            int count = result?.RowCount ?? 0;
            if (count == 0)
            {
                return false;
            }

            UPMDocumentSection lastSection = null;
            int groupingIndex = -1;
            for (int i = 0; i < result.ColumnCount; i++)
            {
                if (result.ColumnFieldMetaInfoAtIndex(i).FunctionName == "groupingKey")
                {
                    groupingIndex = i;
                    break;
                }
            }

            page.AvailableGrouping = groupingIndex != -1;
            DocumentInfoAreaManager documentInfoAreaManager = new DocumentInfoAreaManager(this.preparedSearch.CombinedControl.InfoAreaId, this.preparedSearch.CombinedControl, null);
            Dictionary<string, UPMDocumentSection> groupDictionary = new Dictionary<string, UPMDocumentSection>();
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(i);
                DocumentData documentData = documentInfoAreaManager.DocumentDataForResultRow(resultRow);
                string groupValue = string.Empty;
                if (groupingIndex != -1)
                {
                    groupValue = resultRow.ValueAtIndex(groupingIndex);
                }

                UPMDocumentSection section;
                bool isLastSection;
                if (string.IsNullOrEmpty(groupValue))
                {
                    groupValue = "#";
                    isLastSection = true;
                    section = lastSection;
                }
                else
                {
                    isLastSection = false;
                    section = groupDictionary.ValueOrDefault(groupValue);
                }

                if (section == null)
                {
                    section = new UPMDocumentSection(StringIdentifier.IdentifierWithStringId(groupValue))
                    {
                        GroupName = new UPMStringField(StringIdentifier.IdentifierWithStringId(groupValue))
                        {
                            StringValue = groupValue
                        }
                    };
                    if (isLastSection)
                    {
                        lastSection = section; // Dont add to page here # should always be the last
                    }
                    else
                    {
                        groupDictionary.SetObjectForKey(section, groupValue);
                        page.AddChild(section);
                    }
                }

                UPMDocument document = new UPMDocument(documentData);
                section.AddChild(document);
            }

            if (lastSection != null)
            {
                page.AddChild(lastSection);
            }

            return true;
        }
    }
}
