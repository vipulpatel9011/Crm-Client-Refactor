// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RasLoginAppMapping.cs" company="Aurea Software Gmbh">
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
//   Provides the server and login mappings for RAS
//   The implementation is simmilar to the UPCRMRASLoginAppMapping in CRM.Pad
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Services;

    /// <summary>
    /// Provides the server and login mappings for RAS
    /// The implementation is simmilar to the UPCRMRASLoginAppMapping in CRM.Pad
    /// </summary>
    public class RasLoginAppMapping
    {
        /// <summary>
        /// The singleton lock.
        /// </summary>
        private static readonly object SingletonLock = new object();

        /// <summary>
        /// The instance.
        /// </summary>
        private static RasLoginAppMapping instance;

        /// <summary>
        /// The server app id mapping.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> serverAppIdMapping;

        /// <summary>
        /// The server instance mapping.
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> serverInstanceMapping;

        /// <summary>
        /// Prevents a default instance of the <see cref="RasLoginAppMapping"/> class from being created.
        /// </summary>
        private RasLoginAppMapping()
        {
            this.serverAppIdMapping = new Dictionary<string, Dictionary<string, string>>();
            this.serverInstanceMapping = new Dictionary<string, Dictionary<string, string>>();
        }

        /// <summary>
        /// Currents the mapping.
        /// </summary>
        /// <returns>
        /// The <see cref="RasLoginAppMapping"/>.
        /// </returns>
        public static RasLoginAppMapping CurrentMapping()
        {
            if (instance != null)
            {
                return instance;
            }

            lock (SingletonLock)
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = new RasLoginAppMapping();
                var readObject = PListService.LoadPropertiesAsArray(
                    PListService.LoginAppMapping,
                    PListService.DocumentPath);
                if (readObject != null && readObject.Count > 1)
                {
                    instance.SetInstanceMapping(readObject[0].Value);
                    instance.SetAppIdMapping(readObject[1].Value);
                }
            }

            return instance;
        }

        /// <summary>
        /// Applications the name of the identifier for server identifier ras login.
        /// </summary>
        /// <param name="serverIdentification">
        /// The server identification.
        /// </param>
        /// <param name="loginName">
        /// Name of the login.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string AppIdForServerIdRasLoginName(string serverIdentification, string loginName)
        {
            var userAppIdNameMapping = this.serverAppIdMapping.ContainsKey(serverIdentification)
                                           ? this.serverAppIdMapping[serverIdentification]
                                           : null;
            return userAppIdNameMapping?.ValueOrDefault(loginName);
        }

        /// <summary>
        /// Instances the name of the name for server identifier ras login.
        /// </summary>
        /// <param name="serverIdentification">
        /// The server identification.
        /// </param>
        /// <param name="loginName">
        /// Name of the login.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string InstanceNameForServerIdRasLoginName(string serverIdentification, string loginName)
        {
            var userInstaceNameMapping = this.serverInstanceMapping.ContainsKey(serverIdentification)
                                             ? this.serverInstanceMapping[serverIdentification]
                                             : null;
            return userInstaceNameMapping?.ValueOrDefault(loginName);
        }

        /// <summary>
        /// Sets the name of the application identifier for server identifier ras login.
        /// </summary>
        /// <param name="appId">
        /// The application identifier.
        /// </param>
        /// <param name="serverIdentification">
        /// The server identification.
        /// </param>
        /// <param name="loginName">
        /// Name of the login.
        /// </param>
        public void SetAppIdForServerIdRasLoginName(string appId, string serverIdentification, string loginName)
        {
            var userAppIdMapping = this.serverAppIdMapping.ValueOrDefault(serverIdentification);
            if (userAppIdMapping == null)
            {
                userAppIdMapping = new Dictionary<string, string>();
                this.serverAppIdMapping[serverIdentification] = userAppIdMapping;
            }

            userAppIdMapping[loginName] = appId;
        }

        /// <summary>
        /// Sets the application identifier mapping.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        public void SetAppIdMapping(Dictionary<string, PListValue> mapping)
        {
            this.serverAppIdMapping = mapping?.ToDictionary(
                m => m.Key,
                m => ((PList)m.Value.Value).ToDictionary(p => p.Key, p => (string)p.Value.Value));
        }

        /// <summary>
        /// Sets the instance mapping.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        public void SetInstanceMapping(Dictionary<string, PListValue> mapping)
        {
            this.serverInstanceMapping = mapping?.ToDictionary(
                m => m.Key,
                m => ((PList)m.Value.Value).ToDictionary(p => p.Key, p => (string)p.Value.Value));
        }

        /// <summary>
        /// Sets the name of the instance name for server identifier ras login.
        /// </summary>
        /// <param name="instanceName">
        /// Name of the instance.
        /// </param>
        /// <param name="serverIdentification">
        /// The server identification.
        /// </param>
        /// <param name="loginName">
        /// Name of the login.
        /// </param>
        public void SetInstanceNameForServerIdRasLoginName(
            string instanceName,
            string serverIdentification,
            string loginName)
        {
            var userInstanceNameMapping = this.serverInstanceMapping.ValueOrDefault(serverIdentification);
            if (userInstanceNameMapping == null)
            {
                userInstanceNameMapping = new Dictionary<string, string>();
                this.serverInstanceMapping[serverIdentification] = userInstanceNameMapping;
            }

            userInstanceNameMapping[loginName] = instanceName;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"appIdMapping: {this.serverAppIdMapping} instanceMapping: {this.serverInstanceMapping}";
        }

        /// <summary>
        /// Writes the mapping.
        /// </summary>
        public void WriteMapping()
        {
            var list = new ArrayPList()
                           {
                               new PListValue
                                   {
                                       PropertyType = "dict",
                                       Value = ToPList(this.serverInstanceMapping)
                                   },
                               new PListValue
                                   {
                                       PropertyType = "dict",
                                       Value = ToPList(this.serverAppIdMapping)
                                   },
                           };

            PListService.Save(list, PListService.LoginAppMapping, PListService.DocumentPath);
        }

        /// <summary>
        /// Converts to a p list.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        private static Dictionary<string, PListValue> ToPList(Dictionary<string, Dictionary<string, string>> mapping)
        {
            return mapping.ToDictionary(
                p => p.Key,
                p =>
                new PListValue
                    {
                        PropertyType = "dict",
                        Value =
                            p.Value.ToDictionary(
                                k => k.Key,
                                k => new PListValue { PropertyType = "string", Value = k.Value })
                    });
        }
    }
}
