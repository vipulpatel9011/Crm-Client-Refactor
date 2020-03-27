// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryTreeItem.cs" company="Aurea Software Gmbh">
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
//   Defines the query tree item
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;
    using System.Text;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Defines the query tree item
    /// </summary>
    public class QueryTreeItem
    {
        private const int GenericLinkId = 126;
        private const string Comma = ",";
        private const string NullString = "null";

        /// <summary>
        /// The record template.
        /// </summary>
        private readonly RecordTemplate recordTemplate;

        /// <summary>
        /// The has output columns.
        /// </summary>
        private bool hasOutputColumns;

        /// <summary>
        /// The has virtual link condition.
        /// </summary>
        private bool hasVirtualLinkCondition;

        /// <summary>
        /// The record template owner.
        /// </summary>
        private bool recordTemplateOwner;

        /// <summary>
        /// The sub nodes.
        /// </summary>
        private List<QueryTreeItem> subNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTreeItem"/> class.
        /// </summary>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public QueryTreeItem(CRMDatabase crmDatabase, string infoAreaId)
        {
            this.recordTemplate = new RecordTemplate(crmDatabase, infoAreaId);
            this.recordTemplateOwner = true;
            this.LinkId = -1;
            this.Relation = null;
            this.Alias = null;
            this.subNodes = null;
            this.OutputColumnStartIndex = 0;
            this.Condition = null;
            this.CrmDatabase = crmDatabase;
            this.UseVirtualLinks = true;
            this.SubQuery = null;
            this.RecordIdIndex = -1;
            this.SetAlias(infoAreaId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTreeItem"/> class.
        /// </summary>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public QueryTreeItem(CRMDatabase crmDatabase, string infoAreaId, int linkId)
        {
            this.recordTemplate = new RecordTemplate(crmDatabase, infoAreaId);
            this.recordTemplateOwner = true;
            this.LinkId = linkId;
            this.CrmDatabase = crmDatabase;
            this.UseVirtualLinks = true;
            this.RecordIdIndex = -1;
            this.SetAlias(infoAreaId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTreeItem"/> class.
        /// </summary>
        /// <param name="recordTemplate">
        /// The record template.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public QueryTreeItem(RecordTemplate recordTemplate, int linkId = -1)
        {
            this.recordTemplate = recordTemplate;
            this.LinkId = linkId;
            this.UseVirtualLinks = true;
            this.CrmDatabase = recordTemplate.Database;
            this.RecordIdIndex = -1;
            this.SetAlias(recordTemplate.InfoAreaId);
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        public TreeItemCondition Condition { get; private set; }

        /// <summary>
        /// Gets the CRM database.
        /// </summary>
        /// <value>
        /// The CRM database.
        /// </value>
        public CRMDatabase CrmDatabase { get; }

        /// <summary>
        /// Gets a value indicating whether [ignore lookup rows].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore lookup rows]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreLookupRows { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.recordTemplate?.InfoAreaId;

        /// <summary>
        /// Gets the index of the information area identifier.
        /// </summary>
        /// <value>
        /// The index of the information area identifier.
        /// </value>
        public int InfoAreaIdIndex { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; }

        /// <summary>
        /// Gets the start index of the output column.
        /// </summary>
        /// <value>
        /// The start index of the output column.
        /// </value>
        public int OutputColumnStartIndex { get; private set; }

        /// <summary>
        /// Gets the index of the record identifier.
        /// </summary>
        /// <value>
        /// The index of the record identifier.
        /// </value>
        public int RecordIdIndex { get; private set; }

        /// <summary>
        /// Gets the relation.
        /// </summary>
        /// <value>
        /// The relation.
        /// </value>
        public string Relation { get; private set; }

        /// <summary>
        /// Gets the sub node count.
        /// </summary>
        /// <value>
        /// The sub node count.
        /// </value>
        public int SubNodeCount => this.subNodes?.Count ?? 0;

        /// <summary>
        /// Gets the sub query.
        /// </summary>
        /// <value>
        /// The sub query.
        /// </value>
        public SubQuery SubQuery { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [use virtual links].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use virtual links]; otherwise, <c>false</c>.
        /// </value>
        public bool UseVirtualLinks { get; private set; }

        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void AddCondition(TreeItemCondition condition)
        {
            this.Condition = this.Condition == null
                                 ? condition
                                 : new TreeItemConditionRelation("AND", this.Condition, condition);
        }

        /// <summary>
        /// Adds the condition to statement.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void AddConditionToStatement(StringBuilder sb, StatementCreationContext context)
        {
            context.RecordTemplate = this.recordTemplate;

            var conditionParts = 0;
            var subNodesHaveConditions = this.SubNodesHaveExistConditions();
            var firstPart = true;

            if (this.Condition != null)
            {
                conditionParts++;
            }

            if (this.IgnoreLookupRows)
            {
                conditionParts++;
            }

            if (subNodesHaveConditions)
            {
                conditionParts++;
            }

            if (conditionParts > 1)
            {
                sb.Append("(");
            }

            if (this.Condition != null)
            {
                this.Condition.AddConditionToStatement(sb, context, this.Alias);
                firstPart = false;
            }

            if (this.IgnoreLookupRows)
            {
                if (!firstPart)
                {
                    sb.Append(") AND (");
                }
                else
                {
                    firstPart = false;
                }

                this.AddIgnoreLookupCondition(sb, context);
            }

            if (subNodesHaveConditions)
            {
                if (!firstPart)
                {
                    sb.Append(") AND (");
                }

                this.AddSubNodeConditionsToStatement(sb, context);
            }

            if (conditionParts > 1)
            {
                sb.Append(")");
            }
        }

        /// <summary>
        /// Adds the field condition.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        public void AddFieldCondition(FieldIdType fieldIndex, string fieldValue)
        {
            var newCondition = new TreeItemConditionFieldValue(fieldIndex, fieldValue) as TreeItemCondition;
            this.Condition = this.Condition == null
                                 ? newCondition
                                 : new TreeItemConditionRelation("AND", this.Condition, newCondition);
        }

        /// <summary>
        /// Adds from part.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public void AddFromPart(StringBuilder sb, StatementCreationContext context, QueryTreeItem parent)
        {
            if (parent != null && this.IsExistsCondition())
            {
                return;
            }

            var tableinfo = this.CrmDatabase.GetTableInfoByInfoArea(this.recordTemplate.InfoAreaId);
            if (tableinfo == null)
            {
                return;
            }

            LinkInfo linkInfo = null;
            var linkFieldName = string.Empty;
            context.RecordTemplate = this.recordTemplate;

            if (parent != null)
            {
                linkFieldName = string.Format("LINK_{0}_{1}", this.recordTemplate.GetRootInfoAreaId(), this.LinkId < 0 ? 0 : this.LinkId);
                linkInfo = parent.GetTableInfo().GetLink(linkFieldName);

                if (linkInfo == null && this.LinkId <= 0)
                {
                    linkInfo = parent.GetTableInfo().GetDefaultLink(this.recordTemplate.GetRootInfoAreaId());
                }

                if (linkInfo == null)
                {
                    context.ErrorText = $"link {linkFieldName} does not exist";
                    return;
                }

                sb.Append(this.Relation == "PLUS" ? " LEFT JOIN " : " JOIN ");
            }

            sb.Append(tableinfo.DatabaseTableName);
            if (!string.IsNullOrWhiteSpace(this.Alias))
            {
                sb.Append(" AS ");
                sb.Append(this.Alias);
            }

            if (parent != null)
            {
                sb.Append(" ON ");

                if (linkInfo.UseLinkFields)
                {
                    string sourceValue = null;
                    string destValue = null;

                    var linkFieldCount = linkInfo.LinkFieldCount;
                    for (var i = 0; i < linkFieldCount; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(" AND ");
                        }

                        var sourceFieldId = linkInfo.GetSourceFieldIdWithIndex(i);
                        var destFieldId = linkInfo.GetDestinationFieldIdWithIndex(i);

                        bool isKPCP;
                        bool isDestValue;
                        bool isSourceValue;
                        if (sourceFieldId < 0)
                        {
                            isSourceValue = true;
                            isDestValue = false;
                            sourceValue = linkInfo.GetSourceValueWithIndex(i);
                            isKPCP = !string.IsNullOrEmpty(sourceValue) && (sourceValue == "KP" || sourceValue == "CP");
                        }
                        else if (destFieldId < 0)
                        {
                            isSourceValue = false;
                            isDestValue = true;
                            destValue = linkInfo.GetDestinationValueWithIndex(i);
                            isKPCP = !string.IsNullOrEmpty(destValue) && (destValue == "KP" || destValue == "CP");
                        }
                        else
                        {
                            isSourceValue = false;
                            isDestValue = false;
                            isKPCP = false;
                        }

                        if (isKPCP)
                        {
                            sb.Append("(");
                        }

                        if (sourceFieldId >= 0)
                        {
                            sb.Append(parent.GetFieldName(sourceFieldId));
                        }

                        if (destFieldId >= 0)
                        {
                            if (!isSourceValue)
                            {
                                sb.Append(" = ");
                            }

                            sb.Append(this.GetFieldName(destFieldId));
                        }
                        else if (isDestValue)
                        {
                            if (destValue == null)
                            {
                                sb.Append(" IS NULL");
                            }
                            else if (isKPCP)
                            {
                                sb.Append(" = 'KP' OR ");
                                sb.Append(parent.GetFieldName(sourceFieldId));
                                sb.Append(" = 'CP')");
                            }
                            else
                            {
                                sb.Append(" = '");
                                sb.Append(destValue);
                                sb.Append("'");
                            }
                        }
                        else
                        {
                            context.ErrorText = $"cannot use link {linkFieldName} - no field with index {i}";
                        }

                        if (isSourceValue)
                        {
                            if (string.IsNullOrEmpty(sourceValue))
                            {
                                sb.Append(" IS NULL");
                            }
                            else if (isKPCP)
                            {
                                sb.Append(" = 'KP' OR ");
                                sb.Append(this.GetFieldName(destFieldId));
                                sb.Append(" = 'CP')");
                            }
                            else
                            {
                                sb.Append(" = '");
                                sb.Append(sourceValue);
                                sb.Append("'");
                            }
                        }
                    }
                }
                else if (linkInfo.IsFieldLink)
                {
                    var fieldInfo =
                        $"{parent.GetAliasOrTableName()}.{parent.GetTableInfo().GetFieldName(linkInfo.SourceFieldId)} = {this.GetAliasOrTableName()}.{tableinfo.GetFieldName(linkInfo.DestFieldId)}";
                    sb.Append(fieldInfo);
                }
                else if (linkInfo.IsGeneric)
                {
                    if (linkInfo.HasColumn)
                    {
                        var physicalInfoAreaId = tableinfo.RootPhysicalInfoAreaId;
                        string buf;
                        if (physicalInfoAreaId == "CP" || physicalInfoAreaId == "KP")
                        {
                            buf = $"{parent.GetAliasOrTableName()}.{linkInfo.GetPhysicalColumnName()} ="
                                  + $"{this.GetAliasOrTableName()}.{tableinfo.RecordIdFieldName} AND"
                                  + $" ({parent.GetAliasOrTableName()}.{linkInfo.InfoAreaColumnName} = 'CP' OR {parent.GetAliasOrTableName()}.{linkInfo.InfoAreaColumnName} = 'KP')";
                        }
                        else
                        {
                            buf = $"{parent.GetAliasOrTableName()}.{linkInfo.GetPhysicalColumnName()} "
                                  + $"= {this.GetAliasOrTableName()}.{tableinfo.RecordIdFieldName} AND"
                                  + $" {parent.GetAliasOrTableName()}.{linkInfo.InfoAreaColumnName} = '{physicalInfoAreaId}'";
                        }

                        sb.Append(buf);
                    }
                    else
                    {
                        var parentTableInfo = parent.GetTableInfo();
                        var physicalInfoAreaId = parentTableInfo != null
                                                     ? parentTableInfo.RootPhysicalInfoAreaId
                                                     : string.Empty;
                        string buf;

                        if (physicalInfoAreaId == "KP" || physicalInfoAreaId == "CP")
                        {
                            buf =
                                $"{this.GetAliasOrTableName()}.{linkInfo.GetPhysicalColumnName()} = {parent.GetAliasOrTableName()}"
                                + $".{parent.GetTableInfo().RecordIdFieldName} AND ({this.GetAliasOrTableName()}.{linkInfo.InfoAreaColumnName}"
                                + $"= 'CP' OR {this.GetAliasOrTableName()}.{linkInfo.InfoAreaColumnName} = 'KP')";
                        }
                        else
                        {
                            buf =
                                $"{this.GetAliasOrTableName()}.{linkInfo.GetPhysicalColumnName()} = {parent.GetAliasOrTableName()}"
                                + $".{parent.GetTableInfo().RecordIdFieldName} AND {this.GetAliasOrTableName()}"
                                + $".{linkInfo.InfoAreaColumnName} = '{physicalInfoAreaId}'";
                        }

                        sb.Append(buf);
                    }
                }
                else if (linkInfo.HasColumn || linkInfo.IsIdentLink)
                {
                    var buf =
                        $"{parent.GetAliasOrTableName()}.{linkInfo.GetPhysicalColumnName()} = {this.GetAliasOrTableName()}.{tableinfo.RecordIdFieldName}";
                    sb.Append(buf);
                }
                else
                {
                    linkFieldName = $"LINK_{parent.InfoAreaId}_{linkInfo.ReverseLinkId}";
                    var buf =
                        $"{this.GetAliasOrTableName()}.{linkFieldName} = {parent.GetAliasOrTableName()}.{parent.GetTableInfo().RecordIdFieldName}";
                    sb.Append(buf);
                }

                if (this.Condition != null || this.HasExistsConditions())
                {
                    sb.Append(" AND (");
                    this.Condition.AddConditionToStatement(sb, context, this.Alias);
                    sb.Append(")");
                }
            }

            if (this.subNodes != null)
            {
                for (var i = 0; i < this.SubNodeCount && string.IsNullOrEmpty(context.ErrorText); i++)
                {
                    if (this.subNodes[i].SubQuery != null)
                    {
                        continue;
                    }

                    this.subNodes[i].AddFromPart(sb, context, this);
                }
            }
        }

        /// <summary>
        /// Adds the ignore lookup condition.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void AddIgnoreLookupCondition(StringBuilder sb, StatementCreationContext context)
        {
            sb.Append("(");

            if (this.Alias != null)
            {
                sb.Append(this.Alias);
                sb.Append(".");
            }

            sb.Append(this.recordTemplate.GetFieldName(FieldIdType.LOOKUP));
            sb.Append(" = 0 OR ");
            if (this.Alias != null)
            {
                sb.Append(this.Alias);
                sb.Append(".");
            }

            sb.Append(this.recordTemplate.GetFieldName(FieldIdType.LOOKUP));
            sb.Append(" IS NULL)");
        }

        /// <summary>
        /// Adds the output columns.
        /// </summary>
        /// <param name="stringBuilder">
        /// The sb.
        /// </param>
        /// <param name="startIndex">
        /// The start index reference.
        /// </param>
        /// <param name="isRootItem">
        /// if set to <c>true</c> [is root item].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddOutputColumns(StringBuilder stringBuilder, ref int startIndex, bool isRootItem)
        {
            this.hasOutputColumns = false;
            this.RecordIdIndex = -1;
            this.InfoAreaIdIndex = -1;

            if (!isRootItem && this.IsExistsCondition())
            {
                return false;
            }

            var currentAlias = this.GetAliasOrTableName();
            var fieldPrefix = !string.IsNullOrWhiteSpace(currentAlias) ? $"{currentAlias}." : null;

            var fieldCount = this.recordTemplate?.FieldIdCount ?? 0;

            this.OutputColumnStartIndex = startIndex;
            if (fieldCount > 0)
            {
                this.AddFieldOutputColumn(stringBuilder, fieldCount, startIndex, fieldPrefix);

                startIndex += fieldCount;
            }

            var linkCount = this.recordTemplate?.LinkFieldNameCount ?? 0;
            var nextFieldCount = this.AddLinkOutputColumn(stringBuilder, linkCount, fieldPrefix, fieldCount, ref startIndex);
            nextFieldCount = this.AddRecordTemplateOutputColumn(stringBuilder, fieldCount, fieldPrefix, nextFieldCount, ref startIndex);

            if (this.SubNodeCount > 0 && this.subNodes != null)
            {
                for (var subNodeIndex = 0; subNodeIndex < this.SubNodeCount; subNodeIndex++)
                {
                    if (this.subNodes[subNodeIndex].SubQuery != null)
                    {
                        this.subNodes[subNodeIndex].SubQuery.SetSubQueryInformation(this.RecordIdIndex, nextFieldCount);
                        continue;
                    }

                    this.subNodes[subNodeIndex].AddOutputColumns(stringBuilder, ref startIndex, false);
                }
            }

            return true;
        }

        /// <summary>
        /// Adds the record identifier condition.
        /// </summary>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        public void AddRecordIdCondition(string recordId)
        {
            this.AddFieldCondition(FieldIdType.RECORDID, recordId);
        }

        /// <summary>
        /// Adds the sub node.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        /// <param name="subNode">
        /// The sub node.
        /// </param>
        public void AddSubNode(string relation, QueryTreeItem subNode)
        {
            if (this.subNodes == null)
            {
                this.subNodes = new List<QueryTreeItem>();
            }

            subNode.SetRelation(relation);
            subNode.SetUseVirtualLinks(this.UseVirtualLinks);
            subNode.SetParentAlias(this.Alias ?? string.Empty, this.SubNodeCount, true);

            this.subNodes.Add(subNode);
        }

        /// <summary>
        /// Adds the sub node conditions to statement.
        /// </summary>
        /// <param name="stringBuilder">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void AddSubNodeConditionsToStatement(StringBuilder stringBuilder, StatementCreationContext context)
        {
            var first = true;

            QueryTreeItem treeItem;
            var nodeGroups = new QueryTreeItem[2][]; // AND + OR
            nodeGroups[0] = nodeGroups[1] = null;

            var nodeGroupCount = new[] { 0, 0 };

            for (var subNodeIndex = 0; subNodeIndex < this.SubNodeCount; subNodeIndex++)
            {
                treeItem = this.subNodes[subNodeIndex];

                if (!treeItem.IsExistsCondition())
                {
                    continue;
                }

                this.InitSubNode(nodeGroups, nodeGroupCount, treeItem);
            }

            for (var nodeGroupIndex = 0; nodeGroupIndex < 2; nodeGroupIndex++)
            {
                if (nodeGroupCount[nodeGroupIndex] == 0)
                {
                    continue;
                }

                if (nodeGroupIndex == 1 && !first)
                {
                    stringBuilder.Append(" AND (");
                }

                first = true;

                for (var subNodeIndex = 0; subNodeIndex < nodeGroupCount[nodeGroupIndex]; subNodeIndex++)
                {
                    treeItem = nodeGroups[nodeGroupIndex][subNodeIndex];
                    if (first)
                    {
                        first = false;
                    }
                    else if (nodeGroupIndex == 0)
                    {
                        stringBuilder.Append(" AND");
                    }
                    else
                    {
                        stringBuilder.Append(" OR");
                    }

                    this.AddSubNodeConditionsToStatement(stringBuilder, context, treeItem);
                }
            }

            if (nodeGroups[0] != null && nodeGroups[1] != null)
            {
                stringBuilder.Append(")");
            }
        }

        /// <summary>
        /// Checks the sub queries.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckSubQueries()
        {
            if (this.subNodes == null)
            {
                return false;
            }

            var hasSubQuery = false;
            var hasChildOutput = false;
            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (hasChildOutput)
                {
                    var isChildOutput = this.subNodes[i].IsChildOutputTreeItem(this);
                    if (isChildOutput)
                    {
                        hasSubQuery = true;
                        this.subNodes[i].CreateSubQueryWithParent(this);
                        continue;
                    }
                }
                else
                {
                    hasChildOutput = this.subNodes[i].IsChildOutputTreeItem(this);
                }

                hasSubQuery |= this.subNodes[i].CheckSubQueries();
            }

            return hasSubQuery;
        }

        /// <summary>
        /// Creates the sub query with parent.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public void CreateSubQueryWithParent(QueryTreeItem parent)
        {
            this.SubQuery = parent != null ? new SubQuery(this, parent) : null;
        }

        /// <summary>
        /// Executes the sub queries.
        /// </summary>
        /// <param name="baseRecordSet">
        /// The base record set.
        /// </param>
        /// <returns>
        /// The <see cref="GenericRecordSet"/>.
        /// </returns>
        public GenericRecordSet ExecuteSubQueries(GenericRecordSet baseRecordSet)
        {
            var combinedRecordSet = baseRecordSet;

            /* ignore subqueries until implemented
         int i;
        for (i = 0; i < _subNodeCount; i++) {
            if (_subNodes[i].GetSubQuery()) {
                combinedRecordSet = _subNodes[i].GetSubQuery().Execute(transaction, combinedRecordSet);
            }
        }

        for (i = 0; i < _subNodeCount; i++) {
            if (_subNodes[i].HasSubQueries()) {
                combinedRecordSet = _subNodes[i].ExecuteSubQueries(transaction, combinedRecordSet);
            }
        }
    */
            return combinedRecordSet;
        }

        /// <summary>
        /// Gets the name of the alias or table.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetAliasOrTableName()
        {
            if (!string.IsNullOrEmpty(this.Alias))
            {
                return this.Alias;
            }

            var tableInfo = this.GetTableInfo();
            return tableInfo?.DatabaseTableName;
        }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldInfo"/>.
        /// </returns>
        public FieldInfo GetFieldInfo(int fieldId)
        {
            var tableInfo = this.GetTableInfo();
            return tableInfo?.GetFieldInfo(fieldId);
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetFieldName(int fieldId)
        {
            if (string.IsNullOrEmpty(this.Alias))
            {
                return this.recordTemplate.GetFieldName((FieldIdType)fieldId);
            }

            var fieldPart = this.recordTemplate.GetFieldName((FieldIdType)fieldId);

            return $"{this.Alias}.{fieldPart}";
        }

        /// <summary>
        /// Gets the root information area identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetRootInfoAreaId()
        {
            var tableInfo = this.GetTableInfo();
            return tableInfo != null ? tableInfo.RootInfoAreaId : this.InfoAreaId;
        }

        /// <summary>
        /// Gets the root physical information area identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetRootPhysicalInfoAreaId()
        {
            var tableInfo = this.GetTableInfo();
            return tableInfo != null ? tableInfo.RootPhysicalInfoAreaId : this.InfoAreaId;
        }

        /// <summary>
        /// Gets the sub node.
        /// </summary>
        /// <param name="i">
        /// The element index.
        /// </param>
        /// <returns>
        /// The <see cref="QueryTreeItem"/>.
        /// </returns>
        public QueryTreeItem GetSubNode(int i)
        {
            return i < this.SubNodeCount ? this.subNodes[i] : null;
        }

        /// <summary>
        /// Gets the sub node.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="QueryTreeItem"/>.
        /// </returns>
        public QueryTreeItem GetSubNode(string infoAreaId, int linkId)
        {
            if (this.recordTemplate == null)
            {
                return null;
            }

            if (linkId < 0 && this.recordTemplate.InfoAreaId == infoAreaId)
            {
                return this;
            }

            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (linkId == this.subNodes[i].LinkId && this.subNodes[i].InfoAreaId == infoAreaId)
                {
                    return this.subNodes[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the table information.
        /// </summary>
        /// <returns>
        /// The <see cref="TableInfo"/>.
        /// </returns>
        public TableInfo GetTableInfo() => this.CrmDatabase?.GetTableInfoByInfoArea(this.InfoAreaId);

        /// <summary>
        /// Determines whether this instance has conditions.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasConditions() => this.Condition != null || this.SubNodesHaveConditions();

        /// <summary>
        /// Determines whether [has exists conditions].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasExistsConditions()
        {
            if (this.subNodes == null)
            {
                return false;
            }

            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (this.subNodes[i].IsExistsCondition())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether [has sub queries].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasSubQueries()
        {
            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (this.subNodes[i].SubQuery != null || this.subNodes[i].HasSubQueries())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is child output tree item] [the specified parent].
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsChildOutputTreeItem(QueryTreeItem parent)
        {
            if (parent == null)
            {
                return false;
            }

            if (this.IsExistsCondition())
            {
                return false;
            }

            var linkFieldName = string.Format("LINK_{0}_{1}", this.recordTemplate.GetRootInfoAreaId(), this.LinkId < 0 ? 0 : this.LinkId);
            var linkInfo = parent.GetTableInfo()?.GetLink(linkFieldName);

            if (linkInfo == null && this.LinkId <= 0)
            {
                linkInfo = parent.GetTableInfo()?.GetDefaultLink(this.recordTemplate.GetRootInfoAreaId());
            }

            return linkInfo != null && linkInfo.IsChildLink;
        }

        /// <summary>
        /// Determines whether [is exists condition].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsExistsCondition()
        {
            return !string.IsNullOrEmpty(this.Relation)
                   && (this.Relation.StartsWith("WITHOUT")
                   || this.Relation.StartsWith("HAVING")
                   || (this.SubQuery != null && this.Relation.StartsWith("WITH")));
        }

        /// <summary>
        /// Needses the virtual links.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool NeedsVirtualLinks(QueryTreeItem parent)
        {
            int i;
            if (this.hasVirtualLinkCondition)
            {
                return true;
            }

            for (i = 0; i < this.SubNodeCount; i++)
            {
                if (this.subNodes[i].NeedsVirtualLinks(this))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Needses the virtual links.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool NeedsVirtualLinks() => this.NeedsVirtualLinks(null);

        /// <summary>
        /// Sets the alias.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        public void SetAlias(string alias)
        {
            this.Alias = $"_{alias}";
        }

        /// <summary>
        /// Sets the ignore lookup rows.
        /// </summary>
        /// <param name="ignoreLookupRows">
        /// if set to <c>true</c> [ignore lookup rows].
        /// </param>
        public void SetIgnoreLookupRows(bool ignoreLookupRows)
        {
            this.IgnoreLookupRows = ignoreLookupRows && this.CrmDatabase != null && this.CrmDatabase.HasLookup
                                    && this.CrmDatabase.HasLookupRecords(this.recordTemplate.InfoAreaId);
        }

        /// <summary>
        /// Sets the link record.
        /// </summary>
        /// <param name="linkInfoAreaId">
        /// The link information area identifier.
        /// </param>
        /// <param name="linkRecordId">
        /// The link record identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetLinkRecord(string linkInfoAreaId, string linkRecordId, int linkId)
        {
            var tableInfo = this.GetTableInfo();
            var foreigntableinfo = this.CrmDatabase.GetTableInfoByInfoArea(linkInfoAreaId);
            if (tableInfo == null || foreigntableinfo == null)
            {
                return false;
            }

            linkInfoAreaId = foreigntableinfo.RootPhysicalInfoAreaId;

            if (this.UseVirtualLinks)
            {
                var virtualLinkSet = this.SetVirtualLink(tableInfo, linkInfoAreaId, linkRecordId, linkId);
                if (virtualLinkSet)
                {
                    return true;
                }
            }

            var linkInfo = tableInfo.GetLink(linkInfoAreaId, linkId);
            var infoAreaId = tableInfo.InfoAreaId;
            var rootPhysicalInfoAreaId = tableInfo.RootPhysicalInfoAreaId;

            if (linkId <= 0 && infoAreaId == linkInfoAreaId)
            {
                // handle ident link
                this.AddCondition(new TreeItemConditionFieldValue(FieldIdType.RECORDID, linkRecordId));
                return true;
            }

            var reverseLink = foreigntableinfo.GetLink(infoAreaId, linkInfo?.ReverseLinkId ?? linkId);

            if (linkInfo == null && reverseLink == null)
            {
                return false;
            }

            return this.SetForeignKeyLink(foreigntableinfo, linkInfo, rootPhysicalInfoAreaId, reverseLink, linkInfoAreaId, linkRecordId, linkId);
        }

        /// <summary>
        /// Sets the parent alias.
        /// </summary>
        /// <param name="parentAlias">
        /// The parent alias.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="recursive">
        /// if set to <c>true</c> [recursive].
        /// </param>
        public void SetParentAlias(string parentAlias, int index, bool recursive)
        {
            this.Alias = $"{parentAlias}{this.recordTemplate.InfoAreaId}{index}";

            if (!recursive || this.subNodes == null)
            {
                return;
            }

            for (var i = 0; i < this.SubNodeCount; i++)
            {
                this.subNodes[i]?.SetParentAlias(this.Alias, i, true);
            }
        }

        /// <summary>
        /// Sets the relation.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        public void SetRelation(string relation)
        {
            this.Relation = relation;
        }

        /// <summary>
        /// Sets the use virtual links.
        /// </summary>
        /// <param name="useVirtualLinks">
        /// if set to <c>true</c> [use virtual links].
        /// </param>
        public void SetUseVirtualLinks(bool useVirtualLinks)
        {
            this.UseVirtualLinks = useVirtualLinks;
            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (this.subNodes[i] != null)
                {
                    this.subNodes[i].SetUseVirtualLinks(this.UseVirtualLinks);
                }
            }
        }

        /// <summary>
        /// Subs the nodes have conditions.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SubNodesHaveConditions()
        {
            if (this.subNodes == null)
            {
                return false;
            }

            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (this.subNodes[i].HasConditions() || this.subNodes[i].IsExistsCondition())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Subs the nodes have exist conditions.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SubNodesHaveExistConditions()
        {
            if (this.subNodes == null)
            {
                return false;
            }

            for (var i = 0; i < this.SubNodeCount; i++)
            {
                if (this.subNodes[i].IsExistsCondition())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Trees the item from record template.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <returns>
        /// The <see cref="QueryTreeItem"/>.
        /// </returns>
        public QueryTreeItem TreeItemFromRecordTemplate(RecordTemplate template)
        {
            if (template == this.recordTemplate)
            {
                return this;
            }

            if (this.subNodes == null)
            {
                return null;
            }

            for (var i = 0; i < this.SubNodeCount; i++)
            {
                var subNode = this.subNodes[i].TreeItemFromRecordTemplate(template);
                if (subNode != null)
                {
                    return subNode;
                }
            }

            return null;
        }

        private static void AddSourceValue(QueryTreeItem treeItem, StringBuilder stringBuilder, int destFieldId, string sourceValue, bool isKPCP)
        {
            if (string.IsNullOrWhiteSpace(sourceValue))
            {
                stringBuilder.Append(" IS NULL");
            }
            else if (isKPCP)
            {
                stringBuilder.Append(" = 'KP' OR ");
                stringBuilder.Append(treeItem.GetFieldName(destFieldId));
                stringBuilder.Append(" = 'CP')");
            }
            else
            {
                stringBuilder.Append(" = '");
                stringBuilder.Append(sourceValue);
                stringBuilder.Append("'");
            }
        }

        private void AddSubNodeConditionsToStatement(StringBuilder stringBuilder, StatementCreationContext context, QueryTreeItem treeItem)
        {
            stringBuilder.Append(treeItem.Relation.StartsWith("WITHOUT") ? " NOT EXISTS" : " EXISTS");

            stringBuilder.Append(" (SELECT * FROM ");
            stringBuilder.Append(treeItem.GetTableInfo().DatabaseTableName);
            if (!string.IsNullOrWhiteSpace(treeItem.Alias))
            {
                stringBuilder.Append(" AS ");
                stringBuilder.Append(treeItem.Alias);
            }

            if (treeItem.SubNodeCount > 0)
            {
                QueryTreeItem subNode;
                for (var subNodeIndex = 0; subNodeIndex < treeItem.SubNodeCount; subNodeIndex++)
                {
                    subNode = treeItem.GetSubNode(subNodeIndex);
                    subNode.AddFromPart(stringBuilder, context, treeItem);
                }
            }

            var linkFieldName = $"LINK_{treeItem.GetRootInfoAreaId()}_{(treeItem.LinkId < 0 ? 0 : treeItem.LinkId)}";
            var linkInfo = this.GetTableInfo().GetLink(linkFieldName);
            LinkInfo virtualReverseLink = null;
            LinkInfo reverseLinkInfo = null;
            if (linkInfo == null && this.LinkId <= 0)
            {
                linkInfo = this.GetTableInfo().GetDefaultLink(treeItem.InfoAreaId);
                if (linkInfo == null)
                {
                    linkFieldName = $"LINK_{this.GetRootInfoAreaId()}_0";
                    reverseLinkInfo = treeItem.GetTableInfo().GetLink(linkFieldName);
                    if (reverseLinkInfo != null)
                    {
                        virtualReverseLink = reverseLinkInfo.CreateVirtualReverseLink();
                        linkInfo = virtualReverseLink;
                    }
                }
            }

            if (linkInfo != null)
            {
                this.AddSubNodeConditionsToStatementFromLinkInfo(treeItem, linkInfo, context, stringBuilder, linkFieldName);
            }
            else
            {
                if (treeItem.HasConditions() || treeItem.HasExistsConditions())
                {
                    stringBuilder.Append(" WHERE (");
                }
            }

            if (virtualReverseLink != null)
            {
                reverseLinkInfo.DropVirtualReverseLink(virtualReverseLink);
            }

            if (treeItem.HasConditions() || treeItem.HasExistsConditions())
            {
                treeItem.AddConditionToStatement(stringBuilder, context);
                stringBuilder.Append(")");
            }

            stringBuilder.Append(")");
        }

        private void AddSubNodeConditionsToStatementFromLinkInfo(QueryTreeItem treeItem, LinkInfo linkInfo, StatementCreationContext context, StringBuilder stringBuilder, string linkFieldName)
        {
            stringBuilder.Append(" WHERE ");
            if (linkInfo.UseLinkFields)
            {
                this.UseLinkFields(treeItem, linkInfo, context, stringBuilder, linkFieldName);
            }
            else if (linkInfo.IsFieldLink)
            {
                stringBuilder.Append($"{this.GetAliasOrTableName()}.{this.GetTableInfo().GetFieldName(linkInfo.SourceFieldId)}");
                stringBuilder.Append($" = {treeItem.GetAliasOrTableName()}.{treeItem.GetTableInfo().GetFieldName(linkInfo.DestFieldId)}");
            }
            else if (linkInfo.IsGeneric)
            {
                this.LinkGeneric(treeItem, linkInfo, stringBuilder);
            }
            else if (linkInfo.HasColumn)
            {
                var hasColumnequalsStatement =
                    $"{this.GetAliasOrTableName()}.{linkFieldName} = {treeItem.GetAliasOrTableName()}.{treeItem.GetTableInfo().RecordIdFieldName}";
                stringBuilder.Append(hasColumnequalsStatement);
            }
            else
            {
                linkFieldName = linkInfo.RelationType == LinkType.IDENT
                                    ? this.recordTemplate.GetFieldName(FieldIdType.RECORDID)
                                    : $"LINK_{this.InfoAreaId}_{linkInfo.ReverseLinkId}";

                var equalsStatemnet =
                    $"{treeItem.GetAliasOrTableName()}.{linkFieldName} = {this.GetAliasOrTableName()}.{this.GetTableInfo().RecordIdFieldName}";
                stringBuilder.Append(equalsStatemnet);
            }

            if (treeItem.HasConditions() || treeItem.HasExistsConditions())
            {
                stringBuilder.Append(" AND (");
            }
        }

        private void UseLinkFields(QueryTreeItem treeItem, LinkInfo linkInfo, StatementCreationContext context, StringBuilder stringBuilder, string linkFieldName)
        {
            string sourceValue = null;
            string destValue = null;

            var linkFieldCount = linkInfo.LinkFieldCount;
            for (var linkFieldIndex = 0; linkFieldIndex < linkFieldCount; linkFieldIndex++)
            {
                bool isSourceValue;
                bool isKPCP;

                int sourceFieldId = linkInfo.GetSourceFieldIdWithIndex(linkFieldIndex);
                int destFieldId = linkInfo.GetDestinationFieldIdWithIndex(linkFieldIndex);

                if (sourceFieldId < 0)
                {
                    isSourceValue = true;
                    sourceValue = linkInfo.GetSourceValueWithIndex(linkFieldIndex);
                    isKPCP = !string.IsNullOrWhiteSpace(sourceValue) && (sourceValue == "KP" || sourceValue == "CP");
                }
                else if (destFieldId < 0)
                {
                    isSourceValue = false;
                    destValue = linkInfo.GetDestinationValueWithIndex(linkFieldIndex);
                    isKPCP = !string.IsNullOrWhiteSpace(destValue) && (destValue == "KP" || destValue == "CP");
                }
                else
                {
                    isSourceValue = false;
                    isKPCP = false;
                }

                if (linkFieldIndex > 0)
                {
                    stringBuilder.Append(" AND ");
                }

                if (isKPCP)
                {
                    stringBuilder.Append("(");
                }

                if (sourceFieldId >= 0)
                {
                    stringBuilder.Append(this.GetFieldName(sourceFieldId));
                }

                this.AddNonSourceValue(treeItem, context, linkFieldName, linkFieldIndex, stringBuilder, destFieldId, sourceFieldId, destValue, isSourceValue, isKPCP);

                if (isSourceValue)
                {
                    AddSourceValue(treeItem, stringBuilder, destFieldId, sourceValue, isKPCP);
                }
            }
        }

        private void AddNonSourceValue(QueryTreeItem treeItem, StatementCreationContext context, string linkFieldName, int linkFieldIndex, StringBuilder stringBuilder, int destFieldId, int sourceFieldId, string destValue, bool isSourceValue, bool isKPCP)
        {
            if (destFieldId >= 0)
            {
                if (!isSourceValue)
                {
                    stringBuilder.Append(" = ");
                }

                stringBuilder.Append(treeItem.GetFieldName(destFieldId));
            }
            else if (!isSourceValue)
            {
                if (string.IsNullOrWhiteSpace(destValue))
                {
                    stringBuilder.Append(" IS NULL");
                }
                else if (isKPCP)
                {
                    stringBuilder.Append(" = 'KP' OR ");
                    stringBuilder.Append(this.GetFieldName(sourceFieldId));
                    stringBuilder.Append(" = 'CP')");
                }
                else
                {
                    stringBuilder.Append(" = '");
                    stringBuilder.Append(destValue);
                    stringBuilder.Append("'");
                }
            }
            else
            {
                context.ErrorText = $"cannot use link {linkFieldName} - no field with index {linkFieldIndex}";
            }
        }

        private void LinkGeneric(QueryTreeItem treeItem, LinkInfo linkInfo, StringBuilder stringBuilder)
        {
            if (linkInfo.HasColumn)
            {
                var physicalInfoAreaId = treeItem.GetRootPhysicalInfoAreaId();

                var buf = physicalInfoAreaId == "KP" || physicalInfoAreaId == "CP"
                          ? string.Format(
                              "{0}.{1} = {2}.{3} AND ({4}.{5} = 'CP' OR {6}.{7} = 'KP')",
                              this.GetAliasOrTableName(),
                              linkInfo.GetPhysicalColumnName(),
                              treeItem.GetAliasOrTableName(),
                              treeItem.GetTableInfo().RecordIdFieldName,
                              this.GetAliasOrTableName(),
                              linkInfo.InfoAreaColumnName,
                              this.GetAliasOrTableName(),
                              linkInfo.InfoAreaColumnName)
                          : string.Format(
                              "{0}.{1} = {2}.{3} AND {4}.{5} = '{6}'",
                              this.GetAliasOrTableName(),
                              linkInfo.GetPhysicalColumnName(),
                              treeItem.GetAliasOrTableName(),
                              treeItem.GetTableInfo().RecordIdFieldName,
                              this.GetAliasOrTableName(),
                              linkInfo.InfoAreaColumnName,
                              physicalInfoAreaId);

                stringBuilder.Append(buf);
            }
            else
            {
                string linkGenericStatement;

                var physicalInfoAreaId = this.GetRootPhysicalInfoAreaId();
                if (physicalInfoAreaId == "KP" || physicalInfoAreaId == "CP")
                {
                    linkGenericStatement = string.Format(
                        "{0}.{1} = {2}.{3} AND ({4}.{5} = 'KP' OR {6}.{7} = 'CP')",
                        treeItem.GetAliasOrTableName(),
                        linkInfo.GetPhysicalColumnName(),
                        this.GetAliasOrTableName(),
                        this.GetTableInfo().RecordIdFieldName,
                        treeItem.GetAliasOrTableName(),
                        linkInfo.InfoAreaColumnName,
                        treeItem.GetAliasOrTableName(),
                        linkInfo.InfoAreaColumnName);
                }
                else
                {
                    linkGenericStatement = string.Format(
                        "{0}.{1} = {2}.{3} AND {4}.{5} = '{6}'",
                        treeItem.GetAliasOrTableName(),
                        linkInfo.GetPhysicalColumnName(),
                        this.GetAliasOrTableName(),
                        this.GetTableInfo().RecordIdFieldName,
                        treeItem.GetAliasOrTableName(),
                        linkInfo.InfoAreaColumnName,
                        physicalInfoAreaId);
                }

                stringBuilder.Append(linkGenericStatement);
            }
        }

        private void InitSubNode(QueryTreeItem[][] nodeGroups, int[] nodeGroupCount, QueryTreeItem treeItem)
        {
            if (treeItem.Relation == "WITHOPTIONAL" || treeItem.Relation == "HAVINGOPTIONAL" || treeItem.Relation == "WITHOUTOPTIONAL")
            {
                if (nodeGroups[1] == null)
                {
                    nodeGroups[1] = new QueryTreeItem[this.SubNodeCount];
                }

                nodeGroups[1][nodeGroupCount[1]++] = treeItem;
            }
            else
            {
                if (nodeGroups[0] == null)
                {
                    nodeGroups[0] = new QueryTreeItem[this.SubNodeCount];
                }

                nodeGroups[0][nodeGroupCount[0]++] = treeItem;
            }
        }

        private bool SetForeignKeyLink(TableInfo foreigntableinfo, LinkInfo linkInfo, string rootPhysicalInfoAreaId, LinkInfo reverseLink, string linkInfoAreaId, string linkRecordId, int linkId)
        {
            if (linkInfo == null)
            {
                this.SetForeignKeyLinkOnNoLinkInfo(foreigntableinfo, rootPhysicalInfoAreaId, reverseLink, linkInfoAreaId, linkRecordId, linkId);
            }
            else if (linkInfo.UseLinkFields)
            {
                var subNode = new QueryTreeItem(this.CrmDatabase, linkInfoAreaId, linkId);
                subNode.SetLinkRecord(linkInfoAreaId, linkRecordId, -1);
                this.AddSubNode("HAVING", subNode);
            }
            else if (linkInfo.HasColumn)
            {
                if (linkInfo.LinkId == GenericLinkId)
                {
                    this.AddCondition(
                        new TreeFieldNameCondition(linkInfo.InfoAreaColumnName, foreigntableinfo.RootPhysicalInfoAreaId));
                    this.AddCondition(new TreeFieldNameCondition(linkInfo.ColumnName, linkRecordId));
                }
                else
                {
                    this.AddCondition(new TreeFieldNameCondition(linkInfo.GetPhysicalColumnName(), linkRecordId));
                }
            }
            else if (linkInfo.IsFieldLink)
            {
                var fkCondition =
                    new TreeItemConditionForeignKey(
                        this.GetTableInfo().GetFieldName(linkInfo.SourceFieldId),
                        foreigntableinfo.DatabaseTableName,
                        foreigntableinfo.GetFieldName(linkInfo.DestFieldId));

                fkCondition.AddForeignTableCondition(
                    new TreeItemConditionFieldValue(FieldIdType.RECORDID, linkRecordId));
                this.AddCondition(fkCondition);
            }
            else
            {
                if (reverseLink == null)
                {
                    return false;
                }

                this.AddForeignTableConditionForFieldTypeRecordId(foreigntableinfo, rootPhysicalInfoAreaId, reverseLink, linkRecordId);
            }

            return true;
        }

        private void AddForeignTableConditionForFieldTypeRecordId(TableInfo foreigntableinfo, string rootPhysicalInfoAreaId, LinkInfo reverseLink, string linkRecordId)
        {
            var fkCondition = new TreeItemConditionForeignKey(
                                this.GetTableInfo().GetFieldName(FieldIdType.RECORDID),
                                foreigntableinfo.DatabaseTableName,
                                reverseLink.GetPhysicalColumnName());

            fkCondition.AddForeignTableCondition(
                new TreeItemConditionFieldValue(FieldIdType.RECORDID, linkRecordId));

            if (reverseLink.LinkId == 126)
            {
                fkCondition.AddForeignTableCondition(
                    new TreeFieldNameCondition(reverseLink.InfoAreaColumnName, rootPhysicalInfoAreaId));
            }

            this.AddCondition(fkCondition);
        }

        private void SetForeignKeyLinkOnNoLinkInfo(TableInfo foreigntableinfo, string rootPhysicalInfoAreaId, LinkInfo reverseLink, string linkInfoAreaId, string linkRecordId, int linkId)
        {
            if (reverseLink.UseLinkFields)
            {
                var subNode = new QueryTreeItem(this.CrmDatabase, linkInfoAreaId, linkId);
                subNode.SetLinkRecord(linkInfoAreaId, linkRecordId, -1);
                this.AddSubNode("HAVING", subNode);
            }
            else if (reverseLink.IsFieldLink)
            {
                var fkCondition =
                    new TreeItemConditionForeignKey(
                        this.GetTableInfo().GetFieldName(reverseLink.DestFieldId),
                        foreigntableinfo.DatabaseTableName,
                        foreigntableinfo.GetFieldName(reverseLink.SourceFieldId));

                fkCondition.AddForeignTableCondition(
                    new TreeItemConditionFieldValue(FieldIdType.RECORDID, linkRecordId));
                this.AddCondition(fkCondition);
            }
            else
            {
                var fkCondition =
                    new TreeItemConditionForeignKey(
                        this.GetTableInfo().GetFieldName(FieldIdType.RECORDID),
                        foreigntableinfo.DatabaseTableName,
                        reverseLink.GetPhysicalColumnName());

                fkCondition.AddForeignTableCondition(
                    new TreeItemConditionFieldValue(FieldIdType.RECORDID, linkRecordId));

                if (reverseLink.LinkId == GenericLinkId)
                {
                    fkCondition.AddForeignTableCondition(
                        new TreeFieldNameCondition(reverseLink.InfoAreaColumnName, rootPhysicalInfoAreaId));
                }

                this.AddCondition(fkCondition);
            }
        }

        private bool SetVirtualLink(TableInfo tableInfo, string linkInfoAreaId, string linkRecordId, int linkId)
        {
            TableInfo intermediateTableInfo = null;
            var virtualLinkInfo = tableInfo.GetVirtualLinkInfo(linkInfoAreaId, linkId);

            if (virtualLinkInfo != null)
            {
                intermediateTableInfo =
                    this.CrmDatabase.GetTableInfoByInfoArea(virtualLinkInfo.IntermediateInfoAreaId);
                if (intermediateTableInfo == null)
                {
                    virtualLinkInfo = null;
                }
            }

            if (virtualLinkInfo != null)
            {
                var linkToSource = virtualLinkInfo.LinkToSource;
                var linkToTarget = virtualLinkInfo.LinkToTarget;

                if (linkToSource.UseLinkFields || linkToTarget.UseLinkFields)
                {
                    var intermediateSubNode = new QueryTreeItem(
                        this.CrmDatabase,
                        intermediateTableInfo.InfoAreaId,
                        linkToSource.ReverseLinkId);

                    var subNode = new QueryTreeItem(this.CrmDatabase, linkInfoAreaId, linkId);
                    subNode.SetLinkRecord(linkInfoAreaId, linkRecordId, -1);
                    intermediateSubNode.AddSubNode("HAVING", subNode);
                    this.AddSubNode("HAVING", intermediateSubNode);
                    return true;
                }

                var fkCondition = new TreeItemConditionForeignKey(
                    tableInfo.GetFieldName(FieldIdType.RECORDID),
                    intermediateTableInfo.DatabaseTableName,
                    linkToSource.ColumnName);

                var condition = new TreeFieldNameCondition(linkToTarget.ColumnName, linkRecordId);

                fkCondition.AddForeignTableCondition(condition);
                this.AddCondition(fkCondition);
                this.hasVirtualLinkCondition = true;
                return true;
            }

            return false;
        }

        private int AddLinkOutputColumn(StringBuilder stringBuilder, int linkCount, string fieldPrefix, int nextFieldCount, ref int startIndex)
        {
            if (linkCount > 0)
            {
                this.hasOutputColumns = true;

                for (var linkIndex = 0; linkIndex < linkCount; linkIndex++)
                {
                    if (linkIndex > 0 || startIndex > 0)
                    {
                        stringBuilder.Append(Comma);
                    }

                    if (!string.IsNullOrWhiteSpace(fieldPrefix))
                    {
                        stringBuilder.Append(fieldPrefix);
                    }

                    if (this.recordTemplate != null)
                    {
                        stringBuilder.Append(this.recordTemplate.GetLinkNameByIndex(linkIndex));
                    }
                }

                startIndex += linkCount;
                nextFieldCount += linkCount;
            }

            return nextFieldCount;
        }

        private int AddRecordTemplateOutputColumn(StringBuilder stringBuilder, int fieldCount, string fieldPrefix, int nextFieldCount, ref int startIndex)
        {
            if (this.recordTemplate != null && this.RecordIdIndex == -1)
            {
                if (fieldCount > 0 || startIndex > 0)
                {
                    stringBuilder.Append(Comma);
                }

                if (!string.IsNullOrWhiteSpace(fieldPrefix))
                {
                    stringBuilder.Append(fieldPrefix);
                }

                stringBuilder.Append(this.recordTemplate.GetFieldName(FieldIdType.RECORDID));

                this.RecordIdIndex = nextFieldCount++ + this.OutputColumnStartIndex;
                ++startIndex;
            }

            if (this.recordTemplate != null && this.InfoAreaIdIndex == -1)
            {
                if (fieldCount > 0 || startIndex > 0)
                {
                    stringBuilder.Append(Comma);
                }

                if (!string.IsNullOrWhiteSpace(fieldPrefix))
                {
                    stringBuilder.Append(fieldPrefix);
                }

                stringBuilder.Append(this.recordTemplate.GetFieldName(FieldIdType.INFOAREAID));

                this.InfoAreaIdIndex = nextFieldCount + this.OutputColumnStartIndex;
                ++startIndex;
            }

            return nextFieldCount;
        }

        private void AddFieldOutputColumn(StringBuilder stringBuilder, int fieldCount, int startIndex, string fieldPrefix)
        {
            this.hasOutputColumns = true;

            for (var fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
            {
                if (fieldIndex > 0 || startIndex > 0)
                {
                    stringBuilder.Append(Comma);
                }

                if (this.recordTemplate != null && this.recordTemplate.GetFieldIndex(fieldIndex) == (int)FieldIdType.EMPTY)
                {
                    stringBuilder.Append(NullString);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(fieldPrefix))
                    {
                        stringBuilder.Append(fieldPrefix);
                    }

                    if (this.recordTemplate == null)
                    {
                        stringBuilder.Append(NullString);
                    }
                    else
                    {
                        stringBuilder.Append(this.recordTemplate.GetFieldNameByIndex(fieldIndex));

                        switch (this.recordTemplate.GetFieldIndex(fieldIndex))
                        {
                            case (int)FieldIdType.RECORDID:
                                this.RecordIdIndex = fieldIndex + this.OutputColumnStartIndex;
                                break;
                            case (int)FieldIdType.INFOAREAID:
                                this.InfoAreaIdIndex = fieldIndex + this.OutputColumnStartIndex;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
