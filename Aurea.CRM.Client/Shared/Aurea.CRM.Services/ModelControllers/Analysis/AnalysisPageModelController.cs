// <copyright file="AnalysisPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Services.ModelControllers.Analysis
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Analysis;
    using Aurea.CRM.Core.Analysis.DataSource;
    using Aurea.CRM.Core.Analysis.Drilldown;
    using Aurea.CRM.Core.Analysis.Result;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Implementation of analysis page model controller
    /// </summary>
    public class AnalysisPageModelController : SearchPageModelController, IAnalysisDelegate, UPCopyFieldsDelegate, UPCRMLinkReaderDelegate, ISearchOperationHandler
    {
        private UPCopyFields copyFieldReader;
        private UPContainerMetaInfo crmQuery;
        private UPCRMLinkReader linkReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public AnalysisPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            if (this.ViewReference.ViewName == "Query")
            {
                this.IsQuery = true;
                this.QueryName = this.ViewReference.ContextValueForKey("Query");
            }
            else
            {
                this.IsQuery = false;
                this.AnalysisName = this.ViewReference.ContextValueForKey("Analysis");
            }

            this.RequestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference.ContextValueForKey("RequestOption"), UPRequestOption.BestAvailable);
            this.SourceCopyFieldGroupName = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
            this.SourceCopyRecordIdentification = this.ViewReference.ContextValueForKey("CopySourceRecordId");
            if (string.IsNullOrEmpty(this.SourceCopyRecordIdentification))
            {
                this.SourceCopyRecordIdentification = this.RecordIdentification;
            }

            if (string.IsNullOrEmpty(this.SourceCopyFieldGroupName))
            {
                this.FilterParameters = this.ViewReference.JsonDictionaryForKey("copyFields");
            }

            this.ParentLink = this.ViewReference.ContextValueForKey("ParentLink");
            this.Options = this.ViewReference.JsonDictionaryForKey("Options");
            this.BuildPage();
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets analysis link record identification
        /// </summary>
        public string AnalysisLinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets analysis name
        /// </summary>
        public string AnalysisName { get; private set; }

        /// <summary>
        /// Gets analysis result
        /// </summary>
        public AnalysisResult AnalysisResult { get; private set; }

        /// <summary>
        /// Gets or sets analysis settings
        /// </summary>
        public AnalysisExecutionSettings AnalysisSettings { get; set; }

        /// <summary>
        /// Gets filter parameters
        /// </summary>
        public Dictionary<string, object> FilterParameters { get; private set; }

        /// <summary>
        /// Gets a value indicating whether gets is query
        /// </summary>
        public bool IsQuery { get; private set; }

        /// <summary>
        /// Gets link id
        /// </summary>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets options
        /// </summary>
        public Dictionary<string, object> Options { get; private set; }

        /// <summary>
        /// Gets parent link
        /// </summary>
        public string ParentLink { get; private set; }

        /// <summary>
        /// Gets query name
        /// </summary>
        public string QueryName { get; private set; }

        /// <summary>
        /// Gets query result
        /// </summary>
        public UPCRMResult QueryResult { get; private set; }

        /// <summary>
        /// Gets record identification
        /// </summary>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets request option
        /// </summary>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets source copy field group name
        /// </summary>
        public string SourceCopyFieldGroupName { get; private set; }

        /// <summary>
        /// Gets source copy record identification
        /// </summary>
        public string SourceCopyRecordIdentification { get; private set; }

        /// <inheritdoc/>
        public void AnalysisDidFailWithError(Analysis analysis, Exception error)
        {
            this.ReportError(error, false);
        }

        /// <inheritdoc/>
        public void AnalysisDidFinishWithResult(Analysis analysis, AnalysisResult result)
        {
            bool defaultSettingsUsed = analysis.DefaultExecutionSettings == this.AnalysisSettings;
            this.UpdatePageFromResultReset(result, !defaultSettingsUsed);
        }

        /// <inheritdoc/>
        public override void BuildPage()
        {
            UPMGridPage sp = (UPMGridPage)this.CreatePageInstance();
            sp.Invalid = true;
            this.TopLevelElement = sp;
        }

        /// <summary>
        /// Continues with filter parameters
        /// </summary>
        /// <param name="filterParameters">Filter parameters</param>
        public void ContinueWithFilterParameters(Dictionary<string, object> filterParameters)
        {
            Dictionary<string, object> additionalParameters = this.ViewReference.ContextValueForKey("AdditionalParameters").JsonDictionaryFromString();
            if (additionalParameters == null)
            {
                this.FilterParameters = filterParameters;
            }
            else if (filterParameters == null || filterParameters.Count == 0)
            {
                this.FilterParameters = additionalParameters;
            }
            else
            {
                var dict = new Dictionary<string, object>(additionalParameters);
                dict.Append(filterParameters);
                this.FilterParameters = dict;
            }

            if (this.IsQuery)
            {
                this.ContinueWithQuery(this.FilterParameters);
                return;
            }

            UPConfigAnalysis configAnalysis = ConfigurationUnitStore.DefaultStore.AnalysisByName(this.AnalysisName);
            this.Analysis = null;
            if (configAnalysis != null)
            {
                AnalysisQueryExecutionContext queryExecutionContext;
                if (this.RecordIdentification?.Length > 0)
                {
                    queryExecutionContext = new AnalysisQueryExecutionContext(configAnalysis, this.FilterParameters, this.RecordIdentification, this.LinkId);
                }
                else
                {
                    queryExecutionContext = new AnalysisQueryExecutionContext(configAnalysis, this.FilterParameters);
                }

                if (queryExecutionContext != null)
                {
                    this.Analysis = new Analysis(queryExecutionContext, this.Options);
                }
            }

            if (this.Analysis == null)
            {
                var analysisNotFound = $"Analysis {this.AnalysisName} could not be loaded";
                this.ReportError(new Exception(analysisNotFound), false);
                return;
            }

            this.AnalysisSettings = this.Analysis.DefaultExecutionSettings;
            this.Analysis.ComputeResultWithSettingsRequestOption(this.AnalysisSettings, this.RequestOption, this);
        }

        /// <summary>
        /// Continues with query
        /// </summary>
        /// <param name="filterParameters">Filter parameters</param>
        public void ContinueWithQuery(Dictionary<string, object> filterParameters)
        {
            UPConfigQuery query = ConfigurationUnitStore.DefaultStore.QueryByName(this.QueryName);
            query = query.QueryByApplyingValueDictionaryDefaults(filterParameters, true);
            if (query == null)
            {
                string queryNotFound = $"query {this.QueryName} could not be found";
                this.ReportError(new Exception(queryNotFound), false);
                return;
            }

            this.crmQuery = new UPContainerMetaInfo(query);
            if (this.RecordIdentification?.Length > 0)
            {
                this.crmQuery.SetLinkRecordIdentification(this.RecordIdentification, this.LinkId);
            }

            var strOption = this.Options.ValueOrDefault("MaxResults") as string;
            if (strOption?.Length > 0)
            {
                this.MaxResults = strOption.ToInt();
                if (this.MaxResults > 0)
                {
                    this.crmQuery.MaxResults = this.MaxResults;
                }
            }
            else
            {
                this.MaxResults = 0;
            }

            this.crmQuery.Find(this.RequestOption, this);
        }

        /// <inheritdoc/>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.copyFieldReader = null;
            this.ReportError(error, false);
        }

        /// <inheritdoc/>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.copyFieldReader = null;
        }

        /// <inheritdoc/>
        public override UPMSearchPage CreatePageInstance()
        {
            UPMGridPage page = new UPMGridPage(StringIdentifier.IdentifierWithStringId("Analysis"))
            {
                ViewType = SearchPageViewType.List,
                AvailableFilters = null,
                AvailableOnlineSearch = false,
                HideSearchBar = true,
                FixedFirstColumn = true,
                SumRowAtEnd = true,
                EmptyCategoryText = string.Empty,
                IsUnsortedStateAllowed = true
            };

            object val = this.Options.ValueOrDefault("FixedSumRow");
            if (val != null)
            {
                if (val is int)
                {
                    page.FixedSumRow = (int)val > 0;
                }
                else if (val is string)
                {
                    page.FixedSumRow = ((string)val).ToLower() != "false";
                }
            }
            else
            {
                page.FixedSumRow = false;
            }

            return page;
        }

        /// <inheritdoc/>
        public override List<UPMAction> FindQuickActionsForRowCheckDetails(UPMResultRow resultRow, bool checkDetails)
        {
            return resultRow.DetailActions;
        }

        /// <inheritdoc/>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.linkReader = null;
            this.ReportError(error, false);
        }

        /// <inheritdoc/>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            this.linkReader = null;
            this.ContinueWithAnalysisLinkRecord(linkReader.DestinationRecordIdentification);
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ReportError(error, false);
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.UpdatePageFromQueryResult(result);
        }

        /// <inheritdoc/>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
        }

        /// <inheritdoc/>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            if (element is Page)
            {
                return this.UpdatedElementForPage((UPMGridPage)element);
            }

            if (element is UPMAnalysisResultRowGroup)
            {
                return UpdatedElementForResultRowGroup((UPMAnalysisResultRowGroup)element);
            }

            return element;
        }

        /// <inheritdoc/>
        public override Page UpdatedElementForPage(UPMSearchPage page)
        {
            if (this.AnalysisLinkRecordIdentification?.Length > 0)
            {
                this.ContinueWithAnalysisLinkRecord(this.AnalysisLinkRecordIdentification);
            }
            else if (this.ParentLink?.Length > 0 && this.RecordIdentification?.Length > 0)
            {
                this.linkReader = new UPCRMLinkReader(this.RecordIdentification, this.ParentLink, this.RequestOption, this);
                this.linkReader.Start();
            }
            else
            {
                this.ContinueWithAnalysisLinkRecord(this.RecordIdentification);
            }

            return this.Page;
        }

        private static UPMResultSection CreateSection(List<object> columnInfoArray, UPMGridPage searchPage)
        {
            var identifier = StringIdentifier.IdentifierWithStringId("columnHeader");
            var section = new UPMResultSection(identifier);
            var columnHeaderListRow = new UPMResultRow(identifier);

            var fieldArray = CreateFieldArray(searchPage, columnInfoArray);

            columnHeaderListRow.Fields = fieldArray;
            section.AddResultRow(columnHeaderListRow);

            return section;
        }

        private static List<UPMField> CreateFieldArray(UPMGridPage searchPage, List<object> columnInfoArray)
        {
            var fieldArray = new List<UPMField>();
            var i = 0;
            foreach (AnalysisDrillThruColumn col in columnInfoArray)
            {
                var fieldType = col.SourceField.CrmFieldInfo.FieldType;
                if (fieldType == "F" || fieldType == "L" || fieldType == "S")
                {
                    searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(i, UPMColumnDataType.Numeric, false);
                }
                else if (fieldType == "D")
                {
                    searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(i, UPMColumnDataType.Date, false);
                }

                var field = new UPMStringField(StringIdentifier.IdentifierWithStringId($"col {++i}"))
                {
                    StringValue = col.DataSourceField.Label
                };
                fieldArray.Add(field);
            }

            return fieldArray;
        }

        private static UPMElement UpdatedElementForResultRowGroup(UPMAnalysisResultRowGroup rowGroup)
        {
            var group = new UPMAnalysisResultRowGroup(rowGroup.Identifier)
            {
                Left = rowGroup.Left,
                Row = rowGroup.Row
            };

            var rowDetails = rowGroup.Row.RowDetails;
            if (rowGroup.Left)
            {
                bool firstXCategory = true;
                foreach (AnalysisResultCell resultCell in rowGroup.Row.Values)
                {
                    var field = new UPMStringField(null)
                    {
                        LabelText = resultCell.Column.Label,
                        StringValue = resultCell.StringValue
                    };

                    group.AddField(field);
                    int xCount = resultCell.XResultCells.Count;
                    if (firstXCategory && xCount > 0)
                    {
                        int i;
                        firstXCategory = false;
                        for (i = 0; i < xCount; i++)
                        {
                            field = new UPMStringField(null)
                            {
                                LabelText = $"X:{resultCell.Column.XCategoryValues[i].Label}",
                                StringValue = resultCell.XResultCells[i].StringValue
                            };

                            group.AddField(field);
                        }
                    }
                }
            }
            else
            {
                int maxCount = 0;
                foreach (AnalysisRowDetails rowDet in rowDetails)
                {
                    var stringValues = rowDet.StringValues;
                    int count = stringValues.Count;
                    UPMStringField field = null;
                    if (stringValues.Count > 0)
                    {
                        field = new UPMStringField(null);
                        if (++maxCount > 3)
                        {
                            field.LabelText = "Number of Results:";
                            field.StringValue = $"{rowDetails.Count}";
                            group.AddField(field);
                            break;
                        }

                        if (count > 1)
                        {
                            field.LabelText = stringValues[0];
                            field.StringValue = stringValues[1];
                        }
                        else
                        {
                            field.StringValue = stringValues[0];
                        }
                    }

                    if (field != null)
                    {
                        group.AddField(field);
                    }
                }
            }

            return group;
        }

        private static List<object> CreateResultRows(AnalysisResult result)
        {
            var resultRows = new List<object>(result.Rows);
            var sumRow = result.SumLine;
            if (sumRow != null)
            {
                resultRows.Add(sumRow);
            }

            return resultRows;
        }

        private static List<UPMField> CreateFieldArray(AnalysisRow row, IReadOnlyList<object> xColumnArray, bool keyAsRawString, int columnCount, int i)
        {
            var fieldArray = new List<UPMField>();
            var field = new UPMStringField(StringIdentifier.IdentifierWithStringId($"row {i}"))
            {
                StringValue = row.Label,
                RawStringValue = keyAsRawString ? row.Key : row.Label
            };
            fieldArray.Add(field);

            for (var j = 0; j < columnCount; j++)
            {
                var field2 = new UPMStringField(StringIdentifier.IdentifierWithStringId($"cell{i}_{j}"));
                var resultCell = row.Values[j] as AnalysisResultCell;
                if (resultCell == null)
                {
                    throw new InvalidOperationException("value must be AnalysisResultCell");
                }

                field2.StringValue = resultCell.ToString();
                field2.RawStringValue = resultCell.Value.ToString();
                fieldArray.Add(field2);
                var numberOfXColumn = xColumnArray[j].ToInt();
                for (var k = 0; k < numberOfXColumn; k++)
                {
                    var xField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"cell{i}_{j}_{k}"))
                    {
                        StringValue = resultCell.StringXResultAtIndex(k),
                        RawStringValue = resultCell.RawStringXResultAtIndex(k)
                    };
                    fieldArray.Add(xField);
                }
            }

            return fieldArray;
        }

        private static void ProcessColumns(
            IEnumerable<object> resultColumns,
            UPMGridPage searchPage,
            ICollection<UPMField> fieldArray,
            ICollection<object> xColumnArray)
        {
            var i = 0;
            var k = 0;
            foreach (AnalysisColumn col in resultColumns)
            {
                var colDataType = col.IsTextColumn ? UPMColumnDataType.String : UPMColumnDataType.Numeric;
                searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(++k, colDataType, false);

                var field = new UPMGridColumnHeaderStringField(StringIdentifier.IdentifierWithStringId($"col {++i}"))
                {
                    StringValue = col.Label
                };
                fieldArray.Add(field);

                field.NumberOfChildColumns = col.XCategoryValues?.Count ?? 0;
                xColumnArray.Add(field.NumberOfChildColumns);
                var j = 0;

                if (col.XCategoryValues != null)
                {
                    foreach (var xCol in col.XCategoryValues)
                    {
                        searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(++k, colDataType, false);

                        var xField = new UPMGridColumnHeaderStringField(StringIdentifier.IdentifierWithStringId($"col {i}-x {++j}"))
                        {
                            NumberOfChildColumns = 0,
                            IsSubField = true,
                            StringValue = xCol.Label
                        };
                        fieldArray.Add(xField);
                    }
                }
            }
        }

        private static List<string> GetContextMenuOptions(ICrmDataSource dataSource)
        {
            var contextMenuOptions = new List<string>();
            var configStore = ConfigurationUnitStore.DefaultStore;
            for (var j = 0; j < dataSource.NumberOfResultTables; j++)
            {
                var infoAreaId = dataSource.ResultTableAtIndex(j).InfoAreaId;
                var infoAreaLabel = string.Empty;
                if (infoAreaId?.Length > 0)
                {
                    var configInfoArea = configStore.InfoAreaConfigById(infoAreaId);
                    var expand = configStore.ExpandByName(infoAreaId);
                    var fieldControl = configStore.FieldControlByNameFromGroup("Details", expand.FieldGroupName);
                    if (configInfoArea != null && expand != null && fieldControl != null)
                    {
                        infoAreaLabel = LocalizedString.TextAnalysesShowParam.Replace("%@", configInfoArea.SingularName);
                    }
                }

                contextMenuOptions.Add(infoAreaLabel);
            }

            return contextMenuOptions;
        }

        private void AddCategoriesFromResultToPage(AnalysisResult result, UPMGridPage gridPage)
        {
            AnalysisCategory currentCategory = result.Settings.Category;
            gridPage.AddCategory(new UPMGridCategory(currentCategory.Label, null, true));
            foreach (AnalysisCategory category in result.CategoryOptions)
            {
                UPMOrganizerAction action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId($"action.{category.Key}"));
                action.SetTargetAction(this, this.PerformAnalysisWithCategory);
                action.ViewReference = new ViewReference(new Dictionary<string, object> { { "Category", category.Key } }, "PerformAnalysis");
                action.LabelText = category.Label;
                gridPage.AddCategory(new UPMGridCategory(category.Label, action, false));
            }
        }

        private void AddCategoryActionsToRow(UPMResultRow row)
        {
            var categories = this.Analysis.CategoryDictionary.Values;
            UPMGroup detailGroupCol1 = new UPMGroup(StringIdentifier.IdentifierWithStringId("det1"));
            foreach (AnalysisCategory category in categories)
            {
                UPMStringField field = new UPMStringField(null)
                {
                    LabelText = category.Key,
                    StringValue = category.Label
                };

                detailGroupCol1.AddField(field);
            }

            row.AddDetailGroup(detailGroupCol1);
            UPMGroup detailGroupCol2 = new UPMGroup(StringIdentifier.IdentifierWithStringId("det2"));
            row.AddDetailGroup(detailGroupCol2);
            foreach (var analysisResultColumn in this.AnalysisSettings.ResultColumns)
            {
                UPMStringField field = new UPMStringField(null)
                {
                    LabelText = analysisResultColumn.Key,
                    StringValue = analysisResultColumn.Label
                };
                detailGroupCol2.AddField(field);
            }
        }

        private void AddDetailsActionFromRowToListRow(AnalysisRow row, UPMResultRow listRow)
        {
            if (row.ResultRows.Count > 0 && row.Result.DetailsFields.Count > 0)
            {
                var action = new UPMOrganizerDrillThruAction(StringIdentifier.IdentifierWithStringId("drillThru"))
                {
                    AnalysisRow = row
                };
                action.SetTargetAction(this, this.PerformDrillThru);
                action.LabelText = LocalizedString.TextAnalysesDetails;
                listRow.AddDetailAction(action);
            }
        }

        private void AddDrilldownActionsFromRowToListRow(AnalysisRow row, UPMResultRow listRow)
        {
            string drilldownText = LocalizedString.TextAnalysesDrilldown;
            foreach (AnalysisDrilldownOption drilldownOption in row.DrilldownOptions)
            {
                var action = new UPMOrganizerDrilldownAction(StringIdentifier.IdentifierWithStringId($"action.{drilldownOption.Key}"))
                {
                    DrilldownOption = drilldownOption,
                    AnalysisRow = row
                };
                action.SetTargetAction(this, this.PerformDrilldown);
                action.LabelText = drilldownText.Replace("%@", drilldownOption.Label);
                listRow.AddDetailAction(action);
            }
        }

        private void AddDrillupActionsFromResultToPage(AnalysisResult result, UPMGridPage gridPage)
        {
            if (result.DrillupOptions != null)
            {
                foreach (AnalysisDrillupOption drillupOption in result.DrillupOptions)
                {
                    string label = drillupOption.IsBack ? LocalizedString.TextBack : drillupOption.DrillupDisplayString();

                    UPMOrganizerDrillupAction action = new UPMOrganizerDrillupAction(StringIdentifier.IdentifierWithStringId($"action.{drillupOption.Key}"))
                    {
                        DrillupOption = drillupOption
                    };

                    action.SetTargetAction(this, this.PerformDrillUp);
                    gridPage.AddHeadOption(new UPMGridHeadOption(label, action));
                }
            }
        }

        private void ContinueWithAnalysisLinkRecord(string analysisLinkRecordIdentification)
        {
            this.AnalysisLinkRecordIdentification = analysisLinkRecordIdentification;
            if (this.FilterParameters != null)
            {
                this.ContinueWithFilterParameters(this.FilterParameters);
            }
            else if (this.SourceCopyFieldGroupName?.Length > 0 && this.SourceCopyRecordIdentification?.Length > 0)
            {
                var fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", this.SourceCopyFieldGroupName);
                if (fieldControl != null)
                {
                    this.copyFieldReader = new UPCopyFields(fieldControl);
                    this.copyFieldReader.CopyFieldValuesForRecord(new UPCRMRecord(this.SourceCopyRecordIdentification), false, this);
                }
                else
                {
                    this.ContinueWithFilterParameters(null);
                }
            }
            else
            {
                this.ContinueWithFilterParameters(null);
            }
        }

        private void PerformAnalysisWithCategory(object dict)
        {
            var dictionary = dict as Dictionary<string, object>;
            var action = dictionary.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerAction;
            string categoryKey = action.ViewReference.ContextValueForKey("Category");
            var category = this.Analysis.CategoryDictionary.ValueOrDefault(categoryKey) as AnalysisCategory;
            this.AnalysisSettings = this.Analysis.DefaultExecutionSettings.SettingsWithCategory(category);
            this.Analysis.ComputeResultWithSettingsRequestOption(this.AnalysisSettings, this.RequestOption, this);
            this.ParentOrganizerModelController.SetFocusViewVisible(false);
        }

        private void PerformBackToAnalysis(object dict)
        {
            var dictionary = dict as Dictionary<string, object>;
            var action = dictionary?.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerAnalysisBackAction;
            this.ParentOrganizerModelController.SetFocusViewVisible(false);
            this.UpdatePageFromResultReset(action.AnalysisResult, this.AnalysisSettings != this.Analysis.DefaultExecutionSettings);
        }

        private void PerformDrilldown(object dict)
        {
            var dictionary = dict as Dictionary<string, object>;
            var action = dictionary?.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerDrilldownAction;
            this.ParentOrganizerModelController.SetFocusViewVisible(false);
            this.AnalysisSettings = this.AnalysisSettings.SettingsWithDrilldownOptionRow(action.DrilldownOption, action.AnalysisRow);
            this.Analysis.ComputeResultWithSettingsRequestOption(this.AnalysisSettings, this.RequestOption, this);
        }

        private void PerformDrillThru(object dict)
        {
            var dictionary = dict as Dictionary<string, object>;
            var action = dictionary?.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerDrillThruAction;
            if (action == null)
            {
                throw new InvalidOperationException("dictionary must contain 'UPMOrganizerAction' key with UPMOrganizerDrillThruAction value");
            }

            var row = action.AnalysisRow;
            var analysisResult = row.Result;
            var dataSource = analysisResult.DataSource;
            var searchPage = (UPMGridPage)this.CreatePageInstance();
            var oldPage = this.Page;
            this.ParentOrganizerModelController.SetFocusViewVisible(false);

            var columnInfoArray = this.CreateColumnInfoArray(dataSource);
            var section = CreateSection(columnInfoArray, searchPage);
            searchPage.AddResultSection(section);

            this.ProcessResultRows(row, columnInfoArray, section, searchPage, analysisResult, oldPage);
        }

        private List<object> CreateColumnInfoArray(ICrmDataSource dataSource)
        {
            var columnInfoArray = new List<object>();
            foreach (AnalysisSourceField col in this.AnalysisResult.DetailsFields)
            {
                if (col.QueryResultFieldIndex < 0)
                {
                    continue;
                }

                ICrmDataSourceField field = dataSource.FieldAtIndex(col.QueryResultFieldIndex);
                if (field == null)
                {
                    continue;
                }

                columnInfoArray.Add(new AnalysisDrillThruColumn(col, field));
            }

            return columnInfoArray;
        }

        private void ProcessResultRows(
            AnalysisRow row,
            List<object> columnInfoArray,
            UPMResultSection section,
            UPMGridPage searchPage,
            AnalysisResult analysisResult,
            Page oldPage)
        {
            var dataSource = analysisResult.DataSource;
            var contextMenuOptions = GetContextMenuOptions(dataSource);
            var i = 0;
            foreach (ICrmDataSourceRow crmRow in row.ResultRows)
            {
                var identifier = StringIdentifier.IdentifierWithStringId($"row {i}");
                var listRow = new UPMResultRow(identifier)
                {
                    Context = row
                };
                var fieldArray = new List<UPMField>();
                foreach (AnalysisDrillThruColumn col in columnInfoArray)
                {
                    var field = new UPMStringField(StringIdentifier.IdentifierWithStringId($"row {++i}"))
                    {
                        StringValue = crmRow.ValueAtIndex(col.SourceField.QueryResultFieldIndex),
                        RawStringValue = crmRow.RawValueAtIndex(col.SourceField.QueryResultFieldIndex)
                    };
                    fieldArray.Add(field);
                }

                listRow.Fields = fieldArray;
                section.AddResultRow(listRow);
                ++i;
                this.ProcessSearchPage(searchPage, contextMenuOptions, crmRow, listRow, analysisResult, oldPage, i);
            }
        }

        private void ProcessSearchPage(UPMGridPage searchPage, IReadOnlyList<string> contextMenuOptions, ICrmDataSourceRow crmRow, UPMResultRow listRow, AnalysisResult analysisResult, Page oldPage, int i)
        {
            var dataSource = analysisResult.DataSource;
            for (var j = 0; j < dataSource.NumberOfResultTables; j++)
            {
                var label = contextMenuOptions[j];
                if (string.IsNullOrEmpty(label))
                {
                    continue;
                }

                var recordIdentification = crmRow.RecordIdentificationAtIndex(j);
                if (recordIdentification?.Length > 0)
                {
                    var showRecordAction = new UPMOrganizerAnalysisShowRecordAction(StringIdentifier.IdentifierWithStringId($"action.row {i} record {j}"))
                    {
                        RecordIdentification = recordIdentification
                    };

                    showRecordAction.SetTargetAction(this, this.PerformShowRecordAction);
                    showRecordAction.LabelText = label;
                    listRow.AddDetailAction(showRecordAction);
                }

                var backAction = new UPMOrganizerAnalysisBackAction(StringIdentifier.IdentifierWithStringId("action.back"))
                {
                    AnalysisResult = analysisResult
                };

                backAction.SetTargetAction(this, this.PerformBackToAnalysis);
                backAction.LabelText = LocalizedString.TextAnalysesBackToAnalysis;
                searchPage.AddHeadOption(new UPMGridHeadOption(backAction.LabelText, backAction));
                searchPage.FixedFirstColumn = false;
                searchPage.SumRowAtEnd = false;
                var hasOnlyEmptyLabels = true;
                foreach (var lbl in contextMenuOptions)
                {
                    if (lbl.Length > 0)
                    {
                        hasOnlyEmptyLabels = false;
                        break;
                    }
                }

                searchPage.ShowMenu = !hasOnlyEmptyLabels;
                this.TopLevelElement = searchPage;
                this.InformAboutDidChangeTopLevelElement(oldPage, searchPage, null, null);
            }
        }

        private void PerformDrillUp(object dict)
        {
            var dictionary = dict as Dictionary<string, object>;
            var action = dictionary?.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerDrillupAction;
            this.ParentOrganizerModelController.SetFocusViewVisible(false);
            this.AnalysisSettings = this.AnalysisSettings.SettingsWithDrillupOption(action.DrillupOption);
            this.Analysis.ComputeResultWithSettingsRequestOption(this.AnalysisSettings, this.RequestOption, this);
        }

        private void PerformResetAnalysis(object dict)
        {
            var gridPage = (UPMGridPage)this.Page;
            if (gridPage != null)
            {
                gridPage.Reset = true;
            }

            this.ParentOrganizerModelController.SetFocusViewVisible(false);
            this.AnalysisSettings = this.AnalysisSettings.Analysis.DefaultExecutionSettings;
            this.Analysis.ComputeResultWithSettingsRequestOption(this.AnalysisSettings, this.RequestOption, this);
        }

        private void PerformShowRecordAction(object dict)
        {
            var dictionary = dict as Dictionary<string, object>;
            var action = dictionary?.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerAnalysisShowRecordAction;
            this.ParentOrganizerModelController.SetFocusViewVisible(false);
            var organizerModelController = UPOrganizerModelController.OrganizerFromDefaultActionOfRecordIdentification(action.RecordIdentification);
            if (organizerModelController != null)
            {
                this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
            }
        }

        private void UpdatePageFromQueryResult(UPCRMResult result)
        {
            this.QueryResult = result;
            UPMGridPage searchPage = (UPMGridPage)this.CreatePageInstance();
            Page oldPage = this.Page;
            int i, j;
            int columnCount = result.ColumnCount;
            StringIdentifier identifier = StringIdentifier.IdentifierWithStringId("columnHeader");
            UPMResultSection section = new UPMResultSection(identifier);
            UPMResultRow columnHeaderListRow = new UPMResultRow(identifier);
            var fieldArray = new List<UPMField>();
            searchPage.FixedFirstColumn = false;
            searchPage.ShowMenu = true;
            searchPage.SumRowAtEnd = false;
            for (i = 0; i < columnCount; i++)
            {
                UPContainerFieldMetaInfo fieldMetaInfo = result.ColumnFieldMetaInfoAtIndex(i);
                string fieldType = fieldMetaInfo.CrmFieldInfo.FieldType;
                if (fieldType == "F" || fieldType == "L" || fieldType == "S")
                {
                    searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(i, UPMColumnDataType.Numeric, false);
                }
                else if (fieldType == "D")
                {
                    searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(i, UPMColumnDataType.Date, true);
                }

                UPMGridColumnHeaderStringField field = new UPMGridColumnHeaderStringField(StringIdentifier.IdentifierWithStringId($"col {i}"))
                {
                    StringValue = result.ColumnNameAtIndex(i)
                };
                fieldArray.Add(field);
            }

            columnHeaderListRow.Fields = fieldArray;
            section.AddResultRow(columnHeaderListRow);
            searchPage.AddResultSection(section);
            int numberOfResultTables = result.NumberOfResultTables;
            List<object> contextMenuOptions = new List<object>(numberOfResultTables);
            var configStore = ConfigurationUnitStore.DefaultStore;
            for (j = 0; j < numberOfResultTables; j++)
            {
                string infoAreaId = result.ResultTableAtIndex(j).InfoAreaId;
                string infoAreaLabel = string.Empty;
                if (infoAreaId?.Length > 0)
                {
                    InfoArea configInfoArea = configStore.InfoAreaConfigById(infoAreaId);
                    UPConfigExpand expand = configStore.ExpandByName(infoAreaId);
                    FieldControl fieldControl = configStore.FieldControlByNameFromGroup("Details", expand.FieldGroupName);
                    if (configInfoArea != null && expand != null && fieldControl != null)
                    {
                        infoAreaLabel = LocalizedString.TextAnalysesShowParam.Replace("%@", configInfoArea.SingularName);
                    }
                }

                contextMenuOptions.Add(infoAreaLabel);
            }

            for (i = 0; i < result.RowCount; i++)
            {
                identifier = StringIdentifier.IdentifierWithStringId($"row {i}");
                var listRow = new UPMResultRow(identifier);
                var crmRow = result.ResultRowAtIndex(i) as UPCRMResultRow;
                fieldArray = new List<UPMField>();
                var v = crmRow.Values();
                for (j = 0; j < v.Count; j++)
                {
                    UPMStringField field2 = new UPMStringField(StringIdentifier.IdentifierWithStringId($"cell{i}_{j}"))
                    {
                        StringValue = v[j],
                        RawStringValue = crmRow.RawValueAtIndex(j)
                    };
                    fieldArray.Add(field2);
                }

                listRow.Fields = fieldArray;
                section.AddResultRow(listRow);
                for (j = 0; j < numberOfResultTables; j++)
                {
                    var label = contextMenuOptions[j] as string;
                    if (label.Length == 0)
                    {
                        continue;
                    }

                    string recordIdentification = crmRow.RecordIdentificationAtIndex(j);
                    if (recordIdentification?.Length > 0)
                    {
                        UPMOrganizerAnalysisShowRecordAction showRecordAction = new UPMOrganizerAnalysisShowRecordAction(StringIdentifier.IdentifierWithStringId($"action.row {i} record {j}"))
                        {
                            RecordIdentification = recordIdentification
                        };
                        showRecordAction.SetTargetAction(this, this.PerformShowRecordAction);
                        showRecordAction.LabelText = label;
                        listRow.AddDetailAction(showRecordAction);
                    }
                }
            }

            this.TopLevelElement = searchPage;
            this.InformAboutDidChangeTopLevelElement(oldPage, searchPage, null, null);
        }

        private void UpdatePageFromResultReset(AnalysisResult result, bool reset)
        {
            this.AnalysisResult = result;
            var searchPage = (UPMGridPage)this.CreatePageInstance();
            var oldPage = this.Page;
            var oldGridPage = (UPMGridPage)this.Page;
            if (reset)
            {
                var resetHeadOption = oldGridPage.ResetHeadOption;
                searchPage.ResetHeadOption = resetHeadOption;
            }

            searchPage.InitialSortColumn = this.AnalysisResult.SortColumn + 1;
            var columnCount = result.Columns.Count;
            var identifier = StringIdentifier.IdentifierWithStringId("columnHeader");
            var section = new UPMResultSection(identifier);
            var columnHeaderListRow = new UPMResultRow(identifier);
            var fieldArray = this.CreateFieldArray();

            var xColumnArray = new List<object>(result.Columns.Count);
            ProcessColumns(result.Columns, searchPage, fieldArray, xColumnArray);

            columnHeaderListRow.Fields = fieldArray;
            this.AddCategoryActionsToRow(columnHeaderListRow);
            section.AddResultRow(columnHeaderListRow);
            searchPage.AddResultSection(section);

            var keyAsRawString = result.Settings.Category.IsExplicitCategory;
            if (keyAsRawString)
            {
                searchPage.SetColumnInfoAtIndexDataTypeSpecialSort(0, UPMColumnDataType.String, true);
            }

            this.ProcessResultRows(result, searchPage, xColumnArray, keyAsRawString, columnCount, section);

            this.AddDrillupActionsFromResultToPage(result, searchPage);
            this.AddCategoriesFromResultToPage(result, searchPage);
            this.UpdateResetHeadOption(searchPage, reset, result);

            this.TopLevelElement = searchPage;
            this.InformAboutDidChangeTopLevelElement(oldPage, searchPage, null, oldGridPage.Reset ? UPChangeHints.ChangeHintsWithHint("ResetAnalysis") : null);
        }

        private List<UPMField> CreateFieldArray()
        {
            var fieldArray = new List<UPMField>();

            var stringField = new UPMStringField(StringIdentifier.IdentifierWithStringId("analysis"))
            {
                StringValue = this.Analysis.Configuration.UnitName
            };
            fieldArray.Add(stringField);

            return fieldArray;
        }

        private void UpdateResetHeadOption(UPMGridPage searchPage, bool reset, AnalysisResult result)
        {
            if (reset && searchPage.ResetHeadOption == null)
            {
                var resetAction = new UPMOrganizerAnalysisBackAction(StringIdentifier.IdentifierWithStringId("action.reset"))
                {
                    AnalysisResult = result
                };
                resetAction.SetTargetAction(this, this.PerformResetAnalysis);
                resetAction.LabelText = LocalizedString.TextAnalysesReset;
                var option = new UPMGridHeadOption(resetAction.LabelText, resetAction)
                {
                    IsReset = true
                };
                searchPage.ResetHeadOption = option;
            }
        }

        private void ProcessResultRows(
            AnalysisResult result,
            UPMGridPage searchPage,
            IReadOnlyList<object> xColumnArray,
            bool keyAsRawString,
            int columnCount,
            UPMResultSection section)
        {
            var i = 0;
            var resultRows = CreateResultRows(result);
            var sumRow = result.SumLine;
            foreach (AnalysisRow row in resultRows)
            {
                var identifier = StringIdentifier.IdentifierWithStringId($"row {i}");
                var listRow = new UPMResultRow(identifier)
                {
                    Context = row
                };
                i++;
                var fieldArray = CreateFieldArray(row, xColumnArray, keyAsRawString, columnCount, i);

                listRow.Fields = fieldArray;
                if (searchPage.FixedSumRow && sumRow != null && i == resultRows.Count && resultRows.Count > 12)
                {
                    searchPage.SumResultRow = listRow;
                    searchPage.SumRowAtEnd = false;
                }
                else
                {
                    section.AddResultRow(listRow);
                }

                if (row.HasDetails)
                {
                    var detailGroupCol1 = new UPMAnalysisResultRowGroup(StringIdentifier.IdentifierWithStringId("det1"))
                    {
                        Invalid = true,
                        Left = true,
                        Row = row
                    };
                    listRow.AddDetailGroup(detailGroupCol1);
                    var detailGroupCol2 = new UPMAnalysisResultRowGroup(StringIdentifier.IdentifierWithStringId("det2"))
                    {
                        Invalid = true,
                        Left = false,
                        Row = row
                    };
                    listRow.AddDetailGroup(detailGroupCol2);
                }

                this.AddDrilldownActionsFromRowToListRow(row, listRow);
                this.AddDetailsActionFromRowToListRow(row, listRow);
            }
        }
    }
}
