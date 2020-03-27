// <copyright file="AnalysisDrillThruColumn.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Services.ModelControllers.Analysis
{
    using Aurea.CRM.Core.Analysis;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis drill thru column
    /// </summary>
    public class AnalysisDrillThruColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisDrillThruColumn"/> class.
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="dataSourceField">Data source field</param>
        public AnalysisDrillThruColumn(AnalysisSourceField sourceField, ICrmDataSourceField dataSourceField)
        {
            this.SourceField = sourceField;
            this.DataSourceField = dataSourceField;
        }

        /// <summary>
        /// Gets data source field
        /// </summary>
        public ICrmDataSourceField DataSourceField { get; private set; }

        /// <summary>
        /// Gets source field
        /// </summary>
        public AnalysisSourceField SourceField { get; private set; }
    }
}
