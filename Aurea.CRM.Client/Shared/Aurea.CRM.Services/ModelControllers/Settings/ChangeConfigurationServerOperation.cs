// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeConfigurationServerOperation.cs" company="Aurea Software Gmbh">
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
//   The Change Configuration Server Operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.UIModel.Contexts;

    /// <summary>
    /// Change Configuration Request Delegate
    /// </summary>
    public interface IChangeConfigurationRequestDelegate
    {
        /// <summary>
        /// Changes the configuration request did finish with result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        void ChangeConfigurationRequestDidFinishWithResult(ChangeConfigurationServerOperation sender, object result);
    }

    /// <summary>
    /// Change Configuration Server Operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class ChangeConfigurationServerOperation : JsonResponseServerOperation
    {
        private Dictionary<string, UPEditFieldContext> WebConfigParameterDictionary { get; }

        private IChangeConfigurationRequestDelegate TheDelegate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeConfigurationServerOperation"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="theDelegate">The delegate.</param>
        public ChangeConfigurationServerOperation(Dictionary<string, UPEditFieldContext> dictionary, IChangeConfigurationRequestDelegate theDelegate)
        {
            this.WebConfigParameterDictionary = dictionary;
            this.TheDelegate = theDelegate;
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
                parameterDictionary["Service"] = "ChangeConfiguration";
                if (this.WebConfigParameterDictionary != null)
                {
                    //parameterDictionary["Changes"] = this.ObjectToJSON(NSMutableArray.ArrayFromDictionary(this.WebConfigParameterDictionary));
                }

                return parameterDictionary;
            }
        }

        /// <summary>
        /// Stores the web configuration parameters locally.
        /// </summary>
        public void StoreWebConfigParametersLocally()
        {
            //ConfigurationUnitStore.DefaultStore.UpdateWebConfigValues(this.WebConfigParameterDictionary);
        }

        Exception HandleStatusInfo(object statusInfo)
        {
            if (statusInfo == null)
            {
                return null;
            }
            else
            {
                return null;//NSError.ErrorFromServerErrorResponse(statusInfo);
            }
        }

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="remoteData">The remote data.</param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.StoreWebConfigParametersLocally();
            this.TheDelegate.ChangeConfigurationRequestDidFinishWithResult(this, null);
        }

        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
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
                Exception error = this.HandleStatusInfo(statusInfo);
                if (error != null)
                {
                    this.ProcessErrorWithRemoteData(error, remoteData);
                    return;
                }

            }

            object updatedWebConfigurationValues = json.ValueOrDefault("WebConfigValue");
            if (updatedWebConfigurationValues != null)
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                UPSyncConfiguration syncConfiguration = new UPSyncConfiguration(configStore);
                //syncConfiguration.SyncElementsOfUnitType(updatedWebConfigurationValues, "WebConfigValue", true);
                configStore.Reset();
                ServerSession.CurrentSession.LoadApplicationSettings();
            }

            this.TheDelegate.ChangeConfigurationRequestDidFinishWithResult(this, null);
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

    }
}
