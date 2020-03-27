// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Aurea Software Gmbh">
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
//   Defines global constants
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core
{
    /// <summary>
    /// Defines global constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The upcrm session error domain
        /// </summary>
        public const string UPCRMSessionErrorDomain = "UPCRMSessionErrorDomain";

        /// <summary>
        /// The upcrmsession fullsync expiration seconds
        /// </summary>
        public const int UpcrmsessionFullsyncExpirationSeconds = 43200;

        /// <summary>
        /// The upm user change on offline notification
        /// </summary>
        public const string UPMUserChangeOnOfflineNotification = "UPMUserChangeOnOfflineNotification";

        /// <summary>
        /// Up text no
        /// </summary>
        public const string UpTextNo = "No";

        /// <summary>
        /// Up text yes
        /// </summary>
        public const string UpTextYes = "Yes";

        /// <summary>
        /// The organizer action
        /// </summary>
        public const string OrganizerAction = "UPMOrganizerAction";

        /// <summary>
        /// The organizer action sender
        /// </summary>
        public const string OrganizerActionSender = "UPMOrganizerActionSender";

        /// <summary>
        /// The action identifier toggle favorite
        /// </summary>
        public const string ActionIdToggleFavorite = "action.ToggleFavorite";
    }
}
