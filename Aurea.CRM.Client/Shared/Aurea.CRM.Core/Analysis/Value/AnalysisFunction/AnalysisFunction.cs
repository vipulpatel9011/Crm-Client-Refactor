// <copyright file="AnalysisFunction.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Base implementation of analysis function
    /// </summary>
    public class AnalysisFunction
    {
        private static Dictionary<string, object> functionDictionary = null;

        /// <summary>
        /// Gets name
        /// </summary>
        public virtual string Name => null;

        private static Dictionary<string, object> FunctionDictionary
        {
            get
            {
                if (functionDictionary == null)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    AnalysisFunction func = new AnalysisFunctionCount();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionCountX();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionCountAll();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionSum();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionSumCat();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionSumCatX();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionMin();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionMax();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionBetween();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionSumCatDistinct();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionStatic();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionCurrConv();
                    dict.SetObjectForKey(func, func.Name);
                    func = new AnalysisFunctionWeightConv();
                    dict.SetObjectForKey(func, func.Name);
                    functionDictionary = dict;
                }

                return functionDictionary;
            }
        }

        /// <summary>
        /// Analysis function with name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Returns Analysis function</returns>
        public static AnalysisFunction FunctionWithName(string name)
        {
            return FunctionDictionary[name] as AnalysisFunction;
        }

        /// <summary>
        /// Result with argument row context value context
        /// </summary>
        /// <param name="argument">Argument</param>
        /// <param name="rowContext">Row context</param>
        /// <param name="valueContext">Value.AnalysisValueFunction context</param>
        /// <returns>Analysis value intermediate result</returns>
        public virtual AnalysisValueIntermediateResult ResultWithArgumentRowContextValueContext(JsValue argument, AnalysisProcessingQueryResultRowExecutionContext rowContext, AnalysisProcessingValueExecutionContext valueContext)
        {
            return new AnalysisValueIntermediateResult(0);
        }
    }
}
