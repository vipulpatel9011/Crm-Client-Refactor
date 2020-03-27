// <copyright file="AnalysisSourceField.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis source field
    /// </summary>
    public class AnalysisSourceField : AnalysisField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisSourceField"/> class.
        /// </summary>
        /// <param name="analysisTable">Analysis table</param>
        /// <param name="field">Field</param>
        /// <param name="queryResultFieldIndex">Query result field index</param>
        /// <param name="subFieldQueryResultFieldIndices">Sub field query result field indices</param>
        public AnalysisSourceField(AnalysisTable analysisTable, UPConfigAnalysisField field, int queryResultFieldIndex, List<object> subFieldQueryResultFieldIndices)
            : base(analysisTable.Analysis, field.Key)
        {
            this.QueryResultFieldIndex = queryResultFieldIndex;
            this.SubFieldQueryResultFieldIndices = subFieldQueryResultFieldIndices ?? new List<object>();
            this.ConfigField = field;
            this.AnalysisTable = analysisTable;
            this.CrmFieldInfo = UPCRMDataStore.DefaultStore.FieldInfoForInfoAreaFieldId(this.ConfigField.AnalysisTable.InfoAreaId, this.ConfigField.FieldId);
        }

        /// <summary>
        /// Gets analysis table
        /// </summary>
        public AnalysisTable AnalysisTable { get; private set; }

        /// <inheritdoc/>
        public override string CategoryName => this.ConfigField.CategoryName;

        /// <summary>
        /// Gets config field
        /// </summary>
        public UPConfigAnalysisField ConfigField { get; private set; }

        /// <summary>
        /// Gets field index
        /// </summary>
        public int FieldIndex => this.ConfigField.FieldId;

        /// <inheritdoc/>
        public override bool IsCategory => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.Category) != 0;

        /// <inheritdoc/>
        public override bool IsCurrency => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.Currency) != 0;

        /// <inheritdoc/>
        public override bool IsCurrencyDependent => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DependentOnCurrency) != 0;

        /// <inheritdoc/>
        public override bool IsDateValue => this.CrmFieldInfo.IsDateField;

        /// <inheritdoc/>
        public override bool IsDefaultCategory => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DefaultCategory) != 0;

        /// <inheritdoc/>
        public override bool IsFilter => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.Filter) != 0;

        /// <inheritdoc/>
        public override bool IsResultColumn => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.ResultColumn) != 0;

        /// <inheritdoc/>
        public override bool IsTableCurrency => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.AlternateCurrency) != 0;

        /// <inheritdoc/>
        public override bool IsWeight => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.Weight) != 0;

        /// <inheritdoc/>
        public override bool IsWeightDependent => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.DependentOnWeight) != 0;

        /// <inheritdoc/>
        public override bool IsXCategory => (this.ConfigField.Flags & (int)ConfigAnalysisFieldFlag.XCategory) != 0;

        /// <inheritdoc/>
        public override string Label => this.CrmFieldInfo.Label?.Length > 0 ? this.CrmFieldInfo.Label : this.Key;

        /// <summary>
        /// Gets query result field index
        /// </summary>
        public int QueryResultFieldIndex { get; private set; }

        /// <summary>
        /// Gets sub field query result field indices
        /// </summary>
        public List<object> SubFieldQueryResultFieldIndices { get; private set; }

        /// <inheritdoc/>
        public override bool IsEmptyForRow(ICrmDataSourceRow row)
        {
            return this.CrmFieldInfo.IsEmptyValue(row.RawValueAtIndex(this.QueryResultFieldIndex));
        }

        /// <inheritdoc/>
        public override List<object> RawValueArrayForRow(ICrmDataSourceRow row)
        {
            string v = this.RawValueForRow(row);
            return v.Length == 0 ? null : new List<object> { v };
        }

        /// <inheritdoc/>
        public override string RawValueForRow(ICrmDataSourceRow row)
        {
            return row.RawValueAtIndex(this.QueryResultFieldIndex);
        }

        /// <inheritdoc/>
        public override string StringValueForRow(ICrmDataSourceRow row)
        {
            return row.ValueAtIndex(this.QueryResultFieldIndex);
        }

        /// <summary>
        /// Value for raw value
        /// </summary>
        /// <param name="rawValue">Raw value</param>
        /// <returns>Returns value for raw value</returns>
        public string ValueForRawValue(string rawValue)
        {
            return this.CrmFieldInfo.ValueForRawValueOptions(rawValue, 0, null);
        }
    }
}
