// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MListGroup.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm list group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The upm list group.
    /// </summary>
    public class UPMListGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMListGroup"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        public UPMListGroup(IIdentifier identifier, UPMAction action)
            : base(identifier)
        {
            this.Action = action;
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        public UPMAction Action { get; }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        public TableCellStyle CellStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disable paging.
        /// </summary>
        public bool DisablePaging { get; set; }

        /// <summary>
        /// Gets the list rows.
        /// </summary>
        public List<UPMListRow> ListRows
        {
            get
            {
                return this.Children.Select(x => x as UPMListRow).ToList();
            }
        }

        /// <summary>
        /// Gets or sets the max results.
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// The number of list rows.
        /// </summary>
        public int NumberOfListRows => this.Children.Count;

        /// <summary>
        /// The add list row.
        /// </summary>
        /// <param name="listRow">
        /// The list row.
        /// </param>
        public void AddListRow(UPMListRow listRow)
        {
            this.AddChild(listRow);
        }

        /// <summary>
        /// The list row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMListRow"/>.
        /// </returns>
        public UPMListRow ListRowAtIndex(int index)
        {
            return (UPMListRow)this.Children[index];
        }
    }
}
