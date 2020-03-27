// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmResultRow.cs" company="Aurea Software Gmbh">
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
//   CRM result row
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Utilities;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// CRM result row
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializable" />
    /// <seealso cref="Aurea.CRM.Core.CRM.ICrmDataSourceRow" />
    public class UPCRMResultRow : UPSerializable, ICrmDataSourceRow
    {
        /// <summary>
        /// The database row.
        /// </summary>
        private readonly DatabaseRow databaseRow;

        /// <summary>
        /// The new record identification.
        /// </summary>
        private readonly string newRecordIdentification;

        /// <summary>
        /// The server data.
        /// </summary>
        private readonly List<object> serverData;

        /// <summary>
        /// The server record ids.
        /// </summary>
        private readonly List<string> serverRecordIds;

        /// <summary>
        /// The has local copy.
        /// </summary>
        private bool hasLocalCopy;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultRow"/> class.
        /// </summary>
        /// <param name="databaseRow">
        /// The database row.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public UPCRMResultRow(DatabaseRow databaseRow, UPCRMResult result)
        {
            this.Result = result;
            this.databaseRow = databaseRow;
            this.serverData = null;
            this.serverRecordIds = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultRow"/> class.
        /// </summary>
        /// <param name="rowServerResponse">
        /// The row server response.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public UPCRMResultRow(List<object> rowServerResponse, UPCRMResult result)
        {
            this.Result = result;
            this.databaseRow = null;
            this.serverData = rowServerResponse[1] as List<object>;
            this.serverRecordIds =
                (rowServerResponse[0] as List<string> ?? (rowServerResponse[0] as List<object>)?.Cast<string>())
                ?.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMResultRow"/> class.
        /// </summary>
        /// <param name="newRecordIdentification">
        /// The new record identification.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public UPCRMResultRow(string newRecordIdentification, UPCRMResult result)
        {
            this.Result = result;
            this.databaseRow = null;
            this.serverData = null;
            this.serverRecordIds = null;
            this.newRecordIdentification = newRecordIdentification;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has local copy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has local copy; otherwise, <c>false</c>.
        /// </value>
        public bool HasLocalCopy => this.databaseRow != null || this.hasLocalCopy;

        /// <summary>
        /// Gets a value indicating whether this instance is new row.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is new row; otherwise, <c>false</c>.
        /// </value>
        public bool IsNewRow => !string.IsNullOrEmpty(this.newRecordIdentification);

        /// <summary>
        /// Gets a value indicating whether this instance is server response.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is server response; otherwise, <c>false</c>.
        /// </value>
        public bool IsServerResponse => this.serverData != null;

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>
        /// The number of columns.
        /// </value>
        public int NumberOfColumns => this.Result.ColumnCount;

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public UPCRMResult Result { get; }

        /// <summary>
        /// Gets the root record identifier.
        /// </summary>
        /// <value>
        /// The root record identifier.
        /// </value>
        public string RootRecordId => this.RecordIdAtIndex(0);

        /// <summary>
        /// Gets the root record identification.
        /// </summary>
        /// <value>
        /// The root record identification.
        /// </value>
        public string RootRecordIdentification => this.RecordIdentificationAtIndex(0);

        /// <summary>
        /// Gets the root virtual information area identifier.
        /// </summary>
        /// <value>
        /// The root virtual information area identifier.
        /// </value>
        public string RootVirtualInfoAreaId => this.VirtualInfoAreaIdAtIndex(0);

        /// <summary>
        /// Exts the index of the key value at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExtKeyValueAtIndex(int index)
        {
            return this.ExtKeyValueForRawValueColumnIndex(this.RawValueAtIndex(index), index);
        }

        /// <summary>
        /// Exts the index of the key value for raw value column.
        /// </summary>
        /// <param name="rawValue">
        /// The raw value.
        /// </param>
        /// <param name="columnIndex">
        /// Index of the column.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExtKeyValueForRawValueColumnIndex(string rawValue, int columnIndex)
        {
            if (string.IsNullOrEmpty(rawValue))
            {
                return null;
            }

            var crmField = this.Result.MetaInfo.OutputField(columnIndex).CrmField;
            return !crmField.IsVariableCatalogField ? null : crmField.FieldInfo.ExtKeyForRawValueOptions(rawValue, 0);
        }

        /// <summary>
        /// The formatted field value at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="fieldControl">
        /// The field control.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string FormattedFieldValueAtIndex(int index, int? offset, FieldControl fieldControl)
        {
            UPConfigFieldControlField configField = fieldControl.FieldAtIndex(index);
            FieldAttributes fieldAttributes = configField.Attributes;
            string result = null;
            int innerOffset;
            innerOffset = offset ?? 0;

            if (fieldAttributes.FieldCount > 1)
            {
                List<string> values = !string.IsNullOrEmpty(fieldAttributes.ExtendedOptionForKey("GPS"))
                    ? new List<string> { this.RawValueAtIndex(configField.TabIndependentFieldIndex) }
                    : new List<string> { this.ValueAtIndex(configField.TabIndependentFieldIndex) };

                int k;
                for (k = 1; k < fieldAttributes.FieldCount; k++)
                {
                    UPConfigFieldControlField childfieldConfig = fieldControl.FieldAtIndex(index + ++innerOffset);
                    if (childfieldConfig != null)
                    {
                        string value = (!string.IsNullOrEmpty(fieldAttributes.ExtendedOptionForKey("GPS"))
                            ? this.RawValueAtIndex(childfieldConfig.TabIndependentFieldIndex)
                            : this.ValueAtIndex(childfieldConfig.TabIndependentFieldIndex)) ?? string.Empty;

                        values.Add(value);
                    }
                }

                if (!string.IsNullOrEmpty(fieldAttributes.ExtendedOptionForKey("GPS")))
                {
                    bool firstFieldLongtitude = fieldAttributes.ExtendedOptionForKey("GPS") == "X";
                    if (values.Count >= 2)
                    {
#if PORTING
                        int longtitudeNumberValue;
                        int latitudeNumberValue;
                        if (firstFieldLongtitude)
                        {
                            longtitudeNumberValue = values[0].GpsLongtitudeNumberValue();
                            latitudeNumberValue = values[1].GpsLatitudeNumberValue();
                        }
                        else
                        {
                            longtitudeNumberValue = values[1].GpsLongtitudeNumberValue();
                            latitudeNumberValue = values[0].GpsLatitudeNumberValue();
                        }

                        string longtitudeString = longtitudeNumberValue.FormatedLongtitudeValue();
                        string lattitudeString = latitudeNumberValue.FormatedLatitudeValue();
                        result = $"{lattitudeString} / {longtitudeString}";
#endif
                    }
                }
                else
                {
                    result = fieldAttributes.FormatValues(values);
                }
            }
            else
            {
                result = this.ValueAtIndex(configField.TabIndependentFieldIndex);
            }

            return result;
        }

        /// <summary>
        /// The formatted field value at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="tab">
        /// The tab.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string FormattedFieldValueAtIndex(int index, int? offset, FieldControlTab tab)
        {
            UPConfigFieldControlField field = tab.FieldAtIndex(index);
            if (field != null)
            {
                return this.FormattedFieldValueAtIndex(field.TabIndependentFieldIndex, offset, tab.FieldControl);
            }

            return null;
        }

        /// <summary>
        /// Determines whether [has record at field index] [the specified index].
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasRecordAtFieldIndex(int index)
        {
            if (this.Result?.MetaInfo == null)
            {
                return false;
            }

            var field = this.Result.MetaInfo.FieldAtPosition(index);
            return field.InfoAreaPosition <= 0 || !string.IsNullOrEmpty(this.RecordIdAtIndex(field.InfoAreaPosition));
        }

        /// <summary>
        /// Numbers the of record ids.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int NumberOfRecordIds()
        {
            return !string.IsNullOrEmpty(this.newRecordIdentification)
                       ? 1
                       : (this.databaseRow == null
                              ? this.serverRecordIds.Count
                              : this.Result.MetaInfo.NumberOfResultInfoAreaMetaInfos());
        }

        /// <summary>
        /// Physicals the index of the information area identifier at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string PhysicalInfoAreaIdAtIndex(int index)
        {
            var infoAreaMetaInfo = this.Result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(index);
            return infoAreaMetaInfo.InfoAreaId;
        }

        /// <summary>
        /// Physicals the index of the record identification at field.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string PhysicalRecordIdentificationAtFieldIndex(int index)
        {
            // Sometimes result has no meta information, prevent throwing an exception by checking if its null
            var fieldMetaInfo = this.Result.MetaInfo?.FieldAtPosition(index);
            return fieldMetaInfo != null
                       ? this.PhysicalRecordIdentificationAtIndex(fieldMetaInfo.InfoAreaPosition)
                       : null;
        }

        /// <summary>
        /// Physicals the index of the record identification at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string PhysicalRecordIdentificationAtIndex(int index)
        {
            var physicalInfoArea = this.PhysicalInfoAreaIdAtIndex(index);
            var recordId = this.RecordIdAtIndex(index);
            return !string.IsNullOrEmpty(physicalInfoArea) && !string.IsNullOrEmpty(recordId)
                       ? physicalInfoArea.InfoAreaIdRecordId(recordId)
                       : null;
        }

        /// <summary>
        /// Raws the index of the value at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RawValueAtIndex(int index)
        {
            if (!string.IsNullOrEmpty(this.newRecordIdentification))
            {
                return string.Empty;
            }

            if (this.databaseRow != null)
            {
                var row = this.databaseRow;
                var fieldMetaInfo = this.Result.MetaInfo.FieldAtPosition(index);
                if (fieldMetaInfo == null)
                {
                    return string.Empty;
                }

                var val = row.GetColumn(fieldMetaInfo.PositionInResult);
                if (val == null)
                {
                    return string.Empty;
                }

#if PORTING
                if (!fieldMetaInfo.DateTimeAdjustment || !_result.TimeZone)
                {
                    return val;
                }

                var otherVal = row.GetColumn(fieldMetaInfo.OtherDateTimeField.PositionInResult);

                string resultValue = fieldMetaInfo.IsDateField ?
                    _result.TimeZone.GetAdjustedCurrentMMDateFromCDateCTime(val, otherVal) :
                    _result.TimeZone.GetAdjustedCurrentMMTimeFromCDateCTime(otherVal, val);

                return resultValue ?? string.Empty;
#else
                return val;
#endif
            }

            if (this.IsServerResponse)
            {
                string resultValue;
                List<object> recordData;
                if (this.Result.HasMultipleOutputInfoAreas)
                {
                    var map = this.Result.ServerMap();
                    if (map.Count <= index)
                    {
                        return string.Empty;
                    }

                    var pointer = map[index];
                    recordData = this.serverData[pointer.RecordIndex] as List<object>;
                    if (recordData == null)
                    {
                        return string.Empty;
                    }

                    resultValue = recordData[pointer.FieldIndex] as string;
                }
                else
                {
                    recordData = this.serverData.Count > 0 ? this.serverData[0] as List<object> : null;
                    if (recordData != null && recordData.Count > index)
                    {
                        resultValue = recordData[index] as string;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(resultValue) && this.Result.TimeZone != null)
                {
                    var fieldMetaInfo = this.Result.MetaInfo.FieldAtPosition(index);
                    if (fieldMetaInfo?.OtherDateTimeField != null)
                    {
                        var otherVal = recordData[fieldMetaInfo.OtherDateTimeField.PositionInInfoArea];
#if PORTING
                        resultValue = fieldMetaInfo.IsDateField ? 
                            _result.TimeZone.GetAdjustedCurrentMMDateFromDateTime(resultValue, otherVal) :
                            _result.TimeZone.GetAdjustedCurrentMMTimeFromDateTime(otherVal, resultValue);

                        resultValue = otherVal as string;
#endif
                    }

                    return resultValue ?? string.Empty;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Raws the value for field.
        /// </summary>
        /// <param name="fieldMetaInfo">
        /// The field meta information.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RawValueForField(UPContainerFieldMetaInfo fieldMetaInfo)
        {
            if (fieldMetaInfo == null || !string.IsNullOrEmpty(this.newRecordIdentification))
            {
                return string.Empty;
            }

            if (this.databaseRow != null)
            {
                var row = this.databaseRow;
                var val = row.GetColumn(fieldMetaInfo.PositionInResult);
                return val ?? string.Empty;
            }

            if (this.IsServerResponse)
            {
                if (this.Result.HasMultipleOutputInfoAreas)
                {
                    var pointer = this.Result.ServerMap()[fieldMetaInfo.PositionInServerResultMap];
                    var recordData = this.serverData[pointer.RecordIndex];
                    return recordData is List<object>
                               ? ((List<object>)recordData)[pointer.FieldIndex] as string
                               : string.Empty;
                }
                else
                {
                    var recordData = this.serverData.Count > 0 ? this.serverData[0] as List<object> : null;
                    return recordData != null && recordData.Count > fieldMetaInfo.PositionInResult
                               ? recordData[fieldMetaInfo.PositionInResult] as string
                               : string.Empty;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Raws the value for field identifier information area identifier link identifier.
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
        public string RawValueForFieldIdInfoAreaIdLinkId(int fieldId, string infoAreaId, int linkId)
        {
            var field = this.Result.FieldForFieldIdInfoAreaIdLinkId(fieldId, infoAreaId, linkId);
            return field != null ? this.RawValueForField(field) : null;
        }

        /// <summary>
        /// Raws the values.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> RawValues()
        {
            var count = this.NumberOfColumns;
            var valueArray = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                valueArray.Add(this.RawValueAtIndex(i));
            }

            return valueArray;
        }

        /// <summary>
        /// Records the index of the identifier at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdAtIndex(int index)
        {
            if (!string.IsNullOrEmpty(this.newRecordIdentification))
            {
                return index > 0 ? null : this.newRecordIdentification;
            }

            if (this.databaseRow == null)
            {
                if (this.serverRecordIds == null || this.serverRecordIds.Count <= index)
                {
                    return null;
                }

                var recordIdentification = this.serverRecordIds[index];
                return recordIdentification?.RecordId();
            }

            var infoAreaMetaInfo = this.Result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(index);
            if (infoAreaMetaInfo == null || !infoAreaMetaInfo.HasRecordId)
            {
                return null;
            }

            var colNr = infoAreaMetaInfo.RecordIdColumnIndex;
            var row = this.databaseRow;
            return row.GetColumn(colNr);
        }

        /// <summary>
        /// Records the index of the identification at field.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdentificationAtFieldIndex(int fieldIndex)
        {
            var fieldMetaInfo = this.Result.MetaInfo.FieldAtPosition(fieldIndex);
            return fieldMetaInfo != null ? this.RecordIdentificationAtIndex(fieldMetaInfo.InfoAreaPosition) : null;
        }

        /// <summary>
        /// Records the index of the identification at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdentificationAtIndex(int index)
        {
            if (!string.IsNullOrEmpty(this.newRecordIdentification))
            {
                return index == 0 ? this.newRecordIdentification : null;
            }

            if (this.databaseRow == null && this.serverRecordIds != null && this.serverRecordIds.Count > index)
            {
                string res = this.serverRecordIds[index];
                return res?.Length > 6 ? res : null;
            }

            var recordId = this.RecordIdAtIndex(index);
            if (string.IsNullOrEmpty(recordId))
            {
                return null;
            }

            var infoAreaMetaInfo = this.Result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(index);
            return infoAreaMetaInfo.InfoAreaId.InfoAreaIdRecordId(recordId);
        }

        /// <summary>
        /// Records the identification for link information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdentificationForLinkInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            var position = this.Result.MetaInfo.IndexOfResultInfoAreaIdLinkId(infoAreaId, linkId);
            if (position >= 0)
            {
                return this.RecordIdentificationAtIndex(position);
            }

            var rootInfoAreaId = this.PhysicalInfoAreaIdAtIndex(0);
            var tableInfo = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(rootInfoAreaId);
            var linkInfo = tableInfo.LinkInfoForTargetInfoAreaIdLinkId(infoAreaId, linkId);
            if (linkInfo == null)
            {
                return null;
            }

            if (linkInfo.IsFieldLink && linkInfo.LinkFieldArray?.Count == 1)
            {
                var arNoString = this.RawValueForFieldIdInfoAreaIdLinkId(
                    linkInfo.FirstField.FieldId,
                    linkInfo.InfoAreaId,
                    -1);
                if (string.IsNullOrEmpty(arNoString))
                {
                    return null;
                }

                var crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), linkInfo.TargetInfoAreaId);
                var infoAreaMetaInfo = crmQuery.RootInfoAreaMetaInfo;
                infoAreaMetaInfo.Condition = new UPInfoAreaConditionLeaf(
                    linkInfo.TargetInfoAreaId,
                    linkInfo.FirstField.TargetFieldId,
                    "=",
                    arNoString);
                var res = crmQuery.Find();
                if (res.RowCount > 0)
                {
                    return res.ResultRowAtIndex(0).RootRecordIdentification;
                }
            }
            else if (linkInfo.HasColumn && !this.IsNewRow
                     && !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Disable.82339"))
            {
                var crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), rootInfoAreaId);
                crmQuery.AddCrmFields(
                    new List<UPCRMField>
                        {
                            new UPCRMLinkField(linkInfo.TargetInfoAreaId, linkInfo.LinkId, rootInfoAreaId)
                        });
                crmQuery.SetLinkRecordIdentification(this.RootRecordIdentification);
                var res = crmQuery.Find();
                if (res.RowCount != 1)
                {
                    return null;
                }

                var linkRecordId = res.ResultRowAtIndex(0).RawValueAtIndex(0);
                if (!string.IsNullOrEmpty(linkRecordId))
                {
                    return linkInfo.TargetInfoAreaId.InfoAreaIdRecordId(linkRecordId);
                }
            }

            SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"RecordSelector: could not determine linkRecord for infoArea:{infoAreaId} linkId:{linkId}");
            return null;
        }

        /// <summary>
        /// Records the identifier for link information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdForLinkInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            var infoAreaMetaInfo = this.Result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(
                    this.Result.MetaInfo.IndexOfResultInfoAreaIdLinkId(infoAreaId, linkId));
            if (!infoAreaMetaInfo.HasRecordId)
            {
                return null;
            }

            var colNr = infoAreaMetaInfo.RecordIdColumnIndex;
            var row = this.databaseRow;
            return row?.GetColumn(colNr);
        }

        /// <summary>
        /// Reloads the row.
        /// </summary>
        /// <returns>
        /// The <see cref="UPCRMResult"/>.
        /// </returns>
        public UPCRMResult ReloadRow()
        {
            var metaInfo = this.Result?.MetaInfo;
            return metaInfo?.ReadRecord(this.RootRecordId);
        }

        /// <summary>
        /// Reports the index of the value at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ReportValueAtIndex(int index)
        {
            var rawValue = this.RawValueAtIndex(index);
            var value = this.Result.MetaInfo.ReportValueFromRawValueForColumn(rawValue, index);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void Serialize(UPSerializer writer)
        {
            int count = this.NumberOfRecordIds();
            writer.WriteElementStart("Row");
            writer.WriteAttributeValue("record", this.RecordIdentificationAtIndex(0));
            var serializationInfo = this.Result?.SerializationInfo;
            for (int i = 1; i < count; i++)
            {
                var recordIdentification = this.RecordIdentificationAtIndex(i);
                if (!string.IsNullOrEmpty(recordIdentification))
                {
                    writer.WriteAttributeValue(serializationInfo?.RecordIdAttributeAtIndex(i), recordIdentification);
                }
            }

            count = serializationInfo?.NumberOfColumns ?? 0;
            for (int i = 0; i < count; i++)
            {
                var value = this.ReportValueAtIndex(i);
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var columnInfo = serializationInfo?.ColumnInfoAtIndex(i);
                var attributeDictionary = columnInfo?.HasAttributes ?? false ? new Dictionary<string, string>(3) : null;
                var functionName = columnInfo?.FunctionName;
                if (string.IsNullOrEmpty(functionName))
                {
                    attributeDictionary.SetObjectForKey(columnInfo?.IndexAttributeName, "idx");
                }

                if (columnInfo?.ReportRawValue ?? false)
                {
                    var rawValue = this.RawValueAtIndex(i);
                    attributeDictionary.SetObjectForKey(rawValue, "value");
                    if (columnInfo.ReportExternalKey)
                    {
                        var extKeyValue = columnInfo.ExternalKeyForRawValue(rawValue);
                        if (!string.IsNullOrEmpty(extKeyValue))
                        {
                            attributeDictionary.SetObjectForKey(extKeyValue, "extKey");
                        }
                    }
                }

                writer.WriteElementValueAttributes(
                    !string.IsNullOrEmpty(functionName) ? functionName : "Value",
                    value,
                    attributeDictionary);
            }

            writer.WriteElementEnd();
        }

        /// <summary>
        /// Sets the has local copy.
        /// </summary>
        /// <param name="hasLocal">
        /// if set to <c>true</c> [has local].
        /// </param>
        public void SetHasLocalCopy(bool hasLocal)
        {
            this.hasLocalCopy = hasLocal;
        }

        /// <summary>
        /// Shorts the index of the value at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ShortValueAtIndex(int index)
        {
            var rawValue = this.RawValueAtIndex(index);
            var value = this.Result.MetaInfo.ShortValueFromRawValueForColumn(rawValue, index);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = new StringBuilder($"{this.RootRecordIdentification}:");
            for (var j = 0; j < this.Result.ColumnCount; j++)
            {
                str.Append($",{this.RawValueAtIndex(j)}");
            }

            return str.ToString();
        }

        /// <summary>
        /// Values at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueAtIndex(int index)
        {
            if (!this.HasRecordAtFieldIndex(index))
            {
                return string.Empty;
            }

            var rawValue = this.RawValueAtIndex(index);
            var value = this.Result.MetaInfo.ValueFromRawValueForColumn(rawValue, index);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Values at index options.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueAtIndex(int index, UPFormatOption options)
        {
            var rawValue = this.RawValueAtIndex(index);
            var value = this.Result.MetaInfo.ValueFromRawValueForColumnOptions(rawValue, index, options);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Values for field.
        /// </summary>
        /// <param name="fieldMetaInfo">
        /// The field meta information.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ValueForField(UPContainerFieldMetaInfo fieldMetaInfo)
        {
            var rawValue = this.RawValueForField(fieldMetaInfo);
            var value = fieldMetaInfo.ValueFromRawValue(rawValue);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Values for field identifier information area identifier link identifier.
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
        public string ValueForFieldIdInfoAreaIdLinkId(int fieldId, string infoAreaId, int linkId)
        {
            var field = this.Result.FieldForFieldIdInfoAreaIdLinkId(fieldId, infoAreaId, linkId);
            return field != null ? this.ValueForField(field) : null;
        }

        /// <summary>
        /// Valueses this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> Values()
        {
            var count = this.NumberOfColumns;
            var valueArray = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                valueArray.Add(this.ValueAtIndex(i));
            }

            return valueArray;
        }

        /// <summary>
        /// Valueses the with functions.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ValuesWithFunctions()
        {
            return this.Result?.MetaInfo?.SourceFieldControl?.FunctionNames(this);
        }

        /// <summary>
        /// Valueses the with functions.
        /// </summary>
        /// <param name="includeDisplayValues">
        /// if set to <c>true</c> [include display values].
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, object> ValuesWithFunctions(bool includeDisplayValues)
        {
            return this.Result.MetaInfo.SourceFieldControl.FunctionNames(this, "Text_");
        }

        /// <summary>
        /// Virtuals the index of the information area identifier at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string VirtualInfoAreaIdAtIndex(int index)
        {
            if (!string.IsNullOrEmpty(this.newRecordIdentification))
            {
                return this.newRecordIdentification.InfoAreaId();
            }

            var infoAreaMetaInfo = this.Result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(index);
            if (this.databaseRow == null)
            {
                var recordIdentification = this.RecordIdentificationAtIndex(index);
                return !string.IsNullOrEmpty(recordIdentification)
                           ? recordIdentification.InfoAreaId()
                           : infoAreaMetaInfo.InfoAreaId;
            }

            if (infoAreaMetaInfo.HasInfoAreaIdColumn)
            {
                var row = this.databaseRow;
                var cString = row.GetColumn(infoAreaMetaInfo.InfoAreaIdColumnIndex);
                return !string.IsNullOrEmpty(cString) ? cString : infoAreaMetaInfo.InfoAreaId;
            }

            return infoAreaMetaInfo.InfoAreaId;
        }
    }
}
