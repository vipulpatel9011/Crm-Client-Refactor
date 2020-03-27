// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMDocumentsGroup.cs" company="Aurea Software Gmbh">
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
//   The Documents Group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Documents Group Style
    /// </summary>
    public enum UPMDocumentsGroupStyle
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0,

        /// <summary>
        /// Image
        /// </summary>
        Image = 1,

        /// <summary>
        /// No Images
        /// </summary>
        NoImages = 2
    }

    public class UPMDocumentsGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDocumentsGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMDocumentsGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public UPMDocumentsGroupStyle Style { get; set; }
    }
}
