// <copyright file="AnalysisXColumn.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Analysis.Processing;

    /// <summary>
    /// Implementation for analysis x column
    /// </summary>
    public class AnalysisXColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisXColumn"/> class.
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="xCategoryValue">X category value</param>
        public AnalysisXColumn(AnalysisColumn column, AnalysisProcessingXCategoryValue xCategoryValue)
        {
            this.Key = xCategoryValue.Key;
            this.Label = xCategoryValue.Label;
            this.Category = xCategoryValue.Category;
            this.Column = column;
        }

        /// <summary>
        /// Gets category
        /// </summary>
        public AnalysisCategoryValue Category { get; private set; }

        /// <summary>
        /// Gets column
        /// </summary>
        public AnalysisColumn Column { get; private set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }
    }
}
