// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTreeView.cs" company="Aurea Software Gmbh">
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
//   Tree view cnfigurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Tree view cnfigurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigTreeView : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigTreeView"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public UPConfigTreeView(List<object> definition)
        {
            this.UnitName = definition[0] as string;
            this.InfoAreaId = definition[1] as string;

            var tableNodes = (definition[2] as JArray)?.ToObject<List<object>>();
            if (tableNodes == null || !tableNodes.Any())
            {
                return;
            }

            var count = tableNodes.Count;

            this.RootNode = new UPConfigTreeViewTable((tableNodes[0] as JArray).ToObject<List<object>>(), this);

            var tableDictionary = new Dictionary<int, UPConfigTreeViewTable> { { this.RootNode.Nr, this.RootNode } };

            for (var i = 1; i < count; i++)
            {
                var tableNodeDef = (tableNodes[i] as JArray)?.ToObject<List<object>>();
                if (tableNodeDef == null)
                {
                    continue;
                }

                var parentNode = JObjectExtensions.ToInt(tableNodeDef[1]);
                var parent = tableDictionary.ValueOrDefault(parentNode);
                if (parent != null)
                {
                    var treeViewTable = new UPConfigTreeViewTable(tableNodeDef, parent);
                    parent.AddChild(treeViewTable);
                    tableDictionary.SetObjectForKey(treeViewTable, treeViewTable.Nr);
                }
            }
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <value>
        /// The root node.
        /// </value>
        public UPConfigTreeViewTable RootNode { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"TreeView {this.UnitName} (InfoArea={this.InfoAreaId}), RootNode={this.RootNode}";
        }
    }
}
