// <copyright file="AnalysisProcessingValueAggregator.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis processing value aggregator
    /// </summary>
    public class AnalysisProcessingValueAggregator
    {
        private double doubleValue;
        private string stringValue;
        private int count;

        /// <summary>
        /// Gets or sets string value
        /// </summary>
        public virtual string StringValue
        {
            get
            {
                return this.stringValue;
            }

            protected set
            {
                this.stringValue = value;
            }
        }

        /// <summary>
        /// Gets or sets count
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.count;
            }

            protected set
            {
                this.count = value;
            }
        }

        /// <summary>
        /// Gets or sets double value
        /// </summary>
        public virtual double DoubleValue
        {
            get
            {
                return this.doubleValue;
            }

            protected set
            {
                this.doubleValue = value;
            }
        }

        /// <summary>
        /// Adds string value
        /// </summary>
        /// <param name="stringValue">String value</param>
        public virtual void AddStringValue(string stringValue)
        {
            this.stringValue = stringValue;
            this.count++;
        }

        /// <summary>
        /// Adds double value
        /// </summary>
        /// <param name="doubleValue">Double value</param>
        public virtual void AddDoubleValue(double doubleValue)
        {
            this.doubleValue = doubleValue;
            this.count++;
        }

        /// <summary>
        /// Creates instance
        /// </summary>
        /// <returns>Analysis processing value aggregator</returns>
        public virtual AnalysisProcessingValueAggregator CreateInstance()
        {
            return new AnalysisProcessingValueAggregator();
        }
    }
}
