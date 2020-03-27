// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSEPositionInfoMessage.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Position Info Message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Serial Entry Position Info Message
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMSEPositionInfoMessage : UPMElement
    {
        /// <summary>
        /// Gets or sets the message field.
        /// </summary>
        /// <value>
        /// The message field.
        /// </value>
        public UPMStringField MessageField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [error level message].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [error level message]; otherwise, <c>false</c>.
        /// </value>
        public bool ErrorLevelMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSEPositionInfoMessage"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSEPositionInfoMessage(IIdentifier identifier)
                    : base(identifier)
        {
            this.MessageField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{identifier} MessageField"));
            this.ErrorLevelMessage = false;
        }
    }
}
