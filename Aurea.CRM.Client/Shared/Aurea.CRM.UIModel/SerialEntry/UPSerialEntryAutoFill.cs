// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryAutoFill.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryAutoFill
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSerialEntryAutoFill
    /// </summary>
    public class UPSerialEntryAutoFill
    {
        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; }

        /// <summary>
        /// Gets the fill data.
        /// </summary>
        /// <value>
        /// The fill data.
        /// </value>
        public Dictionary<string, object> FillData { get; }

        /// <summary>
        /// Gets the fill columns.
        /// </summary>
        /// <value>
        /// The fill columns.
        /// </value>
        public List<string> FillColumns { get; }

        /// <summary>
        /// Gets the fill column parameters.
        /// </summary>
        /// <value>
        /// The fill column parameters.
        /// </value>
        public List<string> FillColumnParameters { get; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the item number destination.
        /// </summary>
        /// <value>
        /// The item number destination.
        /// </value>
        public string ItemNumberDestination { get; }

        /// <summary>
        /// Gets the item number source.
        /// </summary>
        /// <value>
        /// The item number source.
        /// </value>
        public string ItemNumberSource { get; }

        /// <summary>
        /// Gets the rows created.
        /// </summary>
        /// <value>
        /// The rows created.
        /// </value>
        public int RowsCreated { get; private set; }

        //public ArrayList UnknownItemNumbers { get; private set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<UPSERow> Rows { get; set; }

        /// <summary>
        /// Gets the item numbers.
        /// </summary>
        /// <value>
        /// The item numbers.
        /// </value>
        public List<string> ItemNumbers => this.FillData[this.ItemNumberSource] as List<string>;

        /// <summary>
        /// Creates the specified serial entry.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <returns></returns>
        public static UPSerialEntryAutoFill Create(UPSerialEntry serialEntry, ViewReference viewReference)
        {
            try
            {
                return new UPSerialEntryAutoFill(serialEntry, viewReference);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryAutoFill"/> class.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <exception cref="Exception">
        /// FillColumns empty
        /// or
        /// ItemNumberDestination or ItemNumberSource is empty
        /// </exception>
        private UPSerialEntryAutoFill(UPSerialEntry serialEntry, ViewReference viewReference)
        {
            this.SerialEntry = serialEntry;
            this.ViewReference = viewReference;
            this.FillColumns = viewReference.ContextValueForKey("fillColumns")?.Split(';')?.ToList();

            if (this.FillColumns == null || this.FillColumns?.Count == 0)
            {
                //return;
                throw new Exception("FillColumns empty");
            }

            this.FillColumnParameters = viewReference.ContextValueForKey("fcp")?.Split(';')?.ToList();
            if (this.FillColumnParameters?.Count == 0)
            {
                this.FillColumnParameters = this.FillColumns;
            }

            Dictionary<string, object> dict = new Dictionary<string, object>(this.FillColumns.Count);
            int index = 0;
            foreach (string parameter in this.FillColumnParameters)
            {
                if (index >= this.FillColumns?.Count)
                {
                    break;
                }

                var values = viewReference.ContextValueForKey(parameter)?.Split(';')?.ToList();
                if (values != null)
                {
                    dict[this.FillColumns[index]] = values;
                }

                ++index;
            }

            this.FillData = dict;
            if (this.SerialEntry.ExplicitItemNumberFunctionName)
            {
                if (this.FillData.ContainsKey(this.SerialEntry.ItemNumberFunctionName))
                {
                    this.ItemNumberSource = this.SerialEntry.ItemNumberFunctionName;
                }

                this.ItemNumberDestination = this.SerialEntry.ItemNumberFunctionName;
            }

            if (string.IsNullOrEmpty(this.ItemNumberSource))
            {
                if (this.FillData.ContainsKey("ItemNumber"))
                {
                    this.ItemNumberSource = "ItemNumber";
                }
                else if (this.FillData.ContainsKey("CopyItemNumber"))
                {
                    this.ItemNumberSource = "CopyItemNumber";
                }
            }

            if (string.IsNullOrEmpty(this.ItemNumberDestination))
            {
                if (this.SerialEntry.SourceColumnsForFunction.ContainsKey("ItemNumber"))
                {
                    this.ItemNumberDestination = "ItemNumber";
                }
                else if (this.SerialEntry.SourceColumnsForFunction.ContainsKey("CopyItemNumber"))
                {
                    this.ItemNumberDestination = "CopyItemNumber";
                }
            }

            if (string.IsNullOrEmpty(this.ItemNumberDestination) || string.IsNullOrEmpty(this.ItemNumberSource))
            {
                throw new Exception("ItemNumberDestination or ItemNumberSource is empty");
            }
        }

        /// <summary>
        /// Fills this instance.
        /// </summary>
        public void Fill()
        {
            this.SerialEntry.AutoFillFinished();
        }

        /// <summary>
        /// Rowses the loaded.
        /// </summary>
        public void RowsLoaded()
        {
            List<string> _unknown = null;
            List<string> itemNumbers = this.ItemNumbers;
            int count = itemNumbers.Count;
            Dictionary<string, UPSERow> rowsForItemNumber = new Dictionary<string, UPSERow>(this.Rows.Count);

            foreach (UPSERow row in this.Rows)
            {
                row.EnsureLoaded();
                string itemNumber = row.ValueForFunctionName(this.ItemNumberDestination);
                if (string.IsNullOrEmpty(itemNumber))
                {
                    continue;
                }

                rowsForItemNumber.SetObjectForKey(row, itemNumber);
            }

            this.RowsCreated = 0;
            List<int> columnIndices = new List<int>();
            for (int i = 0; i < this.FillColumns.Count; i++)
            {
                string columnName = this.FillColumns[i];
                List<UPSEColumn> childColumnArray = this.SerialEntry.DestChildColumnsForFunction[columnName];
                UPSEColumn col = childColumnArray.Count > 0 ? childColumnArray[0] : this.SerialEntry.DestColumnsForFunction.ValueOrDefault(columnName);

                if (col != null)
                {
                    columnIndices.Add(col.Index);
                }
                else
                {
                    columnIndices.Add(-1);
                }
            }

            for (int i = 0; i < count; i++)
            {
                string itemNumber = itemNumbers[i];
                UPSERow row = rowsForItemNumber.ValueOrDefault(itemNumber);
                if (row == null)
                {
                    if (_unknown == null)
                    {
                        _unknown = new List<string> { itemNumber };
                    }
                    else
                    {
                        _unknown.Add(itemNumber);
                    }

                    continue;
                }

                int fillColumnIndex = 0;
                foreach (string col in this.FillColumns)
                {
                    int columnIndex = columnIndices[fillColumnIndex++];
                    if (columnIndex < 0 || col == this.ItemNumberSource)
                    {
                        continue;
                    }

                    List<string> v = this.FillData.ValueOrDefault(col) as List<string>;
                    if (v.Count > i)
                    {
                        string value = v[i];
                        UPSERowOperation rowOperation = row.NewValueForColumnIndexReturnAffectedRows(value, columnIndex, null);
                        if (rowOperation == UPSERowOperation.Add)
                        {
                            this.SerialEntry.AddPosition(row);
                            this.SerialEntry.SaveAllExecuted = false;
                        }
                    }
                }
            }

            this.SerialEntry.AutoFillFinished();
        }
    }
}
