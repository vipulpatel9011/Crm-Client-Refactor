// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMFilter.cs" company="Aurea Software Gmbh">
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
//   The Filter implimentation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Filters
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts.Reps;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Filter Types
    /// </summary>
    public enum UPMFilterType
    {
        /// <summary>
        /// Catalog
        /// </summary>
        Catalog,

        /// <summary>
        /// Dependent catalog
        /// </summary>
        DependentCatalog,

        /// <summary>
        /// Date
        /// </summary>
        Date,

        /// <summary>
        /// No parameter
        /// </summary>
        NoParam,

        /// <summary>
        /// Distance
        /// </summary>
        Distance,

        /// <summary>
        /// Edit field
        /// </summary>
        EditField,

        /// <summary>
        /// Reps
        /// </summary>
        Reps
    }

    /// <summary>
    /// Filter class
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMFilter : UPMElement
    {
        private const string DateFormat = "MM.dd.yyyy";

        private const string CatalogFilterK = "K";
        private const string CatalogFilterX = "X";
        private const string DateFilter = "D";
        private const string EditFieldF = "F";
        private const string EditFieldS = "S";
        private const string EditFieldL = "L";
        private const string EditFieldC = "C";
        private const string EditFieldB = "B";

        private bool active;
        private string displayValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMFilter"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="filterType">Type of the filter.</param>
        public UPMFilter(IIdentifier identifier, UPMFilterType filterType)
            : base(identifier)
        {
            this.displayValue = string.Empty;
            this.FilterType = filterType;
            this.Invalid = true;
            this.Visible = true;
            this.DefaultEnabled = false;
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMFilter"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public bool Active
        {
            get
            {
                return this.active;
            }

            set
            {
                this.Set(nameof(this.Active), ref this.active, value);
            }
        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public UPMFilterType FilterType { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMFilter"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [default enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [default enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool DefaultEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the disabled.
        /// </summary>
        /// <value>
        /// The name of the disabled.
        /// </value>
        public string DisabledName { get; set; }

        /// <summary>
        /// Gets the raw values.
        /// </summary>
        /// <value>
        /// The raw values.
        /// </value>
        public virtual List<string> RawValues => null;

        /// <summary>
        /// Gets the display title.
        /// </summary>
        /// <value>
        /// The display title.
        /// </value>
        public string DisplayTitle => !string.IsNullOrEmpty(this.DisplayName) ? this.DisplayName : this.Name;

        /// <summary>
        /// Gets the display value
        /// </summary>
        public string DisplayValue
        {
            get
            {
                return this.displayValue;
            }

            private set
            {
                this.Set(nameof(this.DisplayValue), ref this.displayValue, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether has parameters
        /// </summary>
        public virtual bool HasParameters => false;

        /// <summary>
        /// Filters for name.
        /// </summary>
        /// <param name="filtername">The filtername.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <returns>Filter</returns>
        public static UPMFilter FilterForName(string filtername, Dictionary<string, object> filterParameters = null)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var configFilter = configStore.FilterByName(filtername);
            UPMFilter filter = null;
            if (configFilter != null)
            {
                var selectCondition = configFilter.ConditionWith("Parameter:Select", true);
                if (selectCondition != null)
                {
                    return FilterForConfigFilter(configFilter, selectCondition.ValueAtIndex(0), filterParameters, false);
                }

                selectCondition = configFilter.ConditionWith("Parameter:SingleSelect");
                if (selectCondition != null)
                {
                    return FilterForConfigFilter(configFilter, selectCondition.ValueAtIndex(0), filterParameters, true);
                }

                var replacements = UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(filterParameters, true);
                configFilter = configFilter.FilterByApplyingReplacements(replacements);
                var parameters = configFilter?.Parameters();
                if (parameters?.NumberOfParameters > 0)
                {
                    filter = GetFilter(filtername, parameters);
                }
                else
                {
                    var identifier = FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(configFilter.InfoAreaId, filtername, string.Empty);
                    filter = new UPMNoParamFilter(identifier);
                }

                if (filter != null)
                {
                    filter.Name = filtername;
                    filter.DisplayName = !string.IsNullOrEmpty(configFilter?.DisplayName) ? configFilter.DisplayName : filtername;
                }
            }

            return filter;
        }

        /// <summary>
        /// Filters for configuration filter.
        /// </summary>
        /// <param name="configFilter">The configuration filter.</param>
        /// <param name="searchAndListName">Name of the search and list.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <param name="singleSelect">if set to <c>true</c> [single select].</param>
        /// /// <returns>Filter</returns>
        public static UPMFilter FilterForConfigFilter(UPConfigFilter configFilter, string searchAndListName, Dictionary<string, object> filterParameters, bool singleSelect)
        {
            if (string.IsNullOrEmpty(searchAndListName))
            {
                return null;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(searchAndListName);
            if (searchAndList != null)
            {
                return null;
            }

            FieldControl listControl = configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
            if (listControl == null)
            {
                return null;
            }

            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(searchAndListName, filterParameters);

            UPCRMResult result = crmQuery.Find();
            int count = result.RowCount;
            if (count == 0)
            {
                return null;
            }

            UPCRMListFormatter listFormatter = new UPCRMListFormatter(listControl.TabAtIndex(0));
            StringIdentifier identifier = StringIdentifier.IdentifierWithStringId("SelectFilter");
            UPMSelectCatalogFilter filter = new UPMSelectCatalogFilter(identifier);
            Dictionary<string, string> dict = new Dictionary<string, string>(count);
            Dictionary<string, UPCRMResultRow> rowDict = new Dictionary<string, UPCRMResultRow>(count);

            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                string fieldValue = listFormatter.StringFromRowForPosition(row, 0);
                dict[row.RootRecordIdentification] = fieldValue;
                rowDict[row.RootRecordIdentification] = row;
            }

            filter.CrmResult = result;
            filter.FilterParameters = filterParameters;
            filter.ResultRowDictionary = rowDict;
            filter.ExplicitCatalogValues = dict;
            filter.ParameterName = "Select";
            filter.Name = configFilter.UnitName;

            filter.DisplayName = !string.IsNullOrEmpty(configFilter.DisplayName) ? configFilter.DisplayName : configFilter.UnitName;

            filter.SingleSelect = singleSelect;
            return filter;
        }

        /// <summary>
        /// Configurations the filter from filter and configuration filter.
        /// </summary>
        /// <param name="upmFilter">The upm filter.</param>
        /// <param name="configFilter">The configuration filter.</param>
        /// <returns>Config</returns>
        public static UPConfigFilter ConfigFilterFromFilterAndConfigFilter(UPMFilter upmFilter, UPConfigFilter configFilter)
        {
            if (upmFilter == null)
            {
                throw new ArgumentNullException(nameof(upmFilter));
            }

            if (configFilter == null)
            {
                throw new ArgumentNullException(nameof(configFilter));
            }

            var parameters = configFilter.Parameters();

            if (upmFilter.GetType() == typeof(UPMSelectCatalogFilter))
            {
                return QueryForCatalogFilter(configFilter, parameters, upmFilter);
            }

            switch (upmFilter.FilterType)
            {
                case UPMFilterType.Distance:
                    return QueryForDistance(configFilter, parameters, upmFilter);
                case UPMFilterType.Date:
                    return QueryForDate(configFilter, parameters, upmFilter);
                case UPMFilterType.Catalog:
                case UPMFilterType.DependentCatalog:
                    return QueryForCatalog(configFilter, parameters, upmFilter);
                default:
                    return QueryForDefault(configFilter, parameters, upmFilter);
            }
        }

        /// <summary>
        /// Actives the filters for filters.
        /// </summary>
        /// <param name="upmFilterArray">The upm filter array.</param>
        /// <returns>Filter list</returns>
        public static List<UPConfigFilter> ActiveFiltersForFilters(List<UPMFilter> upmFilterArray)
        {
            var configFilters = new List<UPConfigFilter>();
            if (upmFilterArray != null)
            {
                foreach (UPMFilter upmFilter in upmFilterArray)
                {
                    if (upmFilter.Active)
                    {
                        UPConfigFilter configFilter = upmFilter.ConfigFilter();
                        if (configFilter != null)
                        {
                            configFilters.Add(configFilter);
                        }
                    }
                }
            }

            return configFilters;
        }

        /// <summary>
        /// Clear the values
        /// </summary>
        public virtual void ClearValue()
        {
            this.UpdateSummary();
        }

        /// <summary>
        /// Updates the summary
        /// </summary>
        public void UpdateSummary()
        {
            var result = string.Empty;
            switch (this.FilterType)
            {
                case UPMFilterType.Catalog:
                case UPMFilterType.DependentCatalog:
                    var cf = (UPMCatalogFilter)this;
                    result = cf.SelectedCatalogsDisplayValue();
                    break;

                case UPMFilterType.Date:
                    var df = (UPMDateFilter)this;
                    if (!string.IsNullOrWhiteSpace(df.SecondParameterName))
                    {
                        result = $"{df.Value.ToString(DateFormat)} - {df.SecondValue.ToString(DateFormat)}";
                    }
                    else
                    {
                        result = $"{df.Value.ToString(DateFormat)}";
                    }

                    break;

                case UPMFilterType.EditField:
                    var ef = (UPMEditFieldFilter)this;
                    result = $"{ef.Editfield?.StringValue}";
                    if (ef.SecondEditfield != null)
                    {
                        result += $", {ef.SecondEditfield?.StringValue}";
                    }

                    break;

                case UPMFilterType.Reps:
                    var rf = (UPMRepFilter)this;
                    result = rf.SelectedRepsDisplayValue();
                    break;

                case UPMFilterType.NoParam:
                    break;

                case UPMFilterType.Distance:
                    break;

                default:
                    break;
            }

            this.DisplayValue = result;
        }

        /// <summary>
        /// Sets the default raw values.
        /// </summary>
        /// <param name="defaultRawValues">The default raw values.</param>
        public virtual void SetDefaultRawValues(List<object> defaultRawValues)
        {
            // Overide in derived calsses if needed.
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Name == (obj as UPMFilter)?.Name;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{base.ToString()} active: {this.active}";
        }

        /// <summary>
        /// Configurations the filter.
        /// </summary>
        /// <returns>Config</returns>
        public UPConfigFilter ConfigFilter()
        {
            UPConfigFilter configFilter = ConfigurationUnitStore.DefaultStore.FilterByName(this.Name);
            if (configFilter == null)
            {
                if (this.Name == "Listing" && this.RawValues?.Count > 0)
                {
                    return new UPConfigSpecialFilter("Listing", this.RawValues[0]);
                }
            }
            else
            {
                configFilter = configFilter.FilterByRemovingFixedConditionsForUnnamedParameters();
                return ConfigFilterFromFilterAndConfigFilter(this, configFilter);
            }

            return null;
        }

        private static UPConfigFilter QueryForCatalogFilter(UPConfigFilter configFilter, UPConfigFilterParameters parameters, UPMFilter upmFilter)
        {
            var selectCatalogFilter = (UPMSelectCatalogFilter)upmFilter;
            var rawValues = upmFilter.RawValues;
            var selectedValueDictionaries = new List<Dictionary<string, object>>(rawValues.Count);
            foreach (var recordIdentification in rawValues)
            {
                var row = selectCatalogFilter.ResultRowDictionary[recordIdentification];
                var rowFunctionNames = row.ValuesWithFunctions();
                if (rowFunctionNames != null)
                {
                    selectedValueDictionaries.Add(rowFunctionNames);
                }
            }

            return configFilter
                .FilterByApplyingArrayOfValueDictionariesParameters(
                    selectedValueDictionaries,
                    selectCatalogFilter.FilterParameters);
        }

        private static UPConfigFilter QueryForDistance(UPConfigFilter configFilter, UPConfigFilterParameters parameters, UPMFilter upmFilter)
        {
            var distanceFilter = (UPMDistanceFilter)upmFilter;
            parameters.SetValuesName(new List<string> { distanceFilter.GetGPSXMin.ToString() }, "$parGPSXmin");
            parameters.SetValuesName(new List<string> { distanceFilter.GetGPSXMax.ToString() }, "$parGPSXmax");
            parameters.SetValuesName(new List<string> { distanceFilter.GetGPSYMin.ToString() }, "$parGPSYmin");
            parameters.SetValuesName(new List<string> { distanceFilter.GetGPSYMax.ToString() }, "$parGPSYmax");
            return (UPConfigFilter)configFilter.QueryByApplyingFilterParameters(parameters);
        }

        private static UPConfigFilter QueryForDate(UPConfigFilter configFilter, UPConfigFilterParameters parameters, UPMFilter upmFilter)
        {
            if (!(parameters?.NumberOfParameters > 0))
            {
                return configFilter;
            }

            if (parameters.NumberOfParameters == parameters.NumberOfUnnamedParameters)
            {
                var unnmaedParameters = parameters.UnnamedParameters;
                var rawValues = upmFilter.RawValues;
                var valueCount = rawValues.Count;
                for (var i = 0; i < parameters.NumberOfUnnamedParameters; i++)
                {
                    var param = unnmaedParameters[i];
                    if (valueCount > i)
                    {
                        param.Value = rawValues[i];
                    }
                }

                return (UPConfigFilter)configFilter.QueryByApplyingFilterParameters(parameters);
            }

            var firstParameter = parameters.FirstParameter();
            firstParameter.Values = upmFilter.RawValues;
            return (UPConfigFilter)configFilter.QueryByApplyingFilterParameter(firstParameter);
        }

        private static UPConfigFilter QueryForCatalog(UPConfigFilter configFilter, UPConfigFilterParameters parameters, UPMFilter upmFilter)
        {
            if (!(parameters?.NumberOfParameters > 0))
            {
                return configFilter;
            }

            var firstParameter = parameters.FirstParameter();
            var rawValues = upmFilter.RawValues;
            firstParameter.Values = rawValues;
            if (firstParameter.ValueIndex == 1 && upmFilter.FilterType == UPMFilterType.Catalog)
            {
                firstParameter.RemoveFirstValuesFromValueSet = true;
                return (UPConfigFilter)configFilter.QueryByApplyingFilterParameter(firstParameter);
            }

            if (upmFilter.FilterType == UPMFilterType.DependentCatalog && rawValues.Count > 1)
            {
                return (UPConfigFilter)configFilter.QueryByApplyingFilterParameter(firstParameter);
            }

            return (UPConfigFilter)configFilter.QueryByApplyingFilterParameter(firstParameter);
        }

        private static UPConfigFilter QueryForDefault(UPConfigFilter configFilter, UPConfigFilterParameters parameters, UPMFilter upmFilter)
        {
            if (!(parameters?.NumberOfParameters > 0))
            {
                return configFilter;
            }

            if (parameters.NumberOfParameters > 1 && parameters.NumberOfParameters == parameters.NumberOfUnnamedParameters)
            {
                var unnmaedParameters = parameters.UnnamedParameters;
                var rawValues = upmFilter.RawValues;
                var valueCount = rawValues.Count;
                if (valueCount == unnmaedParameters.Count)
                {
                    for (var i = 0; i < valueCount; i++)
                    {
                        parameters.SetUnnamedValueForIndex(rawValues[i], i);
                    }

                    return (UPConfigFilter)configFilter.QueryByApplyingFilterParameters(parameters);
                }

                return configFilter;
            }
            else
            {
                var rawValues = upmFilter.RawValues;
                if (rawValues?.Count == 0)
                {
                    return configFilter;
                }

                var firstParameter = parameters.FirstParameter();
                firstParameter.Values = rawValues;
                return (UPConfigFilter)configFilter.QueryByApplyingFilterParameter(firstParameter);
            }
        }

        private static UPMFilter GetFilter(string filtername, UPConfigFilterParameters parameters)
        {
            UPMFilter filter = null;
            var parameter = parameters.FirstParameter();
            var fieldInfo = parameter.CrmFieldInfo;
            var type = fieldInfo.FieldType;
            var identifier = FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(fieldInfo.InfoAreaId, filtername, fieldInfo.FieldId.ToString());

            if (type == CatalogFilterK || type == CatalogFilterX)
            {
                filter = GetCatalogFilter(parameter, identifier, fieldInfo);
            }
            else if (type == DateFilter)
            {
                filter = GetDateFilter(parameters, identifier, fieldInfo);
            }
            else if (parameters.AllParameterNames != null &&
                (parameters.AllParameterNames.Contains("$parGPSXmin") || parameters.AllParameterNames.Contains("$parGPSXmax")
                || parameters.AllParameterNames.Contains("$parGPSYmin") || parameters.AllParameterNames.Contains("$parGPSYmax")))
            {
                filter = GetUPMDistanceFilter(identifier, fieldInfo);
            }
            else if (!string.IsNullOrEmpty(fieldInfo.RepType))
            {
                filter = GetRepFilter(identifier, fieldInfo);
            }
            else if (type == EditFieldF || type == EditFieldS || type == EditFieldL || type == EditFieldC || type == EditFieldB)
            {
                filter = GetEditFieldFilter(parameters, identifier, fieldInfo, type);
            }

            if (filter != null)
            {
                var fieldValueArray = parameter.Condition.FieldValues;
                if (fieldValueArray.Count > 1 && (string)fieldValueArray[0] == "$parValue")
                {
                    var defaultValueArray = new List<object>(fieldValueArray);
                    defaultValueArray.RemoveAt(0);
                    filter.SetDefaultRawValues(defaultValueArray);
                }
            }

            return filter;
        }

        private static UPMFilter GetEditFieldFilter(UPConfigFilterParameters parameters, FieldIdentifier identifier, UPCRMFieldInfo fieldInfo, string type)
        {
            var editFieldFilter = new UPMEditFieldFilter(identifier);
            var rangFilter = parameters.NumberOfParameters > 1;
            if (rangFilter)
            {
                editFieldFilter.ParameterName = LocalizedString.TextFrom;
                editFieldFilter.SecondParameterName = LocalizedString.TextTo;
            }

            CreateEditFields(fieldInfo, rangFilter, type, out var editField, out var secondEditField);

            if (!rangFilter)
            {
                editField.LabelText = fieldInfo.Label;
            }

            editFieldFilter.Editfield = editField;
            editFieldFilter.SecondEditfield = secondEditField;
            return editFieldFilter;
        }

        private static void CreateEditFields(UPCRMFieldInfo fieldInfo, bool rangFilter, string type, out UPMEditField editField, out UPMEditField secondEditField)
        {
            editField = null;
            secondEditField = null;
            switch (type)
            {
                case "F" when fieldInfo.PercentField:
                    editField = new UPMPercentEditField(StringIdentifier.IdentifierWithStringId("percentParam"));
                    if (rangFilter)
                    {
                        secondEditField = new UPMPercentEditField(StringIdentifier.IdentifierWithStringId("percentParam2"));
                    }

                    break;
                case "F":
                    editField = new UPMFloatEditField(StringIdentifier.IdentifierWithStringId("floatParam"));
                    if (rangFilter)
                    {
                        secondEditField = new UPMFloatEditField(StringIdentifier.IdentifierWithStringId("floatParam2"));
                    }

                    break;
                case "C":
                    editField = new UPMStringEditField(StringIdentifier.IdentifierWithStringId("stringParam"));
                    if (rangFilter)
                    {
                        secondEditField = new UPMStringEditField(StringIdentifier.IdentifierWithStringId("stringParam2"));
                    }

                    break;
                case "B":
                    editField = new UPMBooleanEditField(StringIdentifier.IdentifierWithStringId("booleanParam"));
                    break;
                default:
                    editField = new UPMIntegerEditField(StringIdentifier.IdentifierWithStringId("intParam"));
                    if (rangFilter)
                    {
                        secondEditField = new UPMIntegerEditField(StringIdentifier.IdentifierWithStringId("intParam2"));
                    }

                    break;
            }
        }

        private static UPMFilter GetRepFilter(FieldIdentifier identifier, UPCRMFieldInfo fieldInfo)
        {
            var repType = UPCRMReps.RepTypeFromString(fieldInfo.RepType);
            var repFilter = new UPMRepFilter(identifier)
            {
                RepContaner = UPRepsService.CreateRepContainerForRepType(repType)
            };
            return repFilter;
        }

        private static UPMFilter GetUPMDistanceFilter(FieldIdentifier identifier, UPCRMFieldInfo fieldInfo)
        {
            var distanceFilter = new UPMDistanceFilter(identifier)
            {
                ParameterName = fieldInfo.Label,
                Radius = 0
            };
            return distanceFilter;
        }

        private static UPMFilter GetDateFilter(UPConfigFilterParameters parameters, FieldIdentifier identifier, UPCRMFieldInfo fieldInfo)
        {
            var dateFilter = new UPMDateFilter(identifier)
            {
                ParameterName = fieldInfo.Label
            };
            if (parameters.NumberOfParameters > 1)
            {
                dateFilter.ParameterName = LocalizedString.TextFrom;
                dateFilter.SecondParameterName = LocalizedString.TextTo;
            }

            return dateFilter;
        }

        private static UPMFilter GetCatalogFilter(UPConfigFilterParameter parameter, FieldIdentifier identifier, UPCRMFieldInfo fieldInfo)
        {
            var catalogValueProvider = parameter.CatalogValueProvider();
            var catalogFilter = new UPMCatalogFilter(identifier, catalogValueProvider)
            {
                ParameterName = fieldInfo.Label
            };
            var filter = catalogFilter;
            if (catalogValueProvider.IsEmptyValue("0"))
            {
                catalogFilter.NullValueKey = "0";
            }

            return filter;
        }
    }
}