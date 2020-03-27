// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPObjectivesConfiguration.cs" company="Aurea Software Gmbh">
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
//   The UPObjectives Configuration
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Objectives
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The list configuration name
        /// </summary>
        public const string ListConfigurationName = "List";

        /// <summary>
        /// The edit configuration name
        /// </summary>
        public const string EditConfigurationName = "Edit";

        /// <summary>
        /// The parameter destinationfilter configurationname
        /// </summary>
        public const string ParameterDestinationFilterConfigurationName = "DestinationFilter";

        /// <summary>
        /// The parameter destinationfieldgroup configurationname
        /// </summary>
        public const string ParameterDestinationFieldGroupConfigurationName = "DestinationFieldGroupName";

        /// <summary>
        /// The parameter sourcefieldcontrol configurationname
        /// </summary>
        public const string ParameterSourceFieldControlConfigurationName = "SourceFieldControl";

        /// <summary>
        /// The field completed function
        /// </summary>
        public const string FieldCompletedFunction = "Completed";

        /// <summary>
        /// The field completed on function
        /// </summary>
        public const string FieldCompletedOnFunction = "CompletedOn";

        /// <summary>
        /// The field completed by function
        /// </summary>
        public const string FieldCompletedByFunction = "CompletedBy";
    }

    /// <summary>
    /// Objectives Configuration
    /// </summary>
    public class UPObjectivesConfiguration
    {
        /// <summary>
        /// Creates the specified view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="editMode">if set to <c>true</c> [edit mode].</param>
        /// <returns></returns>
        public static UPObjectivesConfiguration Create(ViewReference viewReference, string sectionName, Dictionary<string, object> parameters, bool editMode)
        {
            var newObject = new UPObjectivesConfiguration(viewReference, sectionName);
            return !newObject.SetControlsFromParametersEditMode(parameters, editMode) ? null : newObject;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPObjectivesConfiguration"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="sectionName">Name of the section.</param>
        private UPObjectivesConfiguration(ViewReference viewReference, string sectionName)
        {
            this.SectionName = sectionName;
            this.ViewReference = viewReference;
        }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        public string SectionName { get; private set; }

        /// <summary>
        /// Gets the section header label.
        /// </summary>
        /// <value>
        /// The section header label.
        /// </value>
        public string SectionHeaderLabel { get; private set; }

        /// <summary>
        /// Gets the destination field control.
        /// </summary>
        /// <value>
        /// The destination field control.
        /// </value>
        public FieldControl DestinationFieldControl { get; private set; }

        /// <summary>
        /// Gets the name of the destination filter.
        /// </summary>
        /// <value>
        /// The name of the destination filter.
        /// </value>
        public string DestinationFilterName { get; private set; }

        /// <summary>
        /// Gets the source field control.
        /// </summary>
        /// <value>
        /// The source field control.
        /// </value>
        public FieldControl SourceFieldControl { get; private set; }

        /// <summary>
        /// Gets the additional fields.
        /// </summary>
        /// <value>
        /// The additional fields.
        /// </value>
        public List<UPConfigFieldControlField> AdditionalFields { get; private set; }

        /// <summary>
        /// Gets the fields with function.
        /// </summary>
        /// <value>
        /// The fields with function.
        /// </value>
        public Dictionary<string, UPConfigFieldControlField> FieldsWithFunction { get; private set; }

        /// <summary>
        /// Gets or sets the execute action filter.
        /// </summary>
        /// <value>
        /// The execute action filter.
        /// </value>
        public UPConfigFilter ExecuteActionFilter { get; set; }

        /// <summary>
        /// Gets or sets the filter based decision.
        /// </summary>
        /// <value>
        /// The filter based decision.
        /// </value>
        public UPCRMFilterBasedDecision FilterBasedDecision { get; set; }

        /// <summary>
        /// Fields for function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public UPConfigFieldControlField FieldForFunction(string function)
        {
            return this.FieldsWithFunction.ValueOrDefault(function);
        }

        /// <summary>
        /// Combines the configuration name with section.
        /// </summary>
        /// <param name="configurationName">Name of the configuration.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public static string CombineConfigurationNameWithSection(string configurationName, string section)
        {
            return $"{configurationName}{section}";
        }

        /// <summary>
        /// Contexts the value for view reference key parameters.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="key">The key.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public string ContextValueForViewReferenceKeyParameters(ViewReference viewReference, string key, Dictionary<string, object> parameters)
        {
            if (parameters == null || !parameters.ContainsKey(key))
            {
                return viewReference.ContextValueForKey(key);
            }

            return parameters[key] as string;
        }

        private bool SetControlsFromParametersEditMode(Dictionary<string, object> parameters, bool editMode)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string destinationFieldgroupParameterName = UPObjectivesConfiguration.CombineConfigurationNameWithSection(Constants.ParameterDestinationFieldGroupConfigurationName, this.SectionName);
            string configName = this.ContextValueForViewReferenceKeyParameters(this.ViewReference, destinationFieldgroupParameterName, parameters);
            if (!string.IsNullOrEmpty(configName))
            {
                if (editMode)
                {
                    FieldControl listFieldControl = configStore.FieldControlByNameFromGroup(Constants.ListConfigurationName, configName);
                    this.DestinationFieldControl = configStore.FieldControlByNameFromGroup(Constants.EditConfigurationName, configName);
                    if (listFieldControl?.CrmSortFields != null)
                    {
                        this.DestinationFieldControl = new FieldControl(this.DestinationFieldControl, listFieldControl);
                    }
                }
                else
                {
                    this.DestinationFieldControl = configStore.FieldControlByNameFromGroup(Constants.ListConfigurationName, configName);
                }
            }

            if (this.DestinationFieldControl == null)
            {
                return false;
            }

            List<UPConfigFieldControlField> addFields = new List<UPConfigFieldControlField>();
            Dictionary<string, UPConfigFieldControlField> tempFieldDictionary = new Dictionary<string, UPConfigFieldControlField>();
            this.FieldsWithFunction = new Dictionary<string, UPConfigFieldControlField>();
            foreach (FieldControlTab tab in this.DestinationFieldControl.Tabs)
            {
                foreach (UPConfigFieldControlField field in tab.Fields)
                {
                    FieldAttributes fieldAttributes = field.Attributes;
                    if (!fieldAttributes.Hide)
                    {
                        addFields.Add(field);
                    }

                    if (!string.IsNullOrEmpty(field.Function))
                    {
                        tempFieldDictionary[field.Function] = field;
                    }
                }

                this.FieldsWithFunction = new Dictionary<string, UPConfigFieldControlField>(tempFieldDictionary);
                this.AdditionalFields = new List<UPConfigFieldControlField>(addFields);
            }

            configName = this.ContextValueForViewReferenceKeyParameters(this.ViewReference, CombineConfigurationNameWithSection(Constants.ParameterSourceFieldControlConfigurationName, this.SectionName), parameters);
            if (!string.IsNullOrEmpty(configName))
            {
                this.SourceFieldControl = configStore.FieldControlByNameFromGroup(Constants.ListConfigurationName, configName);
            }

            string destinationFilterParameterName = CombineConfigurationNameWithSection(Constants.ParameterDestinationFilterConfigurationName, this.SectionName);
            this.DestinationFilterName = this.ContextValueForViewReferenceKeyParameters(this.ViewReference, destinationFilterParameterName, parameters);
            UPConfigFilter filter = configStore.FilterByName(this.DestinationFilterName);
            if (filter != null)
            {
                this.SectionHeaderLabel = filter.DisplayName;
            }

            configName = this.ViewReference?.ContextValueForKey("ExecuteActionFilterName");
            if (!string.IsNullOrEmpty(configName))
            {
                this.ExecuteActionFilter = configStore.FilterByName(configName);
            }

            return true;
        }
    }
}
