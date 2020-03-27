// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoILevelViewConfig.cs" company="Aurea Software Gmbh">
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
//   The UPMCoILevelViewConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System.Collections.Generic;

    /// <summary>
    /// UPMCoILevelViewConfig
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.CircleOfInfluence.UPMCoIConfigBase" />
    public class UPMCoILevelViewConfig : UPMCoIConfigBase
    {
        private readonly List<UPMCoINodeViewConfig> groupConfigs;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoILevelViewConfig"/> class.
        /// </summary>
        public UPMCoILevelViewConfig()
        {
            this.groupConfigs = new List<UPMCoINodeViewConfig>();
        }

        /// <summary>
        /// Configurations at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoINodeViewConfig ConfigAtIndex(int index)
        {
            while (index >= this.groupConfigs.Count)
            {
                this.groupConfigs.Add(new UPMCoINodeViewConfig());
            }

            return this.groupConfigs[index];
        }

        /// <summary>
        /// Repeateds the index of the configuration at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoINodeViewConfig RepeatedConfigAtIndex(int index)
        {
            int repeatableIndex = this.groupConfigs.Count == 0 ? 0 : index % this.groupConfigs.Count;
            return this.ConfigAtIndex(repeatableIndex);
        }

        /// <summary>
        /// Adds the configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void AddConfig(UPMCoINodeViewConfig config)
        {
            this.groupConfigs.Add(config);
        }
    }
}
