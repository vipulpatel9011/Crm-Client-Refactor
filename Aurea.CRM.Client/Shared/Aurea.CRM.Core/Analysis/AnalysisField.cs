// <copyright file="AnalysisField.cs" company="Aurea Software Gmbh">
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
    using CRM;
    using CRM.DataModel;

    /// <summary>
    /// Implementation of analysis field
    /// </summary>
    public class AnalysisField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisField"/> class.
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="key">Key</param>
        public AnalysisField(Analysis analysis, string key)
        {
            this.Analysis = analysis;
            this.Key = key;
        }

        /// <summary>
        /// Gets analysis
        /// </summary>
        public Analysis Analysis { get; private set; }

        /// <summary>
        /// Gets category name
        /// </summary>
        public virtual string CategoryName => null;

        /// <summary>
        /// Gets or sets crm field info
        /// </summary>
        public UPCRMFieldInfo CrmFieldInfo { get; set; }

        /// <summary>
        /// Gets a value indicating whether is category
        /// </summary>
        public virtual bool IsCategory => false;

        /// <summary>
        /// Gets a value indicating whether is currency
        /// </summary>
        public virtual bool IsCurrency => false;

        /// <summary>
        /// Gets a value indicating whether is currency dependent
        /// </summary>
        public virtual bool IsCurrencyDependent => false;

        /// <summary>
        /// Gets a value indicating whether is date value
        /// </summary>
        public virtual bool IsDateValue => false;

        /// <summary>
        /// Gets a value indicating whether is default category
        /// </summary>
        public virtual bool IsDefaultCategory => false;

        /// <summary>
        /// Gets a value indicating whether is filter
        /// </summary>
        public virtual bool IsFilter => false;

        /// <summary>
        /// Gets a value indicating whether is result column
        /// </summary>
        public virtual bool IsResultColumn => false;

        /// <summary>
        /// Gets a value indicating whether is table currency
        /// </summary>
        public virtual bool IsTableCurrency => false;

        /// <summary>
        /// Gets a value indicating whether is weight
        /// </summary>
        public virtual bool IsWeight => false;

        /// <summary>
        /// Gets a value indicating whether is weight dependent
        /// </summary>
        public virtual bool IsWeightDependent => false;

        /// <summary>
        /// Gets a value indicating whether is x category
        /// </summary>
        public virtual bool IsXCategory => false;

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets label
        /// </summary>
        public virtual string Label => this.Key;

        /// <summary>
        /// Is empty for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns is empty for row</returns>
        public virtual bool IsEmptyForRow(ICrmDataSourceRow row)
        {
            return this.RawValueForRow(row).Length == 0;
        }

        /// <summary>
        /// Raw value array for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns raw value array for row</returns>
        public virtual List<object> RawValueArrayForRow(ICrmDataSourceRow row)
        {
            string v = this.RawValueForRow(row);
            if (v.Length == 0)
            {
                return null;
            }

            return new List<object> { v };
        }

        /// <summary>
        /// Raw value for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns raw value for row</returns>
        public virtual string RawValueForRow(ICrmDataSourceRow row)
        {
            return string.Empty;
        }

        /// <summary>
        /// String value for row
        /// </summary>
        /// <param name="row">Row</param>
        /// <returns>Returns string value for row</returns>
        public virtual string StringValueForRow(ICrmDataSourceRow row)
        {
            return string.Empty;
        }
    }
}
