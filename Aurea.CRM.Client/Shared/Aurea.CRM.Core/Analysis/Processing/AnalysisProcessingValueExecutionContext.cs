// <copyright file="AnalysisProcessingValueExecutionContext.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Processing
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Implementation value execution context
    /// </summary>
    public class AnalysisProcessingValueExecutionContext
    {
        private List<object> childExecutionContexts;
        private Dictionary<string, object> xCategoryValueContexts;
        private Dictionary<string, object> yCategoryValueContexts;

        /// <summary>
        /// Child execution context at index
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns>Returns value execution context</returns>
        public AnalysisProcessingValueExecutionContext ChildExecutionContextAtIndex(int i)
        {
            if (this.childExecutionContexts == null)
            {
                this.childExecutionContexts = new List<object>();
            }

            if (i >= this.childExecutionContexts.Count)
            {
                for (var j = this.childExecutionContexts.Count; j < i; j++)
                {
                    this.childExecutionContexts.Add(null);
                }

                AnalysisProcessingValueExecutionContext ec = new AnalysisProcessingValueExecutionContext();
                this.childExecutionContexts.Add(ec);
                return ec;
            }

            var obj = this.childExecutionContexts[i];
            if (obj == null)
            {
                AnalysisProcessingValueExecutionContext ec = new AnalysisProcessingValueExecutionContext();
                this.childExecutionContexts[i] = ec;
                return ec;
            }

            return (AnalysisProcessingValueExecutionContext)obj;
        }

        /// <summary>
        /// Context for y category value
        /// </summary>
        /// <param name="yCategoryValue">Y category value</param>
        /// <returns>Return context for y category value</returns>
        public object ContextForYCategoryValue(string yCategoryValue)
        {
            if (yCategoryValue == null)
            {
                yCategoryValue = string.Empty;
            }

            return this.yCategoryValueContexts.ValueOrDefault(yCategoryValue);
        }

        /// <summary>
        /// Context for y and x category value
        /// </summary>
        /// <param name="yCategoryValue">Y category value</param>
        /// <param name="xCategoryValue">X category value</param>
        /// <returns>Return context for y and x category value</returns>
        public object ContextForYCategoryValueXCategoryValue(string yCategoryValue, string xCategoryValue)
        {
            if (yCategoryValue == null)
            {
                yCategoryValue = string.Empty;
            }

            var yDict = this.xCategoryValueContexts.ValueOrDefault(yCategoryValue) as Dictionary<string, object>;
            if (yDict == null)
            {
                return null;
            }

            if (xCategoryValue == null)
            {
                xCategoryValue = string.Empty;
            }

            return yDict.ValueOrDefault(xCategoryValue);
        }

        /// <summary>
        /// Sets context for y category value
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="yCategoryValue">Y category value</param>
        public void SetContextForYCategoryValue(object context, string yCategoryValue)
        {
            if (yCategoryValue == null)
            {
                yCategoryValue = string.Empty;
            }

            if (this.yCategoryValueContexts == null)
            {
                this.yCategoryValueContexts = new Dictionary<string, object> { { yCategoryValue, context } };
            }
            else
            {
                this.yCategoryValueContexts.SetObjectForKey(context, yCategoryValue);
            }
        }

        /// <summary>
        /// Sets context for y and x category value
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="yCategoryValue">Y category value</param>
        /// <param name="xCategoryValue">X category value</param>
        public void SetContextForYCategoryValueXCategoryValue(object context, string yCategoryValue, string xCategoryValue)
        {
            if (yCategoryValue == null)
            {
                yCategoryValue = string.Empty;
            }

            if (xCategoryValue == null)
            {
                xCategoryValue = string.Empty;
            }

            var yDict = this.xCategoryValueContexts.ValueOrDefault(yCategoryValue) as Dictionary<string, object>;
            if (yDict == null)
            {
                yDict = new Dictionary<string, object> { { xCategoryValue, context } };
                if (this.xCategoryValueContexts == null)
                {
                    this.xCategoryValueContexts = new Dictionary<string, object> { { yCategoryValue, yDict } };
                }
                else
                {
                    this.xCategoryValueContexts.SetObjectForKey(yDict, yCategoryValue);
                }
            }
            else
            {
                yDict.SetObjectForKey(context, xCategoryValue);
            }
        }
    }
}
