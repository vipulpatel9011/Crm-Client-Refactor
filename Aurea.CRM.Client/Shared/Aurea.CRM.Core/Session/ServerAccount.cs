// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerAccount.cs" company="Aurea Software Gmbh">
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
//   Manages CRM accounts.
//   The implementation is simmilar to UPCRMAccount
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Logging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;
    using Platform;

    /// <summary>
    /// Manages CRM accounts.
    /// The implementation is simmilar to UPCRMAccount
    /// </summary>
    public class ServerAccount
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAccount"/> class.
        /// the parameter less constructor is there ONLY for XML serialization
        /// </summary>
        public ServerAccount()
        {
            this.FullSyncDates = new Dictionary<string, DateTime>();
            this.IncrementalSyncDates = new Dictionary<string, DateTime>();
            this.LoginDates = new Dictionary<string, DateTime>();
            this.ServiceInfos = new Dictionary<string, ServiceInfo>();
            this.AttributesByLanguage = new Dictionary<string, Dictionary<string, object>>();
            this.SystemOptions = new Dictionary<string, object>();
            this.SyncDocuments = new List<SyncDocument>();

            this.Salt = GenerateSalt256();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAccount"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <param name="rasInstanceName">
        /// Name of the ras instance.
        /// </param>
        public ServerAccount(
            RemoteServer server,
            string username,
            string password,
            string rasApplicationId,
            string rasInstanceName)
            : this()
        {
            this.Server = server;
            this.UserName = username;
            this.Password = password;
            this.RasApplicationId = rasApplicationId;
            this.RasInstanceName = rasInstanceName;
            this.PasswordCaseInsensitive = false;

            // Write updated information
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerAccount"/> class.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        public ServerAccount(RemoteServer server, string username)
            : this(server, username, null, null, null)
        {
        }

        /// <summary>
        /// Gets the account caches path.
        /// </summary>
        /// <value>
        /// The account caches path.
        /// </value>
        public string AccountCachesPath => AccountCachesPathForServer(this.Server, this.UserName, this.RasApplicationId);

        /// <summary>
        /// Gets the account configuration file path.
        /// </summary>
        /// <value>
        /// The account configuration file path.
        /// </value>
        public string AccountConfigurationFilePath => Path.Combine(this.AccountPath, "account.crmpad");

        /// <summary>
        /// Gets the account folder.
        /// </summary>
        /// <value>
        /// The account folder.
        /// </value>
        public string AccountFolder => AccountFolderForServer(this.Server, this.UserName, this.RasApplicationId);

        /// <summary>
        /// Gets the account path.
        /// </summary>
        /// <value>
        /// The account path.
        /// </value>
        public string AccountPath => AccountPathForServer(this.Server, this.UserName, this.RasApplicationId);

        /// <summary>
        /// Gets the attributes by language.
        /// </summary>
        /// <value>
        /// The attributes by language.
        /// </value>
        public Dictionary<string, Dictionary<string, object>> AttributesByLanguage { get; private set; }

        /// <summary>
        /// Gets or sets the automatic login password.
        /// </summary>
        /// <value>
        /// The automatic login password.
        /// </value>
        public string AutoLoginPassword { get; set; }

        /// <summary>
        /// Gets the full synchronize dates.
        /// </summary>
        /// <value>
        /// The full synchronize dates.
        /// </value>
        public Dictionary<string, DateTime> FullSyncDates { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has time zone information.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has time zone information; otherwise, <c>false</c>.
        /// </value>
        public bool HasTimeZoneInfo { get; set; }

        /// <summary>
        /// Gets the incremental synchronize dates.
        /// </summary>
        /// <value>
        /// The incremental synchronize dates.
        /// </value>
        public Dictionary<string, DateTime> IncrementalSyncDates { get; private set; }

        /// <summary>
        /// Gets or sets the last used language key.
        /// </summary>
        /// <value>
        /// The last used language key.
        /// </value>
        public string LastUsedLanguageKey { get; set; }

        /// <summary>
        /// Gets the login dates.
        /// </summary>
        /// <value>
        /// The login dates.
        /// </value>
        public Dictionary<string, DateTime> LoginDates { get; private set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [password case insensitive].
        /// </summary>
        /// <value>
        /// <c>true</c> if [password case insensitive]; otherwise, <c>false</c>.
        /// </value>
        public bool PasswordCaseInsensitive { get; set; }

        /// <summary>
        /// Gets or sets the ras application identifier.
        /// </summary>
        /// <value>
        /// The ras application identifier.
        /// </value>
        public string RasApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the name of the ras instance.
        /// </summary>
        /// <value>
        /// The name of the ras instance.
        /// </value>
        public string RasInstanceName { get; set; }

        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        public byte[] Salt { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public RemoteServer Server { get; set; }

        /// <summary>
        /// Gets the service information.
        /// </summary>
        /// <value>
        /// The service information.
        /// </value>
        public Dictionary<string, ServiceInfo> ServiceInfos { get; private set; }

        /// <summary>
        /// Gets the system options.
        /// </summary>
        /// <value>
        /// The system options.
        /// </value>
        public Dictionary<string, object> SystemOptions { get; private set; }

        /// <summary>
        /// Gets the synchronize documents.
        /// </summary>
        /// <value>
        /// The synchronize documents.
        /// </value>
        public List<SyncDocument> SyncDocuments { get; private set; }

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the platform service.
        /// </summary>
        /// <value>
        /// The platform service.
        /// </value>
        private static IPlatformService PlatformService => SimpleIoc.Default.GetInstance<IPlatformService>();

        /// <summary>
        /// Accounts the caches path for server.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <returns>
        /// the caches folder path
        /// </returns>
        public static string AccountCachesPathForServer(RemoteServer server, string username, string rasApplicationId)
        {
            var platform = SimpleIoc.Default.GetInstance<IPlatformService>();
            var cachesFolder = platform.StorageProvider.CachesFolderPath;
            var serverAccountFolder = AccountFolderForServer(server, username, rasApplicationId);
            return Path.Combine(cachesFolder, serverAccountFolder);
        }

        /// <summary>
        /// Constructs the folder name of a account folder.
        /// </summary>
        /// <param name="server">
        /// The server instance.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <returns>
        /// the account folder name
        /// </returns>
        public static string AccountFolderForServer(RemoteServer server, string username, string rasApplicationId)
        {
            var caseInsensitiveUsername = username.ToUpper();

            return !string.IsNullOrWhiteSpace(rasApplicationId)
                       ? $"{caseInsensitiveUsername}@{rasApplicationId}"
                       : $"{caseInsensitiveUsername}@{server.ServerIdentification}";
        }

        /// <summary>
        /// Creates an account instance for a given server instance.
        /// </summary>
        /// <param name="server">
        /// The server instance.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <param name="passwordCheck">
        /// if set to <c>true</c> for [password check].
        /// </param>
        /// <returns>
        /// the account instance
        /// </returns>
        public static async Task<ServerAccount> AccountForServer(
            RemoteServer server,
            string username,
            string password,
            string rasApplicationId,
            bool passwordCheck)
        {
            var localAccount = await LocalAccountForServer(server, username, rasApplicationId);
            if (!passwordCheck)
            {
                return localAccount;
            }

            if (localAccount != null && localAccount.PasswordMatch(password))
            {
                return localAccount;
            }

            return null;
        }

        /// <summary>
        /// Constructs the Accounts documents folder path.
        /// </summary>
        /// <param name="server">
        /// The server instance.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <returns>
        /// the document folder path
        /// </returns>
        public static string AccountPathForServer(RemoteServer server, string username, string rasApplicationId)
        {
            var documentsFolder = SimpleIoc.Default.GetInstance<IPlatformService>()?.StorageProvider?.DocumentsFolderPath;
            if (documentsFolder == null)
            {
                return string.Empty;
            }

            var accountFolder = AccountFolderForServer(server, username, rasApplicationId);
            return Path.Combine(documentsFolder, accountFolder);
        }

        /// <summary>
        /// Determines whether [has local account for server] for [the specified server].
        /// </summary>
        /// <param name="server">
        /// The server instance.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <returns>
        /// true if local account exists;else false
        /// </returns>
        public static async Task<bool> HasLocalAccountForServer(
            RemoteServer server,
            string username,
            string rasApplicationId)
        {
            return await LocalAccountForServer(server, username, rasApplicationId) != null;
        }

        /// <summary>
        /// Locals the account for server.
        /// </summary>
        /// <param name="server">
        /// The server.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="rasApplicationId">
        /// The ras application identifier.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<ServerAccount> LocalAccountForServer(
            RemoteServer server,
            string username,
            string rasApplicationId)
        {
            if (server == null)
            {
                return null;
            }

            // set the username
            var checkedUserName = username;
            if (server.AuthenticationType == ServerAuthenticationType.Revolution && string.IsNullOrWhiteSpace(rasApplicationId))
            {
                // Get the RAS application Id from the mappings
                rasApplicationId =
                    RasLoginAppMapping.CurrentMapping()
                        .AppIdForServerIdRasLoginName(server.ServerIdentification, username.ToLower());
                if (!string.IsNullOrWhiteSpace(rasApplicationId))
                {
                    var usernameTokens = username.Split('\\');
                    if (usernameTokens.Length > 1 && !rasApplicationId.Equals(server.ServerIdentification))
                    {
                        checkedUserName = usernameTokens[1];
                    }
                }
                else
                {
                    return null;
                }
            }

            var accountFolder = AccountPathForServer(server, checkedUserName, rasApplicationId);
            Logger?.LogMessage(new LogMessage($"ServerAccount - Try account folder {accountFolder}", LogLevel.Info));

            if (PlatformService?.StorageProvider == null || !PlatformService.StorageProvider.DirectoryExists(accountFolder))
            {
                return null;
            }

            Logger?.LogMessage(new LogMessage($"ServerAccount - Use account Folder {accountFolder}", LogLevel.Info));

            var accountConfigurationFile = Path.Combine(accountFolder, "account.crmpad");
            if (!PlatformService.StorageProvider.FileExists(accountConfigurationFile))
            {
                return null;
            }

            var platform = SimpleIoc.Default.GetInstance<IPlatformService>();
            if (platform == null)
            {
                return null;
            }

            return await platform.StorageProvider.LoadObject<ServerAccount>(accountConfigurationFile);
        }

        /// <summary>
        /// Derives the key from string.
        /// </summary>
        /// <param name="theString">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DeriveKeyFromString(string theString)
        {
            var data = Encoding.UTF8.GetBytes(theString ?? string.Empty);
            var kdf = new Pkcs5S2ParametersGenerator();
            kdf.Init(data, this.Salt, 10000);

            var hash = ((KeyParameter)kdf.GenerateDerivedMacParameters(8 * this.Salt.Length)).GetKey();

            return Encoding.UTF8.GetString(hash, 0, hash.Length);
        }

        /// <summary>
        /// Fulls the synchronize date for language.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime? FullSyncDateForLanguage(string languageKey)
        {
            return this.FullSyncDates.ContainsKey(languageKey) ? (DateTime?)this.FullSyncDates[languageKey] : null;
        }

        /// <summary>
        /// Incrementals the synchronize date for language.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime? IncrementalSyncDateForLanguage(string languageKey)
        {
            return this.IncrementalSyncDates.ContainsKey(languageKey)
                       ? (DateTime?)this.IncrementalSyncDates[languageKey]
                       : null;
        }

        /// <summary>
        /// Construct the caches directory path for a local account.
        /// </summary>
        /// <param name="folder">
        /// The folder name.
        /// </param>
        /// <returns>
        /// the local folder name
        /// </returns>
        public string LocalCachesDirectoryForAccountFolder(string folder)
        {
            var cachesFolder = PlatformService.StorageProvider.CachesFolderPath;
            return Path.Combine(cachesFolder, folder);
        }

        /// <summary>
        /// Logins the date for language.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime? LoginDateForLanguage(string languageKey)
        {
            return string.IsNullOrWhiteSpace(languageKey) || !this.LoginDates.ContainsKey(languageKey)
                       ? (DateTime?)null
                       : this.LoginDates[languageKey];
        }

        /// <summary>
        /// Matches a given password against a stored password.
        /// </summary>
        /// <param name="passwordToCheck">
        /// The password to check.
        /// </param>
        /// <returns>
        /// true if matches;else false
        /// </returns>
        public bool PasswordMatch(string passwordToCheck)
        {
            var derivedKeyFromPasswordToCheck =
                this.DeriveKeyFromString(this.PasswordCaseInsensitive ? passwordToCheck.ToLower() : passwordToCheck);
            return this.Password.Equals(passwordToCheck) || this.Password.Equals(derivedKeyFromPasswordToCheck);
        }

        /// <summary>
        /// Updates the full synchronize date.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        public void UpdateFullSyncDate(string languageKey)
        {
            this.FullSyncDates[languageKey] = DateTime.UtcNow;

            // Write updated information
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Updates the incremental synchronize date.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        public void UpdateIncrementalSyncDate(string languageKey)
        {
            this.IncrementalSyncDates[languageKey] = DateTime.UtcNow;

            // Write updated information
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Updates the local password.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="caseInSensitive">
        /// if set to <c>true</c> [case in sensitive].
        /// </param>
        /// <param name="passwordSaveAllowed">
        /// if set to <c>true</c> [password save allowed].
        /// </param>
        public void UpdateLocalPassword(string password, bool caseInSensitive, bool passwordSaveAllowed)
        {
            this.PasswordCaseInsensitive = caseInSensitive;
            this.Password = password;
            if (passwordSaveAllowed
                && (this.Server.LoginMode == ServerLoginMode.StorePassword || this.Server.LoginMode == ServerLoginMode.StorePasswordLogin))
            {
                if (password == null)
                {
                    password = string.Empty;
                }

                this.AutoLoginPassword = password;
            }

            // Write updated information
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Updates the login date.
        /// </summary>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        public void UpdateLoginDate(string languageKey)
        {
            this.LoginDates[languageKey] = DateTime.UtcNow;

            // Write updated information
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Updates the service infos.
        /// </summary>
        /// <param name="serviceInfo">
        /// The service information.
        /// </param>
        public void UpdateServiceInfos(Dictionary<string, ServiceInfo> serviceInfo)
        {
            this.ServiceInfos = serviceInfo;
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Upgrades an existing legacy account into a new RAS account.
        /// </summary>
        /// <param name="rasAppId">
        /// The ras application identifier.
        /// </param>
        /// <param name="rasUsername">
        /// The ras username.
        /// </param>
        /// <param name="rasInstanceName">
        /// Name of the ras instance.
        /// </param>
        public void UpgradeToNewRasAccount(string rasAppId, string rasUsername, string rasInstanceName)
        {
            if (this.Server == null || this.Server.AuthenticationType != ServerAuthenticationType.Revolution)
            {
                Logger.LogError("upgrade to new ras without ras server!");
                return;
            }

            var newAccountFolder = AccountPathForServer(this.Server, rasUsername, rasAppId);
            var storage = PlatformService.StorageProvider;

            if (storage.DirectoryExists(newAccountFolder))
            {
                Logger.LogError($"Error upgrading to new ras New Account Folder already present: {newAccountFolder}");
                return;
            }

            try
            {
                Exception ex;

                // Copy files to the new directory path
                storage.TryMove(this.AccountPath, newAccountFolder, out ex);

                // delete the old files
                storage.TryDelete(this.AccountPath, out ex);

                // process the cache directory
                var oldAccountFolder = AccountFolderForServer(this.Server, this.UserName, null);
                var oldLocalCachePath = this.LocalCachesDirectoryForAccountFolder(oldAccountFolder);

                var newLocalCachePath = this.LocalCachesDirectoryForAccountFolder(newAccountFolder);

                if (storage.DirectoryExists(oldLocalCachePath))
                {
                    storage.TryMove(oldLocalCachePath, newLocalCachePath, out ex);

                    // remove old cache files
                    storage.TryDelete(oldLocalCachePath, out ex);
                }
            }
            catch (Exception)
            {
                return;
            }

            this.UserName = rasUsername;
            this.RasInstanceName = rasInstanceName;
            this.RasApplicationId = rasAppId;

            // Write updated information
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Returns the next synchronize document to download.
        /// </summary>
        /// <returns></returns>
        public SyncDocument NextSyncDocumentToDownload()
        {
            return this.SyncDocuments.Count > 0 ? this.SyncDocuments[0] : null;
        }

        /// <summary>
        /// Removes the synchronize document from download queue.
        /// </summary>
        /// <param name="syncDocument">The synchronize document.</param>
        public void RemoveSyncDocumentFromDownloadQueue(SyncDocument syncDocument)
        {
            this.SyncDocuments.Remove(syncDocument);
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Removes the synchronize document from download queue.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void RemoveSyncDocumentFromDownloadQueue(Uri url)
        {
            UPSyncDocumentDownloadUrlCache urlCache = null;
            if (this.SyncDocuments.Count > 20)
            {
                urlCache = new UPSyncDocumentDownloadUrlCache();
            }

            SyncDocument syncDocumentToRemove = this.SyncDocuments.FirstOrDefault(document => document.DownloadUrlsForDocument(urlCache).Contains(url));

            if (syncDocumentToRemove != null)
            {
                this.RemoveSyncDocumentFromDownloadQueue(syncDocumentToRemove);
            }
        }

        /// <summary>
        /// Updates the synchronize documents.
        /// </summary>
        /// <param name="newSyncDocuments">The new synchronize documents.</param>
        public void UpdateSyncDocuments(List<SyncDocument> newSyncDocuments)
        {
            this.SyncDocuments = new List<SyncDocument>();
            this.SyncDocuments.AddRange(newSyncDocuments);
            this.WriteAccountInformation();
        }

        /// <summary>
        /// Adds the synchronize documents.
        /// </summary>
        /// <param name="syncDocuments">The synchronize documents.</param>
        public void AddSyncDocuments(List<SyncDocument> syncDocuments)
        {
            if (syncDocuments != null)
            {
                this.SyncDocuments.AddRange(syncDocuments);
            }

            this.WriteAccountInformation();
        }

        /// <summary>
        /// Generates the salt256.
        /// </summary>
        /// <returns>
        /// The <see cref="T:byte[]"/>.
        /// </returns>
        private static byte[] GenerateSalt256() => new SecureRandom().GenerateSeed(32);

        /// <summary>
        /// Writes the account information.
        /// </summary>
        private void WriteAccountInformation()
        {
            var clone = this.MemberwiseClone() as ServerAccount;
            if (clone == null)
            {
                return;
            }

            clone.Password = this.DeriveKeyFromString(this.Password);
            PlatformService.StorageProvider.SaveObject(clone, this.AccountConfigurationFilePath);
        }
    }
}
