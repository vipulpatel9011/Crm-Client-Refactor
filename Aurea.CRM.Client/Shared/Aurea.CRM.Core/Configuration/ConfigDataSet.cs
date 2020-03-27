// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigDataSet.cs" company="Aurea Software Gmbh">
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
//   Dataset configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Dataset configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigDataSet : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigDataSet"/> class.
        /// </summary>
        /// <param name="defArray">
        /// The definition array.
        /// </param>
        public UPConfigDataSet(List<object> defArray)
        {
            this.FilterNames = new List<object>();
            this.UnitName = (string)defArray[0];
            this.InfoAreaId = (string)defArray[1];
            this.Label = (string)defArray[4];
            var filterDefs = defArray[2] as JArray;
            if (filterDefs == null)
            {
                return;
            }

            foreach (var def in filterDefs)
            {
                var filterDef = def as JArray;
                if (filterDef == null)
                {
                    continue;
                }

                if ("Documents".Equals((string)filterDef[0]))
                {
                    this.SyncDocumentFieldGroupName = (string)filterDef[1];
                }
            }
        }

        /// <summary>
        /// Gets the filter names.
        /// </summary>
        /// <value>
        /// The filter names.
        /// </value>
        public List<object> FilterNames { get; private set; }

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
        /// Gets the name of the synchronize document field group.
        /// </summary>
        /// <value>
        /// The name of the synchronize document field group.
        /// </value>
        public string SyncDocumentFieldGroupName { get; private set; }

        /// <summary>
        /// Displays the text.
        /// </summary>
        /// <returns>the display text</returns>
        public string DisplayText()
        {
            if (!string.IsNullOrEmpty(this.Label))
            {
                return this.Label;
            }

            var infoArea = ConfigurationUnitStore.DefaultStore.InfoAreaConfigById(this.InfoAreaId);
            return infoArea != null ? infoArea.PluralName : this.UnitName;
        }
    }
}
