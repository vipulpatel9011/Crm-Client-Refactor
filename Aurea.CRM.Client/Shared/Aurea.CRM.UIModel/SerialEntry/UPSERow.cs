// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSERow.cs" company="Aurea Software Gmbh">
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
//   UPSERow
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Enum UPSERowOperation
    /// </summary>
    public enum UPSERowOperation
    {
        /// <summary>
        /// Change
        /// </summary>
        Change,

        /// <summary>
        /// Add
        /// </summary>
        Add,

        /// <summary>
        /// Remove
        /// </summary>
        Remove,

        /// <summary>
        /// The row operation is invalid
        /// </summary>
        UPSERowOPerationInvalid
    }

    /// <summary>
    /// UPSERow
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Features.UPCRMListFormatterFunctionDataProvider" />
    public class UPSERow : UPCRMListFormatterFunctionDataProvider, INotifyPropertyChanged
    {
        private const string Format2DecimalPlaces = "##.00";
        private const string Format4DecimalPlaces = "##.0000";
        private const string KeyDisablePricing = "DisablePricing";
        private const string KeyDiscount = "Discount";
        private const string KeyDiscountBundle = "DiscountBundle";
        private const string KeyDiscountCondition = "DiscountCondition";
        private const string KeyEndPrice = "EndPrice";
        private const string KeyFreeGoods = "FreeGoods";
        private const string KeyFreeGoodsBundle = "FreeGoodsBundle";
        private const string KeyFreeGoodsCondition = "FreeGoodsCondition";
        private const string KeyNetPrice = "NetPrice";
        private const string KeyNoAutoDisablePricing = "NoAutoDisablePricing";
        private const string KeyPricingUnitPrice = "PricingUnitPrice";
        private const string KeyQuantity = "Quantity";
        private const string KeyRebate = "Rebate";
        private const string KeySuccessCase = "true";
        private const string KeyUnitPrice = "UnitPrice";
        private const string KeyUnitPriceBundle = "UnitPriceBundle";
        private const string KeyUnitPriceCondition = "UnitPriceCondition";
        private const string ZeroTill4thDecimalPlace = "0.0000";
        private const string DollarSign = "$";
        private const string QuantityFunctionIdentifier = "Quantity";
        private const string LikeOperator = "%%";

        private bool changed;
        private bool rowPricingLoaded;
        private List<string> childRecordIds;
        private UPSERowPricing rowPricing;
        private UPSERowQuota rowQuota;

        /// <summary>
        /// The row values
        /// </summary>
        protected List<UPSERowValue> RowValues;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERow"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="listing">The listing.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSERow(UPCRMResultRow resultRow, UPSEListing listing, UPSerialEntry serialEntry)
        {
            this.RowRecordId = resultRow.RootRecordId;
            this.RowKey = serialEntry.KeyForSourceRowRecordId(this.RowRecordId);
            this.SerialEntryRecordId = null;
            this.SerialEntry = serialEntry;
            if (listing != null)
            {
                this.ListingRecordId = listing.RecordId;
            }

            if (this.SerialEntry.ItemNumberSourceIndex >= 0)
            {
                this.ItemNumber = resultRow.RawValueAtIndex(this.SerialEntry.ItemNumberSourceIndex);
            }

            this.LoadResultRow = resultRow;
            this.LoadListing = listing;
            this.SourceResultRow = resultRow;
            this.SourceResult = resultRow.Result;
        }

        /// <summary>
        /// Gets the serial entry record identification.
        /// </summary>
        /// <value>
        /// The serial entry record identification.
        /// </value>
        public string SerialEntryRecordIdentification
        {
            get
            {
                string recordId = this.SerialEntryRecordId;
                return string.IsNullOrEmpty(recordId) ? StringExtensions.InfoAreaIdRecordId(this.SerialEntry.DestInfoAreaId, recordId) : recordId;
            }
        }

        /// <summary>
        /// Gets the row value array.
        /// </summary>
        /// <value>
        /// The row value array.
        /// </value>
        public List<UPSERowValue> RowValueArray
        {
            get
            {
                return this.RowValues;
            }
            set
            {
                this.RowValues = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new
                        PropertyChangedEventArgs(nameof(RowValueArray)));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has record.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has record; otherwise, <c>false</c>.
        /// </value>
        public bool HasRecord => this.SerialEntryRecordId != null && this.SerialEntryRecordId != "new";

        /// <summary>
        /// Gets the row record identification.
        /// </summary>
        /// <value>
        /// The row record identification.
        /// </value>
        public string RowRecordIdentification => StringExtensions.InfoAreaIdRecordId(this.SerialEntry.SourceInfoAreaId, this.RowRecordId);

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public double Quantity => this.QuantityRowValue?.DoubleValue ?? 0;

        /// <summary>
        /// Gets the end price without discount.
        /// </summary>
        /// <value>
        /// The end price without discount.
        /// </value>
        public double EndPriceWithoutDiscount
        {
            get
            {
                int quantity = (int)this.Quantity;
                if (quantity == 0)
                {
                    return 0;
                }

                UPSEColumn destColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault("UnitPrice");
                if (destColumn != null)
                {
                    return this.DoubleValueFromColumn(destColumn) * quantity;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [server approved].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [server approved]; otherwise, <c>false</c>.
        /// </value>
        public bool ServerApproved
        {
            get
            {
                if (this.SerialEntry.ServerApprovedColumn != null && !string.IsNullOrEmpty(this.SerialEntryRecordId))
                {
                    UPSERowValue rowValue = this.RowValues[this.SerialEntry.ServerApprovedColumn.Index];
                    string rawValue = rowValue.Value as string;
                    return !string.IsNullOrEmpty(rawValue) && (rawValue == "true" || Convert.ToInt32(rawValue) != 0);
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the default size of the step.
        /// </summary>
        /// <value>
        /// The default size of the step.
        /// </value>
        public int DefaultStepSize
        {
            get
            {
                UPSEColumn col = this.SerialEntry.ColumnForFunctionName("StepSize");
                if (col == null)
                {
                    return 1;
                }

                string val = this.ValueForFunctionName("StepSize");
                if (val != null && Convert.ToInt32(val) > 1)
                {
                    return Convert.ToInt32(val);
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets or sets the photo data.
        /// </summary>
        /// <value>
        /// The photo data.
        /// </value>
        public byte[] PhotoData { get; set; }

        /// <summary>
        /// Gets or sets the file name pattern.
        /// </summary>
        /// <value>
        /// The file name pattern.
        /// </value>
        public string FileNamePattern { get; set; }

        /// <summary>
        /// Gets the row key.
        /// </summary>
        /// <value>
        /// The row key.
        /// </value>
        public string RowKey { get; private set; }

        /// <summary>
        /// Gets the row record identifier.
        /// </summary>
        /// <value>
        /// The row record identifier.
        /// </value>
        public string RowRecordId { get; private set; }

        /// <summary>
        /// Gets or sets the serial entry record identifier.
        /// </summary>
        /// <value>
        /// The serial entry record identifier.
        /// </value>
        public string SerialEntryRecordId { get; set; }

        /// <summary>
        /// Gets the destination root record.
        /// </summary>
        /// <value>
        /// The destination root record.
        /// </value>
        public UPCRMRecord DestinationRootRecord { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPSERow"/> is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets or sets the row parent record identifier.
        /// </summary>
        /// <value>
        /// The row parent record identifier.
        /// </value>
        public string RowParentRecordId { get; set; }

        /// <summary>
        /// Gets or sets the listing record identifier.
        /// </summary>
        /// <value>
        /// The listing record identifier.
        /// </value>
        public string ListingRecordId { get; set; }

        /// <summary>
        /// Gets the row pricing.
        /// </summary>
        /// <value>
        /// The row pricing.
        /// </value>
        public UPSERowPricing RowPricing
        {
            get
            {
                if (!this.rowPricingLoaded)
                {
                    this.LoadRowPricing();
                }

                return this.rowPricing;
            }
        }

        /// <summary>
        /// Gets the row quota.
        /// </summary>
        /// <value>
        /// The row quota.
        /// </value>
        public UPSERowQuota RowQuota => this.rowQuota ?? (this.rowQuota = this.SerialEntry.Quota.RowQuotaForRow(this));

        /// <summary>
        /// Gets or sets the source result offset.
        /// </summary>
        /// <value>
        /// The source result offset.
        /// </value>
        public int SourceResultOffset { get; set; }

        /// <summary>
        /// Gets or sets the source result row.
        /// </summary>
        /// <value>
        /// The source result row.
        /// </value>
        public UPCRMResultRow SourceResultRow { get; set; }

        /// <summary>
        /// Gets or sets the source result.
        /// </summary>
        /// <value>
        /// The source result.
        /// </value>
        public UPCRMResult SourceResult { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets the source value dictionary.
        /// </summary>
        /// <value>
        /// The source value dictionary.
        /// </value>
        public Dictionary<string, object> SourceValueDictionary { get; private set; }

        /// <summary>
        /// Gets the quantity row value.
        /// </summary>
        /// <value>
        /// The quantity row value.
        /// </value>
        public UPSERowValue QuantityRowValue { get; set; }

        /// <summary>
        /// Gets the discount information.
        /// </summary>
        /// <value>
        /// The discount information.
        /// </value>
        public UPSEPricingDiscountInfo DiscountInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [create unchanged].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create unchanged]; otherwise, <c>false</c>.
        /// </value>
        public bool CreateUnchanged { get; private set; }

        /// <summary>
        /// Gets the load result row.
        /// </summary>
        /// <value>
        /// The load result row.
        /// </value>
        public UPCRMResultRow LoadResultRow { get; private set; }

        /// <summary>
        /// Gets the load listing.
        /// </summary>
        /// <value>
        /// The load listing.
        /// </value>
        public UPSEListing LoadListing { get; private set; }

        /// <summary>
        /// Gets or sets the row configuration.
        /// </summary>
        /// <value>
        /// The row configuration.
        /// </value>
        public UPSERowConfiguration RowConfiguration { get; set; }

        /// <summary>
        /// Gets the item number.
        /// </summary>
        /// <value>
        /// The item number.
        /// </value>
        public string ItemNumber { get; private set; }

        /// <summary>
        /// Ensures the loaded.
        /// </summary>
        /// <returns></returns>
        public bool EnsureLoaded()
        {
            if (this.LoadResultRow != null)
            {
                this.LoadRowWithResultRowListing(this.LoadResultRow, this.LoadListing);
                this.LoadResultRow = null;
                this.LoadListing = null;
                return true;
            }

            return false;
        }

        private void LoadRowWithResultRowListing(UPCRMResultRow resultRow, UPSEListing listing)
        {
            this.BuildDataFromSourceResultRow(resultRow, 0, listing, false);
            if (listing != null)
            {
                Dictionary<string, object> dict = listing.ValueDictionary;
                foreach (UPSEColumn column in this.SerialEntry.Columns)
                {
                    if (column.FieldConfig.InfoAreaId == this.SerialEntry.ListingInfoAreaId)
                    {
                        string listingKeyForFieldId = this.SerialEntry.ListingController.ListingKeyForInfoAreaIdFieldId(this.SerialEntry.ListingInfoAreaId, column.FieldId);
                        if (!string.IsNullOrEmpty(listingKeyForFieldId))
                        {
                            string listingValue = dict.ValueOrDefault(listingKeyForFieldId) as string;
                            if (!string.IsNullOrEmpty(listingValue))
                            {
                                this.SetValueAtIndex(listingValue, column.Index);
                            }
                        }
                    }
                }

                foreach (string destinationColumnName in this.SerialEntry.DestColumnsForFunction.Keys)
                {
                    string listingValue = dict.ValueOrDefault(destinationColumnName) as string;
                    if (!string.IsNullOrEmpty(listingValue))
                    {
                        UPSEColumn destCol = this.SerialEntry.DestColumnsForFunction[destinationColumnName];
                        this.SetValueAtIndex(listingValue, destCol.Index);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the specified result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="tableIndex">Index of the table.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="serialEntry">The serial entry.</param>
        /// <returns></returns>
        public static UPSERow Create(UPCRMResultRow resultRow, int tableIndex, int sourceFieldOffset, UPSerialEntry serialEntry)
        {
            try
            {
                return new UPSERow(resultRow, tableIndex, sourceFieldOffset, serialEntry);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERow"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="tableIndex">Index of the table.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="serialEntry">The serial entry.</param>
        /// <exception cref="Exception">RowRecordId is null</exception>
        public UPSERow(UPCRMResultRow resultRow, int tableIndex, int sourceFieldOffset, UPSerialEntry serialEntry)
        {
            this.RowRecordId = resultRow.RecordIdAtIndex(tableIndex);            
            if (this.RowRecordId == null)
            {
                //throw new Exception("RowRecordId is null");
                return;
            }

            this.RowKey = serialEntry.KeyForSourceRowRecordId(this.RowRecordId);
            this.SerialEntryRecordId = resultRow.RootRecordId;
            this.SerialEntry = serialEntry;
            this.SourceResultRow = resultRow;
            this.SourceResult = resultRow.Result;
            this.SourceResultOffset = sourceFieldOffset;
            if (this.SerialEntry.ItemNumberSourceIndex >= 0)
            {
                this.ItemNumber = resultRow.RawValueAtIndex(this.SerialEntry.ItemNumberSourceIndex + this.SourceResultOffset);
            }

            this.BuildDataFromSourceResultRow(resultRow, sourceFieldOffset, null, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERow"/> class.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        public UPSERow(UPSERow sourceRow)
        {
            this.RowRecordId = sourceRow.RowRecordId;
            this.RowKey = sourceRow.SerialEntry.KeyForSourceRowRecordId(this.RowRecordId);
            this.SerialEntryRecordId = null;
            this.SerialEntry = sourceRow.SerialEntry;
            this.ListingRecordId = null;
            this.SourceResultRow = sourceRow.SourceResultRow;
            this.SourceResult = sourceRow.SourceResult;
            this.SourceResultOffset = sourceRow.SourceResultOffset;
            this.ItemNumber = sourceRow.ItemNumber;
            this.BuildDataFromSourceResultRow(this.SourceResultRow, this.SourceResultOffset, null, true);
        }

        private void SetChildRecordIdIndex(string recordId, int childIndex)
        {
            if (this.childRecordIds == null)
            {
                this.childRecordIds = new List<string>(this.SerialEntry.ChildrenCount);
                for (int i = 0; i < this.SerialEntry.ChildrenCount; i++)
                {
                    this.childRecordIds.Add(i == childIndex ? recordId : string.Empty);
                }
            }
            else
            {
                this.childRecordIds[childIndex] = recordId;
            }
        }

        /// <summary>
        /// Sets the child data for index result row.
        /// </summary>
        /// <param name="sourceIndex">Index of the source.</param>
        /// <param name="resultRow">The result row.</param>
        public void SetChildDataForIndexResultRow(int sourceIndex, UPCRMResultRow resultRow)
        {
            int count = this.SerialEntry.Columns.Count;
            for (int i = 0; i < count; i++)
            {
                UPSEColumn column = this.SerialEntry.Columns[i];
                if (column.ColumnFrom == UPSEColumnFrom.DestChild && ((UPSEDestinationChildColumn)column).ChildIndex == sourceIndex)
                {
                    UPSERowValue rowValue = this.RowValues[i];
                    rowValue.Value = resultRow.RawValueAtIndex(column.PositionInControl);
                    rowValue.SetUnchanged();
                }
            }

            this.SetChildRecordIdIndex(resultRow.RootRecordId, sourceIndex);
        }

        /// <summary>
        /// Initials the name of the destination value for function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        public virtual string InitialDestinationValueForFunctionName(string functionName)
        {
            return functionName.StartsWith("FixedValue:") ? functionName.Substring(11) : null;
        }

        /// <summary>
        /// Values at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public object ValueAtIndex(int index)
        {
            return this.RowValues[index].Value;
        }

        /// <summary>
        /// Strings the index of the value at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string StringValueAtIndex(int index)
        {
            object val = this.ValueAtIndex(index);

            return val?.ToString();
        }

        /// <summary>
        /// Parents the index of the catalog value at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public object ParentCatalogValueAtIndex(int index)
        {
            UPSEColumn column = this.SerialEntry.Columns[index];
            int parentColumnIndex = column.ParentColumnIndex;
            return parentColumnIndex < 0 ? null : this.ValueAtIndex(parentColumnIndex);
        }

        /// <summary>
        /// Sets the index of the value at.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        void SetValueAtIndex(object value, int index)
        {
            this.RowValues[index].Value = value;
        }

        /// <summary>
        /// Handles the deleted.
        /// </summary>
        public void HandleDeleted()
        {
            this.Deleted = true;
            this.DestinationRootRecord = null;
            this.SerialEntryRecordId = null;
            this.childRecordIds = null;
            this.Error = null;
            int rowIndex = 0;
            if (this.SourceResultRow != null)
            {
                this.LoadRowWithResultRowListing(this.SourceResultRow, null);
            }
            else
            {
                foreach (UPSEColumn column in this.SerialEntry.Columns)
                {
                    if (column is UPSEDestinationColumnBase)
                    {
                        UPSERowValue value = this.RowValues[rowIndex];
                        value.ResetToInitialValue();
                        value.SetUnchanged();
                    }

                    ++rowIndex;
                }
            }
        }

        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="error">The error.</param>
        public void HandleError(Exception error)
        {
            if (error != null && (this.DestinationRootRecord?.IsNew ?? false))
            {
                this.DestinationRootRecord = null;
            }
        }

        /// <summary>
        /// Handles the column initialize.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="functionValueDictionary">The function value dictionary.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="ignoreDestinationResults">if set to <c>true</c> [ignore destination results].</param>
        /// <returns><see cref="UPSERowValue"/></returns>
        public UPSERowValue HandleColumnInit(
            UPSEColumn column,
            Dictionary<string, object> functionValueDictionary,
            UPCRMResultRow resultRow,
            int sourceFieldOffset,
            bool ignoreDestinationResults)
        {
            switch (column.ColumnFrom)
            {
                case UPSEColumnFrom.Source:
                    {
                        return this.InitColumnFromSource(column, functionValueDictionary, resultRow, sourceFieldOffset);
                    }

                case UPSEColumnFrom.AdditionalSource:
                    {
                        return this.InitColumnFromAdditionalSource(column, functionValueDictionary, resultRow, sourceFieldOffset);
                    }

                case UPSEColumnFrom.SourceChild:
                    {
                        return this.InitColumnFromSourceChild(column, functionValueDictionary, resultRow, sourceFieldOffset);
                    }

                case UPSEColumnFrom.Dest:
                    {
                        return this.InitColumnFromDest(column, functionValueDictionary, resultRow, sourceFieldOffset, ignoreDestinationResults);
                    }

                case UPSEColumnFrom.DestChild:
                    {
                        return this.InitColumnFromDestChild(column, functionValueDictionary, resultRow, sourceFieldOffset, ignoreDestinationResults);
                    }

                default:
                    return new UPSERowValue();
            }
        }

        /// <summary>
        /// Gets the document key.
        /// </summary>
        /// <value>
        /// The document key.
        /// </value>
        public string DocumentKey => this.SerialEntry.DocumentKeyColumn == null ? null : this.StringValueAtIndex(this.SerialEntry.DocumentKeyColumn.Index);

        private void BuildDataFromSourceResultRow(UPCRMResultRow resultRow, int sourceFieldOffset, UPSEListing listing, bool ignoreDestinationResults)
        {
            this.SourceResultOffset = sourceFieldOffset;
            this.SourceResultRow = resultRow;
            this.SourceResult = resultRow.Result;
            this.RowValues = new List<UPSERowValue>(this.SerialEntry.Columns.Count);
            Dictionary<string, object> functionValueDictionary;
            int unitPriceIndex = -1;
            if (listing != null && listing.ValueDictionary.Count > 0)
            {
                functionValueDictionary = new Dictionary<string, object>(listing.ValueDictionary);
            }
            else
            {
                functionValueDictionary = new Dictionary<string, object>();
            }

            foreach (UPSEColumn column in this.SerialEntry.Columns)
            {
                if (column.Function == "UnitPrice" && column.ColumnFrom == UPSEColumnFrom.Dest)
                {
                    unitPriceIndex = this.RowValues.Count;
                    this.RowValues.Add(new UPSERowValue());
                    continue;
                }

                UPSERowValue rowValue = this.HandleColumnInit(column, functionValueDictionary, resultRow, sourceFieldOffset, ignoreDestinationResults);
                this.RowValues.Add(rowValue);
            }

            if (unitPriceIndex >= 0)
            {
                UPSEColumn column = this.SerialEntry.Columns[unitPriceIndex];
                this.RowValues[unitPriceIndex] = this.HandleColumnInit(column, functionValueDictionary, resultRow, sourceFieldOffset, ignoreDestinationResults);
            }

            if (this.SerialEntry.FieldGroupDecider != null)
            {
                UPConfigQueryTable queryTable = this.SerialEntry.FieldGroupDecider.QueryTableForResultRow(resultRow);
                if (queryTable != null)
                {
                    this.RowConfiguration = this.SerialEntry.RowConfigurationForQueryTable(queryTable);
                }
            }
            else
            {
                this.RowConfiguration = this.SerialEntry.DefaultRowConfiguration;
            }

            this.SourceValueDictionary = functionValueDictionary;
            this.changed = false;
        }

        private List<UPSERow> ComputeDependentRows()
        {
            return null;
        }

        private void ComputeRow()
        {
            if (this.SerialEntry.DestChildColumnsForFunction != null)
            {
                foreach (string key in this.SerialEntry.DestChildColumnsForFunction.Keys)
                {
                    UPSEColumn sumColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(key);
                    if (sumColumn == null)
                    {
                        continue;
                    }

                    List<UPSEColumn> columnArray = this.SerialEntry.DestChildColumnsForFunction[key];
                    double sum = 0;
                    bool found = false;
                    foreach (UPSEColumn column in columnArray)
                    {
                        object val = this.ValueAtIndex(column.Index);
                        if (val != null)
                        {
                            var num = Convert.ToDouble(val, System.Globalization.CultureInfo.InvariantCulture);
                            found = true;
                            sum += num;
                        }
                    }

                    UPSERowValue rowValue = this.RowValues[sumColumn.Index];
                    rowValue.Value = found ? sumColumn.ObjectValueFromNumber(sum) : null;
                }
            }

            if (this.SerialEntry.DestCopyColumnArrayForFunction != null)
            {
                foreach (string key in this.SerialEntry.DestColumnsForFunction.Keys)
                {
                    List<UPSEColumn> copyColumns = this.SerialEntry.DestCopyColumnArrayForFunction[key];
                    if (copyColumns.Count > 0)
                    {
                        UPSEColumn sourceColumn = this.SerialEntry.DestColumnsForFunction[key];
                        UPSERowValue sourceRowValue = this.RowValues[sourceColumn.Index];
                        foreach (UPSEDestinationColumn col in copyColumns)
                        {
                            UPSERowValue destRowValue = this.RowValues[col.Index];
                            destRowValue.Value = sourceRowValue.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Computes the row with conditions with dependent.
        /// </summary>
        /// <param name="includeDependent">if set to <c>true</c> [include dependent].</param>
        public void ComputeRowWithConditionsWithDependent(bool includeDependent)
        {
            UPSEColumn quantityColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault("Quantity");
            if (quantityColumn != null)
            {
                this.ComputeRowForChangedIndexIncludeDependent(quantityColumn.Index, includeDependent);
            }
        }

        /// <summary>
        /// Applies the overall discount.
        /// </summary>
        /// <param name="overall">if set to <c>true</c> [overall].</param>
        public void ApplyOverallDiscount(bool overall)
        {
            double discount;
            if (overall)
            {
                discount = this.SerialEntry.Pricing.OverallDiscount;
                this.DiscountInfo = null;
            }
            else if (this.rowPricing.HasDiscount)
            {
                this.rowPricing.UpdateCurrentConditionsWithPositions(this.SerialEntry.Positions);
                this.DiscountInfo = this.rowPricing.DiscountInfoForQuantityRowPrice((int)this.Quantity, this.EndPriceWithoutDiscount);
                discount = this.DiscountInfo.Discount;
            }
            else if (this.rowPricing.PriceProvider.BulkVolumes != null)
            {
                int quantityIndex = this.rowPricing.PriceProvider.BulkVolumes.BulkQuantityIndexForRowQuantity(this, this.Quantity);
                discount = quantityIndex >= 0 ? this.rowPricing.Price.DiscountScale[quantityIndex] : 0;
            }
            else
            {
                discount = 0;
            }

            UPSEColumn discountColumn;
            if (this.SerialEntry.OverallDiscountActive)
            {
                discountColumn = this.SerialEntry.DestColumnsForFunction["DiscountBundle"];
            }
            else
            {
                discountColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(this.rowPricing.IsBundlePricing ? "DiscountBundle" : "DiscountCondition");
            }

            if (discountColumn == null)
            {
                discountColumn = this.SerialEntry.DestColumnsForFunction["Discount"];
            }

            if (discountColumn != null)
            {
                UPSERowValue rowValue = this.RowValues[discountColumn.Index];
                rowValue.Value = discount.ToString(Format4DecimalPlaces);
            }

            UPSEColumn priceColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault("NetPrice");
            if (priceColumn != null)
            {
                UPSERowValue rowValue = this.RowValues[priceColumn.Index];
                rowValue.Value = (this.EndPriceWithoutDiscount * (1 - discount)).ToString(Format2DecimalPlaces);
            }
        }

        /// <summary>
        /// Computes the row for changed index include dependent.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="includeDependent">if set to <c>true</c> [include dependent].</param>
        /// <returns>List of <see cref="UPSERow"/></returns>
        protected virtual List<UPSERow> ComputeRowForChangedIndexIncludeDependent(int columnIndex, bool includeDependent)
        {
            var allOtherRowsSignaled = false;
            var otherChangedRows = new List<UPSERow>();
            var quantityColumn = this.SerialEntry.Columns[columnIndex];
            if (quantityColumn.Function == KeyUnitPrice)
            {
                var disablePricingColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyDisablePricing);
                if (disablePricingColumn != null && Convert.ToInt32(SerialEntry.AdditionalOptions.ValueOrDefault(KeyNoAutoDisablePricing)) == 0)
                {
                    var rowValue = RowValues[disablePricingColumn.Index];
                    rowValue.Value = KeySuccessCase;
                }
            }

            ComputeRow();
            if (!this.SerialEntry.ComputeRowOnEveryColumn
                && quantityColumn.Function != KeyQuantity
                && quantityColumn.Function != KeyUnitPrice
                && !quantityColumn.Function.StartsWith(KeyRebate))
            {
                return null;
            }

            var overAllQuantityColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyQuantity);
            if (overAllQuantityColumn != null)
            {
                quantityColumn = overAllQuantityColumn;
            }

            if (this.RowPricing != null)
            {
                allOtherRowsSignaled = CalculateRowPricingBasedCalculations(ref otherChangedRows, columnIndex, includeDependent);
            }

            if (!allOtherRowsSignaled)
            {
                foreach (var otherRow in otherChangedRows)
                {
                    otherRow.ComputeRowWithConditionsWithDependent(false);
                }
            }

            if (SerialEntry.DestCopyColumnArrayForFunction != null)
            {
                UpdateRowValueFromDestColumnArrayForFunction();
            }

            return otherChangedRows;
        }

        void SetChanged(bool _changed)
        {
            this.changed = _changed;
        }

        bool IsEmptyRow()
        {
            int count = this.SerialEntry.Columns.Count;
            bool hasEmpty = false;
            for (int i = 0; i < count; i++)
            {
                UPSEColumn column = this.SerialEntry.Columns[i];
                if (column.ColumnFrom == UPSEColumnFrom.Dest || column.ColumnFrom == UPSEColumnFrom.DestChild)
                {
                    if (((UPSEDestinationColumnBase)column).Empty)
                    {
                        hasEmpty = true;
                        if (!column.IsEmptyValue(column.StringValueFromObject(this.ValueAtIndex(i))))
                        {
                            return false;
                        }
                    }
                }
            }

            return hasEmpty;
        }

        /// <summary>
        /// Forces the create.
        /// </summary>
        /// <returns></returns>
        public UPSERowOperation ForceCreate()
        {
            this.changed = true;
            this.CreateUnchanged = true;
            return this.SerialEntryRecordId == null ? UPSERowOperation.Add : UPSERowOperation.Change;
        }

        string ValueForEditTriggerInputValue(string inputValue)
        {
            if (inputValue.StartsWith("$"))
            {
                string v = this.SerialEntry.InitialFieldValuesForDestination.ValueOrDefault(inputValue) as string;
                if (v != null)
                {
                    return v;
                }
            }
            else
            {
                string rv = this.RawValueForFunctionName(inputValue);
                if (rv != null)
                {
                    return rv;
                }
            }

            return string.Empty;
        }

        string ValueForChildEditTriggerInputValueChildIndex(string inputValue, int childIndex)
        {
            List<UPSEColumn> destChildColumnArray = this.SerialEntry.DestChildColumnsForFunction.ValueOrDefault(inputValue);
            if (destChildColumnArray != null)
            {
                UPSEColumn col = destChildColumnArray[childIndex];
                string val = this.StringValueAtIndex(col.Index);
                return !string.IsNullOrEmpty(val) ? val : string.Empty;
            }

            return this.ValueForEditTriggerInputValue(inputValue);
        }

        /// <summary>
        /// News the value for column index return affected rows.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="affectedRows">The affected rows.</param>
        /// <returns></returns>
        public UPSERowOperation NewValueForColumnIndexReturnAffectedRows(string value, int columnIndex, List<UPSERow> affectedRows)
        {
            UPSEColumn column = this.SerialEntry.Columns[columnIndex];
            if (column.ColumnFrom == UPSEColumnFrom.Dest || column.ColumnFrom == UPSEColumnFrom.DestChild)
            {
                if (!column.IsValueDifferentThan(value, column.StringValueFromObject(this.ValueAtIndex(columnIndex))))
                {
                    return UPSERowOperation.UPSERowOPerationInvalid;
                }

                object val = column.ObjectValueFromString(value);

                this.SetValueAtIndex(value, columnIndex);
                if (column.ColumnFrom == UPSEColumnFrom.DestChild)
                {
                    int childIndex = ((UPSEDestinationChildColumn)column).ChildIndex;
                    //if (this.SerialEntry.DestChildEditTrigger != null && column.Function.Length > 0)
                    //{
                    //    ArrayList rules = this.SerialEntry.DestChildEditTrigger.RulesForFunctionName(column.Function);
                    //    if (rules.Count > 0)
                    //    {
                    //        foreach (UPCRMEditTriggerRule rule in rules)
                    //        {
                    //            List<string> parameterArray = new List<string>(rule.InputFunctionNames.Count);
                    //            foreach (string inputFunctionName in rule.InputFunctionNames)
                    //            {
                    //                parameterArray.Add(this.ValueForChildEditTriggerInputValueChildIndex(inputFunctionName, childIndex));
                    //            }

                    //            Dictionary<string, object> changedValues = rule.ChangedValuesForInputValueArray(parameterArray);
                    //            foreach (string changedValue in changedValues.Keys)
                    //            {
                    //                UPSEColumn childColumn = this.SerialEntry.DestChildColumnsForFieldKey[changedValue][childIndex];
                    //                if (childColumn != null)
                    //                {
                    //                    object cv = childColumn.ObjectValueFromString(changedValues[changedValue] as string);
                    //                    this.SetValueAtIndex(cv, childColumn.Index);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

                //if (this.SerialEntry.DestEditTrigger != null && column.Function.Length > 0)
                //{
                //    this.ComputeRow();
                //    ArrayList rules = this.SerialEntry.DestEditTrigger.RulesForFunctionName(column.Function);
                //    if (rules.Count)
                //    {
                //        foreach (UPCRMEditTriggerRule rule in rules)
                //        {
                //            List<string> parameterArray = new List<string>(rule.InputFunctionNames.Count);
                //            foreach (string inputFunctionName in rule.InputFunctionNames)
                //            {
                //                parameterArray.Add(this.ValueForEditTriggerInputValue(inputFunctionName));
                //            }

                //            Dictionary<string, object> changedValues = rule.ChangedValuesForInputValueArray(parameterArray);
                //            foreach (string changedValue in changedValues.Keys)
                //            {
                //                UPSEColumn destColumn = this.SerialEntry.DestColumnForFieldKey.ValueOrDefault(changedValue);
                //                if (destColumn != null)
                //                {
                //                    object cv = destColumn.ObjectValueFromString(changedValues.ValueOrDefault(changedValue) as string);
                //                    this.SetValueAtIndex(cv, destColumn.Index);
                //                }
                //            }
                //        }
                //    }
                //}

                foreach (UPSEColumn otherColumn in this.SerialEntry.Columns)
                {
                    if (otherColumn.ParentColumnIndex == columnIndex)
                    {
                        this.SetValueAtIndex("0", otherColumn.Index);
                    }
                }

                this.changed = true;
                List<UPSERow> otherBundleRows = this.ComputeRowForChangedIndexIncludeDependent(columnIndex, true);
                if (otherBundleRows.Count > 0)
                {
                    affectedRows?.AddRange(otherBundleRows);

                    foreach (UPSERow otherBundleRow in otherBundleRows)
                    {
                        otherBundleRow.SetChanged(true);
                    }
                }

                if (column.IsEmptyValue(value) && this.IsEmptyRow())
                {
                    if (!this.Deleted && this.SerialEntryRecordId != null)
                    {
                        return UPSERowOperation.Remove;
                    }

                    return UPSERowOperation.UPSERowOPerationInvalid;
                }

                if (this.Deleted)
                {
                    this.Deleted = false;
                    return UPSERowOperation.Add;
                }

                if (this.SerialEntryRecordId == null)
                {
                    this.SerialEntryRecordId = "new";
                    return UPSERowOperation.Add;
                }

                return UPSERowOperation.Change;
            }

            return UPSERowOperation.UPSERowOPerationInvalid;
        }

        /// <summary>
        /// Listings the source value for column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public string ListingSourceValueForColumn(UPSEDestinationColumnBase column)
        {
            if (!string.IsNullOrEmpty(column.Function))
            {
                UPSEColumn sourceColumn = this.SerialEntry.ListingSourceColumns.ValueOrDefault(column.Function);
                if (sourceColumn != null)
                {
                    string value = this.StringValueAtIndex(sourceColumn.Index);
                    return !string.IsNullOrEmpty(value) ? value : null;
                }
            }

            return null;
        }

        /// <summary>
        /// Values the name of for function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        public string ValueForFunctionName(string functionName)
        {
            UPSEColumn column = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(functionName);
            if (column != null)
            {
                return this.StringValueAtIndex(column.Index);
            }

            UPSESourceColumn sourceColumn = this.SerialEntry.SourceColumnsForFunction.ValueOrDefault(functionName);
            if (sourceColumn != null)
            {
                return this.StringValueAtIndex(sourceColumn.Index);
            }

            return null;
        }

        /// <summary>
        /// Keys for function name array.
        /// </summary>
        /// <param name="functionNameArray">The function name array.</param>
        /// <returns></returns>
        public string KeyForFunctionNameArray(List<string> functionNameArray)
        {
            string retValue = string.Empty;
            this.EnsureLoaded();
            foreach (string functionName in functionNameArray)
            {
                retValue = !string.IsNullOrEmpty(retValue) ? $"{retValue},{this.ValueForFunctionName(functionName)}" : this.ValueForFunctionName(functionName);
            }

            return retValue ?? string.Empty;
        }

        /// <summary>
        /// Sources the function values.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> SourceFunctionValues()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (string functionName in this.SerialEntry.SourceColumnsForFunction.Keys)
            {
                UPSESourceColumn sourceColumn = this.SerialEntry.SourceColumnsForFunction.ValueOrDefault(functionName);
                if (sourceColumn != null)
                {
                    string strVal = this.StringValueAtIndex(sourceColumn.Index);
                    if (!string.IsNullOrEmpty(strVal))
                    {
                        dict[functionName] = strVal;
                        if (functionName.StartsWith("Copy"))
                        {
                            var addKey = functionName.Length == 4 ? "ItemNumber" : functionName.Substring(4);

                            if (!dict.ContainsKey(addKey))
                            {
                                dict[addKey] = strVal;
                            }
                        }
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Changeds the child records for root record parent record.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="parentRecord">The parent record.</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedChildRecordsForRootRecordParentRecord(UPCRMRecord rootRecord, UPCRMRecord parentRecord)
        {
            if (!this.changed)
            {
                return null;
            }

            var processor = new ChangedChildRecordsForRootRecordParentRecordProcessor
            {
                SerialEntry = this.SerialEntry,
                SerialEntryRecordId = this.SerialEntryRecordId,
                DestinationRootRecord = this.DestinationRootRecord,
                CreateUnchanged = this.CreateUnchanged,
                RowRecordId = this.RowRecordId,
                ListingRecordId = this.ListingRecordId,
                RowValues = this.RowValues,
                ChildRecordIds = this.childRecordIds,
                RowQuota = this.rowQuota
            };

            var result = processor.ChangedChildRecordsForRootRecordParentRecord(rootRecord, parentRecord);
            this.DestinationRootRecord = processor.DestinationRootRecord;
            return result;
        }

        /// <summary>
        /// Integers the value from column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected int IntegerValueFromColumn(UPSEColumn column)
        {
            if (column == null)
            {
                return 0;
            }

            //object val = this.IntegerValueAtIndex(column.Index);

            //return Convert.ToInt32(val);
            return Convert.ToInt32(this.DoubleValueFromColumn(column));
        }

        /// <summary>
        /// Bools the value from column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected bool BoolValueFromColumn(UPSEColumn column)
        {
            if (column == null)
            {
                return false;
            }

            object val = this.ValueAtIndex(column.Index);
            if (val is string)
            {
                if ((string)val == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return Convert.ToInt32(val) != 0;
        }

        /// <summary>
        /// Numbers the value from column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public decimal NumberValueFromColumn(UPSEColumn column)
        {
            //object number = column.NumberFromValue(this.ValueAtIndex(column.Index));
            return column.NumberFromValue(this.ValueAtIndex(column.Index));
        }

        /// <summary>
        /// Floats the value from column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected float FloatValueFromColumn(UPSEDestinationColumnBase column)
        {
            if (column == null)
            {
                return 0;
            }

            object val = this.ValueAtIndex(column.Index);
            return (float)Convert.ToDecimal(val, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Doubles the value from column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected double DoubleValueFromColumn(UPSEColumn column)
        {
            if (column == null)
            {
                return 0;
            }

            UPSERowValue rowValue = (UPSERowValue)this.RowValues[column.Index];
            return rowValue.DoubleValue;
        }

        /// <summary>
        /// Updates the destination data from row child result.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="destinationChildResult">The destination child result.</param>
        public void UpdateDestinationDataFromRowChildResult(UPCRMResultRow resultRow, UPCRMResult destinationChildResult)
        {
            this.Error = null;
            this.childRecordIds = null;
            if (resultRow == null)
            {
                this.SerialEntryRecordId = null;
                this.changed = false;
                this.DestinationRootRecord = null;
                return;
            }

            this.SerialEntryRecordId = resultRow.RootRecordId;
            foreach (UPSEColumn col in this.SerialEntry.Columns)
            {
                if (col.ColumnFrom != UPSEColumnFrom.Dest)
                {
                    continue;
                }

                this.SetValueAtIndex(resultRow.RawValueAtIndex(col.PositionInControl), col.Index);
            }

            if (this.SerialEntry.ChildrenCount > 0)
            {
                foreach (UPSEColumn col in this.SerialEntry.Columns)
                {
                    if (col.ColumnFrom != UPSEColumnFrom.DestChild)
                    {
                        continue;
                    }

                    this.SetValueAtIndex(null, col.Index);
                }

                int count = destinationChildResult.RowCount;
                for (int i = 0; i < count; i++)
                {
                    resultRow = (UPCRMResultRow)destinationChildResult.ResultRowAtIndex(i);
                    string sourceChildRecordId = resultRow.RecordIdAtIndex(1);
                    int childIndex = this.SerialEntry.SourceChildIndexForRecordId(sourceChildRecordId);
                    if (childIndex >= 0)
                    {
                        this.SetChildRecordIdIndex(resultRow.RootRecordId, childIndex);
                        foreach (UPSEColumn col in this.SerialEntry.Columns)
                        {
                            if (col.ColumnFrom != UPSEColumnFrom.DestChild)
                            {
                                continue;
                            }

                            UPSEDestinationChildColumn childColumn = (UPSEDestinationChildColumn)col;
                            if (childColumn.ChildIndex != childIndex)
                            {
                                continue;
                            }

                            this.SetValueAtIndex(resultRow.RawValueAtIndex(col.PositionInControl), col.Index);
                        }
                    }
                }
            }

            foreach (UPSERowValue rowValue in this.RowValues)
            {
                rowValue.SetUnchanged();
            }

            this.changed = true;
        }

        /// <summary>
        /// Gets the existing child record ids.
        /// </summary>
        /// <value>
        /// The existing child record ids.
        /// </value>
        public List<string> ExistingChildRecordIds
        {
            get
            {
                List<string> existingChildRecordIds = null;
                foreach (string recordId in this.childRecordIds)
                {
                    if (!string.IsNullOrEmpty(recordId))
                    {
                        if (existingChildRecordIds == null)
                        {
                            existingChildRecordIds = new List<string> { recordId };
                        }
                        else
                        {
                            existingChildRecordIds.Add(recordId);
                        }
                    }
                }

                return existingChildRecordIds;
            }
        }

        private void LoadRowPricing()
        {
            if (!this.rowPricingLoaded)
            {
                this.rowPricingLoaded = true;
                this.rowPricing = this.SerialEntry.Pricing.PriceForRow(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can have dependent rows.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can have dependent rows; otherwise, <c>false</c>.
        /// </value>
        public bool CanHaveDependentRows
        {
            get
            {
                if (this.rowPricingLoaded && !string.IsNullOrEmpty(this.RowPricing.BundleIdentification))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Determines whether [is dependent on row] [the specified row].
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        ///   <c>true</c> if [is dependent on row] [the specified row]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDependentOnRow(UPSERow row)
        {
            if (this.rowPricingLoaded && this.RowPricing.BundleIdentification == row.RowPricing.BundleIdentification)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Dependents the rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public List<UPSERow> DependentRows(List<UPSERow> rows)
        {
            if (!this.CanHaveDependentRows)
            {
                return null;
            }

            List<UPSERow> dependentRows = null;
            foreach (UPSERow currentRow in rows)
            {
                if (currentRow == this)
                {
                    continue;
                }

                if (currentRow.IsDependentOnRow(this))
                {
                    if (dependentRows == null)
                    {
                        dependentRows = new List<UPSERow> { currentRow };
                    }
                    else
                    {
                        dependentRows.Add(currentRow);
                    }
                }
            }

            return dependentRows;
        }

        /// <summary>
        /// Gets the maximum quantity.
        /// </summary>
        /// <value>
        /// The maximum quantity.
        /// </value>
        public int MaxQuantity
        {
            get
            {
                if (this.SerialEntry.Quota != null)
                {
                    UPSERowQuota _rowQuota = this.RowQuota;
                    if (_rowQuota != null)
                    {
                        if (_rowQuota.UnlimitedQuota)
                        {
                            return 0;
                        }

                        return _rowQuota.RemainingQuotaForCount(0);
                    }
                }
                else
                {
                    string val = this.ValueForFunctionName("MaxQuantity");
                    if (val != null && Convert.ToInt32(val) > 0)
                    {
                        return Convert.ToInt32(val);
                    }

                    return 0;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the minimum quantity.
        /// </summary>
        /// <value>
        /// The minimum quantity.
        /// </value>
        public int MinQuantity
        {
            get
            {
                string val = this.ValueForFunctionName("MinQuantity");
                if (val != null && Convert.ToInt32(val) > 1)
                {
                    return Convert.ToInt32(val);
                }

                return 0;
            }
        }

        /// <summary>
        /// Sums for destination columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public int SumForDestinationColumns(List<UPSEColumn> columns)
        {
            return columns.Sum(column => this.IntegerValueFromColumn(column));
        }

        /// <summary>
        /// Gets a value indicating whether [unlimited quota].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unlimited quota]; otherwise, <c>false</c>.
        /// </value>
        public bool UnlimitedQuota
        {
            get
            {
                if (this.SerialEntry.Quota == null)
                {
                    return true;
                }

                UPSERowQuota _rowQuota = this.RowQuota;
                if (_rowQuota != null)
                {
                    return _rowQuota.UnlimitedQuota;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the remaining quota.
        /// </summary>
        /// <value>
        /// The remaining quota.
        /// </value>
        public int RemainingQuota
        {
            get
            {
                if (this.SerialEntry.Quota != null)
                {
                    UPSERowQuota _rowQuota = this.RowQuota;
                    if (_rowQuota != null)
                    {
                        int count = this.SumForDestinationColumns(this.SerialEntry.Quota.DestinationColumns);
                        return _rowQuota.RemainingQuotaForCount(count);
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// The raw value for function name.
        /// </summary>
        /// <param name="functionName">The function name.</param>
        /// <returns>
        /// The <see cref="T:System.String" />.
        /// </returns>
        public string RawValueForFunctionName(string functionName)
        {
            UPSEColumn destColumn = this.SerialEntry.DestColumnsForFunction.ValueOrDefault(functionName);
            if (destColumn != null)
            {
                string val = this.StringValueAtIndex(destColumn.Index);
                if (!string.IsNullOrEmpty(val))
                {
                    return val;
                }
            }

            UPSESourceColumn sourceColumn = this.SerialEntry.SourceColumnsForFunction.ValueOrDefault(functionName);
            if (sourceColumn != null)
            {
                string val = this.StringValueAtIndex(sourceColumn.Index);
                if (!string.IsNullOrEmpty(val))
                {
                    return val;
                }
            }

            return string.Empty;
        }

        private UPSEDestinationColumn DestinationColumnForFieldValue(UPCRMFieldValue fieldValue)
        {
            return this.SerialEntry.Columns.FirstOrDefault(column => column.CrmField.FieldId == fieldValue.FieldId) as UPSEDestinationColumn;
        }

        private UPSEDestinationChildColumn DestinationChildColumnForFieldValueChildRecord(UPCRMFieldValue fieldValue, UPCRMRecord record)
        {
            foreach (UPSEColumn column in this.SerialEntry.Columns)
            {
                if (!(column is UPSEDestinationChildColumn))
                {
                    continue;
                }

                if (column.CrmField.FieldId != fieldValue.FieldId)
                {
                    continue;
                }

                UPSEDestinationChildColumn childColumn = (UPSEDestinationChildColumn)column;
                UPSESourceChild childRecordId = this.SerialEntry.SourceChildren[childColumn.ChildIndex];
                if (!record.IsLinkedToRecordIdentificationLinkId(childRecordId.Record.RecordIdentification, -1))
                {
                    continue;
                }

                return childColumn;
            }

            return null;
        }

        private bool ApplyRecordHierarchy(UPCRMRecordWithHierarchy recordHierarchy)
        {
            foreach (UPCRMFieldValue fieldValue in recordHierarchy.FieldValues)
            {
                UPSEDestinationColumn destinationColumn = this.DestinationColumnForFieldValue(fieldValue);
                if (destinationColumn != null)
                {
                    this.NewValueForColumnIndexReturnAffectedRows(fieldValue.Value, destinationColumn.Index, null);
                }
            }

            if (!string.IsNullOrEmpty(this.SerialEntry.DestChildInfoAreaId))
            {
                foreach (UPCRMRecord subRecord in recordHierarchy.Children)
                {
                    if (subRecord.InfoAreaId == this.SerialEntry.DestChildInfoAreaId)
                    {
                        foreach (UPCRMFieldValue fieldValue in subRecord.FieldValues)
                        {
                            UPSEDestinationChildColumn childColumn = this.DestinationChildColumnForFieldValueChildRecord(fieldValue, subRecord);
                            if (childColumn != null)
                            {
                                this.NewValueForColumnIndexReturnAffectedRows(fieldValue.Value, childColumn.Index, null);
                            }
                        }
                    }
                }
            }

            this.ComputeRowWithConditionsWithDependent(true);
            this.ComputeRow();
            return true;
        }

        /// <summary>
        /// Steps the index of the size for column.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public int StepSizeForColumnIndex(int columnIndex)
        {
            UPSEColumn column = this.SerialEntry.Columns[columnIndex];
            object optionValue = column.FieldConfig.Attributes.ExtendedOptionForKey("StepSize");
            if (optionValue != null)
            {
                int intVal = Convert.ToInt32(optionValue);
                return intVal > 1 ? intVal : 1;
            }

            return this.DefaultStepSize;
        }

        /// <summary>
        /// Hides the row.
        /// </summary>
        /// <returns></returns>
        public bool HideRow()
        {

            if (this.SerialEntry.Quota!=null && this.SerialEntry.Quota.HideZeroQuota)
            {
                UPSERowQuota _rowQuota = this.RowQuota;
                if (_rowQuota != null && _rowQuota.RemainingQuotaForCount(0) == 0)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public UPSEPricingDiscountInfo ClearDiscountInfo()
        {
            UPSEPricingDiscountInfo old = this.DiscountInfo;
            this.DiscountInfo = null;
            return old;
        }

        /// <summary>
        /// Handles the column initialize from Source.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="functionValueDictionary">The function value dictionary.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <returns><see cref="UPSERowValue"/></returns>
        private UPSERowValue InitColumnFromSource(
            UPSEColumn column,
            Dictionary<string, object> functionValueDictionary,
            UPCRMResultRow resultRow,
            int sourceFieldOffset)
        {
            var rawValue = resultRow.RawValueAtIndex(column.PositionInControl + sourceFieldOffset);
            var value = resultRow.ValueAtIndex(column.PositionInControl + sourceFieldOffset);
            if (!string.IsNullOrWhiteSpace(rawValue) && !string.IsNullOrWhiteSpace(column.Function))
            {
                functionValueDictionary[column.Function] = rawValue;
            }

            return new UPSERowValue(value);
        }

        /// <summary>
        /// Handles the column initialize from Additional Source Child.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="functionValueDictionary">The function value dictionary.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <returns><see cref="UPSERowValue"/></returns>
        private UPSERowValue InitColumnFromAdditionalSource(
            UPSEColumn column,
            Dictionary<string, object> functionValueDictionary,
            UPCRMResultRow resultRow,
            int sourceFieldOffset)
        {
            var value = string.Empty;
            var sourceAddColumn = (UPSESourceAdditionalColumn)column;
            var keyValue = resultRow.RawValueAtIndex(sourceAddColumn.KeyColumn.PositionInControl + sourceFieldOffset);
            var rawValue = sourceAddColumn.RawValueForItemKey(keyValue);
            if (!string.IsNullOrWhiteSpace(rawValue))
            {
                if (!string.IsNullOrWhiteSpace(column.Function))
                {
                    functionValueDictionary[column.Function] = rawValue;
                }

                value = sourceAddColumn.ValueForItemKey(keyValue);
            }
            else
            {
                value = string.Empty;
            }

            return new UPSERowValue(value);
        }

        /// <summary>
        /// Handles the column initialize from Source Child.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="functionValueDictionary">The function value dictionary.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <returns><see cref="UPSERowValue"/></returns>
        private UPSERowValue InitColumnFromSourceChild(
            UPSEColumn column,
            Dictionary<string, object> functionValueDictionary,
            UPCRMResultRow resultRow,
            int sourceFieldOffset)
        {
            var sourceChild = SerialEntry.SourceChildren[((UPSESourceChildColumn)column).ChildIndex];
            var rawValue = sourceChild.RawFieldValues[column.PositionInControl];
            var value = sourceChild.FieldValues[column.PositionInControl];
            if (!string.IsNullOrWhiteSpace(rawValue) && !string.IsNullOrWhiteSpace(column.Function))
            {
                var childIndexText = ((UPSESourceChildColumn)column).ChildIndex.ToString();
                var functionKey = column.Function.Replace(LikeOperator, childIndexText);
                functionValueDictionary.SetObjectForKey(rawValue, functionKey);
            }

            return new UPSERowValue(value);
        }

        /// <summary>
        /// Handles the column initialize from Destination.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="functionValueDictionary">The function value dictionary.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="ignoreDestinationResults">if set to <c>true</c> [ignore destination results].</param>
        /// <returns><see cref="UPSERowValue"/></returns>
        private UPSERowValue InitColumnFromDest(
            UPSEColumn column,
            Dictionary<string, object> functionValueDictionary,
            UPCRMResultRow resultRow,
            int sourceFieldOffset,
            bool ignoreDestinationResults)
        {
            var value = string.Empty;
            var initialValue = (string)null;
            var functionName = ((UPSEDestinationColumn)column).Function;
            if (!string.IsNullOrWhiteSpace(functionName))
            {
                initialValue = functionValueDictionary.ValueOrDefault(functionName) as string;
                if (initialValue == null)
                {
                    initialValue = InitialDestinationValueForFunctionName(functionName);
                    if (initialValue == null)
                    {
                        if (SerialEntry.InitialFieldValuesForDestination != null)
                        {
                            initialValue = SerialEntry
                                .InitialFieldValuesForDestination
                                .ValueOrDefault(functionName) as string;
                        }
                    }
                }

                if (initialValue!=null && initialValue.StartsWith(DollarSign))
                {
                    var repl = new UPConditionValueReplacement();
                    var replacedValues = repl.ReplaceFieldValue(initialValue);
                    if (replacedValues.Any())
                    {
                        initialValue = replacedValues[0];
                    }
                }
            }

            var rowValue = (UPSERowValue)null;
            if (!ignoreDestinationResults && sourceFieldOffset > 0)
            {
                value = resultRow.RawValueAtIndex(column.PositionInControl);
                rowValue = new UPSERowValue(value);
                if (initialValue != null)
                {
                    rowValue.InitialValue = initialValue;
                }
            }
            else
            {
                if (initialValue != null)
                {
                    rowValue = new UPSERowValue(initialValue, true);
                    rowValue.RememberCurrentAsInitialValue();
                }
                else
                {
                    rowValue = new UPSERowValue();
                }
            }

            if (functionName == QuantityFunctionIdentifier)
            {
                QuantityRowValue = rowValue;
            }

            return rowValue;
        }

        /// <summary>
        /// Handles the column initialize from Destination Child.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="functionValueDictionary">The function value dictionary.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="ignoreDestinationResults">if set to <c>true</c> [ignore destination results].</param>
        /// <returns><see cref="UPSERowValue"/></returns>
        private UPSERowValue InitColumnFromDestChild(
            UPSEColumn column,
            Dictionary<string, object> functionValueDictionary,
            UPCRMResultRow resultRow,
            int sourceFieldOffset,
            bool ignoreDestinationResults)
        {
            var initialValue = (string)null;
            var replaceWith = ((UPSEDestinationChildColumn)column).ChildIndex.ToString();
            var functionName = ((UPSEDestinationColumn)column).Function.Replace(LikeOperator, replaceWith);
            if (!string.IsNullOrWhiteSpace(functionName))
            {
                initialValue = functionValueDictionary.ValueOrDefault(functionName) as string;
                if (initialValue == null)
                {
                    initialValue = InitialDestinationValueForFunctionName(functionName);
                    if (initialValue == null)
                    {
                        if (SerialEntry.InitialFieldValuesForDestination != null)
                        {
                            initialValue = SerialEntry.InitialFieldValuesForDestination.ValueOrDefault(functionName) as string;
                        }
                    }
                }

                if (initialValue.StartsWith(DollarSign))
                {
                    var repl = new UPConditionValueReplacement();
                    var replacedValues = repl.ReplaceFieldValue(initialValue);
                    if (replacedValues.Any())
                    {
                        initialValue = replacedValues[0];
                    }
                }
            }

            var rowValue = (UPSERowValue)null;
            if (initialValue != null)
            {
                rowValue = new UPSERowValue(initialValue, true);
                rowValue.RememberCurrentAsInitialValue();
            }
            else
            {
                rowValue = new UPSERowValue();
            }

            return rowValue;
        }

        /// <summary>
        /// Calculates Discount value
        /// </summary>
        /// <param name="otherChangedRows">
        /// List of <see cref="UPSERow"/> changed records.
        /// </param>
        /// <param name="quantity">
        /// total quantity
        /// </param>
        /// <param name="includeDependent">
        /// include dependent fields or not.
        /// </param>
        /// <param name="priceWithoutDiscount">
        /// original price
        /// </param>
        /// <param name="discount">
        /// discount amount</param>
        /// <returns> allOther Rows Signaled </returns>
        private bool DiscountComputation(
            ref List<UPSERow> otherChangedRows,
            int quantity,
            bool includeDependent,
            out double priceWithoutDiscount,
            out double discount)
        {
            var allOtherRowsSignaled = false;
            var hasOverallDiscount = SerialEntry.Pricing.HasOverallDiscount;
            priceWithoutDiscount = EndPriceWithoutDiscount;
            discount = 0.0;
            if (hasOverallDiscount)
            {
                if (SerialEntry.CheckOverallPriceWithRow(this))
                {
                    otherChangedRows = new List<UPSERow>();
                    foreach (var otherRow in SerialEntry.Positions)
                    {
                        if (otherRow != this)
                        {
                            otherRow.ApplyOverallDiscount(SerialEntry.OverallDiscountActive);
                            otherChangedRows.Add(otherRow);
                        }
                    }

                    allOtherRowsSignaled = true;
                    if (!SerialEntry.OverallDiscountActive)
                    {
                        discount = DiscountInfo.Discount;
                    }
                }

                if (SerialEntry.OverallDiscountActive)
                {
                    discount = SerialEntry.Pricing.OverallDiscount;
                }
            }

            if (!SerialEntry.OverallDiscountActive
                && (DiscountInfo == null
                    || DiscountInfo.DontCache
                    || DiscountInfo.MinQuantity > quantity
                    || (DiscountInfo.MaxQuantity < quantity
                    && DiscountInfo.MaxQuantity >= 0)))
            {
                if (rowPricing.HasDiscount)
                {
                    DiscountInfo = rowPricing.DiscountInfoForQuantityRowPrice(quantity, priceWithoutDiscount);
                    if (includeDependent)
                    {
                        var otherRows = ComputeDependentRows();
                        if (!allOtherRowsSignaled)
                        {
                            if (otherRows.Any())
                            {
                                otherChangedRows = new List<UPSERow>(otherRows);
                            }

                            var otherBundleColumns = rowPricing.OtherPositions;
                            if (otherBundleColumns.Any())
                            {
                                if (otherChangedRows != null)
                                {
                                    otherChangedRows.AddRange(otherBundleColumns);
                                }
                                else
                                {
                                    otherChangedRows = new List<UPSERow>(otherBundleColumns);
                                }
                            }
                        }
                    }
                }
            }

            return allOtherRowsSignaled;
        }

        /// <summary>
        /// Update row values from DestColumnArrayForFunction property
        /// </summary>
        private void UpdateRowValueFromDestColumnArrayForFunction()
        {
            foreach (var key in SerialEntry.DestColumnsForFunction.Keys)
            {
                var copyColumns = SerialEntry.DestCopyColumnArrayForFunction[key];
                if (copyColumns.Any())
                {
                    var sourceColumn = SerialEntry.DestColumnsForFunction[key];
                    var sourceRowValue = RowValues[sourceColumn.Index];
                    foreach (var col in copyColumns)
                    {
                        var destRowValue = RowValues[col.Index];
                        destRowValue.Value = sourceRowValue.Value;
                    }
                }
            }
        }

        /// <summary>
        /// All calculations dependant on RowPricing
        /// </summary>
        /// <param name="otherChangedRows">
        /// List of <see cref="UPSERow"/> changed records.
        /// </param>
        /// <param name="columnIndex">
        /// column index
        /// </param>
        /// <param name="includeDependent">
        /// include dependent fields or not.
        /// </param>
        /// <returns> allOther Rows Signaled </returns>
        private bool CalculateRowPricingBasedCalculations(ref List<UPSERow> otherChangedRows, int columnIndex, bool includeDependent)
        {
            var allOtherRowsSignaled = false;
            var hasOverallDiscount = SerialEntry.Pricing.HasOverallDiscount;
            var quantityColumn = SerialEntry.Columns[columnIndex];
            var disablePricing = false;
            var quantityNumber = NumberValueFromColumn(quantityColumn);
            var quantity = Convert.ToInt32(quantityNumber,System.Globalization.CultureInfo.InvariantCulture);
            var quantityAsDouble = Convert.ToDouble(quantityNumber,System.Globalization.CultureInfo.InvariantCulture);
            var unitPriceForQuantity = 0.0;
            if (rowPricing.HasUnitPrice || rowPricing.Price.HasPriceScale)
            {
                PreDiscountUnitPriceComputation(
                    quantityAsDouble,
                    quantity,
                    out disablePricing,
                    out unitPriceForQuantity);
            }

            allOtherRowsSignaled = DiscountComputation(
                ref otherChangedRows,
                quantity,
                includeDependent,
                out var priceWithoutDiscount,
                out var discount);

            if (rowPricing.HasFreeGoods)
            {
                CalculateFreeGoodValues(quantity, priceWithoutDiscount);
            }

            if (SerialEntry.OverallDiscountActive || rowPricing.HasDiscount || rowPricing.Price.HasDiscountScale)
            {
                discount = CalculateDiscountColumnValues(discount, quantityAsDouble);
            }

            if ((SerialEntry.RebateColumns?.Any() ?? false))
            {
                discount = CalculateRebateColumn(discount);
            }

            if (rowPricing.HasUnitPrice)
            {
                CalculateUnitPriceColumnValue(unitPriceForQuantity, disablePricing);
            }

            if (disablePricing || unitPriceForQuantity != 0)
            {
                var unitPriceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyUnitPrice);
                if (unitPriceColumn != null)
                {
                    var rowValue = RowValues[unitPriceColumn.Index];
                    unitPriceForQuantity = Convert.ToDouble(rowValue.Value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            if (unitPriceForQuantity != 0 && !SerialEntry.Pricing.DontUpdateRowPrices)
            {
                CalculatePriceColumnValues(quantityAsDouble, unitPriceForQuantity, discount);
            }

            return allOtherRowsSignaled;
        }

        /// <summary>
        /// Unit price calculation before discount.
        /// </summary>
        /// <param name="quantityAsDouble">
        /// quantity variable casted as double
        /// </param>
        /// <param name="quantity">
        /// total quantity
        /// </param>
        /// <param name="disablePricing">
        /// enable/disablePricing check
        /// </param>
        /// <param name="unitPriceForQuantity">
        /// unit price
        /// </param>
        private void PreDiscountUnitPriceComputation(
            double quantityAsDouble,
            int quantity,
            out bool disablePricing,
            out double unitPriceForQuantity)
        {
            disablePricing = false;
            var endPriceWithoutConditions = rowPricing.Price.UnitPrice * quantityAsDouble;
            var blockPricingColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyDisablePricing);
            if (blockPricingColumn != null)
            {
                disablePricing = BoolValueFromColumn(blockPricingColumn);
            }

            if (rowPricing.HasUnitPrice)
            {
                unitPriceForQuantity = rowPricing.UnitPriceForQuantityRowPrice(quantity, endPriceWithoutConditions);
            }
            else
            {
                var quantityIndex = rowPricing.PriceProvider.BulkVolumes.BulkQuantityIndexForRowQuantity(this, quantityAsDouble);
                unitPriceForQuantity = quantityIndex >= 0
                    ? rowPricing.Price.PriceScale[quantityIndex]
                    : rowPricing.Price.UnitPrice;
            }

            var unitPriceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyPricingUnitPrice);
            if (unitPriceColumn != null)
            {
                var rowValue = RowValues[unitPriceColumn.Index];
                rowValue.Value = unitPriceForQuantity.ToString(Format2DecimalPlaces);
            }

            var key = rowPricing.IsBundlePricing
                ? KeyUnitPriceBundle
                : KeyUnitPriceCondition;
            unitPriceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(key);
            if (unitPriceColumn != null)
            {
                var rowValue = RowValues[unitPriceColumn.Index];
                rowValue.Value = unitPriceForQuantity.ToString(Format2DecimalPlaces);
            }

            if (!disablePricing)
            {
                unitPriceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyUnitPrice);
                if (unitPriceColumn != null)
                {
                    var rowValue = RowValues[unitPriceColumn.Index];
                    rowValue.Value = unitPriceForQuantity.ToString(Format2DecimalPlaces);
                }
            }
        }

        /// <summary>
        /// Calculate UnitPrice Column Value
        /// </summary>
        /// <param name="unitPriceForQuantity">
        /// unit price
        /// </param>
        /// <param name="disablePricing">
        /// enable/disablePricing check
        /// </param>
        private void CalculateUnitPriceColumnValue(double unitPriceForQuantity, bool disablePricing)
        {
            var unitPriceColumn = (UPSEColumn)null;
            if (!disablePricing)
            {
                var key = rowPricing.IsBundlePricing
                    ? KeyUnitPriceBundle
                    : KeyUnitPriceCondition;
                unitPriceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(key);
            }

            var rowValue = (UPSERowValue)null;
            if (unitPriceColumn == null)
            {
                unitPriceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyUnitPrice);
            }

            if (!disablePricing && unitPriceColumn != null)
            {
                rowValue = RowValues[unitPriceColumn.Index];
                rowValue.Value = unitPriceForQuantity.ToString(Format2DecimalPlaces);
            }
        }

        /// <summary>
        /// Calculate PriceColumn Values
        /// </summary>
        /// <param name="quantityAsDouble">
        /// quantity
        /// </param>
        /// <param name="unitPriceForQuantity">
        /// unit price
        /// </param>
        /// <param name="discount">
        /// discount
        /// </param>
        private void CalculatePriceColumnValues(double quantityAsDouble, double unitPriceForQuantity, double discount)
        {
            var priceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyEndPrice);
            if (priceColumn != null)
            {
                var rowValue = RowValues[priceColumn.Index];
                rowValue.Value = (unitPriceForQuantity * quantityAsDouble).ToString(Format2DecimalPlaces);
            }

            priceColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyNetPrice);
            if (priceColumn != null)
            {
                var rowValue = RowValues[priceColumn.Index];
                rowValue.Value = (unitPriceForQuantity * quantityAsDouble * (1 - discount)).ToString(Format2DecimalPlaces);
            }
        }

        /// <summary>
        /// Calculate Rebate Column
        /// </summary>
        /// <param name="discount">
        /// discount
        /// </param>
        /// <returns>
        /// updated discount value
        /// </returns>
        private double CalculateRebateColumn(double discount)
        {
            foreach (var rebateColumn in SerialEntry.RebateColumns)
            {
                var rowValue = RowValues[rebateColumn.Index];
                var rebate = Convert.ToDouble(rowValue.Value,System.Globalization.CultureInfo.InvariantCulture);
                if (rebate > 0.0001 || rebate < -0.0001)
                {
                    if (discount > 0)
                    {
                        discount = 1 - ((1 - discount) * (1 - rebate));
                    }
                    else
                    {
                        discount = rebate;
                    }
                }
            }
            return discount;
        }

        /// <summary>
        /// Calculate DiscountColumn Values
        /// </summary>
        /// <param name="discount">
        /// discount
        /// </param>
        /// <param name="quantityAsDouble">
        /// quantity
        /// </param>
        /// <returns>
        /// updated discount value
        /// </returns>
        private double CalculateDiscountColumnValues(double discount, double quantityAsDouble)
        {
            var discountColumn = (UPSEColumn)null;
            if (SerialEntry.OverallDiscountActive)
            {
                discountColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyDiscountBundle);
            }
            else
            {
                var key = rowPricing.IsBundlePricing
                    ? KeyDiscountBundle
                    : KeyDiscountCondition;
                discountColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(key);
            }

            if (discountColumn == null)
            {
                discountColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyDiscount);
            }

            if (discountColumn != null)
            {
                var rowValue = RowValues[discountColumn.Index];
                if (!SerialEntry.OverallDiscountActive)
                {
                    if (rowPricing.HasDiscount)
                    {
                        discount = DiscountInfo.Discount;
                        rowValue.Value = discount.ToString(Format4DecimalPlaces);
                    }
                    else
                    {
                        var quantityIndex = rowPricing.PriceProvider.BulkVolumes.BulkQuantityIndexForRowQuantity(this, quantityAsDouble);
                        if (quantityIndex >= 0)
                        {
                            discount = rowPricing.Price.DiscountScale[quantityIndex];
                            if (discount > 0 || rowPricing.Price.DiscountScale != null)
                            {
                                rowValue.Value = discount.ToString(Format4DecimalPlaces);
                            }
                        }
                        else if (rowPricing.Price.DiscountScale != null)
                        {
                            rowValue.Value = ZeroTill4thDecimalPlace;
                        }
                    }
                }
                else
                {
                    rowValue.Value = discount.ToString(Format4DecimalPlaces);
                }
            }

            return discount;
        }

        /// <summary>
        /// Calculate FreeGood Values
        /// </summary>
        /// <param name="quantity">
        /// quantity
        /// </param>
        /// <param name="priceWithoutDiscount">
        /// price Without Discount
        /// </param>
        private void CalculateFreeGoodValues(int quantity, double priceWithoutDiscount)
        {
            var key = rowPricing.IsBundlePricing
                ? KeyFreeGoodsBundle
                : KeyFreeGoodsCondition;
            var freeGoodsColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(key);
            if (freeGoodsColumn == null)
            {
                freeGoodsColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(KeyFreeGoods);
            }

            if (freeGoodsColumn != null)
            {
                var rowValue = RowValues[freeGoodsColumn.Index];
                rowValue.Value = rowPricing.FreeGoodsForQuantityRowPrice(quantity, priceWithoutDiscount);
            }
        }
    }
}
