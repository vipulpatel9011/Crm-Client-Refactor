// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCharacteristics.cs" company="Aurea Software Gmbh">
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
//   UPCharacteristics
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Characteristics
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The field group string
        /// </summary>
        public const string FieldGroupString = "Group";

        /// <summary>
        /// The field item string
        /// </summary>
        public const string FieldItemString = "Item";

        /// <summary>
        /// The field single string
        /// </summary>
        public const string FieldSingleString = "Single";

        /// <summary>
        /// The field show additional fields string
        /// </summary>
        public const string FieldShowAdditionalFieldsString = "ShowAdditionalFields";

        /// <summary>
        /// The field show group expanded string
        /// </summary>
        public const string FieldShowGroupExpandedString = "ShowGroupExpanded";
    }

    /// <summary>
    /// UPCharacteristics
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class UPCharacteristics : ISearchOperationHandler, UPCRMLinkReaderDelegate
    {
        private Dictionary<string, object> sourceFieldDictionary;
        private Dictionary<string, UPCharacteristicsGroup> groupDict;
        private UPOfflineCharacteristicsRequest offlineRequest;
        private UPOfflineCharacteristicsRequest conflictOfflineRequest;
        private UPContainerMetaInfo crmQuery;
        private uint currentQueryType;
        private UPCRMLinkReader linkReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristics"/> class.
        /// </summary>
        /// <param name="offlineRequest">The offline request.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="parameters">The parameters.</param>
        public UPCharacteristics(UPOfflineCharacteristicsRequest offlineRequest, string recordIdentification, Dictionary<string, object> parameters)
            : this(recordIdentification, parameters, true)
        {
            this.conflictOfflineRequest = offlineRequest;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristics"/> class.
        /// </summary>
        /// <param name="rootRecordIdentification">The root record identification.</param>
        /// <param name="searchAndListConfigurationName">Name of the search and list configuration.</param>
        /// <param name="destinationRequestOption">The destination request option.</param>
        public UPCharacteristics(string rootRecordIdentification, string searchAndListConfigurationName, UPRequestOption destinationRequestOption)
        {
            this.RecordIdentification = rootRecordIdentification;
            this.EditMode = false;
            SearchAndList searchAndListConfiguration = ConfigurationUnitStore.DefaultStore.SearchAndListByName(searchAndListConfigurationName);
            if (searchAndListConfiguration != null)
            {
                this.DestinationFieldControlName = searchAndListConfiguration.FieldGroupName;
                this.DestinationFilterName = searchAndListConfiguration.FilterName;
            }
            else
            {
                this.DestinationFieldControlName = searchAndListConfigurationName;
            }

            this.DestinationRequestOption = destinationRequestOption;
            if (!this.SetControlsFromParameters())
            {
                throw new InvalidOperationException("Failed: SetControlsFromParameters");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristics"/> class.
        /// </summary>
        /// <param name="rootRecordIdentification">The root record identification.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="destinationRequestOption">The destination request option.</param>
        public UPCharacteristics(string rootRecordIdentification, FieldControl fieldControl, UPRequestOption destinationRequestOption)
        {
            this.RecordIdentification = rootRecordIdentification;
            this.DestinationFieldControl = fieldControl;
            this.DestinationRequestOption = destinationRequestOption;

            if (!this.SetControlsFromParameters())
            {
                throw new InvalidOperationException("Failed: SetControlsFromParameters");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristics"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="editMode">if set to <c>true</c> [edit mode].</param>
        public UPCharacteristics(string recordIdentification, Dictionary<string, object> parameters, bool editMode)
        {
            this.Initialize(recordIdentification, parameters, editMode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristics"/> class.
        /// </summary>
        /// <param name="rootRecordIdentification">The root record identification.</param>
        /// <param name="contextName">Name of the context.</param>
        /// <exception cref="Exception">ConfigMenu '{contextName}'</exception>
        public UPCharacteristics(string rootRecordIdentification, string contextName)
        {
            Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(contextName);
            if (menu == null)
            {
                throw new InvalidOperationException($"ConfigMenu '{contextName}' not found.");
            }

            ViewReference viewReferenceContect = menu.ViewReference;
            viewReferenceContect = viewReferenceContect.ViewReferenceWith(rootRecordIdentification);
            this.Initialize(rootRecordIdentification, new Dictionary<string, object> { { "viewReference", viewReferenceContect } }, false);
        }

        private void Initialize(string recordIdentification, Dictionary<string, object> parameters, bool editMode)
        {
            this.RecordIdentification = recordIdentification;
            this.ViewReference = (ViewReference)parameters["viewReference"];
            this.DestinationFieldControlName = this.ViewReference.ContextValueForKey("DestinationFieldGroup");
            this.GroupSearchAndListControlName = this.ViewReference.ContextValueForKey("GroupSearchAndList");
            this.ItemSearchAndListControlName = this.ViewReference.ContextValueForKey("ItemSearchAndList");
            this.SourceFieldControlName = this.ViewReference.ContextValueForKey("SourceFieldControl");
            this.DestinationFilterName = this.ViewReference.ContextValueForKey("DestinationReadFilter");
            this.SourceRecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            this.SourceRequestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference.ContextValueForKey("SourceRequestOption"), UPRequestOption.FastestAvailable);
            this.DestinationRequestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference.ContextValueForKey("DestinationRequestOption"), UPRequestOption.BestAvailable);

            if (string.IsNullOrEmpty(this.SourceRecordIdentification))
            {
                this.SourceRecordIdentification = this.RecordIdentification;
            }

            this.EditMode = editMode;
            if (!this.SetControlsFromParameters())
            {
                throw new InvalidOperationException("Failed: SetControlsFromParameters");
            }
        }

        /// <summary>
        /// Gets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public List<UPCharacteristicsGroup> Groups { get; private set; }

        /// <summary>
        /// Gets the group dictionary.
        /// </summary>
        /// <value>
        /// The group dictionary.
        /// </value>
        public Dictionary<string, UPCharacteristicsGroup> GroupDictionary => this.groupDict;

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineCharacteristicsRequest OfflineRequest => this.offlineRequest ?? (this.offlineRequest = new UPOfflineCharacteristicsRequest(this.ViewReference));

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the destination field control.
        /// </summary>
        /// <value>
        /// The destination field control.
        /// </value>
        public FieldControl DestinationFieldControl { get; private set; }

        /// <summary>
        /// Gets the group search and list control.
        /// </summary>
        /// <value>
        /// The group search and list control.
        /// </value>
        public SearchAndList GroupSearchAndListControl { get; private set; }

        /// <summary>
        /// Gets the item search and list control.
        /// </summary>
        /// <value>
        /// The item search and list control.
        /// </value>
        public SearchAndList ItemSearchAndListControl { get; private set; }

        /// <summary>
        /// Gets the source field control.
        /// </summary>
        /// <value>
        /// The source field control.
        /// </value>
        public FieldControl SourceFieldControl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [edit mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [edit mode]; otherwise, <c>false</c>.
        /// </value>
        public bool EditMode { get; private set; }

        /// <summary>
        /// Gets the additional fields.
        /// </summary>
        /// <value>
        /// The additional fields.
        /// </value>
        public List<UPConfigFieldControlField> AdditionalFields { get; private set; }

        /// <summary>
        /// Gets the destination group field.
        /// </summary>
        /// <value>
        /// The destination group field.
        /// </value>
        public UPConfigFieldControlField DestinationGroupField { get; private set; }

        /// <summary>
        /// Gets the destination item field.
        /// </summary>
        /// <value>
        /// The destination item field.
        /// </value>
        public UPConfigFieldControlField DestinationItemField { get; private set; }

        /// <summary>
        /// Gets the name of the destination field control.
        /// </summary>
        /// <value>
        /// The name of the destination field control.
        /// </value>
        public string DestinationFieldControlName { get; private set; }

        /// <summary>
        /// Gets the name of the group search and list control.
        /// </summary>
        /// <value>
        /// The name of the group search and list control.
        /// </value>
        public string GroupSearchAndListControlName { get; private set; }

        /// <summary>
        /// Gets the name of the item search and list control.
        /// </summary>
        /// <value>
        /// The name of the item search and list control.
        /// </value>
        public string ItemSearchAndListControlName { get; private set; }

        /// <summary>
        /// Gets the name of the source field control.
        /// </summary>
        /// <value>
        /// The name of the source field control.
        /// </value>
        public string SourceFieldControlName { get; private set; }

        /// <summary>
        /// Gets the source record identification.
        /// </summary>
        /// <value>
        /// The source record identification.
        /// </value>
        public string SourceRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the name of the destination filter.
        /// </summary>
        /// <value>
        /// The name of the destination filter.
        /// </value>
        public string DestinationFilterName { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the source request option.
        /// </summary>
        /// <value>
        /// The source request option.
        /// </value>
        public UPRequestOption SourceRequestOption { get; private set; }

        /// <summary>
        /// Gets the destination request option.
        /// </summary>
        /// <value>
        /// The destination request option.
        /// </value>
        public UPRequestOption DestinationRequestOption { get; private set; }

        /// <summary>
        /// Gets the group field control.
        /// </summary>
        /// <value>
        /// The group field control.
        /// </value>
        public FieldControl GroupFieldControl { get; private set; }

        /// <summary>
        /// Gets the item field control.
        /// </summary>
        /// <value>
        /// The item field control.
        /// </value>
        public FieldControl ItemFieldControl { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPCharacteristicsDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Builds the specified the delegate.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public void Build(UPCharacteristicsDelegate theDelegate)
        {
            if (this.TheDelegate != null)
            {
                theDelegate.CharacteristicsDidFailWithError(this, new InvalidOperationException("duplicate call to build method"));
                return;
            }

            this.TheDelegate = theDelegate;
            if (this.SourceFieldControl != null)
            {
                this.crmQuery = new UPContainerMetaInfo(this.SourceFieldControl);
                this.crmQuery.SetLinkRecordIdentification(this.SourceRecordIdentification);
                this.currentQueryType = 3;
                this.crmQuery.Find(this.DestinationRequestOption, this);
            }
            else
            {
                this.HandleSourceFieldResult(null);
            }
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords()
        {
            List<UPCRMRecord> array = new List<UPCRMRecord>();
            foreach (UPCharacteristicsGroup group in this.Groups)
            {
                List<UPCRMRecord> groupChangedRecords = group.ChangedRecords();
                if (groupChangedRecords != null)
                {
                    array.AddRange(groupChangedRecords);
                }
            }

            string syncParentInfoAreaIdString = this.ViewReference.ContextValueForKey("SyncParentInfoAreaIds");
            if (!string.IsNullOrEmpty(syncParentInfoAreaIdString) && array.Count > 0)
            {
                var syncParentInfoAreaIds = syncParentInfoAreaIdString.Split(',');
                foreach (string syncParentInfoAreaId in syncParentInfoAreaIds)
                {
                    var infoAreaIdParts = syncParentInfoAreaId.Split(':');
                    if (infoAreaIdParts.Length == 1)
                    {
                        array.Add(new UPCRMRecord(syncParentInfoAreaId, new UPCRMLink(this.SourceRecordIdentification, -1)));
                    }
                    else if (infoAreaIdParts.Length > 1)
                    {
                        array.Add(new UPCRMRecord(infoAreaIdParts[0], new UPCRMLink(this.SourceRecordIdentification, Convert.ToInt32(infoAreaIdParts[1]))));
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate.CharacteristicsDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.crmQuery = null;
            switch (this.currentQueryType)
            {
                case 0:
                    this.HandleGroupResult(result);
                    break;

                case 1:
                    this.HandleItemResult(result);
                    break;

                case 2:
                    this.HandleDestinationResult(result);
                    break;

                case 3:
                    this.HandleSourceFieldResult(result);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            if (_linkReader.DestinationRecordIdentification != null)
            {
                this.RecordIdentification = _linkReader.DestinationRecordIdentification;
            }

            this.ContinueLoadRecordData();
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            this.TheDelegate.CharacteristicsDidFailWithError(this, error);
        }

        private bool SetControlsFromParameters()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (this.DestinationFieldControl == null && !string.IsNullOrEmpty(this.DestinationFieldControlName))
            {
                this.DestinationFieldControl = configStore.FieldControlByNameFromGroup(this.EditMode ? "Edit" : "List", this.DestinationFieldControlName);
            }

            if (this.DestinationFieldControl == null)
            {
                return false;
            }

            List<UPConfigFieldControlField> addFields = new List<UPConfigFieldControlField>();
            foreach (FieldControlTab tab in this.DestinationFieldControl.Tabs)
            {
                foreach (UPConfigFieldControlField field in tab.Fields)
                {
                    switch (field.Function)
                    {
                        case Constants.FieldGroupString:
                            this.DestinationGroupField = field;
                            break;

                        case Constants.FieldItemString:
                            this.DestinationItemField = field;
                            break;

                        default:
                            addFields.Add(field);
                            break;
                    }
                }
            }

            if (this.DestinationGroupField == null || this.DestinationItemField == null)
            {
                return false;
            }

            if (addFields.Count > 0)
            {
                this.AdditionalFields = addFields;
            }

            if (!string.IsNullOrEmpty(this.GroupSearchAndListControlName))
            {
                string infoAreaSpecificGroupSearchAndListControl = $"{this.GroupSearchAndListControlName}:{this.RecordIdentification.InfoAreaId()}";
                this.GroupSearchAndListControl = configStore.SearchAndListByName(infoAreaSpecificGroupSearchAndListControl) ??
                                                 configStore.SearchAndListByName(this.GroupSearchAndListControlName);
            }

            if (!string.IsNullOrEmpty(this.ItemSearchAndListControlName))
            {
                string infoAreaSpecificItemSearchAndListControl = $"{this.ItemSearchAndListControlName}:{this.RecordIdentification.InfoAreaId()}";
                this.ItemSearchAndListControl = configStore.SearchAndListByName(infoAreaSpecificItemSearchAndListControl) ??
                                                configStore.SearchAndListByName(this.ItemSearchAndListControlName);
            }

            if (!string.IsNullOrEmpty(this.SourceFieldControlName))
            {
                this.SourceFieldControl = configStore.FieldControlByNameFromGroup("List", this.SourceFieldControlName);
            }

            return true;
        }

        private void LoadGroups()
        {
            this.Groups = new List<UPCharacteristicsGroup>();
            this.groupDict = new Dictionary<string, UPCharacteristicsGroup>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            if (this.GroupSearchAndListControl != null)
            {
                this.GroupFieldControl = configStore.FieldControlByNameFromGroup("List", this.GroupSearchAndListControl.FieldGroupName);
                UPConfigFilter filter = null;
                if (!string.IsNullOrEmpty(this.GroupSearchAndListControl.FilterName))
                {
                    filter = configStore.FilterByName(this.GroupSearchAndListControl.FilterName);
                    if (filter != null && this.sourceFieldDictionary != null)
                    {
                        filter = filter.FilterByApplyingReplacements(new UPConditionValueReplacement(this.sourceFieldDictionary));
                    }
                }

                UPConfigFieldControlField groupField = this.GroupFieldControl.FieldWithFunction(Constants.FieldGroupString);
                if (this.GroupFieldControl != null && groupField != null)
                {
                    this.crmQuery = new UPContainerMetaInfo(this.GroupFieldControl);
                    if (filter != null)
                    {
                        this.crmQuery.ApplyFilter(filter);
                    }

                    this.currentQueryType = 0;
                    this.crmQuery.Find(this.SourceRequestOption, this);
                }
            }
            else
            {
                bool showAdditionalFields = !configStore.ConfigValueIsSet("Characteristics.HideAdditionalFields");
                bool showExpanded = !configStore.ConfigValueIsSet("Characteristics.CollapseGroups");
                UPCatalog catalog = UPCRMDataStore.DefaultStore.CatalogForCrmField(this.DestinationGroupField.Field);
                foreach (string key in catalog.SortedValues)
                {
                    UPCharacteristicsGroup group = new UPCharacteristicsGroup(catalog.TextValueForKey(key), key, false, this, showExpanded);
                    if (showAdditionalFields)
                    {
                        group.ShowAdditionalFields = true;
                    }

                    this.groupDict[key] = group;
                    this.Groups.Add(group);
                }

                this.LoadItems();
            }
        }

        private void HandleGroupResult(UPCRMResult result)
        {
            int count = result.RowCount;
            UPConfigFieldControlField groupField = this.GroupFieldControl.FieldWithFunction(Constants.FieldGroupString);
            UPConfigFieldControlField singleField = this.GroupFieldControl.FieldWithFunction(Constants.FieldSingleString);
            UPConfigFieldControlField showGroupExpandedField = this.GroupFieldControl.FieldWithFunction(Constants.FieldShowGroupExpandedString);
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                string key = row.RawValueAtIndex(groupField.TabIndependentFieldIndex);
                if (!this.groupDict.ContainsKey(key))
                {
                    bool singleSelection = false;
                    if (singleField != null)
                    {
                        string singleFieldValue = row.RawValueAtIndex(singleField.TabIndependentFieldIndex);
                        if (singleFieldValue == "true")
                        {
                            singleSelection = true;
                        }
                    }

                    bool showGroupExpanded = true;
                    if (showGroupExpandedField != null)
                    {
                        showGroupExpanded = row.RawValueAtIndex(showGroupExpandedField.TabIndependentFieldIndex).ToBoolWithDefaultValue(true);
                    }

                    UPCharacteristicsGroup group = new UPCharacteristicsGroup(row.ValueAtIndex(groupField.TabIndependentFieldIndex), key, singleSelection, this, showGroupExpanded);
                    this.groupDict[key] = group;
                    this.Groups.Add(group);
                }
            }

            this.LoadItems();
        }

        private void LoadItems()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (this.ItemSearchAndListControl != null)
            {
                this.ItemFieldControl = configStore.FieldControlByNameFromGroup("List", this.ItemSearchAndListControl.FieldGroupName);
                UPConfigFilter filter = null;
                UPConfigFilter groupFilter = null;
                if (!string.IsNullOrEmpty(this.GroupSearchAndListControl?.FilterName))
                {
                    groupFilter = configStore.FilterByName(this.GroupSearchAndListControl.FilterName);
                    if (groupFilter != null && this.sourceFieldDictionary != null)
                    {
                        groupFilter = groupFilter.FilterByApplyingReplacements(new UPConditionValueReplacement(this.sourceFieldDictionary));
                    }
                }

                if (!string.IsNullOrEmpty(this.ItemSearchAndListControl.FilterName))
                {
                    filter = configStore.FilterByName(this.ItemSearchAndListControl.FilterName);
                    if (filter != null && this.sourceFieldDictionary != null)
                    {
                        filter = filter.FilterByApplyingReplacements(new UPConditionValueReplacement(this.sourceFieldDictionary));
                    }
                }

                UPConfigFieldControlField groupField = this.ItemFieldControl.FieldWithFunction(Constants.FieldGroupString);
                UPConfigFieldControlField itemField = this.ItemFieldControl.FieldWithFunction(Constants.FieldItemString);
                if (this.ItemFieldControl != null && groupField != null && itemField != null)
                {
                    this.crmQuery = new UPContainerMetaInfo(this.ItemFieldControl);
                    if (groupFilter != null)
                    {
                        this.crmQuery.ApplyFilter(groupFilter);
                    }

                    if (filter != null)
                    {
                        this.crmQuery.ApplyFilter(filter);
                    }

                    this.currentQueryType = 1;
                    this.crmQuery.Find(this.SourceRequestOption, this);
                }
                else
                {
                    this.HandleGroupsWithAllItems(this.Groups);
                }
            }
            else
            {
                this.HandleGroupsWithAllItems(this.Groups);
            }
        }

        private void HandleItemResult(UPCRMResult result)
        {
            int count = result.RowCount;
            UPConfigFieldControlField groupField = this.ItemFieldControl.FieldWithFunction(Constants.FieldGroupString);
            UPConfigFieldControlField itemField = this.ItemFieldControl.FieldWithFunction(Constants.FieldItemString);
            UPConfigFieldControlField destinationShowAdditionalFieldsField = this.ItemFieldControl.FieldWithFunction(Constants.FieldShowAdditionalFieldsString);
            List<UPCharacteristicsGroup> groupsWithAllItems = new List<UPCharacteristicsGroup>();
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                string groupKey = row.RawValueAtIndex(groupField.TabIndependentFieldIndex);
                UPCharacteristicsGroup group = this.groupDict.ValueOrDefault(groupKey);
                if (group != null)
                {
                    string itemKey = row.RawValueAtIndex(itemField.TabIndependentFieldIndex);
                    bool showAdditionalFields = false;
                    if (destinationShowAdditionalFieldsField != null)
                    {
                        showAdditionalFields = row.RawValueAtIndex(destinationShowAdditionalFieldsField.TabIndependentFieldIndex).ToBoolWithDefaultValue(false);
                    }

                    if (itemKey == "0")
                    {
                        group.ShowAdditionalFields = showAdditionalFields;
                        if (!groupsWithAllItems.Contains(group))
                        {
                            groupsWithAllItems.Add(group);
                        }
                    }
                    else
                    {
                        group.AddItem(new UPCharacteristicsItem(row.ValueAtIndex(itemField.TabIndependentFieldIndex), itemKey, group, showAdditionalFields ? this.AdditionalFields : null));
                    }
                }
            }

            this.HandleGroupsWithAllItems(groupsWithAllItems);
        }

        private void HandleGroupsWithAllItems(List<UPCharacteristicsGroup> groupsWithAllItems)
        {
            if (groupsWithAllItems.Count > 0)
            {
                UPCatalog itemCatalog = UPCRMDataStore.DefaultStore.CatalogForCrmField(this.DestinationItemField.Field);
                foreach (UPCharacteristicsGroup group in groupsWithAllItems)
                {
                    List<UPCatalogValue> items = itemCatalog.ValuesForParentValueIncludeHidden(Convert.ToInt32(group.CatalogValue), false);
                    if (items != null)
                    {
                        foreach (UPCatalogValue catalogValue in items)
                        {
                            group.AddItem(new UPCharacteristicsItem(catalogValue.Text, catalogValue.CodeKey, group, group.ShowAdditionalFields ? this.AdditionalFields : null));
                        }
                    }
                }
            }

            this.LoadRecordData();
        }

        private int PositionForCrmFieldId(int fieldId)
        {
            int pos = 0;
            string infoAreaId = this.DestinationFieldControl.InfoAreaId;
            foreach (UPConfigFieldControlField field in this.AdditionalFields)
            {
                if (field.FieldId == fieldId && field.InfoAreaId == infoAreaId)
                {
                    return pos;
                }

                ++pos;
            }

            return -1;
        }

        private void LoadRecordData()
        {
            string parentLink = this.ViewReference.ContextValueForKey("ParentLink");
            if (!string.IsNullOrEmpty(parentLink))
            {
                this.linkReader = new UPCRMLinkReader(this.RecordIdentification, parentLink, this);
                this.linkReader.Start();
            }
            else
            {
                this.ContinueLoadRecordData();
            }
        }

        private void ContinueLoadRecordData()
        {
            this.crmQuery = new UPContainerMetaInfo(this.DestinationFieldControl);
            this.currentQueryType = 2;
            if (!string.IsNullOrEmpty(this.DestinationFilterName))
            {
                UPConfigFilter destinationFilter = ConfigurationUnitStore.DefaultStore.FilterByName(this.DestinationFilterName);
                if (destinationFilter != null)
                {
                    this.crmQuery.ApplyFilter(destinationFilter);
                }
            }

            if (this.RecordIdentification.InfoAreaId() == "FI")
            {
                string companyFilterName = $"{this.DestinationFieldControl.InfoAreaId}.CompanyRelated";
                UPConfigFilter companyFilter = ConfigurationUnitStore.DefaultStore.FilterByName(companyFilterName);
                if (companyFilter != null)
                {
                    this.crmQuery.ApplyFilter(companyFilter);
                }
            }

            this.crmQuery.SetLinkRecordIdentification(this.RecordIdentification);
            this.crmQuery.Find(this.DestinationRequestOption, this);
        }

        private void HandleDestinationResult(UPCRMResult result)
        {
            int count = result.RowCount;
            bool expandOnData = ConfigurationUnitStore.DefaultStore.ConfigValue("Characteristics.CollapseGroups") == "empty";
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                string groupKey = row.RawValueAtIndex(this.DestinationGroupField.TabIndependentFieldIndex);
                UPCharacteristicsGroup group = this.groupDict.ValueOrDefault(groupKey);
                if (group != null)
                {
                    if (expandOnData)
                    {
                        group.ShowExpanded = true;
                    }

                    string itemKey = row.RawValueAtIndex(this.DestinationItemField.TabIndependentFieldIndex);
                    UPCharacteristicsItem item = group.ItemDictionary.ValueOrDefault(itemKey);
                    UPCRMRecord conflictRecord = null;
                    if (this.conflictOfflineRequest != null)
                    {
                        foreach (UPCRMRecord currentConflictRecord in this.conflictOfflineRequest.Records)
                        {
                            if (currentConflictRecord.RecordIdentification == row.RecordIdentificationAtFieldIndex(0))
                            {
                                conflictRecord = currentConflictRecord;
                                break;
                            }
                        }
                    }

                    if (conflictRecord == null)
                    {
                        item?.SetFromResultRow(row);
                    }
                    else if (conflictRecord.Deleted)
                    {
                        item?.SetFromRecord(conflictRecord);
                    }
                    else
                    {
                        item?.SetFromResultRow(row);
                        foreach (UPCRMFieldValue fieldValue in conflictRecord.FieldValues)
                        {
                            int position = this.PositionForCrmFieldId(fieldValue.FieldId);
                            if (position >= 0)
                            {
                                item.SetValueForAdditionalFieldPosition(fieldValue.Value, position);
                            }
                        }
                    }
                }
            }

            if (this.conflictOfflineRequest != null)
            {
                foreach (UPCRMRecord conflictRecord in this.conflictOfflineRequest.Records)
                {
                    if (conflictRecord.IsNew)
                    {
                        string groupKey = conflictRecord.StringFieldValueForFieldIndex(this.DestinationGroupField.TabIndependentFieldIndex);
                        UPCharacteristicsGroup group = this.groupDict.ValueOrDefault(groupKey);
                        if (group != null)
                        {
                            string itemKey = conflictRecord.StringFieldValueForFieldIndex(this.DestinationItemField.TabIndependentFieldIndex);
                            UPCharacteristicsItem item = group.ItemDictionary[itemKey];
                            item.SetFromRecord(conflictRecord);
                        }
                    }
                }
            }

            if (!this.EditMode)
            {
                this.RemoveEmptyItems();
            }

            this.TheDelegate.CharacteristicsDidFinishWithResult(this, this);
        }

        private void RemoveEmptyItems()
        {
            List<UPCharacteristicsGroup> emptyGroups = new List<UPCharacteristicsGroup>();
            foreach (UPCharacteristicsGroup group in this.Groups)
            {
                group.RemoveEmptyItems();
                if (!group.HasItems)
                {
                    emptyGroups.Add(group);
                }
            }

            foreach (UPCharacteristicsGroup group in emptyGroups)
            {
                this.Groups.Remove(group);
                this.groupDict.Remove(group.CatalogValue);
            }
        }

        private void HandleSourceFieldResult(UPCRMResult result)
        {
            if (result?.RowCount > 0)
            {
                this.sourceFieldDictionary = this.SourceFieldControl.FunctionNames((UPCRMResultRow)result.ResultRowAtIndex(0));
            }

            this.LoadGroups();
        }
    }
}
