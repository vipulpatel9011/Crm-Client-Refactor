// <copyright file="UPContainerMetaInfoAreaTreeNode.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.CRM.Query
{
    using System.Collections.Generic;

    /// <summary>
    /// Container meta info area tree node
    /// </summary>
    public class UPContainerMetaInfoAreaTreeNode
    {

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public UPContainerInfoAreaMetaInfo Node { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfoAreaTreeNode"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="key">The key.</param>
        public UPContainerMetaInfoAreaTreeNode(UPContainerInfoAreaMetaInfo node, string key)
        {
            this.Node = node;
            this.Key = key;
        }

        /// <summary>
        /// Adds the child node key.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UPContainerMetaInfoAreaTreeNode AddChildNodeKey(UPContainerInfoAreaMetaInfo treeNode, string key)
        {
            var childNode = new UPContainerMetaInfoAreaTreeNode(treeNode, key);
            if (this.Children == null)
            {
                this.Children = new Dictionary<string, UPContainerMetaInfoAreaTreeNode>
                {
                    { childNode.Key, childNode }
                };

                this.OrderedChildren = new List<UPContainerMetaInfoAreaTreeNode>() { childNode };
            }
            else
            {
                this.Children[childNode.Key] = childNode;
                this.OrderedChildren.Add(childNode);
            }

            return childNode;
        }

        /// <summary>
        /// Ordereds the information area list.
        /// </summary>
        /// <returns></returns>
        public List<UPContainerMetaInfoAreaTreeNode> OrderedInfoAreaList()
        {
            var orderedList = new List<UPContainerMetaInfoAreaTreeNode> { this };
            if (this.Children == null || this.Children.Count == 0 || this.OrderedChildren == null)
            {
                return orderedList;
            }

            foreach (var childNode in this.OrderedChildren)
            {
                orderedList.AddRange(childNode.OrderedInfoAreaList());
            }

            return orderedList;
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        protected Dictionary<string, UPContainerMetaInfoAreaTreeNode> Children { get; private set; }

        /// <summary>
        /// The ordered children
        /// </summary>
        protected List<UPContainerMetaInfoAreaTreeNode> OrderedChildren;
    }
}
