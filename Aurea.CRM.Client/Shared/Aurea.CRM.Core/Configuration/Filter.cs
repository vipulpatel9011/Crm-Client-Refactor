// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Filter.cs" company="Aurea Software Gmbh">
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
//   Filter cnfigurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Filter cnfigurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.UPConfigQueryFilterBase" />
    public class UPConfigFilter : UPConfigQueryFilterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilter"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigFilter(List<object> definition)
            : base(definition, false)
        {
            this.InfoAreaId = (string)definition[2];
            this.DisplayName = (string)definition[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootTable">
        /// The root table.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        public UPConfigFilter(string name, UPConfigQueryTable rootTable, string infoAreaId, string displayName)
            : base(name, rootTable)
        {
            this.InfoAreaId = infoAreaId;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootTable">
        /// The root table.
        /// </param>
        public UPConfigFilter(string name, UPConfigQueryTable rootTable)
            : base(name, rootTable)
        {
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Checks the value information area identifier field identifier.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFilterCheckResult"/>.
        /// </returns>
        public UPConfigFilterCheckResult CheckValueInfoAreaIdFieldId(string value, string infoAreaId, int fieldId)
        {
            UPConfigQueryTable table = null;
            if (this.InfoAreaId == null || this.RootTable.InfoAreaId == infoAreaId)
            {
                table = this.RootTable;
            }
            else
            {
                var count = this.RootTable.NumberOfSubTables;
                for (var i = 0; i < count; i++)
                {
                    var subTable = this.RootTable.SubTableAtIndex(i);
                    if (subTable.InfoAreaId == infoAreaId)
                    {
                        table = subTable;
                        break;
                    }
                }
            }

            return table?.CheckValueFieldId(value, fieldId);
        }

        /// <summary>
        /// Copies the with root.
        /// </summary>
        /// <param name="newRoot">
        /// The new root.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public override UPConfigQueryFilterBase CopyWithRoot(UPConfigQueryTable newRoot)
        {
            return new UPConfigFilter(this.UnitName, newRoot, this.InfoAreaId, this.DisplayName);
        }

        /// <summary>
        /// Fieldses from condition fields for information area identifier with links.
        /// </summary>
        /// <param name="_infoAreaId">
        /// The _info area identifier.
        /// </param>
        /// <param name="withLinks">
        /// if set to <c>true</c> [with links].
        /// </param>
        /// <returns>
        /// Dictionary
        /// </returns>
        public Dictionary<string, UPCRMField> FieldsFromConditionFieldsForInfoAreaId(string _infoAreaId, bool withLinks)
        {
            var fieldDictionary = new Dictionary<string, UPCRMField>();
            this.RootTable.FillDictionary(fieldDictionary, _infoAreaId, withLinks);
            return fieldDictionary;
        }

        /// <summary>
        /// Filters the by appending or filter.
        /// </summary>
        /// <param name="mergeFilter">
        /// The merge filter.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByAppendingOrFilter(UPConfigFilter mergeFilter)
        {
            var rootTable = this.RootTable.TableByAppendingTableOrRelation(mergeFilter.RootTable, true);
            if (this.RootTable != rootTable)
            {
                return new UPConfigFilter(this.UnitName, rootTable);
            }

            return this;
        }

        /// <summary>
        /// Filters the by applying array of value dictionaries parameters.
        /// </summary>
        /// <param name="valueDictionaryArray">
        /// The value dictionary array.
        /// </param>
        /// <param name="valueDictionaryForAll">
        /// The value dictionary for all.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByApplyingArrayOfValueDictionariesParameters(
            List<Dictionary<string, object>> valueDictionaryArray,
            Dictionary<string, object> valueDictionaryForAll)
        {
            if (valueDictionaryArray.Count == 0)
            {
                return null;
            }

            UPConfigFilter resultFilter = null;
            var emptyDictionary = false;
            foreach (var valueDictionary in valueDictionaryArray)
            {
                Dictionary<string, object> currentDictionary;
                if (valueDictionary.Any())
                {
                    if (emptyDictionary)
                    {
                        continue;
                    }

                    emptyDictionary = true;
                    currentDictionary = valueDictionaryForAll;
                }
                else if (valueDictionaryForAll.Any())
                {
                    var dict = new Dictionary<string, object>(valueDictionaryForAll);
                    dict.Append(valueDictionary);
                    currentDictionary = dict;
                }
                else
                {
                    currentDictionary = valueDictionary;
                }

                var currentFilter = this.FilterByApplyingValueDictionary(currentDictionary);
                resultFilter = resultFilter == null
                                   ? currentFilter
                                   : resultFilter.FilterByAppendingOrFilter(currentFilter);
            }

            return resultFilter;
        }

        /// <summary>
        /// Filters the by applying default replacements.
        /// </summary>
        /// <returns>filter configurations</returns>
        public UPConfigFilter FilterByApplyingDefaultReplacements()
        {
            return (UPConfigFilter)this.QueryByApplyingReplacements(UPConditionValueReplacement.DefaultParameters);
        }

        /// <summary>
        /// Filters the by applying filter parameter.
        /// </summary>
        /// <param name="filterParameter">
        /// The filter parameter.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByApplyingFilterParameter(UPConfigFilterParameter filterParameter)
        {
            return (UPConfigFilter)this.QueryByApplyingFilterParameter(filterParameter);
        }

        /// <summary>
        /// Filters the by applying filter parameters.
        /// </summary>
        /// <param name="filterParameters">
        /// The filter parameters.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByApplyingFilterParameters(UPConfigFilterParameters filterParameters)
        {
            return (UPConfigFilter)this.QueryByApplyingFilterParameters(filterParameters);
        }

        /// <summary>
        /// Filters the by applying replacements.
        /// </summary>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByApplyingReplacements(UPConditionValueReplacement replacements)
        {
            return (UPConfigFilter)this.QueryByApplyingReplacements(replacements);
        }

        /// <summary>
        /// Filters the by applying value dictionary.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByApplyingValueDictionary(Dictionary<string, object> valueDictionary)
        {
            return this.FilterByApplyingValueDictionaryDefaults(valueDictionary, false);
        }

        /// <summary>
        /// Filters the by applying value dictionary defaults.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <param name="defaults">
        /// if set to <c>true</c> [defaults].
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByApplyingValueDictionaryDefaults(Dictionary<string, object> valueDictionary, bool defaults)
        {
            if (!defaults)
            {
                return (UPConfigFilter)this.QueryByApplyingValueDictionary(valueDictionary);
            }

            var replacements = UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(valueDictionary, true);
            return (UPConfigFilter)this.QueryByApplyingReplacements(replacements);
        }

        /// <summary>
        /// Filters the by removing fixed conditions for unnamed parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigFilter"/>.
        /// </returns>
        public UPConfigFilter FilterByRemovingFixedConditionsForUnnamedParameters()
        {
            var rootTable = this.RootTable.TableByRemovingFixedConditionsForUnnamedParameters();
            return rootTable != this.RootTable ? new UPConfigFilter(this.UnitName, rootTable) : this;
        }

        /// <summary>
        /// Filters the by removing mandatory sub tables.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigFilter"/>.
        /// </returns>
        public UPConfigFilter FilterByRemovingMandatorySubTables()
        {
            var rootTable = this.RootTable.TableByRemovingMandatorySubTables();
            return rootTable != this.RootTable ? new UPConfigFilter(this.UnitName, rootTable) : this;
        }

        /// <summary>
        /// Filters the by removing root conditions.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigFilter"/>.
        /// </returns>
        public UPConfigFilter FilterByRemovingRootConditions()
        {
            var rootTable = this.RootTable.TableByRemovingRootConditions();
            return rootTable != this.RootTable ? new UPConfigFilter(this.UnitName, rootTable) : this;
        }

        /// <summary>
        /// Filters the by removing sub tables with information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// filter configurations
        /// </returns>
        public UPConfigFilter FilterByRemovingSubTablesWithInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            var rootTable = this.RootTable.TableByRemovingSubTablesWithInfoAreaIdLinkId(infoAreaId, linkId);
            return rootTable != this.RootTable ? new UPConfigFilter(this.UnitName, rootTable) : this;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[super: {base.ToString()} infoAreaId: {this.InfoAreaId}, displayname: {this.DisplayName}]";
        }
    }

    /// <summary>
    /// Filter check result
    /// </summary>
    public class UPConfigFilterCheckResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilterCheckResult"/> class.
        /// </summary>
        /// <param name="failingCondition">
        /// The failing condition.
        /// </param>
        public UPConfigFilterCheckResult(UPConfigQueryCondition failingCondition)
        {
            this.FailingCondition = failingCondition;
            var errorCondition = this.FailingCondition.PropertyConditions.ValueOrDefault("Error");
            this.ErrorKey = errorCondition?.FirstValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilterCheckResult"/> class.
        /// </summary>
        /// <param name="filterCheckResult">
        /// The filter check result.
        /// </param>
        /// <param name="parentCondition">
        /// The parent condition.
        /// </param>
        public UPConfigFilterCheckResult(UPConfigFilterCheckResult filterCheckResult, UPConfigQueryCondition parentCondition)
        {
            this.FailingCondition = filterCheckResult.FailingCondition;
            this.ErrorKey = filterCheckResult.ErrorKey;
            this.ChildResults = filterCheckResult.ChildResults;

            if (string.IsNullOrEmpty(this.ErrorKey))
            {
                var errorCondition = parentCondition.PropertyConditions.ValueOrDefault("Error");
                this.ErrorKey = errorCondition?.FirstValue;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigFilterCheckResult"/> class.
        /// </summary>
        /// <param name="checkResults">
        /// The check results.
        /// </param>
        /// <param name="parentCondition">
        /// The parent condition.
        /// </param>
        public UPConfigFilterCheckResult(List<UPConfigFilterCheckResult> checkResults, UPConfigQueryCondition parentCondition)
            : this(parentCondition)
        {
            this.ChildResults = checkResults;
        }

        /// <summary>
        /// Gets the child results.
        /// </summary>
        /// <value>
        /// The child results.
        /// </value>
        public List<UPConfigFilterCheckResult> ChildResults { get; private set; }

        /// <summary>
        /// Gets the error key.
        /// </summary>
        /// <value>
        /// The error key.
        /// </value>
        public string ErrorKey { get; private set; }

        /// <summary>
        /// Gets the failing condition.
        /// </summary>
        /// <value>
        /// The failing condition.
        /// </value>
        public UPConfigQueryCondition FailingCondition { get; private set; }

        /// <summary>
        /// Gets the field keys.
        /// </summary>
        /// <value>
        /// The field keys.
        /// </value>
        public List<string> FieldKeys => this.FieldKeyDictionary().Keys.ToList();

        /// <summary>
        /// Fields the key dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, UPConfigQueryCondition> FieldKeyDictionary()
        {
            Dictionary<string, UPConfigQueryCondition> fieldKeys = null;
            if (!string.IsNullOrWhiteSpace(this.FailingCondition.InfoAreaId) && this.FailingCondition.FieldId >= 0)
            {
                string fieldKey = $"{this.FailingCondition.InfoAreaId}.{this.FailingCondition.FieldId}";
                fieldKeys = new Dictionary<string, UPConfigQueryCondition> { { fieldKey, this.FailingCondition } };
            }

            if (this.ChildResults != null)
            {
                foreach (var result in this.ChildResults)
                {
                    var childFieldKeys = result.FieldKeyDictionary();
                    if (fieldKeys == null && childFieldKeys.Count > 0)
                    {
                        fieldKeys = new Dictionary<string, UPConfigQueryCondition>(childFieldKeys);
                    }
                    else
                    {
                        foreach (var childFieldKey in childFieldKeys.Keys)
                        {
                            if (fieldKeys == null)
                            {
                                fieldKeys = new Dictionary<string, UPConfigQueryCondition>();
                            }

                            if (fieldKeys.ContainsKey(childFieldKey) && fieldKeys[childFieldKey] == null)
                            {
                                fieldKeys[childFieldKey] = childFieldKeys[childFieldKey];
                            }
                        }
                    }
                }
            }

            return fieldKeys;
        }
    }

    /// <summary>
    /// Special filter configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.UPConfigFilter" />
    public class UPConfigSpecialFilter : UPConfigFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigSpecialFilter"/> class.
        /// </summary>
        /// <param name="specialName">
        /// Name of the _special.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public UPConfigSpecialFilter(string specialName, object parameter)
            : base(specialName, null)
        {
            this.Parameter = parameter;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public object Parameter { get; private set; }
    }
}
