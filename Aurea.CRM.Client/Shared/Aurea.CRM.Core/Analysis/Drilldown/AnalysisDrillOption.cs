// <copyright file="AnalysisDrillOption.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Analysis.Drilldown
{
    /// <summary>
    /// Implementation of analysis drill option class
    /// </summary>
    public class AnalysisDrillOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDrillOption"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        public AnalysisDrillOption(Analysis analysis)
        {
            this.Analysis = analysis;
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public virtual string Key => null;

        /// <summary>
        /// Gets label
        /// </summary>
        public virtual string Label => null;
    }
}
