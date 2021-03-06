﻿// <copyright file="AnalysisFunctionCountX.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis function count X
    /// </summary>
    public class AnalysisFunctionCountX : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "COUNTX";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            AnalysisResultCounter resultCounter;
            if (!string.IsNullOrEmpty(rowContext.XCategoryKey))
            {
                resultCounter = valueContext.ContextForYCategoryValueXCategoryValue(rowContext.YCategoryKey, rowContext.XCategoryKey) as AnalysisResultCounter;
            }
            else
            {
                resultCounter = valueContext.ContextForYCategoryValue(rowContext.YCategoryKey) as AnalysisResultCounter;
            }

            if (resultCounter == null)
            {
                resultCounter = new AnalysisResultCounter();
                if (!string.IsNullOrEmpty(rowContext.XCategoryKey))
                {
                    valueContext.SetContextForYCategoryValueXCategoryValue(resultCounter, rowContext.YCategoryKey, rowContext.XCategoryKey);
                }
                else
                {
                    valueContext.SetContextForYCategoryValue(resultCounter, rowContext.YCategoryKey);
                }
            }

            string strValue = JavascriptEngine.StringForValue(argument);
            if (!string.IsNullOrEmpty(strValue) && resultCounter.ObjectForKey(strValue) == null)
            {
                resultCounter.SetObjectForKey(strValue, strValue);
                resultCounter.AddValue(1);
            }

            return new AnalysisValueIntermediateResultDelayed(resultCounter, rowContext);
        }
    }
}
