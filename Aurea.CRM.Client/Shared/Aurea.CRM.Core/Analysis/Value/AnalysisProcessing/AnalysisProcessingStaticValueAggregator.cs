// <copyright file="AnalysisProcessingStaticValueAggregator.cs" company="Aurea Software Gmbh">
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
    /// <summary>
    /// Implementation of analysis processing STATIC value aggregator
    /// </summary>
    public class AnalysisProcessingStaticValueAggregator : AnalysisProcessingValueAggregator
    {
        private bool notFirst;

        /// <inheritdoc/>
        public override void AddDoubleValue(double doubleValue)
        {
            if (!this.notFirst)
            {
                this.notFirst = true;
                this.DoubleValue = doubleValue;
            }

            this.Count++;
        }

        /// <inheritdoc/>
        public override AnalysisProcessingValueAggregator CreateInstance()
        {
            return new AnalysisProcessingStaticValueAggregator();
        }
    }
}
