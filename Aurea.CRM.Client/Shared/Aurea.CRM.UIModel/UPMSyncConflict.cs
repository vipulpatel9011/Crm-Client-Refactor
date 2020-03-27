// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSyncConflict.cs" company="Aurea Software Gmbh">
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
//   Sync Conflict implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Status;

    public class UPMSyncConflict : UPMContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContainer"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSyncConflict(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Adds the status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void AddStatus(UPMMessageStatus status)
        {
            this.AddChild(status);
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public List<UPMElement> Status => this.Children;

        //public UIImage Icon { get; set; }     // CRM-5007

        /// <summary>
        /// Gets or sets the main field.
        /// </summary>
        /// <value>
        /// The main field.
        /// </value>
        public UPMStringField MainField { get; set; }

        /// <summary>
        /// Gets or sets error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets errorstack.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string ErrorStack { get; set; }

        /// <summary>
        /// Gets or sets the detail field.
        /// </summary>
        /// <value>
        /// The detail field.
        /// </value>
        public UPMField DetailField { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can be fixed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be fixed; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeFixed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can be reported.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be reported; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeReported { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError => !string.IsNullOrWhiteSpace(this.Error);
    }
}
