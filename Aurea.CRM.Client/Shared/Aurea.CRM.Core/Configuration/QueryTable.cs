// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryTable.cs" company="Aurea Software Gmbh">
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
//   Query table configurations
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

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Query table configurations
    /// </summary>
    public class UPConfigQueryTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryTable"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="isQuery">
        /// if set to <c>true</c> [is query].
        /// </param>
        public UPConfigQueryTable(List<object> definition, bool isQuery)
        {
            this.InfoAreaId = (string)definition[0];
            this.LinkId = JObjectExtensions.ToInt(definition[1]);
            this.ParentRelation = (string)definition[2];
            if (!isQuery)
            {
                if (this.ParentRelation == "WITH")
                {
                    this.ParentRelation = "HAVING";
                }
                else if (this.ParentRelation == "WITHOPTIONAL")
                {
                    this.ParentRelation = "HAVINGOPTIONAL";
                }
            }

            var conditiondef = (definition[3] as JArray)?.ToObject<List<object>>();
            if (conditiondef != null)
            {
                this.Condition = new UPConfigQueryCondition(conditiondef, this.InfoAreaId);
            }

            var subtableDefs = (definition[4] as JArray)?.ToObject<List<object>>();
            if (subtableDefs != null)
            {
                var count = subtableDefs.Count;
                var arr = new List<UPConfigQueryTable>(count);
                for (var i = 0; i < count; i++)
                {
                    arr.Add(new UPConfigQueryTable((subtableDefs[i] as JArray)?.ToObject<List<object>>(), isQuery));
                }

                this.SubTables = arr;
            }

            if (definition.Count > 5)
            {
                this.Alias = (string)definition[5];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryTable"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="parentRelation">
        /// The parent relation.
        /// </param>
        /// <param name="subTables">
        /// The sub tables.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        public UPConfigQueryTable(
            string infoAreaId,
            int linkId,
            string parentRelation,
            List<UPConfigQueryTable> subTables,
            UPConfigQueryCondition condition,
            Dictionary<string, UPConfigQueryCondition> propertyConditions,
            string alias)
        {
            this.InfoAreaId = infoAreaId;
            this.LinkId = linkId;
            this.ParentRelation = parentRelation;
            this.SubTables = subTables;
            this.Condition = condition;
            this.PropertyConditions = propertyConditions;
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public UPConfigQueryCondition Condition { get; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => this.LinkId <= 0 ? this.InfoAreaId : $"{this.InfoAreaId}#{this.LinkId}";

        /// <summary>
        /// Gets the key with relation.
        /// </summary>
        /// <value>
        /// The key with relation.
        /// </value>
        public string KeyWithRelation
            =>
                this.LinkId <= 0
                    ? $"{this.ParentRelation}.{this.InfoAreaId}"
                    : $"{this.ParentRelation}.{this.InfoAreaId}#{this.LinkId}";

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; }

        /// <summary>
        /// Gets a value indicating whether [needs location].
        /// </summary>
        /// <value>
        /// <c>true</c> if [needs location]; otherwise, <c>false</c>.
        /// </value>
        public bool NeedsLocation => this.Condition.NeedsLocation();

        /// <summary>
        /// Gets the number of sub tables.
        /// </summary>
        /// <value>
        /// The number of sub tables.
        /// </value>
        public int NumberOfSubTables => this.SubTables?.Count ?? 0;

        /// <summary>
        /// Gets the parent relation.
        /// </summary>
        /// <value>
        /// The parent relation.
        /// </value>
        public string ParentRelation { get; }

        /// <summary>
        /// Gets the property conditions.
        /// </summary>
        /// <value>
        /// The property conditions.
        /// </value>
        public Dictionary<string, UPConfigQueryCondition> PropertyConditions { get; }

        /// <summary>
        /// Gets the sub tables.
        /// </summary>
        /// <value>
        /// The sub tables.
        /// </value>
        public List<UPConfigQueryTable> SubTables { get; }

        /// <summary>
        /// Adds the fields.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="recursive">
        /// if set to <c>true</c> [recursive].
        /// </param>
        /// <param name="flat">
        /// if set to <c>true</c> [flat].
        /// </param>
        /// <param name="executeJavascript">
        /// if set to <c>true</c> [execute javascript].
        /// </param>
        public void AddFields(Dictionary<string, object> dictionary, bool recursive, bool flat, bool executeJavascript)
        {
            if (executeJavascript)
            {
                UPConfigQueryCondition javascriptCondition = this.PropertyConditions.ValueOrDefault("Arguments");
                if (javascriptCondition != null)
                {
                    this.Condition.AddFieldsFromJavascriptIntoDictionary(dictionary, javascriptCondition.FieldValues);
                    return;
                }
            }

            this.Condition?.AddFieldsWithValuesIntoDictionary(dictionary);

            if (this.PropertyConditions != null)
            {
                foreach (var key in this.PropertyConditions.Keys)
                {
                    var _condition = this.PropertyConditions[key];
                    dictionary[$".{key}"] = _condition.FieldValues;
                }
            }

            if (recursive && this.SubTables != null)
            {
                foreach (var subTable in this.SubTables)
                {
                    var subDictionary = new Dictionary<string, object>();
                    subTable.AddFields(subDictionary, true, flat, executeJavascript);
                    if (subDictionary.Count > 0)
                    {
                        if (flat && subTable.ParentRelation != "WITHOUT")
                        {
                            if (subTable.LinkId > 0)
                            {
                                var keyPrefix = $"{subTable.InfoAreaId}.";
                                var targetPrefix = $"{subTable.InfoAreaId}:{subTable.LinkId}.";
                                foreach (var key in subDictionary.Keys)
                                {
                                    if (key.StartsWith(keyPrefix))
                                    {
                                        dictionary[$"{targetPrefix}{key.Substring(keyPrefix.Length)}"] = subDictionary[key];
                                    }
                                    else
                                    {
                                        dictionary[key] = subDictionary[key];
                                    }
                                }
                            }
                            else
                            {
                                foreach (var key in subDictionary.Keys)
                                {
                                    dictionary[key] = subDictionary[key];
                                }
                            }
                        }
                        else
                        {
                            var linkIdTmp = subTable.LinkId <= 0 ? 0 : subTable.LinkId;
                            if (linkIdTmp <= 0 && subTable.InfoAreaId == this.InfoAreaId)
                            {
                                dictionary = dictionary.Concat(subDictionary).ToDictionary(k => k.Key, k => k.Value);
                                continue;
                            }

                            var key = subTable.ParentRelation == "WITHOUT"
                                          ? $"#{subTable.InfoAreaId}.{linkIdTmp}"
                                          : $".{subTable.InfoAreaId}.{linkIdTmp}";

                            var arr = dictionary.ValueOrDefault(key) as List<object>;
                            if (arr != null)
                            {
                                arr.Add(subDictionary);
                            }
                            else
                            {
                                arr = new List<object> { subDictionary };
                                dictionary[key] = arr;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddParameters(UPConfigFilterParameters parameters)
        {
            var parametersAdded = false;
            if (this.Condition != null)
            {
                parametersAdded |= this.Condition.AddParametersTable(parameters, this);
            }

            if (this.SubTables != null)
            {
                foreach (var subtable in this.SubTables)
                {
                    parametersAdded |= subtable.AddParameters(parameters);
                }
            }

            return parametersAdded;
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
            return this.Condition?.CheckValueFieldId(value, fieldId);
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
            return this.Condition.CheckWithValues(dict);
        }

        /// <summary>
        /// Copies the with sub tables condition property conditions.
        /// </summary>
        /// <param name="newSubTables">
        /// The new sub tables.
        /// </param>
        /// <param name="newCondition">
        /// The new condition.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable CopyWithSubTablesConditionPropertyConditions(
            List<UPConfigQueryTable> newSubTables,
            UPConfigQueryCondition newCondition,
            Dictionary<string, UPConfigQueryCondition> propertyConditions)
        {
            return new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                this.ParentRelation,
                newSubTables,
                newCondition,
                propertyConditions,
                this.Alias);
        }

        /// <summary>
        /// Fieldses the with values.
        /// </summary>
        /// <param name="includeSubtables">
        /// if set to <c>true</c> [include subtables].
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> FieldsWithValues(bool includeSubtables)
        {
            return this.FieldsWithValues(includeSubtables, false, true);
        }

        /// <summary>
        /// Fieldses the with values.
        /// </summary>
        /// <param name="includeSubtables">
        /// if set to <c>true</c> [include subtables].
        /// </param>
        /// <param name="flat">
        /// if set to <c>true</c> [flat].
        /// </param>
        /// <param name="executeJavascript">
        /// if set to <c>true</c> [execute javascript].
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> FieldsWithValues(bool includeSubtables, bool flat, bool executeJavascript)
        {
            var dictionary = new Dictionary<string, object>();
            this.AddFields(dictionary, includeSubtables, flat, executeJavascript);
            return dictionary;
        }

        /// <summary>
        /// Fills the dictionary.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="withLinks">
        /// if set to <c>true</c> [with links].
        /// </param>
        public void FillDictionary(Dictionary<string, UPCRMField> dictionary, string infoAreaId, bool withLinks)
        {
            if (this.InfoAreaId.Equals(infoAreaId) || withLinks)
            {
                this.Condition?.FillConditionField(dictionary, this.InfoAreaId, this.LinkId);
            }

            if (this.SubTables != null)
            {
                foreach (var subTable in this.SubTables)
                {
                    if (subTable.InfoAreaId.Equals(infoAreaId) && subTable.LinkId <= 0)
                    {
                        subTable.FillDictionary(dictionary, infoAreaId, withLinks);
                    }
                    else if (withLinks)
                    {
                        subTable.FillDictionary(dictionary, subTable.InfoAreaId, false);
                    }
                }
            }
        }

        /// <summary>
        /// Queries the conditions.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <param name="includeSubTables">
        /// if set to <c>true</c> [include sub tables].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPConfigQueryCondition> QueryConditions(string functionName, bool includeSubTables)
        {
            var root = this.Condition?.QueryConditionsWithFunctionName(functionName);
            if (!includeSubTables)
            {
                return root;
            }

            var arr = new List<UPConfigQueryCondition>();
            if (root?.Count > 0)
            {
                arr.AddRange(root);
            }

            if (this.SubTables != null)
            {
                foreach (var subTable in this.SubTables)
                {
                    var subTableCond = subTable.QueryConditions(functionName, true);
                    if (subTableCond?.Count > 0)
                    {
                        arr.AddRange(subTableCond);
                    }
                }
            }

            return arr.Count > 0 ? arr : null;
        }

        /// <summary>
        /// Queries the table by applying replacements.
        /// </summary>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable QueryTableByApplyingReplacements(UPConditionValueReplacement replacements)
        {
            List<UPConfigQueryTable> newSubTables = null;
            UPConfigQueryCondition newCondition = null;
            var changed = false;
            if (this.SubTables != null)
            {
                var count = this.SubTables.Count;
                newSubTables = new List<UPConfigQueryTable>(count);
                for (var i = 0; i < count; i++)
                {
                    var oldSubTable = this.SubTables[i];
                    var newSubTable = oldSubTable.QueryTableByApplyingReplacements(replacements);
                    if (newSubTable != null)
                    {
                        newSubTables.Add(newSubTable);
                        if (newSubTable != oldSubTable)
                        {
                            changed = true;
                        }
                    }
                    else
                    {
                        changed = true;
                    }
                }

                if (!changed)
                {
                    newSubTables = null;
                }
            }

            var propertyConditions = this.PropertyConditions;
            if (this.Condition != null)
            {
                newCondition = this.Condition.QueryConditionByApplyingReplacements(replacements);
                if (newCondition == null)
                {
                    return null;
                }

                if (newCondition.IsFixed)
                {
                    newCondition = null; // TODO 20150126: result depending on fixedValue?
                }

                if (newCondition != null && newCondition != this.Condition)
                {
                    if (newCondition.PropertyCondition)
                    {
                        propertyConditions = new Dictionary<string, UPConfigQueryCondition>
                                                 { { newCondition.FunctionName, newCondition } };

                        newCondition = null;
                    }
                    else if (newCondition.PropertyConditions != null)
                    {
                        propertyConditions = newCondition.PropertyConditions;
                    }

                    changed = true;
                }

                if (newCondition?.RemoveTableCondition ?? false)
                {
                    return null;
                }
            }

            if (changed)
            {
                var temp = newSubTables ?? this.SubTables;
                return this.CopyWithSubTablesConditionPropertyConditions(temp, newCondition, propertyConditions);
            }

            return this;
        }

        /// <summary>
        /// Queries the table by applying value dictionary.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable QueryTableByApplyingValueDictionary(Dictionary<string, object> valueDictionary)
        {
            if (valueDictionary == null)
            {
                return this;
            }

            var replacements = new UPConditionValueReplacement(valueDictionary);
            return this.QueryTableByApplyingReplacements(replacements);
        }

        /// <summary>
        /// Queries the table for value dictionary with sub information areas.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <param name="withSubInfoAreas">
        /// if set to <c>true</c> [with sub information areas].
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable QueryTableForValueDictionaryWithSubInfoAreas(
            Dictionary<string, object> valueDictionary,
            bool withSubInfoAreas)
        {
            var hasFailingSubCondition = false;
            if (this.SubTables != null)
            {
                foreach (var subTable in this.SubTables)
                {
                    var sameTable = this.InfoAreaId.Equals(subTable.InfoAreaId) && subTable.LinkId <= 0;
                    if (!withSubInfoAreas && !sameTable)
                    {
                        continue;
                    }

                    var foundTable = subTable.QueryTableForValueDictionaryWithSubInfoAreas(
                        valueDictionary,
                        withSubInfoAreas && sameTable);
                    if (foundTable != null && sameTable)
                    {
                        return foundTable;
                    }

                    if (foundTable == null)
                    {
                        hasFailingSubCondition = true;
                    }
                }
            }

            if (hasFailingSubCondition)
            {
                return null;
            }

            var result = this.CheckWithValues(valueDictionary);
            return result?.FailingCondition != null ? null : this;
        }

        /// <summary>
        /// Subs the index of the table at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable SubTableAtIndex(int index)
        {
            return this.SubTables[index];
        }

        /// <summary>
        /// Subs the table dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, UPConfigQueryTable> SubTableDictionary()
        {
            if (this.SubTables == null || this.SubTables.Count == 0)
            {
                return null;
            }

            var dict = new Dictionary<string, UPConfigQueryTable>();
            foreach (var subTable in this.SubTables)
            {
                dict[subTable.KeyWithRelation] = subTable;
            }

            return dict;
        }

        /// <summary>
        /// Tables the by appending table or relation.
        /// </summary>
        /// <param name="mergeTable">
        /// The merge table.
        /// </param>
        /// <param name="orRelation">
        /// if set to <c>true</c> [or relation].
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable TableByAppendingTableOrRelation(UPConfigQueryTable mergeTable, bool orRelation)
        {
            var mergeSubTableDictionary = new Dictionary<string, UPConfigQueryTable>(mergeTable.SubTableDictionary());
            bool mergeTablePossible;
            bool mergedTableFound;

            var isWithout = this.ParentRelation.StartsWith("WITHOUT");
            if (isWithout)
            {
                orRelation = !orRelation;
            }

            var mergedTables = this.GetMergedTables(orRelation, mergeSubTableDictionary, out mergeTablePossible, out mergedTableFound);

            if (mergeSubTableDictionary.Count > 0)
            {
                mergeTablePossible = false;
            }

            var table = this.CreateTable(mergeTablePossible, mergeTable, mergedTableFound, orRelation, mergedTables);

            return table ?? this.CreateTable(orRelation, mergeTable);
        }

        /// <summary>
        /// Tables the by removing fixed conditions for unnamed parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable TableByRemovingFixedConditionsForUnnamedParameters()
        {
            var subTables = new List<UPConfigQueryTable>();
            var hasChangedTable = false;

            if (this.SubTables?.Count > 0)
            {
                foreach (var subTable in this.SubTables)
                {
                    var changedSubTable = subTable.TableByRemovingFixedConditionsForUnnamedParameters();
                    if (changedSubTable != subTable)
                    {
                        hasChangedTable = true;
                        subTables.Add(changedSubTable);
                    }
                    else
                    {
                        subTables.Add(subTable);
                    }
                }
            }

            var hasChangedCondition = false;
            UPConfigQueryCondition changedCondition = null;
            if (this.Condition != null)
            {
                changedCondition = this.Condition.ConditionByRemovingFixedConditionsForUnnamedParameters();
                if (changedCondition != this.Condition)
                {
                    hasChangedCondition = true;
                }
            }

            if (hasChangedTable)
            {
                return new UPConfigQueryTable(
                    this.InfoAreaId,
                    this.LinkId,
                    this.ParentRelation,
                    subTables,
                    changedCondition,
                    this.PropertyConditions,
                    this.Alias);
            }

            if (hasChangedCondition)
            {
                return new UPConfigQueryTable(
                    this.InfoAreaId,
                    this.LinkId,
                    this.ParentRelation,
                    this.SubTables,
                    changedCondition,
                    this.PropertyConditions,
                    this.Alias);
            }

            return this;
        }

        /// <summary>
        /// Tables the by removing mandatory sub tables.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable TableByRemovingMandatorySubTables()
        {
            var subTables = new List<UPConfigQueryTable>();
            var removedTable = false;
            foreach (var subTable in this.SubTables)
            {
                if (subTable.ParentRelation.IndexOf("OPTIONAL", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    removedTable = true;
                }
                else
                {
                    subTables.Add(subTable);
                }
            }

            if (!removedTable)
            {
                return this;
            }

            if (subTables.Count == 0)
            {
                subTables = null;
            }

            return new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                this.ParentRelation,
                subTables,
                this.Condition,
                this.PropertyConditions,
                this.Alias);
        }

        /// <summary>
        /// Tables the by removing root conditions.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable TableByRemovingRootConditions()
        {
            var subTables = new List<UPConfigQueryTable>();
            var removedTable = false;
            foreach (var subTable in this.SubTables)
            {
                if (subTable.InfoAreaId.Equals(this.InfoAreaId) && subTable.LinkId <= 0)
                {
                    removedTable = true;
                }
                else
                {
                    subTables.Add(subTable);
                }
            }

            if (removedTable)
            {
                return new UPConfigQueryTable(
                    this.InfoAreaId,
                    this.LinkId,
                    this.ParentRelation,
                    subTables,
                    null,
                    this.PropertyConditions,
                    this.Alias);
            }

            if (this.Condition != null)
            {
                return new UPConfigQueryTable(
                    this.InfoAreaId,
                    this.LinkId,
                    this.ParentRelation,
                    this.SubTables,
                    null,
                    this.PropertyConditions,
                    this.Alias);
            }

            return this;
        }

        /// <summary>
        /// Tables the by removing sub tables with information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable TableByRemovingSubTablesWithInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            var subTables = new List<UPConfigQueryTable>();
            var removedTable = false;
            if (this.SubTables != null)
            {
                foreach (var subTable in this.SubTables)
                {
                    if (subTable.InfoAreaId.Equals(infoAreaId)
                        && (subTable.LinkId == linkId || (subTable.LinkId <= 0 && linkId <= 0)))
                    {
                        removedTable = true;
                    }
                    else
                    {
                        subTables.Add(subTable);
                    }
                }
            }

            if (!removedTable)
            {
                return this;
            }

            if (subTables.Count == 0)
            {
                subTables = null;
            }

            return new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                this.ParentRelation,
                subTables,
                this.Condition,
                this.PropertyConditions,
                this.Alias);
        }

        /// <summary>
        /// Tables the by removing unbound parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable TableByRemovingUnboundParameters()
        {
            var changedCondition = this.Condition;
            if (this.Condition != null)
            {
                changedCondition = this.Condition.ConditionByRemovingUnboundParameters();
            }

            var subTables = new List<UPConfigQueryTable>();
            var hasChangedTable = false;
            if (this.SubTables?.Count > 0)
            {
                foreach (var subTable in this.SubTables)
                {
                    var changedSubTable = subTable.TableByRemovingUnboundParameters();
                    if (changedSubTable != subTable)
                    {
                        hasChangedTable = true;
                        subTables.Add(changedSubTable);
                    }
                    else
                    {
                        subTables.Add(subTable);
                    }
                }
            }

            if (changedCondition == this.Condition && !hasChangedTable)
            {
                return this;
            }

            if (hasChangedTable)
            {
                return new UPConfigQueryTable(
                    this.InfoAreaId,
                    this.LinkId,
                    this.ParentRelation,
                    subTables,
                    changedCondition,
                    this.PropertyConditions,
                    this.Alias);
            }

            return new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                this.ParentRelation,
                this.SubTables,
                changedCondition,
                this.PropertyConditions,
                this.Alias);
        }

        /// <summary>
        /// Tables the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, UPConfigQueryTable> TableDictionary()
        {
            if (string.IsNullOrWhiteSpace(this.Alias))
            {
                return null;
            }

            var dict = new Dictionary<string, UPConfigQueryTable> { { this.Alias, this } };

            if (this.SubTables == null || this.SubTables.Count == 0)
            {
                return dict;
            }

            foreach (var subTable in this.SubTables)
            {
                var subDict = subTable.TableDictionary();
                if (subDict.Count > 0)
                {
                    dict.Append(subDict);
                }
            }

            return dict;
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
            builder.AppendFormat(
                !string.IsNullOrWhiteSpace(this.ParentRelation)
                    ? $"[{this.ParentRelation} {this.InfoAreaId}"
                    : $"[{this.InfoAreaId}");

            if (this.LinkId > 0)
            {
                builder.AppendFormat($" link #{this.LinkId}");
            }

            if (this.Condition != null)
            {
                builder.AppendFormat($"{Environment.NewLine}{this.Condition}");
            }

            if (this.SubTables != null && this.SubTables.Count > 0)
            {
                var count = this.SubTables.Count;
                builder.Append(" subTables:[");
                for (var i = 0; i < count; i++)
                {
                    builder.AppendFormat(
                        i == 0
                            ? $"{Environment.NewLine}{this.SubTables[i]}"
                            : $",{Environment.NewLine}{this.SubTables[i]}");
                }

                builder.Append("]");
            }

            builder.AppendFormat("]");

            return builder.ToString();
        }

        private List<UPConfigQueryTable> GetMergedTables(bool orRelation, Dictionary<string, UPConfigQueryTable> mergeSubTableDictionary, out bool mergeTablePossible, out bool mergedTableFound)
        {
            mergeTablePossible = true;
            mergedTableFound = false;
            var mergedTables = new List<UPConfigQueryTable>();
            var mySubTableDictionary = this.SubTableDictionary();
            foreach (var key in mySubTableDictionary.Keys)
            {
                var subTable = mySubTableDictionary[key];
                var mergeSubTable = mergeSubTableDictionary[key];
                if (mergeSubTable == null)
                {
                    mergeTablePossible = false;
                    break;
                }

                if (subTable != mergeSubTable)
                {
                    var mergedTable = subTable.TableByAppendingTableOrRelation(mergeSubTable, orRelation);
                    if (mergedTable != subTable)
                    {
                        if (mergedTableFound)
                        {
                            mergeTablePossible = false;
                            break;
                        }

                        mergedTableFound = true;
                    }

                    mergedTables.Add(mergedTable);
                }
                else
                {
                    mergedTables.Add(subTable);
                }

                mergeSubTableDictionary.Remove(key);
            }

            return mergedTables;
        }

        private UPConfigQueryTable CreateTable(bool mergeTablePossible, UPConfigQueryTable mergeTable, bool mergedTableFound, bool orRelation, List<UPConfigQueryTable> mergedTables)
        {
            UPConfigQueryCondition mergedCondition = null;
            if (this.Condition != null || mergeTable.Condition != null)
            {
                if (this.Condition == null || mergeTable.Condition == null)
                {
                    if (mergedTableFound)
                    {
                        mergeTablePossible = false;
                    }
                }
                else
                {
                    mergedCondition = this.Condition.ConditionByAppendingConditionOrRelation(
                        mergeTable.Condition,
                        orRelation);

                    if (mergedCondition != this.Condition && mergedTableFound)
                    {
                        mergeTablePossible = false;
                    }
                }
            }

            if (mergeTablePossible)
            {
                if (this.Condition == mergedCondition && !mergedTableFound)
                {
                    return this;
                }

                return new UPConfigQueryTable(
                    this.InfoAreaId,
                    this.LinkId,
                    this.ParentRelation,
                    mergedTables,
                    mergedCondition,
                    this.PropertyConditions,
                    this.Alias);
            }

            return null;
        }

        private UPConfigQueryTable CreateTable(bool orRelation, UPConfigQueryTable mergeTable)
        {
            string subTableAlias = null;
            string otherTableAlias = null;
            if (!string.IsNullOrWhiteSpace(this.Alias))
            {
                subTableAlias = $"{this.Alias}_{this.InfoAreaId}m1";
                otherTableAlias = $"{this.Alias}_{this.InfoAreaId}m2";
            }

            var newSubTable = new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                orRelation ? "HAVINGOPTIONAL" : "HAVING",
                this.SubTables,
                this.Condition,
                this.PropertyConditions,
                subTableAlias);

            var otherTable = new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                newSubTable.ParentRelation,
                mergeTable.SubTables,
                mergeTable.Condition,
                mergeTable.PropertyConditions,
                otherTableAlias);

            return new UPConfigQueryTable(
                this.InfoAreaId,
                this.LinkId,
                this.ParentRelation,
                new List<UPConfigQueryTable> { newSubTable, otherTable },
                null,
                this.PropertyConditions,
                this.Alias);
        }
    }
}
