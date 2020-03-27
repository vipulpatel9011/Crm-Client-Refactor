// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmParticipant.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Max Menezes
// </author>
// <summary>
//   Query configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    /// <summary>
    /// CRM Participant
    /// </summary>
    public class UPCRMParticipant
    {
        private string requirementText;
        private string acceptanceText;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name => this.Key;

        /// <summary>
        /// Gets or sets the requirement text.
        /// </summary>
        /// <value>
        /// The requirement text.
        /// </value>
        public virtual string RequirementText
        {
            get
            {
                return this.requirementText;
            }

            set
            {
                this.requirementText = value;
                this.NoUserChanges = false;
            }
        }

        /// <summary>
        /// Gets or sets the acceptance text.
        /// </summary>
        /// <value>
        /// The acceptance text.
        /// </value>
        public string AcceptanceText
        {
            get
            {
                return this.acceptanceText;
            }

            set
            {
                this.acceptanceText = value;
                this.NoUserChanges = false;
            }
        }

        /// <summary>
        /// Gets or sets the original requirement text.
        /// </summary>
        /// <value>
        /// The original requirement text.
        /// </value>
        public string OriginalRequirementText { get; set; }

        /// <summary>
        /// Gets or sets the original acceptance text.
        /// </summary>
        /// <value>
        /// The original acceptance text.
        /// </value>
        public string OriginalAcceptanceText { get; set; }

        /// <summary>
        /// Gets the requirement display text.
        /// </summary>
        /// <value>
        /// The requirement display text.
        /// </value>
        public string RequirementDisplayText => !string.IsNullOrEmpty(this.requirementText) ? this.Context.TextForRequirementCodeString(this.requirementText) : null;

        /// <summary>
        /// Gets the acceptance display text.
        /// </summary>
        /// <value>
        /// The acceptance display text.
        /// </value>
        public string AcceptanceDisplayText => !string.IsNullOrEmpty(this.acceptanceText) ? this.Context.TextForAcceptanceCodeString(this.acceptanceText) : null;

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMParticipant"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Empty => false;

        /// <summary>
        /// Gets a value indicating whether this instance can change acceptance state.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can change acceptance state; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanChangeAcceptanceState => false;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public UPCRMParticipants Context { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [may not be deleted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [may not be deleted]; otherwise, <c>false</c>.
        /// </value>
        public bool MayNotBeDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [mark as deleted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mark as deleted]; otherwise, <c>false</c>.
        /// </value>
        public bool MarkAsDeleted { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public Dictionary<string, object> Options { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [no user changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no user changes]; otherwise, <c>false</c>.
        /// </value>
        public bool NoUserChanges { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMParticipant"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public UPCRMParticipant(string key)
        {
            this.Key = key;
        }
    }
}
