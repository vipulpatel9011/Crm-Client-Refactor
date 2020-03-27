// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationManagerService.cs" company="Aurea Software Gmbh">
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
//   Interface for an operation manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OperationHandling
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    using Aurea.CRM.Core.OperationHandling.Authentication;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Interface for an operation manager
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.IRemoteDataDelegate" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.ILogoutServerOperationDelegate" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.IAuthenticateServerOperationDelegate" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.IRevolutionCookieServerOperationDelegate" />
    /// <seealso cref="System.IDisposable" />
    public interface IOperationManagerService : IRemoteDataDelegate,
                                                ILogoutServerOperationDelegate,
                                                IAuthenticateServerOperationDelegate,
                                                IRevolutionCookieServerOperationDelegate,
                                                IDisposable
    {
    }

    /// <summary>
    /// Defines remote data handling related functions
    /// </summary>
    public interface IRemoteDataDelegate
    {
        /// <summary>
        /// Remotes the data did fail loading with error.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void RemoteDataDidFailLoadingWithError(RemoteData remoteData, Exception error);

        /// <summary>
        /// Remotes the data did finish loading.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        void RemoteDataDidFinishLoading(RemoteData remoteData);

        /// <summary>
        /// Progress changed event handler
        /// </summary>
        /// <param name="progress">Progress value</param>
        /// <param name="total">Total value</param>
        void RemoteDataProgressChanged(ulong progress, ulong total);
    }

    /// <summary>
    /// Defines logout operation related events
    /// </summary>
    public interface ILogoutServerOperationDelegate
    {
        /// <summary>
        /// Logouts the server operation did fail.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void LogoutServerOperationDidFail(LogoutServerOperation operation, Exception error);

        /// <summary>
        /// Logouts the server operation did finish.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        void LogoutServerOperationDidFinish(LogoutServerOperation operation);
    }

    /// <summary>
    /// Defines authentication related events
    /// </summary>
    public interface IAuthenticateServerOperationDelegate
    {
        /// <summary>
        /// Authenticates the server operation did fail.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void AuthenticateServerOperationDidFail(AuthenticateServerOperation operation, Exception error);

        /// <summary>
        /// Authenticates the server operation did finish.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        void AuthenticateServerOperationDidFinish(
            AuthenticateServerOperation operation,
            Dictionary<string, object> json);
    }

    /// <summary>
    /// Defines Revolution authentication related events
    /// </summary>
    public interface IRevolutionCookieServerOperationDelegate
    {
        /// <summary>
        /// Authenticates the revolution cookie server operation did fail.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void AuthenticateRevolutionCookieServerOperationDidFail(
            RevolutionCookieServerOperation operation,
            Exception error);

        /// <summary>
        /// Authenticates the revolution cookie server operation did finish.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="cookies">
        /// The cookies.
        /// </param>
        /// <param name="serverURL">
        /// The server URL.
        /// </param>
        /// <param name="headerParameters">
        /// The header parameters.
        /// </param>
        void AuthenticateRevolutionCookieServerOperationDidFinish(
            RevolutionCookieServerOperation operation,
            List<Cookie> cookies,
            Uri serverURL,
            Dictionary<string, string> headerParameters);
    }
}
