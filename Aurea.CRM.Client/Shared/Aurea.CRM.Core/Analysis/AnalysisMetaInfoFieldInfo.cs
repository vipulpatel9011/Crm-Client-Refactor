// <copyright file="AnalysisMetaInfoFieldInfo.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Implementation of analysis metainfo field infp
    /// </summary>
    public class AnalysisMetaInfoFieldInfo
    {
        private List<object> subFieldArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisMetaInfoFieldInfo"/> class.
        /// </summary>
        /// <param name="tableInfo">Table info</param>
        /// <param name="dataSourceField">Data source field</param>
        /// <param name="fieldIndex">Field index</param>
        public AnalysisMetaInfoFieldInfo(AnalysisMetaInfoTableInfo tableInfo, ICrmDataSourceField dataSourceField, int fieldIndex)
        {
            this.DataSourceField = dataSourceField;
            this.FieldIndex = fieldIndex;
            this.TableInfo = tableInfo;
            this.subFieldArray = null;
        }

        /// <summary>
        /// Gets data source field
        /// </summary>
        public ICrmDataSourceField DataSourceField { get; private set; }

        /// <summary>
        /// Gets field index
        /// </summary>
        public int FieldIndex { get; private set; }

        /// <summary>
        /// Gets key
        /// </summary>
        public string Key => AnalysisMetaInfoFieldInfo.KeyForTableFieldId(this.TableInfo, this.DataSourceField.FieldId);

        /// <summary>
        /// Gets subfields
        /// </summary>
        public List<object> SubFields => this.subFieldArray;

        /// <summary>
        /// Gets table info
        /// </summary>
        public AnalysisMetaInfoTableInfo TableInfo { get; private set; }

        /// <summary>
        /// Gets key for table field id
        /// </summary>
        /// <param name="tableInfo">Table infp</param>
        /// <param name="fieldId">Field info</param>
        /// <returns>Field id</returns>
        public static string KeyForTableFieldId(AnalysisMetaInfoTableInfo tableInfo, int fieldId)
        {
            return $"{tableInfo.Key}.{(long)fieldId}";
        }

        /// <summary>
        /// Adds subfield id
        /// </summary>
        /// <param name="subField">Sub field</param>
        public void AddSubField(AnalysisMetaInfoFieldInfo subField)
        {
            if (this.subFieldArray == null)
            {
                this.subFieldArray = new List<object> { subField };
            }
            else
            {
                this.subFieldArray.Add(subField);
            }
        }
    }
}
