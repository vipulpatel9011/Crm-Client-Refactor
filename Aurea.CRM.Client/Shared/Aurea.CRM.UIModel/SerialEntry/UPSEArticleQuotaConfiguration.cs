// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEArticleQuotaConfiguration.cs" company="Aurea Software Gmbh">
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
//   UPSEArticleQuotaConfiguration
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSEArticleQuotaConfiguration
    /// </summary>
    public class UPSEArticleQuotaConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEArticleQuotaConfiguration"/> class.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="valueDictionary">The value dictionary.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public UPSEArticleQuotaConfiguration(string itemNumber, Dictionary<string, object> valueDictionary, string recordIdentification)
        {
            this.ItemNumber = itemNumber;
            this.RecordIdentification = recordIdentification;
            this.ValueDictionary = valueDictionary;
            string launchDateString = this.ValueDictionary.ValueOrDefault("LaunchDate") as string;
            if (launchDateString != null)
            {
                this.LaunchDate = launchDateString.DateFromCrmValue().GetValueOrDefault();
            }

            string limitedString = this.ValueDictionary.ValueOrDefault("Limited") as string;
            if (limitedString == "false")
            {
                this.Unlimited = true;
            }
        }

        /// <summary>
        /// Gets the value dictionary.
        /// </summary>
        /// <value>
        /// The value dictionary.
        /// </value>
        public Dictionary<string, object> ValueDictionary { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the item number.
        /// </summary>
        /// <value>
        /// The item number.
        /// </value>
        public string ItemNumber { get; private set; }

        /// <summary>
        /// Gets the launch date.
        /// </summary>
        /// <value>
        /// The launch date.
        /// </value>
        public DateTime LaunchDate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSEArticleQuotaConfiguration"/> is unlimited.
        /// </summary>
        /// <value>
        ///   <c>true</c> if unlimited; otherwise, <c>false</c>.
        /// </value>
        public bool Unlimited { get; private set; }

        /// <summary>
        /// Quotas the per period with default.
        /// </summary>
        /// <param name="defaultQuota">The default quota.</param>
        /// <returns></returns>
        public int QuotaPerPeriodWithDefault(int defaultQuota)
        {
            string value = this.ValueDictionary.ValueOrDefault("Quota") as string;
            int quota = Convert.ToInt32(value);
            return quota > 0 ? quota : defaultQuota;
        }
    }
}
