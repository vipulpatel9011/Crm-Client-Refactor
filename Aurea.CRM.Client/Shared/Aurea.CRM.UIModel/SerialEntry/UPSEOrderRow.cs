// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEOrderRow.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Order Row
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Serial Entry Order Row
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSERow" />
    public class UPSEOrderRow : UPSERow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEOrderRow"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="tableIndex">Index of the table.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSEOrderRow(UPCRMResultRow resultRow, int tableIndex, int sourceFieldOffset, UPSerialEntry serialEntry)
            : base(resultRow, tableIndex, sourceFieldOffset, serialEntry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEOrderRow"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="listing">The listing.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSEOrderRow(UPCRMResultRow resultRow, UPSEListing listing, UPSerialEntry serialEntry)
            : base(resultRow, listing, serialEntry)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEOrderRow"/> class.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        public UPSEOrderRow(UPSERow sourceRow)
            : base(sourceRow)
        {
        }

        /// <summary>
        /// Gets the unit count.
        /// </summary>
        /// <value>
        /// The unit count.
        /// </value>
        public int UnitCount => this.IntegerValueFromColumn(((UPSEOrder)this.SerialEntry).RowQuantityColumn);

        /// <summary>
        /// Gets the free goods count.
        /// </summary>
        /// <value>
        /// The free goods count.
        /// </value>
        public int FreeGoodsCount => this.IntegerValueFromColumn(((UPSEOrder)this.SerialEntry).RowFreeGoodsColumn);

        /// <summary>
        /// Gets the end price.
        /// </summary>
        /// <value>
        /// The end price.
        /// </value>
        public float EndPrice
        {
            get
            {
                UPSEDestinationColumn column = ((UPSEOrder)this.SerialEntry).RowEndPriceColumn;
                if (column != null)
                {
                    return this.FloatValueFromColumn(column);
                }

                column = ((UPSEOrder)this.SerialEntry).RowUnitPriceColumn;
                if (column == null)
                {
                    return 0;
                }

                return this.UnitCount * this.FloatValueFromColumn(column);
            }
        }

        /// <summary>
        /// Gets the net price.
        /// </summary>
        /// <value>
        /// The net price.
        /// </value>
        public float NetPrice
        {
            get
            {
                UPSEDestinationColumn netValueColumn = ((UPSEOrder)this.SerialEntry).RowNetPriceColumn;
                if (netValueColumn != null)
                {
                    return this.FloatValueFromColumn(netValueColumn);
                }

                return this.EndPrice;
            }
        }

        /// <summary>
        /// Gets the free goods price.
        /// </summary>
        /// <value>
        /// The free goods price.
        /// </value>
        public float FreeGoodsPrice
        {
            get
            {
                int freeGoodsCount = this.FreeGoodsCount;
                if (freeGoodsCount == 0)
                {
                    return 0;
                }

                UPSEDestinationColumn column = ((UPSEOrder)this.SerialEntry).RowUnitPriceColumn;
                if (column == null)
                {
                    return 0;
                }

                return freeGoodsCount * this.FloatValueFromColumn(column);
            }
        }

        /// <summary>
        /// Gets or sets the pricing.
        /// </summary>
        /// <value>
        /// The pricing.
        /// </value>
        public UPSEPrice Pricing { get; set; }

        /// <summary>
        /// Computes the dependent rows.
        /// </summary>
        /// <returns></returns>
        public List<UPSERow> ComputeDependentRows()
        {
            var destColumnsForFunction = SerialEntry.DestColumnsForFunction;
            var quantityColumn = destColumnsForFunction.ValueOrDefault("Quantity");
            var priceColumn = destColumnsForFunction.ValueOrDefault("EndPrice");
            var unitPriceColumn = destColumnsForFunction.ValueOrDefault("UnitPrice");
            var netValueColumn = destColumnsForFunction.ValueOrDefault("NetPrice");
            var pricingUnitPriceColumn = destColumnsForFunction.ValueOrDefault("PricingUnitPrice");
            var disablePricingColumn = destColumnsForFunction.ValueOrDefault("DisablePricing");

            var endPrice = 0d;
            List<UPSERow> otherBundleRows = null;

            if (quantityColumn != null && priceColumn != null)
            {
                var quantityRowValue = RowValues[quantityColumn.Index];
                var quantity = quantityRowValue.Value;
                var price = ((UPSEOrder)this.SerialEntry).PriceForRow(this);
                otherBundleRows = OtherBundleRows();

                if (otherBundleRows.Count > 0)
                {
                    var allInvolvedPositions = new List<UPSERow>(otherBundleRows)
                    {
                        this
                    };

                    foreach (var otherRow in otherBundleRows)
                    {
                        otherRow.RowPricing.UpdateCurrentConditionsWithPositions(allInvolvedPositions);
                        if (otherRow != this)
                        {
                            otherRow.ClearDiscountInfo();
                        }
                    }
                }

                ProcessUnitPriceColumn(unitPriceColumn, pricingUnitPriceColumn, disablePricingColumn, ref price);

                var priceRowValue = RowValues[priceColumn.Index];

                if (quantity != null)
                {
                    double dQuantity = 0;
                    double.TryParse($"{quantity}", out dQuantity);
                    endPrice = dQuantity * price;
                    priceRowValue.Value = priceColumn.ObjectValueFromNumber(endPrice);
                }
                else
                {
                    priceRowValue.Value = null;
                }
            }

            ProcessNetValueColumn(netValueColumn, endPrice);

            return otherBundleRows;
        }

        private void ProcessUnitPriceColumn(
            UPSEColumn unitPriceColumn,
            UPSEColumn pricingUnitPriceColumn,
            UPSEColumn disablePricingColumn,
            ref double price)
        {
            if (unitPriceColumn != null || pricingUnitPriceColumn != null)
            {
                UPSERowValue unitPriceRowValue = null;
                UPSERowValue pricingUnitPriceRowValue = null;
                UPSERowValue readPrice = null;

                if (unitPriceColumn != null)
                {
                    readPrice = RowValues[unitPriceColumn.Index];
                    unitPriceRowValue = readPrice;
                }

                if (unitPriceRowValue != null && disablePricingColumn != null)
                {
                    if (BoolValueFromColumn((UPSEDestinationColumn)disablePricingColumn))
                    {
                        unitPriceRowValue = null;
                    }
                }

                if (pricingUnitPriceColumn != null)
                {
                    pricingUnitPriceRowValue = RowValues[pricingUnitPriceColumn.Index];

                    if (readPrice == null)
                    {
                        readPrice = pricingUnitPriceRowValue;
                    }
                }

                if (price > 0.0000001 || price < -0.0000001)
                {
                    if (pricingUnitPriceRowValue != null)
                    {
                        pricingUnitPriceRowValue.Value = price;
                    }

                    if (unitPriceRowValue != null)
                    {
                        unitPriceRowValue.Value = price;
                    }
                    else if (readPrice != null && (readPrice.Value is float || readPrice.Value is string))
                    {
                        price = GetDouble(readPrice.Value);
                    }
                }
                else if (readPrice.Value is float || readPrice.Value is string)
                {
                    price = GetDouble(readPrice.Value);
                }
            }
        }

        private double GetDouble(object data)
        {
            double dValue = 0;
            double.TryParse($"{data}", out dValue);
            return dValue;
        }

        private void ProcessNetValueColumn(UPSEColumn netValueColumn, double endPrice)
        {
            if (netValueColumn != null)
            {
                double netPrice = endPrice;

                if (netPrice != 0)
                {
                    var rebateFieldNames = new List<string> { "Discount", "Rebate", "Rebate1", "Rebate2", "DiscountCondition", "DiscountBundle" };

                    foreach (var rebateFieldName in rebateFieldNames)
                    {
                        var rebateColumn = SerialEntry.DestColumnsForFunction.ValueOrDefault(rebateFieldName);

                        if (rebateColumn != null)
                        {
                            var rebateRowValue = RowValues[rebateColumn.Index];
                            object rebate = rebateRowValue.Value;
                            if (rebate != null)
                            {
                                netPrice *= 1 - GetDouble(rebate);
                            }
                        }
                    }
                }

                var netPriceRowValue = RowValues[netValueColumn.Index];
                netPriceRowValue.Value = netPrice;
            }
        }

        /// <summary>
        /// Others the bundle rows.
        /// </summary>
        /// <returns></returns>
        public List<UPSERow> OtherBundleRows()
        {
            return this.RowPricing.OtherPositions;
        }

        /// <summary>
        /// Initials the name of the destination value for function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        public override string InitialDestinationValueForFunctionName(string functionName)
        {
            return functionName == "UnitPrice"
                ? ((UPSEOrder)this.SerialEntry).PriceForRow(this).ToString()
                : base.InitialDestinationValueForFunctionName(functionName);
        }
    }
}
