// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="Aurea Software Gmbh">
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
//   Manages the different server configurations.
//   Functionally this is simmilar to RemoteServerManager of CRM.Pad
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Aurea.CRM.Core.Analysis.Value;
using Aurea.CRM.Core.Logging;
using Aurea.CRM.Core.OfflineStorage;

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Platform;

    /// <summary>
    /// Manages the different server configurations.
    /// Functionally this is simmilar to RemoteServerManager of CRM.Pad
    /// </summary>
    public class ServerManager
    {
        private static ServerManager defaultManager;

        private ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        private Dictionary<string, RemoteServer> serverDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManager"/> class.
        /// </summary>
        private ServerManager()
        {
            this.serverDictionary = new Dictionary<string, RemoteServer>();
        }

        /// <summary>
        /// Gets the default manager.
        /// </summary>
        /// <value>
        /// The default manager.
        /// </value>
        public static ServerManager DefaultManager => defaultManager ?? (defaultManager = new ServerManager());

        /// <summary>
        /// Gets the available servers.
        /// </summary>
        /// <value>
        /// The available servers.
        /// </value>
        public IEnumerable<RemoteServer> AvailableServers => this.serverDictionary.Values;

        /// <summary>
        /// Gets last server tuple, first item is server name and second is username
        /// </summary>
        public Tuple<string, string> LastServer { get; set; }

        /// <summary>
        /// Gets the platform service.
        /// </summary>
        /// <value>
        /// The platform service.
        /// </value>
        private static IPlatformService PlatformService => SimpleIoc.Default.GetInstance<IPlatformService>();

        /// <summary>
        /// Adds anew remote server to the servers collection
        /// </summary>
        /// <param name="remoteServer">Remote server to add</param>
        /// <returns>status of the add operation</returns>
        public AddServerStatus Add(RemoteServer remoteServer)
        {
            var availableServers = DefaultManager.AvailableServers.ToList();

            if (availableServers.Any(s => s.Uri != null && s.Uri.Equals(remoteServer.Uri)))
            {
                return AddServerStatus.IdenticalServerFound;
            }

            if (availableServers.Any(s =>
                s.ServerUrl.Equals(remoteServer.ServerUrl) && s.Name == remoteServer.Name && s.ServerIdentification == remoteServer.ServerIdentification))
            {
                return AddServerStatus.SameServerFound;
            }

            if (availableServers.Any(s =>
                this.IsUrlSame(s.ServerUrl, remoteServer.ServerUrl) && s.Name == remoteServer.Name && s.ServerIdentification == remoteServer.ServerIdentification))
            {
                return AddServerStatus.SimilarUrlFound;
            }

            var sameUrlAndNameServers = availableServers
                .Where(s => this.IsUrlSame(s.ServerUrl, remoteServer.ServerUrl) && ( s.Name == remoteServer.Name || s.ServerIdentification == remoteServer.ServerIdentification));

            if (sameUrlAndNameServers.Any())
            {
                return AddServerStatus.SameServerFound;
            }

            this.serverDictionary.Add(remoteServer.Name, remoteServer);
            Task.Run(async () => await this.SaveServerDictionaryToFile());

            return AddServerStatus.Successful;
        }

        /// <summary>
        /// Registers the servers from configuration file.
        /// </summary>
        /// <returns>task representing the asynchronous operation.</returns>
        public async Task RegisterFromConfiguration()
        {
            try
            {
                var serversFromFile = await PlatformService.StorageProvider.LoadObject<Dictionary<string, RemoteServer>>(GetServerConfigFilePath());

                if (serversFromFile != null)
                {
                    foreach (var server in serversFromFile)
                    {
                        if (!this.serverDictionary.ContainsKey(server.Key))
                        {
                            this.serverDictionary.Add(server.Key, server.Value);
                        }
                    }
                }

                // Loads last server key
                this.LastServer = await PlatformService.StorageProvider.LoadObject<Tuple<string, string>>(GetLastServerFilePath());
            }
            catch (Newtonsoft.Json.JsonSerializationException e)
            {
                // Server list got corrupted
                this.Logger.LogError($"Server list configuration file corrupted: {e}");
                Exception error;
                PlatformService.StorageProvider.TryDelete(GetServerConfigFilePath(), out error);
                PlatformService.StorageProvider.TryDelete(GetLastServerFilePath(), out error);
            }
        }

        /// <summary>
        /// Task that helps one wait for Server list to be loaded from configuration
        /// </summary>
        public Task IsServerListLoaded { get; set; }

        /// <summary>
        /// Removes offline data for the server
        /// </summary>
        /// <param name="serverName">Name of server for which the data is to be removed</param>
        public void RemoveServerData(string serverName)
        {
            const string databaseName = "offlineDB.sql";

            var databaseFilenames = this.GetOfflineStoragePaths(databaseName);

            foreach (var databaseFilename in databaseFilenames)
            {
                if (PlatformService.StorageProvider.FileExists(databaseFilename))
                {
                    using (var database = OfflineDatabase.Create(databaseFilename))
                    {
                        DeleteAllTables(database);

                        database.Reset();
                    }
                }
            }

            var rootFolder = PlatformService.StorageProvider.DocumentsFolderPath;

            var directoriesToBeRemoved = PlatformService.StorageProvider
                .GetSubDirectoryNames(rootFolder)
                .Where(name => name.Contains("@" + serverName))
                .Select(name => Path.Combine(rootFolder, name));

            foreach (var directoryPath in directoriesToBeRemoved)
            {
                Exception error;
                if (!PlatformService.StorageProvider.TryDelete(directoryPath, out error))
                {
                    this.Logger.LogError($"Could not remove offline data when server {serverName}, was removed: {error}");
                }
            }
        }

        /// <summary>
        /// Gets the offline storage paths.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Full path</returns>
        public List<string> GetOfflineStoragePaths(string fileName)
        {
            if (this.LastServer == null || string.IsNullOrWhiteSpace(this.LastServer.Item1) || string.IsNullOrWhiteSpace(this.LastServer.Item2))
            {
                return new List<string>();
            }

            var userServerCombinationPath = Path.Combine(
                PlatformService.StorageProvider.DocumentsFolderPath,
                this.LastServer.Item2.ToUpper() + "@" + this.LastServer.Item1);

            return PlatformService.StorageProvider
                .GetSubDirectoryNames(userServerCombinationPath)
                .Select(language => Path.Combine(userServerCombinationPath, language, fileName)).ToList();
        }

        /// <summary>
        /// Removes a server identified by a given name.
        /// </summary>
        /// <param name="name">
        /// The name of the server.
        /// </param>
        public void RemoveByName(string name)
        {
            this.RemoveServerData(name);
            this.serverDictionary.Remove(name);
            Task.Run(async () => await this.SaveServerDictionaryToFile());           
        }

        /// <summary>
        /// Servers the by identification.
        /// </summary>
        /// <param name="serverIdentification">
        /// The serverIdentification
        /// </param>
        /// <returns>
        /// The <see cref="RemoteServer"/>.
        /// </returns>
        public RemoteServer ServerByIdentification(string serverIdentification)
        {
            return this.serverDictionary.Values.FirstOrDefault(s => s.ServerIdentification == serverIdentification);
        }

        /// <summary>
        /// Servers the name of the by.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// server instance
        /// </returns>
        public RemoteServer ServerByName(string name)
        {
            return this.serverDictionary.ContainsKey(name) ? this.serverDictionary[name] : null;
        }

        /// <summary>
        /// Saves last server
        /// </summary>
        /// <param name="lastServerKey">Last server key</param>
        /// <param name="username">Username</param>
        /// <returns>Task object</returns>
        public async Task SaveLastServerToFile(string lastServerKey, string username)
        {
            this.LastServer = new Tuple<string, string>(lastServerKey, username);
            await PlatformService.StorageProvider.SaveObject(this.LastServer, GetLastServerFilePath());
        }

        /// <summary>
        /// Saves last server
        /// </summary>
        /// <param name="lastServerKey">Last server key</param>
        /// <param name="username">Username</param>
        /// <returns>Task object</returns>
        public async Task ClearLastServer()
        {
            this.LastServer = null;
            await PlatformService.StorageProvider.SaveObject(this.LastServer, GetLastServerFilePath());
        }

        private static void DeleteAllTables(OfflineDatabase db)
        {
            const int maxTableCount = 100;
            var command = db.CreateCommand("SELECT name FROM sqlite_master WHERE type = 'table'");
            var tables = command.ExecuteQuery(maxTableCount)
                .Select(r => r.Count > 0 ? r[0].ToString() : null)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToList();

            foreach (var table in tables)
            {
                var dropQuery = "DROP TABLE IF EXISTS " + table;
                db.Execute(dropQuery);
            }
        }

        /// <summary>
        /// Constructs the Last Server file documents folder path.
        /// </summary>
        /// <returns>
        /// the document folder path
        /// </returns>
        private static string GetLastServerFilePath()
        {
            var documentsFolder = PlatformService.StorageProvider.DocumentsFolderPath;
            return Path.Combine(documentsFolder, "lastserver.crmpad");
        }

        /// <summary>
        /// Constructs the ServerConfiguration documents folder path.
        /// </summary>
        /// <returns>
        /// the document folder path
        /// </returns>
        private static string GetServerConfigFilePath()
        {
            var documentsFolder = PlatformService.StorageProvider.DocumentsFolderPath;
            return Path.Combine(documentsFolder, "servers.crmpad");
        }

        private async Task SaveServerDictionaryToFile()
        {
            await PlatformService.StorageProvider.SaveObject(this.serverDictionary, GetServerConfigFilePath());
        }

        private bool IsUrlSame(Uri first, Uri second)
        {
            if (first.Equals(second))
            {
                return true;
            }

            if (second == null)
            {
                return false;
            }

            if (first.IsBaseOf(second) || second.IsBaseOf(first))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Represents the status of the add server operation
    /// </summary>
    public enum AddServerStatus
    {
        /// <summary>
        /// Same server found, new server not added
        /// </summary>
        SameServerFound,

        /// <summary>
        /// Server added succesfully
        /// </summary>
        Successful,

        /// <summary>
        /// A similar url is found
        /// </summary>
        SimilarUrlFound,
        IdenticalServerFound
    }
}
