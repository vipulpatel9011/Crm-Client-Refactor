// <copyright file="AnalysisFunctionFormulaParseResult.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value.AnalysisFunction
{
    using System;
    using AnalysisValueFunction;

    /// <summary>
    /// Implementation of analysis function formula parse result
    /// </summary>
    public class AnalysisFunctionFormulaParseResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionFormulaParseResult"/> class.
        /// </summary>
        /// <param name="valueFunction">Value.AnalysisValueFunction function</param>
        public AnalysisFunctionFormulaParseResult(AnalysisValueFunction valueFunction)
        {
            this.Result = valueFunction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionFormulaParseResult"/> class.
        /// </summary>
        /// <param name="errorText">Error text</param>
        public AnalysisFunctionFormulaParseResult(string errorText)
            : this(new Exception(errorText))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionFormulaParseResult"/> class.
        /// </summary>
        /// <param name="error">Error</param>
        public AnalysisFunctionFormulaParseResult(Exception error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets error
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets result
        /// </summary>
        public AnalysisValueFunction Result { get; private set; }
    }
}
