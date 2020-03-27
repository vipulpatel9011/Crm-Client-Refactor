// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCatalogEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The catalog edit field context implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Catalog Edit field context for a catalog edit field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    public class UPCatalogEditFieldContext : UPEditFieldContext
    {
        private List<string> arrayValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="childFields">The child fields.</param>
        public UPCatalogEditFieldContext(UPConfigFieldControlField fieldConfig, IIdentifier fieldIdentifier, string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
            if (childFields == null || childFields.Count == 0)
            {
                return;
            }

            var childFieldIndex = 0;
            var checkedChildFields = new List<UPEditFieldContext>(childFields.Count);
            var originalValue = this.OriginalValue;
            if (!string.IsNullOrEmpty(originalValue))
            {
                originalValue = string.Empty;
            }

            this.arrayValues = new List<string> { originalValue };
            var fieldInfo = fieldConfig?.Field?.FieldInfo;
            foreach (var childEditContext in this.ChildFields)
            {
                if (!childEditContext.FieldConfig.Field.FieldType.Equals(fieldInfo?.FieldType) ||
                    childEditContext.FieldConfig.Field.FieldInfo.CatNo != fieldInfo?.CatNo)
                {
                    continue;
                }

                checkedChildFields.Add(childEditContext);
                originalValue = childEditContext.OriginalValue;
                this.arrayValues.Add(string.IsNullOrEmpty(originalValue) ? string.Empty : originalValue);

                var catalogContext = childEditContext as UPChildCatalogEditFieldContext;
                if (catalogContext == null)
                {
                    continue;
                }

                catalogContext.ArrayIndex = ++childFieldIndex;
                catalogContext.RootEditFieldContext = this;
            }

            if (checkedChildFields.Count <= 0)
            {
                return;
            }

            if (checkedChildFields.Count < this.ChildFields.Count)
            {
                this.ChildFields = checkedChildFields;
            }

            this.MultiSelect = true;
            this.ValueCount = this.ChildFields.Count + 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        public UPCatalogEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Gets a value indicating whether [multi select].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multi select]; otherwise, <c>false</c>.
        /// </value>
        public bool MultiSelect { get; }

        /// <summary>
        /// Gets the value count.
        /// </summary>
        /// <value>
        /// The value count.
        /// </value>
        public int ValueCount { get; private set; }

        /// <summary>
        /// Gets or sets the catalog.
        /// </summary>
        /// <value>
        /// The catalog.
        /// </value>
        public virtual UPCatalog Catalog { get; protected set; }

        /// <summary>
        /// Sets the catalog value at position.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="position">The position.</param>
        public void SetCatalogValueAtPosition(string value, int position)
        {
            if (!this.MultiSelect)
            {
                return;
            }

            this.arrayValues[position] = value;

            var catEditField = (UPMCatalogEditField)this.editField;
            catEditField.RemoveAllFieldValues();
            foreach (var str in this.arrayValues)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    catEditField.AddFieldValue(str);
                }
            }
        }

        /// <summary>
        /// Catalogs the value at position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public string CatalogValueAtPosition(int position)
        {
            var arrayIndex = 0;
            var catField = this.editField as UPMCatalogEditField;
            if (catField == null)
            {
                return string.Empty;
            }

            var currentValues = catField.FieldValues;
            foreach (string currentValue in currentValues)
            {
                if (arrayIndex >= this.arrayValues.Count)
                {
                    break;
                }

                this.arrayValues[arrayIndex++] = currentValue;
            }

            while (arrayIndex < this.arrayValues.Count)
            {
                this.arrayValues[arrayIndex++] = string.Empty;
            }

            return this.arrayValues[position];
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.MultiSelect && this.arrayValues.Count > 0 ? this.CatalogValueAtPosition(0) : base.Value;

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetValue(string value)
        {
            if (this.MultiSelect)
            {
                this.SetCatalogValueAtPosition(value, 0);
                return;
            }

            this.changed = this.ServerValue != value && this.OriginalValue != value;

            var catField = (UPMCatalogEditField)this.editField;
            catField.ContinuousUpdate = true;
            catField.SetFieldValue(value);
        }

        /// <summary>
        /// Sets the array value.
        /// </summary>
        /// <param name="valueArray">The value array.</param>
        public void SetArrayValue(List<string> valueArray)
        {
            var catField = (UPMCatalogEditField)this.editField;
            catField.ContinuousUpdate = true;
            foreach (var val in valueArray)
            {
                catField.AddFieldValue(val);
            }
        }

        /// <summary>
        /// Edits the field.
        /// </summary>
        /// <returns></returns>
        public override UPMEditField EditField
        {
            get
            {
                if (this.editField != null)
                {
                    return this.editField;
                }

                this.editField = this.CreateEditField();
                if (this.editField == null)
                {
                    return null;
                }

                this.editField.ExternalKey = this.UniqueKey;
                this.editField.EditFieldContext = this;

                if (this.MultiSelect)
                {
                    var valueArray = new List<string>();
                    if (!string.IsNullOrEmpty(this.OriginalValue))
                    {
                        valueArray.Add(this.OriginalValue);
                    }

                    foreach (UPEditFieldContext childContext in this.ChildFields)
                    {
                        if (!string.IsNullOrEmpty(childContext.OriginalValue))
                        {
                            valueArray.Add(childContext.OriginalValue);
                        }
                    }

                    this.SetArrayValue(valueArray);
                }
                else
                {
                    this.SetValue(this.InitialEditFieldValue);
                }

                if (this.FieldConfig != null)
                {
                    if (this.FieldConfig.Attributes.NoLabel)
                    {
                        return this.editField;
                    }

                    this.editField.LabelText = !string.IsNullOrEmpty(this.FieldLabelPostfix)
                        ? this.FieldConfig.Label.StringByReplacingOccurrencesOfParameterWithIndexWithString(0, this.FieldLabelPostfix)
                        : this.FieldConfig.Label;
                }
                else if (this.WebConfigField != null)
                {
                    this.editField.LabelText = this.WebConfigField.Label;
                }

                return this.editField;
            }
        }
    }
}
