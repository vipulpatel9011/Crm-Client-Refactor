// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogoutServerOperation.cs" company="Aurea Software Gmbh">
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
//   Implements the session logout operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OperationHandling.Authentication
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Implements the session logout operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class LogoutServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutServerOperation"/> class.
        /// </summary>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public LogoutServerOperation(ILogoutServerOperationDelegate theDelegate)
        {
            this.Delegate = theDelegate;
        }

        /// <summary>
        /// Gets a value indicating whether [always perform].
        /// </summary>
        /// <value>
        /// <c>true</c> if [always perform]; otherwise, <c>false</c>.
        /// </value>
        public override bool AlwaysPerform => true;

        /// <summary>
        /// Gets or sets the delegate reference.
        /// </summary>
        /// <value>
        /// The delegate reference.
        /// </value>
        public ILogoutServerOperationDelegate Delegate { get; set; }

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        /// <value>
        /// The request parameters.
        /// </value>
        public override Dictionary<string, string> RequestParameters
            => new Dictionary<string, string> { { "Service", "Authenticate" }, { "Method", "Logout" } };

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
            this.Delegate?.LogoutServerOperationDidFail(this, error);
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json response.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            this.Delegate?.LogoutServerOperationDidFinish(this);
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
