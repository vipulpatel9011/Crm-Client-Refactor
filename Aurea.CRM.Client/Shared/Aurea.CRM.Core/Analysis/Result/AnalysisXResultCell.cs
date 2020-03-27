// <copyright file="AnalysisXResultCell.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Result
{
    /// <summary>
    /// Implementation of x analysis result cell
    /// </summary>
    public class AnalysisXResultCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisXResultCell"/> class.
        /// </summary>
        /// <param name="value">Value.AnalysisValueFunction</param>
        /// <param name="stringValue">String value</param>
        public AnalysisXResultCell(double value, string stringValue)
        {
            this.Value = value;
            this.StringValue = stringValue;
            this.RawValue = this.Value.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisXResultCell"/> class.
        /// </summary>
        /// <param name="stringValue">String value</param>
        public AnalysisXResultCell(string stringValue)
        {
            this.Value = 0;
            this.StringValue = stringValue;
            this.RawValue = stringValue;
        }

        /// <summary>
        /// Gets raw value
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// Gets string value
        /// </summary>
        public string StringValue { get; private set; }

        /// <summary>
        /// Gets value
        /// </summary>
        public double Value { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.StringValue;
        }
    }
}
