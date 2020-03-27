// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Query.cs" company="Aurea Software Gmbh">
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
//   IMplements the database query abstraction
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Collections.Generic;
    using System.Text;

    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// IMplements the database query abstraction
    /// </summary>
    public class Query
    {
        /// <summary>
        /// The crm database.
        /// </summary>
        protected CRMDatabase crmDatabase;

        /// <summary>
        /// The foreign root.
        /// </summary>
        private bool foreignRoot;

        /// <summary>
        /// The sort fields.
        /// </summary>
        private List<QuerySortField> sortFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        public Query(CRMDatabase crmDatabase, string infoAreaId)
        {
            this.RootTreeItem = new QueryTreeItem(crmDatabase, infoAreaId);
            this.sortFields = null;
            this.MaxResultRowCount = this.SkipResultRowCount = 0;
            this.crmDatabase = crmDatabase;
            this.UseVirtualLinks = true;
            this.CollationName = null;
            this.IgnoreLookupOnRoot = crmDatabase.HasLookupRecords(infoAreaId);
            this.foreignRoot = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="rootTreeItem">
        /// The root tree item.
        /// </param>
        /// <param name="isForign">
        /// if set to <c>true</c> [foreign root].
        /// </param>
        public Query(QueryTreeItem rootTreeItem, bool isForign)
        {
            this.RootTreeItem = rootTreeItem;
            this.UseVirtualLinks = rootTreeItem.UseVirtualLinks;
            this.IgnoreLookupOnRoot = rootTreeItem.IgnoreLookupRows;
            this.sortFields = null;
            this.MaxResultRowCount = this.SkipResultRowCount = 0;
            this.crmDatabase = rootTreeItem.CrmDatabase;
            this.CollationName = null;
            this.foreignRoot = isForign;
        }

        /// <summary>
        /// Gets the collation.
        /// </summary>
        /// <returns></returns>
        public string CollationName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore lookup on root].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [ignore lookup on root]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreLookupOnRoot { get; set; }

        /// <summary>
        /// Gets or sets the maximum result row count.
        /// </summary>
        /// <value>
        /// The maximum result row count.
        /// </value>
        public int MaxResultRowCount { get; set; }

        /// <summary>
        /// Gets the root tree item.
        /// </summary>
        /// <value>
        /// The root tree item.
        /// </value>
        public QueryTreeItem RootTreeItem { get; }

        /// <summary>
        /// Gets or sets the skip result row count.
        /// </summary>
        /// <value>
        /// The skip result row count.
        /// </value>
        public int SkipResultRowCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [sort fix by sort information and code].
        /// </summary>
        /// <value>
        /// <c>true</c> if [sort fix by sort information and code]; otherwise, <c>false</c>.
        /// </value>
        public bool SortFixBySortInfoAndCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [sort variable by sort information].
        /// </summary>
        /// <value>
        /// <c>true</c> if [sort variable by sort information]; otherwise, <c>false</c>.
        /// </value>
        public bool SortVarBySortInfo { get; set; }

        /// <summary>
        /// Uses the virtual links.
        /// </summary>
        /// <returns></returns>
        public bool UseVirtualLinks { get; private set; }

        /// <summary>
        /// The sort field count.
        /// </summary>
        private int SortFieldCount => this.sortFields?.Count ?? 0;

        /// <summary>
        /// Adds the sort field.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="descending">
        /// if set to <c>true</c> [descending].
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddSortField(string infoAreaId, int linkId, int fieldId, bool descending)
        {
            var treeItem = this.RootTreeItem.GetSubNode(infoAreaId, linkId);
            if (treeItem == null)
            {
                return false;
            }

            this.AddSortField(treeItem, fieldId, descending);
            return true;
        }

        /// <summary>
        /// Adds the sort field.
        /// </summary>
        /// <param name="queryTreeItem">
        /// The query tree item.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <param name="descending">
        /// if set to <c>true</c> [descending].
        /// </param>
        public void AddSortField(QueryTreeItem queryTreeItem, int fieldId, bool descending)
        {
            var sortField = new QuerySortField(queryTreeItem, fieldId, descending);
            if (this.sortFields == null)
            {
                this.sortFields = new List<QuerySortField>();
            }

            this.sortFields.Add(sortField);
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
            subNode.SetUseVirtualLinks(this.UseVirtualLinks);
            this.RootTreeItem.AddSubNode(relation, subNode);
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Count()
        {
            var recordSet = this.Execute(true);

            if (recordSet == null)
            {
                return 0;
            }

            if (recordSet.GetRowCount() == 0)
            {
                return 0;
            }

            var row = recordSet.GetRow(0);

            return row.GetColumnInt(0, 0);
        }

        /// <summary>
        /// Creates the statement.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="countOnly">
        /// if set to <c>true</c> [count only].
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CreateStatement(StatementCreationContext context, bool countOnly)
        {
            var sb = new StringBuilder();
            var startIndex = 0;
            int i;
            var needsDistinct = false;
            if (this.NeedsDistinct)
            {
                needsDistinct = true;
                if (countOnly)
                {
                    sb.Append("SELECT COUNT(*) FROM (");
                }
            }

            sb.Append("SELECT ");

            if (needsDistinct)
            {
                sb.Append("DISTINCT ");
            }
            else if (countOnly)
            {
                sb.Append("COUNT(*)");
            }

            this.RootTreeItem.CheckSubQueries();

            if (needsDistinct || !countOnly)
            {
                this.RootTreeItem.AddOutputColumns(sb, ref startIndex, true);
            }

            sb.Append(" FROM ");
            this.RootTreeItem.AddFromPart(sb, context, null);

            if (!string.IsNullOrEmpty(context.ErrorText))
            {
                return null;
            }

            if (this.sortFields != null)
            {
                for (i = 0; i < this.SortFieldCount; i++)
                {
                    var qti = this.sortFields[i].GetTreeItem();
                    if (qti == this.RootTreeItem || qti.SubQuery == null)
                    {
                        this.sortFields[i].AddToFrom(sb, context);
                    }
                }
            }

            if (this.RootTreeItem.Condition != null || this.RootTreeItem.HasExistsConditions() || this.RootTreeItem.IgnoreLookupRows)
            {
                sb.Append(" WHERE ");
                this.RootTreeItem.AddConditionToStatement(sb, context);
            }

            if (!countOnly && this.sortFields != null)
            {
                sb.Append(" ORDER BY ");

                var first = true;
                for (i = 0; i < this.SortFieldCount; i++)
                {
                    var qti = this.sortFields[i].GetTreeItem();
                    if (qti != this.RootTreeItem && qti.SubQuery != null)
                    {
                        continue;
                    }

                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }

                    this.sortFields[i].AddToOrderBy(sb, context);
                }
            }

            if (this.MaxResultRowCount > 0)
            {
                sb.Append($" LIMIT {this.MaxResultRowCount}");

                if (this.SkipResultRowCount > 0)
                {
                    sb.Append($", {this.SkipResultRowCount}");
                }
            }

            if (needsDistinct && countOnly)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Executes the specified count only.
        /// </summary>
        /// <param name="countOnly">
        /// if set to <c>true</c> [count only].
        /// </param>
        /// <returns>
        /// The <see cref="GenericRecordSet"/>.
        /// </returns>
        public GenericRecordSet Execute(bool countOnly)
        {
            var recordSet = new DatabaseRecordSet(this.crmDatabase);
            var context = new StatementCreationContext(this);

            var statementString = this.CreateStatement(context, countOnly);

            if (!string.IsNullOrEmpty(context.ErrorText))
            {
                // _crmDatabase.Trace(_crmDatabase, context.GetErrorText());
                return null;
            }

            var parameterList = context.ParameterValues;
            var parameterCount = parameterList?.Count ?? 0;
            if (parameterCount > 0)
            {
                recordSet.Execute(statementString, parameterList?.ToArray(), this.MaxResultRowCount);
            }
            else
            {
                recordSet.Execute(statementString, this.MaxResultRowCount);
            }

            if (recordSet.GetRowCount() > 0 && this.RootTreeItem.HasSubQueries())
            {
                var joinedRecordSet = this.RootTreeItem.ExecuteSubQueries(recordSet);
                return joinedRecordSet;
            }

            return recordSet;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="GenericRecordSet"/>.
        /// </returns>
        public GenericRecordSet Execute()
        {
            return this.Execute(false);
        }

        /// <summary>
        /// Gets a value indicating whether [needs distinct].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [needs distinct]; otherwise, <c>false</c>.
        /// </value>
        public bool NeedsDistinct => this.UseVirtualLinks && this.RootTreeItem.NeedsVirtualLinks();

        /// <summary>
        /// Sets the collation.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public void SetCollation(string name)
        {
            this.CollationName = name;
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
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetLinkRecord(string linkInfoAreaId, string linkRecordId)
        {
            return this.SetLinkRecord(linkInfoAreaId, linkRecordId, 0);
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
            return this.RootTreeItem.SetLinkRecord(linkInfoAreaId, linkRecordId, linkId);
        }

        /// <summary>
        /// Sets the use virtual links.
        /// </summary>
        /// <param name="useLinks">
        /// if set to <c>true</c> [use links].
        /// </param>
        public void SetUseVirtualLinks(bool useLinks)
        {
            this.UseVirtualLinks = useLinks;
            this.RootTreeItem?.SetUseVirtualLinks(this.UseVirtualLinks);
        }

        /// <summary>
        /// Trees the item from record template.
        /// </summary>
        /// <param name="recordTemplate">
        /// The record template.
        /// </param>
        /// <returns>
        /// The <see cref="QueryTreeItem"/>.
        /// </returns>
        public QueryTreeItem TreeItemFromRecordTemplate(RecordTemplate recordTemplate)
        {
            return this.RootTreeItem?.TreeItemFromRecordTemplate(recordTemplate);
        }
    }
}
