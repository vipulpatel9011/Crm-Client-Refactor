// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentFieldGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The UPDocumentFieldGroupModelController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// UPDocumentFieldGroupModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    public class UPDocumentFieldGroupModelController : UPFieldControlBasedGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPDocumentFieldGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPDocumentFieldGroupModelController(IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPDocumentFieldGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        private UPMGroup GroupFromRow(UPCRMResultRow resultRow)
        {
            UPMDocumentsGroup docGroup = null;
            string recordIdentification = resultRow.RootRecordIdentification;
            int fieldCount = this.TabConfig.NumberOfFields;
            DocumentManager documentManager = new DocumentManager();
            for (int j = 0; j < fieldCount; j++)
            {
                UPConfigFieldControlField fieldConfig = this.TabConfig.FieldAtIndex(j);
                string documentKey = resultRow.ValueAtIndex(fieldConfig.TabIndependentFieldIndex);
                if (!string.IsNullOrEmpty(documentKey))
                {
                    DocumentData documentData = documentManager.DocumentForKey(documentKey);
                    if (documentData != null)
                    {
                        if (docGroup == null)
                        {
                            docGroup = new UPMDocumentsGroup(this.TabIdentifierForRecordIdentification(recordIdentification));
                            docGroup.LabelText = this.TabLabel;
                        }

                        UPMDocument document = new UPMDocument(new RecordIdentifier(recordIdentification), fieldConfig.Label,
                            documentData.DateString, documentData.SizeString, null, documentData.Url, documentData.Title,
                            documentData.ServerUpdateDate, documentData.DisplayText, null);

                        docGroup.AddField(document);
                    }
                }
            }

            this.ControllerState = docGroup != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
            this.Group = docGroup;
            return docGroup;
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            string recordIdentification = row.RootRecordIdentification;
            UPMGroup group = this.GroupFromRow(row);
            group?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(recordIdentification));
            return group;
        }
    }
}
