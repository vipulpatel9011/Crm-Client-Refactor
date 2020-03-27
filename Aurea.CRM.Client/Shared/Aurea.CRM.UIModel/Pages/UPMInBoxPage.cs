// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMInBoxPage.cs" company="Aurea Software Gmbh">
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
//   The InBox Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// Inbox Page
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMInBoxPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMInBoxPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the upload field group.
        /// </summary>
        /// <value>
        /// The upload field group.
        /// </value>
        public UPMGroup UploadFieldGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [skip upload if possible].
        /// </summary>
        /// <value>
        /// <c>true</c> if [skip upload if possible]; otherwise, <c>false</c>.
        /// </value>
        public bool SkipUploadIfPossible { get; set; }

        /// <summary>
        /// Gets or sets the file name edit field.
        /// </summary>
        /// <value>
        /// The file name edit field.
        /// </value>
        public UPMEditField FileNameEditField { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has upload edit fields.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has upload edit fields; otherwise, <c>false</c>.
        /// </value>
        public bool HasUploadEditFields => this.UploadFieldGroup.Fields.Count > 0;
    }
}
