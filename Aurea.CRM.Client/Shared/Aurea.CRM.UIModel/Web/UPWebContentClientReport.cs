// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentClientReport.cs" company="Aurea Software Gmbh">
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
//   UPWebContentClientReport
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
using System;

    /// <summary>
    /// UPWebContentClientReport
    /// </summary>
    public class UPWebContentClientReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentClientReport"/> class.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <param name="rootNodeName">Name of the root node.</param>
        /// <param name="parentLinkConfig">The parent link configuration.</param>
        public UPWebContentClientReport(string configName, string rootNodeName, string parentLinkConfig)
        {
            var configNameParts = configName.Split('#');
            if (configNameParts.Length == 2)
            {
                this.ExplicitLinkId = true;
                this.LinkId = Convert.ToInt32(configNameParts[1]);
                this.ConfigName = configNameParts[0];
            }
            else
            {
                this.ConfigName = configName;
            }

            this.RootNodeName = !string.IsNullOrEmpty(rootNodeName) ? rootNodeName : this.ConfigName;

            this.ParentLinkConfig = parentLinkConfig;
        }

        /// <summary>
        /// Gets the name of the configuration.
        /// </summary>
        /// <value>
        /// The name of the configuration.
        /// </value>
        public string ConfigName { get; private set; }

        /// <summary>
        /// Gets the name of the root node.
        /// </summary>
        /// <value>
        /// The name of the root node.
        /// </value>
        public string RootNodeName { get; private set; }

        /// <summary>
        /// Gets the parent link configuration.
        /// </summary>
        /// <value>
        /// The parent link configuration.
        /// </value>
        public string ParentLinkConfig { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [explicit link identifier].
        /// </summary>
        /// <value>
        /// <c>true</c> if [explicit link identifier]; otherwise, <c>false</c>.
        /// </value>
        public bool ExplicitLinkId { get; private set; }
    }
}
