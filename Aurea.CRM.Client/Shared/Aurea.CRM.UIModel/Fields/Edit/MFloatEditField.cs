// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MFloatEditField.cs" company="Aurea Software Gmbh">
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
//   UI control for editing a float value field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UI control for editing a float value field
    /// </summary>
    /// <seealso cref="UPMNumberEditField" />
    public class UPMFloatEditField : UPMNumberEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMFloatEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMFloatEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets the number formatter display.
        /// </summary>
        /// <value>
        /// The number formatter display.
        /// </value>
        public static string NumberFormatterDisplay => "D2";

        /// <summary>
        /// Gets the number formatter edit.
        /// </summary>
        /// <value>
        /// The number formatter edit.
        /// </value>
        public static string NumberFormatterEdit => "D";
    }
}
