// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MBooleanEditField.cs" company="Aurea Software Gmbh">
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
//   Edit field UI control for a boolean value
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Edit field UI control for a boolean value
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMBooleanEditField : UPMEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMBooleanEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMBooleanEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [bool value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [bool value]; otherwise, <c>false</c>.
        /// </value>
        public bool BoolValue
        {
            get
            {
                return Convert.ToInt32(this.FieldValue) > 0;
            }

            set
            {
                this.FieldValue = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <value>
        /// The string display value.
        /// </value>
        public override string StringDisplayValue => this.BoolValue ? "YES" : "NO";
    }
}
