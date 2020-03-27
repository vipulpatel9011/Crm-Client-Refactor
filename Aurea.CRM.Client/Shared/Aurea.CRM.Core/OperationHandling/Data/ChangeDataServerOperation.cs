// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeDataServerOperation.cs" company="Aurea Software Gmbh">
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
// <summary>
//   Defines the change data request handler interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Aurea.CRM.Core.Common;

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the change data request handler interface
    /// </summary>
    public interface IChangeDataRequestHandler
    {
        /// <summary>
        /// Changes the data request did fail with error.
        /// </summary>
        /// <param name="request">
        /// The sender.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void ChangeDataRequestDidFailWithError(UPChangeDataServerOperation request, Exception error);

        /// <summary>
        /// Changes the data request did finish with result.
        /// </summary>
        /// <param name="request">
        /// The sender.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        void ChangeDataRequestDidFinishWithResult(UPChangeDataServerOperation request, Dictionary<string, object> result);
    }

    /// <summary>
    /// Server operation for update record data on server side
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class UPChangeDataServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChangeDataServerOperation"/> class.
        /// </summary>
        /// <param name="records">
        /// The records.
        /// </param>
        /// <param name="requestControlKey">
        /// The request control key.
        /// </param>
        /// <param name="requestNumber">
        /// The request number.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPChangeDataServerOperation(
            List<UPCRMRecord> records,
            string requestControlKey,
            int requestNumber,
            IChangeDataRequestHandler theDelegate)
        {
            this.Records = new List<UPCRMRecord>(records);
            this.RequestNumber = requestNumber;
            this.RequestControlKey = requestControlKey;
            this.Delegate = theDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChangeDataServerOperation"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="requestControlKey">
        /// The request control key.
        /// </param>
        /// <param name="requestNumber">
        /// The request number.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public UPChangeDataServerOperation(
            UPCRMRecord record,
            string requestControlKey,
            int requestNumber,
            IChangeDataRequestHandler theDelegate)
        {
            this.Records = new List<UPCRMRecord> { record };
            this.RequestNumber = requestNumber;
            this.RequestControlKey = requestControlKey;
            this.Delegate = theDelegate;
        }

        /// <summary>
        /// Gets a value indicating whether [blocked by pending up synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [blocked by pending up synchronize]; otherwise, <c>false</c>.
        /// </value>
        public override bool BlockedByPendingUpSync => true;

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IChangeDataRequestHandler Delegate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [four digit numbers].
        /// </summary>
        /// <value>
        /// <c>true</c> if [four digit numbers]; otherwise, <c>false</c>.
        /// </value>
        public bool FourDigitNumbers { get; set; }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public List<UPCRMRecord> Records { get; private set; }

        /// <summary>
        /// Gets the request control key.
        /// </summary>
        /// <value>
        /// The request control key.
        /// </value>
        public string RequestControlKey { get; private set; }

        /// <summary>
        /// Gets the request number.
        /// </summary>
        /// <value>
        /// The request number.
        /// </value>
        public int RequestNumber { get; private set; }

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        /// <value>
        /// The request parameters.
        /// </value>
        public override Dictionary<string, string> RequestParameters
        {
            get
            {
                var parameterDictionary = new Dictionary<string, string>();
                parameterDictionary.SetObjectForKey("SaveRecords", "Service");
                this.FourDigitNumbers = false;
                var currentSession = ServerSession.CurrentSession;
                var serviceInfo = currentSession.ServiceInfoForServiceName("SaveRecords");
                if (serviceInfo.IsAtLeastVersion("1.1"))
                {
                    this.FourDigitNumbers = true;
                    parameterDictionary.SetObjectForKey("2.6", "Version");
                }

                if (!string.IsNullOrEmpty(this.RequestControlKey))
                {
                    parameterDictionary.SetObjectForKey(this.RequestControlKey, "RequestControlKey");
                    if (this.RequestNumber >= 0)
                    {
                        parameterDictionary.SetObjectForKey($"{this.RequestNumber}", "RequestNumber");
                    }
                }

                var index = 0;
                var sentRecords = new List<UPCRMRecord>(this.Records.Count);
                var mapper = ServerSession.CurrentSession.RecordIdentificationMapper;
                foreach (var record in this.Records)
                {
                    record.ApplyMappedRecordIdentifications(mapper);
                    if (string.IsNullOrEmpty(record.RecordId))
                    {
                        record.RecordId = $"newid:{index}";
                    }

                    var jsonString = JsonConvert.SerializeObject(record.JsonModifyRequest());
                    if (jsonString == null || jsonString == "null")
                    {
                        continue;
                    }

                    parameterDictionary.SetObjectForKey(jsonString, this.FourDigitNumbers ? $"Record{index:d4}" : $"Record{index}");

                    ++index;
                    sentRecords.Add(record);
                }

                this.Records = sentRecords;
                parameterDictionary.SetObjectForKey($"{index}", "RecordCount");
                return parameterDictionary;
            }
        }

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.Delegate?.ChangeDataRequestDidFailWithError(this, error);
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
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            var statusInfo = json.ValueOrDefault("StatusInfo") as List<object>;
            if (statusInfo != null)
            {
                //// NSError *error =  [NSError errorFromServerErrorResponse:statusInfo errorTranslationKey:self.errorTranslationKey];
                var error = statusInfo.Count > 3 ? new ServerException(statusInfo[3] as string) : new ServerException(statusInfo.ToString());

                if (statusInfo.Count > 7)
                {
                    error.ServerStackTrace = statusInfo[7] as string;
                }
                this.Delegate?.ChangeDataRequestDidFailWithError(this, error);

                return;
            }

            int index = 0;

            List<string> skippedFieldValues = new List<string>();
            foreach (UPCRMRecord record in this.Records)
            {
                List<object> recordResponse = (this.FourDigitNumbers
                                                 ? json.ValueOrDefault($"Record{index++:D4}")
                                                 : json.ValueOrDefault($"Record{index++}")) as List<object>;

                record.HandleChangeRecordResponse(recordResponse);

                // Refer to CRMPAD-1144
                //for (int i = 0; i < recordResponse.Count; i++)
                //{
                //    Dictionary<string, object> deniedFields = recordResponse[i] as Dictionary<string, object>;
                //    if (deniedFields != null)
                //    {
                //        if (deniedFields.ContainsKey("fieldsDeniedWithAccessRights"))
                //        {
                //            List<object> fieldValues = deniedFields["fieldsDeniedWithAccessRights"] as List<object>;
                //            skippedFieldValues.AddRange(fieldValues.Select(x => x.ToString()));
                //        }
                //    }
                //}
            }

            if (skippedFieldValues.Count > 0)
            {
                string errDetails = string.Format(LocalizedString.TextErrorFieldUpdateDenied, string.Join(", ", skippedFieldValues));
                this.Delegate?.ChangeDataRequestDidFailWithError(this, new Exception(errDetails));
                return;
            }

            this.Delegate?.ChangeDataRequestDidFinishWithResult(this, json);
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
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
