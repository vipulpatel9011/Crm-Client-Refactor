// <copyright file="AnalysisColumn.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using Aurea.CRM.Core.Analysis.Model;
    using Aurea.CRM.Core.Analysis.Processing;

    /// <summary>
    /// Implementation for analysis column
    /// </summary>
    public class AnalysisColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisColumn"/> class.
        /// </summary>
        /// <param name="label">Label</param>
        public AnalysisColumn(string label)
        {
            this.Label = label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisColumn"/> class.
        /// </summary>
        /// <param name="resultColumn">Result column</param>
        /// <param name="xCategoryValues">X category value</param>
        public AnalysisColumn(AnalysisResultColumn resultColumn, List<AnalysisProcessingXCategoryValue> xCategoryValues)
        {
            this.Label = resultColumn.Label;
            this.ResultColumn = resultColumn;
            this.IsTextColumn = this.ResultColumn.ValueOptions?.IsText ?? false;
            if (xCategoryValues?.Count > 0)
            {
                var xCategoryArray = new List<AnalysisXColumn>();
                foreach (var v in xCategoryValues)
                {
                    xCategoryArray.Add(new AnalysisXColumn(this, v));
                }

                this.XCategoryValues = xCategoryArray;
            }
            else
            {
                this.XCategoryValues = null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is text column
        /// </summary>
        public bool IsTextColumn { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets result column
        /// </summary>
        public AnalysisResultColumn ResultColumn { get; private set; }

        /// <summary>
        /// Gets x category values
        /// </summary>
        public List<AnalysisXColumn> XCategoryValues { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Label} {this.XCategoryValues}";
        }
    }
}
