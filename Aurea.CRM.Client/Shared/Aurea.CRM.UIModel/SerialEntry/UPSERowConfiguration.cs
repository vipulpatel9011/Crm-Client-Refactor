// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSERowConfiguration.cs" company="Aurea Software Gmbh">
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
//   UPSERowConfiguration
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSERowConfiguration
    /// </summary>
    public class UPSERowConfiguration
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public List<UPSEColumn> Columns { get; private set; }

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowConfiguration"/> class.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSERowConfiguration(UPSerialEntry serialEntry)
        {
            this.Columns = serialEntry.Columns;
            this.SerialEntry = serialEntry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowConfiguration"/> class.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSERowConfiguration(Dictionary<string, object> dict, UPSerialEntry serialEntry)
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            var columnArray = new List<UPSEColumn>();
            var nextColumn = 0;
            this.SerialEntry = serialEntry ?? throw new ArgumentNullException(nameof(serialEntry));
            var defaultConfiguration = serialEntry.DefaultRowConfiguration;
            var configStore = ConfigurationUnitStore.DefaultStore;
            var sourceColumnFieldGroupName = dict.ValueOrDefault("SourceFG") as string;
            this.SetupDefaultConfiguration(sourceColumnFieldGroupName, configStore, defaultConfiguration, ref nextColumn, columnArray);
            this.SetupSourceAndDestinationColumns(dict, configStore, ref nextColumn, defaultConfiguration, columnArray);
            this.SetupColumnDestinationFieldGroupName(dict, configStore, nextColumn, defaultConfiguration, columnArray);

            Columns = columnArray;
        }

        private void SetupDefaultConfiguration(
            string sourceColumnFieldGroupName,
            IConfigurationUnitStore configStore,
            UPSERowConfiguration defaultConfiguration,
            ref int nextColumn,
            List<UPSEColumn> columnArray)
        {
            FieldControl sourceFieldControl = null;
            if (!string.IsNullOrWhiteSpace(sourceColumnFieldGroupName))
            {
                sourceFieldControl = configStore.FieldControlByNameFromGroup("List", sourceColumnFieldGroupName);
            }

            if (sourceFieldControl != null)
            {
                var sourceColumnDictionary = new Dictionary<string, UPSEColumn>();
                for (var i = 0; i < defaultConfiguration.Columns.Count; i++)
                {
                    var column = defaultConfiguration.Columns[i];
                    if (!(column is UPSESourceColumn))
                    {
                        nextColumn = i;
                        break;
                    }

                    sourceColumnDictionary[column.CrmField.FieldIdentification] = column;
                }

                var tab = sourceFieldControl.TabAtIndex(0);
                foreach (var field in tab.Fields)
                {
                    var column = sourceColumnDictionary.ValueOrDefault(field.Field.FieldIdentification);
                    if (column != null)
                    {
                        columnArray.Add(new UPSESourceColumn(field, column));
                    }
                }
            }
            else
            {
                for (var i = 0; i < defaultConfiguration.Columns.Count; i++)
                {
                    var column = defaultConfiguration.Columns[i];
                    if (!(column is UPSESourceColumn))
                    {
                        nextColumn = i;
                        break;
                    }

                    columnArray.Add(column);
                }
            }

            for (var i = nextColumn; i < defaultConfiguration.Columns.Count; i++)
            {
                var column = defaultConfiguration.Columns[i];
                if (!(column is UPSESourceAdditionalColumn))
                {
                    nextColumn = i;
                    break;
                }

                columnArray.Add(column);
            }
        }

        private void SetupSourceAndDestinationColumns(
            Dictionary<string, object> dict,
            IConfigurationUnitStore configStore,
            ref int nextColumn,
            UPSERowConfiguration defaultConfiguration,
            List<UPSEColumn> columnArray)
        {
            if (SerialEntry.ChildrenCount > 0)
            {
                var sourceChildFG = dict.ValueOrDefault("SourceChildFG") as string;
                var destChildFG = dict.ValueOrDefault("DestChildFG") as string;
                if (!string.IsNullOrWhiteSpace(sourceChildFG) || !string.IsNullOrWhiteSpace(destChildFG))
                {
                    FieldControlTab sourceFieldTab = null, destFieldTab = null;
                    if (!string.IsNullOrWhiteSpace(sourceChildFG))
                    {
                        sourceFieldTab = configStore.FieldControlByNameFromGroup("List", sourceChildFG).TabAtIndex(0);
                    }

                    if (!string.IsNullOrWhiteSpace(destChildFG))
                    {
                        destFieldTab = configStore.FieldControlByNameFromGroup("Edit", destChildFG).TabAtIndex(0);
                    }

                    if (sourceFieldTab == null)
                    {
                        sourceFieldTab = SerialEntry.SourceChildFieldControl.TabAtIndex(0);
                    }

                    if (destFieldTab == null)
                    {
                        destFieldTab = SerialEntry.DestChildFieldControl.TabAtIndex(0);
                    }

                    var sourceChildFieldDictionary = new Dictionary<string, UPSEColumn>();
                    var destChildFieldDictionary = new Dictionary<string, UPSEColumn>();
                    for (var i = nextColumn; i < defaultConfiguration.Columns.Count; i++)
                    {
                        var configurationColumn = defaultConfiguration.Columns[i];
                        if (configurationColumn is UPSESourceChildColumn column)
                        {
                            var colKey = $"{column.ChildIndex}_{column.CrmField.FieldIdentification}";
                            sourceChildFieldDictionary.SetObjectForKey(column, colKey);
                        }
                        else if (configurationColumn is UPSEDestinationChildColumn childColumn)
                        {
                            var colKey = $"{childColumn.ChildIndex}_{childColumn.CrmField.FieldIdentification}";
                            destChildFieldDictionary[colKey] = childColumn;
                        }
                        else
                        {
                            nextColumn = i;
                            break;
                        }
                    }

                    for (var j = 0; j < SerialEntry.ChildrenCount; j++)
                    {
                        var count = sourceFieldTab.NumberOfFields;
                        for (var i = 0; i < count; i++)
                        {
                            var configField = sourceFieldTab.FieldAtIndex(i);
                            var colKey = $"{j}_{configField.Field.FieldIdentification}";
                            if (sourceChildFieldDictionary.ValueOrDefault(colKey) is UPSESourceChildColumn col)
                            {
                                columnArray.Add(new UPSESourceChildColumn(configField, col));
                            }
                        }

                        count = destFieldTab.NumberOfFields;
                        for (var i = 0; i < count; i++)
                        {
                            var configField = destFieldTab.FieldAtIndex(i);
                            var colKey = $"{j}_{configField.Field.FieldIdentification}";
                            if (destChildFieldDictionary.ValueOrDefault(colKey) is UPSEDestinationChildColumn col)
                            {
                                columnArray.Add(new UPSEDestinationChildColumn(configField, col));
                            }
                        }
                    }
                }
                else
                {
                    for (var i = nextColumn; i < defaultConfiguration.Columns.Count; i++)
                    {
                        var column = defaultConfiguration.Columns[i];
                        if (!(column is UPSESourceChildColumn) && !(column is UPSEDestinationChildColumn))
                        {
                            nextColumn = i;
                            break;
                        }

                        columnArray.Add(column);
                    }
                }
            }
        }

        private void SetupColumnDestinationFieldGroupName(
            Dictionary<string, object> dict,
            IConfigurationUnitStore configStore,
            int nextColumn,
            UPSERowConfiguration defaultConfiguration,
            List<UPSEColumn> columnArray)
        {
            var destinationColumnFieldGroupName = dict.ValueOrDefault("DestFG") as string;
            FieldControl destFieldControl = null;
            if (!string.IsNullOrWhiteSpace(destinationColumnFieldGroupName))
            {
                destFieldControl = configStore.FieldControlByNameFromGroup("Edit", destinationColumnFieldGroupName);
            }

            if (destFieldControl != null)
            {
                var destColumnDictionary = new Dictionary<string, UPSEColumn>();
                for (var i = nextColumn; i < defaultConfiguration.Columns.Count; i++)
                {
                    var column = defaultConfiguration.Columns[i];
                    if (!(column is UPSEDestinationColumn))
                    {
                        break;
                    }

                    destColumnDictionary[column.CrmField.FieldIdentification] = column;
                }

                var tab = destFieldControl.TabAtIndex(0);
                foreach (var field in tab.Fields)
                {
                    if (destColumnDictionary.ValueOrDefault(field.Field.FieldIdentification) is UPSEDestinationColumn column)
                    {
                        columnArray.Add(new UPSEDestinationColumn(field, column));
                    }
                }
            }
            else
            {
                for (var i = nextColumn; i < defaultConfiguration.Columns.Count; i++)
                {
                    var column = defaultConfiguration.Columns[i];
                    if (!(column is UPSEDestinationColumn))
                    {
                        break;
                    }

                    columnArray.Add(column);
                }
            }
        }
    }
}
