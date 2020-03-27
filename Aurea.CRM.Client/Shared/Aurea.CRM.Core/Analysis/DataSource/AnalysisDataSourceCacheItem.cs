// <copyright file="AnalysisDataSourceCacheItem.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Analysis.DataSource
{
    using CRM;

    /// <summary>
    /// Implementation of analysis data source cache item
    /// </summary>
    public class AnalysisDataSourceCacheItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDataSourceCacheItem"/> class.
        /// </summary>
        /// <param name="dataSource">Data source</param>
        /// <param name="executionContext">Execution context</param>
        public AnalysisDataSourceCacheItem(ICrmDataSource dataSource, AnalysisExecutionContext executionContext)
        {
            this.DataSource = dataSource;
            this.ExecutionContext = executionContext;
        }

        /// <summary>
        /// Gets data source
        /// </summary>
        public ICrmDataSource DataSource { get; private set; }

        /// <summary>
        /// Gets execution context
        /// </summary>
        public AnalysisExecutionContext ExecutionContext { get; private set; }
    }
}
