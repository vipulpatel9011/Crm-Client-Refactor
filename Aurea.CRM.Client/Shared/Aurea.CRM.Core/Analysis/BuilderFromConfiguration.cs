// <copyright file="BuilderFromConfiguration.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Denis Latushkin
// </author>

namespace Aurea.CRM.Core.Analysis
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Analysis.Model;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Configures analysis fields from configuration
    /// </summary>
    public class BuilderFromConfiguration
    {
        private List<object> sourceFields = new List<object>();
        private Dictionary<string, object> fieldDict = new Dictionary<string, object>();
        private Dictionary<int, object> tableDictByIndex = new Dictionary<int, object>();
        private Dictionary<string, object> tableDictByKey = new Dictionary<string, object>();
        private Dictionary<string, object> valueDictionary = new Dictionary<string, object>();
        private Dictionary<string, object> categoryDictionary = new Dictionary<string, object>();
        private List<object> filters = new List<object>();
        private Dictionary<string, object> resultColumns = new Dictionary<string, object>();
        private int occurrence;
        private bool hasCurrencyField;
        private AnalysisCategory firstCategory;
        private ICrmDataSourceMetaInfo dataSourceMetaInfo;
        private int tableIndex;
        private int fieldIndex;
        private int resultInfoAreaCount;
        private Dictionary<string, object> tablesPerInfoAreaId = new Dictionary<string, object>();
        private Dictionary<string, object> tablesForKey = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets Analysis instance
        /// </summary>
        public Analysis Analysis { get; set; }

        /// <summary>
        /// Gets or sets for DefaultCategory
        /// </summary>
        public Action<AnalysisCategory> SetDefaultCategory { get; set; }

        /// <summary>
        /// Gets or sets for XCategory
        /// </summary>
        public Action<AnalysisCategory> SetXCategory { get; set; }

        /// <summary>
        /// Gets or sets for CurrencyField
        /// </summary>
        public Action<AnalysisSourceField> SetCurrencyField { get; set; }

        /// <summary>
        /// Gets or sets for WeightField
        /// </summary>
        public Action<AnalysisSourceField> SetWeightField { get; set; }

        /// <summary>
        /// Gets or sets for TableDictionaryByKey
        /// </summary>
        public Action<Dictionary<string, object>> SetTableDictionaryByKey { get; set; }

        /// <summary>
        /// Gets or sets for TableDictionaryByIndex
        /// </summary>
        public Action<Dictionary<int, object>> SetTableDictionaryByIndex { get; set; }

        /// <summary>
        /// Gets or sets for FieldDictionary
        /// </summary>
        public Action<Dictionary<string, object>> SetFieldDictionary { get; set; }

        /// <summary>
        /// Gets or sets for SourceFieldArray
        /// </summary>
        public Action<List<object>> SetSourceFieldArray { get; set; }

        /// <summary>
        /// Gets or sets for ResultColumnDictionary
        /// </summary>
        public Action<Dictionary<string, object>> SetResultColumnDictionary { get; set; }

        /// <summary>
        /// Gets or sets for ValueDictionary
        /// </summary>
        public Action<Dictionary<string, object>> SetValueDictionary { get; set; }

        /// <summary>
        /// Gets or sets for Filters
        /// </summary>
        public Action<List<object>> SetFilters { get; set; }

        /// <summary>
        /// Gets or sets for CurrencyConversion
        /// </summary>
        public Action<CurrencyConversion> SetCurrencyConversion { get; set; }

        /// <summary>
        /// Gets or sets for CategoryDictionary
        /// </summary>
        public Action<Dictionary<string, object>> SetCategoryDictionary { get; set; }

        /// <summary>
        /// Build From Configuration
        /// </summary>
        public void BuildFromConfiguration()
        {
            this.IntializeBuild();

            for (var index = 0; index < this.resultInfoAreaCount; index++)
            {
                var table = this.dataSourceMetaInfo.ResultTableAtIndex(index);
                var infoAreaId = table.InfoAreaId;
                var infoAreaIdTables = this.tablesPerInfoAreaId.ValueOrDefault(infoAreaId) as List<object>;
                if (infoAreaIdTables == null)
                {
                    infoAreaIdTables = new List<object>();
                    this.occurrence = 0;
                    this.tablesPerInfoAreaId.SetObjectForKey(infoAreaIdTables, infoAreaId);
                }
                else
                {
                    this.occurrence = infoAreaIdTables.Count;
                }

                var tableInfo =
                    new AnalysisMetaInfoTableInfo(table, this.tableIndex++, this.fieldIndex, this.occurrence);
                infoAreaIdTables.Add(tableInfo);
                this.tablesForKey.SetObjectForKey(tableInfo, tableInfo.Key);
                this.fieldIndex += table.NumberOfFields();
            }

            this.ProcessConfigurationTable();

            foreach (AnalysisSourceField analysisSourceField in this.sourceFields)
            {
                if (analysisSourceField.IsResultColumn)
                {
                    var resultColumn = new AnalysisSourceFieldResultColumn(analysisSourceField);
                    this.resultColumns.SetObjectForKey(resultColumn, resultColumn.AnalysisField.Key);
                }
            }

            if (this.Analysis.DefaultCategory == null && this.firstCategory != null)
            {
                this.SetDefaultCategory(this.firstCategory);
            }

            this.SetTableDictionaryByKey(this.tableDictByKey);
            this.SetTableDictionaryByIndex(this.tableDictByIndex);
            this.SetFieldDictionary(this.fieldDict);
            this.SetCategoryDictionary(this.categoryDictionary);
            this.SetSourceFieldArray(this.sourceFields);

            this.ProcessingAnalysisConfiguration();

            this.SetResultColumnDictionary(this.resultColumns);
            this.SetValueDictionary(this.valueDictionary);
            this.SetFilters(this.filters);
            if (this.hasCurrencyField)
            {
                this.SetCurrencyConversion(CurrencyConversion.DefaultConversion);
            }
        }

        private void IntializeBuild()
        {
            this.sourceFields = new List<object>();
            this.fieldDict = new Dictionary<string, object>();
            this.tableDictByIndex = new Dictionary<int, object>();
            this.tableDictByKey = new Dictionary<string, object>();
            this.valueDictionary = new Dictionary<string, object>();
            this.categoryDictionary = new Dictionary<string, object>();
            this.filters = new List<object>();
            this.resultColumns = new Dictionary<string, object>();
            this.hasCurrencyField = false;
            this.firstCategory = null;
            this.dataSourceMetaInfo = this.Analysis.ExecutionContext.MetaInfo;
            this.tableIndex = 0;
            this.fieldIndex = 0;
            this.tablesPerInfoAreaId = new Dictionary<string, object>();
            this.tablesForKey = new Dictionary<string, object>();
            this.resultInfoAreaCount = this.dataSourceMetaInfo?.NumberOfResultTables ?? 0;
        }

        private void ProcessConfigurationTable()
        {
            foreach (var aTable in this.Analysis.Configuration.Tables)
            {
                var sourceTable = this.tablesForKey.ValueOrDefault(aTable.Key) as AnalysisMetaInfoTableInfo;
                if (sourceTable == null)
                {
                    continue;
                }

                var dataSourceTable = sourceTable.DataSourceTable;
                var table = new AnalysisTable(this.Analysis, aTable, dataSourceTable.InfoAreaPositionInResult());
                this.tableDictByIndex.SetObjectForKey(table, aTable.TableNumber);
                this.tableDictByKey.SetObjectForKey(table, aTable.Key);
                var numberOfFields = dataSourceTable.NumberOfFields();
                this.fieldIndex = sourceTable.FirstFieldIndex;
                var sourceFieldDict = new Dictionary<string, object>();
                for (var index = 0; index < numberOfFields; index++)
                {
                    var sourceField = dataSourceTable.FieldAtIndex(index);
                    var fieldInfo = new AnalysisMetaInfoFieldInfo(sourceTable, sourceField, this.fieldIndex++);
                    sourceFieldDict.SetObjectForKey(fieldInfo, fieldInfo.Key);
                }

                for (var index = 0; index < numberOfFields; index++)
                {
                    var sourceField = dataSourceTable.FieldAtIndex(index);
                    var fieldKey = AnalysisMetaInfoFieldInfo.KeyForTableFieldId(sourceTable, sourceField.FieldId);
                    var fieldInfo = sourceFieldDict.ValueOrDefault(fieldKey) as AnalysisMetaInfoFieldInfo;
                    if (fieldInfo == null)
                    {
                        continue;
                    }

                    var subFieldIndices = sourceField.SubFieldIndices();
                    if (subFieldIndices?.Count > 0)
                    {
                        foreach (var subFieldIndex in subFieldIndices)
                        {
                            var subFieldKey =
                                AnalysisMetaInfoFieldInfo.KeyForTableFieldId(sourceTable, subFieldIndex.ToInt());
                            var subFieldInfo = sourceFieldDict.ValueOrDefault(subFieldKey) as AnalysisMetaInfoFieldInfo;
                            if (subFieldInfo != null)
                            {
                                fieldInfo.AddSubField(subFieldInfo);
                            }
                        }
                    }
                }

                this.ProcessConfigurationFields(aTable, sourceFieldDict, table);
                var tableResultColumn = new AnalysisTableResultColumn(table);
                this.resultColumns.SetObjectForKey(tableResultColumn, table.Key);
            }
        }

        private void ProcessConfigurationFields(
            UPConfigAnalysisTable analysisTable,
            Dictionary<string, object> sourceFieldDict,
            AnalysisTable table)
        {
            foreach (var field in analysisTable.Fields)
            {
                var fieldInfo = sourceFieldDict.ValueOrDefault(field.Key) as AnalysisMetaInfoFieldInfo;
                if (fieldInfo == null)
                {
                    continue;
                }

                List<object> subQueryFieldIndices = null;
                if (fieldInfo.SubFields?.Count > 0)
                {
                    subQueryFieldIndices = new List<object>();
                    foreach (AnalysisMetaInfoFieldInfo subInfo in fieldInfo.SubFields)
                    {
                        subQueryFieldIndices.Add(subInfo.FieldIndex);
                    }
                }

                var analysisSourceField =
                    new AnalysisSourceField(table, field, fieldInfo.FieldIndex, subQueryFieldIndices);
                this.fieldDict.SetObjectForKey(analysisSourceField, analysisSourceField.Key);
                this.sourceFields.Add(analysisSourceField);
                table.AddSourceField(analysisSourceField);
                this.ProcessCategory(analysisSourceField);

                this.ProcessXCategory(analysisSourceField);

                if (analysisSourceField.IsCurrency)
                {
                    this.SetCurrencyField(analysisSourceField);
                    this.hasCurrencyField = true;
                }
                else if (analysisSourceField.IsTableCurrency)
                {
                    table.AlternateCurrencyField = analysisSourceField;
                    this.hasCurrencyField = true;
                }

                if (analysisSourceField.IsWeight)
                {
                    this.SetWeightField(analysisSourceField);
                }

                if (analysisSourceField.IsFilter)
                {
                    var analysisFilter = new AnalysisSourceFieldFilter(analysisSourceField);
                    this.filters.Add(analysisFilter);
                }
            }
        }

        private void ProcessCategory(AnalysisSourceField analysisSourceField)
        {
            if (!analysisSourceField.IsCategory)
            {
                return;
            }

            AnalysisExplicitCategoryField categoryBaseField = null;
            if (analysisSourceField.CategoryName?.Length > 0)
            {
                categoryBaseField = new AnalysisExplicitCategoryField(
                    this.Analysis,
                    analysisSourceField,
                    analysisSourceField.CategoryName);
            }

            var category = categoryBaseField == null
                ? (AnalysisCategory)new AnalysisSourceFieldCategory(analysisSourceField)
                : new AnalysisExplicitCategoryFieldCategory(categoryBaseField);

            this.categoryDictionary.SetObjectForKey(category, category.Key);
            if (analysisSourceField.IsDefaultCategory)
            {
                this.SetDefaultCategory(category);
            }
            else if (this.firstCategory == null)
            {
                this.firstCategory = category;
            }
        }

        private void ProcessXCategory(AnalysisSourceField analysisSourceField)
        {
            if (!analysisSourceField.IsXCategory)
            {
                return;
            }

            AnalysisExplicitCategoryField categoryBaseField = null;
            if (analysisSourceField.CategoryName?.Length > 0)
            {
                categoryBaseField = new AnalysisExplicitCategoryField(
                    this.Analysis,
                    analysisSourceField,
                    analysisSourceField.CategoryName);
            }

            if (categoryBaseField == null)
            {
                this.SetXCategory(new AnalysisSourceFieldCategory(analysisSourceField));
            }
            else
            {
                this.SetXCategory(new AnalysisExplicitCategoryFieldCategory(categoryBaseField));
            }
        }

        private void ProcessingAnalysisConfiguration()
        {
            if (this.Analysis.Configuration.Values == null)
            {
                return;
            }

            foreach (var value in this.Analysis.Configuration.Values)
            {
                var valueField = new AnalysisValueField(this.Analysis, value);

                this.valueDictionary.SetObjectForKey(valueField, valueField.Key);
                var valueResultColumn = new AnalysisValueResultColumn(valueField);

                this.resultColumns.SetObjectForKey(valueResultColumn, valueField.Key);
                if (!valueField.IsCategory && !valueField.IsDefaultCategory && !valueField.IsXCategory)
                {
                    continue;
                }

                var valueCategory = new AnalysisValueCategory(valueField);
                if (valueField.IsXCategory)
                {
                    this.SetXCategory(valueCategory);
                }
                else
                {
                    this.categoryDictionary.SetObjectForKey(valueCategory, valueCategory.Key);
                    if (valueField.IsDefaultCategory)
                    {
                        this.SetDefaultCategory(valueCategory);
                    }
                }
            }
        }
    }
}
