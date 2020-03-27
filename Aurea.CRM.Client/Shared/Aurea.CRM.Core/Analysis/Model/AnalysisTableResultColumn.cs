// <copyright file="AnalysisTableResultColumn.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of analysis table result column
    /// </summary>
    public class AnalysisTableResultColumn : AnalysisResultColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisTableResultColumn"/> class.
        /// </summary>
        /// <param name="table">Analysis table</param>
        public AnalysisTableResultColumn(AnalysisTable table)
        {
            this.AnalysisTable = table;
        }

        /// <inheritdoc/>
        public override AnalysisAggregationType AggregationType => AnalysisAggregationType.GetCount();

        /// <summary>
        /// Gets analysis table
        /// </summary>
        public AnalysisTable AnalysisTable { get; private set; }

        /// <inheritdoc/>
        public override List<object> AvailableAggregationTypes => new List<object> { this.AggregationType };

        /// <inheritdoc/>
        public override string Key => this.AnalysisTable.Key;

        /// <inheritdoc/>
        public override string Label => this.AnalysisTable.Label;
    }
}
