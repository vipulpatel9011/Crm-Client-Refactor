// <copyright file="AnalysisFunctionWeightConv.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis function weight converter
    /// </summary>
    public class AnalysisFunctionWeightConv : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "WEIGHTCONV";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            double argumentValue;
            double weight = 0;
            bool defaultWeight = true;
            JavascriptEngine engine = JavascriptEngine.Current;
            if (JavascriptEngine.IsObject(argument))
            {
                var arr = JavascriptEngine.ArrayForValue(argument);
                if (arr.Count > 0)
                {
                    object argobj = arr[0];
                    argumentValue = argobj.ToDouble();
                }
                else
                {
                    argumentValue = 0;
                }

                if (arr.Count > 1)
                {
                    object argobj = arr[1];
                    weight = argobj.ToDouble();
                    defaultWeight = false;
                }
            }
            else
            {
                argumentValue = JavascriptEngine.DoubleForValue(argument);
            }

            if (defaultWeight)
            {
                if (rowContext.ProcessingContext.Analysis.WeightField != null)
                {
                    string weightStringValue = rowContext.Row.RawValueAtIndex(rowContext.ProcessingContext.Analysis.WeightField.QueryResultFieldIndex);
                    if (weightStringValue?.Length > 0)
                    {
                        weight = weightStringValue.ToDouble();
                    }
                    else
                    {
                        weight = 1;
                    }
                }
                else
                {
                    weight = 1;
                }
            }

            return new AnalysisValueIntermediateResult(argumentValue * weight);
        }
    }
}
