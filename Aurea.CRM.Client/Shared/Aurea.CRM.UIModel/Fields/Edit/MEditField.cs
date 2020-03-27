// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MEditField.cs" company="Aurea Software Gmbh">
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
//   Delegate interface for an edit field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using System.Collections.Generic;

    /// <summary>
    /// Delegate interface for an edit field
    /// </summary>
    public interface IEditFieldDelegate
    {
        /// <summary>
        /// The field changed value.
        /// </summary>
        /// <param name="editField">
        /// The edit field.
        /// </param>
        void FieldChangedValue(UPMEditField editField);
    }

    /// <summary>
    /// The edit modes of an edit field
    /// </summary>
    public enum EditFieldEditMode
    {
        /// <summary>
        /// The writeable.
        /// </summary>
        Writeable,

        /// <summary>
        /// The optional writeable.
        /// </summary>
        OptionalWriteable,

        /// <summary>
        /// The readonly.
        /// </summary>
        Readonly
    }

    /// <summary>
    /// The abstract UI control implementation of an edit field
    /// </summary>
    /// <seealso cref="UPMField" />
    public abstract class UPMEditField : UPMField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        protected UPMEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMEditField"/> is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if changed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Changed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [continuous update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [continuous update]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ContinuousUpdate { get; set; }

        /// <summary>
        /// Gets or sets the edit field delegate.
        /// </summary>
        /// <value>
        /// The edit field delegate.
        /// </value>
        public virtual IEditFieldDelegate EditFieldDelegate { get; set; }

        /// <summary>
        /// Gets or sets the edit fields context.
        /// </summary>
        /// <value>
        /// The edit fields context.
        /// </value>
        public virtual object EditFieldsContext { get; set; }

        /// <summary>
        /// Gets or sets the edit mode.
        /// </summary>
        /// <value>
        /// The edit mode.
        /// </value>
        public virtual EditFieldEditMode EditMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsEditing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [required field].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [required field]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool RequiredField { get; set; }

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <value>
        /// The string display value.
        /// </value>
        public virtual string StringDisplayValue => $"{this.FieldValue}";

        /// <summary>
        /// Gets the string edit value.
        /// </summary>
        /// <value>
        /// The string edit value.
        /// </value>
        public virtual string StringEditValue => this.StringDisplayValue;

        /// <summary>
        /// Gets the display values.
        /// </summary>
        /// <value>
        /// The display values.
        /// </value>
        public virtual List<string> DisplayValues { get; set; }
    }
}
