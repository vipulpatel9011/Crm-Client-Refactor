// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterBase.cs" company="Aurea Software Gmbh">
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
//   Filter base configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.Query;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Filter base configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigQueryFilterBase : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryFilterBase"/> class.
        /// </summary>
        /// <param name="defArray">
        /// The definition array.
        /// </param>
        /// <param name="isQuery">
        /// if set to <c>true</c> [is query].
        /// </param>
        public UPConfigQueryFilterBase(List<object> defArray, bool isQuery)
        {
            this.UnitName = (string)defArray[0];
            var rootTableDef = (defArray[3] as JArray)?.ToObject<List<object>>();
            if (rootTableDef != null)
            {
                this.RootTable = new UPConfigQueryTable(rootTableDef, isQuery);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQueryFilterBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootTable">
        /// The root table.
        /// </param>
        public UPConfigQueryFilterBase(string name, UPConfigQueryTable rootTable)
        {
            this.UnitName = name;
            this.RootTable = rootTable;
        }

        /// <summary>
        /// Gets a value indicating whether [needs location].
        /// </summary>
        /// <value>
        /// <c>true</c> if [needs location]; otherwise, <c>false</c>.
        /// </value>
        public bool NeedsLocation => this.RootTable.NeedsLocation;

        /// <summary>
        /// Gets or sets the root table.
        /// </summary>
        /// <value>
        /// The root table.
        /// </value>
        public UPConfigQueryTable RootTable { get; protected set; }

        /// <summary>
        /// Adds the flat conditions.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="flatConditions">
        /// The flat conditions.
        /// </param>
        public virtual void AddFlatConditions(
            UPConfigQueryCondition condition,
            List<UPConfigQueryCondition> flatConditions)
        {
            if (condition.SubConditions.Count == 0)
            {
                flatConditions.Add(condition);
            }
            else
            {
                foreach (UPConfigQueryCondition subCondition in condition.SubConditions)
                {
                    this.AddFlatConditions(subCondition, flatConditions);
                }
            }
        }

        /// <summary>
        /// Conditions the with.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <param name="includeSubtables">
        /// if set to <c>true</c> [include subtables].
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public virtual UPConfigQueryCondition ConditionWith(string functionName, bool includeSubtables)
        {
            var conditions = this.RootTable.QueryConditions(functionName, includeSubtables);
            if (conditions != null && conditions.Count > 0)
            {
                return conditions[0];
            }

            return null;
        }

        /// <summary>
        /// Conditions the with.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryCondition"/>.
        /// </returns>
        public virtual UPConfigQueryCondition ConditionWith(string functionName)
        {
            return this.ConditionWith(functionName, false);
        }

        /// <summary>
        /// Copies the with root.
        /// </summary>
        /// <param name="newRoot">
        /// The new root.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryFilterBase"/>.
        /// </returns>
        public virtual UPConfigQueryFilterBase CopyWithRoot(UPConfigQueryTable newRoot)
        {
            return new UPConfigQueryFilterBase(this.UnitName, this.RootTable);
        }

        /// <summary>
        /// Fieldses the with conditions.
        /// </summary>
        /// <param name="includeSubtables">
        /// if set to <c>true</c> [include subtables].
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public virtual Dictionary<string, object> FieldsWithConditions(bool includeSubtables)
        {
            return this.FieldsWithValues(includeSubtables, false);
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
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public virtual Dictionary<string, object> FieldsWithValues(bool includeSubtables, bool flat)
        {
            return this.RootTable.FieldsWithValues(includeSubtables, flat, true);
        }

        /// <summary>
        /// Flats the conditions with.
        /// </summary>
        /// <param name="functionName">
        /// Name of the function.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<UPConfigQueryCondition> FlatConditionsWith(string functionName)
        {
            var flatConditions = new List<UPConfigQueryCondition>();

            var conditions = this.RootTable.QueryConditions(functionName, true);
            if (conditions != null)
            {
                foreach (var cond in conditions)
                {
                    this.AddFlatConditions(cond, flatConditions);
                }
            }

            return flatConditions;
        }

        /// <summary>
        /// Parameterses this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="UPConfigFilterParameters"/>.
        /// </returns>
        public virtual UPConfigFilterParameters Parameters()
        {
            var parameters = new UPConfigFilterParameters();
            this.RootTable.AddParameters(parameters);
            return parameters.NumberOfParameters == 0 ? null : parameters;
        }

        /// <summary>
        /// Queries the by applying filter parameter.
        /// </summary>
        /// <param name="filterParameter">
        /// The filter parameter.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryFilterBase"/>.
        /// </returns>
        public virtual UPConfigQueryFilterBase QueryByApplyingFilterParameter(UPConfigFilterParameter filterParameter)
        {
            return this.QueryByApplyingReplacements(new UPConditionValueReplacement(filterParameter));
        }

        /// <summary>
        /// Queries the by applying filter parameters.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryFilterBase"/>.
        /// </returns>
        public virtual UPConfigQueryFilterBase QueryByApplyingFilterParameters(UPConfigFilterParameters parameters)
        {
            return this.QueryByApplyingReplacements(new UPConditionValueReplacement(parameters));
        }

        /// <summary>
        /// Queries the by applying replacements.
        /// </summary>
        /// <param name="replacements">
        /// The replacements.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryFilterBase"/>.
        /// </returns>
        public virtual UPConfigQueryFilterBase QueryByApplyingReplacements(UPConditionValueReplacement replacements)
        {
            var newRoot = this.RootTable.QueryTableByApplyingReplacements(replacements);
            if (replacements.RemoveUnboundParameters())
            {
                newRoot = newRoot.TableByRemovingUnboundParameters();
            }

            if (newRoot == null)
            {
                return null;
            }

            if (newRoot == this.RootTable)
            {
                return this;
            }

            return this.CopyWithRoot(newRoot);
        }

        /// <summary>
        /// Queries the by applying value dictionary.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryFilterBase"/>.
        /// </returns>
        public virtual UPConfigQueryFilterBase QueryByApplyingValueDictionary(
            Dictionary<string, object> valueDictionary)
        {
            if (valueDictionary == null)
            {
                return this;
            }

            var replacements = new UPConditionValueReplacement(valueDictionary);
            return this.QueryByApplyingReplacements(replacements);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.RootTable.ToString();
        }
    }
}
