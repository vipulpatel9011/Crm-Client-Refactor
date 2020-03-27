// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MPhoneField.cs" company="Aurea Software Gmbh">
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
//   UI control to show a phone number
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UI control to show a phone number
    /// </summary>
    /// <seealso cref="UPMStringField" />
    public class UPMPhoneField : UPMStringField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMPhoneField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMPhoneField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use telprompt].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use telprompt]; otherwise, <c>false</c>.
        /// </value>
        public bool UseTelprompt { get; set; }
    }
}
