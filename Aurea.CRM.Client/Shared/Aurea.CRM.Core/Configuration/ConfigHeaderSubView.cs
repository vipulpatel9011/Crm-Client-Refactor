// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigHeaderSubView.cs" company="Aurea Software Gmbh">
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
//   Defines the header subview
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Defines the header subview
    /// </summary>
    public class UPConfigHeaderSubView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigHeaderSubView"/> class.
        /// </summary>
        /// <param name="defarray">
        /// The defarray.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public UPConfigHeaderSubView(List<object> defarray, string name)
        {
            this.Label = (string)defarray[0];
            var viewReferenceDef = (defarray[1] as JArray)?.ToObject<List<object>>();
            this.ViewReference = viewReferenceDef != null ? new ViewReference(viewReferenceDef, $"Tab:{name}") : null;

            this.InfoAreaId = (string)defarray[2];
            this.LinkId = JObjectExtensions.ToInt(defarray[3]);
            this.Options = (string)defarray[4];
        }

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
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public string Options { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }
    }
}
