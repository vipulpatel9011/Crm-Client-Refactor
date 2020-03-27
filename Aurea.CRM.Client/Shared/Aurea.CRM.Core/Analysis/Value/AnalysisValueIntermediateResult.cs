// <copyright file="AnalysisValueIntermediateResult.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value
{
    using System.Collections.Generic;
    using Extensions;
    using Jint.Native;
    using Utilities;

    /// <summary>
    /// Implementation of analysis value intermediate result
    /// </summary>
    public class AnalysisValueIntermediateResult
    {
        private JsValue javascriptResult;
        private double numberResult;
        private string textResult;
        private string xCategoryKey;
        private bool complete;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResult"/> class.
        /// </summary>
        /// <param name="textResult">Text result</param>
        public AnalysisValueIntermediateResult(string textResult)
        {
            this.textResult = textResult;
            this.IsTextResult = true;
            this.complete = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResult"/> class.
        /// </summary>
        /// <param name="result">Result</param>
        public AnalysisValueIntermediateResult(double result)
        {
            this.numberResult = double.IsNaN(result) ? 0 : result;

            this.complete = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueIntermediateResult"/> class.
        /// </summary>
        /// <param name="javascriptResult">Javascript result</param>
        public AnalysisValueIntermediateResult(JsValue javascriptResult)
        {
            this.javascriptResult = javascriptResult;
            this.complete = true;
        }

        /// <summary>
        /// Gets array result
        /// </summary>
        public virtual List<object> ArrayResult => this.javascriptResult != null ? JavascriptEngine.ArrayForValue(this.javascriptResult) : null;

        /// <summary>
        /// Gets or sets a value indicating whether this is complete
        /// </summary>
        public virtual bool Complete
        {
            get
            {
                return this.complete;
            }

            set
            {
                this.complete = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is an object
        /// </summary>
        public virtual bool IsObject => this.javascriptResult != null && JavascriptEngine.IsObject(this.javascriptResult);

        /// <summary>
        /// Gets a value indicating whether this result is text
        /// </summary>
        public virtual bool IsTextResult { get; private set; }

        /// <summary>
        /// Gets javascript result
        /// </summary>
        public virtual JsValue JavascriptResult
            => this.javascriptResult ?? (this.javascriptResult = this.IsTextResult
                                                           ? JavascriptEngine.Current.ValueForString(this.TextResult)
                                                           : JavascriptEngine.Current.ValueForDouble(this.NumberResult));

        /// <summary>
        /// Gets number result
        /// </summary>
        public virtual double NumberResult => this.IsTextResult ? this.textResult.ToDouble() : this.numberResult;

        /// <summary>
        /// Gets text result
        /// </summary>
        public virtual string TextResult => this.IsTextResult ? this.textResult : this.numberResult.ToString("0.00");

        /// <summary>
        /// Gets or sets x category key
        /// </summary>
        public virtual string XCategoryKey
        {
            get
            {
                return this.xCategoryKey;
            }

            set
            {
                this.xCategoryKey = value;
            }
        }

        /// <summary>
        /// Executes step
        /// </summary>
        /// <returns>Returns intermediate result</returns>
        public virtual AnalysisValueIntermediateResult ExecuteStep()
        {
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.complete)
            {
                return this.IsTextResult ? $"res={this.TextResult}" : $"res={this.NumberResult.ToString("0.00")}";
            }

            return "result incomplete";
        }
    }
}
