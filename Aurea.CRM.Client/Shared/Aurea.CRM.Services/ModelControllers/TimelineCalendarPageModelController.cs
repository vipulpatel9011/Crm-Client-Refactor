// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimelineCalendarPageModelController.cs" company="Aurea Software Gmbh">
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
//   Timeline Calendar Page Model Controller
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
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// TImeline Calendar Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Search.SearchPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="ICalendarViewControllerDataProvider" />
    public class TimelineCalendarPageModelController : SearchPageModelController, ICalendarViewControllerDataProvider
    {
        private List<TimelineSearch> searches;
        private int nextSearch;
        private bool includeSystemCalendar;
        private DateTime? fromDate;
        private DateTime? toDate;
        private UPCalendarViewType viewType;
        private List<UPCoreMappingResultContext> resultContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineCalendarPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public TimelineCalendarPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.RecordIdentification = viewReference.ContextValueForKey("RecordId");
            this.TimelineConfiguration = ConfigurationUnitStore.DefaultStore.TimelineByName(viewReference.ContextValueForKey("TimelineConfigName"));
            if (string.IsNullOrEmpty(this.RecordIdentification) || this.TimelineConfiguration == null)
            {
                throw new Exception("Something wrong");
            }

            this.RequestOption = UPCRMDataStore.RequestOptionFromString(viewReference.ContextValueForKey("RequestOption"), UPRequestOption.FastestAvailable);
            RecordIdentifier identifier = new RecordIdentifier(this.RecordIdentification);
            UPMCalendarPage calendarPage = new UPMCalendarPage(identifier)
            {
                DefaultViewType = SearchPageViewType.CalendarMonth,
                IncludeSystemCalendar = viewReference.ContextValueForKey("IncludeSystemCalendar").ToBoolWithDefaultValue(true),
                Invalid = true,

                AvailableViewTypes = new List<SearchPageViewType>
                {
                    SearchPageViewType.CalendarDay,
                    SearchPageViewType.CalendarWeek,
                    SearchPageViewType.CalendarMonth,
                    SearchPageViewType.CalendarList
                }
            };

            this.TopLevelElement = calendarPage;
        }

        /// <summary>
        /// Gets the timeline configuration.
        /// </summary>
        /// <value>
        /// The timeline configuration.
        /// </value>
        public ConfigTimeline TimelineConfiguration { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the timeline searches.
        /// </summary>
        /// <value>
        /// The timeline searches.
        /// </value>
        public List<UPSearchPageModelControllerPreparedSearch> TimelineSearches { get; private set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets the iPad calendar filter.
        /// </summary>
        public UPMCatalogFilter IPadCalendarFilter { get; private set; }

        /// <summary>
        /// Gets the calendar page.
        /// </summary>
        public UPMCalendarPage CalendarPage => (UPMCalendarPage)this.Page;

        /// <summary>
        /// Gets the rep filter.
        /// </summary>
        public UPMFilter RepFilter => null;

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            // Do Nothing
        }

        /// <summary>
        /// Builds the page details.
        /// </summary>
        public override void BuildPageDetails()
        {
            // Do Nothing
        }

        /// <summary>
        /// The register add start date.
        /// </summary>
        /// <param name="date">The date.</param>
        public void RegisterAddStartDate(DateTime date)
        {
            // No implementation in xcode
        }

        /// <summary>
        /// Sets the type of the current calendar view.
        /// </summary>
        /// <param name="_viewType">Type of the view.</param>
        public void SetCurrentCalendarViewType(UPCalendarViewType _viewType)
        {
            this.viewType = _viewType;
        }

        /// <summary>
        /// Searches the specified search page.
        /// </summary>
        /// <param name="searchPage">The search page.</param>
        public override void Search(object searchPage)
        {
            if (this.TimelineSearches == null)
            {
                this.TimelineSearches = new List<UPSearchPageModelControllerPreparedSearch>();
                foreach (ConfigTimelineInfoArea timelineInfoArea in this.TimelineConfiguration.InfoAreas)
                {
                    UPSearchPageModelControllerPreparedSearch preparedSearch = new UPSearchPageModelControllerPreparedSearch(timelineInfoArea);
                    this.TimelineSearches.Add(preparedSearch);
                }
            }

            UPMCalendarPage calendarPage = (UPMCalendarPage)this.Page;
            this.fromDate = calendarPage.CalendarFromDate;
            this.toDate = calendarPage.CalendarToDate;
            string fromDateString = this.fromDate?.CrmValueFromDate();
            string toDateString = this.toDate?.CrmValueFromDate();
            int searchCount = this.TimelineSearches.Count;
            this.searches = new List<TimelineSearch>();

            for (int i = 0; i < searchCount; i++)
            {
                UPSearchPageModelControllerPreparedSearch preparedSearch = this.TimelineSearches[i];
                UPContainerMetaInfo crmQuery = preparedSearch.CrmQueryForValue(null, null, false);
                if (!string.IsNullOrEmpty(fromDateString))
                {
                    UPInfoAreaCondition fromCondition = new UPInfoAreaConditionLeaf(preparedSearch.TimelineConfiguration.InfoAreaId, preparedSearch.TimelineConfiguration.DateField.FieldId, ">=", fromDateString);
                    crmQuery.RootInfoAreaMetaInfo.AddCondition(fromCondition);
                }

                if (!string.IsNullOrEmpty(toDateString))
                {
                    int dateFieldIndex;
                    dateFieldIndex = preparedSearch.TimelineConfiguration.EndDateField?.FieldId ?? preparedSearch.TimelineConfiguration.DateField.FieldId;

                    UPInfoAreaCondition toCondition = new UPInfoAreaConditionLeaf(preparedSearch.TimelineConfiguration.InfoAreaId, dateFieldIndex, "<=", toDateString);
                    crmQuery.RootInfoAreaMetaInfo.AddCondition(toCondition);
                }

                crmQuery.SetLinkRecordIdentification(this.RecordIdentification, preparedSearch.TimelineConfiguration.LinkId);
                this.searches.Add(new TimelineSearch(crmQuery, preparedSearch));
            }

            this.nextSearch = 0;
            this.ExecuteNextSearch();
        }

        /// <summary>
        /// The sender needs calendar item details.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="item">The item.</param>
        public void SenderNeedsCalendarItemDetails(/*UPCalendarViewController*/object sender, ICalendarItem item)
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
        /// Updateds the element for page.
        /// </summary>
        /// <param name="origSearchPage">The original search page.</param>
        /// <returns></returns>
        public override Page UpdatedElementForPage(UPMSearchPage origSearchPage)
        {
            if (this.ModelControllerDelegate == null)
            {
                return origSearchPage;
            }

            if (this.CalendarPage.IncludeSystemCalendar && !ResultRowCalendarItem.EventStoreInitialized)
            {
                //ResultRowCalendarItem.initializeEventStore(delegate (bool granted, NSError error)
                //{
                //    if (!granted)
                //    {
                //        this.CalendarPage.IncludeSystemCalendar = false;
                //    }
                //    else
                //    {
                //        NSNotificationCenter.DefaultCenter().RemoveObserverNameTheObject(this, EKEventStoreChangedNotification, null);
                //        NSNotificationCenter.DefaultCenter().AddObserverSelectorNameTheObject(this, @selector(eventStoreUpdated:), EKEventStoreChangedNotification, null);
                //        ResultRowCalendarItem.RefreshEventStore();
                //    }

                //    this.Search((UPMSearchPage)this.Page);
                //});
                //return this.Page;
            }

            this.Search(origSearchPage);
            return this.Page;
        }

        /// <summary>
        /// Updates the element for calendar group.
        /// </summary>
        /// <param name="origDetailGroup">The original detail group.</param>
        /// <returns></returns>
        public override UPMCalendarPopoverGroup UpdateElementForCalendarGroup(UPMCalendarPopoverGroup origDetailGroup)
        {
            if (!(origDetailGroup.Context is ResultRowCalendarItem))
            {
                return base.UpdateElementForCalendarGroup(origDetailGroup);
            }

            if (origDetailGroup.Invalid)
            {
                ResultRowCalendarItem calendarItem = (ResultRowCalendarItem)origDetailGroup.Context;
                UPCoreMappingResultContext resultContext = calendarItem.ResultContext;
                TimelineSearch timelineSearch = (TimelineSearch)resultContext.Context;
                string sourceCopyFieldGroupName = timelineSearch.FieldGroupName;
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Details", sourceCopyFieldGroupName) ??
                                                  configStore.FieldControlByNameFromGroup("MiniDetails", sourceCopyFieldGroupName);

                this.Loader = new UPCalendarPopoverLoader(this);
                this.Loader.LoadElementForCalendarGroupFieldControl(origDetailGroup, sourceFieldControl);
            }

            return origDetailGroup;
        }

        /// <summary>
        /// Adds the dropdown groups for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="rowContext">The row context.</param>
        /// <param name="expand">The expand.</param>
        public override void AddDropdownGroupsForResultRow(UPMResultRow resultRow, UPCoreMappingResultRowContext rowContext, UPConfigExpand expand)
        {
            UPCoreMappingResultContext resultContext = rowContext.Context;
            if (resultContext.DropdownFields.Any())
            {
                int detailFieldCount = resultContext.DropdownFields.Count;
                if (detailFieldCount > 0)
                {
                    UPCRMResultRow row = rowContext.Row;
                    string recordId = row.RecordIdentificationAtIndex(0);
                    var detailGroupCol1 =
                        new UPMTimelineDetailsGroup(FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(this.InfoAreaId, recordId, "DetailField_Left"))
                        {
                            Invalid = true,
                            ResultContext = resultContext
                        };
                    resultRow.AddDetailGroup(detailGroupCol1);
                }
            }
        }

        /// <summary>
        /// Creates the new calendar item for date with button.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="button">The button.</param>
        public void CreateNewCalendarItemForDate(DateTime date, UPConfigButton button)
        {
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ReportError(error, false);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            TimelineSearch timelineSearch = this.searches[this.nextSearch++];
            timelineSearch.Result = result;
            this.ExecuteNextSearch();
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

        private List<ICalendarItem> CalendarItemsFromResult()
        {
            List<ICalendarItem> calendarItems = new List<ICalendarItem>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.resultContexts = new List<UPCoreMappingResultContext>();
            UPMAction goToAction = new UPMAction(StringIdentifier.IdentifierWithStringId("action"));
            goToAction.SetTargetAction(this, this.SwitchToDetail);
            goToAction.LabelText = LocalizedString.TextShowRecord;

            foreach (TimelineSearch timelineSearch in this.searches)
            {
                UPCRMResult result = timelineSearch.Result;
                int rowCount = result.RowCount;
                if (rowCount == 0)
                {
                    continue;
                }

                UPConfigTableCaption tableCaption = configStore.TableCaptionByName(timelineSearch.TimelineInfoArea.ConfigName) ??
                                                    configStore.TableCaptionByName(timelineSearch.TimelineInfoArea.InfoAreaId);

                List<UPContainerFieldMetaInfo> tableCaptionResultFieldMap = tableCaption.ResultFieldMapFromMetaInfo(result.MetaInfo);
                if (tableCaptionResultFieldMap == null)
                {
                    continue;
                }

                UPCRMResultCondition resultCondition = null;
                var functionNameFieldMapping = result.MetaInfo.SourceFieldControl.FunctionNames();
                UPConfigFieldControlField fromField = functionNameFieldMapping["Date"];
                if (fromField != null)
                {
                    if (this.fromDate != null)
                    {
                        resultCondition = new UPCRMResultFieldCondition(fromField.Field, UPConditionOperator.GreaterEqual,
                            this.fromDate.Value.CrmValueFromDate(), fromField.TabIndependentFieldIndex);
                    }

                    if (this.toDate != null)
                    {
                        UPConfigFieldControlField toField = (functionNameFieldMapping.ContainsKey("EndDate")
                            ? functionNameFieldMapping["EndDate"]
                            : null) ?? fromField;

                        UPCRMResultCondition toCondition = new UPCRMResultFieldCondition(toField.Field, UPConditionOperator.LessEqual,
                            this.toDate.Value.CrmValueFromDate(), toField.TabIndependentFieldIndex);
                        resultCondition = resultCondition != null ? resultCondition.ConditionByAppendingANDCondition(toCondition) : toCondition;
                    }
                }

                UPCoreMappingResultContext resultContext = new UPCoreMappingResultContext(result, result.MetaInfo.SourceFieldControl, timelineSearch.PreparedSearch.ListFieldControl.NumberOfFields);
                resultContext.Context = timelineSearch;
                this.resultContexts.Add(resultContext);

                UPConfigExpand expand = configStore.ExpandByName(timelineSearch.TimelineInfoArea.ConfigName) ??
                                        configStore.ExpandByName(timelineSearch.TimelineInfoArea.InfoAreaId);

                AureaColor defaultColor = null;
                if (!string.IsNullOrEmpty(timelineSearch.TimelineInfoArea.ColorString))
                {
                    defaultColor = AureaColor.ColorWithString(timelineSearch.TimelineInfoArea.ColorString);
                }

                if (defaultColor == null)
                {
                    defaultColor = AureaColor.ColorWithString(expand.ColorKey);
                }

                for (int i = 0; i < rowCount; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                    if (resultCondition != null && !resultCondition.Check(row))
                    {
                        continue;
                    }

                    ConfigTimelineCriteria matchingCriteria = timelineSearch.MatchingCriteriaForRow(row);
                    AureaColor color = null;
                    if (matchingCriteria.Setting1 != null)
                    {
                        color = AureaColor.ColorWithString(matchingCriteria.Setting1);
                    }

                    if (color == null)
                    {
                        color = defaultColor;
                    }

                    ICalendarItem calendarItem = new ResultRowCalendarItem(row, resultContext, new RecordIdentifier(row.RootRecordIdentification), tableCaption, tableCaptionResultFieldMap, null, color);
                    calendarItem.GoToAction = goToAction;
                    calendarItems.Add(calendarItem);
                }
            }

            if (this.fromDate != null && this.toDate != null && this.CalendarPage.IncludeSystemCalendar)
            {
#if PORTING
                ArrayList localCalendarItems = ResultRowCalendarItem.EventsFromLocalCalendarFromToSearchTextCalenderIdentifiers(this.fromDate, this.toDate, null, null);
                if (localCalendarItems.Count)
                {
                    calendarItems.AddRange(localCalendarItems);
                }
#endif
            }

            return calendarItems;
        }

        private void BuildPageFromResults()
        {
            UPMCalendarPage calendarPage = (UPMCalendarPage)this.Page;
            calendarPage.CalendarItems = this.CalendarItemsFromResult();

            if (this.viewType == UPCalendarViewType.Day || this.viewType == UPCalendarViewType.List)
            {
                string sortSequence = this.ViewReference.ContextValueForKey("SortSequence");
                bool isDescending = sortSequence != null && sortSequence == "DESC";
                List<ICalendarItem> sorted = calendarPage.CalendarItems.Sort(!isDescending);
                List<UPMResultSection> resultSections = sorted.ResultSectionsForData(null, null);
                this.SearchPage.RemoveAllChildren();
                foreach (UPMResultSection section in resultSections)
                {
                    this.SearchPage.AddResultSection(section);
                }

                UPMAction goToAction = new UPMAction(StringIdentifier.IdentifierWithStringId("action"));
                goToAction.SetTargetAction(this, this.SwitchToDetail);
                goToAction.LabelText = LocalizedString.TextShowRecord;
                calendarPage.RowAction = goToAction;
            }

            this.Page.Invalid = false;
            base.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);
        }

        private void ExecuteNextSearch()
        {
            if (this.nextSearch < this.searches.Count)
            {
                TimelineSearch timelineSearch = this.searches[this.nextSearch];
                if (timelineSearch.CrmQuery == null)
                {
                    ++this.nextSearch;
                    this.ExecuteNextSearch();
                    return;
                }

                timelineSearch.CrmQuery.Find(this.RequestOption, this);
                return;
            }

            this.BuildPageFromResults();
        }
    }
}
