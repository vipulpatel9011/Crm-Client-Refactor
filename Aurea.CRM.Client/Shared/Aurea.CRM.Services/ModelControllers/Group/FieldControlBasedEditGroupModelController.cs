// <copyright file="FieldControlBasedEditGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Field control based group model controller for edit mode
    /// </summary>
    /// <seealso cref="UPFieldControlBasedGroupModelController" />
    public class UPFieldControlBasedEditGroupModelController : UPFieldControlBasedGroupModelController
    {
        /// <summary>
        /// The enable linked edit fields
        /// </summary>
        protected bool enableLinkedEditFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedEditGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="editPageContext">The edit page context.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPFieldControlBasedEditGroupModelController(FieldControl fieldControl, int tabIndex, UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
            this.EditPageContext = editPageContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedEditGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPFieldControlBasedEditGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedEditGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPFieldControlBasedEditGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
        }

        /// <summary>
        /// Gets the edit page context.
        /// </summary>
        /// <value>
        /// The edit page context.
        /// </value>
        public UPEditPageContext EditPageContext { get; protected set; }

        /// <summary>
        /// Values the by applying initial values for field.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="initialValues">The initial values.</param>
        /// <returns></returns>
        public string ValueByApplyingInitialValuesForField(string value, UPConfigFieldControlField fieldConfig, Dictionary<string, object> initialValues)
        {
            var initialValue = initialValues?.ValueOrDefault(fieldConfig?.Identification) as string;
            return !string.IsNullOrEmpty(initialValue) ? initialValue : value;
        }

        /// <summary>
        /// Values for link field from initial values.
        /// </summary>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <param name="initialValues">The initial values.</param>
        /// <returns></returns>
        public string ValueForLinkFieldFromInitialValues(UPConfigFieldControlField fieldConfig, Dictionary<string, object> initialValues)
        {
            var infoAreaKey = $".{fieldConfig.InfoAreaId}.{(fieldConfig.LinkId < 0 ? 0 : fieldConfig.LinkId)}";
            var fieldValuesForInfoAreaKeyArray = initialValues.ValueOrDefault(infoAreaKey) as List<object>;
            if (fieldValuesForInfoAreaKeyArray == null || !fieldValuesForInfoAreaKeyArray.Any())
            {
                return null;
            }

            var fieldValuesForInfoAreaKey = fieldValuesForInfoAreaKeyArray[0] as Dictionary<string, object>;
            var fieldKey = $"{fieldConfig.InfoAreaId}.{fieldConfig.FieldId}";
            var rawFieldValue = fieldValuesForInfoAreaKey.ValueOrDefault(fieldKey) as string;
            if (!string.IsNullOrEmpty(rawFieldValue) && fieldConfig.Field != null)
            {
                return fieldConfig.Field.ValueForRawValueOptions(rawFieldValue, 0);
            }

            return rawFieldValue;
        }

        /// <summary>
        /// Edits the contexts for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="tabConfig">The tab configuration.</param>
        /// <param name="editFieldDictionary">The edit field dictionary.</param>
        /// <param name="initialValues">The initial values.</param>
        /// <param name="fieldPostfix">The field postfix.</param>
        /// <param name="initialRecords">The initial records.</param>
        /// <returns></returns>
        public List<object> EditContextsForResultRow(UPCRMResultRow resultRow, FieldControlTab tabConfig,
            Dictionary<string, UPEditFieldContext> editFieldDictionary, Dictionary<string, object> initialValues, string fieldPostfix, List<UPCRMRecord> initialRecords)
        {
            var recordIdentification = resultRow?.RecordIdentificationAtIndex(0);

            var fieldArray = new List<object>();
            UPCRMRecord offlineRootRecord = null;
            if (initialRecords?.Count > 0)
            {
                offlineRootRecord = initialRecords.FirstOrDefault();
            }

            var identifierPrefix = recordIdentification;
            if (string.IsNullOrEmpty(identifierPrefix))
            {
                identifierPrefix = $"{this.TabConfig.FieldControl.UnitName}_{this.TabIndex}";
            }

            var fieldCount = tabConfig?.NumberOfFields ?? 0;

            for (var j = 0; j < fieldCount; j++)
            {
                var fieldConfig = tabConfig?.FieldAtIndex(j);
                if (fieldConfig == null)
                {
                    continue;
                }

                var fieldAttributes = fieldConfig.Attributes;
                var currentInfoAreaId = fieldConfig.InfoAreaId;
                var currentLinkId = fieldConfig.LinkId;
                var fieldIdentifier = FieldIdentifier.IdentifierWithRecordIdentificationFieldId(identifierPrefix, fieldConfig.Identification);
                UPSelector selector = null;
                var selectorDef = fieldConfig.Attributes?.Selector;
                if (selectorDef != null)
                {
                    var filterParameters = this.EditPageContext?.ViewReference?.ContextValueForKey("copyFields")?.JsonDictionaryFromString();
                    if (resultRow?.Result != null && resultRow.IsNewRow)
                    {
                        selector = UPSelector.SelectorFor(
                            resultRow.RootRecordIdentification?.InfoAreaId(),
                            resultRow.Result.ParentRecordIdentification,
                            resultRow.Result.LinkId,
                            selectorDef,
                            filterParameters,
                            fieldConfig);
                    }
                    else
                    {
                        selector = UPSelector.SelectorFor(resultRow?.RootRecordIdentification, selectorDef, filterParameters, fieldConfig);
                    }

                    selector.Build();
                    if (selector.OptionCount == 0 && selector.IsStaticSelector)
                    {
                        selector = null;
                    }
                }

                var isEditField = this.enableLinkedEditFields ||
                    selector != null ||
                    (tabConfig.FieldControl.InfoAreaId == currentInfoAreaId && currentLinkId <= 0);

                var isHidden = fieldAttributes.Hide;
                var isReadOnly = isEditField && fieldAttributes.ReadOnly;
                var rawFieldValue0 = resultRow?.RawValueAtIndex(fieldConfig.TabIndependentFieldIndex);
                var fieldInfo = fieldConfig.Field.FieldInfo;
                if (isEditField && !isReadOnly && !(selector is UPRecordSelector && ((UPRecordSelector)selector).IgnoreFieldInfo))
                {
                    if (fieldInfo.IsReadOnly)
                    {
                        isReadOnly = true;
                    }
                    else if (resultRow?.IsNewRow == true || string.IsNullOrEmpty(rawFieldValue0))
                    {
                        if (fieldInfo.LockedOnNew)
                        {
                            isReadOnly = true;
                        }
                    }
                    else if (fieldInfo.LockedOnUpdate && !fieldInfo.IsEmptyValue(rawFieldValue0))
                    {
                        isReadOnly = true;
                    }
                }

                string offlineValue = null;
                bool offlineChanged;
                string rawFieldValue;
                UPEditFieldContext editFieldContext;
                if (isEditField)
                {
                    List<UPEditFieldContext> childFields = null;
                    if (fieldAttributes.FieldCount > 1 && selector == null)
                    {
                        childFields = new List<UPEditFieldContext>();
                        for (var k = 1; k < fieldAttributes.FieldCount; k++)
                        {
                            var childFieldConfig = tabConfig.FieldAtIndex(++j);
                            if (childFieldConfig != null)
                            {
                                rawFieldValue = resultRow.RawValueAtIndex(childFieldConfig.TabIndependentFieldIndex);
                                if (initialValues != null)
                                {
                                    rawFieldValue = this.ValueByApplyingInitialValuesForField(rawFieldValue, childFieldConfig, initialValues);
                                }

                                offlineChanged = false;

                                if (offlineRootRecord != null)
                                {
                                    offlineValue = offlineRootRecord.StringFieldValueForFieldIndex(childFieldConfig.FieldId);
                                    if (offlineValue != null && !offlineValue.Equals(rawFieldValue))
                                    {
                                        offlineChanged = true;
                                    }
                                }

                                editFieldContext = UPEditFieldContext.ChildFieldContextForFieldConfigValue(childFieldConfig, rawFieldValue);
                                if (offlineChanged)
                                {
                                    editFieldContext.SetOfflineChangeValue(offlineValue);
                                }

                                childFields.Add(editFieldContext);
                            }
                        }
                    }

                    var markAsChanged = false;
                    rawFieldValue = rawFieldValue0;
                    if (initialValues != null)
                    {
                        string initialValue = this.ValueByApplyingInitialValuesForField(rawFieldValue, fieldConfig, initialValues);
                        if (!rawFieldValue.Equals(initialValue))
                        {
                            markAsChanged = true;
                            rawFieldValue = initialValue;
                        }
                    }

                    offlineChanged = false;
                    offlineValue = null;
                    if (offlineRootRecord != null)
                    {
                        offlineValue = offlineRootRecord.StringFieldValueForFieldIndex(fieldConfig.FieldId);
                        if (offlineValue != null && !offlineValue.Equals(rawFieldValue))
                        {
                            offlineChanged = true;
                        }
                    }

                    if (selector != null)
                    {
                        // Sometimes it makes sense to add the Link field , so you have the link information on the EditPage , but the field is not displayed .
                        // Thus, the field is interpreted as EditField Selector must be set.
                        if (isHidden)
                        {
                            editFieldContext = UPEditFieldContext.HiddenFieldFor(fieldConfig, fieldIdentifier, rawFieldValue);
                        }
                        else if (isReadOnly && ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Disable.82213"))
                        {
                            editFieldContext = UPEditFieldContext.ReadonlyFieldFor(fieldConfig, fieldIdentifier, rawFieldValue);
                        }
                        else
                        {
                            editFieldContext = UPEditFieldContext.FieldContextFor(fieldConfig, fieldIdentifier, rawFieldValue, selector);
                        }
                    }
                    else
                    {
                        if (isHidden)
                        {
                            editFieldContext = UPEditFieldContext.HiddenFieldFor(fieldConfig, fieldIdentifier, rawFieldValue);
                        }
                        else if (isReadOnly)
                        {
                            editFieldContext = UPEditFieldContext.ReadonlyFieldFor(fieldConfig, fieldIdentifier, rawFieldValue);
                        }
                        else
                        {
                            editFieldContext = UPEditFieldContext.FieldContextFor(fieldConfig, fieldIdentifier, rawFieldValue, childFields as List<UPEditFieldContext>);
                        }
                    }

                    if (fieldInfo.DateFieldId >= 0 && tabConfig.FieldControl.InfoAreaId == currentInfoAreaId)
                    {
                        editFieldContext.DateOriginalValue = resultRow?.RawValueForFieldIdInfoAreaIdLinkId(fieldInfo.DateFieldId, currentInfoAreaId, -1);
                    }
                    else if (fieldInfo.TimeFieldId >= 0 && tabConfig.FieldControl.InfoAreaId == currentInfoAreaId)
                    {
                        editFieldContext.TimeOriginalValue = resultRow?.RawValueForFieldIdInfoAreaIdLinkId(fieldInfo.TimeFieldId, currentInfoAreaId, -1);
                    }

                    if (offlineChanged)
                    {
                        editFieldContext.SetOfflineChangeValue(offlineValue);
                    }
                    else if (markAsChanged)
                    {
                        editFieldContext.SetChanged(true);
                    }

                    if (editFieldContext != null)
                    {
                        if (!string.IsNullOrEmpty(fieldPostfix))
                        {
                            editFieldContext.FieldLabelPostfix = fieldPostfix;
                        }

                        if (editFieldDictionary != null)
                        {
                            editFieldDictionary.SetObjectForKey(editFieldContext, fieldConfig.Identification);
                            if (childFields != null)
                            {
                                foreach (var childFieldContext in childFields)
                                {
                                    editFieldDictionary.SetObjectForKey(childFieldContext, childFieldContext.FieldConfig.Identification);
                                }
                            }
                        }

                        fieldArray.Add(editFieldContext);
                    }
                }
                else
                {
                    string fieldValue;
                    if (fieldAttributes.FieldCount > 1)
                    {
                        fieldValue = resultRow?.ValueAtIndex(fieldConfig.TabIndependentFieldIndex);
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            fieldValue = this.ValueForLinkFieldFromInitialValues(fieldConfig, initialValues);
                        }

                        var values = !string.IsNullOrEmpty(fieldValue) ?
                            new List<string> { fieldValue } :
                            new List<string>();

                        for (var k = 1; k < fieldAttributes.FieldCount; k++)
                        {
                            var childfieldConfig = tabConfig.FieldAtIndex(++j);
                            if (childfieldConfig == null)
                            {
                                continue;
                            }

                            fieldValue = resultRow?.ValueAtIndex(childfieldConfig.TabIndependentFieldIndex);
                            if (string.IsNullOrEmpty(fieldValue))
                            {
                                fieldValue = this.ValueForLinkFieldFromInitialValues(childfieldConfig, initialValues);
                            }

                            if (string.IsNullOrEmpty(fieldValue))
                            {
                                fieldValue = string.Empty;
                            }

                            values.Add(fieldValue);
                        }

                        fieldValue = fieldAttributes.FormatValues(values);
                    }
                    else
                    {
                        fieldValue = resultRow?.ValueAtIndex(fieldConfig.TabIndependentFieldIndex);
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            fieldValue = this.ValueForLinkFieldFromInitialValues(fieldConfig, initialValues);
                        }
                    }

                    UPMField field;
                    if (!isHidden && !string.IsNullOrEmpty(fieldValue))
                    {
                        field = new UPMStringField(fieldIdentifier);
                        ((UPMStringField)field).StringValue = fieldValue;
                    }
                    else
                    {
                        field = null;
                    }

                    if (field != null)
                    {
                        if (!fieldConfig.Attributes.NoLabel)
                        {
                            field.LabelText = fieldConfig.Label;
                        }

                        SetAttributesOnField(fieldAttributes, field);
                        fieldArray.Add(field);
                    }
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// Edits the contexts for.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="tabConfig">The tab configuration.</param>
        /// <param name="editFieldDictionary">The edit field dictionary.</param>
        /// <param name="initialValues">The initial values.</param>
        /// <param name="initialRecords">The initial records.</param>
        /// <returns></returns>
        public List<object> EditContextsFor(UPCRMResultRow resultRow, FieldControlTab tabConfig,
            Dictionary<string, UPEditFieldContext> editFieldDictionary, Dictionary<string, object> initialValues, List<UPCRMRecord> initialRecords)
        {
            return this.EditContextsForResultRow(resultRow, tabConfig, editFieldDictionary, initialValues, null, initialRecords);
        }

        /// <summary>
        /// Edits the group model controller for.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="editPageContext">The edit page context.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPGroupModelController EditGroupModelControllerFor(FieldControl fieldControl, int tabIndex,
            UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
        {
            var tab = fieldControl.TabAtIndex(tabIndex);
            var tabType = tab.Type;
            if (tabType.StartsWith("CHILDREN"))
            {
                if (fieldControl.ControlName != "Edit")
                {
                    // CHILDREN only supported in explicit Edit Controls
                    return null;
                }

                //2019-05-27 Vipul Commented out code for link participants
                //return new EditChildrenGroupModelController(fieldControl, tabIndex, editPageContext, theDelegate);
                return null;
            }

            if (tabType.StartsWith("REPPARTICIPANTS") || tabType.StartsWith("PARTICIPANTS"))
            {
                return new UPEditRepParticipantsGroupModelController(fieldControl, tabIndex, editPageContext, theDelegate);
            }

            if (tabType.StartsWith("LINKPARTICIPANTS"))
            {
                //2019-05-27 Vipul Commented out code for link participants
                //return new UPEditLinkParticipantsGroupModelController(fieldControl, tabIndex, editPageContext, theDelegate);
                return null;
            }

            if (tabType.StartsWith("EDITHEADER"))
            {
                return new UPEditFieldGroupModelController(fieldControl, tabIndex, editPageContext, theDelegate, true);
            }

            if (fieldControl.ControlName == "Details" && tabType == "GRID")
            {
                // PVCS 81779  Dont ignore Grid groups from Detail
                return new UPEditFieldGroupModelController(fieldControl, tabIndex, editPageContext, theDelegate);
            }

            if (!string.IsNullOrEmpty(tabType) && tabType != "GRID")
            {
                // ignore other Tab Types
                return null;
            }

            return new UPEditFieldGroupModelController(fieldControl, tabIndex, editPageContext, theDelegate);
        }
    }
}
