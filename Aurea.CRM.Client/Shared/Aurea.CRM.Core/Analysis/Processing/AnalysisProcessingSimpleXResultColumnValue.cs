﻿// <copyright file="AnalysisProcessingSimpleXResultColumnValue.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Processing
{
    /// <summary>
    /// Implementation of simple x result column value
    /// </summary>
    public class AnalysisProcessingSimpleXResultColumnValue
    {
        /// <summary>
        /// Gets or sets aggregated value
        /// </summary>
        public double AggregatedValue { get; set; }

        /// <summary>
        /// Gets or sets count
        /// </summary>
        public int Count { get; set; }
    }
}
