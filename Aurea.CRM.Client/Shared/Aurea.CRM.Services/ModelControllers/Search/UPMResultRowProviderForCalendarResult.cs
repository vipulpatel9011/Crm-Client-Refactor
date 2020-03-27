// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMResultRowProviderForCalendarResult.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm result row provider for calendar result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;

    /// <summary>
    /// The upm result row provider for calendar result.
    /// </summary>
    public class UPMResultRowProviderForCalendarResult : UPResultRowProvider
    {
        /// <summary>
        /// The result context.
        /// </summary>
        protected UPCoreMappingResultContext resultContext;

        /// <summary>
        /// The result rows.
        /// </summary>
        public List<ICalendarItem> resultRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMResultRowProviderForCalendarResult"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public  UPMResultRowProviderForCalendarResult(
            List<ICalendarItem> result,
            IResultRowProviderDelegate theDelegate,
            UPCoreMappingResultContext context)
            : base(theDelegate)
        {
            this.resultRows = result;
            this.resultContext = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMResultRowProviderForCalendarResult"/> class.
        /// </summary>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public UPMResultRowProviderForCalendarResult(
            IResultRowProviderDelegate theDelegate,
            UPCoreMappingResultContext context)
            : this(null, theDelegate, context)
        {
            this.resultRows = new List<ICalendarItem>();
            this.resultContext = context;
        }

        /// <summary>
        /// The add value.
        /// </summary>
        /// <param name="calendarResult">
        /// The calendar result.
        /// </param>
        public void AddValue(ICalendarItem calendarResult)
        {
            this.resultRows.Add(calendarResult);
        }

        /// <summary>
        /// The number of result rows.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int NumberOfResultRows()
        {
            return this.resultRows.Count;
        }

        /// <summary>
        /// The row at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="UPMResultRow"/>.
        /// </returns>
        public override UPMResultRow RowAtIndex(int index)
        {
            ICalendarItem item = this.resultRows[index];
            UPCRMResultRow crmResultRow = item.CrmResultRow;
            if (crmResultRow != null)
            {
                ResultRowCalendarItem resultRow = (ResultRowCalendarItem)item;
                if (this.resultContext != null)
                {
                    var dict = this.resultContext.FieldControl.FunctionNames(crmResultRow);
                    string startDateString = (string)dict["Date"];
                    string startTimeString = (string)dict["Time"];
                    resultRow.StartDate = StringExtensions.DateFromStrings(startDateString, startTimeString);

                        // .DateFromCrmValueWithCrmTime(startTimeString);
                }

                resultRow.CrmValue = true;
                resultRow.Invalid = true;
                resultRow.DataValid = true;
                resultRow.RowColor = item.Color;
                this.TheDelegate?.ResultRowProviderDidCreateRowFromDataRow(this, resultRow, crmResultRow);

                return resultRow;
            }
            else
            {
                // EKEvent localEvent = null;
                // if (item.IsKindOfClass(typeof(EKEvent)))
                // {
                // localEvent = item;
                // }

                // ResultRowCalendarItem resultRow = new ResultRowCalendarItem(StringIdentifier.IdentifierWithStringId(item.Identification), localEvent);
                // this.TheDelegate?.ResultRowProviderDidCreateRowFromDataRow(this, resultRow, crmResultRow);

                // return resultRow;
            }

            return null;
        }
    }
}
