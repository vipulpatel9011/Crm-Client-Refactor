// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacteristicsEditPageModelController.cs" company="Aurea Software Gmbh">
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
//   The CharacteristicsEditPageModelController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Characteristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Characteristics;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// CharacteristicsEditPageModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Edit.EditPageModelController" />
    /// <seealso cref="Aurea.CRM.UIModel.Characteristics.UPCharacteristicsDelegate" />
    public class UPCharacteristicsEditPageModelController : EditPageModelController, UPCharacteristicsDelegate
    {
        private UPOfflineCharacteristicsRequest offlineRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristicsEditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="characteristicsRootRecordIdentification">The characteristics root record identification.</param>
        /// <param name="_offlineRequest">The offline request.</param>
        public UPCharacteristicsEditPageModelController(ViewReference viewReference, string characteristicsRootRecordIdentification, UPOfflineCharacteristicsRequest _offlineRequest)
            : base(viewReference)
        {
            this.CharacteristicsRootRecordIdentification = characteristicsRootRecordIdentification;
            this.offlineRequest = _offlineRequest;
            this.BuildPage();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristicsEditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPCharacteristicsEditPageModelController(ViewReference viewReference)
            : this(viewReference, viewReference.ContextValueForKey("RecordId"), null)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the characteristics.
        /// </summary>
        /// <value>
        /// The characteristics.
        /// </value>
        public UPCharacteristics Characteristics { get; private set; }

        /// <summary>
        /// Gets the characteristics root record identification.
        /// </summary>
        /// <value>
        /// The characteristics root record identification.
        /// </value>
        public string CharacteristicsRootRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            return this.Page != null
                ? new UPMCharacteristicsPage(this.Page.Identifier)
                : new UPMCharacteristicsPage(FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(this.InfoAreaId, this.recordId, "Page0"));
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            Page page = this.InstantiatePage();
            page.LabelText = LocalizedString.TextEdit;
            page.Invalid = true;
            this.IsReadOnly = this.ViewReference.ContextValueIsSet("ReadOnly");
            this.IsReadOnly = this.IsReadOnly || this.ViewReference.ViewName == "CharacteristicsView";
            this.LinkRecordIdentification = this.ViewReference.ContextValueForKey("LinkRecord");
            if (this.CharacteristicsRootRecordIdentification == null)
            {
                this.CharacteristicsRootRecordIdentification = this.LinkRecordIdentification ??
                                                               StringExtensions.InfoAreaIdRecordId(this.InfoAreaId, this.recordId);
            }

            this.editPageContext = new UPEditPageContext(this.CharacteristicsRootRecordIdentification, false, null, null, this.ViewReference);
            this.TopLevelElement = page;
            this.ApplyLoadingStatusOnPage(page);
        }

        private UPCharacteristicsItem FindCharacteristicItemWithGroupIdentifierItemIdentifier(IIdentifier groupIdentifier, IIdentifier itemIdentifier)
        {
            foreach (UPCharacteristicsGroup crmGroup in this.Characteristics.Groups)
            {
                if (crmGroup.CatalogValue == groupIdentifier.IdentifierAsString)
                {
                    foreach (UPCharacteristicsItem crmItem in crmGroup.Items)
                    {
                        if (crmItem.CatalogValue == itemIdentifier.IdentifierAsString)
                        {
                            return crmItem;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether [process changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [process changes]; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessChanges => this.IsReadOnly;

        /// <summary>
        /// Applies the loading status on page.
        /// </summary>
        /// <param name="page">The page.</param>
        public override void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.StringValue = LocalizedString.TextLoadingData;
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Returns changed records.
        /// </summary>
        /// <returns></returns>
        public override List<UPCRMRecord> ChangedRecords()
        {
            if (this.IsReadOnly)
            {
                return null;
            }

            UPMCharacteristicsPage characteristicsPage = (UPMCharacteristicsPage)this.Page;
            foreach (UPMCharacteristicsItemGroup group in characteristicsPage.Children)
            {
                foreach (UPMCharacteristicsItem item in group.Children)
                {
                    UPCharacteristicsItem crmItem = this.FindCharacteristicItemWithGroupIdentifierItemIdentifier(group.Identifier, item.Identifier);
                    bool updateEditFields = true;
                    if (item.SelectedField.Changed)
                    {
                        if (crmItem != null)
                        {
                            if (item.SelectedField.BoolValue)
                            {
                                crmItem.MarkItemAsSet();
                            }
                            else
                            {
                                crmItem.MarkItemAsUnset();
                                updateEditFields = false;
                            }
                        }
                    }

                    if (updateEditFields)
                    {
                        for (int editFieldIndex = 0; editFieldIndex < item.EditFields.Count; editFieldIndex++)
                        {
                            UPMEditField field = item.EditFields[editFieldIndex] as UPMEditField;
                            if (field != null && field.Changed)
                            {
                                List<UPEditFieldContext> fieldContextList = this.editPageContext.EditFields.Values.ToList();
                                foreach (UPEditFieldContext fieldContext in fieldContextList)
                                {
                                    if (fieldContext.EditField == field)
                                    {
                                        crmItem?.SetValueForAdditionalFieldPosition(fieldContext.Value, editFieldIndex);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return this.Characteristics?.ChangedRecords();
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElementForPage(Page page)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> { { "viewReference", this.ViewReference } };
            this.Characteristics = this.offlineRequest != null
                ? new UPCharacteristics(this.offlineRequest, this.CharacteristicsRootRecordIdentification, parameters)
                : new UPCharacteristics(this.CharacteristicsRootRecordIdentification, parameters, true);

            this.Characteristics.Build(this);
            return (Page)this.TopLevelElement;
        }

        private void FillPage()
        {
            UPMCharacteristicsPage newPage = (UPMCharacteristicsPage)this.InstantiatePage();
            newPage.IsReadOnly = this.IsReadOnly;
            foreach (UPCharacteristicsGroup crmGroup in this.Characteristics.Groups)
            {
                UPMCharacteristicsItemGroup group = new UPMCharacteristicsItemGroup(StringIdentifier.IdentifierWithStringId(crmGroup.CatalogValue))
                {
                    GroupNameField = new UPMStringField(StringIdentifier.IdentifierWithStringId(crmGroup.Label))
                    {
                        StringValue = crmGroup.Label
                    },
                    ShowExpanded = crmGroup.ShowExpanded,
                    GroupType = crmGroup.SingleSelection ? UPMCharacteristicsItemGroupType.SingleSelect : UPMCharacteristicsItemGroupType.MultiSelect
                };

                foreach (UPCharacteristicsItem crmItem in crmGroup.Items)
                {
                    UPMCharacteristicsItem item = new UPMCharacteristicsItem(StringIdentifier.IdentifierWithStringId(crmItem.CatalogValue))
                    {
                        ItemNameField = new UPMStringField(StringIdentifier.IdentifierWithStringId(crmItem.Label))
                        {
                            StringValue = crmItem.Label
                        },
                        SelectedField = new UPMBooleanEditField(StringIdentifier.IdentifierWithStringId(crmItem.Label))
                        {
                            BoolValue = crmItem.Record != null
                        }
                    };
                    group.AddChild(item);

                    List<UPMField> additionalEditFields = new List<UPMField>();
                    if (crmItem.ShowAdditionalFields)
                    {
                        for (int additionalFieldIndex = 0; additionalFieldIndex < crmItem.AdditionalFields.Count; additionalFieldIndex++)
                        {
                            UPConfigFieldControlField configField = crmItem.AdditionalFields[additionalFieldIndex];
                            FieldIdentifier fieldIdentifier = FieldIdentifier.IdentifierWithRecordIdentificationFieldId(this.CharacteristicsRootRecordIdentification, configField.Identification);
                            UPEditFieldContext fieldContext = UPEditFieldContext.FieldContextFor(configField, fieldIdentifier, crmItem.Values[additionalFieldIndex], (List<UPEditFieldContext>)null);
                            if (fieldContext != null)
                            {
                                additionalEditFields.Add(fieldContext.EditField);
                                this.editPageContext.EditFields.SetObjectForKey(fieldContext, $"{configField.Identification}-{crmItem.CatalogValue}-{crmGroup.CatalogValue}");
                            }
                        }
                    }

                    item.EditFields = additionalEditFields;
                }

                if (group.Children.Count > 0)
                {
                    newPage.AddChild(group);
                }
            }

            if (this.IsReadOnly)
            {
                newPage = this.PageForOverview(newPage);
            }

            ITopLevelElement oldPage = this.TopLevelElement;
            this.TopLevelElement = newPage;
            newPage.Invalid = false;
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        private UPMCharacteristicsPage PageForOverview(Page page)
        {
            UPMCharacteristicsPage newPage = (UPMCharacteristicsPage)this.InstantiatePage();
            newPage.IsReadOnly = true;
            foreach (UPMCharacteristicsItemGroup group in page.Children)
            {
                UPMCharacteristicsItemGroup newGroup = new UPMCharacteristicsItemGroup(group.Identifier);
                newGroup.GroupNameField = new UPMStringField(group.GroupNameField.Identifier);
                newGroup.GroupNameField.StringValue = group.GroupNameField.StringValue;
                newGroup.ShowExpanded = true;
                newGroup.GroupType = group.GroupType;

                foreach (UPMCharacteristicsItem item in group.Children)
                {
                    if (item.SelectedField.BoolValue)
                    {
                        UPMCharacteristicsItem newItem = new UPMCharacteristicsItem(item.Identifier)
                        {
                            ItemNameField = new UPMStringField(item.ItemNameField.Identifier),
                            SelectedField = new UPMBooleanEditField(item.SelectedField.Identifier)
                        };
                        newItem.ItemNameField.StringValue = item.ItemNameField.StringValue;
                        newItem.SelectedField.BoolValue = true;

                        List<UPMField> additionalFields = new List<UPMField>();
                        foreach (UPMEditField editField in item.EditFields)
                        {
                            UPMField newField = new UPMStringField(editField.Identifier);
                            newField.LabelText = editField.LabelText;
                            if (!editField.Empty)
                            {
                                newField.FieldValue = editField.StringDisplayValue;
                                additionalFields.Add(newField);
                            }
                        }

                        newItem.EditFields = additionalFields;
                        newGroup.AddChild(newItem);
                    }
                }

                if (newGroup.Children.Count > 0)
                {
                    newPage.AddChild(newGroup);
                }
            }

            return newPage;
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            var page = element as Page;
            return page != null ? this.UpdatedElementForPage(page) : element;
        }

        /// <summary>
        /// Characteristicses the did finish with result.
        /// </summary>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="result">The result.</param>
        public void CharacteristicsDidFinishWithResult(UPCharacteristics characteristics, object result)
        {
            this.FillPage();
        }

        /// <summary>
        /// Characteristicses the did fail with error.
        /// </summary>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="error">The error.</param>
        public void CharacteristicsDidFailWithError(UPCharacteristics characteristics, Exception error)
        {
            this.ReportError(error, false);
        }
    }
}
