// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListResultGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Serdar
// </author>
// <summary>
//   The list result group model controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// List Result Group Model Controller. This class responsible for getting and listing result rows for child records and some other places.
    /// </summary>
    public class UPListResultGroupModelController : UPFieldControlBasedGroupModelController, ISearchOperationHandler, ISwipePageDataSourceController
    {
        private string LinkRecordToken;
        private int CurrentIndex;
        private List<UPContainerFieldMetaInfo> TableCaptionFieldMapping;
        private Dictionary<string, UPCRMResultRow> RowDictionary;
        private bool DetailRecordSwipeEnabled;
        private Dictionary<string, UPCRMField> AlternateExpandFields;
        private UPConfigExpand DefaultExpand;
        private Dictionary<string, object> Parameters;
        private ViewReference CurrentViewReference;
        private bool DisableDetailsAction;
        private string CurrentRecordIdentification;
        private UPConfigFilter CurrentFilter;
        private bool SignalFinished;
        private UPExpandedRowContext ExpandedRowContext;

        private readonly bool _disablePaging;
        private TableCellStyle _cellStyle;

        /// <summary>
        /// Occurs when [search finished event].
        /// </summary>
        public event EventHandler SearchFinishedEvent;

        /// <summary>
        /// Search finished handler.
        /// </summary>
        public void SearchFinished()
        {
            this.SearchFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Link record identification
        /// </summary>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Maximum result count to return. If its zero all records will be returned.
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// SearchList and Config Name
        /// </summary>
        public string SearchAndListConfigurationName { get; private set; }

        /// <summary>
        /// The Query
        /// </summary>
        public UPContainerMetaInfo CrmQuery { get; private set; }

        /// <summary>
        /// Field Control instance
        /// </summary>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Filter Name
        /// </summary>
        public string FilterName { get; private set; }

        /// <summary>
        /// InfoArea Id
        /// </summary>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Default Action
        /// </summary>
        public Menu DefaultAction { get; private set; }

        /// <summary>
        /// Gets the Link Id
        /// </summary>
        public int LinkId { get; private set; }

        /// <summary>
        /// <see cref="UPConfigExpand"/> instance
        /// </summary>
        public UPConfigExpand ExpandMapper { get; set; }

        /// <summary>
        /// <see cref="SearchAndList"/> instance
        /// </summary>
        public SearchAndList SearchAndList { get; private set; }

        /// <summary>
        /// <see cref="UPSearchPageModelControllerPreparedSearch"/> instance
        /// </summary>
        public UPSearchPageModelControllerPreparedSearch PreparedSearch { get; private set; }

        /// <summary>
        /// Dynamic parameters dictionary
        /// </summary>
        public Dictionary<string, object> DynamicParameters { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.CurrentRecordIdentification;

        /// <summary>
        /// Gets a value indicating whether [online only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online only]; otherwise, <c>false</c>.
        /// </value>
        public override bool OnlineOnly => !UPCRMDataStore.DefaultStore.HasOfflineData(this.FieldControl.InfoAreaId);

        /// <summary>
        /// Initializes a new instance of the <see cref="UPListResultGroupModelController"/> class.
        /// </summary>
        /// <param name="searchAndListConfigurationName">Name of the search and list configuration.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="headerSwipeEnabled">if set to <c>true</c> [header swipe enabled].</param>
        /// <param name="cellStyleAsString">The cell style as string.</param>
        /// <param name="disablePaging">if set to <c>true</c> [disable paging].</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPListResultGroupModelController(string searchAndListConfigurationName, int linkId, bool headerSwipeEnabled, string cellStyleAsString, bool disablePaging, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this._disablePaging = disablePaging;
            this.SearchAndListConfigurationName = searchAndListConfigurationName;
            this.LinkId = linkId;
            this.DetailRecordSwipeEnabled = headerSwipeEnabled;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.SearchAndList = configStore.SearchAndListByName(this.SearchAndListConfigurationName);
            if (this.SearchAndList != null)
            {
                this.InfoAreaId = this.SearchAndList.InfoAreaId;
                this.FieldControl = configStore.FieldControlByNameFromGroup("List", this.SearchAndList.FieldGroupName);
                this.DefaultExpand = configStore.ExpandByName(this.SearchAndListConfigurationName);
                this._cellStyle = this.TableCellStyleClassicLayout(cellStyleAsString, this.SearchAndList.FieldGroupName.UpClassicLayoutEnabled());
            }
            else
            {
                this.FieldControl = configStore.FieldControlByNameFromGroup("List", this.SearchAndListConfigurationName);
                this.DefaultExpand = configStore.ExpandByName(this.SearchAndListConfigurationName);
                this._cellStyle = this.TableCellStyleClassicLayout(cellStyleAsString, this.SearchAndListConfigurationName.UpClassicLayoutEnabled());
            }

            this.PreparedSearch = new UPSearchPageModelControllerPreparedSearch(this.InfoAreaId, this.SearchAndListConfigurationName, null);
            this.FieldControl = this.PreparedSearch.CombinedControl;
            if (this.DefaultExpand == null)
            {
                this.DefaultExpand = configStore.ExpandByName(this.InfoAreaId);
            }

            if (this.FieldControl != null)
            {
                if (!string.IsNullOrEmpty(this.SearchAndList?.DefaultAction))
                {
                    this.DefaultAction = configStore.MenuByName(this.SearchAndList.DefaultAction);
                }

                if (this.DefaultAction == null)
                {
                    this.DefaultAction = configStore.MenuByName("SHOWRECORD");
                }
            }

            if (this.DefaultExpand?.AlternateExpands?.Count > 0)
            {
                this.AlternateExpandFields = this.DefaultExpand.FieldsForAlternateExpands(true);
                List<UPCRMField> additionalFields = new List<UPCRMField>();

                foreach (var field in this.AlternateExpandFields.Values)
                {
                    if (this.CrmQuery?.ContainsField(field) == null)
                    {
                        additionalFields.Add(field);
                    }
                }

                if (additionalFields.Count > 0)
                {
                    this.CrmQuery?.AddCrmFields(additionalFields);
                }

                this.ExpandMapper = this.DefaultExpand.ExpandCheckerForCrmQuery(this.CrmQuery);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPListResultGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPListResultGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : this(_constructorParameters(formItem)[0], -1, false, _constructorParameters(formItem)[1], _constructorParameters(formItem)[2].ToBoolWithDefaultValue(false), theDelegate)
        {
            string searchAndListName = formItem.ViewReference.ContextValueForKey("ConfigName");
            if (!string.IsNullOrEmpty(searchAndListName))
            {
                this.SearchAndList = ConfigurationUnitStore.DefaultStore.SearchAndListByName(searchAndListName);
            }

            if (this.SearchAndList != null)
            {
                // string listStyleAsString = formItem.Options["ListStyle"] as string;
                // bool disablePaging = ((string)formItem.Options["DisablePaging"]).ToBoolWithDefaultValue(false);

                this.FormItem = formItem;
                this.LinkRecordToken = formItem.ViewReference.ContextValueForKey("LinkRecord");
                if (!string.IsNullOrEmpty(this.LinkRecordToken) && !this.LinkRecordToken.Contains("$"))
                {
                    this.LinkRecordIdentification = this.LinkRecordToken;
                    this.LinkRecordToken = string.Empty;
                }

                this.InfoAreaId = this.SearchAndList.InfoAreaId;
                this.FilterName = formItem.ViewReference.ContextValueForKey("FilterName");
                if (this.CrmQuery != null && !string.IsNullOrEmpty(this.FilterName))
                {
                    UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.FilterName);
                    if (filter != null)
                    {
                        this.CrmQuery.ApplyFilter(filter);
                    }
                }

                this.PreparedSearch = new UPSearchPageModelControllerPreparedSearch(this.InfoAreaId, searchAndListName, null);
                this.FieldControl = this.PreparedSearch.CombinedControl;
                string strOption = formItem.Options?.ValueOrDefault("MaxResults") as string;
                this.MaxResults = Convert.ToInt32(strOption);

                string strOption2 = formItem.Options?.ValueOrDefault("RequestOption") as string;
                if (!string.IsNullOrEmpty(strOption2))
                {
                    this.RequestOption = UPCRMDataStore.RequestOptionFromString(strOption2, this.RequestOption);
                }

                string swipeDetailRecordsString = formItem.Options?.ValueOrDefault("SwipeDetailRecords") as string;
                this.DetailRecordSwipeEnabled = !string.IsNullOrEmpty(swipeDetailRecordsString)
                    ? swipeDetailRecordsString.ToBoolWithDefaultValue(false)
                    : ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("View.RecordSwipeEnabledDefault", true);

                this.ExplicitTabIdentifier = identifier;
                this.ExplicitLabel = this.FormItem.Label;
                this.AddDependingKeysFromViewReference(formItem.ViewReference);
            }
        }

        // TODO: Find a way to simplify these stuff.
        /// <summary>
        /// Static Method for creating custom constructor parameters.
        /// </summary>
        static string[] _constructorParameters(FormItem formItem)
        {
            var parameterList = new string[3];

            parameterList[0] = formItem.ViewReference.ContextValueForKey("ConfigName");
            parameterList[1] = formItem.Options?.ValueOrDefault("ListStyle") as string;
            parameterList[2] = formItem.Options?.ValueOrDefault("DisablePaging") as string;

            return parameterList;
        }

        private UPContainerMetaInfo BuildCrmQuery()
        {
            List<UPConfigFilter> filterArray = new List<UPConfigFilter>();
            Dictionary<string, object> filterParameters;
            if (this.Parameters?.Count > 0)
            {
                if (this.DynamicParameters?.Count > 0)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>(this.Parameters);
                    foreach (var item in this.DynamicParameters)
                    {
                        dict.Add(item.Key, item.Value);
                    }

                    filterParameters = dict;
                }
                else
                {
                    filterParameters = this.Parameters;
                }
            }
            else
            {
                filterParameters = this.DynamicParameters;
            }

            if (!string.IsNullOrWhiteSpace(this.SearchAndList.FilterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.SearchAndList.FilterName);
                if (filter != null && filterParameters?.Count > 0)
                {
                    filter = filter.FilterByApplyingValueDictionaryDefaults(filterParameters, true);
                    filterArray.Add(filter);
                }
            }

            this.PreparedSearch.FilterParameter = filterParameters;
            this.CrmQuery = this.PreparedSearch.CrmQueryForValue(string.Empty, filterArray, false);
            if (this.CrmQuery != null)
            {
                List<UPCRMField> additionalFields = new List<UPCRMField>();
                if (this.AlternateExpandFields != null)
                {
                    additionalFields.AddRange(this.AlternateExpandFields.Values.Where(field => this.CrmQuery.ContainsField(field) == null));
                }

                if (additionalFields.Count > 0)
                {
                    this.CrmQuery.AddCrmFields(additionalFields);
                }

                this.ExpandMapper = this.DefaultExpand.ExpandCheckerForCrmQuery(this.CrmQuery);
                this.CurrentFilter = null;
                if (this.CrmQuery != null && !string.IsNullOrEmpty(this.FilterName))
                {
                    UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(this.FilterName);
                    if (filter != null)
                    {
                        if (filterParameters?.Count > 0)
                        {
                            filter = filter.FilterByApplyingValueDictionaryDefaults(filterParameters, true);
                            this.CurrentFilter = filter;
                        }

                        this.CrmQuery.ApplyFilter(filter);
                    }
                }

                if (this.DetailRecordSwipeEnabled)
                {
                    string tableCaptionName = this.DefaultExpand.TableCaptionName;
                    if (string.IsNullOrEmpty(tableCaptionName))
                    {
                        tableCaptionName = this.FieldControl.InfoAreaId;
                    }

                    UPConfigTableCaption tableCaption = ConfigurationUnitStore.DefaultStore.TableCaptionByName(tableCaptionName);
                    this.TableCaptionFieldMapping = tableCaption?.ResultFieldMapFromMetaInfo(this.CrmQuery);
                }
            }

            return this.CrmQuery;
        }

        /// <summary>
        /// Groups from result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected virtual UPMGroup GroupFromResult(UPCRMResult result)
        {
            this.RowDictionary = new Dictionary<string, UPCRMResultRow>();

            var count = result.RowCount;
            var moreResultsExist = false;
            this.ClearEmptyGroup();

            if (this.MaxResults > 0 && count > this.MaxResults)
            {
                count = this.MaxResults;
                moreResultsExist = true;
            }

            if (count > 0)
            {
                var listGroup = this.HandleGroup(moreResultsExist, count, result);

                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = listGroup;
                return listGroup;
            }

            this.HandleEmptyGroup();
            return this.Group;
        }

        private UPMListGroup HandleGroup(bool moreResultsExist, int count, UPCRMResult result)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var disableColorOnListResults = configStore.ConfigValueIsSet("ListGroup.NoColor");
            var disableVirtualInfoAreas = configStore.ConfigValueIsSet("ListGroup.NoVirtualInfoAreas");

            UPMAction switchToListOrganizerAction = null;
            if (moreResultsExist)
            {
                switchToListOrganizerAction = new UPMAction(null)
                {
                    IconName = "arrow.png",
                    LabelText = LocalizedString.TextMoreData
                };
                switchToListOrganizerAction.SetTargetAction(this, this.SwitchToListOrganizer);
            }

            var listGroup = new UPMListGroup(this.ExplicitTabIdentifier, switchToListOrganizerAction)
            {
                LabelText = this.TabLabel,
                MaxResults = this.MaxResults,
                DisablePaging = this._disablePaging
            };

            AureaColor color = null;
            var infoAreaConfig = configStore.InfoAreaConfigById(this.FieldControl.InfoAreaId);
            if (!disableColorOnListResults && !string.IsNullOrEmpty(infoAreaConfig.ColorKey))
            {
                color = AureaColor.ColorWithString(infoAreaConfig.ColorKey);
            }

            var listFormatter = new UPCRMListFormatter(this.FieldControl);
            UPCRMListFormatter miniDetailsFormatter = null;
            var miniDetailTab = this.FieldControl.TabAtIndex(1);
            if (miniDetailTab != null)
            {
                miniDetailsFormatter = new UPCRMListFormatter(miniDetailTab);
            }

            for (var i = 0; i < count; i++)
            {
                var row = result.ResultRowAtIndex(i) as UPCRMResultRow;
                if (row == null)
                {
                    throw new InvalidOperationException("UPCRMResult must contain UPCRMResultRow");
                }

                var listRow = this.GetListRow(
                    row,
                    disableVirtualInfoAreas,
                    disableColorOnListResults,
                    configStore,
                    color,
                    miniDetailsFormatter,
                    listFormatter);

                listGroup.AddListRow(listRow);
                this.RowDictionary.SetObjectForKey(row, listRow.Key);
            }

            return listGroup;
        }

        private UPMListRow GetListRow(
            UPCRMResultRow row,
            bool disableVirtualInfoAreas, 
            bool disableColorOnListResults,
            IConfigurationUnitStore configStore, 
            AureaColor color,
            UPCRMListFormatter miniDetailsFormatter,
            UPCRMListFormatter listFormatter)
        {
            var identifier = new RecordIdentifier(this.FieldControl.InfoAreaId, row.RecordIdentificationAtIndex(0).RecordId());
            var listRow = new UPMListRow(identifier)
            {
                OnlineData = !row.HasLocalCopy
            };
            var virtualInfoAreaId = this.GetVirtualInfoAreaId(disableVirtualInfoAreas, row);

            var currentColorKey = this.GetColorKey(virtualInfoAreaId, configStore, disableColorOnListResults, row, out var currentImageName, out var currentExpand);
            if (!string.IsNullOrEmpty(currentColorKey))
            {
                listRow.RowColor = AureaColor.ColorWithString(currentColorKey);
            }
            else if (color != null)
            {
                listRow.RowColor = color;
            }

            var miniDetailGroup = new UPMGroup(
                FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(
                    this.FieldControl.InfoAreaId, 
                    row.RecordIdentificationAtIndex(0).RecordId(),
                    "DetailField_Left")
                );
            if (miniDetailsFormatter != null)
            {
                listRow.AddDetailGroup(miniDetailGroup);
            }

            this.UpdateMiniDetailGroup(miniDetailGroup, miniDetailsFormatter, row, identifier);

            this.AddRowActionsresultRowRecordIdCurrentExpand(listRow, row.RecordIdentificationAtIndex(0), currentExpand);
            this.UpdateListRow(listRow, listFormatter, row, identifier);

            var switchToOrganizerAction = this.CreateUPMAction();
            listRow.RowAction = switchToOrganizerAction;
            return listRow;
        }

        private string GetVirtualInfoAreaId(bool disableVirtualInfoAreas, UPCRMResultRow row)
        {
            string virtualInfoAreaId = null;
            if (!disableVirtualInfoAreas)
            {
                virtualInfoAreaId = row.VirtualInfoAreaIdAtIndex(0);
                var physicalInfoAreaId = row.PhysicalInfoAreaIdAtIndex(0);
                if (virtualInfoAreaId == physicalInfoAreaId)
                {
                    virtualInfoAreaId = null;
                }
            }

            return virtualInfoAreaId;
        }

        private string GetColorKey(
            string virtualInfoAreaId,
            IConfigurationUnitStore configStore,
            bool disableColorOnListResults,
            UPCRMResultRow row,
            out string currentImageName,
            out UPConfigExpand currentExpand)
        {
            string currentColorKey = null;
            currentImageName = null;
            currentExpand = null;
            if (!string.IsNullOrEmpty(virtualInfoAreaId))
            {
                var virtualInfoAreaConfig = configStore.InfoAreaConfigById(virtualInfoAreaId);
                currentExpand = configStore.ExpandByName(virtualInfoAreaId);
                if (virtualInfoAreaConfig != null)
                {
                    currentImageName = virtualInfoAreaConfig.ImageName;
                    if (!disableColorOnListResults)
                    {
                        currentColorKey = virtualInfoAreaConfig.ColorKey;
                    }
                }
            }
            else if (this.ExpandMapper != null)
            {
                currentExpand = this.ExpandMapper.ExpandForResultRow(row);
                currentImageName = currentExpand.ImageName;
                if (!disableColorOnListResults)
                {
                    currentColorKey = currentExpand.ColorKey;
                }
            }

            return currentColorKey;
        }

        private UPMAction CreateUPMAction()
        {
            var switchToOrganizerAction = new UPMAction(null);
            if (this.DisableDetailsAction)
            {
                if (!string.IsNullOrEmpty(this.ValueName))
                {
                    switchToOrganizerAction.SetTargetAction(this, this.InformAboutChangeValue);
                }
            }
            else
            {
                switchToOrganizerAction.IconName = "arrow.png";
                switchToOrganizerAction.SetTargetAction(this, this.SwitchToOrganizer);
            }

            return switchToOrganizerAction;
        }

        private void UpdateMiniDetailGroup(
            UPMGroup miniDetailGroup,
            UPCRMListFormatter miniDetailsFormatter,
            UPCRMResultRow row,
            RecordIdentifier identifier)
        {
            var miniDetailsFieldCount = miniDetailsFormatter?.PositionCount ?? 0;
            for (var fieldCount = 0; fieldCount < miniDetailsFieldCount; fieldCount++)
            {
                var configField = miniDetailsFormatter?.FirstFieldForPosition(fieldCount);
                var fieldIdentifier = identifier.IdentifierWithFieldId(configField != null ? $"{configField.FieldId}" : "0");

                var attributes = configField?.Attributes;
                var field = new UPMStringField(fieldIdentifier)
                {
                    Hidden = attributes.Hide
                };
                var stringValue = miniDetailsFormatter?.StringFromRowForPosition(row, fieldCount);
                if (!attributes.Image)
                {
                    var firstField = miniDetailsFormatter?.FirstFieldForPosition(fieldCount);
                    if (!string.IsNullOrEmpty(firstField?.Label))
                    {
                        if (firstField.Attributes.NoLabel && !string.IsNullOrEmpty(stringValue))
                        {
                            stringValue = $"{firstField.Label} {stringValue}";
                        }
                        else
                        {
                            field.LabelText = firstField.Label;
                        }
                    }

                    field.StringValue = stringValue;
                    SetAttributesOnField(miniDetailsFormatter?.FirstFieldForPosition(fieldCount).Attributes, field);
                    miniDetailGroup.AddField(field);
                }
            }
        }

        private void UpdateListRow(
            UPMListRow listRow,
            UPCRMListFormatter listFormatter,
            UPCRMResultRow row,
            RecordIdentifier identifier)
        {
            const int maxFields = 6;
            for (var fieldCount = 0; fieldCount < maxFields; fieldCount++)
            {
                var configField = listFormatter.FirstFieldForPosition(fieldCount);

                var fieldIdentifier = identifier.IdentifierWithFieldId(configField != null ? $"{configField.FieldId}" : "0");

                var attributes = configField?.Attributes;
                if (configField == null && this._cellStyle != TableCellStyle.Classic)
                {
                    continue;
                }

                var field = new UPMStringField(fieldIdentifier)
                {
                    Hidden = attributes.Hide
                };
                var stringValue = listFormatter.StringFromRowForPosition(row, fieldCount);
                if (attributes.Image)
                {
                    var documentManager = new DocumentManager();
                    var documentKey = stringValue;
                    var documentData = documentManager.DocumentForKey(documentKey);
                    listRow.RecordImageDocument = documentData != null
                        ? new UPMDocument(documentData)
                        : new UPMDocument(fieldIdentifier, ServerSession.CurrentSession.DocumentRequestUrlForDocumentKey(documentKey));

                    continue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        var firstField = listFormatter.FirstFieldForPosition(fieldCount);
                        if (firstField != null && firstField.Attributes.NoLabel && !string.IsNullOrEmpty(firstField?.Label))
                        {
                            stringValue = $"{firstField.Label} {stringValue}";
                        }

                        field.StringValue = stringValue;
                        SetAttributesOnField(listFormatter.FirstFieldForPosition(fieldCount).Attributes, field);
                    }

                    field.IsRowField = true;
                    listRow.AddField(field);
                }
            }
        }

        /// <summary>
        /// Adds row actions by given <see cref="UPMListRow" />
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="currentExpand">The current expand.</param>
        void AddRowActionsresultRowRecordIdCurrentExpand(UPMListRow resultRow, string recordId, UPConfigExpand currentExpand)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigHeader actionHeader;
            if (currentExpand != null)
            {
                actionHeader = ConfigurationUnitStore.DefaultStore.HeaderByNameFromGroup("ExpandOptions", currentExpand.HeaderGroupName);
            }
            else
            {
                actionHeader = ConfigurationUnitStore.DefaultStore.HeaderByNameFromGroup("ExpandOptions", this.SearchAndList.HeaderGroupName);
            }

            if (actionHeader != null)
            {
                UPOrganizerModelController targetController = null;
                foreach (string buttonName in actionHeader.ButtonNames)
                {
                    UPMOrganizerAction action = null;
                    UPConfigButton buttonDef = configStore.ButtonByName(buttonName);
                    if (buttonDef.IsHidden)
                    {
                        continue;
                    }

                    string iconName = string.Empty;
                    if (!string.IsNullOrEmpty(buttonDef.ImageName))
                    {
                        iconName = configStore.FileNameForResourceName(buttonDef.ImageName) + "@dark";
                    }

                    if (buttonName.Contains("GroupStart") || buttonName.Contains("GroupEnd"))
                    {
                        continue;
                    }

                    if (buttonDef.ViewReference != null)
                    {
                        action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId($"action.{buttonName}"));
                        action.SetTargetAction(targetController, this.PerformAction);
                        ViewReference viewReference = buttonDef.ViewReference.ViewReferenceWith(recordId);
                        viewReference = viewReference.ViewReferenceWith(new Dictionary<string, string> { { "1", ".fromPopup" } });
                        action.ViewReference = viewReference;
                        if (!string.IsNullOrEmpty(iconName))
                        {
                            action.IconName = iconName;
                        }

                        if (action.Identifier.Equals(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                        {
                            action = null;
                        }
                        else
                        {
                            action.LabelText = buttonDef.Label;
                        }
                    }

                    if (action != null)
                    {
                        resultRow.AddDetailAction(action);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the quick actions for row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public override void FindQuickActionsForRow(UPMListRow resultRow)
        {
            if (this.AlternateGroupModelControllerActive)
            {
                this.AlternateGroupModelController.FindQuickActionsForRow(resultRow);
                return;
            }

            var actions = resultRow.DetailActions;
            this.ExpandedRowContext = new UPExpandedRowContext(resultRow);
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    action.Invalid = false;
                    if (!action.Identifier.Equals(StringIdentifier.IdentifierWithStringId("startNavigation")))
                    {
                        action.SetTargetAction(this, this.PerformAction);
                    }
                }
            }
        }

        /// <summary>
        /// Applies LinkRecord Identification and returns <see cref="UPMGroup" />
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPMGroup ApplyLinkRecordIdentification(string recordIdentification)
        {
            if (recordIdentification?.Length > 9 && this.FormItem != null
                && this.FormItem.Options.ContainsKey("checkInfoAreaId") && Convert.ToInt32(this.FormItem.Options["checkInfoAreaId"]) != 0)
            {
                var multiRecords = recordIdentification.Split(',');
                if (multiRecords.Length > 0)
                {
                    recordIdentification = multiRecords.FirstOrDefault(rec => rec.InfoAreaId() == this.InfoAreaId);
                }

                if (string.IsNullOrEmpty(recordIdentification))
                {
                    this.HandleEmptyGroup();
                    return this.Group;
                }
            }

            this.CrmQuery = this.BuildCrmQuery();
            if (this.CrmQuery == null)
            {
                this.ControllerState = GroupModelControllerState.Error;
                return null;
            }

            if (this.DetailRecordSwipeEnabled)
            {
                var tableCaptionName = this.DefaultExpand.TableCaptionName;
                if (!string.IsNullOrEmpty(tableCaptionName))
                {
                    tableCaptionName = this.FieldControl.InfoAreaId;
                }

                UPConfigTableCaption tableCaption = ConfigurationUnitStore.DefaultStore.TableCaptionByName(tableCaptionName);
                this.TableCaptionFieldMapping = tableCaption?.ResultFieldMapFromMetaInfo(this.CrmQuery);
            }

            this.LinkRecordIdentification = recordIdentification;
            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                this.CrmQuery.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
            }

            if (this.MaxResults > 0)
            {
                this.CrmQuery.MaxResults = this.MaxResults;
            }

            //this.Group = null;
            this.SignalFinished = false;
            this.ControllerState = GroupModelControllerState.Pending;
            Operation remoteOperation = this.CrmQuery.Find(UPRequestOption.Online, this);
            if (remoteOperation == null)
            {
                this.ControllerState = GroupModelControllerState.Error;
                return null;
            }
            else
            {
                bool waitTillFinishLoading = Convert.ToBoolean(ConfigurationUnitStore.DefaultStore.ConfigValueDefaultValue("Dashboard.WaitTillFinishLoad", "true"));
                if(waitTillFinishLoading && this.Delegate.GetType() == typeof(DashboardPageModelController))
                {
                    //Thread.Sleep(5000);
                }
            }

            this.SignalFinished = true;
            return this.Group;
        }

        /// <summary>
        /// Applies <see cref="UPCRMResultRow" /> and returns <see cref="UPMGroup" />
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            base.ApplyResultRow(row);
            var group = this.ApplyLinkRecordIdentification(row.RootRecordIdentification);
            group.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification));
            return group;
        }

        /// <summary>
        /// Applies Context dictionary and returns <see cref="UPMGroup" />
        /// </summary>
        /// <param name="contextDictionary">The context dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> contextDictionary)
        {
            base.ApplyContext(contextDictionary);
            this.AlternateGroupModelControllerActive = false;
            this.CurrentViewReference = this.FormItem.ViewReference;
            var paramSource = this.FormItem.ViewReference.ContextValueForKey("Param1");
            if (!paramSource.Contains("$") || paramSource.Contains("$par"))
            {
                paramSource = null;
            }

            if (paramSource != null && contextDictionary.ContainsKey(paramSource))
            {
                this.DynamicParameters = this.DataFromValueName(paramSource.Substring(1));
                if (this.DynamicParameters?.Count > 0 && this.DynamicParameters?.ValueOrDefault(".empty") != null)
                {
                    var dict = new Dictionary<string, object>(contextDictionary);
                    dict.SetObjectForKey(paramSource, paramSource);
                    contextDictionary = dict;
                }
                else
                {
                    this.DynamicParameters = null;
                }
            }
            else
            {
                this.DynamicParameters = null;
            }

            if (this.DynamicParameters?.Count > 0 && this.DynamicParameters?.ValueOrDefault("RemoveUnboundParameters") == null)
            {
                var d = new Dictionary<string, object>(this.DynamicParameters);
                d["RemoveUnboundParameters"] = "true";
                this.DynamicParameters = d;
            }

            this.CurrentViewReference = new ViewReference(this.CurrentViewReference, contextDictionary, true);
            if (this.CurrentViewReference == null)
            {
                this.Group = null;
                this.ControllerState = GroupModelControllerState.Empty;
                return null;
            }

            this.DisableDetailsAction = Convert.ToInt32(this.FormItem.Options.ValueOrDefault("NoDetails")) != 0;
            this.FilterName = this.CurrentViewReference.ContextValueForKey("FilterName");
            this.Parameters = this.CurrentViewReference.ParamsDictionary();

            return this.ApplyLinkRecordIdentification(!string.IsNullOrEmpty(this.LinkRecordToken)
                ? contextDictionary[this.LinkRecordToken] as string
                : this.LinkRecordIdentification);
        }

        /// <summary>
        /// Performs action
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void SwitchToListOrganizer(object sender)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary["ConfigName"] = this.SearchAndListConfigurationName;
            dictionary["InfoArea"] = this.FieldControl.InfoAreaId;

            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                dictionary["LinkRecord"] = this.LinkRecordIdentification;
            }

            if (this.LinkId > 0)
            {
                dictionary["LinkId"] = this.LinkId.ToString();
            }

            if (this.CurrentFilter != null)
            {
                dictionary["FilterObject"] = this.CurrentFilter;
            }
            else if (!string.IsNullOrEmpty(this.FilterName))
            {
                dictionary["FilterName"] = this.FilterName;
            }

            if (this.DetailRecordSwipeEnabled)
            {
                dictionary["SwipeDetailRecords"] = "true";
            }

            var initialRquestOptionString = UPCRMDataStore.StringFromRequestOption(this.RequestOption);
            if (!string.IsNullOrEmpty(initialRquestOptionString))
            {
                dictionary["InitialRequestOption"] = initialRquestOptionString;
            }

            this.Delegate?.PerformOrganizerAction(this, new ViewReference(dictionary, "RecordListView"));
        }

        /// <summary>
        /// Informs delegate about changed value
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void InformAboutChangeValue(object sender)
        {
            RecordIdentifier identifier = (RecordIdentifier)((UPMListRow)sender).Identifier;
            this.CurrentRecordIdentification = identifier.RecordIdentification;
            this.Delegate.GroupModelControllerValueChanged(this, this.CurrentRecordIdentification);
        }

        private TableCellStyle TableCellStyleClassicLayout(string cellStyleAsString, bool classicLayout)
        {
            switch (cellStyleAsString)
            {
                case "rowOnly":
                    return TableCellStyle.RowOnly;

                case "row":
                    return TableCellStyle.Row;

                case "card":
                    return TableCellStyle.Card;

                case "card23":
                    return TableCellStyle.Card23;

                case "card2Only":
                    return TableCellStyle.Card2Only;

                case "classic":
                    return TableCellStyle.Classic;
            }

            return classicLayout ? TableCellStyle.Classic : TableCellStyle.Row;
        }

        /// <summary>
        /// Performs action
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void SwitchToOrganizer(object sender)
        {
            UPMListRow listRow = sender as UPMListRow;
            RecordIdentifier identifier = listRow.Identifier as RecordIdentifier;
            ViewReference viewReference = this.DefaultAction.ViewReference.ViewReferenceWith(identifier.RecordIdentification);
            this.CurrentIndex = this.Group.Children.IndexOf((UPMElement)listRow);
            UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(viewReference);
            organizerModelController.OnlineData = listRow.OnlineData;
            if (this.TableCaptionFieldMapping != null && this.DetailRecordSwipeEnabled && this.Group.Children.Count > 1)
            {
                UPSearchResultCachingSwipeRecordController cachingRecordController = new UPSearchResultCachingSwipeRecordController(this);
                cachingRecordController.BuildCache(identifier);
                organizerModelController.ParentSwipePageRecordController = cachingRecordController;
            }
            else
            {
                if (this.DetailRecordSwipeEnabled && this.Group.Children.Count > 1)
                {
                    SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"HeaderSwipe for unit {viewReference.Name} disabled Table Caption could not be determined");
                }
            }

            this.Delegate?.TransitionToContentModelController(organizerModelController);
        }

        void StopProgressHud()
        {
            // TODO: This is after dashboard
            /* if (Delegate.GetType() == typeof(UPDashboardPageModelController))
             {
                 UPDashboardPageModelController dashboard = (UPDashboardPageModelController)Delegate;
                 object viewController = dashboard.ModelControllerDelegate();
                 viewController.PerformSelectorOnMainThreadWithObjectWaitUntilDone(@selector(hideProgressHud), null, false);
             }*/
        }

        /// <summary>
        /// Loads table captions
        /// </summary>
        /// <param name="fromIndex">The from index.</param>
        /// <param name="toIndex">The to index.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public List<UPSwipePageRecordItem> LoadTableCaptionsFromIndexToIndex(int fromIndex, int toIndex)
        {
            List<UPSwipePageRecordItem> result = new List<UPSwipePageRecordItem>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            ICRMDataStore crmStore = UPCRMDataStore.DefaultStore;
            Dictionary<string, List<UPContainerFieldMetaInfo>> tableCaptionMappingDictionary = new Dictionary<string, List<UPContainerFieldMetaInfo>>();
            for (int index = fromIndex; index < toIndex; index++)
            {
                UPMListRow listRow = this.ResultRowAtIndexOffset(index);
                if (listRow != null)
                {
                    RecordIdentifier recordIdentifier = (RecordIdentifier)listRow.Identifier;
                    UPConfigExpand expandConfig = configStore.ExpandByName(recordIdentifier.InfoAreaId);
                    UPCRMResultRow crmRow = this.RowDictionary[listRow.Key];
                    string tableCaptionName = expandConfig.TableCaptionName;
                    if (string.IsNullOrEmpty(tableCaptionName))
                    {
                        tableCaptionName = recordIdentifier.InfoAreaId;
                    }

                    UPConfigTableCaption tableCaption = configStore.TableCaptionByName(tableCaptionName);
                    List<UPContainerFieldMetaInfo> tableCaptionFieldMapping = new List<UPContainerFieldMetaInfo>();

                    if (tableCaption != null)
                    {
                        tableCaptionFieldMapping = tableCaptionMappingDictionary.ValueOrDefault(tableCaption.UnitName);
                        if (tableCaptionFieldMapping == null)
                        {
                            tableCaptionFieldMapping = tableCaption.ResultFieldMapFromMetaInfo(this.CrmQuery);
                            if (tableCaptionFieldMapping != null)
                            {
                                tableCaptionMappingDictionary.SetObjectForKey(tableCaptionFieldMapping, tableCaption.UnitName);
                            }
                        }
                    }

                    if (tableCaptionFieldMapping != null && tableCaption != null)
                    {
                        string recordTableCaption = tableCaption.TableCaptionForResultRow(crmRow, tableCaptionFieldMapping);
                        UPSwipePageRecordItem item = new UPSwipePageRecordItem(recordTableCaption, crmStore.TableInfoForInfoArea(recordIdentifier.InfoAreaId).Label, recordIdentifier.RecordIdentification, false);
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an <see cref="UPOrganizerModelController" /> instance by given Record Identification
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="onlineData">The online data.</param>
        /// <returns>
        /// The <see cref="UPOrganizerModelController" />.
        /// </returns>
        public UPOrganizerModelController DetailOrganizerForRecordIdentification(string recordIdentification, bool onlineData)
        {
            RecordIdentifier identifier = new RecordIdentifier(recordIdentification);
            ViewReference viewReference = this.DefaultAction.ViewReference.ViewReferenceWith(identifier.RecordIdentification);
            UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(viewReference);
            return organizerModelController;
        }

        /// <summary>
        /// Returns <see cref="UPMListRow" /> at given index
        /// </summary>
        /// <param name="indexOffset">The index offset.</param>
        /// <returns></returns>
        public UPMListRow ResultRowAtIndexOffset(int indexOffset)
        {
            int index = this.CurrentIndex + indexOffset;
            if (index >= 0 && index < this.Group.Children.Count)
            {
                return (UPMListRow)this.Group.Children[index];
            }

            return null;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void PerformAction(object actionDictionary)
        {
            UPMultipleOrganizerManager.CurrentOrganizerManager.ModelControllerOfCurrentOrganizer
                .PerformAction(actionDictionary as Dictionary<string, object>, null);
        }

        /// <summary>
        /// Search operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if ((this.RequestOption == UPRequestOption.BestAvailable || this.RequestOption == UPRequestOption.FastestAvailable)
                && error.IsConnectionOfflineError())
            {
                UPCRMResult result = this.CrmQuery.Find();
                if (result.RowCount > 0)
                {
                    this.ControllerState = GroupModelControllerState.Finished;
                    this.Group = this.GroupFromResult(result);
                }
                else
                {
                    this.Group = null;
                    this.ControllerState = GroupModelControllerState.Empty;
                    if (!this.HandleEmptyGroup())
                    {
                        return;
                    }
                }
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Error;
                this.Error = error;
                this.Group = null;
            }

            if (this.SignalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <summary>
        /// Search operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount > 0)
            {
                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = this.GroupFromResult(result);
                this.ControllerState = this.Group != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
            }
            else
            {
                if (!this.HandleEmptyGroup())
                {
                    return;
                }
            }

            if (this.SignalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }

            this.SearchFinished();
        }

        /// <summary>
        /// Search operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
