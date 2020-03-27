// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditRepParticipantsGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2016 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   EditRepParticipantsGroupModelController.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.ModelControllers.Edit
{
#if PORTING
    public class UPEditRepParticipantsGroupModelController : UPEditParticipantsGroupModelController, UPParticipantsGroupModelControllerDelegate, UPCRMParticipantsDelegate
        {
            protected UPParticipantsEditFieldContext editFieldContext;
            protected NSDictionary firstItemOptions;
            protected bool signalFinished;
            UPMGroup GroupFromParticipantControl()
            {
                UPMRepeatableEditGroup repeatableEditGroup = new UPMRepeatableEditGroup(this.TabIdentifierForRecordIdentification(this.LinkRecordIdentification));
                repeatableEditGroup.LabelText = this.TabLabel();
                repeatableEditGroup.AddGroupLabelText = upText_AddNewGroup;
                repeatableEditGroup.AddingEnabled = this.AddRecordEnabled;
                foreach (UPCRMRepParticipant participant in this.ParticipantsControl.Participants)
                {
                    string repIdString = participant.Key;
                    UPMIdentifier participantIdentifier = UPMStringIdentifier.IdentifierWithStringId(repIdString);
                    UPMStandardGroup group = new UPMStandardGroup(participantIdentifier);
                    NSNumber num = participant.Options.ObjectForKey("must");
                    group.Deletable = this.DeleteRecordEnabled && !num.IntegerValue;
                    UPMDependsEditField editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, group);
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
                    repeatableEditGroup.AddGroup(group);
                    this.AddGroupForKey(group, repIdString);
                }
                this.Group = repeatableEditGroup;
                this.ControllerState = UPGroupModelControllerStateFinished;
                return repeatableEditGroup;
            }

            UPMGroup GroupFromRow(UPCRMResultRow resultRow)
            {
                if (resultRow.IsNewRow())
                {
                    this.ParticipantsControl = new UPCRMMutableParticipants(null, resultRow.RootRecordIdentification.InfoAreaId(), null, -1, null, this);
                    NSDictionary parentInitialValues = this.EditPageContext.initialValues;
                    string childGroupKey = this.ChildGroupKey();
                    ArrayList parentInitialValueDictionaries = parentInitialValues.ObjectForKey(childGroupKey);
                    if (parentInitialValueDictionaries.Count > 0)
                    {
                        if (parentInitialValueDictionaries != null)
                        {
                            foreach (NSDictionary parentInitialValueForChild in parentInitialValueDictionaries)
                            {
                                string value = null;
                                NSDictionary options = null;
                                foreach (string key in parentInitialValueForChild)
                                {
                                    if (key.IsEqualToString(".Options"))
                                    {
                                        ArrayList optionStringArray = parentInitialValueForChild.ObjectForKey(key);
                                        options = UPEditChildrenGroupModelControllerBase.OptionsFromStringArray(optionStringArray);
                                    }
                                    else if (value == null)
                                    {
                                        value = parentInitialValueForChild.ObjectForKey(key);
                                    }

                                }
                                if (value.Length > 0)
                                {
                                    this.ParticipantsControl.AddRepParticipantWithRepIdWithOptions(value, options);
                                }
                                else
                                {
                                    firstItemOptions = options;
                                }

                            }
                        }

                    }

                    if (this.ParticipantsField != null)
                    {
                        editFieldContext = new UPParticipantsEditFieldContext(this.ParticipantsField, null, string.Empty, null);
                    }

                }
                else if (this.ParticipantsField != null)
                {
                    this.LinkRecordIdentification = resultRow.RootRecordIdentification();
                    string participantsString = resultRow.ValueAtIndex(this.ParticipantsField.TabIndependentFieldIndex);
                    this.ParticipantsControl = new UPCRMMutableParticipants(null, null, null, -1, resultRow.RootRecordIdentification(), this);
                    editFieldContext = new UPParticipantsEditFieldContext(this.ParticipantsField, null, participantsString, null);
                    this.ParticipantsControl.AddParticipantsFromString(participantsString);
                }
                else
                {
                    this.Group = null;
                    this.ControllerState = UPGroupModelControllerStateEmpty;
                    return this.Group;
                }

                if (editFieldContext != null)
                {
                    editFieldContext.GroupModelController = this;
                    this.EditPageContext.EditFields.SetObjectForKey(editFieldContext, editFieldContext.Key);
                }

                if (this.LinkParticipantsName.Length)
                {
                    this.ParticipantsControl.SetFieldsFromSearchAndListConfigurationName(this.LinkParticipantsName);
                }

                if (this.AddRecordEnabled || this.ParticipantsControl.Participants.Count > 0)
                {
                    this.ControllerState = UPGroupModelControllerStatePending;
                    this.ParticipantsControl.Load();
                }
                else
                {
                    this.Group = null;
                    this.ControllerState = UPGroupModelControllerStateEmpty;
                }

                signalFinished = true;
                return this.Group;
            }

            UPMGroup ApplyResultRow(UPCRMResultRow row)
            {
                return this.GroupFromRow(row);
            }

            UPMDependsEditField CreateEditFieldWithParticipantIdentifierEditGroup(UPMIdentifier participantIdentifier, UPMGroup editGroup)
            {
                UPMDependsEditField dependsEditField = new UPMDependsEditField(UPMStringIdentifier.IdentifierWithStringId(NSString.StringWithFormat("depenfEditField_%@", participantIdentifier.Description)));
                dependsEditField.MainField = this.CreateMainEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, editGroup);
                dependsEditField.DependField = this.CreateDependEditFieldWithParticipantIdentifierCatalogEditGroup(participantIdentifier, this.ParticipantsControl.AcceptanceCatalog, editGroup);
                dependsEditField.DependField2 = this.CreateDependEditFieldWithParticipantIdentifierCatalogEditGroup(participantIdentifier, this.ParticipantsControl.RequirementCatalog, editGroup);
                dependsEditField.initialSelectableOnly = true;
                return dependsEditField;
            }

            UPMEditField CreateMainEditFieldWithParticipantIdentifierEditGroup(UPMIdentifier participantIdentifier, UPMGroup editGroup)
            {
                UPMRepEditField editField = new UPMRepEditField(participantIdentifier);
                editField.Group = editGroup;
                editField.GroupModelController = this;
                editField.MultiSelectMode = true;
                editField.HelpIdentifier = UPMStringIdentifier.IdentifierWithStringId("RepPartitipant");
                UPCRMRepType repType = UPCRMReps.RepTypeFromString(this.ParticipantsField.Field.RepType);
                UPMRepContaner repContaner = UPRepsService.CreateRepContanerForRepType(repType);
                editField.RepContaner = repContaner;
                editField.ContinuousUpdate = true;
                return editField;
            }

            UPMEditField CreateDependEditFieldWithParticipantIdentifierCatalogEditGroup(UPMIdentifier participantIdentifier, UPCatalog catalog, UPMGroup editGroup)
            {
                UPCRMField field;
                if (catalog == this.ParticipantsControl.RequirementCatalog)
                {
                    field = this.ParticipantsControl.RequirementField;
                }
                else if (catalog == this.ParticipantsControl.AcceptanceCatalog)
                {
                    field = this.ParticipantsControl.AcceptanceField;
                }

                UPMParticipantCatalogEditField editField = new UPMParticipantCatalogEditField(UPMStringIdentifier.IdentifierWithStringId(NSString.StringWithFormat("dependField_%@_%@_%ld", participantIdentifier.Description, field.InfoAreaId, (long)field.FieldId)));
                editField.GroupModelController = this;
                editField.Group = editGroup;
                if (catalog != null)
                {
                    ArrayList possibleValues = catalog.Values;
                    ArrayList explicitKeyOrder;
                    NSDictionary possibleValuesAsString = catalog ? catalog.TextValuesForFieldValues() : null;
                    UPConfigurationUnitStore configStore = UPConfigurationUnitStore.DefaultStore();
                    UPConfigCatalogAttributes acceptanceCatalogAttributes = configStore.CatalogAttributesForInfoAreaIdFieldId(field.InfoAreaId, field.FieldId);
                    possibleValues.EnumerateObjectsUsingBlock(delegate (object obj, uint idx, bool stop)
                    {
                        UPMCatalogPossibleValue possibleValue = new UPMCatalogPossibleValue();
                        UPMStringField valueField = new UPMStringField(UPMStringIdentifier.IdentifierWithStringId("x"));
                        UPCatalogValue catalogValue = (UPCatalogValue)obj;
                        valueField.StringValue = catalogValue.Text;
                        possibleValue.TitleLabelField = valueField;
                        UPConfigCatalogValueAttributes configCatalogValueAttributes = acceptanceCatalogAttributes.ValuesByCode().ObjectForKey(NSNumber.NumberWithInt(catalogValue.CodeKey.IntValue));
                        if (configCatalogValueAttributes)
                        {
                            string colorString = configCatalogValueAttributes.ColorKey();
                            if (colorString)
                            {
                                possibleValue.IndicatorColor = UPColor.ColorWithString(colorString);
                            }

                            possibleValue.ImageString = configCatalogValueAttributes.ImageName();
                        }

                        editField.AddPossibleValueForKey(possibleValue, catalogValue.CodeKey);
                    });
                    if (((string)possibleValuesAsString.ObjectForKey("0")).Length == 0)
                    {
                        editField.NullValueKey = "0";
                    }

                    if (catalog != null && acceptanceCatalogAttributes != null && !UPConfigurationUnitStore.DefaultStore().ConfigValueIsSet("FixedCatalog.SortByAttributeFilter"))
                    {
                        explicitKeyOrder = this.ExplicitKeyOrderByCatalogAttributeCodeOrder(acceptanceCatalogAttributes, catalog.ExplicitKeyOrderByCodeEmptyValueIncludeHidden(false, false));
                    }
                    else if (catalog != null && UPConfigurationUnitStore.DefaultStore().ConfigValueIsSet("FixedCatalog.SortByCode"))
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

            UPMStandardGroup CreateNewGroup(NSDictionary initialValues)
            {
                UPCRMRepParticipant participant = this.ParticipantsControl.AddNewRepParticipant();
                participant.Options = firstItemOptions;
                firstItemOptions = null;
                UPMIdentifier participantIdentifier = UPMStringIdentifier.IdentifierWithStringId(participant.Key);
                UPMStandardGroup group = new UPMStandardGroup(participantIdentifier);
                group.Deletable = true;
                UPMDependsEditField editField = this.CreateEditFieldWithParticipantIdentifierEditGroup(participantIdentifier, group);
                editField.MainField.FieldValue = participant.RepIdString;
                editField.DependField.FieldValue = string.Empty;
                editField.DependField2.FieldValue = participant.RequirementText;
                group.AddField(editField);
                this.AddGroupForKey(group, participant.Key);
                return group;
            }
        
            void AddGroupsWithInitialValues(ArrayList initialValues)
            {
                // Console.WriteLine("RepInitial: %@", initialValues);
            }

            string ChildGroupKey()
            {
                return UPEditChildrenGroupModelControllerBase.ChildGroupKeyForInfoAreaIdLinkId("ID", 0);
            }

            string TargetInfoAreaId()
            {
                return "ID";
            }

            int TargetLinkId()
            {
                return 0;
            }

            ArrayList ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
            {
                ArrayList childRecords = this.ParticipantsControl.ChangedRepParticipantAcceptanceRecords();
                if (!userChangesOnly && this.ParticipantsControl.RepAcceptanceInfoAreaId.Length && !parentRecord.IsNew)
                {
                    UPCRMRecord sync = new UPCRMRecord(this.ParticipantsControl.RepAcceptanceInfoAreaId, "Sync");
                    sync.AddLink(new UPCRMLink(parentRecord, this.ParticipantsControl.RepAcceptanceLinkId));
                    if (childRecords)
                    {
                        return childRecords.ArrayByAddingObject(sync);
                    }
                    else
                    {
                        return NSMutableArray.ArrayWithObject(sync);
                    }

                }
                else
                {
                    return childRecords;
                }

            }

            public void CrmParticipantsDidFinishWithResult(UPCRMRecordParticipants recordParticipants, object result)
            {
                this.ControllerState = GroupModelControllerState.Finished;
                this.Group = this.GroupFromParticipantControl();
                if (signalFinished)
                {
                    this.TheDelegate.GroupModelControllerFinished(this);
                }

            }

            public void CrmParticipantsDidFailWithError(UPCRMRecordParticipants recordParticipants, Exception error)
            {
                ControllerState = GroupModelControllerState.Error;
                Error = error;
                Group = null;
                if (signalFinished)
                {
                    TheDelegate.GroupModelControllerFinished(this);
                }

            }

        public List<IIdentifier> UserDidChangeField(UPMEditField field, object pageModelController)
        {
            if (field.GetType() == (typeof(UPMRepEditField)))
            {
                UPMRepEditField _field = (UPMRepEditField)field;
                string key = keyForEditGroup(field.Group);
                if (key != null)
                {
                    UPCRMRepParticipant participant = (UPCRMRepParticipant)this.ParticipantsControl.ParticipantWithKey(key);
                    if (participant != null)
                    {
                        participant.MarkAsDeleted = false;
                        participant.ChangeRep(field.FieldValue);
                    }

                }

            }
            else if (_field.IsKindOfClass(typeof(UPMParticipantCatalogEditField)))
            {
                UPMParticipantCatalogEditField field = (UPMParticipantCatalogEditField)_field;
                string key = this.KeyForEditGroup(field.Group);
                if (key != null)
                {
                    UPCRMRepParticipant participant = (UPCRMRepParticipant)this.ParticipantsControl.ParticipantWithKey(key);
                    string participantIdentification = participant.Key;
                    UPMGroup editGroup = this.EditGroupForKey(participantIdentification);
                    UPMDependsEditField dependsEditField = (UPMDependsEditField)editGroup.Fields.ObjectAtIndex(0);
                    UPMEditField acceptanceField = dependsEditField.DependField;
                    UPMEditField requirementField = dependsEditField.DependField2;
                    if (field == acceptanceField)
                    {
                        participant.AcceptanceText = field.FieldValue;
                    }
                    else if (field == requirementField)
                    {
                        participant.RequirementText = field.FieldValue;
                    }

                }

            }

            return null;
        }
    }
        public class UPParticipantsEditFieldContext : UPEditFieldContext
        {
            public UPEditRepParticipantsGroupModelController GroupModelController { get; set; }

            string Value()
            {
                return this.GroupModelController.ParticipantsControl.ParticipantString;
            }

            bool WasChanged()
            {
                return !this.OriginalValue.IsEqualToString(this.GroupModelController.ParticipantsControl.ParticipantString);
            }

        }
#endif
}
