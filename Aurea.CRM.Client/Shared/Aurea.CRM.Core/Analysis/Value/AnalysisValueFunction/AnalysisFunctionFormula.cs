// <copyright file="AnalysisFunctionFormula.cs" company="Aurea Software Gmbh">
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
    using System.Linq;
    using CRM;
    using Extensions;
    using Jint.Native;
    using Processing;
    using Utilities;

    /// <summary>
    /// Implementation of analysis function formula class
    /// </summary>
    public class AnalysisFunctionFormula : AnalysisValueFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisFunctionFormula"/> class.
        /// </summary>
        /// <param name="formula">Formula</param>
        /// <param name="arguments">Arguments</param>
        /// <param name="analysis">Analysis</param>
        public AnalysisFunctionFormula(string formula, List<object> arguments, Analysis analysis)
            : base(analysis)
        {
            this.FormulaFunction = formula;
            this.JavascriptFunction = JavascriptEngine.Current.FunctionForScript(this.FormulaFunction);
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets argumments
        /// </summary>
        public List<object> Arguments { get; private set; }

        /// <summary>
        /// Gets formula function
        /// </summary>
        public string FormulaFunction { get; private set; }

        /// <summary>
        /// Gets javascript function
        /// </summary>
        public JsValue JavascriptFunction { get; private set; }

        /// <inheritdoc/>
        public override bool ReturnsNumber => false;

        /// <inheritdoc/>
        public override bool ReturnsObject => true;

        /// <inheritdoc/>
        public override bool ReturnsText => false;

        /// <summary>
        /// Object result for arguments
        /// </summary>
        /// <param name="arguments">Arguments</param>
        /// <returns>Returns object result for argument</returns>
        public AnalysisValueIntermediateResult ObjectResultForArguments(List<object> arguments)
        {
            var arr = new List<JsValue>();
            foreach (AnalysisValueIntermediateResult res in arguments)
            {
                if (res.IsTextResult)
                {
                    arr.Add(new JsValue(res.TextResult));
                }
                else
                {
                    arr.Add(new JsValue(res.NumberResult));
                }
            }

            var resultValue = this.JavascriptFunction.Invoke(arr.ToArray());
            if (resultValue.IsString())
            {
                return new AnalysisValueIntermediateResult(resultValue.ToString());
            }
            else if (resultValue.IsNumber())
            {
                return new AnalysisValueIntermediateResult(resultValue.ToDouble());
            }
            else
            {
                return new AnalysisValueIntermediateResult(resultValue);
            }
        }

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ObjectResultForResultRow(ICrmDataSourceRow row)
        {
            var objectResultArray = new List<JsValue>();
            foreach (AnalysisValueFunction func in this.Arguments)
            {
                AnalysisValueIntermediateResult res = func.ObjectResultForResultRow(row);
                if (res == null || !res.Complete)
                {
                    objectResultArray.Add(new JsValue(0));
                }
                else
                {
                    objectResultArray.Add(res.JavascriptResult);
                }
            }

            JsValue resultValue = this.JavascriptFunction.Invoke(objectResultArray.ToArray());
            if (resultValue.IsString())
            {
                return new AnalysisValueIntermediateResult(resultValue.ToString());
            }
            else if (resultValue.IsNumber())
            {
                return new AnalysisValueIntermediateResult(resultValue.ToDouble());
            }
            else
            {
                return new AnalysisValueIntermediateResult(resultValue);
            }
        }

        /// <inheritdoc/>
        public override AnalysisValueIntermediateResult ResultForQueryRowVariableContext(AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueExecutionContext)
        {
            bool complete = true;
            var objectResultArray = new List<object>();
            int i = 0;
            foreach (AnalysisValueFunction func in this.Arguments)
            {
                AnalysisValueIntermediateResult res = func.ResultForQueryRowVariableContext(rowContext, valueExecutionContext.ChildExecutionContextAtIndex(i++));
                if (!res.Complete)
                {
                    complete = false;
                }

                if (res == null)
                {
                    objectResultArray.Add(new AnalysisValueIntermediateResult(0));
                }
                else
                {
                    objectResultArray.Add(res);
                }
            }

            if (!complete)
            {
                return new AnalysisValueIntermediateResultWithFormula(this, objectResultArray, rowContext, valueExecutionContext);
            }

            return this.ObjectResultForArguments(objectResultArray);
        }

        /// <inheritdoc/>
        public override List<object> SignificantQueryResultTableIndices()
        {
            var sigDic = new Dictionary<object, object>();
            foreach (AnalysisValueFunction func in this.Arguments)
            {
                foreach (var num in func.SignificantQueryResultTableIndices())
                {
                    sigDic.SetObjectForKey(num, num);
                }
            }

            return sigDic.Values.ToList();
        }
    }
}
