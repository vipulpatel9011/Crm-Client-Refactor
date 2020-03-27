// <copyright file="UPRightsCheckerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;

    using Aurea.CRM.Core.CRM.Features;

    /// <summary>
    /// Rights checker delegate
    /// </summary>
    public interface UPRightsCheckerDelegate
    {
        /// <summary>
        /// Rights the checker grants permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        void RightsCheckerGrantsPermission(UPRightsChecker sender, string recordIdentification);

        /// <summary>
        /// Rights the checker revoke permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        void RightsCheckerRevokePermission(UPRightsChecker sender, string recordIdentification);

        /// <summary>
        /// Rights the checker did finish with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        void RightsCheckerDidFinishWithError(UPRightsChecker sender, Exception error);
    }
}
