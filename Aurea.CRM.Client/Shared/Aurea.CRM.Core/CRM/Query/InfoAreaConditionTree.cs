// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoAreaConditionTree.cs" company="Aurea Software Gmbh">
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
//   Info area condition tree implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Query
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Info area condition tree implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Query.UPInfoAreaCondition" />
    public class UPInfoAreaConditionTree : UPInfoAreaCondition
    {
        /// <summary>
        /// The conditions.
        /// </summary>
        private readonly List<UPInfoAreaCondition> conditions;

        /// <summary>
        /// The relation.
        /// </summary>
        private readonly string relation;

        // creates an internal cpp querycondition object
        // creates an json object for server requests
        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaConditionTree"/> class.
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
        public UPInfoAreaConditionTree(string relation, UPInfoAreaCondition left, UPInfoAreaCondition right)
            : this(relation)
        {
            this.AddSubCondition(left);
            this.AddSubCondition(right);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaConditionTree"/> class.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public UPInfoAreaConditionTree(string relation, UPInfoAreaCondition condition)
            : this(relation)
        {
            this.AddSubCondition(condition);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaConditionTree"/> class.
        /// </summary>
        /// <param name="relation">
        /// The relation.
        /// </param>
        public UPInfoAreaConditionTree(string relation)
        {
            this.relation = relation;
            this.conditions = new List<UPInfoAreaCondition>();
        }

        /// <summary>
        /// Gets a list of <see cref="UPInfoAreaCondition"/> that are represented by this object
        /// </summary>
        public List<UPInfoAreaCondition> Conditions => this.conditions;

        /// <summary>
        /// Gets a relation type of subconditions in this object
        /// </summary>
        public string Relation => this.relation;

        /// <summary>
        /// Adds the sub condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void AddSubCondition(UPInfoAreaCondition condition)
        {
            this.conditions.Add(condition);
        }

        /// <summary>
        /// Conditions to object.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ConditionToObject()
        {
            var count = this.conditions.Count;
            var subConditionArray = new List<object>(count);
            for (var i = 0; i < count; i++)
            {
                subConditionArray.Add(this.conditions[i].ConditionToObject());
            }

            return new List<object> { this.relation, subConditionArray };
        }

        /// <summary>
        /// Creates the query condition options.
        /// </summary>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="TreeItemConditionRelation"/>.
        /// </returns>
        public override TreeItemCondition CreateQueryConditionOptions(CRMDatabase crmDatabase, int options)
        {
            var condition = new TreeItemConditionRelation(this.relation);
            foreach (var subCondition in this.conditions)
            {
                condition.AddSubCondition(subCondition.CreateQueryConditionOptions(crmDatabase, options));
            }

            return condition;
        }

        /// <summary>
        /// Dates the time condition for information area parent.
        /// </summary>
        /// <param name="infoArea">
        /// The information area.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaDateTimeCondition"/>.
        /// </returns>
        public override UPInfoAreaDateTimeCondition DateTimeConditionForInfoAreaParent(
            UPContainerInfoAreaMetaInfo infoArea,
            UPInfoAreaConditionTree parent)
        {
            if (this.conditions == null)
            {
                return null;
            }

            UPInfoAreaDateTimeCondition combinedCondition = null;
            if (this.relation == "AND")
            {
                foreach (var cond in this.conditions)
                {
                    var dateTimeCond = cond.DateTimeConditionForInfoAreaParent(infoArea, this);
                    if (dateTimeCond != null)
                    {
                        combinedCondition =
                            UPInfoAreaDateTimeCondition.ConditionByANDCombiningConditionWithCondition(
                                dateTimeCond,
                                combinedCondition);
                    }
                }
            }
            else
            {
                foreach (var cond in this.conditions)
                {
                    var dateTimeCond = cond.DateTimeConditionForInfoAreaParent(infoArea, this);
                    if (dateTimeCond != null || combinedCondition != null)
                    {
                        combinedCondition =
                            UPInfoAreaDateTimeCondition.ConditionByOrCombiningConditionWithCondition(
                                dateTimeCond,
                                combinedCondition);
                    }
                }
            }

            return combinedCondition;
        }

        /// <summary>
        /// Informations the area condition by appending and condition.
        /// </summary>
        /// <param name="cond">
        /// The cond.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public override UPInfoAreaCondition InfoAreaConditionByAppendingAndCondition(UPInfoAreaCondition cond)
        {
            if (this.relation == "AND")
            {
                this.AddSubCondition(cond);
                return this;
            }

            return base.InfoAreaConditionByAppendingAndCondition(cond);
        }

        /// <summary>
        /// Informations the area condition by appending or condition.
        /// </summary>
        /// <param name="cond">
        /// The cond.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public override UPInfoAreaCondition InfoAreaConditionByAppendingOrCondition(UPInfoAreaCondition cond)
        {
            if (this.relation == "OR")
            {
                this.AddSubCondition(cond);
                return this;
            }

            return base.InfoAreaConditionByAppendingOrCondition(cond);
        }

        /// <summary>
        /// Replaces the condition with condition.
        /// </summary>
        /// <param name="oldCondition">
        /// The old condition.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ReplaceConditionWithCondition(UPInfoAreaCondition oldCondition, UPInfoAreaCondition condition)
        {
            if (this.conditions.Count == 0)
            {
                return false;
            }

            var replaceIndex = this.conditions.IndexOf(oldCondition);
            if (replaceIndex == -1)
            {
                return false;
            }

            this.conditions[replaceIndex] = condition;
            return true;
        }
    }
}
