// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmCachedResult.cs" company="Aurea Software Gmbh">
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
//   CRM result implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    /// <summary>
    /// CRM Cached record implementation
    /// </summary>
    /// <seealso cref="UPCRMResult" />
    public class UPCRMCachedResult : UPCRMResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMCachedResult"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public UPCRMCachedResult(UPCRMResult result, UPCRMResultCondition condition)
            : base(result.MetaInfo, result.IsServerResult ? result.ServerResponse() : result.RecordSet())
        {
            this.BaseResult = result;
            var resultRowArray = new List<UPCRMResultRow>();
            int i, count = result.RowCount;
            for (i = 0; i < count; i++)
            {
                var row = (UPCRMResultRow)this.ResultRowAtIndex(i);
                if (condition.Check(row))
                {
                    resultRowArray.Add(row);
                }
            }

            this.ResultRows = resultRowArray;
        }

        /// <summary>
        /// Gets the base result.
        /// </summary>
        /// <value>
        /// The base result.
        /// </value>
        public UPCRMResult BaseResult { get; private set; }

        /// <summary>
        /// Gets the result rows.
        /// </summary>
        /// <value>
        /// The result rows.
        /// </value>
        public List<UPCRMResultRow> ResultRows { get; private set; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns></returns>
        public override int RowCount => this.ResultRows.Count;

        /// <summary>
        /// Results the index of the row at.
        /// </summary>
        /// <param name="rowNumber">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMResultRow"/>.
        /// </returns>
        public override ICrmDataSourceRow ResultRowAtIndex(int rowNumber)
        {
            return this.ResultRows[rowNumber];
        }
    }
}
