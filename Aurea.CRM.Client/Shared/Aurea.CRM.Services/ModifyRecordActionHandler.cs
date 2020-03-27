// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifyRecordActionHandler.cs" company="Aurea Software Gmbh">
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
//   Modify Record Action Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.Structs;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Modify Record Action Handler
    /// </summary>
    /// <seealso cref="OrganizerActionHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class ModifyRecordActionHandler : OrganizerActionHandler,
            UPOfflineRequestDelegate,
            UPCopyFieldsDelegate,
            UPCRMLinkReaderDelegate,
            ILocationServiceDelegate
    {
        /// <summary>
        /// The copy fields
        /// </summary>
        private UPCopyFields CopyFields;

        /// <summary>
        /// The template filter
        /// </summary>
        private UPConfigFilter TemplateFilter;

        /// <summary>
        /// The record identification
        /// </summary>
        private string RecordIdentification;

        /// <summary>
        /// The request mode
        /// </summary>
        private UPOfflineRequestMode RequestMode;

        /// <summary>
        /// The link reader
        /// </summary>
        private UPCRMLinkReader LinkReader;

        /// <summary>
        /// The records to save
        /// </summary>
        private List<UPCRMRecord> RecordsToSave;

        /// <summary>
        /// The open child records
        /// </summary>
        private List<UPCRMRecord> OpenChildRecords;

        /// <summary>
        /// The current checked link record
        /// </summary>
        private UPCRMRecord CurrentCheckedLinkRecord;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyRecordActionHandler"/> class.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="viewReference">The view reference.</param>
        public ModifyRecordActionHandler(UPOrganizerModelController modelController, ViewReference viewReference)
            : base(modelController, viewReference)
        {
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            string requestModeString = this.ViewReference.ContextValueForKey("RequestMode");
            if (!ServerSession.CurrentSession.UserChoiceOffline && ServerSession.CurrentSession.ConnectedToServer)
            {
                this.RequestMode = UPOfflineRequest.RequestModeFromString(requestModeString, UPOfflineRequestMode.OnlineOnly);
            }
            else
            {
                this.RequestMode = UPOfflineRequest.RequestModeFromString(requestModeString, UPOfflineRequestMode.Offline);
            }

            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            string filterName = this.ViewReference.ContextValueForKey("TemplateFilter");
            if (string.IsNullOrEmpty(this.RecordIdentification))
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorParameterEmpty, "RecordId"), true);
                return;
            }

            this.TemplateFilter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            if (this.TemplateFilter == null && string.IsNullOrEmpty(this.ViewReference.ContextValueForKey("LinkRecordId")))
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorFilterMissing, filterName), true);
                return;
            }

            if (this.TemplateFilter?.NeedsLocation ?? false)
            {
                SimpleIoc.Default.GetInstance<ILocationService>().GetCurrentLocation(this);
                return;
            }

            this.ContinueTemplateFilterGeoChecked();
        }

        /// <inheritdoc />
        public void LocationResult(Location location)
        {
            this.ContinueTemplateFilterGeoChecked();
        }

        /// <inheritdoc />
        public void LocationError(string error)
        {
            this.ModelController.HandleErrors(new List<Exception> { new Exception(error) });
        }

        private void ContinueTemplateFilterGeoChecked()
        {
            string copySourceFieldGroupName = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
            var fieldControl = !string.IsNullOrEmpty(copySourceFieldGroupName)
                ? ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", copySourceFieldGroupName)
                : null;

            if (fieldControl == null || this.TemplateFilter == null)
            {
                this.ContinueWithParameters(null);
            }
            else
            {
                this.CopyFields = new UPCopyFields(fieldControl);
                this.CopyFields.CopyFieldValuesForRecordIdentification(this.RecordIdentification, this.RequestMode == UPOfflineRequestMode.OnlineOnly, this);
            }
        }

        private void ContinueWithParameters(Dictionary<string, object> parameterDictionary)
        {
            this.TemplateFilter = this.TemplateFilter.FilterByApplyingValueDictionaryDefaults(parameterDictionary, true);
            UPCRMRecord record = new UPCRMRecord(this.RecordIdentification);
            string linkRecordId = this.ViewReference.ContextValueForKey("LinkRecordId");
            if (!string.IsNullOrEmpty(linkRecordId))
            {
                int linkId = Convert.ToInt32(this.ViewReference.ContextValueForKey("LinkId"));
                if (linkId < 0)
                {
                    linkId = -1;
                }

                record.AddLink(new UPCRMLink(linkRecordId, linkId));
            }

            List<UPCRMRecord> childRecords = record.ApplyValuesFromTemplateFilter(this.TemplateFilter, false);
            if (childRecords?.Count > 0)
            {
                List<UPCRMRecord> linkRecords = null;
                List<UPCRMRecord> combinedRecords = new List<UPCRMRecord> { record };
                foreach (UPCRMRecord childRecord in childRecords)
                {
                    if (childRecord.Mode == "ParentUpdate")
                    {
                        if (linkRecords == null)
                        {
                            linkRecords = new List<UPCRMRecord> { childRecord };
                        }
                        else
                        {
                            linkRecords.Add(childRecord);
                        }
                    }
                    else
                    {
                        combinedRecords.Add(childRecord);
                    }
                }

                if (linkRecords?.Count == 0)
                {
                    this.ContinueWithRecords(combinedRecords);
                }
                else
                {
                    this.RecordsToSave = combinedRecords;
                    this.ContinueWithLinkRecords(linkRecords);
                }
            }
            else
            {
                this.ContinueWithRecords(new List<UPCRMRecord> { record });
            }
        }

        private void ContinueWithLinkRecords(List<UPCRMRecord> linkRecords)
        {
            this.OpenChildRecords = null;
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
                    if (this.OpenChildRecords == null)
                    {
                        this.OpenChildRecords = new List<UPCRMRecord> { childRecord };
                    }
                    else
                    {
                        this.OpenChildRecords.Add(childRecord);
                    }

                    continue;
                }

                if (!childRecord.RecordIdentification.IsRecordIdentification())
                {
                    continue;
                }

                sourceRecordIdentification = childRecord.RecordIdentification;
                this.CurrentCheckedLinkRecord = linkRecord;
                parentLinkString = link.LinkId > 0 ? $"{linkRecord.InfoAreaId}:{link.LinkId}" : linkRecord.InfoAreaId;
            }

            if (string.IsNullOrEmpty(parentLinkString) || string.IsNullOrEmpty(sourceRecordIdentification))
            {
                this.ContinueWithRecords(this.RecordsToSave);
                return;
            }

            this.LinkReader = new UPCRMLinkReader(sourceRecordIdentification, parentLinkString, UPRequestOption.FastestAvailable, this);
            if (!this.LinkReader.Start())
            {
                this.ContinueWithRecords(this.RecordsToSave);
            }
        }

        private void ContinueWithRecords(List<UPCRMRecord> childRecords)
        {
            UPOfflineEditRecordRequest request = new UPOfflineOrganizerModifyRecordRequest(0);
            string optionsString = this.ViewReference.ContextValueForKey("Options");
            if (!string.IsNullOrEmpty(optionsString))
            {
                Dictionary<string, object> options = optionsString.JsonDictionaryFromString();
                if (Convert.ToInt32(options["ComputeLinks"]) != 0)
                {
                    request.AlwaysSetImplicitLinks = true;
                }
            }

            if (this.ModelController.SetOfflineRequest(request, true))
            {
                if (!request.StartRequest(this.RequestMode, childRecords, this))
                {
                    this.ModelController.SetOfflineRequest(null, true);
                    this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorActionNotPossible, LocalizedString.TextErrorActionPending, true);
                }
            }
            else
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorActionNotPossible, LocalizedString.TextErrorActionPending, true);
            }
        }

#if PORTING
        void LocationProviderDidProvideLocation(UPLocationProvider locationProvider, CLLocation location)
        {
            this.ContinueTemplateFilterGeoChecked();
        }

        void LocationProviderDidFinishWithError(UPLocationProvider locationProvider, NSError error)
        {
            DDLogCError("ModifyRecord: error from geo location service: %@", error);
            this.ContinueTemplateFilterGeoChecked();
        }

#endif
        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.CopyFields = null;
            this.Error = error;
            this.Logger.LogError(error);
            this.ModelController.SetOfflineRequest(null, true);
            this.Finished();
        }

        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.CopyFields = null;
            this.ContinueWithParameters(dictionary);
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online,
            object context, Dictionary<string, object> result)
        {
            string recordIdentificationForSavedAction = this.ViewReference.ContextValueForKey("RecordIdForSavedAction");
            if (string.IsNullOrEmpty(recordIdentificationForSavedAction))
            {
                recordIdentificationForSavedAction = this.ViewReference.ContextValueForKey("RecordId");
            }

            this.followUpViewReference = this.ViewReferenceWith("SavedAction", recordIdentificationForSavedAction, this.ViewReference.ContextValueForKey("RecordId")) ??
                                         this.ViewReferenceWith("defaultFinishAction", recordIdentificationForSavedAction, this.ViewReference.ContextValueForKey("RecordId"));

            if (this.ViewReference.HasBackToPreviousFollowUpAction())
            {
                this.followUpViewReference = this.followUpViewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                this.followUpReplaceOrganizer = true;
            }

            this.ModelController.SetOfflineRequest(null, true);
            this.ChangedRecords = (List<UPCRMRecord>)data;
            this.Finished();
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
            this.Error = error;
            this.ModelController.SetOfflineRequest(null, true);
            this.Finished();
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

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            this.LinkReader = null;
            if (linkReader.DestinationRecordIdentification.IsRecordIdentification())
            {
                this.CurrentCheckedLinkRecord.UpdateRecordInformationWithRecordIdentification(linkReader.DestinationRecordIdentification, true);
                this.RecordsToSave.Add(this.CurrentCheckedLinkRecord);
                this.CurrentCheckedLinkRecord = null;
            }

            if (this.OpenChildRecords != null)
            {
                this.ContinueWithLinkRecords(this.OpenChildRecords);
            }
            else
            {
                this.ContinueWithRecords(this.RecordsToSave);
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.Error = error;
            this.ModelController.SetOfflineRequest(null, true);
            this.Finished();
        }
    }
}
