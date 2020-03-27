// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecord.cs" company="Aurea Software Gmbh">
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
//   CRM record implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Configuration;
    using Extensions;
    using Session;
    using Sync;
    using Utilities;

    /// <summary>
    /// CRM record implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializable" />
    public class UPCRMRecord : UPSerializable
    {
        private const string LinkRecordProperty = "LinkRecord";
        private const string NewOfflineMode = "NewOffline";
        private const string NoLinkProperty = "NoLink";
        private const string OfflineProperty = "Offline";
        private const string ParentLinksKey = "ParentLinks";
        private const string ParentLinkProperty = "ParentLink";
        private const string ParentRelationWithout = "WITHOUT";
        private const string ParentUpdateMode = "ParentUpdate";
        private const string RemoveEmptyProperty = "RemoveEmpty";
        private const string SyncMode = "Sync";
        private const string YieldKeyword = "Yield";

        /// <summary>
        /// The key fields.
        /// </summary>
        private List<UPCRMField> keyFields;

        /// <summary>
        /// The original record identification.
        /// </summary>
        private string originalRecordIdentification;

        /// <summary>
        /// The record id.
        /// </summary>
        private string recordId;

        /// <summary>
        /// The record identification.
        /// </summary>
        private string recordIdentification;

        /// <summary>
        /// The time zone applied.
        /// </summary>
        private bool timeZoneApplied;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecord"/> class.
        /// </summary>
        protected UPCRMRecord()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPCRMRecord"/> is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public bool IsNew => this.recordId == null || this.Mode == "New";

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public List<UPCRMLink> Links { get; private set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets the offline record number.
        /// </summary>
        /// <value>
        /// The offline record number.
        /// </value>
        public int OfflineRecordNumber { get; set; }

        /// <summary>
        /// Gets or sets the offline station number.
        /// </summary>
        /// <value>
        /// The offline station number.
        /// </value>
        public int OfflineStationNumber { get; set; }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public string Options { get; private set; }

        /// <summary>
        /// Gets the original record identification.
        /// </summary>
        /// <value>
        /// The original record identification.
        /// </value>
        public string OriginalRecordIdentification
            => this.ReferencedRecord != null ? this.ReferencedRecord.RecordIdentification : this.originalRecordIdentification;

        /// <summary>
        /// Gets the physical information area identifier.
        /// </summary>
        /// <value>
        /// The physical information area identifier.
        /// </value>
        public string PhysicalInfoAreaId => UPCRMDataStore.DefaultStore.RootInfoAreaIdForInfoAreaId(this.InfoAreaId);

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordId
        {
            get
            {
                return this.ReferencedRecord?.RecordId ?? this.recordId;
            }

            set
            {
                this.recordId = value;
                this.recordIdentification = StringExtensions.InfoAreaIdRecordId(this.InfoAreaId, this.recordId);
            }
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification => this.ReferencedRecord?.RecordIdentification ?? this.recordIdentification;

        /// <summary>
        /// Gets the referenced record.
        /// </summary>
        /// <value>
        /// The referenced record.
        /// </value>
        public UPCRMRecord ReferencedRecord { get; private set; }

        /// <summary>
        /// Gets the single link.
        /// </summary>
        /// <value>
        /// The single link.
        /// </value>
        public UPCRMLink SingleLink => this.Links.Count == 1 ? this.Links[0] : null;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public Dictionary<string, UPCRMFieldValue> Values { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPCRMRecord"/> is yield.
        /// </summary>
        /// <value>
        /// <c>true</c> if yield; otherwise, <c>false</c>.
        /// </value>
        public bool Yield { get; set; }

        /// <summary>
        /// Arrays the removing yield records from array.
        /// </summary>
        /// <param name="recordArray">
        /// The record array.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<UPCRMRecord> ArrayRemovingYieldRecordsFromArray(List<UPCRMRecord> recordArray)
        {
            List<UPCRMRecord> yieldArray = null;
            foreach (var record in recordArray)
            {
                if (!record.Yield || record.Links.Count <= 0)
                {
                    continue;
                }

                if (yieldArray != null)
                {
                    yieldArray.Add(record);
                }
                else
                {
                    yieldArray = new List<UPCRMRecord> { record };
                }
            }

            if (yieldArray == null || yieldArray.Count == 0)
            {
                return recordArray;
            }

            List<UPCRMRecord> removeArray = null;
            foreach (var record in yieldArray)
            {
                var removeRecord = false;
                foreach (var sourceRecord in recordArray)
                {
                    if (sourceRecord.Yield)
                    {
                        continue;
                    }

                    if (sourceRecord.InfoAreaId == record.InfoAreaId && sourceRecord.MatchingFirstLink(record))
                    {
                        removeRecord = true;
                        break;
                    }
                }

                if (!removeRecord)
                {
                    continue;
                }

                if (removeArray != null)
                {
                    removeArray.Add(record);
                }
                else
                {
                    removeArray = new List<UPCRMRecord> { record };
                }
            }

            if (removeArray == null || removeArray.Count == 0)
            {
                return recordArray;
            }

            var resultArray = new List<UPCRMRecord>(recordArray.Count);
            resultArray.AddRange(recordArray.Where(record => !removeArray.Contains(record)));

            return resultArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecord"/> class.
        /// </summary>
        /// <param name="record">The record.</param>
        public UPCRMRecord(UPCRMRecord record)
        {
            var instance = new UPCRMRecord
            {
                InfoAreaId = record.InfoAreaId,
                Mode = record.Mode,
                Options = record.Options,
                originalRecordIdentification = record.OriginalRecordIdentification,
                recordId = record.RecordId,
                recordIdentification = record.RecordIdentification,
                Deleted = record.Deleted
            };

            if (record.Links?.Count > 0)
            {
                instance.Links = new List<UPCRMLink>(record.Links);
            }

            if (record.Values?.Count > 0)
            {
                instance.Values = record.Values?.ToDictionary(k => k.Key, k => k.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecord"/> class.
        /// </summary>
        /// <param name="infoAreaIdRecordId">The information area identifier record identifier.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="options">The options.</param>
        public UPCRMRecord(string infoAreaIdRecordId, string mode = null, string options = null)
        {
            string recIdentification = ServerSession.CurrentSession?.RecordIdentificationMapper?.MappedRecordIdentification(infoAreaIdRecordId);
            this.recordIdentification = recIdentification;
            this.originalRecordIdentification = recIdentification;
            this.InfoAreaId = recIdentification?.InfoAreaId();
            this.recordId = recIdentification?.RecordId();
            this.Mode = mode;
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecord"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="recordId">The record identifier.</param>
        public UPCRMRecord(string infoAreaId, string recordId)
            : this(StringExtensions.InfoAreaIdRecordId(infoAreaId, recordId))
        {
        }

        /// <summary>
        /// Creates the new.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecord"/>.
        /// </returns>
        public static UPCRMRecord CreateNew(string infoAreaId)
        {
            return new UPCRMRecord { InfoAreaId = infoAreaId, Mode = "New" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecord"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="link">The link.</param>
        public UPCRMRecord(string infoAreaId, UPCRMLink link)
        {
            this.InfoAreaId = infoAreaId;
            this.Mode = "Sync";
            this.AddLink(link);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecord"/> class.
        /// </summary>
        /// <param name="crmRecord">The CRM record.</param>
        /// <param name="mode">The mode.</param>
        public UPCRMRecord(UPCRMRecord crmRecord, string mode = "Update")
        {
            this.InfoAreaId = crmRecord.InfoAreaId;
            this.Mode = mode;
            this.ReferencedRecord = crmRecord;
        }

        /// <summary>
        /// Adds the key field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public void AddKeyField(UPCRMField field)
        {
            if (this.keyFields == null)
            {
                this.keyFields = new List<UPCRMField> { field };
            }
            else
            {
                this.keyFields.Add(field);
            }
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="link">
        /// The link.
        /// </param>
        public void AddLink(UPCRMLink link)
        {
            if (this.Links == null)
            {
                this.Links = new List<UPCRMLink>();
            }

            this.Links.Add(link);
        }

        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddValue(UPCRMFieldValue value)
        {
            if (this.Values == null)
            {
                this.Values = new Dictionary<string, UPCRMFieldValue>();
            }

            var stringFieldId = value.StringFieldId;
            var existingValue = this.Values.ValueOrDefault(stringFieldId);
            if (existingValue == null)
            {
                this.Values.SetObjectForKey(value, stringFieldId);
            }
            else
            {
                if (value.OldValue.Equals(existingValue.Value))
                {
                    var combinedValue = new UPCRMFieldValue(
                        value.Value,
                        existingValue.OldValue,
                        value.InfoAreaId,
                        value.FieldId,
                        value.OnlyOffline);
                    this.Values.SetObjectForKey(combinedValue, stringFieldId);
                }
                else
                {
                    this.Values.SetObjectForKey(value, stringFieldId);
                }
            }
        }

        /// <summary>
        /// Applies the changes fields.
        /// </summary>
        /// <param name="_recordId">
        /// The _record identifier.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<object> ApplyChangesFields(string _recordId, List<object> fields)
        {
            this.recordId = _recordId;
            this.recordIdentification = this.InfoAreaId.InfoAreaIdRecordId(_recordId);
            return null;
        }

        /// <summary>
        /// Applies the mapped record identifications skip links.
        /// </summary>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        /// <param name="skipLinks">
        /// if set to <c>true</c> [skip links].
        /// </param>
        public void ApplyMappedRecordIdentifications(UPRecordIdentificationMapper mapper = null, bool skipLinks = false)
        {
            if (mapper == null)
            {
                mapper = ServerSession.CurrentSession.RecordIdentificationMapper;
            }

            var mappedRecordIdentficiation = mapper.MappedRecordIdentification(this.recordIdentification);
            if (!string.IsNullOrEmpty(mappedRecordIdentficiation))
            {
                this.recordIdentification = mappedRecordIdentficiation;
                this.recordId = this.recordIdentification.RecordId();
            }

            if (skipLinks)
            {
                return;
            }

            foreach (var link in this.Links ?? new List<UPCRMLink>())
            {
                link.ApplyMappedRecordIdentifications(mapper);
            }
        }

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="crmDataStore"></param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ApplyTimeZone(UPCRMTimeZone timeZone, ICRMDataStore crmDataStore)
        {
            if (this.timeZoneApplied || timeZone == null || crmDataStore == null)
            {
                return false;
            }

            if (!timeZone.NeedsTimeZoneAdjustment || this.Values == null)
            {
                return true;
            }

            this.timeZoneApplied = true;
            List<UPCRMRecordDateTimeFieldValue> dateTimeValues = null;
            var tableInfo = crmDataStore.TableInfoForInfoArea(this.InfoAreaId);
            foreach (var fieldKey in this.Values.Keys)
            {
                var found = false;
                var fieldValue = this.Values.ValueOrDefault(fieldKey);
                var fieldInfo = tableInfo.FieldInfoForFieldId(fieldValue.FieldId);
                if (fieldInfo.IsDateField && fieldInfo.TimeFieldId >= 0)
                {
                    if (dateTimeValues != null)
                    {
                        foreach (var fv in dateTimeValues)
                        {
                            if (fv.DateFieldId != fieldValue.FieldId || fv.DateFieldValue != null)
                            {
                                continue;
                            }

                            fv.DateFieldValue = fieldValue;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var dfv = new UPCRMRecordDateTimeFieldValue(fieldValue.FieldId, fieldInfo.TimeFieldId)
                        {
                            DateFieldValue = fieldValue
                        };

                        if (dateTimeValues != null)
                        {
                            dateTimeValues.Add(dfv);
                        }
                        else
                        {
                            dateTimeValues = new List<UPCRMRecordDateTimeFieldValue> { dfv };
                        }
                    }
                }
                else if (fieldInfo.IsTimeField && fieldInfo.DateFieldId >= 0)
                {
                    if (dateTimeValues != null)
                    {
                        foreach (var fv in dateTimeValues)
                        {
                            if (fv.TimeFieldId != fieldValue.FieldId || fv.TimeFieldValue != null)
                            {
                                continue;
                            }

                            fv.TimeFieldValue = fieldValue;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }

                    var dfv = new UPCRMRecordDateTimeFieldValue(fieldInfo.DateFieldId, fieldInfo.FieldId)
                    {
                        TimeFieldValue = fieldValue
                    };

                    if (dateTimeValues != null)
                    {
                        dateTimeValues.Add(dfv);
                    }
                    else
                    {
                        dateTimeValues = new List<UPCRMRecordDateTimeFieldValue> { dfv };
                    }
                }
            }

            if (dateTimeValues != null)
            {
                foreach (var rfv in dateTimeValues)
                {
                    rfv.ApplyTimeZoneRecord(timeZone, this);
                }
            }

            return false;
        }

        /// <summary>
        /// Applies the values from template filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        public void ApplyValuesFromTemplateFilter(UPConfigFilter filter)
        {
            this.ApplyValuesFromTemplateFilter(filter, true);
        }

        /// <summary>
        /// Applies the values from template filter ignore child records.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="ignoreChildRecords">
        /// if set to <c>true</c> [ignore child records].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMRecord> ApplyValuesFromTemplateFilter(UPConfigFilter filter, bool ignoreChildRecords)
        {
            return this.ApplyValuesFromTemplateFilter(filter?.RootTable, ignoreChildRecords);
        }

        /// <summary>
        /// Applies the values from template filter table ignore child records.
        /// </summary>
        /// <param name="filterTable">
        /// The filter table.
        /// </param>
        /// <param name="ignoreChildRecords">
        /// if set to <c>true</c> [ignore child records].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMRecord> ApplyValuesFromTemplateFilter(UPConfigQueryTable filterTable, bool ignoreChildRecords)
        {
            if (filterTable == null)
            {
                return null;
            }

            var prefix = $"{this.InfoAreaId}.";
            var prefixLength = prefix.Length;
            var initialValueDictionary = filterTable.FieldsWithValues(true);
            foreach (var key in initialValueDictionary.Keys)
            {
                if (key.StartsWith(prefix))
                {
                    var value = initialValueDictionary.ValueOrDefault(key) as string;
                    if (value != "*")
                    {
                        var fieldId = int.Parse(key.Substring(prefixLength));
                        this.NewValueFieldId(value, fieldId);
                    }
                }
            }

            var keyFieldArray = initialValueDictionary.ValueOrDefault("KeyField") as List<object>;
            if (keyFieldArray != null)
            {
                foreach (string keyField in keyFieldArray)
                {
                    var parts = keyField.Split('.');
                    if (parts.Length == 2)
                    {
                        var fieldId = int.Parse(parts[1]);
                        this.AddKeyField(UPCRMField.FieldWithFieldIdInfoAreaId(fieldId, this.InfoAreaId));
                    }
                }
            }

            if (ignoreChildRecords)
            {
                return null;
            }

            List<UPCRMRecord> childRecords = null;
            int count = filterTable.NumberOfSubTables;
            for (int i = 0; i < count; i++)
            {
                var subTable = filterTable.SubTableAtIndex(i);
                if (subTable.InfoAreaId.Equals(this.InfoAreaId) && subTable.LinkId <= 0
                    && subTable.ParentRelation == "HAVING")
                {
                    continue;
                }

                var childRecordsForQueryTable = this.ChildRecordsForQueryTableRootRecord(subTable, this);
                if (childRecordsForQueryTable != null && childRecordsForQueryTable.Any())
                {
                    if (childRecords == null)
                    {
                        childRecords = new List<UPCRMRecord>(childRecordsForQueryTable);
                    }
                    else
                    {
                        childRecords.AddRange(childRecordsForQueryTable);
                    }
                }
            }

            return childRecords;
        }

        /// <summary>
        /// Arrays for property conditions.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<object> ArrayForPropertyConditions(
            string property,
            Dictionary<string, UPConfigQueryCondition> propertyConditions)
        {
            var cond = propertyConditions.ValueOrDefault(property);
            if (cond == null || cond.NumberOfValues < 1)
            {
                return null;
            }

            return cond.FieldValues;
        }

        /// <summary>
        /// Bools for property conditions default value.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        /// <param name="defaultValue">
        /// if set to <c>true</c> [default value].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool BoolForPropertyConditionsDefaultValue(
            string property,
            Dictionary<string, UPConfigQueryCondition> propertyConditions,
            bool defaultValue)
        {
            var v = this.StringForPropertyConditions(property, propertyConditions);
            if (string.IsNullOrEmpty(v))
            {
                return defaultValue;
            }

            return defaultValue ? v == "true" : v != "false";
        }

        /// <summary>
        /// Childs the records for query table root record.
        /// </summary>
        /// <param name="queryTable">
        /// The query table.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// List of <see cref="UPCRMRecord"/>.
        /// </returns>
        public List<UPCRMRecord> ChildRecordsForQueryTableRootRecord(UPConfigQueryTable queryTable, UPCRMRecord record)
        {
            if (queryTable.ParentRelation == ParentRelationWithout)
            {
                return null;
            }

            var linkRecord = StringForPropertyConditions(LinkRecordProperty, queryTable.PropertyConditions) != null;
            var sync = BoolForPropertyConditionsDefaultValue(SyncMode, queryTable.PropertyConditions, false);
            var onlyOffline = !sync
                              && BoolForPropertyConditionsDefaultValue(
                                  OfflineProperty,
                                  queryTable.PropertyConditions,
                                  false);
            var childRecord = CreateNew(queryTable.InfoAreaId);

            if (linkRecord)
            {
                childRecord.Mode = ParentUpdateMode;
            }
            else if (sync)
            {
                childRecord.Mode = SyncMode;
            }
            else if (onlyOffline)
            {
                childRecord.Mode = NewOfflineMode;
            }

            if (!BoolForPropertyConditionsDefaultValue(NoLinkProperty, queryTable.PropertyConditions, false))
            {
                childRecord.AddLink(new UPCRMLink(record, queryTable.LinkId));
            }

            if (!sync)
            {
                childRecord.ApplyValuesFromTemplateFilter(queryTable, true);
                if ((childRecord.FieldValues == null || !childRecord.FieldValues.Any())
                    && queryTable.PropertyConditions.ValueOrDefault(RemoveEmptyProperty) != null)
                {
                    return null;
                }

                if (childRecord.Links.Any())
                {
                    var yield = BoolForPropertyConditionsDefaultValue(
                        YieldKeyword,
                        queryTable.PropertyConditions,
                        false);
                    if (yield)
                    {
                        childRecord.Yield = true;
                    }
                }

                childRecord = UPCRMRecordAddLinks(queryTable, childRecord, record);
            }

            return PopulateChildRecordArray(queryTable, childRecord);
        }

        /// <summary>
        /// Clears the links.
        /// </summary>
        public void ClearLinks()
        {
            this.Links = null;
        }

        /// <summary>
        /// Clears the values.
        /// </summary>
        public void ClearValues()
        {
            this.Values = null;
        }

        /// <summary>
        /// Creates the record identification from request nr record nr.
        /// </summary>
        /// <param name="requestNr">
        /// The request nr.
        /// </param>
        /// <param name="recordNr">
        /// The record nr.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateRecordIdentificationFromRequestNrRecordNr(int requestNr, int recordNr)
        {
            if (!string.IsNullOrEmpty(this.Mode) && !this.Mode.StartsWith("New"))
            {
                return this.recordIdentification;
            }

            var _recordId = this.RecordId;
            if (_recordId == null || _recordId.StartsWith("newid:") || _recordId == "new")
            {
                _recordId = $"new{requestNr:D8}{recordNr:D4}";
                this.OfflineRecordNumber = ((requestNr & 65535) << 16) + recordNr;
                this.OfflineStationNumber = Convert.ToInt32(ConfigurationUnitStore.DefaultStore.ConfigValue("System.OfflineStationNumber"));
                this.recordId = _recordId;
            }

            return this.RecordIdentification;
        }

        /// <summary>
        /// Enables the key fields.
        /// </summary>
        public void EnableKeyFields()
        {
            if (this.keyFields == null)
            {
                this.keyFields = new List<UPCRMField>();
            }
        }

        /// <summary>
        /// Fields the index of the value for field.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public UPCRMFieldValue FieldValueForFieldIndex(int fieldIndex)
        {
            var stringFieldId = $"{fieldIndex}";
            return this.Values?.ValueOrDefault(stringFieldId);
        }

        /// <summary>
        /// Gets the Fields values.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMFieldValue> FieldValues => this.Values?.Values.ToList();

        /// <summary>
        /// Handles the change record local request nr.
        /// </summary>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        /// <param name="requestNr">
        /// The request nr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int HandleChangeRecordLocalRequestNr(ICRMDataStore dataStore, int requestNr)
        {
            if (this.Mode == null) this.Mode = "Update";
            if (string.IsNullOrEmpty(this.Mode) || this.Mode.Equals("Sync")
                || string.IsNullOrEmpty(this.RecordIdentification))
            {
                return 0;
            }

            var range = this.RecordIdentification.IndexOf("new");
            if (!this.Mode.StartsWith("New") && range > 0 && !dataStore.RecordExistsOffline(this.RecordIdentification))
            {
                return 0;
            }

            if (this.IsNew)
            {
                if (!dataStore.HasOfflineData(this.RecordIdentification.InfoAreaId()))
                {
                    return 0;
                }
            }

            if ((requestNr >= 0 && string.IsNullOrEmpty(this.originalRecordIdentification))
                || (this.originalRecordIdentification.IndexOf("new") > -1 && this.originalRecordIdentification.RecordId().Length < 6))
            {
                this.originalRecordIdentification = this.recordIdentification;
            }

            if (this.Mode == "Delete" || this.Deleted)
            {
                return dataStore.DeleteRecordWithIdentificationRollbackRequestNr(this.RecordIdentification, requestNr);
            }

            return dataStore.SaveRecordRollbackRequestNr(this, requestNr);
        }

        /// <summary>
        /// Handles the change record response.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HandleChangeRecordResponse(List<object> response)
        {
            if (response == null || response.Count < 1)
            {
                return false;
            }

            bool isNew = this.IsNew;

            this.recordId = (string)response[1];
            this.recordIdentification = this.InfoAreaId.InfoAreaIdRecordId(this.recordId);
            if (this.Deleted)
            {
                UPCRMDataStore.DefaultStore.DeleteRecordWithIdentification(this.recordIdentification);
                return true;
            }

            var storeLocally = isNew
                                   ? UPCRMDataStore.DefaultStore.HasOfflineData(this.recordIdentification.InfoAreaId())
                                   : (!string.IsNullOrEmpty(this.Mode) && this.Mode != "Update")
                                     || UPCRMDataStore.DefaultStore.RecordExistsOffline(this.recordIdentification);

            if (!storeLocally)
            {
                return true;
            }

            var syncInfo = response[3] as List<object>;
            if (syncInfo != null)
            {
                UPCRMRecordSync.SyncRecordSetDefinitions(syncInfo);
            }
            else
            {
                UPCRMDataStore.DefaultStore.DeleteRecordWithIdentification(this.recordIdentification);
            }

            return true;
        }

        /// <summary>
        /// Handles the undo change record local request nr.
        /// </summary>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        /// <param name="requestNr">
        /// The request nr.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int HandleUndoChangeRecordLocalRequestNr(ICRMDataStore dataStore, int requestNr)
        {
            if (this.Mode == "New")
            {
                return dataStore?.DeleteRecordWithIdentification(this.RecordIdentification) ?? 0;
            }

            if (string.IsNullOrEmpty(this.Mode) || this.Mode == "Update")
            {
                return dataStore?.UndoRecord(this) ?? 0;
            }

            return 0;
        }

        /// <summary>
        /// Determines whether [is linked to record identification link identifier] [the specified _record identification].
        /// </summary>
        /// <param name="recordIdentifier">
        /// The _record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsLinkedToRecordIdentificationLinkId(string recordIdentifier, int linkId)
        {
            if (this.Links == null)
            {
                return false;
            }

            foreach (var link in this.Links)
            {
                if (link.LinkId != linkId && (linkId > 0 || link.LinkId > 0))
                {
                    continue;
                }

                if (link.RecordIdentification.Equals(recordIdentifier))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Jsons the links.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<object> JsonLinks()
        {
            if (this.Links == null)
            {
                return null;
            }

            var linkarray = new List<object>(this.Links.Count);

            foreach (var link in this.Links)
            {
                object linkJson = link.Json();
                if (linkJson != null)
                {
                    linkarray.Add(linkJson);
                }
            }

            return linkarray;
        }

        /// <summary>
        /// Jsons the modify request.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object JsonModifyRequest()
        {
            if (this.Mode == "NewOffline" || this.Mode == "UpdateOffline")
            {
                return null;
            }

            if (this.Mode == "Delete")
            {
                if (this.RecordIdentification.Length < 10)
                {
                    return null;
                }

                return new List<object>
                           {
                               this.Mode,
                               this.RecordIdentification,
                               null,
                               this.JsonLinks(),
                               this.OptionArray()
                           };
            }

            if (!string.IsNullOrEmpty(this.Mode) && this.Mode != "Update" && this.Mode != "New")
            {
                return new List<object>
                           {
                               this.Mode,
                               this.RecordIdentification,
                               null,
                               this.JsonLinks(),
                               this.OptionArray()
                           };
            }

            if (this.ReferencedRecord == null && (this.recordId == null || this.recordId.StartsWith("new")))
            {
                if (this.Deleted)
                {
                    return null;
                }

                this.Mode = "New";
                return new List<object>
                           {
                               this.Mode,
                               this.recordIdentification,
                               this.JsonValues(),
                               this.JsonLinks(),
                               this.OptionArray()
                           };
            }

            if (this.Deleted)
            {
                this.Mode = "Delete";
                return this.RecordIdentification.Length < 10
                           ? null
                           : new List<object>
                                 {
                                     this.Mode,
                                     this.RecordIdentification,
                                     this.JsonValues(),
                                     this.JsonLinks(),
                                     this.OptionArray()
                                 };
            }

            var jsonValues = this.JsonValues() as List<object>;
            var jsonLinks = this.JsonLinks();
            if ((jsonValues == null || jsonValues.Count == 0) && (jsonLinks == null || jsonLinks.Count == 0))
            {
                this.Mode = "Sync";
                return new List<object>
                           {
                               this.Mode,
                               this.RecordIdentification,
                               jsonValues,
                               jsonLinks,
                               this.OptionArray()
                           };
            }

            this.Mode = "Update";
            return new List<object> { this.Mode, this.RecordIdentification, jsonValues, jsonLinks, this.OptionArray() };
        }

        /// <summary>
        /// Jsons the values.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object JsonValues()
        {
            if (this.Values == null)
            {
                return null;
            }

            var valuearray = new List<object>(this.Values.Count);
            valuearray.AddRange(this.Values.Values.Select(fieldValue => fieldValue.Json()).Where(fieldJSon => fieldJSon != null));

            return valuearray;
        }

        /// <summary>
        /// Justs the synchronize.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool JustSync()
        {
            return this.Mode != null && this.Mode.StartsWith("Sync");
        }

        /// <summary>
        /// Links for link information.
        /// </summary>
        /// <param name="linkInfo">
        /// The link information.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLink"/>.
        /// </returns>
        public UPCRMLink LinkForLinkInfo(UPCRMLinkInfo linkInfo)
        {
            var dataStore = UPCRMDataStore.DefaultStore;
            if (this.Links != null)
            {
                foreach (UPCRMLink link in this.Links)
                {
                    var currentLinkInfo = dataStore.LinkInfoForInfoAreaTargetInfoAreaLinkId(
                        this.InfoAreaId,
                        link.InfoAreaId,
                        link.LinkId);

                    if (currentLinkInfo.Equals(linkInfo))
                    {
                        return link;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Links the with information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLink"/>.
        /// </returns>
        public UPCRMLink LinkWithInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            return this.Links?.FirstOrDefault(link => link.MatchesInfoAreaIdLinkId(infoAreaId, linkId));
        }

        /// <summary>
        /// Matchings the first link.
        /// </summary>
        /// <param name="compareRecord">
        /// The compare record.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool MatchingFirstLink(UPCRMRecord compareRecord)
        {
            if (this.Links == null || compareRecord.Links == null)
            {
                return false;
            }

            if (this.Links.Count < 1 || compareRecord.Links.Count < 1)
            {
                return false;
            }

            var myFirstLink = this.Links?.FirstOrDefault();
            var otherFirstLink = compareRecord.Links?.FirstOrDefault();
            if (!myFirstLink.MatchesInfoAreaIdLinkId(otherFirstLink.InfoAreaId, otherFirstLink.LinkId))
            {
                return false;
            }

            if (myFirstLink.Record == otherFirstLink.Record)
            {
                return true;
            }

            return myFirstLink.RecordId.Length > 8 && myFirstLink.RecordId.Equals(otherFirstLink.RecordId);
        }

        /// <summary>
        /// News the value field identifier.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public UPCRMFieldValue NewValueFieldId(string newValue, int fieldId)
        {
            return this.NewValueFieldIdOnlyOffline(newValue, fieldId, false);
        }

        /// <summary>
        /// News the value field identifier only offline.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public UPCRMFieldValue NewValueFieldIdOnlyOffline(string newValue, int fieldId, bool onlyOffline)
        {
            if (this.Values == null)
            {
                this.Values = new Dictionary<string, UPCRMFieldValue>();
            }

            var fieldValue = this.Values.ValueOrDefault($"{fieldId}");
            if (fieldValue == null)
            {
                fieldValue = UPCRMFieldValue.ValueInfoAreaIdFieldId(string.Empty, this.InfoAreaId, fieldId);
                this.Values.SetObjectForKey(fieldValue, $"{fieldId}");
            }

            fieldValue.ChangeValue = newValue;
            return fieldValue;
        }

        /// <summary>
        /// News the value from value field identifier.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public UPCRMFieldValue NewValueFromValueFieldId(string newValue, string oldValue, int fieldId)
        {
            return this.NewValueFromValueFieldIdOnlyOffline(newValue, oldValue, fieldId, false);
        }

        /// <summary>
        /// News the value from value field identifier only offline.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMFieldValue"/>.
        /// </returns>
        public UPCRMFieldValue NewValueFromValueFieldIdOnlyOffline(
            string newValue,
            string oldValue,
            int fieldId,
            bool onlyOffline)
        {
            if (this.Values == null)
            {
                this.Values = new Dictionary<string, UPCRMFieldValue>();
            }

            var fieldNumber = fieldId;
            var fieldValue = this.Values.ValueOrDefault($"{fieldNumber}");
            if (fieldValue == null)
            {
                fieldValue = UPCRMFieldValue.ChangeValueOldValueInfoAreaIdFieldIdOnlyOffline(
                    newValue,
                    oldValue,
                    this.InfoAreaId,
                    fieldId,
                    onlyOffline);
                this.Values.SetObjectForKey(fieldValue, $"{fieldNumber}");
                return fieldValue;
            }

            return fieldValue.ChangeValueOldValue(newValue, oldValue, onlyOffline) ? fieldValue : null;
        }

        /// <summary>
        /// Options the array.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public List<object> OptionArray()
        {
            List<object> optionArray = null;
            if (!string.IsNullOrEmpty(this.Options))
            {
                optionArray = this.Options.ParseAsJson() as List<object>;
            }

            if (this.keyFields != null)
            {
                var keyFieldIndices = this.keyFields.Select(field => field.FieldId).ToList();

                var keyFieldArray = new List<object> { "KeyFields", keyFieldIndices };
                if (optionArray == null)
                {
                    optionArray = new List<object> { keyFieldArray };
                }
                else
                {
                    var newarray = new List<object> { optionArray, keyFieldArray };
                    optionArray = newarray;
                }
            }

            return optionArray;
        }

        /// <summary>
        /// Records the by merging with record.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecord"/>.
        /// </returns>
        public UPCRMRecord RecordByMergingWithRecord(UPCRMRecord record)
        {
            if (this.Mode == null || this.Mode == "Delete")
            {
                return this;
            }

            if (record.Mode == "Delete" || this.Mode == "Sync")
            {
                return record;
            }

            if (record.Mode.StartsWith("Sync"))
            {
                return this;
            }

            var mergedRecord = new UPCRMRecord(this.RecordIdentification, this.Mode, null);
            foreach (UPCRMLink link in this.Links)
            {
                mergedRecord.AddLink(link);
            }

            if (this.FieldValues != null)
            {
                foreach (UPCRMFieldValue fieldValue in this.FieldValues)
                {
                    mergedRecord.AddValue(fieldValue);
                }
            }

            if (record.FieldValues != null)
            {
                foreach (UPCRMFieldValue fieldValue in record.FieldValues)
                {
                    mergedRecord.AddValue(fieldValue);
                }
            }

            return mergedRecord;
        }

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void Serialize(UPSerializer writer)
        {
            writer.WriteElementStart("Record");
            writer.WriteAttributeValue("infoAreaId", this.InfoAreaId);
            if (!string.IsNullOrEmpty(this.RecordId))
            {
                writer.WriteAttributeValue("recordId", this.RecordId);
            }

            if (!string.IsNullOrEmpty(this.OriginalRecordIdentification))
            {
                writer.WriteAttributeValue("originalId", this.OriginalRecordIdentification);
            }

            writer.WriteAttributeValue("mode", this.Mode);
            var fieldValues = this.FieldValues;
            if (fieldValues.Count > 0)
            {
                writer.WriteElementStart("Values");
                foreach (var value in fieldValues)
                {
                    value.Serialize(writer);
                }

                writer.WriteElementEnd();
            }

            if (this.Links.Count > 0)
            {
                writer.WriteElementStart("Links");
                foreach (UPCRMLink link in this.Links)
                {
                    link.Serialize(writer);
                }

                writer.WriteElementEnd();
            }

            if (this.keyFields.Count > 0)
            {
                writer.WriteElementStart("KeyFields");
                foreach (UPCRMField field in this.keyFields)
                {
                    writer.WriteElementValueAttributes("KeyFieldIndex", $"{field.FieldId}", null);
                }

                writer.WriteElementEnd();
            }

            writer.WriteElementEnd();
        }

        /// <summary>
        /// Strings the index of the field value for field.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFieldValueForFieldIndex(int fieldIndex)
        {
            var fieldValue = this.FieldValueForFieldIndex(fieldIndex);
            return fieldValue?.Value;
        }

        /// <summary>
        /// Strings for property conditions.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="propertyConditions">
        /// The property conditions.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringForPropertyConditions(string property, Dictionary<string, UPConfigQueryCondition> propertyConditions)
        {
            var cond = propertyConditions.ValueOrDefault(property);
            if (cond == null || cond.NumberOfValues < 1)
            {
                return null;
            }

            return cond.ValueAtIndex(0);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var desc = new StringBuilder();
            desc.Append(
                this.recordIdentification != null
                    ? $"RecordIdentification={this.recordIdentification}{Environment.NewLine}"
                    : $"New {this.InfoAreaId}{Environment.NewLine}");

            desc.Append($"OriginalRecordIdentification={this.originalRecordIdentification}{Environment.NewLine}");

            if (this.Values != null && this.Values.Any())
            {
                desc.Append("Values:");
                foreach (var fieldValue in this.Values.Values)
                {
                    desc.Append($"{Environment.NewLine}{fieldValue}");
                }
            }

            if (this.Links != null && this.Links.Any())
            {
                desc.Append("{Environment.NewLine}Links:");
                foreach (var link in this.Links)
                {
                    desc.Append($"{Environment.NewLine}{link}");
                }
            }

            if (this.keyFields != null)
            {
                desc.Append($"{Environment.NewLine}KeyFields:");
                foreach (var field in this.keyFields)
                {
                    desc.Append($"{Environment.NewLine}{field.FieldId},");
                }
            }

            if (!string.IsNullOrEmpty(this.Options))
            {
                desc.Append($"{Environment.NewLine}Options: {this.Options}");
            }

            return desc.ToString();
        }

        /// <summary>
        /// Updates the record information with record identification clear links.
        /// </summary>
        /// <param name="recordIdentifier">
        /// The _record identification.
        /// </param>
        /// <param name="clearLinks">
        /// if set to <c>true</c> [clear links].
        /// </param>
        public void UpdateRecordInformationWithRecordIdentification(string recordIdentifier, bool clearLinks)
        {
            this.recordIdentification = recordIdentifier;
            this.originalRecordIdentification = recordIdentifier;
            this.recordId = this.recordIdentification.RecordId();
            if (!string.IsNullOrEmpty(this.Mode) && this.Mode.StartsWith("Parent"))
            {
                this.Mode = this.Mode.Substring(6);
            }

            if (clearLinks)
            {
                this.Links = null;
            }
        }

        /// <summary>
        /// Populates list of child records.
        /// </summary>
        /// <param name="queryTable">
        /// The query table.
        /// </param>
        /// <param name="childRecord">
        /// The record.
        /// </param>
        /// <returns>
        /// List of <see cref="UPCRMRecord"/>.
        /// </returns>
        private List<UPCRMRecord> PopulateChildRecordArray(UPConfigQueryTable queryTable, UPCRMRecord childRecord)
        {
            var subTableCount = queryTable.NumberOfSubTables;
            var childRecordArray = new List<UPCRMRecord> { childRecord };

            if (subTableCount <= 0)
            {
                return childRecordArray;
            }

            for (var i = 0; i < subTableCount; i++)
            {
                var childRecordsForQueryTable = ChildRecordsForQueryTableRootRecord(queryTable.SubTableAtIndex(i), childRecord);
                if (childRecordsForQueryTable != null && childRecordsForQueryTable.Any())
                {
                    childRecordArray.AddRange(childRecordsForQueryTable);
                }
            }

            return childRecordArray;
        }

        /// <summary>
        /// Add links to UPCRMRecord field
        /// </summary>
        /// <param name="queryTable">
        /// The query table.
        /// </param>
        /// <param name="childRecord">
        /// Record to add link to.
        /// </param>
        /// <param name="parentRecord">
        /// Parent Record.
        /// </param>
        /// <returns>
        /// <see cref="UPCRMRecord"/>
        /// </returns>
        private UPCRMRecord UPCRMRecordAddLinks(UPConfigQueryTable queryTable, UPCRMRecord childRecord, UPCRMRecord parentRecord)
        {
            var parentLinks = ArrayForPropertyConditions(ParentLinksKey, queryTable.PropertyConditions);
            if (parentLinks == null || !parentLinks.Any())
            {
                parentLinks = ArrayForPropertyConditions(ParentLinkProperty, queryTable.PropertyConditions);
            }

            var allLinks = parentLinks?.Count == 1 && parentLinks[0] as string == "true";
            if (allLinks && parentRecord.Links != null)
            {
                foreach (var link in parentRecord.Links)
                {
                    childRecord.AddLink(link);
                }
            }
            else
            {
                if (parentLinks != null)
                {
                    foreach (string linkString in parentLinks)
                    {
                        var linkInfoAreaId = string.Empty;
                        var linkId = 0;
                        var parts = linkString.Split('#');
                        if (parts.Length > 1)
                        {
                            linkInfoAreaId = parts[0];
                            linkId = int.Parse(parts[1]);
                        }
                        else
                        {
                            linkInfoAreaId = linkString;
                            linkId = -1;
                        }

                        var link = parentRecord.LinkWithInfoAreaIdLinkId(linkInfoAreaId, linkId);
                        if (link != null)
                        {
                            childRecord.AddLink(link);
                        }
                    }
                }
            }

            return childRecord;
        }
    }
}
