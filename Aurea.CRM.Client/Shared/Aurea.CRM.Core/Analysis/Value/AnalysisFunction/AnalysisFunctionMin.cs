// <copyright file="AnalysisFunctionMin.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis function min
    /// </summary>
    public class AnalysisFunctionMin : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "MIN";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            JavascriptEngine engine = JavascriptEngine.Current;
            double minimumValue = 0;
            bool first = true;
            var arr = JavascriptEngine.ArrayForValue(argument);
            if (arr == null)
            {
                return new AnalysisValueIntermediateResult(0);
            }

            foreach (object v in arr)
            {
                double curVal = JavascriptEngine.DoubleForObject(v);
                if (first)
                {
                    minimumValue = curVal;
                    first = false;
                }
                else if (curVal < minimumValue)
                {
                    minimumValue = curVal;
                }
            }

            return new AnalysisValueIntermediateResult(minimumValue);
        }
    }
}
