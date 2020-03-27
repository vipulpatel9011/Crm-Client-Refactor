// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerSession.cs" company="Aurea Software Gmbh">
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
//   Session error code types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Utilities;
    using GalaSoft.MvvmLight.Messaging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Questionnaire;

    /// <summary>
    /// Session error code types
    /// </summary>
    public enum SessionErrorCode
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The online login failed.
        /// </summary>
        OnlineLoginFailed,

        /// <summary>
        /// The licensing failed.
        /// </summary>
        LicensingFailed,

        /// <summary>
        /// The local password incorrect.
        /// </summary>
        LocalPasswordIncorrect,

        /// <summary>
        /// The local database missing.
        /// </summary>
        LocalDatabaseMissing,

        /// <summary>
        /// The local database language missing.
        /// </summary>
        LocalDatabaseLanguageMissing,

        /// <summary>
        /// The local full sync required.
        /// </summary>
        LocalFullSyncRequired,

        /// <summary>
        /// The nointernet connection.
        /// </summary>
        NointernetConnection,

        /// <summary>
        /// The offline mode rejected.
        /// </summary>
        OfflineModeRejected,

        /// <summary>
        /// The sync failed.
        /// </summary>
        SyncFailed,

        /// <summary>
        /// The server starting.
        /// </summary>
        ServerStarting,

        /// <summary>
        /// The newer client version required.
        /// </summary>
        NewerClientVersionRequired,

        /// <summary>
        /// The password expired.
        /// </summary>
        PasswordExpired,
    }

    /// <summary>
    /// Session continue options
    /// </summary>
    public enum SessionContinueOption
    {
        /// <summary>
        /// The continue.
        /// </summary>
        Continue = 0,

        /// <summary>
        /// The cancel.
        /// </summary>
        Cancel
    }

    /// <summary>
    /// Session status values
    /// </summary>
    public enum SessionStatus
    {
        /// <summary>
        /// The not connected.
        /// </summary>
        NotConnected = 0,

        /// <summary>
        /// The requiring language.
        /// </summary>
        RequiringLanguage,

        /// <summary>
        /// The connected.
        /// </summary>
        Connected
    }

    /// <summary>
    /// Server sesion elegate interface
    /// </summary>
    public interface IServerSessionDelegate
    {
        /// <summary>
        /// Servers the session did perform login.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        Task ServerSessionDidPerformLogin(ServerSession session, PasswordChangeResult passwordChanged);

        /// <summary>
        /// Servers the session did fail login
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password change result.
        /// </param>
        void ServerSessionDidFailLogin(ServerSession session, Exception error, PasswordChangeResult passwordChanged);

        /// <summary>
        /// Servers the session requires newer client version password changed.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        void ServerSessionRequiresNewerClientVersion(ServerSession session, PasswordChangeResult passwordChanged);

        /// <summary>
        /// Servers the operation manager requires language for session.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="availableServerLanguages">
        /// The available server languages.
        /// </param>
        /// <param name="sessionAttributes">
        /// The session attributes.
        /// </param>
        void ServerOperationManagerRequiresLanguage(
            ServerSession session,
            List<ServerLanguage> availableServerLanguages,
            Dictionary<string, object> sessionAttributes);

        /// <summary>
        /// Servers the session warning show warn message text.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        void ServerSessionWarningShowWarnMessageText(ServerSession session, string text);
    }

    /// <summary>
    /// Server session implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Session.IRemoteSessionDelegate" />
    public class ServerSession : IServerSession
#if PORTING
        : IDisposable
#endif
    {
        /// <summary>
        /// The current session.
        /// </summary>
        private static IServerSession currentSession;

        /// <summary>
        /// The password.
        /// </summary>
        private string password;

        /// <summary>
        /// The ras instance name.
        /// </summary>
        private string rasInstanceName;

        // can be diiferent to the username <RasInstancename>\<Usernanme>

        /// <summary>
        /// The login name.
        /// </summary>
        private string loginName;

        /// <summary>
        /// The remote sessions.
        /// </summary>
        private List<RemoteSession> remoteSessions;

        /// <summary>
        /// The all user tenants.
        /// </summary>
        private List<int> allUserTenants;

        /// <summary>
        /// The session parameter replacements.
        /// </summary>
        private Dictionary<string, List<string>> sessionParameterReplacements;

        /// <summary>
        /// The custom error text dictionary.
        /// </summary>
        private Dictionary<string, UPErrorTexts> customErrorTextDictionary;

        /// <summary>
        /// The local operation manager.
        /// </summary>
        private LocalOperationManager localOperationManager;

        /// <summary>
        /// The custom default error texts.
        /// </summary>
        private UPErrorTexts customDefaultErrorTexts;

        /// <summary>
        /// Questionnaire manager backing field.
        /// </summary>
        private UPQuestionnaireManager questionnaireManager;

        /// <summary>
        /// Client culture information.
        /// </summary>
        private CultureInfo clientCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSession"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        /// <param name="userChoiceOffline">
        /// if set to <c>true</c> [user choice offline].
        /// </param>
        /// <param name="clientCulture">
        /// The client culture info variable.
        /// </param>
        private ServerSession(RemoteServer server, string username, IServerSessionDelegate theDelegate,
            bool userChoiceOffline, CultureInfo clientCulture)
        {
            // set the current session
            if (currentSession == null)
            {
                currentSession = this;
            }

            this.loginName = username;
            this.Delegate = theDelegate;
            this.UserChoiceOffline = userChoiceOffline;
            this.IsServerReachable = !userChoiceOffline;
            if (server.AuthenticationType == ServerAuthenticationType.Revolution)
            {
                var usernameTokens = username.Split('\\');
                if (usernameTokens.Length > 1)
                {
                    this.rasInstanceName = usernameTokens[0];
                    username = usernameTokens[1];
                }
                else
                {
                    this.rasInstanceName =
                        RasLoginAppMapping.CurrentMapping()
                            .InstanceNameForServerIdRasLoginName(server.ServerIdentification, this.loginName);
                }
            }

            this.CrmServer = server;
            this.UserName = username;
            this.localOperationManager = new LocalOperationManager();
            this.SessionStatus = SessionStatus.NotConnected;
            this.clientCulture = clientCulture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSession"/> class.
        /// </summary>
        /// <param name="account">
        /// The account.
        /// </param>
        public ServerSession(ServerAccount account)
        {
            // set the current session
            if (currentSession == null)
            {
                currentSession = this;
            }

            this.loginName = account.UserName;
            this.UserChoiceOffline = true;
            this.IsServerReachable = false;
            this.CrmServer = account.Server;
            this.CrmAccount = account;
            this.LanguageKey = "eng";
            this.UserName = this.loginName;
            this.SyncManager = new UPSyncManager(this);
            this.SetupSessionForAccount();
            this.localOperationManager = new LocalOperationManager();
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets time of showing "Please wait. Data loading..." message if we cannot connect to server.
        /// </summary>
        /// <value>
        /// Duration in milliseconds.
        /// </value>
        public static int LostConnectionMessageDuration => 2000;

        /// <summary>
        /// Gets token for LostConnectionMessage
        /// </summary>
        public static string LostConnection => "LostConnection";

        /// <summary>
        /// Creates the specified server.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="_password">
        /// The _password.
        /// </param>
        /// <param name="newPassword">
        /// The new password.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        /// <param name="userChoiceOffline">
        /// if set to <c>true</c> [user choice offline].
        /// </param>
        /// <param name="clientCulture">
        /// the culture of the client.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<ServerSession> Create(
            RemoteServer server,
            string username,
            string _password,
            string newPassword,
            IServerSessionDelegate theDelegate,
            bool userChoiceOffline,
            CultureInfo clientCulture = null)
        {
            var instance = new ServerSession(server, username, theDelegate, userChoiceOffline, clientCulture);

            var account = await ServerAccount.AccountForServer(server, username, _password, null, false);
            instance.LanguageKey = account?.LastUsedLanguageKey;
            if (server.LoginMode == ServerLoginMode.StorePasswordLogin && string.IsNullOrEmpty(_password))
            {
                instance.password = account?.AutoLoginPassword;
            }
            else
            {
                instance.password = _password;
            }

            var remoteSession = new RemoteSession(
                server,
                instance.LanguageKey,
                instance.loginName,
                instance.password,
                newPassword,
                instance.rasInstanceName,
                true,
                instance,
                instance.UserChoiceOffline);

            instance.remoteSessions = new List<RemoteSession> { remoteSession };

            // Setting resource manager instance
            instance.ResourceManager = new SmartbookResourceManager(instance);

            instance.SyncManager = new UPSyncManager(instance);

            // Subscribe to the SyncManager notifications
            Messenger.Default.Register<SyncManagerMessage>(instance, instance.OnSyncMessageReceive);

#if PORTING
   
            NSNotificationCenter.DefaultCenter().AddObserverSelectorNameTheObject(this, @selector(syncManagerDidStartFullSync:), UPSyncManagerDidStartFullSyncNotification, SyncManager);
            NSNotificationCenter.DefaultCenter().AddObserverSelectorNameTheObject(this, @selector(syncManagerDidFinishFullSync:), UPSyncManagerDidFinishFullSyncNotification, SyncManager);
            NSNotificationCenter.DefaultCenter().AddObserverSelectorNameTheObject(this, @selector(syncManagerDidFailFullSync:), UPSyncManagerDidFailFullSyncNotification, SyncManager);
            NSNotificationCenter.DefaultCenter().AddObserverSelectorNameTheObject(this, @selector(syncManagerDidFinishMetadataSync:), UPSyncManagerDidFinishMetadataSyncNotification, SyncManager);
            remoteSession.ServerOperationManager.AddObserverForKeyPathOptionsContext(this, "connectedToServer", NSKeyValueObservingOptionNew, null);
            sessionOperationQueue = NSOperationQueue.TheNew();
#endif
            instance.TimeZone = new UPCRMTimeZone(clientCulture);
            return instance;
        }

        /// <summary>
        /// Called when [synchronize message receive].
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnSyncMessageReceive(SyncManagerMessage message)
        {
            switch (message.MessageKey)
            {
                case SyncManagerMessageKey.DidFinishFullSync:
                    if (this.SyncManager.FullSyncRequirementStatus == FullSyncRequirementStatus.MandatoryForOnlineAccess)
                    {
                        this.RemoteSession.ServerOperationManager.DisallowOnlineRequests = true;
                    }
                    else
                    {
                        this.RemoteSession.ServerOperationManager.DisallowOnlineRequests = false;
                    }

                    this.LoadApplicationSettings();
                    if (this.SessionStatus != SessionStatus.Connected)
                    {
                        this.SessionStatus = SessionStatus.Connected;
                        this.SyncManager.SetUpIncrementalSyncTimer();
                        this.Delegate?.ServerSessionDidPerformLogin(this, PasswordChangeResult.NoPasswordChangeRequested);
                    }

                    bool caseInsensitive = this.ConfigUnitStore.ConfigValueIsSet("Login.CaseInsensitive");
                    bool passwordSave = this.ConfigUnitStore.ConfigValueIsSetDefaultValue("Login.PasswordSaveAllowed", false);
                    if (this.CrmAccount.PasswordCaseInsensitive != caseInsensitive)
                    {
                        this.CrmAccount.UpdateLocalPassword(this.password, caseInsensitive, passwordSave);
                    }

                    this.CrmDataStore.ResetDataModel();

                    break;
                case SyncManagerMessageKey.DidFinishMetadataSync:
                    var caseInsensitive2 = this.ConfigUnitStore.ConfigValueIsSet("Login.CaseInsensitive");
                    var passwordSave2 = this.ConfigUnitStore.ConfigValueIsSetDefaultValue("Login.PasswordSaveAllowed", false);
                    if (this.CrmAccount.PasswordCaseInsensitive != caseInsensitive2)
                    {
                        this.CrmAccount.UpdateLocalPassword(this.password, caseInsensitive2, passwordSave2);
                    }

                    this.LoadApplicationSettings();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        protected static IPlatformService Platform => SimpleIoc.Default.GetInstance<IPlatformService>();

        /// <summary>
        /// Gets the remote session.
        /// </summary>
        /// <value>
        /// The remote session.
        /// </value>
        public RemoteSession RemoteSession => this.remoteSessions?[0];

        /// <summary>
        /// Gets the session credentials.
        /// </summary>
        /// <value>
        /// The session credentials.
        /// </value>
        public NetworkCredential SessionCredentials
            => this.RemoteSession?.ServerOperationManager.ServerRequestCredentials;

        /// <summary>
        /// Gets a value indicating whether [connected to server].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connected to server]; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectedToServer => this.RemoteSession?.ServerOperationManager.ConnectedToServer ?? false;

        /// <summary>
        /// Gets or sets a value indicating whether [connected to server].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [connected to server]; otherwise, <c>false</c>.
        /// </value>
        public bool IsServerReachable { get; set; }

        /// <summary>
        /// Gets a value indicating whether [connected to server for full synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [connected to server for full synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool ConnectedToServerForFullSync
            => this.RemoteSession.ServerOperationManager.ConnectedToServerForFullSync;

        /// <summary>
        /// Gets the session specific path.
        /// </summary>
        /// <value>
        /// The session specific path.
        /// </value>
        public string SessionSpecificPath => Path.Combine(this.CrmAccount?.AccountPath, this.LanguageKey);

        /// <summary>
        /// Gets the session specific caches path.
        /// </summary>
        /// <value>
        /// The session specific caches path.
        /// </value>
        public string SessionSpecificCachesPath => Path.Combine(this.CrmAccount?.AccountCachesPath, this.LanguageKey);

        /// <summary>
        /// Gets the current rep.
        /// </summary>
        /// <value>
        /// The current rep.
        /// </value>
        public string CurRep => this.AttributeWithKey("repId");

        /// <summary>
        /// Gets the service information.
        /// </summary>
        /// <value>
        /// The service information.
        /// </value>
        public Dictionary<string, ServiceInfo> ServiceInfo => this.CrmAccount?.ServiceInfos;

        /// <summary>
        /// Gets the tenant no.
        /// </summary>
        /// <value>
        /// The tenant no.
        /// </value>
        public int TenantNo => Convert.ToInt32(this.AttributeWithKey("tenantNo"));

        /// <summary>
        /// Gets the connection watch dog.
        /// </summary>
        /// <value>
        /// The connection watch dog.
        /// </value>
        public IConnectionWatchdog ConnectionWatchDog => this.RemoteSession?.ServerOperationManager?.ConnectionWatchDog;

        /// <summary>
        /// Gets a value indicating whether this instance is update CRM.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is update CRM; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateCrm
        {
            get
            {
                var isUpdateCrm = this.ValueForKey("System.IsUpdateCrm");
                return !string.IsNullOrWhiteSpace(isUpdateCrm)
                    ? this.ValueIsSet("System.IsUpdateCrm")
                    : this.AttributeWithKey("webversion")?.StartsWith("8.") ?? true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is enterprise.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enterprise; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnterprise
        {
            get
            {
#if !PORTING
                return true;
#else
                return NSUserDefaults.StandardUserDefaults().BoolForKey("System.isEnterprise");
#endif
            }
        }

        /// <summary>
        /// Gets all user tenants.
        /// </summary>
        /// <value>
        /// All user tenants.
        /// </value>
        public List<int> AllUserTenants
        {
            get
            {
                if (this.allUserTenants != null)
                {
                    return this.allUserTenants.Count > 0 ? this.allUserTenants : null;
                }

                var allUserTenantString = this.AttributeWithKey("tenantAdd");
                if (!string.IsNullOrEmpty(allUserTenantString))
                {
                    var arr = this.TenantNo > 0 ? new List<int> { this.TenantNo } : new List<int>();

                    var stringParts = allUserTenantString.Split(',');

                    arr.AddRange(stringParts.Select(int.Parse));

                    this.allUserTenants = arr;
                }
                else
                {
                    this.allUserTenants = this.TenantNo > 0 ? new List<int> { this.TenantNo } : new List<int>();
                }

                return this.allUserTenants.Count > 0 ? this.allUserTenants : null;
            }
        }

        /// <summary>
        /// Gets the session parameter replacements.
        /// </summary>
        /// <value>
        /// The session parameter replacements.
        /// </value>
        public Dictionary<string, List<string>> SessionParameterReplacements
        {
            get
            {
                if (this.sessionParameterReplacements != null)
                {
                    return this.sessionParameterReplacements;
                }

                this.sessionParameterReplacements = new Dictionary<string, List<string>>
                {
                    {"$curRep", this.AttributeArrayWithKey("repId")},
                    {"$curTenant", this.AttributeArrayWithKey("tenantNo")},
                    {"$curDeputy", this.AttributeArrayWithKey("repDeputyId")},
                    {"$curSuperior", this.AttributeArrayWithKey("repSuperiorId")},
                    {"$curOrgGroup", this.AttributeArrayWithKey("repGroupId")},
                    { "$cur$$$Y", new List<string> { DateTime.Now.Year.ToString() } }
                };

                var repCopySearchAndLists = this.ValueForKey("RepCopySearchAndLists");

                if (string.IsNullOrWhiteSpace(repCopySearchAndLists))
                {
                    return this.sessionParameterReplacements;
                }

                var searchAndListNames = repCopySearchAndLists.Split(',');

                foreach (var searchAndListName in searchAndListNames)
                {
                    var crmQuery = new UPContainerMetaInfo(searchAndListName);
                    var result = crmQuery.Find();
                    if (result == null || result.RowCount < 1)
                    {
                        continue;
                    }

                    var functionNameValues =
                        crmQuery.SourceFieldControl.FunctionNames(result.ResultRowAtIndex(0) as UPCRMResultRow);
                    if (functionNameValues == null)
                    {
                        continue;
                    }

                    foreach (var key in functionNameValues.Keys)
                    {
                        this.sessionParameterReplacements.SetObjectForKey(
                            new List<string> { functionNameValues.ValueOrDefault(key) as string },
                            $"$cur{key}");

                        this.sessionParameterReplacements.SetObjectForKey(
                            new List<string> { functionNameValues.ValueOrDefault(key) as string },
                            $"$par{key}");
                    }
                }

                Logger.LogDebug($"sessionParameterReplacements: {string.Join(",", this.sessionParameterReplacements.Keys)}", LogFlag.LogConfig);
                // DDLogConfig("sessionParameterReplacements: %@", parameters.AllKeys);
                return this.sessionParameterReplacements;
            }
        }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IServerSessionDelegate Delegate { get; set; }

        /// <summary>
        /// Gets or sets the requireFullSyncOnLogin flag.
        /// </summary>
        /// <value>
        /// Flag that describes if full sync is required on login.
        /// </value>
        public static bool RequireFullSyncOnLogin { get; set; }


        /// <summary>
        /// Gets the session status.
        /// </summary>
        /// <value>
        /// The session status.
        /// </value>
        public SessionStatus SessionStatus { get; private set; }

        /// <summary>
        /// Gets the available server languages.
        /// </summary>
        /// <value>
        /// The available server languages.
        /// </value>
        public List<ServerLanguage> AvailableServerLanguages { get; private set; }

        /// <summary>
        /// Gets the CRM server.
        /// </summary>
        /// <value>
        /// The CRM server.
        /// </value>
        public RemoteServer CrmServer { get; private set; }

        /// <summary>
        /// Gets the CRM account.
        /// </summary>
        /// <value>
        /// The CRM account.
        /// </value>
        public ServerAccount CrmAccount { get; private set; }

        /// <summary>
        /// Gets the language key.
        /// </summary>
        /// <value>
        /// The language key.
        /// </value>
        public string LanguageKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [presentation mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [presentation mode]; otherwise, <c>false</c>.
        /// </value>
        public bool PresentationMode { get; private set; }

        /// <summary>
        /// Gets the presentation mode alpha.
        /// </summary>
        /// <value>
        /// The presentation mode alpha.
        /// </value>
        public float PresentationModeAlpha { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use sort locale].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use sort locale]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSortLocale { get; private set; }

        /// <summary>
        /// Gets the client request timeout.
        /// </summary>
        /// <value>
        /// The client request timeout.
        /// </value>
        public double ClientRequestTimeout { get; private set; }

        /// <summary>
        /// Gets the client update link.
        /// </summary>
        /// <value>
        /// The client update link.
        /// </value>
        public string ClientUpdateLink { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable virtual links].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable virtual links]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableVirtualLinks { get; private set; }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the asynchronous retry time.
        /// </summary>
        /// <value>
        /// The asynchronous retry time.
        /// </value>
        public int AsynchronousRetryTime { get; private set; }

        /// <summary>
        /// Gets the asynchronous wait time.
        /// </summary>
        /// <value>
        /// The asynchronous wait time.
        /// </value>
        public int AsynchronousWaitTime { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user choice offline].
        /// </summary>
        /// <value>
        /// <c>true</c> if [user choice offline]; otherwise, <c>false</c>.
        /// </value>
        public bool UserChoiceOffline { get; set; }

        /// <summary>
        /// Gets a value indicating whether [fix cat sort by sort information].
        /// </summary>
        /// <value>
        /// <c>true</c> if [fix cat sort by sort information]; otherwise, <c>false</c>.
        /// </value>
        public bool FixCatSortBySortInfo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [variable cat sort by sort information].
        /// </summary>
        /// <value>
        /// <c>true</c> if [variable cat sort by sort information]; otherwise, <c>false</c>.
        /// </value>
        public bool VarCatSortBySortInfo { get; private set; }

        /// <summary>
        /// Gets the file store.
        /// </summary>
        /// <value>
        /// The file store.
        /// </value>
        public IFileStorage FileStore { get; private set; }

        /// <summary>
        /// Gets the system options.
        /// </summary>
        /// <value>
        /// The system options.
        /// </value>
        public Dictionary<string, object> SystemOptions { get; private set; }

        /// <summary>
        /// Gets the custom sort locale.
        /// </summary>
        /// <value>
        /// The custom sort locale.
        /// </value>
        public CultureInfo CustomSortLocale { get; private set; }

        /// <summary>
        /// Gets or sets the CRM data store.
        /// </summary>
        /// <value>
        /// The CRM data store.
        /// </value>
        public ICRMDataStore CrmDataStore { get; set; }

        /// <summary>
        /// Gets the synchronize manager.
        /// </summary>
        /// <value>
        /// The synchronize manager.
        /// </value>
        public UPSyncManager SyncManager { get; private set; }

        /// <summary>
        /// The record identification mapper.
        /// </summary>
        private UPRecordIdentificationMapper recordIdentificationMapper;

        /// <summary>
        /// Gets the record identification mapper.
        /// </summary>
        /// <value>
        /// The record identification mapper.
        /// </value>
        public UPRecordIdentificationMapper RecordIdentificationMapper
        {
            get
            {
                return this.recordIdentificationMapper ??
                       (this.recordIdentificationMapper = new UPRecordIdentificationMapper());
            }

            private set { this.recordIdentificationMapper = value; }
        }

        /// <summary>
        /// Gets offline storage
        /// </summary>
        public IOfflineStorage OfflineStorage { get; private set; }

        /// <summary>
        /// Gets resource manager
        /// </summary>
        public SmartbookResourceManager ResourceManager { get; private set; }

        /// <summary>
        /// Gets questionnaire manager
        /// </summary>
        public UPQuestionnaireManager QuestionnaireManager
        {
            get
            {
                if (this.questionnaireManager == null)
                {
                    this.questionnaireManager = new UPQuestionnaireManager();
                }

                return this.questionnaireManager;
            }
        }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public UPCRMTimeZone TimeZone { get; private set; }

        /// <summary>
        /// Gets the session attributes.
        /// </summary>
        /// <value>
        /// The session attributes.
        /// </value>
        public IDictionary<string, object> SessionAttributes { get; private set; }

        /// <summary>
        /// Gets the configuration unit store.
        /// </summary>
        /// <value>
        /// The configuration unit store.
        /// </value>
        public IConfigurationUnitStore ConfigUnitStore { get; set; }

        /// <summary>
        /// Gets or sets the URL parameters to execute.
        /// </summary>
        /// <value>
        /// The URL parameters to execute.
        /// </value>
        public static CaseInsensitiveDictionary<object> UrlParametersToExecute { get; set; }

        /// <summary>
        /// Gets or sets the record identification to execute.
        /// </summary>
        /// <value>
        /// The record identification to execute.
        /// </value>
        public static string RecordIdentificationToExecute { get; set; }

        /// <summary>
        /// Replaces the current session.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <returns>
        /// The <see cref="ServerSession"/>.
        /// </returns>
        public static IServerSession ReplaceCurrentSession(IServerSession session)
        {
#if PORTING
            Class clz = UiApplicationClass();
            object application = clz.ValueForKey("sharedApplication");
            object oldSession = application.ValueForKey("delegate").ValueForKey("currentCRMSession");
            application.ValueForKey("delegate").SetValueForKey(session, "currentCRMSession");
            return oldSession;
#else
            var oldSession = currentSession;
            currentSession = session;
            return oldSession;
#endif
        }

        /// <summary>
        /// Currents the local json configuration dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, object> CurrentLocalJsonConfigDictionary()
        {
#if PORTING
            var clz = UiApplicationClass();
            object application = clz.ValueForKey("sharedApplication");
            return application.ValueForKey("delegate").ValueForKey("localJSONConfigDictionary");
#else
            return new Dictionary<string, object>();
#endif
        }

        /// <summary>
        /// Values for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForKey(string key)
        {
            return this.ConfigUnitStore.ConfigValue(key);
        }

        /// <summary>
        /// Values for key default value.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForKeyDefaultValue(string key, string defaultValue)
        {
            return this.ConfigUnitStore.ConfigValueDefaultValue(key, defaultValue);
        }

        /// <summary>
        /// Values the is set.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValueIsSet(string key)
        {
            var value = this.ValueForKey(key);
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            return value != "false" && value != "0";
        }

        /// <summary>
        /// Ints the value for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int IntValueForKey(string key, int defaultValue = 0)
        {
            var value = this.ValueForKey(key);
            return string.IsNullOrEmpty(value) ? defaultValue : int.Parse(value);
        }

        /// <summary>
        /// Attributes the with key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string AttributeWithKey(string key)
        {
            if (this.CrmAccount != null && this.CrmAccount.AttributesByLanguage.ContainsKey(this.LanguageKey))
            {
                return this.CrmAccount?.AttributesByLanguage[this.LanguageKey].ValueOrDefault(key) as string;
            }

            return null;
        }

        /// <summary>
        /// Attributes the array with key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> AttributeArrayWithKey(string key)
        {
            var v = this.AttributeWithKey(key);
            return v == null ? new List<string>() : new List<string> { v };
        }

        /// <summary>
        /// Changes the user choice offline.
        /// </summary>
        public void ChangeUserChoiceOffline()
        {
            this.UserChoiceOffline = !this.UserChoiceOffline;
            this.RemoteSession.ChangeUserChoiceOffline(this.UserChoiceOffline);
#if PORTING
            NSNotificationCenter.DefaultCenter().PostNotificationNameTheObject(Constants.UPMUserChangeOnOfflineNotification, this);
            if (!UserChoiceOffline)
            {
                ConnectionWatchDog.AssumeOffline();
                ConnectionWatchDog.CheckForConnectionStatusChange();
            }

            WillChangeValueForKey("connectedToServer");
            DidChangeValueForKey("connectedToServer");
#endif
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
            this.AvailableServerLanguages = availableLanguages;
            this.SessionAttributes = sessionAttributes;

            this.Delegate?.ServerOperationManagerRequiresLanguage(
                this,
                this.AvailableServerLanguages,
                sessionAttributes);
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
        public async void ServerOperationManagerDidPerformServerLogin(
            ServerOperationManager serverOperationManager,
            List<ServerLanguage> availableLanguages,
            Dictionary<string, object> sessionAttributes,
            List<object> serverInfo,
            PasswordChangeResult passwordChanged)
        {
            this.AvailableServerLanguages = availableLanguages;
            this.SessionAttributes = sessionAttributes;

            const string systemOptionsKey = "systemOptions";

            if (!sessionAttributes.ContainsKey(systemOptionsKey))
            {
                return;
            }

            var systemOptionString = sessionAttributes[systemOptionsKey] as string;
            this.SystemOptions = systemOptionString != null
                ? systemOptionString.JsonDictionaryFromString()
                : new Dictionary<string, object>();

            if (passwordChanged == PasswordChangeResult.PasswordChanged)
            {
                this.password = serverOperationManager.Password;
            }

            if (this.SessionStatus != SessionStatus.Connected)
            {
                this.UserName = serverOperationManager.RasUsername ?? this.UserName;
                this.rasInstanceName = serverOperationManager.RasInstanceName;
                var hasLocalAccount =
                    await
                        ServerAccount.HasLocalAccountForServer(
                            this.CrmServer,
                            this.UserName,
                            serverOperationManager.RasApplicationId);
                if (hasLocalAccount)
                {
                    this.CrmAccount =
                        await
                            ServerAccount.AccountForServer(
                                this.CrmServer,
                                this.UserName,
                                null,
                                serverOperationManager.RasApplicationId,
                                false);
                    this.UpdateRASLoginMapping(serverOperationManager);
                }
                else
                {
                    if (!string.IsNullOrEmpty(serverOperationManager.RasApplicationId))
                    {
                        // Maybe we have an old RAS Account to copy
                        Logger.LogDebug("ServerSession - Search for old RAS Account", LogFlag.LogNetwork);
                        var oldRasAccount =
                            await
                                ServerAccount.AccountForServer(
                                    this.CrmServer,
                                    this.loginName,
                                    null,
                                    this.CrmServer.ServerIdentification,
                                    false);
                        if (oldRasAccount != null)
                        {
                            Logger.LogDebug($"ServerSession - Old RAS Account found in Path: {oldRasAccount.AccountPath}", LogFlag.LogNetwork);
                            oldRasAccount.UpgradeToNewRasAccount(
                                serverOperationManager.RasApplicationId,
                                serverOperationManager.RasUsername,
                                serverOperationManager.RasInstanceName);
                            this.CrmAccount = oldRasAccount;
                        }
                    }

                    if (this.CrmAccount == null)
                    {
                        // Now we really need a new Account
                        Logger.LogDebug("ServerSession - Create new account", LogFlag.LogNetwork);
                        this.CrmAccount = new ServerAccount(
                            this.CrmServer,
                            this.UserName,
                            this.password,
                            serverOperationManager.RasApplicationId,
                            serverOperationManager.RasInstanceName);
                    }

                    this.UpdateRASLoginMapping(serverOperationManager);
                }

                // CrmAccount.UpdateSystemOptions(SystemOptions);
                this.SetupSessionForAccount();
                var caseInsensitive = this.ConfigUnitStore.ConfigValueIsSet("Login.CaseInsensitive");
                var passwordSave = this.ConfigUnitStore.ConfigValueIsSetDefaultValue(
                    "Login.SavePasswordAllowed",
                    false);
                this.CrmAccount.UpdateLocalPassword(this.password, caseInsensitive, passwordSave);
            }
            else
            {
                // RAS Application differs from current something strange must have happend
                if (this.CrmAccount != null && !string.IsNullOrEmpty(serverOperationManager.RasApplicationId)
                    && !string.IsNullOrEmpty(this.CrmAccount.RasApplicationId)
                    && !serverOperationManager.RasApplicationId.Equals(this.CrmAccount.RasApplicationId))
                {
                    // Relogon We have a new RAS Version or a migration is necessary
                    this.Delegate.ServerSessionDidFailLogin(this, new Exception("OnlineLoginFailed"), passwordChanged);
                }
            }

            if (passwordChanged == PasswordChangeResult.PasswordChanged && this.CrmAccount != null)
            {
                // CrmAccount.ResetPasswordChangeTimer();
            }

            var clientRequestTimeout = (string)sessionAttributes["clientRequestTimeout"];
            if (!string.IsNullOrEmpty(clientRequestTimeout))
            {
                this.ClientRequestTimeout = double.Parse(clientRequestTimeout);
            }

            var clientSessionTimeout = this.ConfigUnitStore?.ConfigValue("ClientSessionTimeout");
            int clientSessionTimeoutParsed;
            if (!string.IsNullOrWhiteSpace(clientSessionTimeout) && int.TryParse(clientSessionTimeout, out clientSessionTimeoutParsed))
            {
                this.ClientSessionTimeout = clientSessionTimeoutParsed;
            }

            var asynchronousWaitTime = sessionAttributes["asyncRequestWaitTime"] as string;
            if (!string.IsNullOrEmpty(asynchronousWaitTime))
            {
                this.AsynchronousWaitTime = int.Parse(asynchronousWaitTime);
                var asynchronousRetryTime = sessionAttributes["asyncRequestRetryTime"] as string;
                if (!string.IsNullOrEmpty(asynchronousRetryTime))
                {
                    this.AsynchronousRetryTime = int.Parse(asynchronousRetryTime);
                }

                if (this.AsynchronousRetryTime < 1000)
                {
                    this.AsynchronousRetryTime = 1000;
                }
            }

            if (serverInfo != null && serverInfo.Count > 3)
            {
                var serviceInfo = serverInfo[3] as List<object>;
                if (serviceInfo != null)
                {
                    this.CrmAccount.UpdateServiceInfos(Session.ServiceInfo.ServiceInfoDictionaryFromArray(serviceInfo));
                }
            }

            this.CrmAccount.AttributesByLanguage[this.LanguageKey] = sessionAttributes;
            this.CrmAccount.UpdateLoginDate(this.LanguageKey);
            this.CrmAccount.LastUsedLanguageKey = this.LanguageKey;

            try
            {
                await this.SyncDataAndConnectSession(passwordChanged);
            }
            catch (InvalidOperationException e)
            {
                ServerSession.Logger.LogError($"Recovering from corrupted db state: {e}");
                ServerSession.RequireFullSyncOnLogin = true;
                this.SessionStatus = SessionStatus.NotConnected;
                await this.SyncDataAndConnectSession(passwordChanged);
                ServerSession.Logger.LogWarn("Recovered from corrupted db state");
            }
            finally
            {
                ServerSession.RequireFullSyncOnLogin = false;
            }
        }

        private async Task SyncDataAndConnectSession(PasswordChangeResult passwordChanged)
        {
            FullSyncRequirementStatus syncRequirementStatus = this.SyncManager.FullSyncRequirementStatus;
            if (syncRequirementStatus == FullSyncRequirementStatus.Mandatory)
            {
                this.SyncManager.PerformFullSync();
            }
            else if (syncRequirementStatus == FullSyncRequirementStatus.Resumable)
            {
                this.SyncManager.PerformFullSyncWithResumeSync(true);
            }
            else
            {
                this.LoadApplicationSettings();
                if (this.SessionStatus != SessionStatus.Connected)
                {
                    this.SyncManager.SuspendIncrementalSyncIfFullSyncRequired(true);
                    this.SyncManager.SetUpIncrementalSyncTimer();

                    this.SessionStatus = SessionStatus.Connected;
                    await this.Delegate?.ServerSessionDidPerformLogin(this, passwordChanged);
                }

                this.SyncManager.PerformUpSync();
                if (this.SyncManager.FullSyncRequirementStatus == FullSyncRequirementStatus.MandatoryForOnlineAccess)
                {
                    var manager = this.RemoteSession.ServerOperationManager;
                    manager.DisallowOnlineRequests = true;
                }
                else
                {
                    this.SyncManager.StartSyncingDocumentsAfterServerLogin();
                }
            }
        }

        /// <summary>
        /// Servers the operation manager password change requested.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChangeResult">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerPasswordChangeRequested(
            ServerOperationManager serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChangeResult)
        {
            var sessionOperationManager = this.RemoteSession.ServerOperationManager;
            if (error.Message == "IsPasswordExpiredError")
            {
                this.Delegate?.ServerSessionDidFailLogin(this, new Exception($"{SessionErrorCode.PasswordExpired}"),
                    passwordChangeResult);
            }
            else if (error.IsInvalidLoginError())
            {
                this.Delegate?.ServerSessionDidFailLogin(this, new Exception($"{SessionErrorCode.OnlineLoginFailed}"),
                    passwordChangeResult);
            }
            else if (this.SessionStatus != SessionStatus.Connected)
            {
                Logger.LogError($" {error.Message}");
                if (error.Message == "IsInvalidClientVersion")
                {
                    sessionOperationManager.DisallowOnlineRequests = true;
                    this.ClientUpdateLink = "error.AdditionalInfo";
                    this.Delegate.ServerSessionRequiresNewerClientVersion(this, passwordChangeResult);
                }
                else
                {
                    this.InitializeOfflineSession(passwordChangeResult);
                }
            }
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
            if (this.SessionStatus != SessionStatus.Connected)
            {
                this.InitializeOfflineSession(passwordChangeResult);
            }
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
        public bool ServerOperationManagerPasswordChangeRequested(ServerOperationManager serverOperationManager,
            bool isRequested)
        {
            if (this.CrmAccount == null)
            {
                // We are starting up --> Allways yes
                return true;
            }

            if (isRequested)
            {
                // && !ServerAccount.PasswordChangeRequested)
                // No password change was requested before --> inform the user
                // CrmAccount.PasswordChangeRequestedNow();
                this.Delegate.ServerSessionWarningShowWarnMessageText(this,
                    LocalizedString.TextErrorWarningPasswordChangeNeeded);

                return false;
            }

            if (isRequested)
            {
                // 24h expired ?
                return false; // CrmAccount.IsPasswordChangeMandatory();
            }

            // Not rquested anymore
            // CrmAccount.ResetPasswordChangeTimer();
            return false;
        }

        /// <summary>
        /// Servers the operation manager did fail server login.
        /// </summary>
        /// <param name="_serverOperationManager">
        /// The _server operation manager.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="passwordChanged">
        /// The password changed.
        /// </param>
        public void ServerOperationManagerDidFailServerLogin(
            ServerOperationManager _serverOperationManager,
            Exception error,
            PasswordChangeResult passwordChanged)
        {
            var serverOperationManager = this.RemoteSession.ServerOperationManager;
            if (error.Message == "IsPasswordExpiredError")
            {
                this.Delegate?.ServerSessionDidFailLogin(this, new Exception($"{SessionErrorCode.PasswordExpired}"),
                    passwordChanged);
            }
            else if (error.Message.Contains("Forbidden"))
            {
                currentSession = null;
                this.Delegate?.ServerSessionDidFailLogin(this, new Exception($"{SessionErrorCode.OnlineLoginFailed}"),
                    passwordChanged);
            }
            else if (this.SessionStatus != SessionStatus.Connected)
            {
                Logger.LogError($" {error.Message}");
                if (error.Message == "IsInvalidClientVersion")
                {
                    serverOperationManager.DisallowOnlineRequests = true;
                    this.ClientUpdateLink = "error.AdditionalInfo";
                    this.Delegate?.ServerSessionRequiresNewerClientVersion(this, passwordChanged);
                }
                else
                {
                    this.InitializeOfflineSession(passwordChanged);
                }
            }
        }

        /// <summary>
        /// Creates the session with language.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        public void CreateSessionWithLanguage(ServerLanguage language)
        {
            this.LanguageKey = language.Key;
            foreach (var remoteSession in this.remoteSessions)
            {
                remoteSession.ServerOperationManager.LoginWithLanguageKey(language.Key);
            }
        }

        /// <summary>
        /// Continues the session creation with language.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        private void ContinueSessionCreationWithLanguage(ServerLanguage language)
        {
            this.LanguageKey = language.Key;
            foreach (var remoteSession in this.remoteSessions)
            {
                remoteSession.ServerOperationManager.UseLanguageKeyForSession(language.Key);
            }
        }

        /// <summary>
        /// Sets the logon information.
        /// </summary>
        /// <param name="logonInfo">
        /// The logon information.
        /// </param>
        public void SetLogonInfo(Dictionary<string, object> logonInfo)
        {
            var sessionInfo = logonInfo["sessioninfo"] as List<object>;
            var serverInfo = logonInfo["serverinfo"] as List<object>;
            var newLanguages = new List<ServerLanguage>();
            var serverLanguageInformation = serverInfo?[2] as List<object>;
            if (serverLanguageInformation != null)
            {
                foreach (List<string> lang in serverLanguageInformation)
                {
                    newLanguages.Add(
                        new ServerLanguage(
                            lang[0],
                            lang[1],
                            int.Parse(lang[2]),
                            int.Parse(lang[3]),
                            lang[4]));
                }
            }

            List<object> sessionInfoAttributes = null;
            string serverUserDefaultLanguage = null;
            if (sessionInfo?.Count > 1)
            {
                if (sessionInfo[0] != null)
                {
                    serverUserDefaultLanguage = sessionInfo[0] as string;
                }

                sessionInfoAttributes = sessionInfo[1] as List<object>;
            }

            var dictionary = new Dictionary<string, object>();

            if (sessionInfoAttributes != null)
            {
                foreach (List<object> attribute in sessionInfoAttributes)
                {
                    dictionary[attribute[0] as string] = attribute[1];
                }
            }

            if (serverUserDefaultLanguage != null)
            {
                dictionary["serverUserDefaultLanguage"] = serverUserDefaultLanguage;
            }

            if (newLanguages.Count > 0)
            {
                this.LanguageKey = newLanguages[0].Key;

                // TODO:      CrmAccount.UpdateAttributesForLanguageKey(dictionary, LanguageKey);
            }

            var systemOptionString = dictionary?.ValueOrDefault("systemOptions");
            var s = systemOptionString as string;
            this.SystemOptions = s != null ? s.JsonDictionaryFromString() : new Dictionary<string, object>();

            this.AvailableServerLanguages = newLanguages;
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public static IServerSession CurrentSession => currentSession;

        /// <summary>
        /// Updates the ras login mapping.
        /// </summary>
        /// <param name="serverOperationManager">
        /// The server operation manager.
        /// </param>
        public void UpdateRASLoginMapping(ServerOperationManager serverOperationManager)
        {
            if (string.IsNullOrEmpty(serverOperationManager.RasApplicationId))
            {
                return;
            }

            var mapping = RasLoginAppMapping.CurrentMapping();
            mapping.SetAppIdForServerIdRasLoginName(
                serverOperationManager.RasApplicationId,
                this.CrmServer.ServerIdentification,
                this.loginName.ToLower());

            if (this.loginName.IndexOf('\\') < 0 && !string.IsNullOrEmpty(this.rasInstanceName))
            {
                mapping.SetAppIdForServerIdRasLoginName(
                    serverOperationManager.RasApplicationId,
                    this.CrmServer.ServerIdentification,
                    $"{this.rasInstanceName.ToLower()}\\{this.loginName.ToLower()}");

                mapping.SetInstanceNameForServerIdRasLoginName(
                    serverOperationManager.RasInstanceName,
                    this.CrmServer.ServerIdentification,
                    this.loginName.ToLower());
            }

            mapping.WriteMapping();
            Logger.LogInfo($"Adjusted User AppId mapping: {RasLoginAppMapping.CurrentMapping()}");
        }

        /// <summary>
        /// Initializes the session.
        /// </summary>
        /// <param name="firstTime">
        /// The first time.
        /// </param>
        public void InitializeSession(int firstTime)
        {
            if (this.remoteSessions == null)
            {
                return;
            }

            foreach (var remoteSession in this.remoteSessions)
            {
                remoteSession.Login();
            }
        }

        /// <summary>
        /// Initializes the remote session.
        /// </summary>
        /// <param name="remoteSession">
        /// The remote session.
        /// </param>
        public void InitializeRemoteSession(RemoteSession remoteSession)
        {
            remoteSession?.Login();
        }

        /// <summary>
        /// Creates the remote session with delegate.
        /// </summary>
        /// <param name="sessionDelegate">
        /// The session delegate.
        /// </param>
        /// <returns>
        /// The <see cref="RemoteSession"/>.
        /// </returns>
        public RemoteSession CreateRemoteSessionWithDelegate(IRemoteSessionDelegate sessionDelegate)
        {
            if (!(this.RemoteSession != null && this.RemoteSession.IsLoggedIn))
            {
                return null;
            }

            var session = new RemoteSession(
                this.CrmServer,
                this.LanguageKey,
                this.loginName,
                this.password,
                null,
                this.rasInstanceName,
                false,
                sessionDelegate,
                this.UserChoiceOffline);
            this.InitializeRemoteSession(session);
            this.remoteSessions.Add(session);
            return session;
        }

        /// <summary>
        /// Closes the remote session.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        public void CloseRemoteSession(RemoteSession session)
        {
            if (session == null)
            {
                return;
            }

            if (session.IsDefault)
            {
                Logger.LogError("Cannot explicitly close default session");
                return;
            }

            if (session.IsLoggedIn)
            {
                session.Logout();
            }

            if (!this.ConfigUnitStore.ConfigValueIsSet("Disable.287"))
            {
                session.Clear();
            }

            this.remoteSessions.Remove(session);
        }

        /// <summary>
        /// Subs the session with delegate.
        /// </summary>
        /// <param name="_delegate">
        /// The _delegate.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<ServerSession> SubSessionWithDelegate(IServerSessionDelegate _delegate)
        {
            var subSession = await Create(this.CrmServer, this.UserName, this.password, null, _delegate, this.UserChoiceOffline);
            return subSession;
        }

        /// <summary>
        /// Loads the application settings.
        /// </summary>
        public void LoadApplicationSettings()
        {
            this.customDefaultErrorTexts = null;
            this.customErrorTextDictionary = null;

            this.PresentationMode = this.ConfigUnitStore?.ConfigValueDefaultValue("PresentationMode", "0") == "1";
            if (this.PresentationMode)
            {
                var dictionary = CurrentLocalJsonConfigDictionary();

                float presentationMode;
                if (float.TryParse(dictionary.ValueOrDefault("presentationMode") as string, out presentationMode))
                {
                    this.PresentationModeAlpha = presentationMode;
                }
            }
            else
            {
                this.PresentationModeAlpha = 0.0f;
            }

            var localeString = this.ConfigUnitStore?.ConfigValue("SortLocale");
            if (!string.IsNullOrEmpty(localeString))
            {
                // use the sqlite default search collaction BINARY
                if (localeString.Equals("BINARY"))
                {
                    this.CustomSortLocale = null;
                    this.UseSortLocale = false;
                }
                else
                {
                    this.UseSortLocale = true;

                    // use the device collation for sorting
                    this.CustomSortLocale = localeString.Equals("DEVICE") ? null : new CultureInfo(localeString);
                }
            }
            else
            {
                this.UseSortLocale = false;
                this.CustomSortLocale = null;
            }

            this.DisableVirtualLinks = this.ConfigUnitStore.ConfigValueDefaultValue("System.DisableVirtualLinks", "0") != "0";
            this.FixCatSortBySortInfo = this.ConfigUnitStore.ConfigValueIsSetDefaultValue("System.SortFixCatBySortInfo", true);
            this.VarCatSortBySortInfo = this.ConfigUnitStore.ConfigValueIsSetDefaultValue("System.SortVarCatBySortInfo", false);
            UPCatalog.SetEnableExplicitTenantCheck(this.ConfigUnitStore.ConfigValueIsSetDefaultValue("System.ExplicitCatalogTenantCheck", false));
#if PORTING
            LogSettings.ResetDefault();
            UPJavascriptEngine.ResetJavascriptEngine();
#endif
            string timeZoneName = this.CrmDataStore.TimeZoneName();
            this.TimeZone = new UPCRMTimeZone(timeZoneName, this.clientCulture);
            this.TimeZone = new UPCRMTimeZone(timeZoneName, this.clientCulture);
        }

        /// <summary>
        /// Initializes the offline session.
        /// </summary>
        /// <param name="passwordChangeResult">
        /// The password change result.
        /// </param>
        public async void InitializeOfflineSession(PasswordChangeResult passwordChangeResult)
        {
            var offlineSessionErrorCode = await this.PerformOfflineSessionCreation();
            if (offlineSessionErrorCode == SessionErrorCode.None)
            {
                // In case of performing an offline login, start in offline mode
                if (!this.UserChoiceOffline)
                {
                    this.IsServerReachable = false;
                    currentSession.ChangeUserChoiceOffline();
                    Messenger.Default.Send(new NotificationMessage(LostConnection), LostConnection);
                }

                this.Delegate?.ServerSessionDidPerformLogin(this, passwordChangeResult);
                this.LoadApplicationSettings();

                // Create instance for local database
            }
            else
            {
                // clear current session object
                currentSession = null;

                this.Delegate?.ServerSessionDidFailLogin(this, new Exception($"{offlineSessionErrorCode}"), passwordChangeResult);
            }
        }

        /// <summary>
        /// Performs the login.
        /// </summary>
        public void PerformLogin()
        {
            this.InitializeSession(1);
        }

        /// <summary>
        /// Determines whether this instance [can perform offline session creation].
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<SessionErrorCode> CanPerformOfflineSessionCreation()
        {
            var hasLocalAccount = await ServerAccount.HasLocalAccountForServer(this.CrmServer, this.loginName, null);
            if (!hasLocalAccount)
            {
                return SessionErrorCode.LocalDatabaseMissing;
            }

            var account = await ServerAccount.AccountForServer(this.CrmServer, this.loginName, this.password, null, true);
            return account != null ? SessionErrorCode.None : SessionErrorCode.LocalPasswordIncorrect;
        }

        /// <summary>
        /// Performs the offline session creation.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<SessionErrorCode> PerformOfflineSessionCreation()
        {
            var offlineErrorCode = await this.CanPerformOfflineSessionCreation();
            if (offlineErrorCode == SessionErrorCode.None)
            {
                // Do something
                this.CrmAccount = await ServerAccount.AccountForServer(this.CrmServer, this.loginName, this.password, null, true);
                if (this.LanguageKey == null)
                {
                    this.LanguageKey = this.CrmAccount.LastUsedLanguageKey;
                }

                if (this.LanguageKey == null)
                {
                    this.SessionStatus = SessionStatus.NotConnected;
                    return SessionErrorCode.LocalDatabaseLanguageMissing;
                }

                if (this.SyncManager.FullSyncRequirementStatus == FullSyncRequirementStatus.Mandatory)
                {
                    this.SessionStatus = SessionStatus.NotConnected;
                    return SessionErrorCode.LocalFullSyncRequired;
                }

                this.SetupSessionForAccount();
                this.SessionStatus = SessionStatus.Connected;

                return SessionErrorCode.None;
            }

            this.SessionStatus = SessionStatus.NotConnected;
            return offlineErrorCode;
        }

        /// <summary>
        /// Removes the specific paths.
        /// </summary>
        public void RemoveSpecificPaths()
        {
            var sessionSpecificPath = this.SessionSpecificPath;
            Exception error = null;
            if (!Platform.StorageProvider.TryDelete(sessionSpecificPath, out error))
            {
                Logger.LogWarn($"[Error] {error} ({sessionSpecificPath})");
            }

            var sessionSpecificCachesPath = this.SessionSpecificCachesPath;
            if (!Platform.StorageProvider.TryDelete(sessionSpecificCachesPath, out error))
            {
                Logger.LogWarn($"[Error] {error} ({sessionSpecificCachesPath})");
            }
        }

        /// <summary>
        /// Setups the session for account.
        /// </summary>
        public void SetupSessionForAccount()
        {
            if (this.CrmAccount == null)
            {
                return;
            }

            Platform.StorageProvider.CreateDirectory(this.SessionSpecificCachesPath);
            Platform.StorageProvider.CreateDirectory(this.SessionSpecificPath);

            this.ConfigUnitStore = new ConfigurationUnitStore(this.SessionSpecificCachesPath);

            // Caches Databases
            var oldConfigPath = Path.Combine(this.SessionSpecificPath, ConfigurationUnitStore.DefaultDatabaseName);
            var newConfigPath = Path.Combine(this.SessionSpecificPath, ConfigurationUnitStore.DefaultDatabaseName);
            this.CheckForDatabaseUpateNewFileName(oldConfigPath, newConfigPath);

            var oldCrmPath = Path.Combine(this.SessionSpecificPath, UPCRMDataStore.DefaultDatabaseName);
            var newCrmPath = Path.Combine(this.SessionSpecificPath, UPCRMDataStore.DefaultDatabaseName);
            this.CheckForDatabaseUpateNewFileName(oldCrmPath, newCrmPath);
            this.SystemOptions = this.CrmAccount.SystemOptions;
            this.CrmDataStore = new UPCRMDataStore(
                this.SessionSpecificCachesPath,
                this.IsUpdateCrm,
                this.ConfigUnitStore);

            this.FileStore = Platform.StorageProvider.FileStoreAtPath(this.SessionSpecificCachesPath);

            var oldFileStore = Path.Combine(this.SessionSpecificPath, this.FileStore.DefaultFolderName);
            var newFileStore = Path.Combine(this.SessionSpecificCachesPath, this.FileStore.DefaultFolderName);
            this.CheckForDatabaseUpateNewFileName(oldFileStore, newFileStore);
            this.FileStore.UpgradeFileProtectionForResources();

            // DocumentPath
            this.OfflineStorage = new UPOfflineStorage(this.SessionSpecificPath, this.ConfigUnitStore);
        }

        /// <summary>
        /// Checks the name of for database upate new file.
        /// </summary>
        /// <param name="oldFileName">
        /// Old name of the file.
        /// </param>
        /// <param name="newFileName">
        /// New name of the file.
        /// </param>
        public void CheckForDatabaseUpateNewFileName(string oldFileName, string newFileName)
        {
            if (Platform.StorageProvider.FileExists(newFileName) || !Platform.StorageProvider.FileExists(oldFileName))
            {
                return;
            }

            Exception copyError;
            if (!Platform.StorageProvider.TryCopy(oldFileName, newFileName, out copyError))
            {
                Logger.LogError($"Error database copy: {copyError}");
            }
            else
            {
                Exception deleteError;
                if (!Platform.StorageProvider.TryDelete(oldFileName, out deleteError))
                {
                    Logger.LogWarn($"Error delete old database: {deleteError}");
                }
            }
        }

        /// <summary>
        /// Performs the logout.
        /// </summary>
        public void PerformLogout()
        {
            if (this.remoteSessions != null)
            {
                foreach (var remoteSession in this.remoteSessions)
                {
                    remoteSession.ServerOperationManager.Logout();
                }
            }

            currentSession = null;

            bool terminateProcess;
            var terminateOnLogout = this.ConfigUnitStore?.ConfigValue("Application.TerminateOnLogout");
            if (!string.IsNullOrEmpty(terminateOnLogout))
            {
                terminateProcess = this.ConfigUnitStore.ConfigValueIsSet("Application.TerminateOnLogout");
            }
            else
            {
                terminateProcess = this.CrmServer.AuthenticationType == ServerAuthenticationType.SSOCredentialNoCache
                                    || this.CrmServer.AuthenticationType == ServerAuthenticationType.SSOCredentialSessionCache;
            }

            Messenger.Default.Send(terminateProcess ? LoginEventMessage.LogoutAndTerminateMessage : LoginEventMessage.LogoutMessage);
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void ExecuteRequest(Operation request)
        {
            this.ExecuteRequestInSession(request, this.RemoteSession);
        }

        /// <summary>
        /// Executes the request in session.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="remoteSession">
        /// The remote session.
        /// </param>
        public void ExecuteRequestInSession(Operation request, RemoteSession remoteSession)
        {
            if (request is ServerOperation)
            {
                if (remoteSession == null)
                {
                    var so = (ServerOperation)request;

                    so.ProcessErrorWithRemoteData(new Exception("ConnectionOfflineError"), null);
                    return;
                }

                remoteSession.ServerOperationManager.QueueServerOperation((ServerOperation)request);
            }
            else if (request is LocalOperation)
            {
                this.localOperationManager.QueueLocalOperation((LocalOperation)request);
            }
        }

        /// <summary>
        /// Resets the record based session variables.
        /// </summary>
        public void ResetRecordBasedSessionVariables()
        {
            // questionnaireManager = null;
        }

        /// <summary>
        /// Resets the session variables.
        /// </summary>
        public void ResetSessionVariables()
        {
            this.sessionParameterReplacements = null;
            this.ResetRecordBasedSessionVariables();
        }

        /// <summary>
        /// Replaces the CRM data store with store.
        /// </summary>
        /// <param name="newStore">
        /// The new store.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMDataStore"/>.
        /// </returns>
        public ICRMDataStore ReplaceCrmDataStoreWithStore(ICRMDataStore newStore)
        {
            if (newStore == this.CrmDataStore)
            {
                return this.CrmDataStore;
            }

            this.CrmDataStore.DeleteDatabase(false);
            newStore.RenameDatabaseToDefault();
            this.CrmDataStore = new UPCRMDataStore(
                this.SessionSpecificCachesPath,
                this.IsUpdateCrm,
                this.ConfigUnitStore);
            this.ResetSessionVariables();
            return this.CrmDataStore;
        }

        /// <summary>
        /// Replaces the configuration store with store.
        /// </summary>
        /// <param name="newStore">
        /// The new store.
        /// </param>
        /// <returns>
        /// The <see cref="ConfigUnitStore"/>.
        /// </returns>
        public IConfigurationUnitStore ReplaceConfigStoreWithStore(IConfigurationUnitStore newStore)
        {
            if (newStore == this.ConfigUnitStore)
            {
                return newStore;
            }

            this.ConfigUnitStore.DeleteDatabase(false);
            newStore.RenameDatabaseToDefault();
            this.ConfigUnitStore = new ConfigurationUnitStore(this.SessionSpecificCachesPath);
            this.ResetSessionVariables();
            return this.ConfigUnitStore;
        }

        /// <summary>
        /// Ups the text for yes.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string UpTextForYES()
        {
            return LocalizedString.TextYes;
        }

        /// <summary>
        /// Ups the text for no.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string UpTextForNO()
        {
            return LocalizedString.TextNo;
        }

        /// <summary>
        /// Updates the session with new language.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        public void UpdateSessionWithNewLanguage(ServerLanguage language)
        {
            this.CrmAccount.LastUsedLanguageKey = language.Key;
            this.ContinueSessionCreationWithLanguage(language);
            this.SetupSessionForAccount();
        }

        /// <summary>
        /// Customs the default error texts.
        /// </summary>
        /// <returns>
        /// The <see cref="UPErrorTexts"/>.
        /// </returns>
        public UPErrorTexts CustomDefaultErrorTexts()
        {
            if (this.customDefaultErrorTexts != null)
            {
                return this.customDefaultErrorTexts;
            }

            var defaultName = this.ConfigUnitStore.ConfigValue("System.ErrorTexts");
            if (!string.IsNullOrEmpty(defaultName))
            {
                this.customDefaultErrorTexts = new UPErrorTexts(defaultName);
            }

            return this.customDefaultErrorTexts;
        }

        /// <summary>
        /// Customs the name of the error texts with.
        /// </summary>
        /// <param name="errorTextsName">
        /// Name of the error texts.
        /// </param>
        /// <returns>
        /// The <see cref="UPErrorTexts"/>.
        /// </returns>
        public UPErrorTexts CustomErrorTextsWithName(string errorTextsName)
        {
            lock (this)
            {
                var texts = this.customErrorTextDictionary[errorTextsName];
                if (texts != null)
                {
                    return texts;
                }

                var errorTexts = new UPErrorTexts(errorTextsName, this.CustomDefaultErrorTexts());

                if (this.customErrorTextDictionary == null)
                {
                    this.customErrorTextDictionary = new Dictionary<string, UPErrorTexts>
                                                         {
                                                             { errorTextsName, errorTexts }
                                                         };
                }
                else
                {
                    this.customErrorTextDictionary[errorTextsName] = errorTexts;
                }

                return errorTexts;
            }
        }

        /// <summary>
        /// Allwayses the fullsync when login.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AllwaysFullsyncWhenLogin()
        {
#if PORTING
            var defaults = NSUserDefaults.StandardUserDefaults();
            bool allwaysFullsync = defaults.BoolForKey("crmpad.login.withFullsync");
            defaults.SetBoolForKey(false, "crmpad.login.withFullsync");
            return allwaysFullsync;
#else
            return ServerSession.RequireFullSyncOnLogin;
#endif
        }

        /// <summary>
        /// Services the name of the information for service.
        /// </summary>
        /// <param name="serviceName">
        /// Name of the service.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceInfo"/>.
        /// </returns>
        public ServiceInfo ServiceInfoForServiceName(string serviceName)
        {
            return this.ServiceInfo.ValueOrDefault(serviceName);
        }

        /// <summary>
        /// Resets the record identification mapper.
        /// </summary>
        public void ResetRecordIdentificationMapper()
        {
            this.RecordIdentificationMapper = null;
        }

        /// <summary>
        /// Creates the synchronizer.
        /// </summary>
        /// <returns>
        /// The <see cref="UPSynchronization"/>.
        /// </returns>
        public UPSynchronization CreateSynchronizer()
        {
            return new UPSynchronization((UPCRMDataStore)this.CrmDataStore, this.ConfigUnitStore);
        }

        /// <summary>
        /// Creates the time zone.
        /// </summary>
        /// <param name="clientDataTimeZone">
        /// The client data time zone.
        /// </param>
        /// <param name="currentTimeZone">
        /// The current time zone.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMTimeZone"/>.
        /// </returns>
        public UPCRMTimeZone CreateTimeZone(string clientDataTimeZone, string currentTimeZone)
        {
            return new UPCRMTimeZone(clientDataTimeZone, currentTimeZone);
        }

        /// <summary>
        /// Changes the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMTimeZone"/>.
        /// </returns>
        public UPCRMTimeZone ChangeTimeZone(UPCRMTimeZone timeZone)
        {
            var old = this.TimeZone;
            this.TimeZone = timeZone;
            return old;
        }

        /// <summary>
        /// Cancels all operations.
        /// </summary>
        public void CancelAllOperations()
        {
            this.localOperationManager.CancelAllOperations();
        }

        /// <summary>
        /// Gets a value indicating whether [serial entry mode online].
        /// </summary>
        /// <value>
        /// <c>true</c> if [serial entry mode online]; otherwRequireFullSyncOnLoginise, <c>false</c>.
        /// </value>
        public bool SerialEntryModeOnline
        {
            get
            {
                string mainTrackerEnabledConfig = this.ConfigUnitStore.ConfigValue(@"SerialEntry.DefaultModeOnline");
                return mainTrackerEnabledConfig != null && mainTrackerEnabledConfig != @"false";
            }
        }

        /// <summary>
        /// Gets the client session timeout in minutes. The user is logged off after being idle
        /// </summary>
        /// <value>
        /// Client session timeout in minutes
        /// </value>
        public int ClientSessionTimeout { get; private set; }
    }
}
