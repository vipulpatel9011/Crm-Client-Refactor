// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteSession.cs" company="Aurea Software Gmbh">
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
//   Remote session delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Remote session delegate interface
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.IOperationManagerDelegate" />
    public interface IRemoteSessionDelegate : IOperationManagerDelegate
    {
    }

    /// <summary>
    /// Implements the remote server session functionality
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.IOperationManagerDelegate" />
    public class RemoteSession : IOperationManagerDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteSession"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="passwordNew">
        /// The password new.
        /// </param>
        /// <param name="rasInstanceName">
        /// Name of the ras instance.
        /// </param>
        /// <param name="isDefault">
        /// if set to <c>true</c> [is default].
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        /// <param name="userChoiceOffline">
        /// if set to <c>true</c> [user choice offline].
        /// </param>
        public RemoteSession(
            RemoteServer server,
            string languageKey,
            string username,
            string password,
            string passwordNew,
            string rasInstanceName,
            bool isDefault,
            IRemoteSessionDelegate theDelegate,
            bool userChoiceOffline)
        {
            this.Delegate = theDelegate;
            this.IsDefault = isDefault;
            this.ServerOperationManager = new ServerOperationManager(
                server,
                languageKey,
                username,
                password,
                passwordNew,
                rasInstanceName,
                this,
                this,
                userChoiceOffline);

            if (!this.IsDefault)
            {
                this.CookieStorage = new CookieStorage();
            }
        }

        /// <summary>
        /// Gets the cookie storage.
        /// </summary>
        /// <value>
        /// The cookie storage.
        /// </value>
        public CookieStorage CookieStorage { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IRemoteSessionDelegate Delegate { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is logged in.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is logged in; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoggedIn { get; private set; }

        /// <summary>
        /// Gets the server operation manager.
        /// </summary>
        /// <value>
        /// The server operation manager.
        /// </value>
        public ServerOperationManager ServerOperationManager { get; private set; }

        /// <summary>
        /// Changes the user choice offline.
        /// </summary>
        /// <param name="userChoice">
        /// if set to <c>true</c> [user choice].
        /// </param>
        public void ChangeUserChoiceOffline(bool userChoice)
        {
            this.ServerOperationManager.UserChoiceOffline = userChoice;
            if (!userChoice)
            {
                this.Login();
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.ServerOperationManager.Clear();
        }

        /// <summary>
        /// Logins this instance.
        /// </summary>
        public void Login()
        {
            this.ServerOperationManager.Login();
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        public void Logout()
        {
            this.ServerOperationManager.Logout();
            this.IsLoggedIn = false;
        }

        /// <summary>
        /// Servers the operation manager did fail server login.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerDidFailServerLogin(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChanged)
        {
            this.IsLoggedIn = false;
            this.Delegate?.ServerOperationManagerDidFailServerLogin(serverOperationManager, error, passwordChanged);
        }

        /// <summary>
        /// Servers the operation manager did fail with internet connection error.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChangeResult">
        /// The password change result.
        /// </param>
        public void ServerOperationManagerDidFailWithInternetConnectionError(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChangeResult)
        {
            this.IsLoggedIn = false;
            this.Delegate?.ServerOperationManagerDidFailWithInternetConnectionError(
                serverOperationManager,
                error,
                passwordChangeResult);
        }

        /// <summary>
        /// Servers the operation manager did perform server login.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="availableLanguages">
        /// The available languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        /// <param name="serverInfo">
        /// The server information.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerDidPerformServerLogin(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged)
        {
            this.IsLoggedIn = true;

            this.Delegate?.ServerOperationManagerDidPerformServerLogin(
                serverOperationManager,
                availableLanguages,
                sessionAttributes,
                serverInfo,
                passwordChanged);
        }

        /// <summary>
        /// Servers the operation manager password change requested.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="isRequested">
        /// if set to <c>true</c> [is requested].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ServerOperationManagerPasswordChangeRequested(
            ServerOperationManager serverOperationManager,
            bool isRequested)
        {
            return this.Delegate != null
                   && this.Delegate.ServerOperationManagerPasswordChangeRequested(serverOperationManager, isRequested);
        }

        /// <summary>
        /// Servers the operation manager requires language for session.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="availableLanguages">
        /// The available languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        /// <param name="serverInfo">
        /// The server information.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerRequiresLanguageForSession(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged)
        {
            this.Delegate?.ServerOperationManagerRequiresLanguageForSession(
                serverOperationManager,
                availableLanguages,
                sessionAttributes,
                serverInfo,
                passwordChanged);
        }
    }
}
