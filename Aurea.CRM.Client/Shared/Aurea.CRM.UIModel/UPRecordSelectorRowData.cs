// <copyright file="UPRecordSelectorRowData.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;

    /// <summary>
    /// The row data of a record selector
    /// </summary>
    public class UPRecordSelectorRowData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordSelectorRowData"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="functionValues">The function values.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="rowResult">The row result.</param>
        public UPRecordSelectorRowData(UPMResultRow resultRow, Dictionary<string, object> functionValues, string recordIdentification, object rowResult)
        {
            this.ResultRow = resultRow;
            this.FunctionValues = functionValues;
            this.RootRecordIdentification = recordIdentification;
            this.RowResult = rowResult;
        }

        /// <summary>
        /// Gets the result row.
        /// </summary>
        /// <value>
        /// The result row.
        /// </value>
        public UPMResultRow ResultRow { get; private set; }

        /// <summary>
        /// Gets the function values.
        /// </summary>
        /// <value>
        /// The function values.
        /// </value>
        public Dictionary<string, object> FunctionValues { get; private set; }

        /// <summary>
        /// Gets the root record identification.
        /// </summary>
        /// <value>
        /// The root record identification.
        /// </value>
        public string RootRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the row result.
        /// </summary>
        /// <value>
        /// The row result.
        /// </value>
        public object RowResult { get; private set; }

        /// <summary>
        /// Determines whether the specified _object is equal.
        /// </summary>
        /// <param name="_object">The _object.</param>
        /// <returns></returns>
        public bool IsEqual(UPRecordSelectorRowData _object)
        {
            return this.ResultRow.Identifier.Equals(_object.ResultRow.Identifier);
        }
    }
}
