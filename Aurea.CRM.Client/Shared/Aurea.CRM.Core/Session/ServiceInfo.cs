// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfo.cs" company="Aurea Software Gmbh">
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
//   Server information
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    using System.Collections.Generic;

    /// <summary>
    /// Server information
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        public ServiceInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="versionInfo">
        /// The version information.
        /// </param>
        public ServiceInfo(string name, string version, string versionInfo)
        {
            this.Name = name;
            this.Version = version;
            this.VersionInfo = versionInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInfo"/> class.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        public ServiceInfo(List<object> def)
            : this((string)def[0], (string)def[1], (string)def[2])
        {
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the version information.
        /// </summary>
        /// <value>
        /// The version information.
        /// </value>
        public string VersionInfo { get; private set; }

        /// <summary>
        /// Services the information dictionary from array.
        /// </summary>
        /// <param name="def">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, ServiceInfo> ServiceInfoDictionaryFromArray(List<object> def)
        {
            if (def == null || def.Count == 0)
            {
                return null;
            }

            var dict = new Dictionary<string, ServiceInfo>();
            foreach (List<object> element in def)
            {
                var serviceInfo = new ServiceInfo(element);
                if (!string.IsNullOrEmpty(serviceInfo.Name))
                {
                    dict[serviceInfo.Name] = serviceInfo;
                }
            }

            return dict;
        }

        /// <summary>
        /// Determines whether [is at least version] [the specified version].
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsAtLeastVersion(string version)
        {
            return string.CompareOrdinal(this.Version, version) > 0;
        }
    }
}
