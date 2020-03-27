// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryInfo.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSerialEntryInfo
    /// </summary>
    public class UPSerialEntryInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSerialEntryInfo(string name, UPSerialEntry serialEntry)
        {
            this.Name = name;
            Label = name;
            this.SerialEntry = serialEntry;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [vertical rows].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertical rows]; otherwise, <c>false</c>.
        /// </value>
        public bool VerticalRows { get; set; }

        /// <summary>
        /// Gets or sets the column names.
        /// </summary>
        /// <value>
        /// The column names.
        /// </value>
        public List<string> ColumnNames { get; protected set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; set; }

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Results for row.
        /// </summary>
        /// <param name="serialEntryRow">The serial entry row.</param>
        /// <returns></returns>
        public virtual UPSerialEntryInfoResult ResultForRow(UPSERow serialEntryRow)
        {
            return null;
        }

        /// <summary>
        /// Serials the entry information from definition serial entry.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="serialEntry">The serial entry.</param>
        /// <returns></returns>
        public static UPSerialEntryInfo SerialEntryInfoFromDefinitionSerialEntry(Dictionary<string, string> definition, UPSerialEntry serialEntry)
        {
            string name = definition["name"];
            UPSerialEntryInfo serialEntryInfo = null;
            if (name == "Pricing")
            {
                //serialEntryInfo = new UPSerialEntryPricingInfo(definition, serialEntry);
            }
            else
            {
                serialEntryInfo = UPSerialEntrySourceRowInfo.Create(definition, serialEntry);
            }

            string verticalLayoutString = definition.ValueOrDefault("verticalRows");
            if (!string.IsNullOrEmpty(verticalLayoutString))
            {
                serialEntryInfo.VerticalRows = Convert.ToBoolean(verticalLayoutString);
            }

            return serialEntryInfo;
        }
    }
}
