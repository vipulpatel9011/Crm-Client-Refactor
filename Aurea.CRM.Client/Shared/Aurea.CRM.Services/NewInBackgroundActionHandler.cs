// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewInBackgroundActionHandler.cs" company="Aurea Software Gmbh">
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
//   NewInBackground Action Handler
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
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// NewInBackground Action Handler
    /// </summary>
    /// <seealso cref="OrganizerActionHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class NewInBackgroundActionHandler : OrganizerActionHandler,
         ISearchOperationHandler,
         UPOfflineRequestDelegate,
         // UPLocationProviderDelegate,
         UPCopyFieldsDelegate,
         UPCRMLinkReaderDelegate
    {
        private Dictionary<string, object> sourceCopyFields;
        private UPCRMRecord record;
        private UPCopyFields copyFields;
        private UPCRMLinkReader linkReader;

        /// <summary>
        /// Gets the CRM query.
        /// </summary>
        /// <value>
        /// The CRM query.
        /// </value>
        public UPContainerMetaInfo CrmQuery { get; private set; }

        /// <summary>
        /// Gets the parent link.
        /// </summary>
        /// <value>
        /// The parent link.
        /// </value>
        public string ParentLink { get; private set; }

        /// <summary>
        /// Gets the source copy field group.
        /// </summary>
        /// <value>
        /// The source copy field group.
        /// </value>
        public FieldControl SourceCopyFieldGroup { get; private set; }

        /// <summary>
        /// Gets the link record identification.
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewInBackgroundActionHandler"/> class.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="viewReference">The view reference.</param>
        public NewInBackgroundActionHandler(UPOrganizerModelController modelController, ViewReference viewReference)
            : base(modelController, viewReference)
        {
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ParentLink = this.ViewReference.ContextValueForKey("ParentLink");
            string linkRecordIdentification = this.ViewReference.ContextValueForKey("LinkRecordId");
            if (!string.IsNullOrEmpty(this.ParentLink) && !string.IsNullOrEmpty(linkRecordIdentification))
            {
                if (this.ParentLink == "NoLink")
                {
                    this.LinkRecordIdentification = null;
                }
                else
                {
                    this.linkReader = new UPCRMLinkReader(linkRecordIdentification, this.ParentLink, this);
                    if (!this.linkReader.Start())
                    {
                        this.Error = new Exception("cannot start link reader");
                        this.Finished();
                    }

                    return;
                }
            }
            else
            {
                this.LinkRecordIdentification = linkRecordIdentification;
            }

            this.ParentLinkLoaded();
        }

        private void ParentLinkLoaded()
        {
            string sourceCopyFieldGroupName = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
            if (!string.IsNullOrEmpty(sourceCopyFieldGroupName))
            {
                this.SourceCopyFieldGroup = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", sourceCopyFieldGroupName);
                if (this.SourceCopyFieldGroup != null)
                {
                    string recordIdentification = this.ViewReference.ContextValueForKey("LinkRecordId");
                    this.copyFields = new UPCopyFields(this.SourceCopyFieldGroup);
                    this.copyFields.CopyFieldValuesForRecordIdentification(recordIdentification, false, this);
                    return;
                }
            }

            this.CopyFieldsLoaded();
        }

        private void CopyFieldsLoaded()
        {
            string filterName = this.ViewReference.ContextValueForKey("TemplateFilter");
            if (!string.IsNullOrEmpty(filterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
                if (filter.NeedsLocation)
                {
                    //UPLocationProvider.Current().RequestLocationForObject(this);
                    return;
                }
            }

            this.ContinueTemplateFilterGeoChecked();
        }

        private void ContinueTemplateFilterGeoChecked()
        {
            bool checkExisting = this.ViewReference.ContextValueIsSet("CheckExisting") || this.ViewReference.ContextValueIsSet("ExistsAction");
            if (!checkExisting)
            {
                this.ContinueRecordDoesNotExist();
                return;
            }

            string requestModeString = this.ViewReference.ContextValueForKey("RequestMode");
            UPRequestOption requestOption = UPCRMDataStore.RequestOptionFromString(requestModeString, UPRequestOption.Online);
            string filterName = this.ViewReference.ContextValueForKey("TemplateFilter");
            string infoAreaId = this.ViewReference.ContextValueForKey("InfoAreaId");
            string linkIdString = this.ViewReference.ContextValueForKey("LinkId");
            if (string.IsNullOrEmpty(infoAreaId))
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorParameterEmpty, "InfoAreaId"), true);
                return;
            }

            UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            filter = filter.FilterByApplyingValueDictionaryDefaults(this.sourceCopyFields, true);
            this.CrmQuery = new UPContainerMetaInfo(null, infoAreaId);
            if (this.CrmQuery == null)
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorParameterEmpty, "crmQuery"), true);
                return;
            }

            this.CrmQuery.ApplyFilter(filter);
            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                int linkId = -1;
                if (!string.IsNullOrEmpty(linkIdString))
                {
                    linkId = Convert.ToInt32(linkIdString);
                }

                this.CrmQuery.SetLinkRecordIdentification(this.LinkRecordIdentification, linkId);
            }

            this.CrmQuery.Find(requestOption, this);
        }

        private void ContinueRecordDoesNotExist()
        {
            string requestModeString = this.ViewReference.ContextValueForKey("RequestMode");
            UPOfflineRequestMode requestMode = UPOfflineRequest.RequestModeFromString(requestModeString, UPOfflineRequestMode.OnlineOnly);
            string filterName = this.ViewReference.ContextValueForKey("TemplateFilter");
            string infoAreaId = this.ViewReference.ContextValueForKey("InfoAreaId");
            string linkIdString = this.ViewReference.ContextValueForKey("LinkId");
            if (string.IsNullOrEmpty(infoAreaId))
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorParameterEmpty, "InfoAreaId"), true);
                return;
            }

            int linkId = -1;
            bool noLink = false;
            if (!string.IsNullOrEmpty(linkIdString))
            {
                if (linkIdString == "NoLink")
                {
                    noLink = true;
                }

                linkId = Convert.ToInt32(linkIdString);
            }

            UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
            if (filter == null)
            {
                this.ModelController.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorFilterMissing, filterName), true);
                return;
            }

            filter = filter.FilterByApplyingValueDictionaryDefaults(this.sourceCopyFields, true);
            this.record = UPCRMRecord.CreateNew(infoAreaId);
            if (!noLink && !string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                this.record.AddLink(new UPCRMLink(this.LinkRecordIdentification, linkId));
            }

            this.record.ApplyValuesFromTemplateFilter(filter);
            UPOfflineEditRecordRequest request = new UPOfflineEditRecordRequest(0);
            string optionsString = this.ViewReference.ContextValueForKey("Options");
            if (!string.IsNullOrEmpty(optionsString))
            {
                Dictionary<string, object> options = optionsString.JsonDictionaryFromString();
                if (Convert.ToInt32(options["ComputeLinks"]) != 0)
                {
                    request.AlwaysSetImplicitLinks = true;
                }
            }

            List<UPCRMRecord> recordArray = new List<UPCRMRecord> { this.record };
            string syncParentInfoAreaIdString = this.ViewReference.ContextValueForKey("SyncParentInfoAreaIds");
            if (!string.IsNullOrEmpty(syncParentInfoAreaIdString))
            {
                var syncParentInfoAreaIds = syncParentInfoAreaIdString.Split(',');
                foreach (string syncParentInfoAreaId in syncParentInfoAreaIds)
                {
                    var infoAreaIdParts = syncParentInfoAreaId.Split(':');
                    if (infoAreaIdParts.Length == 1)
                    {
                        recordArray.Add(new UPCRMRecord(syncParentInfoAreaId, new UPCRMLink(this.record, -1)));
                    }
                    else if (infoAreaIdParts.Length > 1)
                    {
                        recordArray.Add(new UPCRMRecord(infoAreaIdParts[0], new UPCRMLink(this.record, Convert.ToInt32(infoAreaIdParts[1]))));
                    }
                }
            }

            if (this.ModelController.SetOfflineRequest(request, true))
            {
                if (request.StartRequest(requestMode, recordArray, this) == false)
                {
                    this.ModelController.SetOfflineRequest(null, true);
                    return;
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
            DDLogCError("NewInBrackground: error from geo location service: %@", error);
            this.ContinueTemplateFilterGeoChecked();
        }
#endif

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.Error = error;
            this.Logger.LogError(error);
            this.Finished();
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount == 0)
            {
                this.ContinueRecordDoesNotExist();
            }
            else
            {
                this.followUpViewReference = this.ViewReferenceWith("ExistsAction", ((UPCRMResultRow)result.ResultRowAtIndex(0)).RootRecordIdentification, null);
                this.Finished();
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
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.sourceCopyFields = dictionary;
            this.CopyFieldsLoaded();
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.Error = error;
            this.Finished();
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            this.LinkRecordIdentification = _linkReader.DestinationRecordIdentification;
            this.ParentLinkLoaded();
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            this.Error = error;
            this.Finished();
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context,
            Dictionary<string, object> result)
        {
            this.followUpViewReference = this.ViewReferenceWith("CreatedAction", this.record.RecordIdentification, this.ViewReference.ContextValueForKey("LinkRecordId")) ??
                                         this.ViewReferenceWith("defaultFinishAction", this.record.RecordIdentification, this.ViewReference.ContextValueForKey("LinkRecordId"));

            if (this.ViewReference.HasBackToPreviousFollowUpAction())
            {
                this.followUpViewReference = this.followUpViewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                this.followUpReplaceOrganizer = true;
            }

            this.ChangedRecords = (List<UPCRMRecord>)data;
            this.ModelController.SetOfflineRequest(null, true);
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
            this.followUpViewReference = this.ViewReferenceWith("CreateFailureAction", this.ViewReference.ContextValueForKey("LinkRecordId"), this.ViewReference.ContextValueForKey("LinkRecordId"));
            if (this.followUpViewReference == null)
            {
                this.Error = error;
            }

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
    }
}
