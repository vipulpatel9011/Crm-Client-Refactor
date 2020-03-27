// <copyright file="ContainerMetaInfo.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>


namespace Aurea.CRM.Core.CRM.Query
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.Utilities;

    /// <summary>
    /// Container meta info
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.ICrmDataSourceMetaInfo" />
    public class UPContainerMetaInfo : ICrmDataSourceMetaInfo
    {
        private const string Empty = "empty";
        private List<UPCRMSortField> sortFields;
        private List<UPContainerInfoAreaMetaInfo> resultInfoAreas;
        private string _linkRecordIdentification;
        private int _linkId;
        private int _onlineLinkId;
        private DAL.Query _query;
        private bool _timeZoneApplied;

        /// <summary>
        /// Gets the field enumerator.
        /// </summary>
        /// <value>
        /// The field enumerator.
        /// </value>
        public List<UPContainerFieldMetaInfo>.Enumerator? FieldEnumerator => this.OutputFields?.GetEnumerator();

        /// <summary>
        /// Gets the output fields.
        /// </summary>
        /// <value>
        /// The output fields.
        /// </value>
        public List<UPContainerFieldMetaInfo> OutputFields { get; private set; }

        /// <summary>
        /// Gets the root information area meta information.
        /// </summary>
        /// <value>
        /// The root information area meta information.
        /// </value>
        public UPContainerInfoAreaMetaInfo RootInfoAreaMetaInfo { get; private set; }

        /// <summary>
        /// Gets or sets the maximum results.
        /// </summary>
        /// <value>
        /// The maximum results.
        /// </value>
        public int MaxResults { get; set; }

        /// <summary>
        /// Gets the source field control.
        /// </summary>
        /// <value>
        /// The source field control.
        /// </value>
        public FieldControl SourceFieldControl { get; private set; }

        /// <summary>
        /// Gets the database instance.
        /// </summary>
        /// <value>
        /// The database instance.
        /// </value>
        public CRMDatabase DatabaseInstance { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable virtual links].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable virtual links]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableVirtualLinks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show lookups on root].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show lookups on root]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowLookupsOnRoot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [replace case sensitive characters].
        /// </summary>
        /// <value>
        /// <c>true</c> if [replace case sensitive characters]; otherwise, <c>false</c>.
        /// </value>
        public bool ReplaceCaseSensitiveCharacters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [skip result merge].
        /// </summary>
        /// <value>
        /// <c>true</c> if [skip result merge]; otherwise, <c>false</c>.
        /// </value>
        public bool SkipResultMerge { get; set; }

        /// <summary>
        /// Updates the after adding field meta infos.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public void UpdateAfterAddingFieldMetaInfos(List<UPContainerFieldMetaInfo> fields)
        {
            this.OutputFields.AddRange(fields);
            var dummyFields = new List<object>();
            this.RootInfoAreaMetaInfo.AddFieldsToArray(dummyFields);
        }

        /// <summary>
        /// Adds the CRM fields.
        /// </summary>
        /// <param name="crmFields">The CRM fields.</param>
        /// <returns></returns>
        public List<UPContainerFieldMetaInfo> AddCrmFields(List<UPCRMField> crmFields)
        {
            var fields = new List<UPContainerFieldMetaInfo>(crmFields.Count);
            foreach (var field in crmFields)
            {
                var fieldMetaInfo = new UPContainerFieldMetaInfo(field);
                fields.Add(fieldMetaInfo);
            }

            var fieldInfos = new List<UPContainerFieldMetaInfo>(fields.Count);
            foreach (var fieldMetaInfo in fields)
            {
                this.RootInfoAreaMetaInfo.AddField(fieldMetaInfo);
                fieldInfos.Add(fieldMetaInfo);
            }

            this.UpdateAfterAddingFieldMetaInfos(fields);
            return fieldInfos;
        }

        /// <summary>
        /// Updates the after adding CRM fields.
        /// </summary>
        /// <param name="crmFields">The CRM fields.</param>
        public void UpdateAfterAddingCrmFields(List<UPCRMField> crmFields)
        {
            var fields = new List<UPContainerFieldMetaInfo>(crmFields.Count);
            fields.AddRange(crmFields.Select(field => new UPContainerFieldMetaInfo(field)));

            this.UpdateAfterAddingFieldMetaInfos(fields);
        }

        /// <summary>
        /// Adds the query table to node dictionary array.
        /// </summary>
        /// <param name="queryTable">The query table.</param>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <param name="dict">The dictionary.</param>
        /// <param name="arr">The arr.</param>
        public void AddQueryTableToNodeDictionaryArray(UPConfigQueryTable queryTable,
            UPContainerInfoAreaMetaInfo infoAreaMetaInfo, Dictionary<string, object> dict,
            List<UPContainerInfoAreaMetaInfo> arr)
        {
            int i, count = queryTable.NumberOfSubTables;
            if (queryTable.Condition != null)
            {
                infoAreaMetaInfo.AddCondition(queryTable.Condition.Condition());
            }

            for (i = 0; i < count; i++)
            {
                var subTable = queryTable.SubTableAtIndex(i);
                if (string.IsNullOrEmpty(subTable.Alias) || dict.ValueOrDefault(subTable.Alias) != null)
                {
                    continue;
                }

                var subInfoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(subTable.InfoAreaId, subTable.LinkId, subTable.ParentRelation);
                infoAreaMetaInfo.AddTable(subInfoAreaMetaInfo);
                subInfoAreaMetaInfo.Position = dict.Count;
                dict[subTable.Alias] = subInfoAreaMetaInfo;
                arr.Add(subInfoAreaMetaInfo);
                this.AddQueryTableToNodeDictionaryArray(subTable, subInfoAreaMetaInfo, dict, arr);
            }
        }

        /// <summary>
        /// Fields at index.
        /// </summary>
        /// <param name="fieldIndex">Index of the field.</param>
        /// <returns></returns>
        public ICrmDataSourceField FieldAtIndex(int fieldIndex)
        {
            return this.OutputFields[fieldIndex];
        }

        /// <summary>
        /// Fills from query.
        /// </summary>
        /// <param name="configQuery">The configuration query.</param>
        public void FillFromQuery(UPConfigQuery configQuery)
        {
            var infoAreaMetaInfos = new Dictionary<string, object>();
            var infoAreas = new List<UPContainerInfoAreaMetaInfo>();
            this.RootInfoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(configQuery.RootTable.InfoAreaId, string.Empty);
            if (!this.ShowLookupsOnRoot)
            {
                this.RootInfoAreaMetaInfo.IgnoreLookup = true;
            }

            this.RootInfoAreaMetaInfo.Position = 0;
            this.OutputFields = new List<UPContainerFieldMetaInfo>();
            infoAreaMetaInfos[configQuery.RootTable.Alias] = this.RootInfoAreaMetaInfo;
            infoAreas.Add(this.RootInfoAreaMetaInfo);
            this.AddQueryTableToNodeDictionaryArray(configQuery.RootTable, this.RootInfoAreaMetaInfo, infoAreaMetaInfos, infoAreas);

            foreach (var field in configQuery.QueryFields)
            {
                var infoAreaMetaInfo = infoAreaMetaInfos.ValueOrDefault(field.QueryTable.Alias) as UPContainerInfoAreaMetaInfo;
                if (infoAreaMetaInfo == null)
                {
                    continue;
                }

                var fieldMetaInfo = new UPContainerFieldMetaInfo(new UPCRMField(field.FieldIndex, infoAreaMetaInfo.InfoAreaId));
                fieldMetaInfo.PositionInResult = infoAreaMetaInfo.AddField(fieldMetaInfo);
                fieldMetaInfo.PositionInInfoArea = fieldMetaInfo.PositionInResult;
                fieldMetaInfo.InfoAreaPosition = infoAreaMetaInfo.Position;
                this.OutputFields.Add(fieldMetaInfo);
            }

            var infoAreaPosition = 0;
            foreach (var infoAreaMetaInfo in infoAreas)
            {
                infoAreaMetaInfo.ResultFieldOffset = infoAreaPosition;
                infoAreaPosition += infoAreaMetaInfo.FieldCount;
            }

            foreach (var fieldMetaInfoTmp in this.OutputFields)
            {
                fieldMetaInfoTmp.PositionInResult = infoAreas[fieldMetaInfoTmp.InfoAreaPosition].ResultFieldOffset +
                                                    fieldMetaInfoTmp.PositionInInfoArea;
            }
        }

        /// <summary>
        /// Fills from field configuration.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        public void FillFromFieldConfig(FieldControl fieldControl)
        {
            if (fieldControl == null)
            {
                // DDLogError("UPContainerMetaInfo::fillFromFieldConfig called with nullptr");
                return;
            }

            this.sortFields = null;
            this.OutputFields = new List<UPContainerFieldMetaInfo>();
            var dict = new Dictionary<string, object>();
            var treeDict = new Dictionary<string, object>();
            var infoAreas = new List<UPContainerInfoAreaMetaInfo>();
            this.RootInfoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(fieldControl.InfoAreaId, string.Empty);
            if (!this.ShowLookupsOnRoot)
            {
                this.RootInfoAreaMetaInfo.IgnoreLookup = true;
            }

            this.RootInfoAreaMetaInfo.Position = 0;
            var rootTreeNode = new UPContainerMetaInfoAreaTreeNode(this.RootInfoAreaMetaInfo, fieldControl.InfoAreaId);
            treeDict[fieldControl.InfoAreaId ?? Empty] = rootTreeNode;
            dict[fieldControl.InfoAreaId ?? Empty] = this.RootInfoAreaMetaInfo;

            AddContainerInfoAreaMetaInfo(fieldControl, dict, treeDict, rootTreeNode);

            int pos = 0;
            foreach (var node in rootTreeNode.OrderedInfoAreaList())
            {
                infoAreas.Add(node.Node);
                node.Node.Position = pos++;
            }

            this.AddContainerFieldMetaInfo(fieldControl, dict);

            var infoAreaPosition = 0;
            foreach (var infoAreaMetaInfo in infoAreas)
            {
                infoAreaMetaInfo.ResultFieldOffset = infoAreaPosition;
                infoAreaPosition += infoAreaMetaInfo.FieldCount;
            }

            foreach (var fieldMetaInfoTmp in this.OutputFields)
            {
                fieldMetaInfoTmp.PositionInResult = infoAreas[fieldMetaInfoTmp.InfoAreaPosition].ResultFieldOffset +
                                                    fieldMetaInfoTmp.PositionInInfoArea;
            }
        }

        /// <summary>
        /// Fills from field array.
        /// </summary>
        /// <param name="fieldArray">The field array.</param>
        /// <param name="rootInfoAreaId">The root information area identifier.</param>
        public void FillFromFieldArray(List<UPCRMField> fieldArray, string rootInfoAreaId)
        {
            this.sortFields = null;
            this.OutputFields = new List<UPContainerFieldMetaInfo>();
            var dict = new Dictionary<string, object>();
            var infoAreas = new List<object>();
            this.RootInfoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(rootInfoAreaId, string.Empty);
            if (!this.ShowLookupsOnRoot)
            {
                this.RootInfoAreaMetaInfo.IgnoreLookup = true;
            }

            this.RootInfoAreaMetaInfo.Position = 0;
            infoAreas.Add(this.RootInfoAreaMetaInfo);
            dict[rootInfoAreaId] = this.RootInfoAreaMetaInfo;

            var infoAreaPosition = 0;
            if (fieldArray != null)
            {
                foreach (var field in fieldArray)
                {
                    var infoAreaIdWithLink = field.InfoAreaIdWithLink;
                    var metaInfo = (UPContainerInfoAreaMetaInfo)dict.ValueOrDefault(infoAreaIdWithLink);
                    if (metaInfo == null)
                    {
                        metaInfo = new UPContainerInfoAreaMetaInfo(field.InfoAreaId, field.LinkId)
                        {
                            Position = ++infoAreaPosition
                        };

                        this.RootInfoAreaMetaInfo.AddTable(metaInfo);
                        dict[infoAreaIdWithLink] = metaInfo;
                        infoAreas.Add(metaInfo);
                    }

                    var fieldMetaInfo = new UPContainerFieldMetaInfo(field);
                    fieldMetaInfo.PositionInResult = metaInfo.AddField(fieldMetaInfo);
                    fieldMetaInfo.PositionInInfoArea = fieldMetaInfo.PositionInResult;
                    fieldMetaInfo.InfoAreaPosition = metaInfo.Position;
                    this.OutputFields.Add(fieldMetaInfo);
                }
            }

            infoAreaPosition = 0;
            foreach (UPContainerInfoAreaMetaInfo area in infoAreas)
            {
                area.ResultFieldOffset = infoAreaPosition;
                infoAreaPosition += area.FieldCount;
            }

            foreach (var outFields in this.OutputFields)
            {
                outFields.PositionInResult = ((UPContainerInfoAreaMetaInfo)infoAreas[outFields.InfoAreaPosition]).ResultFieldOffset + outFields.PositionInInfoArea;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="configQuery">The configuration query.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        public UPContainerMetaInfo(UPConfigQuery configQuery, Dictionary<string, object> filterParameters)
        {
            this.DatabaseInstance = UPCRMDataStore.DefaultStore.DatabaseInstance;
            configQuery = configQuery.QueryByApplyingValueDictionaryDefaults(filterParameters, true);
            if (configQuery == null)
            {
                return;
            }

            this.FillFromQuery(configQuery);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="configQuery">The configuration query.</param>
        public UPContainerMetaInfo(UPConfigQuery configQuery)
            : this(configQuery, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        public UPContainerMetaInfo(FieldControl fieldControl, UPConfigFilter filter, Dictionary<string, object> filterParameters)
        {
            this.InitWithFieldControl(fieldControl, filter, filterParameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        public UPContainerMetaInfo(FieldControl fieldControl)
            : this(fieldControl, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="searchAndList">The search and list.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <param name="controls">The controls.</param>
        public UPContainerMetaInfo(SearchAndList searchAndList, Dictionary<string, object> filterParameters, List<object> controls)
        {
            this.InitWithSearchAndListControl(searchAndList, filterParameters, controls);
        }

        /// <summary>
        /// Initializes the with field control.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        private void InitWithFieldControl(FieldControl fieldControl, UPConfigFilter filter, Dictionary<string, object> filterParameters)
        {
            this.DatabaseInstance = UPCRMDataStore.DefaultStore.DatabaseInstance;
            this.SourceFieldControl = fieldControl;
            this.FillFromFieldConfig(this.SourceFieldControl);
            this.SetSortFields(this.SourceFieldControl?.CrmSortFields);
            if (filter != null)
            {
                if (filterParameters != null)
                {
                    var replacement = new UPConditionValueReplacement(filterParameters);
                    filter = filter.FilterByApplyingReplacements(replacement);
                    if (filter == null)
                    {
                        return;
                    }
                }

                this.ApplyFilter(filter);
            }

            var maxResultString = this.SourceFieldControl?.ValueForAttribute("MaxResults");
            if (!string.IsNullOrEmpty(maxResultString))
            {
                this.MaxResults = int.Parse(maxResultString);
                if (this.MaxResults > 1)
                {
                    this.MaxResults--;
                }
            }
            else
            {
                this.MaxResults = 0;
            }
        }

        /// <summary>
        /// Initializes the with search and list control.
        /// </summary>
        /// <param name="searchAndList">The search and list.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <param name="controls">The controls.</param>
        private void InitWithSearchAndListControl(SearchAndList searchAndList, Dictionary<string, object> filterParameters, List<object> controls)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            FieldControl fieldControl = null;
            if (controls == null)
            {
                controls = new List<object> { "List" };
            }

            foreach (string controlName in controls)
            {
                fieldControl = configStore.FieldControlByNameFromGroup(controlName, searchAndList.FieldGroupName);
                if (fieldControl != null)
                {
                    break;
                }
            }

            var filter = !string.IsNullOrEmpty(searchAndList.FilterName) ? configStore.FilterByName(searchAndList.FilterName) : null;
            this.InitWithFieldControl(fieldControl, filter, filterParameters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="searchAndListName">Name of the search and list.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <param name="controls">The controls.</param>
        public UPContainerMetaInfo(string searchAndListName, Dictionary<string, object> filterParameters, List<object> controls)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var searchAndList = configStore.SearchAndListByName(searchAndListName);
            if (searchAndList != null)
            {
                this.InitWithSearchAndListControl(searchAndList, filterParameters, controls);
            }

            var fieldControl = configStore.FieldControlByNameFromGroup("List", searchAndListName);
            if (fieldControl == null)
            {
                return;
            }

            this.InitWithFieldControl(fieldControl, null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="searchAndListName">Name of the search and list.</param>
        public UPContainerMetaInfo(string searchAndListName)
            : this(searchAndListName, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="searchAndListName">Name of the search and list.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        public UPContainerMetaInfo(string searchAndListName, Dictionary<string, object> filterParameters)
            : this(searchAndListName, filterParameters, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="searchAndList">The search and list.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        public UPContainerMetaInfo(SearchAndList searchAndList, Dictionary<string, object> filterParameters)
        {
            this.InitWithSearchAndListControl(searchAndList, filterParameters, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPContainerMetaInfo"/> class.
        /// </summary>
        /// <param name="fieldArray">The field array.</param>
        /// <param name="rootInfoAreaId">The root information area identifier.</param>
        public UPContainerMetaInfo(List<UPCRMField> fieldArray, string rootInfoAreaId)
        {
            this.DatabaseInstance = UPCRMDataStore.DefaultStore.DatabaseInstance;
            this.FillFromFieldArray(fieldArray, rootInfoAreaId);
        }

        /// <summary>
        /// Fills the result information areas.
        /// </summary>
        public void FillResultInfoAreas()
        {
            this.resultInfoAreas = new List<UPContainerInfoAreaMetaInfo>();
            this.RootInfoAreaMetaInfo.InsertIntoResultInfoAreas(this.resultInfoAreas);
        }

        /// <summary>
        /// Results the index of the information area meta information at.
        /// </summary>
        /// <param name="nr">The nr.</param>
        /// <returns></returns>
        public UPContainerInfoAreaMetaInfo ResultInfoAreaMetaInfoAtIndex(int nr)
        {
            if (this.resultInfoAreas == null)
            {
                this.FillResultInfoAreas();
            }

            if (this.resultInfoAreas == null || this.resultInfoAreas?.Count <= nr)
            {
                // don't crash with wrong indexes
                return null;
            }

            return this.resultInfoAreas[nr];
        }

        /// <summary>
        /// Indexes the of result information area.
        /// </summary>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <returns></returns>
        public int IndexOfResultInfoArea(UPContainerInfoAreaMetaInfo infoAreaMetaInfo)
        {
            if (this.resultInfoAreas == null)
            {
                this.FillResultInfoAreas();
            }

            return this.resultInfoAreas?.IndexOf(infoAreaMetaInfo) ?? -1;
        }

        /// <summary>
        /// Indexes the of result information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <returns></returns>
        public int IndexOfResultInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            if (this.resultInfoAreas == null)
            {
                this.FillResultInfoAreas();
            }

            var count = this.resultInfoAreas?.Count;
            if (count.HasValue && count.Value == 0)
            {
                return -1;
            }

            for (var position = 0; position < count; position++)
            {
                var infoAreaMetaInfo = this.resultInfoAreas[position];
                if (infoAreaMetaInfo.InfoAreaId.Equals(infoAreaId) && (infoAreaMetaInfo.LinkId == linkId || (infoAreaMetaInfo.LinkId <= 0 && linkId <= 0)))
                {
                    return position;
                }
            }

            return -1;
        }

        /// <summary>
        /// Indexes the name of the of function.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <returns></returns>
        public int IndexOfFunctionName(string functionName)
        {
            return this.SourceFieldControl.ResultIndexOfFunctionName(functionName);
        }

        /// <summary>
        /// Positions for field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public int PositionForField(UPCRMField field)
        {
            var count = this.OutputFields.Count;
            for (var i = 0; i < count; i++)
            {
                var fieldMetaInfo = this.OutputFields[i];
                if (fieldMetaInfo.CrmField.IsEqualToField(field))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the specified add field contains field.
        /// </summary>
        /// <param name="addField">The add field.</param>
        /// <returns></returns>
        public UPContainerFieldMetaInfo ContainsField(UPCRMField addField)
        {
            if (addField == null || this.OutputFields == null)
            {
                return null;
            }

            foreach (var field in this.OutputFields)
            {
                if (field.CrmField.FieldId == addField.FieldId && field.InfoAreaId == addField.InfoAreaId &&
                    (field.CrmField.LinkId == addField.LinkId || (addField.LinkId <= 0 && field.CrmField.LinkId <= 0)))
                {
                    return field;
                }
            }

            return null;
        }

        /// <summary>
        /// Numbers the of result information area meta infos.
        /// </summary>
        /// <returns></returns>
        public int NumberOfResultInfoAreaMetaInfos()
        {
            if (this.resultInfoAreas == null)
            {
                this.FillResultInfoAreas();
            }

            return this.resultInfoAreas?.Count ?? 0;
        }

        /// <summary>
        /// Creates a new CRM query.
        /// </summary>
        /// <returns></returns>
        public DAL.Query NewQuery()
        {
            var rootQueryTreeItem = this.RootInfoAreaMetaInfo.AddtoParentItemCrmDatabaseOptions(
                null, this.DatabaseInstance, this.ReplaceCaseSensitiveCharacters ? 1 : 0);
            if (rootQueryTreeItem == null)
            {
                return null;
            }

            var query = new DAL.Query(rootQueryTreeItem, false);
            var currentSession = ServerSession.CurrentSession;
            //if (currentSession.UseSortLocale)
            //{
            //    query.SetCollation(currentSession.CustomSortLocale != null ? "UTF8CICL" : "UTF8CIDL");
            //}

            query.SortFixBySortInfoAndCode = currentSession.FixCatSortBySortInfo;
            query.SortVarBySortInfo = currentSession.VarCatSortBySortInfo;

            if (this.DisableVirtualLinks || currentSession.DisableVirtualLinks)
            {
                query.SetUseVirtualLinks(false);
            }

            if (this._linkRecordIdentification != null)
            {
                var infoAreaId = this._linkRecordIdentification.InfoAreaId();
                var recordId = this._linkRecordIdentification.RecordId();
                query.SetLinkRecord(infoAreaId, recordId, this._linkId);
            }

            if (this.sortFields != null)
            {
                foreach (var sortField in this.sortFields)
                {
                    query.AddSortField(sortField.InfoAreaId, sortField.LinkId, sortField.FieldId, !sortField.Ascending);
                }
            }

            if (this.MaxResults > 0)
            {
                query.MaxResultRowCount = this.MaxResults;// was + 1; There is no point I can find in having this +1 here.
            }

            this._query = query;
            return query;
        }

        /// <summary>
        /// Maps the record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public string MappedRecordIdentification(string recordIdentification)
        {
            return ServerSession.CurrentSession.RecordIdentificationMapper.MappedRecordIdentification(recordIdentification);
        }

        /// <summary>
        /// Reads the record.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPCRMResult ReadRecord(string recordIdentification)
        {
            this.ApplyTimeZone(ServerSession.CurrentSession?.TimeZone);
            recordIdentification = this.MappedRecordIdentification(recordIdentification.IsRecordIdentification() ?
                recordIdentification : this.RootInfoAreaMetaInfo.InfoAreaId.InfoAreaIdRecordId(recordIdentification));

            var recordId = recordIdentification.RecordId();
            var query = this.NewQuery();
            if (query == null)
            {
                return null;
            }

            query.RootTreeItem.AddRecordIdCondition(recordId);
            var resultSet = query.Execute();
            this.BuildResultMetaInfoResult(query, resultSet);
            var result = new UPCRMResult(this, resultSet);
#if PORTING
            if (UPLogSettings.LogResults())
            {
                result.Log();
            }
#endif
            result.Log();

            return result;
        }

        /// <summary>
        /// Reads the record.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation ReadRecord(string recordIdentification, ISearchOperationHandler theDelegate)
        {
            recordIdentification = this.MappedRecordIdentification(recordIdentification);
            var request = new RemoteSearchOperation(this, recordIdentification, theDelegate);
            ServerSession.CurrentSession.ExecuteRequest(request);
            return request;
        }

        /// <summary>
        /// Reads the record.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="option">The option.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation ReadRecord(string recordIdentification, UPRequestOption option, ISearchOperationHandler theDelegate)
        {
            this.ApplyTimeZone(ServerSession.CurrentSession?.TimeZone);
            recordIdentification = this.MappedRecordIdentification(recordIdentification);
            if ((option == UPRequestOption.Online || option == UPRequestOption.BestAvailable) && ServerSession.CurrentSession.ConnectedToServer)
            {
                return this.ReadRecord(recordIdentification, theDelegate);
            }

            if (option == UPRequestOption.Offline || option == UPRequestOption.BestAvailable)
            {
                var recordId = recordIdentification.RecordId();
                var query = this.NewQuery();
                if (query == null)
                {
                    return null;
                }

                query.RootTreeItem.AddRecordIdCondition(recordId);
                var operation = new UPLocalSearchOperation(query, this, theDelegate);
                ServerSession.CurrentSession.ExecuteRequest(operation);
                return operation;
            }

            if (option == UPRequestOption.FastestAvailable)
            {
                var localResult = this.ReadRecord(recordIdentification);
                if (localResult.RowCount > 0)
                {
                    var operation = new UPLocalSearchOperationWithResult(localResult, this, theDelegate);
                    ServerSession.CurrentSession.ExecuteRequest(operation);
                    return operation;
                }
                else
                {
                    var recordId = recordIdentification.RecordId();
                    var query = this.NewQuery();
                    if (query == null)
                    {
                        return null;
                    }

                    query.RootTreeItem.AddRecordIdCondition(recordId);
                    var operation = new UPLocalSearchOperation(query, this, theDelegate);
                    ServerSession.CurrentSession.ExecuteRequest(operation);
                    return operation;
                }
            }

            return null;
        }

        /// <summary>
        /// Counts the specified the delegate.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation Count(ISearchOperationHandler theDelegate)
        {
            var request = !string.IsNullOrEmpty(this._linkRecordIdentification) ?
                new RemoteCountOperation(this, this._linkRecordIdentification, this._onlineLinkId, theDelegate) :
                new RemoteCountOperation(this, theDelegate);

            ServerSession.CurrentSession.ExecuteRequest(request);
            return request;
        }

        /// <summary>
        /// Counts the delegate.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation CountTheDelegate(UPRequestOption option, ISearchOperationHandler theDelegate)
        {
            if ((option == UPRequestOption.Online || option == UPRequestOption.BestAvailable) &&
                ServerSession.CurrentSession.ConnectedToServer)
            {
                return this.Count(theDelegate);
            }

            if (option == UPRequestOption.Offline || option == UPRequestOption.BestAvailable)
            {
                var query = this.NewQuery();
                if (query == null)
                {
                    return null;
                }

                var operation = new UPLocalCountOperation(query, this, theDelegate);
                ServerSession.CurrentSession.ExecuteRequest(operation);
                return operation;
            }

            if (option == UPRequestOption.FastestAvailable)
            {
                var localCount = this.Count();
                if (localCount > 0 || !ServerSession.CurrentSession.ConnectedToServer)
                {
                    var operation = new UPLocalCountOperationWithResult(localCount, this, theDelegate);
                    ServerSession.CurrentSession.ExecuteRequest(operation);
                    return operation;
                }

                return this.Count(theDelegate);
            }

            return null;
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            var query = this.NewQuery();
            return query?.Count() ?? 0;
        }

        /// <summary>
        /// Builds the result meta information result.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public bool BuildResultMetaInfoResult(DAL.Query query, object result)
        {
            if (this.resultInfoAreas == null)
            {
                this.FillResultInfoAreas();
            }

            if (this.resultInfoAreas == null)
            {
                return false;
            }

            var dict = new Dictionary<string, object>();
            foreach (var table in this.resultInfoAreas)
            {
                var resultTableArray = dict.ValueOrDefault(table.InfoAreaIdWithLink) as List<object>;
                if (resultTableArray != null)
                {
                    resultTableArray.Add(table);
                }
                else
                {
                    resultTableArray = new List<object> { table };
                    dict.SetObjectForKey(resultTableArray, table.InfoAreaIdWithLink);
                }
            }

            foreach (var t in this.resultInfoAreas)
            {
                t.BuildResultMetaInfoForQuery(query);
            }

            if (this.OutputFields == null)
            {
                return true;
            }

            foreach (var fieldMetaInfo in this.OutputFields)
            {
                var resultInfoAreaArray = dict.ValueOrDefault(fieldMetaInfo.InfoAreaIdWithLinkIgnoreParent) as List<object>;
                UPContainerInfoAreaMetaInfo infoAreaMetaInfo = null;
                if (resultInfoAreaArray.Count > 0)
                {
                    infoAreaMetaInfo = (UPContainerInfoAreaMetaInfo)resultInfoAreaArray[0];
                    var count = resultInfoAreaArray.Count;
                    if (count > 1)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var table = resultInfoAreaArray[i] as UPContainerInfoAreaMetaInfo;
                            if (table?.Position == fieldMetaInfo.InfoAreaPosition)
                            {
                                infoAreaMetaInfo = table;
                                break;
                            }
                        }
                    }
                }

                fieldMetaInfo.PositionInResult = fieldMetaInfo.PositionInInfoArea + infoAreaMetaInfo.OutputColumnStartIndex;
            }

            return true;
        }

        /// <summary>
        /// Outputs the field.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPContainerFieldMetaInfo OutputField(int index)
        {
            return this.OutputFields.Count <= index ? null : this.OutputFields[index];
        }

        /// <summary>
        /// Outputs the field count.
        /// </summary>
        /// <returns></returns>
        public int OutputFieldCount()
        {
            return this.OutputFields.Count;
        }

        /// <summary>
        /// Finds this instance.
        /// </summary>
        /// <returns></returns>
        public UPCRMResult Find()
        {
            this.ApplyTimeZone(ServerSession.CurrentSession?.TimeZone);
            var query = this.NewQuery();

            var resultSet = query?.Execute();
            if (resultSet == null)
            {
                return null;
            }

            this.BuildResultMetaInfoResult(query, resultSet);
            var result = new UPCRMResult(this, resultSet);
#if PORTING
            if (UPLogSettings.LogResults())
            {
                result.Log();
            }
#endif
            result.Log();
            return result;
        }

        /// <summary>
        /// Finds the specified option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="skipLocalMerge">if set to <c>true</c> [skip local merge].</param>
        /// <returns></returns>
        public Operation Find(UPRequestOption option, ISearchOperationHandler theDelegate, bool skipLocalMerge = false)
        {
            if ((option == UPRequestOption.Online || option == UPRequestOption.BestAvailable) && ServerSession.CurrentSession.ConnectedToServer)
            {
                return skipLocalMerge ? this.Find(theDelegate, true, null) : this.Find(theDelegate);
            }

            if (option == UPRequestOption.Offline || option == UPRequestOption.BestAvailable)
            {
                this.ApplyTimeZone(ServerSession.CurrentSession?.TimeZone);
                var query = this.NewQuery();
                if (query == null)
                {
                    return null;
                }

                var operation = new UPLocalSearchOperation(query, this, theDelegate);
                ServerSession.CurrentSession.ExecuteRequest(operation);
                return operation;
            }

            if (option == UPRequestOption.FastestAvailable)
            {
                var localResult = this.Find();
                if ((localResult != null && localResult.RowCount > 0) || !ServerSession.CurrentSession.ConnectedToServer)
                {
                    var operation = new UPLocalSearchOperationWithResult(localResult, this, theDelegate);
                    ServerSession.CurrentSession.ExecuteRequest(operation);
                    return operation;
                }

                if (localResult == null)
                {
                    localResult = UPCRMResult.EmptyClientResult();
                }

                return this.Find(theDelegate, false, localResult);
            }

            return null;
        }

        /// <summary>
        /// Finds the specified the delegate.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation Find(ISearchOperationHandler theDelegate)
        {
            return this.Find(theDelegate, this.SkipResultMerge, null);
        }

        /// <summary>
        /// Finds the specified the delegate.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="skipMerge">if set to <c>true</c> [_skip merge].</param>
        /// <param name="localResult">The local result.</param>
        /// <returns></returns>
        public Operation Find(ISearchOperationHandler theDelegate, bool skipMerge, UPCRMResult localResult)
        {
            this.ApplyTimeZone(ServerSession.CurrentSession?.TimeZone);
            RemoteSearchOperation request = !string.IsNullOrEmpty(this._linkRecordIdentification)
                ? new RemoteSearchOperation(this, this._linkRecordIdentification, this._onlineLinkId, theDelegate)
                : new RemoteSearchOperation(this, theDelegate);

            request.SkipLocalMerge = skipMerge;
            if (!skipMerge)
            {
                request.LocalMergeResult = localResult;
            }

            ServerSession.CurrentSession.ExecuteRequest(request);
            return request;
        }

        /// <summary>
        /// Sorts the field to array.
        /// </summary>
        /// <param name="sortField">The sort field.</param>
        /// <returns></returns>
        public List<object> SortFieldToArray(UPCRMSortField sortField)
        {
            return new List<object> { sortField.FieldId, !sortField.Ascending, sortField.InfoAreaId };
        }

        /// <summary>
        /// Queries to object.
        /// </summary>
        /// <returns></returns>
        public object QueryToObject()
        {
            var treeItemArray = this.RootInfoAreaMetaInfo?.TreeNodeToObject();

            treeItemArray = new List<object> { treeItemArray };
            var count = this.sortFields?.Count ?? 0;
            if (count <= 0)
            {
                return new List<object> { treeItemArray, null };
            }

            var sortFieldArray = new List<object>();
            for (var i = 0; i < count; i++)
            {
                sortFieldArray.Add(this.SortFieldToArray(this.sortFields[i]));
            }

            return new List<object> { treeItemArray, sortFieldArray };
        }

        /// <summary>
        /// Adds the sort field ascending.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        public void AddSortFieldAscending(UPCRMField field, bool ascending)
        {
            this.AddSortField(new UPCRMSortField(field, ascending));
        }

        /// <summary>
        /// Adds the sort field.
        /// </summary>
        /// <param name="field">The field.</param>
        public void AddSortField(UPCRMSortField field)
        {
            if (this.sortFields == null)
            {
                this.sortFields = new List<UPCRMSortField>();
            }

            this.sortFields.Add(field);
        }

        /// <summary>
        /// Sets the sort fields.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public void SetSortFields(List<UPCRMSortField> fields)
        {
            if (fields == null)
            {
                this.sortFields = null;
            }
            else
            {
                this.sortFields = new List<UPCRMSortField>(fields.Count);
                foreach (var sortField in fields)
                {
                    this.AddSortField(sortField);
                }
            }
        }

        /// <summary>
        /// Shorts the value from raw value for column.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="columnindex">The columnindex.</param>
        /// <returns></returns>
        public string ShortValueFromRawValueForColumn(string rawValue, int columnindex)
        {
            var fieldMetaInfo = this.OutputField(columnindex);
            return fieldMetaInfo == null ? rawValue : fieldMetaInfo.ShortValueFromRawValue(rawValue, 0);
        }

        /// <summary>
        /// Values from raw value for column.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="columnindex">The columnindex.</param>
        /// <returns></returns>
        public string ValueFromRawValueForColumn(string rawValue, int columnindex)
        {
            var fieldMetaInfo = this.OutputField(columnindex);
            return fieldMetaInfo == null ? rawValue : fieldMetaInfo.ValueFromRawValue(rawValue);
        }

        /// <summary>
        /// Values from raw value for column options.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="columnindex">The columnindex.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public string ValueFromRawValueForColumnOptions(string rawValue, int columnindex, UPFormatOption options)
        {
            var fieldMetaInfo = this.OutputField(columnindex);
            return fieldMetaInfo == null ? rawValue : fieldMetaInfo.ValueFromRawValue(rawValue, options);
        }

        /// <summary>
        /// Reports the value from raw value for column.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="columnindex">The columnindex.</param>
        /// <returns></returns>
        public string ReportValueFromRawValueForColumn(string rawValue, int columnindex)
        {
            var fieldMetaInfo = this.OutputField(columnindex);
            return fieldMetaInfo == null ? rawValue : fieldMetaInfo.ReportValueFromRawValue(rawValue);
        }

        /// <summary>
        /// Sets the link record identification with link identifier online link identifier.
        /// </summary>
        /// <param name="linkRecordIdentification">The link record identification.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="onlineLinkId">The online link identifier.</param>
        public void SetLinkRecordIdentification(string linkRecordIdentification, int linkId, int onlineLinkId)
        {
            this._linkRecordIdentification = this.MappedRecordIdentification(linkRecordIdentification);
            this._linkId = linkId;
            this._onlineLinkId = onlineLinkId;
        }

        /// <summary>
        /// Sets the link record identification with link identifier.
        /// </summary>
        /// <param name="linkRecordIdentification">The link record identification.</param>
        /// <param name="linkId">The link identifier.</param>
        public void SetLinkRecordIdentification(string linkRecordIdentification, int linkId)
        {
            var onlineLinkId = linkId;
            if (linkId > 0)
            {
                var linkInfo = UPCRMDataStore.DefaultStore.LinkInfoForInfoAreaTargetInfoAreaLinkId(
                    this.RootInfoAreaMetaInfo.InfoAreaId, linkRecordIdentification.InfoAreaId(), linkId);

                if (linkInfo != null)
                {
                    onlineLinkId = linkInfo.ReverseLinkId;
                }
            }

            this.SetLinkRecordIdentification(linkRecordIdentification, linkId, onlineLinkId);
        }

        /// <summary>
        /// Sets the link record identification.
        /// </summary>
        /// <param name="linkRecordIdentification">The link record identification.</param>
        public void SetLinkRecordIdentification(string linkRecordIdentification)
        {
            this.SetLinkRecordIdentification(linkRecordIdentification, -1, -1);
        }

        /// <summary>
        /// Fields at position.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPContainerFieldMetaInfo FieldAtPosition(int index)
        {
            if (this.OutputFields.Count <= index)
            {
                // prevent crash with invalid indexes
                return null;
            }

            return this.OutputFields[index];
        }

        /// <summary>
        /// Applies the filter with replacement dictionary.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public bool ApplyFilterWithReplacementDictionary(UPConfigFilter filter, Dictionary<string, object> dictionary)
        {
            if (dictionary == null || filter == null)
            {
                return this.ApplyFilter(filter);
            }

            var replacements = new UPConditionValueReplacement(dictionary);
            var replacedFilter = filter.FilterByApplyingReplacements(replacements);
            return replacedFilter == null || this.ApplyFilter(replacedFilter);
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool ApplyFilter(UPConfigFilter filter)
        {
            if (filter == null)
            {
                return true;
            }

            var replacements = UPConditionValueReplacement.DefaultParameters;
            var replacedFilter = filter.FilterByApplyingReplacements(replacements);
            return replacedFilter == null || this.RootInfoAreaMetaInfo.ApplyFilter(replacedFilter);
        }

        /// <summary>
        /// Simples the set search conditions on search value on fields.
        /// </summary>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <param name="searchValue">The search value.</param>
        /// <param name="crmFields">The CRM fields.</param>
        public void SimpleSetSearchConditionsOnSearchValueOnFields(UPContainerInfoAreaMetaInfo infoAreaMetaInfo,
            string searchValue, List<UPCRMField> crmFields)
        {
            if (crmFields == null)
            {
                return;
            }

            foreach (var field in crmFields)
            {
                infoAreaMetaInfo.AddConditionWithOr(new UPInfoAreaConditionLeaf(infoAreaMetaInfo.InfoAreaId,
                    field.FieldId, "=", searchValue));
            }
        }

        /// <summary>
        /// Multis the search conditions on search values on fields.
        /// </summary>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <param name="searchValueParts">The search value parts.</param>
        /// <param name="crmFields">The CRM fields.</param>
        public void MultiSearchConditionsOnSearchValuesOnFields(UPContainerInfoAreaMetaInfo infoAreaMetaInfo,
            List<string> searchValueParts, List<UPCRMField> crmFields)
        {
            if (crmFields == null || searchValueParts == null)
            {
                return;
            }

            foreach (var field in crmFields)
            {
                UPInfoAreaCondition fieldCondition = null;
                foreach (var part in searchValueParts)
                {
                    var cond = new UPInfoAreaConditionLeaf(infoAreaMetaInfo.InfoAreaId, field.FieldId, "=", $"*{part}*");
                    fieldCondition = fieldCondition == null
                        ? cond
                        : fieldCondition.InfoAreaConditionByAppendingAndCondition(cond);
                }

                infoAreaMetaInfo.AddConditionWithOr(fieldCondition);
            }
        }

        /// <summary>
        /// Conditions the with values fields map.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="map">The map.</param>
        /// <returns></returns>
        public UPInfoAreaCondition ConditionWithValuesFieldsMap(List<string> values, List<UPCRMField> fields, List<int> map)
        {
            UPInfoAreaCondition condition = null;
            int count = map.Count;
            for (int i = 0; i < count; i++)
            {
                if (values[i] == null)
                {
                    continue;
                }

                var field = fields[map[i]];
                var leafCondition = new UPInfoAreaConditionLeaf(field.InfoAreaId, field.FieldId, "=", values[i]);
                condition = condition == null
                    ? leafCondition
                    : condition.InfoAreaConditionByAppendingAndCondition(leafCondition);
            }

            return condition;
        }

        /// <summary>
        /// Conditions the by iterating Start index values fields.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="startIndex">The Start index.</param>
        /// <param name="values">The values.</param>
        /// <param name="crmFields">The CRM fields.</param>
        /// <returns></returns>
        public UPInfoAreaCondition ConditionByIteratingStartIndexValuesFields(List<int> map, int startIndex,
            List<string> values, List<UPCRMField> crmFields)
        {
            int count = map.Count;
            if (startIndex + 1 == count)
            {
                return this.ConditionWithValuesFieldsMap(values, crmFields, map);
            }

            var condition = this.ConditionByIteratingStartIndexValuesFields(map, startIndex + 1, values, crmFields);
            for (int i = startIndex + 1; i < count; i++)
            {
                var src = map[startIndex];
                map[startIndex] = map[i];
                map[i] = src;
                condition =
                    condition.InfoAreaConditionByAppendingOrCondition(this.ConditionByIteratingStartIndexValuesFields(map,
                        startIndex + 1, values, crmFields));
                map[i] = map[0];
                map[startIndex] = src;
            }

            return condition;
        }

        /// <summary>
        /// Dates the field search conditions for search value on fields.
        /// </summary>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <param name="searchValue">The search value.</param>
        /// <param name="crmFields">The CRM fields.</param>
        /// <returns></returns>
        public UPInfoAreaCondition DateFieldSearchConditionsForSearchValueOnFields(
            UPContainerInfoAreaMetaInfo infoAreaMetaInfo, string searchValue, List<UPCRMField> crmFields)
        {
            UPInfoAreaCondition rootCondition = null;
            UPDateDetectorResult dateResult = searchValue.DetectAllDates(UPDateDetectorOptions.RangeDisabled);
            if (dateResult != null)
            {
                foreach (var searchField in crmFields)
                {
                    foreach (var date in dateResult.AllDates)
                    {
                        UPInfoAreaCondition currentCondition = new UPInfoAreaConditionLeaf(searchField.InfoAreaId,
                            searchField.FieldId, StringExtensions.CrmValueFromDate(date));
                        rootCondition = rootCondition == null
                            ? currentCondition
                            : rootCondition.InfoAreaConditionByAppendingOrCondition(currentCondition);
                    }
                }
            }

            return rootCondition;
        }

        /// <summary>
        /// Twoes the field search conditions on search values on fields.
        /// </summary>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="crmFields">The CRM fields.</param>
        public void TwoFieldSearchConditionsOnSearchValuesOnFields(UPContainerInfoAreaMetaInfo infoAreaMetaInfo,
            List<string> parts, List<UPCRMField> crmFields)
        {
            if (crmFields.Count != 2)
            {
                return;
            }

            var startStringParts = new List<object>(parts.Count + 1);
            var endStringParts = new List<object>(parts.Count + 1);
            int count = parts.Count;
            var startString = string.Empty;
            var endString = string.Empty;
            startStringParts.Add(startString);
            endStringParts.Add(endString);
            for (int i = 0; i < count; i++)
            {
                startString += parts[i];
                startStringParts.Add(startString);
                endString = i == 0 ? parts[count - 1] : $"{parts[count - i - 1]}{endString}";

                endStringParts.Add(endString);
            }

            var leftField = crmFields[0];
            var rightField = crmFields[1];
            var leftFieldId = leftField.FieldId;
            var rightFieldId = rightField.FieldId;
            var fieldInfoAreaId = leftField.InfoAreaId;
            UPInfoAreaCondition rootCondition = null;
            for (int i = 0; i <= count; i++)
            {
                startString = startStringParts[i] as string;
                endString = endStringParts[count - i] as string;
                UPInfoAreaCondition currentCondition;
                if (!string.IsNullOrEmpty(startString))
                {
                    currentCondition = new UPInfoAreaConditionLeaf(fieldInfoAreaId, rightFieldId, endString);
                }
                else if (!string.IsNullOrEmpty(endString))
                {
                    currentCondition = new UPInfoAreaConditionLeaf(fieldInfoAreaId, leftFieldId, startString);
                }
                else
                {
                    currentCondition = new UPInfoAreaConditionLeaf(fieldInfoAreaId, leftFieldId, startString);
                    currentCondition = currentCondition.InfoAreaConditionByAppendingAndCondition(
                            new UPInfoAreaConditionLeaf(fieldInfoAreaId, rightFieldId, endString));
                }

                rootCondition = rootCondition == null
                    ? currentCondition
                    : rootCondition.InfoAreaConditionByAppendingOrCondition(currentCondition);
            }
            
            if (rootCondition != null)
            {
                UPInfoAreaCondition currentCondition = new UPInfoAreaConditionLeaf(fieldInfoAreaId, leftFieldId, startStringParts[count - 1].ToString());
                rootCondition.InfoAreaConditionByAppendingOrCondition(currentCondition);
                infoAreaMetaInfo.AddCondition(rootCondition);
            }
        }

        /// <summary>
        /// Mixeds the set search conditions on search values on fields.
        /// </summary>
        /// <param name="infoAreaMetaInfo">The information area meta information.</param>
        /// <param name="parts">The parts.</param>
        /// <param name="crmFields">The CRM fields.</param>
        public void MixedSetSearchConditionsOnSearchValuesOnFields(UPContainerInfoAreaMetaInfo infoAreaMetaInfo,
            List<string> parts, List<UPCRMField> crmFields)
        {
            var partCount = parts.Count;
            var fieldCount = crmFields.Count;
            if (partCount < fieldCount)
            {
                var arr = new List<string>(parts);
                for (; partCount < fieldCount; ++partCount)
                {
                    arr.Add(null);
                }

                parts = arr;
            }

            var map = new List<int>(fieldCount);
            for (var i = 0; i < fieldCount; i++)
            {
                map.Add(i);
            }

            infoAreaMetaInfo.AddCondition(this.ConditionByIteratingStartIndexValuesFields(map, 0, parts.ToList(), crmFields));
        }

        /// <summary>
        /// CRMs the fields with adjusted rep fields.
        /// </summary>
        /// <param name="crmFields">The CRM fields.</param>
        /// <returns></returns>
        public List<UPCRMField> CrmFieldsWithAdjustedRepFields(List<UPCRMField> crmFields)
        {
            if (crmFields == null)
            {
                return null;
            }

            var adjustedFields = new List<UPCRMField>(crmFields.Count);
            var dataStore = UPCRMDataStore.DefaultStore;
            foreach (var field in crmFields)
            {
                if (field == null)
                {
                    continue;
                }

                var fieldInfo = dataStore.FieldInfoForField(field);
                if (fieldInfo != null)
                {
                    var referencedZRepField = fieldInfo.ReferencedRepZField;
                    adjustedFields.Add(referencedZRepField >= 0
                        ? UPCRMField.FieldWithFieldIdInfoAreaId(referencedZRepField, field.InfoAreaId)
                        : field);
                }
                else
                {
                    adjustedFields.Add(field);
                }
            }

            return adjustedFields;
        }

        /// <summary>
        /// Sets the search conditions for.
        /// </summary>
        /// <param name="searchValue">The search value.</param>
        /// <param name="crmFields">The CRM fields.</param>
        /// <param name="fullTextSearch">if set to <c>true</c> [full text search].</param>
        public void SetSearchConditionsFor(string searchValue, List<UPCRMField> crmFields, bool fullTextSearch)
        {
            this.SetSearchConditionsFor(searchValue, crmFields, null, fullTextSearch);
        }

        /// <summary>
        /// Sets the search conditions for.
        /// </summary>
        /// <param name="searchValue">The search value.</param>
        /// <param name="crmFields">The CRM fields.</param>
        /// <param name="multiSearchFields">The multi search fields.</param>
        /// <param name="fullTextSearch">if set to <c>true</c> [full text search].</param>
        public void SetSearchConditionsFor(string searchValue,
            List<UPCRMField> crmFields,
            List<UPCRMField> multiSearchFields, bool fullTextSearch)
        {
            if (searchValue == null)
            {
                return;
            }

            crmFields = this.CrmFieldsWithAdjustedRepFields(crmFields) ?? new List<UPCRMField>();

            searchValue = searchValue.Trim();
            var dateSearchValue = searchValue;
            var disable78152 = ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Disable.78152");
            List<UPCRMField> linkedFields = null;
            List<UPCRMField> infoAreaFields = null;
            List<UPCRMField> infoAreaDateFields = null;
            foreach (var field in crmFields)
            {
                if (field.InfoAreaId != this.RootInfoAreaMetaInfo.InfoAreaId || field.LinkId > 0)
                {
                    if (linkedFields == null)
                    {
                        linkedFields = new List<UPCRMField>();
                    }

                    linkedFields.Add(field);
                }
                else
                {
                    if (field.FieldType == "D")
                    {
                        if (infoAreaDateFields == null)
                        {
                            infoAreaDateFields = new List<UPCRMField> { field };
                        }
                        else
                        {
                            infoAreaDateFields.Add(field);
                        }
                    }

                    if (infoAreaFields == null)
                    {
                        infoAreaFields = new List<UPCRMField> { field };
                    }
                    else
                    {
                        infoAreaFields.Add(field);
                    }
                }
            }

            string[] parts = null;
            //if (crmFields.Count < 4)
            {
                var explicitParts = false;
                if (disable78152)
                {
                    parts = searchValue.Split(' ');
                }
                else
                {
                    parts = searchValue.Split(',');
                    if (parts.Length == 1)
                    {
                        parts = searchValue.Split(' ');
                    }
                    else
                    {
                        explicitParts = true;
                        var trimmedParts = new List<string>(parts.Length);
                        foreach (var part in parts)
                        {
                            var trimmedPart = part.Trim();
                            var parts2 = trimmedPart.Split(' ');
                            if (parts2.Length > 1)
                            {
                                trimmedPart = null;
                                foreach (var part2 in parts2)
                                {
                                    if (trimmedPart == null)
                                    {
                                        trimmedPart = fullTextSearch ? $"*{part2}*" : $"{part2}*";
                                    }
                                    else
                                    {
                                        trimmedPart += $"{part2}*";
                                    }
                                }
                            }

                            if (trimmedPart != null)
                            {
                                trimmedParts.Add(trimmedPart);
                            }
                        }

                        parts = trimmedParts.ToArray();
                    }
                }

                //if (parts.Length == 1 || (parts.Length > infoAreaFields?.Count))
                //{
                //    if (explicitParts || parts.Length > 4 || infoAreaFields?.Count > 2 || disable78152)
                //    {
                //        parts = null;
                //    }
                //}
            }

            if (disable78152)
            {
                searchValue = fullTextSearch ? $"*{searchValue}*" : $"{searchValue}*";
            }
            else
            {
                var searchValueParts = searchValue.Split(' ');
                searchValue = null;
                foreach (var part in searchValueParts)
                {
                    if (searchValue == null)
                    {
                        searchValue = fullTextSearch ? $"*{part}*" : $"{part}*";
                    }
                    else
                    {
                        searchValue += $"{part}*";
                    }
                }
            }

            UPContainerInfoAreaMetaInfo infoAreaMetaInfo;
            if (linkedFields != null)
            {
                var subMetaInfos = new Dictionary<string, object>();
                foreach (var field in linkedFields)
                {
                    infoAreaMetaInfo = subMetaInfos.ValueOrDefault(field.InfoAreaIdWithLink) as UPContainerInfoAreaMetaInfo;
                    if (infoAreaMetaInfo == null)
                    {
                        infoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(field.InfoAreaId, field.LinkId, "HAVINGOPTIONAL");
                        this.RootInfoAreaMetaInfo.AddTable(infoAreaMetaInfo);
                        subMetaInfos.SetObjectForKey(infoAreaMetaInfo, field.InfoAreaIdWithLink);
                    }

                    infoAreaMetaInfo.AddConditionWithOr(new UPInfoAreaConditionLeaf(infoAreaMetaInfo.InfoAreaId,
                        field.FieldId, "=", searchValue));
                }

                if (infoAreaFields != null)
                {
                    infoAreaMetaInfo = new UPContainerInfoAreaMetaInfo(this.RootInfoAreaMetaInfo.InfoAreaId, "HAVINGOPTIONAL");
                    this.RootInfoAreaMetaInfo.AddTable(infoAreaMetaInfo);
                }
                else
                {
                    infoAreaMetaInfo = null;
                }
            }
            else
            {
                infoAreaMetaInfo = this.RootInfoAreaMetaInfo;
            }

            if (infoAreaFields != null)
            {
                if (parts == null || !parts.Any())
                {
                    this.SimpleSetSearchConditionsOnSearchValueOnFields(infoAreaMetaInfo, searchValue, infoAreaFields);
                }
                else
                {
                    var arr = new List<string>(parts.Length);
                    arr.AddRange(parts.Select(t => fullTextSearch ? $"*{t}*" : $"{t}*"));

                    parts = arr.ToArray();
                    if (parts.Length > 2 && infoAreaFields.Count == 2)
                    {
                        this.TwoFieldSearchConditionsOnSearchValuesOnFields(infoAreaMetaInfo, parts.ToList(), infoAreaFields);
                    }
                    else if (infoAreaFields.Count == 1)
                    {
                        var combined = parts[0];
                        for (var i = 1; i < parts.Length; i++)
                        {
                            combined += $"{parts[i]}";
                        }

                        this.SimpleSetSearchConditionsOnSearchValueOnFields(infoAreaMetaInfo, combined, infoAreaFields);
                    }
                    else
                    {
                        if (parts.Length > 1)
                        {
                            this.MixedSetSearchConditionsOnSearchValuesOnFields(infoAreaMetaInfo, parts.ToList(), infoAreaFields);
                        }

                        this.SimpleSetSearchConditionsOnSearchValueOnFields(infoAreaMetaInfo, searchValue, infoAreaFields);
                    }
                }
            }

            if (infoAreaDateFields != null && infoAreaDateFields.Count > 0)
            {
                var dateRootCondition = this.DateFieldSearchConditionsForSearchValueOnFields(infoAreaMetaInfo,
                    dateSearchValue, infoAreaDateFields);
                if (dateRootCondition != null)
                {
                    infoAreaMetaInfo.AddConditionWithOr(dateRootCondition);
                }
            }

            if (multiSearchFields?.Count <= 0)
            {
                return;
            }

            if (parts == null || parts.Length < 2)
            {
                searchValue = $"*{searchValue}";
                this.SimpleSetSearchConditionsOnSearchValueOnFields(infoAreaMetaInfo, searchValue, multiSearchFields);
            }
            else
            {
                this.MultiSearchConditionsOnSearchValuesOnFields(infoAreaMetaInfo, parts.ToList(), multiSearchFields);
            }
        }

        /// <summary>
        /// News the record with link link identifier record identifier.
        /// </summary>
        /// <param name="linkRecordId">The link record identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="newRecordId">The new record identifier.</param>
        /// <returns></returns>
        public UPCRMResult NewRecordWithLinkLinkIdRecordId(string linkRecordId, int linkId, string newRecordId)
        {
            // TODO: handle link, handle new Record Id
            return new UPCRMResult(this, this.RootInfoAreaMetaInfo.InfoAreaId.InfoAreaIdRecordId(newRecordId),
                linkRecordId, linkId);
        }

        /// <summary>
        /// News the record with link link identifier.
        /// </summary>
        /// <param name="linkRecordId">The link record identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <returns></returns>
        public UPCRMResult NewRecordWithLinkLinkId(string linkRecordId, int linkId)
        {
            return this.NewRecordWithLinkLinkIdRecordId(linkRecordId, linkId, "new");
        }

        /// <summary>
        /// News the record with record identifier.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <returns></returns>
        public UPCRMResult NewRecordWithRecordId(string recordId)
        {
            return this.NewRecordWithLinkLinkIdRecordId(null, -1, recordId);
        }

        /// <summary>
        /// News the record.
        /// </summary>
        /// <returns></returns>
        public UPCRMResult NewRecord()
        {
            return this.NewRecordWithRecordId("new");
        }

        /// <summary>
        /// Gets a value indicating whether this instance has multiple output information areas.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has multiple output information areas; otherwise, <c>false</c>.
        /// </value>
        public bool HasMultipleOutputInfoAreas => this.RootInfoAreaMetaInfo?.HasOutputChildInfoAreas ?? false;

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">The time zone.</param>
        public void ApplyTimeZone(UPCRMTimeZone timeZone)
        {
            if (!this._timeZoneApplied && timeZone != null)
            {
                this.RootInfoAreaMetaInfo.ApplyTimeZone(timeZone, new List<UPContainerFieldMetaInfo>());
                this.RootInfoAreaMetaInfo.AddFieldsToArray(new List<object>());
            }

            this._timeZoneApplied = true;
        }

        #region UPCRMDataSourceMetaInfo

        /// <summary>
        /// Numbers the of result tables.
        /// </summary>
        /// <returns></returns>
        public virtual int NumberOfResultTables => this.NumberOfResultInfoAreaMetaInfos();

        /// <summary>
        /// Results the index of the table at.
        /// </summary>
        /// <param name="tableIndex">Index of the table.</param>
        /// <returns></returns>
        public ICrmDataSourceTable ResultTableAtIndex(int tableIndex)
        {
            return this.ResultInfoAreaMetaInfoAtIndex(tableIndex);
        }

        #endregion

        private static void AddContainerInfoAreaMetaInfo(FieldControl fieldControl, Dictionary<string, object> dict, Dictionary<string, object> treeDict, UPContainerMetaInfoAreaTreeNode rootTreeNode)
        {
            if (fieldControl.Tabs == null)
            {
                return;
            }

            foreach (var tab in fieldControl.Tabs)
            {
                if (tab.Fields == null)
                {
                    continue;
                }

                foreach (var configField in tab.Fields)
                {
                    var field = configField.Field;
                    var infoAreaIdWithLink = field.InfoAreaIdWithLink;
                    var metaInfo = (UPContainerInfoAreaMetaInfo)dict.ValueOrDefault(infoAreaIdWithLink);
                    if (metaInfo != null)
                    {
                        continue;
                    }

                    var parentInfoAreaMetaInfoTreeNode = rootTreeNode;
                    if (field.ParentLink != null)
                    {
                        var parentPath = field.ParentLink.ParentLinkPath();
                        foreach (var parentLink in parentPath)
                        {
                            var treeNode = (UPContainerMetaInfoAreaTreeNode)treeDict.ValueOrDefault(parentLink.InfoAreaIdWithLink);
                            if (treeNode == null)
                            {
                                metaInfo = new UPContainerInfoAreaMetaInfo(parentLink.InfoAreaId, parentLink.LinkId);
                                parentInfoAreaMetaInfoTreeNode.Node.AddTable(metaInfo);
                                dict[parentLink.InfoAreaIdWithLink] = metaInfo;
                                treeNode = parentInfoAreaMetaInfoTreeNode.AddChildNodeKey(metaInfo, parentLink.InfoAreaIdWithLink);
                                treeDict.SetObjectForKey(treeNode, treeNode.Key);
                            }

                            parentInfoAreaMetaInfoTreeNode = treeNode;
                        }
                    }

                    metaInfo = new UPContainerInfoAreaMetaInfo(field.InfoAreaId, field.LinkId);
                    parentInfoAreaMetaInfoTreeNode.Node.AddTable(metaInfo);
                    dict.SetObjectForKey(metaInfo, infoAreaIdWithLink);
                    parentInfoAreaMetaInfoTreeNode = parentInfoAreaMetaInfoTreeNode.AddChildNodeKey(metaInfo, infoAreaIdWithLink);
                    treeDict.SetObjectForKey(parentInfoAreaMetaInfoTreeNode, parentInfoAreaMetaInfoTreeNode.Key);
                }
            }
        }

        private void AddContainerFieldMetaInfo(FieldControl fieldControl, Dictionary<string, object> dict)
        {
            if (fieldControl.Tabs == null)
            {
                return;
            }

            foreach (var tab in fieldControl.Tabs)
            {
                if (tab.Fields == null)
                {
                    continue;
                }

                foreach (var configField in tab.Fields)
                {
                    var field = configField.Field;
                    var infoAreaIdWithLink = field.InfoAreaIdWithLink;
                    var metaInfo = (UPContainerInfoAreaMetaInfo)dict.ValueOrDefault(infoAreaIdWithLink);
                    if (metaInfo == null)
                    {
                        continue;
                    }

                    var fieldMetaInfo = new UPContainerFieldMetaInfo(configField);
                    fieldMetaInfo.PositionInResult = metaInfo.AddFieldWithoutChildCheck(fieldMetaInfo);
                    fieldMetaInfo.PositionInInfoArea = fieldMetaInfo.PositionInResult;
                    fieldMetaInfo.InfoAreaPosition = metaInfo.Position;
                    this.OutputFields.Add(fieldMetaInfo);
                }
            }
        }
    }
}
