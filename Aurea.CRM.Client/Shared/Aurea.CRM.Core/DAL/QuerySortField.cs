// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuerySortField.cs" company="Aurea Software Gmbh">
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
//   Defines the query sort field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using System.Text;

    /// <summary>
    /// Defines the query sort field
    /// </summary>
    public class QuerySortField
    {
        /// <summary>
        /// The field id.
        /// </summary>
        private readonly int fieldId;

        /// <summary>
        /// The query tree item.
        /// </summary>
        private readonly QueryTreeItem queryTreeItem;

        /// <summary>
        /// The alias.
        /// </summary>
        private string alias;

        /// <summary>
        /// The code column.
        /// </summary>
        private string codeColumn;

        /// <summary>
        /// The free column names.
        /// </summary>
        private bool freeColumnNames;

        /// <summary>
        /// The lookup table name.
        /// </summary>
        private string lookupTableName;

        /// <summary>
        /// The prio sort column.
        /// </summary>
        private string prioSortColumn;

        /// <summary>
        /// The sort column.
        /// </summary>
        private string sortColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySortField"/> class.
        /// </summary>
        /// <param name="treeItem">
        /// The tree item.
        /// </param>
        /// <param name="fieldIdentifier">
        /// The field identifier.
        /// </param>
        /// <param name="isDescending">
        /// if set to <c>true</c> [is descending].
        /// </param>
        public QuerySortField(QueryTreeItem treeItem, int fieldIdentifier, bool isDescending)
        {
            this.queryTreeItem = treeItem;
            this.fieldId = fieldIdentifier;
            this.IsDescending = isDescending;

            this.freeColumnNames = true;

            var fieldInfo = this.queryTreeItem.GetFieldInfo(this.fieldId);
            if (fieldInfo == null)
            {
                return;
            }

            switch (fieldInfo.FieldType)
            {
                case 'K':
                case 'X':
                    this.InitializeCatSorting(fieldInfo);
                    break;
            }
        }

        /// <summary>
        /// Determines whether this instance is descending.
        /// </summary>
        /// <returns></returns>
        public bool IsDescending { get; }

        /// <summary>
        /// Determines whether [is fix cat].
        /// </summary>
        /// <returns></returns>
        public bool IsFixCat { get; private set; }

        /// <summary>
        /// Determines whether [is variable cat].
        /// </summary>
        /// <returns></returns>
        public bool IsVarCat { get; private set; }

        /// <summary>
        /// Adds to from.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void AddToFrom(StringBuilder sb, StatementCreationContext context)
        {
            if (string.IsNullOrEmpty(this.lookupTableName))
            {
                return;
            }

            var fieldName = this.queryTreeItem.GetFieldName(this.fieldId);

            // const char *tableAlias = _queryTreeItem->GetAlias();
            // if (tableAlias)
            // sprintf (joinClause, " LEFT JOIN %s AS %s ON %s.%s = %s.%s", _lookupTableName, _alias, _queryTreeItem->GetAlias(), fieldName, _alias, _codeColumn);
            // else
            var joinClause =
                $" LEFT JOIN {this.lookupTableName} AS {this.alias} ON {fieldName} = {this.alias}.{this.codeColumn}";

            sb.Append(joinClause);
        }

        /// <summary>
        /// Adds to order by.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void AddToOrderBy(StringBuilder sb, StatementCreationContext context)
        {
            if (!string.IsNullOrEmpty(this.lookupTableName))
            {
                var _usePrioColumn = (this.IsFixCat && context.Query.SortFixBySortInfoAndCode)
                                     || (this.IsVarCat && context.Query.SortVarBySortInfo);
                var _useCode = this.IsFixCat && context.Query.SortFixBySortInfoAndCode;

                if (_usePrioColumn && !string.IsNullOrEmpty(this.prioSortColumn))
                {
                    sb.Append("CASE ");
                    sb.Append(this.alias);
                    sb.Append(".");
                    sb.Append(this.prioSortColumn);
                    sb.Append(" WHEN 0 THEN 30000 ELSE IFNULL(");
                    sb.Append(this.alias);
                    sb.Append(".");
                    sb.Append(this.prioSortColumn);
                    sb.Append(",32000) END, ");
                }

                sb.Append(this.alias);
                sb.Append(".");

                sb.Append(_useCode ? this.codeColumn : this.sortColumn);
            }
            else
            {
                var fieldName = this.queryTreeItem.GetFieldName(this.fieldId);
                sb.Append(fieldName);
            }

            var collation = context.Collation;

            if (!string.IsNullOrEmpty(collation))
            {
                var fieldInfo = this.queryTreeItem.GetFieldInfo(this.fieldId);

                if (fieldInfo != null && !fieldInfo.IsNumeric)
                {
                    sb.Append(" COLLATE ");
                    sb.Append(collation);
                }
            }

            if (this.IsDescending)
            {
                sb.Append(" DESC");
            }
        }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetFieldId() => this.fieldId;

        /// <summary>
        /// Gets the tree item.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryTreeItem"/>.
        /// </returns>
        public QueryTreeItem GetTreeItem() => this.queryTreeItem;

        /// <summary>
        /// Initializes the cat sorting.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        private void InitializeCatSorting(FieldInfo fieldInfo)
        {
            CatalogInfo catalogInfo;
            if (fieldInfo.FieldType == 'X')
            {
                this.IsFixCat = true;
                this.IsVarCat = false;
                catalogInfo = this.queryTreeItem.CrmDatabase.DataModel.GetFixCat(fieldInfo.Cat);
            }
            else
            {
                this.IsVarCat = true;
                this.IsFixCat = false;
                catalogInfo = this.queryTreeItem.CrmDatabase.DataModel.GetVarCat(fieldInfo.Cat);
            }

            if (catalogInfo == null)
            {
                return;
            }

            this.lookupTableName = catalogInfo.GetDatabaseTableName();
            this.freeColumnNames = false;
            this.codeColumn = catalogInfo.GetCodeColumnName();
            this.sortColumn = catalogInfo.GetTextColumnName();

            this.prioSortColumn = catalogInfo.GetSortInfoColumnName();
            var parentAlias = this.queryTreeItem.Alias;

            this.alias = !string.IsNullOrEmpty(parentAlias)
                             ? $"{parentAlias}_S{fieldInfo.FieldId}"
                             : $"S{fieldInfo.FieldId}";
        }
    }
}
