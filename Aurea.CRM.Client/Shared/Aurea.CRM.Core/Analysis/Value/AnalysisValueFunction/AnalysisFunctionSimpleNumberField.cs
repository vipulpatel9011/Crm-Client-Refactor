// <copyright file="AnalysisFunctionSimpleNumberField.cs" company="Aurea Software Gmbh">
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
    using CRM;
    using Extensions;

    /// <summary>
    /// Implementation of analysis function simple number field class
    /// </summary>
    public class AnalysisFunctionSimpleNumberField : AnalysisFunctionSimpleField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionSimpleNumberField"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="analysis">Analysis</param>
        public AnalysisFunctionSimpleNumberField(AnalysisSourceField field, Analysis analysis)
            : base(field, analysis)
        {
        }

        /// <inheritdoc/>
        public override bool ReturnsNumber => true;

        /// <inheritdoc/>
        public override double NumberResultForResultRow(ICrmDataSourceRow row) => row.RawValueAtIndex(this.SourceField.QueryResultFieldIndex).ToDouble();

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ObjectResultForResultRow(ICrmDataSourceRow row)
        {
            return new AnalysisValueIntermediateResult(this.NumberResultForResultRow(row));
        }
    }
}
