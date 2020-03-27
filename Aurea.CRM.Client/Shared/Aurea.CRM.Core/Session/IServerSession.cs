// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerSession.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Jakub Majewski
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.Questionnaire;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Utilities;

    /// <summary>
    /// Server session interface
    /// </summary>
    public interface IServerSession : IRemoteSessionDelegate
    {
        /// <summary>
        ///     Gets the remote session.
        /// </summary>
        /// <value>
        ///     The remote session.
        /// </value>
        RemoteSession RemoteSession { get; }

        /// <summary>
        ///     Gets the session credentials.
        /// </summary>
        /// <value>
        ///     The session credentials.
        /// </value>
        NetworkCredential SessionCredentials { get; }

        /// <summary>
        ///     Gets a value indicating whether [connected to server].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [connected to server]; otherwise, <c>false</c>.
        /// </value>
        bool ConnectedToServer { get; }

        /// <summary>
        ///     Gets a value indicating whether [connected to server].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [connected to server]; otherwise, <c>false</c>.
        /// </value>
        bool IsServerReachable { get; }

        /// <summary>
        ///     Gets a value indicating whether [connected to server for full synchronize].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [connected to server for full synchronize]; otherwise, <c>false</c>.
        /// </value>
        bool ConnectedToServerForFullSync { get; }

        /// <summary>
        ///     Gets the session specific path.
        /// </summary>
        /// <value>
        ///     The session specific path.
        /// </value>
        string SessionSpecificPath { get; }

        /// <summary>
        ///     Gets the session specific caches path.
        /// </summary>
        /// <value>
        ///     The session specific caches path.
        /// </value>
        string SessionSpecificCachesPath { get; }

        /// <summary>
        ///     Gets the current rep.
        /// </summary>
        /// <value>
        ///     The current rep.
        /// </value>
        string CurRep { get; }

        /// <summary>
        ///     Gets the service information.
        /// </summary>
        /// <value>
        ///     The service information.
        /// </value>
        Dictionary<string, ServiceInfo> ServiceInfo { get; }

        /// <summary>
        ///     Gets the tenant no.
        /// </summary>
        /// <value>
        ///     The tenant no.
        /// </value>
        int TenantNo { get; }

        /// <summary>
        ///     Gets the connection watch dog.
        /// </summary>
        /// <value>
        ///     The connection watch dog.
        /// </value>
        IConnectionWatchdog ConnectionWatchDog { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is update CRM.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is update CRM; otherwise, <c>false</c>.
        /// </value>
        bool IsUpdateCrm { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is enterprise.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is enterprise; otherwise, <c>false</c>.
        /// </value>
        bool IsEnterprise { get; }

        /// <summary>
        ///     Gets all user tenants.
        /// </summary>
        /// <value>
        ///     All user tenants.
        /// </value>
        List<int> AllUserTenants { get; }

        /// <summary>
        ///     Gets the session parameter replacements.
        /// </summary>
        /// <value>
        ///     The session parameter replacements.
        /// </value>
        Dictionary<string, List<string>> SessionParameterReplacements { get; }

        /// <summary>
        ///     Gets or sets the delegate.
        /// </summary>
        /// <value>
        ///     The delegate.
        /// </value>
        IServerSessionDelegate Delegate { get; set; }

        /// <summary>
        ///     Gets the session status.
        /// </summary>
        /// <value>
        ///     The session status.
        /// </value>
        SessionStatus SessionStatus { get; }

        /// <summary>
        ///     Gets the available server languages.
        /// </summary>
        /// <value>
        ///     The available server languages.
        /// </value>
        List<ServerLanguage> AvailableServerLanguages { get; }

        /// <summary>
        ///     Gets the CRM server.
        /// </summary>
        /// <value>
        ///     The CRM server.
        /// </value>
        RemoteServer CrmServer { get; }

        /// <summary>
        ///     Gets the CRM account.
        /// </summary>
        /// <value>
        ///     The CRM account.
        /// </value>
        ServerAccount CrmAccount { get; }

        /// <summary>
        ///     Gets the language key.
        /// </summary>
        /// <value>
        ///     The language key.
        /// </value>
        string LanguageKey { get; }

        /// <summary>
        ///     Gets a value indicating whether [presentation mode].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [presentation mode]; otherwise, <c>false</c>.
        /// </value>
        bool PresentationMode { get; }

        /// <summary>
        ///     Gets the presentation mode alpha.
        /// </summary>
        /// <value>
        ///     The presentation mode alpha.
        /// </value>
        float PresentationModeAlpha { get; }

        /// <summary>
        ///     Gets a value indicating whether [use sort locale].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use sort locale]; otherwise, <c>false</c>.
        /// </value>
        bool UseSortLocale { get; }

        /// <summary>
        ///     Gets the client request timeout.
        /// </summary>
        /// <value>
        ///     The client request timeout.
        /// </value>
        double ClientRequestTimeout { get; }

        /// <summary>
        ///     Gets the client update link.
        /// </summary>
        /// <value>
        ///     The client update link.
        /// </value>
        string ClientUpdateLink { get; }

        /// <summary>
        ///     Gets a value indicating whether [disable virtual links].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [disable virtual links]; otherwise, <c>false</c>.
        /// </value>
        bool DisableVirtualLinks { get; }

        /// <summary>
        ///     Gets the name of the user.
        /// </summary>
        /// <value>
        ///     The name of the user.
        /// </value>
        string UserName { get; }

        /// <summary>
        ///     Gets the asynchronous retry time.
        /// </summary>
        /// <value>
        ///     The asynchronous retry time.
        /// </value>
        int AsynchronousRetryTime { get; }

        /// <summary>
        ///     Gets the asynchronous wait time.
        /// </summary>
        /// <value>
        ///     The asynchronous wait time.
        /// </value>
        int AsynchronousWaitTime { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether [user choice offline].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [user choice offline]; otherwise, <c>false</c>.
        /// </value>
        bool UserChoiceOffline { get; set; }

        /// <summary>
        ///     Gets a value indicating whether [fix cat sort by sort information].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [fix cat sort by sort information]; otherwise, <c>false</c>.
        /// </value>
        bool FixCatSortBySortInfo { get; }

        /// <summary>
        ///     Gets a value indicating whether [variable cat sort by sort information].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [variable cat sort by sort information]; otherwise, <c>false</c>.
        /// </value>
        bool VarCatSortBySortInfo { get; }

        /// <summary>
        ///     Gets the file store.
        /// </summary>
        /// <value>
        ///     The file store.
        /// </value>
        IFileStorage FileStore { get; }

        /// <summary>
        ///     Gets the system options.
        /// </summary>
        /// <value>
        ///     The system options.
        /// </value>
        Dictionary<string, object> SystemOptions { get; }

        /// <summary>
        ///     Gets the custom sort locale.
        /// </summary>
        /// <value>
        ///     The custom sort locale.
        /// </value>
        CultureInfo CustomSortLocale { get; }

        /// <summary>
        ///     Gets the CRM data store.
        /// </summary>
        /// <value>
        ///     The CRM data store.
        /// </value>
        ICRMDataStore CrmDataStore { get; }

        /// <summary>
        ///     Gets the synchronize manager.
        /// </summary>
        /// <value>
        ///     The synchronize manager.
        /// </value>
        UPSyncManager SyncManager { get; }

        /// <summary>
        ///     Gets the record identification mapper.
        /// </summary>
        /// <value>
        ///     The record identification mapper.
        /// </value>
        UPRecordIdentificationMapper RecordIdentificationMapper { get; }

        /// <summary>
        ///     Gets offline storage
        /// </summary>
        IOfflineStorage OfflineStorage { get; }

        /// <summary>
        ///     Gets resource manager
        /// </summary>
        SmartbookResourceManager ResourceManager { get; }

        /// <summary>
        ///     Gets questionnaire manager
        /// </summary>
        UPQuestionnaireManager QuestionnaireManager { get; }

        /// <summary>
        ///     Gets the time zone.
        /// </summary>
        /// <value>
        ///     The time zone.
        /// </value>
        UPCRMTimeZone TimeZone { get; }

        /// <summary>
        /// Gets the session attributes.
        /// </summary>
        /// <value>
        /// The session attributes.
        /// </value>
        IDictionary<string, object> SessionAttributes { get; }

        /// <summary>
        ///     Gets the configuration unit store.
        /// </summary>
        /// <value>
        ///     The configuration unit store.
        /// </value>
        IConfigurationUnitStore ConfigUnitStore { get; }

        /// <summary>
        ///     Gets a value indicating whether [serial entry mode online].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [serial entry mode online]; otherwRequireFullSyncOnLoginise, <c>false</c>.
        /// </value>
        bool SerialEntryModeOnline { get; }

        /// <summary>
        ///     Gets gets or sets gets a value indicating the session timeout
        /// </summary>
        /// <value>
        ///    value in minutes
        /// </value>
        int ClientSessionTimeout { get; }

        /// <summary>
        ///     Called when [synchronize message receive].
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        void OnSyncMessageReceive(SyncManagerMessage message);

        /// <summary>
        ///     Values for key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string ValueForKey(string key);

        /// <summary>
        ///     Values for key default value.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string ValueForKeyDefaultValue(string key, string defaultValue);

        /// <summary>
        ///     Values the is set.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ValueIsSet(string key);

        /// <summary>
        ///     Ints the value for key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <param name="defaultValue">
        ///     The default value.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int IntValueForKey(string key, int defaultValue = 0);

        /// <summary>
        ///     Attributes the with key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string AttributeWithKey(string key);

        /// <summary>
        ///     Attributes the array with key.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" />.
        /// </returns>
        List<string> AttributeArrayWithKey(string key);

        /// <summary>
        ///     Changes the user choice offline.
        /// </summary>
        void ChangeUserChoiceOffline();

        /// <summary>
        ///     Creates the session with language.
        /// </summary>
        /// <param name="language">
        ///     The language.
        /// </param>
        void CreateSessionWithLanguage(ServerLanguage language);

        /// <summary>
        ///     Sets the logon information.
        /// </summary>
        /// <param name="logonInfo">
        ///     The logon information.
        /// </param>
        void SetLogonInfo(Dictionary<string, object> logonInfo);

        /// <summary>
        ///     Updates the ras login mapping.
        /// </summary>
        /// <param name="serverOperationManager">
        ///     The server operation manager.
        /// </param>
        void UpdateRASLoginMapping(ServerOperationManager serverOperationManager);

        /// <summary>
        ///     Initializes the session.
        /// </summary>
        /// <param name="firstTime">
        ///     The first time.
        /// </param>
        void InitializeSession(int firstTime);

        /// <summary>
        ///     Initializes the remote session.
        /// </summary>
        /// <param name="remoteSession">
        ///     The remote session.
        /// </param>
        void InitializeRemoteSession(RemoteSession remoteSession);

        /// <summary>
        ///     Creates the remote session with delegate.
        /// </summary>
        /// <param name="sessionDelegate">
        ///     The session delegate.
        /// </param>
        /// <returns>
        ///     The <see cref="ServerSession.RemoteSession" />.
        /// </returns>
        RemoteSession CreateRemoteSessionWithDelegate(IRemoteSessionDelegate sessionDelegate);

        /// <summary>
        ///     Closes the remote session.
        /// </summary>
        /// <param name="session">
        ///     The session.
        /// </param>
        void CloseRemoteSession(RemoteSession session);

        /// <summary>
        ///     Subs the session with delegate.
        /// </summary>
        /// <param name="sessionDelegate">
        ///     The sessionDelegate.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<ServerSession> SubSessionWithDelegate(IServerSessionDelegate sessionDelegate);

        /// <summary>
        ///     Loads the application settings.
        /// </summary>
        void LoadApplicationSettings();

        /// <summary>
        ///     Initializes the offline session.
        /// </summary>
        /// <param name="passwordChangeResult">
        ///     The password change result.
        /// </param>
        void InitializeOfflineSession(PasswordChangeResult passwordChangeResult);

        /// <summary>
        ///     Performs the login.
        /// </summary>
        void PerformLogin();

        /// <summary>
        ///     Determines whether this instance [can perform offline session creation].
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<SessionErrorCode> CanPerformOfflineSessionCreation();

        /// <summary>
        ///     Performs the offline session creation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        Task<SessionErrorCode> PerformOfflineSessionCreation();

        /// <summary>
        ///     Removes the specific paths.
        /// </summary>
        void RemoveSpecificPaths();

        /// <summary>
        ///     Setups the session for account.
        /// </summary>
        void SetupSessionForAccount();

        /// <summary>
        ///     Checks the name of for database upate new file.
        /// </summary>
        /// <param name="oldFileName">
        ///     Old name of the file.
        /// </param>
        /// <param name="newFileName">
        ///     New name of the file.
        /// </param>
        void CheckForDatabaseUpateNewFileName(string oldFileName, string newFileName);

        /// <summary>
        ///     Performs the logout.
        /// </summary>
        void PerformLogout();

        /// <summary>
        ///     Executes the request.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        void ExecuteRequest(Operation request);

        /// <summary>
        ///     Executes the request in session.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <param name="remoteSession">
        ///     The remote session.
        /// </param>
        void ExecuteRequestInSession(Operation request, RemoteSession remoteSession);

        /// <summary>
        ///     Resets the record based session variables.
        /// </summary>
        void ResetRecordBasedSessionVariables();

        /// <summary>
        ///     Resets the session variables.
        /// </summary>
        void ResetSessionVariables();

        /// <summary>
        ///     Replaces the CRM data store with store.
        /// </summary>
        /// <param name="newStore">
        ///     The new store.
        /// </param>
        /// <returns>
        ///     The <see cref="UPCRMDataStore" />.
        /// </returns>
        ICRMDataStore ReplaceCrmDataStoreWithStore(ICRMDataStore newStore);

        /// <summary>
        ///     Replaces the configuration store with store.
        /// </summary>
        /// <param name="newStore">
        ///     The new store.
        /// </param>
        /// <returns>
        ///     The <see cref="ServerSession.ConfigUnitStore" />.
        /// </returns>
        IConfigurationUnitStore ReplaceConfigStoreWithStore(IConfigurationUnitStore newStore);

        /// <summary>
        ///     Ups the text for yes.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string UpTextForYES();

        /// <summary>
        ///     Ups the text for no.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        string UpTextForNO();

        /// <summary>
        ///     Updates the session with new language.
        /// </summary>
        /// <param name="language">
        ///     The language.
        /// </param>
        void UpdateSessionWithNewLanguage(ServerLanguage language);

        /// <summary>
        ///     Customs the default error texts.
        /// </summary>
        /// <returns>
        ///     The <see cref="UPErrorTexts" />.
        /// </returns>
        UPErrorTexts CustomDefaultErrorTexts();

        /// <summary>
        ///     Customs the name of the error texts with.
        /// </summary>
        /// <param name="errorTextsName">
        ///     Name of the error texts.
        /// </param>
        /// <returns>
        ///     The <see cref="UPErrorTexts" />.
        /// </returns>
        UPErrorTexts CustomErrorTextsWithName(string errorTextsName);

        /// <summary>
        ///     Allwayses the fullsync when login.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool AllwaysFullsyncWhenLogin();

        /// <summary>
        ///     Services the name of the information for service.
        /// </summary>
        /// <param name="serviceName">
        ///     Name of the service.
        /// </param>
        /// <returns>
        ///     The <see cref="ServerSession.ServiceInfo" />.
        /// </returns>
        ServiceInfo ServiceInfoForServiceName(string serviceName);

        /// <summary>
        ///     Resets the record identification mapper.
        /// </summary>
        void ResetRecordIdentificationMapper();

        /// <summary>
        ///     Creates the synchronizer.
        /// </summary>
        /// <returns>
        ///     The <see cref="UPSynchronization" />.
        /// </returns>
        UPSynchronization CreateSynchronizer();

        /// <summary>
        ///     Creates the time zone.
        /// </summary>
        /// <param name="clientDataTimeZone">
        ///     The client data time zone.
        /// </param>
        /// <param name="currentTimeZone">
        ///     The current time zone.
        /// </param>
        /// <returns>
        ///     The <see cref="UPCRMTimeZone" />.
        /// </returns>
        UPCRMTimeZone CreateTimeZone(string clientDataTimeZone, string currentTimeZone);

        /// <summary>
        ///     Changes the time zone.
        /// </summary>
        /// <param name="timeZone">
        ///     The time zone.
        /// </param>
        /// <returns>
        ///     The <see cref="UPCRMTimeZone" />.
        /// </returns>
        UPCRMTimeZone ChangeTimeZone(UPCRMTimeZone timeZone);

        /// <summary>
        ///     Cancels all operations.
        /// </summary>
        void CancelAllOperations();
    }
}
