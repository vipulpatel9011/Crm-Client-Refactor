// <copyright file="UPRecordSelectorEditFieldContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Utilities;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;

    /// <summary>
    /// Edit field context for record selector field
    /// </summary>
    /// <seealso cref="UPEditFieldContext" />
    /// <seealso cref="IRecordSelectorEditFieldDelegate" />
    public class UPRecordSelectorEditFieldContext : UPEditFieldContext, IRecordSelectorEditFieldDelegate
    {
        private string LastSetValue;
        private string LastSetDisplayValue;
        private UPCatalog Catalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelectorEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="childFields">The child fields.</param>
        public UPRecordSelectorEditFieldContext(UPConfigFieldControlField fieldConfig, IIdentifier fieldIdentifier, string value, List<UPEditFieldContext> childFields)
            : base(fieldConfig, fieldIdentifier, value, childFields)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelectorEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        public UPRecordSelectorEditFieldContext(WebConfigLayoutField fieldConfig, IIdentifier fieldIdentifier, string value)
            : base(fieldConfig, fieldIdentifier, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelectorEditFieldContext"/> class.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="fieldIdentifier">The field identifier.</param>
        /// <param name="value">The value.</param>
        /// <param name="selector">The selector.</param>
        public UPRecordSelectorEditFieldContext(UPConfigFieldControlField fieldConfig, IIdentifier fieldIdentifier, string value, UPRecordSelector selector)
            : base(fieldConfig, fieldIdentifier, value, null)
        {
            this.Selector = selector;
        }

        /// <summary>
        /// Gets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public UPRecordSelector Selector { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override string Value => this.Catalog != null && base.Value != null && base.Value.Equals(this.LastSetDisplayValue) ? this.LastSetValue : base.Value;

        /// <summary>
        /// Creates the edit field.
        /// </summary>
        /// <returns><see cref="UPMEditField"/></returns>
        public override UPMEditField CreateEditField()
        {
            var field = new UPMRecordSelectorEditField(this.FieldIdentifier);
            this.ApplyAttributesOnEditFieldConfig(field, this.FieldConfig);
            if (this.Selector != null)
            {
                field.SelectorArray = new List<UPRecordSelector> { this.Selector };
                field.Delegate = this;
            }

            field.ContinuousUpdate = true;
            this.Catalog = null;
            if (this.FieldConfig.Field.FieldType == "K")
            {
                this.Catalog = UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(this.FieldConfig.Field.CatNo);
            }
            else if (this.FieldConfig.Field.FieldType == "X")
            {
                this.Catalog = UPCRMDataStore.DefaultStore.CatalogForFixedCatalogId(this.FieldConfig.Field.CatNo);
            }

            return field;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                var onlyOnline = UPSelector.StaticSelectorContextDelegate?.LinkOnlyOnlineAvailable(this, this.Selector.LinkTargetInfoAreaId, this.Selector.LinklinkId) ?? false;
                if (onlyOnline)
                {
                    var field = (UPMRecordSelectorEditField)this.Field;
                    var editOfflineRecord = ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("RecordSelect.EditOfflineRecord", false);
                    field.DisableEdit = !editOfflineRecord;
                    base.SetValue(LocalizedString.TextOfflineNotAvailable);
                    return;
                }
            }

            if (this.Catalog != null)
            {
                var textValue = this.Catalog.TextValueForKey(value);
                if (!string.IsNullOrEmpty(textValue))
                {
                    this.LastSetValue = value;
                    this.LastSetDisplayValue = textValue;
                    base.SetValue(textValue);
                }
                else
                {
                    this.LastSetValue = null;
                    this.LastSetDisplayValue = null;
                    base.SetValue(value);
                }
            }
            else
            {
                base.SetValue(value);
            }
        }

        /// <summary>
        /// Context record for given edit field.
        /// </summary>
        /// <param name="editField">Edit field</param>
        /// <returns><see cref="string"/></returns>
        public string ContextRecordForEditField(UPMEditField editField)
        {
            if (this.Selector.DisableLinkOption && this.Selector.LinkIsDisabled)
            {
                return null;
            }

            if (this.Selector.RecordLinkInfoAreaIds?.Count > 0)
            {
                if (this.ChildEditContext != null)
                {
                    return this.ChildEditContext.ContextRecordForSenderSelector(editField, this.Selector);
                }

                for (var i = 0; i < this.Selector.RecordLinkInfoAreaIds.Count; i++)
                {
                    var linkInfoAreaId = this.Selector.RecordLinkInfoAreaIds[i];
                    var linkId = this.Selector.RecordLinkLinkIds[i].ToInt();
                    var rid = UPSelector.StaticSelectorContextDelegate?.SenderLinkForInfoAreaIdLinkId(this, linkInfoAreaId, linkId);
                    if (rid?.Length > 8)
                    {
                        return rid;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns current record for given edit field
        /// </summary>
        /// <param name="editField">Edit field</param>
        /// <returns><see cref="string"/></returns>
        public string CurrentRecordForEditField(UPMEditField editField)
        {
            if (this.ChildEditContext != null)
            {
                return this.ChildEditContext.CurrentRecordForSenderSelector(editField, this.Selector);
            }

            if (this.Selector.LinkTargetInfoAreaId == null)
            {
                return null;
            }

            return UPSelector.StaticSelectorContextDelegate.SenderLinkForInfoAreaIdLinkId(this, this.Selector.LinkTargetInfoAreaId, this.Selector.LinklinkId);
        }
    }
}
