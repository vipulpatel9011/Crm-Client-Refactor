// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditChildrenGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Edit Children Group Model Controller class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Edit Children Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPEditChildrenGroupModelControllerBase" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class EditChildrenGroupModelController : UPEditChildrenGroupModelControllerBase, ISearchOperationHandler
    {
        private Dictionary<string, UPMStandardGroup> groupForKey;
        private UPCRMResult retainedResult;
        private Dictionary<string, object> combinedInitialValues;
        private Dictionary<string, object> initialValues;

        /// <summary>
        /// Gets a value indicating whether this instance is new link record.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is new link record; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewLinkRecord { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the child field control.
        /// </summary>
        /// <value>
        /// The child field control.
        /// </value>
        public FieldControl ChildFieldControl { get; private set; }

        /// <summary>
        /// Gets the child edit contexts.
        /// </summary>
        /// <value>
        /// The child edit contexts.
        /// </value>
        public Dictionary<string, UPChildEditContext> ChildEditContexts { get; private set; }

        /// <summary>
        /// Gets the added child edit contexts.
        /// </summary>
        /// <value>
        /// The added child edit contexts.
        /// </value>
        public List<UPChildEditContext> AddedChildEditContexts { get; private set; }

        /// <summary>
        /// Gets the child information area identifier.
        /// </summary>
        /// <value>
        /// The child information area identifier.
        /// </value>
        public string ChildInfoAreaId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditChildrenGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="editPageContext">The edit page context.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="Exception">
        /// TabConfig is null
        /// or
        /// ChildFieldControl is null
        /// </exception>
        public EditChildrenGroupModelController(FieldControl fieldControl, int tabIndex, UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, editPageContext, theDelegate)
        {
            FieldControlTab tabConfig = fieldControl.TabAtIndex(tabIndex);
            if (tabConfig == null)
            {
                throw new Exception("TabConfig is null");
            }

            var typeParts = tabConfig.Type.Split('_');
            if (typeParts.Length > 1)
            {
                string detailsConfigurationName = (string)typeParts[1];
                var configNameParts = detailsConfigurationName.Split('#');
                if (configNameParts.Length > 1)
                {
                    this.LinkId = Convert.ToInt32(configNameParts[1]);
                    detailsConfigurationName = configNameParts[0];
                }

                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                this.ChildFieldControl = configStore.FieldControlByNameFromGroup("Edit", detailsConfigurationName);
                if (this.ChildFieldControl.NumberOfTabs > 1)
                {
                    this.ChildFieldControl = this.ChildFieldControl.FieldControlWithSingleTab(0);
                }

                this.ChildInfoAreaId = this.ChildFieldControl.InfoAreaId;
                if (typeParts.Length > 2)
                {
                    UPConfigFilter templateFilter = configStore.FilterByName((string)typeParts[2]);
                    if (templateFilter != null)
                    {
                        templateFilter = templateFilter.FilterByApplyingReplacements(UPConditionValueReplacement.DefaultParameters);
                        this.initialValues = templateFilter.FieldsWithConditions(false);
                    }
                }
            }
            else if (tabConfig.NumberOfFields > 0)
            {
                UPConfigFieldControlField childField = tabConfig.FieldAtIndex(0);
                this.ChildInfoAreaId = childField.InfoAreaId;
                this.LinkId = childField.LinkId;
                this.ChildFieldControl = fieldControl.FieldControlWithSingleTabRootInfoAreaIdRootLinkId(tabIndex, this.ChildInfoAreaId, this.LinkId);
            }
            else
            {
                this.ChildFieldControl = null;
            }

            if (this.ChildFieldControl == null)
            {
                throw new Exception("ChildFieldControl is null");
            }
        }

        private void AddChildRecordContext(UPChildEditContext childEditContext)
        {
            if (!childEditContext.IsNew)
            {
                if (this.ChildEditContexts == null)
                {
                    this.ChildEditContexts = new Dictionary<string, UPChildEditContext>();
                }

                this.ChildEditContexts.SetObjectForKey(childEditContext, childEditContext.RecordIdentification);
            }
            else
            {
                if (this.AddedChildEditContexts == null)
                {
                    this.AddedChildEditContexts = new List<UPChildEditContext>();
                }

                this.AddedChildEditContexts.Add(childEditContext);
            }
        }

        private UPChildEditContext ChildEditContextForGroup(UPMStandardGroup group)
        {
            UPChildEditContext context = this.AddedChildEditContexts.FirstOrDefault(x => x.Group == group);
            return context ?? this.ChildEditContexts.Values.FirstOrDefault(x => x.Group == group);
        }

        private UPChildEditContext ChildContextForGroup(UPMGroup group)
        {
            UPChildEditContext editContext = this.ChildEditContexts.Values.FirstOrDefault(x => x.Group == group);
            return editContext ?? this.AddedChildEditContexts.FirstOrDefault(x => x.Group == group);
        }

        private void UpdateChildEditContext(UPChildEditContext childEditContext, Dictionary<string, object> _initialValues)
        {
            foreach (string fieldKey in _initialValues.Keys)
            {
                UPEditFieldContext editFieldContext = childEditContext.EditFieldContext[fieldKey];
                if (editFieldContext != null)
                {
                    editFieldContext.SetValue(_initialValues[fieldKey] as string);
                    editFieldContext.ChildEditContext = childEditContext;
                }
            }
        }

        private UPMStandardGroup CreateNewGroupRecordAsNew(Dictionary<string, object> parentInitialValues, UPCRMRecord record, bool asNew)
        {
            UPContainerMetaInfo metaInfo = new UPContainerMetaInfo(this.ChildFieldControl);
            UPCRMResult result = !string.IsNullOrEmpty(record?.RecordId) ? metaInfo.NewRecordWithRecordId(record.RecordId) : metaInfo.NewRecord();

            UPCRMResultRow childRow = (UPCRMResultRow)result.ResultRowAtIndex(0);
            UPChildEditContext childEditContext = new UPChildEditContext(childRow, this.NextPostfix);
            if (asNew)
            {
                childEditContext.SetAsNew();
            }

            if (record != null)
            {
                childEditContext.AddChangedLinksFromRecordParentLink(record, null);
            }

            FieldControlTab childFieldControlTab = this.ChildFieldControl.TabAtIndex(0);
            this.combinedInitialValues = null;
            if (this.initialValues != null)
            {
                if (parentInitialValues != null)
                {
                    Dictionary<string, object> combined = new Dictionary<string, object>(this.initialValues);
                    foreach (var item in parentInitialValues)
                    {
                        combined[item.Key] = item.Value;
                    }

                    this.combinedInitialValues = combined;
                }
                else
                {
                    this.combinedInitialValues = this.initialValues;
                }
            }
            else
            {
                this.combinedInitialValues = parentInitialValues;
            }

            List<UPCRMRecord> initialRecords = record != null ? new List<UPCRMRecord> { record } : null;

            List<object> editFieldContextArray = this.EditContextsFor(childRow, childFieldControlTab,
                                childEditContext.EditFieldContext, this.combinedInitialValues, initialRecords);
            int editFieldCount = editFieldContextArray.Count;
            if (editFieldCount > 0)
            {
                UPMStandardGroup group = new UPMStandardGroup(new RecordIdentifier(childRow.RootRecordIdentification));
                group.Deletable = true;
                childEditContext.Group = group;
                this.AddChildRecordContext(childEditContext);
                for (int j = 0; j < editFieldCount; j++)
                {
                    UPEditFieldContext editFieldContext = editFieldContextArray[j] as UPEditFieldContext;
                    if (editFieldContext != null)
                    {
                        editFieldContext.ChildEditContext = childEditContext;
                        editFieldContext.FieldLabelPostfix = childEditContext.FieldLabelPostfix;
                        List<UPMEditField> editFields = editFieldContext.EditFields;
                        if (editFields.Count > 0)
                        {
                            foreach (UPMEditField editField in editFields)
                            {
                                editField.EditFieldsContext = childEditContext;
                                group.AddField(editField);
                            }
                        }
                        else
                        {
                            UPMField field = editFieldContext.Field;
                            if (field != null)
                            {
                                group.AddField(field);
                            }
                        }
                    }
                    else
                    {
                        group.AddField((UPMField)editFieldContextArray[j]);
                    }
                }

                childEditContext.HandleDependentFields();
                return group;
            }

            return null;
        }

        private UPMStandardGroup CreateNewGroup(Dictionary<string, object> parentInitialValues)
        {
            return this.CreateNewGroupRecordAsNew(parentInitialValues, null, false);
        }

        private UPMStandardGroup CreateNewGroupFromRecordAsNew(UPCRMRecord record, bool asNew)
        {
            return this.CreateNewGroupRecordAsNew(null, record, asNew);
        }

        /// <summary>
        /// Adds the groups with initial values.
        /// </summary>
        /// <param name="_initialValues">The initial values.</param>
        public override void AddGroupsWithInitialValues(List<Dictionary<string, object>> _initialValues)
        {
            UPMRepeatableEditGroup repeatableEditGroup = (UPMRepeatableEditGroup)this.Group;
            foreach (var initialValuesForGroup in _initialValues)
            {
                UPMStandardGroup group = null;
                string key = (initialValuesForGroup[".Key"] as List<string>)?[0];

                if (!string.IsNullOrEmpty(key))
                {
                    group = this.groupForKey[key];
                    UPChildEditContext childEditContext = this.ChildEditContextForGroup(group);
                    if (childEditContext != null && repeatableEditGroup.Groups.Contains(group))
                    {
                        this.UpdateChildEditContext(childEditContext, initialValuesForGroup);
                        return;
                    }
                    else
                    {
                        group = null;
                    }
                }

                if (group == null)
                {
                    group = this.CreateNewGroup(initialValuesForGroup);
                    if (group != null)
                    {
                        repeatableEditGroup.AddChild(group);
                        if (!string.IsNullOrEmpty(key))
                        {
                            if (this.groupForKey != null)
                            {
                                this.groupForKey.SetObjectForKey(group, key);
                            }
                            else
                            {
                                this.groupForKey = new Dictionary<string, UPMStandardGroup> { { key, group } };
                            }
                        }
                    }
                }
            }
        }

        private static void AddFieldToGroup(UPMStandardGroup standardGroup, object field, UPChildEditContext childEditContext)
        {
            if (field is UPEditFieldContext)
            {
                var editFieldContext = (UPEditFieldContext)field;
                editFieldContext.ChildEditContext = childEditContext;

                foreach (var editField in editFieldContext.EditFields)
                {
                    editField.EditFieldsContext = childEditContext;
                    standardGroup.AddField(editField);
                }
            }
            else
            {
                standardGroup.AddField((UPMField)field);
            }
        }


        private UPMGroup GroupFromChildResult(UPCRMResult childResult)
        {
            var repeatableEditGroup =
                new UPMRepeatableEditGroup(this.TabIdentifierForRecordIdentification(this.LinkRecordIdentification))
                {
                    LabelText = this.TabLabel,
                    AddGroupLabelText = LocalizedString.TextAddNewGroup,
                    AddingEnabled = this.AddRecordEnabled
                };
            var count = childResult?.RowCount ?? 0;
            this.retainedResult = childResult;
            var childFieldControlTab = this.ChildFieldControl.TabAtIndex(0);
            this.ChildEditContexts = new Dictionary<string, UPChildEditContext>();

            this.PopulateRecords(out var addedRecords, out var changedRecords);

            if (count > 0)
            {
                this.AddRecordsToGroup(repeatableEditGroup, count, childResult, changedRecords, childFieldControlTab);
            }
            else if (this.EditPageContext.IsNewRecord && string.IsNullOrEmpty(this.EditPageContext.OfflineRequest?.Error))
            {
                count = this.AddRecordsToGroup(repeatableEditGroup);
                if (count == 0 && !this.AddRecordEnabled)
                {
                    return null;
                }
            }

            count = this.AddRecordsToGroup(repeatableEditGroup, addedRecords, count);

            if (this.MaxChildren > 0 && repeatableEditGroup.Groups.Count >= this.MaxChildren)
            {
                repeatableEditGroup.AddingEnabled = false;
            }

            this.Group = repeatableEditGroup;
            return this.Group;
        }

        private int AddRecordsToGroup(UPMRepeatableEditGroup repeatableEditGroup, List<UPCRMRecord> addedRecords, int count)
        {
            if (addedRecords != null)
            {
                foreach (var record in addedRecords)
                {
                    var standardGroup = this.CreateNewGroupFromRecordAsNew(record, true);
                    if (standardGroup != null)
                    {
                        ++count;
                        repeatableEditGroup.AddChild(standardGroup);
                    }
                }
            }

            return count;
        }

        private void PopulateRecords(out List<UPCRMRecord> addedRecords, out Dictionary<string, UPCRMRecord> changedRecords)
        {
            addedRecords = null;
            changedRecords = null;

            var existingRecords = this.EditPageContext.OfflineRequest?.RecordsWithInfoAreaLinkId(this.ChildInfoAreaId, this.LinkId);
            if (existingRecords == null)
            {
                return;
            }

            foreach (var record in existingRecords)
            {
                if (record.IsNew)
                {
                    if (addedRecords == null)
                    {
                        addedRecords = new List<UPCRMRecord> { record };
                    }
                    else
                    {
                        addedRecords.Add(record);
                    }
                }
                else if (changedRecords == null)
                {
                    changedRecords = new Dictionary<string, UPCRMRecord> { { record.RecordId, record } };
                }
                else
                {
                    changedRecords.SetObjectForKey(record, record.RecordId);
                }
            }
        }

        private void AddRecordsToGroup(
            UPMRepeatableEditGroup repeatableEditGroup,
            int count,
            UPCRMResult childResult,
            Dictionary<string, UPCRMRecord> changedRecords,
            FieldControlTab childFieldControlTab)
        {
            for (var i = 0; i < count; i++)
            {
                var childRow = (UPCRMResultRow)childResult.ResultRowAtIndex(i);
                var childEditContext = new UPChildEditContext(childRow, this.NextPostfix);
                var changedRecord = changedRecords.ValueOrDefault(childRow.RootRecordId);
                var initialRecords = changedRecord != null ? new List<UPCRMRecord> { changedRecord } : null;

                var editFieldContextArray = this.EditContextsForResultRow(childRow, childFieldControlTab, childEditContext.EditFieldContext, null, childEditContext.FieldLabelPostfix, initialRecords);
                var editFieldCount = editFieldContextArray.Count;
                if (editFieldCount > 0)
                {
                    var standardGroup = new UPMStandardGroup(new RecordIdentifier(childRow.RootRecordIdentification))
                    {
                        Deletable = this.DeleteRecordEnabled
                    };
                    childEditContext.Group = standardGroup;
                    this.AddChildRecordContext(childEditContext);
                    for (var j = 0; j < editFieldCount; j++)
                    {
                        var field = editFieldContextArray[j];
                        AddFieldToGroup(standardGroup, field, childEditContext);
                    }

                    childEditContext.HandleDependentFields();
                    repeatableEditGroup.AddChild(standardGroup);
                }
            }
        }

        private int AddRecordsToGroup(UPMRepeatableEditGroup repeatableEditGroup)
        {
            var count = 0;
            var parentInitialValues = this.EditPageContext.InitialValues;
            var childGroupKey = this.ChildGroupKey;
            var parentInitialValueDictionaries = parentInitialValues.ValueOrDefault(childGroupKey) as List<Dictionary<string, object>>;
            if (parentInitialValueDictionaries != null)
            {
                foreach (var parentInitialValueForChild in parentInitialValueDictionaries)
                {
                    var standardGroup = this.CreateNewGroup(parentInitialValueForChild);
                    if (standardGroup != null)
                    {
                        ++count;
                        repeatableEditGroup.AddChild(standardGroup);
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the child group key.
        /// </summary>
        /// <value>
        /// The child group key.
        /// </value>
        public override string ChildGroupKey => ChildGroupKeyForInfoAreaIdLinkId(this.ChildInfoAreaId, this.LinkId);

        /// <summary>
        /// Gets the target information area identifier.
        /// </summary>
        /// <value>
        /// The target information area identifier.
        /// </value>
        public override string TargetInfoAreaId => this.ChildInfoAreaId;

        /// <summary>
        /// Gets the target link identifier.
        /// </summary>
        /// <value>
        /// The target link identifier.
        /// </value>
        public override int TargetLinkId => this.LinkId;

        /// <summary>
        /// Gets a value indicating whether [delete record enabled default].
        /// </summary>
        /// <value>
        /// <c>true</c> if [delete record enabled default]; otherwise, <c>false</c>.
        /// </value>
        public override bool DeleteRecordEnabledDefault => false;

        private UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            if (resultRow.IsNewRow)
            {
                if (this.AddRecordEnabled)
                {
                    this.Group = this.GroupFromChildResult(null);
                    this.ControllerState = GroupModelControllerState.Finished;
                }
                else
                {
                    this.Group = null;
                    this.ControllerState = GroupModelControllerState.Empty;
                }

                return this.Group;
            }

            UPContainerMetaInfo childMetaInfo = new UPContainerMetaInfo(this.ChildFieldControl);
            this.LinkRecordIdentification = resultRow.RootRecordIdentification;
            childMetaInfo.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
            if (this.RequestOption == UPRequestOption.Offline || this.RequestOption == UPRequestOption.FastestAvailable)
            {
                UPCRMResult childResult = childMetaInfo.Find();
                if (childResult.RowCount > 0)
                {
                    this.ControllerState = GroupModelControllerState.Finished;
                    return this.GroupFromChildResult(childResult);
                }
            }

            if (this.RequestOption != UPRequestOption.Offline)
            {
                Operation operation = childMetaInfo.Find(this);
                if (operation != null)
                {
                    this.ControllerState = GroupModelControllerState.Error;
                    return null;
                }
                else
                {
                    this.ControllerState = GroupModelControllerState.Pending;
                    return null;
                }
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Empty;
                return this.GroupFromChildResult(null);
            }
        }

        /// <summary>
        /// Adds the record enabled default.
        /// </summary>
        /// <param name="isNewRecord">if set to <c>true</c> [is new record].</param>
        /// <returns></returns>
        public override bool AddRecordEnabledDefault(bool isNewRecord)
        {
            return isNewRecord;
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            return this.GroupFromRow(row);
        }

        /// <summary>
        /// Changeds the records for context new record link user changes only.
        /// </summary>
        /// <param name="childEditContext">The child edit context.</param>
        /// <param name="newRecord">if set to <c>true</c> [new record].</param>
        /// <param name="link">The link.</param>
        /// <param name="userChangesOnly">if set to <c>true</c> [user changes only].</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecordsForContextNewRecordLinkUserChangesOnly(UPChildEditContext childEditContext, bool newRecord, UPCRMLink link, bool userChangesOnly)
        {
            if (childEditContext.DeleteRecord)
            {
                if (newRecord)
                {
                    return null;
                }

                UPCRMRecord record = new UPCRMRecord(childEditContext.RecordIdentification);
                record.Deleted = true;
                return new List<UPCRMRecord> { record };
            }

            Dictionary<string, UPEditFieldContext> changedFields = null;
            if (newRecord && !userChangesOnly)
            {
                if (this.combinedInitialValues != null)
                {
                    foreach (string fieldIdentification in this.combinedInitialValues.Keys)
                    {
                        int fieldId = fieldIdentification.FieldIdFromStringWithInfoAreaId(this.ChildInfoAreaId);
                        if (fieldId >= 0)
                        {
                            UPEditFieldContext initialValueEditField = new UPEditFieldContext(fieldId, this.combinedInitialValues[fieldIdentification] as string);
                            if (changedFields == null)
                            {
                                changedFields = new Dictionary<string, UPEditFieldContext> { { initialValueEditField.Key, initialValueEditField } };
                            }
                            else
                            {
                                changedFields.SetObjectForKey(initialValueEditField, initialValueEditField.Key);
                            }
                        }
                    }
                }
            }

            foreach (UPEditFieldContext editFieldContext in childEditContext.EditFieldContext.Values)
            {
                if (editFieldContext.WasChanged(userChangesOnly))
                {
                    if (changedFields != null)
                    {
                        changedFields.SetObjectForKey(editFieldContext, editFieldContext.Key);
                    }
                    else
                    {
                        changedFields = new Dictionary<string, UPEditFieldContext> { { editFieldContext.Key, editFieldContext } };
                    }
                }
            }

            List<UPCRMRecord> additionalRecords = null;
            ICollection<UPCRMLink> changedLinks = (ICollection<UPCRMLink>)childEditContext.ChangedLinkArray ?? new List<UPCRMLink>();
            if (changedFields?.Count > 0 || changedLinks?.Count > 0)
            {
                UPCRMRecord record;
                if (newRecord)
                {
                    record = childEditContext.RecordIdentification.RecordId().Length > 6
                        ? new UPCRMRecord(childEditContext.RecordIdentification)
                        : UPCRMRecord.CreateNew(childEditContext.RecordIdentification.InfoAreaId());

                    record.AddLink(link);
                    UPConfigFilter createFilter = ConfigurationUnitStore.DefaultStore.FilterByName($"{record.InfoAreaId}.ChildCreateTemplate");
                    createFilter = createFilter.FilterByApplyingDefaultReplacements();
                    additionalRecords = record.ApplyValuesFromTemplateFilter(createFilter, false);
                }
                else
                {
                    record = new UPCRMRecord(childEditContext.RecordIdentification);
                }

                foreach (UPEditFieldContext changedField in changedFields.Values)
                {
                    if (newRecord)
                    {
                        record.NewValueFieldId(changedField.Value, changedField.FieldId);
                    }
                    else
                    {
                        record.NewValueFromValueFieldId(changedField.Value, changedField.OriginalValue, changedField.FieldId);
                    }
                }

                    foreach (UPCRMLink changedLink in changedLinks)
                    {
                        record.AddLink(changedLink);
                    }

                if (additionalRecords?.Count > 0)
                {
                    List<UPCRMRecord> arr = new List<UPCRMRecord> { record };
                    arr.AddRange(additionalRecords);
                    return arr;
                }

                return new List<UPCRMRecord> { record };
            }

            return null;
        }

        /// <summary>
        /// Returns changed child records list for given parent record
        /// </summary>
        /// <param name="parentRecord">Parent record</param>
        /// <param name="userChangesOnly">User changes only</param>
        /// <returns>
        ///   <see cref="List{UPCRMRecord}" />
        /// </returns>
        public override List<UPCRMRecord> ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
        {
            List<UPCRMRecord> changedChildRecords = null;
            if (this.ChildEditContexts != null)
            {
                foreach (UPChildEditContext childEditContext in this.ChildEditContexts.Values)
                {
                    List<UPCRMRecord> records = this.ChangedRecordsForContextNewRecordLinkUserChangesOnly(childEditContext, false, null, userChangesOnly);
                    if (records?.Count > 0)
                    {
                        if (changedChildRecords != null)
                        {
                            changedChildRecords.AddRange(records);
                        }
                        else
                        {
                            changedChildRecords = new List<UPCRMRecord>(records);
                        }
                    }
                }

                if (this.AddedChildEditContexts != null)
                {
                    UPCRMLink link = new UPCRMLink(parentRecord, -1);
                    foreach (UPChildEditContext childEditContext in this.AddedChildEditContexts)
                    {
                        List<UPCRMRecord> records = this.ChangedRecordsForContextNewRecordLinkUserChangesOnly(childEditContext, true, link, userChangesOnly);
                        if (records?.Count > 0)
                        {
                            if (changedChildRecords != null)
                            {
                                changedChildRecords.AddRange(records);
                            }
                            else
                            {
                                changedChildRecords = new List<UPCRMRecord>(records);
                            }
                        }
                    }
                }
            }

            return changedChildRecords?.Count > 0 ? changedChildRecords : null;
        }

        /// <summary>
        /// Constraints the violations with page context.
        /// </summary>
        /// <param name="editPageContext">The edit page context.</param>
        /// <returns></returns>
        public override List<UPEditConstraintViolation> ConstraintViolationsWithPageContext(UPEditPageContext editPageContext)
        {
            List<UPEditConstraintViolation> violations = null;
            foreach (UPChildEditContext childEditContext in this.ChildEditContexts?.Values)
            {
                foreach (UPEditFieldContext fieldContext in childEditContext.EditFieldContext.Values)
                {
                    List<UPEditConstraintViolation> contraintViolations = fieldContext?.ConstraintViolationsWithPageContext(editPageContext);
                    if (contraintViolations != null)
                    {
                        if (violations != null)
                        {
                            violations.AddRange(contraintViolations);
                        }
                        else
                        {
                            violations = new List<UPEditConstraintViolation>(contraintViolations);
                        }
                    }
                }
            }

            if (this.AddedChildEditContexts != null)
            {
                foreach (UPChildEditContext childEditContext in this.AddedChildEditContexts)
                {
                    foreach (UPEditFieldContext fieldContext in childEditContext.EditFieldContext.Values)
                    {
                        List<UPEditConstraintViolation> contraintViolations = fieldContext.ConstraintViolationsWithPageContext(editPageContext);
                        if (contraintViolations != null)
                        {
                            if (violations != null)
                            {
                                violations.AddRange(contraintViolations);
                            }
                            else
                            {
                                violations = new List<UPEditConstraintViolation>(contraintViolations);
                            }
                        }
                    }
                }
            }

            return violations;
        }

        /// <summary>
        /// Handles adding new item to repeatable edit group
        /// </summary>
        /// <param name="repeatableEditGroup">Repeatable edit group for adding new item</param>
        public override void UserWillInsertNewGroupInRepeatableEditGroup(UPMRepeatableEditGroup repeatableEditGroup)
        {
            string key = $"#{this.ChildInfoAreaId}.{(this.LinkId < 0 ? 0 : this.LinkId)}";
            var childInitialValueArray = this.EditPageContext.InitialValues[key] as List<Dictionary<string, object>>;
            Dictionary<string, object> childInitialValues = null;

            if (childInitialValueArray?.Count > 0)
            {
                childInitialValues = childInitialValueArray[0];
            }

            UPMStandardGroup group = this.CreateNewGroup(childInitialValues);
            if (group != null)
            {
                repeatableEditGroup.AddChild(group);
            }

            if (this.MaxChildren > 0 && repeatableEditGroup.Groups.Count >= this.MaxChildren)
            {
                repeatableEditGroup.AddingEnabled = false;
            }
        }

        /// <summary>
        /// Handles removing an item from repeatable edit group
        /// </summary>
        /// <param name="group">Group to remove</param>
        /// <param name="repeatableEditGroup">Repeatable edit group to remove from</param>
        public override void UserWillDeleteGroupInRepeatableEditGroup(UPMGroup group, UPMRepeatableEditGroup repeatableEditGroup)
        {
            UPChildEditContext editContext = this.ChildContextForGroup(group);
            if (editContext != null)
            {
                repeatableEditGroup.RemoveChild(group);
                if (this.MaxChildren > 0)
                {
                    repeatableEditGroup.AddingEnabled = this.AddRecordEnabled;
                }

                editContext.DeleteRecord = true;
                if (this.AddedChildEditContexts.Contains(editContext))
                {
                    this.AddedChildEditContexts.Remove(editContext);
                }
            }
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.RequestOption == UPRequestOption.BestAvailable && error.IsConnectionOfflineError())
            {
                UPContainerMetaInfo childMetaInfo = new UPContainerMetaInfo(this.ChildFieldControl);
                childMetaInfo.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
                UPCRMResult childResult = childMetaInfo.Find();
                this.ControllerState = childResult.RowCount > 0 ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Error;
            }

            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount > 0)
            {
                this.GroupFromChildResult(result);
                this.ControllerState = GroupModelControllerState.Finished;
            }
            else
            {
                this.GroupFromChildResult(null);
                this.ControllerState = GroupModelControllerState.Empty;
            }

            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
