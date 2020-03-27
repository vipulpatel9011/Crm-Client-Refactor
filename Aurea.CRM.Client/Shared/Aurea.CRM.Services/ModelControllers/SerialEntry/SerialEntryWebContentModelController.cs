// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialEntryWebContentModelController.cs" company="Aurea Software Gmbh">
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
//   The SerialEntryWebContentModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.ModelControllers.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using Aurea.CRM.UIModel.Web;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Enum Serial Entry State
    /// </summary>
    public enum UPMSerialEntryState
    {
        /// <summary>
        /// Approved
        /// </summary>
        Approved,

        /// <summary>
        /// Not approved
        /// </summary>
        NotApproved,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }

    /// <summary>
    /// SerialEntryWebContentModelController
    /// </summary>
    /// <seealso cref="UPWebContentPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class SerialEntryWebContentModelController : UPWebContentPageModelController, UPOfflineRequestDelegate,
        ISearchOperationHandler, UPCRMLinkReaderDelegate
    {
        private ViewReference buttonViewReference;
        private UPOfflineRequest buttonRequest;
        private UPContainerMetaInfo currentQuery;
        private UPContainerMetaInfo emailFilterQuery;
        private UPOfflineMultiRequest multiRequest;
        private List<IIdentifier> unreportedRecordChanges;
        private bool disable85106;
        private bool disableSigning;
        private UPCRMLinkReader parentLinkReader;
        private string parentLinkRecordIdentification;
        private byte[] pdfData;
        private List<UPCRMRecord> templateFilterLinkRecords;
        private UPCRMLinkReader templateFilterLinkReader;
        private List<UPCRMRecord> recordsToSave;
        private List<UPCRMRecord> openChildRecords;
        private UPCRMRecord currentCheckedLinkRecord;
        private UPConfigFilter sendByEmailFilter;
        private object approveActionDelegate;
        private string reportFileName;
        private UPOfflineUploadDocumentRequest uploadDocumentRequest;
        private bool shouldWaitForPendingChanges;
        private UPContainerMetaInfo documentQuery;

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryWebContentModelController"/> class.
        /// </summary>
        /// <param name="_viewReference">The view reference.</param>
        public SerialEntryWebContentModelController(ViewReference _viewReference)
            : this(_viewReference, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialEntryWebContentModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="disableSigning">if set to <c>true</c> [disable signing].</param>
        public SerialEntryWebContentModelController(ViewReference viewReference, bool disableSigning)
            : base(viewReference)
        {
            this.disableSigning = disableSigning;
            string buttonName = viewReference.ContextValueForKey("ButtonName");
            string sendByEmailFilterName = viewReference.ContextValueForKey("SendByEmailFilter");
            if (!string.IsNullOrEmpty(sendByEmailFilterName))
            {
                this.sendByEmailFilter = ConfigurationUnitStore.DefaultStore.FilterByName(sendByEmailFilterName);
                if (this.sendByEmailFilter == null)
                {
                    this.Logger.LogError($"Filter {this.sendByEmailFilter} not found");
                    //DDLogError("Filter \"%@\" not found", sendByEmailFilterName);
                }
                else
                {
                    this.SendByEmailButtonIsShown = false;
                }
            }

            UPConfigButton button = null;
            if (!this.disableSigning && !string.IsNullOrEmpty(buttonName))
            {
                button = ConfigurationUnitStore.DefaultStore.ButtonByName(buttonName);
            }

            if (button != null)
            {
                this.buttonViewReference = button.ViewReference.ViewReferenceWith(viewReference.ContextValueForKey("RecordId"));
                if (this.buttonViewReference.ViewName != "Action:modifyRecord")
                {
                    button = null;
                }
            }

            if (button != null)
            {
                UPMAction action = new UPMAction(StringIdentifier.IdentifierWithStringId("Action.Approve"));
                action.LabelText = button.Label;
                action.SetTargetAction(this, this.Approve);
                this.ApproveAction = action;
            }

            this.UnknownApprovedStateText = "...";
            this.SerialEntryApproved = UPMSerialEntryState.Unknown;
            this.RecordIdentification = viewReference.ContextValueForKey("RecordId");
        }

        /// <summary>
        /// Gets or sets a value indicating whether [should wait for pending changes].
        /// </summary>
        /// <value>
        /// <c>true</c> if [should wait for pending changes]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldWaitForPendingChanges
        {
            get
            {
                return this.shouldWaitForPendingChanges;
            }

            set
            {
                if (this.shouldWaitForPendingChanges == value)
                {
                    return;
                }

                this.shouldWaitForPendingChanges = value;
                if (this.ShouldWaitForPendingChanges)
                {
                    if (this.Page.Status == null)
                    {
                        UPMProgressStatus webViewLoadingStatus = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("webViewLoadingId"));
                        UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
                        statusField.FieldValue = LocalizedString.TextMessageLoadingReport;
                        webViewLoadingStatus.StatusMessageField = statusField;
                        this.Page.Status = webViewLoadingStatus;
                        if (this.ModelControllerDelegate != null)
                        {
                            this.ForcePageUpdate(new List<IIdentifier> { this.Page.Identifier });
                        }
                    }
                }
                else
                {
                    this.pageRequested = false;
                    this.Page.Status = null;
                    this.ForcePageUpdate(new List<IIdentifier> { this.Page.Identifier });
                }
            }
        }

        /// <summary>
        /// Gets or sets the approve action.
        /// </summary>
        /// <value>
        /// The approve action.
        /// </value>
        public UPMAction ApproveAction { get; set; }

        /// <summary>
        /// Gets or sets the serial entry approved.
        /// </summary>
        /// <value>
        /// The serial entry approved.
        /// </value>
        public UPMSerialEntryState SerialEntryApproved { get; set; }

        /// <summary>
        /// Gets or sets the approved text.
        /// </summary>
        /// <value>
        /// The approved text.
        /// </value>
        public string ApprovedText { get; set; }

        /// <summary>
        /// Gets or sets the unknown approved state text.
        /// </summary>
        /// <value>
        /// The unknown approved state text.
        /// </value>
        public string UnknownApprovedStateText { get; set; }

        /// <summary>
        /// Gets the local URL of report PDF.
        /// </summary>
        /// <value>
        /// The local URL of report PDF.
        /// </value>
        public string LocalUrlOfReportPdf { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable signing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable signing]; otherwise, <c>false</c>.
        /// </value>
        public override bool DisableSigning => this.disableSigning;

        /// <summary>
        /// Builds the client report for page cached.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="cached">if set to <c>true</c> [cached].</param>
        /// <returns></returns>
        public override bool BuildClientReportForPage(UPMWebContentPage page, bool cached = false)
        {
            if (cached || !this.UpdateReportButtonState())
            {
                return base.BuildClientReportForPage(page, cached);
            }

            return false;
        }

        /// <summary>
        /// Performs the upload report PDF sender.
        /// </summary>
        /// <param name="viewTag">The view tag.</param>
        /// <param name="sender">The sender.</param>
        public void PerformUploadReportPdfSender(int viewTag, object sender)
        {
            if (this.ApproveAction == null)
            {
                this.PerformUploadReportPdf(viewTag, sender);
                return;
            }

            if (this.templateFilterLinkReader != null || (this.parentLinkReader != null && this.parentLinkRecordIdentification == null))
            {
                return;
            }

            this.disable85106 = false;
            this.pdfData = this.CreatePdfData(viewTag);
            this.approveActionDelegate = sender;
            this.ContinueUploadReportPdf();
        }

        private bool UpdateReportButtonState()
        {
            string filterName = this.ViewReference.ContextValueForKey("ButtonShowFilter");
            UPConfigButton button = null;
            string buttonName = this.ViewReference.ContextValueForKey("ButtonName");
            if (!string.IsNullOrEmpty(buttonName))
            {
                button = ConfigurationUnitStore.DefaultStore.ButtonByName(buttonName);
            }

            UPConfigFilter buttonFilter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            if (buttonFilter != null || this.sendByEmailFilter != null)
            {
                this.shouldWaitForPendingChanges = true;
                if (buttonFilter != null)
                {
                    this.ApprovedText = buttonFilter.DisplayName;
                    if (string.IsNullOrEmpty(button?.Label))
                    {
                        this.ApprovedText = LocalizedString.TextProcessOrderWasApproved;
                    }

                    this.currentQuery = new UPContainerMetaInfo(new List<UPCRMField>(), buttonFilter.InfoAreaId);
                    this.currentQuery.ApplyFilter(buttonFilter);
                    this.currentQuery.SetLinkRecordIdentification(this.RecordIdentification);
                }
                else
                {
                    this.SerialEntryApproved = UPMSerialEntryState.NotApproved;
                }

                if (this.sendByEmailFilter != null)
                {
                    this.emailFilterQuery = new UPContainerMetaInfo(new List<UPCRMField>(), this.sendByEmailFilter.InfoAreaId);
                    this.emailFilterQuery.ApplyFilter(this.sendByEmailFilter);
                    this.emailFilterQuery.SetLinkRecordIdentification(this.RecordIdentification);
                }

                UPContainerMetaInfo queryToExecute = this.currentQuery ?? this.emailFilterQuery;
                queryToExecute.Find(UPRequestOption.FastestAvailable, this);
                return true;
            }

            this.SerialEntryApproved = UPMSerialEntryState.NotApproved;
            return false;
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        protected override void BuildPage()
        {
            ViewReference documentUploadViewReference = this.DetermineDocumentUploadViewReference();
            if (documentUploadViewReference != null)
            {
                int fieldId = -1;
                fieldId = this.DetermineFieldIdDocumentUploadViewReference(fieldId, documentUploadViewReference);
                if (fieldId >= 0)
                {
                    string infoAreaIdForQuery = this.ViewReference.ContextValueForKey("RecordId").InfoAreaId();
                    UPCRMField field = new UPCRMField(fieldId, infoAreaIdForQuery);
                    this.documentQuery = new UPContainerMetaInfo(new List<UPCRMField> { field }, infoAreaIdForQuery);
                    this.documentQuery.SetLinkRecordIdentification(this.RecordIdentification);
                    string filterName = documentUploadViewReference.ContextValueForKey("FilterName");
                    if (!string.IsNullOrEmpty(filterName))
                    {
                        UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
                        if (filter != null)
                        {
                            this.documentQuery.ApplyFilter(filter);
                        }
                    }

                    if (this.documentQuery.Find(UPRequestOption.FastestAvailable, this) != null)
                    {
                        return;
                    }
                }
            }

            base.BuildPage();
        }

        /// <summary>
        /// Specials the handling for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected override Page SpecialHandlingForPage(UPMWebContentPage page)
        {
            if (this.LocalUrlOfReportPdf != null)
            {
                page.Invalid = false;
                this.TopLevelElement = page;
                this.UpdateReportButtonState();
                return page;
            }

            return null;
        }

        private void ContinueUploadReportPdf()
        {
            string recordIdentification = this.RecordIdentification;
            int fieldId = -1;
            ViewReference documentUploadViewReference = this.DetermineDocumentUploadViewReference();
            if (documentUploadViewReference != null)
            {
                if (this.parentLinkReader != null)
                {
                    recordIdentification = this.parentLinkRecordIdentification;
                    this.parentLinkReader = null;
                }
                else
                {
                    recordIdentification = documentUploadViewReference.ContextValueForKey("RecordId");
                    if (string.IsNullOrEmpty(recordIdentification))
                    {
                        recordIdentification = this.RecordIdentification;
                    }

                    string parentLinkString = documentUploadViewReference.ContextValueForKey("ParentLink");
                    if (!string.IsNullOrEmpty(parentLinkString))
                    {
                        this.parentLinkReader = new UPCRMLinkReader(recordIdentification, parentLinkString, this);
                        if (this.parentLinkReader.Start())
                        {
                            return;
                        }

                        recordIdentification = this.parentLinkReader.RequestLinkRecordOffline();
                    }
                }

                fieldId = this.DetermineFieldIdDocumentUploadViewReference(fieldId, documentUploadViewReference);
            }

            this.reportFileName = this.CalcReportFileName();
            this.uploadDocumentRequest = new UPOfflineUploadDocumentRequest(this.pdfData, -1, this.reportFileName, "application/pdf", recordIdentification, fieldId);
            this.multiRequest = new UPOfflineMultiRequest(this);
            this.multiRequest.AddRequest(this.uploadDocumentRequest);
            recordIdentification = this.buttonViewReference.ContextValueForKey("RecordId");
            string filterName = this.buttonViewReference.ContextValueForKey("TemplateFilter");
            UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            filter = filter.FilterByApplyingValueDictionaryDefaults(this.FieldValueDictionary, true);
            if (!string.IsNullOrEmpty(recordIdentification) && filter != null)
            {
                UPCRMRecord record = new UPCRMRecord(recordIdentification);
                List<UPCRMRecord> combinedRecords = new List<UPCRMRecord> { record };
                List<UPCRMRecord> childRecords = record.ApplyValuesFromTemplateFilter(filter, false);
                foreach (UPCRMRecord childRecord in childRecords)
                {
                    if (childRecord.Mode == "ParentUpdate")
                    {
                        if (this.templateFilterLinkRecords == null)
                        {
                            this.templateFilterLinkRecords = new List<UPCRMRecord> { childRecord };
                        }
                        else
                        {
                            this.templateFilterLinkRecords.Add(childRecord);
                        }
                    }
                    else
                    {
                        combinedRecords.Add(childRecord);
                    }
                }

                if (this.templateFilterLinkRecords.Count > 0)
                {
                    this.recordsToSave = combinedRecords;
                    this.ContinueWithLinkRecords(this.templateFilterLinkRecords);
                    return;
                }

                this.PerformUploadReportPdfWithRecords(combinedRecords);
            }
            else
            {
                this.PerformUploadReportPdfWithRecords(null);
            }
        }

        private void ContinueWithLinkRecords(List<UPCRMRecord> linkRecords)
        {
            this.openChildRecords = null;
            string parentLinkString = null;
            string sourceRecordIdentification = null;
            foreach (UPCRMRecord linkRecord in linkRecords)
            {
                UPCRMLink link = linkRecord.SingleLink;
                if (link == null)
                {
                    continue;
                }

                UPCRMRecord childRecord = link.Record;
                if (childRecord.Mode == "ParentUpdate" || !string.IsNullOrEmpty(sourceRecordIdentification))
                {
                    if (this.openChildRecords == null)
                    {
                        this.openChildRecords = new List<UPCRMRecord> { childRecord };
                    }
                    else
                    {
                        this.openChildRecords.Add(childRecord);
                    }

                    continue;
                }

                if (!childRecord.RecordIdentification.IsRecordIdentification())
                {
                    continue;
                }

                sourceRecordIdentification = childRecord.RecordIdentification;
                this.currentCheckedLinkRecord = linkRecord;
                parentLinkString = link.LinkId > 0 ? $"{linkRecord.InfoAreaId}:{link.LinkId}" : linkRecord.InfoAreaId;
            }

            if (string.IsNullOrEmpty(parentLinkString) || string.IsNullOrEmpty(sourceRecordIdentification))
            {
                this.PerformUploadReportPdfWithRecords(this.recordsToSave);
                return;
            }

            this.templateFilterLinkReader = new UPCRMLinkReader(sourceRecordIdentification, parentLinkString, UPRequestOption.FastestAvailable, this);
            if (!this.templateFilterLinkReader.Start())
            {
                this.PerformUploadReportPdfWithRecords(this.recordsToSave);
            }
        }

        private void PerformUploadReportPdfWithRecords(List<UPCRMRecord> records)
        {
            if (records.Count > 0)
            {
                string requestOptionString = this.buttonViewReference.ContextValueForKey("RequestOption");
                UPOfflineRequestMode requestMode = UPOfflineRequest.RequestModeFromString(requestOptionString, UPOfflineRequestMode.OnlineConfirm);
                UPOfflineEditRecordRequest request = new UPOfflineEditRecordRequest(requestMode, null);
                this.buttonRequest = request;
                foreach (UPCRMRecord record in records)
                {
                    request.AddRecord(record);
                }

                this.multiRequest.AddRequest(request);
            }

            this.multiRequest.Start();
        }

        private void PerformUploadReportPdf(int viewTag, object sender)
        {
            this.disable85106 = true;
            byte[] data = this.CreatePdfData(viewTag);
            string recordIdentification = this.RecordIdentification;
            int fieldId = -1;
            ViewReference documentUploadViewReference = this.DetermineDocumentUploadViewReference();
            if (documentUploadViewReference != null)
            {
                recordIdentification = documentUploadViewReference.ContextValueForKey("RecordId");
                if (string.IsNullOrEmpty(recordIdentification))
                {
                    recordIdentification = this.RecordIdentification;
                }

                string parentLinkString = documentUploadViewReference.ContextValueForKey("ParentLink");
                if (!string.IsNullOrEmpty(parentLinkString))
                {
                    UPCRMLinkReader _linkReader = new UPCRMLinkReader(recordIdentification, parentLinkString, null);
                    recordIdentification = _linkReader.RequestLinkRecordOffline();
                }

                fieldId = this.DetermineFieldIdDocumentUploadViewReference(fieldId, documentUploadViewReference);
            }

            this.approveActionDelegate = sender;
            this.reportFileName = this.CalcReportFileName();
            this.uploadDocumentRequest = new UPOfflineUploadDocumentRequest(data, -1, this.reportFileName, "application/pdf", recordIdentification, fieldId);
            this.uploadDocumentRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, this);
        }

        private int DetermineFieldIdDocumentUploadViewReference(int fieldId, ViewReference documentUploadViewReference)
        {
            string configValue = documentUploadViewReference.ContextValueForKey("DocumentFieldFieldGroupName");
            if (!string.IsNullOrEmpty(configValue))
            {
                FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("Edit", configValue);
                if (fieldControl.NumberOfFields > 0)
                {
                    UPConfigFieldControlField field = fieldControl.FieldAtIndex(0);
                    if (field != null)
                    {
                        fieldId = field.FieldId;
                    }
                }
            }

            if (fieldId == -1)
            {
                configValue = documentUploadViewReference.ContextValueForKey("DocumentFieldId");
                if (!string.IsNullOrEmpty(configValue))
                {
                    fieldId = Convert.ToInt32(configValue);
                }
            }

            return fieldId;
        }

        private ViewReference DetermineDocumentUploadViewReference()
        {
            ViewReference documentUploadViewReference = null;
            if (this.disableSigning)
            {
                return null;
            }

            string documentUploadContextMenu = this.ViewReference.ContextValueForKey("DocumentUploadConfiguration");
            if (!string.IsNullOrEmpty(documentUploadContextMenu))
            {
                Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(documentUploadContextMenu);
                if (menu != null)
                {
                    documentUploadViewReference = menu.ViewReference.ViewReferenceWith(this.RecordIdentification);
                }
            }

            return documentUploadViewReference;
        }

        private string CalcReportFileName()
        {
            return null;
            //UPWebContentMetadataClientReport webContentMetadataReport = (UPWebContentMetadataClientReport)this.WebContentMetadata();
            //string fileName = webContentMetadataReport.PdfFileName;
            //string fileNameDateFormat = webContentMetadataReport.PdfFileNameDateFormat;
            //NSDateFormatter dateFormatter = new NSDateFormatter();
            //dateFormatter.DateFormat = fileNameDateFormat;
            //return this.ReplaceTokensDateFormatter(fileName, dateFormatter);
        }

        /// <summary>
        /// Calculates the signature titles.
        /// </summary>
        /// <returns></returns>
        public List<string> CalcSignatureTitles()
        {
            List<string> modTitles = new List<string>();
            UPWebContentMetadataClientReport metadataClientReport = (UPWebContentMetadataClientReport)this.WebContentMetadata;
            int signatureCount = metadataClientReport.SignatureCount;
            string fieldGroupName = metadataClientReport.SignatureTitle;
            if (!string.IsNullOrEmpty(fieldGroupName))
            {
                FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", fieldGroupName);
                string label = fieldControl?.FullLabelTextForFunctionName("SIGNATURETITLES");
                if (!string.IsNullOrEmpty(label))
                {
                    var titles = label.Split(';');
                    long count = Math.Min(titles.Length, signatureCount);
                    for (int i = 0; i < count; i++)
                    {
                        string title = titles[i];
                        title = this.ReplaceTokensDateFormatter(title, null);
                        modTitles[i] = title;
                    }
                }
            }

            int k = 0;
            if (signatureCount > 1)
            {
                for (k = 0; k < signatureCount; k++)
                {
                    modTitles[k] = $"{LocalizedString.TextProcessSignatureTitle} {k + 1}";
                }
            }
            else if (signatureCount == 1 && k == 0)
            {
                modTitles[0] = LocalizedString.TextProcessSignatureTitle;
            }

            return modTitles;
        }

        private string ReplaceTokensDateFormatter(string text, /*NSDateFormatter*/ object dateFormatter)
        {
            return null;
            //NSDictionary prefixedFieldValueDictionary = this.PrefixedFieldValueDictionary;
            //foreach (string key in prefixedFieldValueDictionary)
            //{
            //    string value = prefixedFieldValueDictionary.ObjectForKey(key);
            //    string keyString = $"{key}";
            //    text = text.Replace(keyString, value);
            //}

            //if (dateFormatter)
            //{
            //    NSRange range1;
            //    while ((range1 = text.RangeOfString("$Date(")).Location != NSNotFound)
            //    {
            //        NSRange range2 = text.SubstringFromIndex(range1.Location + range1.Length).RangeOfString(")");
            //        if (range2.Location != NSNotFound)
            //        {
            //            string dateString = text.SubstringWithRange(NSMakeRange(range1.Location + range1.Length, range2.Location));
            //            string token = $"$Date({dateString})";
            //            NSDate date = dateString.DateFromCrmValue();
            //            string replacement = date ? dateFormatter.StringFromDate(date) : dateString;
            //            text = text.Replace(token, replacement);
            //        }
            //    }
            //}

            //return text;
        }

        /// <summary>
        /// Approves the specified dummy.
        /// </summary>
        /// <param name="dummy">The dummy.</param>
        public void Approve(object dummy)
        {
            if (this.disableSigning)
            {
                return;
            }

            string recordIdentification = this.buttonViewReference.ContextValueForKey("RecordId");
            string filterName = this.buttonViewReference.ContextValueForKey("TemplateFilter");
            if (string.IsNullOrEmpty(recordIdentification))
            {
                this.ParentOrganizerModelController.HandleOrganizerActionError(
                    LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorParameterEmpty, "RecordId"), true);
                return;
            }

            UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            if (filter == null)
            {
                this.HandlePageErrorDetails(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorFilterMissing, filterName));
                return;
            }

            filter = filter.FilterByApplyingValueDictionaryDefaults(this.FieldValueDictionary, true);
            UPCRMRecord record = new UPCRMRecord(recordIdentification);
            record.ApplyValuesFromTemplateFilter(filter);
            UPOfflineEditRecordRequest request = new UPOfflineEditRecordRequest(0);
            this.buttonRequest = request;
            string requestOptionString = this.buttonViewReference.ContextValueForKey("RequestOption");
            UPOfflineRequestMode requestMode = UPOfflineRequest.RequestModeFromString(requestOptionString, UPOfflineRequestMode.OnlineConfirm);
            if (request.StartRequest(requestMode, new List<UPCRMRecord> { record }, this) == false)
            {
                this.buttonRequest = null;
            }
            else
            {
                this.ShouldWaitForPendingChanges = true;
            }
        }

        /// <summary>
        /// Emails the report as PDF action.
        /// </summary>
        /// <param name="_viewTag">The view tag.</param>
        /// <param name="action">The action.</param>
        public override void EmailReportAsPdfAction(int _viewTag, Menu action)
        {
#if PORTING
            if (!reportFileName && ((UPWebContentMetadataClientReport)this.WebContentMetadata).PdfFileName)
            {
                reportFileName = this.CalcReportFileName();
            }

            reportFileName = reportFileName ? reportFileName : "report.pdf";
            UPMail mail = UPMail.TheNew();
            if (action != null)
            {
                ViewReference sendEmailActionViewReference = action.ViewReference();
                sendEmailActionViewReference = sendEmailActionViewReference.ViewReferenceWithRecordIdentification(recordIdentification);
                bool attachReport = "true".CompareOptions(viewReference.ContextValueForKey("SendByEmailAttachReport"), NSCaseInsensitiveSearch) == NSOrderedSame;
                string fieldGroup = sendEmailActionViewReference.ContextValueForKey("EmailFieldgroup");
                string recordId = sendEmailActionViewReference.ContextValueForKey("RecordId");
                mail.FillFromEmailTemplateFieldGroupForRecord(fieldGroup, recordId);
                if (attachReport)
                {
                    NSData pdfDataOut = this.CreatePdfData(_viewTag);
                    mail.AddAttachment(new UPMailAttachment(pdfDataOut, "application/pdf", reportFileName));
                }

            }
            else
            {
                NSData pdfDataOut = this.CreatePdfData(_viewTag);
                string title = this.ParentOrganizerModelController.Organizer.TitleText;
                mail.Subject = NSString.StringWithFormat(upText_OrderReportEmailSubject, title);
                mail.AddAttachment(new UPMailAttachment(pdfDataOut, "application/pdf", reportFileName));
            }

            this.ModelControllerDelegate.SendMailModal(mail, true);
#endif
        }

        private void SetShouldWaitForPendingChangesWithoutPageUpdate(bool _shouldWaitForPendingChanges, bool withoutPageUpdate)
        {
            if (withoutPageUpdate)
            {
                this.shouldWaitForPendingChanges = _shouldWaitForPendingChanges;
                return;
            }

            this.ShouldWaitForPendingChanges = _shouldWaitForPendingChanges;
        }

        /// <summary>
        /// Updates the element for current changes.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public override void UpdateElementForCurrentChanges(List<IIdentifier> changes)
        {
            if (this.ShouldWaitForPendingChanges)
            {
                return;
            }

            Page page = this.TopLevelElement as Page;
            if (page != null && page.Invalid)
            {
                this.LocalUrlOfReportPdf = null;
                this.pageRequested = false;
                this.pageBuilt = false;
                this.SerialEntryApproved = UPMSerialEntryState.NotApproved;
            }

            base.UpdateElementForCurrentChanges(changes);
        }

        /// <summary>
        /// Offlines the request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            if (this.uploadDocumentRequest != null)
            {
                this.uploadDocumentRequest = null;
                if (this.disable85106)
                {
                    this.ApproveAction.PerformAction(this.approveActionDelegate);
                }

                this.pageBuilt = false;
                if (this.ParentOrganizerModelController is DetailOrganizerModelController)
                {
                    ((DetailOrganizerModelController)this.ParentOrganizerModelController).RefreshAfterDocumentUploaded(this.RecordIdentification);
                }

                if (!this.disable85106)
                {
                    this.ShouldWaitForPendingChanges = true;
                }
            }
            else
            {
                this.buttonRequest = null;
                this.pageBuilt = false;
                this.SerialEntryApproved = UPMSerialEntryState.Approved;
                if (!this.disable85106)
                {
                    this.ShouldWaitForPendingChanges = false;
                }
                else
                {
                    List<UPCRMRecord> records = (List<UPCRMRecord>)data;
                    if (records.Count > 0)
                    {
                        UPCRMRecord record = records[0];
                        RecordIdentifier recordIdentifier = new RecordIdentifier(record.RecordIdentification);
                        UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { recordIdentifier });
                        if (this.ParentOrganizerModelController is DetailOrganizerModelController)
                        {
                            List<IIdentifier> changes = UPChangeManager.CurrentChangeManager.ChangesToApplyForCurrentViewController();
                            if (this.sendByEmailFilter != null)
                            {
                                this.unreportedRecordChanges = changes;
                            }
                            else
                            {
                                this.ParentOrganizerModelController.ProcessChanges(changes);
                            }
                        }
                    }

                    if (this.sendByEmailFilter != null)
                    {
                        UPContainerMetaInfo _emailFilterQuery = new UPContainerMetaInfo(new List<UPCRMField>(), this.sendByEmailFilter.InfoAreaId);
                        this.emailFilterQuery = _emailFilterQuery;
                        _emailFilterQuery.ApplyFilter(this.sendByEmailFilter);
                        _emailFilterQuery.SetLinkRecordIdentification(this.RecordIdentification);
                        _emailFilterQuery.Find(UPRequestOption.FastestAvailable, this);
                    }
                    else
                    {
                        this.SetShouldWaitForPendingChangesWithoutPageUpdate(false, true);
                    }
                }
            }
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.uploadDocumentRequest = null;
            this.buttonRequest = null;
            this.SerialEntryApproved = UPMSerialEntryState.Unknown;
            this.HandlePageErrorDetails(error.Message, error.StackTrace);
            this.SetShouldWaitForPendingChangesWithoutPageUpdate(false, true);
            this.ForcePageUpdate(new List<IIdentifier> { this.Page.Identifier });
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.currentQuery = null;
            this.ShouldWaitForPendingChanges = false;
            this.SerialEntryApproved = UPMSerialEntryState.Unknown;
            this.HandlePageErrorDetails(error.Message, error.StackTrace);
            this.ForcePageUpdate(new List<IIdentifier> { this.Page.Identifier });
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (this.documentQuery != null && result.MetaInfo == this.documentQuery)
            {
                if (result.RowCount > 0)
                {
                    string documentKey = result.ResultRowAtIndex(0).RawValueAtIndex(0);
                    if (!string.IsNullOrEmpty(documentKey))
                    {
                        DocumentData documentData = new DocumentManager().DocumentForKey(documentKey);
                        if (documentData != null)
                        {
                            bool downloaded = false;
                            Uri downloadUrl = ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(documentData.RecordIdentification, documentData.Title);
                            ResourceManager resourceManager = SmartbookResourceManager.DefaultResourceManager;
                            Resource resource = resourceManager.ResourceForUrl(downloadUrl, documentData.Title);
                            if (resource == null && this.reportFileName != null && this.reportFileName != documentData.Title)
                            {
                                Uri alternateDownloadUrl = ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(documentData.RecordIdentification, this.reportFileName);
                                resource = resourceManager.ResourceForUrl(alternateDownloadUrl, this.reportFileName);
                                if (resource != null)
                                {
                                    UPMDocument document = new UPMDocument(documentData);
                                    resourceManager.QueueHighPriorityDownloadForResourceAtUrl(document.Url, document.LocalFileName, document.ModificationDate, true);
                                }
                            }

                            if (resource != null)
                            {
                                //NSFileManager fileManager = NSFileManager.DefaultManager();
                                //NSDictionary attributes = fileManager.AttributesOfItemAtPathError(resource.LocalUrl.Path, null);
                                //if (attributes.FileSize)
                                //{
                                //    downloaded = true;
                                //}
                                //else
                                //{
                                //    fileManager.RemoveItemAtPathError(resource.LocalUrl.Path, null);
                                //}
                            }

                            this.LocalUrlOfReportPdf = resource.LocalUrl;
                            if (!downloaded)
                            {
                                UPMDocument document = new UPMDocument(documentData);
                                ResourceDownload download = resourceManager.QueueHighPriorityDownloadForResourceAtUrl(document.Url, document.LocalFileName, document.ModificationDate, true);
                                this.LocalUrlOfReportPdf = download.LocalUrl;
                            }
                        }
                    }
                }

                this.documentQuery = null;
                Page oldPage = this.Page;
                base.BuildPage();
                if (this.Page != oldPage)
                {
                    base.UpdateElementForCurrentChanges(null);
                }

                return;
            }

            if (result.MetaInfo == this.currentQuery)
            {
                if (result.RowCount >= 1)
                {
                    this.SerialEntryApproved = UPMSerialEntryState.Approved;
                    ((UPWebContentMetadataClientReport)this.WebContentMetadata).ClearSignature();
                }
                else
                {
                    this.SerialEntryApproved = UPMSerialEntryState.NotApproved;
                }

                this.currentQuery = null;
            }

            if (result.MetaInfo == this.emailFilterQuery)
            {
                this.SendByEmailButtonIsShown = result.RowCount >= 1;
                this.emailFilterQuery = null;
            }

            if (this.emailFilterQuery == null)
            {
                this.shouldWaitForPendingChanges = false;
                if (this.unreportedRecordChanges != null)
                {
                    List<IIdentifier> changedRecords = this.unreportedRecordChanges;
                    this.unreportedRecordChanges = null;
                    this.ParentOrganizerModelController.ProcessChanges(changedRecords);
                }
                else
                {
                    base.BuildClientReportForPage((UPMWebContentPage)this.Page, false);
                }
            }
            else
            {
                this.emailFilterQuery.Find(UPRequestOption.FastestAvailable, this);
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

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public override void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            if (_linkReader == this.templateFilterLinkReader)
            {
                this.templateFilterLinkReader = null;
                if (_linkReader.DestinationRecordIdentification.IsRecordIdentification())
                {
                    this.currentCheckedLinkRecord.UpdateRecordInformationWithRecordIdentification(_linkReader.DestinationRecordIdentification, true);
                    this.recordsToSave.Add(this.currentCheckedLinkRecord);
                    this.currentCheckedLinkRecord = null;
                }

                if (this.openChildRecords != null)
                {
                    this.ContinueWithLinkRecords(this.openChildRecords);
                }
                else
                {
                    this.PerformUploadReportPdfWithRecords(this.recordsToSave);
                }
            }
            else if (_linkReader == this.parentLinkReader)
            {
                this.parentLinkRecordIdentification = _linkReader.DestinationRecordIdentification ?? string.Empty;

                this.ContinueUploadReportPdf();
            }
            else
            {
                base.LinkReaderDidFinishWithResult(_linkReader, result);
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public override void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            if (_linkReader == this.templateFilterLinkReader)
            {
                this.templateFilterLinkReader = null;
                this.ReportError(error, true);
            }
            else if (_linkReader == this.parentLinkReader)
            {
                this.parentLinkReader = null;
                this.ReportError(error, true);
            }
            else
            {
                base.LinkReaderDidFinishWithError(_linkReader, error);
            }
        }

        private byte[] CreatePdfData(int viewTag)
        {
            return this.ModelControllerDelegate.CreatePdfData(viewTag);
        }
    }
}
