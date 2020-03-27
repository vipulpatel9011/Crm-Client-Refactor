// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPObjectives.cs" company="Aurea Software Gmbh">
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
//   The UPObjectives
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Objectives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The person information area
        /// </summary>
        public const string PersonInfoArea = "KP";

        /// <summary>
        /// The account information area
        /// </summary>
        public const string AccountInfoArea = "FI";

        /// <summary>
        /// The additional fieldgroups configuration name
        /// </summary>
        public const string AdditionalFieldgroupsConfigurationName = "AdditionalDestinationFieldGroups";

        /// <summary>
        /// The additional filter configuration name
        /// </summary>
        public const string AdditionalFilterConfigurationName = "AdditionalDestinationFilter";

        /// <summary>
        /// The additional source field control configuration name
        /// </summary>
        public const string AdditionalSourceFieldControlConfigurationName = "AdditionalSourceFieldControls";

        /// <summary>
        /// The request option configuration name
        /// </summary>
        public const string RequestOptionConfigurationName = "RequestOption";

        /// <summary>
        /// The individual section post fix
        /// </summary>
        public const string IndividualSectionPostFix = "Individual";

        /// <summary>
        /// The sales section post fix
        /// </summary>
        public const string SalesSectionPostFix = "Sales";

        /// <summary>
        /// The rights filter configuration name
        /// </summary>
        public const string RightsFilterConfigurationName = "RightsFilter";

        /// <summary>
        /// The rights filter copy field configuration name
        /// </summary>
        public const string RightsFilterCopyFieldConfigurationName = "RightsFilterCopyFields";
    }

    /// <summary>
    /// UPObjectives
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.UIModel.Objectives.UPObjectivesDocumentDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    public class UPObjectives : ISearchOperationHandler, UPObjectivesDocumentDelegate, UPCopyFieldsDelegate
    {
        private List<UPObjectivesConfiguration> groupConfigurations;
        private bool filterLoaded;
        private Operation objectivesForRecordOperation;
        private Operation rightFilterOperation;
        private Dictionary<string, UPCRMResultRow> filterItems;
        private List<UPCRMResultRow> allItems;
        private UPCRMResult allItemsResult;
        private UPCRMResult filterItemsResult;
        private Dictionary<string, Dictionary<string, object>> copyFieldsDictionary;
        private UPObjectivesConfiguration currentSectionConfiguration;
        private UPCopyFields currentCopyFields;

        private UPOfflineObjectivesRequest offlineRequest;
        private string rootRecordIdentification;
        private bool companyRelated;
        private UPCRMRecord record;
        private int currentGroupIndex;
        private int currentItemDocumentIndex;
        private int itemDocumentsToLoad;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPObjectives"/> class.
        /// </summary>
        /// <param name="rootRecordIdentification">The root record identification.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="editMode">if set to <c>true</c> [edit mode].</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPObjectives(string rootRecordIdentification, Dictionary<string, object> parameters, bool editMode, UPObjectivesDelegate theDelegate)
        {
            this.RecordIdentification = rootRecordIdentification;
            this.Parameters = parameters;
            this.EditMode = editMode;
            this.TheDelegate = theDelegate;
            this.RequestOption = UPRequestOption.BestAvailable;
            this.MaxDocuments = 0;
        }

        /// <summary>
        /// Gets the Objectives groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public List<UPObjectivesGroup> Groups { get; private set; }

        /// <summary>
        /// Gets the group dictionary.
        /// </summary>
        /// <value>
        /// The group dictionary.
        /// </value>
        public Dictionary<string, UPObjectivesGroup> GroupDictionary { get; private set; }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineObjectivesRequest OfflineRequest => this.offlineRequest ?? (this.offlineRequest = new UPOfflineObjectivesRequest(0));

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get;   }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, object> Parameters { get;   }

        /// <summary>
        /// Gets a value indicating whether [edit mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [edit mode]; otherwise, <c>false</c>.
        /// </value>
        public bool EditMode { get;   }

        /// <summary>
        /// Gets the additional fields.
        /// </summary>
        /// <value>
        /// The additional fields.
        /// </value>
        public List<UPConfigFieldControlField> AdditionalFields { get; private set; }

        /// <summary>
        /// Gets the parent link.
        /// </summary>
        /// <value>
        /// The parent link.
        /// </value>
        public string ParentLink { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPObjectivesDelegate TheDelegate { get;   }

        /// <summary>
        /// Gets or sets the maximum documents.
        /// </summary>
        /// <value>
        /// The maximum documents.
        /// </value>
        public int MaxDocuments { get; set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets the rights filter.
        /// </summary>
        /// <value>
        /// The rights filter.
        /// </value>
        public UPConfigFilter RightsFilter { get; private set; }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public bool Build()
        {
            return this.SetControlsFromParameters() && this.LoadObjectives();
        }

        /// <summary>
        /// Objectives document did finish from sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ObjectivesDocumentDidFinishFromSender(UPObjectivesItem sender)
        {
            if (this.currentItemDocumentIndex < this.itemDocumentsToLoad - 1)
            {
                this.currentItemDocumentIndex++;
            }
            else
            {
                this.LoadNextGroup();
            }
        }

        /// <summary>
        /// Objectives document from sender did fail with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void ObjectivesDocumentFromSenderDidFailWithError(UPObjectivesItem sender, Exception error)
        {
            if (this.currentItemDocumentIndex < this.itemDocumentsToLoad - 1)
            {
                this.currentItemDocumentIndex++;
            }
            else
            {
                this.LoadNextGroup();
            }
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.FailWithError(error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="searchResult">The search result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult searchResult)
        {
            if (operation == this.rightFilterOperation)
            {
                this.filterItems = null;
                if (searchResult?.RowCount > 0)
                {
                    Dictionary<string, UPCRMResultRow> tempFilterItems = new Dictionary<string, UPCRMResultRow>();
                    for (int i = 0; i < searchResult.RowCount; i++)
                    {
                        UPCRMResultRow resultRow = (UPCRMResultRow)searchResult.ResultRowAtIndex(i);
                        tempFilterItems[resultRow.RootRecordIdentification] = resultRow;
                    }

                    this.filterItems = tempFilterItems;
                }
                else
                {
                    this.filterItems = new Dictionary<string, UPCRMResultRow>();
                }

                this.filterItemsResult = searchResult;
                this.filterLoaded = true;
                this.rightFilterOperation = null;
            }
            else if (operation == this.objectivesForRecordOperation)
            {
                this.allItems = null;
                if (searchResult != null && searchResult.RowCount > 0)
                {
                    List<UPCRMResultRow> itemArray = new List<UPCRMResultRow>();
                    for (int i = 0; i < searchResult.RowCount; i++)
                    {
                        UPCRMResultRow resultRow = (UPCRMResultRow)searchResult.ResultRowAtIndex(i);
                        itemArray.Add(resultRow);
                    }

                    // load document for each item
                    this.currentItemDocumentIndex = 0;
                    this.itemDocumentsToLoad = searchResult.RowCount;
                    this.allItems = new List<UPCRMResultRow>(itemArray);
                    this.allItemsResult = searchResult;
                }

                this.objectivesForRecordOperation = null;
            }

            // everthing loaded .... continue building the groups
            if (this.filterLoaded && this.rightFilterOperation == null && this.objectivesForRecordOperation == null)
            {
                this.BuildGroupWithConfigurationItemsUpdateableItems(this.groupConfigurations[this.currentGroupIndex], this.allItems, this.filterItems);
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
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.ContinueLoadWithFieldValueDictionary(dictionary);
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.FailWithError(error);
        }

        private void LoadNextGroup()
        {
            this.currentGroupIndex++;
            if (this.groupConfigurations != null && this.groupConfigurations.Count > this.currentGroupIndex)
            {
                UPObjectivesConfiguration configuration = this.groupConfigurations[this.currentGroupIndex];
                this.LoadObjectivesForSectionConfiguration(configuration);
            }
            else
            {
                this.TheDelegate?.ObjectivesDidFinish(this);
            }
        }

        private void FailWithError(Exception error)
        {
            this.allItems = null;
            this.allItemsResult = null;
            this.filterItemsResult = null;
            this.objectivesForRecordOperation = null;
            this.rightFilterOperation = null;
            this.TheDelegate?.ObjectivesDidFailWithError(this, error);
        }

        private void BuildGroupWithConfigurationItemsUpdateableItems(UPObjectivesConfiguration groupConfiguration, List<UPCRMResultRow> items, Dictionary<string, UPCRMResultRow> updateableItems)
        {
            // search for group and create if not found
            string groupKey = groupConfiguration.SectionName;
            UPObjectivesGroup group = this.GroupDictionary.ValueOrDefault(groupKey);
            UPCRMFilterBasedDecision filterBasedDecision = groupConfiguration.FilterBasedDecision;
            if (group == null)
            {
                group = new UPObjectivesGroup(groupKey, this, groupConfiguration);
                this.Groups.Add(group);
                this.GroupDictionary.SetObjectForKey(group, groupKey);
            }

            int count = items?.Count ?? 0;
            if (count == 0)
            {
                this.LoadNextGroup();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = items[i];
                DateTime? date = row.RawValueAtIndex(groupConfiguration
                                    .FieldForFunction(Constants.FieldCompletedOnFunction)
                                    .TabIndependentFieldIndex)
                                    .DateFromCrmValue();

                bool canBeDeleted = false;
                if (updateableItems != null)
                {
                    canBeDeleted = updateableItems.ValueOrDefault(row.RootRecordIdentification) != null;
                }

                // create item
                UPObjectivesItem item = new UPObjectivesItem(row.RootRecordIdentification, date ?? DateTime.MinValue, group, canBeDeleted, this);
                if (filterBasedDecision != null)
                {
                    List<UPConfigButton> actionButtons = filterBasedDecision.ButtonsForResultRow(row);
                    foreach (UPConfigButton button in actionButtons)
                    {
                        item.AddButtonAction(button);
                    }
                }

                item.SetFromResultRow(row);
                item.LoadDocumentsWithMaxResults(this.MaxDocuments);
                group.AddItem(item);
            }
        }

        private bool SetControlsFromParameters()
        {
            ViewReference viewReference = null;

            if (this.Parameters.ContainsKey("viewReference"))
            {
                viewReference = (ViewReference)this.Parameters["viewReference"];
            }

            // create rightsfilter
            string rightsFilterName = viewReference?.ContextValueForKey(Constants.RightsFilterConfigurationName);
            if (!string.IsNullOrEmpty(rightsFilterName))
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                this.RightsFilter = configStore.FilterByName(rightsFilterName);
            }

            if (this.RightsFilter != null)
            {
#if PORTING
                NSDictionary filterParameters = (NSDictionary)viewReference.ContextValueForKey(Constants.RightsFilterCopyFieldConfigurationName);
                if (filterParameters != null)
                {
                    UPConditionValueReplacement replacement = new UPConditionValueReplacement(filterParameters);
                    this.RightsFilter = this.RightsFilter.FilterByApplyingReplacements(replacement);
                }
                else
#endif
                {
                    this.RightsFilter = this.RightsFilter.FilterByApplyingDefaultReplacements();
                }
            }

            this.RequestOption = UPCRMDataStore.RequestOptionFromString(viewReference?.ContextValueForKey(Constants.RequestOptionConfigurationName), UPRequestOption.Offline);
            this.groupConfigurations = new List<UPObjectivesConfiguration>();
            UPObjectivesConfiguration individualConfiguration = UPObjectivesConfiguration.Create(viewReference, Constants.IndividualSectionPostFix, this.Parameters, this.EditMode);
            if (individualConfiguration != null)
            {
                this.groupConfigurations.Add(individualConfiguration);
            }

            UPObjectivesConfiguration salesConfiguration = UPObjectivesConfiguration.Create(viewReference, Constants.SalesSectionPostFix, this.Parameters, this.EditMode);
            if (salesConfiguration != null)
            {
                this.groupConfigurations.Add(salesConfiguration);
            }

            List<string> additionalFieldGroups = this.StringsFromCommaSeparatedString(viewReference?.ContextValueForKey(Constants.AdditionalFieldgroupsConfigurationName));

            // load additional sections
            if (additionalFieldGroups?.Count > 0)
            {
                List<string> additionalDestinationFilter = this.StringsFromCommaSeparatedString(viewReference?.ContextValueForKey(Constants.AdditionalFilterConfigurationName));
                List<string> additionalSourceFieldControl = this.StringsFromCommaSeparatedString(viewReference?.ContextValueForKey(Constants.AdditionalSourceFieldControlConfigurationName));
                if ((additionalDestinationFilter != null && additionalSourceFieldControl != null) && (additionalFieldGroups.Count == additionalDestinationFilter.Count) && (additionalDestinationFilter.Count == additionalSourceFieldControl.Count))
                {
                    Dictionary<string, object> additionalParameterDictionary = new Dictionary<string, object>(this.Parameters);
                    for (int additionalSectionIndex = 0; additionalSectionIndex < additionalDestinationFilter.Count; additionalSectionIndex++)
                    {
                        string sectionName = $"AdditionalSection_{additionalSectionIndex}";
                        string destinationFieldGroupName = UPObjectivesConfiguration.CombineConfigurationNameWithSection(Constants.ParameterDestinationFieldGroupConfigurationName, sectionName);
                        string destinationFilterName = UPObjectivesConfiguration.CombineConfigurationNameWithSection(Constants.ParameterDestinationFilterConfigurationName, sectionName);
                        string sourceFieldControlName = UPObjectivesConfiguration.CombineConfigurationNameWithSection(Constants.ParameterSourceFieldControlConfigurationName, sectionName);
                        additionalParameterDictionary[destinationFieldGroupName] = additionalFieldGroups[additionalSectionIndex];
                        additionalParameterDictionary[destinationFilterName] = additionalDestinationFilter[additionalSectionIndex];
                        additionalParameterDictionary.SetObjectForKey(additionalSourceFieldControl[additionalSectionIndex], sourceFieldControlName);
                        UPObjectivesConfiguration additionalConfiguration = UPObjectivesConfiguration.Create(viewReference, sectionName, additionalParameterDictionary, this.EditMode);
                        if (additionalConfiguration != null)
                        {
                            this.groupConfigurations.Add(additionalConfiguration);
                        }
                    }
                }
            }

            this.record = new UPCRMRecord(this.RecordIdentification);
            this.ParentLink = viewReference?.ContextValueForKey("ParentLink");
            return true;
        }

        private string LoadRootRecordIdentification()
        {
            string rootIdentification;

            // load objectives linked to a call
            if (!string.IsNullOrEmpty(this.ParentLink))
            {
                UPContainerMetaInfo personLoadContainer = new UPContainerMetaInfo(Constants.PersonInfoArea);
                personLoadContainer.SetLinkRecordIdentification(this.RecordIdentification);
                UPCRMResult personResult = personLoadContainer.Find();

                // person related record...load objectives for person
                if (personResult.RowCount == 1)
                {
                    rootIdentification = personResult.ResultRowAtIndex(0).RootRecordIdentification;
                }
                else
                {
                    // company related record..load objectives for company
                    UPContainerMetaInfo companyLoadContainer = new UPContainerMetaInfo(Constants.AccountInfoArea);
                    companyLoadContainer.SetLinkRecordIdentification(this.RecordIdentification);
                    UPCRMResult companyResult = companyLoadContainer.Find();
                    rootIdentification = companyResult.RowCount == 1
                        ? companyResult.ResultRowAtIndex(0).RootRecordIdentification
                        : this.RecordIdentification;
                }
            }
            else
            {
                rootIdentification = this.RecordIdentification;
            }

            return rootIdentification;
        }

        private bool LoadObjectives()
        {
            this.GroupDictionary = new Dictionary<string, UPObjectivesGroup>();
            this.Groups = new List<UPObjectivesGroup>();
            this.rootRecordIdentification = this.LoadRootRecordIdentification();
            this.companyRelated = this.rootRecordIdentification.InfoAreaId() == Constants.AccountInfoArea;
            if (this.groupConfigurations != null && this.groupConfigurations.Count > 0)
            {
                this.currentGroupIndex = 0;
                UPObjectivesConfiguration configuration = this.groupConfigurations[this.currentGroupIndex];
                this.LoadObjectivesForSectionConfiguration(configuration);
                return true;
            }

            SimpleIoc.Default.GetInstance<ILogger>().LogError("Objectives could not be loaded - groupConfiguration undefined");
            return false;
        }

        private void LoadObjectivesForSectionConfiguration(UPObjectivesConfiguration section)
        {
            this.currentSectionConfiguration = section;
            FieldControl sourceFieldControl = section.SourceFieldControl;
            if (sourceFieldControl != null)
            {
                Dictionary<string, object> copyFieldValues = this.copyFieldsDictionary.ValueOrDefault(sourceFieldControl.UnitName);
                if (copyFieldValues == null)
                {
                    this.currentCopyFields = new UPCopyFields(sourceFieldControl);
                    this.currentCopyFields.CopyFieldValuesForRecord(this.record, false, this);
                    return;
                }

                this.ContinueLoadWithFieldValueDictionary(copyFieldValues);
            }
            else
            {
                this.ContinueLoadWithFieldValueDictionary(null);
            }
        }

        private void ContinueLoadWithFieldValueDictionary(Dictionary<string, object> fieldValueDictionary)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (fieldValueDictionary != null)
            {
                if (this.copyFieldsDictionary == null)
                {
                    this.copyFieldsDictionary = new Dictionary<string, Dictionary<string, object>>();
                }

                this.copyFieldsDictionary.SetObjectForKey(fieldValueDictionary, this.currentSectionConfiguration.SourceFieldControl.UnitName);
            }

            this.currentCopyFields = null;
            FieldControl destinationFieldControl = this.currentSectionConfiguration.DestinationFieldControl;
            string destinationFilterName = this.currentSectionConfiguration.DestinationFilterName;
            UPConfigFilter companyRelatedFilter = null;
            if (this.companyRelated)
            {
                string companyFilterName = $"{destinationFieldControl.InfoAreaId}.CompanyRelated";
                companyRelatedFilter = configStore.FilterByName(companyFilterName);
            }

            UPContainerMetaInfo container = new UPContainerMetaInfo(destinationFieldControl);
            container.SetLinkRecordIdentification(this.rootRecordIdentification);
            List<UPConfigFilter> appliedFilters = new List<UPConfigFilter>();
            UPConfigFilter filter = configStore.FilterByName(destinationFilterName);
            if (filter != null)
            {
                filter = filter.FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(fieldValueDictionary));
                container.ApplyFilter(filter);
                appliedFilters.Add(filter);
            }

            if (companyRelatedFilter != null && this.currentSectionConfiguration.SourceFieldControl != null)
            {
                companyRelatedFilter = companyRelatedFilter.FilterByApplyingReplacements(new UPConditionValueReplacement(fieldValueDictionary));
                container.ApplyFilter(companyRelatedFilter);
                appliedFilters.Add(companyRelatedFilter);
            }

            this.objectivesForRecordOperation = null;
            this.rightFilterOperation = null;
            this.filterLoaded = true;
            if (this.RightsFilter != null && this.filterItemsResult == null)
            {
                this.filterLoaded = false;
            }

            if (this.currentSectionConfiguration.ExecuteActionFilter != null)
            {
                this.currentSectionConfiguration.ExecuteActionFilter = this.currentSectionConfiguration.ExecuteActionFilter.FilterByApplyingValueDictionaryDefaults(fieldValueDictionary, true);
                if (this.currentSectionConfiguration.ExecuteActionFilter != null)
                {
                    this.currentSectionConfiguration.FilterBasedDecision = new UPCRMFilterBasedDecision(this.currentSectionConfiguration.ExecuteActionFilter);
                    List<UPCRMField> fields = this.currentSectionConfiguration.FilterBasedDecision.FieldDictionary.Values.ToList();
                    if (fields.Count > 0)
                    {
                        container.AddCrmFields(fields);
                    }

                    this.currentSectionConfiguration.FilterBasedDecision.UseCrmQuery(container);
                }
            }

            this.objectivesForRecordOperation = container.Find(this.RequestOption, this);
            if (this.objectivesForRecordOperation == null)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError("Could not create operation for loading objectives.");
                this.FailWithError(new Exception("ConnectionOfflineError"));
            }

            if (this.RightsFilter != null && this.filterItemsResult == null)
            {
                UPContainerMetaInfo clonedContainer = new UPContainerMetaInfo(new List<UPCRMField>(), destinationFieldControl.InfoAreaId);
                clonedContainer.SetLinkRecordIdentification(this.rootRecordIdentification);
                clonedContainer.ApplyFilter(this.RightsFilter);
                foreach (UPConfigFilter filter1 in appliedFilters)
                {
                    clonedContainer.ApplyFilter(filter1);
                }

                this.rightFilterOperation = clonedContainer.Find(this.RequestOption, this);
                if (this.rightFilterOperation == null)
                {
                    SimpleIoc.Default.GetInstance<ILogger>().LogError("Could not create operation for filtering objectives.");
                    this.FailWithError(new Exception("ConnectionOfflineError"));
                }
            }
        }

        private List<string> StringsFromCommaSeparatedString(string inputString)
        {
            List<string> partsArray = null;
            if (!string.IsNullOrEmpty(inputString))
            {
                var arrayOfStrings = inputString.Split(',');
                partsArray = new List<string>();

                foreach (string partialName in arrayOfStrings)
                {
                    string trimmedPartialName = partialName.Trim();
                    partsArray.Add(trimmedPartialName);
                }
            }

            return partsArray;
        }
    }
}
