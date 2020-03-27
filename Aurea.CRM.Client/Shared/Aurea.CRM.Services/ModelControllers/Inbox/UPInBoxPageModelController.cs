// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPInBoxPageModelController.cs" company="Aurea Software Gmbh">
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
//   The InBox Page Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.ModelControllers.Inbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Inbox Page Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    public class UPInBoxPageModelController : UPPageModelController, UPOfflineRequestDelegate
    {
        private UPOfflineUploadDocumentRequest docUploadRequest;
        private UPOfflineEditRecordRequest editRecordRequest;
        private Dictionary<StringIdentifier, UPEditFieldContext> editFieldContexts;
        private string recordIdentification;
        private bool removeUploadedFileFromInbox;
        private UPMInboxFile uploadingFile;
        private UPOfflineMultiRequest multiRequest;

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInBoxPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPInBoxPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            UPMInBoxPage page = new UPMInBoxPage(StringIdentifier.IdentifierWithStringId("Inbox"))
            {
                Invalid = true,
                SkipUploadIfPossible = viewReference.ContextValueIsSet("SkipUploadPageIfPossible")
            };
            this.removeUploadedFileFromInbox = viewReference.ContextValueIsSet("RemoveUploadedFile");
            this.TopLevelElement = page;
            this.editFieldContexts = new Dictionary<StringIdentifier, UPEditFieldContext>();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            FieldControl editFieldControl = configStore.FieldControlByNameFromGroup("Edit", viewReference.ContextValueForKey("UploadFields"));
            UPMGroup uploadFieldGroup = new UPMGroup(StringIdentifier.IdentifierWithStringId("uploadFieldGroup"));
            if (editFieldControl != null)
            {
                int numberOfFields = editFieldControl.NumberOfFields;
                for (int index = 0; index < numberOfFields; index++)
                {
                    UPConfigFieldControlField field = editFieldControl.FieldAtIndex(index);
                    StringIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId($"Field {field.FieldId} {field.InfoAreaId}");
                    UPEditFieldContext initialValueEditField = UPEditFieldContext.FieldContextFor(field, fieldIdentifier, null, (List<UPEditFieldContext>)null);
                    this.editFieldContexts[fieldIdentifier] = initialValueEditField;
                    if (field.Function == "Filename")
                    {
                        page.FileNameEditField = initialValueEditField.EditField;
                    }

                    uploadFieldGroup.AddField(initialValueEditField.EditField);
                }
            }

            this.recordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            page.UploadFieldGroup = uploadFieldGroup; // load directly
            this.UpdatedElement(page);
        }

        /// <summary>
        /// Gets the inbox page.
        /// </summary>
        /// <value>
        /// The inbox page.
        /// </value>
        public UPMInBoxPage InboxPage => (UPMInBoxPage)this.Page;

        /// <summary>
        /// Views the will appear.
        /// </summary>
        public override void ViewWillAppear()
        {
            Messenger.Default.Register<InboxFileManagerMessage>(this, InboxFileManagerMessage.FileRemovedFromInboxMessageKey, this.InboxChanged);
            Messenger.Default.Register<InboxFileManagerMessage>(this, InboxFileManagerMessage.FileAddedToInboxMessageKey, this.InboxChanged);
        }

        /// <summary>
        /// Views the will disappear.
        /// </summary>
        public override void ViewWillDisappear()
        {
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void UploadFile(UPMInboxFile file)
        {
            this.uploadingFile = file;
            byte[] data = null; //NSData.DataWithContentsOfURL(file.URL);
            string fileName = file.Name;
            foreach (UPEditFieldContext changedField in this.editFieldContexts.Values)
            {
                if (changedField.FieldConfig.Function == "Filename" && !string.IsNullOrEmpty(changedField.Value))
                {
                    fileName = changedField.Value;
                    // New filename has no extension. Add original extension
                    //if (fileName.PathExtension().Length == 0)
                    //{
                    //    fileName = $"{fileName}.{file.Path.PathExtension()}";
                    //}

                    break;
                }
            }

            this.docUploadRequest = new UPOfflineUploadDocumentRequest(data, -1, fileName, file.MimeTye, this.recordIdentification, -1);
            List<UPCRMRecord> docUploadRecords = this.docUploadRequest.CreateOfflineRecords();
            string d1RecordId = docUploadRecords[0].RecordId;
            string d3RecordId = docUploadRecords[1].RecordId;
            UPCRMRecord d1Record = new UPCRMRecord(d1RecordId, "Update");
            UPCRMRecord d3Record = new UPCRMRecord(d3RecordId, "Update");
            this.editRecordRequest = new UPOfflineEditRecordRequest(UPOfflineRequestMode.OnlineConfirm, this.ViewReference);
            foreach (UPEditFieldContext changedField in this.editFieldContexts.Values)
            {
                UPCRMRecord record = changedField.FieldConfig.InfoAreaId == "D1" ? d1Record : d3Record;
                if (changedField.FieldConfig.Function == "Filename")
                {
                    continue;
                }

                UPCRMFieldValue fieldValue = record.NewValueFieldId(changedField.Value, changedField.FieldId);
                if (!string.IsNullOrEmpty(changedField.DateOriginalValue))
                {
                    fieldValue.DateOriginalValue = changedField.DateOriginalValue;
                }
                else if (!string.IsNullOrEmpty(changedField.TimeOriginalValue))
                {
                    fieldValue.TimeOriginalValue = changedField.TimeOriginalValue;
                }
            }

            this.editRecordRequest.TitleLine = "FieldUploadFields";
            this.editRecordRequest.DetailsLine = "";
            this.multiRequest = new UPOfflineMultiRequest(this);
            this.multiRequest.AddRequest(this.docUploadRequest);
            this.multiRequest.AddRequest(this.editRecordRequest);
            this.multiRequest.Start();
        }

        void Dealloc()
        {
            Messenger.Default.Unregister(this);
        }

        private void InboxChanged(InboxFileManagerMessage notfication)
        {
            this.UpdatedElement(this.Page);
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            UPMInBoxPage oldPage = (UPMInBoxPage)this.TopLevelElement;
            UPMInBoxPage page = new UPMInBoxPage(StringIdentifier.IdentifierWithStringId("Inbox"))
            {
                UploadFieldGroup = oldPage.UploadFieldGroup,
                FileNameEditField = oldPage.FileNameEditField,
                SkipUploadIfPossible = oldPage.SkipUploadIfPossible
            };

            UPInboxFileManager manager = UPInboxFileManager.CurrentInboxFileManager();
            List<UPInboxFile> files = manager.AllInboxFiles();
            foreach (UPInboxFile inboxFile in files)
            {
                UPMInboxFile upmInboxFile = ModelInboxFileFromInboxFile(inboxFile);
                page.AddChild(upmInboxFile);
            }

            this.TopLevelElement = page;
            this.SortFilesByDate();
            page.Invalid = false;
            this.InformAboutDidChangeTopLevelElement(page, page, null, null);
            return element;
        }

        /// <summary>
        /// Sorts the files by date.
        /// </summary>
        public void SortFilesByDate()
        {
            UPMInBoxPage page = this.InboxPage;
            List<UPMElement> sortedArray = page.Children.OrderBy(x => ((UPMInboxFile)x).Date).ToList();
            page.RemoveAllChildren();
            foreach (UPMElement element in sortedArray)
            {
                page.AddChild(element);
            }
        }

        /// <summary>
        /// Sorts the name of the files by file.
        /// </summary>
        public void SortFilesByFileName()
        {
            UPMInBoxPage page = this.InboxPage;
            List<UPMElement> sortedArray = page.Children.OrderBy(x => ((UPMInboxFile)x).Name).ToList();
            page.RemoveAllChildren();
            foreach (UPMElement element in sortedArray)
            {
                page.AddChild(element);
            }
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="deleteFiles">The delete files.</param>
        public void DeleteFiles(List<UPMInboxFile> deleteFiles)
        {
            foreach (UPMInboxFile file in deleteFiles)
            {
                UPInboxFileManager.DeleteFileWithURL(file.URL);
            }

            this.UpdatedElement(this.Page);
        }

        /// <summary>
        /// Inboxes the file from URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static UPMInboxFile InboxFileFromURL(Uri url)
        {
            UPInboxFile inboxFile = UPInboxFileManager.InboxFileFromURL(url);
            return ModelInboxFileFromInboxFile(inboxFile);
        }

        private static UPMInboxFile ModelInboxFileFromInboxFile(UPInboxFile inboxFile)
        {
            UPMInboxFile upmInboxFile = new UPMInboxFile(StringIdentifier.IdentifierWithStringId(inboxFile.Path))
            {
                Path = inboxFile.Path,
                URL = inboxFile.URL
            };

#if PORTING
            upmInboxFile.Name = inboxFile.Path.LastPathComponent();
            Exception attributesErr;
            NSDictionary fileAttributes = NSFileManager.DefaultManager().AttributesOfItemAtPathError(inboxFile.Path, attributesErr);
            if (attributesErr)
            {
                DDLogError("UPInBoxPageModelController error no fileAttributes: %@", fileAttributes);
            }

            NSNumber fileSizeNumber = fileAttributes.ObjectForKey(NSFileSize);
            upmInboxFile.Size = fileSizeNumber.LongLongValue();
            upmInboxFile.FormattedSize = NSByteCountFormatter.StringFromByteCountCountStyle(upmInboxFile.Size, NSByteCountFormatterCountStyleFile);
            upmInboxFile.Date = fileAttributes.ObjectForKey(NSFileCreationDate);
            // Cant use TimeZone here. Called before Login and server independent.
            upmInboxFile.FormattedDate = UPInBoxPageModelController.UpDateFormatter().StringFromDate(upmInboxFile.Date);
            UIDocumentInteractionController interactionController = UIDocumentInteractionController.InteractionControllerWithURL(inboxFile.URL);
            ArrayList icons = interactionController.Icons;
            //upmInboxFile.Icon = icons.LastObject();  // Adding largest Icon available
#endif
            upmInboxFile.MimeTye = UPInboxFileManager.MimeTypeForPath(upmInboxFile.Path);
            upmInboxFile.Color = UPInBoxPageModelController.ColorForMimeType(upmInboxFile.MimeTye);
            return upmInboxFile;
        }

#if PORTING
        static NSDateFormatter UpDateFormatter()
        {
            static NSDateFormatter dateFormatter;
            dateFormatter = NSDateFormatter.TheNew();
            dateFormatter.SetDateStyle(NSDateFormatterMediumStyle);
            dateFormatter.SetTimeStyle(NSDateFormatterNoStyle);
            return dateFormatter;
        }
#endif

        private static readonly AureaColor docsColor = AureaColor.RedGreenBlueAlpha(51.0 / 255.0, 77.0 / 255.0, 92.0 / 255.0, 1.0);
        private static readonly AureaColor xlsColor = AureaColor.RedGreenBlueAlpha(69.0 / 255.0, 178.0 / 255.0, 157.0 / 255.0, 1.0);
        private static readonly AureaColor pdfColor = AureaColor.RedGreenBlueAlpha(223.0 / 255.0, 73.0 / 255.0, 73.0 / 255.0, 1.0);
        private static readonly AureaColor pptColor = AureaColor.RedGreenBlueAlpha(226.0 / 255.0, 122.0 / 255.0, 63.0 / 255.0, 1.0);
        private static readonly AureaColor otherColor = AureaColor.RedGreenBlueAlpha(239.0 / 255.0, 201.0 / 255.0, 76.0 / 255.0, 1.0);

        private static readonly List<string> docMimeTypes = new List<string>
            {
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
                "application/vnd.ms-word.document.macroEnabled.12",
                "application/vnd.ms-word.template.macroEnabled.12",
                "application/x-iwork-pages-sffpages"
            };

        private static readonly List<string> xlsMimeTypes = new List<string>
            {
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
                "application/vnd.ms-excel.sheet.macroEnabled.12",
                "application/vnd.ms-excel.addin.macroEnabled.12",
                "application/vnd.ms-excel.sheet.binary.macroEnabled.12",
                "application/x-iwork-numbers-sffnumbers"
            };

        private static readonly List<string> pdfMimeTypes = new List<string> { "application/pdf", "application/x-pdf" };

        private static readonly List<string> pptMimeTypes = new List<string>
            {
                "application/vnd.ms-powerpoint",
                "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "application/vnd.openxmlformats-officedocument.presentationml.template",
                "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                "application/vnd.ms-powerpoint.addin.macroEnabled.12",
                "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
                "application/vnd.ms-powerpoint.template.macroEnabled.12",
                "application/vnd.ms-powerpoint.slideshow.macroEnabled.12",
                "application/x-iwork-keynote-sffkey"
            };

        private static AureaColor ColorForMimeType(string mimeType)
        {
            if (docMimeTypes.Contains(mimeType))
            {
                return docsColor;
            }

            if (xlsMimeTypes.Contains(mimeType))
            {
                return xlsColor;
            }

            if (pdfMimeTypes.Contains(mimeType))
            {
                return pdfColor;
            }

            return pptMimeTypes.Contains(mimeType) ? pptColor : otherColor;
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            if (this.removeUploadedFileFromInbox)
            {
                this.DeleteFiles(new List<UPMInboxFile> { this.uploadingFile });
            }

            if (!string.IsNullOrEmpty(this.recordIdentification))
            {
                RecordIdentifier recordIdentifier = new RecordIdentifier(this.recordIdentification);
                UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { recordIdentifier });
                this.ParentOrganizerModelController.ProcessChanges(new List<IIdentifier> { recordIdentifier });
            }

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, UPChangeHints.ChangeHintsWithHint("Upload finished"));
        }

        /// <summary>
        /// Offlines the request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.Logger.LogError($"File upload failed {error}");
            this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
        }

        /// <summary>
        /// Offlines the request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
