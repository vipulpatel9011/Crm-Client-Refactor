// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QlikViewAuthenticateServerOperation.cs" company="Aurea Software Gmbh">
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
//   UPQlikViewAuthenticateServerOperation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// UPQlikViewAuthenticateServerOperationDelegate
    /// </summary>
    public interface UPQlikViewAuthenticateServerOperationDelegate
    {
        /// <summary>
        /// Qliks the view server operation did finish did finish with json response.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="json">The json.</param>
        void QlikViewServerOperationDidFinishDidFinishWithJSONResponse(UPQlikViewAuthenticateServerOperation operation, Dictionary<string, object> json);

        /// <summary>
        /// Qliks the view authenticate server operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        void QlikViewAuthenticateServerOperationDidFailWithError(UPQlikViewAuthenticateServerOperation operation, Exception error);
    }

    /// <summary>
    /// UPQlikViewAuthenticateServerOperation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class UPQlikViewAuthenticateServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPQlikViewAuthenticateServerOperation"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="documentName">Name of the document.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPQlikViewAuthenticateServerOperation(string server, string userName, string documentName, UPQlikViewAuthenticateServerOperationDelegate theDelegate)
            : this(server, userName, documentName, null, null, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPQlikViewAuthenticateServerOperation"/> class.
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="documentName">Name of the document.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="loginMethod">The login method.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPQlikViewAuthenticateServerOperation(string server, string userName, string documentName, string scheme, string loginMethod, UPQlikViewAuthenticateServerOperationDelegate theDelegate)
        {
            this.TheDelegate = theDelegate;
            this.Server = server;
            this.UserName = userName;
            this.DocumentName = documentName;
            this.Scheme = scheme ?? "http";
            this.LoginMethod = loginMethod ?? string.Empty;
            this.AlwaysPerform = true;
        }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPQlikViewAuthenticateServerOperationDelegate TheDelegate { get; set; }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public string Server { get; private set; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the name of the document.
        /// </summary>
        /// <value>
        /// The name of the document.
        /// </value>
        public string DocumentName { get; private set; }

        /// <summary>
        /// Gets the scheme.
        /// </summary>
        /// <value>
        /// The scheme.
        /// </value>
        public string Scheme { get; private set; }

        /// <summary>
        /// Gets the login method.
        /// </summary>
        /// <value>
        /// The login method.
        /// </value>
        public string LoginMethod { get; private set; }

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
                Dictionary<string, string> parameterDictionary = new Dictionary<string, string>
                {
                    ["Service"] = "Authenticate",
                    ["Method"] = "QlikViewAuthenticate",
                    ["server"] = this.Server,
                    ["userlogin"] = this.UserName,
                    ["documentname"] = this.DocumentName,
                    ["connectiontype"] = this.Scheme,
                    ["loginMethod"] = this.LoginMethod
                };
                return parameterDictionary;
            }
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="remoteData">The remote data.</param>
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            this.TheDelegate.QlikViewServerOperationDidFinishDidFinishWithJSONResponse(this, json);
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
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="remoteData">The remote data.</param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.TheDelegate.QlikViewAuthenticateServerOperationDidFailWithError(this, error);
        }

        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">The remote data.</param>
        /// <exception cref="NotImplementedException"></exception>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
