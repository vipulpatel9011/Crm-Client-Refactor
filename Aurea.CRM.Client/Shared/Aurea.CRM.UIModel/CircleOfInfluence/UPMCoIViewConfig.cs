// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoIViewConfig.cs" company="Aurea Software Gmbh">
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
//   The UPMCoIViewConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System.Collections.Generic;

    /// <summary>
    /// UPMCoIViewConfig
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.CircleOfInfluence.UPMCoIConfigBase" />
    public class UPMCoIViewConfig : UPMCoIConfigBase
    {
        private List<UPMCoILevelViewConfig> levelConfigs;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoIViewConfig"/> class.
        /// </summary>
        public UPMCoIViewConfig()
        {
            this.levelConfigs = new List<UPMCoILevelViewConfig>();
        }

        /// <summary>
        /// Configurations at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoILevelViewConfig ConfigAtIndex(int index)
        {
            while (index >= this.levelConfigs.Count)
            {
                this.levelConfigs.Add(new UPMCoILevelViewConfig());
            }

            return this.levelConfigs[index];
        }

        /// <summary>
        /// Adds the configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void AddConfig(UPMCoILevelViewConfig config)
        {
            this.levelConfigs.Add(config);
        }
    }
}
