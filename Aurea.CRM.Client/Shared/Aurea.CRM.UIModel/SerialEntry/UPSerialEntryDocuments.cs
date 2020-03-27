// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryDocuments.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryDocuments
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSerialEntryDocuments
    /// </summary>
    public class UPSerialEntryDocuments
    {
        /// <summary>
        /// Gets a value indicating whether [with add button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [with add button]; otherwise, <c>false</c>.
        /// </value>
        public bool WithAddButton => string.IsNullOrEmpty(this.AddPhotoDirectButtonName);

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>
        /// The name of the filter.
        /// </value>
        public string FilterName { get; private set; }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public string Style { get; private set; }

        /// <summary>
        /// Gets the name of the add photo direct button.
        /// </summary>
        /// <value>
        /// The name of the add photo direct button.
        /// </value>
        public string AddPhotoDirectButtonName { get; private set; }

        /// <summary>
        /// Gets the name of the has documents column function.
        /// </summary>
        /// <value>
        /// The name of the has documents column function.
        /// </value>
        public string HasDocumentsColumnFunctionName { get; private set; }

        /// <summary>
        /// Gets the has documents column value.
        /// </summary>
        /// <value>
        /// The has documents column value.
        /// </value>
        public string HasDocumentsColumnValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryDocuments"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_filterName">Name of the filter.</param>
        /// <param name="_style">The style.</param>
        /// <param name="_addPhotoDirectButtonName">Name of the add photo direct button.</param>
        /// <param name="_hasDocumentsColumnFunctionName">Name of the has documents column function.</param>
        /// <param name="_hasDocumentsColumnValue">The has documents column value.</param>
        /// <param name="_serialEntry">The serial entry.</param>
        UPSerialEntryDocuments(string _name, string _filterName, string _style, string _addPhotoDirectButtonName,
            string _hasDocumentsColumnFunctionName, string _hasDocumentsColumnValue, UPSerialEntry _serialEntry)
        {
            this.Name = _name;
            this.SerialEntry = _serialEntry;
            this.FilterName = _filterName;
            this.Style = _style;
            this.AddPhotoDirectButtonName = _addPhotoDirectButtonName;
            this.HasDocumentsColumnFunctionName = _hasDocumentsColumnFunctionName;
            this.HasDocumentsColumnValue = _hasDocumentsColumnValue;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigFilter filter = configStore.FilterByName(this.FilterName);
            if (!string.IsNullOrEmpty(filter?.DisplayName))
            {
                this.Label = filter.DisplayName;
            }
        }

        /// <summary>
        /// Creates the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <returns></returns>
        public static UPSerialEntryDocuments Create(Dictionary<string, string> definition, UPSerialEntry _serialEntry)
        {
            string name = definition.ValueOrDefault("name");
            string filterName = definition.ValueOrDefault("filter");
            string style = definition.ValueOrDefault("style").ToUpper();
            if (style != "IMG" && style != "NOIMG")
            {
                style = "DEFAULT";
            }

            string addPhotoDirectButtonName = definition.ValueOrDefault("addPhotoDirectButtonName");
            string hasDocumentsColumnFunctionName = definition.ValueOrDefault("hasDocumentsColumnFunctionName");
            string hasDocumentsColumnValue = definition.ValueOrDefault("hasDocumentsColumnValue");

            if (name == null || filterName == null)
            {
                return null;
            }

            return new UPSerialEntryDocuments(name, filterName, style, addPhotoDirectButtonName, hasDocumentsColumnFunctionName, hasDocumentsColumnValue, _serialEntry);
        }

        /// <summary>
        /// Results for row row.
        /// </summary>
        /// <param name="_serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPCRMResult ResultForRowRow(UPSerialEntry _serialEntry, UPSERow row)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            FieldControl documentFieldControl = configStore.FieldControlByNameFromGroup("List", "D1DocData");
            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(documentFieldControl);
            UPConfigFilter filter = configStore.FilterByName(this.FilterName);
            if (filter != null)
            {
                Dictionary<string, object> valuesForFunction = new Dictionary<string, object>();
                Dictionary<string, UPSEColumn> columnsForFunction = _serialEntry.DestColumnsForFunction;
                foreach (string functionName in columnsForFunction.Keys)
                {
                    UPSEColumn column = columnsForFunction.ValueOrDefault(functionName) ??
                                        columnsForFunction.ValueOrDefault($"$par{functionName}");

                    if (column != null)
                    {
                        string value = row.StringValueAtIndex(column.Index);
                        if (!string.IsNullOrEmpty(value))
                        {
                            valuesForFunction.SetObjectForKey(value, functionName);
                        }
                    }
                }

                UPConfigFilter replacedFilter = filter.FilterByApplyingValueDictionaryDefaults(valuesForFunction, true);
                crmQuery.ApplyFilter(replacedFilter);
            }
            else
            {
                crmQuery.MaxResults = 10;
            }

            UPCRMResult crmResult = crmQuery.Find();
            return crmResult;
        }
    }
}
