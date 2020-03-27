// <copyright file="AnalysisFunctionSimpleTextField.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Implementation of analysis function simple text class
    /// </summary>
    public class AnalysisFunctionSimpleTextField : AnalysisFunctionSimpleField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionSimpleTextField"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="analysis">Analysis</param>
        public AnalysisFunctionSimpleTextField(AnalysisSourceField field, Analysis analysis)
            : base(field, analysis)
        {
        }

        /// <inheritdoc/>
        public override bool ReturnsObject => true;

        /// <inheritdoc/>
        public override bool ReturnsText => true;

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ObjectResultForResultRow(ICrmDataSourceRow row) => new AnalysisValueIntermediateResult(this.TextResultForResultRow(row));

        /// <inheritdoc/>
        public override string TextResultForResultRow(ICrmDataSourceRow row) => row.ValueAtIndex(this.SourceField.QueryResultFieldIndex);
    }
}
