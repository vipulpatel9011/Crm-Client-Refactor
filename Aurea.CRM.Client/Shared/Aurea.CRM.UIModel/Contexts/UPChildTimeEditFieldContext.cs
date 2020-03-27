// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChildTimeEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for a child time field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Edit field context for a child time field
    /// </summary>
    /// <seealso cref="UPChildEditFieldContext" />
    public class UPChildTimeEditFieldContext : UPChildEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildTimeEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPChildTimeEditFieldContext(UPConfigFieldControlField fieldConfig, string value)
            : base(fieldConfig, value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.GetValue();

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            this.SetChanged(true);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetValue()
        {
            if (this.parentContext is UPDateEditFieldContext)
            {
                var dateValue = ((UPDateEditFieldContext)this.parentContext).DateValue;
                return dateValue.CrmValueFromTime();
            }

            return string.Empty;
        }
    }
}
