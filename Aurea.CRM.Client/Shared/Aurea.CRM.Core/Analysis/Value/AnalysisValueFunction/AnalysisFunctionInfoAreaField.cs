// <copyright file="AnalysisFunctionInfoAreaField.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value.AnalysisValueFunction
{
    using System.Collections.Generic;
    using CRM;

    /// <summary>
    /// Implementation of analysis function info area field class
    /// </summary>
    public class AnalysisFunctionInfoAreaField : AnalysisValueFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionInfoAreaField"/> class.
        /// </summary>
        /// <param name="countField">Count field</param>
        /// <param name="analysis">Analysis</param>
        public AnalysisFunctionInfoAreaField(AnalysisCountField countField, Analysis analysis)
            : base(analysis)
        {
            this.CountField = countField;
        }

        /// <summary>
        /// Gets count field
        /// </summary>
        public AnalysisCountField CountField { get; private set; }

        /// <inheritdoc/>
        public override bool ReturnsText => true;

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ObjectResultForResultRow(ICrmDataSourceRow row)
        {
            return new AnalysisValueIntermediateResult(this.TextResultForResultRow(row));
        }

        /// <inheritdoc/>
        public override List<object> SignificantQueryResultTableIndices()
        {
            return new List<object> { this.CountField.QueryResultTableIndex };
        }

        /// <inheritdoc/>
        public override string TextResultForResultRow(ICrmDataSourceRow row)
        {
            if (this.CountField.QueryResultTableIndex >= 0)
            {
                return row.RecordIdentificationAtIndex(this.CountField.QueryResultTableIndex);
            }

            return string.Empty;
        }
    }
}
