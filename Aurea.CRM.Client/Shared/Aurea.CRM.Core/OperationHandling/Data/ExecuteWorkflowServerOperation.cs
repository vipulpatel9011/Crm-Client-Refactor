// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteWorkflowServerOperation.cs" company="Aurea Software Gmbh">
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
//  ExecuteWorkflowServerOperation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Sync;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The service keyname
        /// </summary>
        public const string SERVICE_KEYNAME = "Service";

        /// <summary>
        /// The service name
        /// </summary>
        public const string SERVICE_NAME = "ExecuteWorkflow";

        /// <summary>
        /// The recordidentifier keyname
        /// </summary>
        public const string RECORDIDENTIFIER_KEYNAME = "RecordIdentifier";

        /// <summary>
        /// The workflowname keyname
        /// </summary>
        public const string WORKFLOWNAME_KEYNAME = "WorkflowName";

        /// <summary>
        /// The parameter keyname
        /// </summary>
        public const string PARAMETER_KEYNAME = "Parameter";

        /// <summary>
        /// The executionflags keyname
        /// </summary>
        public const string EXECUTIONFLAGS_KEYNAME = "ExecutionFlags";

        /// <summary>
        /// The additionalrecords keyname
        /// </summary>
        public const string ADDITIONALRECORDS_KEYNAME = "AdditionalRecords";
    }

    /// <summary>
    /// ExecuteWorkflowServerOperation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class UPExecuteWorkflowServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPExecuteWorkflowServerOperation"/> class.
        /// </summary>
        /// <param name="workflowName">Name of the workflow.</param>
        /// <param name="recordIdentifier">The record identifier.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="executionFlags">The execution flags.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPExecuteWorkflowServerOperation(string workflowName, string recordIdentifier, string parameter, string executionFlags, UPExecuteWorkflowRequestDelegate theDelegate)
        {
            this.WorkflowName = workflowName;
            this.RecordIdentifier = recordIdentifier;
            this.Parameter = parameter;
            this.ExecutionFlags = executionFlags;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPExecuteWorkflowServerOperation"/> class.
        /// </summary>
        /// <param name="workflowName">Name of the workflow.</param>
        /// <param name="records">The records.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="executionFlags">The execution flags.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPExecuteWorkflowServerOperation(string workflowName, List<string> records, List<object> parameters, string executionFlags, UPExecuteWorkflowRequestDelegate theDelegate)
        {
            this.WorkflowName = workflowName;
            if (records.Count > 0)
            {
                this.RecordIdentifier = records[0];
            }

            if (records.Count > 1)
            {
                for (int i = 1; i < records.Count; i++)
                {
                    this.AdditionalRecords = i == 1 ? records[i] : $"{this.AdditionalRecords},{records[i]}";
                }
            }

            this.Parameter = parameters.Count > 0 ? StringExtensions.StringFromObject(parameters) : string.Empty;

            this.ExecutionFlags = executionFlags;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Errors from status information.
        /// </summary>
        /// <param name="statusInfo">The status information.</param>
        /// <returns></returns>
        private Exception ErrorFromStatusInfo(object statusInfo)
        {
            if (statusInfo == null)
            {
                return null;
            }

            return new Exception(); //NSError.ErrorFromServerErrorResponseErrorTranslationKey(statusInfo, this.ErrorTranslationKey);
        }

        /// <summary>
        /// Gets the name of the workflow.
        /// </summary>
        /// <value>
        /// The name of the workflow.
        /// </value>
        public string WorkflowName { get; private set; }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordIdentifier { get; private set; }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string Parameter { get; private set; }

        /// <summary>
        /// Gets the execution flags.
        /// </summary>
        /// <value>
        /// The execution flags.
        /// </value>
        public string ExecutionFlags { get; private set; }

        /// <summary>
        /// Gets the additional records.
        /// </summary>
        /// <value>
        /// The additional records.
        /// </value>
        public string AdditionalRecords { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPExecuteWorkflowRequestDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [blocked by pending up synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [blocked by pending up synchronize]; otherwise, <c>false</c>.
        /// </value>
        public override bool BlockedByPendingUpSync => true;

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="remoteData">The remote data.</param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.TheDelegate.ExecuteWorkflowRequestDidFailWithError(this, error);
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="remoteData">The remote data.</param>
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            object statusInfo = json.ValueOrDefault("StatusInfo");
            if (statusInfo != null)
            {
                this.ProcessErrorWithRemoteData(this.ErrorFromStatusInfo(statusInfo), remoteData);
                return;
            }

            List<string> changedRecords = null;
            List<string> messages = json.ValueOrDefault("Messages") as List<string>;

            List<Dictionary<string, object>> changedRecordsFromServer = json.ValueOrDefault("ChangedRecords") as List<Dictionary<string, object>>;
            if (changedRecordsFromServer?.Count > 0)
            {
                changedRecords = new List<string>(changedRecordsFromServer.Count);
                foreach (Dictionary<string, object> record in changedRecordsFromServer)
                {
                    string recordIdentification;
                    object recordPart = record["RecordId"];
                    if (recordPart is long)
                    {
                        recordIdentification = StringExtensions.InfoAreaIdNumberRecordId((string)record["InfoAreaId"], (long)recordPart);
                    }
                    else
                    {
                        recordIdentification = StringExtensions.InfoAreaIdRecordId((string)record["InfoAreaId"], recordPart.ToString());
                    }

                    changedRecords.Add(recordIdentification);
                }
            }

            List<object> changedRecordData = json.ValueOrDefault("ChangedRecordsData") as List<object>;
            UPCRMRecordSync.SyncRecordSetDefinitions(changedRecordData);
            UPExecuteWorkflowResult executeWorkflowResult = new UPExecuteWorkflowResult(changedRecords, messages);
            this.TheDelegate.ExecuteWorkflowRequestDidFinishWithResult(this, executeWorkflowResult);
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
        /// <param name="remoteData">The remote data.</param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

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
                Dictionary<string, string> parameterDictionary = new Dictionary<string, string>();
                parameterDictionary[Constants.SERVICE_KEYNAME] = Constants.SERVICE_NAME;
                if (!string.IsNullOrEmpty(this.RecordIdentifier))
                {
                    parameterDictionary[Constants.RECORDIDENTIFIER_KEYNAME] = this.RecordIdentifier;
                }

                if (!string.IsNullOrEmpty(this.WorkflowName))
                {
                    parameterDictionary[Constants.WORKFLOWNAME_KEYNAME] = this.WorkflowName;
                }

                if (this.Parameter != null)
                {
                    parameterDictionary[Constants.PARAMETER_KEYNAME] = this.Parameter;
                }

                if (this.ExecutionFlags != null)
                {
                    parameterDictionary[Constants.EXECUTIONFLAGS_KEYNAME] = this.ExecutionFlags;
                }

                if (this.AdditionalRecords != null)
                {
                    parameterDictionary[Constants.ADDITIONALRECORDS_KEYNAME] = this.AdditionalRecords;
                }

                return parameterDictionary;
            }
        }
    }
}
