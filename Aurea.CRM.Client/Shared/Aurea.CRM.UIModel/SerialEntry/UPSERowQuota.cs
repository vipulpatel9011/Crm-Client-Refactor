// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSERowQuota.cs" company="Aurea Software Gmbh">
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
//   UPSERowQuota
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPSERowQuota
    /// </summary>
    public class UPSERowQuota
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSERowQuota"/> class.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <param name="articleConfiguration">The article configuration.</param>
        /// <param name="quota">The quota.</param>
        /// <param name="quotaHandler">The quota handler.</param>
        public UPSERowQuota(string itemNumber, UPSEArticleQuotaConfiguration articleConfiguration, UPSEQuota quota, UPSEQuotaHandler quotaHandler)
        {
            this.ArticleConfiguration = articleConfiguration;
            this.Quota = quota;
            this.QuotaHandler = quotaHandler;
            this.ItemNumber = itemNumber;
            this.InitialCount = 0;
            this.CurrentCount = 0;
        }

        /// <summary>
        /// Gets the maximum quota.
        /// </summary>
        /// <value>
        /// The maximum quota.
        /// </value>
        public int MaxQuota => this.ArticleConfiguration?.QuotaPerPeriodWithDefault(this.QuotaHandler.DefaultQuotaPerYear)
            ?? this.QuotaHandler.DefaultQuotaPerYearWithoutConfiguration;

        /// <summary>
        /// Gets a value indicating whether [unlimited quota].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unlimited quota]; otherwise, <c>false</c>.
        /// </value>
        public bool UnlimitedQuota => this.ArticleConfiguration.Unlimited;

        /// <summary>
        /// Gets the item number.
        /// </summary>
        /// <value>
        /// The item number.
        /// </value>
        public string ItemNumber { get; private set; }

        /// <summary>
        /// Gets the article configuration.
        /// </summary>
        /// <value>
        /// The article configuration.
        /// </value>
        public UPSEArticleQuotaConfiguration ArticleConfiguration { get; private set; }

        /// <summary>
        /// Gets the quota.
        /// </summary>
        /// <value>
        /// The quota.
        /// </value>
        public UPSEQuota Quota { get; private set; }

        /// <summary>
        /// Gets the quota handler.
        /// </summary>
        /// <value>
        /// The quota handler.
        /// </value>
        public UPSEQuotaHandler QuotaHandler { get; private set; }

        /// <summary>
        /// Gets the initial count.
        /// </summary>
        /// <value>
        /// The initial count.
        /// </value>
        public int InitialCount { get; set; }

        /// <summary>
        /// Gets or sets the current count.
        /// </summary>
        /// <value>
        /// The current count.
        /// </value>
        public int CurrentCount { get; set; }

        /// <summary>
        /// Sets the initial count.
        /// </summary>
        /// <param name="initialCount">The initial count.</param>
        public void SetInitialCount(int initialCount)
        {
            this.InitialCount = initialCount;
            this.CurrentCount = initialCount;
        }

        /// <summary>
        /// Gets the remaining quota.
        /// </summary>
        /// <value>
        /// The remaining quota.
        /// </value>
        public int RemainingQuota => this.RemainingQuotaForCount(0);

        /// <summary>
        /// Remainings the quota for count.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int RemainingQuotaForCount(int count)
        {
            if (this.Quota.HasValidQuotaRecord)
            {
                return this.MaxQuota - this.Quota.IssuedItems + this.InitialCount - count;
            }

            if (this.Quota.PeriodCreationAllowedWithMaxPeriodCount(this.QuotaHandler.NumberOfQuotaYears))
            {
                return this.MaxQuota - count;
            }

            return 0;
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords()
        {
            if (this.CurrentCount == this.InitialCount)
            {
                return null;
            }

            int issued = this.Quota.IssuedItems + this.CurrentCount - this.InitialCount;
            return new List<UPCRMRecord> { this.Quota.OfflineRecordForCount(issued) };
        }
    }
}
