// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSEPosition.cs" company="Aurea Software Gmbh">
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
//   UPMSEPosition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Fields.SerialEntry;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// UPMSEPosition
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMContainer" />
    public class UPMSEPosition : UPMContainer
    {
        private Dictionary<string, UPMSEPositionInfoMessage> mutableInfoMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSEPosition"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSEPosition(IIdentifier identifier)
            : base(identifier)
        {
            this.mutableInfoMessages = new Dictionary<string, UPMSEPositionInfoMessage>();
        }

        /// <summary>
        /// Gets or sets the third row title.
        /// </summary>
        /// <value>
        /// The third row title.
        /// </value>
        public UPMStringField ThirdRowTitle { get; set; }

        /// <summary>
        /// Gets or sets the third row currency.
        /// </summary>
        /// <value>
        /// The third row currency.
        /// </value>
        public UPMStringField ThirdRowCurrency { get; set; }

        /// <summary>
        /// Gets or sets the third row value.
        /// </summary>
        /// <value>
        /// The third row value.
        /// </value>
        public UPMStringField ThirdRowValue { get; set; }

        /// <summary>
        /// Gets or sets the initial focus field.
        /// </summary>
        /// <value>
        /// The initial focus field.
        /// </value>
        public UPMField InitialFocusField { get; set; }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public UPSERow Row => null;

        /// <summary>
        /// Gets the information messages.
        /// </summary>
        /// <value>
        /// The information messages.
        /// </value>
        public Dictionary<string, UPMSEPositionInfoMessage> InfoMessages => this.mutableInfoMessages;

        /// <summary>
        /// Gets or sets the title field.
        /// </summary>
        /// <value>
        /// The title field.
        /// </value>
        public UPMStringField TitleField { get; set; }

        /// <summary>
        /// Gets or sets the subtitle field.
        /// </summary>
        /// <value>
        /// The subtitle field.
        /// </value>
        public UPMStringField SubtitleField { get; set; }

        /// <summary>
        /// Gets or sets the price field.
        /// </summary>
        /// <value>
        /// The price field.
        /// </value>
        public UPMStringField PriceField { get; set; }

        /// <summary>
        /// Gets or sets the total price field.
        /// </summary>
        /// <value>
        /// The total price field.
        /// </value>
        public UPMStringField TotalPriceField { get; set; }

        /// <summary>
        /// Gets or sets the total price currency field.
        /// </summary>
        /// <value>
        /// The total price currency field.
        /// </value>
        public UPMStringField TotalPriceCurrencyField { get; set; }

        /// <summary>
        /// Gets or sets the total quantity field.
        /// </summary>
        /// <value>
        /// The total quantity field.
        /// </value>
        public UPMStringField TotalQuantityField { get; set; }

        /// <summary>
        /// Gets or sets the image field.
        /// </summary>
        /// <value>
        /// The image field.
        /// </value>
        public UPMImageEditField ImageField { get; set; }

        //public UIImage Icon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [position selected].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [position selected]; otherwise, <c>false</c>.
        /// </value>
        public bool PositionSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [server approved].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [server approved]; otherwise, <c>false</c>.
        /// </value>
        public bool ServerApproved { get; set; }

        /// <summary>
        /// Gets or sets the position error.
        /// </summary>
        /// <value>
        /// The position error.
        /// </value>
        public Exception PositionError { get; set; }

        /// <summary>
        /// Gets or sets the liste edit boolean field.
        /// </summary>
        /// <value>
        /// The liste edit boolean field.
        /// </value>
        public UPMSerialEntryBooleanEditField ListeEditBooleanField { get; set; }

        /// <summary>
        /// Gets or sets the liste edit quantity field.
        /// </summary>
        /// <value>
        /// The liste edit quantity field.
        /// </value>
        public UPMSerialEntryNumberEditField ListeEditQuantityField { get; set; }

        /// <summary>
        /// Gets the detail input groups.
        /// </summary>
        /// <value>
        /// The detail input groups.
        /// </value>
        public List<UPMElement> DetailInputGroups => this.Children;

        /// <summary>
        /// Adds the detail input group.
        /// </summary>
        /// <param name="detailGroup">The detail group.</param>
        public void AddDetailInputGroup(UPMGroup detailGroup)
        {
            this.AddChild(detailGroup);
        }

        /// <summary>
        /// Removes all detail input groups.
        /// </summary>
        public void RemoveAllDetailInputGroups()
        {
            this.Children.Clear();
        }

        /// <summary>
        /// Sets the information message for key.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The key.</param>
        public void SetInfoMessageForKey(UPMSEPositionInfoMessage message, string key)
        {
            this.mutableInfoMessages[key] = message;
        }

        /// <summary>
        /// Informations the message for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UPMSEPositionInfoMessage InfoMessageForKey(string key)
        {
            return this.mutableInfoMessages[key];
        }

        /// <summary>
        /// Removes the information messages for key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void RemoveInfoMessagesForKey(string key)
        {
            this.mutableInfoMessages.Remove(key);
        }

        /// <summary>
        /// Gets the or create information message for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UPMSEPositionInfoMessage GetOrCreateInfoMessageForKey(string key)
        {
            UPMSEPositionInfoMessage stepSizeInfoMessage = this.InfoMessageForKey(key);
            if (stepSizeInfoMessage == null)
            {
                stepSizeInfoMessage = new UPMSEPositionInfoMessage(StringIdentifier.IdentifierWithStringId(key));
                this.SetInfoMessageForKey(stepSizeInfoMessage, key);
            }

            return stepSizeInfoMessage;
        }
    }
}
