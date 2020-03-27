// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MDocumentSection.cs" company="Aurea Software Gmbh">
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
//   Document Section
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Document Section
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMContainer" />
    public class UPMDocumentSection : UPMContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMContainer"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMDocumentSection(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public UPMStringField GroupName { get; set; }
    }
}
