// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MWarnStatus.cs" company="Aurea Software Gmbh">
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
//   Basic status view implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Status
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Status view for a warning
    /// </summary>
    /// <seealso cref="UPMMessageStatus" />
    public class UPMWarnStatus : UPMMessageStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMWarnStatus"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMWarnStatus(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Warns the status with message details.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        /// <returns>
        /// The <see cref="UPMWarnStatus"/>.
        /// </returns>
        public static UPMWarnStatus WarnStatusWithMessageDetails(string message, string details)
        {
            var status = new UPMWarnStatus(StringIdentifier.IdentifierWithStringId("warning"));
            status.SetMessageDetails(message, details);
            return status;
        }
    }
}
