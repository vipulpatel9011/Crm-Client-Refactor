// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntry.cs" company="Aurea Software Gmbh">
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
//   Serial Entry
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.SerialEntry
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
    using Aurea.CRM.Core.Session;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// SerialEntryType
    /// </summary>
    public enum SerialEntryType
    {
        /// <summary>
        /// Order
        /// </summary>
        Order,

        /// <summary>
        /// Offer
        /// </summary>
        Offer,

        /// <summary>
        /// POS
        /// </summary>
        POS
    }

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The search operation source children constant
        /// </summary>
        public const int UPSESearchOperationSourceChildren = 1;

        /// <summary>
        /// The search operation destination constant
        /// </summary>
        public const int UPSESearchOperationDestination = 2;

        /// <summary>
        /// The search operation destination children constant
        /// </summary>
        public const int UPSESearchOperationDestinationChildren = 3;

        /// <summary>
        /// The search rows for filter constant
        /// </summary>
        public const int UPSESearchRowsForFilter = 4;

        /// <summary>
        /// The search operation destination parent constant
        /// </summary>
        public const int UPSESearchOperationDestinationParent = 5;

        /// <summary>
        /// The search operation root details information panel constant
        /// </summary>
        public const int UPSESearchOperationRootDetailsInfoPanel = 7;

        /// <summary>
        /// The search rows for automatic fill constant
        /// </summary>
        public const int UPSESearchRowsForAutoFill = 8;

        /// <summary>
        /// The search operation product catalog constant
        /// </summary>
        public const int UPSESearchOperationProductCatalog = 9;
    }

    /// <summary>
    /// UPSerialEntry
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEListingControllerDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Features.UPCRMListFormatterFunctionDataProvider" />
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEAdditionalItemInformationsDelegate" />
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEQuotaHandlerDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class UPSerialEntry : ISearchOperationHandler, UPSEListingControllerDelegate, UPCRMListFormatterFunctionDataProvider,
        UPSEPricingDelegate, UPSEAdditionalItemInformationsDelegate, UPSEQuotaHandlerDelegate, UPCopyFieldsDelegate, UPCRMLinkReaderDelegate
    {
        private const string ColumnItemNumber = "ItemNumber";
        private const string FieldConfigFunctionSourceImage = "SourceImage";
        private const string FunctionPrefixColumnCopy = "ColumnCopy:";
        private const string FunctionServerApproved = "ServerApproved";
        private const string KeyActiveFilter = "ActiveFilter";
        private const string KeyAddSourceConfigs = "addSourceConfigs";
        private const string KeyAutoCreatePositions = "autoCreatePositions";
        private const string KeyComputeRowOnEveryColumn = "computeRowOnEveryColumn";
        private const string KeyCopyItemNumber = "CopyItemNumber";
        private const string KeyDestinationChildConfigName = "DestinationChildConfigName";
        private const string KeyDestinationChildTemplateFilter = "DestinationChildTemplateFilter";
        private const string KeyDestinationParentConfigName = "DestinationParentConfigName";
        private const string KeyDestinationParentTemplateFilter = "DestinationParentTemplateFilter";
        private const string KeyDestinationRequestOption = "DestinationRequestOption";
        private const string KeyDestinationRootConfig = "DestinationRootConfig";
        private const string KeyDestinationTemplateFilter = "DestinationTemplateFilter";
        private const string KeyDetails = "Details";
        private const string KeyDisableAutoCorrectMinMax = "disableAutoCorrectMinMax";
        private const string KeyDisableAutoCorrectPackageSize = "disableAutoCorrectPackageSize";
        private const string KeyDisableRowUpdate = "disableRowUpdate";
        private const string KeyDocumentsDefinition = "DocumentsDefinition";
        private const string KeyEdit = "Edit";
        private const string KeyEditTrigger = "EditTrigger";
        private const string KeyFieldGroupDecider = "FieldGroupDecider";
        private const string KeyInfoPanelDefinition = "InfoPanelDefinition";
        private const string KeyItemNumber = "ItemNumber";
        private const string KeyList = "List";
        private const string KeyListingConfiguration = "ListingConfiguration";
        private const string KeyListingControlName = "ListingControlName";
        private const string KeyMinSearchTextLength = "minSearchTextLength";
        private const string KeyOptions = "Options";
        private const string KeyPdfHlColor = "pdfHlColor";
        private const string KeyPricingAutoItemNumber = "Pricing.AutoItemNumber";
        private const string KeyPricingConfiguration = "PricingConfiguration";
        private const string KeyProductCatalogSource = "ProductCatalogSource";
        private const string KeyQuotaConfiguration = "QuotaConfiguration";
        private const string KeyRootFilterTemplateForEndSerialEntry = "RootFilterTemplateForEndSerialEntry";
        private const string KeyRootFilterTemplateForStartSerialEntry = "RootFilterTemplateForStartSerialEntry";
        private const string KeyRowDisplayConfiguration = "RowDisplayConfiguration";
        private const string KeySerialEntryComputeRowOnEveryColumn = "SerialEntry.ComputeRowOnEveryColumn";
        private const string KeySourceChildConfigName = "SourceChildConfigName";
        private const string KeySourceParentInfoAreaId = "SourceParentInfoAreaId";
        private const string KeySourceRequestOption = "SourceRequestOption";
        private const string KeySumLineConfiguration = "SumLineConfiguration";
        private const string KeySyncRowAfterChildren = "syncRowAfterChildren";
        private const string KeySyncStrategy = "syncStrategy";

        private bool isEnterprise;
        private Dictionary<string, UPSERow> rowsWithData;
        private Dictionary<string, UPSERow> positionsForListing;
        private Dictionary<string, UPSERow> deletedRows;
        private int currentSearchOperationType;
        private UPContainerMetaInfo currentSearchOperation;
        private UPRequestOption currentRequestOption;
        private UPRequestOption sourceRequestOption;
        private UPRequestOption destinationRequestOption;
        private UPContainerInfoAreaMetaInfo sourceInfoAreaMetaInfo;
        private UPContainerInfoAreaMetaInfo positionInfoAreaMetaInfo;
        private UPContainerInfoAreaMetaInfo sourceChildInfoAreaMetaInfo;
        private UPConfigFilter fixedSourceFilter;
        private UPSEFilter currentFilter;
        private Operation currentQueuedSearchOperation;
        private string currentListingOwner;
        private List<UPSerialEntryRequest> requestQueue;
        private UPSerialEntryRequest currentRequest;
        private Dictionary<string, int> nextGenericKeyForSourceKeyDictionary;
        private Dictionary<string, List<string>> positionsForSourceKeyDictionary;
        private Dictionary<string, UPSEDestinationParent> destinationParents;
        private UPCRMListFormatter sumLineListFormatter;
        private UPCRMLinkReader linkReader;
        private UPCopyFields copyFields;
        private ViewReference viewReference;
        private UPSerialEntryAutoFill autoFill;
        private bool rowsChanged;
        private int currentCheckedActiveFilterIndex;
        private bool checkActiveFilters;
        private List<UPConfigFilter> currentSearchFilters;
        private Dictionary<long, UPSERowConfiguration> rowConfigurations;
        private int minSearchTextLength;
        private DocumentInfoAreaManager productCatalogDocumentManager;
        private int sourceFieldControlOffset;

        public List<UPSERow> positions;
        protected UPOfflineSerialEntryRequest offlineRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntry"/> class.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSerialEntry(UPCRMRecord rootRecord, Dictionary<string, object> parameters, UPSerialEntryDelegate theDelegate)
            : this(rootRecord, parameters, null, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntry"/> class.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="offlineRequest">The offline request.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSerialEntry(UPCRMRecord rootRecord, Dictionary<string, object> parameters, UPOfflineSerialEntryRequest offlineRequest, UPSerialEntryDelegate theDelegate)
        {
            this.RequestOption = UPRequestOption.FastestAvailable;
            this.isEnterprise = ServerSession.CurrentSession.IsEnterprise;
            this.Record = rootRecord;
            this.Parameters = parameters;
            this.SetFilterArrayFromParameters();
            this.TheDelegate = theDelegate;
            this.sourceRequestOption = UPRequestOption.Offline;
            this.destinationRequestOption = offlineRequest != null ? UPRequestOption.Online : UPRequestOption.FastestAvailable;

            this.rowsWithData = new Dictionary<string, UPSERow>();
            this.positions = new List<UPSERow>();
            this.positionsForListing = new Dictionary<string, UPSERow>();
            this.deletedRows = new Dictionary<string, UPSERow>();
            this.positionsForSourceKeyDictionary = new Dictionary<string, List<string>>();
            this.nextGenericKeyForSourceKeyDictionary = new Dictionary<string, int>();
            this.offlineRequest = offlineRequest;
            this.ConflictHandling = this.offlineRequest != null;
            this.requestQueue = new List<UPSerialEntryRequest>();
            this.viewReference = this.Parameters["viewReference"] as ViewReference;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string configName = this.viewReference.ContextValueForKey("DestinationConfigName");
            this.DestFieldControl = configStore.FieldControlByNameFromGroup("Edit", configName);
            FieldControl destListFieldControl = configStore.FieldControlByNameFromGroup("List", configName);

            if (this.DestFieldControl != null && destListFieldControl?.CrmSortFields?.Count > 0)
            {
                this.DestFieldControl = new FieldControl(this.DestFieldControl, destListFieldControl);
            }

            configName = this.viewReference.ContextValueForKey("SourceConfigName");
            this.SourceConfig = configStore.SearchAndListByName(configName);
            if (this.SourceConfig != null)
            {
                this.SourceFieldControl = configStore.FieldControlByNameFromGroup("List", this.SourceConfig.FieldGroupName);
                this.SourceColumnCount = this.SourceFieldControl.NumberOfFields;
                this.SourceSearchControl = configStore.FieldControlByNameFromGroup("Search", this.SourceConfig.FieldGroupName);
                if (!string.IsNullOrEmpty(this.SourceConfig.FilterName))
                {
                    this.fixedSourceFilter = configStore.FilterByName(this.SourceConfig.FilterName);
                }
            }

            this.ItemNumberFunctionName = this.viewReference.ContextValueForKey("ItemNumberFunctionName");
            if (string.IsNullOrEmpty(this.ItemNumberFunctionName))
            {
                Dictionary<string, UPConfigFieldControlField> dict = this.SourceFieldControl.FunctionNames();
                if (dict.ContainsKey("ItemNumber"))
                {
                    this.ItemNumberFunctionName = "ItemNumber";
                }
                else if (dict.ContainsKey("CopyItemNumber"))
                {
                    this.ItemNumberFunctionName = "CopyItemNumber";
                }

                this.ExplicitItemNumberFunctionName = false;
            }
            else
            {
                this.ExplicitItemNumberFunctionName = true;
            }

            if (!string.IsNullOrEmpty(this.ItemNumberFunctionName) && this.SourceFieldControl != null)
            {
                UPConfigFieldControlField fieldControlField = this.SourceFieldControl.FieldWithFunction(this.ItemNumberFunctionName);
                this.ItemNumberSourceIndex = fieldControlField?.TabIndependentFieldIndex ?? -1;
            }
            else
            {
                this.ItemNumberSourceIndex = -1;
            }

            this.SaveAllExecuted = true;
        }

        /// <summary>
        /// Gets or sets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; set; }

        /// <summary>
        /// Gets or sets the initial field values for destination.
        /// </summary>
        /// <value>
        /// The initial field values for destination.
        /// </value>
        public Dictionary<string, object> InitialFieldValuesForDestination { get; set; }

        /// <summary>
        /// Gets the dest information area identifier.
        /// </summary>
        /// <value>
        /// The dest information area identifier.
        /// </value>
        public string DestInfoAreaId => this.DestFieldControl.InfoAreaId;

        /// <summary>
        /// Gets the dest child information area identifier.
        /// </summary>
        /// <value>
        /// The dest child information area identifier.
        /// </value>
        public string DestChildInfoAreaId => this.DestChildFieldControl?.InfoAreaId;

        /// <summary>
        /// Gets the source information area identifier.
        /// </summary>
        /// <value>
        /// The source information area identifier.
        /// </value>
        public string SourceInfoAreaId => this.SourceFieldControl?.InfoAreaId;

        /// <summary>
        /// Gets the position count.
        /// </summary>
        /// <value>
        /// The position count.
        /// </value>
        public int PositionCount { get; private set; }

        /// <summary>
        /// Gets the positions.
        /// </summary>
        /// <value>
        /// The positions.
        /// </value>
        public List<UPSERow> Positions {
            get
            {
                return this.positions;
            }
            private set
            {
                this.positions = value;
            }
        }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public virtual UPOfflineSerialEntryRequest OfflineRequest => this.offlineRequest;

        /// <summary>
        /// Gets a value indicating whether this instance has minimum maximum columns.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has minimum maximum columns; otherwise, <c>false</c>.
        /// </value>
        public bool HasMinMaxColumns { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has step size columns.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has step size columns; otherwise, <c>false</c>.
        /// </value>
        public bool HasStepSizeColumns { get; private set; }

        /// <summary>
        /// Gets the serial entry parent information panel.
        /// </summary>
        /// <value>
        /// The serial entry parent information panel.
        /// </value>
        public UPSerialEntryParentInfoPanel SerialEntryParentInfoPanel { get; private set; }

        /// <summary>
        /// Gets the product catalog documents.
        /// </summary>
        /// <value>
        /// The product catalog documents.
        /// </value>
        public List<DocumentData> ProductCatalogDocuments { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable row delete].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable row delete]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableRowDelete { get; private set; }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<UPSEFilter> Filters { get; private set; }

        /// <summary>
        /// Gets the source configuration.
        /// </summary>
        /// <value>
        /// The source configuration.
        /// </value>
        public SearchAndList SourceConfig { get; private set; }

        /// <summary>
        /// Gets the source child configuration.
        /// </summary>
        /// <value>
        /// The source child configuration.
        /// </value>
        public SearchAndList SourceChildConfig { get; private set; }

        /// <summary>
        /// Gets the dest field control.
        /// </summary>
        /// <value>
        /// The dest field control.
        /// </value>
        public FieldControl DestFieldControl { get; private set; }

        /// <summary>
        /// Gets the dest child field control.
        /// </summary>
        /// <value>
        /// The dest child field control.
        /// </value>
        public FieldControl DestChildFieldControl { get; private set; }

        /// <summary>
        /// Gets the source field control.
        /// </summary>
        /// <value>
        /// The source field control.
        /// </value>
        public FieldControl SourceFieldControl { get; private set; }

        /// <summary>
        /// Gets the source child field control.
        /// </summary>
        /// <value>
        /// The source child field control.
        /// </value>
        public FieldControl SourceChildFieldControl { get; private set; }

        /// <summary>
        /// Gets the children count.
        /// </summary>
        /// <value>
        /// The children count.
        /// </value>
        public int ChildrenCount { get; private set; }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public List<UPSEColumn> Columns { get; private set; }

        /// <summary>
        /// Gets the source children.
        /// </summary>
        /// <value>
        /// The source children.
        /// </value>
        public List<UPSESourceChild> SourceChildren { get; private set; }

        /// <summary>
        /// Gets the source column count.
        /// </summary>
        /// <value>
        /// The source column count.
        /// </value>
        public int SourceColumnCount { get; private set; }

        /// <summary>
        /// Gets the dest child columns for function.
        /// </summary>
        /// <value>
        /// The dest child columns for function.
        /// </value>
        public Dictionary<string, List<UPSEColumn>> DestChildColumnsForFunction { get; private set; }

        /// <summary>
        /// Gets the dest columns for function.
        /// </summary>
        /// <value>
        /// The dest columns for function.
        /// </value>
        public Dictionary<string, UPSEColumn> DestColumnsForFunction { get; private set; }

        /// <summary>
        /// Gets the source columns for function.
        /// </summary>
        /// <value>
        /// The source columns for function.
        /// </value>
        public Dictionary<string, UPSESourceColumn> SourceColumnsForFunction { get; private set; }

        /// <summary>
        /// Gets the dest root field control.
        /// </summary>
        /// <value>
        /// The dest root field control.
        /// </value>
        public FieldControl DestRootFieldControl { get; private set; }

        /// <summary>
        /// Gets the source search control.
        /// </summary>
        /// <value>
        /// The source search control.
        /// </value>
        public FieldControl SourceSearchControl { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSerialEntryDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has running change requests.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has running change requests; otherwise, <c>false</c>.
        /// </value>
        public bool HasRunningChangeRequests { get; private set; }

        /// <summary>
        /// Gets the serial entry row information panels.
        /// </summary>
        /// <value>
        /// The serial entry row information panels.
        /// </value>
        public List<UPSerialEntryInfo> SerialEntryRowInfoPanels { get; private set; }

        /// <summary>
        /// Gets the serial entry row documents.
        /// </summary>
        /// <value>
        /// The serial entry row documents.
        /// </value>
        public List<UPSerialEntryDocuments> SerialEntryRowDocuments { get; private set; }

        /// <summary>
        /// Gets the dest parent edit field control.
        /// </summary>
        /// <value>
        /// The dest parent edit field control.
        /// </value>
        public FieldControl DestParentEditFieldControl { get; private set; }

        /// <summary>
        /// Gets the dest parent list field control.
        /// </summary>
        /// <value>
        /// The dest parent list field control.
        /// </value>
        public FieldControl DestParentListFieldControl { get; private set; }

        /// <summary>
        /// Gets the source parent information area identifier.
        /// </summary>
        /// <value>
        /// The source parent information area identifier.
        /// </value>
        public string SourceParentInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the dest parent information area identifier.
        /// </summary>
        /// <value>
        /// The dest parent information area identifier.
        /// </value>
        public string DestParentInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the listing controller.
        /// </summary>
        /// <value>
        /// The listing controller.
        /// </value>
        public UPSEListingController ListingController { get; private set; }

        /// <summary>
        /// Gets the listing configuration.
        /// </summary>
        /// <value>
        /// The listing configuration.
        /// </value>
        public ViewReference ListingConfiguration { get; private set; }

        /// <summary>
        /// Gets the row display field control.
        /// </summary>
        /// <value>
        /// The row display field control.
        /// </value>
        public FieldControl RowDisplayFieldControl { get; private set; }

        /// <summary>
        /// Gets the sum line field control.
        /// </summary>
        /// <value>
        /// The sum line field control.
        /// </value>
        public FieldControl SumLineFieldControl { get; private set; }

        /// <summary>
        /// Gets the listing information area identifier.
        /// </summary>
        /// <value>
        /// The listing information area identifier.
        /// </value>
        public string ListingInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the additional options.
        /// </summary>
        /// <value>
        /// The additional options.
        /// </value>
        public Dictionary<string, object> AdditionalOptions { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable single row update].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable single row update]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableSingleRowUpdate { get; private set; }

        /// <summary>
        /// Gets the synchronize strategy.
        /// </summary>
        /// <value>
        /// The synchronize strategy.
        /// </value>
        public string SyncStrategy { get; private set; }

        /// <summary>
        /// Gets the pricing configuration.
        /// </summary>
        /// <value>
        /// The pricing configuration.
        /// </value>
        public ViewReference PricingConfiguration { get; private set; }

        /// <summary>
        /// Gets the pricing.
        /// </summary>
        /// <value>
        /// The pricing.
        /// </value>
        public UPSEPricing Pricing { get; private set; }

        /// <summary>
        /// Gets the column sort information.
        /// </summary>
        /// <value>
        /// The column sort information.
        /// </value>
        public UPSESortInfo ColumnSortInfo { get; private set; }

        /// <summary>
        /// Gets the additional source configuration names.
        /// </summary>
        /// <value>
        /// The additional source configuration names.
        /// </value>
        public List<string> AdditionalSourceConfigNames { get; private set; }

        /// <summary>
        /// Gets the additional item informations.
        /// </summary>
        /// <value>
        /// The additional item informations.
        /// </value>
        public UPSEAdditionalItemInformations AdditionalItemInformations { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [conflict handling].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [conflict handling]; otherwise, <c>false</c>.
        /// </value>
        public bool ConflictHandling { get; private set; }

        /// <summary>
        /// Gets the server approved column.
        /// </summary>
        /// <value>
        /// The server approved column.
        /// </value>
        public UPSEColumn ServerApprovedColumn { get; private set; }

        /// <summary>
        /// Gets the quota.
        /// </summary>
        /// <value>
        /// The quota.
        /// </value>
        public UPSEQuotaHandler Quota { get; private set; }

        /// <summary>
        /// Gets the quota configuration.
        /// </summary>
        /// <value>
        /// The quota configuration.
        /// </value>
        public ViewReference QuotaConfiguration { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference => this.viewReference;

        /// <summary>
        /// Gets a value indicating whether [disable automatic correct package size].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable automatic correct package size]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableAutoCorrectPackageSize { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable automatic correct minimum maximum].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable automatic correct minimum maximum]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableAutoCorrectMinMax { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets the root filter template for end serial entry.
        /// </summary>
        /// <value>
        /// The root filter template for end serial entry.
        /// </value>
        public UPConfigFilter RootFilterTemplateForEndSerialEntry { get; private set; }

        /// <summary>
        /// Gets the root filter template for start serial entry.
        /// </summary>
        /// <value>
        /// The root filter template for start serial entry.
        /// </value>
        public UPConfigFilter RootFilterTemplateForStartSerialEntry { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [start serial entry filter applied].
        /// </summary>
        /// <value>
        /// <c>true</c> if [start serial entry filter applied]; otherwise, <c>false</c>.
        /// </value>
        public bool StartSerialEntryFilterApplied { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [save all executed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [save all executed]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveAllExecuted { get; set; }

        /// <summary>
        /// Gets the name of the item number function.
        /// </summary>
        /// <value>
        /// The name of the item number function.
        /// </value>
        public string ItemNumberFunctionName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [explicit item number function name].
        /// </summary>
        /// <value>
        /// <c>true</c> if [explicit item number function name]; otherwise, <c>false</c>.
        /// </value>
        public bool ExplicitItemNumberFunctionName { get; private set; }

        /// <summary>
        /// Gets the index of the item number source.
        /// </summary>
        /// <value>
        /// The index of the item number source.
        /// </value>
        public int ItemNumberSourceIndex { get; private set; }

        /// <summary>
        /// Gets the destination child template filter.
        /// </summary>
        /// <value>
        /// The destination child template filter.
        /// </value>
        public UPConfigFilter DestinationChildTemplateFilter { get; private set; }

        /// <summary>
        /// Gets the destination parent template filter.
        /// </summary>
        /// <value>
        /// The destination parent template filter.
        /// </value>
        public UPConfigFilter DestinationParentTemplateFilter { get; private set; }

        /// <summary>
        /// Gets the destination template filter.
        /// </summary>
        /// <value>
        /// The destination template filter.
        /// </value>
        public UPConfigFilter DestinationTemplateFilter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [compute row on every column].
        /// </summary>
        /// <value>
        /// <c>true</c> if [compute row on every column]; otherwise, <c>false</c>.
        /// </value>
        public bool ComputeRowOnEveryColumn { get; private set; }

        /// <summary>
        /// Gets the listing source columns.
        /// </summary>
        /// <value>
        /// The listing source columns.
        /// </value>
        public Dictionary<string, UPSEColumn> ListingSourceColumns { get; private set; }

        /// <summary>
        /// Gets the active filter.
        /// </summary>
        /// <value>
        /// The active filter.
        /// </value>
        public string[] ActiveFilter { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic create positions].
        /// </summary>
        /// <value>
        /// <c>true</c> if [automatic create positions]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoCreatePositions { get; set; }

        /// <summary>
        /// Gets the last result.
        /// </summary>
        /// <value>
        /// The last result.
        /// </value>
        public UPCRMResult LastResult { get; private set; }

        /// <summary>
        /// Gets the field group decider filter.
        /// </summary>
        /// <value>
        /// The field group decider filter.
        /// </value>
        public UPConfigFilter FieldGroupDeciderFilter { get; private set; }

        /// <summary>
        /// Gets the field group decider.
        /// </summary>
        /// <value>
        /// The field group decider.
        /// </value>
        public UPCRMFilterBasedDecision FieldGroupDecider { get; private set; }

        /// <summary>
        /// Gets the default row configuration.
        /// </summary>
        /// <value>
        /// The default row configuration.
        /// </value>
        public UPSERowConfiguration DefaultRowConfiguration { get; private set; }

        /// <summary>
        /// Gets the dest copy column array for function.
        /// </summary>
        /// <value>
        /// The dest copy column array for function.
        /// </value>
        public Dictionary<string, List<UPSEColumn>> DestCopyColumnArrayForFunction { get; private set; }

        /// <summary>
        /// Gets the document key column.
        /// </summary>
        /// <value>
        /// The document key column.
        /// </value>
        public UPSESourceColumn DocumentKeyColumn { get; private set; }

        /// <summary>
        /// Gets the root details field control.
        /// </summary>
        /// <value>
        /// The root details field control.
        /// </value>
        public FieldControl RootDetailsFieldControl { get; private set; }

        /// <summary>
        /// Gets the product catalog search and list.
        /// </summary>
        /// <value>
        /// The product catalog search and list.
        /// </value>
        public SearchAndList ProductCatalogSearchAndList { get; private set; }

        /// <summary>
        /// Gets the rebate columns.
        /// </summary>
        /// <value>
        /// The rebate columns.
        /// </value>
        public List<UPSEColumn> RebateColumns { get; private set; }

        //public UPCRMEditTrigger DestEditTrigger { get; private set; }

        //public UPCRMEditTrigger DestChildEditTrigger { get; private set; }

        /// <summary>
        /// Gets the dest column for field key.
        /// </summary>
        /// <value>
        /// The dest column for field key.
        /// </value>
        public Dictionary<string, UPSEColumn> DestColumnForFieldKey { get; private set; }

        /// <summary>
        /// Gets the dest child columns for field key.
        /// </summary>
        /// <value>
        /// The dest child columns for field key.
        /// </value>
        public Dictionary<string, List<UPSEColumn>> DestChildColumnsForFieldKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [overall discount active].
        /// </summary>
        /// <value>
        /// <c>true</c> if [overall discount active]; otherwise, <c>false</c>.
        /// </value>
        public bool OverallDiscountActive { get; private set; }

        /// <summary>
        /// Gets the row display list formatter.
        /// </summary>
        /// <value>
        /// The row display list formatter.
        /// </value>
        public UPCRMListFormatter RowDisplayListFormatter { get; private set; }

        /// <summary>
        /// Gets the color of the PDF hl.
        /// </summary>
        /// <value>
        /// The color of the PDF hl.
        /// </value>
        public string PdfHlColor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [synchronize row after children].
        /// </summary>
        /// <value>
        /// <c>true</c> if [synchronize row after children]; otherwise, <c>false</c>.
        /// </value>
        public bool SyncRowAfterChildren { get; private set; }

        /// <summary>
        /// Gets the number of filters.
        /// </summary>
        /// <value>
        /// The number of filters.
        /// </value>
        public int NumberOfFilters => this.Filters.Count;

        /// <summary>
        /// Filters at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPSEFilter FilterAtIndex(int index)
        {
            return this.Filters[index];
        }

        /// <summary>
        /// Sources the child index for record identifier.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        public int SourceChildIndexForRecordId(string recordId)
        {
            int count = this.SourceChildren.Count;
            for (int i = 0; i < count; i++)
            {
                UPSESourceChild sourceChild = this.SourceChildren[i];
                if (sourceChild.Record.RecordId == recordId)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Formatteds the value for column with function row.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public string FormattedValueForColumnWithFunctionRow(string function, UPSERow row)
        {
            UPSEColumn column = this.DestColumnsForFunction.ValueOrDefault(function);
            return column == null ? string.Empty : this.FormattedValueForColumnRow(column, row);
        }

        /// <summary>
        /// Formatteds the value for column row.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public string FormattedValueForColumnRow(UPSEColumn column, UPSERow row)
        {
            List<string> stringValues = new List<string>();
            for (int i = column.Index; i <= column.Index + column.FieldConfig.Attributes.FieldCount; i++)
            {
                UPSEColumn childColumn = this.Columns[i];
                object columnValue = row.ValueAtIndex(childColumn.Index);
                if (columnValue == null)
                {
                    stringValues.Add(string.Empty);
                }
                else
                {
                    bool valueAdded = false;
                    if (!(columnValue is string))
                    {                        
                        stringValues.Add(columnValue.ToString());
                        valueAdded = true;
                    }

                    if (childColumn is UPSEDestinationColumnBase)
                    {
                        stringValues.Add(((UPSEDestinationColumnBase)childColumn).DisplayValueForRawValue((string)columnValue));
                        valueAdded = true;
                    }

                    //if(childColumn is UPSESourceColumn)
                    //{
                    //    stringValues.Add(((UPSESourceColumn)childColumn).StringValueFromObject(columnValue));
                    //}
                    if (!valueAdded)
                    {
                        stringValues.Add(Convert.ToString(columnValue));
                    }
                }
            }

            return column.FieldConfig.Attributes.FormatValues(stringValues);
        }

        /// <summary>
        /// Determines whether [has source value for column span index] [the specified index].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if [has source value for column span index] [the specified index]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasSourceValueForColumnSpanIndex(int index)
        {
            int startIndex = this.ColumnIndexForColumnSpanIndex(index);
            if (this.Columns.Count <= startIndex)
            {
                return false;
            }

            UPSEColumn column = this.Columns[startIndex];
            return column.ColumnFrom == UPSEColumnFrom.Source;
        }

        /// <summary>
        /// Fields the index of the attributes for column span.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public FieldAttributes FieldAttributesForColumnSpanIndex(int index)
        {
            int startIndex = this.ColumnIndexForColumnSpanIndex(index);
            if (this.Columns.Count <= startIndex)
            {
                return null;
            }

            UPSEColumn column = this.Columns[startIndex];
            return column.FieldConfig.Attributes;
        }

        /// <summary>
        /// Formatteds the source value for column span index row.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public string FormattedSourceValueForColumnSpanIndexRow(int index, UPSERow row)
        {
            int startIndex = this.ColumnIndexForColumnSpanIndex(index);
            if (this.Columns.Count <= startIndex)
            {
                return string.Empty;
            }

            UPSEColumn column = this.Columns[startIndex];
            if (column.ColumnFrom != UPSEColumnFrom.Source)
            {
                return string.Empty;
            }

            return this.FormattedValueForColumnRow(column, row);
        }

        /// <summary>
        /// Refreshes the row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public bool RefreshRow(UPSERow row)
        {
            if (!string.IsNullOrEmpty(row.DestinationRootRecord?.RecordIdentification))
            {
                UPContainerMetaInfo existingPositionQuery = new UPContainerMetaInfo(this.DestFieldControl);
                existingPositionQuery.SetLinkRecordIdentification(row.DestinationRootRecord.RecordIdentification);
                UPCRMResult result = existingPositionQuery.Find();
                if (result.RowCount == 0)
                {
                    row.UpdateDestinationDataFromRowChildResult(null, null);
                    this.RemoveDeleteRowWithKey(row.RowKey);
                    return false;
                }

                UPCRMResult destinationChildResult = null;
                if (this.DestChildFieldControl != null)
                {
                    UPContainerMetaInfo existingChildPositionQuery = new UPContainerMetaInfo(this.DestChildFieldControl);
                    existingChildPositionQuery.RootInfoAreaMetaInfo.AddTable(new UPContainerInfoAreaMetaInfo(this.SourceChildFieldControl.InfoAreaId));
                    existingChildPositionQuery.SetLinkRecordIdentification(row.DestinationRootRecord.RecordIdentification);
                    destinationChildResult = existingChildPositionQuery.Find();
                }

                row.UpdateDestinationDataFromRowChildResult((UPCRMResultRow)result.ResultRowAtIndex(0), destinationChildResult);
                return true;
            }

            row.UpdateDestinationDataFromRowChildResult(null, null);
            if (this.positions.Contains(row))
            {
                this.RemovePosition(row);
            }

            return false;
        }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Automatics the fill finished.
        /// </summary>
        public void AutoFillFinished()
        {
            this.TheDelegate?.SerialEntryBuildDidFinishWithSuccess(this, null);
        }

        private int ColumnIndexForColumnSpanIndex(int index)
        {
            int startIndex = 0;
            while (index > 0 && this.Columns.Count > startIndex)
            {
                UPSEColumn column = this.Columns[startIndex];
                if (column.FieldConfig.Attributes.Hide)
                {
                    startIndex++;
                    continue;
                }

                if (column.FieldConfig.Attributes.FieldCount > 1)
                {
                    startIndex += column.FieldConfig.Attributes.FieldCount;
                }
                else
                {
                    startIndex++;
                }

                index--;
            }

            while (this.Columns.Count > startIndex)
            {
                UPSEColumn column = this.Columns[startIndex];
                if (!column.FieldConfig.Attributes.Hide)
                {
                    return startIndex;
                }

                ++startIndex;
            }

            return startIndex;
        }

        private void ReadProductCatalogDocuments()
        {
            UPContainerMetaInfo productCatalogQuery = this.productCatalogDocumentManager.CrmQueryWithFilterParameter(this.InitialFieldValuesForDestination);
            this.currentSearchOperationType = Constants.UPSESearchOperationProductCatalog;
            this.currentSearchOperation = productCatalogQuery;
            productCatalogQuery.Find(this.RequestOption, this, false);
        }

        private void ReadRootDetailsInfoPanel()
        {
            UPContainerMetaInfo rootDetailsQuery = new UPContainerMetaInfo(this.RootDetailsFieldControl);
            rootDetailsQuery.SetLinkRecordIdentification(this.Record.RecordIdentification);
            this.currentSearchOperationType = Constants.UPSESearchOperationRootDetailsInfoPanel;
            this.currentSearchOperation = rootDetailsQuery;
            this.currentRequestOption = this.isEnterprise ? this.RequestOption : this.destinationRequestOption;
            rootDetailsQuery.Find(this.currentRequestOption, this, false);
        }

        private void ReadDestinationParent()
        {
            UPContainerMetaInfo existingParentPositionQuery = new UPContainerMetaInfo(this.DestParentListFieldControl);
            existingParentPositionQuery.SetLinkRecordIdentification(this.Record.RecordIdentification);
            this.currentSearchOperationType = Constants.UPSESearchOperationDestinationParent;
            this.currentSearchOperation = existingParentPositionQuery;
            this.currentRequestOption = this.isEnterprise ? this.RequestOption : this.destinationRequestOption;
            existingParentPositionQuery.Find(this.currentRequestOption, this, true);
        }

        private void ReadDestination()
        {
            UPContainerMetaInfo existingPositionQuery = new UPContainerMetaInfo(this.DestFieldControl);
            UPContainerMetaInfo sourceQuery = new UPContainerMetaInfo(this.SourceFieldControl);
            this.sourceInfoAreaMetaInfo = sourceQuery.RootInfoAreaMetaInfo;
            this.sourceInfoAreaMetaInfo.ParentRelation = "PLUS";
            this.sourceInfoAreaMetaInfo.LinkId = -1;
            this.FieldGroupDecider = null;
            if (this.FieldGroupDeciderFilter != null)
            {
                UPConfigFilter filter = this.FieldGroupDeciderFilter.FilterByApplyingValueDictionaryDefaults(this.InitialFieldValuesForDestination, true);
                if (filter != null)
                {
                    this.FieldGroupDecider = new UPCRMFilterBasedDecision(filter);
                    sourceQuery.AddCrmFields(this.FieldGroupDecider.FieldDictionary.Values.ToList());
                    this.rowConfigurations = null;
                }
            }

            this.sourceFieldControlOffset = existingPositionQuery.OutputFields.Count;
            existingPositionQuery.RootInfoAreaMetaInfo.AddTable(sourceQuery.RootInfoAreaMetaInfo);
            existingPositionQuery.UpdateAfterAddingFieldMetaInfos(sourceQuery.OutputFields);
            if (string.IsNullOrEmpty(this.Record.RecordIdentification))
            {
                this.TheDelegate?.SerialEntryDidFailWithError(this, new Exception("wrong organizer for serial entry edit"));
                return;
            }

            existingPositionQuery.SetLinkRecordIdentification(this.Record.RecordIdentification);
            string filterName = this.viewReference.ContextValueForKey("DestinationFilter");
            if (!string.IsNullOrEmpty(filterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
                if (filter != null)
                {
                    filter = filter.FilterByApplyingValueDictionaryDefaults(this.InitialFieldValuesForDestination, true);
                    existingPositionQuery.ApplyFilter(filter);
                }
            }

            this.FieldGroupDecider?.UseCrmQuery(existingPositionQuery);
            this.currentSearchOperationType = Constants.UPSESearchOperationDestination;
            this.currentSearchOperation = existingPositionQuery;
            this.currentRequestOption = this.isEnterprise ? this.RequestOption : this.destinationRequestOption;
            existingPositionQuery.Find(this.currentRequestOption, this, true);
        }

        private void InitializeWithDestinationChildResult(UPCRMResult destinationChildResult, UPContainerMetaInfo existingChildPositionQuery)
        {
            int count = 0;
            if (destinationChildResult != null)
            {
                count = destinationChildResult.RowCount;
            }
            if (count > 0)
            {
                int positionRecordPosition = existingChildPositionQuery.IndexOfResultInfoArea(this.positionInfoAreaMetaInfo);
                int sourceChildRecordPosition = existingChildPositionQuery.IndexOfResultInfoArea(this.sourceChildInfoAreaMetaInfo);
                Dictionary<string, UPSERow> rowsWithDataByPositionRecordId = new Dictionary<string, UPSERow>(this.rowsWithData.Count);
                foreach (UPSERow row in this.rowsWithData.Values)
                {
                    rowsWithDataByPositionRecordId.SetObjectForKey(row, row.SerialEntryRecordId);
                }

                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow resultRow = (UPCRMResultRow)destinationChildResult.ResultRowAtIndex(i);
                    string positionRecordId = resultRow.RecordIdAtIndex(positionRecordPosition);
                    UPSERow row = rowsWithDataByPositionRecordId.ValueOrDefault(positionRecordId);
                    if (row != null)
                    {
                        string childInfoAreaId = resultRow.RecordIdAtIndex(sourceChildRecordPosition);
                        int sourceChildIndex = this.SourceChildIndexForRecordId(childInfoAreaId);
                        if (sourceChildIndex >= 0)
                        {
                            row.SetChildDataForIndexResultRow(sourceChildIndex, resultRow);
                        }
                    }
                }
            }

            if (this.Pricing != null)
            {
                foreach (UPSERow row in this.positions)
                {
                    row.RowPricing.LoadConditions();
                }
            }

            this.CheckOverallPriceWithRow(null);
            this.ReadListing();
        }

        private void ReadPricing()
        {
            if (this.PricingConfiguration != null)
            {
                this.Pricing = new UPSEPricing(this, this.PricingConfiguration, this);
            }

            if (this.Pricing != null)
            {
                this.Pricing.Load(this.InitialFieldValuesForDestination);
            }
            else
            {
                if (this.DestParentListFieldControl != null)
                {
                    this.ReadDestinationParent();
                }
                else
                {
                    this.ReadDestination();
                }
            }
        }

        private void ReadListing()
        {
            if (this.ListingConfiguration != null)
            {
                Dictionary<string, object> copyValueDictionary = null;
                if (this.InitialFieldValuesForDestination.Count > 0)
                {
                    copyValueDictionary = new Dictionary<string, object> { { "RecordFieldValues", this.InitialFieldValuesForDestination } };
                }

                Dictionary<string, object> _parameters = this.ListingConfiguration.ParameterDictionaryByAppendingDictionary(copyValueDictionary);
                this.ListingController = new UPSEListingController(this.Record.RecordIdentification, _parameters, this, this);
                if (!this.ListingController.Load())
                {
                    this.Logger.LogWarn($"Listing could not be loaded: {this.Record?.RecordIdentification}");
                    this.ListingController = null;
                    this.ReadQuota();
                }
            }
            else
            {
                this.ReadQuota();
            }
        }

        private void ReadQuota()
        {
            if (this.QuotaConfiguration != null)
            {
                DateTime date = DateTime.UtcNow;
                string dateParameter = this.QuotaConfiguration.ContextValueForKey("DateParameterName");
                if (!string.IsNullOrEmpty(dateParameter))
                {
                    string dateString = this.InitialFieldValuesForDestination[dateParameter] as string;
                    date = dateString?.DateFromCrmValue() ?? DateTime.UtcNow;
                }

                this.Quota = UPSEQuotaHandler.Create(this, this.QuotaConfiguration, date, this);
                if (this.Quota != null)
                {
                    this.Quota.LoadForLinkRecord(this.Record, this.InitialFieldValuesForDestination);
                }
                else
                {
                    this.FinishedLoading();
                }
            }
            else
            {
                this.FinishedLoading();
            }
        }

        private void FinishedLoading()
        {
            if (this.ConflictHandling)
            {
                UPOfflineSerialEntryApplyResult applyResult = this.offlineRequest.ApplyChangesToSerialEntry(this);
                if (applyResult.Error != null)
                {
                    this.TheDelegate?.SerialEntryDidFailWithError(this, applyResult.Error);

                    return;
                }
            }

            this.autoFill = UPSerialEntryAutoFill.Create(this, this.viewReference);
            if (this.autoFill != null)
            {
                List<string> itemNumbers = this.autoFill.ItemNumbers;
                this.RowsForItemNumbers(itemNumbers);
            }
            else
            {
                this.TheDelegate?.SerialEntryBuildDidFinishWithSuccess(this, null);
            }
        }

        private void AddDestinationParent(UPSEDestinationParent destinationParent)
        {
            if (this.destinationParents == null)
            {
                this.destinationParents = new Dictionary<string, UPSEDestinationParent> { { destinationParent.SourceRecordId, destinationParent } };
            }
            else
            {
                this.destinationParents.SetObjectForKey(destinationParent, destinationParent.SourceRecordId);
            }
        }

        private void InitializeWithDestinationParentResult(UPCRMResult destinationResult, UPContainerMetaInfo existingPositionQuery)
        {
            this.destinationRequestOption = destinationResult.IsServerResult ? UPRequestOption.Online : UPRequestOption.Offline;

            int sourceInfoAreaIdPosition = destinationResult.MetaInfo.IndexOfResultInfoAreaIdLinkId(this.SourceParentInfoAreaId, -1);
            if (sourceInfoAreaIdPosition >= 0)
            {
                int i, count = destinationResult.RowCount;
                for (i = 0; i < count; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)destinationResult.ResultRowAtIndex(i);
                    string sourceRowRecordIdentification = row.RecordIdentificationAtIndex(sourceInfoAreaIdPosition);
                    if (!string.IsNullOrEmpty(sourceRowRecordIdentification))
                    {
                        UPSEDestinationParent destinationParent = new UPSEDestinationParent(sourceRowRecordIdentification, row.RootRecordIdentification, this);
                        this.AddDestinationParent(destinationParent);
                    }
                }
            }

            this.ReadDestination();
        }

        private void InitializeWithProductCatalogResult(UPCRMResult productCatalogResult)
        {
            int count = productCatalogResult.RowCount;
            List<DocumentData> documents = new List<DocumentData>();
            for (int i = 0; i < count; i++)
            {
                DocumentData data = this.productCatalogDocumentManager.DocumentDataForResultRow((UPCRMResultRow)productCatalogResult.ResultRowAtIndex(i));
                documents.Add(data);
            }

            this.ProductCatalogDocuments = documents;
            if (this.RootDetailsFieldControl != null)
            {
                this.ReadRootDetailsInfoPanel();
            }
            else
            {
                this.ReadPricing();
            }
        }

        private void InitializeWithRootDetailsInfoPanelResult(UPCRMResult destinationResult)
        {
            int count = destinationResult.RowCount;
            if (count > 1)
            {
                Logger.LogWarn("InitializeWithDestinationParentInfoPanelResult - Expected 1 row");
            }

            if (count > 0)
            {
                UPCRMResultRow row = (UPCRMResultRow)destinationResult.ResultRowAtIndex(0);
                this.SerialEntryParentInfoPanel = new UPSerialEntryParentInfoPanel(this.RootDetailsFieldControl, row);
            }

            this.ReadPricing();
        }

        /// <summary>
        /// Initializes the with destination result.
        /// </summary>
        /// <param name="destinationResult">The destination result.</param>
        /// <param name="existingPositionQuery">The existing position query.</param>
        protected virtual void InitializeWithDestinationResult(UPCRMResult destinationResult, UPContainerMetaInfo existingPositionQuery)
        {
            List<UPSEColumn> _rebateColumns = null;
            foreach (UPSEColumn col in this.Columns)
            {
                if (col.Function!= null && col.Function.StartsWith("Rebate") && col.Function?.Length > 6)
                {
                    if (_rebateColumns == null)
                    {
                        _rebateColumns = new List<UPSEColumn> { col };
                        //_rebateColumns.Add(col);
                    }
                    else
                    {
                        _rebateColumns.Add(col);
                    }
                }
            }

            this.RebateColumns = _rebateColumns;
            this.destinationRequestOption = destinationResult.IsServerResult ? UPRequestOption.Online : UPRequestOption.Offline;

            int parentInfoAreaPosition = -1;
            if (!string.IsNullOrEmpty(this.SourceParentInfoAreaId))
            {
                parentInfoAreaPosition = destinationResult.MetaInfo.IndexOfResultInfoAreaIdLinkId(this.SourceParentInfoAreaId, -1);
            }

            int listingInfoAreaPosition = -1;
            if (!string.IsNullOrEmpty(this.ListingInfoAreaId))
            {
                listingInfoAreaPosition = destinationResult.MetaInfo.IndexOfResultInfoAreaIdLinkId(this.ListingInfoAreaId, -1);
            }

            int count = destinationResult.RowCount;
            if (count > 0)
            {
                int sourceRecordPosition = existingPositionQuery.IndexOfResultInfoArea(this.sourceInfoAreaMetaInfo);
                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow resultRow = (UPCRMResultRow)destinationResult.ResultRowAtIndex(i);
                    UPSERow row = this.RowFromDestinationResultRow(resultRow, sourceRecordPosition, this.sourceFieldControlOffset);
                    if (row != null)
                    {
                        if (listingInfoAreaPosition >= 0)
                        {
                            string listingRecordId = resultRow.RecordIdAtIndex(listingInfoAreaPosition);
                            if (!string.IsNullOrEmpty(listingRecordId))
                            {
                                row.ListingRecordId = listingRecordId;
                            }
                        }

                        if (parentInfoAreaPosition >= 0)
                        {
                            row.RowParentRecordId = resultRow.RecordIdAtIndex(parentInfoAreaPosition);
                            UPSEDestinationParent destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                            if (destinationParent == null)
                            {
                                string sourceRowRecordIdentification = resultRow.RecordIdentificationAtIndex(parentInfoAreaPosition);
                                if (!string.IsNullOrEmpty(sourceRowRecordIdentification))
                                {
                                    destinationParent = new UPSEDestinationParent(sourceRowRecordIdentification, this);
                                    this.AddDestinationParent(destinationParent);
                                }
                            }

                            destinationParent.AddPosition(row);
                        }

                        this.AddPosition(row);
                    }
                }
            }

            UPContainerMetaInfo existingChildPositionQuery;
            if (this.DestChildFieldControl != null)
            {
                existingChildPositionQuery = new UPContainerMetaInfo(this.DestChildFieldControl);
                this.positionInfoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(this.DestFieldControl.InfoAreaId, -1);
                this.positionInfoAreaMetaInfo.ParentRelation = "WITH";
                existingChildPositionQuery.RootInfoAreaMetaInfo.AddTable(this.positionInfoAreaMetaInfo);
                existingChildPositionQuery.UpdateAfterAddingFieldMetaInfos(this.positionInfoAreaMetaInfo.Fields);
                this.sourceChildInfoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(this.SourceChildFieldControl.InfoAreaId, -1);
                this.sourceChildInfoAreaMetaInfo.ParentRelation = "WITH";
                existingChildPositionQuery.RootInfoAreaMetaInfo.AddTable(this.sourceChildInfoAreaMetaInfo);
                existingChildPositionQuery.SetLinkRecordIdentification(this.Record.RecordIdentification);
                existingChildPositionQuery.UpdateAfterAddingFieldMetaInfos(this.sourceChildInfoAreaMetaInfo.Fields);
                this.currentSearchOperation = existingChildPositionQuery;
                this.currentSearchOperationType = Constants.UPSESearchOperationDestinationChildren;
                this.currentRequestOption = this.isEnterprise ? this.RequestOption : this.destinationRequestOption;
                existingChildPositionQuery.Find(this.currentRequestOption, this, true);
            }
            else
            {
                this.InitializeWithDestinationChildResult(null, null);
            }
        }

        /// <summary>
        /// Sets the filter array from parameters.
        /// </summary>
        public void SetFilterArrayFromParameters()
        {
            List<UPSEFilter> filterarray = new List<UPSEFilter>();
            ViewReference _viewReference = this.Parameters["viewReference"] as ViewReference;

            for (int i = 1; i <= 6; i++)
            {
                string filterName = _viewReference.ContextValueForKey($"Filter{i}");
                if (!string.IsNullOrEmpty(filterName))
                {
                    UPSEFilter filter = UPSEFilter.FilterFromName(filterName, this.InitialFieldValuesForDestination);
                    if (filter != null)
                    {
                        filterarray.Add(filter);
                    }
                }
            }

            if (filterarray.Count == 0)
            {
                UPSEFilter filter = UPSEFilter.FilterFromName($"{this.SourceInfoAreaId}:All");
                if (filter != null)
                {
                    filterarray.Add(filter);
                }
            }

            this.Filters = filterarray;
        }

        private bool ApplyFixedConditionsOnSourceQuery(UPContainerMetaInfo sourceQuery)
        {
            if (this.fixedSourceFilter != null)
            {
                sourceQuery.ApplyFilter(this.fixedSourceFilter);
            }

            return true;
        }

        /// <summary>
        /// Rows from source result row listing.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="listing">The listing.</param>
        /// <returns></returns>
        public virtual UPSERow RowFromSourceResultRow(UPCRMResultRow row, UPSEListing listing)
        {
            return new UPSERow(row, listing, this);
        }

        /// <summary>
        /// Rows from destination result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <returns></returns>
        public virtual UPSERow RowFromDestinationResultRow(UPCRMResultRow row, int offset, int sourceFieldOffset)
        {
            return UPSERow.Create(row, offset, sourceFieldOffset, this);
        }

        /// <summary>
        /// Rows from source row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public virtual UPSERow RowFromSourceRow(UPSERow row)
        {
            return new UPSERow(row);
        }

        /// <summary>
        /// Rowses for initial filters.
        /// </summary>
        public void RowsForInitialFilters()
        {
            this.currentCheckedActiveFilterIndex = 0;
            this.checkActiveFilters = true;
            this.NextInitialFilter();
        }

        private static Dictionary<string, string> GetFunctionNamesWithValues(
            IDictionary<string, UPConfigFieldControlField> sourceFunctionMapping,
            UPCRMResultRow resultRow)
        {
            if (sourceFunctionMapping == null)
            {
                throw new ArgumentNullException(nameof(sourceFunctionMapping));
            }

            if (resultRow == null)
            {
                throw new ArgumentNullException(nameof(resultRow));
            }

            var functionNamesWithValues = new Dictionary<string, string>();
            foreach (var functionName in sourceFunctionMapping.Keys)
            {
                var field = sourceFunctionMapping[functionName];
                var value = resultRow.RawValueAtIndex(field.TabIndependentFieldIndex);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    functionNamesWithValues[functionName] = value;
                }
            }

            return functionNamesWithValues;
        }

        private void PositionsForFilterFinishedWithRows(List<UPSERow> rowArray)
        {
            if (this.TheDelegate != null)
            {
                if (this.ColumnSortInfo != null)
                {
                    rowArray = this.ColumnSortInfo.SortRows(rowArray);
                }

                this.TheDelegate.SerialEntryPositionsForFilterDidFinishWithSuccess(this, rowArray);
            }
        }

        private void ContinueApplyRowsFromResultForFilter(UPCRMResult filterResult, UPSEFilter filter)
        {
            List<UPSERow> result;
            bool isListing = false;
            Dictionary<string, UPConfigFieldControlField> sourceFunctionMapping = null;
            List<string> uniqueListingNames = null;
            this.LastResult = filterResult;
            List<UPSEListing> listings = null;
            if (filter.IsListingFilter && filterResult.RowCount > 0 && this.ListingController != null)
            {
                listings = this.ListingController.Listings;
                if (listings.Count > 0 || !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Listing.AllSourcesIfEmpty"))
                {
                    isListing = true;
                    sourceFunctionMapping = this.SourceFieldControl.FunctionNames();
                }
            }
            else if (!string.IsNullOrEmpty(this.currentListingOwner))
            {
                UPSEListingOwner owner = this.ListingController.OwnerForKey(this.currentListingOwner);
                listings = owner.AllListings.Values.ToList();
                if (listings.Count > 0)
                {
                    isListing = true;
                    sourceFunctionMapping = this.SourceFieldControl.FunctionNames();
                }
            }

            if (listings!=null && listings.Count > 0)
            {
                uniqueListingNames = this.ListingController.DistinctListingFunctionNames.Where(uniqueName => this.ColumnForFunctionName(uniqueName) != null).ToList();
                uniqueListingNames.AddRange(this.ListingController.DestinationFieldFunctionNames.Where(uniqueName => this.ColumnForFunctionName(uniqueName) != null));
            }

            result = this.GetRows(
                filterResult,
                isListing,
                sourceFunctionMapping,
                uniqueListingNames,
                listings);

            if (this.checkActiveFilters)
            {
                if (result.Count == 0)
                {
                    ++this.currentCheckedActiveFilterIndex;
                    this.NextInitialFilter();
                }
                else
                {
                    this.checkActiveFilters = false;
                    this.TheDelegate.SerialEntrySignalActivateInitialFilters(this, this.currentSearchFilters);
                }
            }

            if (this.currentSearchOperationType != Constants.UPSESearchRowsForAutoFill)
            {
                this.PositionsForFilterFinishedWithRows(result);
            }
            else
            {
                this.autoFill.Rows = result;
            }
        }

        private List<UPSERow> GetRows(
            UPCRMResult filterResult,
            bool isListing,
            IDictionary<string, UPConfigFieldControlField> sourceFunctionMapping,
            List<string> uniqueListingNames,
            List<UPSEListing> listings)
        {
            if (filterResult == null)
            {
                throw new ArgumentNullException(nameof(filterResult));
            }

            List<UPSERow> result;
            if (filterResult.RowCount == 0)
            {
                result = new List<UPSERow>();
            }
            else
            {
                int parentInfoAreaPosition;
                var resultArray = new List<UPSERow>(filterResult.RowCount);
                var count = filterResult.RowCount;
                if (!string.IsNullOrWhiteSpace(this.SourceParentInfoAreaId))
                {
                    parentInfoAreaPosition = filterResult.MetaInfo.IndexOfResultInfoAreaIdLinkId(this.SourceParentInfoAreaId, -1);
                }
                else
                {
                    parentInfoAreaPosition = -1;
                }

                for (var i = 0; i < count; i++)
                {
                    var resultRow = (UPCRMResultRow)filterResult.ResultRowAtIndex(i);
                    var loadedListingRecordIds = new Dictionary<string, UPSERow>();
                    var rowsWithoutListing = new Dictionary<string, UPSERow>();
                    var rowKeys = this.positionsForSourceKeyDictionary.ValueOrDefault(resultRow.RootRecordId);
                    var rowAdded = false;
                    if (rowKeys != null)
                    {
                        rowAdded = this.ProcessRows(
                            isListing,
                            uniqueListingNames,
                            resultArray,
                            resultRow,
                            loadedListingRecordIds,
                            rowsWithoutListing,
                            rowKeys,
                            rowAdded);
                    }

                    if (isListing)
                    {
                        var functionNamesWithValues = GetFunctionNamesWithValues(sourceFunctionMapping, resultRow);

                        var foundKeys = new Dictionary<string, UPSERow>();
                        var resultArrayCandidates = this.GetArrayCandidates(
                            uniqueListingNames,
                            listings,
                            resultArray,
                            resultRow,
                            loadedListingRecordIds,
                            functionNamesWithValues,
                            foundKeys);

                        foreach (var row in resultArrayCandidates)
                        {
                            this.ProcessRow(
                                uniqueListingNames,
                                parentInfoAreaPosition,
                                resultArray,
                                resultRow,
                                rowsWithoutListing,
                                foundKeys,
                                row);
                        }
                    }
                    else if (!rowAdded)
                    {
                        this.AddRow(parentInfoAreaPosition, resultArray, resultRow);
                    }
                }

                result = resultArray;
            }

            return result;
        }

        private bool ProcessRows(
            bool isListing,
            List<string> uniqueListingNames,
            List<UPSERow> resultArray,
            UPCRMResultRow resultRow,
            Dictionary<string, UPSERow> loadedListingRecordIds,
            Dictionary<string, UPSERow> rowsWithoutListing,
            List<string> rowKeys,
            bool rowAdded)
        {
            if (resultArray == null)
            {
                throw new ArgumentNullException(nameof(resultArray));
            }

            if (resultRow == null)
            {
                throw new ArgumentNullException(nameof(resultRow));
            }

            if (loadedListingRecordIds == null)
            {
                throw new ArgumentNullException(nameof(loadedListingRecordIds));
            }

            if (rowsWithoutListing == null)
            {
                throw new ArgumentNullException(nameof(rowsWithoutListing));
            }

            foreach (var rowKey in rowKeys)
            {
                var row = this.rowsWithData.ValueOrDefault(rowKey);
                if (row != null)
                {
                    if (!string.IsNullOrWhiteSpace(row.ListingRecordId))
                    {
                        loadedListingRecordIds.SetObjectForKey(row, row.ListingRecordId);
                    }
                    else
                    {
                        if (uniqueListingNames!=null && uniqueListingNames.Count > 0)
                        {
                            rowsWithoutListing[row.KeyForFunctionNameArray(uniqueListingNames)] = row;
                        }
                    }

                    if (!isListing)
                    {
                        resultArray.Add(row);
                        row.SourceResultRow = resultRow;
                        row.SourceResultOffset = 0;
                        row.SourceResult = resultRow.Result;
                        rowAdded = true;
                    }
                }
            }

            return rowAdded;
        }

        private List<UPSERow> GetArrayCandidates(
            List<string> uniqueListingNames,
            List<UPSEListing> listings,
            IList<UPSERow> resultArray,
            UPCRMResultRow resultRow,
            Dictionary<string, UPSERow> loadedListingRecordIds,
            Dictionary<string, string> functionNamesWithValues,
            Dictionary<string, UPSERow> foundKeys)
        {
            if (resultArray == null)
            {
                throw new ArgumentNullException(nameof(resultArray));
            }

            if (loadedListingRecordIds == null)
            {
                throw new ArgumentNullException(nameof(loadedListingRecordIds));
            }

            if (foundKeys == null)
            {
                throw new ArgumentNullException(nameof(foundKeys));
            }

            var currentListings = this.ListingController.ListingsMatchValues(listings, functionNamesWithValues);
            List<UPSERow> resultArrayCandidates = null;
            foreach (var listing in currentListings)
            {
                var row = loadedListingRecordIds.ValueOrDefault(listing.RecordId);
                if (row != null)
                {
                    resultArray.Add(row);
                    foundKeys.SetObjectForKey(row, row.KeyForFunctionNameArray(uniqueListingNames));
                }
                else
                {
                    row = this.RowFromSourceResultRow(resultRow, listing);
                    if (resultArrayCandidates != null)
                    {
                        resultArrayCandidates.Add(row);
                    }
                    else
                    {
                        resultArrayCandidates = new List<UPSERow> { row };
                    }
                }
            }

            return resultArrayCandidates;
        }

        private void ProcessRow(
            List<string> uniqueListingNames,
            int parentInfoAreaPosition,
            List<UPSERow> resultArray,
            UPCRMResultRow resultRow,
            Dictionary<string, UPSERow> rowsWithoutListing,
            Dictionary<string, UPSERow> foundKeys,
            UPSERow row)
        {
            if (resultArray == null)
            {
                throw new ArgumentNullException(nameof(resultArray));
            }

            if (resultRow == null)
            {
                throw new ArgumentNullException(nameof(resultRow));
            }

            if (rowsWithoutListing == null)
            {
                throw new ArgumentNullException(nameof(rowsWithoutListing));
            }

            if (foundKeys == null)
            {
                throw new ArgumentNullException(nameof(foundKeys));
            }

            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var key = row.KeyForFunctionNameArray(uniqueListingNames);
            if (!foundKeys.ContainsKey(key))
            {
                var existingRow = rowsWithoutListing.ValueOrDefault(key);
                if (existingRow != null)
                {
                    resultArray.Add(existingRow);
                    foundKeys.SetObjectForKey(existingRow, key);
                    return;
                }

                if (parentInfoAreaPosition >= 0)
                {
                    row.RowParentRecordId = resultRow.RecordIdAtIndex(parentInfoAreaPosition);
                    var destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                    if (destinationParent == null)
                    {
                        destinationParent = new UPSEDestinationParent(resultRow.RecordIdentificationAtIndex(parentInfoAreaPosition), this);
                        this.AddDestinationParent(destinationParent);
                    }
                }

                resultArray.Add(row);
                row.SourceResultRow = resultRow;
                row.SourceResult = resultRow.Result;
                row.SourceResultOffset = 0;
                foundKeys.SetObjectForKey(row, key);
            }
        }

        private void AddRow(int parentInfoAreaPosition, List<UPSERow> resultArray, UPCRMResultRow resultRow)
        {
            if (resultArray == null)
            {
                throw new ArgumentNullException(nameof(resultArray));
            }

            if (resultRow == null)
            {
                throw new ArgumentNullException(nameof(resultRow));
            }

            var row = this.RowFromSourceResultRow(resultRow, null);
            if (!row.HideRow())
            {
                if (parentInfoAreaPosition >= 0)
                {
                    row.RowParentRecordId = resultRow.RecordIdAtIndex(parentInfoAreaPosition);
                    var destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                    if (destinationParent == null)
                    {
                        destinationParent = new UPSEDestinationParent(resultRow.RecordIdentificationAtIndex(parentInfoAreaPosition), this);
                        this.AddDestinationParent(destinationParent);
                    }
                }

                resultArray.Add(row);
            }
        }

        private void LoadAdditionalInformationForSourceResultFilter(UPCRMResult result, UPSEFilter filter)
        {
            this.ContinueApplyRowsFromResultForFilter(result, filter);
        }

        private void ResultForCrmQuery(UPContainerMetaInfo positionCrmQuery, UPSEFilter seFilter, string searchText, List<UPConfigFilter> searchFilter, int searchOperationType)
        {
            if (!string.IsNullOrEmpty(searchText) && this.SourceSearchControl != null)
            {
                positionCrmQuery.SetSearchConditionsFor(searchText, this.SourceSearchControl.AllCRMFields, true);
            }

            bool ok = this.ApplyFixedConditionsOnSourceQuery(positionCrmQuery);
            if (ok && seFilter != null && !seFilter.IsListingFilter)
            {
                ok = seFilter.ApplyFilterOnSourceQueryParameters(positionCrmQuery, this.InitialFieldValuesForDestination);
            }

            this.currentListingOwner = null;
            List<UPConfigFilter> currentFilters = new List<UPConfigFilter>();
            if (ok && searchFilter.Count > 0)
            {
                foreach (UPConfigFilter filter in searchFilter)
                {
                    if (!(filter is UPConfigSpecialFilter))
                    {
                        UPConfigFilter replacedFilter = filter.FilterByApplyingValueDictionary(this.InitialFieldValuesForDestination);
                        if (replacedFilter != null)
                        {
                            positionCrmQuery.ApplyFilter(replacedFilter);
                            currentFilters.Add(replacedFilter);
                        }
                    }
                    else
                    {
                        UPConfigSpecialFilter specialFilter = (UPConfigSpecialFilter)filter;
                        if (specialFilter.UnitName == "Listing")
                        {
                            this.currentListingOwner = specialFilter.Parameter as string;
                            currentFilters.Add(specialFilter);
                        }
                    }

                    if (!ok)
                    {
                        break;
                    }
                }
            }

            this.currentSearchFilters = currentFilters;
            this.FieldGroupDecider = null;
            if (this.FieldGroupDeciderFilter != null)
            {
                UPConfigFilter filter = this.FieldGroupDeciderFilter.FilterByApplyingValueDictionaryDefaults(this.InitialFieldValuesForDestination, true);
                if (filter != null)
                {
                    this.FieldGroupDecider = new UPCRMFilterBasedDecision(filter);
                    positionCrmQuery.AddCrmFields(this.FieldGroupDecider.FieldDictionary.Values.ToList());
                    this.FieldGroupDecider.UseCrmQuery(positionCrmQuery);
                    this.rowConfigurations = null;
                }
            }

            if (!ok)
            {
                this.TheDelegate?.SerialEntryDidFailWithError(this, new Exception("query error"));
            }
            else
            {
                this.currentSearchOperationType = searchOperationType;
                this.currentSearchOperation = positionCrmQuery;
                this.currentRequestOption = this.isEnterprise ? this.RequestOption : this.sourceRequestOption;
                this.currentFilter = seFilter;
                this.currentQueuedSearchOperation = positionCrmQuery.Find(this.currentRequestOption, this, true);
            }
        }

        private void NextInitialFilter()
        {
            if (this.currentCheckedActiveFilterIndex >= this.ActiveFilter.Count())
            {
                this.checkActiveFilters = false;
                this.AutoCreatePositions = false;
                this.RowsForFilter(UPSEFilter.AllPositionsFilter(), string.Empty, null);
                return;
            }

            if (this.currentCheckedActiveFilterIndex > 0)
            {
                this.AutoCreatePositions = false;
            }

            string filterName = this.ActiveFilter[this.currentCheckedActiveFilterIndex];
            UPSEFilter allFilter = new UPSEFilter(this.sourceInfoAreaMetaInfo.InfoAreaId, null);
            if (filterName.Contains(":Listing"))
            {
                if (this.ListingController.ListingOwner.AllListings.Count == 0)
                {
                    ++this.currentCheckedActiveFilterIndex;
                    this.NextInitialFilter();
                    return;
                }

                UPConfigFilter filter = new UPConfigSpecialFilter("Listing", this.ListingController.ListingOwner.ActualListingOwner.RecordIdentification);
                this.RowsForFilter(allFilter, string.Empty, new List<UPConfigFilter> { filter });
                return;
            }

            if (filterName.Contains(":Positions"))
            {
                this.AutoCreatePositions = false;
                this.checkActiveFilters = false;
                this.RowsForFilter(UPSEFilter.AllPositionsFilter(), string.Empty, null);
                return;
            }

            UPConfigFilter filter1 = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            filter1 = filter1?.FilterByApplyingValueDictionaryDefaults(this.InitialFieldValuesForDestination, true);

            if (filter1 == null)
            {
                ++this.currentCheckedActiveFilterIndex;
                this.NextInitialFilter();
                return;
            }

            this.RowsForFilter(allFilter, string.Empty, new List<UPConfigFilter> { filter1 });
        }

        private void RowsForItemNumbers(List<string> itemNumbers)
        {
            this.currentQueuedSearchOperation.Cancel();
            UPContainerMetaInfo positionCrmQuery = new UPContainerMetaInfo(this.SourceFieldControl);
            if (itemNumbers.Count < 20 && this.autoFill.ItemNumberSource != null)
            {
                UPSESourceColumn col = this.SourceColumnsForFunction[this.autoFill.ItemNumberSource];
                UPInfoAreaCondition cond = null;
                foreach (string itemNumber in itemNumbers)
                {
                    UPInfoAreaConditionLeaf leafCond = new UPInfoAreaConditionLeaf(col.InfoAreaId, col.FieldId, itemNumber);
                    cond = cond == null ? leafCond : cond.InfoAreaConditionByAppendingOrCondition(leafCond);
                }

                if (cond != null)
                {
                    positionCrmQuery.RootInfoAreaMetaInfo.Condition = positionCrmQuery.RootInfoAreaMetaInfo.Condition != null
                        ? positionCrmQuery.RootInfoAreaMetaInfo.Condition.InfoAreaConditionByAppendingAndCondition(cond) : cond;
                }
            }

            this.ResultForCrmQuery(positionCrmQuery, null, null, null, Constants.UPSESearchRowsForAutoFill);
        }

        /// <summary>
        /// Rowses for filter search text search filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="searchFilter">The search filter.</param>
        public void RowsForFilter(UPSEFilter filter, string searchText, List<UPConfigFilter> searchFilter)
        {
            this.currentQueuedSearchOperation?.Cancel();
            if (filter.SpecialFilterName == "AllPositions")
            {
                this.PositionsForFilterFinishedWithRows(this.positions);
                return;
            }

            if (filter.SpecialFilterName == "Errors")
            {
                List<UPSERow> errorRows = this.positions.Where(row => row.Error != null).ToList();

                this.PositionsForFilterFinishedWithRows(errorRows);
                return;
            }

            if (searchText?.Length < this.minSearchTextLength && filter.CrmFilter == null && !filter.IsListingFilter)
            {
                this.PositionsForFilterFinishedWithRows(new List<UPSERow>());
                return;
            }

            UPContainerMetaInfo positionCrmQuery = new UPContainerMetaInfo(this.SourceFieldControl);
            positionCrmQuery.ReplaceCaseSensitiveCharacters = ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Search.ReplaceCaseSensitiveCharacters");
            this.ResultForCrmQuery(positionCrmQuery, filter, searchText, searchFilter, Constants.UPSESearchRowsForFilter);
        }

        /// <summary>
        /// Existings the row for record identification.
        /// </summary>
        /// <param name="serialEntryRecordIdentification">The serial entry record identification.</param>
        /// <returns></returns>
        public UPSERow ExistingRowForRecordIdentification(string serialEntryRecordIdentification)
        {
            string serialEntryRecordId = serialEntryRecordIdentification.RecordId();
            return this.rowsWithData.Values.FirstOrDefault(row => row.SerialEntryRecordId == serialEntryRecordId);
        }

        /// <summary>
        /// Duplicates the source row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPSERow DuplicateSourceRow(UPSERow row)
        {
            return this.RowFromSourceRow(row);
        }

        /// <summary>
        /// Creates the row for source record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPSERow CreateRowForSourceRecordIdentification(string recordIdentification)
        {
            UPContainerMetaInfo positionCrmQuery = new UPContainerMetaInfo(this.SourceFieldControl);
            if (!this.ApplyFixedConditionsOnSourceQuery(positionCrmQuery))
            {
                return null;
            }

            positionCrmQuery.SetLinkRecordIdentification(recordIdentification);
            UPCRMResult result = positionCrmQuery.Find();
            if (result.RowCount == 1)
            {
                UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(0);
                UPSERow row = this.RowFromSourceResultRow(resultRow, null);
                int parentInfoAreaPosition = -1;
                if (!string.IsNullOrEmpty(this.SourceParentInfoAreaId))
                {
                    parentInfoAreaPosition = result.MetaInfo.IndexOfResultInfoAreaIdLinkId(this.SourceParentInfoAreaId, -1);
                }

                if (parentInfoAreaPosition >= 0)
                {
                    row.RowParentRecordId = resultRow.RecordIdAtIndex(parentInfoAreaPosition);
                    UPSEDestinationParent destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                    if (destinationParent == null)
                    {
                        destinationParent = new UPSEDestinationParent(resultRow.RecordIdentificationAtIndex(parentInfoAreaPosition), this);
                        this.AddDestinationParent(destinationParent);
                    }
                }

                return row;
            }

            return null;
        }

        /// <summary>
        /// Determines whether [is step size column] [the specified column].
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>
        ///   <c>true</c> if [is step size column] [the specified column]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStepSizeColumn(UPSEColumn column)
        {
            return column.Function.Contains("uantity") || column.FieldConfig.Attributes.ExtendedOptionIsSet("StepSizeCheck");
        }

        /// <summary>
        /// Determines whether [is minimum maximum column] [the specified column].
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>
        ///   <c>true</c> if [is minimum maximum column] [the specified column]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMinMaxColumn(UPSEColumn column)
        {
            return column.Function.Contains("uantity") || column.FieldConfig.Attributes.ExtendedOptionIsSet("MinMaxCheck");
        }

        private void ApplyRowsForFilter(UPCRMResult filterResult, UPSEFilter filter)
        {
            if (filterResult.RowCount == 0 && this.checkActiveFilters)
            {
                ++this.currentCheckedActiveFilterIndex;
                this.NextInitialFilter();
                return;
            }

            this.LoadAdditionalInformationForSourceResultFilter(filterResult, filter);
        }

        private UPSERow DeletedRowWithKey(string key)
        {
            lock (this.deletedRows)
            {
                return (this.deletedRows?.Count > 0) ? this.deletedRows[key] : null;
            }
        }

        private bool RemoveDeleteRowWithKey(string key)
        {
            lock (this.deletedRows)
            {
                if (this.deletedRows.ContainsKey(key))
                {
                    this.deletedRows.Remove(key);
                    return true;
                }

                return false;
            }
        }

        private void AddDeletedRow(UPSERow row)
        {
            lock (this.deletedRows)
            {
                this.deletedRows.SetObjectForKey(row, row.RowKey);
            }
        }

        private List<UPSERow> AllDeletedRows()
        {
            lock (this.deletedRows)
            {
                return this.deletedRows?.Count > 0 ? new List<UPSERow>(this.deletedRows.Values) : new List<UPSERow>();
            }
        }

        /// <summary>
        /// Forces the create.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPSERowOperation ForceCreate(UPSERow row)
        {
            UPSERowOperation rowOperation = row.ForceCreate();
            if (rowOperation == UPSERowOperation.Add)
            {
                this.RemoveDeleteRowWithKey(row.RowKey);
                this.AddPosition(row);
            }

            this.SaveAllExecuted = false;
            return rowOperation;
        }

        /// <summary>
        /// Updates the row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="value">The value.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="withChecks">if set to <c>true</c> [with checks].</param>
        /// <param name="affectedRows">The affected rows.</param>
        /// <returns></returns>
        public UPSERowOperation UpdateRow(UPSERow row, string value, int columnIndex, bool withChecks, List<UPSERow> affectedRows)
        {
            if (withChecks)
            {
                int newIntValue = Convert.ToInt32(value);
                int correctedValue = newIntValue;
                int maxCount = -1;
                if ((this.Quota?.AutoCorrectQuota ?? false) && !row.UnlimitedQuota && (this.Quota?.IsQuotaColumnIndex(columnIndex) ?? false))
                {
                    int remainingQuota = row.RemainingQuota;
                    if (remainingQuota < 0)
                    {
                        if (-remainingQuota > newIntValue)
                        {
                            correctedValue = 0;
                        }
                        else
                        {
                            correctedValue += remainingQuota;
                        }
                    }
                }

                UPSEColumn column = this.Columns[columnIndex];
                if (!this.DisableAutoCorrectPackageSize && this.IsStepSizeColumn(column))
                {
                    int stepSize = row.StepSizeForColumnIndex(columnIndex);
                    if (stepSize > 1)
                    {
                        int modulo = newIntValue % stepSize;
                        if (modulo > 0)
                        {
                            correctedValue += stepSize - modulo;
                            if (maxCount >= 0 && maxCount < correctedValue)
                            {
                                correctedValue -= stepSize;
                            }
                        }
                    }
                }

                if (!this.DisableAutoCorrectMinMax && this.IsMinMaxColumn(column))
                {
                    if (row.MinQuantity > 0 && correctedValue < row.MinQuantity)
                    {
                        correctedValue = row.MinQuantity;
                    }
                    else if (row.MaxQuantity > 0 && correctedValue > row.MaxQuantity)
                    {
                        correctedValue = row.MaxQuantity;
                    }
                }

                if (newIntValue != correctedValue)
                {
                    value = correctedValue.ToString();
                }
            }

            UPSERowOperation rowOperation = row.NewValueForColumnIndexReturnAffectedRows(value, columnIndex, affectedRows);
            if (rowOperation == UPSERowOperation.Remove)
            {
                if (row.SerialEntryRecordId != null && row.SerialEntryRecordId != "new")
                {
                    this.AddDeletedRow(row);
                }

                row.Deleted = true;
                this.RemovePosition(row);
            }
            else if (rowOperation == UPSERowOperation.Add)
            {
                this.RemoveDeleteRowWithKey(row.RowKey);
                this.AddPosition(row);
            }

            this.SaveAllExecuted = false;
            return rowOperation;
        }

        /// <summary>
        /// Rows the line parts for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public List<string> RowLinePartsForRow(UPSERow row)
        {
            if (this.RowDisplayListFormatter == null)
            {
                return null;
            }

            int partCount = this.RowDisplayListFormatter.PositionCount;
            List<string> arr = new List<string>(partCount);
            for (int i = 0; i < partCount; i++)
            {
                string value = this.RowDisplayListFormatter.StringFromProviderForPosition(row, i) ?? string.Empty;
                arr.Add(value);
            }

            return arr;
        }

        private void ReadAdditionalSourceInformation()
        {
            this.AdditionalItemInformations.LoadWithRequestOption(this.sourceRequestOption);
        }

        private void ReadSourceChildren()
        {
            if (this.SourceChildFieldControl != null)
            {
                UPContainerMetaInfo sourceChildQuery = new UPContainerMetaInfo(this.SourceChildFieldControl);
                sourceChildQuery.SetLinkRecordIdentification(this.Record?.RecordIdentification);
                this.currentSearchOperationType = Constants.UPSESearchOperationSourceChildren;
                this.currentRequestOption = this.isEnterprise ? this.RequestOption : this.destinationRequestOption;
                sourceChildQuery.Find(this.currentRequestOption, this, false);
            }
            else
            {
                this.InitializeFromSource(null);
            }
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string sourceCopyFieldGroup = this.viewReference.ContextValueForKey("SourceCopyFieldGroup");
            if (!string.IsNullOrEmpty(sourceCopyFieldGroup))
            {
                FieldControl fieldControl = configStore.FieldControlByNameFromGroup("List", sourceCopyFieldGroup);
                if (fieldControl != null)
                {
                    this.copyFields = new UPCopyFields(fieldControl);
                }
            }

            if (this.copyFields != null)
            {
                this.copyFields.CopyFieldValuesForRecord(this.Record, false, this);
            }
            else
            {
                this.ContinueBuildWithCopyFieldDictionary(null);
            }
        }

        /// <summary>
        /// Method Builds Serial Entry with <see cref="UPCRMRecord"/> root object.
        /// </summary>
        /// <param name="rootRecord">
        /// <see cref="UPCRMRecord"/> root record
        /// </param>
        private void ContinueBuildWithRootRecord(UPCRMRecord rootRecord)
        {
            linkReader = null;
            var configName = viewReference.ContextValueForKey(KeySourceChildConfigName);
            var configStore = ConfigurationUnitStore.DefaultStore;
            Record = rootRecord;

            SetSourceChildFieldControl(configStore, configName);

            SetDestChildFieldControl(configStore, configName);

            PopulateEditTriggerFilterArray();

            SetRootDetailsFieldControl(configStore, configName);

            SetSourceRequestOption();

            SetDestinationRequestOption();

            SetSerialEntryRowInfoPanels();

            SetSerialEntryRowDocuments();

            SetDestParentInfoAreaId(configStore);

            SetSourceParentInfoAreaId();

            SetListingInfoAreaId(configStore);

            SetPricingConfiguration(configStore);

            SetQuickConfiguration(configStore);

            SetRowDisplayListFormatter(configStore);

            SetSumLineListFormatter(configStore);

            SetAdditionalOptionProperties();

            SetActiveFilter();

            SetComputeRowOnEveryColumn(configStore);

            SetAdditionalItemInformations();

            SetRootFilterTemplateForStartSerialEntry(configStore);

            SetRootFilterTemplateForEndSerialEntry(configStore);

            SetFieldGroupDeciderFilter(configStore);

            SetProductCatalogDocumentManager(configStore);

            if (AdditionalItemInformations != null)
            {
                ReadAdditionalSourceInformation();
            }
            else
            {
                ReadSourceChildren();
            }

            SetDestinationTemplateFilter(configStore);

            SetDestinationChildTemplateFilter(configStore);

            SetDestinationParentTemplateFilter(configStore);

            SetDestinationTemplateFilter();

            SetDestinationChildTemplateFilter();

            SetDestinationParentTemplateFilter();
        }

        /// <summary>
        /// Serials the type of the entry of.
        /// </summary>
        /// <param name="serialEntryType">Type of the serial entry.</param>
        /// <param name="record">The record.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="offlineRequest">The offline request.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPSerialEntry SerialEntryOfType(SerialEntryType serialEntryType, UPCRMRecord record,
            Dictionary<string, object> parameters, UPOfflineSerialEntryRequest offlineRequest, UPSerialEntryDelegate theDelegate)
        {
            if (serialEntryType == SerialEntryType.Order || serialEntryType == SerialEntryType.Offer)
            {
                return new UPSEOrder(record, parameters, offlineRequest, theDelegate);
            }

            if (serialEntryType == SerialEntryType.POS)
            {
                return new UPSEPOS(record, parameters, offlineRequest, theDelegate);
            }

            return new UPSerialEntry(record, parameters, offlineRequest, theDelegate);
        }

        /// <summary>
        /// Serials the type of the entry of.
        /// </summary>
        /// <param name="serialEntryType">Type of the serial entry.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="request">The request.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPSerialEntry SerialEntryOfType(SerialEntryType serialEntryType, string recordIdentification,
            Dictionary<string, object> parameters, UPOfflineSerialEntryRequest request, UPSerialEntryDelegate theDelegate)
        {
            UPCRMRecord record = null;
            if (!string.IsNullOrEmpty(recordIdentification))
            {
                record = new UPCRMRecord(recordIdentification);
            }

            return SerialEntryOfType(serialEntryType, record, parameters, request, theDelegate);
        }

        /// <summary>
        /// Updateds the root record with template filter.
        /// </summary>
        /// <param name="templateFilter">The template filter.</param>
        /// <returns></returns>
        public virtual UPCRMRecord UpdatedRootRecordWithTemplateFilter(UPConfigFilter templateFilter)
        {
            if (string.IsNullOrEmpty(this.Record.RecordIdentification))
            {
                return null;
            }

            List<string> recordOptions = null;
            string recordOptionString = null;
            if (!string.IsNullOrEmpty(this.SyncStrategy))
            {
                recordOptions = new List<string> { "syncStrategy", this.SyncStrategy };
            }

            if (recordOptions != null)
            {
                recordOptionString = StringExtensions.StringFromObject(recordOptions);
            }

            if (templateFilter != null)
            {
                UPCRMRecord rootRecord = new UPCRMRecord(this.Record.RecordIdentification, "Update", recordOptionString);
                rootRecord.ApplyValuesFromTemplateFilter(templateFilter);
                return rootRecord;
            }

            return new UPCRMRecord(this.Record.RecordIdentification, "Sync", recordOptionString);
        }

        /// <summary>
        /// Determines whether this instance has changes.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </returns>
        public bool HasChanges()
        {
            if (this.SaveAllExecuted)
            {
                return false;
            }

            if (this.destinationParents != null)
            {
                foreach (UPSEDestinationParent parent in this.destinationParents.Values)
                {
                    UPCRMRecord changedRecord = parent.ChangedRecord();
                    if (changedRecord != null)
                    {
                        return true;
                    }
                }
            }

            if (this.RootFilterTemplateForEndSerialEntry != null && this.StartSerialEntryFilterApplied)
            {
                return true;
            }

            foreach (UPSERow currentRow in this.rowsWithData.Values)
            {
                UPCRMRecord parentRecord = null;
                if (!string.IsNullOrEmpty(currentRow.RowParentRecordId))
                {
                    UPSEDestinationParent parent = this.destinationParents.ValueOrDefault(currentRow.RowParentRecordId);
                    if (parent != null)
                    {
                        parentRecord = parent.DestinationRecord;
                    }
                }

                List<UPCRMRecord> changedRecordsForRow = currentRow.ChangedChildRecordsForRootRecordParentRecord(this.Record, parentRecord);
                if (changedRecordsForRow.Count > 0)
                {
                    return true;
                }
            }

            foreach (UPSERow currentRow in this.AllDeletedRows())
            {
                List<UPCRMRecord> changedRecordsForRow = this.ChangedChildRecordsForDeleteRow(currentRow);
                if (changedRecordsForRow.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Changeds the child records with changed rows.
        /// </summary>
        /// <param name="returnChangedRows">The return changed rows.</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedChildRecordsWithChangedRows(List<UPSERow> returnChangedRows)
        {
            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>();
            if (this.SaveAllExecuted && !this.ConflictHandling)
            {
                return null;
            }

            if (this.destinationParents != null)
            {
                foreach (UPSEDestinationParent parent in this.destinationParents.Values)
                {
                    UPCRMRecord changedRecord = parent.ChangedRecord();
                    if (changedRecord != null)
                    {
                        changedRecords.Add(changedRecord);
                    }
                }
            }

            List<UPSERow> rowArray = this.positions;
            foreach (UPSERow currentRow in rowArray)
            {
                UPCRMRecord parentRecord = null;
                if (!string.IsNullOrEmpty(currentRow.RowParentRecordId))
                {
                    UPSEDestinationParent parent = this.destinationParents.ValueOrDefault(currentRow.RowParentRecordId);
                    if (parent != null)
                    {
                        parentRecord = parent.DestinationRecord;
                    }
                }

                List<UPCRMRecord> changedRecordsForRow = currentRow.ChangedChildRecordsForRootRecordParentRecord(this.Record, parentRecord);
                if (changedRecordsForRow?.Count > 0)
                {
                    changedRecords.AddRange(changedRecordsForRow);
                    returnChangedRows?.Add(currentRow);
                }
            }

            foreach (UPSERow currentRow in this.AllDeletedRows())
            {
                List<UPCRMRecord> changedRecordsForRow = this.ChangedChildRecordsForDeleteRow(currentRow);
                if (changedRecordsForRow?.Count > 0)
                {
                    changedRecords.AddRange(changedRecordsForRow);
                    returnChangedRows?.Add(currentRow);
                }
            }

            if (changedRecords?.Count > 0 || this.rowsChanged)
            {
                List<UPCRMRecord> quotaRecordChanges = this.Quota?.ChangedRecords();
                if (quotaRecordChanges?.Count > 0)
                {
                    changedRecords.AddRange(quotaRecordChanges);
                }
            }

            bool startTemplateFilterApplied = this.StartSerialEntryFilterApplied;
            if (changedRecords?.Count > 0 && !startTemplateFilterApplied && this.RootFilterTemplateForStartSerialEntry != null)
            {
                UPCRMRecord rootRecord = this.UpdatedRootRecordWithTemplateFilter(this.RootFilterTemplateForStartSerialEntry);
                if (rootRecord != null)
                {
                    List<UPCRMRecord> cr = changedRecords;
                    changedRecords = new List<UPCRMRecord> { rootRecord };
                    changedRecords.AddRange(cr);
                }

                startTemplateFilterApplied = true;
            }

            UPCRMRecord rootRecord1 = this.UpdatedRootRecordWithTemplateFilter(startTemplateFilterApplied ? this.RootFilterTemplateForEndSerialEntry : null);
            if (rootRecord1 != null)
            {
                changedRecords.Add(rootRecord1);
            }

            if (changedRecords.Count > 0)
            {
                return changedRecords;
            }

            return null;
        }

        /// <summary>
        /// Changeds the records for ending serial entry.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecordsForEndingSerialEntry()
        {
            bool startTemplateFilterApplied = this.StartSerialEntryFilterApplied;
            UPCRMRecord rootRecord = this.UpdatedRootRecordWithTemplateFilter(startTemplateFilterApplied ? this.RootFilterTemplateForEndSerialEntry : null);
            if (rootRecord != null)
            {
                return new List<UPCRMRecord> { rootRecord };
            }

            return null;
        }

        /// <summary>
        /// Changeds the child records for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedChildRecordsForRow(UPSERow row)
        {
            if (row == null)
            {
                return this.ChangedChildRecordsWithChangedRows(null);
            }

            List<UPCRMRecord> changedRecords = null;
            if (this.DeletedRowWithKey(row.RowKey) != null)
            {
                changedRecords = this.ChangedChildRecordsForDeleteRow(row);
                if (changedRecords == null)
                {
                    this.RemoveDeleteRowWithKey(row.RowKey);
                }
            }
            else if (this.rowsWithData.ContainsKey(row.RowKey))
            {
                UPCRMRecord parentChangeRecord = null;
                UPCRMRecord parentRecord = null;
                if (!string.IsNullOrEmpty(row.RowParentRecordId))
                {
                    UPSEDestinationParent destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                    if (destinationParent != null)
                    {
                        parentChangeRecord = destinationParent.ChangedRecord();
                    }

                    parentRecord = destinationParent.DestinationRecord;
                }

                changedRecords = row.ChangedChildRecordsForRootRecordParentRecord(this.Record, parentRecord);
                if (parentChangeRecord != null)
                {
                    if (changedRecords == null)
                    {
                        changedRecords = new List<UPCRMRecord> { parentChangeRecord };
                    }
                    else
                    {
                        List<UPCRMRecord> arr = new List<UPCRMRecord> { parentChangeRecord };
                        arr.AddRange(changedRecords);
                        changedRecords = arr;
                    }
                }
            }

            if (changedRecords?.Count > 0)
            {
                if (this.RootFilterTemplateForStartSerialEntry != null && !this.StartSerialEntryFilterApplied)
                {
                    UPCRMRecord rootRecord1 = new UPCRMRecord(this.Record.RecordIdentification, "Update", null);
                    rootRecord1.ApplyValuesFromTemplateFilter(this.RootFilterTemplateForStartSerialEntry);
                    List<UPCRMRecord> cr = new List<UPCRMRecord> { rootRecord1 };
                    cr.AddRange(changedRecords);
                    changedRecords = cr;
                }

                this.rowsChanged = true;
                UPCRMRecord rootRecord = this.UpdatedRootRecordWithTemplateFilter(null);
                if (rootRecord != null)
                {
                    List<UPCRMRecord> changedRecordArray = new List<UPCRMRecord>(changedRecords);
                    changedRecordArray.Add(rootRecord);
                    changedRecords = changedRecordArray;
                }

                return changedRecords;
            }

            return null;
        }

        /// <summary>
        /// Starts the next request.
        /// </summary>
        public void StartNextRequest()
        {
            UPSerialEntryRequest request = null;
            lock (this.requestQueue)
            {
                if (this.requestQueue?.Count > 0)
                {
                    request = this.requestQueue[0];
                    this.currentRequest = request;
                    this.requestQueue.Remove(request);
                    this.HasRunningChangeRequests = true;
                }
                else
                {
                    this.currentRequest = null;
                    this.HasRunningChangeRequests = false;
                }
            }

            request?.Start();
        }

        /// <summary>
        /// Submits the request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void SubmitRequest(UPSerialEntryRequest request)
        {
            bool submitRequest;
            lock (this.requestQueue)
            {                
                submitRequest = this.requestQueue.Count == 0 && this.currentRequest != request;
                this.requestQueue.Add(request);
            }

            if (submitRequest)
            {
                this.StartNextRequest();
            }
        }

        /// <summary>
        /// Handles the row changes result context save all.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="result">The result.</param>
        /// <param name="context">The context.</param>
        /// <param name="saveAll">if set to <c>true</c> [save all].</param>
        public void HandleRowChangesResultContextSaveAll(List<UPSERow> rows, object result, object context, bool saveAll)
        {
            foreach (UPSERow row in rows)
            {
                this.RefreshRow(row);
            }

            UPSERow reportRow = rows.Count == 1 ? rows[0] : null;
            if (reportRow != null)
            {
                List<UPSERow> dependentPositions = reportRow.DependentRows(this.positions);
                if (dependentPositions != null)
                {
                    List<UPSERow> bundlePositions = dependentPositions.ToList();
                    bundlePositions.Add(reportRow);
                    foreach (UPSERow depRow in dependentPositions)
                    {
                        depRow.RowPricing.UpdateCurrentConditionsWithPositions(bundlePositions);
                        depRow.ComputeRowWithConditionsWithDependent(false);
                    }
                }
            }

            this.StartSerialEntryFilterApplied = !saveAll;
            this.TheDelegate?.SerialEntryRowChangedWithSuccessContext(this, reportRow, context);
        }

        /// <summary>
        /// Handles the photo uploaded context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public void HandlePhotoUploadedContext(UPSERow row, object context)
        {
            this.TheDelegate.SerialEntryRowPhotoUploadedContext(this, row, context);
        }

        /// <summary>
        /// Handles the row delete result context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="result">The result.</param>
        /// <param name="context">The context.</param>
        public void HandleRowDeleteResultContext(UPSERow row, object result, object context)
        {
            row.HandleDeleted();
            if (this.positions.Contains(row))
            {
                this.RemovePosition(row);
            }

            this.RemoveDeleteRowWithKey(row.RowKey);
            List<UPSERow> dependentPositions = null;
            if (this.Pricing.HasOverallDiscount && this.CheckOverallPriceWithRow(null))
            {
                dependentPositions = this.positions;
                foreach (UPSERow depRow in dependentPositions)
                {
                    depRow.ClearDiscountInfo();
                    depRow.ApplyOverallDiscount(this.OverallDiscountActive);
                }
            }
            else
            {
                dependentPositions = row.DependentRows(this.positions);

                if (dependentPositions != null)
                {
                    foreach (UPSERow depRow in dependentPositions)
                    {
                        depRow.ClearDiscountInfo();
                        depRow.RowPricing.UpdateCurrentConditionsWithPositions(dependentPositions);
                        depRow.ComputeRowWithConditionsWithDependent(false);
                    }
                }
            }

            this.TheDelegate?.SerialEntryRowDeletedWithSuccess(this, row, dependentPositions, context);
        }

        /// <summary>
        /// Unblocks the offline request with up synchronize.
        /// </summary>
        /// <param name="upSync">if set to <c>true</c> [up synchronize].</param>
        public void UnblockOfflineRequestWithUpSync(bool upSync)
        {
            UPOfflineStorage.DefaultStorage.BlockingRequest = null;
            if (!upSync)
            {
                upSync = false;
            }

            if (upSync && !this.ConflictHandling && this.offlineRequest?.RequestNr > 0)
            {
                UPSyncManager syncManager = ServerSession.CurrentSession.SyncManager;
                syncManager.PerformUpSync();
            }
        }

        /// <summary>
        /// Handles the row unchanged context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public void HandleRowUnchangedContext(UPSERow row, object context)
        {
            this.TheDelegate?.SerialEntryNoChangesInRowContext(this, row, context);
        }

        /// <summary>
        /// Handles the row delete unchanged context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public void HandleRowDeleteUnchangedContext(UPSERow row, object context)
        {
            row.HandleDeleted();
            if (this.positions.Contains(row))
            {
                this.RemovePosition(row);
            }

            this.RemoveDeleteRowWithKey(row.RowKey);
            this.TheDelegate?.SerialEntryRowDeletedWithSuccess(this, row, null, context);
        }

        /// <summary>
        /// Handles the row error context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="_error">The error.</param>
        /// <param name="context">The context.</param>
        public void HandleRowErrorContext(UPSERow row, Exception _error, object context)
        {
            if (_error != null)
            {
                row?.HandleError(_error);
            }

            this.TheDelegate?.SerialEntryRowErrorContext(this, row, _error, context);
        }

        /// <summary>
        /// Deletes the row context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool DeleteRowContext(UPSERow row, object context)
        {
            if (this.DisableSingleRowUpdate)
            {
                return false;
            }

            this.SubmitRequest(new UPSerialEntryDeleteRowRequest(row, context));
            return true;
        }

        /// <summary>
        /// Saves the rows context.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool SaveRowsContext(List<UPSERow> rows, object context)
        {
            this.SubmitRequest(new UPSerialEntrySaveRowsRequest(rows, context, this));
            return true;
        }

        /// <summary>
        /// Saves the row context.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool SaveRowContext(UPSERow row, object context)
        {
            if (row == null)
            {
                this.SaveAllChangesWithContext(context);
                return true;
            }

            if (this.DisableSingleRowUpdate)
            {
                return false;
            }

            this.SubmitRequest(new UPSerialEntrySaveRowRequest(row, context));
            this.SubmitRequest(new UPSerialEntryUploadPhotoRowRequest(row, context));
            return true;
        }

        /// <summary>
        /// Saves all changes with context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveAllChangesWithContext(object context)
        {
            this.SubmitRequest(new UPSerialEntrySaveAllRequest(context, this));
        }

        /// <summary>
        /// Saves the ignoring all changes.
        /// </summary>
        public void SaveIgnoringAllChanges()
        {
            this.SubmitRequest(new UPSerialEntrySaveAllRequest(null, true, this));
        }

        /// <summary>
        /// Changeds the child records for delete row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedChildRecordsForDeleteRow(UPSERow row)
        {
            if (string.IsNullOrEmpty(row.SerialEntryRecordId) || (row.SerialEntryRecordId.Contains("new") && row.SerialEntryRecordId.Length < 6))
            {
                return null;
            }

            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>();
            if (this.ChildrenCount > 0)
            {
                List<string> existingChildren = row.ExistingChildRecordIds;
                if (existingChildren != null)
                {
                    foreach (string childRecordId in existingChildren)
                    {
                        changedRecords.Add(new UPCRMRecord(StringExtensions.InfoAreaIdRecordId(this.DestChildInfoAreaId, childRecordId), "Delete", null));
                    }
                }
            }

            changedRecords.Add(new UPCRMRecord(StringExtensions.InfoAreaIdRecordId(this.DestInfoAreaId, row.SerialEntryRecordId), "Delete", null));
            if (changedRecords.Count > 0)
            {
                UPCRMRecord rootRecord = this.UpdatedRootRecordWithTemplateFilter(null);
                if (rootRecord != null)
                {
                    changedRecords.Add(rootRecord);
                }
            }

            if (changedRecords.Count > 0)
            {
                this.rowsChanged = true;
            }

            return changedRecords.Count > 0 ? changedRecords : null;
        }

        /// <summary>
        /// Columns the name of for function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        public UPSEColumn ColumnForFunctionName(string functionName)
        {
            UPSEColumn column = this.DestColumnsForFunction.ValueOrDefault(functionName) ??
                                this.SourceColumnsForFunction.ValueOrDefault(functionName);

            return column;
        }

        /// <summary>
        /// Adds the position.
        /// </summary>
        /// <param name="row">The row.</param>
        public void AddPosition(UPSERow row)
        {
            List<string> rowSourceArray = this.positionsForSourceKeyDictionary.ValueOrDefault(row.RowRecordId);
            if (rowSourceArray == null)
            {
                this.positionsForSourceKeyDictionary[row.RowRecordId] = new List<string> { row.RowKey };
            }
            else
            {
                rowSourceArray.Add(row.RowKey);
            }

            this.rowsWithData.SetObjectForKey(row, row.RowKey);
            if (!string.IsNullOrEmpty(row.RowParentRecordId))
            {
                UPSEDestinationParent destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                destinationParent?.AddPosition(row);
            }

            if (!string.IsNullOrEmpty(row.ListingRecordId))
            {
                this.positionsForListing.SetObjectForKey(row, row.ListingRecordId);
            }

            this.positions.Add(row);
        }

        /// <summary>
        /// Removes the position.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public bool RemovePosition(UPSERow row)
        {
            List<string> rowSourceArray = this.positionsForSourceKeyDictionary.ValueOrDefault(row.RowRecordId);
            rowSourceArray?.Remove(row.RowKey);

            this.rowsWithData.Remove(row.RowKey);
            if (!string.IsNullOrEmpty(row.RowParentRecordId))
            {
                UPSEDestinationParent destinationParent = this.destinationParents.ValueOrDefault(row.RowParentRecordId);
                destinationParent?.RemovePosition(row);
            }

            if (!string.IsNullOrEmpty(row.ListingRecordId))
            {
                this.positionsForListing.Remove(row.ListingRecordId);
            }

            this.positions.Remove(row);
            return true;
        }

        /// <summary>
        /// Keys for source row record identifier.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        public string KeyForSourceRowRecordId(string recordId)
        {
            int nextId = 0;
            if (this.nextGenericKeyForSourceKeyDictionary.ContainsKey(recordId))
            {
                nextId = this.nextGenericKeyForSourceKeyDictionary[recordId];
            }

            string key = $"{recordId}:{nextId}";
            this.nextGenericKeyForSourceKeyDictionary[recordId] = ++nextId;
            return key;
        }

        /// <summary>
        /// Rows the configuration for query table.
        /// </summary>
        /// <param name="queryTable">The query table.</param>
        /// <returns></returns>
        public UPSERowConfiguration RowConfigurationForQueryTable(UPConfigQueryTable queryTable)
        {
            if (queryTable == null)
            {
                return this.DefaultRowConfiguration;
            }

            long configurationKey = 0; //NSNumber.NumberWithLong((long)queryTable);
            UPSERowConfiguration rowConfiguration = this.rowConfigurations.ValueOrDefault(configurationKey);
            if (rowConfiguration != null)
            {
                return rowConfiguration;
            }

            Dictionary<string, object> dict = this.FieldGroupDecider.PropertiesForQueryTable(queryTable, false);
            if (dict.Count == 0)
            {
                return this.DefaultRowConfiguration;
            }

            rowConfiguration = new UPSERowConfiguration(dict, this);
            if (this.rowConfigurations != null)
            {
                this.rowConfigurations.SetObjectForKey(rowConfiguration, configurationKey);
            }
            else
            {
                this.rowConfigurations = new Dictionary<long, UPSERowConfiguration> { { configurationKey, rowConfiguration } };
            }

            return rowConfiguration;
        }

        private double OverallDiscountPriceWithRow(UPSERow currentRow)
        {
            double price = 0;
            if (currentRow != null)
            {
                foreach (UPSERow row in this.positions)
                {
                    price += row.EndPriceWithoutDiscount;
                    if (row == currentRow)
                    {
                        currentRow = null;
                    }
                }

                if (currentRow != null)
                {
                    price += currentRow.EndPriceWithoutDiscount;
                }
            }
            else
            {
                price += this.positions.Sum(row => row.EndPriceWithoutDiscount);
            }

            return price;
        }

        /// <summary>
        /// Checks the overall price with row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public bool CheckOverallPriceWithRow(UPSERow row)
        {
            bool changed = false;
            if (this.Pricing == null || !this.Pricing.HasOverallDiscount)
            {
                return false;
            }

            double _currentEndPrice = this.OverallDiscountPriceWithRow(row);
            if (this.OverallDiscountActive && _currentEndPrice < this.Pricing.OverallDiscountPrice)
            {
                this.OverallDiscountActive = false;
                changed = true;
            }
            else if (!this.OverallDiscountActive && _currentEndPrice >= this.Pricing.OverallDiscountPrice)
            {
                this.OverallDiscountActive = true;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if ((error == null || error.IsConnectionOfflineError()) && this.currentRequestOption == UPRequestOption.FastestAvailable)
            {
                this.SearchOperationDidFinishWithResult(operation, null);
                return;
            }

            this.TheDelegate?.SerialEntryDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            switch (this.currentSearchOperationType)
            {
                case Constants.UPSESearchOperationSourceChildren:
                    this.currentSearchOperationType = 0;
                    this.InitializeFromSource(result);
                    break;

                case Constants.UPSESearchOperationDestinationParent:
                    this.currentSearchOperationType = 0;
                    this.InitializeWithDestinationParentResult(result, this.currentSearchOperation);
                    break;

                case Constants.UPSESearchOperationDestination:
                    this.currentSearchOperationType = 0;
                    this.InitializeWithDestinationResult(result, this.currentSearchOperation);
                    break;

                case Constants.UPSESearchOperationDestinationChildren:
                    this.currentSearchOperationType = 0;
                    this.InitializeWithDestinationChildResult(result, this.currentSearchOperation);
                    break;

                case Constants.UPSESearchRowsForFilter:
                    if (!operation.Canceled)
                    {
                        this.currentQueuedSearchOperation = null;
                        this.ApplyRowsForFilter(result, this.currentFilter);
                    }

                    break;

                case Constants.UPSESearchRowsForAutoFill:
                    if (!operation.Canceled)
                    {
                        this.currentQueuedSearchOperation = null;
                        this.ApplyRowsForFilter(result, this.currentFilter);
                    }

                    this.autoFill.RowsLoaded();
                    break;

                case Constants.UPSESearchOperationRootDetailsInfoPanel:
                    if (!operation.Canceled)
                    {
                        this.currentQueuedSearchOperation = null;
                        this.InitializeWithRootDetailsInfoPanelResult(result);
                    }

                    break;

                case Constants.UPSESearchOperationProductCatalog:
                    if (!operation.Canceled)
                    {
                        this.currentQueuedSearchOperation = null;
                        this.InitializeWithProductCatalogResult(result);
                    }

                    break;
            }
        }

        /// <summary>
        /// Listings the controller context did return owner.
        /// </summary>
        /// <param name="listingController">The listing controller.</param>
        /// <param name="context">The context.</param>
        /// <param name="owner">The owner.</param>
        public void ListingControllerContextDidReturnOwner(UPSEListingController listingController, object context, UPSEListingOwner owner)
        {
            this.ReadQuota();
        }

        /// <summary>
        /// Listings the controller context did return listing for owner.
        /// </summary>
        /// <param name="listingController">The listing controller.</param>
        /// <param name="context">The context.</param>
        /// <param name="owner">The owner.</param>
        public void ListingControllerContextDidReturnListingForOwner(UPSEListingController listingController, object context, UPSEListingOwner owner)
        {
        }

        /// <summary>
        /// Listings the controller context did fail with error.
        /// </summary>
        /// <param name="listingController">The listing controller.</param>
        /// <param name="context">The context.</param>
        /// <param name="_error">The error.</param>
        public void ListingControllerContextDidFailWithError(UPSEListingController listingController, object context, Exception _error)
        {
            this.TheDelegate?.SerialEntryDidFailWithError(this, _error);
        }

        /// <summary>
        /// The raw value for function name.
        /// </summary>
        /// <param name="functionName">The function name.</param>
        /// <returns>
        /// The <see cref="T:System.String" />.
        /// </returns>
        public string RawValueForFunctionName(string functionName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Pricings the did finish with result.
        /// </summary>
        /// <param name="pricing">The pricing.</param>
        /// <param name="result">The result.</param>
        public void PricingDidFinishWithResult(UPSEPricing pricing, object result)
        {
            if (this.DestParentListFieldControl != null)
            {
                this.ReadDestinationParent();
            }
            else
            {
                this.ReadDestination();
            }
        }

        /// <summary>
        /// Pricings the did fail with error.
        /// </summary>
        /// <param name="pricing">The pricing.</param>
        /// <param name="error">The error.</param>
        public void PricingDidFailWithError(UPSEPricing pricing, Exception error)
        {
            this.TheDelegate?.SerialEntryDidFailWithError(this, error);
        }

        /// <summary>
        /// Additionals the items information did finish with result.
        /// </summary>
        /// <param name="addItem">The add item.</param>
        /// <param name="result">The result.</param>
        public void AdditionalItemsInformationDidFinishWithResult(UPSEAdditionalItemInformations addItem, object result)
        {
            this.ReadSourceChildren();
        }

        /// <summary>
        /// Additionals the items information did fail with error.
        /// </summary>
        /// <param name="addItem">The add item.</param>
        /// <param name="error">The error.</param>
        public void AdditionalItemsInformationDidFailWithError(UPSEAdditionalItemInformations addItem, Exception error)
        {
            this.TheDelegate?.SerialEntryDidFailWithError(this, error);
        }

        /// <summary>
        /// Serials the entry quota handler did finish with result.
        /// </summary>
        /// <param name="quota">The quota.</param>
        /// <param name="result">The result.</param>
        public void SerialEntryQuotaHandlerDidFinishWithResult(UPSEQuotaHandler quota, object result)
        {
            this.FinishedLoading();
        }

        /// <summary>
        /// Serials the entry quota handler did fail with error.
        /// </summary>
        /// <param name="quota">The quota.</param>
        /// <param name="error">The error.</param>
        public void SerialEntryQuotaHandlerDidFailWithError(UPSEQuotaHandler quota, Exception error)
        {
            this.TheDelegate.SerialEntryDidFailWithError(this, error);
        }

        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.ContinueBuildWithCopyFieldDictionary(dictionary);
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.TheDelegate.SerialEntryDidFailWithError(this, error);
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            this.ContinueBuildWithRootRecord(new UPCRMRecord(_linkReader.DestinationRecordIdentification));
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.TheDelegate.SerialEntryDidFailWithError(this, error);
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

        private void ContinueBuildWithCopyFieldDictionary(Dictionary<string, object> dictionary)
        {
            Dictionary<string, object> fixedCopyFieldValues = null;
            string fixedCopyFieldValueString = this.viewReference.ContextValueForKey("FixedCopyFieldValues");
            if (!string.IsNullOrEmpty(fixedCopyFieldValueString))
            {
                Dictionary<string, object> dict = fixedCopyFieldValueString.JsonDictionaryFromString();
                if (dict.Count > 0)
                {
                    Dictionary<string, object> fixedValues = new Dictionary<string, object>(dict.Count);
                    foreach (string key in dict.Keys)
                    {
                        fixedValues[key] = dict[key];
                    }

                    fixedCopyFieldValues = fixedValues;
                }
            }

            if (fixedCopyFieldValues?.Count > 0)
            {
                if (dictionary.Count == 0)
                {
                    this.InitialFieldValuesForDestination = fixedCopyFieldValues;
                }
                else
                {
                    Dictionary<string, object> combinedDictionary = new Dictionary<string, object>(fixedCopyFieldValues);
                    foreach (var entry in dictionary)
                    {
                        combinedDictionary[entry.Key] = entry.Value;
                    }

                    this.InitialFieldValuesForDestination = combinedDictionary;
                }
            }
            else
            {
                this.InitialFieldValuesForDestination = dictionary;
            }

            string destinationEditTriggerName = this.DestFieldControl.ValueForAttribute("EditTrigger");
            if (!string.IsNullOrEmpty(destinationEditTriggerName))
            {
                var editTriggersArray = destinationEditTriggerName.Split(';');
                List<UPConfigFilter> editTriggerFilterArray = new List<UPConfigFilter>();
                foreach (string filterSeg in editTriggersArray)
                {
                    UPConfigFilter editTriggerFilter = ConfigurationUnitStore.DefaultStore.FilterByName(filterSeg);
                    if (editTriggerFilter != null)
                    {
                        editTriggerFilter = editTriggerFilter.FilterByApplyingValueDictionaryDefaults(this.InitialFieldValuesForDestination, true);
                        editTriggerFilterArray.Add(editTriggerFilter);
                    }
                }
            }

            if (this.fixedSourceFilter != null)
            {
                this.fixedSourceFilter = this.fixedSourceFilter.FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(this.InitialFieldValuesForDestination));
            }

            this.copyFields = null;
            string parentLinkString = this.viewReference.ContextValueForKey("ParentLink");
            if (!string.IsNullOrEmpty(parentLinkString))
            {
                this.linkReader = new UPCRMLinkReader(this.Record.RecordIdentification, parentLinkString, this);
                this.linkReader.Start();
            }
            else
            {
                this.ContinueBuildWithRootRecord(this.Record);
            }
        }

        /// <summary>
        /// Sets SourceChildFieldControl
        /// </summary>
        /// <param name="configStore">config store</param>
        /// <param name="configName">config name</param>
        private void SetSourceChildFieldControl(IConfigurationUnitStore configStore, string configName)
        {
            if (!string.IsNullOrWhiteSpace(configName))
            {
                SourceChildConfig = configStore.SearchAndListByName(configName);
                if (SourceChildConfig != null)
                {
                    SourceChildFieldControl = configStore.FieldControlByNameFromGroup(KeyList, SourceChildConfig.FieldGroupName);
                }
            }
        }

        /// <summary>
        /// Sets DestChildFieldControl
        /// </summary>
        /// <param name="configStore">config store</param>
        /// <param name="configName">config name</param>
        private void SetDestChildFieldControl(IConfigurationUnitStore configStore, string configName)
        {
            if (SourceChildConfig != null)
            {
                configName = viewReference.ContextValueForKey(KeyDestinationChildConfigName);
                DestChildFieldControl = configStore.FieldControlByNameFromGroup(KeyEdit, configName);
            }
        }

        /// <summary>
        /// Populates EditTriggerFilterArray
        /// </summary>
        private void PopulateEditTriggerFilterArray()
        {
            var destinationChildEditTriggerName = DestChildFieldControl?.ValueForAttribute(KeyEditTrigger);
            if (!string.IsNullOrWhiteSpace(destinationChildEditTriggerName))
            {
                var editTriggersArray = destinationChildEditTriggerName.Split(';');
                var editTriggerFilterArray = new List<UPConfigFilter>();
                foreach (var filterSeg in editTriggersArray)
                {
                    var editTriggerFilter = ConfigurationUnitStore.DefaultStore.FilterByName(filterSeg);
                    if (editTriggerFilter != null)
                    {
                        editTriggerFilter = editTriggerFilter.FilterByApplyingValueDictionaryDefaults(InitialFieldValuesForDestination, true);
                        editTriggerFilterArray.Add(editTriggerFilter);
                    }
                }
            }
        }

        /// <summary>
        /// Sets RootDetailsFieldControl
        /// </summary>
        /// <param name="configStore">config store</param>
        /// <param name="configName">config name</param>
        private void SetRootDetailsFieldControl(IConfigurationUnitStore configStore, string configName)
        {
            configName = viewReference.ContextValueForKey(KeyDestinationRootConfig);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                DestRootFieldControl = configStore.FieldControlByNameFromGroup(KeyEdit, configName);
                RootDetailsFieldControl = configStore.FieldControlByNameFromGroup(KeyDetails, configName);
                if (RootDetailsFieldControl == null && DestRootFieldControl != null)
                {
                    RootDetailsFieldControl = configStore.FieldControlByNameFromGroup(KeyDetails, DestRootFieldControl.InfoAreaId);
                }
            }
        }

        /// <summary>
        /// Sets SourceRequestOption
        /// </summary>
        private void SetSourceRequestOption()
        {
            var requestOption = viewReference.ContextValueForKey(KeySourceRequestOption);
            sourceRequestOption = UPCRMDataStore.RequestOptionFromString(requestOption, sourceRequestOption);
        }

        /// <summary>
        /// Sets DestinationRequestOption
        /// </summary>
        private void SetDestinationRequestOption()
        {
            if (offlineRequest == null)
            {
                var requestOption = viewReference.ContextValueForKey(KeyDestinationRequestOption);
                destinationRequestOption = UPCRMDataStore.RequestOptionFromString(requestOption, destinationRequestOption);
            }
        }

        /// <summary>
        /// Sets SerialEntryRowInfoPanels
        /// </summary>
        private void SetSerialEntryRowInfoPanels()
        {
            var infoPanelDefinitionString = viewReference.ContextValueForKey(KeyInfoPanelDefinition);
            if (!string.IsNullOrWhiteSpace(infoPanelDefinitionString))
            {
                var infoPanelDefinition = infoPanelDefinitionString.JsonDictionaryFromString();
                if (infoPanelDefinition != null)
                {
                    var infoPanels = new List<UPSerialEntryInfo>();
                    foreach (var entry in infoPanelDefinition)
                    {
                        var infoPanelDef = entry.Value as Dictionary<string, string>;
                        if (infoPanelDef != null)
                        {
                            var info = UPSerialEntryInfo.SerialEntryInfoFromDefinitionSerialEntry(infoPanelDef, this);
                            if (info != null)
                            {
                                infoPanels.Add(info);
                            }
                        }
                    }

                    SerialEntryRowInfoPanels = infoPanels;
                }
            }
        }

        /// <summary>
        /// Sets SerialEntryRowDocuments
        /// </summary>
        private void SetSerialEntryRowDocuments()
        {
            var documentsDefinitionString = viewReference.ContextValueForKey(KeyDocumentsDefinition);
            if (!string.IsNullOrWhiteSpace(documentsDefinitionString))
            {
                var documentsDefinition = documentsDefinitionString.JsonDictionaryFromString();
                if (documentsDefinition != null)
                {
                    var documents = new List<UPSerialEntryDocuments>();
                    foreach (var entry in documentsDefinition)
                    {
                        var documentsDef = entry.Value as Dictionary<string, string>;
                        if (documentsDef != null)
                        {
                            var info = UPSerialEntryDocuments.Create(documentsDef, this);
                            if (info != null)
                            {
                                documents.Add(info);
                            }
                        }
                    }

                    SerialEntryRowDocuments = documents;
                }
            }
        }

        /// <summary>
        /// Sets DestParentInfoAreaId
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetDestParentInfoAreaId(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyDestinationParentConfigName);
            if (configName != null)
            {
                DestParentListFieldControl = configStore.FieldControlByNameFromGroup(KeyList, configName);
                DestParentEditFieldControl = configStore.FieldControlByNameFromGroup(KeyEdit, configName);
                if (DestParentListFieldControl != null)
                {
                    DestParentInfoAreaId = DestParentListFieldControl.InfoAreaId;
                }
            }
        }

        /// <summary>
        /// Sets SourceParentInfoAreaId
        /// </summary>
        private void SetSourceParentInfoAreaId()
        {
            SourceParentInfoAreaId = viewReference.ContextValueForKey(KeySourceParentInfoAreaId);
        }

        /// <summary>
        /// Sets ListingInfoAreaId
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetListingInfoAreaId(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyListingConfiguration);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                var menu = configStore.MenuByName(configName);
                if (menu != null)
                {
                    ListingConfiguration = menu.ViewReference;
                    string listingSearchAndListName = ListingConfiguration.ContextValueForKey(KeyListingControlName);
                    if (!string.IsNullOrWhiteSpace(listingSearchAndListName))
                    {
                        var searchAndListControl = configStore.SearchAndListByName(listingSearchAndListName);
                        if (searchAndListControl != null)
                        {
                            ListingInfoAreaId = searchAndListControl.InfoAreaId;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets PricingConfiguration
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetPricingConfiguration(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyPricingConfiguration);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                var menu = configStore.MenuByName(configName);
                if (menu != null)
                {
                    PricingConfiguration = menu.ViewReference;
                }
            }
        }

        /// <summary>
        /// Sets QuickConfiguration
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetQuickConfiguration(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyQuotaConfiguration);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                var menu = configStore.MenuByName(configName);
                if (menu != null)
                {
                    QuotaConfiguration = menu.ViewReference;
                }
            }
        }

        /// <summary>
        /// Sets RowDisplayListFormatter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetRowDisplayListFormatter(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyRowDisplayConfiguration);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                RowDisplayFieldControl = configStore.FieldControlByNameFromGroup(KeyList, configName);
                if (RowDisplayFieldControl != null)
                {
                    RowDisplayListFormatter = new UPCRMListFormatter(RowDisplayFieldControl);
                }
            }
        }

        /// <summary>
        /// Sets SumLineListFormatter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetSumLineListFormatter(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeySumLineConfiguration);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                SumLineFieldControl = configStore.FieldControlByNameFromGroup(KeyList, configName);
                if (SumLineFieldControl != null)
                {
                    sumLineListFormatter = new UPCRMListFormatter(SumLineFieldControl);
                }
            }
        }

        /// <summary>
        /// Sets AdditionalOptionProperties
        /// </summary>
        private void SetAdditionalOptionProperties()
        {
            var configName = viewReference.ContextValueForKey(KeyOptions);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                AdditionalOptions = configName.JsonDictionaryFromString();
                SyncStrategy = AdditionalOptions[KeySyncStrategy] as string;
                DisableSingleRowUpdate = Convert.ToInt32(AdditionalOptions[KeyDisableRowUpdate]) != 0;
                DisableAutoCorrectPackageSize = Convert.ToInt32(AdditionalOptions.ValueOrDefault(KeyDisableAutoCorrectPackageSize)) != 0;
                DisableAutoCorrectMinMax = Convert.ToInt32(AdditionalOptions.ValueOrDefault(KeyDisableAutoCorrectMinMax)) != 0;
                AdditionalSourceConfigNames = AdditionalOptions.ValueOrDefault(KeyAddSourceConfigs) as List<string>;
                ComputeRowOnEveryColumn = Convert.ToInt32(AdditionalOptions.ValueOrDefault(KeyComputeRowOnEveryColumn)) != 0;
                AutoCreatePositions = Convert.ToInt32(AdditionalOptions.ValueOrDefault(KeyAutoCreatePositions)) != 0;
                minSearchTextLength = Convert.ToInt32(AdditionalOptions.ValueOrDefault(KeyMinSearchTextLength));
                PdfHlColor = AdditionalOptions.ValueOrDefault(KeyPdfHlColor) as string;
                SyncRowAfterChildren = Convert.ToInt32(AdditionalOptions.ValueOrDefault(KeySyncRowAfterChildren)) != 0;
            }
        }

        /// <summary>
        /// Sets ActiveFilter
        /// </summary>
        private void SetActiveFilter()
        {
            var configName = viewReference.ContextValueForKey(KeyActiveFilter);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                ActiveFilter = configName.Split(',');
            }
        }

        /// <summary>
        /// Sets DestinationParentTemplateFilter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetComputeRowOnEveryColumn(IConfigurationUnitStore configStore)
        {
            if (!ComputeRowOnEveryColumn && configStore.ConfigValueIsSet(KeySerialEntryComputeRowOnEveryColumn))
            {
                ComputeRowOnEveryColumn = true;
            }
        }

        /// <summary>
        /// Sets AdditionalItemInformations
        /// </summary>
        private void SetAdditionalItemInformations()
        {
            if (AdditionalSourceConfigNames != null)
            {
                AdditionalItemInformations = UPSEAdditionalItemInformations.Create(
                    this,
                    AdditionalSourceConfigNames,
                    ColumnItemNumber,
                    InitialFieldValuesForDestination,
                    this);
            }
        }

        /// <summary>
        /// Sets RootFilterTemplateForStartSerialEntry
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetRootFilterTemplateForStartSerialEntry(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyRootFilterTemplateForStartSerialEntry);
            if (configName != null)
            {
                RootFilterTemplateForStartSerialEntry = configStore.FilterByName(configName);
            }
        }

        /// <summary>
        /// Sets RootFilterTemplateForEndSerialEntry
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetRootFilterTemplateForEndSerialEntry(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyRootFilterTemplateForEndSerialEntry);
            if (configName != null)
            {
                RootFilterTemplateForEndSerialEntry = configStore.FilterByName(configName);
            }
        }

        /// <summary>
        /// Sets FieldGroupDeciderFilter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetFieldGroupDeciderFilter(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyFieldGroupDecider);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                FieldGroupDeciderFilter = configStore.FilterByName(configName);
            }
        }

        /// <summary>
        /// Sets ProductCatalogDocumentManager
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetProductCatalogDocumentManager(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyProductCatalogSource);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                ProductCatalogSearchAndList = configStore.SearchAndListByName(configName);
                if (ProductCatalogSearchAndList != null)
                {
                    productCatalogDocumentManager = new DocumentInfoAreaManager(ProductCatalogSearchAndList);
                }
            }
        }

        /// <summary>
        /// Sets DestinationTemplateFilter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetDestinationTemplateFilter(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyDestinationTemplateFilter);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                DestinationTemplateFilter = configStore.FilterByName(configName);
            }
        }

        /// <summary>
        /// Sets DestinationChildTemplateFilter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetDestinationChildTemplateFilter(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyDestinationChildTemplateFilter);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                DestinationChildTemplateFilter = configStore.FilterByName(configName);
            }
        }

        /// <summary>
        /// Sets DestinationParentTemplateFilter
        /// </summary>
        /// <param name="configStore">config store</param>
        private void SetDestinationParentTemplateFilter(IConfigurationUnitStore configStore)
        {
            var configName = viewReference.ContextValueForKey(KeyDestinationParentTemplateFilter);
            if (!string.IsNullOrWhiteSpace(configName))
            {
                DestinationParentTemplateFilter = configStore.FilterByName(configName);
            }
        }

        /// <summary>
        /// Sets DestinationChildTemplateFilter
        /// </summary>
        private void SetDestinationChildTemplateFilter()
        {
            if (DestinationChildTemplateFilter != null)
            {
                DestinationChildTemplateFilter = DestinationChildTemplateFilter.FilterByApplyingValueDictionaryDefaults(
                    InitialFieldValuesForDestination,
                    true);
            }
        }

        /// <summary>
        /// Sets DestinationTemplateFilter
        /// </summary>
        private void SetDestinationTemplateFilter()
        {
            if (DestinationTemplateFilter != null)
            {
                DestinationTemplateFilter = DestinationTemplateFilter.FilterByApplyingValueDictionaryDefaults(
                    InitialFieldValuesForDestination,
                    true);
            }
        }

        /// <summary>
        /// Sets DestinationParentTemplateFilter
        /// </summary>
        private void SetDestinationParentTemplateFilter()
        {
            if (DestinationParentTemplateFilter != null)
            {
                DestinationParentTemplateFilter = DestinationParentTemplateFilter.FilterByApplyingValueDictionaryDefaults(
                    InitialFieldValuesForDestination,
                    true);
            }
        }

        /// <summary>
        /// Initializes UPSearialEntry object from <see cref="UPCRMResult"/> source.
        /// </summary>
        /// <param name="sourceChildrenResult">
        /// source object <see cref="UPCRMResult"/>
        /// </param>
        private void InitializeFromSource(UPCRMResult sourceChildrenResult)
        {
            if (sourceChildrenResult?.RowCount > 0)
            {
                SetSourceChilren(sourceChildrenResult);
            }
            else
            {
                ChildrenCount = 0;
            }

            var columnCount = 0;
            var columnArray = new List<UPSEColumn>();
            var fieldTab = SourceFieldControl.TabAtIndex(0);
            var pricingAutoItemNumber = ConfigurationUnitStore.DefaultStore.ConfigValueIsSet(KeyPricingAutoItemNumber);
            if (!ExplicitItemNumberFunctionName && !pricingAutoItemNumber && !string.IsNullOrWhiteSpace(ItemNumberFunctionName))
            {
                ExplicitItemNumberFunctionName = true;
            }

            columnCount = SetSourceColumnsForFunction(fieldTab, columnArray, columnCount);

            if (AdditionalItemInformations != null)
            {
                columnCount = AddAdditionalItemInformationColumns(columnArray, columnCount);
            }

            if (ChildrenCount > 0)
            {
                columnCount = SetDestChildColumnsForFunction(columnArray, columnCount);
            }

            fieldTab = DestFieldControl.TabAtIndex(0);
            columnCount = SetDestColumnsForFunction(fieldTab, columnArray, columnCount);

            Columns = columnArray;
            ListingSourceColumns = GetListingSourceColumns();

            var sortInfoArray = GetSortInfoArray();

            if (sortInfoArray != null)
            {
                ColumnSortInfo = new UPSESortInfo(sortInfoArray);
            }

            DefaultRowConfiguration = new UPSERowConfiguration(this);
            if (ProductCatalogSearchAndList != null)
            {
                ReadProductCatalogDocuments();
            }
            else if (RootDetailsFieldControl != null)
            {
                ReadRootDetailsInfoPanel();
            }
            else
            {
                ReadPricing();
            }
        }

        /// <summary>
        /// Sets SourceChildren Property
        /// </summary>
        /// <param name="sourceChildrenResult">
        /// Soruce object <see cref="UPCRMResult"/>
        /// </param>
        private void SetSourceChilren(UPCRMResult sourceChildrenResult)
        {
            var rowCount = sourceChildrenResult.RowCount;
            ChildrenCount = rowCount;
            var sourceChildrenArray = new List<UPSESourceChild>(rowCount);
            for (var index = 0; index < rowCount; index++)
            {
                var child = new UPSESourceChild((UPCRMResultRow)sourceChildrenResult.ResultRowAtIndex(index));
                sourceChildrenArray.Add(child);
            }

            SourceChildren = sourceChildrenArray;
        }

        /// <summary>
        /// Sets SourceColumnsForFunction Property
        /// </summary>
        /// <param name="fieldTab">
        /// <see cref="FieldControlTab"/> object.
        /// </param>
        /// <param name="columnArray">
        /// list of columns
        /// </param>
        /// <param name="columnCount">
        /// current column count
        /// </param>
        /// <returns>Updated Column Count</returns>
        private int SetSourceColumnsForFunction(FieldControlTab fieldTab, List<UPSEColumn> columnArray, int columnCount)
        {
            var count = fieldTab.NumberOfFields;
            for (var index = 0; index < count; index++)
            {
                var fieldConfig = fieldTab.FieldAtIndex(index);
                var sourceColumn = new UPSESourceColumn(fieldConfig, columnCount++, index);
                columnArray.Add(sourceColumn);
                if (fieldConfig.Attributes.Image || fieldConfig.Function == FieldConfigFunctionSourceImage)
                {
                    DocumentKeyColumn = sourceColumn;
                }

                if (!string.IsNullOrWhiteSpace(fieldConfig.Function))
                {
                    if (SourceColumnsForFunction == null)
                    {
                        SourceColumnsForFunction = new Dictionary<string, UPSESourceColumn>();
                    }

                    SourceColumnsForFunction.SetObjectForKey(sourceColumn, fieldConfig.Function);
                }
            }

            return columnCount;
        }

        /// <summary>
        /// Adds AdditionalItemInformationColumns to list of columns
        /// </summary>
        /// <param name="columnArray">
        /// list of columns
        /// </param>
        /// <param name="columnCount">
        /// current column count
        /// </param>
        /// <returns>Returns updated column count</returns>
        private int AddAdditionalItemInformationColumns(List<UPSEColumn> columnArray, int columnCount)
        {
            var keyColumn = SourceColumnsForFunction.ValueOrDefault(KeyItemNumber)
                            ?? SourceColumnsForFunction.ValueOrDefault(KeyCopyItemNumber);

            if (keyColumn != null)
            {
                var additionalColumns = AdditionalItemInformations.AdditionalSourceFieldsWithKeyColumn(keyColumn, columnCount);
                if (additionalColumns?.Any() == true)
                {
                    columnArray.AddRange(additionalColumns);
                    columnCount += additionalColumns.Count;
                }
            }

            return columnCount;
        }

        /// <summary>
        /// Sets DestColumnsForFunction Property
        /// </summary>
        /// <param name="fieldTab">
        /// <see cref="FieldControlTab"/> object.
        /// </param>
        /// <param name="columnArray">
        /// list of columns
        /// </param>
        /// <param name="columnCount">
        /// current column count
        /// </param>
        /// <returns>Updated Column Count</returns>
        private int SetDestColumnsForFunction(FieldControlTab fieldTab, List<UPSEColumn> columnArray, int columnCount)
        {
            var count = fieldTab.NumberOfFields;
            DestColumnForFieldKey = new Dictionary<string, UPSEColumn>();
            for (var index = 0; index < count; index++)
            {
                var destFieldConfig = fieldTab.FieldAtIndex(index);
                var field = destFieldConfig.Field;
                var parentFieldId = field.FieldInfo.ParentCatalogFieldId;
                var parentFieldIndex = -1;
                if (parentFieldId >= 0)
                {
                    for (var parentIndex = 0; parentIndex < count; parentIndex++)
                    {
                        if (parentIndex == index)
                        {
                            continue;
                        }

                        var compareFieldConfig = fieldTab.FieldAtIndex(parentIndex);
                        var compareField = compareFieldConfig.Field;
                        if (compareField.FieldId == parentFieldId
                            && compareField.InfoAreaId == field.InfoAreaId
                            && (compareField.LinkId == field.LinkId
                            || (compareField.LinkId <= 0 && field.LinkId <= 0)))
                        {
                            parentFieldIndex = columnCount + parentIndex - index;
                            break;
                        }
                    }
                }

                var seColumn = new UPSEDestinationColumn(destFieldConfig, columnCount++, parentFieldIndex, index, DestInfoAreaId);
                columnArray.Add(seColumn);
                var fieldKey = destFieldConfig.Field.FieldIdentification;
                if (fieldKey != null)
                {
                    DestColumnForFieldKey.SetObjectForKey(seColumn, fieldKey);
                }

                if (!string.IsNullOrWhiteSpace(destFieldConfig.Function))
                {
                    if (destFieldConfig.Function.StartsWith(FunctionPrefixColumnCopy) || (DestColumnsForFunction != null && DestColumnsForFunction.ContainsKey(destFieldConfig.Function)))
                    {
                        var functionName = destFieldConfig.Function;
                        if (functionName.StartsWith(FunctionPrefixColumnCopy))
                        {
                            functionName = functionName.Substring(11);
                        }

                        var copyArray = (List<UPSEColumn>)null;
                        if (DestCopyColumnArrayForFunction != null)
                        {
                            copyArray = DestCopyColumnArrayForFunction.ValueOrDefault(functionName);
                            if (copyArray == null)
                            {
                                DestCopyColumnArrayForFunction.SetObjectForKey(new List<UPSEColumn> { seColumn }, functionName);
                            }
                            else
                            {
                                copyArray.Add(seColumn);
                            }
                        }
                        else
                        {
                            DestCopyColumnArrayForFunction = new Dictionary<string, List<UPSEColumn>> { { functionName, new List<UPSEColumn> { seColumn } } };
                        }
                    }
                    else
                    {
                        if (DestColumnsForFunction == null)
                        {
                            DestColumnsForFunction = new Dictionary<string, UPSEColumn> { { destFieldConfig.Function, seColumn } };
                        }
                        else
                        {
                            DestColumnsForFunction.SetObjectForKey(seColumn, destFieldConfig.Function);
                        }

                        if (destFieldConfig.Function == FunctionServerApproved)
                        {
                            ServerApprovedColumn = seColumn;
                        }
                    }
                }
            }

            return columnCount;
        }

        /// <summary>
        /// Fetches ListingSourceColumns
        /// </summary>
        /// <returns>
        /// Dictionary of <see cref="UPSEColumn"/>
        /// </returns>
        private Dictionary<string, UPSEColumn> GetListingSourceColumns()
        {
            var listingSourceColumns = (Dictionary<string, UPSEColumn>)null;
            foreach (var column in Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.ListingDestinationFunctionName))
                {
                    if (listingSourceColumns == null)
                    {
                        listingSourceColumns = new Dictionary<string, UPSEColumn>
                        {
                            { column.ListingDestinationFunctionName, column }
                        };
                    }
                    else
                    {
                        listingSourceColumns.SetObjectForKey(column, column.ListingDestinationFunctionName);
                    }
                }
            }

            return ListingSourceColumns;
        }

        /// <summary>
        /// Returns SortInfoArray
        /// </summary>
        /// <returns>
        /// List of <see cref="UPSEColumnSortInfo"/>
        /// </returns>
        private List<UPSEColumnSortInfo> GetSortInfoArray()
        {
            var sortInfoArray = (List<UPSEColumnSortInfo>)null;
            foreach (var column in Columns)
            {
                if (column.SortInfo != null)
                {
                    if (sortInfoArray == null)
                    {
                        sortInfoArray = new List<UPSEColumnSortInfo> { column.SortInfo };
                    }
                    else
                    {
                        sortInfoArray.Add(column.SortInfo);
                    }
                }
            }

            return sortInfoArray;
        }

        /// <summary>
        /// Sets DestChildColumnsForFunction property
        /// </summary>
        /// <param name="columnArray">
        /// list of columns
        /// </param>
        /// <param name="columnCount">
        /// current column count
        /// </param>
        /// <returns>Returns updated column count</returns>
        private int SetDestChildColumnsForFunction(List<UPSEColumn> columnArray, int columnCount)
        {
            var seColumn = (UPSEColumn)null;
            var destFieldConfig = (UPConfigFieldControlField)null;
            var sourceFieldTab = SourceChildFieldControl.TabAtIndex(0);
            var destFieldTab = DestChildFieldControl.TabAtIndex(0);
            DestChildColumnsForFieldKey = new Dictionary<string, List<UPSEColumn>>();
            for (var childIndex = 0; childIndex < ChildrenCount; childIndex++)
            {
                var fieldCount = sourceFieldTab.NumberOfFields;
                for (var fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
                {
                    columnArray.Add(new UPSESourceChildColumn(sourceFieldTab.FieldAtIndex(fieldIndex), columnCount++, fieldIndex, childIndex));
                }

                fieldCount = destFieldTab.NumberOfFields;
                for (var fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
                {
                    destFieldConfig = destFieldTab.FieldAtIndex(fieldIndex);
                    var field = destFieldConfig.Field;
                    var parentFieldId = field.FieldInfo.ParentCatalogFieldId;
                    var parentFieldIndex = -1;
                    if (parentFieldId >= 0)
                    {
                        for (var parentIndex = 0; parentIndex < fieldCount; parentIndex++)
                        {
                            if (parentIndex == fieldIndex)
                            {
                                continue;
                            }

                            var compareFieldConfig = destFieldTab.FieldAtIndex(parentIndex);
                            var compareField = compareFieldConfig.Field;
                            if (compareField.FieldId == parentFieldId
                                && compareField.InfoAreaId == field.InfoAreaId
                                && (compareField.LinkId == field.LinkId
                                    || (compareField.LinkId <= 0 && field.LinkId <= 0)))
                            {
                                parentFieldIndex = columnCount + parentIndex - fieldIndex;
                                break;
                            }
                        }
                    }

                    seColumn = new UPSEDestinationChildColumn(
                                    destFieldConfig,
                                    columnCount++,
                                    parentFieldIndex,
                                    fieldIndex,
                                    childIndex,
                                    DestChildInfoAreaId);

                                columnArray.Add(seColumn);
                                var fieldKey = destFieldConfig.Field.FieldIdentification;
                                if (fieldKey != null)
                                {
                                    if (childIndex == 0)
                                    {
                                        DestChildColumnsForFieldKey[fieldKey] = new List<UPSEColumn> { seColumn };
                                    }
                                    else
                                    {
                                        var destChildColumns = DestChildColumnsForFieldKey[fieldKey];
                                        destChildColumns.Add(seColumn);
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(destFieldConfig.Function))
                                {
                                    if (DestChildColumnsForFunction == null)
                                    {
                                        DestChildColumnsForFunction = new Dictionary<string, List<UPSEColumn>>();
                                    }

                                    var childColumnArray = DestChildColumnsForFunction.ValueOrDefault(destFieldConfig.Function);
                                    if (childColumnArray == null)
                                    {
                                        childColumnArray = new List<UPSEColumn> { seColumn };
                                        DestChildColumnsForFunction.SetObjectForKey(childColumnArray, destFieldConfig.Function);
                                    }
                                    else
                                    {
                                        childColumnArray.Add(seColumn);
                                    }
                                }
                            }
                        }

            return columnCount;
        }
    }
}
