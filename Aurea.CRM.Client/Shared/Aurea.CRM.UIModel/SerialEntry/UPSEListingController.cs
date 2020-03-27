// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEListingController.cs" company="Aurea Software Gmbh">
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
//   UPSEListingController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// UPSEListingController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Catalogs.UPCatalogValueProvider" />
    public class UPSEListingController : ISearchOperationHandler, UPCatalogValueProvider
    {
        private bool fastRequest;
        private int loadStep;
        private List<object> loadQueue;
        private UPSEListingOwner currentLoadedOwner;
        private UPSEListingLoadRequest currentLoadRequest;
        private UPContainerMetaInfo currentQuery;
        private Dictionary<string, UPSEListingOwner> listingOwners;
        private bool ownersLoaded;
        private Dictionary<string, UPSEColumn> rowColumnsForFunctionNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEListingController"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSEListingController(string recordIdentification, Dictionary<string, object> parameters, UPSerialEntry serialEntry,
            UPSEListingControllerDelegate theDelegate)
        {
            this.fastRequest = ServerSession.CurrentSession.IsEnterprise;
            this.LinkRecordIdentification = recordIdentification;
            this.RootRecordIdentification = recordIdentification;
            this.SerialEntry = serialEntry;
            string listingControlName = parameters.ValueOrDefault("ListingControlName") as string;
            string listingOwnerFieldGroupName = parameters.ValueOrDefault("ListingOwnerFieldGroupName") as string;
            string relatedListingOwnersConfigName = parameters.ValueOrDefault("RelatedListingOwnersConfigName") as string;
            string distinctListingFunctionNamesString = parameters.ValueOrDefault("DistinctListingFunctionNames") as string;
            this.rowColumnsForFunctionNames = new Dictionary<string, UPSEColumn>();
            if (!string.IsNullOrEmpty(distinctListingFunctionNamesString))
            {
                this.DistinctListingFunctionNames = distinctListingFunctionNamesString.Split(',').ToList();
            }

            string listingApplyHierarchyString = parameters.ValueOrDefault("ApplyHierarchy") as string;
            if (!string.IsNullOrEmpty(listingControlName))
            {
                List<UPSEListingFieldMatch> hierarchyItemArray = new List<UPSEListingFieldMatch>();
                var hierarchyItemStrings = listingApplyHierarchyString.Split(';');
                int index = 0;
                foreach (string itemStrings in hierarchyItemStrings)
                {
                    if (string.IsNullOrEmpty(itemStrings))
                    {
                        continue;
                    }

                    hierarchyItemArray.Add(new UPSEListingFieldMatch(index++, itemStrings));
                }

                this.ListingFieldMatchHierarchy = hierarchyItemArray;
            }

            string destinationFieldFunctionNameString = parameters.ValueOrDefault("DestinationFieldFunctionNames") as string;
            if (!string.IsNullOrEmpty(destinationFieldFunctionNameString))
            {
                this.DestinationFieldFunctionNames = destinationFieldFunctionNameString.Split(',').ToList();
            }

            this.RootFieldValues = parameters.ValueOrDefault("RecordFieldValues") as Dictionary<string, object>;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.ListingOwnerFieldGroup = configStore.FieldControlByNameFromGroup("List", listingOwnerFieldGroupName);
            this.RootListingOwnerMapping = this.ListingOwnerFieldGroup.FunctionNames();
            this.RelatedListingOwnersSearch = configStore.SearchAndListByName(relatedListingOwnersConfigName);
            FieldControl fieldControl = configStore.FieldControlByNameFromGroup("List", this.RelatedListingOwnersSearch.FieldGroupName);
            this.ListingOwnerMapping = fieldControl.FunctionNames();
            this.ListingSearch = configStore.SearchAndListByName(listingControlName);
            fieldControl = configStore.FieldControlByNameFromGroup("List", this.ListingSearch.FieldGroupName);
            this.ListingMapping = fieldControl.FunctionNames();
            if (this.ListingMapping.Count > 0)
            {
                Dictionary<string, string> listingKeyForFieldKey = new Dictionary<string, string>(this.ListingMapping.Count);
                foreach (string key in this.ListingMapping.Keys)
                {
                    UPConfigFieldControlField field = this.ListingMapping[key];
                    listingKeyForFieldKey[$"{field.InfoAreaId}.{field.FieldId}"] = key;
                }

                this.ListingFieldToKey = listingKeyForFieldKey;
            }

            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Gets the row columns for function names.
        /// </summary>
        /// <value>
        /// The row columns for function names.
        /// </value>
        public Dictionary<string, UPSEColumn> RowColumnsForFunctionNames => this.rowColumnsForFunctionNames;

        /// <summary>
        /// Gets the listings.
        /// </summary>
        /// <value>
        /// The listings.
        /// </value>
        public List<UPSEListing> Listings => this.ListingOwner.AllListings.Values.ToList();

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.ListingSearch.InfoAreaId;

        /// <summary>
        /// Gets the root record identification.
        /// </summary>
        /// <value>
        /// The root record identification.
        /// </value>
        public string RootRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the listing search.
        /// </summary>
        /// <value>
        /// The listing search.
        /// </value>
        public SearchAndList ListingSearch { get; private set; }

        /// <summary>
        /// Gets the listing owner field group.
        /// </summary>
        /// <value>
        /// The listing owner field group.
        /// </value>
        public FieldControl ListingOwnerFieldGroup { get; private set; }

        /// <summary>
        /// Gets the related listing owners search.
        /// </summary>
        /// <value>
        /// The related listing owners search.
        /// </value>
        public SearchAndList RelatedListingOwnersSearch { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSEListingControllerDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the listing owner.
        /// </summary>
        /// <value>
        /// The listing owner.
        /// </value>
        public UPSEListingOwner ListingOwner { get; private set; }

        /// <summary>
        /// Gets the root listing owner mapping.
        /// </summary>
        /// <value>
        /// The root listing owner mapping.
        /// </value>
        public Dictionary<string, UPConfigFieldControlField> RootListingOwnerMapping { get; private set; }

        /// <summary>
        /// Gets the listing owner mapping.
        /// </summary>
        /// <value>
        /// The listing owner mapping.
        /// </value>
        public Dictionary<string, UPConfigFieldControlField> ListingOwnerMapping { get; private set; }

        /// <summary>
        /// Gets the listing mapping.
        /// </summary>
        /// <value>
        /// The listing mapping.
        /// </value>
        public Dictionary<string, UPConfigFieldControlField> ListingMapping { get; private set; }

        /// <summary>
        /// Gets the root field values.
        /// </summary>
        /// <value>
        /// The root field values.
        /// </value>
        public Dictionary<string, object> RootFieldValues { get; private set; }

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the distinct listing function names.
        /// </summary>
        /// <value>
        /// The distinct listing function names.
        /// </value>
        public List<string> DistinctListingFunctionNames { get; private set; }

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets the listing field match hierarchy.
        /// </summary>
        /// <value>
        /// The listing field match hierarchy.
        /// </value>
        public List<UPSEListingFieldMatch> ListingFieldMatchHierarchy { get; private set; }

        /// <summary>
        /// Gets the destination field function names.
        /// </summary>
        /// <value>
        /// The destination field function names.
        /// </value>
        public List<string> DestinationFieldFunctionNames { get; private set; }

        /// <summary>
        /// Gets the listing field to key.
        /// </summary>
        /// <value>
        /// The listing field to key.
        /// </value>
        public Dictionary<string, string> ListingFieldToKey { get; private set; }

        /// <summary>
        /// Gets the listing label format.
        /// </summary>
        /// <value>
        /// The listing label format.
        /// </value>
        public string ListingLabelFormat { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Aurea.CRM.Core.CRM.Catalogs.UPCatalogValueProvider" /> is dependent.
        /// </summary>
        /// <value>
        /// <c>true</c> if dependent; otherwise, <c>false</c>.
        /// </value>
        public bool Dependent => false;

        /// <summary>
        /// Gets the parent values.
        /// </summary>
        /// <value>
        /// The parent values.
        /// </value>
        public Dictionary<string, string> ParentValues => null;

        /// <summary>
        /// Gets the sorted parent value codes.
        /// </summary>
        /// <value>
        /// The sorted parent value codes.
        /// </value>
        public List<string> SortedParentValueCodes => null;

        /// <summary>
        /// Listings the key for information area identifier field identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="fieldId">The field identifier.</param>
        /// <returns></returns>
        public string ListingKeyForInfoAreaIdFieldId(string infoAreaId, int fieldId)
        {
            return this.ListingFieldToKey.ValueOrDefault($"{infoAreaId}.{fieldId}");
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            if (this.ListingOwner == null)
            {
                this.loadStep = 1;
                this.currentQuery = new UPContainerMetaInfo(this.ListingOwnerFieldGroup);
                this.currentQuery.SetLinkRecordIdentification(this.RootRecordIdentification);
                if (this.currentQuery.Find(this.SerialEntry.RequestOption, this) != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the listing loaded for owner context.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="context">The context.</param>
        public void HandleListingLoadedForOwnerContext(UPSEListingOwner owner, object context)
        {
            if (this.loadStep == 3)
            {
                if (this.loadQueue.Count == 0)
                {
                    this.loadStep = 0;
                    this.LoadSerialEntryInformation();
                    this.TheDelegate?.ListingControllerContextDidReturnOwner(this, context, owner);
                }
                else
                {
                    this.LoadNextListingFromQueue();
                }

                return;
            }

            this.TheDelegate?.ListingControllerContextDidReturnListingForOwner(owner.ListingController, context, owner);

            this.LoadNextListingFromQueue();
        }

        /// <summary>
        /// Handles the listing load for owner context did finish with error.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void HandleListingLoadForOwnerContextDidFinishWithError(UPSEListingOwner owner, object context, Exception error)
        {
            this.TheDelegate?.ListingControllerContextDidFailWithError(owner.ListingController, context, error);
        }

        /// <summary>
        /// Listingses the match values.
        /// </summary>
        /// <param name="_listings">The listings.</param>
        /// <param name="valueDictionary">The value dictionary.</param>
        /// <returns></returns>
        public List<UPSEListing> ListingsMatchValues(List<UPSEListing> _listings, Dictionary<string, string> valueDictionary)
        {
            List<UPSEListing> foundListings = null;
            foreach (UPSEListing listing in _listings)
            {
                int matchIndex = listing.ValueDictionaryMatchInHierarchyMaxIndex(valueDictionary,
                    this.ListingFieldMatchHierarchy, 0);
                if (matchIndex >= 0)
                {
                    if (foundListings == null)
                    {
                        foundListings = new List<UPSEListing> { listing };
                    }
                    else
                    {
                        foundListings.Add(listing);
                    }
                }
            }

            return foundListings;
        }

        /// <summary>
        /// Owners for key.
        /// </summary>
        /// <param name="listingKey">The listing key.</param>
        /// <returns></returns>
        public UPSEListingOwner OwnerForKey(string listingKey)
        {
            return this.listingOwners[listingKey];
        }

        /// <summary>
        /// Possibles the values.
        /// </summary>
        /// <returns>
        /// The <see cref="!:Dictionary" />.
        /// </returns>
        public Dictionary<string, string> PossibleValues()
        {
            Dictionary<string, string> listingDictionary = new Dictionary<string, string>(this.listingOwners.Count);
            foreach (string key in this.listingOwners.Keys)
            {
                UPSEListingOwner owner = this.listingOwners[key];
                if (!owner.TransparentOwner)
                {
                    listingDictionary[key] = owner.Label;
                }
            }

            return listingDictionary;
        }

        /// <summary>
        /// Sorts the order for codes.
        /// </summary>
        /// <param name="codes">The codes.</param>
        /// <returns>
        /// The <see cref="!:List" />.
        /// </returns>
        public List<string> SortOrderForCodes(List<string> codes)
        {
            var dict = this.PossibleValues();
            return dict.SortedKeysFromKeyArray(codes);
        }

        /// <summary>
        /// Possibles the values for parent code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// The <see cref="!:Dictionary" />.
        /// </returns>
        public Dictionary<string, string> PossibleValuesForParentCode(string code)
        {
            return null;
        }

        /// <summary>
        /// Parents the code for code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// The <see cref="T:System.Int32" />.
        /// </returns>
        public int ParentCodeForCode(string code)
        {
            return -1;
        }

        /// <summary>
        /// Determines whether [is empty value] [the specified code].
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// The <see cref="T:System.Boolean" />.
        /// </returns>
        public bool IsEmptyValue(string code)
        {
            return false;
        }

        /// <summary>
        /// Displays the string for codes.
        /// </summary>
        /// <param name="codes">The codes.</param>
        /// <returns>
        /// The <see cref="T:System.String" />.
        /// </returns>
        public string DisplayStringForCodes(List<string> codes)
        {
            StringBuilder mutString = new StringBuilder();
            foreach (string code in codes)
            {
                string label = this.listingOwners[code].Label;
                if (!string.IsNullOrEmpty(label))
                {
                    if (mutString.Length > 0)
                    {
                        mutString.AppendFormat(", {0}", label);
                    }
                    else
                    {
                        mutString.Append(label);
                    }
                }
            }

            return mutString.ToString();
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate?.ListingControllerContextDidFailWithError(this, null, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            switch (this.loadStep)
            {
                case 1:
                    this.HandleRootOwnerResult(result);
                    break;

                case 2:
                    this.HandleOwnerResult(result);
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

        private void LoadNextItemFromQueue()
        {
            if (this.loadQueue.Count > 0)
            {
                this.currentLoadedOwner = this.loadQueue[0] as UPSEListingOwner;
                this.loadStep = 2;
                this.currentQuery = new UPContainerMetaInfo(this.RelatedListingOwnersSearch, null);
                this.currentQuery.SetLinkRecordIdentification(this.currentLoadedOwner.RecordIdentification);
                this.loadQueue.RemoveAt(0);
                this.currentQuery.Find(this.SerialEntry.RequestOption, this, false);
            }
            else
            {
                this.currentQuery = null;
                this.currentLoadedOwner = null;
                this.ownersLoaded = true;
                this.loadStep = 3;
                foreach (UPSEListingOwner currentOwner in this.listingOwners.Values)
                {
                    this.LoadListingForOwnerContext(currentOwner, null);
                }
            }
        }

        private void LoadNextListingFromQueue()
        {
            UPSEListingLoadRequest loadRequest = null;
            lock (this.loadQueue)
            {
                if (this.currentLoadedOwner == null && this.loadQueue.Count > 0)
                {
                    this.currentLoadRequest = this.loadQueue[0] as UPSEListingLoadRequest;
                    loadRequest = this.currentLoadRequest;
                    this.loadQueue.RemoveAt(0);
                }
            }

            loadRequest?.Load();
        }

        private bool LoadListingForOwnerContext(UPSEListingOwner owner, object context)
        {
            if (owner.ListingsLoaded)
            {
                this.TheDelegate?.ListingControllerContextDidReturnListingForOwner(this, context, owner);

                return true;
            }

            if (!this.ownersLoaded)
            {
                this.TheDelegate?.ListingControllerContextDidFailWithError(this, context, new Exception("ownersNotLoaded"));

                return false;
            }

            bool startNextListing = false;
            lock (this.loadQueue)
            {
                if (this.currentLoadRequest == null && this.loadQueue.Count == 0)
                {
                    startNextListing = true;
                }

                this.loadQueue.Add(new UPSEListingLoadRequest(owner, context));
            }

            if (startNextListing)
            {
                this.LoadNextListingFromQueue();
            }

            return true;
        }

        private void HandleOwnerResult(UPCRMResult result)
        {
            int count = result.RowCount;
            if (count > 0)
            {
                List<UPSEListingOwner> relatedOwnersForCurrentOwner = new List<UPSEListingOwner>();
                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                    string recordIdentification = row.RecordIdentificationAtIndex(1);
                    if (string.IsNullOrEmpty(recordIdentification))
                    {
                        continue;
                    }

                    UPSEListingOwner relatedOwner = this.listingOwners.ValueOrDefault(row.RecordIdentificationAtIndex(1));
                    if (relatedOwner == null)
                    {
                        relatedOwner = new UPSEListingOwner((UPCRMResultRow)result.ResultRowAtIndex(i), 1, this.ListingOwnerMapping, this);
                        this.loadQueue.Add(relatedOwner);
                        this.listingOwners.SetObjectForKey(relatedOwner, relatedOwner.RecordIdentification);
                    }

                    relatedOwnersForCurrentOwner.Add(relatedOwner);
                }

                this.currentLoadedOwner.RelatedOwners = relatedOwnersForCurrentOwner;
            }

            this.LoadNextItemFromQueue();
        }

        private void HandleRootOwnerResult(UPCRMResult result)
        {
            UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
            this.RootRecordIdentification = row.RecordIdentificationAtIndex(0);
            this.ListingOwner = new UPSEListingOwner(row, 0, this.RootListingOwnerMapping, this);
            if (!string.IsNullOrEmpty(this.ListingOwner.RecordIdentification))
            {
                this.listingOwners = new Dictionary<string, UPSEListingOwner> { { this.ListingOwner.RecordIdentification, this.ListingOwner } };
                this.loadQueue = new List<object> { this.ListingOwner };
            }

            this.LoadNextItemFromQueue();
        }

        private void LoadSerialEntryInformation()
        {
            if (this.DistinctListingFunctionNames.Count > 0)
            {
                foreach (string functionName in this.DistinctListingFunctionNames)
                {
                    UPSEColumn column = this.SerialEntry.ColumnForFunctionName(functionName);
                    if (column != null)
                    {
                        this.rowColumnsForFunctionNames[functionName] = column;
                    }
                }
            }

            if (this.ListingFieldMatchHierarchy.Count > 0)
            {
                foreach (UPSEListingFieldMatch item in this.ListingFieldMatchHierarchy)
                {
                    foreach (string functionName in item.FunctionNames)
                    {
                        if (this.rowColumnsForFunctionNames.ContainsKey(functionName))
                        {
                            UPSEColumn column = this.SerialEntry.ColumnForFunctionName(functionName);
                            if (column != null)
                            {
                                this.rowColumnsForFunctionNames[functionName] = column;
                            }
                        }
                    }
                }
            }
        }
    }
}