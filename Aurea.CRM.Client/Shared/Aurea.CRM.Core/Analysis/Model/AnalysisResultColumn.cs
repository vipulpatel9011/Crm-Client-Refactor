// <copyright file="AnalysisResultColumn.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of analysis result column
    /// </summary>
    public class AnalysisResultColumn
    {
        /// <summary>
        /// Gets aggregation type
        /// </summary>
        public virtual AnalysisAggregationType AggregationType => null;

        /// <summary>
        /// Gets aggregation types which available
        /// </summary>
        public virtual List<object> AvailableAggregationTypes => null;

        /// <summary>
        /// Gets key
        /// </summary>
        public virtual string Key => null;

        /// <summary>
        /// Gets label
        /// </summary>
        public virtual string Label => null;

        /// <summary>
        /// Gets value options
        /// </summary>
        public virtual AnalysisValueOptions ValueOptions => null;

        /// <summary>
        /// Display string from number
        /// </summary>
        /// <param name="value">Value.AnalysisValueFunction</param>
        /// <returns>Returns display string from number</returns>
        public virtual string DisplayStringFromNumber(double value)
        {
            return $"{value}";
        }

        /// <summary>
        /// Result column with aggregation type
        /// </summary>
        /// <param name="aggregationTypeString">Aggregation type string</param>
        /// <returns>Analysis result column</returns>
        public virtual AnalysisResultColumn ResultColumnWithAggregationType(string aggregationTypeString)
        {
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.AggregationType}({this.Key})";
        }
    }
}
