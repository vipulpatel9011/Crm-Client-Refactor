// <copyright file="AnalysisValueFilter.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis
{
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Analysis value filter class implementation
    /// </summary>
    public class AnalysisValueFilter : AnalysisFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueFilter"/> class.
        /// </summary>
        /// <param name="analysisValue">Analysis value</param>
        /// <param name="val">Value</param>
        public AnalysisValueFilter(AnalysisValueField analysisValue, string val)
            : base(val, val)
        {
            this.AnalysisValue = analysisValue;
        }

        /// <summary>
        /// Gets analysis value
        /// </summary>
        public AnalysisValueField AnalysisValue { get; private set; }

        /// <inheritdoc />
        public override string Key => this.AnalysisValue.Key;

        /// <inheritdoc />
        public override string RawValueForRow(ICrmDataSourceRow row)
        {
            return this.AnalysisValue.RawValueForRow(row);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.AnalysisValue.Key}-{base.ToString()}";
        }
    }
}
