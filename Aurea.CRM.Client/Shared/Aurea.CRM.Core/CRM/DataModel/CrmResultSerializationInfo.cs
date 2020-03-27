// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResultSerializationInfo.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   CRM result serialization column infomation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    using Query;

    /// <summary>
    /// CRM result serialization column infomation
    /// </summary>
    public class UPCRMResultSerializationColumnInfo
    {
        /// <summary>
        /// Gets or sets the CRM field.
        /// </summary>
        /// <value>
        /// The CRM field.
        /// </value>
        public UPCRMField CrmField { get; set; }

        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public string FunctionName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has attributes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has attributes; otherwise, <c>false</c>.
        /// </value>
        public bool HasAttributes { get; set; }

        /// <summary>
        /// Gets or sets the name of the index attribute.
        /// </summary>
        /// <value>
        /// The name of the index attribute.
        /// </value>
        public string IndexAttributeName { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [report external key].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [report external key]; otherwise, <c>false</c>.
        /// </value>
        public bool ReportExternalKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [report raw value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [report raw value]; otherwise, <c>false</c>.
        /// </value>
        public bool ReportRawValue { get; set; }

        /// <summary>
        /// Externals the key for raw value.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExternalKeyForRawValue(string rawValue)
        {
            return this.CrmField?.FieldInfo?.ExtKeyForRawValueOptions(rawValue, 0);
        }
    }

    /// <summary>
    /// CRM result serialization infomation
    /// </summary>
    public class UPCRMResultSerializationInfo
    {
        /// <summary>
        /// The column infos.
        /// </summary>
        private readonly List<UPCRMResultSerializationColumnInfo> columnInfos;

        /// <summary>
        /// The record id attribute names.
        /// </summary>
        private readonly List<string> recordIdAttributeNames;

        /// <summary>
        /// The result info area meta infos.
        /// </summary>
        private readonly List<UPContainerInfoAreaMetaInfo> resultInfoAreaMetaInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultSerializationInfo"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public UPCRMResultSerializationInfo(UPCRMResult result)
        {
            this.resultInfoAreaMetaInfos = new List<UPContainerInfoAreaMetaInfo>();
            this.recordIdAttributeNames = new List<string>();
            var crmQuery = result.MetaInfo;
            var count = crmQuery.NumberOfResultInfoAreaMetaInfos();
            for (var i = 0; i < count; i++)
            {
                var infoAreaMetaInfo = crmQuery.ResultInfoAreaMetaInfoAtIndex(i);
                this.resultInfoAreaMetaInfos.Add(infoAreaMetaInfo);
                this.recordIdAttributeNames.Add(
                    infoAreaMetaInfo.LinkId > 0
                        ? $"record{infoAreaMetaInfo.InfoAreaId}{infoAreaMetaInfo.LinkId}"
                        : $"record{infoAreaMetaInfo.InfoAreaId}");
            }

            var columnCount = result.ColumnCount;
            this.columnInfos = new List<UPCRMResultSerializationColumnInfo>(columnCount);
            for (var i = 0; i < columnCount; i++)
            {
                var fieldMetaInfo = crmQuery.OutputField(i);
                var columnInfo = new UPCRMResultSerializationColumnInfo();
                this.columnInfos.Add(columnInfo);
                columnInfo.FunctionName = fieldMetaInfo.FunctionName;
                if (string.IsNullOrEmpty(fieldMetaInfo.FunctionName))
                {
                    columnInfo.IndexAttributeName = $"{i}";
                }

                columnInfo.Label = fieldMetaInfo.ConfigField.Label;
                columnInfo.CrmField = fieldMetaInfo.CrmField;
                columnInfo.ReportRawValue = result.ReportRawValueForColumnNumber(i);
                if (columnInfo.ReportRawValue)
                {
                    columnInfo.ReportExternalKey = columnInfo.CrmField.IsVariableCatalogField;
                }

                columnInfo.HasAttributes = columnInfo.ReportRawValue
                                           || !string.IsNullOrEmpty(columnInfo.IndexAttributeName);
            }
        }

        /// <summary>
        /// Numbers the of columns.
        /// </summary>
        /// <returns></returns>
        public int NumberOfColumns => this.columnInfos.Count;

        /// <summary>
        /// Columns the index of the information at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMResultSerializationColumnInfo"/>.
        /// </returns>
        public UPCRMResultSerializationColumnInfo ColumnInfoAtIndex(int index)
        {
            return this.columnInfos[index];
        }

        /// <summary>
        /// Records the index of the identifier attribute at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdAttributeAtIndex(int index)
        {
            return this.recordIdAttributeNames[index];
        }
    }
}
