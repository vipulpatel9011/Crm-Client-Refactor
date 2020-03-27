// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TreeItemCondition.cs" company="Aurea Software Gmbh">
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
//   Abstract implementation of tree item condition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Abstract implementation of tree item condition
    /// </summary>
    public abstract class TreeItemCondition
    {
        /// <summary>
        /// Adds the condition to statement.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="tableAlias">
        /// The table alias.
        /// </param>
        public abstract void AddConditionToStatement(
            StringBuilder sb,
            StatementCreationContext context,
            string tableAlias);
    }

    /// <summary>
    /// Tree item condition relation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.TreeItemCondition" />
    public class TreeItemConditionRelation : TreeItemCondition
    {
        /// <summary>
        /// The relation.
        /// </summary>
        private readonly string relation;

        /// <summary>
        /// The sub items.
        /// </summary>
        private List<TreeItemCondition> subItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemConditionRelation"/> class.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        public TreeItemConditionRelation(string relation, TreeItemCondition left, TreeItemCondition right)
        {
            this.subItems = new List<TreeItemCondition>(2) { left, right };
            this.relation = relation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemConditionRelation"/> class.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        /// <param name="single">
        /// The single.
        /// </param>
        public TreeItemConditionRelation(string relation, TreeItemCondition single)
        {
            this.subItems = new List<TreeItemCondition>(1) { single };
            this.relation = relation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemConditionRelation"/> class.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        public TreeItemConditionRelation(string relation)
        {
            this.subItems = null;
            this.relation = relation;
        }

        /// <summary>
        /// The sub item count.
        /// </summary>
        private int SubItemCount => this.subItems?.Count ?? 0;

        /// <summary>
        /// Adds the condition to statement.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="tableAlias">
        /// The table alias.
        /// </param>
        public override void AddConditionToStatement(StringBuilder sb, StatementCreationContext context, string tableAlias)
        {
            if (this.SubItemCount == 1)
            {
                this.subItems[0].AddConditionToStatement(sb, context, tableAlias);
            }
            else if (this.SubItemCount > 1)
            {
                for (var i = 0; i < this.SubItemCount; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(") ");
                        sb.Append(this.relation);
                        sb.Append(" (");
                    }
                    else
                    {
                        sb.Append("(");
                    }

                    this.subItems[i].AddConditionToStatement(sb, context, tableAlias);
                }

                sb.Append(")");
            }
        }

        /// <summary>
        /// Adds the sub condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void AddSubCondition(TreeItemCondition condition)
        {
            if (this.subItems == null)
            {
                this.subItems = new List<TreeItemCondition>();
            }

            this.subItems.Add(condition);
        }
    }

    /// <summary>
    /// Field value of a tree item condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.TreeItemCondition" />
    public class TreeItemConditionFieldValue : TreeItemCondition
    {
        /// <summary>
        /// The field compare.
        /// </summary>
        private readonly string fieldCompare;

        /* class TreeItemConditionFieldValue */

        /// <summary>
        /// The field index.
        /// </summary>
        private readonly FieldIdType fieldIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemConditionFieldValue"/> class.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        public TreeItemConditionFieldValue(FieldIdType fieldIndex, string fieldValue)
            : this(fieldIndex, fieldValue, "=")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemConditionFieldValue"/> class.
        /// </summary>
        /// <param name="fieldIndex">
        /// Index of the field.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        /// <param name="compare">
        /// The compare.
        /// </param>
        public TreeItemConditionFieldValue(FieldIdType fieldIndex, string fieldValue, string compare)
        {
            this.fieldIndex = fieldIndex;
            this.FieldValue = fieldValue;
            this.fieldCompare = compare;
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public string FieldValue { get; private set; }

        /// <summary>
        /// Adds the condition to statement.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="tableAlias">
        /// The table alias.
        /// </param>
        public override void AddConditionToStatement(
            StringBuilder sb,
            StatementCreationContext context,
            string tableAlias)
        {
            var equal = this.fieldCompare == "=";
            var notequal = this.fieldCompare == "<>";
            var containsEqual = this.fieldCompare.Contains("=");

            var fieldName = context.RecordTemplate.GetFieldName(this.fieldIndex);
            var fieldInfo = context.RecordTemplate.GetFieldInfo(this.fieldIndex);

            string alternateValue = null;

            if (fieldInfo != null)
            {
                if (!string.IsNullOrEmpty(this.FieldValue))
                {
                    if (fieldInfo.IsNumeric)
                    {
                        if (this.FieldValue == "0")
                        {
                            if (equal || notequal || containsEqual)
                            {
                                this.FieldValue = null;
                                alternateValue = "0";
                            }
                        }
                    }
                    else if (fieldInfo.IsCatalog)
                    {
                        if (this.FieldValue == "0")
                        {
                            this.FieldValue = null;
                            alternateValue = "0";
                        }
                    }

                    if (fieldInfo.FieldType == 'B' && this.FieldValue == "false")
                    {
                        this.FieldValue = null;
                        alternateValue = "false";
                    }
                }
                else if (fieldInfo.IsNumeric)
                {
                    if (equal || notequal || containsEqual)
                    {
                        alternateValue = "0";
                    }
                }
                else if (fieldInfo.IsCatalog)
                {
                    alternateValue = "0";
                }
                else if (fieldInfo.FieldType == 'B')
                {
                    alternateValue = "false";
                }
            }

            if (string.IsNullOrEmpty(this.FieldValue))
            {
                sb.Append("(");
                if (!string.IsNullOrEmpty(tableAlias))
                {
                    sb.Append(tableAlias);
                    sb.Append(".");
                }

                sb.Append(fieldName);
                sb.Append(" ");

                if (!string.IsNullOrEmpty(alternateValue))
                {
                    sb.Append((equal || containsEqual) ? "=" : "<>");
                    sb.Append(" ?");
                    context.AddParameterValue(alternateValue);
                    sb.Append((equal || containsEqual) ? " OR " : " AND ");
                    if (!string.IsNullOrEmpty(tableAlias))
                    {
                        sb.Append(tableAlias);
                        sb.Append(".");
                    }

                    sb.Append(fieldName);
                    sb.Append(" ");
                }

                sb.Append(this.fieldCompare);
                sb.Append(" ?");
                context.AddParameterValue(string.Empty);

                sb.Append((equal || containsEqual) ? " OR " : " AND ");

                if (!equal && !containsEqual)
                {
                    sb.Append("NOT ");
                }

                if (!string.IsNullOrEmpty(tableAlias))
                {
                    sb.Append(tableAlias);
                    sb.Append(".");
                }

                sb.Append(fieldName);
                sb.Append(" IS NULL");
                sb.Append(")");
            }
            else
            {
                var checkEqualNull = notequal;
                CatalogInfo catalogInfo = null;
                string participantsTableName = null;

                if (fieldInfo != null && fieldInfo.IsParticipantsField)
                {
                    var tableInfo = context.RecordTemplate.GetTableInfo();
                    if (tableInfo != null)
                    {
                        participantsTableName = tableInfo.GetDatabaseTableNameForParticipantsField(fieldInfo);
                    }
                }

                if (!string.IsNullOrEmpty(participantsTableName))
                {
                    sb.Append("EXISTS (SELECT repId FROM ");
                    sb.Append(participantsTableName);
                    sb.Append(" WHERE repid ");
                    checkEqualNull = false;
                }
                else
                {
                    if (fieldInfo != null)
                    {
                        switch (fieldInfo.FieldType)
                        {
                            case 'K':
                                if (this.IsCatalogTextValue())
                                {
                                    catalogInfo = context.RecordTemplate.Database.DataModel.GetVarCat(fieldInfo.Cat);
                                }

                                break;
                            case 'X':
                                if (this.IsCatalogTextValue())
                                {
                                    catalogInfo = context.RecordTemplate.Database.DataModel.GetFixCat(fieldInfo.Cat);
                                }

                                break;
                        }

                        if (catalogInfo != null)
                        {
                            checkEqualNull = false;
                        }
                    }

                    if (checkEqualNull)
                    {
                        sb.Append("(");
                    }

                    if (!string.IsNullOrEmpty(tableAlias))
                    {
                        sb.Append(tableAlias);
                        sb.Append(".");
                    }

                    sb.Append(fieldName);
                    sb.Append(" ");

                    if (catalogInfo != null)
                    {
                        sb.Append("IN (SELECT code FROM ");
                        sb.Append(catalogInfo.GetDatabaseTableName());
                        sb.Append(" WHERE text ");
                    }

                    if ((fieldInfo?.RepMode?.Contains("Rep") ?? false) && this.FieldValue.Contains("*"))
                    {
                        sb.Append("IN (SELECT F0 FROM CRM_ID WHERE F3 ");
                    }
                }

                if (this.FieldValue != null && (equal || notequal) && (this.FieldValue.Contains("*") || this.FieldValue.Contains("?")))
                {
                    for (var i = 0; i < this.FieldValue?.Length; i++)
                    {
                        if (this.FieldValue[i] == '*')
                        {
                            var tb = new StringBuilder(this.FieldValue) { [i] = '%' };
                            this.FieldValue = tb.ToString();
                        }

                        if (this.FieldValue[i] == '?')
                        {
                            var tb = new StringBuilder(this.FieldValue) { [i] = '_' };
                            this.FieldValue = tb.ToString();
                        }
                    }

                    sb.Append(equal ? " LIKE " : " NOT LIKE ");
                }
                else
                {
                    sb.Append(this.fieldCompare);
                }

                sb.Append(" ?");
                context.AddParameterValue(this.FieldValue);

                if (catalogInfo != null || ((fieldInfo?.RepMode?.Contains("Rep") ?? false) && this.FieldValue.Contains("%")))
                {
                    sb.Append(")");
                }
                else if (!string.IsNullOrEmpty(participantsTableName))
                {
                    sb.Append(" AND ");
                    sb.Append(participantsTableName);
                    sb.Append(".recid = ");
                    sb.Append(tableAlias);
                    sb.Append(".recid)");
                }
                else if (checkEqualNull)
                {
                    sb.Append(" OR ");
                    if (!string.IsNullOrEmpty(tableAlias))
                    {
                        sb.Append(tableAlias);
                        sb.Append(".");
                    }

                    sb.Append(fieldName);
                    sb.Append(" IS NULL)");
                }
            }
        }

        /// <summary>
        /// Changes the field value.
        /// </summary>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        public void ChangeFieldValue(string fieldValue)
        {
            this.FieldValue = fieldValue;
        }

        /// <summary>
        /// Determines whether [is catalog text value].
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsCatalogTextValue()
        {
            int value;

            return !int.TryParse(this.FieldValue, out value);
        }
    }

    /// <summary>
    /// Defines the tree field name condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.TreeItemCondition" />
    public class TreeFieldNameCondition : TreeItemCondition
    {
        /// <summary>
        /// The field compare.
        /// </summary>
        private readonly string fieldCompare;

        /// <summary>
        /// The field name.
        /// </summary>
        private readonly string fieldName;

        /// <summary>
        /// The field value.
        /// </summary>
        private readonly string fieldValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeFieldNameCondition"/> class.
        /// </summary>
        /// <param name="fieldName">
        /// Name of the field.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        /// <param name="compare">
        /// The compare.
        /// </param>
        public TreeFieldNameCondition(string fieldName, string fieldValue, string compare = "=")
        {
            this.fieldName = fieldName;
            this.fieldCompare = compare;
            this.fieldValue = fieldValue;
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
        /// <param name="tableAlias">
        /// The table alias.
        /// </param>
        public override void AddConditionToStatement(StringBuilder sb, StatementCreationContext context, string tableAlias)
        {
            if (!string.IsNullOrEmpty(tableAlias))
            {
                sb.Append(tableAlias);
                sb.Append(".");
            }

            sb.Append(this.fieldName);
            sb.Append(" ");
            sb.Append(this.fieldCompare);
            sb.Append(" ?");
            context.AddParameterValue(this.fieldValue);
        }
    }

    /// <summary>
    /// Tree item condition forign key condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.DAL.TreeItemCondition" />
    public class TreeItemConditionForeignKey : TreeItemCondition
    {
        /// <summary>
        /// The field name.
        /// </summary>
        private readonly string fieldName;

        /// <summary>
        /// The foreign field.
        /// </summary>
        private readonly string foreignField;

        /// <summary>
        /// The foreign table.
        /// </summary>
        private readonly string foreignTable;

        /// <summary>
        /// The foreign condition.
        /// </summary>
        private TreeItemCondition foreignCondition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeItemConditionForeignKey"/> class.
        /// </summary>
        /// <param name="fieldName">
        /// Name of the field.
        /// </param>
        /// <param name="foreignTable">
        /// The foreign table.
        /// </param>
        /// <param name="foreignField">
        /// The foreign field.
        /// </param>
        public TreeItemConditionForeignKey(string fieldName, string foreignTable, string foreignField)
        {
            this.fieldName = fieldName;
            this.foreignTable = foreignTable;
            this.foreignField = foreignField;
            this.foreignCondition = null;
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
        /// <param name="tableAlias">
        /// The table alias.
        /// </param>
        public override void AddConditionToStatement(StringBuilder sb, StatementCreationContext context, string tableAlias)
        {
            if (!string.IsNullOrEmpty(tableAlias))
            {
                sb.Append(tableAlias);
                sb.Append(".");
            }

            sb.Append(this.fieldName);
            sb.Append(" IN (SELECT ");
            sb.Append(this.foreignField);
            sb.Append(" FROM ");
            sb.Append(this.foreignTable);
            sb.Append(" AS FKEY");

            if (this.foreignCondition != null)
            {
                sb.Append(" WHERE ");
                this.foreignCondition.AddConditionToStatement(sb, context, "FKEY");
            }

            sb.Append(")");
        }

        /// <summary>
        /// Adds the foreign table condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="TreeItemCondition"/>.
        /// </returns>
        public TreeItemCondition AddForeignTableCondition(TreeItemCondition condition)
        {
            this.foreignCondition = this.foreignCondition == null
                                        ? condition
                                        : new TreeItemConditionRelation("AND", this.foreignCondition, condition);
            return this.foreignCondition;
        }
    }
}