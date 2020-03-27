// <copyright file="AnalysisValueField.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Analysis.Model;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis value field
    /// </summary>
    public class AnalysisValueField : AnalysisField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisValueField"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="value">Value</param>
        public AnalysisValueField(Analysis analysis, UPConfigAnalysisValue value)
            : base(analysis, value?.Key)
        {
            this.ConfigValue = value;
            this.Options = new AnalysisValueOptions(this.ConfigValue?.Options);
        }

        /// <summary>
        /// Gets config value
        /// </summary>
        public UPConfigAnalysisValue ConfigValue { get; private set; }

        /// <summary>
        /// Gets fixed aggregation type
        /// </summary>
        public AnalysisAggregationType FixedAggregationType
        {
            get
            {
                if (this.Options.IsStatic)
                {
                    return AnalysisAggregationType.WithType("static");
                }

                string fixedTypeName = this.ConfigValue?.FixedType;
                return fixedTypeName?.Length > 0 ? AnalysisAggregationType.WithType(fixedTypeName) : null;
            }
        }

        /// <inheritdoc/>
        public override bool IsCategory => this.Options.IsCategory;

        /// <inheritdoc/>
        public override bool IsDefaultCategory => this.Options.IsDefaultCategory;

        /// <inheritdoc/>
        public override bool IsResultColumn => !this.Options.IsCategory || this.Options.IsColumn;

        /// <inheritdoc/>
        public override string Label => this.ConfigValue?.Label?.Length > 0 ? this.ConfigValue.Label : this.Key;

        /// <summary>
        /// Gets options
        /// </summary>
        public AnalysisValueOptions Options { get; private set; }

        /// <inheritdoc/>
        public override string RawValueForRow(ICrmDataSourceRow row)
        {
            return "0";
        }
    }
}
