// <copyright file="AnalysisFunctionSum.cs" company="Aurea Software Gmbh">
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
    using Jint.Native;
    using Processing;
    using Utilities;

    /// <summary>
    /// Implementation of analysis function sum
    /// </summary>
    public class AnalysisFunctionSum : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "SUM";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            var resultCounter = valueContext.ContextForYCategoryValue("all") as AnalysisResultCounter;
            if (resultCounter == null)
            {
                resultCounter = new AnalysisResultCounter();
                valueContext.SetContextForYCategoryValue(resultCounter, "all");
            }

            resultCounter.AddValue(JavascriptEngine.DoubleForValue(argument));
            return new AnalysisValueIntermediateResultDelayed(resultCounter, rowContext);
        }
    }
}
