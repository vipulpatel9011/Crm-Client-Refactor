// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditPageContext.cs" company="Aurea Software Gmbh">
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
//   Edit Page Context implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Contexts
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.UIModel.Fields.Edit;

    /// <summary>
    /// Context information for an edit page
    /// </summary>
    public class UPEditPageContext
    {
        private List<UPEditFieldContext> orderedEditFieldContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditPageContext"/> class.
        /// </summary>
        /// <param name="rootRecordIdentification">The root record identification.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        /// <param name="initialValues">The _initial values.</param>
        /// <param name="offlineRequest">The _offline request (UPOfflineRecordRequest).</param>
        /// <param name="viewReference">The _view reference.</param>
        public UPEditPageContext(string rootRecordIdentification, bool isNew, Dictionary<string, object> initialValues,
            UPOfflineRecordRequest offlineRequest, ViewReference viewReference)
        {
            this.InitialValues = initialValues;
            this.EditFields = new Dictionary<string, UPEditFieldContext>();
            this.IsNewRecord = isNew;
            this.RootRecordIdentification = rootRecordIdentification;
            this.OfflineRequest = offlineRequest;
            this.ViewReference = viewReference;
        }

        /// <summary>
        /// Gets the context for keys.
        /// </summary>
        /// <value>
        /// The context for keys.
        /// </value>
        public Dictionary<string, object> ContextForKeys { get; private set; }

        /// <summary>
        /// Gets or sets the initial values.
        /// </summary>
        /// <value>
        /// The initial values.
        /// </value>
        public Dictionary<string, object> InitialValues { get; set; }

        /// <summary>
        /// Gets the name of the context for function.
        /// </summary>
        /// <value>
        /// The name of the context for function.
        /// </value>
        public Dictionary<string, UPEditFieldContext> ContextForFunctionName { get; private set; }

        /// <summary>
        /// Gets the contexts for CRM field.
        /// </summary>
        /// <value>
        /// The contexts for CRM field.
        /// </value>
        public Dictionary<string, List<UPEditFieldContext>> ContextsForCrmField { get; private set; }

        /// <summary>
        /// Gets the edit fields.
        /// </summary>
        /// <value>
        /// The edit fields.
        /// </value>
        public Dictionary<string, UPEditFieldContext> EditFields { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new record.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is new record; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewRecord { get; private set; }

        /// <summary>
        /// Gets the root record identification.
        /// </summary>
        /// <value>
        /// The root record identification.
        /// </value>
        public string RootRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineRecordRequest OfflineRequest { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets or sets the client check filter.
        /// </summary>
        /// <value>
        /// The client check filter.
        /// </value>
        public UPConfigFilter ClientCheckFilter { get; set; }

#if PORTING
        public UPCRMEditTrigger EditTrigger { get; set; }
#else
        public dynamic EditTrigger { get; set; }
#endif

        /// <summary>
        /// Gets the ordered edit field contexts.
        /// </summary>
        /// <value>
        /// The ordered edit field contexts.
        /// </value>
        public List<UPEditFieldContext> OrderedEditFieldContexts
        {
            get
            {
                if (this.orderedEditFieldContexts != null)
                {
                    return this.orderedEditFieldContexts;
                }

                var independentFields = new List<UPEditFieldContext>();
                List<UPEditFieldContext> dependentFields = null;
                foreach (var ctx in this.EditFields.Values)
                {
                    if (ctx.ParentField != null && this.EditFields.ValueOrDefault(ctx.ParentField.FieldIdentification) != null)
                    {
                        if (dependentFields == null)
                        {
                            dependentFields = new List<UPEditFieldContext> { ctx };
                        }
                        else
                        {
                            dependentFields.Add(ctx);
                        }
                    }
                    else
                    {
                        independentFields.Add(ctx);
                    }
                }

                if (dependentFields != null && dependentFields.Any())
                {
                    independentFields.AddRange(dependentFields);
                }

                this.orderedEditFieldContexts = independentFields;
                return this.orderedEditFieldContexts;
            }
        }

        /// <summary>
        /// Handles the dependent fields.
        /// </summary>
        public void HandleDependentFields()
        {
            this.orderedEditFieldContexts = null;
            var parentFieldContextArray = new List<object>();
            foreach (var fieldContext in this.EditFields.Values)
            {
                var parentField = fieldContext.ParentField;
                if (parentField != null)
                {
                    UPEditFieldContext parentFieldContext = this.EditFields.ValueOrDefault(parentField.FieldIdentification);
                    if (parentFieldContext != null)
                    {
                        parentFieldContext.AddDependentFieldContext(fieldContext);
                        if (!parentFieldContextArray.Contains(parentFieldContext))
                        {
                            parentFieldContextArray.Add(parentFieldContext);
                        }
                    }
                }
            }

            foreach (UPEditFieldContext parentFieldContext in parentFieldContextArray)
            {
                parentFieldContext.NotifyDependentFields();
            }
        }

        /// <summary>
        /// Adds the edit field contexts.
        /// </summary>
        /// <param name="editFieldContextArray">The edit field context array.</param>
        public void AddEditFieldContexts(List<object> editFieldContextArray)
        {
            if (editFieldContextArray == null)
            {
                return;
            }

            this.orderedEditFieldContexts = null;
            foreach (var field in editFieldContextArray)
            {
                var editFieldContext = field as UPEditFieldContext;
                if (editFieldContext == null)
                {
                    continue;
                }

                if (this.EditTrigger != null && !string.IsNullOrEmpty(editFieldContext.FieldFunction))
                {
                    var foundKey = false;
                    string foundAdditionalKey = null;
                    if (this.EditTrigger.RulesForFunctionName(editFieldContext.FieldFunction) != null)
                    {
                        foundKey = true;
                    }

                    if (editFieldContext.MayHaveExtKey)
                    {
                        foundAdditionalKey = $"{editFieldContext.FieldFunction}.extkey";
                        if (this.EditTrigger.RulesForFunctionName(foundAdditionalKey) == null)
                        {
                            foundAdditionalKey = null;
                        }
                    }

                    if (foundKey || foundAdditionalKey != null)
                    {
                        if (editFieldContext.Field is UPMEditField)
                        {
                            var editField = (UPMEditField)editFieldContext.Field;
                            editField.ContinuousUpdate = true;
                        }

                        if (this.ContextForFunctionName == null)
                        {
                            this.ContextForFunctionName = new Dictionary<string, UPEditFieldContext>();
                        }

                        if (foundKey)
                        {
                            this.ContextForFunctionName[editFieldContext.FieldFunction] = editFieldContext;
                        }

                        if (foundAdditionalKey != null)
                        {
                            this.ContextForFunctionName[foundAdditionalKey] = editFieldContext;
                        }
                    }
                }

                if (this.ContextForKeys == null)
                {
                    this.ContextForKeys = new Dictionary<string, object> { { editFieldContext.UniqueKey, editFieldContext } };
                }
                else
                {
                    this.ContextForKeys.SetObjectForKey(editFieldContext, editFieldContext.UniqueKey);
                }

                var fieldKey = editFieldContext.FieldConfig.Field.FieldIdentification;
                if (fieldKey != null)
                {
                    if (this.ContextsForCrmField == null)
                    {
                        this.ContextsForCrmField = new Dictionary<string, List<UPEditFieldContext>>();
                    }

                    var contexts = this.ContextsForCrmField.ValueOrDefault(fieldKey);
                    if (contexts == null)
                    {
                        contexts = new List<UPEditFieldContext> { editFieldContext };
                        this.ContextsForCrmField[fieldKey] = contexts;
                    }
                    else
                    {
                        contexts.Add(editFieldContext);
                    }
                }

                if (editFieldContext.ChildFields != null)
                {
                    this.AddEditFieldContexts(editFieldContext.ChildFields?.Select(s => s as object).ToList());
                }
            }
        }

        /// <summary>
        /// Contexts for edit field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public UPEditFieldContext ContextForEditField(UPMEditField field)
        {
            return this.EditFields?.Values?.FirstOrDefault(context => context.EditField == field);
        }
    }
}
