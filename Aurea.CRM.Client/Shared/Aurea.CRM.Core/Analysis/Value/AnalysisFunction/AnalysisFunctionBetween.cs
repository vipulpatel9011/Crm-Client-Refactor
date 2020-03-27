// <copyright file="AnalysisFunctionBetween.cs" company="Aurea Software Gmbh">
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
    using AnalysisValueFunction;
    using Jint.Native;
    using Processing;
    using Utilities;

    /// <summary>
    /// Implementation of analysis function between
    /// </summary>
    public class AnalysisFunctionBetween : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "BETWEEN";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            JavascriptEngine engine = JavascriptEngine.Current;
            var arr = JavascriptEngine.ArrayForValue(argument);
            if (arr == null || arr.Count < 3)
            {
                return new AnalysisValueIntermediateResult(0);
            }

            double curVal = JavascriptEngine.DoubleForObject(arr[0]);
            double minVal = JavascriptEngine.DoubleForObject(arr[1]);
            double maxVal = JavascriptEngine.DoubleForObject(arr[2]);
            if (minVal <= curVal && curVal <= maxVal)
            {
                if (arr.Count > 3)
                {
                    return new AnalysisValueIntermediateResult(JavascriptEngine.DoubleForObject(arr[3]));
                }
                else
                {
                    return new AnalysisValueIntermediateResult(1);
                }
            }
            else if (arr.Count > 4)
            {
                return new AnalysisValueIntermediateResult(JavascriptEngine.DoubleForObject(arr[4]));
            }
            else
            {
                return new AnalysisValueIntermediateResult(0);
            }
        }
    }
}
