// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialEntryPageModelController.cs" company="Aurea Software Gmbh">
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
//   The Serial Entry Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Fields.SerialEntry;
    using Aurea.CRM.UIModel.Filters;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.SerialEntry;
    using Aurea.CRM.UIModel.Status;
    using Aurea.CRM.UIModel.Structs;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// UPSerialEntryPageModelControllerDelegate
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.Delegates.IModelControllerDelegate" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.IModelControllerUIDelegate" />
    public interface UPSerialEntryPageModelControllerDelegate : IModelControllerDelegate
    {
    }

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The discard row changes hint
        /// </summary>
        public const string DiscardRowChangesHint = "DiscardRowChanges";

        /// <summary>
        /// The row delete change hint
        /// </summary>
        public const string RowDeleteChangeHint = "RowDeleted";

        /// <summary>
        /// The row duplicated change hint
        /// </summary>
        public const string RowDuplicatedChangeHint = "RowDuplicated";

        /// <summary>
        /// The filter finish success change hint
        /// </summary>
        public const string FilterFinishSuccessChangeHint = "PositionsForFilterDidFinishWithSuccess";

        /// <summary>
        /// The unselect filters and shopping cart change hint
        /// </summary>
        public const string UnselectFiltersAndShoppingCartChangeHint = "Unselect_Filters_And_Shopping_Cart";

        /// <summary>
        /// The sum line updated change hint
        /// </summary>
        public const string SumLineUpdatedChangeHint = "SumLineUpdated";

        /// <summary>
        /// The jump to next position hint
        /// </summary>
        public const string JumpToNextPosHint = "JumpToNextPos";

        /// <summary>
        /// The quota information message key
        /// </summary>
        public const string QuotaInfoMessageKey = "QuotaInfoMessage";

        /// <summary>
        /// The step size information message key
        /// </summary>
        public const string StepSizeInfoMessageKey = "StepSizeInfoMessage";

        /// <summary>
        /// The minimum maximum information message key
        /// </summary>
        public const string MinMaxInfoMessageKey = "MinMaxInfoMessage";
    }

    /// <summary>
    /// SerialEntryPageModelController
    /// </summary>
    /// <seealso cref="UPPageModelController" />
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPRightsCheckerDelegate" />
    public class SerialEntryPageModelController : UPPageModelController, UPSerialEntryDelegate, UPRightsCheckerDelegate
    {
        private const string KeyFinishAction = "FinishAction";
        private const string StringIdCurrency = "currency";
        private const string FunctionCurrency = "Currency";
        private const string StringIdUnitPrice = "unitPrice";
        private const string FunctionUnitPrice = "UnitPrice";
        private const string StringIdQuantity = "quantity";
        private const string FunctionQuantity = "Quantity";
        private const string StringIdEndPrice = "endPrice";
        private const string StringIdThird1 = "third-1";
        private const string StringIdThird2 = "third-2";
        private const string StringIdThird3 = "third-3";
        private const string FunctionNetPrice = "NetPrice";
        private const string FunctionEndPrice = "EndPrice";
        private const string KeyDontShowRebates = "DontShowRebates";
        private const string StringIdTitle = "title";
        private const string StringIdSubtitle = "subtitle";
        private Dictionary<string, UPSEFilter> filterMapping;
        private bool showErrorRows;
        private bool hasEditableChildFields;
        private bool hasEditablePositionFields;
        private uint runningRequests;
        private bool sumLineInitialized;
        private UPMField lastEditField;
        private UPMSEPosition lastEditPosition;
        private UPRightsChecker rightsChecker;
        private List<UPSERow> displayedRows;
        private List<UPSERow> duplicatedRows;
        private bool addToDisplayedRows;
        private bool markPositionIfRecordExists;
        private int countOfStepSizeViolations;
        private int countOfMinMaxViolations;
        private List<string> availableFilters;
        private List<string> positionFilters;
        private bool showSumLineEndPrice;
        private bool showRowLineEndPrice;
        private bool showFreeGoodsPrice;
        private bool showSumLineWithAllRows;
        private bool hierarchicalPositionFilters;
        private /*UIAlertView*/ object autoCreateAlertBox;
        private List<UPMSEPositionWithContext> contextArray;
        private DocumentManager documentManager;
        private string sumLineCurrencyText;
        private string finishActionName;
        private bool initialFilterLoaded;
        private UPMFilter lastTappedPositionFilter;

        /// <summary>
        /// Gets the serial entry page.
        /// </summary>
        /// <value>
        /// The serial entry page.
        /// </value>
        public UPMSEPage SerialEntryPage => (UPMSEPage)this.Page;

        /// <summary>
        /// Gets the last shopping cart count.
        /// </summary>
        /// <value>
        /// The last shopping cart count.
        /// </value>
        public int LastShoppingCartCount => this.SerialEntry.PositionCount;

        /// <summary>
        /// Gets a value indicating whether this instance has running change requests.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has running change requests; otherwise, <c>false</c>.
        /// </value>
        public bool HasRunningChangeRequests => this.runningRequests > 0;

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        //public UIImage PositionImage { get; private set; }    // CRM-5007

        /// <summary>
        /// Gets the type of the serial entry.
        /// </summary>
        /// <value>
        /// The type of the serial entry.
        /// </value>
        public SerialEntryType SerialEntryType { get; private set; }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineSerialEntryRequest OfflineRequest { get; private set; }

        /// <summary>
        /// Gets the organizer context items.
        /// </summary>
        /// <value>
        /// The organizer context items.
        /// </value>
        public Dictionary<string, string> OrganizerContextItems { get; private set; }

        /// <summary>
        /// Gets the change request error.
        /// </summary>
        /// <value>
        /// The change request error.
        /// </value>
        public Exception ChangeRequestError { get; private set; }

        /// <summary>
        /// Gets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public string Sections { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [hierarchical position filters].
        /// </summary>
        /// <value>
        /// <c>true</c> if [hierarchical position filters]; otherwise, <c>false</c>.
        /// </value>
        public bool HierarchicalPositionFilters => this.hierarchicalPositionFilters;

        /// <summary>
        /// Gets a value indicating whether [rights filter revocation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [rights filter revocation]; otherwise, <c>false</c>.
        /// </value>
        public bool RightsFilterRevocation { get; private set; }

        /// <summary>
        /// Gets or sets the cached result for filter dictionary.
        /// </summary>
        /// <value>
        /// The cached result for filter dictionary.
        /// </value>
        public Dictionary<int, List<UPSERow>> CachedResultForFilterDictionary { get; set; }

        public List<UPSERow> DisplayedRows
        {
            get
            {
                return this.displayedRows;
            }
            set
            {
                this.displayedRows = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryPageModelController"/> class.
        /// </summary>
        /// <param name="_viewReference">The view reference.</param>
        /// <param name="_offlineRequest">The offline request.</param>
        public SerialEntryPageModelController(ViewReference _viewReference, UPOfflineSerialEntryRequest _offlineRequest = null)
            : base(_viewReference)
        {
            this.filterMapping = new Dictionary<string, UPSEFilter>();
            this.CachedResultForFilterDictionary = new Dictionary<int, List<UPSERow>>();
            string hpfAsString = this.ViewReference.ContextValueForKey("HierarchicalPositionFilter");
            if (!string.IsNullOrEmpty(hpfAsString) && hpfAsString.ToLower() == "true")
            {
                this.hierarchicalPositionFilters = true;
            }

            this.documentManager = new DocumentManager();
            this.OfflineRequest = _offlineRequest;
            this.BuildPage();
            this.ApplyLoadingStatusOnPage((Page)this.TopLevelElement);
        }

        /// <summary>
        /// Applies the loading status on page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"))
            {
                FieldValue = LocalizedString.TextLoadingData
            };
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Gets the serial entry page model controller delegate.
        /// </summary>
        /// <value>
        /// The serial entry page model controller delegate.
        /// </value>
        public UPSerialEntryPageModelControllerDelegate SerialEntryPageModelControllerDelegate =>
            (UPSerialEntryPageModelControllerDelegate)this.ModelControllerDelegate;

        private UPSEFilter CurrentFilter
        {
            get
            {
                if (this.SerialEntryPage.SelectedFilter == null)
                {
                    return this.SerialEntry.Filters.Count == 0 ? UPSEFilter.AllPositionsFilter() : this.SerialEntry.Filters[0];
                }

                return this.filterMapping[this.SerialEntryPage.SelectedFilter.Name];
            }
        }

        private void BuildFilterControl()
        {
            this.SerialEntryPage.RemoveAllFilters();
            foreach (UPSEFilter filter in this.SerialEntry.Filters)
            {
                if (filter.IsListingFilter && this.SerialEntry.ListingController == null)
                {
                    continue;
                }

                FieldIdentifier identifier = FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(filter.InfoAreaId, filter.Label, string.Empty);
                UPMNoParamFilter noParamFilter = new UPMNoParamFilter(identifier)
                {
                    Name = filter.Name,
                    DisplayName = filter.Label
                };
                this.SerialEntryPage.AddAvailableFilter(noParamFilter);
                if (filter.Name != null)
                {
                    this.filterMapping[filter.Name] = filter;
                }
            }

            this.SerialEntryPage.RemoveAllPositionFilters();
            foreach (string filterName in this.positionFilters)
            {
                if (filterName.Contains(":Listing") && this.SerialEntry.ListingController != null)
                {
                    StringIdentifier identifier = StringIdentifier.IdentifierWithStringId("Listing");
                    UPCatalogValueProvider listingValueProvider = this.SerialEntry.ListingController;
                    UPMCatalogFilter catalogFilter = new UPMCatalogFilter(identifier, listingValueProvider)
                    {
                        ParameterName = LocalizedString.TextProcessFilterListings,
                        Name = "Listing",
                        DisplayName = LocalizedString.TextProcessFilterListings,
                        DisabledName = LocalizedString.TextSerialEntryNoListing
                    };
                    this.SerialEntryPage.AddAvailablePositionFilter(catalogFilter);
                }
                else
                {
                    UPMFilter tmpFilter = UPMFilter.FilterForName(filterName, this.SerialEntry.InitialFieldValuesForDestination);
                    if (tmpFilter != null)
                    {
                        this.SerialEntryPage.AddAvailablePositionFilter(tmpFilter);
                    }
                }
            }
        }

        private UPMField ApplyAttributesOnField(UPConfigFieldControlField fieldConfig, UPMEditField editField)
        {
            if (editField != null && fieldConfig.Attributes.ReadOnly)
            {
                editField.EditMode = EditFieldEditMode.Readonly;
            }

            if (fieldConfig.Attributes.ExtendedOptionIsSet("newLine"))
            {
                UPMSerialEntryEditField serialEntryEditField = (UPMSerialEntryEditField)editField;
                serialEntryEditField.NewLine = true;
            }

            if (fieldConfig.Attributes.ExtendedOptionIsSet("oneColumn"))
            {
                UPMSerialEntryEditField serialEntryEditField = (UPMSerialEntryEditField)editField;
                serialEntryEditField.OneColumn = true;
            }

            return editField;
        }

        private UPMSerialEntryCatalogEditField CreateSelectorField(UPSEDestinationColumnBase destinationColumn, UPMSEPosition position, IIdentifier fieldIdentifier, string value)
        {
            UPMSerialEntrySelectorCatalogEditField field = new UPMSerialEntrySelectorCatalogEditField(fieldIdentifier, destinationColumn, position, false)
            {
                MultiSelectMaxCount = 0
            };
            UPConfigFieldControlField fieldConfig = destinationColumn.FieldConfig;
            if (destinationColumn.FieldConfig.Attributes.ExtendedOptionForKey("catalogStyle") != null
                && destinationColumn.FieldConfig.Attributes.ExtendedOptionForKey("catalogStyle") == "popOver")
            {
                field.SerialEntryCatalogStyle = UPSerialEntryCatalogStyle.Popover;
            }
            else
            {
                field.SerialEntryCatalogStyle = UPSerialEntryCatalogStyle.Keyboard;
            }

            UPSelector selector;
            if (destinationColumn.Selector != null)
            {
                selector = destinationColumn.Selector;
            }
            else
            {
                selector = UPSelector.SelectorFor(position.Row.RowRecordIdentification, fieldConfig.Attributes.Selector, null, fieldConfig);
                selector.Build();
                destinationColumn.Selector = selector;
            }

            if (selector.OptionCount == 0 && selector.IsStaticSelector)
            {
                selector = null;
                return null;
            }

            field.Selector = selector;
            field.SelectorOptions = selector.PossibleValues;
            List<string> explicitKeyOrder = selector.PossibleValues.Keys.ToList();
            foreach (var entry in field.SelectorOptions)
            {
                UPMCatalogPossibleValue possibleValue = new UPMCatalogPossibleValue();
                UPMStringField valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"))
                {
                    StringValue = entry.Value.Name
                };
                possibleValue.TitleLabelField = valueField;
                field.AddPossibleValue(possibleValue, entry.Key);
            }

            field.FieldValue = string.IsNullOrEmpty(value) ? "0" : value;

            this.ApplyAttributesOnField(fieldConfig, field);
            field.ExplicitKeyOrder = explicitKeyOrder;
            field.ContinuousUpdate = true;
            return field;
        }

        private UPMSerialEntryCatalogEditField CreateCatalogEditField(UPSEDestinationColumnBase destinationColumn, UPMSEPosition position,
            IIdentifier fieldIdentifier, string value, string parentValue, List<UPSEColumn> childFields)
        {
            UPMSerialEntryCatalogEditField field = new UPMSerialEntryCatalogEditField(fieldIdentifier, destinationColumn, position, childFields.Count > 0)
            {
                MultiSelectMaxCount = childFields.Count
            };
            UPConfigFieldControlField fieldConfig = destinationColumn.FieldConfig;
            if (destinationColumn.FieldConfig.Attributes.ExtendedOptionForKey("catalogStyle") != null
                && destinationColumn.FieldConfig.Attributes.ExtendedOptionForKey("catalogStyle") == "popOver")
            {
                field.SerialEntryCatalogStyle = UPSerialEntryCatalogStyle.Popover;
            }
            else
            {
                field.SerialEntryCatalogStyle = UPSerialEntryCatalogStyle.Keyboard;
            }

            bool isFixedCatalog = fieldConfig.Field.FieldType == "X";
            UPCatalog catalog = isFixedCatalog
                ? UPCRMDataStore.DefaultStore.CatalogForFixedCatalogId(fieldConfig.Field.CatNo)
                : UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(fieldConfig.Field.CatNo);

            Dictionary<string, string> possibleValues = null;
            List<string> explicitKeyOrder = null;
            if (destinationColumn.ParentColumnIndex >= 0)
            {
                List<UPCatalogValue> parentValues = catalog?.SortedValuesForParentValueIncludeHidden(Convert.ToInt32(parentValue), false);
                if (parentValues?.Count > 0)
                {
                    possibleValues = UPCatalog.ValueDictionaryForCatalogValues(parentValues);
                    explicitKeyOrder = UPCatalog.ExplicitKeyOrderForCatalogValues(parentValues);
                }
            }
            else
            {
                possibleValues = catalog?.TextValuesForFieldValues(false);
                explicitKeyOrder = catalog?.ExplicitKeyOrder;
            }

            if (possibleValues != null)
            {
                foreach (var entry in possibleValues)
                {
                    UPMCatalogPossibleValue possibleValue = new UPMCatalogPossibleValue();
                    UPMStringField valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"));
                    valueField.StringValue = entry.Value;
                    possibleValue.TitleLabelField = valueField;
                    field.AddPossibleValue(possibleValue, entry.Key);
                }
            }

            if (!string.IsNullOrEmpty(value) && (possibleValues?.ContainsKey(value) ?? false))
            {
                UPMCatalogPossibleValue possibleValue = new UPMCatalogPossibleValue();
                UPMStringField valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"));
                string textValue = catalog.TextValueForKey(value);
                if (string.IsNullOrEmpty(textValue))
                {
                    textValue = $"?{value}";
                }

                valueField.StringValue = textValue;
                possibleValue.TitleLabelField = valueField;
                field.AddPossibleValue(possibleValue, value);
                if (explicitKeyOrder != null)
                {
                    explicitKeyOrder = new List<string>(explicitKeyOrder) { value };
                }
            }

            field.FieldValue = string.IsNullOrEmpty(value) ? "0" : value;

            this.ApplyAttributesOnField(fieldConfig, field);
            field.ExplicitKeyOrder = explicitKeyOrder;
            if (fieldConfig.Field.FieldType == "X" && field.PossibleValueForKey("0").TitleLabelField.StringValue.Length == 0)
            {
                field.NullValueKey = "0";
            }
            else if (fieldConfig.Field.FieldType == "K")
            {
                field.NullValueKey = "0";
            }

            field.ContinuousUpdate = true;
            return field;
        }

        private UPMField DestinationFieldForColumnPositionRowChildColumns(UPSEDestinationColumnBase destinationColumn, UPMSEPosition position, UPSERow row, List<UPSEColumn> childColumns)
        {
            UPCRMField crmField = destinationColumn.FieldConfig.Field;
            IIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId("field");
            if (destinationColumn.FieldConfig.Attributes.Selector != null)
            {
                string value = row.ValueAtIndex(destinationColumn.Index) as string;
                if (!string.IsNullOrEmpty(value) && (crmField.FieldType == "K" || crmField.FieldType == "X"))
                {
                    UPCatalog cat = UPCRMDataStore.DefaultStore.CatalogForCrmField(crmField);
                    string stringValue = cat.TextValueForKey(value);
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        value = stringValue;
                    }
                }

                return this.CreateSelectorField(destinationColumn, position, fieldIdentifier, value);
            }

            if (crmField.FieldType == "K" || crmField.FieldType == "X")
            {
                string value = row.ValueAtIndex(destinationColumn.Index) as string;
                string parentValue = row.ParentCatalogValueAtIndex(destinationColumn.Index) as string;
                return this.CreateCatalogEditField(destinationColumn, position, fieldIdentifier, value, parentValue, childColumns);
            }

            if (crmField.FieldType == "D")
            {
                UPMSerialEntryDateTimeEditField field = new UPMSerialEntryDateTimeEditField(fieldIdentifier, destinationColumn, position, DateTimeType.Date);
                string rawValue = row.ValueAtIndex(destinationColumn.Index) as string;
                field.DateValue = rawValue.DateFromCrmValue();
                return this.ApplyAttributesOnField(destinationColumn.FieldConfig, field);
            }

            if (crmField.FieldType == "T")
            {
                UPMSerialEntryDateTimeEditField field = new UPMSerialEntryDateTimeEditField(fieldIdentifier, destinationColumn, position, DateTimeType.Time);
                string rawValue = row.ValueAtIndex(destinationColumn.Index) as string;
                field.DateValue = rawValue.TimeFromCrmValue();
                return this.ApplyAttributesOnField(destinationColumn.FieldConfig, field);
            }

            if (crmField.FieldType == "C")
            {
                UPMSerialEntryStringEditField field = new UPMSerialEntryStringEditField(fieldIdentifier, destinationColumn, position);
                FieldAttributes fieldAttributes = destinationColumn.FieldConfig.Attributes;
                field.IsMultiline = fieldAttributes.MultiLine;
                if (field.IsMultiline)
                {
                    field.OneColumn = true;
                    field.MultiLineHeight = fieldAttributes.MultiLineHeight;
                }

                int fieldLength = destinationColumn.FieldConfig.Field.FieldInfo.FieldLength;
                field.MaxLength = fieldLength > 0 ? fieldLength : 0;
                string v = row.ValueAtIndex(destinationColumn.Index) as string;
                if (!string.IsNullOrEmpty(v))
                {
                    field.StringValue = v;
                }
                else if (destinationColumn.FieldConfig.Attributes.Empty)
                {
                    return null;
                }
                else
                {
                    field.StringValue = string.Empty;
                }

                return this.ApplyAttributesOnField(destinationColumn.FieldConfig, field);
            }

            if (crmField.FieldType == "B")
            {
                UPMSerialEntryBooleanEditField field = new UPMSerialEntryBooleanEditField(fieldIdentifier, destinationColumn, position);
                string value = row.ValueAtIndex(destinationColumn.Index) as string;
                field.BoolValue = value == "true" || value == "1";
                field.EditInList = destinationColumn.Function == "ListEditField";
                return this.ApplyAttributesOnField(destinationColumn.FieldConfig, field);
            }

            if (crmField.FieldInfo.PercentField)
            {
                UPMSerialEntryPercentEditField field = new UPMSerialEntryPercentEditField(fieldIdentifier, destinationColumn, position);
                if (destinationColumn.FieldConfig.Attributes.FieldStyle == "OptionalWrite")
                {
                    field.EditMode = EditFieldEditMode.OptionalWriteable;
                }

                decimal value = row.NumberValueFromColumn(destinationColumn);
                field.NumberValue = value != 0 ? Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture) : 0;

                return this.ApplyAttributesOnField(destinationColumn.FieldConfig, field);
            }

            if (crmField.FieldType == "F")
            {
                //throw new Exception("Complete implementation");
                //UPMSerialEntryFloatEditField field = new UPMSerialEntryFloatEditField(fieldIdentifier, destinationColumn, position);
                UPMSerialEntryNumberEditField field = new UPMSerialEntryNumberEditField(fieldIdentifier, destinationColumn, position);
                return this.ApplyAttributes(field, destinationColumn, row, position);
            }
            else
            {
                UPMSerialEntryNumberEditField field = new UPMSerialEntryNumberEditField(fieldIdentifier, destinationColumn, position);
                return this.ApplyAttributes(field, destinationColumn, row, position);
            }
        }

        private UPMField ApplyAttributes(UPMSerialEntryNumberEditField field, UPSEDestinationColumnBase destinationColumn, UPSERow row, UPMSEPosition position)
        {
            if (destinationColumn.FieldConfig.Attributes.FieldStyle == "OptionalWrite")
            {
                field.EditMode = EditFieldEditMode.OptionalWriteable;
            }

            field.QuantityStepField = destinationColumn.FieldConfig.Attributes.ExtendedOptionIsSet("stepableField") || destinationColumn.FieldConfig.Attributes.ExtendedOptionIsSet("StepableField");
            if (destinationColumn.FieldConfig.Attributes.ExtendedOptionForKey("supportsDecimals") != null)
            {
                field.SupportsDecimals = destinationColumn.FieldConfig.Attributes.ExtendedOptionIsSet("supportsDecimals");
            }

            decimal value = row.NumberValueFromColumn(destinationColumn);
            field.NumberValue = value != 0 ? Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture) : 0;

            if (this.SerialEntry.IsStepSizeColumn(destinationColumn))
            {
                field.HasStepSizeCheck = true;
            }

            if (this.SerialEntry.IsMinMaxColumn(destinationColumn))
            {
                field.HasMinMaxCheck = true;
            }

            if (field.HasStepSizeCheck)
            {
                field.StepSize = row.StepSizeForColumnIndex(destinationColumn.Index);
                if (field.StepSize > 1)
                {
                    UPMSEPositionInfoMessage stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.StepSizeInfoMessageKey);
                    stepSizeInfoMessage.MessageField.StringValue = string.Format(LocalizedString.TextSerialEntryPackageSize, field.StepSize);
                    int intValue = Convert.ToInt32(field.NumberValue);
                    if (intValue > 0 && (intValue % field.StepSize) != 0)
                    {
                        stepSizeInfoMessage.ErrorLevelMessage = true;
                        this.countOfStepSizeViolations++;
                        field.ViolatesStep = true;
                    }
                }
            }
            else
            {
                field.StepSize = 1;
            }

            if (field.HasMinMaxCheck)
            {
                field.MinQuantity = row.MinQuantity;
                field.MaxQuantity = row.MaxQuantity;
                if (field.MinQuantity > 0 || field.MaxQuantity > 0)
                {
                    UPMSEPositionInfoMessage stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.MinMaxInfoMessageKey);
                    stepSizeInfoMessage.MessageField.StringValue = this.MinMaxMessageForMinValueMaxValue(field.MinQuantity, field.MaxQuantity);
                    int intValue = Convert.ToInt32(field.NumberValue);
                    if (field.MinQuantity > 0 && intValue > 0 && intValue < field.MinQuantity)
                    {
                        stepSizeInfoMessage.ErrorLevelMessage = true;
                        ++this.countOfMinMaxViolations;
                        field.ViolatesMinMax = true;
                    }
                    else if (field.MaxQuantity > 0 && intValue > 0 && intValue > field.MaxQuantity)
                    {
                        stepSizeInfoMessage.ErrorLevelMessage = true;
                        ++this.countOfMinMaxViolations;
                        field.ViolatesMinMax = true;
                    }
                    else
                    {
                        stepSizeInfoMessage.ErrorLevelMessage = false;
                    }
                }
            }

            return this.ApplyAttributesOnField(destinationColumn.FieldConfig, field);
        }

        private void UpdateChildrenGroup(UPMGroup shippingDatesGroup)
        {
            UPSESourceChildColumn currentSourceChildColumn = null;
            bool initSourceChildColumn = true;
            this.countOfStepSizeViolations = 0;
            this.countOfMinMaxViolations = 0;
            UPMSEPositionWithContext position = (UPMSEPositionWithContext)shippingDatesGroup.Parent;
            UPSERow row = position.Row;
            shippingDatesGroup.RemoveAllChildren();
            foreach (UPSEColumn col in row.RowConfiguration.Columns)
            {
                if (col.ColumnFrom == UPSEColumnFrom.SourceChild)
                {
                    if (initSourceChildColumn)
                    {
                        currentSourceChildColumn = (UPSESourceChildColumn)col;
                        initSourceChildColumn = false;
                    }

                    continue;
                }

                if (col.ColumnFrom != UPSEColumnFrom.DestChild || col.Hidden)
                {
                    continue;
                }

                initSourceChildColumn = true;
                UPSEDestinationChildColumn column = (UPSEDestinationChildColumn)col;
                UPMField field = this.DestinationFieldForColumnPositionRowChildColumns(column, position, row, null);
                if (field != null)
                {
                    string sourceReplacement = string.Empty;
                    if (currentSourceChildColumn != null)
                    {
                        sourceReplacement = this.SerialEntry.FormattedValueForColumnRow(currentSourceChildColumn, row);
                    }

                    string childFieldPostfix = (column.ChildIndex + 1).ToString();
                    field.LabelText = column.Label.StringByReplacingOccurrencesOfParameterWithIndexWithString(0, childFieldPostfix)
                        .StringByReplacingOccurrencesOfParameterWithIndexWithString(1, sourceReplacement);
                    if (position.InitialFocusField == null)
                    {
                        if (column.InitialFocus == InitialFocusMode.IfEmpty)
                        {
                            if (column.CrmField.IsEmptyValue(row.StringValueAtIndex(column.Index)))
                            {
                                position.InitialFocusField = field;
                            }
                        }
                        else
                        {
                            position.InitialFocusField = field;
                        }
                    }

                    shippingDatesGroup.AddField(field);
                }
            }

            shippingDatesGroup.Invalid = false;
        }

        private void UpdatePositionGroup(UPMGroup orderPositionGroup)
        {
            UPMSEPositionWithContext position = (UPMSEPositionWithContext)orderPositionGroup.Parent;
            UPSERow row = position.Row;
            orderPositionGroup.RemoveAllChildren();
            position.InitialFocusField = null;
            int childFieldCount = 0;
            UPSEDestinationColumn mainDestinationColumn = null;
            List<UPSEColumn> childDestinationColumns = null;
            foreach (UPSEColumn col in row.RowConfiguration.Columns)
            {
                if (col.Hidden || col.ColumnFrom != UPSEColumnFrom.Dest)
                {
                    continue;
                }

                UPSEDestinationColumn column = (UPSEDestinationColumn)col;
                if (childFieldCount > 0)
                {
                    childDestinationColumns.Add(column);
                    --childFieldCount;
                    if (childFieldCount > 0)
                    {
                        continue;
                    }

                    column = mainDestinationColumn;
                }
                else
                {
                    childFieldCount = column.FieldConfig.Attributes.FieldCount - 1;
                    if (childFieldCount > 0)
                    {
                        childDestinationColumns = new List<UPSEColumn>();
                        mainDestinationColumn = column;
                        continue;
                    }
                }

                UPMField field = this.DestinationFieldForColumnPositionRowChildColumns(column, position, row, childDestinationColumns);
                field.ListingSourceValue = row.ListingSourceValueForColumn(column);
                if (field != null)
                {
                    field.LabelText = column.Label;
                    orderPositionGroup.AddField(field);
                    if (position.InitialFocusField == null)
                    {
                        if (column.InitialFocus == InitialFocusMode.IfEmpty)
                        {
                            if (column.CrmField.IsEmptyValue(row.StringValueAtIndex(column.Index)))
                            {
                                position.InitialFocusField = field;
                            }
                        }
                        else
                        {
                            position.InitialFocusField = field;
                        }
                    }
                }
            }

            orderPositionGroup.Invalid = false;
        }

        private void UpdateInfoPanelName(UPMSEPositionInfoPanel group, string name)
        {
            UPMSEPositionWithContext position = (UPMSEPositionWithContext)group.Parent;
            UPSERow row = position.Row;
            foreach (UPSerialEntryInfo infoPanel in this.SerialEntry.SerialEntryRowInfoPanels)
            {
                if (infoPanel.Name == name)
                {
                    group.TableTyp = infoPanel.VerticalRows == false ? UPInfoPanelTableTyp.Vertikal : UPInfoPanelTableTyp.Horizontal;
                    UPSerialEntryInfoResult infoResult = infoPanel.ResultForRow(row);
                    if (infoResult.Rows.Count > 0)
                    {
                        List<UPMStringField> columnNames = new List<UPMStringField>();
                        List<UPMContainer> rows = new List<UPMContainer>();
                        UPMStringField titleLabelField = UPMStringField.StringFieldWithIdentifierValue(
                            StringIdentifier.IdentifierWithStringId($"{position.Identifier}-label"), infoPanel.Label);
                        FieldControl fieldControl = null;
                        if (infoPanel is UPSerialEntrySourceRowInfo)
                        {
                            fieldControl = ((UPSerialEntrySourceRowInfo)infoPanel).FieldControl;
                        }

                        int count = 0;
                        foreach (string columnName in infoResult.ColumnNames)
                        {
                            UPMStringField labelField = UPMStringField.StringFieldWithIdentifierValueLabel(
                                StringIdentifier.IdentifierWithStringId($"{position.Identifier}-columnName-{count}"), columnName, columnName);
                            columnNames.Add(labelField);
                            if (fieldControl != null && count < fieldControl.NumberOfFields)
                            {
                                UPConfigFieldControlField fieldControlField = fieldControl.FieldAtIndex(count);
                                labelField.SetAttributes(fieldControlField.Attributes);
                                if (fieldControlField.TargetFieldNumber > 0)
                                {
                                    group.UpdateColumnWidthInPercentColumn(fieldControlField.TargetFieldNumber, count);
                                }
                            }

                            count++;
                        }

                        int i = 0;
                        foreach (UPSerialEntryInfoRow infoRow in infoResult.Rows)
                        {
                            UPMContainer currentRow = new UPMContainer(StringIdentifier.IdentifierWithStringId($"{position.Identifier}-row-{i}"));
                            List<string> infoRowCells = infoRow.Cells;
                            int j = 0;
                            foreach (string data in infoRowCells)
                            {
                                UPMStringField dataField = UPMStringField.StringFieldWithIdentifierValue(
                                    StringIdentifier.IdentifierWithStringId($"{position.Identifier}-cell-{i}-{j}"), data);
                                currentRow.AddChild(dataField);
                                if (fieldControl != null && j < fieldControl.NumberOfFields)
                                {
                                    UPConfigFieldControlField fieldControlField = fieldControl.FieldAtIndex(j);
                                    dataField.SetAttributes(fieldControlField.Attributes);
                                    UPInfoPanelCellTextAlignment alignment = fieldControlField.Field.FieldInfo.PercentField
                                        || fieldControlField.Field.FieldInfo.AmountField || fieldControlField.Field.FieldType == "L"
                                        || fieldControlField.Field.FieldType == "S" || fieldControlField.Field.FieldType == "F"
                                        || fieldControlField.Field.FieldType == "N" ? UPInfoPanelCellTextAlignment.Right : UPInfoPanelCellTextAlignment.Left;

                                    group.UpdateTextAlignmentForCellRowColumn(alignment, i, j);
                                }

                                j++;
                            }

                            rows.Add(currentRow);
                            i++;
                        }

                        group.TitleField = titleLabelField;
                        group.ColumnNames = columnNames;
                        group.Rows = rows;
                    }

                    group.Invalid = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Updates the name of the documents group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="name">The name.</param>
        public void UpdateDocumentsGroupName(UPMDocumentsGroup group, string name)
        {
            UPMSEPositionWithContext position = (UPMSEPositionWithContext)group.Parent;
            UPSERow row = position.Row;
            int index = 0;
            foreach (UPSerialEntryDocuments documents in this.SerialEntry.SerialEntryRowDocuments)
            {
                if (documents.Name == name)
                {
                    group.RemoveAllChildren();
                    if (documents.WithAddButton)
                    {
                        group.AddField(new UPMDocument(documents.AddPhotoDirectButtonName, index, row.SerialEntryRecordIdentification));
                    }

                    UPCRMResult documentsResult = documents.ResultForRowRow(this.SerialEntry, row);
                    if (documentsResult != null && documentsResult.RowCount > 0)
                    {
                        IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                        FieldControl documentFieldControl = configStore.FieldControlByNameFromGroup("List", "D1DocData");
                        DocumentInfoAreaManager documentInfoAreaManager = new DocumentInfoAreaManager(documentFieldControl.InfoAreaId, documentFieldControl, null);
                        for (int i = 0; i < documentsResult.RowCount; i++)
                        {
                            UPCRMResultRow resultRow = (UPCRMResultRow)documentsResult.ResultRowAtIndex(i);
                            DocumentData documentData = documentInfoAreaManager.DocumentDataForResultRow(resultRow);
                            UPMDocument document = new UPMDocument(documentData);
                            group.AddField(document);
                        }
                    }
                }

                group.Invalid = false;
                index++;
            }
        }

        /// <summary>
        /// Updates the group for list display.
        /// </summary>
        /// <param name="group">The group.</param>
        public void UpdateGroupForListDisplay(UPMGroup group)
        {
            if (group.Identifier.IdentifierAsString == "children")
            {
                this.UpdateChildrenGroup(group);
            }
            else if (group.Identifier.IdentifierAsString == "position")
            {
              //  this.UpdatePositionGroup(group);
            }
        }

        /// <summary>
        /// Updates the group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void UpdateGroup(UPMGroup group)
        {
            if (group.Identifier.IdentifierAsString == "children")
            {
                this.UpdateChildrenGroup(group);
            }
            else if (group.Identifier.IdentifierAsString == "position")
            {
                this.UpdatePositionGroup(group);
            }
            else if (group.Identifier.IdentifierAsString.StartsWith("infoPanel"))
            {
                this.UpdateInfoPanelName((UPMSEPositionInfoPanel)group, group.Identifier.IdentifierAsString.Substring(10));
            }
            else if (group.Identifier.IdentifierAsString.StartsWith("documentGroup"))
            {
                this.UpdateDocumentsGroupName((UPMDocumentsGroup)group, group.Identifier.IdentifierAsString.Substring(14));
            }
        }

        private string MinMaxMessageForMinValueMaxValue(int minValue, int maxValue)
        {
            if (minValue > 0)
            {
                if (maxValue > 0)
                {
                    return string.Format(LocalizedString.TextSerialEntryMinMaxQuantity.Replace("%i - %i", "{0} - {1}"), minValue, maxValue);
                }

                return string.Format(LocalizedString.TextSerialEntryMinQuantity.Replace("%i", "{0}"), minValue);
            }

            return string.Format(LocalizedString.TextSerialEntryMaxQuantity.Replace("%i", "{0}"), maxValue);
        }

        private void CheckValueForPositionRowField(UPMSEPosition position, UPSERow row, UPMField field)
        {
            UPMSEPositionInfoMessage stepSizeInfoMessage;
            if ((this.SerialEntry.Quota?.HasQuotaColumns ?? false) && !row.UnlimitedQuota)
            {
                stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.QuotaInfoMessageKey);
                int remainingQuota = row.RemainingQuota;
                stepSizeInfoMessage.MessageField.StringValue = string.Format(LocalizedString.TextSerialEntryQuota.Replace("%i","{0}"), remainingQuota);
                stepSizeInfoMessage.ErrorLevelMessage = false;
                stepSizeInfoMessage.ErrorLevelMessage = remainingQuota < 0;

                return;
            }

            if (!this.SerialEntry.HasMinMaxColumns && !this.SerialEntry.HasStepSizeColumns)
            {
                return;
            }

            bool stepSizeDefault = true;
            bool minMaxDefault = true;
            if (field is UPMSerialEntryNumberEditField)
            {
                UPMSerialEntryNumberEditField numberEditField = (UPMSerialEntryNumberEditField)field;
                if (numberEditField.StepSize > 1)
                {
                    if (this.countOfStepSizeViolations == 0 || (this.countOfStepSizeViolations == 1 && numberEditField.ViolatesStep))
                    {
                        stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.StepSizeInfoMessageKey);
                        stepSizeInfoMessage.MessageField.StringValue = string.Format(LocalizedString.TextSerialEntryPackageSize.Replace("%i", "{0}"), numberEditField.StepSize);
                        int intValue = Convert.ToInt32(numberEditField.NumberValue);
                        if (intValue > 0 && (intValue % numberEditField.StepSize != 0))
                        {
                            stepSizeInfoMessage.ErrorLevelMessage = true;
                        }
                        else
                        {
                            stepSizeInfoMessage.ErrorLevelMessage = false;
                        }

                        stepSizeDefault = false;
                    }
                }

                if (numberEditField.MinQuantity > 0 || numberEditField.MaxQuantity > 0)
                {
                    if (this.countOfMinMaxViolations == 0 || (this.countOfMinMaxViolations == 1 && numberEditField.ViolatesMinMax))
                    {
                        int intValue = Convert.ToInt32(numberEditField.NumberValue);
                        bool violation = intValue > 0 && ((numberEditField.MinQuantity > 0 && intValue < numberEditField.MinQuantity) || (numberEditField.MaxQuantity > 0 && intValue > numberEditField.MaxQuantity));
                        stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.MinMaxInfoMessageKey);
                        stepSizeInfoMessage.MessageField.StringValue = this.MinMaxMessageForMinValueMaxValue(numberEditField.MinQuantity, numberEditField.MaxQuantity);
                        stepSizeInfoMessage.ErrorLevelMessage = violation;
                        minMaxDefault = false;
                    }
                }
            }

            if (stepSizeDefault && this.SerialEntry.HasStepSizeColumns && this.countOfStepSizeViolations == 0)
            {
                int packageSize = row.DefaultStepSize;
                if (packageSize > 1)
                {
                    stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.StepSizeInfoMessageKey);
                    stepSizeInfoMessage.MessageField.StringValue = string.Format(LocalizedString.TextSerialEntryPackageSize, packageSize);
                    stepSizeInfoMessage.ErrorLevelMessage = false;
                }
                else
                {
                    position.RemoveInfoMessagesForKey(Constants.StepSizeInfoMessageKey);
                }
            }

            if (minMaxDefault && this.SerialEntry.HasMinMaxColumns && this.countOfMinMaxViolations == 0)
            {
                int minQuantity = row.MinQuantity;
                int maxQuantity = row.MaxQuantity;
                if (minQuantity > 0 || maxQuantity > 0)
                {
                    stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.MinMaxInfoMessageKey);
                    stepSizeInfoMessage.MessageField.StringValue = this.MinMaxMessageForMinValueMaxValue(minQuantity, maxQuantity);
                    stepSizeInfoMessage.ErrorLevelMessage = false;
                }
                else
                {
                    position.RemoveInfoMessagesForKey(Constants.MinMaxInfoMessageKey);
                }
            }
        }

        private void SetFieldValuesForPositionRow(UPMSEPosition position, UPSERow row)
        {
            this.SetTitleFieldValuesForPositionRow(position, row);
            if (this.SerialEntry.RowDisplayFieldControl == null)
            {
                this.SetThirdRowCurrencyFieldValuesForPositionRow(position, row);
            }

            if (this.SerialEntry.RowDisplayFieldControl != null)
            {
                this.SetFieldValuesFromSerialEntryRowLineForPositionRow(position, row);
            }
            else if (this.SerialEntryType == SerialEntryType.Order || this.SerialEntryType == SerialEntryType.Offer)
            {
                this.SetFieldValuesFromFunctionsForPositionRow(position, row);
            }
            else
            {
                position.PositionSelected = row.HasRecord;
                position.PositionError = row.Error;
            }

            position.ServerApproved = row.ServerApproved;
        }

        private void SetFieldValuesFromFunctionsForPositionRow(UPMSEPosition position, UPSERow row)
        {
            var currencyText = this.SerialEntry.FormattedValueForColumnWithFunctionRow(FunctionCurrency, row);
            var quantityText = this.SerialEntry.FormattedValueForColumnWithFunctionRow(FunctionQuantity, row);
            var unitPrice = this.SerialEntry.FormattedValueForColumnWithFunctionRow(FunctionUnitPrice, row);
            if (string.IsNullOrWhiteSpace(quantityText) || quantityText == "0")
            {
                quantityText = currencyText;
                position.PositionSelected = this.markPositionIfRecordExists && row.HasRecord;
            }
            else
            {
                quantityText = string.Format(LocalizedString.TextQuantityXCurrency.Replace("%@ x %@", "{0} x {1}"), quantityText, currencyText);               
                position.PositionSelected = !this.markPositionIfRecordExists || row.HasRecord;
            }

            position.PositionError = row.Error;
            var endPriceFunctionName = FunctionNetPrice;
            var totalPrice = this.SerialEntry.FormattedValueForColumnWithFunctionRow(endPriceFunctionName, row);

            object dontShowRebates = null;
            this.SerialEntry?.AdditionalOptions?.TryGetValue(KeyDontShowRebates, out dontShowRebates);

            var dontShowRebatesAsInt = 0;
            if (dontShowRebates != null)
            {
                int.TryParse(dontShowRebates.ToString(), out dontShowRebatesAsInt);
            }

            if (string.IsNullOrWhiteSpace(totalPrice) || this.showRowLineEndPrice || dontShowRebatesAsInt != 0)
            {
                endPriceFunctionName = FunctionEndPrice;
                totalPrice = this.SerialEntry.FormattedValueForColumnWithFunctionRow(endPriceFunctionName, row);
            }

            position.TotalQuantityField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdQuantity), quantityText);
            position.TotalQuantityField.SetAttributes(this.SerialEntry.DestColumnsForFunction[FunctionQuantity].FieldConfig.Attributes);
            position.PriceField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdUnitPrice), unitPrice);
            if (this.SerialEntry.DestColumnsForFunction.ContainsKey(FunctionUnitPrice))
            {
                position.PriceField.SetAttributes(this.SerialEntry.DestColumnsForFunction[FunctionUnitPrice].FieldConfig.Attributes);
            }
            if (string.IsNullOrWhiteSpace(totalPrice) || totalPrice == "0")
            {
                totalPrice = string.Empty;
                currencyText = string.Empty;
            }

            position.TotalPriceField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdEndPrice), totalPrice);
            if (this.SerialEntry.DestColumnsForFunction.ContainsKey(endPriceFunctionName))
            {
                position.TotalPriceField.SetAttributes(this.SerialEntry.DestColumnsForFunction[endPriceFunctionName].FieldConfig.Attributes);
            }
            position.TotalPriceCurrencyField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdCurrency), currencyText);
            if (this.SerialEntry.DestColumnsForFunction.ContainsKey(FunctionCurrency))
            {
                position.TotalPriceCurrencyField.SetAttributes(this.SerialEntry.DestColumnsForFunction[FunctionCurrency].FieldConfig.Attributes);
            }
        }

        private void SetFieldValuesFromSerialEntryRowLineForPositionRow(UPMSEPosition position, UPSERow row)
        {
            var rowTexts = this.SerialEntry.RowLinePartsForRow(row);
            if (rowTexts.Count > 2)
            {
                position.TotalQuantityField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdQuantity), rowTexts[0]);
                position.TotalQuantityField.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(0).Attributes);
                position.TotalPriceCurrencyField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdCurrency), rowTexts[1]);
                position.TotalPriceCurrencyField.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(1).Attributes);
                position.PriceField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdUnitPrice), rowTexts[2]);
                position.PriceField.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(2).Attributes);
                if (rowTexts.Count > 3)
                {
                    position.TotalPriceField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdEndPrice), rowTexts[3]);
                    position.TotalPriceField.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(3).Attributes);
                }

                if (rowTexts.Count > 4)
                {
                    position.ThirdRowCurrency = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdThird2), rowTexts[4]);
                    position.ThirdRowCurrency.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(4).Attributes);
                }

                if (rowTexts.Count > 5)
                {
                    position.ThirdRowValue = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdThird3), rowTexts[5]);
                    position.ThirdRowValue.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(5).Attributes);
                }
            }
            else
            {
                if (rowTexts.Count > 0)
                {
                    position.PriceField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdUnitPrice), rowTexts[0]);
                    position.PriceField.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(0).Attributes);
                    if (rowTexts.Count > 1)
                    {
                        position.TotalPriceField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdEndPrice), rowTexts[1]);
                        position.TotalPriceField.SetAttributes(this.SerialEntry.RowDisplayListFormatter.FirstFieldForPosition(1).Attributes);
                    }
                }
            }

            position.PositionSelected = row.HasRecord;
            position.PositionError = row.Error;
        }

        private void SetThirdRowCurrencyFieldValuesForPositionRow(UPMSEPosition position, UPSERow row)
        {
            position.ThirdRowCurrency = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdThird2), this.SerialEntry.FormattedSourceValueForColumnSpanIndexRow(3, row));
            position.ThirdRowCurrency.SetAttributes(this.SerialEntry.FieldAttributesForColumnSpanIndex(3));
            position.ThirdRowValue = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdThird3), this.SerialEntry.FormattedSourceValueForColumnSpanIndexRow(4, row));
            position.ThirdRowValue.SetAttributes(this.SerialEntry.FieldAttributesForColumnSpanIndex(4));
        }

        private void SetTitleFieldValuesForPositionRow(UPMSEPosition position, UPSERow row)
        {
            position.TitleField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdTitle), this.SerialEntry.FormattedSourceValueForColumnSpanIndexRow(0, row));
            position.TitleField.SetAttributes(this.SerialEntry.FieldAttributesForColumnSpanIndex(0));
            position.SubtitleField = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdSubtitle), this.SerialEntry.FormattedSourceValueForColumnSpanIndexRow(1, row));
            position.SubtitleField.SetAttributes(this.SerialEntry.FieldAttributesForColumnSpanIndex(1));
            position.ThirdRowTitle = UPMStringField.StringFieldWithIdentifierValue(StringIdentifier.IdentifierWithStringId(StringIdThird1), this.SerialEntry.FormattedSourceValueForColumnSpanIndexRow(2, row));
            position.ThirdRowTitle.SetAttributes(this.SerialEntry.FieldAttributesForColumnSpanIndex(2));
        }

        /// <summary>
        /// Sets the current position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SetCurrentPosition(UPMSEPosition position)
        {
            UPSERow row = ((UPMSEPositionWithContext)position).Row;
            if (row == null)
            {
                return;
            }

            if (this.OrganizerContextItems.Count > 0)
            {
                foreach (string key in this.OrganizerContextItems.Keys)
                {
                    string contextValueName = this.OrganizerContextItems[key];
                    string value = row.ValueForFunctionName(key);
                    this.Delegate.PageModelControllerSetContextValueForKey(this, value, contextValueName);
                }
            }
        }

        private void AddListEditFieldsPositionRow(UPMGroup orderPositionGroup, UPMSEPosition position, UPSERow row)
        {
            foreach (UPSEColumn col in row.RowConfiguration.Columns)
            {
                //UPSEDestinationColumn column = (UPSEDestinationColumn)col;
                UPSEDestinationColumn column;
                if (col is UPSEDestinationColumn)
                {
                    column = (UPSEDestinationColumn)col;
                    //else
                    //    column = this.ConvertUPSESourceColumnToUPSEDestinationColumn((UPSESourceColumn)col);

                    bool stepable = column.FieldConfig.Attributes.ExtendedOptionIsSet("stepableField")
                        || column.FieldConfig.Attributes.ExtendedOptionIsSet("StepableField")
                        || col.Function == "Quantity";

                    if (column.Function == "ListEditField")
                    {
                        UPMSerialEntryBooleanEditField field = (UPMSerialEntryBooleanEditField)this.DestinationFieldForColumnPositionRowChildColumns(column, position, row, null);
                        if (field != null)
                        {
                            field.ListingSourceValue = row.ListingSourceValueForColumn(column);
                            position.ListeEditBooleanField = field;
                            field.LabelText = column.Label;
                            orderPositionGroup.AddField(field);
                        }

                        break;
                    }

                    if (this.ViewReference.ContextValueIsSet("QuantityChangeInList") && stepable)
                    {
                        UPMSerialEntryNumberEditField field = (UPMSerialEntryNumberEditField)this.DestinationFieldForColumnPositionRowChildColumns(column, position, row, null);
                        if (field != null)
                        {
                            field.ListingSourceValue = row.ListingSourceValueForColumn(column);
                            position.ListeEditQuantityField = field;
                            field.LabelText = column.Label;
                            orderPositionGroup.AddField(field);
                        }

                        break;
                    }
                }
            }
        }

        private UPSEDestinationColumn ConvertUPSESourceColumnToUPSEDestinationColumn(UPSESourceColumn srcCol)
        {
            UPSEDestinationColumn destCol = new UPSEDestinationColumn(srcCol.FieldConfig, srcCol.Index, srcCol.ParentColumnIndex, srcCol.PositionInControl, srcCol.InfoAreaId);
            return destCol;
        }

        /// <summary>
        /// Updates the position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void UpdatePosition(UPMSEPosition position)
        {
            UPSERow row = ((UPMSEPositionWithContext)position).Row;
            if (row == null)
            {
                return;
            }

            row.EnsureLoaded();
            this.CheckValueForPositionRowField(position, row, null);
            this.SetFieldValuesForPositionRow(position, row);
            position.RemoveAllDetailInputGroups();
            if (this.hasEditableChildFields)
            {
                UPMGroup shippingDatesGroup = new UPMGroup(StringIdentifier.IdentifierWithStringId("children"))
                {
                    Invalid = true
                };
                position.AddDetailInputGroup(shippingDatesGroup);
                this.AddListEditFieldsPositionRow(shippingDatesGroup, position, row);
            }

            if (this.hasEditablePositionFields)
            {
                UPMGroup orderPositionGroup = new UPMGroup(StringIdentifier.IdentifierWithStringId("position"))
                {
                    Invalid = true
                };
                position.AddDetailInputGroup(orderPositionGroup);
                this.AddListEditFieldsPositionRow(orderPositionGroup, position, row);
            }

            if (this.SerialEntry.SerialEntryRowInfoPanels?.Count > 0)
            {
                foreach (UPSerialEntryInfo infoPanel in this.SerialEntry.SerialEntryRowInfoPanels)
                {
                    UPMSEPositionInfoPanel infoPanelGroup = new UPMSEPositionInfoPanel(StringIdentifier.IdentifierWithStringId($"infoPanel-{infoPanel.Name}"))
                    {
                        Invalid = true
                    };
                    position.AddDetailInputGroup(infoPanelGroup);
                }
            }

            if (this.SerialEntry.SerialEntryRowDocuments?.Count > 0)
            {
                foreach (UPSerialEntryDocuments infoDocumentGroup in this.SerialEntry.SerialEntryRowDocuments)
                {
                    UPMDocumentsGroup documentGroup = new UPMDocumentsGroup(StringIdentifier.IdentifierWithStringId($"documentGroup-{infoDocumentGroup.Name}"))
                    {
                        LabelText = infoDocumentGroup.Label
                    };
                    string style = infoDocumentGroup.Style;
                    switch (style)
                    {
                        case "IMG":
                            documentGroup.Style = UPMDocumentsGroupStyle.Image;
                            break;

                        case "NOIMG":
                            documentGroup.Style = UPMDocumentsGroupStyle.NoImages;
                            break;

                        case "DEFAULT":
                            documentGroup.Style = UPMDocumentsGroupStyle.Default;
                            break;

                        default:
                            documentGroup.Style = UPMDocumentsGroupStyle.Default;
                            break;
                    }

                    documentGroup.Invalid = true;
                    position.AddDetailInputGroup(documentGroup);
                }
            }

            //if (this.PositionImage != null)
            //{
            //    position.Icon = this.PositionImage;       // CRM-5007
            //}

            if (this.SerialEntry.DocumentKeyColumn != null)
            {
                position.ImageField = this.ImageFieldForDocumentKey(row.DocumentKey);
            }

            position.Invalid = false;
        }

        private UPMImageEditField ImageFieldForDocumentKey(string documentKey)
        {
            UPMImageEditField imageField = !string.IsNullOrEmpty(documentKey)
                ? new UPMImageEditField(StringIdentifier.IdentifierWithStringId(documentKey))
                : new UPMImageEditField(StringIdentifier.IdentifierWithStringId("no image"));

            imageField.ImageDocumentMaxSize = new Size(176, 176);
            FieldAttribute attribute = this.SerialEntry.DocumentKeyColumn.FieldConfig.Attributes.AttributForId((int)FieldAttr.Image);
            if (attribute?.ValueOptionsForKey("previewWidth") != null && attribute?.ValueOptionsForKey("previewHeight") != null)
            {
                object width = attribute.ValueOptionsForKey("previewWidth");
                object height = attribute.ValueOptionsForKey("previewHeight");
                imageField.ImageDocumentMaxSize = new Size(Convert.ToInt32(width), Convert.ToInt32(height));
            }

            if (!string.IsNullOrEmpty(documentKey))
            {
                DocumentData documentData = this.documentManager.DocumentForKey(documentKey);
                UPMDocument imageDocument = documentData != null ? new UPMDocument(documentData)
                    : new UPMDocument(imageField.Identifier, ServerSession.CurrentSession.DocumentRequestUrlForDocumentKey(documentKey));

                imageField.ImageDocument = imageDocument;
            }

            return imageField;
        }

        private void InitialBuildPositions()
        {
            //if (this.SerialEntry.PositionCount > 0)
           if (this.SerialEntry.Positions.Count > 0)
           {
                bool disableStartWithShoppingCart = false;

                if (this.SerialEntry?.AdditionalOptions != null)
                {
                    disableStartWithShoppingCart = Convert.ToInt32(this.SerialEntry?.AdditionalOptions["DisableShoppingCartStart"]) != 0;
                }
                
                
                this.SerialEntryPage.ShowShoppingCart = !disableStartWithShoppingCart;
            }

            if (this.SerialEntry.ActiveFilter != null && !this.SerialEntryPage.ShowShoppingCart)
            {
                this.SerialEntry.RowsForInitialFilters();
                this.initialFilterLoaded = true;
            }
            else
            {
                this.BuildPositionFields();
            }
        }

        private void BuildPositionFields()
        {
            UPSEFilter currentFilter;
            if (this.SerialEntryPage.ShowShoppingCart)
            {
                currentFilter = UPSEFilter.AllPositionsFilter();
            }
            else if (this.showErrorRows)
            {
                currentFilter = UPSEFilter.ErrorPositionsFilter();
            }
            else
            {
                currentFilter = this.CurrentFilter;
            }

            List<UPConfigFilter> allSearchFilters = new List<UPConfigFilter>();
            List<UPConfigFilter> searchFilterArray = UPMFilter.ActiveFiltersForFilters(this.SerialEntryPage.AdditionalSearchFilters);
            if (searchFilterArray.Count > 0)
            {
                allSearchFilters.AddRange(searchFilterArray);
            }

            List<UPConfigFilter> positionFilterArray = UPMFilter.ActiveFiltersForFilters(this.SerialEntryPage.AvailablePositionFilters);
            if (positionFilterArray.Count > 0)
            {
                allSearchFilters.AddRange(positionFilterArray);
            }

            this.SerialEntry.RowsForFilter(currentFilter, ((UPMSEPage)this.TopLevelElement).SearchText, allSearchFilters);
        }

        private void BuildPage()
        {
            SerialEntryType type;
            Enum.TryParse(this.ViewReference.ContextValueForKey("EditType"), out type);
            this.SerialEntryType = type;

            var recordId = this.ViewReference.ContextValueForKey("RecordId");
            this.Sections = this.ViewReference.ContextValueForKey("Sections");
            IIdentifier pageIdentifier;
            if (!recordId.IsRecordIdentification())
            {
                pageIdentifier = StringIdentifier.IdentifierWithStringId("root");
            }
            else
            {
                pageIdentifier = new RecordIdentifier(recordId);
            }

            var page = new UPMSEPage(pageIdentifier);
            this.TopLevelElement = page;
            this.markPositionIfRecordExists = true;
            if (this.SerialEntryType == SerialEntryType.Order || this.SerialEntryType == SerialEntryType.Offer)
            {
                page.ShowSumLine = true;
                this.markPositionIfRecordExists = !ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("SerialEntry.MarkOrderPositionOnQuantity", false);
            }
            else
            {
                page.ShowSumLine = false;
            }

            var configStore = ConfigurationUnitStore.DefaultStore;
            page.ProductCatalogFullscreen = configStore.ConfigValueIsSetDefaultValue("SerialEntry.ProductCatalogFullscreen", true);

            var val = configStore.ConfigValue(@"Search.OnlineDelayTime");
            page.OnlineWaitTime = !string.IsNullOrEmpty(val) ? Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture) : Aurea.CRM.Services.ModelControllers.Search.Constants.OnlineDefaultDelaySeconds;

            val = configStore.ConfigValue(@"Search.OfflineDelayTime");
            page.OfflineWaitTime = !string.IsNullOrEmpty(val) ? Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture) : 0.3;

            page.Invalid = true;
            this.availableFilters = this.FilterNameFromViewReferenceFilterKeyAdditionalFilterKey(this.ViewReference, "SearchFilter", "SearchAdditionalFilter");
            if (this.availableFilters.Count > 0)
            {
                this.SerialEntryPage.AdditionalSearchFilters = new List<UPMFilter>();
            }

            this.positionFilters = this.FilterNameFromViewReferenceFilterKeyAdditionalFilterKey(this.ViewReference, "PositionFilter", "PositionAdditionalFilter");
            var configValue = this.ViewReference.ContextValueForKey("OrganizerContextValues");
            if (!string.IsNullOrWhiteSpace(configValue))
            {
                this.SetOrganizerContextItems(configValue);
            }

            page.ScanMode = this.ViewReference.ContextValueIsSet("ScanMode");
            page.ScanAddQuantity = this.ViewReference.ContextValueIsSet("ScanAddQuantity");
            var changeQuantityInList = this.ViewReference.ContextValueIsSet("QuantityChangeInList");
            page.ListQuantityChangeEnabled = changeQuantityInList;

            this.SetFinishAction(page, configStore);

            var additionalOptions = this.ViewReference.ContextValueForKey("Options")?.JsonDictionaryFromString();
            this.SetCloseButtonText(additionalOptions);
            this.SetCloseAction(additionalOptions);
            this.SetCancelAction(additionalOptions);
        }

        /// <summary>
        /// Performs the cancel action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PerformCancelAction(object sender)
        {
            if (this.ParentOrganizerModelController is SerialEntryOrganizerModelController)
            {
                ((SerialEntryOrganizerModelController)this.ParentOrganizerModelController).PopToPreviousContentViewController();
            }
        }

        /// <summary>
        /// Performs the close action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PerformCloseAction(object sender)
        {
            if (this.ParentOrganizerModelController is SerialEntryOrganizerModelController)
            {
                ((SerialEntryOrganizerModelController)this.ParentOrganizerModelController).PerformFinishAction(null);
            }
        }

        /// <summary>
        /// Performs the finish action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PerformFinishAction(object sender)
        {
            if (this.ParentOrganizerModelController is SerialEntryOrganizerModelController)
            {
                ((SerialEntryOrganizerModelController)this.ParentOrganizerModelController).PerformFinishAction(this.finishActionName);
            }
        }

        private List<string> FilterNameFromViewReferenceFilterKeyAdditionalFilterKey(ViewReference _viewReference, string filterKey, string additionalFilterKey)
        {
            List<string> filterNames = new List<string>();
            string availableFilterName;
            if (!string.IsNullOrEmpty(filterKey))
            {
                for (int i = 1; i <= 5; i++)
                {
                    availableFilterName = _viewReference.ContextValueForKey($"{filterKey}{i}");
                    if (!string.IsNullOrEmpty(availableFilterName))
                    {
                        filterNames.Add(availableFilterName);
                    }
                }
            }

            if (!string.IsNullOrEmpty(additionalFilterKey))
            {
                availableFilterName = _viewReference.ContextValueForKey(additionalFilterKey);
                if (!string.IsNullOrEmpty(availableFilterName))
                {
                    var filterParts = availableFilterName.Split(';');
                    filterNames.AddRange(filterParts);
                }
            }

            return filterNames;
        }

        private void ContinueAfterSuccessfulSerialEntryBuild()
        {
            string configuredSummaryTitleString = this.SerialEntry.AdditionalOptions.ValueOrDefault("SummaryTitle") as string;
            this.SerialEntryPage.SummaryTitle = configuredSummaryTitleString != null
                ? configuredSummaryTitleString.ReferencedStringWithDefault(LocalizedString.TextSerialEntrySummaryTitle)
                : LocalizedString.TextSerialEntrySummaryTitle;

            string configuredButtonTitleString = this.SerialEntry.AdditionalOptions.ValueOrDefault("AllItemsButtonTitle") as string;
            this.SerialEntryPage.AllItemsButtonTitle = configuredButtonTitleString != null
                ? configuredButtonTitleString.ReferencedStringWithDefault(LocalizedString.TextSerialEntryAllItemsButtonTitle)
                : LocalizedString.TextSerialEntryAllItemsButtonTitle;

            string configuredSummaryButtonTitleString = this.SerialEntry.AdditionalOptions.ValueOrDefault("SummaryButtonTitle") as string;
            this.SerialEntryPage.SummaryButtonTitle = configuredSummaryButtonTitleString != null
                ? configuredSummaryButtonTitleString.ReferencedStringWithDefault(LocalizedString.TextSerialEntrySummaryButtonTitle)
                : LocalizedString.TextSerialEntrySummaryButtonTitle;

            this.SerialEntryPage.DisableDuplicateRow = Convert.ToInt32(this.SerialEntry.AdditionalOptions.ValueOrDefault("DisableDuplicateRow")) != 0;
            this.SerialEntryPage.DisableDeleteRow = Convert.ToInt32(this.SerialEntry.AdditionalOptions.ValueOrDefault("DisableDeleteRow")) != 0
                || Convert.ToInt32(this.SerialEntry.AdditionalOptions.ValueOrDefault("disableRowDelete")) != 0;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.showFreeGoodsPrice = configStore.ConfigValueIsSet("SerialEntry.ShowFreeGoodsPrice");
            this.showRowLineEndPrice = configStore.ConfigValueIsSet("SerialEntry.RowLineShowEndPrice");
            this.showSumLineEndPrice = configStore.ConfigValueIsSet("SerialEntry.SumLineShowEndPrice");
            this.showSumLineWithAllRows = configStore.ConfigValueIsSet("SerialEntry.SumLineWithAllRows");
            this.Delegate.PageModelControllerSetContextValueForKey(this, this.SerialEntry.Record.RecordIdentification, "RootRecordIdentification");

            List<UPMFilter> upmFilterArray = new List<UPMFilter>();
            foreach (string filterNameItem in this.availableFilters)
            {
                if (!string.IsNullOrEmpty(filterNameItem))
                {
                    UPMFilter filter = UPMFilter.FilterForName(filterNameItem, this.SerialEntry.InitialFieldValuesForDestination);
                    if (filter != null)
                    {
                        upmFilterArray.Add(filter);
                    }
                }
            }

            if (upmFilterArray.Count > 0)
            {
                this.SerialEntryPage.AdditionalSearchFilters = upmFilterArray;
            }

            StringBuilder searchLabel = new StringBuilder();
            int count = this.SerialEntry.SourceSearchControl.NumberOfFields;
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    searchLabel.Append(this.SerialEntry.SourceSearchControl.FieldAtIndex(i).Label);
                }
                else
                {
                    searchLabel.AppendFormat(" | {0}", this.SerialEntry.SourceSearchControl.FieldAtIndex(i).Label);
                }
            }

            this.SerialEntryPage.SearchPlaceholder = searchLabel.ToString();
            this.SerialEntryPage.ConflictHandling = this.SerialEntry.ConflictHandling;
            UPMAction searchAction = new UPMAction(null)
            {
                Enabled = true
            };
            searchAction.SetTargetAction(this, this.Search);
            this.SerialEntryPage.SearchAction = searchAction;
            UPMAction scannerSearchAction = new UPMAction(null)
            {
                Enabled = true
            };
            scannerSearchAction.SetTargetAction(this, this.ScannerSearch);
            this.SerialEntryPage.ScannerSearchAction = scannerSearchAction;
            this.hasEditableChildFields = this.SerialEntry.ChildrenCount > 0;
            foreach (UPSEColumn col in this.SerialEntry.Columns)
            {
                if (!col.Hidden && col.ColumnFrom == UPSEColumnFrom.Dest)
                {
                    this.hasEditablePositionFields = true;
                    break;
                }
            }

            InfoArea infoAreaConfig = ConfigurationUnitStore.DefaultStore.InfoAreaConfigById(this.SerialEntry.SourceInfoAreaId);
            if (infoAreaConfig != null)
            {
                //this.PositionImage = UIImage.UpImageWithFileName(infoAreaConfig.ImageName);   // CRM-5007
            }

            this.SerialEntryPage.ListImageViewAvailable = this.SerialEntry.DocumentKeyColumn != null;
            this.BuildFilterControl();
            this.InitialBuildPositions();
        }

        /// <summary>
        /// Scanners the search.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ScannerSearch(object sender)
        {
            this.addToDisplayedRows = true;
            this.SerialEntry.RowsForFilter(UPSEFilter.SingleArticleFilter(), ((UPMSEPage)this.TopLevelElement).SearchText, null);
        }

        /// <summary>
        /// Leaves the shopping cart with filter.
        /// </summary>
        /// <param name="_filter">The filter.</param>
        public void LeaveShoppingCartWithFilter(UPMFilter _filter)
        {
            this.SerialEntryPage.ShowShoppingCart = false;
            if (!this.initialFilterLoaded && this.SerialEntry.ActiveFilter != null)
            {
                this.SerialEntry.RowsForInitialFilters();
                this.initialFilterLoaded = true;
            }
            else
            {
                this.PositionsForFilter(_filter);
            }
        }

        /// <summary>
        /// Positionses for filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void PositionsForFilter(UPMFilter filter)
        {
            this.showErrorRows = false;
            if (this.SerialEntryPage.SearchAction != null)
            {
                this.SerialEntryPage.SearchAction.Enabled = true;
            }
            this.SerialEntryPage.SelectedFilter = filter;
            UPMSEPage oldPage = this.SerialEntryPage;
            UPMSEPage newPage = (UPMSEPage)this.UpdatedElement(this.Page);
            if (newPage.Invalid == false)
            {
                this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
            }
        }

        /// <summary>
        /// Unselects the shopping cart and filters.
        /// </summary>
        /// <returns></returns>
        public bool UnselectShoppingCartAndFilters()
        {
            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, UPChangeHints.ChangeHintsWithHint(Constants.UnselectFiltersAndShoppingCartChangeHint));
            return false;
        }

        /// <summary>
        /// Updates the sum line.
        /// </summary>
        public void UpdateSumLine()
        {
            if (this.SerialEntryType == SerialEntryType.Order || this.SerialEntryType == SerialEntryType.Offer)
            {
                if (this.SerialEntry is UPSEOrder)
                {
                    UPMSESummaryGroup summaryGroup = new UPMSESummaryGroup(StringIdentifier.IdentifierWithStringId("SESummaryGroup"));
                    UPSEOrder order = (UPSEOrder)this.SerialEntry;
                    if (!this.sumLineInitialized)
                    {
                        this.sumLineInitialized = true;
                        this.sumLineCurrencyText = this.SerialEntry.InitialFieldValuesForDestination.ValueOrDefault("Currency") as string;
                        if (!string.IsNullOrEmpty(this.sumLineCurrencyText))
                        {
                            UPSEColumn destColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault("Currency");
                            if (destColumn != null)
                            {
                                this.sumLineCurrencyText = destColumn.CrmField.ValueForRawValueOptions(this.sumLineCurrencyText, 0);
                            }
                            else
                            {
                                string currencyText = this.SerialEntry.InitialFieldValuesForDestination.ValueOrDefault("Currency.text") as string;
                                if (!string.IsNullOrEmpty(currencyText))
                                {
                                    this.sumLineCurrencyText = currencyText;
                                }
                            }
                        }
                    }

                    double endPrice;
                    double freeGoodsValue = 0;
                    int freeGoods;
                    int units;
                    bool complete = this.displayedRows.Count == 0 || this.showSumLineWithAllRows;
                    bool dontshowrebates = this.showSumLineEndPrice || Convert.ToInt32(this.SerialEntry.AdditionalOptions.ValueOrDefault("DontShowRebates")) != 0;
                    if (!complete)
                    {
                        units = order.UnitCountForRows(this.displayedRows);
                        freeGoods = order.FreeGoodsCountForRows(this.displayedRows);
                        endPrice = dontshowrebates ? order.EndPriceForRows(this.displayedRows) : order.NetPriceForRows(this.displayedRows);
                        if (this.showFreeGoodsPrice && freeGoods != 0)
                        {
                            freeGoodsValue = order.FreeGoodsPriceForRows(this.displayedRows);
                        }
                    }
                    else
                    {
                        units = order.UnitCount;
                        freeGoods = order.FreeGoodsCount;
                        endPrice = dontshowrebates ? order.EndPrice : order.NetPrice;
                        if (this.showFreeGoodsPrice && freeGoods != 0)
                        {
                            freeGoodsValue = order.FreeGoodsPrice;
                        }
                    }

                    UPMSESummaryGroupItem totalAmountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("totalAmountItem"));
                    UPMSESummaryGroupItem itemCountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("itemCountItem"));
                    itemCountItem.UpdateWithTitleItems(LocalizedString.TextProcessSerialEntrySumItems, units);
                    UPMSESummaryGroupItem freeGoodsAmountItem = null;
                    UPMSESummaryGroupItem freeGoodsItemCountItem = null;
                    string totalPrice = string.Format("{0:#,0.00}", endPrice);
                    totalAmountItem.UpdateWithTitleValueCurrency(LocalizedString.TextProcessSerialEntrySumTotal, totalPrice, this.sumLineCurrencyText);
                    if (freeGoods > 0 && this.showFreeGoodsPrice && freeGoodsValue > 0.004)
                    {
                        itemCountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("itemCountItem"));
                        itemCountItem.UpdateWithTitleItems(LocalizedString.TextProcessSerialEntrySumItems, units);
                        freeGoodsAmountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("freeGoodsAmountItem"));
                        string freeGoodsPriceString = freeGoodsValue.ToString();
                        freeGoodsAmountItem.UpdateWithTitleValueCurrency(string.Empty, freeGoodsPriceString, this.sumLineCurrencyText);
                        freeGoodsItemCountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("freeGoodsItemCountItem"));
                        freeGoodsItemCountItem.UpdateWithTitleItems(LocalizedString.TextProcessSerialEntryFreeGoodsSumTotal, freeGoods);
                    }
                    else if (freeGoods > 0)
                    {
                        itemCountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("itemCountItem"));
                        itemCountItem.UpdateWithTitleItems(LocalizedString.TextProcessSerialEntrySumItems, units);
                        freeGoodsItemCountItem = new UPMSESummaryGroupItem(StringIdentifier.IdentifierWithStringId("freeGoodsItemCountItem"));
                        freeGoodsItemCountItem.UpdateWithTitleItems(LocalizedString.TextProcessSerialEntryFreeGoodsSumTotal, freeGoods);
                    }

                    summaryGroup.AddChild(totalAmountItem);
                    summaryGroup.AddChild(itemCountItem);
                    if (freeGoodsItemCountItem != null)
                    {
                        summaryGroup.AddChild(freeGoodsItemCountItem);
                        if (freeGoodsAmountItem != null)
                        {
                            summaryGroup.AddChild(freeGoodsAmountItem);
                        }
                    }

                    this.SerialEntryPage.SummaryGroup = summaryGroup;
                    this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, UPChangeHints.ChangeHintsWithHint(Constants.SumLineUpdatedChangeHint));
                }
            }
        }

        /// <summary>
        /// Positionses the with error.
        /// </summary>
        public void PositionsWithError()
        {
            this.UnselectShoppingCartAndFilters();
            this.showErrorRows = true;
            this.SerialEntryPage.ShowShoppingCart = false;
            this.SerialEntryPage.SelectedFilter = null;
            UPMSEPage oldPage = this.SerialEntryPage;
            UPMSEPage newPage = (UPMSEPage)this.UpdatedElement(this.Page);
            newPage.SearchAction.Enabled = false;
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        /// <summary>
        /// Positionses for shopping cart.
        /// </summary>
        public void PositionsForShoppingCart()
        {
            this.SerialEntryPage.ShowShoppingCart = true;
            this.showErrorRows = false;
            this.SerialEntryPage.SelectedFilter = null;
            UPMSEPage oldPage = this.SerialEntryPage;
            UPMSEPage newPage = (UPMSEPage)this.UpdatedElement(this.Page);
            if(newPage != null)
            {
                if (newPage.SearchAction != null)
                {
                    newPage.SearchAction.Enabled = false;
                }
            }
            
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        /// <summary>
        /// Rightses the checker.
        /// </summary>
        private void RightsChecker()
        {
            string rightsFilterName = this.ViewReference.ContextValueForKey("RightsFilterName");
            if (!string.IsNullOrEmpty(rightsFilterName))
            {
                UPConfigFilter rightsFilter = ConfigurationUnitStore.DefaultStore.FilterByName(rightsFilterName);
                if (rightsFilter != null)
                {
                    this.rightsChecker = new UPRightsChecker(rightsFilter);
                    string _recordIdentification = this.ViewReference.ContextValueForKey("RecordId");
                    this.rightsChecker.CheckPermission(_recordIdentification, false, this);
                    return;
                }
            }

            this.RightsCheckerGrantsPermission(null, null);
        }

        /// <summary>
        /// Creates the serial entry.
        /// </summary>
        public void CreateSerialEntry()
        {
            string _recordId = this.ViewReference.ContextValueForKey("RecordId");
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "viewReference", this.ViewReference } };
            this.SerialEntry = UPSerialEntry.SerialEntryOfType(this.SerialEntryType, _recordId, parameters, this.OfflineRequest, this);
            if (ServerSession.CurrentSession.IsEnterprise)
            {
                this.SerialEntry.RequestOption = ServerSession.CurrentSession.SerialEntryModeOnline ? (ServerSession.CurrentSession.IsServerReachable ? UPRequestOption.Online : UPRequestOption.Offline) : UPRequestOption.Offline;
            }

            this.SerialEntry.Build();
        }

        /// <summary>
        /// Updates the element for changes in background with changes.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public override void UpdateElementForChangesInBackground(List<IIdentifier> changes)
        {
            if (changes.Count == 0)
            {
                base.UpdateElementForChangesInBackground(changes);
            }
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="page1">The page1.</param>
        /// <returns></returns>
        public Page UpdatedElementForPage(Page page1)
        {
            if (this.SerialEntry == null)
            {
                this.RightsChecker();
                return this.Page;
            }

            UPMSEPage oldPage = this.SerialEntryPage;
            UPMSEPage page = new UPMSEPage(oldPage.Identifier)
            {
                AvailableFilters = oldPage.AvailableFilters,
                AvailablePositionFilters = oldPage.AvailablePositionFilters,
                AdditionalSearchFilters = oldPage.AdditionalSearchFilters,
                SelectedFilter = oldPage.SelectedFilter,
                SearchText = oldPage.SearchText,
                ShowShoppingCart = oldPage.ShowShoppingCart,
                SearchAction = oldPage.SearchAction,
                SearchPlaceholder = oldPage.SearchPlaceholder,
                ScanMode = oldPage.ScanMode,
                ScanAddQuantity = oldPage.ScanAddQuantity,
                ListImageViewAvailable = oldPage.ListImageViewAvailable,
                ParentInfoPanel = oldPage.ParentInfoPanel,
                FinishAction = oldPage.FinishAction,
                CloseAction = oldPage.CloseAction,
                CancelAction = oldPage.CancelAction,
                NumberOfDisplayedRows = oldPage.NumberOfDisplayedRows,
                ProductCatalogs = oldPage.ProductCatalogs,
                ProductCatalogFullscreen = oldPage.ProductCatalogFullscreen,
                ListQuantityChangeEnabled = oldPage.ListQuantityChangeEnabled,
                SummaryButtonTitle = oldPage.SummaryButtonTitle,
                SummaryTitle = oldPage.SummaryTitle,
                AllItemsButtonTitle = oldPage.AllItemsButtonTitle,
                CloseButtonText = oldPage.CloseButtonText,
                DisableDuplicateRow = oldPage.DisableDuplicateRow,
                DisableDeleteRow = oldPage.DisableDeleteRow,
                ConflictHandling = this.SerialEntry.ConflictHandling
            };
            this.TopLevelElement = page;
            this.BuildPositionFields();
            return this.Page;
        }

        /// <summary>
        /// Searches the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        public void Search(object page)
        {
            this.SerialEntryPage.ShowShoppingCart = false;
            this.showErrorRows = false;
            UPMSEPage oldPage = this.SerialEntryPage;
            UPMSEPage newPage = (UPMSEPage)this.UpdatedElement(this.Page);
            if (!newPage.Invalid)
            {
                this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
            }
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
            if (element is UPMSEPage)
            {
                return this.UpdatedElementForPage((UPMSEPage)element);
            }

            if (element is UPMSEPositionWithContext)
            {
                UPMSEPositionWithContext position = new UPMSEPositionWithContext(element.Identifier);
                position.Row = ((UPMSEPositionWithContext)element).Row;
                this.UpdatePosition(position);
                return position;
            }

            if (element is UPMSEPositionInfoPanel)
            {
                UPMSEPositionInfoPanel infoPanel = new UPMSEPositionInfoPanel(element.Identifier);
                this.UpdateInfoPanelName(infoPanel, element.Identifier.IdentifierAsString.Substring(10));
                return infoPanel;
            }

            if (element is UPMGroup)
            {
                UPMGroup group = new UPMGroup(element.Identifier);
                this.UpdateGroup(group);
                return group;
            }

            return element;
        }

        /// <summary>
        /// Removes the order position from shopping cart.
        /// </summary>
        /// <param name="position">The position.</param>
        public void RemoveOrderPositionFromShoppingCart(UPMSEPosition position)
        {
            UPSERow row = null;
            if (position != null)
            {
                position.Invalid = true;
                row = ((UPMSEPositionWithContext)position).Row;
            }

            if (this.SerialEntry.DisableRowDelete && !string.IsNullOrEmpty(row.SerialEntryRecordIdentification))
            {
                return;
            }

            if (this.SerialEntry.ConflictHandling)
            {
                return;
            }

            if (!this.SerialEntry.DisableSingleRowUpdate)
            {
                //this.WillChangeValueForKey("hasRunningChangeRequests");
                ++this.runningRequests;

                if (!this.SerialEntry.DeleteRowContext(row, position))
                {
                    --this.runningRequests;
                }

                //this.DidChangeValueForKey("hasRunningChangeRequests");
            }
        }

        /// <summary>
        /// Discards the changes.
        /// </summary>
        /// <param name="position">The position.</param>
        public void DiscardChanges(UPMSEPosition position)
        {
            if (this.lastEditField != null && this.lastEditPosition != null)
            {
                this.UserDidChangeFieldForPositionEditFinished(this.lastEditField, this.lastEditPosition, true);
            }

            if (position.PositionSelected)
            {
                this.DiscardRowChangesFromShoppingCart(position);
            }
            else
            {
                this.RemoveOrderPositionFromShoppingCart(position);
            }
        }

        /// <summary>
        /// Discards the row changes from shopping cart.
        /// </summary>
        /// <param name="position">The position.</param>
        public void DiscardRowChangesFromShoppingCart(UPMSEPosition position)
        {
            UPSERow row = null;
            if (position != null)
            {
                row = ((UPMSEPositionWithContext)position).Row;
                if (row == null)
                {
                    return;
                }
            }

            this.SerialEntry.RefreshRow(row);
            position.Invalid = true;
            if (position.Identifier != null)
            {
                List<IIdentifier> changedElements = new List<IIdentifier> { position.Identifier };
                this.InformAboutDidChangeTopLevelElement(null, null, changedElements, UPChangeHints.ChangeHintsWithHint(Constants.DiscardRowChangesHint));
            }
        }

        private void InvalidateDependentRowsForPositionRowDependentRows(UPMSEPosition _position, UPSERow currentRow, List<UPSERow> dependentRows)
        {
            List<UPMElement> visiblePositions = this.SerialEntryPage.Children;
            List<IIdentifier> invalidatedPositions = null;
            bool isDependent;

            if (currentRow == null)
            {
                currentRow = _position.Row;
            }

            foreach (UPMElement child in visiblePositions)
            {
                if (child is UPMResultSection)
                {
                    UPMResultSection resultSection = (UPMResultSection)child;
                    foreach (UPMElement sectionChild in resultSection.Children)
                    {
                        if (sectionChild is UPMSEPositionWithContext)
                        {
                            UPMSEPositionWithContext position = (UPMSEPositionWithContext)sectionChild;
                            isDependent = dependentRows?.Contains(position.Row) ?? position.Row.IsDependentOnRow(currentRow);

                            if (isDependent)
                            {
                                position.Invalid = true;
                                foreach (UPMElement group in position.Children)
                                {
                                    group.Invalid = true;
                                }

                                if (position.Row.HasRecord)
                                {
                                    this.UpdatePosition(position);
                                }

                                if (invalidatedPositions == null)
                                {
                                    invalidatedPositions = new List<IIdentifier> { position.Identifier };
                                }
                                else
                                {
                                    invalidatedPositions.Add(position.Identifier);
                                }
                            }
                        }
                    }
                }
                else if (child is UPMSEPositionWithContext)
                {
                    UPMSEPositionWithContext position = (UPMSEPositionWithContext)child;
                    isDependent = dependentRows?.Contains(position.Row) ?? position.Row.IsDependentOnRow(currentRow);

                    if (isDependent)
                    {
                        position.Invalid = true;
                        if (invalidatedPositions == null)
                        {
                            invalidatedPositions = new List<IIdentifier> { position.Identifier };
                        }
                        else
                        {
                            invalidatedPositions.Add(position.Identifier);
                        }
                    }
                }
            }

            this.InformAboutDidChangeTopLevelElement(null, null, invalidatedPositions, null);
        }

        /// <summary>
        /// Saves the changed position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public List<Exception> SaveChangedPosition(UPMSEPosition position)
        {
            return this.SaveChangedPositionDocumentsIndexDataFileNamePattern(position, -1, null, null);
        }

        /// <summary>
        /// Saves the changed position documents index data file name pattern.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="documentsIndex">Index of the documents.</param>
        /// <param name="data">The data.</param>
        /// <param name="fileNamePattern">The file name pattern.</param>
        /// <returns></returns>
        public List<Exception> SaveChangedPositionDocumentsIndexDataFileNamePattern(UPMSEPosition position, int documentsIndex, byte[] data, string fileNamePattern)
        {
            if (this.SerialEntry.ConflictHandling)
            {
                return null;
            }

            if (this.lastEditField != null && this.lastEditPosition != null)
            {
                this.UserDidChangeFieldForPositionEditFinished(this.lastEditField, this.lastEditPosition, true);
            }

            UPSERow row = ((UPMSEPositionWithContext)position).Row;
            List<Exception> validationErrors = new List<Exception>();
            if (this.SerialEntry.ChangedChildRecordsForRow(row)?.Count > 0)
            {
                foreach (UPMGroup group in position.DetailInputGroups)
                {
                    if (group.Identifier.IdentifierAsString == "position" || group.Identifier.IdentifierAsString == "children")
                    {
                        foreach (UPMSerialEntryEditField serialEntryEditField in group.Fields)
                        {
                            UPSEColumn column = serialEntryEditField.SerialEntryColumn;
                            if (column.Must && (column.ColumnFrom == UPSEColumnFrom.DestChild || column.ColumnFrom == UPSEColumnFrom.Dest)
                                && column.CrmField.IsEmptyValue(row.StringValueAtIndex(column.Index)))
                            {
                                ((UPMEditField)serialEntryEditField).HasError = true;
                                Exception error = null; //NSError.ErrorWithDomainCodeUserInfo("Must", 1, NSDictionary.DictionaryWithObjectForKey(UPLocalizedString(kUPLocalizationTextGroupErrors, UPLocalizationTextErrors_EditMandatoryFieldNotSet), NSLocalizedDescriptionKey));
                                validationErrors.Add(error);
                            }
                        }
                    }
                }
            }

            if (validationErrors.Count > 0)
            {
                return validationErrors;
            }

            if (position != null)
            {
                if (documentsIndex >= 0 && data != null && !string.IsNullOrEmpty(fileNamePattern))
                {
                    UPSerialEntryDocuments serialEntryDocuments = this.SerialEntry.SerialEntryRowDocuments[documentsIndex];
                    if (serialEntryDocuments.HasDocumentsColumnFunctionName != null && serialEntryDocuments.HasDocumentsColumnValue != null)
                    {
                        UPSEColumn column = this.SerialEntry.ColumnForFunctionName(serialEntryDocuments.HasDocumentsColumnFunctionName);
                        if (column != null)
                        {
                            this.SerialEntry.UpdateRow(row, serialEntryDocuments.HasDocumentsColumnValue, column.Index, false, null);
                        }
                    }
                }

                row.PhotoData = data;
                row.FileNamePattern = fileNamePattern;
                if (this.SerialEntry.ChangedChildRecordsForRow(row)?.Count == 0 && row.PhotoData == null)
                {
                    return null;
                }

                if (row.CanHaveDependentRows)
                {
                    this.InvalidateDependentRowsForPositionRowDependentRows((UPMSEPositionWithContext)position, null, null);
                }
            }

            this.ChangeRequestError = null;
            if (!this.SerialEntry.DisableSingleRowUpdate && (position != null || this.SerialEntry.HasChanges()))
            {
                //this.WillChangeValueForKey("hasRunningChangeRequests");
                ++this.runningRequests;
                if (!this.SerialEntry.SaveRowContext(row, position))
                {
                    --this.runningRequests;
                }

                //this.DidChangeValueForKey("hasRunningChangeRequests");
            }

            return null;
        }

        /// <summary>
        /// Saves all.
        /// </summary>
        /// <returns></returns>
        public bool SaveAll()
        {
            if (this.SerialEntry == null)
            {
                return false;
            }

            this.ChangeRequestError = null;
            if (this.lastEditField != null && this.lastEditPosition != null)
            {
                this.UserDidChangeFieldForPositionEditFinished(this.lastEditField, this.lastEditPosition, true);
            }

            //this.WillChangeValueForKey("hasRunningChangeRequests");
            ++this.runningRequests;
            this.SerialEntry.SaveAllChangesWithContext(null);
            //this.DidChangeValueForKey("hasRunningChangeRequests");
            return true;
        }

        /// <summary>
        /// Saves the with discarded changes.
        /// </summary>
        /// <returns></returns>
        public bool SaveWithDiscardedChanges()
        {
            if (this.SerialEntry == null)
            {
                return false;
            }

            this.ChangeRequestError = null;
            //this.WillChangeValueForKey("hasRunningChangeRequests");
            ++this.runningRequests;
            this.SerialEntry.SaveIgnoringAllChanges();
            //this.DidChangeValueForKey("hasRunningChangeRequests");
            return true;
        }

        /// <summary>
        /// Users the did change field for position edit finished.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="position">The position.</param>
        /// <param name="editFinished">if set to <c>true</c> [edit finished].</param>
        public void UserDidChangeFieldForPositionEditFinished(UPMField field, UPMSEPosition position, bool editFinished)
        {
            var serialEntryEditField = (UPMSerialEntryEditField)field;
            var row = ((UPMSEPositionWithContext)position).Row;
            if (row == null)
            {
                return;
            }

            var enableNextPos = false;
            string stringFieldValue = null;
            UPSelectorOption selectorOption = null;
            this.ExtractFieldValue(field, position, out stringFieldValue, out enableNextPos, out selectorOption);

            this.CheckValueForPositionRowField(serialEntryEditField.SerialEntryPosition, row, field);
            var affectedRows = new List<UPSERow>();
            if (selectorOption != null)
            {
                this.UpdateRowAndSetAffectedRows(row, selectorOption, affectedRows);
            }
            else
            {
                this.SerialEntry.UpdateRow(row, stringFieldValue, serialEntryEditField.SerialEntryColumn.Index, editFinished, affectedRows);
            }

            if (editFinished)
            {
                this.lastEditField = null;
                this.lastEditPosition = null;
            }
            else
            {
                this.lastEditField = field;
                this.lastEditPosition = position;
            }

            this.SetFieldValuesForPositionRow(serialEntryEditField.SerialEntryPosition, row);
            var changedIdentifiers = new List<IIdentifier>();
            changedIdentifiers.Add(serialEntryEditField.SerialEntryPosition.Identifier);
            foreach (var affectedRow in affectedRows)
            {
                var affectedRowPosition = this.PositionForRow(affectedRow);
                if (affectedRowPosition?.Identifier != null)
                {
                    affectedRowPosition.Invalid = true;
                    this.SetFieldValuesForPositionRow(affectedRowPosition, affectedRow);
                    changedIdentifiers.Add(affectedRowPosition.Identifier);
                }
            }

            this.UpdateSumLine();
            position.Invalid = true;
            var jumpToNextPos = false;
            var column = serialEntryEditField.SerialEntryColumn as UPSEDestinationColumnBase;
            if (enableNextPos && !string.IsNullOrWhiteSpace(column?.ChangeAction))
            {
                if (column.ChangeAction == "nextpos")
                {
                    jumpToNextPos = true;
                }
            }

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, changedIdentifiers, jumpToNextPos ? UPChangeHints.ChangeHintsWithHint(Constants.JumpToNextPosHint) : null);
        }


        public void UserDidChangeField(UPMSEPosition position, string stringFieldValue, int columnIndex, bool editFinished)
        {            
            var row = ((UPMSEPositionWithContext)position).Row;
            if (row == null)
            {
                return;
            }

            //var enableNextPos = false;
            //string stringFieldValue = null;
            //UPSelectorOption selectorOption = null;
            //this.ExtractFieldValue(field, position, out stringFieldValue, out enableNextPos, out selectorOption);

            //this.CheckValueForPositionRowField(serialEntryEditField.SerialEntryPosition, row, field);
            var affectedRows = new List<UPSERow>();
            //if (selectorOption != null)
            //{
            //    this.UpdateRowAndSetAffectedRows(row, selectorOption, affectedRows);
            //}
            //else
            //{
            //    this.SerialEntry.UpdateRow(row, stringFieldValue, serialEntryEditField.SerialEntryColumn.Index, editFinished, affectedRows);
            //}

            this.SerialEntry.UpdateRow(row, stringFieldValue, columnIndex, false, affectedRows);

            //if (editFinished)
            //{
            //    this.lastEditField = null;
            //    this.lastEditPosition = null;
            //}
            //else
            //{
            //    this.lastEditField = field;
            //    this.lastEditPosition = position;
            //}

            this.SetFieldValuesForPositionRow(position, row);
            var changedIdentifiers = new List<IIdentifier>();
            changedIdentifiers.Add(position.Identifier);
            foreach (var affectedRow in affectedRows)
            {
                var affectedRowPosition = this.PositionForRow(affectedRow);
                if (affectedRowPosition?.Identifier != null)
                {
                    affectedRowPosition.Invalid = true;
                    this.SetFieldValuesForPositionRow(affectedRowPosition, affectedRow);
                    changedIdentifiers.Add(affectedRowPosition.Identifier);
                }
            }

            this.UpdateSumLine();
            position.Invalid = true;
            var jumpToNextPos = false;
            //var column = serialEntryEditField.SerialEntryColumn as UPSEDestinationColumnBase;
            //if (enableNextPos && !string.IsNullOrWhiteSpace(column?.ChangeAction))
            //{
            //    if (column.ChangeAction == "nextpos")
            //    {
            //        jumpToNextPos = true;
            //    }
            //}

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, changedIdentifiers, jumpToNextPos ? UPChangeHints.ChangeHintsWithHint(Constants.JumpToNextPosHint) : null);
        }

        private static void EditFinishedUPMSerialEntryPercentEditField(UPMField field, UPMSEPosition position, ref string stringFieldValue)
        {
#if PORTING
            NSNumberFormatter formatter = NSNumberFormatter.TheNew();
            formatter.SetMaximumFractionDigits(4);
            formatter.SetDecimalSeparator(".");
            formatter.SetUsesGroupingSeparator(false);
            formatter.SetNumberStyle(NSNumberFormatterDecimalStyle);
            formatter.SetFormatterBehavior(NSNumberFormatterBehaviorDefault);
            stringFieldValue = formatter.StringFromNumber(field.FieldValue);
#endif
        }

        private static string EditFinishedUPMSerialEntryNumberEditField(UPMField field, UPMSEPosition position)
        {
            string stringFieldValue;
            var numberEditField = (UPMSerialEntryNumberEditField)field;
            if (numberEditField.StepSize > 1)
            {
                var stepSizeInfoMessage = position.GetOrCreateInfoMessageForKey(Constants.StepSizeInfoMessageKey);
                stepSizeInfoMessage.ErrorLevelMessage = false;
                stepSizeInfoMessage.MessageField.StringValue = string.Format(LocalizedString.TextSerialEntryPackageSize, numberEditField.StepSize);
                var intValue = Convert.ToInt32(numberEditField.NumberValue);
                if (intValue > 0 && intValue % numberEditField.StepSize != 0)
                {
                    stepSizeInfoMessage.ErrorLevelMessage = true;
                }
            }

            stringFieldValue = field.FieldValue == null ? "0" : field.FieldValue.ToString();
            return stringFieldValue;
        }

        private void UpdateRowAndSetAffectedRows(UPSERow row, UPSelectorOption selectorOption, List<UPSERow> affectedRows)
        {
            foreach (var functionName in selectorOption.FieldValues.Keys)
            {
                var col = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(functionName);
                if (col != null)
                {
                    var affectedRowsForValue = new List<UPSERow>();
                    this.SerialEntry.UpdateRow(row, selectorOption.FieldValues[functionName] as string, col.Index, false, affectedRowsForValue);
                    if (affectedRowsForValue.Count > 0)
                    {
                        if (affectedRows.Count == 0)
                        {
                            affectedRows = affectedRowsForValue;
                        }
                        else
                        {
                            foreach (var ar in affectedRowsForValue)
                            {
                                if (!affectedRows.Contains(ar))
                                {
                                    affectedRows.Add(ar);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExtractFieldValue(UPMField field, UPMSEPosition position, out string stringFieldValue, out bool enableNextPos, out UPSelectorOption selectorOption)
        {
            stringFieldValue = null;
            enableNextPos = false;
            selectorOption = null;

            if (field is UPMSerialEntrySelectorCatalogEditField)
            {
                var catalogField = (UPMSerialEntrySelectorCatalogEditField)field;
                stringFieldValue = catalogField.FieldValue as string;
                selectorOption = catalogField.OptionForName(stringFieldValue);
            }
            else if (field is UPMSerialEntryCatalogEditField)
            {
                var catalogField = (UPMSerialEntryCatalogEditField)field;
                stringFieldValue = catalogField.FieldValue as string;
                enableNextPos = true;
            }
            else if (field is UPMSerialEntryPercentEditField)
            {
                EditFinishedUPMSerialEntryPercentEditField(field, position, ref stringFieldValue);
            }
            else if (field is UPMSerialEntryBooleanEditField)
            {
                stringFieldValue = ((UPMSerialEntryBooleanEditField)field).BoolValue ? "true" : "false";
                enableNextPos = true;
            }
            else if (field is UPMSerialEntryNumberEditField)
            {
                stringFieldValue = EditFinishedUPMSerialEntryNumberEditField(field, position);
            }
            else if (field is UPMSerialEntryDateTimeEditField)
            {
                var dateField = (UPMSerialEntryDateTimeEditField)field;
                if (field.FieldValue == null)
                {
                    stringFieldValue = string.Empty;
                }
                else if (dateField.Type == DateTimeType.Date)
                {
                    stringFieldValue = dateField.DateValue.CrmValueFromDate();
                }
                else if (dateField.Type == DateTimeType.Time)
                {
                    stringFieldValue = dateField.DateValue.CrmValueFromTime();
                }
                else
                {
                    stringFieldValue = string.Empty;
                    this.Logger.LogWarn($"unsuported date type in serialEntry {field.LabelText}");
                }
            }
            else
            {
                stringFieldValue = field.FieldValue == null ? string.Empty : $"{field.FieldValue}";
            }
        }

        /// <summary>
        /// Duplicates the row clicked.
        /// </summary>
        /// <param name="_position">The position.</param>
        public void DuplicateRowClicked(UPMSEPosition _position)
        {
            UPMSEPositionWithContext positionWithContext = (UPMSEPositionWithContext)_position;
            UPSERow duplicatedRow = this.SerialEntry.DuplicateSourceRow(positionWithContext.Row);
            if (duplicatedRow != null)
            {
                if (this.duplicatedRows != null)
                {
                    this.duplicatedRows.Add(duplicatedRow);
                }
                else
                {
                    this.duplicatedRows = new List<UPSERow> { duplicatedRow };
                }

                StringIdentifier positionId = StringIdentifier.IdentifierWithStringId(duplicatedRow.RowKey);
                UPMSEPositionWithContext duplicatedPositionWithContext = new UPMSEPositionWithContext(positionId);
                duplicatedPositionWithContext.Row = duplicatedRow;
                _position.Invalid = true;
                for (int sectionIndex = 0; sectionIndex < this.Page.Children.Count; sectionIndex++)
                {
                    UPMResultSection resultSection = (UPMResultSection)this.Page.Children[sectionIndex];
                    for (int rowIndex = 0; rowIndex < resultSection.Children.Count; rowIndex++)
                    {
                        UPMSEPosition position = (UPMSEPosition)resultSection.Children[rowIndex];
                        IIdentifier identifier = position.Identifier;
                        if (identifier.MatchesIdentifier(_position.Identifier))
                        {
                            resultSection.InsertChildAtIndex(duplicatedPositionWithContext, resultSection.IndexOfChild(_position) + 1);
                        }
                    }
                }

                this.UpdatePosition(duplicatedPositionWithContext);
                duplicatedPositionWithContext.PositionSelected = true;

                if (this.ModelControllerDelegate != null)
                {
                    Dictionary<string, object> hintDictionary = new Dictionary<string, object>
                    {
                        { "SourceRow", _position.Identifier },
                        { "DuplicatedRow", positionId }
                    };
                    this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, new List<IIdentifier> { positionId }, UPChangeHints.ChangeHintsWithHintDetailHints(Constants.RowDuplicatedChangeHint, hintDictionary));
                }
            }
        }

        /// <summary>
        /// Adds the position.
        /// </summary>
        /// <param name="_position">The position.</param>
        public void AddPosition(UPMSEPosition _position)
        {
            bool insert = true;
            foreach (UPSERow orderRow in this.SerialEntry.Positions)
            {
                if (orderRow.RowKey == ((UPMSEPositionWithContext)_position).Row.RowKey)
                {
                    insert = false;
                    break;
                }
            }

            if (insert)
            {
                if (((UPMSEPositionWithContext)_position).Row.SerialEntryRecordId == null)
                {
                    ((UPMSEPositionWithContext)_position).Row.SerialEntryRecordId = "new";
                }

                this.SerialEntry.AddPosition(((UPMSEPositionWithContext)_position).Row);
            }
        }

        /// <summary>
        /// Informs the delegate about success.
        /// </summary>
        public void InformDelegateAboutSuccess()
        {
            this.Page.Status = null;
            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, UPChangeHints.ChangeHintsWithHint(Constants.FilterFinishSuccessChangeHint));
        }

        /// <summary>
        /// Serials the entry build did finish with success.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="result">The result.</param>
        public void SerialEntryBuildDidFinishWithSuccess(UPSerialEntry serialEntry, object result)
        {
            this.ContinueAfterSuccessfulSerialEntryBuild();
        }

        /// <summary>
        /// Serials the entry positions for filter did finish with success.
        /// </summary>
        /// <param name="serialEntryArg">The serial entry.</param>
        /// <param name="positions">The positions.</param>
        public void SerialEntryPositionsForFilterDidFinishWithSuccess(UPSerialEntry serialEntryArg, List<UPSERow> positions)
        {
            var saveAllPositions = false;
            this.contextArray = new List<UPMSEPositionWithContext>();
            if (this.addToDisplayedRows)
            {
                this.AddToDisplayeRows(positions);
            }
            else
            {
                this.displayedRows = positions;
            }

            this.duplicatedRows = null;
            if (serialEntryArg.AutoCreatePositions)
            {
                saveAllPositions = true;
                serialEntryArg.AutoCreatePositions = false;
            }

            this.UpdateSumLine();
            this.Page.RemoveAllChildren();
            var hasSections = this.Sections == "true";
            var lastSection = this.GetLastSection(ref saveAllPositions, hasSections);

            if (this.SerialEntry.SerialEntryParentInfoPanel != null)
            {
                this.FillParentInfoPanel();
            }

            var productCatalogDocuments = new List<UPMDocument>();
            if (this.SerialEntry.ProductCatalogDocuments != null && this.SerialEntry.ProductCatalogDocuments.Count > 0)
            {
                foreach (var data in this.SerialEntry.ProductCatalogDocuments)
                {
                    productCatalogDocuments.Add(new UPMDocument(data));
                }
            }

            this.SerialEntryPage.ProductCatalogs = productCatalogDocuments;
            if (lastSection != null)
            {
                this.Page.AddChild(lastSection);
            }

            this.SerialEntryPage.HideSectionIndex = !hasSections;
            this.SetNumberOfSerialEntryPageDisplayedRows();

            this.Page.Invalid = false;
            if (saveAllPositions)
            {
                if (this.SaveAllPositions())
                {
                    return;
                }
            }
            else if (this.ModelControllerDelegate != null)
            {
                this.InformDelegateAboutSuccess();
            }
        }

        /// <summary>
        /// Serials the entry did fail with error.
        /// </summary>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <param name="error">The error.</param>
        public void SerialEntryDidFailWithError(UPSerialEntry _serialEntry, Exception error)
        {
            this.SerialEntry = null;
            this.HandlePageErrorDetails(error.Message, error.StackTrace);
        }

        /// <summary>
        /// Serials the entry row changed with success context.
        /// </summary>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public void SerialEntryRowChangedWithSuccessContext(UPSerialEntry _serialEntry, UPSERow row, object context)
        {
            UPMSEPosition position = (UPMSEPosition)context;
            this.ChangeRequestError = null;
            if (!this.contextArray.Contains(position))
            {
                position = this.contextArray.FirstOrDefault(pwc => pwc.Row.RowKey == row.RowKey);
            }

            if (position != null)
            {
                position.Invalid = true;
                UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { position.Identifier, new RecordIdentifier(this.SerialEntry.Record.RecordIdentification) });
                if (this.ModelControllerDelegate != null && position.Identifier != null)
                {
                    //this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, new List<IIdentifier> { position.Identifier }, null);
                }
            }

            //this.WillChangeValueForKey("hasRunningChangeRequests");
            --this.runningRequests;
            //this.DidChangeValueForKey("hasRunningChangeRequests");
            //this.UpdateSumLine();
        }

        /// <summary>
        /// Serials the entry row photo uploaded context.
        /// </summary>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public void SerialEntryRowPhotoUploadedContext(UPSerialEntry _serialEntry, UPSERow row, object context)
        {
            UPMSEPosition position = (UPMSEPosition)context;
            foreach (UPMGroup group in position.DetailInputGroups)
            {
                if (group is UPMDocumentsGroup)
                {
                    this.UpdateGroup(group);
                }
            }

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, new List<IIdentifier> { position.Identifier }, null);
        }

        /// <summary>
        /// Serials the entry no changes in row context.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public void SerialEntryNoChangesInRowContext(UPSerialEntry serialEntry, UPSERow row, object context)
        {
            this.ChangeRequestError = null;
            //this.WillChangeValueForKey("hasRunningChangeRequests");
            --this.runningRequests;
            //this.DidChangeValueForKey("hasRunningChangeRequests");
        }

        /// <summary>
        /// Serials the entry row deleted with success.
        /// </summary>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="dependentRows">The dependent rows.</param>
        /// <param name="context">The context.</param>
        public void SerialEntryRowDeletedWithSuccess(UPSerialEntry _serialEntry, UPSERow row, List<UPSERow> dependentRows, object context)
        {
            UPMSEPosition position = (UPMSEPosition)context;
            this.UpdatePosition(position);
            this.InvalidateDependentRowsForPositionRowDependentRows(position, row, dependentRows);
            UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { position.Identifier, new RecordIdentifier(this.SerialEntry.Record.RecordIdentification) });
            if (this.SerialEntryPage.ShowShoppingCart || this.showErrorRows)
            {
                UPMSEPage oldPage = this.SerialEntryPage;
                UPMSEPage newPage = (UPMSEPage)this.UpdatedElement(this.Page);
                this.InformAboutDidChangeTopLevelElement(oldPage, newPage, new List<IIdentifier> { position.Identifier }, UPChangeHints.ChangeHintsWithHint(Constants.RowDeleteChangeHint));
            }
            else
            {
                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, new List<IIdentifier> { position.Identifier }, UPChangeHints.ChangeHintsWithHint(Constants.RowDeleteChangeHint));
            }

            this.ChangeRequestError = null;
            //this.WillChangeValueForKey("hasRunningChangeRequests");
            --this.runningRequests;
            //this.DidChangeValueForKey("hasRunningChangeRequests");
            this.UpdateSumLine();
        }

        /// <summary>
        /// Serials the entry row error context.
        /// </summary>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="error">The error.</param>
        /// <param name="context">The context.</param>
        public void SerialEntryRowErrorContext(UPSerialEntry _serialEntry, UPSERow row, Exception error, object context)
        {
            if (row != null)
            {
                UPMSEPosition position = (UPMSEPosition)context;
                position.PositionError = error;
                if (this.ModelControllerDelegate != null && position.Identifier != null)
                {
                    this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, new List<IIdentifier> { position.Identifier }, null);
                }
            }
            else
            {
                this.ChangeRequestError = error;
            }

            //this.WillChangeValueForKey("hasRunningChangeRequests");
            --this.runningRequests;
            //this.DidChangeValueForKey("hasRunningChangeRequests");
        }

        /// <summary>
        /// Serials the entry field values for query.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="fieldValues">The field values.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SerialEntryFieldValuesForQuery(UPSerialEntry serialEntry, List<object> fieldValues)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rights the checker grants permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void RightsCheckerGrantsPermission(UPRightsChecker sender, string recordIdentification)
        {
            this.rightsChecker = null;
            this.CreateSerialEntry();
        }

        /// <summary>
        /// Rights the checker revoke permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void RightsCheckerRevokePermission(UPRightsChecker sender, string recordIdentification)
        {
            this.rightsChecker = null;
            this.RightsFilterRevocation = true;
            this.HandlePageMessageDetails(LocalizedString.TextErrorActionNotAllowed, sender.ForbiddenMessage);
        }

        /// <summary>
        /// Rights the checker did finish with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void RightsCheckerDidFinishWithError(UPRightsChecker sender, Exception error)
        {
            this.RightsCheckerRevokePermission(sender, null);
        }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Caches the catalog filter data for filter filter list.
        /// </summary>
        /// <param name="currentFilter">The current filter.</param>
        /// <param name="filterList">The filter list.</param>
        public void CacheCatalogFilterDataForFilterFilterList(UPMFilter currentFilter, List<UPMFilter> filterList)
        {
            bool resetNextFilter = currentFilter == null;
            int i = 0;
            foreach (UPMFilter filter in filterList)
            {
                if (resetNextFilter)
                {
                    //DDLogInfo("reset Filter: %ld (DisplayedRows: %ld)", (long)i, (long)displayedRows.Count);
                    this.Logger.LogDebug(
                        $"reset Filter: {i} (DisplayedRows: {this.displayedRows.Count})",
                        LogFlag.LogSerialEntry);
                    this.CachedResultForFilterDictionary[i] = new List<UPSERow>(this.displayedRows);
                }

                if (currentFilter != null && filter.Equals(currentFilter))
                {
                    resetNextFilter = true;
                }

                i++;
            }
        }

        /// <summary>
        /// Updates the catalog filter data for filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public Dictionary<string, string> UpdateCatalogFilterDataForFilter(UPMFilter filter)
        {
            return this.UpdateCatalogFilterDataForFilter(filter, true, this.displayedRows);
        }

        /// <summary>
        /// Updates the catalog filter data for filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        /// <param name="rowsToFilterOriginal">The rows to filter original.</param>
        /// <returns></returns>
        public Dictionary<string, string> UpdateCatalogFilterDataForFilter(UPMFilter filter, bool useCache, List<UPSERow> rowsToFilterOriginal)
        {
            var rowsToFilter = rowsToFilterOriginal;
            if (this.hierarchicalPositionFilters && useCache)
            {
                var cachedRows = this.GetRowsFromCache(filter);
                if (cachedRows != null)
                {
                    rowsToFilter = cachedRows;
                }
            }

            var configFilter = ConfigurationUnitStore.DefaultStore.FilterByName(filter.Name);
            var parameter = configFilter?.Parameters().FirstParameter();
            if (configFilter == null || parameter == null)
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> defaultDictionary = null;
            var result = new Dictionary<string, string>();
            ReadCatalogFilter(filter, rowsToFilter, parameter, result, ref defaultDictionary);

            if (defaultDictionary != null && result.Count == 0)
            {
                return defaultDictionary;
            }

            return result;
        }

        /// <summary>
        /// Serials the entry signal activate initial filters.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="filters">The filters.</param>
        public void SerialEntrySignalActivateInitialFilters(UPSerialEntry serialEntry, List<UPConfigFilter> filters)
        {
            foreach (UPConfigFilter filter in filters)
            {
                if (filter is UPConfigSpecialFilter)
                {
                    UPConfigSpecialFilter specialFilter = (UPConfigSpecialFilter)filter;
                    if (specialFilter.UnitName == "Listing")
                    {
                        foreach (UPMFilter currentFilter in this.SerialEntryPage.AvailablePositionFilters)
                        {
                            if (currentFilter.Name == "Listing")
                            {
                                UPMCatalogFilter listingFilter = (UPMCatalogFilter)currentFilter;
                                listingFilter.Active = true;
                                listingFilter.SelectedCatalogCodes.Add(specialFilter.Parameter as string);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    int i = 0;
                    foreach (UPMFilter currentFilter in this.SerialEntryPage.AvailableFilters)
                    {
                        if (currentFilter.Name == filter.UnitName)
                        {
                            this.SerialEntryPage.SelectedFilter = currentFilter;
                            this.SerialEntryPage.Invalid = true;
                            this.FilterChangedAtIndex(i);
                            break;
                        }

                        i++;
                    }

                    if (this.SerialEntryPage.AvailablePositionFilters.Count > 0)
                    {
                        foreach (UPMFilter currentFilter in this.SerialEntryPage.AvailablePositionFilters)
                        {
                            if (currentFilter.Name == filter.UnitName)
                            {
                                currentFilter.Active = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Positions the filter changed.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void PositionFilterChanged(UPMFilter filter)
        {
            if (this.hierarchicalPositionFilters)
            {
                bool changeFilter = false;
                int i = 0;
                foreach (UPMFilter currentFilter in this.SerialEntryPage.AvailablePositionFilters)
                {
                    if (changeFilter)
                    {
                        currentFilter.Active = false;
                        this.CachedResultForFilterDictionary.Remove(i);
                    }

                    if (currentFilter.Equals(filter))
                    {
                        changeFilter = true;
                    }

                    i++;
                }
            }

            this.PositionsForFilter(this.SerialEntryPage.SelectedFilter);
        }

        /// <summary>
        /// Filters the index of the changed at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void FilterChangedAtIndex(int index)
        {
            if (this.hierarchicalPositionFilters)
            {
                this.CachedResultForFilterDictionary.Clear();
                this.lastTappedPositionFilter = null;
                foreach (UPMFilter filter in this.SerialEntryPage.AvailablePositionFilters)
                {
                    filter.Active = false;
                }
            }

            this.PositionsForFilter(this.SerialEntryPage.AvailableFilters[index]);
        }

        /// <summary>
        /// Alerts the index of the view clicked button at.
        /// </summary>
        /// <param name="alertView">The alert view.</param>
        /// <param name="buttonIndex">Index of the button.</param>
        public void AlertViewClickedButtonAtIndex(/*UIAlertView*/ object alertView, int buttonIndex)
        {
            if (alertView == this.autoCreateAlertBox)
            {
                this.autoCreateAlertBox = null;
                if (buttonIndex == 1)
                {
                    foreach (UPSERow row in this.displayedRows)
                    {
                        this.SerialEntry.ForceCreate(row);
                    }

                    this.SerialEntry.SaveRowsContext(this.displayedRows, this.contextArray);
                }
            }
        }

        /// <summary>
        /// Catalogs the position selected.
        /// </summary>
        /// <param name="_catalogPositionInfo">The catalog position information.</param>
        public void CatalogPositionSelected(Dictionary<string, string> _catalogPositionInfo)
        {
            string infoData = _catalogPositionInfo["Contents"];
            Dictionary<string, object> infoDictionary = infoData.JsonDictionaryFromString();
            string itemNumber = null;
            if (infoDictionary == null)
            {
                itemNumber = infoData;
            }
            else
            {
                itemNumber = infoDictionary.ValueOrDefault("ItemNumber") as string;
                if (string.IsNullOrEmpty(itemNumber))
                {
                    return;
                }
            }

            foreach (UPSERow row in this.displayedRows)
            {
                if (row.ItemNumber == itemNumber)
                {
                    row.EnsureLoaded();
                    UPMSEPosition position = this.PositionForRow(row);
                    string stepMode = infoDictionary["StepMode"] as string;
                    if (stepMode == "delete")
                    {
                        this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, new List<IIdentifier> { position.Identifier }, UPChangeHints.ChangeHintsWithHint("ProductCatalogPostionTapped"));
                        this.RemoveOrderPositionFromShoppingCart(position);
                    }
                    else
                    {
                        this.ChangeQuantityByStepModeInfoDictionary(position, infoDictionary);
                        List<IIdentifier> changedIdentifiers = position.Identifier != null ? new List<IIdentifier> { position.Identifier } : null;
                        this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, changedIdentifiers, UPChangeHints.ChangeHintsWithHint("ProductCatalogPostionTapped"));
                    }

                    return;
                }
            }
        }

        private string SectionKeyForRow(UPSERow dataRow)
        {
            string sectionIndex = dataRow.ValueAtIndex(0) as string;
            return this.CheckSectionIndex(sectionIndex);
        }

        private string CheckSectionIndex(string sectionIndex)
        {
            if (string.IsNullOrEmpty(sectionIndex))
            {
                return "?";
            }

            //NSRange range = sectionIndex.RangeOfCharacterFromSet(NSCharacterSet.AlphanumericCharacterSet());
            //if (range.Length > 0 && range.Location == 0)
            //{
            //    string vRet = sectionIndex.DecomposedStringWithCanonicalMapping().SubstringToIndex(1).UppercaseString();
            //    if (vRet[0] > 255)
            //    {
            //        return "?";
            //    }
            //    else
            //    {
            //        return vRet;
            //    }
            //}

            return "?";
        }

        private static void ReadCatalogFilter(UPMFilter filter, List<UPSERow> rowsToFilter, UPConfigFilterParameter parameter, Dictionary<string, string> result, ref Dictionary<string, string> defaultDictionary)
        {
            var firstRow = true;
            UPCRMResult firstResult = null;
            UPContainerFieldMetaInfo firstField = null;

            foreach (var row in rowsToFilter)
            {
                UPContainerFieldMetaInfo field;
                if (firstRow)
                {
                    firstResult = row.SourceResult;
                    firstField = row.SourceResult.FieldForFieldIdInfoAreaIdLinkId(parameter.CrmFieldInfo.FieldId, parameter.CrmFieldInfo.InfoAreaId, parameter.Table.LinkId);
                    field = firstField;
                    if (field != null)
                    {
                        firstRow = false;
                    }
                }
                else if (row.SourceResult == firstResult)
                {
                    field = firstField;
                }
                else
                {
                    field = row.SourceResult.FieldForFieldIdInfoAreaIdLinkId(parameter.CrmFieldInfo.FieldId, parameter.CrmFieldInfo.InfoAreaId, parameter.Table.LinkId);
                }

                var rawData = row.SourceResultRow.RawValueForField(field);
                if (filter.FilterType == UPMFilterType.Catalog || filter.FilterType == UPMFilterType.DependentCatalog)
                {
                    var catalogFilter = (UPMCatalogFilter)filter;
                    if (field == null)
                    {
                        if (defaultDictionary == null)
                        {
                            defaultDictionary = ((UPMCatalogFilter)filter).CatalogValueDictionary;
                        }

                        continue;
                    }
                    else if (field.CrmField.IsEmptyValue(rawData))
                    {
                        var emptyLabel = catalogFilter.CatalogValueDictionary.ValueOrDefault(catalogFilter.NullValueKey);
                        result.SetObjectForKey(!string.IsNullOrEmpty(emptyLabel) ? emptyLabel : LocalizedString.TextEmptyCatalog, rawData);
                    }
                    else
                    {
                        var catalog = UPCRMDataStore.DefaultStore.CatalogForCrmField(field.CrmField);
                        string title = catalog.TextValueForCode(Convert.ToInt32(rawData));
                        result.SetObjectForKey(!string.IsNullOrEmpty(title) ? title : $"?{rawData}", rawData);
                    }
                }
                else if (filter.FilterType == UPMFilterType.EditField && field != null && !string.IsNullOrEmpty(rawData))
                {
                    UpdateCatalogFilterForEdit(filter, result, rawData);
                }
            }
        }

        private static void UpdateCatalogFilterForEdit(UPMFilter filter, Dictionary<string, string> result, string rawData)
        {
            var editFieldFilter = (UPMEditFieldFilter)filter;

            if (editFieldFilter.Editfield is UPMBooleanEditField)
            {
                if (rawData.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    result.SetObjectForKey(LocalizedString.TextYes, "true");
                }
                else if (rawData.Equals(bool.FalseString, StringComparison.OrdinalIgnoreCase))
                {
                    result.SetObjectForKey(LocalizedString.TextNo, "false");
                }
            }
            else if (editFieldFilter.Editfield is UPMStringEditField)
            {
                result.SetObjectForKey(rawData, rawData);
            }
            else if (editFieldFilter.Editfield is UPMNumberEditField)
            {
                result.SetObjectForKey(rawData, rawData);
            }
        }

        private UPMSEPosition PositionForRow(UPSERow row)
        {
            List<UPMElement> visiblePositions = this.SerialEntryPage.Children;
            foreach (UPMElement child in visiblePositions)
            {
                if (child is UPMResultSection)
                {
                    UPMResultSection resultSection = (UPMResultSection)child;
                    foreach (UPMElement sectionChild in resultSection.Children)
                    {
                        if (sectionChild is UPMSEPositionWithContext)
                        {
                            UPMSEPositionWithContext position = (UPMSEPositionWithContext)sectionChild;
                            if (position.Row == row)
                            {
                                return position;
                            }
                        }
                    }
                }
                else if (child is UPMSEPositionWithContext)
                {
                    UPMSEPositionWithContext position = (UPMSEPositionWithContext)child;
                    if (position.Row == row)
                    {
                        return position;
                    }
                }
            }

            return null;
        }

        private void FillParentInfoPanel()
        {
            UPMSEPage serialEntryPage = this.SerialEntryPage;
            serialEntryPage.ParentInfoPanel = new UPMSEParentInfoPanel(StringIdentifier.IdentifierWithStringId("ParentInfoPanel"));
            if (serialEntryPage.ParentInfoPanel != null)
            {
                UPMSEParentInfoPanel parentInfoPanel = serialEntryPage.ParentInfoPanel;
                for (int index = 0; index < this.SerialEntry.SerialEntryParentInfoPanel.Cells.Count; index++)
                {
                    UPSerialEntryParentInfoPanelCell cell = this.SerialEntry.SerialEntryParentInfoPanel.Cells[index];
                    UPMSEParentInfoPanelGroup group = new UPMSEParentInfoPanelGroup(StringIdentifier.IdentifierWithStringId($"PanelGroup{index}"));
                    //group.Icon = UIImage.UpImageWithFileName(cell.ImageName);     // CRM-5007
                    for (int fieldIndex = 0; fieldIndex < cell.FieldValues.Count; fieldIndex++)
                    {
                        UPMStringField stringField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"Field{index}-{fieldIndex}"));
                        stringField.StringValue = cell.FieldValues[fieldIndex];
                        group.AddChild(stringField);
                    }

                    parentInfoPanel.AddChild(group);
                }
            }
        }

        private UPMSerialEntryNumberEditField FindFirstQuantityEditField(UPMSEPosition position)
        {
            UPMSerialEntryNumberEditField quantityEditField = null;
            foreach (UPMGroup group in position.DetailInputGroups)
            {
                if (quantityEditField != null)
                {
                    break;
                }

                if (group.Identifier.IdentifierAsString == "position" || group.Identifier.IdentifierAsString == "children")
                {
                    foreach (UPMSerialEntryEditField serialEntryEditField in group.Fields)
                    {
                        if (serialEntryEditField.SerialEntryColumn.Function == "Quantity")
                        {
                            quantityEditField = (UPMSerialEntryNumberEditField)serialEntryEditField;
                            break;
                        }
                    }
                }
            }

            return quantityEditField;
        }

        private void ChangeQuantityByStepModeInfoDictionary(UPMSEPosition position, Dictionary<string, object> linkInfoDictionary)
        {
            UPMSerialEntryNumberEditField quantityEditField = this.FindFirstQuantityEditField(position);
            if (quantityEditField != null)
            {
                double numberValue = -1;
                UPMSerialEntryNumberEditField serialEntryNumberEditField = quantityEditField;

                string stepMode = linkInfoDictionary.ValueOrDefault("StepMode") as string;
                if (stepMode == "nop" || stepMode == "delete")
                {
                    return;
                }

                if (linkInfoDictionary == null || stepMode == null)
                {
                    numberValue = serialEntryNumberEditField.NumberValue + serialEntryNumberEditField.StepSize;
                }
                else
                {
                    int stepSize = linkInfoDictionary.ContainsKey("StepSize") ? Convert.ToInt32(linkInfoDictionary["StepSize"]) : -1;
                    if (stepMode == "inc")
                    {
                        if (stepSize == -1)
                        {
                            numberValue = serialEntryNumberEditField.NumberValue + serialEntryNumberEditField.StepSize;
                        }
                        else
                        {
                            numberValue = serialEntryNumberEditField.NumberValue + stepSize;
                        }
                    }
                    else if (stepMode == "set")
                    {
                        if (stepSize != -1)
                        {
                            numberValue = stepSize;
                        }
                    }
                }

                if (numberValue != -1)
                {
                    if (serialEntryNumberEditField.NumberValue != numberValue)
                    {
                        serialEntryNumberEditField.NumberValue = numberValue;
                        this.UserDidChangeFieldForPositionEditFinished(serialEntryNumberEditField, position, false);
                    }
                }
            }
        }

        private void SetFinishAction(UPMSEPage page, IConfigurationUnitStore configStore)
        {
            var finishActionButton = this.ViewReference.ContextValueForKey(KeyFinishAction);
            if (!string.IsNullOrWhiteSpace(finishActionButton))
            {
                Menu menu = null;
                UPConfigButton button = null;
                if (finishActionButton.StartsWith("Button:"))
                {
                    button = configStore.ButtonByName(finishActionButton.Substring(7));
                }
                else if (finishActionButton.StartsWith("Menu:"))
                {
                    menu = configStore.MenuByName(finishActionButton.Substring(5));
                }
                else if (finishActionButton != "Return")
                {
                    menu = configStore.MenuByName(finishActionButton);
                }

                UPMAction action = new UPMAction(StringIdentifier.IdentifierWithStringId("finishAction"));
                action.SetTargetAction(this, this.PerformFinishAction);
                if (button != null)
                {
                    action.LabelText = button.Label;
                    action.IconName = button.ImageName;
                }
                else if (menu != null)
                {
                    action.LabelText = menu.DisplayName;
                    action.IconName = menu.ImageName;
                }
                else
                {
                    action.LabelText = LocalizedString.TextClose;
                    action.IconName = "Icon:Ok";
                }

                page.FinishAction = action;
                this.finishActionName = finishActionButton;
            }
        }

        private void SetCloseButtonText(Dictionary<string, object> additionalOptions)
        {
            var closeButtonText = additionalOptions.ValueOrDefault("CloseButtonTitle") as string;
            if (!string.IsNullOrWhiteSpace(closeButtonText))
            {
                this.SerialEntryPage.CloseButtonText = closeButtonText.ReferencedStringWithDefault(LocalizedString.TextClose);
            }
            else
            {
                this.SerialEntryPage.CloseButtonText = LocalizedString.TextClose;
            }
        }

        private void SetOrganizerContextItems(string configValue)
        {
            var valueDictionary = new Dictionary<string, string>();
            var parts = configValue.Split(',');
            foreach (string part in parts)
            {
                var keyValue = part.Split(':');
                if (keyValue.Length >= 2)
                {
                    valueDictionary[keyValue[1]] = keyValue[0];
                }
                else
                {
                    valueDictionary.SetObjectForKey(part, part);
                }
            }

            this.OrganizerContextItems = valueDictionary;
        }

        private void SetCancelAction(Dictionary<string, object> additionalOptions)
        {
            var cancelAction = new UPMAction(StringIdentifier.IdentifierWithStringId("cancelAction"));
            cancelAction.SetTargetAction(this, this.PerformCancelAction);
            var cancelButtonText = additionalOptions.ValueOrDefault("CancelButtonTitle") as string;
            cancelButtonText = !string.IsNullOrEmpty(cancelButtonText)
                ? cancelButtonText.ReferencedStringWithDefault(LocalizedString.TextCancel)
                : LocalizedString.TextCancel;

            cancelAction.LabelText = cancelButtonText;
            this.SerialEntryPage.CancelAction = cancelAction;
        }

        private void SetCloseAction(Dictionary<string, object> additionalOptions)
        {
            var closeAction = new UPMAction(StringIdentifier.IdentifierWithStringId("closeAction"));
            var initialCloseButtonText = additionalOptions.ValueOrDefault("InitialCloseButtonTitle") as string;
            if (!string.IsNullOrWhiteSpace(initialCloseButtonText))
            {
                initialCloseButtonText = initialCloseButtonText.ReferencedStringWithDefault(LocalizedString.TextClose);
            }
            else
            {
                initialCloseButtonText = this.SerialEntryPage.CloseButtonText;
            }

            closeAction.LabelText = initialCloseButtonText;
            closeAction.SetTargetAction(this, this.PerformCloseAction);
            this.SerialEntryPage.CloseAction = closeAction;
        }

        private bool SaveAllPositions()
        {
            if (this.ModelControllerDelegate != null)
            {
                this.InformDelegateAboutSuccess();
            }

            object autoCreateDoNotAsk;
            if (this.SerialEntry.AdditionalOptions.TryGetValue("autoCreateDoNotAsk", out autoCreateDoNotAsk))
            {
                if (Convert.ToInt32(autoCreateDoNotAsk) > 0)
                {
                    foreach (var row in this.displayedRows)
                    {
                        this.SerialEntry.ForceCreate(row);
                    }

                    this.SerialEntry.SaveRowsContext(this.displayedRows, this.contextArray);
                    return true;
                }
            }

            return false;
        }

        private void SetNumberOfSerialEntryPageDisplayedRows()
        {
            var hasThirdRow = this.SerialEntry.HasSourceValueForColumnSpanIndex(2) || this.SerialEntry.HasSourceValueForColumnSpanIndex(3) || this.SerialEntry.HasSourceValueForColumnSpanIndex(4);
            var disableThirdRow = ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("SerialEntry.DisableThirdListRow", false);
            if (this.SerialEntry.AdditionalOptions != null)
            {
                disableThirdRow = this.SerialEntry.AdditionalOptions.ContainsKey("DisableThirdListRow") ? Convert.ToInt32(this.SerialEntry?.AdditionalOptions["DisableThirdListRow"]) == 1 : disableThirdRow;
            }
            this.SerialEntryPage.NumberOfDisplayedRows = hasThirdRow && !disableThirdRow ? 3 : 2;
        }

        private UPMResultSection GetLastSection(ref bool saveAllPositions, bool hasSections)
        {
            UPMResultSection resultSection = null;
            UPMResultSection lastSection = null;
            var sectionDictionary = new Dictionary<string, UPMResultSection>();
            foreach (var row in this.displayedRows)
            {
                if (saveAllPositions && !string.IsNullOrWhiteSpace(row.SerialEntryRecordIdentification))
                {
                    saveAllPositions = false;
                }

                if (hasSections)
                {
                    var currentSectionKey = this.SectionKeyForRow(row);
                    var isLastSection = currentSectionKey == "?";
                    resultSection = isLastSection ? lastSection : sectionDictionary.ValueOrDefault(currentSectionKey);

                    if (resultSection == null)
                    {
                        resultSection = new UPMResultSection(StringIdentifier.IdentifierWithStringId($"Result_Section_{currentSectionKey}"))
                        {
                            SectionField = new UPMField(StringIdentifier.IdentifierWithStringId("SectionLabel")),
                            SectionIndexKey = currentSectionKey
                        };
                        resultSection.SectionField.FieldValue = resultSection.SectionIndexKey;
                        if (isLastSection)
                        {
                            lastSection = resultSection;
                        }
                        else
                        {
                            this.Page.AddChild(resultSection);
                            sectionDictionary.SetObjectForKey(resultSection, currentSectionKey);
                        }
                    }
                }
                else
                {
                    if (resultSection == null)
                    {
                        resultSection = new UPMResultSection(StringIdentifier.IdentifierWithStringId("Result_Section_0"));
                        this.Page.AddChild(resultSection);
                    }
                }

                var positionId = StringIdentifier.IdentifierWithStringId(row.RowKey);
                var position = new UPMSEPositionWithContext(positionId)
                {
                    Row = row,
                    Invalid = true
                };
                resultSection.AddChild(position);
                this.contextArray.Add(position);
            }

            return lastSection;
        }

        private void AddToDisplayeRows(List<UPSERow> positions)
        {
            this.addToDisplayedRows = false;
            var dict = new Dictionary<string, UPSERow>(this.displayedRows.Count);

            foreach (var row in this.displayedRows)
            {
                dict.SetObjectForKey(row, row.RowRecordId);
            }

            var displayedRowsNew = new List<UPSERow>(this.displayedRows);
            displayedRowsNew.AddRange(positions.Where(row => !dict.ContainsKey(row.RowRecordId)));

            this.displayedRows = displayedRowsNew;
        }

        private List<UPSERow> GetRowsFromCache(UPMFilter filter)
        {
            this.CacheCatalogFilterDataForFilterFilterList(this.lastTappedPositionFilter, this.SerialEntryPage.AvailablePositionFilters);

            this.lastTappedPositionFilter = filter;
            var cachedDataAvailable = false;
            var i = 0;
            foreach (var currentFilter in this.SerialEntryPage.AvailablePositionFilters)
            {
                if (filter == currentFilter)
                {
                    cachedDataAvailable = true;
                    break;
                }

                i++;
            }

            if (cachedDataAvailable)
            {
                var cachedResultData = this.CachedResultForFilterDictionary.ValueOrDefault(i);
                if (cachedResultData != null)
                {
                    return cachedResultData;
                }
            }

            return null;
        }
    }
}
