// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryCondition.cs" company="Aurea Software Gmbh">
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
//   Query condition configuration
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json.Linq;
    using Platform;

    /// <summary>
    /// Query condition configuration
    /// </summary>
    public class UPConfigQueryCondition
    {
        /// <summary>
        /// The true condition
        /// </summary>
        private static UPConfigQueryCondition trueCondition;

        /// <summary>
        /// The true condition
        /// </summary>
        private static UPConfigQueryCondition falseCondition;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryCondition"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public UPConfigQueryCondition(List<object> definition, string infoAreaId)
        {
            this.Relation = (string)definition[0];
            if (this.Relation == "LEAF")
            {
                this.InfoAreaId = infoAreaId;
                this.FieldId = JObjectExtensions.ToInt(definition[1]);
                this.CompareOperator = (string)definition[2];
                this.FunctionName = (string)definition[3];
                this.FieldValues = (definition[4] as JArray)?.ToObject<List<object>>();
            }
            else
            {
                var count = definition.Count;
                var arr = new List<UPConfigQueryCondition>(count);
                for (var i = 1; i < count; i++)
                {
                    arr.Add(new UPConfigQueryCondition((definition[i] as JArray)?.ToObject<List<object>>(), infoAreaId));
                }

                this.SubConditions = arr;
                this.FieldId = -1;
                this.FunctionName = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryCondition"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public UPConfigQueryCondition(string key, List<object> values)
        {
            this.PropertyCondition = true;
            this.FunctionName = key;
            this.FieldValues = values;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryCondition"/> class.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="compare">
        /// The compare.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <param name="subConditions">
        /// The sub conditions.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        public UPConfigQueryCondition(
            string relation,
            string infoAreaId,
            int fieldId,
            string compare,
            List<object> values,
            string functionName,
            List<UPConfigQueryCondition> subConditions,
            Dictionary<string, UPConfigQueryCondition> propertyConditions = null)
        {
            this.Relation = relation;
            this.InfoAreaId = infoAreaId;
            this.FieldId = fieldId;
            this.CompareOperator = compare;
            this.FunctionName = functionName;
            this.FieldValues = values;
            this.SubConditions = subConditions;
            this.PropertyConditions = propertyConditions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryCondition"/> class.
        /// </summary>
        public UPConfigQueryCondition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryCondition"/> class.
        /// </summary>
        /// <param name="removeTableCondition">
        /// Remove table condition
        /// </param>
        public UPConfigQueryCondition(bool removeTableCondition)
        {
            this.RemoveTableCondition = removeTableCondition;
        }

        /// <summary>
        /// Gets the false condition.
        /// </summary>
        /// <value>
        /// The false condition.
        /// </value>
        public static UPConfigQueryCondition FalseCondition
            => falseCondition ?? (falseCondition = new UPConfigFixedQueryCondition(false));

        /// <summary>
        /// Gets the true condition.
        /// </summary>
        /// <value>
        /// The true condition.
        /// </value>
        public static UPConfigQueryCondition TrueCondition
            => trueCondition ?? (trueCondition = new UPConfigFixedQueryCondition(true));

        /// <summary>
        /// Gets the compare operator.
        /// </summary>
        /// <value>
        /// The compare operator.
        /// </value>
        public string CompareOperator { get; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; }

        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <value>
        /// The field values.
        /// </value>
        public List<object> FieldValues { get; }

        /// <summary>
        /// Gets the first value.
        /// </summary>
        /// <value>
        /// The first value.
        /// </value>
        public string FirstValue => this.FieldValues.Count > 0 ? (string)this.FieldValues[0] : null;

        /// <summary>
        /// Gets a value indicating whether [fixed value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [fixed value]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool FixedValue => true;

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public string FunctionName { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsFixed => false;

        /// <summary>
        /// Gets the number of sub conditions.
        /// </summary>
        /// <value>
        /// The number of sub conditions.
        /// </value>
        public int NumberOfSubConditions => this.SubConditions?.Count ?? 0;

        /// <summary>
        /// Gets the number of values.
        /// </summary>
        /// <value>
        /// The number of values.
        /// </value>
        public int NumberOfValues => this.FieldValues?.Count ?? 0;

        /// <summary>
        /// Gets a value indicating whether [property condition].
        /// </summary>
        /// <value>
        /// <c>true</c> if [property condition]; otherwise, <c>false</c>.
        /// </value>
        public bool PropertyCondition { get; }

        /// <summary>
        /// Gets the property conditions.
        /// </summary>
        /// <value>
        /// The property conditions.
        /// </value>
        public Dictionary<string, UPConfigQueryCondition> PropertyConditions { get; }

        /// <summary>
        /// Gets the relation.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public string Relation { get; }

        /// <summary>
        /// Gets a value indicating whether [remove table condition].
        /// </summary>
        /// <value>
        /// <c>true</c> if [remove table condition]; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveTableCondition { get; }

        /// <summary>
        /// Gets the sub conditions.
        /// </summary>
        /// <value>
        /// The sub conditions.
        /// </value>
        public List<UPConfigQueryCondition> SubConditions { get; }

        /// <summary>
        /// Adds the fields from javascript into dictionary with parameters.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void AddFieldsFromJavascriptIntoDictionary(
            Dictionary<string, object> dictionary,
            List<object> parameters)
        {
            if (this.Relation == "LEAF")
            {
                var key = StringExtensions.InfoAreaIdFieldId(this.InfoAreaId, this.FieldId);
                foreach (string value in this.FieldValues)
                {
#if PORTING
                    JSValue function = UPJavascriptEngine.Current().FunctionForScript(value);
                    if (function != null)
                    {
                        JSValue resultValue = function.CallWithArguments(new List<object> {parameters});
                        string resultValueString = resultValue.ToString();
                        if (!string.IsNullOrWhiteSpace(resultValueString))
                        {
                            dictionary[key] = resultValueString;
                            break;
                        }
                    }
#endif
                }

                if (!string.IsNullOrWhiteSpace(this.FunctionName))
                {
                    var array = (List<object>)dictionary[this.FunctionName];
                    if (array.Count > 0)
                    {
                        var arr = new List<object>(array) { key };
                        dictionary[this.FunctionName] = arr;
                    }
                    else
                    {
                        dictionary[this.FunctionName] = new List<object> { key };
                    }
                }
            }
            else
            {
                foreach (var subCondition in this.SubConditions)
                {
                    subCondition.AddFieldsFromJavascriptIntoDictionary(dictionary, parameters);
                }
            }
        }

        /// <summary>
        /// Adds the fields with values into dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public void AddFieldsWithValuesIntoDictionary(Dictionary<string, object> dictionary)
        {
            if (this.Relation == "LEAF")
            {
                var key = StringExtensions.InfoAreaIdFieldId(this.InfoAreaId, this.FieldId);
                foreach (string value in this.FieldValues)
                {
                    if (value.StartsWith("$curGeo") && SimpleIoc.Default.GetInstance<ILocationService>()?.CurrentLocation != null)
                    {
                        //var ls = SimpleIoc.Default.GetInstance<ILocationService>();
                        var ls = SimpleIoc.Default.GetInstance<ILocationService>();

                        if (value == "$curGeoLatitude" || value == "$curGeoY")
                        {
                            dictionary[key] = $"{ls.CurrentLocation.Latitude}";
                        }
                        else if (value == "$curGeoLongitude" || value == "$curGeoX")
                        {
                            dictionary[key] = $"{ls.CurrentLocation.Longitude}";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(value) && !value.StartsWith("$par") && !value.StartsWith("$cur") && value != "*")
                    {
                        dictionary[key] = value;
                    }
                }

                if (string.IsNullOrWhiteSpace(this.FunctionName))
                {
                    return;
                }

                var array = (List<object>)dictionary[this.FunctionName];
                if (array.Count > 0)
                {
                    var arr = new List<object>(array) { key };
                    dictionary[this.FunctionName] = arr;
                }
                else
                {
                    dictionary[this.FunctionName] = new List<object> { key };
                }
            }
            else
            {
                foreach (var subCondition in this.SubConditions)
                {
                    subCondition.AddFieldsWithValuesIntoDictionary(dictionary);
                }
            }
        }

        /// <summary>
        /// Adds the parameters table.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddParametersTable(UPConfigFilterParameters parameters, UPConfigQueryTable table)
        {
            var parametersAdded = false;
            var count = this.FieldValues?.Count;
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var v = (string)this.FieldValues[i];
                    if (!v.StartsWith("$par"))
                    {
                        continue;
                    }

                    parametersAdded = true;
                    parameters.AddParameter(new UPConfigFilterParameter(v, table, this, i));
                }
            }

            if (this.SubConditions != null)
            {
                foreach (var subCondition in this.SubConditions)
                {
                    parametersAdded |= subCondition.AddParametersTable(parameters, table);
                }
            }

            return parametersAdded;
        }

        /// <summary>
        /// Checks the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilterCheckResult"/>.
        /// </returns>
        public UPConfigFilterCheckResult CheckValue(string value)
        {
            if (this.SubConditions == null || this.SubConditions.Count == 0)
            {
                return this.CheckValueLeaf(value);
            }

            UPConfigFilterCheckResult firstResult = null;
            var breakOnFirst = this.Relation != "OR";
            foreach (var subCondition in this.SubConditions)
            {
                var currentResult = subCondition.CheckValue(value);
                if (currentResult != null)
                {
                    if (breakOnFirst)
                    {
                        return new UPConfigFilterCheckResult(currentResult, this);
                    }

                    if (firstResult == null)
                    {
                        firstResult = currentResult;
                    }
                }
                else if (!breakOnFirst)
                {
                    return null;
                }
            }

            return firstResult != null ? new UPConfigFilterCheckResult(firstResult, this) : null;
        }

        /// <summary>
        /// Checks the value field identifier.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilterCheckResult"/>.
        /// </returns>
        public UPConfigFilterCheckResult CheckValueFieldId(string value, int fieldId)
        {
            var conditionPart = this.ConditionForFieldId(fieldId);
            return conditionPart?.CheckValue(value);
        }

        /// <summary>
        /// Checks the value leaf.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilterCheckResult"/>.
        /// </returns>
        public UPConfigFilterCheckResult CheckValueLeaf(string value)
        {
            if (this.FieldValues.Count == 0)
            {
                return null;
            }

            var compareType = 0;
            foreach (string fieldValue in this.FieldValues)
            {
                if (fieldValue.StartsWith("$"))
                {
                    if (fieldValue == "$compareNumber")
                    {
                        compareType = 1;
                    }

                    continue;
                }

                var match = compareType == 1
                                ? this.ValueMatchesNumber(value, fieldValue)
                                : this.ValueMatchesString(value, fieldValue);

                if (match)
                {
                    return null;
                }
            }

            return new UPConfigFilterCheckResult(this);
        }

        /// <summary>
        /// Checks the with values.
        /// </summary>
        /// <param name="dict">
        /// The dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilterCheckResult"/>.
        /// </returns>
        public UPConfigFilterCheckResult CheckWithValues(Dictionary<string, object> dict)
        {
            return this.CheckWithValuesInfoAreaIdLinkId(dict, null, -1);
        }

        /// <summary>
        /// Checks the with values information area identifier link identifier.
        /// </summary>
        /// <param name="dict">
        /// The dictionary.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilterCheckResult"/>.
        /// </returns>
        public UPConfigFilterCheckResult CheckWithValuesInfoAreaIdLinkId(
            Dictionary<string, object> dict,
            string infoAreaId,
            int linkId)
        {
            if (this.SubConditions == null || this.SubConditions.Count == 0)
            {
                if (infoAreaId == null)
                {
                    infoAreaId = this.InfoAreaId;
                    linkId = -1;
                }

                var key = StringExtensions.InfoAreaIdLinkIdFieldId(infoAreaId, linkId, this.FieldId);
                var value = string.Empty;
                if (dict.Keys.Contains(key))
                {
                    value = (string)dict[key] ?? string.Empty;
                }

                return this.CheckValueLeaf(value);
            }

            List<UPConfigFilterCheckResult> allResults = null;
            bool orCondition = this.Relation == "OR";
            UPConfigFilterCheckResult firstResult = null;
            foreach (var subCondition in this.SubConditions)
            {
                var result = subCondition.CheckWithValuesInfoAreaIdLinkId(dict, infoAreaId, linkId);
                if (result != null && !orCondition)
                {
                    return result.ErrorKey == null ? new UPConfigFilterCheckResult(result, this) : result;
                }

                if (result == null && orCondition)
                {
                    return null;
                }

                if (result != null)
                {
                    if (firstResult == null)
                    {
                        firstResult = result;
                    }
                    else
                    {
                        if (allResults == null)
                        {
                            allResults = new List<UPConfigFilterCheckResult> { firstResult };
                        }

                        allResults.Add(result);
                    }
                }
            }

            if (allResults != null)
            {
                return new UPConfigFilterCheckResult(allResults, this);
            }

            if (firstResult != null && firstResult.ErrorKey == null)
            {
                return new UPConfigFilterCheckResult(firstResult, this);
            }

            return firstResult;
        }

        /// <summary>
        /// Compares the type of the check with field values compare.
        /// </summary>
        /// <param name="fieldValues">
        /// The field values.
        /// </param>
        /// <param name="compareType">
        /// Type of the compare.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CompareCheckWithFieldValuesCompareType(List<object> fieldValues, int compareType)
        {
            if (fieldValues.Count < 2)
            {
                return true;
            }

            var lastValue = (string)fieldValues[0];
            var count = fieldValues.Count;
            for (var i = 1; i < count; i++)
            {
                var currentValue = (string)fieldValues[i];
                if (string.IsNullOrEmpty(currentValue))
                {
                    continue;
                }

                bool match;
                switch (compareType)
                {
                    case 1:
                        match = this.ValueMatchesNumber(lastValue, currentValue);
                        break;
                    default:
                        match = this.ValueMatchesString(lastValue, currentValue);
                        break;
                }

                if (!match)
                {
                    return false;
                }

                lastValue = currentValue;
            }

            return true;
        }

        /// <summary>
        /// Conditions this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public UPInfoAreaCondition Condition()
        {
            if (this.Relation == "LEAF")
            {
                return new UPInfoAreaConditionLeaf(
                    this.InfoAreaId,
                    this.FieldId,
                    this.CompareOperator,
                    this.FieldValues);
            }

            var mainCondition = new UPInfoAreaConditionTree(this.Relation);
            var count = this.SubConditions.Count;
            for (var i = 0; i < count; i++)
            {
                mainCondition.AddSubCondition(this.SubConditions[i].Condition());
            }

            return mainCondition;
        }

        /// <summary>
        /// Conditions the by appending condition or relation.
        /// </summary>
        /// <param name="compareCondition">
        /// The compare condition.
        /// </param>
        /// <param name="orRelation">
        /// if set to <c>true</c> [or relation].
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition ConditionByAppendingConditionOrRelation(
            UPConfigQueryCondition compareCondition,
            bool orRelation)
        {
            if (this.IsEqualCondition(compareCondition))
            {
                return this;
            }

            var relation = orRelation ? "OR" : "AND";
            List<UPConfigQueryCondition> changedConditions;
            if (relation.Equals(this.Relation))
            {
                var subConditions = new List<UPConfigQueryCondition>(this.SubConditions);
                if (compareCondition.Relation.Equals(relation))
                {
                    subConditions.AddRange(compareCondition.SubConditions);
                }
                else
                {
                    subConditions.Add(compareCondition);
                }

                changedConditions = subConditions;
            }
            else
            {
                changedConditions = new List<UPConfigQueryCondition> { this, compareCondition };
            }

            return new UPConfigQueryCondition(
                orRelation ? "OR" : "AND",
                this.InfoAreaId,
                -1,
                null,
                null,
                null,
                changedConditions,
                this.PropertyConditions);
        }

        /// <summary>
        /// Conditions the by removing fixed conditions for unnamed parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition ConditionByRemovingFixedConditionsForUnnamedParameters()
        {
            if (this.Relation != "LEAF" || this.FieldValues.Count < 2 || !this.FieldValues[0].Equals("$parValue"))
            {
                return this;
            }

            return new UPConfigQueryCondition(
                this.Relation,
                this.InfoAreaId,
                this.FieldId,
                this.CompareOperator,
                new List<object> { "$parValue" },
                this.FunctionName,
                this.SubConditions,
                this.PropertyConditions);
        }

        /// <summary>
        /// Conditions the by removing unbound parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition ConditionByRemovingUnboundParameters()
        {
            if (this.Relation == "LEAF" || this.SubConditions == null || this.SubConditions.Count == 0)
            {
                var hasRemoveCondition = false;
                foreach (string v in this.FieldValues)
                {
                    if (!v.StartsWith("$par"))
                    {
                        continue;
                    }

                    hasRemoveCondition = true;
                    break;
                }

                if (!hasRemoveCondition)
                {
                    return this;
                }

                if (this.FieldValues.Count == 1)
                {
                    return null;
                }

                List<object> a = new List<object>();
                foreach (string v in this.FieldValues)
                {
                    if (!v.StartsWith("$par"))
                    {
                        a.Add(v);
                    }
                }

                if (a.Count == 0)
                {
                    return null;
                }

                return new UPConfigQueryCondition(
                    this.Relation,
                    this.InfoAreaId,
                    this.FieldId,
                    this.CompareOperator,
                    a,
                    this.FunctionName,
                    null,
                    this.PropertyConditions);
            }

            var hasChangedCondition = false;
            var subConditions = new List<UPConfigQueryCondition>();
            foreach (var s in this.SubConditions)
            {
                var changedCondition = s.ConditionByRemovingUnboundParameters();
                if (changedCondition != s)
                {
                    hasChangedCondition = true;
                    if (changedCondition != null)
                    {
                        subConditions.Add(changedCondition);
                    }
                }
                else
                {
                    subConditions.Add(s);
                }
            }

            if (!hasChangedCondition)
            {
                return this;
            }

            switch (subConditions.Count)
            {
                case 0:
                    return null;
                case 1:
                    return subConditions[0];
            }

            return new UPConfigQueryCondition(
                this.Relation,
                this.InfoAreaId,
                this.FieldId,
                this.CompareOperator,
                this.FieldValues,
                this.FunctionName,
                subConditions,
                this.PropertyConditions);
        }

        /// <summary>
        /// Conditions for field identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition ConditionForFieldId(int fieldId)
        {
            if (this.SubConditions == null || this.SubConditions.Count == 0)
            {
                return this.FieldId == fieldId ? this : null;
            }

            List<UPConfigQueryCondition> changedSubConditions = null;
            var count = this.SubConditions.Count;
            for (var i = 0; i < count; i++)
            {
                var currentCondition = this.SubConditions[i];
                var conditionForFieldId = currentCondition.ConditionForFieldId(fieldId);
                if (changedSubConditions != null || conditionForFieldId != currentCondition)
                {
                    if (changedSubConditions == null)
                    {
                        changedSubConditions = new List<UPConfigQueryCondition>();
                        for (var j = 0; j < i; j++)
                        {
                            changedSubConditions.Add(this.SubConditions[j]);
                        }
                    }

                    if (conditionForFieldId != null)
                    {
                        changedSubConditions.Add(conditionForFieldId);
                    }
                }
            }

            if (changedSubConditions == null)
            {
                return this;
            }

            if (changedSubConditions.Count == 0)
            {
                return null;
            }

            return new UPConfigQueryCondition(
                this.Relation,
                this.InfoAreaId,
                this.FieldId,
                this.CompareOperator,
                this.FieldValues,
                this.FunctionName,
                changedSubConditions,
                this.PropertyConditions);
        }

        /// <summary>
        /// Copies the with values sub conditions property conditions.
        /// </summary>
        /// <param name="newValues">
        /// The new values.
        /// </param>
        /// <param name="newSubConditions">
        /// The new sub conditions.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition CopyWithValuesSubConditionsPropertyConditions(
            List<object> newValues,
            List<UPConfigQueryCondition> newSubConditions,
            Dictionary<string, UPConfigQueryCondition> propertyConditions)
        {
            return new UPConfigQueryCondition(
                this.Relation,
                this.InfoAreaId,
                this.FieldId,
                this.CompareOperator,
                newValues,
                this.FunctionName,
                newSubConditions,
                propertyConditions);
        }

        /// <summary>
        /// Fills the condition field.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public void FillConditionField(Dictionary<string, UPCRMField> dictionary, string infoAreaId, int linkId)
        {
            if (this.Relation == "LEAF")
            {
                if (this.FieldId >= 0)
                {
                    var field = new UPCRMField(this.FieldId, infoAreaId, linkId);
                    dictionary[field.FieldIdentification] = field;
                }
            }
            else
            {
                foreach (var subCondition in this.SubConditions)
                {
                    subCondition.FillConditionField(dictionary, infoAreaId, linkId);
                }
            }
        }

        /// <summary>
        /// Determines whether [is equal condition] [the specified condition].
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEqualCondition(UPConfigQueryCondition condition)
        {
            if (!this.Relation.Equals(condition.Relation))
            {
                return false;
            }

            if (this.Relation == "LEAF")
            {
                if (!this.CompareOperator.Equals(condition.CompareOperator) || this.FieldId != condition.FieldId
                    || this.FieldValues.Count != condition.FieldValues.Count)
                {
                    return false;
                }

                var fieldValueCount = this.FieldValues.Count;
                for (var i = 0; i < fieldValueCount; i++)
                {
                    var cmpLeft = $"{this.FieldValues[i]}";
                    var cmpRight = $"{condition.FieldValues[0]}";
                    if (!cmpLeft.Equals(cmpRight))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (this.SubConditions.Count != condition.SubConditions.Count)
            {
                return false;
            }

            var conditionCount = this.SubConditions.Count;
            for (var i = 0; i < conditionCount; i++)
            {
                var subCondition = this.SubConditions[i];
                var compareCondition = condition.SubConditions[i];
                if (!subCondition.IsEqualCondition(compareCondition))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Needses the location.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool NeedsLocation()
        {
            if (this.Relation == "LEAF")
            {
                return this.FieldValues.Cast<string>().Any(value => value.StartsWith("$curGeo"));
            }

            return this.SubConditions.Any(subCondition => subCondition.NeedsLocation());
        }

        /// <summary>
        /// Queries the condition by applying replacements.
        /// </summary>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition QueryConditionByApplyingReplacements(UPConditionValueReplacement replacements)
        {
            List<object> newFieldValues = replacements.ReplaceFieldValues(this.FieldValues);
            if (this.FunctionName?.StartsWith("RemoveInfoAreaIf") == true)
            {
                return this.RemoveConditionForCondition(newFieldValues);
            }

            if (this.FunctionName?.StartsWith("Compare") == true)
            {
                var match = this.CompareCheckWithFieldValuesCompareType(
                    newFieldValues,
                    this.FunctionName.StartsWith("CompareNumeric") ? 1 : 0);

                return match ? TrueCondition : FalseCondition;
            }

            if (this.FieldValues?.Count > 0 && newFieldValues == null)
            {
                return null;
            }

            if (this.FunctionName?.StartsWith("Parameter:") == true)
            {
                return new UPConfigQueryCondition(this.FunctionName.Substring(10), newFieldValues);
            }

            return this.CreateCondition(replacements, out var newSubConditions, out var propertyConditions) ??
                   this.CreateCondition(newFieldValues, newSubConditions, propertyConditions);
        }

        /// <summary>
        /// Queries the name of the conditions with function.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// List of <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public List<UPConfigQueryCondition> QueryConditionsWithFunctionName(string functionName)
        {
            if (!string.IsNullOrWhiteSpace(functionName))
            {
                if (functionName == this.FunctionName)
                {
                    return new List<UPConfigQueryCondition> { this };
                }
            }

            if (!string.IsNullOrWhiteSpace(this.FunctionName))
            {
                return null;
            }

            if (this.SubConditions == null || !this.SubConditions.Any())
            {
                return string.IsNullOrEmpty(functionName) ? new List<UPConfigQueryCondition> { this } : null;
            }

            List<UPConfigQueryCondition> subConditions = null;
            var returnUnchanged = true;
            foreach (var queryCondition in this.SubConditions)
            {
                var conds = queryCondition.QueryConditionsWithFunctionName(functionName);
                if (conds == null || conds.Count != 1 || conds[0] != queryCondition)
                {
                    returnUnchanged = false;
                }

                if (conds?.Any() == true)
                {
                    if (subConditions != null)
                    {
                        subConditions.AddRange(conds);
                    }
                    else
                    {
                        subConditions = new List<UPConfigQueryCondition>(conds);
                    }
                }
            }

            return returnUnchanged ? new List<UPConfigQueryCondition> { this } : subConditions;
        }

        /// <summary>
        /// Removes the condition for condition.
        /// </summary>
        /// <param name="newFieldValues">
        /// The new field values.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition RemoveConditionForCondition(List<object> newFieldValues)
        {
            var check0 = false;
            var invert = false;
            var checkValue = false;
            var removeTable = false;

            if (this.FunctionName.IndexOf("Not", StringComparison.OrdinalIgnoreCase) > 0)
            {
                invert = true;
            }

            if (this.FunctionName.IndexOf("Or0", StringComparison.OrdinalIgnoreCase) > 0)
            {
                check0 = true;
            }

            if (this.FunctionName.IndexOf("HasValue", StringComparison.OrdinalIgnoreCase) > 0)
            {
                checkValue = true;
            }

            var firstValue = string.Empty;
            if (newFieldValues.Count > 0)
            {
                firstValue = (string)newFieldValues[0];
            }

            if (check0 && firstValue == "0")
            {
                removeTable = !invert;
            }
            else if (checkValue)
            {
                var found = false;
                for (var i = 1; i < newFieldValues.Count; i++)
                {
                    if (firstValue.Equals(newFieldValues[i]))
                    {
                        found = true;
                        break;
                    }
                }

                removeTable = found ? !invert : invert;
            }
            else if (string.IsNullOrWhiteSpace(firstValue) || firstValue.StartsWith("$cur") || firstValue.StartsWith("$par"))
            {
                removeTable = !invert;
            }
            else
            {
                removeTable = invert;
            }

            return removeTable ? new UPConfigQueryCondition() : null;
        }

        /// <summary>
        /// Subs the index of the condition at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public UPConfigQueryCondition SubConditionAtIndex(int index)
        {
            return this.SubConditions[index];
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            if (this.PropertyCondition)
            {
                builder.AppendFormat($"Property-Key:{this.FunctionName}, Values={this.FieldValues}");
                return builder.ToString();
            }

            if (!string.IsNullOrWhiteSpace(this.FunctionName))
            {
                builder.AppendFormat($"FunctionName={this.FunctionName}[");
            }

            if (this.SubConditions != null)
            {
                foreach (var subcondition in this.SubConditions)
                {
                    builder.AppendFormat(
                        builder.Length == 0 ? $"({subcondition})" : $" {this.Relation} ({subcondition})");
                }
            }
            else
            {
                if (builder.Length == 0)
                {
                    builder.AppendFormat($"F{this.FieldId} {this.CompareOperator}");
                }
                else
                {
                    builder.AppendFormat($"F{this.FieldId} {this.CompareOperator}");
                }

                if (this.FieldValues.Count == 1)
                {
                    builder.AppendFormat($" '{this.FieldValues[0]}'");
                }
                else
                {
                    var first = true;
                    builder.Append("[");
                    foreach (string value in this.FieldValues)
                    {
                        if (first)
                        {
                            builder.Append(value);
                            first = false;
                        }
                        else
                        {
                            builder.AppendFormat($",{value}");
                        }
                    }

                    builder.Append("]");
                }
            }

            if (this.PropertyConditions?.Count > 0)
            {
                builder.AppendFormat($",Prop={this.PropertyConditions}");
            }

            if (!string.IsNullOrWhiteSpace(this.FunctionName))
            {
                builder.Append("]");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Values at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueAtIndex(int index)
        {
            return this.FieldValues?.Count > index ? (string)this.FieldValues[index] : null;
        }

        /// <summary>
        /// Values the matches number.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValueMatchesNumber(string value, string pattern)
        {
            var doubleValue = double.Parse(value);
            var doublePattern = double.Parse(pattern);
            if (this.CompareOperator == "=")
            {
                return doubleValue.Equals(doublePattern);
            }

            if (this.CompareOperator == "<>")
            {
                return !doubleValue.Equals(doublePattern);
            }

            if (this.CompareOperator == ">=")
            {
                return doubleValue >= doublePattern;
            }

            if (this.CompareOperator == "<=")
            {
                return doubleValue <= doublePattern;
            }

            if (this.CompareOperator == ">")
            {
                return doubleValue > doublePattern;
            }

            if (this.CompareOperator == "<")
            {
                return doubleValue < doublePattern;
            }

            return true;
        }

        /// <summary>
        /// Values the matches string.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValueMatchesString(string value, string pattern)
        {
            var comparisonResult = string.Compare(value, pattern, StringComparison.Ordinal);
            if (this.CompareOperator == "=")
            {
                return comparisonResult == 0;
            }

            if (this.CompareOperator == "<>")
            {
                return comparisonResult != 0;
            }

            if (this.CompareOperator == ">=")
            {
                return comparisonResult >= 0;
            }

            if (this.CompareOperator == "<=")
            {
                return comparisonResult <= 0;
            }

            if (this.CompareOperator == ">")
            {
                return comparisonResult > 0;
            }

            if (this.CompareOperator == "<")
            {
                return comparisonResult < 0;
            }

            return true;
        }

        private UPConfigQueryCondition CreateCondition(
           UPConditionValueReplacement replacements,
           out List<UPConfigQueryCondition> newSubConditions,
           out Dictionary<string, UPConfigQueryCondition> propertyConditions)
        {
            newSubConditions = null;
            propertyConditions = null;

            var count = this.SubConditions?.Count ?? 0;
            if (count > 0)
            {
                var foundFixedCondition = false;
                for (var i = 0; i < count; i++)
                {
                    var oldCondition = this.SubConditions?[i];
                    var newCondition = oldCondition?.QueryConditionByApplyingReplacements(replacements);

                    if (newSubConditions != null || oldCondition != newCondition)
                    {
                        if (newCondition.RemoveTableCondition)
                        {
                            return newCondition;
                        }

                        if (newCondition.IsFixed)
                        {
                            if (newCondition.FixedValue)
                            {
                                if (this.Relation == "OR")
                                {
                                    return newCondition;
                                }

                                foundFixedCondition = true;
                                newCondition = null;
                            }
                            else
                            {
                                if (this.Relation == "AND")
                                {
                                    return newCondition;
                                }

                                foundFixedCondition = true;
                                newCondition = null;
                            }
                        }

                        if (newSubConditions == null)
                        {
                            newSubConditions = new List<UPConfigQueryCondition>(count);
                            for (int j = 0; j < i; j++)
                            {
                                newSubConditions.Add(this.SubConditions[j]);
                            }
                        }

                        if (newCondition?.PropertyCondition == true)
                        {
                            if (propertyConditions == null)
                            {
                                propertyConditions = new Dictionary<string, UPConfigQueryCondition>();
                            }

                            propertyConditions[newCondition.FunctionName] = newCondition;
                            continue;
                        }

                        if (newCondition?.PropertyConditions?.Count > 0)
                        {
                            if (propertyConditions == null)
                            {
                                propertyConditions = new Dictionary<string, UPConfigQueryCondition>();
                            }

                            propertyConditions.Append(newCondition.PropertyConditions);
                        }

                        if (newCondition != null)
                        {
                            newSubConditions.Add(newCondition);
                        }
                    }
                }

                if (foundFixedCondition && newSubConditions.Count == 0 && propertyConditions == null)
                {
                    return new UPConfigFixedQueryCondition(this.Relation == "AND");
                }
            }

            return null;
        }

        private UPConfigQueryCondition CreateCondition(
            List<object> newFieldValues,
            List<UPConfigQueryCondition> newSubConditions,
            Dictionary<string, UPConfigQueryCondition> propertyConditions)
        {
            if (newFieldValues == this.FieldValues && newSubConditions == null && propertyConditions == null)
            {
                return this;
            }

            var currentPropertyConditions = propertyConditions ?? this.PropertyConditions;

            if (newSubConditions?.Count == 1)
            {
                var subCond = newSubConditions[0];
                return new UPConfigQueryCondition(
                    subCond.Relation,
                    subCond.InfoAreaId,
                    subCond.FieldId,
                    subCond.CompareOperator,
                    subCond.FieldValues,
                    subCond.FunctionName,
                    subCond.SubConditions,
                    currentPropertyConditions);
            }

            var temp = newSubConditions ?? this.SubConditions;
            return this.CopyWithValuesSubConditionsPropertyConditions(newFieldValues, temp, currentPropertyConditions);
        }
    }

    /// <summary>
    /// Fixed query condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.UPConfigQueryCondition" />
    public class UPConfigFixedQueryCondition : UPConfigQueryCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFixedQueryCondition"/> class.
        /// </summary>
        /// <param name="value">
        /// if set to <c>true</c> [value].
        /// </param>
        public UPConfigFixedQueryCondition(bool value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating whether [fixed value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [fixed value]; otherwise, <c>false</c>.
        /// </value>
        public override bool FixedValue => this.Value;

        /// <summary>
        /// Gets a value indicating whether this instance is fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fixed; otherwise, <c>false</c>.
        /// </value>
        public override bool IsFixed => true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPConfigFixedQueryCondition"/> is value.
        /// </summary>
        /// <value>
        /// <c>true</c> if value; otherwise, <c>false</c>.
        /// </value>
        public bool Value { get; }
    }
}
