// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebConfigValue.cs" company="Aurea Software Gmbh">
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
//   Defines the web config value
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the web config value
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class WebConfigValue : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConfigValue"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public WebConfigValue(List<object> definition)
        {
            this.UnitName = definition[0] as string;
            this.Value = (string)definition[1];

            int tmp;
            int.TryParse(definition[2].ToString(), out tmp);
            this.Inherited = tmp;
        }

        /// <summary>
        /// Gets the inherited.
        /// </summary>
        /// <value>
        /// The inherited.
        /// </value>
        public int Inherited { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }
    }
}
