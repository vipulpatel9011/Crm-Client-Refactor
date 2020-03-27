// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMOrganizerAction.cs" company="Aurea Software Gmbh">
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
//   The organizer action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The UPM organizer action.
    /// </summary>
    public class UPMOrganizerAction : UPMAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMOrganizerAction"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMOrganizerAction(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the active icon name.
        /// </summary>
        public string ActiveIconName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether aktive.
        /// </summary>
        public bool Aktive { get; set; }

        /// <summary>
        /// Gets or sets the view reference.
        /// </summary>
        public ViewReference ViewReference { get; set; }

        /// <summary>
        /// Creates action dictionary by given parameters
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="action">Action object</param>
        /// <returns><see cref="Dictionary{TKey, TValue}"/></returns>
        public static Dictionary<string, object> OrganizerActionDictionaryWithSender(object sender, UPMOrganizerAction action)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                { Core.Constants.OrganizerActionSender, sender },
                { Core.Constants.OrganizerAction, action }
            };
            return dictionary;
        }

        /// <inheritdoc />
        public override void PerformAction(object sender)
        {
            base.PerformAction(OrganizerActionDictionaryWithSender(sender, this));
        }
    }
}