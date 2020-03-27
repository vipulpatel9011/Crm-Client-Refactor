// <copyright file="AnalysisResult.cs" company="Aurea Software Gmbh">
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
    using System;
    using System.Collections.Generic;
    using CRM;
    using Drilldown;
    using Extensions;

    /// <summary>
    /// Implementation of Analysis result class
    /// </summary>
    public class AnalysisResult
    {
        private List<object> categoryOptions;
        private List<object> columns;
        private List<object> detailsFields;
        private bool detailsFieldsLoaded;
        private List<object> drilldownOptions;
        private List<object> drillupOptions;
        private Dictionary<string, object> rowPerCategoryValue;
        private List<object> rows;
        private int sortColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisResult"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="dataSource">Data source</param>
        public AnalysisResult(Analysis analysis, ICrmDataSource dataSource)
        {
            this.Analysis = analysis;
            this.Settings = this.Analysis.CurrentSettings;
            this.DataSource = dataSource;
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets category options
        /// </summary>
        public List<object> CategoryOptions => this.categoryOptions;

        /// <summary>
        /// Gets columns
        /// </summary>
        public List<object> Columns => this.columns;

        /// <summary>
        /// Gets data source
        /// </summary>
        public ICrmDataSource DataSource { get; private set; }

        /// <summary>
        /// Gets details fields
        /// </summary>
        public List<object> DetailsFields
        {
            get
            {
                if (!this.detailsFieldsLoaded)
                {
                    List<object> detailsFieldArray = null;
                    int maxIndex = 0;
                    foreach (AnalysisSourceField sourceField in this.Analysis.SourceFieldArray)
                    {
                        int curIndex = sourceField.ConfigField.ListColNr;
                        if (curIndex <= 0)
                        {
                            continue;
                        }

                        while (maxIndex < curIndex)
                        {
                            if (maxIndex++ <= 0)
                            {
                                detailsFieldArray = new List<object>();
                            }
                            else
                            {
                                detailsFieldArray.Add(null);
                            }
                        }

                        if (maxIndex == curIndex)
                        {
                            detailsFieldArray.Add(sourceField);
                            ++maxIndex;
                        }
                        else
                        {
                            detailsFieldArray[curIndex - 1] = sourceField;
                        }
                    }

                    this.detailsFields = detailsFieldArray;
                    this.detailsFieldsLoaded = true;
                }

                return this.detailsFields;
            }
        }

        /// <summary>
        /// Gets drilldown options
        /// </summary>
        public List<object> DrilldownOptions => this.drilldownOptions;

        /// <summary>
        /// Gets drillup options
        /// </summary>
        public List<object> DrillupOptions => this.drillupOptions;

        /// <summary>
        /// Gets error
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this has details fields
        /// </summary>
        public bool HasDetailsFields => this.DetailsFields.Count > 0;

        /// <summary>
        /// Gets a value indicating whether is server result
        /// </summary>
        public bool IsServerResult => this.DataSource.IsServerResult;

        /// <summary>
        /// Gets number of columns
        /// </summary>
        public int NumberOfColumns => this.columns.Count;

        /// <summary>
        /// Gets number of rows
        /// </summary>
        public int NumberOfRows => this.rows.Count;

        /// <summary>
        /// Gets rows
        /// </summary>
        public List<object> Rows => this.rows;

        /// <summary>
        /// Gets settings
        /// </summary>
        public AnalysisExecutionSettings Settings { get; private set; }

        /// <summary>
        /// Gets or sets sort column
        /// </summary>
        public int SortColumn
        {
            get
            {
                return this.sortColumn;
            }

            set
            {
                this.sortColumn = value;
            }
        }

        /// <summary>
        /// Gets or sets sum line
        /// </summary>
        public AnalysisRow SumLine { get; set; }

        /// <summary>
        /// Adds category option
        /// </summary>
        /// <param name="category">Category</param>
        public void AddCategoryOption(AnalysisCategory category)
        {
            if (this.categoryOptions == null)
            {
                this.categoryOptions = new List<object> { category };
            }
            else
            {
                this.categoryOptions.Add(category);
            }
        }

        /// <summary>
        /// Adds column
        /// </summary>
        /// <param name="column">Column</param>
        public void AddColumn(AnalysisColumn column)
        {
            if (this.columns == null)
            {
                this.columns = new List<object> { column };
            }
            else
            {
                this.columns.Add(column);
            }
        }

        /// <summary>
        /// Adds drilldown option
        /// </summary>
        /// <param name="option">Option</param>
        public void AddDrilldownOption(AnalysisDrilldownOption option)
        {
            if (this.drilldownOptions == null)
            {
                this.drilldownOptions = new List<object> { option };
            }
            else
            {
                this.drilldownOptions.Add(option);
            }
        }

        /// <summary>
        /// Adds drillup option
        /// </summary>
        /// <param name="option">Option</param>
        public void AddDrillupOption(AnalysisDrillupOption option)
        {
            if (this.drillupOptions == null)
            {
                this.drillupOptions = new List<object> { option };
            }
            else
            {
                this.drillupOptions.Add(option);
            }
        }

        /// <summary>
        /// Adds row
        /// </summary>
        /// <param name="row">Row</param>
        public void AddRow(AnalysisRow row)
        {
            if (this.rows == null)
            {
                this.rows = new List<object> { row };
            }
            else
            {
                this.rows.Add(row);
            }
        }

        /// <summary>
        /// Column at index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Return column at index</returns>
        public AnalysisColumn ColumnAtIndex(int index)
        {
            return (AnalysisColumn)this.columns[index];
        }

        /// <summary>
        /// Row at index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Return row at index</returns>
        public AnalysisRow RowAtIndex(int index)
        {
            return (AnalysisRow)this.rows[index];
        }

        /// <summary>
        /// Row for category value
        /// </summary>
        /// <param name="categoryValue">Category value</param>
        /// <returns>Returns row for category value</returns>
        public AnalysisRow RowForCategoryValue(string categoryValue)
        {
            if (this.rowPerCategoryValue == null)
            {
                var rowForCategory = new Dictionary<string, object>();
                foreach (AnalysisRow row in this.rows)
                {
                    rowForCategory[row.Key] = row;
                }

                this.rowPerCategoryValue = rowForCategory;
            }

            return this.rowPerCategoryValue.ValueOrDefault(categoryValue) as AnalysisRow;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"AnalysisResult ({this.columns}): {this.rows}";
        }
    }
}
