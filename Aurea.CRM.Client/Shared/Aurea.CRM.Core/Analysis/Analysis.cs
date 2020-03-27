// <copyright file="Analysis.cs" company="Aurea Software Gmbh">
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
    using System;
    using System.Collections.Generic;
    using Configuration;
    using CRM;
    using CRM.Features;
    using DataSource;
    using Extensions;
    using Model;
    using Processing;
    using Result;

    /// <summary>
    /// Implementation of analysis
    /// </summary>
    public class Analysis : IAnalysisExecutionContextDelegate
    {
        private AnalysisExecutionContext currentContext;
        private AnalysisExecutionSettings currentSettings;
        private AnalysisDataSourceCache dataSourceCache;
        private AnalysisExecutionSettings defaultSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Analysis"/> class.
        /// </summary>
        /// <param name="executionContext">Execution context</param>
        /// <param name="options">Options</param>
        public Analysis(AnalysisExecutionContext executionContext, Dictionary<string, object> options)
        {
            this.ExecutionContext = executionContext;
            this.Configuration = this.ExecutionContext.AnalysisConfiguration;
            this.Options = options;
            this.BuildFromConfiguration();
        }

        /// <summary>
        /// Gets category dictionary
        /// </summary>
        public Dictionary<string, object> CategoryDictionary { get; private set; }

        /// <summary>
        /// Gets configuration
        /// </summary>
        public UPConfigAnalysis Configuration { get; private set; }

        /// <summary>
        /// Gets currency conversion
        /// </summary>
        public CurrencyConversion CurrencyConversion { get; private set; }

        /// <summary>
        /// Gets currency field
        /// </summary>
        public AnalysisSourceField CurrencyField { get; private set; }

        /// <summary>
        /// Gets current settings
        /// </summary>
        public AnalysisExecutionSettings CurrentSettings => this.currentSettings;

        /// <summary>
        /// Gets default category
        /// </summary>
        public AnalysisCategory DefaultCategory { get; private set; }

        /// <summary>
        /// Gets default execution settings
        /// </summary>
        public AnalysisExecutionSettings DefaultExecutionSettings => this.defaultSettings ??
                                                                     (this.defaultSettings = new AnalysisExecutionSettings(this, this.Options));

        /// <summary>
        /// Gets execution context
        /// </summary>
        public AnalysisExecutionContext ExecutionContext { get; }

        /// <summary>
        /// Gets field dictionary
        /// </summary>
        public Dictionary<string, object> FieldDictionary { get; private set; }

        /// <summary>
        /// Gets filters
        /// </summary>
        public List<object> Filters { get; private set; }

        /// <summary>
        /// Gets options
        /// </summary>
        public Dictionary<string, object> Options { get; }

        /// <summary>
        /// Gets result column dictionary
        /// </summary>
        public Dictionary<string, object> ResultColumnDictionary { get; private set; }

        /// <summary>
        /// Gets source field array
        /// </summary>
        public List<object> SourceFieldArray { get; private set; }

        /// <summary>
        /// Gets table dictionary by index
        /// </summary>
        public Dictionary<int, object> TableDictionaryByIndex { get; private set; }

        /// <summary>
        /// Gets table dictionary by key
        /// </summary>
        public Dictionary<string, object> TableDictionaryByKey { get; private set; }

        /// <summary>
        /// Gets the delegate
        /// </summary>
        public IAnalysisDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets value dictionary
        /// </summary>
        public Dictionary<string, object> ValueDictionary { get; private set; }

        /// <summary>
        /// Gets weight field
        /// </summary>
        public AnalysisSourceField WeightField { get; private set; }

        /// <summary>
        /// Gets x category
        /// </summary>
        public AnalysisCategory XCategory { get; private set; }

        /// <summary>
        /// Compute result with settings
        /// </summary>
        /// <param name="settings">LogSettings</param>
        /// <returns>Analysis result</returns>
        public AnalysisResult ComputeResultWithSettings(AnalysisExecutionSettings settings)
        {
            this.currentSettings = settings;
            this.currentContext = this.ExecutionContext.ExecutionContext();
            var result = this.currentContext.QueryResult();
            AnalysisProcessing processing = new AnalysisProcessing(this, this.currentSettings, result);
            return processing.ComputeResult();
        }

        /// <summary>
        /// Compute result settings request option
        /// </summary>
        /// <param name="settings">LogSettings</param>
        /// <param name="requestOption">Request option</param>
        /// <param name="theDelegate">Delegate</param>
        public void ComputeResultWithSettingsRequestOption(AnalysisExecutionSettings settings, UPRequestOption requestOption, IAnalysisDelegate theDelegate)
        {
            AnalysisExecutionContext currentContext = this.ExecutionContext.ExecutionContext();
            AnalysisDataSourceCacheItem cacheItem = this.dataSourceCache?.CacheItemForExecutionContext(currentContext);
            if (cacheItem != null)
            {
                this.TheDelegate = theDelegate;
                this.currentSettings = settings;
                this.currentContext = currentContext;
                this.ContinueWithQueryResult(cacheItem.DataSource);

                return;
            }

            this.TheDelegate = theDelegate;
            this.currentSettings = settings;
            this.currentContext = currentContext;
            this.currentContext.QueryResultWithRequestOptionDelegate(requestOption, this);
        }

        /// <inheritdoc />
        public void ExecutionContextDidFailWithError(AnalysisExecutionContext executionContext, Exception error)
        {
            this.TheDelegate.AnalysisDidFailWithError(this, error);
        }

        /// <inheritdoc />
        public void ExecutionContextDidFinishWithResult(AnalysisExecutionContext executionContext, ICrmDataSource result)
        {
            if (this.dataSourceCache == null)
            {
                this.dataSourceCache = new AnalysisDataSourceCache();
            }

            this.dataSourceCache.AddCacheItemWithExecutionContextResult(this.currentContext, result);
            this.ContinueWithQueryResult(result);
        }

        /// <summary>
        /// Field with table number field id
        /// </summary>
        /// <param name="tableNumber">Table number</param>
        /// <param name="fieldId">Field id</param>
        /// <returns>Returns analysis field</returns>
        public AnalysisField FieldWithTableNumberFieldId(int tableNumber, int fieldId)
        {
            return this.TableWithNumber(tableNumber).FieldDictionaryByFieldIndex.ValueOrDefault(fieldId) as AnalysisField;
        }

        /// <summary>
        /// Result column with key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns analysis result column</returns>
        public AnalysisResultColumn ResultColumnWithKey(string key)
        {
            return this.ResultColumnDictionary.ValueOrDefault(key) as AnalysisResultColumn;
        }

        /// <summary>
        /// Result column with table number field
        /// </summary>
        /// <param name="tableNumber">Table number</param>
        /// <param name="fieldId">Field id</param>
        /// <returns>Result column</returns>
        public AnalysisResultColumn ResultColumnWithTableNumberFieldId(int tableNumber, int fieldId)
        {
            string key;
            if (fieldId < 0)
            {
                AnalysisTable table = this.TableWithNumber(tableNumber);
                key = table.Key;
            }
            else
            {
                AnalysisField field = this.FieldWithTableNumberFieldId(tableNumber, fieldId);
                key = field.Key;
            }

            if (key?.Length > 0)
            {
                return this.ResultColumnDictionary.ValueOrDefault(key) as AnalysisResultColumn;
            }

            return null;
        }

        /// <summary>
        /// Table with info area
        /// </summary>
        /// <param name="infoAreaId">Info area id</param>
        /// <param name="occurrence">Occurrence</param>
        /// <returns>Returns analysis table</returns>
        public AnalysisTable TableWithInfoAreaIdOccurrence(string infoAreaId, int occurrence)
        {
            return this.TableWithKey(UPConfigAnalysisTable.KeyWithInfoAreaIdOccurrence(infoAreaId, occurrence));
        }

        /// <summary>
        /// Table with key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns analysis key</returns>
        public AnalysisTable TableWithKey(string key)
        {
            return this.TableDictionaryByKey.ValueOrDefault(key) as AnalysisTable;
        }

        /// <summary>
        /// Table with number
        /// </summary>
        /// <param name="tableNumber">Table number</param>
        /// <returns>Returns analysis table</returns>
        public AnalysisTable TableWithNumber(int tableNumber)
        {
            return this.TableDictionaryByIndex.ValueOrDefault(tableNumber) as AnalysisTable;
        }

        /// <summary>
        /// String representation of instance
        /// </summary>
        /// <returns>Returns string representation of instance</returns>
        public override string ToString()
        {
            return $"cat={this.CategoryDictionary.Values}, col={this.ResultColumnDictionary.Values}, filter={this.Filters}, config={this.Configuration}";
        }

        private void BuildFromConfiguration()
        {
            var builder = new BuilderFromConfiguration
            {
                Analysis = this,
                SetDefaultCategory = category => this.DefaultCategory = category,
                SetXCategory = category => this.XCategory = category,
                SetCurrencyField = field => this.CurrencyField = field,
                SetWeightField = field => this.WeightField = field,
                SetTableDictionaryByKey = dictionary => this.TableDictionaryByKey = dictionary,
                SetTableDictionaryByIndex = dictionary => this.TableDictionaryByIndex = dictionary,
                SetFieldDictionary = dictionary => this.FieldDictionary = dictionary,
                SetSourceFieldArray = array => this.SourceFieldArray = array,
                SetResultColumnDictionary = dictionary => this.ResultColumnDictionary = dictionary,
                SetValueDictionary = dictionary => this.ValueDictionary = dictionary,
                SetFilters = filters => this.Filters = filters,
                SetCurrencyConversion = conversion => this.CurrencyConversion = conversion,
                SetCategoryDictionary = dictionary => this.CategoryDictionary = dictionary
            };

            builder.BuildFromConfiguration();
        }

        private void ContinueWithQueryResult(ICrmDataSource dataSource)
        {
            AnalysisProcessing processing = new AnalysisProcessing(this, this.currentSettings, dataSource);
            AnalysisResult result = processing.ComputeResult();
            IAnalysisDelegate theDelegate = this.TheDelegate;
            this.TheDelegate = null;
            if (result.Error != null)
            {
                theDelegate.AnalysisDidFailWithError(this, result.Error);
            }
            else
            {
                theDelegate.AnalysisDidFinishWithResult(this, result);
            }
        }
    }
}
