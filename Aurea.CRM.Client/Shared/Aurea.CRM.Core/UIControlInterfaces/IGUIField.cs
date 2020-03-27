// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGUIField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The GUIField interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.UIControlInterfaces
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// The GUIField interface.
    /// </summary>
    public interface IGUIField
    {
        /// <summary>
        /// Gets or sets the detail text.
        /// </summary>
        string DetailText { get; set; }

        /// <summary>
        /// Gets or sets the external key.
        /// </summary>
        string ExternalKey { get; set; }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        object FieldValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hidden.
        /// </summary>
        bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        string LabelText { get; set; }

        /// <summary>
        /// Gets or sets the string value.
        /// </summary>
        string StringValue { get; set; }

        /// <summary>
        /// The remove label.
        /// </summary>
        void RemoveLabel();

        /// <summary>
        /// The set attributes.
        /// </summary>
        /// <param name="attributes">
        /// The attributes.
        /// </param>
        void SetAttributes(FieldAttributes attributes);
    }
}
