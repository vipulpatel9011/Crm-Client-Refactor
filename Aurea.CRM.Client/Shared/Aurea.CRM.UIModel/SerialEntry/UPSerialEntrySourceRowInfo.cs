// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntrySourceRowInfo.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntrySourceRowInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSerialEntrySourceRowInfo
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryInfo" />
    public class UPSerialEntrySourceRowInfo : UPSerialEntryInfo
    {
        private Dictionary<string, UPSerialEntryInfoResultFromCRMResult> cachedResults;

        /// <summary>
        /// Gets the search and list.
        /// </summary>
        /// <value>
        /// The search and list.
        /// </value>
        public SearchAndList SearchAndList { get; private set; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public UPConfigFilter Filter { get; private set; }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets the maximum results.
        /// </summary>
        /// <value>
        /// The maximum results.
        /// </value>
        public int MaxResults { get; private set; }

        /// <summary>
        /// Gets the column defs.
        /// </summary>
        /// <value>
        /// The column defs.
        /// </value>
        public List<UPConfigFieldControlField> ColumnDefs { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [ignore source record].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore source record]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreSourceRecord { get; private set; }

        /// <summary>
        /// Creates the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="serialEntry">The serial entry.</param>
        /// <returns></returns>
        public static UPSerialEntrySourceRowInfo Create(Dictionary<string, string> definition, UPSerialEntry serialEntry)
        {
            string name = definition.ValueOrDefault("name");
            if (name == null)
            {
                return null;
            }

            var newObj = new UPSerialEntrySourceRowInfo(name, serialEntry);
            newObj.VerticalRows = false;

            string configName = definition.ValueOrDefault("configName") ?? name;
            string parameters = definition.ValueOrDefault("maxResults");
            newObj.MaxResults = !string.IsNullOrEmpty(parameters) ? Convert.ToInt32(parameters) : 10;
            newObj.IgnoreSourceRecord = Convert.ToInt32(definition.ValueOrDefault("NoLink")) != 0;

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            newObj.SearchAndList = configStore.SearchAndListByName(configName);
            if (newObj.SearchAndList == null)
            {
                return null;
            }

            newObj.FieldControl = configStore.FieldControlByNameFromGroup("List", newObj.SearchAndList.FieldGroupName);
            if (newObj.FieldControl == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(newObj.SearchAndList.FilterName))
            {
                newObj.Filter = configStore.FilterByName(newObj.SearchAndList.FilterName);
                if (!string.IsNullOrEmpty(newObj.Filter?.DisplayName))
                {
                    newObj.Label = newObj.Filter.DisplayName;
                }
            }

            List<string> columnNameArray = new List<string>();
            List<UPConfigFieldControlField> columnDefArray = new List<UPConfigFieldControlField>();
            foreach (FieldControlTab tab in newObj.FieldControl.Tabs)
            {
                foreach (UPConfigFieldControlField field in tab.Fields)
                {
                    columnDefArray.Add(field);
                    columnNameArray.Add(field.Label);
                }
            }

            newObj.ColumnDefs = columnDefArray;
            newObj.ColumnNames = columnNameArray;

            return newObj;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntrySourceRowInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="serialEntry">The serial entry.</param>
        private UPSerialEntrySourceRowInfo(string name, UPSerialEntry serialEntry)
            : base(name, serialEntry)
        {
        }

        /// <summary>
        /// Results for row.
        /// </summary>
        /// <param name="serialEntryRow">The serial entry row.</param>
        /// <returns></returns>
        public override UPSerialEntryInfoResult ResultForRow(UPSERow serialEntryRow)
        {
            string sourceRecordIdentification = StringExtensions.InfoAreaIdRecordId(serialEntryRow.SerialEntry.SourceInfoAreaId, serialEntryRow.RowRecordId);
            UPSerialEntryInfoResultFromCRMResult result = this.cachedResults.ValueOrDefault(sourceRecordIdentification);
            if (result != null)
            {
                return result;
            }

            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(this.FieldControl);
            if (!this.IgnoreSourceRecord)
            {
                crmQuery.SetLinkRecordIdentification(sourceRecordIdentification);
            }

            if (this.MaxResults > 0)
            {
                crmQuery.MaxResults = this.MaxResults;
            }

            if (this.Filter != null)
            {
                Dictionary<string, object> parameterValues = serialEntryRow.SourceFunctionValues();
                if (this.SerialEntry.InitialFieldValuesForDestination.Count > 0)
                {
                    if (parameterValues.Count > 0)
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>(this.SerialEntry.InitialFieldValuesForDestination);
                        foreach (var entry in parameterValues)
                        {
                            dict[entry.Key] = entry.Value;
                        }

                        parameterValues = dict;
                    }
                    else
                    {
                        parameterValues = this.SerialEntry.InitialFieldValuesForDestination;
                    }
                }

                if (parameterValues.Count > 0)
                {
                    UPConfigFilter applyFilter = this.Filter.FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(parameterValues));
                    if (applyFilter != null)
                    {
                        crmQuery.ApplyFilter(applyFilter);
                    }
                }
                else
                {
                    crmQuery.ApplyFilter(this.Filter);
                }
            }

            UPCRMResult crmResult = crmQuery.Find();
            result = new UPSerialEntryInfoResultFromCRMResult(crmResult, this);
            if (this.cachedResults == null)
            {
                this.cachedResults = new Dictionary<string, UPSerialEntryInfoResultFromCRMResult> { { sourceRecordIdentification, result } };
            }
            else
            {
                this.cachedResults[sourceRecordIdentification] = result;
            }

            return result;
        }
    }
}
