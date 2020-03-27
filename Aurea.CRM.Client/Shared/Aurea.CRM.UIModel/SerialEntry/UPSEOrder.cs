// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEOrder.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Order
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OfflineStorage;

    /// <summary>
    /// Serial Entry Order
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntry" />
    public class UPSEOrder : UPSerialEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEOrder"/> class.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="offlineRequest">The offline request.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="InvalidOperationException">FieldControl is null</exception>
        public UPSEOrder(UPCRMRecord rootRecord, Dictionary<string, object> parameters,
            UPOfflineSerialEntryRequest offlineRequest, UPSerialEntryDelegate theDelegate)
            : base(rootRecord, parameters, offlineRequest, theDelegate)
        {
            if (this.SourceFieldControl == null || this.DestFieldControl == null)
            {
                throw new InvalidOperationException("FieldControl is null");
            }
        }

        /// <summary>
        /// Gets the unit count.
        /// </summary>
        /// <value>
        /// The unit count.
        /// </value>
        public int UnitCount => this.UnitCountForRows(this.positions);

        /// <summary>
        /// Gets the free goods count.
        /// </summary>
        /// <value>
        /// The free goods count.
        /// </value>
        public int FreeGoodsCount => this.FreeGoodsCountForRows(this.positions);

        /// <summary>
        /// Gets the end price.
        /// </summary>
        /// <value>
        /// The end price.
        /// </value>
        public double EndPrice => this.EndPriceForRows(this.positions);

        /// <summary>
        /// Gets the free goods price.
        /// </summary>
        /// <value>
        /// The free goods price.
        /// </value>
        public double FreeGoodsPrice => this.FreeGoodsPriceForRows(this.positions);

        /// <summary>
        /// Gets the net price.
        /// </summary>
        /// <value>
        /// The net price.
        /// </value>
        public double NetPrice => this.NetPriceForRows(this.positions);

        /// <summary>
        /// Gets the row quantity column.
        /// </summary>
        /// <value>
        /// The row quantity column.
        /// </value>
        public UPSEDestinationColumn RowQuantityColumn { get; private set; }

        /// <summary>
        /// Gets the row end price column.
        /// </summary>
        /// <value>
        /// The row end price column.
        /// </value>
        public UPSEDestinationColumn RowEndPriceColumn { get; private set; }

        /// <summary>
        /// Gets the row unit price column.
        /// </summary>
        /// <value>
        /// The row unit price column.
        /// </value>
        public UPSEDestinationColumn RowUnitPriceColumn { get; private set; }

        /// <summary>
        /// Gets the row free goods column.
        /// </summary>
        /// <value>
        /// The row free goods column.
        /// </value>
        public UPSEDestinationColumn RowFreeGoodsColumn { get; private set; }

        /// <summary>
        /// Gets the row net price column.
        /// </summary>
        /// <value>
        /// The row net price column.
        /// </value>
        public UPSEDestinationColumn RowNetPriceColumn { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [compute price for quantity1].
        /// </summary>
        /// <value>
        /// <c>true</c> if [compute price for quantity1]; otherwise, <c>false</c>.
        /// </value>
        public bool ComputePriceForQuantity1 { get; private set; }

        /// <summary>
        /// Rows from source result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="listing">The listing.</param>
        /// <returns></returns>
        public override UPSERow RowFromSourceResultRow(UPCRMResultRow row, UPSEListing listing)
        {
            return new UPSEOrderRow(row, listing, this);
        }

        /// <summary>
        /// Rows from destination result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <returns></returns>
        public override UPSERow RowFromDestinationResultRow(UPCRMResultRow row, int offset, int sourceFieldOffset)
        {
            return new UPSEOrderRow(row, offset, sourceFieldOffset, this);
        }

        /// <summary>
        /// Rows from source row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPSERow RowFromSourceRow(UPSERow row)
        {
            return new UPSEOrderRow(row);
        }

        protected override void InitializeWithDestinationResult(UPCRMResult result, UPContainerMetaInfo searchOperation)
        {
            foreach (UPSEColumn column in this.Columns)
            {
                var destColumn = column as UPSEDestinationColumn;
                if (destColumn != null)
                {
                    switch (column.Function)
                    {
                        case "UnitPrice":
                            this.RowUnitPriceColumn = destColumn;
                            break;

                        case "EndPrice":
                            this.RowEndPriceColumn = destColumn;
                            break;

                        case "Quantity":
                            this.RowQuantityColumn = destColumn;
                            break;

                        case "FreeGoods":
                            this.RowFreeGoodsColumn = destColumn;
                            break;

                        case "NetPrice":
                            this.RowNetPriceColumn = destColumn;
                            break;
                    }
                }
            }

            this.ComputePriceForQuantity1 = ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("SerialEntry.UnitPriceCheckPriceScaleFor1");
            base.InitializeWithDestinationResult(result, searchOperation);
        }

        private bool RowHasHigherPriorityThanRow(UPCRMResultRow testRow, UPCRMResultRow row)
        {
            int columnCount = testRow.Result.ColumnCount;
            for (int i = 0; i < columnCount; i++)
            {
                string rowValue = row.RawValueAtIndex(i);
                if (!string.IsNullOrEmpty(rowValue) && rowValue != "0")
                {
                    return false;
                }

                rowValue = testRow.RawValueAtIndex(i);
                if (!string.IsNullOrEmpty(rowValue) && rowValue != "0")
                {
                    return true;
                }
            }

            return false;
        }

        private UPSERowPricing RowPricingForRow(UPSEOrderRow row)
        {
            return this.Pricing.PriceForRow(row);
        }

        private UPSEPrice PricingForRow(UPSEOrderRow row)
        {
            if (row.Pricing != null)
            {
                return row.Pricing;
            }

            UPSERowPricing rowPricing = this.RowPricingForRow(row);
            if (rowPricing != null)
            {
                row.Pricing = rowPricing.Price;
            }

            return row.Pricing;
        }

        /// <summary>
        /// Prices for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public double PriceForRow(UPSEOrderRow row)
        {
            double basePrice = this.BasePriceForRow(row);
            if (this.ComputePriceForQuantity1 && basePrice == 0)
            {
                UPSERowPricing rowPricing = this.RowPricingForRow(row);
                if (rowPricing.HasUnitPrice)
                {
                    return rowPricing.UnitPriceForQuantityRowPrice(1, 0);
                }
            }

            return basePrice;
        }

        /// <summary>
        /// Bases the price for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public double BasePriceForRow(UPSEOrderRow row)
        {
            UPSEPrice pricing = this.PricingForRow(row);
            return pricing?.UnitPrice ?? 0;
        }

        /// <summary>
        /// Updateds the root record with template filter.
        /// </summary>
        /// <param name="templateFilter">The template filter.</param>
        /// <returns></returns>
        public override UPCRMRecord UpdatedRootRecordWithTemplateFilter(UPConfigFilter templateFilter)
        {
            UPCRMRecord record = base.UpdatedRootRecordWithTemplateFilter(templateFilter);
            if (record == null)
            {
                return null;
            }

            if (this.DestRootFieldControl != null)
            {
                record.Mode = "Update";
                foreach (FieldControlTab tab in this.DestRootFieldControl.Tabs)
                {
                    foreach (UPConfigFieldControlField field in tab.Fields)
                    {
                        string value = null;
                        if (field.Attributes.Dontsave && field.Attributes.DontcacheOffline)
                        {
                            continue;
                        }

                        switch (field.Function)
                        {
                            case "Quantity":
                                value = this.UnitCount.ToString();
                                break;

                            case "EndPrice":
                                value = this.EndPrice.ToString("##.00");
                                break;

                            case "FreeGoods":
                                value = this.FreeGoodsCount.ToString();
                                break;

                            case "NetPrice":
                                value = this.NetPrice.ToString("##.00");
                                break;

                            case "Rebate":
                            case "Discount":
                                double rebate = 0;
                                double endPrice = this.EndPrice;
                                if (endPrice > 0.0001)
                                {
                                    rebate = 1 - (this.NetPrice / endPrice);
                                }

                                value = rebate.ToString("##.00000");
                                break;

                            default:
                                continue;
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            record.AddValue(new UPCRMFieldValue(value, this.DestRootFieldControl.InfoAreaId, field.FieldId, field.Attributes.Dontsave));
                        }
                    }
                }
            }

            return record;
        }

        /// <summary>
        /// Units the count for rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public int UnitCountForRows(List<UPSERow> rows)
        {
            return rows.Cast<UPSEOrderRow>().Sum(row => row.UnitCount);
        }

        /// <summary>
        /// Frees the goods count for rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public int FreeGoodsCountForRows(List<UPSERow> rows)
        {
            return rows.Cast<UPSEOrderRow>().Sum(row => row.FreeGoodsCount);
        }

        /// <summary>
        /// Ends the price for rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public double EndPriceForRows(List<UPSERow> rows)
        {
            return rows.Cast<UPSEOrderRow>().Sum(row => row.EndPrice);
        }

        /// <summary>
        /// Nets the price for rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public double NetPriceForRows(List<UPSERow> rows)
        {
            return rows.Cast<UPSEOrderRow>().Sum(row => row.NetPrice);
        }

        /// <summary>
        /// Frees the goods price for rows.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public double FreeGoodsPriceForRows(List<UPSERow> rows)
        {
            return rows.Cast<UPSEOrderRow>().Sum(row => row.FreeGoodsPrice);
        }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public override UPOfflineSerialEntryRequest OfflineRequest
        {
            get
            {
                if (this.offlineRequest == null)
                {
                    ViewReference viewReference = this.Parameters["viewReference"] as ViewReference;
                    this.offlineRequest = new UPOfflineSerialEntryOrderRequest(viewReference);

                    if (!this.ConflictHandling)
                    {
                        this.offlineRequest.RelatedInfoDictionary = this.InitialFieldValuesForDestination;
                        UPOfflineStorage.DefaultStorage.BlockingRequest = this.offlineRequest;
                    }
                }

                return this.offlineRequest;
            }
        }
    }
}
