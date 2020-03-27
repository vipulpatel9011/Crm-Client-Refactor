// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditFieldGroupModelController.cs" company="Aurea Software Gmbh">
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
//   Group model controller for edit fields
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Edit
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Group model controller for edit fields
    /// </summary>
    /// <seealso cref="UPFieldControlBasedEditGroupModelController" />
    public class UPEditFieldGroupModelController : UPFieldControlBasedEditGroupModelController
    {
        /// <summary>
        /// The edit header.
        /// </summary>
        private readonly bool editHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditFieldGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <param name="tabIndex">
        /// Index of the tab.
        /// </param>
        /// <param name="editPageContext">
        /// The edit page context.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        /// <param name="_editHeader">
        /// if set to <c>true</c> [_edit header].
        /// </param>
        public UPEditFieldGroupModelController(
            FieldControl fieldControl,
            int tabIndex,
            UPEditPageContext editPageContext,
            IGroupModelControllerDelegate theDelegate,
            bool _editHeader = false)
            : base(fieldControl, tabIndex, editPageContext, theDelegate)
        {
            this.editHeader = _editHeader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditFieldGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPEditFieldGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            this.Group = this.GroupFromRow(row);
            this.ControllerState = GroupModelControllerState.Finished;
            return this.Group;
        }

        /// <summary>
        /// Fieldses for edit field context.
        /// </summary>
        /// <param name="editFieldContext">
        /// The edit field context.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<UPMField> FieldsForEditFieldContext(UPEditFieldContext editFieldContext)
        {
            if (editFieldContext == null)
            {
                return null;
            }

            if (editFieldContext.ReadOnly)
            {
                var field = editFieldContext.Field;
                return field != null ? new List<UPMField> { editFieldContext.Field } : null;
            }

            var fieldArray = new List<UPMField>();
            var editFields = editFieldContext.EditFields;
            if (editFields != null)
            {
                foreach (var editField in editFields)
                {
                    editField.EditFieldsContext = this.EditPageContext;
                    fieldArray.Add(editField);
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// Gets the Group delegate.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public virtual object GroupDelegate => null;

        /// <summary>
        /// Groups from row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <returns>
        /// The <see cref="UPMGroup"/>.
        /// </returns>
        public virtual UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            List<UPCRMRecord> initialRecords = this.EditPageContext.OfflineRequest?.Records;

            var editFieldContextArray = this.EditContextsFor(
                resultRow,
                this.TabConfig,
                this.EditPageContext.EditFields,
                this.EditPageContext.InitialValues,
                initialRecords);
            this.EditPageContext.AddEditFieldContexts(editFieldContextArray);
            this.EditPageContext.HandleDependentFields();

            var fieldCount = editFieldContextArray.Count;
            if (fieldCount == 0)
            {
                return null;
            }

            var detailGroup = new UPMStandardEditGroup(this.GroupIdentifierForResultRow(resultRow))
            {
                HeaderStyle = this.editHeader,
                LabelText = this.TabLabel,
                GroupDelegate = this.GroupDelegate
            };

            var stringValue = this.TabConfig.FieldControl.ValueForAttribute($"labelWidthPercent_{this.TabIndex + 1}");
            var configFieldControlTab = this.FieldControlConfig?.TabAtIndex(this.TabIndex);
            var stringValueTab = configFieldControlTab?.ValueForAttribute("labelWidthPercent");
            if (!string.IsNullOrEmpty(stringValueTab))
            {
                stringValue = stringValueTab;
            }

            if (!string.IsNullOrEmpty(stringValue))
            {
                var labelSpace = float.Parse(stringValue) / 100.0f;
                if (labelSpace <= 1.0 && labelSpace >= 0.0)
                {
                    detailGroup.LabelSpace = Math.Min(labelSpace, 0.84f);
                    detailGroup.GapSpace = 0.05f;
                    detailGroup.ValueSpace = Math.Max((float)(1.0 - detailGroup.GapSpace - labelSpace), 0.0f);
                }
            }

            for (var j = 0; j < fieldCount; j++)
            {
                var obj = editFieldContextArray[j];
                var editFieldContext = obj as UPEditFieldContext;
                if (editFieldContext != null)
                {
                    var fields = this.FieldsForEditFieldContext(editFieldContext);
                    if (fields == null)
                    {
                        continue;
                    }

                    foreach (var field in fields)
                    {
                        detailGroup.AddField(field);
                    }
                }
                else
                {
                    detailGroup.AddField((UPMField)obj);
                }
            }

            return detailGroup;
        }

        /// <summary>
        /// Groups the identifier for result row.
        /// </summary>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <returns>
        /// The <see cref="IIdentifier"/>.
        /// </returns>
        public virtual IIdentifier GroupIdentifierForResultRow(UPCRMResultRow resultRow)
        {
            return resultRow == null
                       ? StringIdentifier.IdentifierWithStringId($"{this.TabConfig.FieldControl.UnitName}_{this.TabIndex}")
                       : this.TabIdentifierForRecordIdentification(resultRow.RootRecordIdentification);
        }

        /// <summary>
        /// Sets the index of the value for field at.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValueForFieldAtIndex(string rawValue, int index)
        {
            var editFieldContext = this.EditPageContext.OrderedEditFieldContexts[index];
            editFieldContext.SetValue(rawValue);
            editFieldContext.SetChanged(true);

            if (editFieldContext.EditField != null)
            {
                this.Delegate.GetType().GetTypeInfo().GetDeclaredMethod("UserDidChangeField")?.Invoke(this, new object[] { editFieldContext.EditField });
            }

            editFieldContext.NotifyDependentFields();
            return true;
        }

        /// <summary>
        /// Sets the index of the value for field with field.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValueForFieldWithFieldIndex(string rawValue, int fieldIndex)
        {
            return this.SetValueForFieldWithFieldIndexInfoAreaId(
                rawValue,
                fieldIndex,
                this.FieldControlConfig.InfoAreaId);
        }

        /// <summary>
        /// Sets the value for field with field index information area identifier.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetValueForFieldWithFieldIndexInfoAreaId(string rawValue, int fieldIndex, string infoAreaId)
        {
            var position = 0;
            foreach (var fieldContext in this.EditPageContext.OrderedEditFieldContexts)
            {
                if (fieldContext.FieldId == fieldIndex && fieldContext.FieldConfig.InfoAreaId.Equals(infoAreaId))
                {
                    return this.SetValueForFieldAtIndex(rawValue, position);
                }

                position++;
            }

            return false;
        }
    }
}
