// <copyright file="AnalysisFunctionSumCatDistinct.cs" company="Aurea Software Gmbh">
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
    using Extensions;
    using Jint.Native;
    using Processing;
    using Utilities;

    /// <summary>
    /// Implementation of analysis function sum cat distinct
    /// </summary>
    public class AnalysisFunctionSumCatDistinct : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "SUMCATDISTINCT";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            double val;
            string key;
            object valueObject;
            JavascriptEngine engine = JavascriptEngine.Current;
            var arr = JavascriptEngine.ArrayForValue(argument);
            if (arr == null)
            {
                val = JavascriptEngine.DoubleForObject(argument);
                key = null;
            }
            else
            {
                if (arr.Count > 1)
                {
                    valueObject = arr[1];
                    if (valueObject is string)
                    {
                        key = (string)valueObject;
                    }
                    else
                    {
                        key = $"{valueObject}";
                    }

                    valueObject = arr[0];
                }
                else if (arr.Count == 1)
                {
                    valueObject = arr[0];
                    key = "const";
                }
                else
                {
                    return new AnalysisValueIntermediateResult(0);
                }

                if (valueObject is int || valueObject is string)
                {
                    val = valueObject.ToDouble();
                }
                else
                {
                    val = 0;
                }
            }

            var resultCounter = valueContext.ContextForYCategoryValue(rowContext.YCategoryKey) as AnalysisResultCounter;
            if (resultCounter == null)
            {
                resultCounter = new AnalysisResultCounter();
                valueContext.SetContextForYCategoryValue(resultCounter, rowContext.YCategoryKey);
            }

            if (key == null)
            {
                resultCounter.AddValue(val);
            }
            else
            {
                var num = resultCounter.ObjectForKey(key);
                if (num == null)
                {
                    resultCounter.AddValue(val);
                    resultCounter.SetObjectForKey(1, key);
                }
                else
                {
                    resultCounter.SetObjectForKey(num.ToInt() + 1, key);
                }
            }

            return new AnalysisValueIntermediateResultDelayed(resultCounter, rowContext);
        }
    }
}
