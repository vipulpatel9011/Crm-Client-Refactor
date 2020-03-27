// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoITreeInfoAreaConfig.cs" company="Aurea Software Gmbh">
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
//   The CoITreeInfoAreaConfig.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// CoITreeInfoAreaConfig
    /// </summary>
    public class CoITreeInfoAreaConfig
    {
        /// <summary>
        /// Gets or sets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; set; }

        /// <summary>
        /// Gets or sets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; set; }

        /// <summary>
        /// Gets or sets the function name suffix.
        /// </summary>
        /// <value>
        /// The function name suffix.
        /// </value>
        public string FunctionNameSuffix { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public UPConfigTreeViewTable Config { get; set; }

        /// <summary>
        /// Gets or sets the definition.
        /// </summary>
        /// <value>
        /// The definition.
        /// </value>
        public Dictionary<string, object> Definition { get; set; }
    }
}
