// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterBasedDecision.cs" company="Aurea Software Gmbh">
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
//   The upcrm filter based decision.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections;
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Extensions;

    /// <summary>
    /// The upcrm filter based decision.
    /// </summary>
    public class UPCRMFilterBasedDecision
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFilterBasedDecision"/> class.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public UPCRMFilterBasedDecision(UPConfigFilter filter)
        {
            this.Filter = filter;
            this.FieldDictionary = filter.FieldsFromConditionFieldsForInfoAreaId(filter.InfoAreaId, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMFilterBasedDecision"/> class.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="filterParameters">
        /// The filter parameters.
        /// </param>
        public UPCRMFilterBasedDecision(UPConfigFilter filter, Dictionary<string, object> filterParameters)
            : this(filter.FilterByApplyingValueDictionaryDefaults(filterParameters, true))
        {
        }

        /// <summary>
        /// Gets the field dictionary.
        /// </summary>
        public Dictionary<string, UPCRMField> FieldDictionary { get; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public UPConfigFilter Filter { get; }

        /// <summary>
        /// Gets the result field map.
        /// </summary>
        public Dictionary<string, int> ResultFieldMap { get; private set; }

        /// <summary>
        /// The button for result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigButton"/>.
        /// </returns>
        public UPConfigButton ButtonForResultRow(UPCRMResultRow row)
        {
            UPConfigQueryTable matchingTable = this.QueryTableForResultRow(row);

            if (matchingTable != null)
            {
                UPConfigQueryCondition propertyCondition = matchingTable.PropertyConditions.ValueOrDefault("DefaultAction");

                if (!string.IsNullOrEmpty(propertyCondition?.FirstValue))
                {
                    Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(propertyCondition.FirstValue);
                    return new UPConfigButton(menu.DisplayName, menu.ImageName, menu.ViewReference);
                }

                propertyCondition = matchingTable.PropertyConditions.ValueOrDefault("DefaultButtonAction");

                if (!string.IsNullOrEmpty(propertyCondition?.FirstValue))
                {
                    return ConfigurationUnitStore.DefaultStore.ButtonByName(propertyCondition.FirstValue);
                }
            }

            return null;
        }

        /// <summary>
        /// The buttons for result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public List<UPConfigButton> ButtonsForResultRow(UPCRMResultRow row)
        {
            UPConfigQueryTable matchingTable = this.QueryTableForResultRow(row);

            if (matchingTable != null)
            {
                List<UPConfigButton> matchingButtons = new List<UPConfigButton>();

                UPConfigQueryCondition propertyCondition = matchingTable.PropertyConditions[@"Action"];

                if (propertyCondition != null)
                {
                    IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

                    foreach (string menuName in propertyCondition.FieldValues)
                    {
                        Menu menu = configStore.MenuByName(menuName);
                        if (menu != null)
                        {
                            UPConfigButton button = new UPConfigButton(
                                menu.DisplayName,
                                menu.ImageName,
                                menu.ViewReference);
                            matchingButtons.Add(button);
                        }
                        else if (menuName.StartsWith(@"Button:"))
                        {
                            UPConfigButton button = configStore.ButtonByName(menuName.Substring(7));
                            if (button != null)
                            {
                                matchingButtons.Add(button);
                            }
                        }
                    }
                }

                propertyCondition = matchingTable.PropertyConditions.ValueOrDefault("ButtonAction");

                if (propertyCondition != null)
                {
                    foreach (string buttonName in propertyCondition.FieldValues)
                    {
                        UPConfigButton button = ConfigurationUnitStore.DefaultStore.ButtonByName(buttonName);

                        if (button != null)
                        {
                            matchingButtons.Add(button);
                        }
                    }
                }

                return matchingButtons.Count > 0 ? matchingButtons : null;
            }

            return null;
        }

        /// <summary>
        /// The properties for query table.
        /// </summary>
        /// <param name="queryTable">
        /// The query table.
        /// </param>
        /// <param name="multiValue">
        /// The multi value.
        /// </param>
        /// <returns>
        /// Dictionary
        /// </returns>
        public Dictionary<string, object> PropertiesForQueryTable(UPConfigQueryTable queryTable, bool multiValue)
        {
            if (queryTable != null && queryTable.PropertyConditions.Count > 0)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>(queryTable.PropertyConditions.Count);

                if (multiValue)
                {
                    foreach (string condKey in queryTable.PropertyConditions.Keys)
                    {
                        UPConfigQueryCondition cond = queryTable.PropertyConditions[condKey];
                        dict.Add(condKey, cond);
                    }
                }
                else
                {
                    foreach (string condKey in queryTable.PropertyConditions.Keys)
                    {
                        UPConfigQueryCondition cond = queryTable.PropertyConditions[condKey];

                        if (cond.FieldValues.Count == 1)
                        {
                            dict.Add(condKey, cond.FieldValues[0]);
                        }
                    }
                }

                return dict;
            }

            return null;
        }

        /// <summary>
        /// The properties for result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="multiValue">
        /// The multi value.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> PropertiesForResultRow(UPCRMResultRow row, bool multiValue)
        {
            UPConfigQueryTable matchingTable = this.QueryTableForResultRow(row);
            return this.PropertiesForQueryTable(matchingTable, multiValue);
        }

        /// <summary>
        /// The query table for result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigQueryTable"/>.
        /// </returns>
        public UPConfigQueryTable QueryTableForResultRow(UPCRMResultRow row)
        {
            var resultFieldMap = this.ResultFieldMap ?? this.ResultFeldMapFromCrmQuery(row.Result.MetaInfo);

            if (resultFieldMap == null)
            {
                return null;
            }

            Dictionary<string, object> valueDictionary = new Dictionary<string, object>(resultFieldMap.Count);

            foreach (string key in resultFieldMap.Keys)
            {
                int position = resultFieldMap[key];

                var value = position < 0 ? string.Empty : row.RawValueAtIndex(position);

                valueDictionary.Add(key, value);
            }

            UPConfigQueryTable foundTable =
                this.Filter.RootTable.QueryTableForValueDictionaryWithSubInfoAreas(valueDictionary, true);

            if (foundTable == null)
            {
                return this.Filter.RootTable;
            }

            return foundTable;
        }

        /// <summary>
        /// The use crm query.
        /// </summary>
        /// <param name="crmQuery">
        /// The crm query.
        /// </param>
        public void UseCrmQuery(UPContainerMetaInfo crmQuery)
        {
            this.ResultFieldMap = this.ResultFeldMapFromCrmQuery(crmQuery);
        }

        /// <summary>
        /// The view reference for result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="ViewReference"/>.
        /// </returns>
        public ViewReference ViewReferenceForResultRow(UPCRMResultRow row)
        {
            return this.ButtonForResultRow(row)?.ViewReference;
        }

        /// <summary>
        /// The result feld map from crm query.
        /// </summary>
        /// <param name="crmQuery">
        /// The crm query.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        Dictionary<string, int> ResultFeldMapFromCrmQuery(UPContainerMetaInfo crmQuery)
        {
            if (crmQuery == null)
            {
                return null;
            }

            var resultFieldMap = new Dictionary<string, int>(this.FieldDictionary.Count);

            foreach (var fieldKey in this.FieldDictionary)
            {
                int pos = crmQuery.PositionForField(this.FieldDictionary[fieldKey.Key]);
                resultFieldMap.Add(fieldKey.Key, pos);
            }

            return resultFieldMap;
        }
    }
}
