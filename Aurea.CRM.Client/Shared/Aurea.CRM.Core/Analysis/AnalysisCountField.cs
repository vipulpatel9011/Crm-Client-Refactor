// <copyright file="AnalysisCountField.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis
{
    using Configuration;

    /// <summary>
    /// Implementation of analysis count field
    /// </summary>
    public class AnalysisCountField : AnalysisField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisCountField"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="table">Table</param>
        /// <param name="queryResultTableIndex">Query result table index</param>
        public AnalysisCountField(Analysis analysis, UPConfigAnalysisTable table, int queryResultTableIndex)
            : base(analysis, table.Key)
        {
            this.ConfigTable = table;
            this.QueryResultTableIndex = queryResultTableIndex;
        }

        /// <summary>
        /// Gets config table
        /// </summary>
        public UPConfigAnalysisTable ConfigTable { get; private set; }

        /// <summary>
        /// Gets query result table index
        /// </summary>
        public int QueryResultTableIndex { get; private set; }
    }
}
