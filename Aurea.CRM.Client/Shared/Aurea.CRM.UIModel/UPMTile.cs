// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMTile.cs" company="Aurea Software Gmbh">
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
//   The UPMTile class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// The Tile class implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMElement" />
    public class UPMTile : UPMElement
    {
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>
        /// The text field.
        /// </value>
        public UPMStringField TextField { get; set; }

        /// <summary>
        /// Gets or sets the value field.
        /// </summary>
        /// <value>
        /// The value field.
        /// </value>
        public UPMStringField ValueField { get; set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMElement"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMTile(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Text: {this.TextField.StringValue} Value: {this.ValueField.StringValue} Image: {this.ImageName}";
        }
    }
}
