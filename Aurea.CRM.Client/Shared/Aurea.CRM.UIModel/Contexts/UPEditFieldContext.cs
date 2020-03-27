// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The edit field context implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// The edit field context implementation
    /// </summary>
    public class UPEditFieldContext
    {
        private const string KeyPreviewWidth = "previewWidth";
        private const string KeyPreviewHeight = "previewHeight";
        private const string KeyFileNamePattern = "fileNamePattern";

        /// <summary>
        /// The next unique identifier
        /// </summary>
        private static int nextUniqueId;

        /// <summary>
        /// The changed.
        /// </summary>
        protected bool changed;

        /// <summary>
        /// The dependent fields.
        /// </summary>
        protected List<UPEditFieldContext> dependentFields;

        /// <summary>
        /// The edit field.
        /// </summary>
        protected UPMEditField editField;

        /// <summary>
        /// The initial user change value.
        /// </summary>
        protected string initialUserChangeValue;

        /// <summary>
        /// The parent context.
        /// </summary>
        protected UPEditFieldContext parentContext;

        /// <summary>
        /// The initial value.
        /// </summary>
        private readonly string initialValue;

        /// <summary>
        /// The multi line group field.
        /// </summary>
        private bool multiLineGroupField;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditFieldContext"/> class.
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
        public UPEditFieldContext(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
        {
            this.FieldConfig = fieldConfig;
            this.FieldId = this.FieldConfig?.FieldId ?? 0;
            this.OriginalValue = value;
            this.initialValue = value;
            this.ChildFields = childFields;
            this.FieldIdentifier = fieldIdentifier;
            this.WebConfigField = null;
            this.FieldFunction = fieldConfig?.Function;
            this.Key = $"{this.FieldId}";
            if (this.ChildFields != null)
            {
                foreach (var childField in this.ChildFields)
                {
                    childField.SetParentContext(this);
                }
            }

            this.UniqueKey = NextKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditFieldContext"/> class.
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
        public UPEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
        {
            this.FieldConfig = null;
            this.FieldId = -1;
            this.OriginalValue = value;
            this.initialValue = value;
            this.ChildFields = null;
            this.FieldIdentifier = fieldIdentifier;
            this.WebConfigField = fieldConfig;
            this.Key = fieldConfig.ValueName;
            this.UniqueKey = NextKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public UPEditFieldContext(int fieldId, string value)
        {
            this.FieldConfig = null;
            this.FieldId = fieldId;
            this.OriginalValue = null;
            this.initialValue = value;
            this.ChildFields = null;
            this.FieldIdentifier = null;
            this.WebConfigField = null;
            this.Key = $"{this.FieldId}";
            this.UniqueKey = NextKey;
        }

        /// <summary>
        /// Gets the next key.
        /// </summary>
        /// <value>
        /// The next key.
        /// </value>
        public static string NextKey => $"{nextUniqueId++:D8}";

        /// <summary>
        /// Gets or sets the child edit context.
        /// </summary>
        /// <value>
        /// The child edit context.
        /// </value>
        public UPChildEditContext ChildEditContext { get; set; }

        /// <summary>
        /// Gets or sets the child fields.
        /// </summary>
        /// <value>
        /// The child fields.
        /// </value>
        public List<UPEditFieldContext> ChildFields { get; protected set; }

        /// <summary>
        /// Gets or sets the date original value.
        /// </summary>
        /// <value>
        /// The date original value.
        /// </value>
        public string DateOriginalValue { get; set; }

        /// <summary>
        /// Gets the extkey.
        /// </summary>
        /// <value>
        /// The extkey.
        /// </value>
        public virtual string Extkey => null;

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public virtual UPMField Field => this.EditField;

        /// <summary>
        /// Gets the field configuration.
        /// </summary>
        /// <value>
        /// The field configuration.
        /// </value>
        public UPConfigFieldControlField FieldConfig { get; private set; }

        /// <summary>
        /// Gets the field function.
        /// </summary>
        /// <value>
        /// The field function.
        /// </value>
        public string FieldFunction { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public IIdentifier FieldIdentifier { get; private set; }

        /// <summary>
        /// Gets or sets the field label postfix.
        /// </summary>
        /// <value>
        /// The field label postfix.
        /// </value>
        public string FieldLabelPostfix { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has dependent fields.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has dependent fields; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasDependentFields => this.dependentFields?.Any() ?? false;

        /// <summary>
        /// Gets the help field identifier.
        /// </summary>
        /// <value>
        /// The help field identifier.
        /// </value>
        public virtual IIdentifier HelpFieldIdentifier
            => this.editField != null ? this.editField.HelpIdentifier : this.FieldIdentifier;

        /// <summary>
        /// Gets the initial edit field value.
        /// </summary>
        /// <value>
        /// The initial edit field value.
        /// </value>
        public virtual string InitialEditFieldValue
            => !string.IsNullOrEmpty(this.initialUserChangeValue) ? this.initialUserChangeValue : this.OriginalValue;

        /// <summary>
        /// Gets a value indicating whether this instance is multi line group field.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is multi line group field; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsMultiLineGroupField => this.multiLineGroupField;

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [may have ext key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [may have ext key]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool MayHaveExtKey => false;

        /// <summary>
        /// Gets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        public string OriginalValue { get; private set; }

        /// <summary>
        /// Gets or sets the parent field.
        /// </summary>
        /// <value>
        /// The parent field.
        /// </value>
        public UPCRMField ParentField { get; protected set; }

        /// <summary>
        /// Gets the parent field identifier.
        /// </summary>
        /// <value>
        /// The parent field identifier.
        /// </value>
        public virtual IIdentifier ParentFieldIdentifier => null;

        /// <summary>
        /// Gets a value indicating whether [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ReadOnly => false;

        /// <summary>
        /// Gets the server value.
        /// </summary>
        /// <value>
        /// The server value.
        /// </value>
        public virtual string ServerValue => this.Value;

        /// <summary>
        /// Gets or sets the time original value.
        /// </summary>
        /// <value>
        /// The time original value.
        /// </value>
        public string TimeOriginalValue { get; set; }

        /// <summary>
        /// Gets the unique key.
        /// </summary>
        /// <value>
        /// The unique key.
        /// </value>
        public string UniqueKey { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual string Value
        {
            get { return this.editField != null ? this.editField.FieldValue as string : this.initialValue; }
            set { this.editField.FieldValue = value; }
        }

        /// <summary>
        /// Gets the web configuration field.
        /// </summary>
        /// <value>
        /// The web configuration field.
        /// </value>
        public WebConfigLayoutField WebConfigField { get; private set; }

        /// <summary>
        /// Childs the field context for field configuration value.
        /// </summary>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="UPEditFieldContext"/>.
        /// </returns>
        public static UPEditFieldContext ChildFieldContextForFieldConfigValue(
            UPConfigFieldControlField fieldConfig,
            string value)
        {
            if (fieldConfig == null)
            {
                return null;
            }

            var gps = fieldConfig.Attributes?.ExtendedOptionForKey("GPS");
            if (!string.IsNullOrEmpty(gps))
            {
                return new UPChildGpsEditFieldContext(fieldConfig, value);
            }

            var fieldType = fieldConfig.Field?.FieldType ?? string.Empty;
            if (fieldType == "T")
            {
                return new UPChildTimeEditFieldContext(fieldConfig, value);
            }

            if (fieldType == "K" || fieldType == "X")
            {
                return new UPChildCatalogEditFieldContext(fieldConfig, value);
            }

            return new UPChildEditFieldContext(fieldConfig, value);
        }

        /// <summary>
        /// Fields the context for.
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
        /// <returns>
        /// The <see cref="UPEditFieldContext"/>.
        /// </returns>
        public static UPEditFieldContext FieldContextFor(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            List<UPEditFieldContext> childFields)
        {
            var fieldType = fieldConfig?.Field?.FieldType;
            if (string.IsNullOrEmpty(fieldType))
            {
                return null;
            }

            var gps = fieldConfig.Attributes?.ExtendedOptionForKey("GPS");
            if (!string.IsNullOrEmpty(gps))
            {
                return new UPGpsEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
            }

            switch (fieldType[0])
            {
                case 'X':
                    return new UPFixedCatalogEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                case 'K':
                    return new UPVariableCatalogEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                case 'D':
                    return new UPDateEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                case 'T':
                    return new UPTimeEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                case 'B':
                    return new UPBooleanEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                case 'N':
                case 'L':
                case 'S':
                case 'F':
                    if (fieldType[0] == 'L' && fieldConfig.Field.IsRepField)
                    {
                        var repId = UPCRMReps.FormattedRepId(value);
                        return new UPRepEditFieldContext(fieldConfig, fieldIdentifier, repId, childFields);
                    }
                    else
                    {
                        return new UPNumberEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                    }

                case 'Z':
                    {
                        return new UPEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
                    }

                default:
                    return new UPEditFieldContext(fieldConfig, fieldIdentifier, value, childFields);
            }
        }

        /// <summary>
        /// Fields the context for.
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
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="UPEditFieldContext"/>.
        /// </returns>
        public static UPEditFieldContext FieldContextFor(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value,
            UPSelector selector)
        {
            return selector is UPRecordSelector
                       ? (UPEditFieldContext)
                         new UPRecordSelectorEditFieldContext(
                             fieldConfig,
                             fieldIdentifier,
                             value,
                             (UPRecordSelector)selector)
                       : new UPSelectorEditFieldContext(fieldConfig, fieldIdentifier, value, selector);
        }

        /// <summary>
        /// Fields the context for web configuration parameter field identifier value.
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
        /// <returns>
        /// The <see cref="UPEditFieldContext"/>.
        /// </returns>
        public static UPEditFieldContext FieldContextForWebConfigParameterFieldIdentifierValue(
            WebConfigLayoutField fieldConfig,
            IIdentifier fieldIdentifier,
            string value)
        {
            if (fieldConfig?.Options?.Count > 0)
            {
                return new UPOptionsEditFieldContext(fieldConfig, fieldIdentifier, value);
            }

            if (fieldConfig?.FieldType == "Checkbox")
            {
                return new UPBooleanEditFieldContext(fieldConfig, fieldIdentifier, value);
            }

            return new UPEditFieldContext(fieldConfig, fieldIdentifier, value);
        }

        /// <summary>
        /// Hiddens the field for.
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
        /// <returns>
        /// The <see cref="UPEditFieldContext"/>.
        /// </returns>
        public static UPEditFieldContext HiddenFieldFor(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value)
        {
            return new UPHiddenEditFieldContext(fieldConfig, fieldIdentifier, value, null);
        }

        /// <summary>
        /// Readonlies the field for.
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
        /// <returns>
        /// The <see cref="UPEditFieldContext"/>.
        /// </returns>
        public static UPEditFieldContext ReadonlyFieldFor(
            UPConfigFieldControlField fieldConfig,
            IIdentifier fieldIdentifier,
            string value)
        {
            return new UPReadOnlyEditFieldContext(fieldConfig, fieldIdentifier, value, null);
        }

        /// <summary>
        /// Adds the dependent field context.
        /// </summary>
        /// <param name="editFieldContext">
        /// The edit field context.
        /// </param>
        public virtual void AddDependentFieldContext(UPEditFieldContext editFieldContext)
        {
            if (this.dependentFields == null)
            {
                this.dependentFields = new List<UPEditFieldContext>();
            }
            else if (this.dependentFields.Contains(editFieldContext))
            {
                return;
            }

            this.dependentFields.Add(editFieldContext);
        }

        /// <summary>
        /// Applies the attributes on edit field configuration.
        /// </summary>
        /// <param name="_editField">
        /// The _edit field.
        /// </param>
        /// <param name="fieldConfig">
        /// The field configuration.
        /// </param>
        public virtual void ApplyAttributesOnEditFieldConfig(
            UPMEditField _editField,
            UPConfigFieldControlField fieldConfig)
        {
            if (fieldConfig == null)
            {
                return;
            }

            if (fieldConfig.Attributes.Must)
            {
                _editField.RequiredField = true;
            }
            else if (!fieldConfig.IsLinkedField)
            {
                var fieldInfo = fieldConfig.Field?.FieldInfo;
                _editField.RequiredField = fieldInfo != null && fieldInfo.MustField;
            }
        }

        /// <summary>
        /// Clients the constraints with page context.
        /// </summary>
        /// <param name="clientFilter">
        /// The client filter.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<UPEditConstraintViolation> ClientConstraintsWithPageContext(UPConfigFilter clientFilter)
        {
            if (clientFilter == null || this.FieldConfig == null)
            {
                return null;
            }

            var checkResult = clientFilter.CheckValueInfoAreaIdFieldId(
                this.Value,
                this.FieldConfig.InfoAreaId,
                this.FieldConfig.FieldId);

            return checkResult != null
                       ? new List<UPEditConstraintViolation>
                        {
                            new UPEditConstraintViolation(this, EditConstraintViolationType.ClientConstraint, checkResult.ErrorKey)
                        }
                       : null;
        }

        /// <summary>
        /// Constraints the violations with page context.
        /// </summary>
        /// <param name="editPageContext">
        /// The edit page context.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<UPEditConstraintViolation> ConstraintViolationsWithPageContext(UPEditPageContext editPageContext)
        {
            if (this.editField != null && this.editField.RequiredField)
            {
                if (string.IsNullOrEmpty(this.Value) || this.FieldConfig.IsEmptyValue(this.Value))
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
        public virtual UPMEditField CreateEditField()
        {
            if (FieldIdentifier == null)
            {
                return null;
            }

            if (FieldConfig == null)
            {
                return new UPMStringEditField(FieldIdentifier) { Type = StringEditFieldType.Plain };
            }

            var fieldAttributes = FieldConfig.Attributes;
            var createdField = (UPMEditField)null;
            if (fieldAttributes.MultiLine)
            {
                createdField = CreateMultiLineField(fieldAttributes);
            }
            else if (fieldAttributes.Image)
            {
                createdField = CreateImageField(fieldAttributes);
            }
            else
            {
                createdField = CreateStringEditField(fieldAttributes);
            }

            ApplyAttributesOnEditFieldConfig(createdField, FieldConfig);
            return createdField;
        }

        /// <summary>
        /// Creates the field.
        /// </summary>
        /// <returns>
        /// The <see cref="UPMField"/>.
        /// </returns>
        public virtual UPMField CreateField()
        {
            UPMField field = this.CreateEditField();
            field.ExternalKey = this.UniqueKey;
            field.EditFieldContext = this;
            return field;
        }

        /// <summary>
        /// Gets the edit field.
        /// </summary>
        /// <value>
        /// The edit field.
        /// </value>
        public virtual UPMEditField EditField
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
                this.SetValue(this.InitialEditFieldValue);
                if (this.FieldConfig != null)
                {
                    if (this.FieldConfig.Attributes.NoLabel)
                    {
                        return this.editField;
                    }

                    this.editField.LabelText = !string.IsNullOrEmpty(this.FieldLabelPostfix)
                                                   ? this.FieldConfig.Label
                                                         .StringByReplacingOccurrencesOfParameterWithIndexWithString(
                                                             0,
                                                             this.FieldLabelPostfix)
                                                   : this.FieldConfig.Label;
                }
                else if (this.WebConfigField != null)
                {
                    this.editField.LabelText = this.WebConfigField.Label;
                }

                return this.editField;
            }
        }

        /// <summary>
        /// Gets the edit fields.
        /// </summary>
        /// <value>
        /// The edit fields.
        /// </value>
        public virtual List<UPMEditField> EditFields => this.EditField == null ? null : new List<UPMEditField> { this.editField };

        /// <summary>
        /// Notifies the dependent fields.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<IIdentifier> NotifyDependentFields()
        {
            List<IIdentifier> allChanges = null;
            if (this.dependentFields != null)
            {
                foreach (var dependentField in this.dependentFields)
                {
                    var changes = dependentField.ParentContextChangedValue(this, this.Value);
                    if (changes == null)
                    {
                        continue;
                    }

                    if (allChanges != null)
                    {
                        allChanges.AddRange(changes);
                    }
                    else
                    {
                        allChanges = new List<IIdentifier>(changes);
                    }
                }
            }

            return allChanges;
        }

        /// <summary>
        /// Parents the context changed value.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<IIdentifier> ParentContextChangedValue(UPEditFieldContext context, string value) => null;

        /// <summary>
        /// Sets the changed.
        /// </summary>
        /// <param name="_changed">
        /// if set to <c>true</c> [_changed].
        /// </param>
        public virtual void SetChanged(bool _changed)
        {
            this.changed = _changed;
        }

        /// <summary>
        /// Sets the offline change value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public virtual void SetOfflineChangeValue(string value)
        {
            if (this.editField != null)
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
        /// Sets the parent context.
        /// </summary>
        /// <param name="_parentContext">
        /// The _parent context.
        /// </param>
        public virtual void SetParentContext(UPEditFieldContext _parentContext)
        {
            this.parentContext = _parentContext;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public virtual void SetValue(string value)
        {
            if (this.editField != null)
            {
                this.editField.FieldValue = value ?? string.Empty;
            }
            else
            {
                this.OriginalValue = value;
            }
        }

        /// <summary>
        /// Wases the changed.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool WasChanged()
        {
            if (this.FieldConfig?.Attributes != null && this.FieldConfig.Attributes.Dontsave)
            {
                return false;
            }

            return this.changed || (this.editField?.Changed ?? false);
        }

        /// <summary>
        /// Wases the changed.
        /// </summary>
        /// <param name="userChangesOnly">
        /// if set to <c>true</c> [user changes only].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool WasChanged(bool userChangesOnly)
            => userChangesOnly ? this.WasChangedByUser() : this.WasChanged();

        /// <summary>
        /// Wases the changed by user.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool WasChangedByUser()
        {
            if (this.FieldConfig?.Attributes != null && this.FieldConfig.Attributes.Dontsave)
            {
                return false;
            }

            return this.editField?.Changed ?? false;
        }

        /// <summary>
        /// Creates MultiLine Field
        /// </summary>
        /// <param name="fieldAttributes">
        /// Field Configuration Attributes
        /// </param>
        /// <returns>
        /// MultiLine <see cref="UPMMultilineEditField"/>.
        /// </returns>
        private UPMMultilineEditField CreateMultiLineField(FieldAttributes fieldAttributes)
        {
            multiLineGroupField = true;
            var fieldLength = FieldConfig.Field.FieldInfo.FieldLength;
            return new UPMMultilineEditField(FieldIdentifier)
            {
                MaxLength = fieldLength > 0 ? fieldLength : 0,
                Html = FieldConfig.Field.FieldInfo.HtmlField,
                NoLabel = fieldAttributes.NoLabel,
             //   MultiLine = GetFieldAttribuyesValye(fieldAttributes, (int)FieldAttr.MultiLine),
             //   RowSpan =  Convert.ToInt32(GetFieldAttribuyesValye(fieldAttributes, (int)FieldAttr.RowSpan)),
            };
        }

        private string GetFieldAttribuyesValye(FieldAttributes fieldAttributes, int AttrId)
        {
            foreach (var Attribute in fieldAttributes.AttributeArray)
            {
                if (Attribute.Attrid == AttrId)
                {
                    return Attribute.Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Creates Image Field
        /// </summary>
        /// <param name="fieldAttributes">
        /// Field Configuration Attributes
        /// </param>
        /// <returns>
        /// Image <see cref="UPMEditField"/>.
        /// </returns>
        private UPMImageEditField CreateImageField(FieldAttributes fieldAttributes)
        {
            var imageEditField = new UPMImageEditField(FieldIdentifier);
            var attribute = fieldAttributes.AttributForId((int)FieldAttr.Image);
            var previewWidth = attribute.ValueOptionsForKey(KeyPreviewWidth);
            var previewHeight = attribute.ValueOptionsForKey(KeyPreviewHeight);
            imageEditField.ImageDocumentMaxSize = new Size(176, 176);

            if (previewWidth != null && previewHeight != null)
            {
                var width = JObjectExtensions.ToInt(previewWidth);
                var height = JObjectExtensions.ToInt(previewHeight);
                imageEditField.ImageDocumentMaxSize = new Size(width, height);
            }

            var explicitFileName = attribute.ValueOptionsForKey(KeyFileNamePattern) as string;
            if (explicitFileName != null)
            {
                imageEditField.ExplicitFileName = explicitFileName;
            }

            var documentKey = Value;
            if (!string.IsNullOrWhiteSpace(documentKey))
            {
                var documentManager = new DocumentManager();
                var documentData = documentManager.DocumentForKey(documentKey);
                imageEditField.ImageDocument = new UPMDocument(
                    FieldIdentifier,
                    null,
                    null,
                    null,
                    null,
                    documentData?.Url ?? ServerSession.CurrentSession.DocumentRequestUrlForDocumentKey(documentKey),
                    documentData?.Title,
                    null,
                    null,
                    documentData?.UrlForD1RecordId());
            }

            return imageEditField;
        }

        /// <summary>
        /// Creates String Edit Field
        /// </summary>
        /// <param name="fieldAttributes">
        /// Field Configuration Attributes
        /// </param>
        /// <returns>
        /// Text <see cref="UPMEditField"/>.
        /// </returns>
        private UPMStringEditField CreateStringEditField(FieldAttributes fieldAttributes)
        {
            var textEditField = new UPMStringEditField(FieldIdentifier);
            if (fieldAttributes.Email)
            {
                textEditField.Type = StringEditFieldType.Email;
            }
            else if (fieldAttributes.Httplink)
            {
                textEditField.Type = StringEditFieldType.Url;
            }
            else if (fieldAttributes.Phone)
            {
                textEditField.Type = StringEditFieldType.Phone;
            }
            else
            {
                textEditField.Type = StringEditFieldType.Plain;
            }

            var fieldLength = FieldConfig.Field.FieldInfo.FieldLength;
            textEditField.MaxLength = fieldLength > 0 ? fieldLength : 0;
            return textEditField;
        }
    }
}
