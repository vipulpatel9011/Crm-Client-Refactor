// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoArea.cs" company="Aurea Software Gmbh">
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
//   Defines the InfArea configuration details
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Defines the InfoArea configuration details
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class InfoArea : ConfigUnit
    {
        /// <summary>
        /// The plural name.
        /// </summary>
        private readonly string pluralName;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoArea"/> class.
        /// </summary>
        /// <param name="defarray">
        /// The defarray.
        /// </param>
        public InfoArea(List<object> defarray)
        {
            if (defarray == null || defarray.Count < 5)
            {
                return;
            }

            this.UnitName = (string)defarray[0];
            this.DefaultAction = (string)defarray[1];
            this.DefaultMenu = (string)defarray[2];
            this.ColorKey = (string)defarray[3];
            this.ImageName = (string)defarray[4];
            if (defarray.Count > 5)
            {
                this.SingularName = (string)defarray[5];
            }

            if (defarray.Count > 6)
            {
                this.pluralName = (string)defarray[6];
            }

            if (string.IsNullOrWhiteSpace(this.SingularName))
            {
                this.SingularName = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(this.UnitName)?.Label;
            }
        }

        /// <summary>
        /// Gets the color key.
        /// </summary>
        /// <value>
        /// The color key.
        /// </value>
        public string ColorKey { get; private set; }

        /// <summary>
        /// Gets the default action.
        /// </summary>
        /// <value>
        /// The default action.
        /// </value>
        public string DefaultAction { get; private set; }

        /// <summary>
        /// Gets the default menu.
        /// </summary>
        /// <value>
        /// The default menu.
        /// </value>
        public string DefaultMenu { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the name of the plural.
        /// </summary>
        /// <value>
        /// The name of the plural.
        /// </value>
        public string PluralName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.pluralName))
                {
                    return this.pluralName;
                }

                return this.SingularName;
            }
        }

        /// <summary>
        /// Gets the name of the singular.
        /// </summary>
        /// <value>
        /// The name of the singular.
        /// </value>
        public string SingularName { get; private set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.SingularName ?? this.UnitName;
        }
    }
}
