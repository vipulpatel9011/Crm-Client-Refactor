// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCircleOfInfluencePage.cs" company="Aurea Software Gmbh">
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
//   The UPMCircleOfInfluence Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Pages
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.CircleOfInfluence;

    /// <summary>
    /// UPMCircleOfInfluencePage
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Pages.Page" />
    public class UPMCircleOfInfluencePage : Page
    {
        private UPMCoINode rootNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMCircleOfInfluencePage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the root node.
        /// </summary>
        /// <value>
        /// The root node.
        /// </value>
        public UPMCoINode RootNode
        {
            get
            {
                return this.rootNode;
            }

            set
            {
                this.rootNode = value;
                this.AddChild(this.rootNode);
            }
        }

        /// <summary>
        /// Gets or sets the refreshed node.
        /// </summary>
        /// <value>
        /// The refreshed node.
        /// </value>
        public UPMCoINode RefreshedNode { get; set; }

        /// <summary>
        /// Gets or sets the configuration provider.
        /// </summary>
        /// <value>
        /// The configuration provider.
        /// </value>
        public UPMCoIViewConfigProvider ConfigProvider { get; set; }

        /// <summary>
        /// Gets or sets the calculated metadata.
        /// </summary>
        /// <value>
        /// The calculated metadata.
        /// </value>
        public Dictionary<string, object> CalculatedMetadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [refresh needed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [refresh needed]; otherwise, <c>false</c>.
        /// </value>
        public bool RefreshNeeded { get; set; }

        private UPMCoINodeViewConfig RekNodeConfigForNodeIdCurrentNodeCurrentLevel(IIdentifier nodeId, UPMCoINode currentNode, int level)
        {
            for (int index = 0; index < currentNode.ChildCount; index++)
            {
                UPMCoINode childNode = currentNode.ChildNodeAtIndex(index);
                if (this.RootNode.Identifier.MatchesIdentifier(nodeId))
                {
                    return this.ConfigProvider.ConfigAtIndex(0).ConfigAtIndex(level).RepeatedConfigAtIndex(index);
                }

                UPMCoINodeViewConfig config = this.RekNodeConfigForNodeIdCurrentNodeCurrentLevel(nodeId, childNode, level + 1);
                if (config != null)
                {
                    return config;
                }
            }

            return null;
        }
    }
}
