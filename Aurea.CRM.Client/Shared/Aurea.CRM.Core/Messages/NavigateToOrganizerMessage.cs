// <copyright file="NavigateToOrganizerMessage.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Azat Jalilov
// </author>
// <summary>
//   Message to navigate to organizer
// </summary>

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Message to navigate to organizer
    /// </summary>
    public class NavigateToOrganizerMessage : INotificationMessage
    {
        /// <summary>
        /// Navigate back token
        /// </summary>
        public static readonly string NavigateBack = "NavigateBack";

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigateToOrganizerMessage"/> class.
        /// </summary>
        /// <param name="organizerModelController">Model controller to navigate</param>
        /// <param name="parameter">Parameter to pass</param>
        /// <param name="switchView">Switch ui view</param>
        /// <param name="setAsPrevious">set current organizer as previous</param>
        public NavigateToOrganizerMessage(object organizerModelController, object parameter, bool switchView = true, bool setAsPrevious = false)
        {
            this.OrganizerModelController = organizerModelController;
            this.Parameter = parameter;
            this.SwitchView = switchView;
            this.SetCurrentOrganizerAsPrevious = setAsPrevious;
        }

        /// <summary>
        /// Gets model controller
        /// </summary>
        public object OrganizerModelController { get; private set; }

        /// <summary>
        /// Gets passed parameter
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it should switch view
        /// </summary>
        public bool SwitchView { get; }

         /// <summary>
        /// Gets a value indicating whether navigation occurs without organizer change
        /// </summary>
        public bool SetCurrentOrganizerAsPrevious { get; }
    }
}
