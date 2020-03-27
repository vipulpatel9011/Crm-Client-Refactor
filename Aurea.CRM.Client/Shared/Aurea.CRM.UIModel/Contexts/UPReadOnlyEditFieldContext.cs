// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPReadOnlyEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Field context for a read only field
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Edit field context for a read only field
    /// </summary>
    /// <seealso cref="UPHiddenEditFieldContext" />
    public class UPReadOnlyEditFieldContext : UPHiddenEditFieldContext
    {
        /// <summary>
        /// The field.
        /// </summary>
        private UPMField field;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPReadOnlyEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="fieldIdentifier">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="childFields">
        /// The child fields.
        /// </param>
        public UPReadOnlyEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPReadOnlyEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="fieldIdentifier">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPReadOnlyEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPReadOnlyEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPReadOnlyEditFieldContext(int fieldId, string value)
            : base(fieldId, value)
        {
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public override UPMField Field => this.InitField();

        /// <summary>
        /// Gets a value indicating whether [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        public override bool ReadOnly => true;

        /// <summary>
        /// Creates the field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMField"/>.
        /// </returns>
        public override UPMField CreateField()
        {
            var stringField = new UPMStringField(this.FieldIdentifier) { EditFieldContext = this };
            return stringField;
        }

        /// <summary>
        /// Sets the offline change value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetOfflineChangeValue(string value)
        {
            if (this.field != null)
            {
                this.SetValue(value);
            }
            else
            {
                this.initialUserChangeValue = value;
            }

            this.changed = true;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(string value)
        {
            base.SetValue(value);
            ((UPMStringField)this.field).FieldValue = this.FieldConfig.Field.FieldInfo.ValueForRawValueOptions(value, 0, null);
        }

        /// <summary>
        /// Initializes the field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMField"/>.
        /// </returns>
        private UPMField InitField()
        {
            if (this.field != null)
            {
                return this.field;
            }

            this.field = this.CreateField();
            if (this.field == null)
            {
                return null;
            }

            this.SetValue(this.InitialEditFieldValue);
            if (this.FieldConfig == null)
            {
                return this.field;
            }

            if (this.FieldConfig.Attributes.NoLabel)
            {
                return this.field;
            }

            if (!string.IsNullOrEmpty(this.FieldLabelPostfix))
            {
                this.field.LabelText =
                    this.FieldConfig.Label.StringByReplacingOccurrencesOfParameterWithIndexWithString(
                        0,
                        this.FieldLabelPostfix);
            }
            else
            {
                this.field.LabelText = this.FieldConfig.Label;
            }

            return this.field;
        }
    }
}
