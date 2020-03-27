// <copyright file="AnalysisDataSourceCache.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using CRM;

    /// <summary>
    /// Implementation of analysis data source cache
    /// </summary>
    public class AnalysisDataSourceCache
    {
        private List<AnalysisDataSourceCacheItem> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDataSourceCache"/> class.
        /// </summary>
        public AnalysisDataSourceCache()
        {
            this.cache = new List<AnalysisDataSourceCacheItem>();
        }

        /// <summary>
        /// Adds a cache item
        /// </summary>
        /// <param name="cacheItem">Cache item</param>
        public void AddCacheItem(AnalysisDataSourceCacheItem cacheItem)
        {
            this.cache.Add(cacheItem);
        }

        /// <summary>
        /// Cache item execution context result
        /// </summary>
        /// <param name="executionContext">Execution context</param>
        /// <param name="result">Result</param>
        public void AddCacheItemWithExecutionContextResult(AnalysisExecutionContext executionContext, ICrmDataSource result)
        {
            AnalysisDataSourceCacheItem cacheItem = new AnalysisDataSourceCacheItem(result, executionContext);
            this.AddCacheItem(cacheItem);
        }

        /// <summary>
        /// Cache item execution context
        /// </summary>
        /// <param name="executionContext">Execution context</param>
        /// <returns>Cache item</returns>
        public AnalysisDataSourceCacheItem CacheItemForExecutionContext(AnalysisExecutionContext executionContext)
        {
            foreach (AnalysisDataSourceCacheItem item in this.cache)
            {
                if (item.ExecutionContext == executionContext)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
