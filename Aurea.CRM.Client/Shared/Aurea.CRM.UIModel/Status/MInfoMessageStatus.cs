// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MInfoMessageStatus.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Status view for information message
    /// </summary>
    /// <seealso cref="UPMMessageStatus" />
    public class UPMInfoMessageStatus : UPMMessageStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMInfoMessageStatus"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMInfoMessageStatus(IIdentifier identifier)
            : base(identifier)
        {
        }
    }
}
