// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResult.cs" company="Aurea Software Gmbh">
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
//   CRM result implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Logging;
    using DAL;
    using Extensions;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Query;
    using Session;
    using Utilities;

    /// <summary>
    /// CRM result implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializable" />
    /// <seealso cref="Aurea.CRM.Core.CRM.ICrmDataSource" />
    public class UPCRMResult : UPSerializable, ICrmDataSource
    {
        /// <summary>
        /// The crm fields.
        /// </summary>
        private List<UPCRMField> crmFields;

        /// <summary>
        /// The field map.
        /// </summary>
        private Dictionary<string, UPContainerFieldMetaInfo> fieldMap;

        /// <summary>
        /// The link id.
        /// </summary>
        private readonly int linkId;

        /// <summary>
        /// The parent record identification.
        /// </summary>
        private readonly string parentRecordIdentification;

        /// <summary>
        /// The record set.
        /// </summary>
        private readonly object recordSet;

        /// <summary>
        /// The serialization info.
        /// </summary>
        private UPCRMResultSerializationInfo serializationInfo;

        /// <summary>
        /// The server map.
        /// </summary>
        private List<UPCRMServerResultPointer> serverMap;

        /// <summary>
        /// The server result rows.
        /// </summary>
        private readonly List<object> serverResultRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResult"/> class.
        /// </summary>
        /// <param name="metaInfo">
        /// The meta information.
        /// </param>
        /// <param name="recordSet">
        /// The record set.
        /// </param>
        public UPCRMResult(UPContainerMetaInfo metaInfo, object recordSet)
        {
            this.recordSet = recordSet;
            this.MetaInfo = metaInfo;
            this.InitializeCrmFields();
            this.serverResultRows = (recordSet as Dictionary<string, object>)?.ValueOrDefault("resultRows") as List<object>;
            this.IsServer = false;
            this.MultipleInfoAreas = this.MetaInfo?.HasMultipleOutputInfoAreas ?? false;
            this.NewRecordIdentification = null;
            this.TimeZone = ServerSession.CurrentSession?.TimeZone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResult"/> class.
        /// </summary>
        /// <param name="metaInfo">
        /// The meta information.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        public UPCRMResult(UPContainerMetaInfo metaInfo, Dictionary<string, object> response)
        {
            this.MetaInfo = metaInfo;
            this.InitializeCrmFields();
            this.serverResultRows = response.ValueOrDefault("resultRows") as List<object>;
            this.recordSet = null;
            this.IsServer = true;
            this.MultipleInfoAreas = this.MetaInfo?.HasMultipleOutputInfoAreas ?? false;
            this.NewRecordIdentification = null;
            this.TimeZone = ServerSession.CurrentSession?.TimeZone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResult"/> class.
        /// </summary>
        /// <param name="metaInfo">
        /// The meta information.
        /// </param>
        /// <param name="newRecordIdentification">
        /// The new record identification.
        /// </param>
        /// <param name="parentRecordIdentification">
        /// The parent record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public UPCRMResult(
            UPContainerMetaInfo metaInfo,
            string newRecordIdentification,
            string parentRecordIdentification,
            int linkId)
        {
            this.MetaInfo = metaInfo;
            this.InitializeCrmFields();
            this.NewRecordIdentification = newRecordIdentification;
            this.parentRecordIdentification = parentRecordIdentification;
            this.linkId = linkId;
            this.recordSet = 0;
            this.IsServer = false;
            this.MultipleInfoAreas = false;
            this.serverResultRows = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResult"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        public UPCRMResult(string recordIdentification)
        {
            this.recordSet = null;
            this.MetaInfo = null;
            var recordRow = new List<object> { new List<string> { recordIdentification }, new List<object>() };
            this.serverResultRows = new List<object> { recordRow };
            this.IsServer = false;
            this.MultipleInfoAreas = false;
            this.NewRecordIdentification = null;
        }

        /// <summary>
        /// Result property list option
        /// </summary>
        public enum UPCRMResultPropertyListOptions
        {
            /// <summary>
            /// The raw value.
            /// </summary>
            RawValue = 1
        }

        /// <summary>
        /// Columns the count.
        /// </summary>
        /// <returns></returns>
        public int ColumnCount => this.MetaInfo?.OutputFieldCount() ?? 0;

        /// <summary>
        /// Determines whether [has multiple output information areas].
        /// </summary>
        /// <returns></returns>
        public bool HasMultipleOutputInfoAreas => this.MultipleInfoAreas;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is server.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is server; otherwise, <c>false</c>.
        /// </value>
        public bool IsServer { get; protected set; }

        /// <summary>
        /// Determines whether [is server result].
        /// </summary>
        /// <returns></returns>
        public bool IsServerResult => this.IsServer;

        /// <summary>
        /// Links the identifier.
        /// </summary>
        /// <returns></returns>
        public int LinkId => this.linkId;

        /// <summary>
        /// Gets or sets the meta information.
        /// </summary>
        /// <value>
        /// The meta information.
        /// </value>
        public UPContainerMetaInfo MetaInfo { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether {CC2D43FA-BBC4-448A-9D0B-7B57ADF2655C}[multiple information areas].
        /// </summary>
        /// <value>
        /// <c>true</c> if [multiple information areas]; otherwise, <c>false</c>.
        /// </value>
        public bool MultipleInfoAreas { get; protected set; }

        /// <summary>
        /// Gets or sets the new record identification.
        /// </summary>
        /// <value>
        /// The new record identification.
        /// </value>
        public string NewRecordIdentification { get; protected set; }

        /// <summary>
        /// Gets the number of result tables.
        /// </summary>
        /// <value>
        /// The number of result tables.
        /// </value>
        public int NumberOfResultTables => this.MetaInfo?.NumberOfResultTables ?? 0;

        /// <summary>
        /// Gets the Parent record identification.
        /// </summary>
        /// <returns></returns>
        public string ParentRecordIdentification => this.parentRecordIdentification;

        /// <summary>
        /// Gets the Row count.
        /// </summary>
        /// <returns></returns>
        public virtual int RowCount
        {
            get
            {
                if (this.serverResultRows != null)
                {
                    return this.serverResultRows.Count;
                }

                if (!string.IsNullOrEmpty(this.NewRecordIdentification))
                {
                    return 1;
                }

                return (this.recordSet as DatabaseRecordSet)?.GetRowCount() ?? 0;
            }
        }

        /// <summary>
        /// Gets the serialization information.
        /// </summary>
        /// <value>
        /// The serialization information.
        /// </value>
        public UPCRMResultSerializationInfo SerializationInfo
        {
            get
            {
                if (this.serializationInfo != null)
                {
                    return this.serializationInfo;
                }

                this.serializationInfo = new UPCRMResultSerializationInfo(this);
                return this.serializationInfo;
            }
        }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public UPCRMTimeZone TimeZone { get; private set; }

        /// <summary>
        /// Empties the client result.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMResult"/>.
        /// </returns>
        public static UPCRMResult EmptyClientResult()
        {
            return new UPCRMResult(null, 0);
        }

        /// <summary>
        /// Columns the index of the field meta information at.
        /// </summary>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerFieldMetaInfo"/>.
        /// </returns>
        public UPContainerFieldMetaInfo ColumnFieldMetaInfoAtIndex(int columnNumber)
        {
            return this.MetaInfo.OutputField(columnNumber);
        }

        /// <summary>
        /// Columns the index of the name at.
        /// </summary>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ColumnNameAtIndex(int columnNumber)
        {
            var fieldMetaInfo = this.ColumnFieldMetaInfoAtIndex(columnNumber);
            return fieldMetaInfo.CrmField.Label;
        }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceField"/>.
        /// </returns>
        public ICrmDataSourceField FieldAtIndex(int fieldIndex)
        {
            return this.MetaInfo.FieldAtPosition(fieldIndex);
        }

        /// <summary>
        /// Fields for field identifier information area identifier link identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerFieldMetaInfo"/>.
        /// </returns>
        public UPContainerFieldMetaInfo FieldForFieldIdInfoAreaIdLinkId(int fieldId, string infoAreaId, int linkId)
        {
            string key;
            if (infoAreaId == null)
            {
                infoAreaId = this.MetaInfo.RootInfoAreaMetaInfo.InfoAreaId;
            }

            if (this.fieldMap == null)
            {
                var mapDict = new Dictionary<string, UPContainerFieldMetaInfo>();
                foreach (var field in this.MetaInfo.OutputFields)
                {
                    key = this.KeyForFieldIdInfoAreaIdLinkId(field.FieldId, field.InfoAreaId, field.LinkId);
                    if (!mapDict.ContainsKey(key))
                    {
                        mapDict[key] = field;
                    }
                }

                this.fieldMap = mapDict;
            }

            key = this.KeyForFieldIdInfoAreaIdLinkId(fieldId, infoAreaId, linkId);
            return this.fieldMap.ValueOrDefault(key);
        }

        /// <summary>
        /// Keys for field identifier information area identifier link identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string KeyForFieldIdInfoAreaIdLinkId(int fieldId, string infoAreaId, int linkId)
        {
            return linkId > 0 ? $"{infoAreaId}#{linkId}.{fieldId}" : $"{infoAreaId}.{fieldId}";
        }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Logs this instance.
        /// </summary>
        public void Log()
        {
            this.Logger.LogDebug(this.ToString(), LogFlag.LogResults);
            // DDLogCSQL("%@", Description);
        }

        /// <summary>
        /// Records the set.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object RecordSet()
        {
            return this.recordSet;
        }

        /// <summary>
        /// Reports the catalog fields for column number.
        /// </summary>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ReportCatalogFieldsForColumnNumber(int columnNumber)
        {
            return this.crmFields.Count > columnNumber && this.crmFields[columnNumber].IsVariableCatalogField;
        }

        /// <summary>
        /// Reports the raw value for column number.
        /// </summary>
        /// <param name="columnNumber">
        /// The column number.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ReportRawValueForColumnNumber(int columnNumber)
        {
            return this.crmFields.Count > columnNumber && this.crmFields[columnNumber].ReportRawValue;
        }

        /// <summary>
        /// Results the index of the row at.
        /// </summary>
        /// <param name="rowNumber">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceRow"/>.
        /// </returns>
        public virtual ICrmDataSourceRow ResultRowAtIndex(int rowNumber)
        {
            if (this.serverResultRows != null)
            {
                return new UPCRMResultRow(this.serverResultRows[rowNumber] as List<object>, this);
            }

            if (!string.IsNullOrEmpty(this.NewRecordIdentification))
            {
                return rowNumber > 0 ? null : new UPCRMResultRow(this.NewRecordIdentification, this);
            }

            var recordSet = this.recordSet as DatabaseRecordSet;

            var row = recordSet?.GetRow(rowNumber);
            return row == null ? null : new UPCRMResultRow(row, this);
        }

        /// <summary>
        /// Results the index of the table at.
        /// </summary>
        /// <param name="tableIndex">
        /// Index of the table.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceTable"/>.
        /// </returns>
        public ICrmDataSourceTable ResultTableAtIndex(int tableIndex)
        {
            return this.MetaInfo.ResultTableAtIndex(tableIndex);
        }

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void Serialize(UPSerializer writer)
        {
            this.SerializeRootElementName(writer, "Result");
        }

        /// <summary>
        /// Serializes the name of the root element.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="rootElementName">
        /// Name of the root element.
        /// </param>
        public void SerializeRootElementName(UPSerializer writer, string rootElementName)
        {
            int count = this.RowCount;
            writer.WriteElementStart(rootElementName);
            writer.WriteAttributeValue("online", this.IsServer ? "true" : "false");
            writer.WriteElementStart("Rows");
            for (int i = 0; i < count; i++)
            {
                writer.WriteObject(this.ResultRowAtIndex(i) as UPSerializable);
            }

            writer.WriteElementEnd();
            if (this.MetaInfo != null)
            {
                var _serializationInfo = this.SerializationInfo;
                writer.WriteElementStart("Columns");
                count = _serializationInfo.NumberOfColumns;

                for (int i = 0; i < count; i++)
                {
                    var columnInfo = _serializationInfo.ColumnInfoAtIndex(i);
                    var label = columnInfo.Label;
                    if (!string.IsNullOrEmpty(columnInfo.FunctionName))
                    {
                        writer.WriteElementValue(columnInfo.FunctionName, label);
                    }
                    else
                    {
                        var attributes = new Dictionary<string, string> { { "idx", columnInfo.IndexAttributeName } };
                        writer.WriteElementValueAttributes("Column", label, attributes);
                    }
                }

                writer.WriteElementEnd();
            }

            writer.WriteElementEnd();
        }

        /// <summary>
        /// Servers the map.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMServerResultPointer> ServerMap()
        {
            if (!this.IsServer)
            {
                return null;
            }

            if (this.serverMap != null)
            {
                return this.serverMap;
            }

            var mapper = new List<UPCRMServerResultPointer>();
            var positionInServerResultMap = 0;
            foreach (var fieldMetaInfo in this.MetaInfo.OutputFields)
            {
                mapper.Add(new UPCRMServerResultPointer(fieldMetaInfo.PositionInInfoArea, fieldMetaInfo.InfoAreaPosition));
                fieldMetaInfo.PositionInServerResultMap = positionInServerResultMap++;
            }

            this.serverMap = mapper;
            return this.serverMap;
        }

        /// <summary>
        /// Servers the response.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ServerResponse()
        {
            return this.serverResultRows != null
                       ? new Dictionary<string, object> { { "resultRows", this.serverResultRows } }
                       : null;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new System.Text.StringBuilder();
            builder.AppendLine($"UPCRMResult rowCount={this.RowCount}, columnCount={this.ColumnCount}");

            if (this.ColumnCount > 0)
            {
                builder.Append("columns: ");
                for (int i = 0; i < this.ColumnCount; i++)
                {
                    var columnName = this.ColumnNameAtIndex(i);
                    builder.Append(i == 0 ? columnName : $", {columnName}");
                }

                builder.AppendLine();
            }

            var maxRow = this.RowCount;
            if (maxRow > 100)
            {
                maxRow = 100;
            }

            for (int i = 0; i < maxRow; i++)
            {
                var row = this.ResultRowAtIndex(i) as UPCRMResultRow;
                if (row != null)
                {
                    builder.AppendLine($" {row}");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Initializes the CRM fields.
        /// </summary>
        private void InitializeCrmFields()
        {
            if (this.MetaInfo == null)
            {
                return;
            }

            this.crmFields = this.MetaInfo.OutputFields?.Select(field => field.CrmField).ToList();
        }
    }
}
