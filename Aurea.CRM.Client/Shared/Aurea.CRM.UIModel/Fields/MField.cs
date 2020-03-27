// <copyright file="MField.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// Font Style
    /// </summary>
    public enum UPMFontStyle
    {
        /// <summary>
        /// Plain
        /// </summary>
        Plain,

        /// <summary>
        /// Bold
        /// </summary>
        Bold,

        /// <summary>
        /// Italic
        /// </summary>
        Italic
    }

    /// <summary>
    /// Base implementation of showing field values
    /// </summary>
    /// <seealso cref="UPMElement" />
    public class UPMField : UPMElement
    {
        private bool hidden;
        private string labelText;
        private string detailText;
        private string externalKey;
        private object fieldValue;
        private FieldAttributes fieldAttributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMField(IIdentifier identifier)
            : base(identifier)
        {
            this.hidden = false;
        }

        /// <summary>
        /// Gets or sets the GUI field.
        /// </summary>
        /// <value>
        /// The GUI field.
        /// </value>
        public IGUIField GUIField { get; set; }

        /// <summary>
        /// Gets or sets the field attributes.
        /// </summary>
        /// <value>
        /// The field attributes.
        /// </value>
        public FieldAttributes FieldAttributes
        {
            get
            {
                return this.fieldAttributes;
            }

            set
            {
                this.fieldAttributes = value;
                this.GUIField?.SetAttributes(this.fieldAttributes);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPMField"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Empty => this.FieldValue == null;

        /// <summary>
        /// Gets a value indicating whether this instance can become first responder.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can become first responder; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanBecomeFirstResponder => false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMField"/> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Hidden
        {
            get
            {
                return this.hidden;
            }

            set
            {
                this.hidden = value;
                if (this.GUIField != null)
                {
                    this.GUIField.Hidden = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        public virtual string LabelText
        {
            get
            {
                return this.labelText;
            }

            set
            {
                this.labelText = value;
                if (this.GUIField != null)
                {
                    this.GUIField.LabelText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the detail text.
        /// </summary>
        /// <value>
        /// The detail text.
        /// </value>
        public virtual string DetailText
        {
            get
            {
                return this.detailText;
            }

            set
            {
                this.detailText = value;
                if (this.GUIField != null)
                {
                    this.GUIField.DetailText = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public virtual object FieldValue
        {
            get
            {
                return this.fieldValue;
            }

            set
            {
                this.fieldValue = value;
                if (this.GUIField != null)
                {
                    this.GUIField.FieldValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the edit field context.
        /// </summary>
        /// <value>
        /// The edit field context.
        /// </value>
        public object EditFieldContext { get; set; }

        /// <summary>
        /// Gets or sets the external key.
        /// </summary>
        /// <value>
        /// The external key.
        /// </value>
        public virtual string ExternalKey
        {
            get
            {
                return this.externalKey;
            }

            set
            {
                this.externalKey = value;
                if (this.GUIField != null)
                {
                    this.GUIField.ExternalKey = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the string value.
        /// Used in MiniDetails to display in 2 Columns and more Lines
        /// </summary>
        /// <value>
        /// The string value.
        /// </value>
        public virtual string StringValue
        {
            get
            {
                return Convert.ToString(this.FieldValue);
            }

            set
            {
                this.FieldValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether new line should be added
        /// </summary>
        public bool NewLine { get; set; }

        /// <summary>
        /// Gets or sets the listing source value.
        /// </summary>
        /// <value>
        /// The listing source value.
        /// </value>
        public virtual string ListingSourceValue { get; set; }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        /// <param name="appliedIdentifiers">The applied identifiers.</param>
        public override void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// Sets the attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        public void SetAttributes(FieldAttributes attributes)
        {
            this.FieldAttributes = attributes;
        }
    }
}
