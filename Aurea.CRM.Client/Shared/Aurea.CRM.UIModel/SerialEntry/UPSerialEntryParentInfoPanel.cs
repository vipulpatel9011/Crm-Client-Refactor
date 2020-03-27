// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryParentInfoPanel.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryParentInfoPanel
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPSerialEntryParentInfoPanel
    /// </summary>
    public class UPSerialEntryParentInfoPanel
    {
        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public List<UPSerialEntryParentInfoPanelCell> Cells { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryParentInfoPanel"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="resultRow">The result row.</param>
        public UPSerialEntryParentInfoPanel(FieldControl fieldControl, UPCRMResultRow resultRow)
        {
            int tabs = fieldControl.NumberOfTabs;
            List<UPSerialEntryParentInfoPanelCell> cells = new List<UPSerialEntryParentInfoPanelCell>();
            for (int index = 0; index < tabs; index++)
            {
                FieldControlTab tabControl = fieldControl.TabAtIndex(index);
                if (tabControl.NumberOfFields > 0 && string.IsNullOrEmpty(tabControl.Type))
                {
                    UPSerialEntryParentInfoPanelCell cell = new UPSerialEntryParentInfoPanelCell(tabControl, resultRow);
                    string ownImageName = fieldControl.ValueForAttribute($"ParentInfoImage{index}") ??
                                          tabControl.ValueForAttribute("ParentInfoImage");

                    if (!string.IsNullOrEmpty(ownImageName))
                    {
                        cell.ImageName = ownImageName;
                    }

                    int minFields = !string.IsNullOrEmpty(tabControl.Label) ? 1 : 0;
                    if (cell.FieldValues.Count > minFields)
                    {
                        cells.Add(cell);
                    }
                }
            }

            this.Cells = cells;
        }
    }
}
