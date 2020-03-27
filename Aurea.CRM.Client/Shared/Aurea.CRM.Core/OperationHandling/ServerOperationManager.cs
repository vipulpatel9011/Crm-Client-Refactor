// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerOperationManager.cs" company="Aurea Software Gmbh">
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
//   The server operation manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling.Authentication;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;

    using GalaSoft.MvvmLight.Messaging;

    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using System.Threading;

    /// <summary>
    /// The server operation manager.
    /// </summary>
    public class ServerOperationManager : IDisposable,
                                          IRemoteDataDelegate,
                                          IAuthenticateServerOperationDelegate,
                                          IRevolutionCookieServerOperationDelegate,
                                          ILogoutServerOperationDelegate
    {

        /// <summary>
        /// The application restarting retry counter.
        /// </summary>
        private int applicationRestartingRetryCounter;

        /// <summary>
        /// The crm server.
        /// </summary>
        private RemoteServer crmServer;

        /// <summary>
        /// The current remote data request.
        /// </summary>
        private RemoteData currentRemoteDataRequest;

        /// <summary>
        /// The current remote session.
        /// </summary>
        private RemoteSession currentRemoteSession;

        /// <summary>
        /// The current server operation.
        /// </summary>
        private ServerOperation currentServerOperation;

        /// <summary>
        /// The language key.
        /// </summary>
        private string languageKey;

        /// <summary>
        /// The login name.
        /// </summary>
        private string loginName;

        /// <summary>
        /// The new password.
        /// </summary>
        private string newPassword;

        /// <summary>
        /// The remote session.
        /// </summary>
        private RemoteSession remoteSession;

        /// <summary>
        /// The server operations.
        /// </summary>
        private List<Operation> serverOperations;

        /// <summary>
        /// The starting up.
        /// </summary>
        private bool startingUp;

        /// <summary>
        /// The suspended.
        /// </summary>
        private bool suspended;

        /// <summary>
        /// true if the sync operation cancel is requested
        /// </summary>
        private bool operationCancelRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOperationManager"/> class.
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
        /// <param name="remoteSession">
        /// The remote session.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        /// <param name="userChoiceOffline">
        /// if set to <c>true</c> [user choice offline].
        /// </param>
        public ServerOperationManager(
            RemoteServer server,
            string languageKey,
            string username,
            string password,
            string passwordNew,
            string rasInstanceName,
            RemoteSession remoteSession,
            IOperationManagerDelegate theDelegate,
            bool userChoiceOffline)
        {
            this.crmServer = server;
            this.languageKey = languageKey;
            this.loginName = username;
            this.Password = password;
            this.Delegate = theDelegate;
            this.remoteSession = remoteSession;
            this.RasInstanceName = rasInstanceName;
            this.UserChoiceOffline = userChoiceOffline;
            this.newPassword = passwordNew;
            this.serverOperations = new List<Operation>();
            this.startingUp = true;
            this.ConnectionWatchDog = SimpleIoc.Default.GetInstance<IConnectionWatchdog>()?.ConnectionWatchDogForServerURL(server.OriginalServerUrl);

            // Set the credentials
            switch (this.crmServer.AuthenticationType)
            {
                case ServerAuthenticationType.Revolution:
                    break;

                case ServerAuthenticationType.SSO:
                    this.loginName = null;
                    this.Password = null;
                    break;

                case ServerAuthenticationType.SSOCredentialNoCache:
                case ServerAuthenticationType.SSOCredentialSessionCache:
                    this.ServerRequestCredentials = new NetworkCredential(this.loginName, this.Password);
                    this.loginName = null;
                    this.Password = null;
                    break;

                case ServerAuthenticationType.UsernamePassword:
                    break;

                case ServerAuthenticationType.UsernamePasswordCredentials:
                    this.ServerRequestCredentials = new NetworkCredential
                    {
                        UserName = this.crmServer.NetworkUsername,
                        Password = this.crmServer.NetworkPassword
                    };
                    break;
            }

            Messenger.Default.Register<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidEstablishServerConnectivity, this.ConnectionWatchDogDidEstablishServerConnectivity);
            Messenger.Default.Register<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidLooseServerConnectivity, this.ConnectionWatchDogDidLooseServerConnectivity);
            Messenger.Default.Register<ConnectionWatchDogMessage>(this, ConnectionWatchDogMessageKey.DidChangeConnectivityQuality, this.ConnectionWatchDogDidChangeConnectivityQuality);

            Task.Factory.StartNew(this.StopStartingUp);
        }

        /// <summary>
        /// Gets a value indicating whether [connected to server].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connected to server]; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectedToServer
            =>
                this.ConnectionWatchDog != null && this.ConnectionWatchDog.HasInternetConnection
                && this.ConnectionWatchDog.HasServerConnection && this.DisallowOnlineRequests == false
                && !this.UserChoiceOffline;

        /// <summary>
        /// Gets a value indicating whether [connected to server for full synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [connected to server for full synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectedToServerForFullSync => this.IsConnectedToServer;

        /// <summary>
        /// Gets or sets the connection watch dog.
        /// </summary>
        /// <value>
        /// The connection watch dog.
        /// </value>
        public IConnectionWatchdog ConnectionWatchDog { get; set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IOperationManagerDelegate Delegate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disallow online requests].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disallow online requests]; otherwise, <c>false</c>.
        /// </value>
        public bool DisallowOnlineRequests { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to server.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected to server; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnectedToServer
            => this.ConnectionWatchDog != null && this.ConnectionWatchDog.HasInternetConnection
                && this.ConnectionWatchDog.HasServerConnection && !this.UserChoiceOffline;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the ras application identifier.
        /// </summary>
        /// <value>
        /// The ras application identifier.
        /// </value>
        public string RasApplicationId { get; private set; }

        /// <summary>
        /// Gets the name of the ras instance.
        /// </summary>
        /// <value>
        /// The name of the ras instance.
        /// </value>
        public string RasInstanceName { get; private set; }

        /// <summary>
        /// Gets the ras username.
        /// </summary>
        /// <value>
        /// The ras username.
        /// </value>
        public string RasUsername { get; private set; }

        /// <summary>
        /// Gets the server request credentials.
        /// Additional RAS2 Parameters Needed to build a unique Application base path
        /// e.g. "LIVE" or "TEST"
        /// e.g EE7E5616-6B4A-4382-B74B-629C726D58FF
        /// RAS Username withot the Instance prefix
        /// </summary>
        /// <value>
        /// The server request credentials.
        /// </value>
        public NetworkCredential ServerRequestCredentials { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user choice offline].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user choice offline]; otherwise, <c>false</c>.
        /// </value>
        public bool UserChoiceOffline { get; set; }

        /// <summary>
        /// The revolution cookie authentication server operation did fail.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void AuthenticateRevolutionCookieServerOperationDidFail(
            RevolutionCookieServerOperation operation,
            Exception error)
        {
            if (error != null)
            {
                this.Logger.LogError($"AuthenticateRevolutionCookieServerOperation {error.Message}");
            }

            this.CleanUpFinishedServerOperation();
            PasswordChangeResult passwordChangeResult;
            if (!string.IsNullOrEmpty(this.newPassword))
            {
                this.newPassword = null;
                passwordChangeResult = PasswordChangeResult.PasswordNotChanged;
            }
            else
            {
                passwordChangeResult = PasswordChangeResult.NoPasswordChangeRequested;
            }

            this.Delegate?.ServerOperationManagerDidFailServerLogin(this, error, passwordChangeResult);
        }

        /// <summary>
        /// The revolution cookie authentication server operation did finish.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="cookies">
        /// The cookies.
        /// </param>
        /// <param name="serverUrl">
        /// The server URL.
        /// </param>
        /// <param name="headerParameters">
        /// The header parameters.
        /// </param>
        public void AuthenticateRevolutionCookieServerOperationDidFinish(
            RevolutionCookieServerOperation operation,
            List<Cookie> cookies,
            Uri serverUrl,
            Dictionary<string, string> headerParameters)
        {
            this.crmServer.ServerUrl = serverUrl;
            var dictionary = ServerSession.CurrentLocalJsonConfigDictionary();
            var newPasswordRequired = string.Empty;
            if (!dictionary.ValueEquals("oldRasHandling", "true"))
            {
                this.RasUsername = headerParameters.ValueOrDefault("Ras-User-Email");
                this.RasApplicationId = headerParameters.ValueOrDefault("Ras-App-Id");
                this.RasInstanceName = headerParameters.ValueOrDefault("Ras-App-Type");
                newPasswordRequired = headerParameters.ValueOrDefault("Ras-User-PwChange");
            }
            else
            {
                this.Logger.LogDebug("using oldRasHandling", LogFlag.LogNetwork);
            }

            // newPassword.length != 0 --> A new password will be transmitted
            if (newPasswordRequired != null && newPasswordRequired == "true" && string.IsNullOrEmpty(this.newPassword))
            {
                var mandatory = this.startingUp;
                if (!mandatory)
                {
                    this.Delegate?.ServerOperationManagerPasswordChangeRequested(this, true);
                }

                if (mandatory)
                {
                    // var error = NSError.ErrorWithDomainCodeUserInfo("New password required",SessionErrorCode.PasswordExpired, NSDictionary.DictionaryWithObjectForKey(newPasswordRequired, kUPErrorRASErrorPasswordExpired));
                    this.Delegate?.ServerOperationManagerDidFailServerLogin(
                        this,
                        new Exception("New password required"),
                        PasswordChangeResult.NoPasswordChangeRequested);
                    return;
                }
            }
            else
            {
                this.Delegate?.ServerOperationManagerPasswordChangeRequested(this, false);
            }

            if (string.IsNullOrEmpty(this.RasApplicationId))
            {
                // Fallback for Old Ras version
                this.RasUsername = this.loginName;
                this.RasInstanceName = null;

                // Bei nÃ¤chsten login mit Header Parameter InstanzName Ã¼bernehmen
                this.RasApplicationId = this.crmServer.Name;
                this.Logger.LogDebug("ServerOperationManager - No RAS Header Parameter", LogFlag.LogNetwork);
            }
            else
            {
                this.Logger.LogDebug(
                    $"ServerOperationManager - RAS AppId: {this.RasApplicationId} instanceName: {this.RasInstanceName}", LogFlag.LogNetwork);
            }

            var revolutionLoginOperation = new RevolutionAuthenticationServerOperation(
                cookies,
                true,
                true,
                this.languageKey,
                this);
            if (!string.IsNullOrEmpty(this.newPassword))
            {
                revolutionLoginOperation.ChangePasswordTo(this.newPassword);
            }

            this.QueueServerOperationAtTopOfTheQueue(revolutionLoginOperation);
        }

        /// <summary>
        /// The Authentication server operation did fail.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void AuthenticateServerOperationDidFail(AuthenticateServerOperation operation, Exception error)
        {
            if (error != null)
            {
                this.Logger.LogError($"AuthenticateServerOperation {error.Message}");
            }

            this.CleanUpFinishedServerOperation();
            PasswordChangeResult passwordChangeResult;
            if (!string.IsNullOrEmpty(this.newPassword))
            {
                this.newPassword = null;
                passwordChangeResult = PasswordChangeResult.PasswordNotChanged;
            }
            else
            {
                passwordChangeResult = PasswordChangeResult.NoPasswordChangeRequested;
            }

            this.Delegate?.ServerOperationManagerDidFailServerLogin(this, error, passwordChangeResult);
        }

        /// <summary>
        /// The Authentication server operation did finish.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="json">
        /// The json.
        /// </param>
        public void AuthenticateServerOperationDidFinish(AuthenticateServerOperation operation, Dictionary<string, object> json)
        {
            if (json == null)
            {
                return;
            }

            var passwordChanged = json.ContainsKey("passwordChanged") ? (bool?)json["passwordChanged"] : null;
            PasswordChangeResult passwordChangeResult;
            if (!string.IsNullOrEmpty(this.newPassword))
            {
                if (!passwordChanged.HasValue)
                {
                    passwordChangeResult = PasswordChangeResult.PasswordChangeNotSupported;
                }
                else
                {
                    passwordChangeResult = passwordChanged.Value
                                               ? PasswordChangeResult.PasswordChanged
                                               : PasswordChangeResult.PasswordNotChanged;
                }
            }
            else
            {
                passwordChangeResult = PasswordChangeResult.NoPasswordChangeRequested;
            }

            var statusInfo = json.ContainsKey("StatusInfo") ? (List<object>)json["StatusInfo"] : null;
            if (statusInfo != null && ((string)statusInfo[1]).Equals("Error"))
            {
                this.CleanUpFinishedServerOperation();

                // var error = NSError.ErrorFromServerErrorResponse(statusInfo);
                this.Delegate?.ServerOperationManagerDidFailServerLogin(
                    this,
                    new Exception("ErrorFromServerErrorResponse"),
                    passwordChangeResult);
                return;
            }

            List<ServerLanguage> availableServerLanguages = null;
            var serverInformation = (List<object>)json.ValueOrDefault("serverinfo");

            var serverLanguageInformation = (List<object>)serverInformation?[2];
            if (serverLanguageInformation != null)
            {
                availableServerLanguages = new List<ServerLanguage>();

                foreach (List<object> lang in serverLanguageInformation)
                {
                    availableServerLanguages.Add(
                        new ServerLanguage(
                            (string)lang[0],
                            (string)lang[1],
                            (int)lang[2],
                            (int)lang[3],
                            (string)lang[4]));
                }
            }

            Dictionary<string, object> sessionAttributeDictionary = null;
            var sessionInfo = (List<object>)json.ValueOrDefault("sessioninfo");
            var serverUserDefaultLanguage = string.Empty;
            if (sessionInfo?.Count > 1)
            {
                if (sessionInfo[0] != null)
                {
                    serverUserDefaultLanguage = (string)sessionInfo[0];
                }

                var sessionInfoAttributes = (List<object>)sessionInfo[1];
                sessionAttributeDictionary = this.AttributeDictionaryForSession(
                    sessionInfoAttributes,
                    serverUserDefaultLanguage);
            }

            if (sessionAttributeDictionary == null)
            {
                sessionAttributeDictionary = new Dictionary<string, object>();
            }

            if (!string.IsNullOrEmpty(this.newPassword) && passwordChanged != null)
            {
                this.Password = this.newPassword;
                this.newPassword = null;
            }

            if (this.languageKey == null)
            {
                this.suspended = true;
                this.Delegate?.ServerOperationManagerRequiresLanguageForSession(
                    this,
                    availableServerLanguages,
                    sessionAttributeDictionary,
                    serverInformation,
                    passwordChangeResult);
            }
            else
            {
                this.Delegate?.ServerOperationManagerDidPerformServerLogin(
                    this,
                    availableServerLanguages,
                    sessionAttributeDictionary,
                    serverInformation,
                    passwordChangeResult);
            }
        }

        /// <summary>
        /// Cancels all operations.
        /// </summary>
        public void CancelAllOperations()
        {
            if (this.serverOperations == null)
            {
                return;
            }

            foreach (var serverOperation in this.serverOperations)
            {
                serverOperation.Cancel();
            }
        }

        /// <summary>
        /// Cleans up finished server operation.
        /// </summary>
        public void CleanUpFinishedServerOperation()
        {
            this.CleanUpFinishedServerOperationStartNextOperation(true);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Performs when Connections watch dog did change connectivity quality.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public void ConnectionWatchDogDidChangeConnectivityQuality(object notification)
        {
            //WillChangeValueForKey("connectedToServer");
            //DidChangeValueForKey("connectedToServer");
        }

        /// <summary>
        /// Performs when Connection watch dog did establish server connectivity.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public void ConnectionWatchDogDidEstablishServerConnectivity(object notification)
        {
            this.Login();

            // WillChangeValueForKey("connectedToServer");
            // DidChangeValueForKey("connectedToServer");
        }

        /// <summary>
        /// Performs when Connections watch dog did loose server connectivity.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public void ConnectionWatchDogDidLooseServerConnectivity(object notification)
        {
            // WillChangeValueForKey("connectedToServer");
            // DidChangeValueForKey("connectedToServer");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Clear();
        }

        /// <summary>
        /// Fails the current server operation because of missing internet connection.
        /// </summary>
        public void FailCurrentServerOperationBecauseOfMissingInternetConnection()
        {
            if (this.currentServerOperation == null)
            {
                return;
            }

            if (this.currentServerOperation is AuthenticateServerOperation
                || this.currentServerOperation is RevolutionCookieServerOperation)
            {
                this.Delegate?.ServerOperationManagerDidFailWithInternetConnectionError(
                    this,
                    null,
                    !string.IsNullOrEmpty(this.newPassword) ? PasswordChangeResult.PasswordNotChanged : PasswordChangeResult.NoPasswordChangeRequested);
            }
            else
            {
                this.currentServerOperation.ProcessFailedDueToInternetConnection(
                    new Exception("ConnectionOfflineError"),
                    null);
            }

            this.currentServerOperation.FinishProcessing();
            this.CleanUpFinishedServerOperation();
        }

        /// <summary>
        /// Performs the login operation and Start processing the operation queue.
        /// </summary>
        public void Login()
        {
            if (this.serverOperations == null)
            {
                return;
            }

            if (this.ConnectionWatchDog != null
                && (this.ConnectionWatchDog.HasServerConnection == false || this.ConnectionWatchDog.HasInternetConnection == false))
            {
                this.TimeoutLogin();
                return;
            }

            var alreadyQueued = false;
            var queuePosition = 0;
            foreach (var operation in this.serverOperations)
            {
                if (operation is AuthenticateServerOperation)
                {
                    alreadyQueued = true;
                    break;
                }

                if (operation is RevolutionCookieServerOperation)
                {
                    alreadyQueued = true;
                    break;
                }

                ++queuePosition;
            }

            if (alreadyQueued)
            {
                if ((this.currentServerOperation == null && queuePosition > 0) || queuePosition > 1)
                {
                    var serverOperation = this.serverOperations[queuePosition];
                    this.serverOperations.RemoveAt(queuePosition);
                    this.serverOperations.Insert(this.currentServerOperation != null ? 1 : 0, serverOperation);
                }

                this.StartNextOperation();
            }
            else
            {
                ServerOperation serverOperation;
                if (this.crmServer.AuthenticationType == ServerAuthenticationType.Revolution)
                {
                    string usedRasUsername;
                    if (!string.IsNullOrEmpty(this.RasInstanceName)
                        && this.loginName.IndexOf("\\", StringComparison.OrdinalIgnoreCase) < 0 && !this.startingUp)
                    {
                        this.Logger.LogDebug($"ServerOperationManager RAS relogon with prefix: {this.RasInstanceName}", LogFlag.LogNetwork);
                        usedRasUsername = $"{this.RasInstanceName}\\{this.loginName}";
                    }
                    else
                    {
                        this.Logger.LogDebug($"ServerOperationManager RAS logon with loginName: {this.loginName}", LogFlag.LogNetwork);
                        usedRasUsername = this.loginName;
                    }

                    serverOperation = new RevolutionCookieServerOperation(usedRasUsername, this.Password, this);
                }
                else
                {
                    serverOperation = new AuthenticateServerOperation(
                        this.loginName,
                        this.Password,
                        true,
                        true,
                        this.languageKey,
                        this);
                    if (!string.IsNullOrEmpty(this.newPassword))
                    {
                        ((AuthenticateServerOperation)serverOperation).ChangePasswordTo(this.newPassword);
                    }
                }

                this.QueueServerOperationAtTopOfTheQueue(serverOperation);
            }
        }

        /// <summary>
        /// Logins the with language key.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        public void LoginWithLanguageKey(string languageKey)
        {
            this.languageKey = languageKey;
            this.suspended = false;
            this.Logout();
            this.Login();
        }

        /// <summary>
        /// Logouts this session.
        /// </summary>
        public void Logout()
        {
            var logoutOperation = new LogoutServerOperation(this);
            this.QueueServerOperationAtTopOfTheQueue(logoutOperation);
        }

        /// <summary>
        /// Triggers when logouts from the server operation did fail.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void LogoutServerOperationDidFail(LogoutServerOperation operation, Exception error)
        {
            if (error != null)
            {
                this.Logger.LogError($" {error.Message}");
            }
        }

        /// <summary>
        /// Triggers when logouts from the server.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        public void LogoutServerOperationDidFinish(LogoutServerOperation operation)
        {
        }

        /// <summary>
        /// Observes the value for key path of object change context.
        /// </summary>
        /// <param name="keyPath">
        /// The key path.
        /// </param>
        /// <param name="theObject">
        /// The object.
        /// </param>
        /// <param name="change">
        /// The change.
        /// </param>
        public void ObserveValueForKeyPathOfObjectChangeContext(
            string keyPath,
            object theObject,
            Dictionary<string, string> change)
        {
            if (keyPath != "cancelled")
            {
                return;
            }

            this.currentRemoteDataRequest.Cancel();
            this.CleanUpFinishedServerOperation();
        }

        /// <summary>
        /// Queues the server operation.
        /// </summary>
        /// <param name="serverOperation">
        /// The server operation.
        /// </param>
        public void QueueServerOperation(ServerOperation serverOperation)
        {
            if (this.serverOperations == null)
            {
                return;
            }

            lock (this.serverOperations)
            {
                this.serverOperations.Add(serverOperation);
            }

            this.StartNextOperation();
        }

        /// <summary>
        /// Queues the server operation at top of the queue.
        /// </summary>
        /// <param name="serverOperation">
        /// The server operation.
        /// </param>
        public void QueueServerOperationAtTopOfTheQueue(ServerOperation serverOperation)
        {
            if (this.serverOperations == null)
            {
                return;
            }

            // #82360: retain remote session until RemoteData object is created
            this.currentRemoteSession = this.remoteSession;

            lock (this.serverOperations)
            {
                this.serverOperations.Insert(this.currentServerOperation != null ? 1 : 0, serverOperation);
            }

            this.StartNextOperation();
        }

        /// <summary>
        /// Resets the cancelled sync operation
        /// </summary>
        public void Reset()
        {
            this.operationCancelRequested = false;
        }

        /// <summary>
        /// Stops the current sync in progress
        /// </summary>
        public void Stop()
        {
            if (this.currentServerOperation != null)
            {
                this.operationCancelRequested = true;
                this.serverOperations.Remove(this.currentServerOperation);
                this.currentServerOperation = null;
                this.currentRemoteDataRequest = null;
            }
        }

        /// <summary>
        /// Triggers when remotes data did fail while loading.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void RemoteDataDidFailLoadingWithError(RemoteData remoteData, Exception error)
        {
            if (error.IsApplicationStartingError())
            {
                this.applicationRestartingRetryCounter++;
                if (this.applicationRestartingRetryCounter == 5)
                {
                    this.applicationRestartingRetryCounter = 0;
                    this.currentServerOperation?.ProcessFailedDueToInternetConnection(error, remoteData);
                    this.currentServerOperation?.FinishProcessing();
                    this.CleanUpFinishedServerOperation();
                }
                else
                {
                    this.CleanRemoteData();
                    this.suspended = true;
                    this.TriggerRetryForRemoteOperationForCurrentServerOperation();
                }
            }
            else if (error.IsNotAuthenticatedError()
                     && !(this.currentServerOperation is AuthenticateServerOperation)
                     && !(this.currentServerOperation is LogoutServerOperation)
                     && !(this.currentServerOperation is RevolutionCookieServerOperation))
            {
                this.CleanRemoteData();
                this.currentServerOperation = null;
                this.Login();
            }
            else if (error.Message == ".Code == UP_NO_DATA_RETURNED")
            {
                if (this.currentServerOperation is AuthenticateServerOperation
                    || this.currentServerOperation is RevolutionCookieServerOperation)
                {
                    this.Delegate?.ServerOperationManagerDidFailWithInternetConnectionError(
                        this,
                        error,
                        !string.IsNullOrEmpty(this.newPassword) ? PasswordChangeResult.PasswordNotChanged : PasswordChangeResult.NoPasswordChangeRequested);
                }
                else
                {
                    this.currentServerOperation.ProcessFailedDueToInternetConnection(error, remoteData);
                }

                this.currentServerOperation.FinishProcessing();
                this.CleanUpFinishedServerOperation();
            }
            else
            {
                this.currentServerOperation?.ProcessErrorWithRemoteData(error, remoteData);
                this.currentServerOperation?.FinishProcessing();
                this.CleanUpFinishedServerOperation();
            }
        }

        /// <summary>
        /// Triggers when remotes data did finish loading.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public void RemoteDataDidFinishLoading(RemoteData remoteData)
        {
            if (this.operationCancelRequested)
            {
                this.operationCancelRequested = false;
                return;
            }

            try
            {
                if(this.currentServerOperation is SyncRequestServerOperation && ((SyncRequestServerOperation)this.currentServerOperation).DataSets != null)
                {
                    string responseData = null;
                    if (remoteData.Data != null)
                    {
                        responseData = Encoding.UTF8.GetString(remoteData.Data, 0, remoteData.Data.Length);
                    }

                    GC.GetTotalMemory(false);

                    var syncObject = responseData?.DataSyncObjectFromString();

                    ((JsonResponseServerOperation)this.currentServerOperation).ProcessJsonSyncObject(
                            syncObject,
                            remoteData);
                }
                else if (!(this.currentServerOperation is UploadDocumentServerOperation) && this.currentServerOperation is JsonResponseServerOperation)
                {
                    string responseData = null;
                    if (remoteData.Data != null)
                    {
                        responseData = Encoding.UTF8.GetString(remoteData.Data, 0, remoteData.Data.Length);
                    }

                    var jsonDictionary = this.currentServerOperation is SyncRequestServerOperation
                                             ? responseData?.JsonDictionaryFromString(false)
                                             : responseData?.JsonDictionaryFromString();

                    if (jsonDictionary != null)
                    {
                        if (jsonDictionary.ContainsKey("PendingKey"))
                        {
                            remoteData.HandlePending(jsonDictionary);
                            return;
                        }

                        GC.GetTotalMemory(false);

                        ((JsonResponseServerOperation)this.currentServerOperation).ProcessJsonResponse(
                            jsonDictionary,
                            remoteData);
                    }
                    else
                    {
                        if (responseData?.IndexOf("update Login", StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            this.RemoteDataDidFailLoadingWithError(remoteData, new Exception("NotAuthenticatedError"));
                            return;
                        }

                        if (responseData.StartsWith("empty"))
                        {
                            if (remoteData.RetryOnce())
                            {
                                return;
                            }
                        }

                        var error = new Exception($"Invalid json response: {responseData}");
                        ((JsonResponseServerOperation)this.currentServerOperation).ProcessInvalidJsonError(
                            error,
                            remoteData);
                    }
                }
                else
                {
                    this.currentServerOperation?.ProcessRemoteData(remoteData);
                }

                this.currentServerOperation?.FinishProcessing();
                this.CleanUpFinishedServerOperation();
            }
            catch (Exception dataFinishedLoadingException)
            {
                this.Logger.LogError(dataFinishedLoadingException);

                // If server operation was cancelled, but remote data processing already started
                // handle exception and proceeed
                // If exception happens not during cancellation, throw
                if (this.operationCancelRequested)
                {
                    this.operationCancelRequested = false;
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public void RemoteDataProgressChanged(ulong progress, ulong total)
        {
        }

        /// <summary>
        /// Starts the next operation.
        /// </summary>
        public void StartNextOperation()
        {
            if (this.serverOperations == null || this.operationCancelRequested)
            {
                return;
            }

            if (this.serverOperations.Count == 0 || this.currentServerOperation != null)
            {
                // || suspended)
                return;
            }

            var cancelledOperations = this.serverOperations.Where(serverOperation => serverOperation.Canceled).ToList();
            foreach (var cancelledOperation in cancelledOperations)
            {
                this.serverOperations.Remove(cancelledOperation);
            }

            if (!this.serverOperations.Any())
            {
                return;
            }

            this.currentServerOperation = this.serverOperations[0] as ServerOperation;
            if (this.currentServerOperation == null)
            {
                return;
            }

#if PORTING
            CurrentServerOperation.AddObserverForKeyPathOptionsContext(this, "cancelled", NSKeyValueObservingOptionNew, null);
#endif
            this.applicationRestartingRetryCounter = 0;

            if (!this.currentServerOperation.AlwaysPerform && this.currentServerOperation.BlockedByPendingUpSync
                && UPOfflineStorage.DefaultStorage.BlockOnlineRecordRequest)
            {
                this.FailCurrentServerOperationBecauseOfMissingInternetConnection();
                return;
            }

            this.StartRemoteOperationForCurrentServerOperation();
        }

        /// <summary>
        /// Starts the remote operation for current server operation.
        /// </summary>
        public void StartRemoteOperationForCurrentServerOperation()
        {
            if (!this.IsConnectedToServer || (this.DisallowOnlineRequests && !this.currentServerOperation.AlwaysPerform))
            {
                // if (LogSettings.LogNetwork())
                // {
                // DDLogCSQL("no connection to server: %@", CrmServer.OriginalServerURL);
                // }
                this.Logger?.LogDebug($"no connection to server:", LogFlag.LogNetwork);
                this.FailCurrentServerOperationBecauseOfMissingInternetConnection();
                return;
            }

            if (this.serverOperations == null)
            {
                return;
            }

            if (this.currentServerOperation == null || this.currentRemoteDataRequest != null)
            {
                return;
            }

            var url = this.currentServerOperation is RevolutionCookieServerOperation
                          ? new Uri(this.crmServer.OriginalServerUrl, "Authenticate.axd")
                          : this.currentServerOperation.RequestUrl ?? this.crmServer.MobileWebserviceUrl;

            this.currentRemoteDataRequest = new RemoteData(url, this.remoteSession, this)
            {
                TrackingDelegate = this.currentServerOperation.TrackingDelegate,
                Credentials = this.ServerRequestCredentials,
                UserAgent = this.crmServer.UserAgent
            };

            this.currentRemoteSession = null;

            var parametersForServerOperation = this.currentServerOperation.RequestParameters;

            this.currentServerOperation.AddAppInfoInToRequestParameters(parametersForServerOperation);

            if (parametersForServerOperation != null)
            {
                var uploadDocumentServerOperation = this.currentServerOperation as UploadDocumentServerOperation;
                if (uploadDocumentServerOperation != null)
                {
                    this.currentRemoteDataRequest.SetRequestBody(
                                uploadDocumentServerOperation.Data,
                                uploadDocumentServerOperation.FileNameParamName,
                                uploadDocumentServerOperation.FileName,
                                uploadDocumentServerOperation.ContentType,
                                uploadDocumentServerOperation.TransferEncoding,
                                parametersForServerOperation);
                }
                else
                {
                    this.currentRemoteDataRequest.SetRequestBody(parametersForServerOperation);
                }
            }

            this.currentRemoteDataRequest.CustomHttpHeaderFields = this.currentServerOperation.HttpHeaderFields
                                                                   ?? new Dictionary<string, string>();

            var newRemoteData = this.currentRemoteDataRequest;

            newRemoteData.Load();
        }

        /// <summary>
        /// Stops the starting up.
        /// </summary>
        public void StopStartingUp()
        {
            this.startingUp = false;
            this.StartNextOperation();
        }

        /// <summary>
        /// Timeouts the login process.
        /// </summary>
        public void TimeoutLogin()
        {
            this.Delegate?.ServerOperationManagerDidFailWithInternetConnectionError(
                this,
                null,
                !string.IsNullOrEmpty(this.newPassword) ? PasswordChangeResult.PasswordNotChanged : PasswordChangeResult.NoPasswordChangeRequested);
            this.newPassword = null;
        }

        /// <summary>
        /// Triggers the retry for remote operation for current server operation.
        /// </summary>
        public void TriggerRetryForRemoteOperationForCurrentServerOperation()
        {
            if (!this.suspended)
            {
                return;
            }

            this.suspended = false;
            this.StartRemoteOperationForCurrentServerOperation();
        }

        /// <summary>
        /// Uses the language key for session.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        public void UseLanguageKeyForSession(string languageKey)
        {
            if (this.suspended || (this.languageKey != null && !this.languageKey.Equals(languageKey)))
            {
                this.LoginWithLanguageKey(languageKey);
            }
        }

        /// <summary>
        /// Attributes the dictionary for session.
        /// </summary>
        /// <param name="sessionInfoAttributes">
        /// The session information attributes.
        /// </param>
        /// <param name="serverUserDefaultLanguage">
        /// The server user default language.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        private Dictionary<string, object> AttributeDictionaryForSession(
            List<object> sessionInfoAttributes,
            string serverUserDefaultLanguage)
        {
            var dictionary = new Dictionary<string, object>();
            if (sessionInfoAttributes != null)
            {
                foreach (List<object> attribute in sessionInfoAttributes)
                {
                    dictionary[(string)attribute[0]] = attribute.Count > 1 ? attribute[1] : string.Empty;
                }
            }

            if (serverUserDefaultLanguage != null)
            {
                dictionary["serverUserDefaultLanguage"] = serverUserDefaultLanguage;
            }

            return dictionary;
        }

        /// <summary>
        /// Cleans the remote data.
        /// </summary>
        private void CleanRemoteData()
        {
            if (this.serverOperations == null)
            {
                return;
            }

            this.currentRemoteDataRequest = null;
        }

        /// <summary>
        /// Cleans up finished server operation Start next operation.
        /// </summary>
        /// <param name="startNextOperation">
        /// if set to <c>true</c> [Start next operation].
        /// </param>
        private void CleanUpFinishedServerOperationStartNextOperation(bool startNextOperation)
        {
            if (this.serverOperations == null)
            {
                return;
            }

            if (this.currentServerOperation == null && this.currentRemoteDataRequest == null)
            {
                return;
            }

            // CurrentRemoteDataRequest.OperationManagerService = null;
            this.currentRemoteDataRequest = null;

            // CurrentServerOperation.RemoveObserverForKeyPath(this, "cancelled");
            this.serverOperations.Remove(this.currentServerOperation);
            this.currentServerOperation = null;

            if (startNextOperation)
            {
                this.StartNextOperation();
            }
        }
    }
}
