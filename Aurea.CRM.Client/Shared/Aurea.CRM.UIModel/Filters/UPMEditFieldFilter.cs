// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMEditFieldFilter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm edit field filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Filters
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// The upm edit field filter.
    /// </summary>
    public class UPMEditFieldFilter : UPMFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMEditFieldFilter"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMEditFieldFilter(IIdentifier identifier)
            : base(identifier, UPMFilterType.EditField)
        {
            this.Invalid = false;
        }

        /// <summary>
        /// Gets or sets the editfield.
        /// </summary>
        public UPMEditField Editfield { get; set; }

        /// <inheritdoc/>
        public override bool HasParameters => this.Editfield != null || this.SecondEditfield != null;

        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets the raw values.
        /// </summary>
        public override List<string> RawValues
        {
            get
            {
                if (this.Editfield is UPMBooleanEditField)
                {
                    return ((UPMBooleanEditField)this.Editfield).BoolValue
                               ? new List<string> { "true" }
                               : new List<string> { "false" };
                }

                if (this.Editfield is UPMStringEditField)
                {
                    UPMStringEditField stringField = (UPMStringEditField)this.Editfield;
                    UPMStringEditField secondStringField = (UPMStringEditField)this.SecondEditfield;
                    if (!string.IsNullOrEmpty(stringField.StringValue)
                        && !string.IsNullOrEmpty(secondStringField?.StringValue))
                    {
                        return new List<string> { $"{stringField.StringValue}*", $"{secondStringField.StringValue}*" };
                    }

                    if (!string.IsNullOrEmpty(stringField.StringValue))
                    {
                        return new List<string> { $"{stringField.StringValue}*" };
                    }
                }
                else if (this.Editfield is UPMNumberEditField)
                {
                    UPMNumberEditField numberField = (UPMNumberEditField)this.Editfield;
                    UPMNumberEditField secondNumberField = (UPMNumberEditField)this.SecondEditfield;
                    if (secondNumberField != null)
                    {
                        return new List<string>
                                   {
                                       numberField.NumberValue.ToString(),
                                       secondNumberField.NumberValue.ToString()
                                   };
                    }

                    return new List<string> { numberField.NumberValue.ToString() };
                }
                else if (this.Editfield.FieldValue != null && !string.IsNullOrEmpty(this.Editfield.StringEditValue)
                         && this.SecondEditfield?.FieldValue != null
                         && !string.IsNullOrEmpty(this.SecondEditfield.StringEditValue))
                {
                    return new List<string>
                               {
                                   (string)this.Editfield.FieldValue,
                                   (string)this.SecondEditfield.FieldValue
                               };
                }
                else if (this.Editfield.FieldValue != null && this.Editfield.StringEditValue.Length > 0)
                {
                    return new List<string> { (string)this.Editfield.FieldValue };
                }

                return new List<string>();
            }
        }

        /// <summary>
        /// Gets or sets the second editfield.
        /// </summary>
        public UPMEditField SecondEditfield { get; set; }

        /// <summary>
        /// Gets or sets the second parameter name.
        /// </summary>
        public string SecondParameterName { get; set; }

        /// <inheritdoc />
        public override void ClearValue()
        {
            if (this.Editfield != null)
            {
                this.Editfield.FieldValue = null;
            }

            if (this.SecondEditfield != null)
            {
                this.SecondEditfield.FieldValue = null;
            }

            base.ClearValue();
        }
    }
}
