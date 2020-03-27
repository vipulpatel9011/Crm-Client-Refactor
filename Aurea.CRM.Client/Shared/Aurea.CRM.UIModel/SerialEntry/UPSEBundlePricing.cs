// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEBundlePricing.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Bundle Pricing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Serial Entry Bundle Pricing
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSEPricingConditionBase" />
    public class UPSEBundlePricing : UPSEPricingConditionBase
    {
        private Dictionary<string, string> articleRecordIds;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingConditionBase"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="dataDictionary">The data dictionary.</param>
        /// <param name="pricingSet">The pricing set.</param>
        public UPSEBundlePricing(string recordIdentification, Dictionary<string, object> dataDictionary, UPSEPricingSet pricingSet)
            : base(recordIdentification, dataDictionary, pricingSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingConditionBase"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="pricingSet">The pricing set.</param>
        public UPSEBundlePricing(UPCRMResultRow row, FieldControl fieldControl, UPSEPricingSet pricingSet)
            : base(row, fieldControl, pricingSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEBundlePricing"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="pricing">The pricing.</param>
        public UPSEBundlePricing(UPCRMResultRow row, FieldControl fieldControl, UPSEPricing pricing)
            : base(row, fieldControl, pricing)
        {
        }

        /// <summary>
        /// Gets the other positions.
        /// </summary>
        /// <value>
        /// The other positions.
        /// </value>
        public override List<UPSERow> OtherPositions => null;

        /// <summary>
        /// Bundles the pricing for row record identifier positions.
        /// </summary>
        /// <param name="rowRecordId">The row record identifier.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public UPSEComputedBundlePricing BundlePricingForRowRecordIdPositions(string rowRecordId, List<UPSERow> rows)
        {
            return new UPSEComputedBundlePricing(this, rowRecordId, rows);
        }

        /// <summary>
        /// Adds the article record identifier.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        public bool AddArticleRecordId(string recordId)
        {
            if (this.articleRecordIds == null)
            {
                this.articleRecordIds = new Dictionary<string, string> { { recordId, recordId } };
                return true;
            }

            if (this.articleRecordIds.ContainsKey(recordId))
            {
                return false;
            }

            this.articleRecordIds[recordId] = recordId;
            return true;
        }
    }
}
