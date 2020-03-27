// <copyright file="UploadDocumentServerOperation.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Platform;
    using Session;

    /// <summary>
    /// Upload document server operation class implementation
    /// </summary>
    public class UploadDocumentServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadDocumentServerOperation"/> class.
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="recordIdentification">Record identification</param>
        /// <param name="fieldId">Field identification</param>
        /// <param name="delegate">Delegate object</param>
        public UploadDocumentServerOperation(
            UploadDocument document,
            string recordIdentification,
            int fieldId,
            IUploadDocumentRequestDelegate @delegate)
            : this(document, recordIdentification, fieldId, null, @delegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadDocumentServerOperation"/> class.
        /// </summary>
        /// <param name="document">Document</param>
        /// <param name="recordIdentification">Record identification</param>
        /// <param name="fieldId">Field identification</param>
        /// <param name="noD3Link">No D3 Link data</param>
        /// <param name="delegate">Delegate object</param>
        public UploadDocumentServerOperation(
            UploadDocument document,
            string recordIdentification,
            int fieldId,
            string noD3Link,
            IUploadDocumentRequestDelegate @delegate)
        {
            this.Document = document;
            this.RecordIdentification = recordIdentification;
            this.FieldId = fieldId;
            this.Delegate = @delegate;
            this.NoD3Link = noD3Link;
        }

        /// <summary>
        /// Gets data
        /// </summary>
        public byte[] Data => this.Document.Data;

        /// <summary>
        /// Gets file names parameter name
        /// </summary>
        public string FileNameParamName => "filename";

        /// <summary>
        /// Gets file name
        /// </summary>
        public string FileName => this.Document.Filename;

        /// <summary>
        /// Gets content type
        /// </summary>
        public string ContentType => this.Document.MimeType;

        /// <summary>
        /// Gets transfer encoding
        /// </summary>
        public string TransferEncoding => "binary";

        /// <summary>
        /// Gets document
        /// </summary>
        public UploadDocument Document { get; private set; }

        /// <summary>
        /// Gets field identification
        /// </summary>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets record identification
        /// </summary>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets no d3 link data
        /// </summary>
        public string NoD3Link { get; private set; }

        /// <summary>
        /// Gets delegate
        /// </summary>
        public IUploadDocumentRequestDelegate Delegate { get; private set; }

        /// <summary>
        /// Gets request parameters
        /// </summary>
        public override Dictionary<string, string> RequestParameters => this.GetRequestParameters();

        /// <summary>
        /// Processes Json data response
        /// </summary>
        /// <param name="json">JSON data</param>
        /// <param name="remoteData">Remote data</param>
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            var statusInfo = json.ValueOrDefault("StatusInfo") as List<object>;
            if (statusInfo != null)
            {
                var error = statusInfo.Count > 3
                                ? new Exception(statusInfo[3] as string)
                                : new Exception(statusInfo.ToString());
                this.Delegate.UploadDocumentRequestDidFailWithError(this, error);
                return;
            }

            this.Delegate?.UploadDocumentRequestDidFinishWithJSON(this, json);
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessJsonSyncObject(DataModelSyncDeserializer json, RemoteData remoteData)
        {
            //this.Delegate?.OnFinishWithObjectResponse(this, json);
        }

        /// <summary>
        /// Processes error within the remote data
        /// </summary>
        /// <param name="error">Error</param>
        /// <param name="remoteData">Remote data</param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.Delegate?.UploadDocumentRequestDidFailWithError(this, error);
        }

        /// <summary>
        /// Processes remote data
        /// </summary>
        /// <param name="remoteData">Remote data</param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            var upOfflineUploadDocumentRequest = (OfflineStorage.UPOfflineUploadDocumentRequest)this.Delegate;
            var upOfflineStorage = upOfflineUploadDocumentRequest.OfflineRequestDelegate as OfflineStorage.UPOfflineStorage;
            if (upOfflineStorage != null)
            {
                var docUploadRequests = new List<OfflineStorage.UPOfflineRequest>();
                foreach (var offlineRequest in upOfflineStorage.OfflineRequests)
                {
                    if (offlineRequest.RequestType.Equals(OfflineStorage.OfflineRequestType.DocumentUpload))
                    {
                        docUploadRequests.Add(offlineRequest);
                    }
                }
                docUploadRequests[docUploadRequests.Count - 1].DeleteRequest(false);
            }
        }

        private Dictionary<string, string> GetRequestParameters()
        {
            Dictionary<string, string> parameterDictionary = new Dictionary<string, string>();
            parameterDictionary.SetObjectForKey("Documents", "Service");
            parameterDictionary.SetObjectForKey("Upload", "Mode");
            if (this.FieldId > 0)
            {
                ServiceInfo serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("Documents");
                if (string.CompareOrdinal(serviceInfo.Version, "1.1") > -1)
                {
                    parameterDictionary["FieldId"] = this.FieldId.ToString();
                }

            }

            if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                parameterDictionary.SetObjectForKey(this.RecordIdentification, "RecordId");
            }

            if (!string.IsNullOrEmpty(this.NoD3Link))
            {
                parameterDictionary.SetObjectForKey("true", "NoD3Link");
            }

            return parameterDictionary;
        }
    }
}
