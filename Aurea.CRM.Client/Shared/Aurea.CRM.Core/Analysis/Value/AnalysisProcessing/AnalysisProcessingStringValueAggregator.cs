// <copyright file="AnalysisProcessingStringValueAggregator.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value.AnalysisProcessing
{
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of analysis processing STRING value aggregator
    /// </summary>
    public class AnalysisProcessingStringValueAggregator : AnalysisProcessingValueAggregator
    {
        private List<string> parts;
        private AnalysisValueOptions valueOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisProcessingStringValueAggregator"/> class.
        /// </summary>
        /// <param name="valueOptions">Value.AnalysisValueFunction options</param>
        public AnalysisProcessingStringValueAggregator(AnalysisValueOptions valueOptions)
        {
            this.valueOptions = valueOptions;
            this.Delimiter = !string.IsNullOrEmpty(this.valueOptions.Concatenate) ? valueOptions.Concatenate : ",";
        }

        /// <summary>
        /// Gets delimiter
        /// </summary>
        public string Delimiter { get; private set; }

        /// <inheritdoc/>
        public override string StringValue => string.Join(this.Delimiter, this.parts);

        /// <inheritdoc/>
        public override void AddStringValue(string addValue)
        {
            if (this.parts == null)
            {
                this.parts = new List<string> { addValue };
            }
            else
            {
                this.parts.Add(addValue);
            }
        }

        /// <inheritdoc/>
        public override AnalysisProcessingValueAggregator CreateInstance()
        {
            return new AnalysisProcessingStringValueAggregator(this.valueOptions);
        }
    }
}
