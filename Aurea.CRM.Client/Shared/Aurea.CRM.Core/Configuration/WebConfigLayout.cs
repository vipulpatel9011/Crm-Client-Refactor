// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebConfigLayout.cs" company="Aurea Software Gmbh">
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
//   configurations related to web config layout
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// configurations related to web config layout
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class WebConfigLayout : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConfigLayout"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public WebConfigLayout(List<object> definition)
        {
            this.UnitName = definition[0] as string;
            var tabDefs = (List<object>)definition[1];

            this.Tabs = new List<WebConfigLayoutTab>(tabDefs.Count);
            foreach (var tabdef in tabDefs)
            {
                this.Tabs.Add(new WebConfigLayoutTab((List<object>)tabdef));
            }
        }

        /// <summary>
        /// Gets the tab count.
        /// </summary>
        /// <value>
        /// The tab count.
        /// </value>
        public int TabCount => this.Tabs?.Count ?? 0;

        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <value>
        /// The tabs.
        /// </value>
        protected List<WebConfigLayoutTab> Tabs { get; private set; }

        /// <summary>
        /// Tabs at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="WebConfigLayoutTab"/>.
        /// </returns>
        public WebConfigLayoutTab TabAtIndex(int index)
        {
            return this.Tabs != null && index < this.Tabs.Count ? this.Tabs[index] : null;
        }
    }
}
