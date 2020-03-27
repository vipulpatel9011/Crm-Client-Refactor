// <copyright file="AnalysisRowDetails.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Result
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis row details
    /// </summary>
    public class AnalysisRowDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisRowDetails"/> class.
        /// </summary>
        /// <param name="resultRow">Result row</param>
        /// <param name="dataSourceRow">Data source row</param>
        public AnalysisRowDetails(AnalysisRow resultRow, ICrmDataSourceRow dataSourceRow)
        {
            this.DataSourceRow = dataSourceRow;
            this.AnalysisResultRow = resultRow;
        }

        /// <summary>
        /// Gets analysis result row
        /// </summary>
        public AnalysisRow AnalysisResultRow { get; private set; }

        /// <summary>
        /// Gets data source row
        /// </summary>
        public ICrmDataSourceRow DataSourceRow { get; private set; }

        /// <summary>
        /// Gets string values
        /// </summary>
        public List<string> StringValues => this.AnalysisResultRow.StringValuesForResultRow(this.DataSourceRow);

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.StringValues}";
        }
    }
}
