// <copyright file="DocumentGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Document Group Model Controller
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
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// The Document Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPDocumentGroupModelController : UPFieldControlBasedGroupModelController, ISearchOperationHandler
    {
        private FieldControl fieldControl;
        private SearchAndList searchAndList;
        private string _sendEmailFieldgroup;
        private string _linkedRecordId;

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UPDocumentGroupModelController"/> class.
        /// </summary>
        /// <param name="_fieldControl">The field control.</param>
        /// <param name="_tabIndex">Index of the tab.</param>
        /// <param name="_theDelegate">The delegate.</param>
        public UPDocumentGroupModelController(FieldControl _fieldControl, int _tabIndex, IGroupModelControllerDelegate _theDelegate)
            : base(_fieldControl, _tabIndex, _theDelegate)
        {
            var typeParts = this.TabConfig.Type.Split('_');
            this.MaxResults = 100;
            if (typeParts.Length > 1)
            {
                bool docSearch = typeParts[0].StartsWith("DOCSEARCH");
                if (docSearch)
                {
                    IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                    if (typeParts.Length > 1)
                    {
                        string searchAndListName = (string)typeParts[1];
                        this.searchAndList = configStore.SearchAndListByName(searchAndListName);
                    }

                    if (typeParts.Length > 2)
                    {
                        this.MaxResults = Convert.ToInt32(typeParts[2]);
                    }
                }
                else
                {
                    if (typeParts.Length > 1)
                    {
                        this.MaxResults = Convert.ToInt32(typeParts[1]);
                    }
                }

                this.ControllerState = GroupModelControllerState.Pending;
                string sendEmailFieldgroup = _fieldControl.ValueForAttribute("documentClientEmailFieldgroup");
                if (sendEmailFieldgroup != null)
                {
                    this._sendEmailFieldgroup = sendEmailFieldgroup;
                }
            }
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the maximum results.
        /// </summary>
        /// <value>
        /// The maximum results.
        /// </value>
        public int MaxResults { get; private set; }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            this.RecordIdentification = row.RootRecordIdentification;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string infoAreaId = this.RecordIdentification.InfoAreaId();
            this.fieldControl = null;
            UPContainerMetaInfo containerMetaInfo;
            if (this.searchAndList != null)
            {
                containerMetaInfo = new UPContainerMetaInfo(this.searchAndList, null);
                this.fieldControl = containerMetaInfo.SourceFieldControl;
            }
            else
            {
                if (infoAreaId == "D1")
                {
                    this.fieldControl = configStore.FieldControlByNameFromGroup("List", "D1DocData");
                }

                if (this.fieldControl == null)
                {
                    this.fieldControl = configStore.FieldControlByNameFromGroup("List", "D3DocData");
                }

                if (this.fieldControl == null)
                {
                    this.ControllerState = GroupModelControllerState.Error;
                    this.Group = null;
                    this.Error = new Exception("configuration error");
                    return null;
                }

                containerMetaInfo = new UPContainerMetaInfo(this.fieldControl);
            }

            if (infoAreaId == "D1" || infoAreaId == "D3")
            {
                containerMetaInfo.SetLinkRecordIdentification(this.RecordIdentification);
            }
            else
            {
                containerMetaInfo.SetLinkRecordIdentification(this.RecordIdentification, 126, 127);
            }

            if (this.MaxResults > 0)
            {
                containerMetaInfo.MaxResults = this.MaxResults;
            }

            if (this.RequestOption == UPRequestOption.FastestAvailable || this.RequestOption == UPRequestOption.Offline)
            {
                UPCRMResult result = containerMetaInfo.Find();
                if (result != null)
                {
                    UPMGroup group = this.GroupFromResult(result);
                    group?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(this.RecordIdentification));
                    return group;
                }
            }

            if (this.RequestOption != UPRequestOption.Offline)
            {
                containerMetaInfo.Find(this.RequestOption, this);
                this.ControllerState = GroupModelControllerState.Pending;
                return null;
            }

            base.ControllerState = GroupModelControllerState.Empty;
            base.Group = null;
            return null;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Error = error;
            this.Group = null;
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
                this.ControllerState = GroupModelControllerState.Finished;
                base.Group = this.GroupFromResult(result);
                this.ControllerState = base.Group != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
                this.Delegate.GroupModelControllerFinished(this);
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Empty;
                this.Group = null;
                this.Delegate.GroupModelControllerFinished(this);
            }
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

        private bool FillGroupWithLocalDocumentsResult(UPMDocumentsGroup group, UPCRMResult result)
        {
            if (this._sendEmailFieldgroup != null)
            {
                this._linkedRecordId = this.RecordIdentification;
            }

            int count = result.RowCount;
            if (count == 0)
            {
                return false;
            }

            if (this.MaxResults > 0 && count > this.MaxResults)
            {
                count = this.MaxResults;
            }

            DocumentInfoAreaManager documentInfoAreaManager;
            if (this.fieldControl.InfoAreaId == "D3" && this.TabConfig.Type.StartsWith("D1"))
            {
                documentInfoAreaManager = new DocumentInfoAreaManager(this.fieldControl.InfoAreaId, this.fieldControl, 1, null);
            }
            else
            {
                documentInfoAreaManager = new DocumentInfoAreaManager(this.fieldControl.InfoAreaId, this.fieldControl, null);
            }

            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(i);
                DocumentData documentData = documentInfoAreaManager.DocumentDataForResultRow(resultRow);
                UPMDocument document = new UPMDocument(documentData);
                if (this._sendEmailFieldgroup != null)
                {
                    document.LinkedRecordId = this._linkedRecordId;
                    document.EmailFieldgroup = this._sendEmailFieldgroup;
                }

                if (this.IsDocumentIncludedInGroup(document, group))
                {
                    group.AddField(document);
                }
            }

            if (group.Fields.Count == 0)
            {
                return false;
            }

            return true;
        }

        private bool IsDocumentIncludedInGroup(UPMDocument document, UPMDocumentsGroup group)
        {
            var imageExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".bmp" };
            switch (group.Style)
            {
                case UPMDocumentsGroupStyle.Image:
                case UPMDocumentsGroupStyle.NoImages:
                    bool withImage = group.Style == UPMDocumentsGroupStyle.Image;
                    string lowerFname = document.LocalFileName.ToLower();
                    return imageExtensions.Any(extension => lowerFname.EndsWith(extension)) ? withImage : !withImage;

                default:
                    return true;
            }
        }

        private UPMGroup GroupFromResult(UPCRMResult result)
        {
            UPMDocumentsGroup docGroup = new UPMDocumentsGroup(this.TabIdentifierForRecordIdentification(this.RecordIdentification));
            docGroup.LabelText = this.TabLabel;
            int location = this.TabConfig.Type.IndexOf(".");
            if (location != -1)
            {
                string style = this.TabConfig.Type.Substring(location + 1);
                if (style.StartsWith("IMG"))
                {
                    docGroup.Style = UPMDocumentsGroupStyle.Image;
                }
                else if (style.StartsWith("NOIMG"))
                {
                    docGroup.Style = UPMDocumentsGroupStyle.NoImages;
                }
                else
                {
                    Logger.LogWarn($"Unknown style for DOC group: {style}");
                }
            }

            if (this.FillGroupWithLocalDocumentsResult(docGroup, result))
            {
                this.Group = docGroup;
                this.ControllerState = GroupModelControllerState.Finished;
                return docGroup;
            }

            this.Group = null;
            this.ControllerState = GroupModelControllerState.Empty;
            return null;
        }
    }
}
