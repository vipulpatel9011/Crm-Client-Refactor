// <copyright file="EditRepParticipantsGroupModelController.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Contexts.Reps;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Fields.Edit.Delegates;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Implementation of UPEditRepParticipantsGroupModelController
    /// </summary>
    public class UPEditRepParticipantsGroupModelController : UPEditParticipantsGroupModelController, UPParticipantsGroupModelControllerDelegate, UPCRMParticipantsDelegate
    {
        private UPParticipantsEditFieldContext editFieldContext;
        private Dictionary<string, object> firstItemOptions;
        private bool signalFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPEditRepParticipantsGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">Field control</param>
        /// <param name="tabIndex">Tab index</param>
        /// <param name="editPageContext">Edit page context</param>
        /// <param name="theDelegate">Delegate</param>
        public UPEditRepParticipantsGroupModelController(FieldControl fieldControl, int tabIndex, UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, editPageContext, theDelegate)
        {
        }

        /// <inheritdoc/>
        public override string ChildGroupKey => ChildGroupKeyForInfoAreaIdLinkId("ID", 0);

        /// <inheritdoc/>
        public override string TargetInfoAreaId => "ID";

        /// <inheritdoc/>
        public override int TargetLinkId => 0;

        /// <inheritdoc/>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            return this.GroupFromRow(row);
        }

        /// <inheritdoc/>
        public override UPMStandardGroup CreateNewGroup(Dictionary<string, string> initialValues)
        {
            var participant = this.ParticipantsControl.AddNewRepParticipant();
            participant.Options = this.firstItemOptions;
            this.firstItemOptions = null;

            IIdentifier participantIdentifier = StringIdentifier.IdentifierWithStringId(participant.Key);
            var group = new UPMStandardGroup(participantIdentifier);
            group.Deletable = true;

            var editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, group);
            editField.MainField.FieldValue = participant.RepIdString;
            editField.DependField.FieldValue = null;
            editField.DependField2.FieldValue = participant.RequirementText;
            group.AddField(editField);
            this.AddGroupForKey(group, participant.Key);

            return group;
        }

        /// <inheritdoc/>
        public List<IIdentifier> UserDidChangeField(UPMEditField field, object pageModelController)
        {
            if (field is UPMRepEditField)
            {
                var editField = (UPMRepEditField)field;
                var key = this.KeyForEditGroup(editField.Group);
                if (key != null)
                {
                    var participant = (UPCRMRepParticipant)this.ParticipantsControl.ParticipantWithKey(key);
                    if (participant != null)
                    {
                        participant.MarkAsDeleted = false;
                        participant.ChangeRep((string)field.FieldValue);
                    }
                }
            }
            else if (field is UPMParticipantCatalogEditField)
            {
                var editField = (UPMParticipantCatalogEditField)field;
                var key = this.KeyForEditGroup(editField.Group);

                if (key != null)
                {
                    var participant = (UPCRMRepParticipant)this.ParticipantsControl.ParticipantWithKey(key);
                    var participantIdentification = participant.Key;
                    var editGroup = this.EditGroupForKey(participantIdentification);
                    var dependsEditField = (UPMDependsEditField)editGroup.Fields[0];
                    var acceptanceField = dependsEditField.DependField;
                    var requirementField = dependsEditField.DependField2;

                    if (field == acceptanceField)
                    {
                        participant.AcceptanceText = (string)field.FieldValue;
                    }
                    else if (field == requirementField)
                    {
                        participant.RequirementText = (string)field.FieldValue;
                    }
                }
            }
            else if (field is UPMDependsEditField)
            {
                var editField = (UPMDependsEditField)field;
                var selectedRep = ((UPMRepEditField)editField.MainField).RepContainer.SelectedRepKeys;
                foreach (var key in selectedRep)
                {
                    var participant = (UPCRMRepParticipant)this.ParticipantsControl.ParticipantWithKey(key);
                    if (participant == null)
                    {
                        var newParticipant = this.ParticipantsControl.AddRepParticipantWithRepId(key, null);                        
                    }
                }

                var deletedRep = this.ParticipantsControl.RepParticipants.Where(p => !selectedRep.Contains(p.Key)).ToList();
                foreach (var rep in deletedRep)
                {
                    rep.MarkAsDeleted = true;
                    this.ParticipantsControl.RemoveRepParticipant(rep);
                }

                this.Group = this.GroupFromParticipantControl();
            }

            return null;
        }

        public void UserDidChangeRequirementField(string repId, string requirmentValue)
        {
            this.ParticipantsControl?.ChangedRepParticipantRequirementRecords(repId, requirmentValue);
        }

        /// <inheritdoc/>
        public override List<UPCRMRecord> ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
        {
            var childRecords = this.ParticipantsControl.ChangedRepParticipantAcceptanceRecords();

            if (!userChangesOnly && !string.IsNullOrEmpty(this.ParticipantsControl.RepAcceptanceInfoAreaId) && !parentRecord.IsNew)
            {
                var sync = new UPCRMRecord(this.ParticipantsControl.RepAcceptanceInfoAreaId, "Sync", null);
                sync.AddLink(new UPCRMLink(parentRecord, this.ParticipantsControl.RepAcceptanceLinkId));

                if (childRecords == null)
                {
                    childRecords = new List<UPCRMRecord>();
                }

                childRecords.Add(sync);
            }

            return childRecords;
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

        private UPMGroup GroupFromParticipantControl()
        {
            var repeatableEditGroup = new UPMRepeatableEditGroup(this.TabIdentifierForRecordIdentification(this.LinkRecordIdentification))
            {
                LabelText = this.TabLabel,
                AddGroupLabelText = LocalizedString.TextAddNewGroup,
                AddingEnabled = this.AddRecordEnabled
            };

            if (this.ParticipantsControl.Participants.Count > 0)
            {
                UPCRMRepParticipant sparticipant = (UPCRMRepParticipant)this.ParticipantsControl.Participants[0];
                IIdentifier sparticipantIdentifier = StringIdentifier.IdentifierWithStringId(sparticipant.Key);
                var group = new UPMStandardGroup(sparticipantIdentifier);


                foreach (UPCRMRepParticipant participant in this.ParticipantsControl.Participants)
                {
                    var repIdString = participant.Key;
                    IIdentifier participantIdentifier = StringIdentifier.IdentifierWithStringId(repIdString);
                    //var group = new UPMStandardGroup(participantIdentifier);

                    var num = participant.Options.ValueOrDefault("must") as string;
                    group.Deletable = this.DeleteRecordEnabled && Convert.ToInt32(num) == 0;

                    var editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, group);
                    editField.MainField.FieldValue = participant.RepIdString;

                    if (this.ParticipantsControl.HasRepAcceptance && participant.CanChangeAcceptanceState)
                    {
                        editField.DependField.FieldValue = participant.AcceptanceText;
                    }
                    else
                    {
                        editField.DependField.FieldValue = null;
                    }

                    editField.DependField2.FieldValue = participant.RequirementText;
                    group.AddField(editField);

                }

                repeatableEditGroup.AddChild(group);
                this.AddGroupForKey(group, sparticipant.Key);
            }
            else
            {
                IIdentifier sparticipantIdentifier = StringIdentifier.IdentifierWithStringId("00000");
                var group = new UPMStandardGroup(sparticipantIdentifier);
                var editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(sparticipantIdentifier, group);
                group.AddField(editField);
                repeatableEditGroup.AddChild(group);
                this.AddGroupForKey(group, "00000");
            }

            this.Group = repeatableEditGroup;
            this.ControllerState = GroupModelControllerState.Finished;
            return repeatableEditGroup;
        }

        private UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            if (resultRow.IsNewRow)
            {
                this.ParticipantsControl = new UPCRMMutableParticipants(
                    null,
                    resultRow.RootRecordIdentification.InfoAreaId(),
                    resultRow.RootRecordIdentification,
                    null,
                    -1,
                    this);

                var parentInitialValues = this.EditPageContext.InitialValues;
                var childGroupKey = this.ChildGroupKey;
                var parentInitialValueDictionaries = parentInitialValues.ValueOrDefault(childGroupKey) as List<Dictionary<string, object>>;

                if (parentInitialValueDictionaries?.Count > 0)
                {
                    foreach (var parentInitialValueForChild in parentInitialValueDictionaries)
                    {
                        string value = null;
                        Dictionary<string, object> options = null;
                        foreach (var key in parentInitialValueForChild.Keys)
                        {
                            if (key == ".Options")
                            {
                                var optionStringArray = parentInitialValueForChild.ValueOrDefault(key) as List<string>;
                                options = OptionsFromStringArray(optionStringArray);
                            }
                            else if (value == null)
                            {
                                value = parentInitialValueForChild.ValueOrDefault(key) as string;
                            }
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            this.ParticipantsControl.AddRepParticipantWithRepId(value, options);
                        }
                        else
                        {
                            this.firstItemOptions = options;
                        }
                    }
                }

                if (this.ParticipantsField != null)
                {
                    this.editFieldContext = new UPParticipantsEditFieldContext(this.ParticipantsField, null, string.Empty, null);
                }

            }
            else if (this.ParticipantsField != null)
            {
                this.LinkRecordIdentification = resultRow.RootRecordIdentification;
                var participantsString = resultRow.ValueAtIndex(this.ParticipantsField.TabIndependentFieldIndex);
                this.ParticipantsControl = new UPCRMMutableParticipants(null, null, resultRow.RootRecordIdentification, null, -1, this);
                this.editFieldContext = new UPParticipantsEditFieldContext(this.ParticipantsField, null, participantsString, null);
                this.ParticipantsControl.AddParticipantsFromString(participantsString);                
            }
            else
            {
                this.Group = null;
                this.ControllerState = GroupModelControllerState.Empty;
                return this.Group;
            }

            if (this.editFieldContext != null)
            {
                this.editFieldContext.GroupModelController = this;
                this.EditPageContext.EditFields.SetObjectForKey(this.editFieldContext, this.editFieldContext.Key);
            }

            if (!string.IsNullOrEmpty(this.LinkParticipantsName))
            {
                this.ParticipantsControl?.SetFieldsFromSearchAndListConfigurationName(this.LinkParticipantsName);
            }

            if (this.AddRecordEnabled || this.ParticipantsControl?.Participants.Count > 0)
            {
                this.ControllerState = GroupModelControllerState.Pending;
                this.ParticipantsControl?.Load();
            }
            else
            {
                this.Group = null;
                this.ControllerState = GroupModelControllerState.Empty;
            }

            this.signalFinished = true;
            return this.Group;
        }

        private UPMDependsEditField CreateEditFieldWithParticipantIdentifierEditGroup(IIdentifier participantIdentifier, UPMGroup editGroup)
        {
            var dependsEditField = new UPMDependsEditField(StringIdentifier.IdentifierWithStringId($"depenfEditField_{participantIdentifier}"))
            {
                MainField = this.CreateMainEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, editGroup),
                DependField = this.CreateDependEditField(participantIdentifier, this.ParticipantsControl.AcceptanceCatalog, editGroup),
                DependField2 = this.CreateDependEditField(participantIdentifier, this.ParticipantsControl.RequirementCatalog, editGroup),
                InitialSelectableOnly = true
            };

            return dependsEditField;
        }

        private UPMEditField CreateMainEditFieldWithParticipantIdentifierEditGroup(IIdentifier participantIdentifier, UPMGroup editGroup)
        {
            var field = new UPMRepEditField(participantIdentifier)
            {
                Group = editGroup,
                GroupModelController = this,
                MultiSelectMode = false,
                HelpIdentifier = StringIdentifier.IdentifierWithStringId("RepPartitipant")
            };

            var repType = UPCRMReps.RepTypeFromString(this.ParticipantsField.Field.RepType);
            var possibleValues = UPCRMDataStore.DefaultStore.Reps.AllRepsOfTypes(repType);
            var explicitKeyOrder = UPCRMDataStore.DefaultStore.Reps.AllRepIdsOfTypes(repType);
            var repContainer = UPRepsService.CreateRepContainerForRepType(repType);
            field.RepContainer = repContainer;

            // Adding all rep values from UPCRMDataStore to the PossibleValues list.
            foreach (var obj in possibleValues)
            {
                var possibleValue = new UPMCatalogPossibleValue();
                var valueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"));
                var rep = obj;
                valueField.StringValue = rep.RepName;
                possibleValue.TitleLabelField = valueField;
                possibleValue.Key = rep.RepId;
                field.AddPossibleValue(possibleValue);
            }

            field.NullValueKey = "0";
            field.ExplicitKeyOrder = explicitKeyOrder;

            return field;
        }

        private UPMEditField CreateDependEditField(IIdentifier participantIdentifier, UPCatalog catalog, UPMGroup editGroup)
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

            var editField = new UPMParticipantCatalogEditField(StringIdentifier.IdentifierWithStringId(
                field != null ? $"dependField_{participantIdentifier}_{field.InfoAreaId}_{field.FieldId}" : $"dependField_{participantIdentifier}_(null)_0"))
            {
                GroupModelController = this,
                Group = editGroup
            };

            if (catalog != null)
            {
                var possibleValues = catalog.Values;
                List<string> explicitKeyOrder;
                var possibleValuesAsString = catalog.TextValuesForFieldValues(false);
                var configStore = ConfigurationUnitStore.DefaultStore;
                var acceptanceCatalogAttributes = configStore.CatalogAttributesForInfoAreaIdFieldId(field.InfoAreaId, field.FieldId);
                foreach (var catalogValue in possibleValues)
                {
                    var possibleValue = new UPMCatalogPossibleValue
                    {
                        TitleLabelField = new UPMStringField(StringIdentifier.IdentifierWithStringId("x"))
                        {
                            StringValue = catalogValue.Text
                        }
                    };

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

                    possibleValue.Key = catalogValue.CodeKey;

                    editField.AddPossibleValue(possibleValue);
                }

                if (string.IsNullOrEmpty(possibleValuesAsString.ValueOrDefault("0")))
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
                editField.DisplayValues = possibleValuesAsString.Values.ToList();
            }
            
            return editField;
        }
    }
}
