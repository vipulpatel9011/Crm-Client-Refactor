// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetailsFieldGroupModelController.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------s

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Services;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Structs;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Details field group model controller
    /// </summary>
    /// <seealso cref="UPFieldControlBasedGroupModelController" />
    public class UPDetailsFieldGroupModelController : UPFieldControlBasedGroupModelController
    {
        private ViewReference _currentViewReference;
        private bool columnStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDetailsFieldGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPDetailsFieldGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
            var tab = fieldControl.TabAtIndex(tabIndex);
            var tabType = tab?.Type;
            this.columnStyle = tabType == "GRID";
        }

        public UPDetailsFieldGroupModelController(FormItem formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
        }

        /// <summary>
        /// Performs the link record action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PerformLinkRecordAction(object sender)
        {
            var linkRecordField = sender as UPMLinkRecordField;
            if (linkRecordField == null)
            {
                return;
            }

            var viewReference = this.ActionIdentifierToViewReferenceDictionary.ValueOrDefault($"{linkRecordField.Identifier}");
            this.Delegate?.PerformOrganizerAction(sender, viewReference);
        }

        /// <summary>
        /// Groups from row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            return this.GroupFromRowGroup(resultRow, null);
        }

        /// <summary>
        /// Groups from row group.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        public UPMGroup GroupFromRowGroup(UPCRMResultRow resultRow, UPMGroup group)
        {
            var recordIdentification = resultRow.RootRecordIdentification;

            var detailGroup = this.AddFieldsToDetailGroup(group, recordIdentification, resultRow, out var documentKey, out var documentFieldConfig);

            var standardGroup = detailGroup as UPMStandardGroup;
            if (standardGroup != null && !string.IsNullOrEmpty(documentKey))
            {
                ProccessStandardGroup(standardGroup, documentFieldConfig, documentKey, recordIdentification);
            }

            this.ControllerState = detailGroup != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
            this.Group = detailGroup;
            return detailGroup;
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            var group = this.GroupFromRow(row);

            group?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification));
            return group;
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> dictionary)
        {
            this._currentViewReference = this.FormItem.ViewReference.ViewReferenceWith(dictionary);
            if (this._currentViewReference != null)
            {
                this.Group = null;
                this.ControllerState = GroupModelControllerState.Empty;
                return null;
            }

            this.Group = null;
            this.ControllerState = GroupModelControllerState.Empty;
            return null;
        }

        private static UPMStringField CreateStringField(FieldIdentifier fieldIdentifier, FieldAttributes fieldAttributes, string fieldValue)
        {
            var field = new UPMStringField(fieldIdentifier)
            {
                StripNewLines = fieldAttributes.NoMultiLine,
                StringValue = fieldValue
            };
            return field;
        }

        private static UPMPhoneField CreatePhoneField(FieldAttributes fieldAttributes, FieldIdentifier fieldIdentifier)
        {
            var field = new UPMPhoneField(fieldIdentifier);
            var phoneInExtendedOptions = fieldAttributes.ExtendedOptions.ValueOrDefault("phone");
            if (phoneInExtendedOptions != null)
            {
                field.UseTelprompt = phoneInExtendedOptions == "telprompt";
            }

            return field;
        }

        private static UPMStringField CreateStringField(FieldAttributes fieldAttributes, FieldIdentifier fieldIdentifier)
        {
            UPMStringField field;
            var phoneInExtendedOptions = fieldAttributes?.ExtendedOptions?.ValueOrDefault("phone");
            if (phoneInExtendedOptions != null)
            {
                field = new UPMPhoneField(fieldIdentifier);
                ((UPMPhoneField)field).UseTelprompt = phoneInExtendedOptions == "telprompt";
            }
            else
            {
                field = new UPMStringField(fieldIdentifier);
            }

            return field;
        }

        private static void UpdateField(UPMStringField field, FieldAttributes fieldAttributes, UPConfigFieldControlField fieldConfig, string fieldValue)
        {
            if (fieldConfig.Attributes.ExtendedOptionIsSet("newLine"))
            {
                field.NewLine = true;
            }

            var multilineInGridMaxRows = 2;
            object migmrAsObject = fieldAttributes.ExtendedOptions.ValueOrDefault("multiLineMaxRows");
            if (migmrAsObject != null)
            {
                multilineInGridMaxRows = int.Parse((string)migmrAsObject);
                multilineInGridMaxRows = Math.Max(2, multilineInGridMaxRows);
            }

            field.MultilineInGridMaxRows = multilineInGridMaxRows;
            field.StripNewLines = fieldAttributes.NoMultiLine;
            var htmlField = fieldConfig.Field.FieldInfo?.HtmlField == true;
            field.StringValue = htmlField ? fieldValue.StripHtml() : fieldValue;

            if (!fieldAttributes.NoLabel)
            {
                field.LabelText = fieldConfig.Label;
            }
        }

        private static void ProccessStandardGroup(UPMStandardGroup standardGroup, UPConfigFieldControlField documentFieldConfig, string documentKey, string recordIdentification)
        {
            var fieldAttributes = documentFieldConfig?.Attributes;
            var attribute = fieldAttributes?.AttributForId((int)FieldAttr.Image);
            if (attribute?.ValueOptionsForKey("previewWidth") != null &&
                attribute.ValueOptionsForKey("previewHeight") != null)
            {
                var width = attribute.ValueOptionsForKey("previewWidth");
                var height = attribute.ValueOptionsForKey("previewHeight");

                standardGroup.ImageDocumentMaxSize = new Size(
                    width.ToInt(),
                    height.ToInt());
            }

            var documentManager = new DocumentManager();
            var documentData = documentManager.DocumentForKey(documentKey);
            var imageDocument = documentData != null
                ? new UPMDocument(documentData)
                : new UPMDocument(
                    new RecordIdentifier(recordIdentification),
                    null,
                    null,
                    null,
                    null,
                    ServerSession.CurrentSession.DocumentRequestUrlForDocumentKey(documentKey),
                    null,
                    null,
                    null,
                    null);

            standardGroup.ImageDocument = imageDocument;
        }

        private UPMGroup AddFieldsToDetailGroup(UPMGroup upmGroup, string recordIdentification, UPCRMResultRow resultRow, out string documentKey, out UPConfigFieldControlField documentFieldConfig)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var hideEmptyFields = configStore.ConfigValueIsSet("View.HideEmptyFields");
            documentKey = string.Empty;
            documentFieldConfig = null;
            var listFormatter = new UPCRMListFormatter(this.TabConfig, false);
            var fieldCount = listFormatter.PositionCount;
            var detailGroup = upmGroup;
            for (var j = 0; j < fieldCount; j++)
            {
                var fieldConfig = listFormatter.FirstFieldForPosition(j);
                var fieldAttributes = fieldConfig.Attributes;
                var fieldIdentifier = FieldIdentifier.IdentifierWithRecordIdentificationFieldId(recordIdentification, fieldConfig.Identification);
                var hasFieldValue = false;
                if (fieldAttributes.Image)
                {
                    documentKey = resultRow.ValueAtIndex(fieldConfig.TabIndependentFieldIndex);
                    documentFieldConfig = fieldConfig;
                    if (!string.IsNullOrEmpty(documentKey) && fieldCount == 1)
                    {
                        detailGroup = this.AddFieldToDetailGroup(detailGroup, fieldIdentifier, recordIdentification);
                    }

                    continue;
                }

                if (fieldAttributes.Hide)
                {
                    continue;
                }

                if (fieldAttributes.Empty)
                {
                    detailGroup = this.AddFieldToDetailGroup(detailGroup, recordIdentification, fieldIdentifier, fieldAttributes);
                    continue;
                }

                string fieldValue = null;

                try
                {
                    fieldValue = listFormatter.StringFromRowForPosition(resultRow, j);
                }
                catch (Exception error)
                {
                    SimpleIoc.Default.GetInstance<ILogger>().LogError(error);
                }

                if (!string.IsNullOrEmpty(fieldValue))
                {
                    hasFieldValue = true;
                }

                if (!hasFieldValue && hideEmptyFields && !this.columnStyle)
                {
                    if (!(detailGroup is UPMCalendarPopoverGroup) || j > 3)
                    {
                        continue;
                    }
                }

                if (fieldAttributes.MultiLine && j + 1 == fieldCount && detailGroup == null)
                {
                    var multiLineGroup = this.CreateMultilineGroup(recordIdentification, CreateStringField(fieldIdentifier, fieldAttributes, fieldValue), fieldConfig);
                    this.ControllerState = GroupModelControllerState.Finished;
                    this.Group = multiLineGroup;
                    return multiLineGroup;
                }

                var field = this.CreateField(fieldAttributes, fieldIdentifier, fieldConfig, resultRow, recordIdentification, configStore);
                if (field == null)
                {
                    continue;
                }

                UpdateField(field, fieldAttributes, fieldConfig, fieldValue);
                if (string.IsNullOrWhiteSpace(field.LabelText) && string.IsNullOrEmpty(fieldValue))
                {
                    continue;
                }

                detailGroup = this.AddFieldToDetailGroup(detailGroup, fieldAttributes, field, recordIdentification);
            }

            return detailGroup;
        }

        private UPMGroup AddFieldToDetailGroup(UPMGroup detailGroup, FieldIdentifier fieldIdentifier, string recordIdentification)
        {
            if (detailGroup == null)
            {
                detailGroup = new UPMStandardGroup(this.TabIdentifierForRecordIdentification(recordIdentification));
                ((UPMStandardGroup)detailGroup).ColumnStyle = this.columnStyle;
                detailGroup.LabelText = this.TabLabel;
            }

            var field = new UPMStringField(fieldIdentifier);
            detailGroup.AddField(field);

            return detailGroup;
        }

        private UPMGroup AddFieldToDetailGroup(UPMGroup detailGroup, FieldAttributes fieldAttributes, UPMStringField field, string recordIdentification)
        {
            SetAttributesOnField(fieldAttributes, field);
            if (detailGroup == null)
            {
                detailGroup = new UPMStandardGroup(this.TabIdentifierForRecordIdentification(recordIdentification));
                ((UPMStandardGroup)detailGroup).ColumnStyle = this.columnStyle;
                detailGroup.LabelText = this.TabLabel;
            }

            detailGroup.AddField(field);

            return detailGroup;
        }

        private UPMGroup AddFieldToDetailGroup(UPMGroup detailGroup, string recordIdentification, FieldIdentifier fieldIdentifier, FieldAttributes fieldAttributes)
        {
            if (detailGroup == null)
            {
                detailGroup = new UPMStandardGroup(this.TabIdentifierForRecordIdentification(recordIdentification));
                ((UPMStandardGroup)detailGroup).ColumnStyle = this.columnStyle;
                detailGroup.LabelText = this.TabLabel;
            }

            var field = new UPMStringField(fieldIdentifier)
            {
                StripNewLines = fieldAttributes.NoMultiLine
            };

            detailGroup.AddField(field);

            return detailGroup;
        }

        private UPMGroup CreateMultilineGroup(string recordIdentification, UPMStringField field, UPConfigFieldControlField fieldConfig)
        {
            var multilineGroup = new UPMMultilineGroup(this.TabIdentifierForRecordIdentification(recordIdentification))
            {
                LabelText = this.TabLabel,
                MultilineStringField = field,
                Html = fieldConfig.Field.FieldInfo.HtmlField
            };

            return multilineGroup;
        }

        private UPMStringField CreateField(
            FieldAttributes fieldAttributes,
            FieldIdentifier fieldIdentifier,
            UPConfigFieldControlField fieldConfig,
            UPCRMResultRow resultRow,
            string recordIdentification,
            IConfigurationUnitStore configStore)
        {
            UPMStringField field = null;
            if (fieldAttributes.Email)
            {
                var linkEmailAction = new UPMAction(StringIdentifier.IdentifierWithStringId("linkEmailActionId"));
                linkEmailAction.SetTargetAction(this, (sender) =>
                {
                    var url = "mailto://" + (sender as UPMEmailField)?.StringValue;
                    var deviceService = SimpleIoc.Default.GetInstance<IDeviceService>();
                    deviceService?.OpenUri(new Uri(url));
                });
                field = new UPMEmailField(fieldIdentifier, linkEmailAction);
            }
            else if (fieldAttributes.Phone)
            {
                field = CreatePhoneField(fieldAttributes, fieldIdentifier);
            }
            else if (fieldAttributes.Httplink)
            {
                field = this.CreateStringField(fieldConfig, fieldIdentifier, resultRow, recordIdentification, configStore);
            }
            else
            {
                field = CreateStringField(fieldAttributes, fieldIdentifier);
            }

            return field;
        }

        private UPMStringField CreateStringField(
            UPConfigFieldControlField fieldConfig,
            FieldIdentifier fieldIdentifier,
            UPCRMResultRow resultRow,
            string recordIdentification,
            IConfigurationUnitStore configStore)
        {
            UPMStringField field;
            if (fieldConfig.IsLinkedField)
            {
                var fieldRecordIdentification = resultRow.PhysicalRecordIdentificationAtFieldIndex(fieldConfig.TabIndependentFieldIndex);
                if (!string.IsNullOrEmpty(fieldRecordIdentification))
                {
                    var linkRecordAction = new UPMAction(StringIdentifier.IdentifierWithStringId("linkRecordActionId"));
                    var showRecordMenu = configStore.DefaultMenuForInfoAreaId(fieldConfig.InfoAreaId);
                    var linkRecordViewReference = showRecordMenu.ViewReference.ViewReferenceWith(fieldRecordIdentification);
                    this.AddViewReferenceForActionKey(linkRecordViewReference, $"{fieldIdentifier}");
                    linkRecordAction.SetTargetAction(this, this.PerformLinkRecordAction);
                    field = new UPMLinkRecordField(fieldIdentifier, linkRecordAction);
                }
                else
                {
                    field = new UPMStringField(fieldIdentifier);
                }
            }
            else
            {
                var linkUrlAction = new UPMAction(StringIdentifier.IdentifierWithStringId("linkUrlActionId"));
                linkUrlAction.SetTargetAction(this, (sender) =>
                {
                    var url = (sender as UPMURLField)?.StringValue;
                    if (url != null && !url.Contains("://"))
                    {
                        url = "http://" + url;
                    }
                    var deviceService = SimpleIoc.Default.GetInstance<IDeviceService>();
                    deviceService?.OpenUri(new Uri(url));
                });
                field = new UPMURLField(fieldIdentifier, linkUrlAction);
            }

            return field;
        }
    }
}
