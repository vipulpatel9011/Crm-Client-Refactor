// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MEditPage.cs" company="Aurea Software Gmbh">
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
//   Base edit page implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Pages
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// Base edit page implementation
    /// </summary>
    /// <seealso cref="Page" />
    public class EditPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditPage"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public EditPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool HasChanges
        {
            get
            {
                if (this.Groups == null)
                {
                    return false;
                }

                foreach (UPMStandardEditGroup editGroup in this.Groups)
                {
                    if (editGroup.HasChanges)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [qr code beep enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [qr code beep enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool QrCodeBeepEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [qr code controls enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [qr code controls enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool QrCodeControlsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [qr code enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [qr code enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool QrCodeEnabled { get; set; }
    }
}
