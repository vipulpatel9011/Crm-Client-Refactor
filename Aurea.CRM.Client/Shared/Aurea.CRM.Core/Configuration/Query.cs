// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Query.cs" company="Aurea Software Gmbh">
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
//   Query configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Query;
    using Extensions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Query configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.UPConfigQueryFilterBase" />
    public class UPConfigQuery : UPConfigQueryFilterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQuery"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigQuery(List<object> definition)
            : base(definition, true)
        {
            var fieldTempArray = definition.Count > 4 ? (definition[4] as JArray)?.ToObject<List<object>>() : null;
            var sortTempFieldArray = definition.Count > 5 ? (definition[5] as JArray)?.ToObject<List<object>>() : null;
            var tableDictionary = this.RootTable.TableDictionary();

            var fieldArray = new List<object>();
            var sortFieldArray = new List<object>();

            foreach (var element in fieldTempArray)
            {
                if (!(element is JArray))
                {
                    fieldArray.Add(element);
                }
                else
                {
                    var castedElement = ((JArray)element).ToObject<List<object>>();
                    fieldArray.Add(castedElement);
                }
            }

            foreach (var element in sortFieldArray)
            {
                if (!(element is JArray))
                {
                    sortFieldArray.Add(element);
                }
                else
                {
                    var castedElement = ((JArray)element).ToObject<List<object>>();
                    sortFieldArray.Add(castedElement);
                }
            }

            if (fieldArray != null)
            {
                var queryFields = new List<UPConfigQueryField>(fieldArray.Count);
                foreach (List<object> field in fieldArray)
                {
                    var alias = field.Count > 1 ? field[1] as string : null;
                    var qt = tableDictionary.ValueOrDefault(alias);
                    if (qt != null)
                    {
                        var field0 = 0;
                        var field2 = 0;

                        if (field.Count > 2)
                        {
                            field0 = field[0].ToInt();
                            field2 = field[2].ToInt();
                            var queryField = new UPConfigQueryField(field0, qt, field2);

                            queryFields.Add(queryField);
                        }
                    }
                }

                this.QueryFields = queryFields;
            }

            if (sortFieldArray == null)
            {
                return;
            }

            var sortFields = new List<UPConfigQuerySortField>(sortFieldArray.Count);
            foreach (var field in sortFieldArray)
            {
                var sortfield = (field as JArray)?.ToObject<List<object>>();
                var alias = sortfield.Count > 1 ? sortfield[1] as string : null;
                var qt = tableDictionary[alias];
                if (qt != null)
                {
                    var sortfield0 = 0;
                    var sortfield2 = 0;

                    if (sortfield.Count > 2)
                    {
                        sortfield0 = sortfield[0].ToInt();
                        sortfield2 = sortfield[2].ToInt();
                        var querySortField = new UPConfigQuerySortField(sortfield0, qt, sortfield2);

                        sortFields.Add(querySortField);
                    }
                }
            }

            this.SortFields = sortFields;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigQuery"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="rootTable">
        /// The root table.
        /// </param>
        /// <param name="queryFields">
        /// The query fields.
        /// </param>
        /// <param name="sortFields">
        /// The sort fields.
        /// </param>
        public UPConfigQuery(
            string name,
            UPConfigQueryTable rootTable,
            List<UPConfigQueryField> queryFields,
            List<UPConfigQuerySortField> sortFields)
            : base(name, rootTable)
        {
            this.QueryFields = queryFields;
            this.SortFields = sortFields;
        }

        /// <summary>
        /// Gets the query fields.
        /// </summary>
        /// <value>
        /// The query fields.
        /// </value>
        public List<UPConfigQueryField> QueryFields { get; }

        /// <summary>
        /// Gets the sort fields.
        /// </summary>
        /// <value>
        /// The sort fields.
        /// </value>
        public List<UPConfigQuerySortField> SortFields { get; }

        /// <summary>
        /// Copies the with root.
        /// </summary>
        /// <param name="newRoot">
        /// The new root.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryFilterBase"/>.
        /// </returns>
        public override UPConfigQueryFilterBase CopyWithRoot(UPConfigQueryTable newRoot)
        {
            return new UPConfigQuery(this.UnitName, newRoot, this.QueryFields, this.SortFields);
        }

        /// <summary>
        /// Queries the by applying value dictionary defaults.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="defaults">
        /// if set to <c>true</c> [defaults].
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQuery"/>.
        /// </returns>
        public UPConfigQuery QueryByApplyingValueDictionaryDefaults(
            Dictionary<string, object> parameters,
            bool defaults)
        {
            if (!defaults)
            {
                return (UPConfigQuery)this.QueryByApplyingValueDictionary(parameters);
            }

            var replacements = UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(parameters, true);
            return (UPConfigQuery)this.QueryByApplyingReplacements(replacements);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"name={this.UnitName}, fields={this.QueryFields}, tables={this.RootTable}, sort={this.SortFields}";
        }
    }
}
