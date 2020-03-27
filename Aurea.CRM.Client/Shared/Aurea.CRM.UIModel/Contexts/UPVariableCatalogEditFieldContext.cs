// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPVariableCatalogEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Edit field context for Variabe catalog field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Edit field context for Variabe catalog field
    /// </summary>
    /// <seealso cref="UPCatalogEditFieldContext" />
    public class UPVariableCatalogEditFieldContext : UPCatalogEditFieldContext
    {
        /// <summary>
        /// The locked current values.
        /// </summary>
        protected List<UPCatalogValue> lockedCurrentValues;

        /// <summary>
        /// The locked parent value.
        /// </summary>
        private int lockedParentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPVariableCatalogEditFieldContext"/> class.
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
        public UPVariableCatalogEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
            var fieldInfo = fieldConfig.Field.FieldInfo;
            if (fieldInfo.ParentFieldId >= 0)
            {
                this.ParentField = new UPCRMField(fieldInfo.ParentFieldId, fieldConfig.InfoAreaId);
            }
        }

        /// <summary>
        /// Gets the catalog.
        /// </summary>
        /// <value>
        /// The catalog.
        /// </value>
        public override UPCatalog Catalog
            => UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(this.FieldConfig.Field.CatNo);

        /// <summary>
        /// Gets the extkey.
        /// </summary>
        /// <value>
        /// The extkey.
        /// </value>
        public override string Extkey
            => this.Catalog.ValueForCode(JObjectExtensions.ToInt(this.Value))?.ExtKey ?? string.Empty;

        /// <summary>
        /// Gets a value indicating whether [may have ext key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [may have ext key]; otherwise, <c>false</c>.
        /// </value>
        public override bool MayHaveExtKey => true;

        /// <summary>
        /// Gets the server value.
        /// </summary>
        /// <value>
        /// The server value.
        /// </value>
        public override string ServerValue
            =>
                this.ParentField != null && !this.FieldConfig.Attributes.ExtendedOptionIsSet("ReturnClientValue")
                    ? $"{JObjectExtensions.ToInt(base.ServerValue) % 65535}"
                    : base.ServerValue;

        /// <summary>
        /// Constraints the violations with page context.
        /// </summary>
        /// <param name="editPageContext">
        /// The edit page context.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public override List<UPEditConstraintViolation> ConstraintViolationsWithPageContext(
            UPEditPageContext editPageContext)
        {
            if (this.editField != null && this.editField.RequiredField)
            {
                if (string.IsNullOrEmpty(this.Value) || this.Value == "0")
                {
                    return new List<UPEditConstraintViolation>
                               {
                                   new UPEditConstraintViolation(this, EditConstraintViolationType.MustField)
                               };
                }
            }

            return this.ClientConstraintsWithPageContext(editPageContext.ClientCheckFilter);
        }

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMEditField"/>.
        /// </returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMCatalogEditField(this.FieldIdentifier, this.MultiSelect) { NullValueKey = "0" };
            if (this.MultiSelect)
            {
                field.MultiSelectMaxCount = this.ChildFields.Count + 1;
            }

            var possibleValues = this.Catalog?.TextValuesForFieldValues(false);
            var explicitKeyOrder = this.Catalog?.ExplicitKeyOrderEmptyValueIncludeHidden(!this.MultiSelect, false);

            if (possibleValues != null)
            {
                foreach (var p in possibleValues)
                {
                    var possibleValue = new UPMCatalogPossibleValue
                    {
                        Key = p.Key,
                        TitleLabelField = new UPMStringField(StringIdentifier.IdentifierWithStringId("X"))
                        {
                            StringValue = p.Value
                        }
                    };

                    if (!this.MultiSelect || p.Key != field.NullValueKey)
                    {
                        field.AddPossibleValue(possibleValue);
                    }
                }
            }

            UPEditFieldContext currentFieldContext = this;
            var childEditIndex = 0;
            List<UPCatalogValue> _lockedCurrentValues = null;
            while (currentFieldContext != null)
            {
                if (!string.IsNullOrEmpty(currentFieldContext.Value)
                    && possibleValues.ValueOrDefault(currentFieldContext.Value) == null)
                {
                    var textValue = this.Catalog?.TextValueForKey(currentFieldContext.Value);
                    if (string.IsNullOrEmpty(textValue))
                    {
                        // Rashan: removed the prefix '?'
                        textValue = $"{currentFieldContext.Value}";
                    }

                    var possibleValue = new UPMCatalogPossibleValue
                    {
                        Key = currentFieldContext.Value,
                        TitleLabelField = new UPMStringField(StringIdentifier.IdentifierWithStringId("X"))
                        {
                            StringValue = textValue
                        }
                    };

                    field.AddPossibleValue(possibleValue);
                    if (explicitKeyOrder != null)
                    {
                        var newExplicitOrder = new List<string>(explicitKeyOrder) { currentFieldContext.Value };
                        explicitKeyOrder = newExplicitOrder;
                    }

                    var catVal = this.Catalog?.ValueForCode(JObjectExtensions.ToInt(currentFieldContext.Value));
                    if (catVal != null)
                    {
                        if (_lockedCurrentValues == null)
                        {
                            _lockedCurrentValues = new List<UPCatalogValue> { catVal };
                            this.lockedParentValue = catVal.Code >> 16;
                        }
                        else
                        {
                            _lockedCurrentValues.Add(catVal);
                        }
                    }
                }

                currentFieldContext = this.ChildFields != null && this.ChildFields.Count > childEditIndex
                                          ? this.ChildFields[childEditIndex++]
                                          : null;
            }

            this.lockedCurrentValues = _lockedCurrentValues;
            field.ExplicitKeyOrder = explicitKeyOrder;
            field.ContinuousUpdate = true;
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);
            return field;
        }

        /// <summary>
        /// Parents the context changed value.
        /// </summary>
        /// <param name="parentContext">
        /// The parent context.
        /// </param>
        /// <param name="_value">
        /// The _value.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public override List<IIdentifier> ParentContextChangedValue(UPEditFieldContext parentContext, string _value)
        {
            var intValue = JObjectExtensions.ToInt(_value);
            var values = this.Catalog.SortedValuesForParentValueIncludeHidden(intValue, false);
            var field = (UPMCatalogEditField)this.EditField;
            field.ResetPossibleValues();
            if (this.lockedCurrentValues?.Count > 0 && intValue == this.lockedParentValue)
            {
                var extendedArray = new List<UPCatalogValue>(values);
                extendedArray.AddRange(this.lockedCurrentValues);
                values = extendedArray;
            }

            var sortOrder = new List<string>();
            var currentValue = this.Value;
            var currentValueFound = false;

            // Leer Eintrag
            if (!this.MultiSelect)
            {
                var possibleValue = new UPMCatalogPossibleValue { Key = "0" };
                var valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"))
                {
                    StringValue = string.Empty
                };

                possibleValue.TitleLabelField = valueField;
                field.AddPossibleValue(possibleValue);
                sortOrder.Add("0");
            }

            foreach (var value in values)
            {
                var possibleValue = new UPMCatalogPossibleValue
                {
                    Key = value.CodeKey,
                    TitleLabelField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"))
                    {
                        StringValue = value.Text
                    }
                };

                field.AddPossibleValue(possibleValue);
                sortOrder.Add(value.CodeKey);
                if (value.CodeKey == currentValue)
                {
                    currentValueFound = true;
                }
            }

            field.ExplicitKeyOrder = sortOrder;
            if (!currentValueFound)
            {
                this.editField.FieldValue = !this.MultiSelect ? "0" : null;
            }

            return this.editField.Identifier != null ? new List<IIdentifier> { this.editField.Identifier } : null;
        }
    }
}
