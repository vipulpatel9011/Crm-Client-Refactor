// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MMessageStatus.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Message status view implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Status
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Status with message
    /// </summary>
    /// <seealso cref="UPMStatus" />
    public class UPMMessageStatus : UPMStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMMessageStatus"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMMessageStatus(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the detail message field.
        /// </summary>
        /// <value>
        /// The detail message field.
        /// </value>
        public UPMStringField DetailMessageField { get; set; }

        /// <summary>
        /// Gets or sets the message field.
        /// </summary>
        /// <value>
        /// The message field.
        /// </value>
        public UPMStringField MessageField { get; set; }

        /// <summary>
        /// Messages the status with message details.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <returns>
        /// The <see cref="UPMMessageStatus"/>.
        /// </returns>
        public static UPMMessageStatus MessageStatusWithMessageDetails(string message, string details)
        {
            var status = new UPMMessageStatus(StringIdentifier.IdentifierWithStringId("message"));
            status.SetMessageDetails(message, details);
            return status;
        }

        /// <summary>
        /// Sets the message details.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        public void SetMessageDetails(string message, string details)
        {
            this.MessageField = new UPMStringField(this.Identifier) { StringValue = message };
            if (!string.IsNullOrEmpty(details))
            {
                this.Children.Clear();
                this.DetailMessageField = new UPMStringField(this.Identifier) { StringValue = details };
                this.Children.Add(this.DetailMessageField);
            }
        }
    }
}
