// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTimeLine.cs" company="Aurea Software Gmbh">
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
//   Time line related configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// time line related configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class ConfigTimeline : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigTimeline"/> class.
        /// </summary>
        /// <param name="defArray">
        /// The definition array.
        /// </param>
        public ConfigTimeline(List<object> defArray)
        {
            this.UnitName = defArray[0] as string;
            var infoAreaDefArray = (defArray[4] as JArray)?.ToObject<List<object>>();
            if (infoAreaDefArray == null)
            {
                return;
            }

            var infoAreaArray = new List<ConfigTimelineInfoArea>(infoAreaDefArray.Count);
            foreach (JArray infoAreaDef in infoAreaDefArray)
            {
                if (infoAreaDef == null)
                {
                    continue;
                }

                var infoArea = new ConfigTimelineInfoArea(infoAreaDef.ToObject<List<object>>());
                infoAreaArray.Add(infoArea);
            }

            this.InfoAreas = infoAreaArray;
        }

        /// <summary>
        /// Gets the information areas.
        /// </summary>
        /// <value>
        /// The information areas.
        /// </value>
        public List<ConfigTimelineInfoArea> InfoAreas { get; private set; }
    }
}
