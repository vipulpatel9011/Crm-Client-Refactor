// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebConfigLayoutTab.cs" company="Aurea Software Gmbh">
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
//   Defines Web config layout tab
//   corresponds to UPConfigWebConfigLayoutTab
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines Web config layout tab
    /// corresponds to UPConfigWebConfigLayoutTab
    /// </summary>
    public class WebConfigLayoutTab
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebConfigLayoutTab"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public WebConfigLayoutTab(List<object> definition)
        {
            this.Label = (string)definition[0];

            var allfieldsdef = (List<object>)definition[1];

            this.FieldArray = new List<WebConfigLayoutField>(allfieldsdef.Count);
            foreach (var fdef in allfieldsdef)
            {
                this.FieldArray.Add(new WebConfigLayoutField((List<object>)fdef));
            }
        }

        /// <summary>
        /// Gets the field array.
        /// </summary>
        /// <value>
        /// The field array.
        /// </value>
        public List<WebConfigLayoutField> FieldArray { get; }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>
        /// The field count.
        /// </value>
        public int FieldCount => this.FieldArray?.Count ?? 0;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// web config layout field
        /// </returns>
        public WebConfigLayoutField FieldAtIndex(int index)
        {
            if (this.FieldArray != null && index < this.FieldArray.Count)
            {
                return this.FieldArray[index];
            }

            return null;
        }
    }
}
