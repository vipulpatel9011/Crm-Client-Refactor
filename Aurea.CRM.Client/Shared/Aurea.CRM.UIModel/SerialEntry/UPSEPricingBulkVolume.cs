// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingBulkVolume.cs" company="Aurea Software Gmbh">
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
//   Pricing Bulk Volume
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Serial Entry Pricing Bulk Volume
    /// </summary>
    public class UPSEPricingBulkVolume
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBulkVolume"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="bulkVolumes">The bulk volumes.</param>
        public UPSEPricingBulkVolume(UPCRMResultRow resultRow, UPSEPricingBulkVolumes bulkVolumes)
        {
            this.BulkVolumes = bulkVolumes;
            this.MaxVolumeIndex = 0;
            List<double> quantityScale = new List<double>(10);
            this.Data = resultRow.ValuesWithFunctions();
            this.ItemNumber = this.Data.ValueOrDefault(this.BulkVolumes.ItemNumberFunctionName) as string;
            double lastNumber = 0;

            for (int i = 0; i < 10; i++)
            {
                string valueForKey = this.Data.ValueOrDefault($"Quantity{i}") as string;
                if (!string.IsNullOrEmpty(valueForKey) && valueForKey != "0")
                {
                    this.MaxVolumeIndex = i;
                    lastNumber = Convert.ToDouble(valueForKey, System.Globalization.CultureInfo.InvariantCulture);
                }

                quantityScale.Add(lastNumber);
            }

            this.QuantityScale = quantityScale;
        }

        /// <summary>
        /// Gets the bulk volumes.
        /// </summary>
        /// <value>
        /// The bulk volumes.
        /// </value>
        public UPSEPricingBulkVolumes BulkVolumes { get; private set; }

        /// <summary>
        /// Gets the quantity scale.
        /// </summary>
        /// <value>
        /// The quantity scale.
        /// </value>
        public List<double> QuantityScale { get; private set; }

        /// <summary>
        /// Gets the item number.
        /// </summary>
        /// <value>
        /// The item number.
        /// </value>
        public string ItemNumber { get; private set; }

        /// <summary>
        /// Gets the maximum index of the volume.
        /// </summary>
        /// <value>
        /// The maximum index of the volume.
        /// </value>
        public int MaxVolumeIndex { get; private set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public Dictionary<string, object> Data { get; private set; }

        /// <summary>
        /// Indexes for quantity.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public int IndexForQuantity(double quantity)
        {
            for (int i = 0; i <= this.MaxVolumeIndex; i++)
            {
                if (quantity < this.QuantityScale[i])
                {
                    return i > 0 ? i - 1 : -1;
                }
            }

            return this.MaxVolumeIndex;
        }
    }
}
