// <copyright file="AnalysisSourceFieldResultColumn.cs" company="Aurea Software Gmbh">
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
    /// Implementation of analysis source field result column
    /// </summary>
    public class AnalysisSourceFieldResultColumn : AnalysisResultColumn
    {
        private AnalysisAggregationType currentAggregationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisSourceFieldResultColumn"/> class.
        /// </summary>
        /// <param name="field">Analysis source field</param>
        public AnalysisSourceFieldResultColumn(AnalysisSourceField field)
            : this(field, AnalysisAggregationType.GetSum())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisSourceFieldResultColumn"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="aggregationType">Aggregation type</param>
        public AnalysisSourceFieldResultColumn(AnalysisSourceField field, AnalysisAggregationType aggregationType)
        {
            this.AnalysisField = field;
            this.currentAggregationType = aggregationType;

            if (field.IsWeightDependent)
            {
                this.WeightField = field.Analysis.WeightField;
            }

            if (field.IsCurrencyDependent)
            {
                this.CurrencyField = field.AnalysisTable.AlternateCurrencyField ?? field.Analysis.CurrencyField;
            }
        }

        /// <inheritdoc/>
        public override AnalysisAggregationType AggregationType => this.currentAggregationType;

        /// <summary>
        /// Gets analysis field
        /// </summary>
        public AnalysisSourceField AnalysisField { get; private set; }

        /// <inheritdoc/>
        public override List<object> AvailableAggregationTypes => AnalysisAggregationType.All();

        /// <summary>
        /// Gets currency field
        /// </summary>
        public AnalysisSourceField CurrencyField { get; private set; }

        /// <inheritdoc/>
        public override string Key => this.AnalysisField.Key;

        /// <inheritdoc/>
        public override string Label => this.AnalysisField.Label;

        /// <summary>
        /// Gets weight field
        /// </summary>
        public AnalysisSourceField WeightField { get; private set; }

        /// <inheritdoc/>
        public override AnalysisResultColumn ResultColumnWithAggregationType(string aggregationTypeString)
        {
            if (this.currentAggregationType.Type == aggregationTypeString)
            {
                return this;
            }

            AnalysisAggregationType type = AnalysisAggregationType.WithType(aggregationTypeString);
            return type == null ? null : new AnalysisSourceFieldResultColumn(this.AnalysisField, type);
        }
    }
}
