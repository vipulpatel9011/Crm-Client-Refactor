// <copyright file="AnalysisFunctionCurrConv.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis function curr conv
    /// </summary>
    public class AnalysisFunctionCurrConv : AnalysisFunction
    {
        /// <inheritdoc/>
        public override string Name => "CURRCONV";

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            double argumentValue = 0;
            int currencyCode = 0;
            bool defaultCurrencyField = true;
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
                    currencyCode = argobj.ToInt();
                    defaultCurrencyField = false;
                }
            }
            else
            {
                argumentValue = JavascriptEngine.DoubleForValue(argument);
            }

            if (defaultCurrencyField)
            {
                if (rowContext.ProcessingContext.Analysis.CurrencyField != null)
                {
                    var currencyStringValue = rowContext.Row.RawValueAtIndex(rowContext.ProcessingContext.Analysis.CurrencyField.QueryResultFieldIndex);
                    if (currencyStringValue?.Length > 0)
                    {
                        currencyCode = currencyStringValue.ToInt();
                    }
                    else
                    {
                        currencyCode = 0;
                    }
                }
                else
                {
                    currencyCode = 0;
                }
            }

            int targetCode = rowContext.ProcessingContext.Analysis.CurrentSettings.CurrencyCode;
            if (currencyCode == 0 || targetCode == 0 || currencyCode == targetCode)
            {
                return new AnalysisValueIntermediateResult(argumentValue);
            }

            var exchangeRate = rowContext.ProcessingContext.Analysis.CurrencyConversion.ExchangeRateFromCodeToCode(currencyCode, targetCode);
            if (exchangeRate != 0)
            {
                argumentValue /= exchangeRate;
            }

            return new AnalysisValueIntermediateResult(argumentValue);
        }
    }
}
