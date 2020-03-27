// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operation.cs" company="Aurea Software Gmbh">
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
//   Defines the abstract implementation of an Operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;

    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Defines the abstract implementation of an Operation
    /// </summary>
    public abstract class Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        protected Operation()
        {
            this.TokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        protected Operation(Action action)
            : this()
        {
            this.Work = new Task(action, this.TokenSource.Token);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the log settings.
        /// </summary>
        /// <value>
        /// The log settings.
        /// </value>
        public static ILogSettings LogSettings => SimpleIoc.Default.GetInstance<ILogSettings>();

        /// <summary>
        /// Gets a value indicating whether this <see cref="Operation"/> is canceled.
        /// </summary>
        /// <value>
        /// <c>true</c> if canceled; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Canceled => this.Work != null && this.Work.IsCanceled;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Operation"/> is finished.
        /// </summary>
        /// <value>
        /// <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Finished => this.Work != null && this.Work.IsCompleted;

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public CancellationTokenSource TokenSource { get; }

        /// <summary>
        /// Gets the work.
        /// </summary>
        /// <value>
        /// The work.
        /// </value>
        public virtual Task Work { get; private set; }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public virtual void Cancel()
        {
            // Cancel the operation
            this.TokenSource?.Cancel();
        }

        /// <summary>
        /// Finishes the processing.
        /// </summary>
        public virtual void FinishProcessing()
        {
            // [self setFinishedFlag: YES];
        }
    }

    /// <summary>
    /// Defines the abstract implementation of a server operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Operation" />
    public abstract class ServerOperation : Operation
    {
        /// <summary>
        /// Gets or sets a value indicating whether [always perform].
        /// </summary>
        /// <value>
        /// <c>true</c> if [always perform]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AlwaysPerform { get; set; }

        /// <summary>
        /// Gets a value indicating whether [blocked by pending up synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [blocked by pending up synchronize]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool BlockedByPendingUpSync => false;

        /// <summary>
        /// Gets or sets the error translation key.
        /// </summary>
        /// <value>
        /// The error translation key.
        /// </value>
        public string ErrorTranslationKey { get; set; }

        /// <summary>
        /// Gets the HTTP header fields.
        /// </summary>
        /// <value>
        /// The HTTP header fields.
        /// </value>
        public virtual Dictionary<string, string> HttpHeaderFields => null;

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        /// <value>
        /// The request parameters.
        /// </value>
        public virtual Dictionary<string, string> RequestParameters => null;


        public void AddAppInfoInToRequestParameters(Dictionary<string, string> parametersForServerOperation)
        {
            if (parametersForServerOperation == null)
                return;
            if (parametersForServerOperation.ContainsKey("AppInfo"))
                return;
            parametersForServerOperation.Add("AppInfo", "crmclient");
        }

        /// <summary>
        /// Gets the request URL.
        /// </summary>
        /// <value>
        /// The request URL.
        /// </value>
        public virtual Uri RequestUrl => null;

        /// <summary>
        /// Gets or sets the tracking delegate.
        /// </summary>
        /// <value>
        /// The tracking delegate.
        /// </value>
        public IRemoteDataTracking TrackingDelegate { get; set; }

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public abstract void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData);

        /// <summary>
        /// Processes the operation failer due to internet connection issue.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public virtual void ProcessFailedDueToInternetConnection(Exception error, RemoteData remoteData)
        {
            this.ProcessErrorWithRemoteData(error, remoteData);
        }

        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public abstract void ProcessRemoteData(RemoteData remoteData);

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var desc = new StringBuilder();
            if (this.RequestUrl != null)
            {
                desc.AppendFormat("Url = {0}\r", this.RequestUrl.AbsolutePath);
            }

            if (this.RequestParameters == null)
            {
                return desc.ToString();
            }

            foreach (var key in this.RequestParameters.Keys)
            {
                desc.AppendFormat("{0} = {1}\r", key, this.RequestParameters[key]);
            }

            return desc.ToString();
        }
    }

    /// <summary>
    /// Defines abstract server operation with Json format response
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.ServerOperation" />
    public abstract class JsonResponseServerOperation : ServerOperation
    {
        /// <summary>
        /// Processes the invalid json error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public virtual void ProcessInvalidJsonError(Exception error, RemoteData remoteData)
        {
            this.ProcessErrorWithRemoteData(error, remoteData);
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
        public abstract void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData);

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public abstract void ProcessJsonSyncObject(DataModelSyncDeserializer json, RemoteData remoteData);
        
    }

    /// <summary>
    /// Defines the abstract local operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Operation" />
    public abstract class LocalOperation : Operation
    {
        /// <summary>
        /// Performs the operation.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool PerformOperation() => false;
    }
}
