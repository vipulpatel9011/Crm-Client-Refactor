// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryParentInfoPanelCell.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryParentInfoPanelCell
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// UPSerialEntryParentInfoPanelCell
    /// </summary>
    public class UPSerialEntryParentInfoPanelCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryParentInfoPanelCell"/> class.
        /// </summary>
        /// <param name="fieldControlTab">The field control tab.</param>
        /// <param name="resultRow">The result row.</param>
        public UPSerialEntryParentInfoPanelCell(FieldControlTab fieldControlTab, UPCRMResultRow resultRow)
        {
            int numberOfFields = fieldControlTab.NumberOfFields;
            List<string> fields = new List<string>();
            if (!string.IsNullOrEmpty(fieldControlTab.Label))
            {
                fields.Add(fieldControlTab.Label);
            }

            for (int index = 0; index < numberOfFields; index++)
            {
                UPConfigFieldControlField field = fieldControlTab.FieldAtIndex(index);
                if (index == 0)
                {
                    IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                    this.ImageName = configStore.InfoAreaConfigById(field.InfoAreaId).ImageName;
                }

                int offset = 0;
                string value = resultRow.FormattedFieldValueAtIndex(index, offset, fieldControlTab);
                index += offset;
                if (!string.IsNullOrEmpty(value))
                {
                    fields.Add(value);
                }
            }

            this.FieldValues = fields;
        }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <value>
        /// The field values.
        /// </value>
        public List<string> FieldValues { get; private set; }
    }
}
