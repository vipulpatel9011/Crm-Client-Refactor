// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizerManagerOrganizerBarDelegate.cs" company="Aurea Software Gmbh">
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
//   The Organizer Manager Organizer Bar Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    /// <summary>
    /// The Organizer Manager Organizer Bar Delegate
    /// </summary>
    public interface IOrganizerManagerOrganizerBarDelegate
    {
        /// <summary>
        /// Wills the leave organizer with nav controller identifier.
        /// </summary>
        /// <param name="oldId">The old identifier.</param>
        void WillLeaveOrganizerWithNavControllerId(int oldId);
    }
}
