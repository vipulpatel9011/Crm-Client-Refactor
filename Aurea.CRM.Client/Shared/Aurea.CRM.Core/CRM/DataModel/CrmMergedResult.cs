// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmMergedResult.cs" company="Aurea Software Gmbh">
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
//   Merged results implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    using Extensions;
    using Query;

    /// <summary>
    /// Merged results implementation
    /// </summary>
    /// <seealso cref="UPCRMResult" />
    public class UPCRMMergedResult : UPCRMResult
    {
        /// <summary>
        /// The results.
        /// </summary>
        private readonly List<UPCRMResult> results;

        /// <summary>
        /// The result rows.
        /// </summary>
        private List<ICrmDataSourceRow> resultRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMMergedResult"/> class.
        /// </summary>
        /// <param name="metaInfo">
        /// The meta information.
        /// </param>
        public UPCRMMergedResult(UPContainerMetaInfo metaInfo)
            : base(string.Empty)
        {
            this.MetaInfo = metaInfo;
            this.IsServer = false;
            this.MultipleInfoAreas = metaInfo.HasMultipleOutputInfoAreas;
            this.NewRecordIdentification = null;
            this.results = new List<UPCRMResult>();
        }

        /// <summary>
        /// Gets the Row count.
        /// </summary>
        public override int RowCount => this.resultRows?.Count ?? 0;

        /// <summary>
        /// Adds the result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public void AddResult(UPCRMResult result)
        {
            this.results.Add(result);
        }

        /// <summary>
        /// Merges the server result with local results.
        /// </summary>
        /// <param name="serverResult">
        /// The server result.
        /// </param>
        /// <param name="localResults">
        /// The local results.
        /// </param>
        public void MergeServerResultWithLocalResults(UPCRMResult serverResult, Dictionary<string, object> localResults)
        {
            var resultsOfCurrentQuery = new List<ICrmDataSourceRow>();
            for (var i = 0; i < serverResult.RowCount; i++)
            {
                var serverResultRow = (UPCRMResultRow)serverResult.ResultRowAtIndex(i);
                var localResultRow = localResults.ValueOrDefault(serverResultRow.RootRecordId) as UPCRMResultRow;
                if (localResultRow != null)
                {
                    serverResultRow.SetHasLocalCopy(true);
                }

                resultsOfCurrentQuery.Add(serverResultRow);
            }

            this.AddResult(serverResult);
            this.SetResultRows(resultsOfCurrentQuery);
        }

        /// <summary>
        /// Results the index of the row at.
        /// </summary>
        /// <param name="rowNumber">
        /// The row number.
        /// </param>
        /// <returns>
        /// The <see cref="ICrmDataSourceRow"/>.
        /// </returns>
        public override ICrmDataSourceRow ResultRowAtIndex(int rowNumber) => this.resultRows?[rowNumber];

        /// <summary>
        /// Sets the result rows.
        /// </summary>
        /// <param name="_resultRows">
        /// The _result rows.
        /// </param>
        public void SetResultRows(List<ICrmDataSourceRow> _resultRows)
        {
            this.resultRows = _resultRows;
        }
    }
}
