// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoAreaDateTimeCondition.cs" company="Aurea Software Gmbh">
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
//   Info area date time condition
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Info area date time condition
    /// </summary>
    public class UPInfoAreaDateTimeCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaDateTimeCondition"/> class.
        /// </summary>
        /// <param name="infoAreaMetaInfo">
        /// The _info area meta information.
        /// </param>
        public UPInfoAreaDateTimeCondition(UPContainerInfoAreaMetaInfo infoAreaMetaInfo)
        {
            this.InfoAreaMetaInfo = infoAreaMetaInfo;
        }

        /// <summary>
        /// Gets the information area meta information.
        /// </summary>
        /// <value>
        /// The information area meta information.
        /// </value>
        public UPContainerInfoAreaMetaInfo InfoAreaMetaInfo { get; private set; }

        /// <summary>
        /// Conditions the by and combining condition with condition.
        /// </summary>
        /// <param name="condition1">
        /// The condition1.
        /// </param>
        /// <param name="condition2">
        /// The condition2.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaDateTimeCondition"/>.
        /// </returns>
        public static UPInfoAreaDateTimeCondition ConditionByANDCombiningConditionWithCondition(
            UPInfoAreaDateTimeCondition condition1,
            UPInfoAreaDateTimeCondition condition2)
        {
            if (condition1 == null)
            {
                return condition2;
            }

            if (condition2 == null)
            {
                return condition1;
            }

            if (condition1 is UPInfoAreaORDateTimeCondition)
            {
                ((UPInfoAreaORDateTimeCondition)condition1).AppendCondition(condition2);
                return condition1;
            }

            if (condition2 is UPInfoAreaORDateTimeCondition)
            {
                ((UPInfoAreaORDateTimeCondition)condition2).AppendCondition(condition1);
                return condition2;
            }

            if (condition1 is UPInfoAreaANDDateTimeCondition)
            {
                ((UPInfoAreaANDDateTimeCondition)condition1).AppendCondition(condition2);
                return condition1;
            }

            if (condition2 is UPInfoAreaANDDateTimeCondition)
            {
                ((UPInfoAreaANDDateTimeCondition)condition2).AppendCondition(condition1);
                return condition2;
            }

            return
                ((UPInfoAreaSingleDateTimeCondition)condition1).AppendSingleCondition(
                    (UPInfoAreaSingleDateTimeCondition)condition2)
                    ? condition1
                    : new UPInfoAreaANDDateTimeCondition(
                          condition1.InfoAreaMetaInfo,
                          (UPInfoAreaSingleDateTimeCondition)condition1,
                          (UPInfoAreaSingleDateTimeCondition)condition2);
        }

        /// <summary>
        /// Conditions the by or combining condition with condition.
        /// </summary>
        /// <param name="condition1">
        /// The condition1.
        /// </param>
        /// <param name="condition2">
        /// The condition2.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaDateTimeCondition"/>.
        /// </returns>
        public static UPInfoAreaDateTimeCondition ConditionByOrCombiningConditionWithCondition(
            UPInfoAreaDateTimeCondition condition1,
            UPInfoAreaDateTimeCondition condition2)
        {
            if (condition1 == null)
            {
                return condition2 != null ? OrCombined(condition2) : null;
            }

            if (condition2 == null)
            {
                return OrCombined(condition1);
            }

            if (condition1 is UPInfoAreaORDateTimeCondition)
            {
                ((UPInfoAreaORDateTimeCondition)condition1).AppendCondition(condition2);
                return condition1;
            }

            if (condition2 is UPInfoAreaORDateTimeCondition)
            {
                ((UPInfoAreaORDateTimeCondition)condition2).AppendCondition(condition1);
                return condition2;
            }

            var orCondition = OrCombined(condition1);
            orCondition.AppendCondition(condition2);
            return orCondition;
        }

        /// <summary>
        /// The or combined.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaORDateTimeCondition"/>.
        /// </returns>
        public static UPInfoAreaORDateTimeCondition OrCombined(UPInfoAreaDateTimeCondition condition)
        {
            if (condition is UPInfoAreaORDateTimeCondition)
            {
                return (UPInfoAreaORDateTimeCondition)condition;
            }

            return new UPInfoAreaORDateTimeCondition(condition.InfoAreaMetaInfo, condition);
        }

        /// <summary>
        /// Appends the single condition.
        /// </summary>
        /// <param name="singleCondition">
        /// The single condition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool AppendSingleCondition(UPInfoAreaSingleDateTimeCondition singleCondition)
        {
            return false;
        }

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        public virtual void ApplyTimeZone(UPCRMTimeZone timeZone)
        {
        }
    }

    /// <summary>
    /// Info area single date time condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Query.UPInfoAreaDateTimeCondition" />
    public class UPInfoAreaSingleDateTimeCondition : UPInfoAreaDateTimeCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaSingleDateTimeCondition"/> class.
        /// </summary>
        /// <param name="infoAreaMetaInfo">
        /// The information area meta information.
        /// </param>
        /// <param name="dateFieldId">
        /// The date field identifier.
        /// </param>
        /// <param name="timeFieldId">
        /// The time field identifier.
        /// </param>
        /// <param name="dateFieldCondition">
        /// The date field condition.
        /// </param>
        /// <param name="parentCondition">
        /// The parent condition.
        /// </param>
        public UPInfoAreaSingleDateTimeCondition(
            UPContainerInfoAreaMetaInfo infoAreaMetaInfo,
            int dateFieldId,
            int timeFieldId,
            UPInfoAreaConditionLeaf dateFieldCondition,
            UPInfoAreaConditionTree parentCondition)
            : base(infoAreaMetaInfo)
        {
            this.DateCondition = dateFieldCondition;
            this.ParentCondition = parentCondition;
            this.DateFieldId = dateFieldId;
            this.TimeFieldId = timeFieldId;
        }

        /// <summary>
        /// Gets the date condition.
        /// </summary>
        /// <value>
        /// The date condition.
        /// </value>
        public UPInfoAreaConditionLeaf DateCondition { get; private set; }

        /// <summary>
        /// Gets the date field identifier.
        /// </summary>
        /// <value>
        /// The date field identifier.
        /// </value>
        public int DateFieldId { get; private set; }

        /// <summary>
        /// Gets the parent condition.
        /// </summary>
        /// <value>
        /// The parent condition.
        /// </value>
        public UPInfoAreaConditionTree ParentCondition { get; private set; }

        /// <summary>
        /// Gets the time condition.
        /// </summary>
        /// <value>
        /// The time condition.
        /// </value>
        public UPInfoAreaConditionLeaf TimeCondition { get; private set; }

        /// <summary>
        /// Gets the time field identifier.
        /// </summary>
        /// <value>
        /// The time field identifier.
        /// </value>
        public int TimeFieldId { get; private set; }

        /// <summary>
        /// Afters the date condition including time zone.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="including">
        /// if set to <c>true</c> [including].
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public UPInfoAreaCondition AfterDateCondition(DateTime date, bool including, UPCRMTimeZone timeZone)
        {
            string limitTime = timeZone.GetAdjustedCurrentMMTime(date);
            string limitDate = timeZone.GetAdjustedCurrentMMDate(date);
            if (limitTime == "0000" && including)
            {
                return new UPInfoAreaConditionLeaf(this.InfoAreaMetaInfo.InfoAreaId, this.DateFieldId, ">=", limitDate);
            }
            else if (limitTime == "2359" && !including)
            {
                return new UPInfoAreaConditionLeaf(this.InfoAreaMetaInfo.InfoAreaId, this.DateFieldId, ">", limitDate);
            }

            var timeCondition = new UPInfoAreaConditionLeaf(
                this.InfoAreaMetaInfo.InfoAreaId,
                this.TimeFieldId,
                including ? ">=" : ">",
                limitTime);

            var dateCondition = new UPInfoAreaConditionLeaf(this.InfoAreaMetaInfo.InfoAreaId, this.DateFieldId, limitDate);

            var afterDateCondition = new UPInfoAreaConditionLeaf(this.InfoAreaMetaInfo.InfoAreaId, this.DateFieldId, ">", limitDate);
            var cond = timeCondition.InfoAreaConditionByAppendingAndCondition(dateCondition);

            return cond.InfoAreaConditionByAppendingOrCondition(afterDateCondition);
        }

        /// <summary>
        /// Appends the single condition.
        /// </summary>
        /// <param name="singleCondition">
        /// The single condition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool AppendSingleCondition(UPInfoAreaSingleDateTimeCondition singleCondition)
        {
            if (singleCondition.DateFieldId != this.DateFieldId)
            {
                return false;
            }

            if (this.DateCondition == null & singleCondition.DateCondition != null)
            {
                this.DateCondition = singleCondition.DateCondition;
                return true;
            }

            if (this.TimeCondition != null || singleCondition.TimeCondition == null)
            {
                return false;
            }

            this.TimeCondition = singleCondition.TimeCondition;
            return true;
        }

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        public override void ApplyTimeZone(UPCRMTimeZone timeZone)
        {
            if (this.DateCondition != null && this.TimeCondition != null)
            {
                var date = timeZone.DateFromClientDataMMDateStringTimeString(this.DateCondition.FirstValue, this.TimeCondition.FirstValue);
                if (date != null)
                {
                    this.DateCondition.ReplaceValue(timeZone.GetAdjustedCurrentMMDate(date.Value));
                    this.TimeCondition.ReplaceValue(timeZone.GetAdjustedCurrentMMTime(date.Value));
                }
            }
            else if (this.DateCondition != null)
            {
                var startDate = timeZone.DateFromClientDataMMDateStringTimeString(this.DateCondition.FirstValue, "0000");
                var endDate = timeZone.DateFromClientDataMMDateStringTimeString(this.DateCondition.FirstValue, "2359")?.AddMinutes(60);

                if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                {
                    return;
                }

                if (startDate != null && endDate != null)
                {
                    UPInfoAreaCondition replaceCondition;
                    if (this.DateCondition.CompareOperator == ">=")
                    {
                        replaceCondition = this.AfterDateCondition(startDate.Value, true, timeZone);
                    }
                    else if (this.DateCondition.CompareOperator == ">")
                    {
                        replaceCondition = this.AfterDateCondition(endDate.Value, false, timeZone);
                    }
                    else if (this.DateCondition.CompareOperator == "<=")
                    {
                        replaceCondition = this.BeforeDateCondition(endDate.Value, false, timeZone);
                    }
                    else if (this.DateCondition.CompareOperator == "<")
                    {
                        replaceCondition = this.BeforeDateCondition(startDate.Value, false, timeZone);
                    }
                    else if (this.DateCondition.CompareOperator == "<>")
                    {
                        var cond1 = this.BeforeDateCondition(startDate.Value, false, timeZone);
                        var cond2 = this.AfterDateCondition(endDate.Value, true, timeZone);
                        replaceCondition = cond1.InfoAreaConditionByAppendingOrCondition(cond2);
                    }
                    else
                    {
                        replaceCondition = this.WholeDayConditionForTimeZone(startDate.Value, timeZone);
                    }

                    if (replaceCondition != null)
                    {
                        this.ParentCondition.ReplaceConditionWithCondition(this.DateCondition, replaceCondition);
                    }
                }
            }
        }

        /// <summary>
        /// Befores the date condition including time zone.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="including">
        /// if set to <c>true</c> [including].
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public UPInfoAreaCondition BeforeDateCondition(DateTime date, bool including, UPCRMTimeZone timeZone)
        {
            string limitTime = timeZone.GetAdjustedCurrentMMTime(date);
            string limitDate = timeZone.GetAdjustedCurrentMMDate(date);
            if (limitTime == "0000" && !including)
            {
                return new UPInfoAreaConditionLeaf(this.InfoAreaMetaInfo.InfoAreaId, this.DateFieldId, "<", limitDate);
            }

            if (limitTime == "2359" && including)
            {
                return new UPInfoAreaConditionLeaf(this.InfoAreaMetaInfo.InfoAreaId, this.DateFieldId, "<=", limitDate);
            }

            var timeCondition = new UPInfoAreaConditionLeaf(
                this.InfoAreaMetaInfo.InfoAreaId,
                this.TimeFieldId,
                including ? "<=" : "<",
                limitTime);
            var dateCondition = new UPInfoAreaConditionLeaf(
                this.InfoAreaMetaInfo.InfoAreaId,
                this.DateFieldId,
                limitDate);
            var beforeDateCondition = new UPInfoAreaConditionLeaf(
                this.InfoAreaMetaInfo.InfoAreaId,
                this.DateFieldId,
                "<",
                limitDate);
            var cond = timeCondition.InfoAreaConditionByAppendingAndCondition(dateCondition);
            return cond.InfoAreaConditionByAppendingOrCondition(beforeDateCondition);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var dateString = "(null)";
            var timeString = "(null)";
            if (this.DateCondition != null)
            {
                dateString = $"F{this.DateCondition.FieldIndex}={this.DateCondition.FirstValue}";
            }

            if (this.TimeCondition != null)
            {
                timeString = $"F{this.TimeCondition.FieldIndex}={this.TimeCondition.FirstValue}";
            }

            return $"SINGLE({this.DateFieldId}/{this.TimeFieldId}): date={dateString} time={timeString}";
        }

        /// <summary>
        /// Wholes the day condition for time zone.
        /// </summary>
        /// <param name="startDate">
        /// The Start date.
        /// </param>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        /// <returns>
        /// The <see cref="UPInfoAreaCondition"/>.
        /// </returns>
        public UPInfoAreaCondition WholeDayConditionForTimeZone(DateTime startDate, UPCRMTimeZone timeZone)
        {
            var infoAreaId = this.InfoAreaMetaInfo.InfoAreaId;
            var timeString = timeZone.GetAdjustedCurrentMMTime(startDate);
            if (timeString == "0000")
            {
                return new UPInfoAreaConditionLeaf(
                    infoAreaId,
                    this.DateFieldId,
                    timeZone.GetAdjustedCurrentMMDate(startDate));
            }

            var date1 = new UPInfoAreaConditionLeaf(
                infoAreaId,
                this.DateFieldId,
                timeZone.GetAdjustedCurrentMMDate(startDate));
            var time1 = new UPInfoAreaConditionLeaf(infoAreaId, this.TimeFieldId, ">=", timeString);
            var date2 = new UPInfoAreaConditionLeaf(
                infoAreaId,
                this.DateFieldId,
                timeZone.GetAdjustedCurrentMMDate(startDate.AddSeconds(60 * 60 * 24)));
            var time2 = new UPInfoAreaConditionLeaf(infoAreaId, this.TimeFieldId, "<", timeString);
            var fromCondition = date1.InfoAreaConditionByAppendingAndCondition(time1);
            var toCondition = date2.InfoAreaConditionByAppendingAndCondition(time2);
            return fromCondition.InfoAreaConditionByAppendingOrCondition(toCondition);
        }
    }

    /// <summary>
    /// Info area and date time condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Query.UPInfoAreaDateTimeCondition" />
    public class UPInfoAreaANDDateTimeCondition : UPInfoAreaDateTimeCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaANDDateTimeCondition"/> class.
        /// </summary>
        /// <param name="infoAreaMetaInfo">
        /// The information area meta information.
        /// </param>
        /// <param name="single1">
        /// The single1.
        /// </param>
        /// <param name="single2">
        /// The single2.
        /// </param>
        public UPInfoAreaANDDateTimeCondition(
            UPContainerInfoAreaMetaInfo infoAreaMetaInfo,
            UPInfoAreaSingleDateTimeCondition single1,
            UPInfoAreaSingleDateTimeCondition single2)
            : base(infoAreaMetaInfo)
        {
            this.SingleConditions = new List<UPInfoAreaSingleDateTimeCondition> { single1, single2 };
        }

        /// <summary>
        /// Gets the single conditions.
        /// </summary>
        /// <value>
        /// The single conditions.
        /// </value>
        public List<UPInfoAreaSingleDateTimeCondition> SingleConditions { get; private set; }

        /// <summary>
        /// Appends the and date condition.
        /// </summary>
        /// <param name="andCondition">
        /// The and condition.
        /// </param>
        public void AppendANDDateCondition(UPInfoAreaANDDateTimeCondition andCondition)
        {
            var currentConditions = new List<UPInfoAreaSingleDateTimeCondition>(this.SingleConditions);
            foreach (var addSingle in andCondition.SingleConditions)
            {
                var added = currentConditions.Any(single => single.AppendSingleCondition(addSingle));

                if (!added)
                {
                    this.SingleConditions.Add(addSingle);
                }
            }
        }

        /// <summary>
        /// Appends the condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void AppendCondition(UPInfoAreaDateTimeCondition condition)
        {
            if (condition is UPInfoAreaSingleDateTimeCondition)
            {
                this.AppendSingleCondition((UPInfoAreaSingleDateTimeCondition)condition);
            }
            else if (condition is UPInfoAreaANDDateTimeCondition)
            {
                this.AppendANDDateCondition((UPInfoAreaANDDateTimeCondition)condition);
            }
        }

        /// <summary>
        /// Appends the single condition.
        /// </summary>
        /// <param name="singleCondition">
        /// The single condition.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool AppendSingleCondition(UPInfoAreaSingleDateTimeCondition singleCondition)
        {
            if (this.SingleConditions.Any(single => single.AppendSingleCondition(singleCondition)))
            {
                return true;
            }

            this.SingleConditions.Add(singleCondition);
            return true;
        }

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        public override void ApplyTimeZone(UPCRMTimeZone timeZone)
        {
            foreach (var cond in this.SingleConditions)
            {
                cond.ApplyTimeZone(timeZone);
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = new StringBuilder($"AND [{Environment.NewLine}");
            foreach (var cond in this.SingleConditions)
            {
                str = str.Append($"{cond}{Environment.NewLine}");
            }

            str = str.Append($"]{Environment.NewLine}");
            return str.ToString();
        }
    }

    /// <summary>
    /// Info area OR date time condition
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Query.UPInfoAreaDateTimeCondition" />
    public class UPInfoAreaORDateTimeCondition : UPInfoAreaDateTimeCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaORDateTimeCondition"/> class.
        /// </summary>
        /// <param name="infoAreaMetaInfo">
        /// The information area meta information.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public UPInfoAreaORDateTimeCondition(UPContainerInfoAreaMetaInfo infoAreaMetaInfo, UPInfoAreaDateTimeCondition condition)
            : base(infoAreaMetaInfo)
        {
            this.Conditions = new List<UPInfoAreaDateTimeCondition> { condition };
        }

        /// <summary>
        /// Gets the conditions.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public List<UPInfoAreaDateTimeCondition> Conditions { get; private set; }

        /// <summary>
        /// Appends the condition.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public void AppendCondition(UPInfoAreaDateTimeCondition condition)
        {
            if (condition is UPInfoAreaORDateTimeCondition)
            {
                foreach (var c in ((UPInfoAreaORDateTimeCondition)condition).Conditions)
                {
                    this.Conditions.Add(c);
                }
            }
            else
            {
                this.Conditions.Add(condition);
            }
        }

        /// <summary>
        /// Applies the time zone.
        /// </summary>
        /// <param name="timeZone">
        /// The time zone.
        /// </param>
        public override void ApplyTimeZone(UPCRMTimeZone timeZone)
        {
            foreach (var cond in this.Conditions)
            {
                cond.ApplyTimeZone(timeZone);
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var str = new StringBuilder($"OR [{Environment.NewLine}");
            foreach (var cond in this.Conditions)
            {
                str = str.Append($"{cond}{Environment.NewLine}");
            }

            str = str.Append($"]{Environment.NewLine}");
            return str.ToString();
        }
    }
}
