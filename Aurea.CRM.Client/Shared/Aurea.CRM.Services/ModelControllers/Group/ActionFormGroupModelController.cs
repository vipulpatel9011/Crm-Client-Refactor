// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionFormGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The UPActionFormGroupModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using UIModel.Fields;

    /// <summary>
    /// UPActionFormGroupModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Edit.UPEditFieldGroupModelController" />
    /// <seealso cref="Aurea.CRM.UIModel.Fields.Edit.IEditFieldDelegate" />
    public class UPActionFormGroupModelController : UPEditFieldGroupModelController, IEditFieldDelegate
    {
        private Dictionary<string, UPEditFieldContext> editFieldDictionary;
        private Dictionary<string, string> mustFieldDictionary;
        private Dictionary<string, string> dependentFieldDictionary;
        private bool skipTemplateFilter;
        private int emptyMustFieldCount;
        private Dictionary<string, string> linkDictionary;
        private UPEditFieldContext currentContext;
        private UPMEditField currentField;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPActionFormGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="Exception">
        /// Invalid Config Name.
        /// or
        /// Invalid TabIndex.
        /// </exception>
        public UPActionFormGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
            string configName = this.FormItem.ViewReference.ContextValueForKey("Func0");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.FieldControl = configStore.FieldControlByNameFromGroup("Edit", configName);
            int tabIndex = 0;
            if (this.FieldControl == null)
            {
                var parts = configName.Split('_');
                if (parts.Length < 2)
                {
                    throw new Exception("Invalid Config Name.");
                }

                this.FieldControl = configStore.FieldControlByNameFromGroup("Edit", parts[0]);
                if (this.FieldControl != null)
                {
                    tabIndex = Convert.ToInt32(parts[1]);
                    if (tabIndex < 0 || tabIndex >= this.FieldControl.NumberOfTabs)
                    {
                        throw new Exception("Invalid TabIndex.");
                    }
                }
            }

            this.enableLinkedEditFields = true;
            this.editFieldDictionary = new Dictionary<string, UPEditFieldContext>();
            this.TabConfig = this.FieldControl.TabAtIndex(tabIndex);
            Dictionary<string, string> _dependentFieldDictionary = null;
            foreach (UPConfigFieldControlField field in this.TabConfig.Fields)
            {
                string inheritance = field.Attributes.ExtendedOptionForKey("Inherits");
                if (!string.IsNullOrEmpty(field.Function) && inheritance != null)
                {
                    if (_dependentFieldDictionary == null)
                    {
                        _dependentFieldDictionary = new Dictionary<string, string> { { inheritance, field.Function } };
                    }
                    else
                    {
                        _dependentFieldDictionary[inheritance] = field.Function;
                    }
                }
            }

            if (_dependentFieldDictionary != null)
            {
                this.dependentFieldDictionary = _dependentFieldDictionary;
                foreach (string val in _dependentFieldDictionary.Keys)
                {
                    this.AddDependingKey(val);
                }
            }

            this.mustFieldDictionary = new Dictionary<string, string>();
            this.emptyMustFieldCount = -1;
            this.SignalEveryChange = !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Disable.295");
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="contextDictionary">The context dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> contextDictionary)
        {
            UPMGroup returnGroup;
            base.ApplyContext(contextDictionary);
            if (!this.skipTemplateFilter)
            {
                string templateFilterName = this.FormItem.ViewReference.ContextValueForKey("Func1");
                Dictionary<string, object> initialValues = null;
                if (!string.IsNullOrEmpty(templateFilterName))
                {
                    UPConfigFilter templateFilter = ConfigurationUnitStore.DefaultStore.FilterByName(templateFilterName);
                    if (templateFilter != null)
                    {
                        Dictionary<string, object> dict = contextDictionary["$_copyFields"] as Dictionary<string, object>;
                        templateFilter = templateFilter.FilterByApplyingValueDictionaryDefaults(dict, true);
                        initialValues = templateFilter.FieldsWithValues(true, true);
                    }
                }

                this.EditPageContext = new UPEditPageContext(this.FieldControl.InfoAreaId, true, initialValues, null, null);
                returnGroup = this.ApplyResultRow(null);
                this.skipTemplateFilter = true;
            }
            else
            {
                returnGroup = this.Group;
            }

            if (this.dependentFieldDictionary != null)
            {
                foreach (string dependentKey in this.dependentFieldDictionary.Keys)
                {
                    if (contextDictionary.Keys.Contains(dependentKey))
                    {
                        string depVal = contextDictionary[dependentKey] as string;
                        if (!string.IsNullOrEmpty(depVal))
                        {
                            UPEditFieldContext editFieldContext =
                                this.editFieldDictionary[this.dependentFieldDictionary[dependentKey]];
                            if (editFieldContext != null)
                            {
                                editFieldContext.Value = depVal;
                                if (editFieldContext.EditField != null)
                                {
                                    this.SimpleChangedValue(editFieldContext.EditField);
                                }
                            }
                        }
                    }
                }
            }

            return returnGroup;
        }

        /// <summary>
        /// Fieldses for edit field context.
        /// </summary>
        /// <param name="editFieldContext">The edit field context.</param>
        /// <returns>
        /// The <see cref="List" />.
        /// </returns>
        public override List<UPMField> FieldsForEditFieldContext(UPEditFieldContext editFieldContext)
        {
            List<UPMField> editFieldArray = base.FieldsForEditFieldContext(editFieldContext);

            if (editFieldArray != null)
            {
                foreach (UPMEditField editField in editFieldArray)
                {
                    editField.EditFieldDelegate = this;
                    editField.ContinuousUpdate = true;
                    if (!string.IsNullOrEmpty(editFieldContext.FieldFunction))
                    {
                        this.editFieldDictionary[editFieldContext.FieldFunction] = editFieldContext;
                        if (editField.RequiredField)
                        {
                            if (!editFieldContext.FieldConfig.IsEmptyValue(string.Empty))
                            {
                                this.mustFieldDictionary[editFieldContext.FieldFunction] = "0";
                            }
                            else
                            {
                                this.mustFieldDictionary[editFieldContext.FieldFunction] = string.Empty;
                            }
                        }
                    }

                    if (editField is UPMCatalogEditField)
                    {
                        ((UPMCatalogEditField)editField).CatalogElementViewType = CatalogElementViewType.PopOver;
                    }
                }
            }

            return editFieldArray;
        }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [signal every change].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [signal every change]; otherwise, <c>false</c>.
        /// </value>
        public bool SignalEveryChange { get; private set; }

        /// <summary>
        /// Gets the Group delegate.
        /// </summary>
        public override object GroupDelegate => this;

        /// <summary>
        /// Groups from row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns>
        /// The <see cref="UPMGroup" />.
        /// </returns>
        public override UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            UPMGroup group = base.GroupFromRow(resultRow);
            this.skipTemplateFilter = false;
            if (group != null)
            {
                if (this.emptyMustFieldCount < 0)
                {
                    this.emptyMustFieldCount = 0;
                    List<string> mustFieldFunctionNames = this.mustFieldDictionary.Keys.ToList();
                    foreach (string functionName in mustFieldFunctionNames)
                    {
                        UPEditFieldContext ctx = this.editFieldDictionary[functionName];
                        if (ctx.FieldConfig.IsEmptyValue(ctx.Value))
                        {
                            this.emptyMustFieldCount++;
                            this.mustFieldDictionary[functionName] = string.Empty;
                        }
                        else if (string.IsNullOrEmpty(ctx.Value))
                        {
                            this.mustFieldDictionary[functionName] = "0";
                        }
                        else
                        {
                            this.mustFieldDictionary[functionName] = ctx.Value;
                        }
                    }
                }

                this.Delegate.GroupModelControllerValueChanged(this, this.CurrentData);
            }

            return group;
        }

        /// <summary>
        /// Gets the current data.
        /// </summary>
        /// <value>
        /// The current data.
        /// </value>
        public override Dictionary<string, object> CurrentData
        {
            get
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                foreach (string name in this.editFieldDictionary.Keys)
                {
                    UPEditFieldContext editFieldContext = this.editFieldDictionary[name];
                    string val = editFieldContext.ServerValue;
                    if (!editFieldContext.FieldConfig.IsEmptyValue(val))
                    {
                        if (string.IsNullOrEmpty(val))
                        {
                            val = "0";
                        }
                    }

                    if (!string.IsNullOrEmpty(val))
                    {
                        data[name] = val;
                    }
                }

                if (this.emptyMustFieldCount > 0)
                {
                    List<string> emptyMustFields = this.mustFieldDictionary.Keys.Where(mf => string.IsNullOrEmpty(this.mustFieldDictionary[mf])).ToList();

                    data[".empty"] = emptyMustFields;
                }

                if (this.linkDictionary?.Count > 0)
                {
                    data[".links"] = this.linkDictionary;
                }

                return data;
            }
        }

        private bool NeedsSignalValueChange(UPMEditField editField)
        {
            if (editField.RequiredField)
            {
                if (this.currentField != editField)
                {
                    foreach (UPEditFieldContext ctx in this.editFieldDictionary.Values)
                    {
                        if (ctx.EditField == editField)
                        {
                            this.currentContext = ctx;
                            break;
                        }
                    }

                    this.currentField = editField;
                }

                if (this.currentContext != null)
                {
                    string val = this.currentContext.Value;
                    string storedVal = this.mustFieldDictionary[this.currentContext.FieldFunction];
                    if (!string.IsNullOrEmpty(val) && string.IsNullOrEmpty(storedVal))
                    {
                        this.mustFieldDictionary[this.currentContext.FieldFunction] = val;
                        return --this.emptyMustFieldCount == 0;
                    }

                    if (string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(storedVal))
                    {
                        this.mustFieldDictionary[this.currentContext.FieldFunction] = string.Empty;
                        return ++this.emptyMustFieldCount == 1;
                    }

                    if (!string.IsNullOrEmpty(val))
                    {
                        this.mustFieldDictionary[this.currentContext.FieldFunction] = val;
                    }

                    return this.emptyMustFieldCount <= 0 && this.SignalEveryChange;
                }
            }

            return this.SignalEveryChange;
        }

        private List<UPMEditField> ApplyCopyValuesRecordSelectorEditField(Dictionary<string, object> copyValues, UPMRecordSelectorEditField field)
        {
            List<UPMEditField> changedFields = new List<UPMEditField>();
            if (copyValues != null && field.EditFieldsContext != null)
            {
                UPEditPageContext fieldEditPageContext = (UPEditPageContext)field.EditFieldsContext;
                foreach (UPEditFieldContext fieldContext in fieldEditPageContext.EditFields.Values)
                {
                    if (!string.IsNullOrEmpty(fieldContext.FieldFunction))
                    {
                        string value = copyValues[fieldContext.FieldFunction] as string;
                        if (value != null)
                        {
                            fieldContext.Value = value;
                            fieldContext.SetChanged(true);
                            if (fieldContext.EditField != null)
                            {
                                changedFields.Add(fieldContext.EditField);
                            }
                        }
                    }
                }
            }

            return changedFields;
        }

        private void UserDidChangeRecordSelectorEditField(UPMRecordSelectorEditField field)
        {
            UPRecordSelector selector = field.CurrentSelector;
            Dictionary<string, object> copyValues = field.ResultRows != null ? field.ResultRows.FunctionValues : selector.ValuesFromResultRow(null);

            string linkRecordIdentification = field.ResultRows.RootRecordIdentification;
            string linkKey = selector.LinkKey;
            if (!selector.NoLink && !string.IsNullOrEmpty(linkKey))
            {
                if (!string.IsNullOrEmpty(linkRecordIdentification))
                {
                    if (this.linkDictionary != null)
                    {
                        this.linkDictionary[linkKey] = linkRecordIdentification;
                    }
                    else
                    {
                        this.linkDictionary = new Dictionary<string, string> { { linkKey, linkRecordIdentification } };
                    }
                }
                else if (!string.IsNullOrEmpty(linkKey) && this.linkDictionary.ContainsKey(linkKey))
                {
                    this.linkDictionary.Remove(linkKey);
                }
            }

            List<UPMEditField> changedFields = this.ApplyCopyValuesRecordSelectorEditField(copyValues, field);
            foreach (UPMEditField editField in changedFields)
            {
                this.SimpleChangedValue(editField);
            }

            if (changedFields.Count > 0)
            {
                this.Delegate.ForceRedraw(this);
            }
        }

        private void UserDidChangeCatalogField(UPMCatalogEditField editField)
        {
            UPEditPageContext fieldEditPageContext = (UPEditPageContext)editField.EditFieldsContext;
            UPEditFieldContext context = fieldEditPageContext.ContextForEditField(editField);
            if (context.HasDependentFields)
            {
                context.NotifyDependentFields();
                this.SimpleChangedValue(editField);
                this.Delegate.ForceRedraw(this);
            }
            else
            {
                this.SimpleChangedValue(editField);
            }
        }

        /// <summary>
        /// Records the selector page model controller for field.
        /// </summary>
        /// <param name="editField">The edit field.</param>
        /// <returns></returns>
        public UPRecordSelectorPageModelController RecordSelectorPageModelControllerForField(UPMRecordSelectorEditField editField)
        {
            ViewReference viewRef = editField.CurrentSelector.SearchViewReference;
            viewRef = viewRef?.ViewReferenceWith(editField.ContextRecord);

            UPRecordSelectorPageModelController searchPageController = new UPRecordSelectorPageModelController(viewRef);
            return searchPageController;
        }

        /// <summary>
        /// The field changed value.
        /// </summary>
        /// <param name="editField">The edit field.</param>
        public void FieldChangedValue(UPMEditField editField)
        {
            if (editField is UPMRecordSelectorEditField)
            {
                this.UserDidChangeRecordSelectorEditField((UPMRecordSelectorEditField)editField);
            }
            else if (editField is UPMCatalogEditField)
            {
                this.UserDidChangeCatalogField((UPMCatalogEditField)editField);
            }
            else
            {
                this.SimpleChangedValue(editField);
            }
        }

        private void SimpleChangedValue(UPMEditField editField)
        {
            if (this.NeedsSignalValueChange(editField))
            {
                this.Delegate.GroupModelControllerValueChanged(this, this.CurrentData);
            }
        }
    }
}
