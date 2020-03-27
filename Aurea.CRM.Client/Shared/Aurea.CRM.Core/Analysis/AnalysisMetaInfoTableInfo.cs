// <copyright file="AnalysisMetaInfoTableInfo.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis metainfo table info
    /// </summary>
    public class AnalysisMetaInfoTableInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisMetaInfoTableInfo"/> class.
        /// </summary>
        /// <param name="dataSourceTable">Data source table</param>
        /// <param name="tableIndex">Table index</param>
        /// <param name="firstFieldIndex">First field index</param>
        /// <param name="occurrence">Occurrence</param>
        public AnalysisMetaInfoTableInfo(ICrmDataSourceTable dataSourceTable, int tableIndex, int firstFieldIndex, int occurrence)
        {
            this.DataSourceTable = dataSourceTable;
            this.TableIndex = tableIndex;
            this.FirstFieldIndex = firstFieldIndex;
            this.Occurrence = occurrence;
        }

        /// <summary>
        /// Gets data source table
        /// </summary>
        public ICrmDataSourceTable DataSourceTable { get; private set; }

        /// <summary>
        /// Gets first field index
        /// </summary>
        public int FirstFieldIndex { get; private set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key => $"{this.DataSourceTable.InfoAreaId}#{this.Occurrence}";

        /// <summary>
        /// Gets occurrence
        /// </summary>
        public int Occurrence { get; private set; }

        /// <summary>
        /// Gets table index
        /// </summary>
        public int TableIndex { get; private set; }
    }
}
