// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Selector.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Selecter context delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Selecter context delegate
    /// </summary>
    public interface UPSelectorContextDelegate
    {
        /// <summary>
        /// Links the only online available.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="linkTargetInfoAreaId">
        /// The link target information area identifier.
        /// </param>
        /// <param name="linklinkId">
        /// The linklink identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool LinkOnlyOnlineAvailable(object context, string linkTargetInfoAreaId, int linklinkId);

        /// <summary>
        /// Senders the link for information area identifier link identifier.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string SenderLinkForInfoAreaIdLinkId(object context, string infoAreaId, int linkId);
    }

    /// <summary>
    /// Selecter implementation
    /// </summary>
    public class UPSelector
    {
        private const string KeyLinkInfoArea = "LinkInfoArea";
        private const string KeyNoLink = "NoLink";
        private const string KeyTemplateFilter = "TemplateFilter";
        private const string KeyListConfigs = "ListConfigs";
        private const string KeySearchAndListName = "SearchAndListName";

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSelector"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="filterParameters">
        /// The filter parameters.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        public UPSelector(
            string infoAreaId,
            string recordIdentification,
            int linkId,
            object definition,
            Dictionary<string, object> filterParameters,
            UPConfigFieldControlField fieldConfig)
        {
            this.InfoAreaId = infoAreaId;
            this.RecordIdentification = recordIdentification;
            this.LinkId = linkId;
            this.FilterParameters = filterParameters;
            this.Definition = definition as Dictionary<string, object>;
            var contextMenuName = this.Definition?.ValueOrDefault("value") as string;
            if (string.IsNullOrEmpty(contextMenuName))
            {
                return;
            }

            var menu = ConfigurationUnitStore.DefaultStore.MenuByName(contextMenuName);
            var viewReference = menu?.ViewReference;

            var parameters = viewReference?.ParameterDictionary();
            if (parameters == null || this.Definition == null)
            {
                return;
            }

            foreach (var parameter in parameters.Keys)
            {
                if (this.Definition?.ValueOrDefault(parameter) != null)
                {
                    continue;
                }

                var value = viewReference.ContextValueForKey(parameter);
                if (parameter == "ListConfigs")
                {
                    this.Definition[parameter] = value?.Split(',');
                }
                else
                {
                    this.Definition[parameter] = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSelector"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="filterParameters">
        /// The filter parameters.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        public UPSelector(
            string recordIdentification,
            object definition,
            Dictionary<string, object> filterParameters,
            UPConfigFieldControlField fieldConfig)
            : this(
                recordIdentification?.InfoAreaId(),
                recordIdentification,
                0,
                definition,
                filterParameters,
                fieldConfig)
        {
        }

        /// <summary>
        /// Gets or sets the static selector context delegate.
        /// </summary>
        /// <value>
        /// The static selector context delegate.
        /// </value>
        public static UPSelectorContextDelegate StaticSelectorContextDelegate { get; set; }

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>
        /// The definition.
        /// </value>
        public Dictionary<string, object> Definition { get; private set; }

        /// <summary>
        /// Gets the explicit key order.
        /// </summary>
        /// <value>
        /// The explicit key order.
        /// </value>
        public List<string> ExplicitKeyOrder { get; private set; }

        /// <summary>
        /// Gets the filter parameters.
        /// </summary>
        /// <value>
        /// The filter parameters.
        /// </value>
        public Dictionary<string, object> FilterParameters { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is static selector.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is static selector; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsStaticSelector => true;

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the option count.
        /// </summary>
        /// <value>
        /// The option count.
        /// </value>
        public int OptionCount => this.ExplicitKeyOrder?.Count ?? 0;

        /// <summary>
        /// Gets the possible values.
        /// </summary>
        /// <value>
        /// The possible values.
        /// </value>
        public Dictionary<string, UPSelectorOption> PossibleValues { get; private set; }

        /// <summary>
        /// Gets the possible values with label.
        /// </summary>
        /// <value>
        /// The possible values with label.
        /// </value>
        public Dictionary<string, object> PossibleValuesWithLabel { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the template filter.
        /// </summary>
        /// <value>
        /// The template filter.
        /// </value>
        public UPConfigFilter TemplateFilter { get; private set; }

        /// <summary>
        /// Selectors for.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="filterParameters">
        /// The filter parameters.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <returns>
        /// The <see cref="UPSelector"/>.
        /// </returns>
        public static UPSelector SelectorFor(
            string infoAreaId,
            string recordIdentification,
            int linkId,
            object definition,
            Dictionary<string, object> filterParameters,
            UPConfigFieldControlField fieldConfig)
        {
            var selectorType = SelectorTypeFromDefinition(definition);
            if (selectorType == "Record")
            {
                return new UPRecordSelector(
                    infoAreaId,
                    recordIdentification,
                    linkId,
                    definition,
                    filterParameters,
                    fieldConfig);
            }
            else
            {
                return new UPSelector(
                    infoAreaId,
                    recordIdentification,
                    linkId,
                    definition,
                    filterParameters,
                    fieldConfig);
            }
        }

        /// <summary>
        /// Selectors for.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="filterParameters">
        /// The filter parameters.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <returns>
        /// The <see cref="UPSelector"/>.
        /// </returns>
        public static UPSelector SelectorFor(
            string recordIdentification,
            object definition,
            Dictionary<string, object> filterParameters,
            UPConfigFieldControlField fieldConfig)
        {
            var selectorType = SelectorTypeFromDefinition(definition);
            return selectorType == "Record"
                ? new UPRecordSelector(recordIdentification, definition, filterParameters, fieldConfig)
                : new UPSelector(recordIdentification, definition, filterParameters, fieldConfig);
        }

        /// <summary>
        /// Selectors the type from definition.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string SelectorTypeFromDefinition(object definition)
        {
            var def = definition as Dictionary<string, object>;
            var contextMenuName = def?.ValueOrDefault("value") as string;
            if (contextMenuName != null)
            {
                var menu = ConfigurationUnitStore.DefaultStore.MenuByName(contextMenuName);
                if (menu?.ViewReference?.ViewName != null && menu.ViewReference.ViewName.StartsWith("RecordSelect"))
                {
                    return "Record";
                }
            }

            return def?.ValueOrDefault("Type") as string;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public virtual void Build()
        {
            var parentRecordIdentification = string.Empty;
            var linkInfoAreaId = this.Definition?.ValueOrDefault(KeyLinkInfoArea) as string;
            var noLink = linkInfoAreaId == KeyNoLink;
            if (!noLink)
            {
                noLink = this.Definition.ValueOrDefault(KeyNoLink).ToInt() != 0;
            }

            if (!noLink)
            {
                if (this.ProcessNoLink(linkInfoAreaId, ref parentRecordIdentification))
                {
                    return;
                }
            }

            var templateFilterName = this.Definition?.ValueOrDefault(KeyTemplateFilter) as string;
            if (!string.IsNullOrEmpty(templateFilterName))
            {
                this.TemplateFilter = ConfigurationUnitStore.DefaultStore.FilterByName(templateFilterName);
            }

            var listConfigurations = (List<object>)this.Definition?.ValueOrDefault(KeyListConfigs);
            var firstConfig = this.Definition?.ValueOrDefault(KeySearchAndListName) as string;
            if (firstConfig != null)
            {
                if (listConfigurations == null)
                {
                    listConfigurations = new List<object> { firstConfig };
                }
                else
                {
                    var combinedConfig = new List<object> { firstConfig };
                    combinedConfig.AddRange(listConfigurations);
                    listConfigurations = combinedConfig;
                }
            }

            if (listConfigurations == null)
            {
                return;
            }

            this.ProcessConfiguration(listConfigurations, noLink, parentRecordIdentification);
        }

        private bool ProcessNoLink(string linkInfoAreaId, ref string parentRecordIdentification)
        {
            if (string.IsNullOrWhiteSpace(linkInfoAreaId))
            {
                linkInfoAreaId = this.InfoAreaId;
                if (string.IsNullOrWhiteSpace(linkInfoAreaId))
                {
                    return true;
                }
            }

            if (linkInfoAreaId.Equals(this.RecordIdentification.InfoAreaId()))
            {
                parentRecordIdentification = this.RecordIdentification;
            }
            else if (!string.IsNullOrWhiteSpace(this.RecordIdentification))
            {
                var crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), linkInfoAreaId)
                {
                    DisableVirtualLinks = true
                };

                crmQuery.SetLinkRecordIdentification(this.RecordIdentification, this.LinkId);
                var result = crmQuery.Find();
                if (result.RowCount > 0)
                {
                    parentRecordIdentification = result.ResultRowAtIndex(0).RootRecordId;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private void ProcessConfiguration(
            IEnumerable<object> listConfigurations,
            bool noLink,
            string parentRecordIdentification)
        {
            var duplicatePostfix = 0;
            foreach (var configName in listConfigurations)
            {
                var crmQuery = new UPContainerMetaInfo(configName as string, this.FilterParameters);
                if (!noLink && !string.IsNullOrEmpty(parentRecordIdentification))
                {
                    crmQuery.SetLinkRecordIdentification(parentRecordIdentification);
                }

                var result = crmQuery.Find();
                var count = result.RowCount;
                for (var index = 0; index < count; index++)
                {
                    var option = new UPSelectorOption(
                        result.ResultRowAtIndex(index) as UPCRMResultRow,
                        crmQuery.SourceFieldControl);
                    if (this.PossibleValues == null)
                    {
                        this.PossibleValues = new Dictionary<string, UPSelectorOption> { { option.Name, option } };
                        this.ExplicitKeyOrder = new List<string> { option.Name };
                        this.PossibleValuesWithLabel = new Dictionary<string, object> { { option.Name, option.Name } };
                    }
                    else if (this.PossibleValues.ValueOrDefault(option.Name) == null)
                    {
                        this.PossibleValues[option.Name] = option;
                        this.ExplicitKeyOrder.Add(option.Name);
                        this.PossibleValuesWithLabel[option.Name] = option.Name;
                    }
                    else if (this.Definition.ValueOrDefault("DuplicateNames").ToInt() != 0)
                    {
                        var key = $"{option.Name}_{++duplicatePostfix}";
                        this.PossibleValues[key] = option;
                        this.ExplicitKeyOrder.Add(key);
                        this.PossibleValuesWithLabel[key] = option.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Displays the name of the value dictionary for option.
        /// </summary>
        /// <param name="optionName">
        /// Name of the option.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, string> DisplayValueDictionaryForOptionName(string optionName)
        {
            var option = this.PossibleValues?.ValueOrDefault(optionName);
            return option?.DisplayFieldValues;
        }

        /// <summary>
        /// Templates the filter for.
        /// </summary>
        /// <param name="optionName">
        /// Name of the option.
        /// </param>
        /// <param name="sourceValueDictionary">
        /// The source value dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilter"/>.
        /// </returns>
        public UPConfigFilter TemplateFilterFor(string optionName, Dictionary<string, object> sourceValueDictionary)
        {
            return this.TemplateFilter?.FilterByApplyingValueDictionaryDefaults(sourceValueDictionary, true);
        }

        /// <summary>
        /// Values the name of the dictionary for option.
        /// </summary>
        /// <param name="optionName">
        /// Name of the option.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ValueDictionaryForOptionName(string optionName)
        {
            var option = this.PossibleValues.ValueOrDefault(optionName);
            return option?.FieldValues;
        }
    }

    /// <summary>
    /// Selector option implementation
    /// </summary>
    public class UPSelectorOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSelectorOption"/> class.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        public UPSelectorOption(UPCRMResultRow row, FieldControl fieldControl)
        {
            string firstFieldName = null;
            var first = true;
            var dictionary = new Dictionary<string, object>();
            var displayDictionary = new Dictionary<string, string>();
            foreach (var tab in fieldControl.Tabs ?? new List<FieldControlTab>())
            {
                if (tab?.Fields == null)
                {
                    continue;
                }

                foreach (var field in tab.Fields)
                {
                    if (first)
                    {
                        firstFieldName = row.ValueAtIndex(field.TabIndependentFieldIndex);
                        first = false;
                    }

                    if (field.Function == "Name")
                    {
                        this.Name = row.ValueAtIndex(field.TabIndependentFieldIndex);
                        firstFieldName = null;
                    }
                    else if (!string.IsNullOrEmpty(field.Function))
                    {
                        dictionary[field.Function] = row.RawValueAtIndex(field.TabIndependentFieldIndex);
                        displayDictionary[field.Function] = row.ValueAtIndex(field.TabIndependentFieldIndex);
                    }
                }
            }

            if (this.Name == null)
            {
                this.Name = firstFieldName;
                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = "?";
                }
            }

            this.FieldValues = dictionary;
            this.DisplayFieldValues = displayDictionary;
        }

        /// <summary>
        /// Gets the display field values.
        /// </summary>
        /// <value>
        /// The display field values.
        /// </value>
        public Dictionary<string, string> DisplayFieldValues { get; private set; }

        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <value>
        /// The field values.
        /// </value>
        public Dictionary<string, object> FieldValues { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Labels the with format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string LabelWithFormat(string format)
        {
            if (this.DisplayFieldValues == null)
            {
                return format;
            }

            foreach (var key in this.DisplayFieldValues.Keys)
            {
                format = format.Replace($"%{key}", this.DisplayFieldValues[key]);
            }

            return format;
        }
    }

    /// <summary>
    /// Record selector implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Features.UPSelector" />
    public class UPRecordSelector : UPSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelector"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="def">
        /// The definition.
        /// </param>
        /// <param name="_filterParameters">
        /// The _filter parameters.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        public UPRecordSelector(
            string infoAreaId,
            string recordIdentification,
            int linkId,
            object def,
            Dictionary<string, object> _filterParameters,
            UPConfigFieldControlField fieldConfig)
            : base(infoAreaId, recordIdentification, linkId, def, _filterParameters, fieldConfig)
        {
            var definition = def as Dictionary<string, object>;
            var configStore = ConfigurationUnitStore.DefaultStore;
            var contextMenuName = definition?.ValueOrDefault("value") as string;
            if (string.IsNullOrEmpty(contextMenuName))
            {
                contextMenuName = definition?.ValueOrDefault("ContextMenu") as string;
            }

            var menu = configStore.MenuByName(contextMenuName);
            this.DisplayName = menu?.DisplayName;
            this.ImageName = menu?.ImageName;
            var viewReference = menu?.ViewReference;
            if (viewReference != null && (viewReference.ViewName == null || !viewReference.ViewName.StartsWith("RecordSelect")))
            {
                viewReference = null;
            }

            object linkIdString;
            if (viewReference != null)
            {
                this.TargetPrefix = viewReference?.ContextValueForKey("TargetPrefix");
                var recordLinkInfoAreaIdString = viewReference.ContextValueForKey("LinkRecord");

                this.RecordLinkInfoAreaIds = !string.IsNullOrEmpty(recordLinkInfoAreaIdString)
                                                 ? recordLinkInfoAreaIdString.Split(',').ToList()
                                                 : null;

                this.LinkTargetInfoAreaId = viewReference.ContextValueForKey("TargetLinkInfoAreaId");
                if (this.LinkTargetInfoAreaId == "NoLink")
                {
                    this.NoLink = true;
                }

                this.HideStandardFilter = viewReference.ContextValueForKey("HideStandardFilter") == "true";
                this.DisableLinkOption = viewReference.ContextValueForKey("DisableLinkOption") == "true";
                this.IgnoreFieldInfo = viewReference.ContextValueForKey("IgnoreFieldInfo") == "true";
                linkIdString = viewReference.ContextValueForKey("TargetLinkId");
                var strObj = viewReference.ContextValueForKey("ClearValues");
                if (!string.IsNullOrEmpty(strObj))
                {
                    this.ClearValues = strObj.ParseToList();
                }

                strObj = viewReference.ContextValueForKey("FixedValues");
                if (!string.IsNullOrEmpty(strObj))
                {
                    this.FixedValues = strObj.JsonDictionaryFromString();
                }

                var formattedParametersString = viewReference.ContextValueForKey("FormattedParameters");
                if (formattedParametersString != null)
                {
                    this.FormattedParameters = formattedParametersString.JsonDictionaryFromString();
                }

                this.SearchViewReference = viewReference;
            }
            else
            {
                this.TargetPrefix = definition.ValueOrDefault("TargetPrefix") as string;
                var recordLinkInfoAreaIdString = definition.ValueOrDefault("LinkRecord") as string;
                this.RecordLinkInfoAreaIds = !string.IsNullOrEmpty(recordLinkInfoAreaIdString)
                                                 ? recordLinkInfoAreaIdString.Split(',').ToList()
                                                 : null;

                this.LinkTargetInfoAreaId = definition.ValueOrDefault("TargetLinkInfoAreaId") as string;
                if (this.LinkTargetInfoAreaId == "NoLink")
                {
                    this.NoLink = true;
                }
                else if (Convert.ToInt32(definition.ValueOrDefault("NoLink")) > 0)
                {
                    this.NoLink = true;
                }

                this.FormattedParameters = definition.ValueOrDefault("FormattedParameters") as Dictionary<string, object>;
                linkIdString = definition.ValueOrDefault("TargetLinkId");
                this.ClearValues = definition.ValueOrDefault("ClearValues") as List<string>
                                   ?? definition.ValueOrDefault("Clear") as List<string>;

                this.FixedValues = definition.ValueOrDefault("FixedValues") as Dictionary<string, object>;
                this.HideStandardFilter = definition.ValueOrDefault("HideStandardFilter") as string == "true";
                this.DisableLinkOption = definition.ValueOrDefault("DisableLinkOption") as string == "true";
                this.IgnoreFieldInfo = definition.ValueOrDefault("IgnoreFieldInfo") as string == "true";
                var configMenu = configStore.MenuByName(contextMenuName);
                this.SearchViewReference = configMenu?.ViewReference;
            }

            if (this.RecordLinkInfoAreaIds != null && this.RecordLinkInfoAreaIds.Any())
            {
                var linkIdArray = new List<int>(this.RecordLinkInfoAreaIds.Count);
                var infoAreaArray = new List<string>(this.RecordLinkInfoAreaIds.Count);
                foreach (var current in this.RecordLinkInfoAreaIds)
                {
                    var parts = current.Split('#');
                    if (parts.Length > 1)
                    {
                        infoAreaArray.Add(parts[0]);
                        linkIdArray.Add(Convert.ToInt32(parts[1]));
                    }
                    else
                    {
                        infoAreaArray.Add(current);
                        linkIdArray.Add(-1);
                    }
                }

                this.RecordLinkInfoAreaIds = infoAreaArray;
                this.RecordLinkLinkIds = linkIdArray;
            }

            if (!string.IsNullOrEmpty(this.LinkTargetInfoAreaId))
            {
                if (linkIdString != null)
                {
                    this.LinklinkId = Convert.ToInt32(linkIdString);
                }
                else
                {
                    this.LinklinkId = -1;
                }
            }
            else if (this.SearchViewReference != null)
            {
                var searchAndList = configStore.SearchAndListByName(this.SearchViewReference.ContextValueForKey("ConfigName"));
                this.LinkTargetInfoAreaId = searchAndList?.InfoAreaId;
                if (fieldConfig != null && fieldConfig.InfoAreaId == this.LinkTargetInfoAreaId)
                {
                    this.LinklinkId = fieldConfig.LinkId;
                }
                else
                {
                    this.LinklinkId = -1;
                }
            }

            if (this.NoLink)
            {
                this.LinkTargetInfoAreaId = null;
                this.LinklinkId = -1;
            }
            else if (string.IsNullOrEmpty(this.LinkTargetInfoAreaId) && fieldConfig != null)
            {
                this.LinkTargetInfoAreaId = fieldConfig.InfoAreaId;
                this.LinklinkId = fieldConfig.LinkId;
            }

            if (!string.IsNullOrEmpty(this.LinkTargetInfoAreaId))
            {
                this.LinkInfo = UPCRMDataStore.DefaultStore.LinkInfoForInfoAreaTargetInfoAreaLinkId(
                    this.InfoAreaId,
                    this.LinkTargetInfoAreaId,
                    this.LinklinkId);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelector"/> class.
        /// </summary>
        /// <param name="_recordIdentification">
        /// The _record identification.
        /// </param>
        /// <param name="_definition">
        /// The _definition.
        /// </param>
        /// <param name="_filterParameters">
        /// The _filter parameters.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        public UPRecordSelector(
            string _recordIdentification,
            object _definition,
            Dictionary<string, object> _filterParameters,
            UPConfigFieldControlField fieldConfig)
            : this(
                _recordIdentification.InfoAreaId(),
                _recordIdentification,
                0,
                _definition,
                _filterParameters,
                fieldConfig)
        {
        }

        /// <summary>
        /// Gets the clear values.
        /// </summary>
        /// <value>
        /// The clear values.
        /// </value>
        public List<string> ClearValues { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable link option].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable link option]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableLinkOption { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the fixed values.
        /// </summary>
        /// <value>
        /// The fixed values.
        /// </value>
        public Dictionary<string, object> FixedValues { get; private set; }

        /// <summary>
        /// Gets the formatted parameters.
        /// </summary>
        /// <value>
        /// The formatted parameters.
        /// </value>
        public Dictionary<string, object> FormattedParameters { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [hide standard filter].
        /// </summary>
        /// <value>
        /// <c>true</c> if [hide standard filter]; otherwise, <c>false</c>.
        /// </value>
        public bool HideStandardFilter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [ignore field information].
        /// </summary>
        /// <value>
        /// <c>true</c> if [ignore field information]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreFieldInfo { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is static selector.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is static selector; otherwise, <c>false</c>.
        /// </value>
        public override bool IsStaticSelector => false;

        /// <summary>
        /// Gets the link information.
        /// </summary>
        /// <value>
        /// The link information.
        /// </value>
        public UPCRMLinkInfo LinkInfo { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [link is disabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [link is disabled]; otherwise, <c>false</c>.
        /// </value>
        public bool LinkIsDisabled { get; set; }

        /// <summary>
        /// Gets the link key.
        /// </summary>
        /// <value>
        /// The link key.
        /// </value>
        public string LinkKey
            =>
                this.LinkTargetInfoAreaId != null
                    ? $"{this.LinkTargetInfoAreaId}#{(this.LinklinkId >= 0 ? this.LinklinkId : 0)}"
                    : null;

        /// <summary>
        /// Gets the linklink identifier.
        /// </summary>
        /// <value>
        /// The linklink identifier.
        /// </value>
        public int LinklinkId { get; private set; }

        /// <summary>
        /// Gets the link target information area identifier.
        /// </summary>
        /// <value>
        /// The link target information area identifier.
        /// </value>
        public string LinkTargetInfoAreaId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [no link].
        /// </summary>
        /// <value>
        /// <c>true</c> if [no link]; otherwise, <c>false</c>.
        /// </value>
        public bool NoLink { get; private set; }

        /// <summary>
        /// Gets the record link information area ids.
        /// </summary>
        /// <value>
        /// The record link information area ids.
        /// </value>
        public List<string> RecordLinkInfoAreaIds { get; private set; }

        /// <summary>
        /// Gets the record link link ids.
        /// </summary>
        /// <value>
        /// The record link link ids.
        /// </value>
        public List<int> RecordLinkLinkIds { get; private set; }

        /// <summary>
        /// Gets the search view reference.
        /// </summary>
        /// <value>
        /// The search view reference.
        /// </value>
        public ViewReference SearchViewReference { get; private set; }

        /// <summary>
        /// Gets the target prefix.
        /// </summary>
        /// <value>
        /// The target prefix.
        /// </value>
        public string TargetPrefix { get; private set; }

        /// <summary>
        /// Strings the by applying values to string.
        /// </summary>
        /// <param name="rowValues">
        /// The row values.
        /// </param>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringByApplyingValuesToString(Dictionary<string, object> rowValues, string str)
        {
            if (rowValues == null)
            {
                return null;
            }

            foreach (var parameter in rowValues.Keys)
            {
                var v = rowValues.ValueOrDefault(parameter);
                str = str.Replace($"%{parameter}", v as string);
            }

            return str;
        }

        /// <summary>
        /// Valueses from result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ValuesFromResultRow(UPCRMResultRow resultRow)
        {
            Dictionary<string, object> dict = null;

            if (resultRow == null)
            {
                dict = new Dictionary<string, object>();
                var configStore = ConfigurationUnitStore.DefaultStore;
                var contextMenuName = this.Definition.ValueOrDefault("value") as string;
                if (string.IsNullOrEmpty(contextMenuName))
                {
                    contextMenuName = this.Definition.ValueOrDefault("ContextMenu") as string;
                }

                var menu = configStore.MenuByName(contextMenuName);
                var viewReference = menu?.ViewReference;
                var configExpand = configStore.ExpandByName(viewReference?.ContextValueForKey("ConfigName"));
                var fieldControl = configStore.FieldControlByNameFromGroup(
                    "List",
                    configExpand != null ? configExpand.FieldGroupName : viewReference?.ContextValueForKey("ConfigName"));

                var functionNameFieldMapping = fieldControl?.FunctionNames();
                if (functionNameFieldMapping != null)
                {
                    foreach (var key in functionNameFieldMapping.Keys)
                    {
                        if (!string.IsNullOrEmpty(this.TargetPrefix))
                        {
                            dict[$"{this.TargetPrefix}{key}"] = string.Empty;
                        }
                        else
                        {
                            dict[$"{key}"] = string.Empty;
                        }
                    }
                }

                if (this.FormattedParameters != null)
                {
                    foreach (var parameterName in this.FormattedParameters.Keys)
                    {
                        if (!string.IsNullOrEmpty(this.TargetPrefix))
                        {
                            dict[$"{this.TargetPrefix}{parameterName}"] = string.Empty;
                        }
                        else
                        {
                            dict[$"{parameterName}"] = string.Empty;
                        }
                    }
                }

                return dict;
            }

            var rowValues = resultRow.ValuesWithFunctions(true);
            if (this.FormattedParameters != null)
            {
                dict = new Dictionary<string, object>(rowValues);
                if (rowValues.Count > 0)
                {
                    foreach (var parameterName in this.FormattedParameters.Keys)
                    {
                        var patternString = this.FormattedParameters.ValueOrDefault(parameterName) as string;
                        var str = patternString.ReferencedStringWithDefault(patternString);
                        str = this.StringByApplyingValuesToString(rowValues, str);
                        if (!string.IsNullOrEmpty(str))
                        {
                            dict[parameterName] = str;
                        }
                    }
                }

                rowValues = dict;
            }

            // Dictionary<string, string> dict = null;
            if (!string.IsNullOrEmpty(this.TargetPrefix))
            {
                dict = new Dictionary<string, object>(rowValues.Count);
                foreach (var name in rowValues.Keys)
                {
                    dict[$"{this.TargetPrefix}{name}"] = rowValues.ValueOrDefault(name);
                }
            }
            else
            {
                dict = new Dictionary<string, object>(rowValues);
            }

            foreach (var clearValue in this.ClearValues ?? new List<string>())
            {
                if (dict.ValueOrDefault(clearValue) == null)
                {
                    dict[clearValue] = string.Empty;
                }
            }

            foreach (var fixedValue in (this.FixedValues ?? new Dictionary<string, object>()).Keys)
            {
                if (dict.ValueOrDefault(fixedValue) == null)
                {
                    dict[fixedValue] = this.FixedValues.ValueOrDefault(fixedValue) as string;
                }
            }

            return dict;
        }
    }
}