// <copyright file="AnalysisTable.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Implementation of analysis table
    /// </summary>
    public class AnalysisTable
    {
        private List<object> fieldArray;
        private Dictionary<int, object> fieldDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisTable"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="configTable">Config table</param>
        /// <param name="queryTableIndex">Query table index</param>
        public AnalysisTable(Analysis analysis, UPConfigAnalysisTable configTable, int queryTableIndex)
        {
            this.ConfigTable = configTable;
            this.Analysis = analysis;
            this.Key = configTable.Key;
            this.QueryTableIndex = queryTableIndex;
            this.fieldArray = new List<object>();
            this.fieldDictionary = new Dictionary<int, object>();
        }

        /// <summary>
        /// Gets or sets alternate currency field
        /// </summary>
        public AnalysisSourceField AlternateCurrencyField { get; set; }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets config table
        /// </summary>
        public UPConfigAnalysisTable ConfigTable { get; private set; }

        /// <summary>
        /// Gets field array
        /// </summary>
        public List<object> FieldArray => this.fieldArray;

        /// <summary>
        /// Gets field dictionary by field index
        /// </summary>
        public Dictionary<int, object> FieldDictionaryByFieldIndex => this.fieldDictionary;

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public string Label => this.ConfigTable.InfoAreaId;

        /// <summary>
        /// Gets query table index
        /// </summary>
        public int QueryTableIndex { get; private set; }

        /// <summary>
        /// Key with info area id
        /// </summary>
        /// <param name="infoAreaId">Info area id</param>
        /// <param name="occurrence">Occurrence</param>
        /// <returns>kKey</returns>
        public static string KeyWithInfoAreaIdOccurrence(string infoAreaId, int occurrence)
        {
            return UPConfigAnalysisTable.KeyWithInfoAreaIdOccurrence(infoAreaId, occurrence);
        }

        /// <summary>
        /// Adds source field
        /// </summary>
        /// <param name="analysisSourceField">Source field</param>
        public void AddSourceField(AnalysisSourceField analysisSourceField)
        {
            this.fieldArray.Add(analysisSourceField);
            this.fieldDictionary.SetObjectForKey(analysisSourceField, analysisSourceField.FieldIndex);
        }

        /// <summary>
        /// Creates a new <see cref="AnalysisCountField"/> instance
        /// </summary>
        /// <returns>New <see cref="AnalysisCountField"/></returns>
        public AnalysisCountField CountField()
        {
            return new AnalysisCountField(this.Analysis, this.ConfigTable, this.QueryTableIndex);
        }

        /// <summary>
        /// Field with index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Field</returns>
        public AnalysisSourceField FieldWithIndex(int index)
        {
            return this.fieldDictionary.ValueOrDefault(index) as AnalysisSourceField;
        }
    }
}
