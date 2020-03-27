// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChildCatalogEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for child catalog field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Edit field context for child catalog field
    /// </summary>
    /// <seealso cref="UPChildEditFieldContext" />
    public class UPChildCatalogEditFieldContext : UPChildEditFieldContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChildCatalogEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPChildCatalogEditFieldContext(UPConfigFieldControlField fieldConfig, string value)
            : base(fieldConfig, value)
        {
        }

        /// <summary>
        /// Gets or sets the index of the array.
        /// </summary>
        /// <value>
        /// The index of the array.
        /// </value>
        public int ArrayIndex { get; set; }

        /// <summary>
        /// Gets or sets the root edit field context.
        /// </summary>
        /// <value>
        /// The root edit field context.
        /// </value>
        public UPCatalogEditFieldContext RootEditFieldContext { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value
            =>
                this.RootEditFieldContext == null
                    ? this.OriginalValue
                    : this.RootEditFieldContext.CatalogValueAtPosition(this.ArrayIndex);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            this.RootEditFieldContext.SetCatalogValueAtPosition(value, this.ArrayIndex);
            base.SetValue(value);
        }
    }
}
