// <copyright file="AnalysisFunctionCountAll.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using Extensions;
    using Jint.Native;
    using Processing;
    using Utilities;

    /// <summary>
    /// Implementation of analysis function count all
    /// </summary>
    public class AnalysisFunctionCountAll : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "COUNTALL";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            var resultCounter = valueContext.ContextForYCategoryValue("all") as AnalysisResultCounter;
            if (resultCounter == null)
            {
                resultCounter = new AnalysisResultCounter();
                valueContext.SetContextForYCategoryValue(resultCounter, "all");
            }

            string strValue = JavascriptEngine.StringForValue(argument);
            if (strValue.Length > 0 && resultCounter.ObjectForKey(strValue) == null)
            {
                resultCounter.SetObjectForKey(strValue, strValue);
                resultCounter.AddValue(1);
            }

            return new AnalysisValueIntermediateResultDelayed(resultCounter, rowContext);
        }
    }
}
