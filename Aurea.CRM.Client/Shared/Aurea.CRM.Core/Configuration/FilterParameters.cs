// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterParameters.cs" company="Aurea Software Gmbh">
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
//   Filter parameter definition
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Filter parameter definition
    /// </summary>
    public class UPConfigFilterParameters
    {
        /// <summary>
        /// The unnamed parameters
        /// </summary>
        public List<UPConfigFilterParameter> UnnamedParameters;

        /// <summary>
        /// Gets all parameter names.
        /// </summary>
        /// <value>
        /// All parameter names.
        /// </value>
        public List<string> AllParameterNames { get; private set; }

        /// <summary>
        /// Gets the named parameters.
        /// </summary>
        /// <value>
        /// The named parameters.
        /// </value>
        public Dictionary<string, List<UPConfigFilterParameter>> namedParameters { get; private set; }

        /// <summary>
        /// Gets the number of named parameters.
        /// </summary>
        /// <value>
        /// The number of named parameters.
        /// </value>
        public int NumberOfNamedParameters => this.namedParameters?.Count() ?? 0;

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        /// <value>
        /// The number of parameters.
        /// </value>
        public int NumberOfParameters => this.NumberOfNamedParameters + this.NumberOfUnnamedParameters;

        /// <summary>
        /// Gets the number of unnamed parameters.
        /// </summary>
        /// <value>
        /// The number of unnamed parameters.
        /// </value>
        public int NumberOfUnnamedParameters => this.UnnamedParameters?.Count ?? 0;

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void AddParameter(UPConfigFilterParameter parameter)
        {
            if (parameter == null)
            {
                return;
            }

            if (parameter.ParameterName == "$parValue")
            {
                if (this.UnnamedParameters == null)
                {
                    this.UnnamedParameters = new List<UPConfigFilterParameter>();
                }

                this.UnnamedParameters.Add(parameter);
            }
            else
            {
                if (this.namedParameters == null)
                {
                    this.namedParameters = new Dictionary<string, List<UPConfigFilterParameter>>();
                    this.AllParameterNames = new List<string>();
                }

                List<UPConfigFilterParameter> parameters = this.namedParameters.ContainsKey(parameter.ParameterName)
                                                               ? this.namedParameters[parameter.ParameterName]
                                                               : null;

                if (parameters == null)
                {
                    parameters = new List<UPConfigFilterParameter>();
                    this.namedParameters[parameter.ParameterName] = parameters;
                    this.AllParameterNames.Add(parameter.ParameterName);
                }

                parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Firsts the parameter.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigFilterParameter"/>.
        /// </returns>
        public UPConfigFilterParameter FirstParameter()
        {
            if (this.UnnamedParameters != null && this.UnnamedParameters.Any())
            {
                return this.UnnamedParameters[0];
            }

            if (this.namedParameters != null && this.namedParameters.Any())
            {
                return this.namedParameters.Values.First()[0];
            }

            return null;
        }

        /// <summary>
        /// Determines whether [has unvalued parameters].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasUnvaluedParameters()
        {
            if (this.UnnamedParameters.Any(parameter => !parameter.ValueWasSet))
            {
                return true;
            }

            foreach (var parameter in this.namedParameters.Values)
            {
                if (parameter.Any(p => !p.ValueWasSet))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the unnamed value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetUnnamedValue(string value)
        {
            return this.SetUnnamedValues(new List<string> { value });
        }

        /// <summary>
        /// Sets the index of the unnamed value for.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetUnnamedValueForIndex(string value, int index)
        {
            return this.SetUnnamedValuesForIndex(new List<string> { value }, index);
        }

        /// <summary>
        /// Sets the unnamed values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetUnnamedValues(List<string> values)
        {
            var wasSet = false;
            foreach (var parameter in this.UnnamedParameters)
            {
                parameter.Values = values;
                wasSet = true;
            }

            return wasSet;
        }

        /// <summary>
        /// Sets the index of the unnamed values for.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetUnnamedValuesForIndex(List<string> values, int index)
        {
            var parameter = this.UnnamedParameters[index];
            if (parameter == null)
            {
                return false;
            }

            parameter.Values = values;
            return true;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValue(string value)
        {
            return this.SetValues(new List<string> { value });
        }

        /// <summary>
        /// Sets the name of the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValueName(string value, string name)
        {
            return this.SetValuesName(new List<string> { value }, name);
        }

        /// <summary>
        /// Sets the values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValues(List<string> values)
        {
            if (this.UnnamedParameters != null)
            {
                SetValuesForParameters(values, this.UnnamedParameters);
            }

            if (this.namedParameters != null)
            {
                foreach (var parameters in this.namedParameters.Values)
                {
                    SetValuesForParameters(values, parameters);
                }
            }

            return this.UnnamedParameters != null || this.namedParameters != null;
        }

        /// <summary>
        /// Sets the name of the values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValuesName(List<string> values, string name)
        {
            List<UPConfigFilterParameter> parameters;
            if (name == "parValue")
            {
                parameters = this.UnnamedParameters;
            }
            else if (this.namedParameters != null)
            {
                parameters = this.namedParameters[name];
            }
            else
            {
                return false;
            }

            SetValuesForParameters(values, parameters);
            return true;
        }

        /// <summary>
        /// Sets the values for parameters.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        private static void SetValuesForParameters(List<string> values, List<UPConfigFilterParameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                parameter.Values = values;
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class UPConfigFilterParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilterParameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="valueIndex">
        /// Index of the value.
        /// </param>
        public UPConfigFilterParameter(
            string name,
            UPConfigQueryTable table,
            UPConfigQueryCondition condition,
            int valueIndex)
        {
            this.ParameterName = name;
            this.Table = table;
            this.Condition = condition;
            this.ValueIndex = valueIndex;
            this.Values = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilterParameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPConfigFilterParameter(string name, string value)
        {
            this.ParameterName = name;
            this.Table = null;
            this.Condition = null;
            if (value == null)
            {
                return;
            }

            this.Values = new List<string> { value };
            this.ValueWasSet = true;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public UPConfigQueryCondition Condition { get; }

        /// <summary>
        /// CRMs the field information.
        /// </summary>
        /// <returns></returns>
        public UPCRMFieldInfo CrmFieldInfo
            => UPCRMDataStore.DefaultStore.FieldInfoForInfoAreaFieldId(this.InfoAreaId, this.FieldId);

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId => this.Condition.FieldId;

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.Table.InfoAreaId;

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <value>
        /// The name of the parameter.
        /// </value>
        public string ParameterName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [remove first values from value set].
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove first values from value set]; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveFirstValuesFromValueSet { get; set; }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public UPConfigQueryTable Table { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get
            {
                return this.ValueWasSet ? this.Values[0] : null;
            }

            set
            {
                this.Values = new List<string>() { value };
                this.ValueWasSet = true;
            }
        }

        /// <summary>
        /// Gets the index of the value.
        /// </summary>
        /// <value>
        /// The index of the value.
        /// </value>
        public int ValueIndex { get; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<string> Values { get; set; }

        /// <summary>
        /// Gets a value indicating whether [value was set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [value was set]; otherwise, <c>false</c>.
        /// </value>
        public bool ValueWasSet { get; private set; }

        /// <summary>
        /// Catalogs the value provider.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCatalogValueProvider"/>.
        /// </returns>
        public UPCatalogValueProvider CatalogValueProvider()
        {
            var fieldInfo = this.CrmFieldInfo;
            if (fieldInfo != null && fieldInfo.IsCatalogField)
            {
                if (fieldInfo.ParentCatalogFieldId >= 0 && this.ValueIndex > 0)
                {
                    return new UPCatalogValueLoaderWithFixedParent(fieldInfo, this.Condition.ValueAtIndex(0));
                }

                return new UPCatalogValueLoader(fieldInfo);
            }

            return !string.IsNullOrEmpty(fieldInfo?.RepType) ? new UPRepValueLoader(fieldInfo) : null;
        }
    }
}
