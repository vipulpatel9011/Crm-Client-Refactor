// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigButton.cs" company="Aurea Software Gmbh">
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
//   Button flags
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Button flags
    /// </summary>
    [Flags]
    public enum ConfigButtonFlags
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The hidden.
        /// </summary>
        Hidden = 1,

        /// <summary>
        /// The disabled.
        /// </summary>
        Disabled = 2,

        /// <summary>
        /// The owner draw.
        /// </summary>
        OwnerDraw = 4,

        /// <summary>
        /// The hide text.
        /// </summary>
        HideText = 8,

        /// <summary>
        /// The hide in quick view.
        /// </summary>
        HideInQuickView = 16,
    }

    /// <summary>
    /// Configurations related to buttons
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigButton : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigButton"/> class.
        /// </summary>
        /// <param name="defarray">
        /// The defarray.
        /// </param>
        public UPConfigButton(List<object> defarray)
        {
            if (defarray == null || defarray.Count < 4)
            {
                return;
            }

            this.UnitName = (string)defarray[0];
            this.Label = (string)defarray[1];
            this.ImageName = (string)defarray[2];

            var viewReferenceDef = (defarray[3] as JArray)?.ToObject<List<object>>();
            this.ViewReference = viewReferenceDef != null
                                     ? new ViewReference(viewReferenceDef, $"Button:{this.UnitName}")
                                     : null;

            this.Flags = defarray.Count < 5 ? 0 : (ConfigButtonFlags)JObjectExtensions.ToInt(defarray[4]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigButton"/> class.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="imageName">
        /// Name of the image.
        /// </param>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        public UPConfigButton(string label, string imageName, ViewReference viewReference)
        {
            this.Label = label;
            this.ImageName = imageName;
            this.ViewReference = viewReference;
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public ConfigButtonFlags Flags { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public bool IsHidden => this.Flags.HasFlag(ConfigButtonFlags.Hidden);

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; }
    }
}
