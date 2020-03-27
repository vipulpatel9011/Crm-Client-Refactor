// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChildGpsEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for child Gps field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Edit field context for child Gps field
    /// </summary>
    /// <seealso cref="UPChildEditFieldContext" />
    public class UPChildGpsEditFieldContext : UPChildEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildGpsEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPChildGpsEditFieldContext(UPConfigFieldControlField fieldConfig, string value)
            : base(fieldConfig, value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value
            => this.parentContext is UPGpsEditFieldContext ? ((UPGpsEditFieldContext)this.parentContext).YValue : "0";

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            (this.parentContext as UPGpsEditFieldContext)?.SetYValue(value);
        }
    }
}
