// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigHeader.cs" company="Aurea Software Gmbh">
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
//   Header configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Header configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigHeader : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigHeader"/> class.
        /// </summary>
        /// <param name="defarray">
        /// The defarray.
        /// </param>
        public UPConfigHeader(List<object> defarray)
        {
            this.UnitName = defarray[0] as string;
            this.InfoAreaId = (string)defarray[1];
            this.ImageName = (string)defarray[2];
            this.Label = (string)defarray[3];
            this.Flags = JObjectExtensions.ToInt(defarray[4]);
            this.ButtonNames = (defarray[5] as JArray)?.ToObject<List<string>>();

            var subviewarray = (defarray[6] as JArray)?.ToObject<List<object>>();
            if (subviewarray == null)
            {
                return;
            }

            this.SubViews = new List<UPConfigHeaderSubView>(subviewarray.Count);
            var subViewCount = 0;

            foreach (JArray subViewDef in subviewarray)
            {
                var def = subViewDef?.ToObject<List<object>>();
                if (def == null)
                {
                    continue;
                }

                this.SubViews.Add(new UPConfigHeaderSubView(def, $"{this.UnitName}:{subViewCount++}"));
            }
        }

        /// <summary>
        /// Gets the button names.
        /// </summary>
        /// <value>
        /// The button names.
        /// </value>
        public List<string> ButtonNames { get; }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>
        /// The flags.
        /// </value>
        public int Flags { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

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
        /// Gets the number of buttons.
        /// </summary>
        /// <value>
        /// The number of buttons.
        /// </value>
        public int NumberOfButtons => this.ButtonNames?.Count ?? 0;

        /// <summary>
        /// Gets the number of sub views.
        /// </summary>
        /// <value>
        /// The number of sub views.
        /// </value>
        public int NumberOfSubViews => this.SubViews?.Count ?? 0;

        /// <summary>
        /// Gets the sub views.
        /// </summary>
        /// <value>
        /// The sub views.
        /// </value>
        public List<UPConfigHeaderSubView> SubViews { get; }

        /// <summary>
        /// Buttons at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigButton"/>.
        /// </returns>
        public UPConfigButton ButtonAtIndex(int index)
        {
            var buttonName = this.ButtonNameAtIndex(index);
            return !string.IsNullOrEmpty(buttonName)
                       ? ConfigurationUnitStore.DefaultStore.ButtonByName(buttonName)
                       : null;
        }

        /// <summary>
        /// Buttons the index of the name at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ButtonNameAtIndex(int index)
            => this.ButtonNames == null || this.ButtonNames.Count <= index ? null : this.ButtonNames[index];

        /// <summary>
        /// Subs the index of the view at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigHeaderSubView"/>.
        /// </returns>
        public UPConfigHeaderSubView SubViewAtIndex(int index)
            => this.SubViews == null || this.SubViews.Count <= index ? null : this.SubViews[index];
    }
}
