// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerInfoAreaMetaInfo.cs" company="Aurea Software Gmbh">
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
//   Table and field index
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Table and field index
    /// </summary>
    public class UPTableAndFieldIndex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPTableAndFieldIndex"/> class.
        /// </summary>
        public UPTableAndFieldIndex()
        {
            this.TableIndex = this.FieldIndex = 0;
        }

        /// <summary>
        /// Gets or sets the index of the field.
        /// </summary>
        /// <value>
        /// The index of the field.
        /// </value>
        public int FieldIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the table.
        /// </summary>
        /// <value>
        /// The index of the table.
        /// </value>
        public int TableIndex { get; set; }
    }

    /// <summary>
    /// Container info area meta info
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.ICrmDataSourceTable" />
    public class UPContainerInfoAreaMetaInfo : ICrmDataSourceTable
    {
        private const int LinkId126 = 126;

        /// <summary>
        /// The _cdata initialized.
        /// </summary>
        protected bool _cdataInitialized;

        /// <summary>
        /// The _c link field count.
        /// </summary>
        protected int _cLinkFieldCount;

        /// <summary>
        /// The _c link fields.
        /// </summary>
        protected string[] _cLinkFields;

        /// <summary>
        /// The _c link map count.
        /// </summary>
        protected int _cLinkMapCount;

        /// <summary>
        /// The _crm database.
        /// </summary>
        protected object _crmDatabase;

        /// <summary>
        /// The _field count.
        /// </summary>
        protected int _fieldCount;

        /// <summary>
        /// The _field ids.
        /// </summary>
        protected List<int> _fieldIds;

        /// <summary>
        /// The _info area id index.
        /// </summary>
        protected int _infoAreaIdIndex;

        /// <summary>
        /// The _int array.
        /// </summary>
        protected FieldIdType[] _intArray;

        /// <summary>
        /// The _is sync.
        /// </summary>
        protected bool _isSync;

        /// <summary>
        /// The _link id.
        /// </summary>
        protected int _linkId;

        /// <summary>
        /// The _link map.
        /// </summary>
        protected int[] _linkMap;

        /// <summary>
        /// The _query tree item.
        /// </summary>
        protected QueryTreeItem _queryTreeItem;

        /// <summary>
        /// The _record id index.
        /// </summary>
        protected int _recordIdIndex;

        /// <summary>
        /// The _record template.
        /// </summary>
        protected object _recordTemplate;

        /// <summary>
        /// The _source field count.
        /// </summary>
        protected int _sourceFieldCount;

        /// <summary>
        /// The _table info.
        /// </summary>
        protected TableInfo _tableInfo;

        /// <summary>
        /// The sub tables.
        /// </summary>
        private List<UPContainerInfoAreaMetaInfo> subTables;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerInfoAreaMetaInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="parentRelation">
        /// The parent relation.
        /// </param>
        public UPContainerInfoAreaMetaInfo(string infoAreaId, int linkId = -1, string parentRelation = null)
        {
            this._cdataInitialized = false;
            this.InfoAreaId = infoAreaId;
            this.InfoAreaIdWithLink = infoAreaId.InfoAreaIdLinkId(linkId);
            this._fieldIds = new List<int>();
            this.Fields = new List<UPContainerFieldMetaInfo>();
            this._linkId = linkId;
            this.Condition = null;
            this.Links = null;
            this._isSync = false;
            this.ParentRelation = parentRelation ?? "PLUS";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerInfoAreaMetaInfo"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="parentRelation">
        /// The parent relation.
        /// </param>
        public UPContainerInfoAreaMetaInfo(string infoAreaId, string parentRelation)
            : this(infoAreaId, -1, parentRelation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerInfoAreaMetaInfo"/> class.
        /// </summary>
        /// <param name="querytable">
        /// The querytable.
        /// </param>
        public UPContainerInfoAreaMetaInfo(UPConfigQueryTable querytable)
        {
            this.InfoAreaId = querytable.InfoAreaId;
            this._linkId = querytable.LinkId;
            this.InfoAreaIdWithLink = this.InfoAreaId.InfoAreaIdLinkId(this._linkId);
            this._fieldIds = null;
            this.Links = null;
            this._cdataInitialized = false;
            this.ParentRelation = querytable.ParentRelation;
            if (querytable.Condition != null)
            {
                this.AddConditions(querytable);
            }
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public UPInfoAreaCondition Condition { get; set; }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        /// <value>
        /// The field count.
        /// </value>
        public int FieldCount => this._fieldIds?.Count ?? 0;

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPContainerFieldMetaInfo> Fields { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has information area identifier column.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has information area identifier column; otherwise, <c>false</c>.
        /// </value>
        public bool HasInfoAreaIdColumn => this._infoAreaIdIndex >= 0;

        /// <summary>
        /// Gets a value indicating whether this instance has output child information areas.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has output child information areas; otherwise, <c>false</c>.
        /// </value>
        public bool HasOutputChildInfoAreas
            => this.subTables != null && this.subTables.Any(subtable => subtable.IsResultInfoArea);

        /// <summary>
        /// Gets a value indicating whether this instance has record identifier.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has record identifier; otherwise, <c>false</c>.
        /// </value>
        public bool HasRecordId => this._recordIdIndex >= 0;

        /// <summary>
        /// Gets or sets a value indicating whether [ignore lookup].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore lookup]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreLookup { get; set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the index of the information area identifier column.
        /// </summary>
        /// <value>
        /// The index of the information area identifier column.
        /// </value>
        public int InfoAreaIdColumnIndex => this._infoAreaIdIndex;

        /// <summary>
        /// Gets the information area identifier with link.
        /// </summary>
        /// <value>
        /// The information area identifier with link.
        /// </value>
        public string InfoAreaIdWithLink { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is result information area.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is result information area; otherwise, <c>false</c>.
        /// </value>
        public bool IsResultInfoArea
            => !this.ParentRelation.StartsWith("HAVING") && !this.ParentRelation.StartsWith("WITHOUT");

        // weak reference owned by Query object, so must be insured to be valid by Query::EnsureValid
        /// <summary>
        /// Gets or sets the link id.
        /// </summary>
        public int LinkId
        {
            get
            {
                return this._linkId;
            }

            set
            {
                this._linkId = value;
            }
        }

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public List<object> Links { get; private set; }

        /// <summary>
        /// Gets the number of sub tables.
        /// </summary>
        /// <value>
        /// The number of sub tables.
        /// </value>
        public int NumberOfSubTables => this.subTables?.Count ?? 0;

        /// <summary>
        /// Gets the Start index of the output column.
        /// </summary>
        /// <value>
        /// The Start index of the output column.
        /// </value>
        public int OutputColumnStartIndex { get; private set; }

        /// <summary>
        /// Gets or sets the parent relation.
        /// </summary>
        /// <value>
        /// The parent relation.
        /// </value>
        public string ParentRelation { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Position { get; set; }

        /// <summary>
        /// Gets the index of the record identifier column.
        /// </summary>
        /// <value>
        /// The index of the record identifier column.
        /// </value>
        public int RecordIdColumnIndex => this._recordIdIndex;

        /// <summary>
        /// Gets or sets the result field offset.
        /// </summary>
        /// <value>
        /// The result field offset.
        /// </value>
        public int ResultFieldOffset { get; set; }

        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public UPInfoAreaCondition AddCondition(UPInfoAreaCondition condition)
        {
            this.Condition = this.Condition == null
                                 ? condition
                                 : this.Condition.InfoAreaConditionByAppendingAndCondition(condition);

            return this.Condition;
        }

        /// <summary>
        /// Adds the conditions.
        /// </summary>
        /// <param name="querytable">
        /// The querytable.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddConditions(UPConfigQueryTable querytable)
        {
            if (querytable.Condition == null)
            {
                return false;
            }

            this.AddCondition(querytable.Condition.Condition());
            return true;
        }

        /// <summary>
        /// Adds the condition with or.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public UPInfoAreaCondition AddConditionWithOr(UPInfoAreaCondition condition)
        {
            this.Condition = this.Condition == null
                                 ? condition
                                 : this.Condition.InfoAreaConditionByAppendingOrCondition(condition);

            return this.Condition;
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddField(UPContainerFieldMetaInfo field)
        {
            if (field.InfoAreaIdWithLink != this.InfoAreaIdWithLink)
            {
                if (this.subTables != null)
                {
                    foreach (var subTable in this.subTables)
                    {
                        if (field.InfoAreaIdWithLink.Equals(subTable.InfoAreaIdWithLink))
                        {
                            return subTable.AddField(field);
                        }
                    }
                }

                var addSubTable = new UPContainerInfoAreaMetaInfo(field.InfoAreaId, field.LinkId)
                {
                    ParentRelation = "PLUS"
                };

                if (this.subTables == null)
                {
                    this.subTables = new List<UPContainerInfoAreaMetaInfo> { addSubTable };
                }
                else
                {
                    this.subTables.Add(addSubTable);
                }

                return addSubTable.AddField(field);
            }

            return this.AddFieldWithoutChildCheck(field);
        }

        /// <summary>
        /// Adds the fields to array.
        /// </summary>
        /// <param name="fieldarray">
        /// The fieldarray.
        /// </param>
        public void AddFieldsToArray(List<object> fieldarray)
        {
            this.AddFieldsToArrayContext(fieldarray, new UPTableAndFieldIndex());
        }

        /// <summary>
        /// Adds the fields to array context.
        /// </summary>
        /// <param name="fieldArray">
        /// The field array.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void AddFieldsToArrayContext(List<object> fieldArray, UPTableAndFieldIndex context)
        {
            this.Position = context.TableIndex;
            context.TableIndex++;
            var count = this.Fields?.Count;
            for (var i = 0; i < count; i++)
            {
                var fieldMetaInfo = this.Fields[i];
                fieldMetaInfo.PositionInResult = context.FieldIndex;
                fieldMetaInfo.PositionInInfoArea = i;
                fieldMetaInfo.InfoAreaPosition = this.Position;
                context.FieldIndex++;
                fieldArray.Add(fieldMetaInfo);
            }

            count = this.subTables?.Count;
            for (var i = 0; i < count; i++)
            {
                this.subTables[i].AddFieldsToArrayContext(fieldArray, context);
            }
        }

        /// <summary>
        /// Adds the field without child check.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddFieldWithoutChildCheck(UPContainerFieldMetaInfo field)
        {
            this._cdataInitialized = false;
            this.Fields.Add(field);
            this._fieldIds.Add(field.CrmField.FieldId);
            if (field.IsDateField)
            {
                var otherMetaInfo = this.FieldInfoAreaForFieldIndex(field.CrmField.FieldInfo.TimeFieldId);
                if (otherMetaInfo != null)
                {
                    field.OtherDateTimeField = otherMetaInfo;
                    otherMetaInfo.OtherDateTimeField = field;
                }
            }
            else if (field.IsTimeField)
            {
                var otherMetaInfo = this.FieldInfoAreaForFieldIndex(field.CrmField.FieldInfo.DateFieldId);
                if (otherMetaInfo != null)
                {
                    field.OtherDateTimeField = otherMetaInfo;
                    otherMetaInfo.OtherDateTimeField = field;
                }
            }

            return this._fieldIds.Count - 1;
        }

        /// <summary>
        /// Adds the new field to array.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="addedFields">
        /// The added fields.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddNewFieldToArray(UPContainerFieldMetaInfo field, List<UPContainerFieldMetaInfo> addedFields)
        {
            if (addedFields == null)
            {
                return false;
            }

            foreach (UPContainerFieldMetaInfo _field in addedFields)
            {
                if (_field.CrmField.FieldIdentification == field.CrmField.FieldIdentification)
                {
                    return false;
                }
            }

            this.AddFieldWithoutChildCheck(field);
            addedFields.Add(field);
            return true;
        }

        /// <summary>
        /// Adds the sub table.
        /// </summary>
        /// <param name="querytable">
        /// The querytable.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddSubTable(UPConfigQueryTable querytable)
        {
            return this.AddSubTableRelation(querytable, null);
        }

        /// <summary>
        /// Adds the sub table relation.
        /// </summary>
        /// <param name="querytable">
        /// The querytable.
        /// </param>
        /// <param name="relation">
        /// The relation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddSubTableRelation(UPConfigQueryTable querytable, string relation)
        {
            var table = new UPContainerInfoAreaMetaInfo(querytable);
            if (relation != null)
            {
                table.ParentRelation = relation;
            }

            if (this.subTables == null)
            {
                this.subTables = new List<UPContainerInfoAreaMetaInfo>();
            }

            this.subTables.Add(table);
            var count = querytable.NumberOfSubTables;
            for (var i = 0; i < count; i++)
            {
                var subQueryTable = querytable.SubTableAtIndex(i);
                var subParentRelation = subQueryTable.ParentRelation;
                if (string.IsNullOrEmpty(subParentRelation))
                {
                    subParentRelation = relation;
                }

                table.AddSubTableRelation(subQueryTable, subParentRelation);
            }

            return true;
        }

        /// <summary>
        /// Adds the table.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        public void AddTable(UPContainerInfoAreaMetaInfo table)
        {
            if (this.subTables == null)
            {
                this.subTables = new List<UPContainerInfoAreaMetaInfo>();
            }

            this.subTables.Add(table);
        }

        /// <summary>
        /// Addtoes the parent item CRM database options.
        /// </summary>
        /// <param name="parentItem">
        /// The parent item.
        /// </param>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="QueryTreeItem"/>.
        /// </returns>
        public QueryTreeItem AddtoParentItemCrmDatabaseOptions(QueryTreeItem parentItem, CRMDatabase crmDatabase, int options)
        {
            if (!this._cdataInitialized)
            {
                this.InitCData(crmDatabase);
            }

            if (this._recordTemplate == null)
            {
                return null;
            }

            var queryTreeItem = new QueryTreeItem((RecordTemplate)this._recordTemplate, this._linkId);
            if (this.IgnoreLookup)
            {
                queryTreeItem.SetIgnoreLookupRows(true);
            }

            this._queryTreeItem = queryTreeItem;
            if (this.Condition != null)
            {
                queryTreeItem.AddCondition(this.Condition.CreateQueryConditionOptions(crmDatabase, options));
            }

            var count = this.subTables?.Count;
            for (int i = 0; i < count; i++)
            {
                this.subTables[i].AddtoParentItemCrmDatabaseOptions(this._queryTreeItem, crmDatabase, options);
            }

            parentItem?.AddSubNode(this.ParentRelation, queryTreeItem);

            return queryTreeItem;
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ApplyFilter(UPConfigFilter filter)
        {
            var rootTable = filter.RootTable;
            if (this.InfoAreaId == rootTable.InfoAreaId)
            {
                this.AddConditions(rootTable);
                int i, subTableIndex = rootTable.NumberOfSubTables;
                var hasOptional = false;
                var filterHasOptional = false;
                foreach (var subTable in this.subTables ?? new List<UPContainerInfoAreaMetaInfo>())
                {
                    if (subTable.ParentRelation.IndexOf("OPTIONAL", StringComparison.Ordinal) <= 0)
                    {
                        continue;
                    }

                    hasOptional = true;
                    break;
                }

                if (hasOptional)
                {
                    for (i = 0; !filterHasOptional && i < subTableIndex; i++)
                    {
                        if (rootTable.SubTableAtIndex(i).ParentRelation.IndexOf("OPTIONAL", StringComparison.Ordinal) > 0)
                        {
                            filterHasOptional = true;
                        }
                    }
                }

                if (hasOptional && filterHasOptional)
                {
                    this.AddSubTableRelation(rootTable, "HAVING");
                }
                else
                {
                    for (i = 0; i < subTableIndex; i++)
                    {
                        this.AddSubTable(rootTable.SubTableAtIndex(i));
                    }
                }
            }
            else
            {
                this.AddSubTableRelation(rootTable, "HAVING");
            }

            return true;
        }

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <param name="addedFields">
        /// The added fields.
        /// </param>
        public void ApplyTimeZone(UPCRMTimeZone timeZone, List<UPContainerFieldMetaInfo> addedFields)
        {
            if (this.Fields != null)
            {
                var currentFields = new List<UPContainerFieldMetaInfo>(this.Fields);

                foreach (var fieldMetaInfo in currentFields)
                {
                    if (fieldMetaInfo.IsDateField && fieldMetaInfo.OtherDateTimeField == null)
                    {
                        var timeFieldId = fieldMetaInfo.CrmFieldInfo.TimeFieldId;
                        if (timeFieldId < 0)
                        {
                            continue;
                        }

                        var field = new UPCRMField(timeFieldId, this.InfoAreaId);
                        if (field.FieldInfo == null)
                        {
                            continue;
                        }

                        var otherInfo = new UPContainerFieldMetaInfo(field);
                        this.AddNewFieldToArray(otherInfo, addedFields);
                    }
                    else if (fieldMetaInfo.IsTimeField && fieldMetaInfo.OtherDateTimeField == null)
                    {
                        var dateFieldId = fieldMetaInfo.CrmFieldInfo?.DateFieldId ?? 0;
                        if (dateFieldId == 0)
                        {
                            continue;
                        }

                        var field = new UPCRMField(dateFieldId, this.InfoAreaId);
                        if (field.FieldInfo == null)
                        {
                            continue;
                        }

                        var otherInfo = new UPContainerFieldMetaInfo(field);
                        this.AddNewFieldToArray(otherInfo, addedFields);
                    }
                }
            }

            var dateTimeCondition = this.Condition?.DateTimeConditionForInfoAreaParent(this, null);
            dateTimeCondition?.ApplyTimeZone(timeZone);

            if (this.subTables == null)
            {
                return;
            }

            foreach (var subInfos in this.subTables)
            {
                subInfos?.ApplyTimeZone(timeZone, addedFields);
            }
        }

        /// <summary>
        /// Builds the result meta information for query.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool BuildResultMetaInfoForQuery(Query query)
        {
            var qti = this._queryTreeItem;
            if (qti == null)
            {
                return false;
            }

            this.OutputColumnStartIndex = qti.OutputColumnStartIndex;
            this._recordIdIndex = qti.RecordIdIndex;
            this._infoAreaIdIndex = qti.InfoAreaIdIndex;
            return true;
        }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceField"/>.
        /// </returns>
        public ICrmDataSourceField FieldAtIndex(int index)
        {
            return this.Fields[index];
        }

        /// <summary>
        /// Fields the index of the information area for field.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerFieldMetaInfo"/>.
        /// </returns>
        public UPContainerFieldMetaInfo FieldInfoAreaForFieldIndex(int fieldIndex)
        {
            if (fieldIndex < 0)
            {
                return null;
            }

            return this.Fields.FirstOrDefault(fmi => fmi.FieldId == fieldIndex);
        }

        /// <summary>
        /// Informations the area position in result.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int InfoAreaPositionInResult()
        {
            return this.Position;
        }

        /// <summary>
        /// Initializes the c data.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        public void InitCData(CRMDatabase database)
        {
            if (_cdataInitialized)
            {
                return;
            }

            _cdataInitialized = true;
            var infoAreaIdString = InfoAreaId;
            _tableInfo = database.GetTableInfoByInfoArea(infoAreaIdString);
            if (_tableInfo == null)
            {
                return;
            }

            _sourceFieldCount = _fieldIds?.Count ?? 0;
            _intArray = new FieldIdType[_sourceFieldCount + 1];
            _fieldCount = 0;
            var linkFields = GetUPContainerFieldMetaInfoLinkFields();
            _cLinkFieldCount = 0;
            _cLinkMapCount = 0;

            if (linkFields?.Any() == true)
            {
                PopulateLinkMapAndCLinkFieldsFromLinkFields(linkFields);
            }
            else if (Links?.Any() == true)
            {
                PopulateLinkMapAndCLinkFieldsFromLinks();
            }
            else
            {
                _cLinkFields = null;
                _linkMap = null;
            }

            _recordTemplate = new RecordTemplate(
                database,
                _isSync,
                infoAreaIdString,
                _fieldCount,
                _intArray,
                _cLinkFieldCount,
                _cLinkFields?.ToList(),
                false,
                false);
        }

        /// <summary>
        /// Inserts the into result information areas.
        /// </summary>
        /// <param name="infoAreaArray">
        /// The information area array.
        /// </param>
        public void InsertIntoResultInfoAreas(List<UPContainerInfoAreaMetaInfo> infoAreaArray)
        {
            infoAreaArray.Add(this);
            var count = this.subTables?.Count ?? 0;
            if (count <= 0)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                var subTable = this.subTables[i];
                if (subTable.IsResultInfoArea)
                {
                    subTable.InsertIntoResultInfoAreas(infoAreaArray);
                }
            }
        }

        /// <summary>
        /// Numbers the of fields.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int NumberOfFields()
        {
            return this.FieldCount;
        }

        /// <summary>
        /// Sets the condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void SetCondition(UPInfoAreaCondition condition)
        {
            this.Condition = condition;
        }

        /// <summary>
        /// Subs the index of the table at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceTable"/>.
        /// </returns>
        public ICrmDataSourceTable SubTableAtIndex(int index)
        {
            return this.subTables?[index];
        }

        /// <summary>
        /// Subs the table for information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerInfoAreaMetaInfo"/>.
        /// </returns>
        public UPContainerInfoAreaMetaInfo SubTableForInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            foreach (var infoAreaMetaInfo in this.subTables)
            {
                if (infoAreaMetaInfo.InfoAreaId == infoAreaId && (infoAreaMetaInfo.LinkId == linkId || (linkId <= 0 && infoAreaMetaInfo.LinkId <= 0)))
                {
                    return infoAreaMetaInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Trees the node to object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object TreeNodeToObject()
        {
            object subnodes;
            var fieldids = this._fieldIds?.Count == 0 ? null : this._fieldIds;
            var condition = this.Condition?.ConditionToObject();

            if (this.subTables != null && this.subTables.Count > 0)
            {
                var subNodeArray = new List<object>(this.subTables.Count);
                subNodeArray.AddRange(this.subTables.Select(subnode => subnode.TreeNodeToObject()));

                subnodes = subNodeArray;
            }
            else
            {
                subnodes = null;
            }

            return new List<object>
                       {
                           this.ParentRelation,
                           this.InfoAreaId,
                           this._linkId,
                           fieldids,
                           condition,
                           subnodes
                       };
        }

        /// <summary>
        /// Fetches Link Fields
        /// </summary>
        /// <returns>
        /// List of <see cref="UPContainerFieldMetaInfo"/>.
        /// </returns>
        private List<UPContainerFieldMetaInfo> GetUPContainerFieldMetaInfoLinkFields()
        {
            var linkFields = (List<UPContainerFieldMetaInfo>)null;
            var linkFieldsAtEnd = 0;
            var linkFieldIndices = (int[])null;
            for (var i = 0; i < _sourceFieldCount; i++)
            {
                var sourceFieldId = _fieldIds[i];
                if (sourceFieldId == -1 && Fields != null)
                {
                    var fieldMetaInfo = Fields[i];
                    if (fieldMetaInfo.IsLinkField)
                    {
                        if (linkFields == null)
                        {
                            linkFields = new List<UPContainerFieldMetaInfo> { fieldMetaInfo };
                            linkFieldIndices = new int[_sourceFieldCount + 1];
                            linkFieldIndices[0] = i;
                        }
                        else
                        {
                            linkFieldIndices[linkFields.Count] = i;
                            linkFields.Add(fieldMetaInfo);
                        }
                    }

                    linkFieldsAtEnd++;
                    _intArray[_fieldCount++] = 0;
                }
                else
                {
                    var fieldInfo = _tableInfo.GetFieldInfo(sourceFieldId);
                    if (fieldInfo != null)
                    {
                        _intArray[_fieldCount++] = (FieldIdType)sourceFieldId;
                    }
                    else
                    {
                        _intArray[_fieldCount++] = 0;
                    }

                    linkFieldsAtEnd = 0;
                }
            }

            _fieldCount -= linkFieldsAtEnd;
            return linkFields;
        }

        /// <summary>
        /// Populate LinkMap And CLinkFields From LinkFields
        /// </summary>
        /// <param name="linkFields">
        /// List of link fields
        /// </param>
        private void PopulateLinkMapAndCLinkFieldsFromLinkFields(List<UPContainerFieldMetaInfo> linkFields)
        {
            var linkIndex = 0;
            var linkCount = linkFields.Count;
            _cLinkFields = new string[linkCount + 1];
            _linkMap = new int[linkCount + 1];
            linkIndex = 0;
            for (var i = 0; i < linkCount; i++)
            {
                var linkField = (UPCRMLinkField)linkFields[i].CrmField;
                var linkFieldName = linkField.LinkFieldName;
                var linkInfo = _tableInfo.GetLink(linkFieldName);
                if (linkInfo != null && linkInfo.HasColumn && !linkInfo.IsGeneric)
                {
                    _cLinkFields[_cLinkFieldCount] = linkFieldName;
                    _linkMap[linkIndex++] = _cLinkFieldCount++;
                }
            }

            _cLinkMapCount = linkIndex;
        }

        /// <summary>
        /// Populate LinkMap And CLinkFields From Links
        /// </summary>
        private void PopulateLinkMapAndCLinkFieldsFromLinks()
        {
            var linkIndex = 0;
            var linkCount = Links.Count;
            var hadGeneric = false;
            _cLinkFields = new string[linkCount + 1];
            _linkMap = new int[linkCount + 1];
            linkIndex = 0;
            for (var i = 0; i < linkCount; i++)
            {
                var linkFieldName = (string)Links[i];
                var linkInfo = _tableInfo.GetLink(linkFieldName);
                if (linkInfo != null && linkInfo.HasColumn)
                {
                    if (linkInfo.LinkId == LinkId126)
                    {
                        if (hadGeneric)
                        {
                            _linkMap[linkIndex++] = -1;
                        }
                        else
                        {
                            _cLinkFields[_cLinkFieldCount] = linkInfo.InfoAreaColumnName;
                            _linkMap[linkIndex++] = _cLinkFieldCount++;
                            _cLinkFields[_cLinkFieldCount] = linkInfo.ColumnName;
                            _linkMap[linkIndex++] = _cLinkFieldCount++;
                            hadGeneric = true;
                        }
                    }
                    else
                    {
                        _cLinkFields[_cLinkFieldCount] = linkFieldName;
                        _linkMap[linkIndex++] = _cLinkFieldCount++;
                    }
                }
                else
                {
                    _linkMap[linkIndex++] = -1;
                }
            }

            _cLinkMapCount = linkIndex;
        }
    }
}
