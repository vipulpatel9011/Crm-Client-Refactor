// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMDependsEditField.cs" company="Aurea">
// Copyright (c) 2016 Aurea. All rights reserved. 
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// UI control for editing a depends field value
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMDependsEditField : UPMEditField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDependsEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMDependsEditField(IIdentifier identifier)
            : base(identifier)
        {
            this.InitialSelectableOnly = false;
            this.Deletable = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [initial selectable only].
        /// </summary>
        /// <value>
        /// <c>true</c> if [initial selectable only]; otherwise, <c>false</c>.
        /// </value>
        public bool InitialSelectableOnly { get; set; }

        /// <summary>
        /// Gets or sets the main field.
        /// </summary>
        /// <value>
        /// The main field.
        /// </value>
        public UPMEditField MainField { get; set; }

        /// <summary>
        /// Gets or sets the depend field.
        /// </summary>
        /// <value>
        /// The depend field.
        /// </value>
        public UPMEditField DependField { get; set; }

        /// <summary>
        /// Gets or sets the depend field2.
        /// </summary>
        /// <value>
        /// The depend field2.
        /// </value>
        public UPMEditField DependField2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMDependsEditField"/> is deletable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deletable; otherwise, <c>false</c>.
        /// </value>
        public bool Deletable { get; set; }

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <value>
        /// The string display value.
        /// </value>
        public override string StringDisplayValue => this.MainField?.StringDisplayValue;

        /// <summary>
        /// Gets the string edit value.
        /// </summary>
        /// <value>
        /// The string edit value.
        /// </value>
        public override string StringEditValue => this.MainField?.StringEditValue;

        /// <summary>
        /// List of Self for Participant List
        /// </summary>
        public ObservableCollection<UPMDependsEditField> FieldList { get; set;}

    }
}
