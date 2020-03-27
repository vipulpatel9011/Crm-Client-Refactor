// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMTriggerActionGroup.cs" company="Aurea Software Gmbh">
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
//   Trigger Action Group implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Trigger Action Group
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMTriggerActionGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMTriggerActionGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMTriggerActionGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        /// <value>
        /// The button text.
        /// </value>
        public string ButtonText { get; set; }

        /// <summary>
        /// Gets or sets the execution note.
        /// </summary>
        /// <value>
        /// The execution note.
        /// </value>
        public string ExecutionNote { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [HTML mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [HTML mode]; otherwise, <c>false</c>.
        /// </value>
        public bool HtmlMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMTriggerActionGroup"/> is executing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if executing; otherwise, <c>false</c>.
        /// </value>
        public bool Executing { get; set; }

        /// <summary>
        /// Gets or sets the executing text.
        /// </summary>
        /// <value>
        /// The executing text.
        /// </value>
        public string ExecutingText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMTriggerActionGroup"/> is disable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public bool Disable { get; set; }

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>
        /// The error text.
        /// </value>
        public string ErrorText { get; set; }

        /// <summary>
        /// Gets or sets the done text.
        /// </summary>
        /// <value>
        /// The done text.
        /// </value>
        public string DoneText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMTriggerActionGroup"/> is done.
        /// </summary>
        /// <value>
        ///   <c>true</c> if done; otherwise, <c>false</c>.
        /// </value>
        public bool Done { get; set; }

        /// <summary>
        /// Gets or sets the retry text.
        /// </summary>
        /// <value>
        /// The retry text.
        /// </value>
        public string RetryText { get; set; }
    }
}
