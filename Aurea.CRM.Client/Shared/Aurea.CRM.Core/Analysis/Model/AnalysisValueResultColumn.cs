// <copyright file="AnalysisValueResultColumn.cs" company="Aurea Software Gmbh">
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
    using Value.AnalysisValueFunction;

    /// <summary>
    /// Implementation of analysis value result column
    /// </summary>
    public class AnalysisValueResultColumn : AnalysisResultColumn
    {
        private AnalysisAggregationType currentAggregationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueResultColumn"/> class.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="aggregationType">Aggregation type</param>
        public AnalysisValueResultColumn(AnalysisValueField field, AnalysisAggregationType aggregationType = null)
        {
            this.AnalysisValue = field;
            this.currentAggregationType = field.FixedAggregationType ?? aggregationType;

            this.ValueFunction = AnalysisValueFunction.ValueFunctionForFormulaAnalysis(field.ConfigValue?.Parameter, field.Analysis);
            if (this.ValueFunction == null)
            {
                return;
            }

            int fractionDigits = field.Options?.FractionDigits ?? 2;
            this.Format = field.Options?.Format;
        }

        /// <inheritdoc/>
        public override AnalysisAggregationType AggregationType => this.currentAggregationType;

        /// <summary>
        /// Gets analysis value field
        /// </summary>
        public AnalysisValueField AnalysisValue { get; private set; }

        /// <inheritdoc/>
        public override List<object> AvailableAggregationTypes
        {
            get
            {
                AnalysisAggregationType fixedType = this.AnalysisValue.FixedAggregationType;
                return fixedType != null ? new List<object> { fixedType } : AnalysisAggregationType.All();
            }
        }

        /// <summary>
        /// Gets format
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is static.
        /// </summary>
        public bool IsStatic => this.currentAggregationType.StaticAggregator;

        /// <inheritdoc/>
        public override string Key => this.AnalysisValue.Key;

        /// <inheritdoc/>
        public override string Label => this.AnalysisValue.Label;

        /// <summary>
        /// Gets significant query resutl table indices
        /// </summary>
        public List<object> SignificantQueryResultTableIndices => this.ValueFunction.SignificantQueryResultTableIndices();

        /// <summary>
        /// Gets value function
        /// </summary>
        public AnalysisValueFunction ValueFunction { get; private set; }

        /// <inheritdoc/>
        public override AnalysisValueOptions ValueOptions => this.AnalysisValue.Options;

        /// <inheritdoc/>
        public override string DisplayStringFromNumber(double value)
        {
            return this.Format != null ? this.Format.Replace("#", base.DisplayStringFromNumber(value)) : base.DisplayStringFromNumber(value);
        }

        /// <inheritdoc/>
        public override AnalysisResultColumn ResultColumnWithAggregationType(string aggregationTypeString)
        {
            if (this.currentAggregationType.Type == aggregationTypeString || aggregationTypeString == "static")
            {
                return this;
            }

            AnalysisAggregationType type = AnalysisAggregationType.WithType(aggregationTypeString);
            return type == null ? null : new AnalysisValueResultColumn(this.AnalysisValue, type);
        }
    }
}
