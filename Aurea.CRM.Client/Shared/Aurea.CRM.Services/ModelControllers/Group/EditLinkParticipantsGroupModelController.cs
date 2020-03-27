// <copyright file="EditLinkParticipantsGroupModelController.cs" company="Aurea Software Gmbh">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Implementation of UPEditLinkParticipantsGroupModelController class
    /// </summary>
    public class UPEditLinkParticipantsGroupModelController : UPEditParticipantsGroupModelController, UPCRMParticipantsDelegate, UPParticipantsGroupModelControllerDelegate, IRecordSelectorEditFieldDelegate, IRecordSelectorEditFieldCRMResultDelegate
    {
        private bool isNew;
        private bool isFirst = true;
        private Dictionary<string, object> firstItemOptions;
        private bool signalFinished;
        private List<UPRecordSelector> selectorArray;
        private bool fixedFirstParticipant;
        private bool noAutoParticipant;
        private bool keyField;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditLinkParticipantsGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">Field control</param>
        /// <param name="tabIndex">Tab index</param>
        /// <param name="editPageContext">Edit page context</param>
        /// <param name="Delegate">Delegate</param>
        public UPEditLinkParticipantsGroupModelController(FieldControl fieldControl, int tabIndex, UPEditPageContext editPageContext, IGroupModelControllerDelegate Delegate)
            : base(fieldControl, tabIndex, editPageContext, Delegate)
        {
            this.ProtectedParentLinkString = fieldControl.ValueForAttribute($"ProtectedParent_{tabIndex + 1}");
            if (string.IsNullOrEmpty(this.ProtectedParentLinkString))
            {
                this.ProtectedParentLinkString = fieldControl.ValueForAttribute("ProtectedParent");
            }

            if (this.ProtectedParentLinkString == "none")
            {
                this.ProtectedParentLinkString = string.Empty;
            }
            else if (string.IsNullOrEmpty(this.ProtectedParentLinkString))
            {
                this.ProtectedParentLinkString = ServerSession.CurrentSession.IsUpdateCrm ? "CP;PE;FI" : "KP;FI";
            }
        }

        /// <summary>
        /// Gets protected parent link string
        /// </summary>
        public string ProtectedParentLinkString { get; private set; }

        /// <summary>
        /// Gets record identification for new participants control
        /// </summary>
        public string RecordIdentificationForNewParticipantsControl => this.RootInfoAreaId;

        /// <summary>
        /// Gets child group key
        /// </summary>
        public override string ChildGroupKey => ChildGroupKeyForInfoAreaIdLinkId(this.ParticipantsControl.LinkParticipantsInfoAreaId, this.ParticipantsControl.LinkParticipantsLinkId);

        /// <summary>
        /// Gets target info area id
        /// </summary>
        public override string TargetInfoAreaId => this.ParticipantsControl?.LinkParticipantsInfoAreaId;

        /// <summary>
        /// Gets target link id
        /// </summary>
        public override int TargetLinkId => this.ParticipantsControl?.LinkParticipantsLinkId ?? 0;

        /// <summary>
        /// Returns changed links for given info area id
        /// </summary>
        /// <param name="infoAreaId">Info Area Id</param>
        /// <param name="userChangesOnly">Handles user changes only if set to true</param>
        /// <returns><see cref="Dictionary{string, UPCRMLink}"/></returns>
        public override Dictionary<string, UPCRMLink> ChangedLinksForInfoAreaId(string infoAreaId, bool userChangesOnly)
        {
            if (this.isNew && !userChangesOnly)
            {
                UPCRMLinkParticipant participant = this.ParticipantsControl.FirstLinkParticipant();
                if (!string.IsNullOrEmpty(participant?.LinkRecordIdentification))
                {
                    string val = participant.Options.ValueOrDefault("parentlink") as string;
                    if (Convert.ToInt32(val) != 0)
                    {
                        UPCRMLinkInfo linkInfo = UPCRMDataStore.DefaultStore.LinkInfoForInfoAreaTargetInfoAreaLinkId(infoAreaId, participant.LinkRecordIdentification.InfoAreaId(), 0);
                        if (linkInfo != null)
                        {
                            UPCRMLink link = new UPCRMLink(participant.LinkRecordIdentification);
                            return new Dictionary<string, UPCRMLink> { { linkInfo.Key, link } };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Participants view reference
        /// </summary>
        /// <returns><see cref="ViewReference"/></returns>
        public ViewReference ParticipantsViewReference()
        {
            string contextMenuName = ConfigurationUnitStore.DefaultStore.ConfigValue("LinkParticipants.Configuration");
            if (!string.IsNullOrEmpty(contextMenuName))
            {
                return ConfigurationUnitStore.DefaultStore.MenuByName(contextMenuName).ViewReference;
            }

            return null;
        }

        /// <inheritdoc/>
        public List<IIdentifier> UserDidChangeField(UPMEditField field, object pageModelController)
        {
            List<IIdentifier> changedFields = null;
            if (field is UPMParticipantsRecordSelectorEditField)
            {
                var selectorEditfield = (UPMParticipantsRecordSelectorEditField)field;
                string recordIdentification = null;
                var crmLinkInfo = selectorEditfield.CurrentSelector.LinkInfo;
                if (crmLinkInfo != null && !string.IsNullOrEmpty(selectorEditfield.ResultRows.RootRecordIdentification))
                {
                    recordIdentification = selectorEditfield.ResultRows.RootRecordIdentification;
                }

                string key = this.KeyForEditGroup(selectorEditfield.Group);
                if (key != null)
                {
                    var participant = this.ParticipantsControl.ParticipantWithKey(key) as UPCRMLinkParticipant;
                    if (participant != null)
                    {
                        if (!string.IsNullOrEmpty(recordIdentification))
                        {
                            participant.MarkAsDeleted = false;
                            var tableCaption = selectorEditfield.ResultRows.RowResult as string;
                            participant.ChangeRepName(recordIdentification, tableCaption);
                            selectorEditfield.FieldValue = participant.Name;
                            selectorEditfield.Participant = participant;
                            changedFields = new List<IIdentifier> { selectorEditfield.Identifier };
                        }
                        else
                        {
                            participant.MarkAsDeleted = true;
                        }
                    }
                }
            }
            else if (field is UPMParticipantCatalogEditField)
            {
                var selectorEditfield = (UPMParticipantCatalogEditField)field;
                var key = this.KeyForEditGroup(selectorEditfield.Group);
                if (key != null)
                {
                    var participant = this.ParticipantsControl.ParticipantWithKey(key) as UPCRMLinkParticipant;
                    if (participant != null)
                    {
                        changedFields = new List<IIdentifier> { field.Identifier };
                    }
                }
            }

            return changedFields;
        }

        /// <inheritdoc/>
        public object DataFromResultRow(UPCRMResultRow resultRow)
        {
            return UPCRMLinkParticipant.TableCaptionForInfoAreaIdResultRow(resultRow.RootRecordIdentification.InfoAreaId(), resultRow);
        }

        /// <inheritdoc/>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            return this.GroupFromRow(row);
        }

        /// <inheritdoc/>
        public override UPMStandardGroup CreateNewGroup(Dictionary<string, string> initialValues)
        {
            var participant = this.ParticipantsControl.AddNewLinkParticipant();
            participant.Options = this.firstItemOptions;
            this.firstItemOptions = null;
            var requirementKey = initialValues.ValueOrDefault("Requirement");
            var acceptanceKey = initialValues.ValueOrDefault("Acceptance");
            if (!string.IsNullOrEmpty(requirementKey))
            {
                var v = initialValues[requirementKey];
                if (!string.IsNullOrEmpty(v))
                {
                    participant.RequirementText = v;
                }
            }

            if (!string.IsNullOrEmpty(acceptanceKey))
            {
                var v = initialValues[acceptanceKey];
                if (!string.IsNullOrEmpty(v))
                {
                    participant.AcceptanceText = v;
                }
            }

            IIdentifier participantIdentifier = StringIdentifier.IdentifierWithStringId(participant.Key);
            var group = new UPMStandardGroup(participantIdentifier);

            group.Deletable = true;

            if (this.isFirst)
            {
                this.isFirst = false;
                if (this.fixedFirstParticipant)
                {
                    group.Deletable = false;
                }
            }

            var editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, group);
            editField.MainField.FieldValue = participant.Name;
            editField.DependField2.FieldValue = participant.RequirementText;
            editField.DependField.FieldValue = participant.CanChangeAcceptanceState ? participant.AcceptanceText : string.Empty;
            group.AddField(editField);
            this.AddGroupForKey(group, participant.Key);
            return group;
        }

        /// <inheritdoc/>
        public string ContextRecordForEditField(UPMEditField field)
        {
            var editField = field as UPMParticipantsRecordSelectorEditField;
            if (editField.CurrentSelector.DisableLinkOption && editField.CurrentSelector.LinkIsDisabled)
            {
                return null;
            }

            if (editField.CurrentSelector?.RecordLinkInfoAreaIds?.Count > 0)
            {
                UPRecordSelector selector = editField.CurrentSelector;
                for (int i = 0; i < selector.RecordLinkInfoAreaIds.Count; i++)
                {
                    string linkInfoAreaId = selector.RecordLinkInfoAreaIds[i];
                    int linkId = selector.RecordLinkLinkIds[i];
                    string rid = UPSelector.StaticSelectorContextDelegate.SenderLinkForInfoAreaIdLinkId(this, linkInfoAreaId, linkId);
                    if (rid.Length > 8)
                    {
                        return rid;
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public string CurrentRecordForEditField(UPMEditField field)
        {
            return null;
        }

        /// <inheritdoc/>
        public void CrmParticipantsDidFinishWithResult(UPCRMParticipants recordParticipants, object result)
        {
            this.ControllerState = GroupModelControllerState.Finished;
            this.Group = this.GroupFromParticipantControl();
            if (this.signalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <inheritdoc/>
        public void CrmParticipantsDidFailWithError(UPCRMParticipants recordParticipants, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Error = error;
            this.Group = null;

            if (this.signalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <inheritdoc/>
        public override List<UPCRMRecord> ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
        {
            List<UPCRMRecord> childRecords = null;
            string skipRecordIdentification = null;
            var skipFirst = false;
            if (!this.noAutoParticipant)
            {
                if (this.fixedFirstParticipant || (this.isNew && parentRecord.IsNew && (parentRecord.Links == null || parentRecord.Links.Count == 0)))
                {
                    skipFirst = true;
                }
                else if (this.isNew && parentRecord.IsNew && parentRecord.Links?.Count > 0)
                {
                    skipRecordIdentification = parentRecord.Links.First().RecordIdentification;
                }
            }

            foreach (var participant in this.ParticipantsControl.LinkParticipants)
            {
                var record = this.CreateRecord(parentRecord, participant, userChangesOnly);

                if (record == null)
                {
                    continue;
                }

                skipFirst = this.UpdateRecord(record, skipFirst, skipRecordIdentification);

                if (childRecords != null)
                {
                    childRecords.Add(record);
                }
                else
                {
                    childRecords = new List<UPCRMRecord> { record };
                }
            }

            return childRecords;
        }

        private UPCRMRecord CreateRecord(UPCRMRecord parentRecord, UPCRMLinkParticipant participant, bool userChangesOnly)
        {
            UPCRMRecord record = null;

            var hasAcceptance = this.ParticipantsControl.RequirementField != null;
            var hasRequirement = this.ParticipantsControl.AcceptanceField != null;
            var participantIdentification = participant.RecordIdentification?.Length > 6 ? participant.RecordIdentification : participant.Key;

            var editGroup = this.EditGroupForKey(participantIdentification);
            var dependsEditField = (UPMDependsEditField)editGroup.Fields.FirstOrDefault();
            if (dependsEditField == null)
            {
                throw new InvalidOperationException("Collection must contain UPMDependsEditField");
            }

            var acceptanceField = dependsEditField.DependField;
            var requirementField = dependsEditField.DependField2;

            if (participant.MarkAsDeleted)
            {
                if (!string.IsNullOrWhiteSpace(participant.RecordIdentification))
                {
                    record = new UPCRMRecord(participant.RecordIdentification, "Delete", null);
                }
            }
            else if ((!userChangesOnly || !participant.NoUserChanges)
                     && (participant.RecordIdentification == null || participant.RecordIdentification.Length < 5)
                     && !string.IsNullOrWhiteSpace(participant.LinkRecordIdentification))
            {
                record = this.CreateNewRecord(parentRecord, participant, hasRequirement, requirementField, hasAcceptance, acceptanceField);
            }
            else if (requirementField != null
                     && acceptanceField != null
                     && (requirementField.Changed || acceptanceField.Changed))
            {
                record = this.CreateNewRecord(participant, hasRequirement, requirementField, hasAcceptance, acceptanceField);
            }

            return record;
        }

        private UPCRMRecord CreateNewRecord(
            UPCRMRecord parentRecord,
            UPCRMLinkParticipant participant,
            bool hasRequirement,
            UPMEditField requirementField,
            bool hasAcceptance,
            UPMEditField acceptanceField)
        {
            var record = UPCRMRecord.CreateNew(this.ParticipantsControl.LinkParticipantsInfoAreaId);
            if (parentRecord.IsNew && (parentRecord.Links == null || parentRecord.Links.Count == 0))
            {
                parentRecord.AddLink(new UPCRMLink(participant.LinkRecordIdentification));
            }

            this.ApplyTemplateFilterToRecord(record);
            record.AddLink(new UPCRMLink(parentRecord, -1));
            record.AddLink(new UPCRMLink(participant.LinkRecordIdentification));
            if (hasRequirement)
            {
                record.NewValueFieldId(requirementField.StringValue, participant.Context.RequirementField.FieldId);
            }

            if (hasAcceptance)
            {
                record.NewValueFieldId(acceptanceField.StringValue, participant.Context.AcceptanceField.FieldId);
            }

            return record;
        }

        private UPCRMRecord CreateNewRecord(
            UPCRMLinkParticipant participant,
            bool hasRequirement,
            UPMEditField requirementField,
            bool hasAcceptance,
            UPMEditField acceptanceField)
        {
            var record = UPCRMRecord.CreateNew(participant.RecordIdentification);
            this.ApplyTemplateFilterToRecord(record);
            if (hasRequirement && requirementField.StringValue != participant.RequirementText)
            {
                record.NewValueFromValueFieldId(requirementField.StringValue, participant.RequirementText, participant.Context.RequirementField.FieldId);
            }

            if (hasAcceptance && acceptanceField.StringValue != participant.AcceptanceText)
            {
                record.NewValueFromValueFieldId(acceptanceField.StringValue, participant.AcceptanceText, participant.Context.AcceptanceField.FieldId);
            }

            return record;
        }

        private bool UpdateRecord(UPCRMRecord record, bool skipFirst, string skipRecordIdentification)
        {
            if (skipFirst)
            {
                skipFirst = false;
                if (this.keyField)
                {
                    record.EnableKeyFields();
                }
                else
                {
                    record.Mode = "NewOffline";
                }
            }
            else if (!string.IsNullOrWhiteSpace(skipRecordIdentification))
            {
                foreach (var link in record.Links)
                {
                    if (skipRecordIdentification == link.RecordIdentification)
                    {
                        if (this.keyField)
                        {
                            record.EnableKeyFields();
                        }
                        else
                        {
                            record.Mode = "NewOffline";
                        }

                        break;
                    }
                }
            }

            return skipFirst;
        }

        private UPMGroup GroupFromParticipantControl()
        {
            UPMRepeatableEditGroup repeatableEditGroup = new UPMRepeatableEditGroup(this.TabIdentifierForRecordIdentification(this.LinkRecordIdentification));
            repeatableEditGroup.LabelText = this.TabLabel;
            this.selectorArray = new List<UPRecordSelector>();

            for (int fieldNumber = 0; fieldNumber < this.TabConfig.NumberOfFields; fieldNumber++)
            {
                UPConfigFieldControlField configFieldControlField = this.TabConfig.Fields[fieldNumber];
                var selector = configFieldControlField.Attributes.Selector;

                if (selector != null)
                {
                    var recordSelector = new UPRecordSelector("KP", null, -1, selector, null, this.ParticipantsField);
                    this.selectorArray.Add(recordSelector);
                }
            }

            if (this.selectorArray.Count < 1)
            {
                repeatableEditGroup.AddingEnabled = false;
                if (this.ParticipantsControl.Participants.Count < 1)
                {
                    this.Group = null;
                    this.ControllerState = GroupModelControllerState.Empty;
                    return null;
                }
            }
            else
            {
                repeatableEditGroup.AddGroupLabelText = LocalizedString.TextAddNewGroup;
                repeatableEditGroup.AddingEnabled = this.AddRecordEnabled;
            }

            foreach (UPCRMLinkParticipant participant in this.ParticipantsControl.Participants)
            {
                string recordIdentification = participant.RecordIdentification;
                IIdentifier participantIdentifier = new RecordIdentifier(recordIdentification);
                UPMStandardGroup group = new UPMStandardGroup(participantIdentifier);
                string num = participant.Options.ValueOrDefault("must") as string;
                group.Deletable = !participant.MayNotBeDeleted && this.DeleteRecordEnabled && num.ToInt() == 0;
                UPMDependsEditField editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, group);
                editField.MainField.FieldValue = participant.Name;
                ((UPMParticipantsRecordSelectorEditField)editField.MainField).Participant = participant;
                editField.DependField.FieldValue = participant.AcceptanceText;
                editField.DependField2.FieldValue = participant.RequirementText;
                bool editOfflineRecord = ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("RecordSelect.EditOfflineRecord", false);
                editField.Deletable = !participant.IsOfflineEmptyParticipant || editOfflineRecord;
                group.AddField(editField);
                repeatableEditGroup.AddChild(group);
                this.AddGroupForKey(group, participant.Key);
            }

            this.Group = repeatableEditGroup;
            this.ControllerState = GroupModelControllerState.Finished;
            return repeatableEditGroup;
        }

        private string ParentLinkFromPage(string parentLinkString)
        {
            if (parentLinkString == "KPFI")
            {
                parentLinkString = "KP;FI";
            }

            var parts = parentLinkString.Split(';');
            foreach (string part in parts)
            {
                var part2 = part.Split('#');
                string _infoAreaId;
                int _linkId;

                if (part2.Length > 1)
                {
                    _infoAreaId = part2[0];
                    _linkId = Convert.ToInt32(part2[1]);
                }
                else
                {
                    _infoAreaId = part;
                    _linkId = -1;
                }

                string recordIdentification = UPSelector.StaticSelectorContextDelegate?.SenderLinkForInfoAreaIdLinkId(this, _infoAreaId, _linkId);
                if (recordIdentification?.Length > 9)
                {
                    return recordIdentification;
                }
            }

            return null;
        }

        private UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            if (resultRow.IsNewRow)
            {
                this.UpdateGroup(resultRow);
                return this.Group;
            }

            this.LinkRecordIdentification = resultRow.RootRecordIdentification;
            var participantsControl = this.CreateUPCRMRecordParticipants();
            this.ParticipantsControl = participantsControl;

            this.ControllerState = GroupModelControllerState.Pending;
            participantsControl.LinkParticipantsRequestOption = this.RequestOption;
            participantsControl.Load();
            this.signalFinished = true;

            return this.Group;
        }

        private void UpdateGroup(UPCRMResultRow resultRow)
        {
            this.isNew = true;
            var parentInitialValues = this.EditPageContext.InitialValues;
            var participantsControl = new UPCRMRecordParticipants(
                this.ParticipantsViewReference(),
                this.RootInfoAreaId,
                this.ParticipantInfoAreaId,
                this.ParticipantLinkId,
                this.RecordIdentificationForNewParticipantsControl,
                this);
            this.ParticipantsControl = participantsControl;
            var childGroupKey = this.ChildGroupKey;

            var parentInitialValueDictionaries = parentInitialValues.ValueOrDefault(childGroupKey) as List<Dictionary<string, object>>;

            List<UPCRMRecord> offlineRecords = null;

            if (this.EditPageContext.OfflineRequest != null)
            {
                offlineRecords = this.EditPageContext.OfflineRequest.RecordsWithInfoAreaLinkId(this.ParticipantsControl.LinkParticipantsInfoAreaId, this.ParticipantsControl.LinkParticipantsLinkId);
            }

            if (this.AddRecordEnabled || parentInitialValueDictionaries?.Count > 0 || offlineRecords?.Count > 0)
            {
                this.ParticipantsControl.SetFieldsFromSearchAndListConfigurationName(this.LinkParticipantsName);
                if (parentInitialValueDictionaries?.Count > 0)
                {
                    this.ProcessParentInitialValueDictionaries(parentInitialValueDictionaries);
                }

                if (offlineRecords?.Count > 0)
                {
                    this.ProcessOfflineRecords(offlineRecords, resultRow);
                }

                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = this.GroupFromParticipantControl();
            }
            else
            {
                this.Group = null;
                this.ControllerState = GroupModelControllerState.Empty;
            }

            this.signalFinished = true;
        }

        private void ProcessParentInitialValueDictionaries(IEnumerable<Dictionary<string, object>> parentInitialValueDictionaries)
        {
            foreach (var parentInitialValueForChild in parentInitialValueDictionaries)
            {
                Dictionary<string, object> options = null;
                string value = null;
                string requirement = null;
                string acceptance = null;
                var requirementKey = (parentInitialValueForChild.ValueOrDefault("Requirement") as List<string>)?[0];
                var acceptanceKey = (parentInitialValueForChild.ValueOrDefault("Acceptance") as List<string>)?[0];

                foreach (var key in parentInitialValueForChild.Keys)
                {
                    switch (key)
                    {
                        case ".Options":
                            var optionStringArray = parentInitialValueForChild.ValueOrDefault(key) as List<string>;
                            options = OptionsFromStringArray(optionStringArray);
                            break;
                        case "Requirement":
                        case "Acceptance":
                            continue;
                        default:
                            if (requirementKey != null && key == requirementKey)
                            {
                                requirement = parentInitialValueForChild.ValueOrDefault(key) as string;
                            }
                            else if (acceptanceKey != null && key == acceptanceKey)
                            {
                                acceptance = parentInitialValueForChild.ValueOrDefault(key) as string;
                            }
                            else if (value.Length < 8)
                            {
                                value = parentInitialValueForChild.ValueOrDefault(key) as string;
                            }

                            break;
                    }
                }

                if (value != null && value.Length >= 8)
                {
                    var participant = this.ParticipantsControl.AddLinkParticipantWithRecordIdentification(value, options);
                    if (!string.IsNullOrEmpty(requirement))
                    {
                        participant.RequirementText = requirement;
                    }

                    if (!string.IsNullOrEmpty(acceptance))
                    {
                        participant.AcceptanceText = acceptance;
                    }

                    participant.NoUserChanges = true;
                    this.fixedFirstParticipant = true;
                }
                else
                {
                    this.firstItemOptions = options;
                }

                if (options != null)
                {
                    if (options["noAutoParticipant"] != null)
                    {
                        this.noAutoParticipant = true;
                    }

                    if (options["keyfield"] != null)
                    {
                        this.keyField = true;
                    }
                }
            }
        }

        private void ProcessOfflineRecords(IEnumerable<UPCRMRecord> offlineRecords, UPCRMResultRow resultRow)
        {
            foreach (var record in offlineRecords)
            {
                if (record.Mode == "NewOffline" || record.Mode == "Sync")
                {
                    continue;
                }

                string participantsRecordIdentification = null;
                foreach (var link in record.Links)
                {
                    if (link.RecordIdentification.InfoAreaId() == resultRow.RootRecordIdentification.InfoAreaId())
                    {
                        continue;
                    }

                    participantsRecordIdentification = link.RecordIdentification;
                }

                if (!string.IsNullOrEmpty(participantsRecordIdentification))
                {
                    var participant = this.ParticipantsControl.AddLinkParticipantWithRecordIdentification(participantsRecordIdentification, null);
                    if (this.firstItemOptions != null)
                    {
                        participant.Options = this.firstItemOptions;
                        this.firstItemOptions = null;
                    }
                }
            }
        }

        private UPCRMRecordParticipants CreateUPCRMRecordParticipants()
        {
            var participantsControl = new UPCRMRecordParticipants(
                this.ParticipantsViewReference(),
                this.RootInfoAreaId,
                !string.IsNullOrEmpty(this.LinkParticipantsName) ? this.LinkParticipantsName : this.ParticipantInfoAreaId,
                this.ParticipantLinkId,
                this.LinkRecordIdentification,
                this);

            if (!string.IsNullOrEmpty(this.ProtectedParentLinkString))
            {
                var protectedLinkRecordIdentification = this.ParentLinkFromPage(this.ProtectedParentLinkString);
                if (!string.IsNullOrEmpty(protectedLinkRecordIdentification))
                {
                    participantsControl.ProtectedLinkRecordIdentification = protectedLinkRecordIdentification;
                }
            }

            return participantsControl;
        }

        private UPMDependsEditField CreateEditFieldWithParticipantIdentifierEditGroup(IIdentifier participantIdentifier, UPMGroup editGroup)
        {
            var dependsEditField = new UPMDependsEditField(participantIdentifier);
            dependsEditField.MainField = this.CreateMainEditFieldWithParticipantIdentifier(participantIdentifier, editGroup);
            dependsEditField.DependField = this.CreateDependEditFieldWithParticipantIdentifier(participantIdentifier, this.ParticipantsControl.AcceptanceCatalog, editGroup);
            dependsEditField.DependField2 = this.CreateDependEditFieldWithParticipantIdentifier(participantIdentifier, this.ParticipantsControl.RequirementCatalog, editGroup);
            dependsEditField.InitialSelectableOnly = true;
            return dependsEditField;
        }

        private UPMEditField CreateMainEditFieldWithParticipantIdentifier(IIdentifier participantIdentifier, UPMGroup editGroup)
        {
            var field = new UPMParticipantsRecordSelectorEditField(participantIdentifier)
            {
                Group = editGroup,
                GroupModelController = this,
                Delegate = this,
                CrmResultDelegate = this,
                HelpIdentifier = StringIdentifier.IdentifierWithStringId("LinkRecordParticipant"),
                SelectorArray = this.selectorArray,
                ContinuousUpdate = true
            };
            return field;
        }

        private UPMEditField CreateDependEditFieldWithParticipantIdentifier(IIdentifier participantIdentifier, UPCatalog catalog, UPMGroup editGroup)
        {
            UPCRMField field = null;
            if (catalog == this.ParticipantsControl.RequirementCatalog)
            {
                field = this.ParticipantsControl.RequirementField;
            }
            else if (catalog == this.ParticipantsControl.AcceptanceCatalog)
            {
                field = this.ParticipantsControl.AcceptanceField;
            }

            var editField = new UPMParticipantCatalogEditField(StringIdentifier.IdentifierWithStringId($"dependField_{participantIdentifier}_{field?.InfoAreaId}_{field?.FieldId}"));
            editField.GroupModelController = this;
            editField.Group = editGroup;

            if (catalog != null)
            {
                var possibleValues = catalog.Values;
                List<string> explicitKeyOrder;
                var possibleValuesAsString = catalog.TextValuesForFieldValues(false);
                var configStore = ConfigurationUnitStore.DefaultStore;
                var acceptanceCatalogAttributes = configStore.CatalogAttributesForInfoAreaIdFieldId(field.InfoAreaId, field.FieldId);

                foreach (var catalogValue in possibleValues)
                {
                    var possibleValue = new UPMCatalogPossibleValue();
                    var valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"));
                    valueField.StringValue = catalogValue.Text;
                    possibleValue.TitleLabelField = valueField;
                    possibleValue.Key = catalogValue.CodeKey;
                    var configCatalogValueAttributes = acceptanceCatalogAttributes.ValuesByCode.ValueOrDefault(catalogValue.CodeKey.ToInt());
                    if (configCatalogValueAttributes != null)
                    {
                        var colorString = configCatalogValueAttributes.ColorKey;
                        if (!string.IsNullOrEmpty(colorString))
                        {
                            possibleValue.IndicatorColor = AureaColor.ColorWithString(colorString);
                        }

                        possibleValue.ImageString = configCatalogValueAttributes.ImageName;
                    }

                    editField.AddPossibleValue(possibleValue);
                }

                if (!possibleValuesAsString.ContainsKey("0") || string.IsNullOrEmpty(possibleValuesAsString["0"]))
                {
                    editField.NullValueKey = "0";
                }

                if (acceptanceCatalogAttributes != null && !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("FixedCatalog.SortByAttributeFilter"))
                {
                    explicitKeyOrder = this.ExplicitKeyOrderByCatalogAttributeCodeOrder(acceptanceCatalogAttributes, catalog.ExplicitKeyOrderByCodeEmptyValueIncludeHidden(false, false));
                }
                else if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("FixedCatalog.SortByCode"))
                {
                    explicitKeyOrder = catalog.ExplicitKeyOrderByCodeEmptyValueIncludeHidden(false, false);
                }
                else
                {
                    explicitKeyOrder = catalog.ExplicitKeyOrderEmptyValueIncludeHidden(false, false);
                }

                if (explicitKeyOrder != null)
                {
                    editField.ExplicitKeyOrder = explicitKeyOrder;
                }

                editField.ContinuousUpdate = true;
            }

            return editField;
        }

        private bool ApplyTemplateFilterToRecord(UPCRMRecord record)
        {
            string templateFilterName = this.FieldControlConfig.ValueForAttribute($"TemplateFilter_{this.TabIndex + 1}");
            if (!string.IsNullOrEmpty(templateFilterName))
            {
                var filter = ConfigurationUnitStore.DefaultStore.FilterByName(templateFilterName);
                if (filter != null)
                {
                    record.ApplyValuesFromTemplateFilter(filter);
                    return true;
                }
            }

            return false;
        }
    }
}
