// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTreeViewTable.cs" company="Aurea Software Gmbh">
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
//   Tree view table flags
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Tree view table flags
    /// </summary>
    [Flags]
    public enum UPConfigTreeViewTableFlags
    {
        /// <summary>
        /// The expand children.
        /// </summary>
        ExpandChildren = 1 << 3,

        /// <summary>
        /// The hide group node.
        /// </summary>
        HideGroupNode = 1 << 6
    }

    /// <summary>
    /// Tree view table configurations
    /// </summary>
    public class UPConfigTreeViewTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigTreeViewTable"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="parentTable">
        /// The parent table.
        /// </param>
        public UPConfigTreeViewTable(List<object> definition, UPConfigTreeViewTable parentTable)
            : this(definition, parentTable?.TreeView)
        {
            this.Parent = parentTable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigTreeViewTable"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <param name="treeView">
        /// The _tree view.
        /// </param>
        public UPConfigTreeViewTable(List<object> definition, UPConfigTreeView treeView)
        {
            if (definition == null)
            {
                return;
            }

            this.TreeView = treeView;
            this.Nr = JObjectExtensions.ToInt(definition[0]);
            this.InfoAreaId = definition[2] as string;
            this.LinkId = JObjectExtensions.ToInt(definition[3]);
            this.RelationName = definition[4] as string;
            this.SearchAndListName = definition[5] as string;
            this.ExpandName = definition[6] as string;
            this.TableCaptionName = definition[7] as string;
            this.RootMenuLabel = definition[8] as string;
            this.MenuLabel = definition[9] as string;
            this.Flags = (UPConfigTreeViewTableFlags)JObjectExtensions.ToInt(definition[10]);
            this.FilterName = definition[11] as string;

            if (definition.Count > 12)
            {
                this.RecordCustomControl = definition[12] as string;
                this.InfoAreaCustomControl = definition[13] as string;
            }

            if (definition.Count > 14)
            {
                this.RecordCount = JObjectExtensions.ToInt(definition[14]);
                this.Label = definition[15] as string;
            }
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        public List<UPConfigTreeViewTable> ChildNodes { get; private set; }

        /// <summary>
        /// Gets the name of the expand.
        /// </summary>
        /// <value>
        /// The name of the expand.
        /// </value>
        public string ExpandName { get; private set; }

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        /// <value>
        /// The name of the filter.
        /// </value>
        public string FilterName { get; private set; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public UPConfigTreeViewTableFlags Flags { get; private set; }

        /// <summary>
        /// Gets the information area custom control.
        /// </summary>
        /// <value>
        /// The information area custom control.
        /// </value>
        public string InfoAreaCustomControl { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the menu label.
        /// </summary>
        /// <value>
        /// The menu label.
        /// </value>
        public string MenuLabel { get; private set; }

        /// <summary>
        /// Gets the nr.
        /// </summary>
        /// <value>
        /// The nr.
        /// </value>
        public int Nr { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public UPConfigTreeViewTable Parent { get; private set; }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <value>
        /// The record count.
        /// </value>
        public int RecordCount { get; private set; }

        /// <summary>
        /// Gets the record custom control.
        /// </summary>
        /// <value>
        /// The record custom control.
        /// </value>
        public string RecordCustomControl { get; private set; }

        /// <summary>
        /// Gets the name of the relation.
        /// </summary>
        /// <value>
        /// The name of the relation.
        /// </value>
        public string RelationName { get; private set; }

        /// <summary>
        /// Gets the root menu label.
        /// </summary>
        /// <value>
        /// The root menu label.
        /// </value>
        public string RootMenuLabel { get; private set; }

        /// <summary>
        /// Gets the name of the search and list.
        /// </summary>
        /// <value>
        /// The name of the search and list.
        /// </value>
        public string SearchAndListName { get; private set; }

        /// <summary>
        /// Gets the name of the table caption.
        /// </summary>
        /// <value>
        /// The name of the table caption.
        /// </value>
        public string TableCaptionName { get; private set; }

        /// <summary>
        /// Gets the TreeView.
        /// </summary>
        /// <value>
        /// The TreeView.
        /// </value>
        public UPConfigTreeView TreeView { get; private set; }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        public void AddChild(UPConfigTreeViewTable table)
        {
            if (this.ChildNodes == null)
            {
                this.ChildNodes = new List<UPConfigTreeViewTable> { table };
            }
            else
            {
                this.ChildNodes.Add(table);
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string desc;
            if (!string.IsNullOrEmpty(this.RelationName))
            {
                desc = $"{this.InfoAreaId} ({this.RelationName})";
            }
            else if (this.LinkId > 0)
            {
                desc = $"{this.InfoAreaId}#{this.LinkId}";
            }
            else
            {
                desc = this.InfoAreaId;
            }

            return $"{desc}, children:{this.ChildNodes}";
        }
    }
}
