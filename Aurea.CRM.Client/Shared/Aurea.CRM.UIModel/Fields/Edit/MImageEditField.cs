// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MImageEditField.cs" company="Aurea Software Gmbh">
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
//   UI control for editing an image field value
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// UI control for editing an image field value
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMImageEditField : UPMEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMImageEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMImageEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the name of the explicit file.
        /// </summary>
        /// <value>
        /// The name of the explicit file.
        /// </value>
        public string ExplicitFileName { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the image document.
        /// </summary>
        /// <value>
        /// The image document.
        /// </value>
        public UPMDocument ImageDocument { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the image document.
        /// </summary>
        /// <value>
        /// The maximum size of the image document.
        /// </value>
        public Size ImageDocumentMaxSize { get; set; }
    }
}
