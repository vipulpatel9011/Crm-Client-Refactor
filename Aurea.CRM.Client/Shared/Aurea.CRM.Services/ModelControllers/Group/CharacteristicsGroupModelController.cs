// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacteristicsGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Characteristics Group Model Controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Characteristics;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Characteristics Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    /// <seealso cref="Aurea.CRM.UIModel.Characteristics.UPCharacteristicsDelegate" />
    public class UPCharacteristicsGroupModelController : UPFieldControlBasedGroupModelController, UPCharacteristicsDelegate
    {
        private bool signalFinished;
        private string recordIdentification;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristicsGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPCharacteristicsGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
            var typeParts = this.TabConfig.Type.Split('_');
            if (typeParts.Length > 1)
            {
                if (typeParts[0] == "CHARACTERISTICS")
                {
                    this.CharacteristicsSearchAndListName = typeParts[1];
                }
                else if (typeParts[0] == "CHARACTERISTICSCONTEXT")
                {
                    this.CharacteristicsContext = typeParts[1];
                }
            }
            else if (this.TabConfig.Fields.Count > 0)
            {
                this.CharacteristicsFieldControl = fieldControl.FieldControlWithSingleTab(tabIndex, this.TabConfig.Fields[0].InfoAreaId);
            }
            else
            {
                this.CharacteristicsSearchAndListName = "IT";
            }
        }

        /// <summary>
        /// Gets the name of the characteristics search and list.
        /// </summary>
        /// <value>
        /// The name of the characteristics search and list.
        /// </value>
        public string CharacteristicsSearchAndListName { get; private set; }

        /// <summary>
        /// Gets the characteristics context.
        /// </summary>
        /// <value>
        /// The characteristics context.
        /// </value>
        public string CharacteristicsContext { get; private set; }

        /// <summary>
        /// Gets the characteristics.
        /// </summary>
        /// <value>
        /// The characteristics.
        /// </value>
        public UPCharacteristics Characteristics { get; private set; }

        /// <summary>
        /// Gets the characteristics field control.
        /// </summary>
        /// <value>
        /// The characteristics field control.
        /// </value>
        public FieldControl CharacteristicsFieldControl { get; private set; }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            UPMGroup upmGroup = this.ApplyLinkRecordIdentification(row.RootRecordIdentification);
            upmGroup?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(this.recordIdentification));
            return upmGroup;
        }

        /// <summary>
        /// Characteristicses the did finish with result.
        /// </summary>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="result">The result.</param>
        public void CharacteristicsDidFinishWithResult(UPCharacteristics characteristics, object result)
        {
            this.Group = this.CreateGroup(this.recordIdentification);
            this.ControllerState = this.Group != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;

            if (this.signalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        /// <summary>
        /// Characteristicses the did fail with error.
        /// </summary>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="error">The error.</param>
        public void CharacteristicsDidFailWithError(UPCharacteristics characteristics, Exception error)
        {
            this.Error = error;
            if (this.signalFinished)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }

        private UPMGroup ApplyLinkRecordIdentification(string _recordIdentification)
        {
            this.recordIdentification = _recordIdentification;
            if (this.CharacteristicsFieldControl != null)
            {
                this.Characteristics = new UPCharacteristics(this.recordIdentification, this.CharacteristicsFieldControl, this.RequestOption);
            }
            else
            {
                if (!string.IsNullOrEmpty(this.CharacteristicsSearchAndListName))
                {
                    this.Characteristics = new UPCharacteristics(this.recordIdentification, this.CharacteristicsSearchAndListName, this.RequestOption);
                }
                else if (!string.IsNullOrEmpty(this.CharacteristicsContext))
                {
                    this.Characteristics = new UPCharacteristics(this.recordIdentification, this.CharacteristicsContext);
                }
            }

            this.signalFinished = false;
            this.ControllerState = GroupModelControllerState.Pending;
            this.Characteristics.Build(this);
            this.signalFinished = true;
            return this.Group;
        }

        private UPMGroup CreateGroup(string _recordIdentification)
        {
            UPMCharacteristicsGroup charakteristicsGroup = new UPMCharacteristicsGroup(StringIdentifier.IdentifierWithStringId(this.Characteristics.RecordIdentification));
            charakteristicsGroup.LabelText = this.TabLabel;
            int groupId = 0;
            int itemId = 0;

            if (this.Characteristics.Groups != null)
            {
                foreach (UPCharacteristicsGroup crmCharacteristicsGroup in this.Characteristics.Groups)
                {
                    FieldIdentifier fieldIdentifier = FieldIdentifier.IdentifierWithRecordIdentificationFieldId(_recordIdentification, this.Characteristics.DestinationGroupField.Identification);
                    UPMCharacteristicsItemGroup characteristicsItemGroup = new UPMCharacteristicsItemGroup(StringIdentifier.IdentifierWithStringId($"Group{groupId}_{itemId}"));
                    characteristicsItemGroup.GroupNameField = new UPMStringField(fieldIdentifier);
                    SetAttributesOnField(this.Characteristics.DestinationGroupField.Attributes, characteristicsItemGroup.GroupNameField);
                    characteristicsItemGroup.GroupNameField.StringValue = crmCharacteristicsGroup.Label;
                    charakteristicsGroup.AddChild(characteristicsItemGroup);
                    foreach (UPCharacteristicsItem crmCharacteristicsItem in crmCharacteristicsGroup.Items)
                    {
                        fieldIdentifier = FieldIdentifier.IdentifierWithRecordIdentificationFieldId(_recordIdentification, this.Characteristics.DestinationItemField.Identification);
                        UPMCharacteristicsItem characteristicsItem = new UPMCharacteristicsItem(StringIdentifier.IdentifierWithStringId($"Item{groupId}_{itemId}"));
                        characteristicsItem.ItemNameField = new UPMStringField(fieldIdentifier);
                        SetAttributesOnField(this.Characteristics.DestinationItemField.Attributes, characteristicsItem.ItemNameField);
                        characteristicsItem.ItemNameField.StringValue = crmCharacteristicsItem.Label;
                        characteristicsItemGroup.AddChild(characteristicsItem);
                        List<UPMField> additionalFields = new List<UPMField>();
                        if (crmCharacteristicsItem.ShowAdditionalFields)
                        {
                            int i = 0;
                            foreach (UPConfigFieldControlField crmAdditionalField in crmCharacteristicsItem.AdditionalFields)
                            {
                                fieldIdentifier = FieldIdentifier.IdentifierWithRecordIdentificationFieldId(_recordIdentification, crmAdditionalField.Identification);
                                UPMStringField additionalField = new UPMStringField(fieldIdentifier);
                                if (!string.IsNullOrEmpty(crmCharacteristicsItem.Values[i]))
                                {
                                    SetAttributesOnField(crmAdditionalField.Attributes, additionalField);
                                    FieldAttributes fieldAttributes = crmAdditionalField.Attributes;
                                    if (!fieldAttributes.NoLabel)
                                    {
                                        additionalField.LabelText = crmAdditionalField.Field.Label;
                                    }

                                    additionalField.StringValue = crmCharacteristicsItem.Values[i];
                                    if (!string.IsNullOrEmpty(additionalField.StringValue))
                                    {
                                        additionalFields.Add(additionalField);
                                    }
                                }

                                i++;
                            }
                        }

                        characteristicsItem.EditFields = additionalFields;
                        itemId++;
                    }

                    groupId++;
                }
            }

            return charakteristicsGroup;
        }
    }
}
