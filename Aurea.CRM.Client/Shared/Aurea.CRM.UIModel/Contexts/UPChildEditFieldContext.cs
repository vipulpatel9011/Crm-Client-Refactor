// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChildEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for a child field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Edit field context for a child field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPChildEditFieldContext : UPEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPChildEditFieldContext(UPConfigFieldControlField fieldConfig, string value)
            : base(fieldConfig, null, value, null)
        {
        }

        /// <summary>
        /// Gets the parent field identifier.
        /// </summary>
        /// <value>
        /// The parent field identifier.
        /// </value>
        public override IIdentifier ParentFieldIdentifier => this.parentContext?.FieldIdentifier;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => string.Empty;

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField() => null;

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
        }

        /// <summary>
        /// Wases the changed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool WasChanged() => this.parentContext != null && this.parentContext.WasChanged();

        /// <summary>
        /// Wases the changed by user.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool WasChangedByUser() => this.parentContext != null && this.parentContext.WasChangedByUser();
    }
}
