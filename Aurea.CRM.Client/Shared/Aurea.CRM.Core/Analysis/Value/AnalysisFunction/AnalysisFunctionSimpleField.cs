// <copyright file="AnalysisFunctionSimpleField.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Implementation of analysis function simple field class
    /// </summary>
    public class AnalysisFunctionSimpleField : AnalysisValueFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionSimpleField"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="analysis">Analysis</param>
        public AnalysisFunctionSimpleField(AnalysisSourceField field, Analysis analysis)
            : base(analysis)
        {
            this.SourceField = field;
        }

        /// <summary>
        /// Gets source field
        /// </summary>
        public AnalysisSourceField SourceField { get; private set; }

        /// <inheritdoc/>
        public override List<object> SignificantQueryResultTableIndices()
        {
            return new List<object> { this.SourceField.AnalysisTable.QueryTableIndex };
        }
    }
}
