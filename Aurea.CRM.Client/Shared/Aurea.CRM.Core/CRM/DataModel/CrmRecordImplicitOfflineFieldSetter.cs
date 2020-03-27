// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordImplicitOfflineFieldSetter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//   The Record Implicit Offline Field Setter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// The Record Implicit Offline Field Setter
    /// </summary>
    public class UPCRMRecordImplicitOfflineFieldSetter
    {
        /// <summary>
        /// The information area identifier
        /// </summary>
        private string infoAreaId;

        /// <summary>
        /// The data store
        /// </summary>
        private ICRMDataStore dataStore;

        /// <summary>
        /// The table information
        /// </summary>
        private UPCRMTableInfo tableInfo;

        /// <summary>
        /// The offline station number
        /// </summary>
        private int offlineStationNumber;

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordImplicitOfflineFieldSetter"/> class.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="_offlineStationNumber">The offline station number.</param>
        public UPCRMRecordImplicitOfflineFieldSetter(UPCRMRecord record, int _offlineStationNumber)
        {
            this.Record = record;
            this.infoAreaId = this.Record.InfoAreaId;
            this.dataStore = UPCRMDataStore.DefaultStore;
            this.tableInfo = this.dataStore.TableInfoForInfoArea(this.infoAreaId);
            this.offlineStationNumber = _offlineStationNumber;
        }

        /// <summary>
        /// Sources the field result row for link.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns></returns>
        List<string> SourceFieldResultRowForLink(UPCRMLink link)
        {
            UPCRMTableInfo sourceTableInfo = this.dataStore.TableInfoForInfoArea(link.InfoAreaId);
            UPCRMLinkInfo linkInfo = this.tableInfo.LinkInfoForTargetInfoAreaIdLinkId(link.InfoAreaId, link.LinkId);
            bool noFields = true;
            if (sourceTableInfo == null || linkInfo?.LinkFieldArray == null)
            {
                return null;
            }

            List<UPCRMFieldSetterField> fieldMap = new List<UPCRMFieldSetterField>(linkInfo.LinkFieldArray.Count);
            foreach (UPCRMLinkInfoField field in linkInfo.LinkFieldArray)
            {
                UPCRMFieldInfo fieldInfo = sourceTableInfo.FieldInfoForFieldId(field.TargetFieldId);
                if (fieldInfo != null)
                {
                    fieldMap.Add(new UPCRMFieldSetterSourceField(field.TargetFieldId));
                    noFields = false;
                }
                else
                {
                    UPCRMFieldSetterSourceLink sourceLink = null;
                    List<UPCRMLinkInfo> allLinks = sourceTableInfo.LinksWithField();
                    foreach (UPCRMLinkInfo currentLinkInfo in allLinks)
                    {
                        foreach (UPCRMLinkInfoField linkInfoField in currentLinkInfo.LinkFieldArray)
                        {
                            if (linkInfoField.FieldId == field.TargetFieldId)
                            {
                                UPCRMTableInfo parentTableInfo = this.dataStore.TableInfoForInfoArea(currentLinkInfo.TargetInfoAreaId);
                                UPCRMLinkInfo parentIdentLink = parentTableInfo.IdentLink;
                                if (parentIdentLink != null)
                                {
                                    if (parentIdentLink.FirstField.TargetFieldId == linkInfoField.TargetFieldId)
                                    {
                                        sourceLink = new UPCRMFieldSetterSourceLink(currentLinkInfo.TargetInfoAreaId, currentLinkInfo.LinkId, 0);
                                        break;
                                    }

                                    if (parentIdentLink.SecondField.TargetFieldId == linkInfoField.TargetFieldId)
                                    {
                                        sourceLink = new UPCRMFieldSetterSourceLink(currentLinkInfo.TargetInfoAreaId, currentLinkInfo.LinkId, 1);
                                        break;
                                    }
                                }
                            }
                        }

                        if (sourceLink != null)
                        {
                            break;
                        }
                    }

                    if (sourceLink != null)
                    {
                        fieldMap.Add(sourceLink);
                        noFields = false;
                    }
                    else
                    {
                        fieldMap.Add(new UPCRMFieldSetterField());
                    }
                }
            }

            if (noFields)
            {
                return null;
            }

            List<UPCRMField> queryFields = new List<UPCRMField>();
            int resultPosition = 0;
            foreach (UPCRMFieldSetterField fieldSetterField in fieldMap)
            {
                if (fieldSetterField.IsField)
                {
                    queryFields.Add(fieldSetterField.FieldWithInfoAreaId(link.InfoAreaId));
                    fieldSetterField.ResultPosition = resultPosition++;
                }
            }

            foreach (UPCRMFieldSetterField fieldSetterField in fieldMap)
            {
                if (fieldSetterField.IsLink)
                {
                    queryFields.Add(fieldSetterField.FieldWithInfoAreaId(link.InfoAreaId));
                    fieldSetterField.ResultPosition = resultPosition++;
                }
            }

            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(queryFields, linkInfo.TargetInfoAreaId);
            UPCRMResult result = crmQuery.ReadRecord(link.RecordIdentification);
            if (result != null && result.RowCount == 1)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
                List<string> resultArray = new List<string>(fieldMap.Count);
                foreach (UPCRMFieldSetterField field in fieldMap)
                {
                    if (field.IsField)
                    {
                        resultArray.Add(row.RawValueAtIndex(field.ResultPosition));
                    }
                    else if (field.IsLink)
                    {
                        string recordId = row.RawValueAtIndex(field.ResultPosition);
                        if (string.IsNullOrEmpty(recordId))
                        {
                            resultArray.Add(string.Empty);
                        }
                        else if (recordId.StartsWith("new") && recordId.Length == 15)
                        {
                            long offlineLnr = (Convert.ToInt64(recordId.Substring(3, 8), 16) << 16) + Convert.ToInt64(recordId.Substring(11, 4), 16);
                            if (this.offlineStationNumber > 0 && offlineLnr > 0)
                            {
                                if (((UPCRMFieldSetterSourceLink)field).FieldIndex == 0)
                                {
                                    resultArray.Add(this.offlineStationNumber.ToString());
                                }
                                else
                                {
                                    resultArray.Add(offlineLnr.ToString());
                                }
                            }
                        }
                        else if (((UPCRMFieldSetterSourceLink)field).FieldIndex == 0)
                        {
                            resultArray.Add(recordId.StatNoFromRecordIdString());
                        }
                        else
                        {
                            resultArray.Add(recordId.RecordNoFromRecordIdString());
                        }
                    }
                    else
                    {
                        resultArray.Add(string.Empty);
                    }
                }

                return resultArray;
            }

            return null;
        }

        /// <summary>
        /// Sets the implicit fields.
        /// </summary>
        /// <param name="onlyOffline">if set to <c>true</c> [only offline].</param>
        public void SetImplicitFields(bool onlyOffline)
        {
            Dictionary<int, string> rawValuesForUnknownFields = null;
            if (this.Record.Links != null)
            {
                foreach (var link in this.Record.Links)
                {
                    var linkInfo =
                        this.tableInfo.LinkInfoForTargetInfoAreaIdLinkId(link.PhysicalInfoAreaId, link.LinkId);
                    if (linkInfo != null && !linkInfo.IsGeneric && linkInfo.LinkFieldArray != null)
                    {
                        var linkInitialized = false;
                        var sourceResultInitialized = false;
                        List<string> sourceResult = null;
                        var firstField = 0;
                        var secondField = 0;
                        var count = linkInfo.LinkFieldArray.Count;
                        for (var index = 0; index < count; index++)
                        {
                            var linkInfoField = linkInfo.LinkFieldArray[index];
                            if (linkInfoField.FieldId >= 0)
                            {
                                var fieldInfo =
                                    this.CreateFieldInfo(linkInfoField, link, ref linkInitialized, ref firstField, ref secondField);

                                string sourceValue;
                                if (linkInfoField.TargetFieldId == firstField)
                                {
                                    sourceValue = this.ProcessFirstField(link);
                                }
                                else if (linkInfoField.TargetFieldId == secondField)
                                {
                                    sourceValue = this.ProcessSecondField(link);
                                }
                                else if (linkInfoField.TargetFieldId < 0)
                                {
                                    sourceValue = linkInfoField.TargetValue ?? string.Empty;
                                }
                                else
                                {
                                    sourceValue =
                                        this.ProcessDefaultField(link, index, ref sourceResultInitialized, ref sourceResult);
                                }

                                rawValuesForUnknownFields = this.ApplyFieldInfo(
                                    onlyOffline,
                                    fieldInfo,
                                    sourceValue,
                                    rawValuesForUnknownFields,
                                    linkInfoField);
                            }
                        }
                    }
                }
            }

            this.SetAllPhysicalFields(onlyOffline, rawValuesForUnknownFields);
        }

        private string ProcessDefaultField(
            UPCRMLink link,
            int index,
            ref bool sourceResultInitialized,
            ref List<string> sourceResult)
        {
            if (!sourceResultInitialized)
            {
                sourceResult = this.SourceFieldResultRowForLink(link);
                sourceResultInitialized = true;
            }

            var sourceValue = sourceResult != null && sourceResult.Count > index
                ? sourceResult[index]
                : string.Empty;

            return sourceValue;
        }

        private string ProcessFirstField(UPCRMLink link)
        {
            var sourceValue = this.offlineStationNumber > 0 &&
                                 link.RecordId.StartsWith("new") &&
                                 link.RecordId.Length == 15
                ? this.offlineStationNumber.ToString()
                : link?.RecordId.StatNoFromRecordIdString();

            return sourceValue;
        }

        private string ProcessSecondField(UPCRMLink link)
        {
            string sourceValue;
            if (this.offlineStationNumber > 0 && link.RecordId.StartsWith("new") &&
                link.RecordId.Length == 15)
            {
                var highPart = Convert.ToInt64(link.RecordId.Substring(3, 8), 16) << 16;
                var lowPart = Convert.ToInt64(link.RecordId.Substring(11, 4), 16);
                var valueNumber = highPart + lowPart;
                sourceValue = valueNumber.ToString();
            }
            else
            {
                sourceValue = link.RecordId.RecordNoFromRecordIdString();
            }

            return sourceValue;
        }

        private UPCRMFieldInfo CreateFieldInfo(
            UPCRMLinkInfoField linkInfoField,
            UPCRMLink link,
            ref bool linkInitialized,
            ref int firstField,
            ref int secondField)
        {
            var fieldInfo = this.tableInfo.FieldInfoForFieldId(linkInfoField.FieldId);
            if (!linkInitialized)
            {
                var targetTableInfo = this.dataStore.TableInfoForInfoArea(link.InfoAreaId);
                var targetIdentLink = targetTableInfo.IdentLink;
                firstField = targetIdentLink?.FirstField.TargetFieldId ?? -1;
                secondField = targetIdentLink?.SecondField.TargetFieldId ?? -1;
                linkInitialized = true;
            }

            return fieldInfo;
        }

        private Dictionary<int, string> ApplyFieldInfo(
            bool onlyOffline,
            UPCRMFieldInfo fieldInfo,
            string sourceValue,
            Dictionary<int, string> rawValuesForUnknownFields,
            UPCRMLinkInfoField linkInfoField)
        {
            if (fieldInfo == null && !string.IsNullOrWhiteSpace(sourceValue))
            {
                if (rawValuesForUnknownFields == null)
                {
                    rawValuesForUnknownFields = new Dictionary<int, string>
                    {
                        { linkInfoField.FieldId, sourceValue }
                    };
                }
                else
                {
                    rawValuesForUnknownFields[linkInfoField.FieldId] = sourceValue;
                }
            }
            else if (!string.IsNullOrEmpty(sourceValue))
            {
                var fieldValue = this.Record.FieldValueForFieldIndex(linkInfoField.FieldId);
                if (fieldValue == null)
                {
                    this.Record.AddValue(
                        new UPCRMFieldValue(sourceValue, this.infoAreaId, linkInfoField.FieldId, onlyOffline));
                }
                else if (fieldInfo.IsEmptyValue(fieldValue.Value))
                {
                    fieldValue.ChangeValueOldValue(sourceValue, null);
                }
            }

            return rawValuesForUnknownFields;
        }

        private void SetAllPhysicalFields(bool onlyOffline, Dictionary<int, string> rawValuesForUnknownFields)
        {
            var allPhysicalLinks = this.tableInfo.LinksWithField();
            if (allPhysicalLinks == null)
            {
                return;
            }

            foreach (var linkInfo in allPhysicalLinks)
            {
                if (linkInfo.IsGeneric || linkInfo.IsFieldLink || this.Record.LinkForLinkInfo(linkInfo) != null)
                {
                    continue;
                }

                var targetIdentLink = UPCRMDataStore.DefaultStore.IdentLinkForInfoArea(linkInfo.TargetInfoAreaId);
                if (targetIdentLink.LinkFieldArray.Count == 2)
                {
                    var field1 =
                        linkInfo.LinkInfoFieldWithTargetFieldIndex(targetIdentLink.FirstField.TargetFieldId);
                    var field2 =
                        linkInfo.LinkInfoFieldWithTargetFieldIndex(targetIdentLink.SecondField.TargetFieldId);
                    if (field1 != null && field2 != null)
                    {
                        long intStatNo = -1;
                        var intRecordNo = -1L;
                        var fieldValue = this.Record.FieldValueForFieldIndex(field1.FieldId);
                        if (fieldValue != null)
                        {
                            intStatNo = Convert.ToInt64(fieldValue.Value);
                        }
                        else
                        {
                            var valueOrDefault = rawValuesForUnknownFields.ValueOrDefault(field1.FieldId);
                            if (valueOrDefault != null)
                            {
                                intStatNo = Convert.ToInt64(valueOrDefault);
                            }
                        }

                        if (intStatNo >= 0)
                        {
                            fieldValue = this.Record.FieldValueForFieldIndex(field2.FieldId);
                            if (fieldValue != null)
                            {
                                intRecordNo = Convert.ToInt64(fieldValue.Value);
                            }
                            else
                            {
                                var valueOrDefault = rawValuesForUnknownFields.ValueOrDefault(field2.FieldId);
                                intRecordNo = Convert.ToInt64(valueOrDefault);
                            }
                        }

                        this.FillLinkInfo(onlyOffline, intStatNo, intRecordNo, linkInfo);
                    }
                }
            }
        }

        private void FillLinkInfo(bool onlyOffline, long intStatNo, long intRecordNo, UPCRMLinkInfo linkInfo)
        {
            if (intStatNo < 0 || intRecordNo < 0)
            {
                return;
            }

            var ignore = false;
            foreach (var infoField in linkInfo.LinkFieldArray)
            {
                if (infoField.TargetFieldId < 0 && !string.IsNullOrEmpty(infoField.TargetValue))
                {
                    var fieldValue = this.Record.FieldValueForFieldIndex(infoField.FieldId);
                    if (fieldValue != null && fieldValue.Value != infoField.TargetValue)
                    {
                        ignore = true;
                        break;
                    }
                }
            }

            if (!ignore)
            {
                string linkRecordIdentification;
                if (intStatNo == this.offlineStationNumber && this.offlineStationNumber > 0)
                {
                    // Format: "%@.new%08lx%04x"
                    linkRecordIdentification =
                        $"{linkInfo.TargetInfoAreaId}.new{intRecordNo >> 16:X8)}{intRecordNo & 65535:X4)}";
                }
                else
                {
                    linkRecordIdentification = linkInfo.TargetInfoAreaId.InfoAreaIdRecordId(
                        StringExtensions.RecordIdFromStatNoRecordNo(intStatNo, intRecordNo));
                }

                if (!string.IsNullOrWhiteSpace(linkRecordIdentification))
                {
                    this.Record.AddLink(new UPCRMLink(linkRecordIdentification, linkInfo.LinkId, onlyOffline));
                }
            }
        }
    }
}
