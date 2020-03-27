// <copyright file="AnalysisSourceFieldFilter.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Analysis source field filter class implementation
    /// </summary>
    public class AnalysisSourceFieldFilter : AnalysisFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisSourceFieldFilter"/> class.
        /// </summary>
        /// <param name="analysisSourceField">Source field</param>
        public AnalysisSourceFieldFilter(AnalysisSourceField analysisSourceField)
            : base(string.Empty, string.Empty)
        {
            this.AnalysisField = analysisSourceField;
        }

        /// <summary>
        /// Gets or sets analysis field
        /// </summary>
        public AnalysisSourceField AnalysisField { get; set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public override string Key => this.AnalysisField.Key;

        /// <inheritdoc />
        public override string RawValueForRow(ICrmDataSourceRow row)
        {
            return row.RawValueAtIndex(this.AnalysisField.QueryResultFieldIndex);
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.AnalysisField.Key}{this}";

        /// <inheritdoc />
        public override string ValueForRawValue(string rawValue)
        {
            string infoAreaId = this.AnalysisField.AnalysisTable.ConfigTable.InfoAreaId;
            int fieldIndex = this.AnalysisField.ConfigField.FieldId;

            var fieldInfo = UPCRMDataStore.DefaultStore.FieldInfoForInfoAreaFieldId(infoAreaId, fieldIndex);
            return fieldInfo != null ? fieldInfo.ValueForRawValueOptions(rawValue, 0, null) : base.ValueForRawValue(rawValue);
        }
    }
}
