// <copyright file="ParticipantsGroupModelController.cs" company="Aurea Software Gmbh">
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

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Participants Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPEditChildrenGroupModelControllerBase" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMParticipantsDelegate" />
    public class UPParticipantsGroupModelController : UPEditChildrenGroupModelControllerBase, UPCRMParticipantsDelegate
    {
        private UPCRMRecordParticipants recordParticipants;
        private string linkParticipantsName;
        private ViewReference linkParticipantsViewReference;
        private UPConfigCatalogAttributes acceptanceCatalogAttributes;
        private bool signalFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPParticipantsGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="editPageContext">The edit page context.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPParticipantsGroupModelController(FieldControl fieldControl, int tabIndex,
            UPEditPageContext editPageContext, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, editPageContext, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPParticipantsGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPParticipantsGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow resultRow)
        {
            this.ExplicitTabIdentifier = this.TabIdentifierForRecordIdentification(resultRow.RootRecordIdentification);
            int fieldCount = this.TabConfig.NumberOfFields;
            UPConfigFieldControlField participantsField = null;

            for (int i = 0; i < fieldCount; i++)
            {
                UPConfigFieldControlField fieldConfig = this.TabConfig.FieldAtIndex(i);
                if (fieldConfig.Field.FieldInfo.IsParticipantsField)
                {
                    participantsField = fieldConfig;
                    break;
                }
            }

            var tabTypeParts = this.TabConfig.Type.Split('_');
            this.linkParticipantsName = null;
            if (tabTypeParts.Length > 1)
            {
                this.linkParticipantsName = tabTypeParts[1];
                var configParts = this.linkParticipantsName.Split('#');
                if (configParts.Length > 1)
                {
                    this.linkParticipantsName = configParts[0];
                }
            }

            this.recordParticipants = !string.IsNullOrEmpty(this.linkParticipantsName)
                ? new UPCRMRecordParticipants(resultRow.RootRecordIdentification, this.linkParticipantsName, -1, this)
                : new UPCRMRecordParticipants(resultRow.RootRecordIdentification, this);

            if (this.linkParticipantsName != null && !this.recordParticipants.SetFieldsFromSearchAndListConfigurationName(this.linkParticipantsName))
            {
                this.linkParticipantsName = null;
            }

            if (participantsField != null)
            {
                this.recordParticipants.AddParticipantsFromString(resultRow.ValueAtIndex(participantsField.TabIndependentFieldIndex));
            }

            //2019-07-22 Rep Selection - Reverted if condition
            //if (!string.IsNullOrEmpty(this.linkParticipantsName))
            //{
            //    this.Group = null;
            //    this.ControllerState = GroupModelControllerState.Pending;
            //    this.recordParticipants.LinkParticipantsRequestOption = this.RequestOption;
            //    this.recordParticipants.Load();
            //}
            //else
            //{
            //    this.Group = this.CreateGroup();
            //    this.ControllerState = GroupModelControllerState.Finished;
            //}

            if (string.IsNullOrEmpty(this.linkParticipantsName))
            {
                this.Group = null;
                this.ControllerState = GroupModelControllerState.Pending;
                this.recordParticipants.LinkParticipantsRequestOption = this.RequestOption;
                this.recordParticipants.Load();
            }
            else
            {
                this.Group = this.CreateGroup();
                this.ControllerState = GroupModelControllerState.Finished;
            }
            //

            this.signalFinished = true;
            return this.Group;
        }

        private UPMGroup BuildGroup(UPMGroup group)
        {
            Dictionary<string, string> infoAreaImageFileNames = new Dictionary<string, string>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigExpand repExpand = configStore.ExpandByName("ID");
            this.linkParticipantsViewReference = null;

            if (!string.IsNullOrEmpty(this.linkParticipantsName))
            {
                SearchAndList searchAndList = configStore.SearchAndListByName(this.linkParticipantsName);
                Menu menu = searchAndList != null ? configStore.MenuByName(searchAndList.DefaultAction) : null;
                this.linkParticipantsViewReference = menu?.ViewReference;
            }

            if (this.acceptanceCatalogAttributes == null && this.recordParticipants.AcceptanceField != null)
            {
                this.acceptanceCatalogAttributes = configStore.CatalogAttributesForInfoAreaIdFieldId(this.recordParticipants.AcceptanceField.InfoAreaId, this.recordParticipants.AcceptanceField.FieldId);
            }

            foreach (UPCRMParticipant participant in this.recordParticipants.Participants)
            {
                bool isRepParticipant = participant is UPCRMRepParticipant;
                IIdentifier identifier = StringIdentifier.IdentifierWithStringId(participant.Key);
                UPMListRow listRow = new UPMListRow(identifier);
                UPMStringField nameField = new UPMStringField(StringIdentifier.IdentifierWithStringId("name"));
                nameField.StringValue = participant.Name;
                listRow.AddField(nameField);
                UPMStringField acceptanceField = new UPMStringField(StringIdentifier.IdentifierWithStringId("acceptance"));
                if (!isRepParticipant || this.recordParticipants.HasRepAcceptance)
                {
                    acceptanceField.StringValue = participant.AcceptanceDisplayText;
                    UPConfigCatalogValueAttributes configCatalogValueAttributes = this.acceptanceCatalogAttributes?.ValuesByCode[Convert.ToInt32(participant.AcceptanceText)];
                    string colorString = configCatalogValueAttributes?.ColorKey;
                    if (!string.IsNullOrEmpty(colorString))
                    {
                        listRow.RowColor = AureaColor.ColorWithString(colorString);
                    }
                }
                else
                {
                    acceptanceField.StringValue = string.Empty;
                }

                listRow.AddField(acceptanceField);
                UPMStringField requirementField = new UPMStringField(StringIdentifier.IdentifierWithStringId("requirement"));
                requirementField.StringValue = participant.RequirementDisplayText;
                listRow.AddField(requirementField);
                if (isRepParticipant)
                {
                    // listRow.Icon = UIImage.UpImageWithFileName(repExpand.ImageName);     // CRM-5007
                    listRow.RowAction = null;
                }
                else
                {
                    UPCRMLinkParticipant linkParticipant = (UPCRMLinkParticipant)participant;
                    string _infoAreaId = linkParticipant.LinkRecordIdentification.InfoAreaId();
                    string imageName = infoAreaImageFileNames.ValueOrDefault(_infoAreaId);
                    if (imageName == null)
                    {
                        UPConfigExpand expand = configStore.ExpandByName(_infoAreaId);
                        imageName = expand.ImageName ?? string.Empty;

                        infoAreaImageFileNames.SetObjectForKey(imageName, _infoAreaId);
                    }

                    // listRow.Icon = UIImage.UpImageWithFileName(imageName);       // CRM-5007
                    listRow.OnlineData = !UPCRMDataStore.DefaultStore.RecordExistsOffline(linkParticipant.LinkRecordIdentification);

                    if (this.linkParticipantsViewReference != null)
                    {
                        UPMAction switchToOrganizerAction = new UPMAction(null);
                        switchToOrganizerAction.IconName = "arrow.png";
                        switchToOrganizerAction.SetTargetAction(this, this.SwitchToOrganizerForLinkParticipant);
                        listRow.RowAction = switchToOrganizerAction;
                    }
                }

                group.AddChild(listRow);
            }

            return group;
        }

        /// <summary>
        /// Switches to organizer for link participant.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void SwitchToOrganizerForLinkParticipant(object sender)
        {
            UPMListRow listRow = (UPMListRow)sender;
            IIdentifier identifier = listRow.Identifier;
            UPCRMLinkParticipant linkParticipant = (UPCRMLinkParticipant)this.recordParticipants.ParticipantWithKey(identifier.IdentifierAsString);
            if (linkParticipant != null)
            {
                ViewReference viewReference = this.linkParticipantsViewReference.ViewReferenceWith(linkParticipant.LinkRecordIdentification);
                if (viewReference != null)
                {
                    this.Delegate.PerformOrganizerAction(this, viewReference, listRow.OnlineData);
                }
            }
        }

        private UPMGroup CreateGroup()
        {
            UPMListGroup listGroup = new UPMListGroup(this.ExplicitTabIdentifier, null);
            listGroup.LabelText = this.TabLabel;
            return this.BuildGroup(listGroup);
        }

        /// <summary>
        /// The CRM participants did finish with result.
        /// </summary>
        /// <param name="recordParticipants">The record participants.</param>
        /// <param name="result">The result.</param>
        public void CrmParticipantsDidFinishWithResult(UPCRMParticipants recordParticipants, object result)
        {
            this.Group = this.CreateGroup();
            this.ControllerState = GroupModelControllerState.Finished;
            if (this.signalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <summary>
        /// The CRM participants did fail with error.
        /// </summary>
        /// <param name="recordParticipants">The record participants.</param>
        /// <param name="error">The error.</param>
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
    }
}
