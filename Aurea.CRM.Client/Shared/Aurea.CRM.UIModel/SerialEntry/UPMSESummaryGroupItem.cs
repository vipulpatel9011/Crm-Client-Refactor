// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSESummaryGroupItem.cs" company="Aurea Software Gmbh">
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
//   UPMSESummaryGroupItem
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UPMSESummaryGroupItem
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMSESummaryGroupItem : UPMElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSESummaryGroupItem"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSESummaryGroupItem(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>
        /// The currency.
        /// </value>
        public string Currency { get; set; }

        /// <summary>
        /// Updates the with title value currency.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="value">The value.</param>
        /// <param name="currency">The currency.</param>
        public void UpdateWithTitleValueCurrency(string title, string value, string currency)
        {
            this.Title = title ?? string.Empty;
            this.Value = value ?? string.Empty;
            this.Currency = currency ?? string.Empty;
        }

        /// <summary>
        /// Updates the with title items.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="count">The count.</param>
        public void UpdateWithTitleItems(string title, int count)
        {
            var itemString = count > 0 ? count.ToString() : string.Empty;

            this.UpdateWithTitleValueCurrency(title, itemString, null);
        }
    }
}
