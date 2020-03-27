// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalSearchOperation.cs" company="Aurea Software Gmbh">
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
//   Defines search operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// A search operation performed against the local data
    /// Simmilar to "UPLocalSearchOperation" on CRM.Pad
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.LocalOperation" />
    public class UPLocalSearchOperation : LocalOperation
    {
        /// <summary>
        /// The container meta info.
        /// </summary>
        private readonly UPContainerMetaInfo containerMetaInfo;

        /// <summary>
        /// The query.
        /// </summary>
        private readonly Query query;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPLocalSearchOperation"/> class.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="metaInfo">
        /// The meta information.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public UPLocalSearchOperation(Query query, UPContainerMetaInfo metaInfo, ISearchOperationHandler handler)
        {
            this.query = query;
            this.containerMetaInfo = metaInfo;
            this.SearchOperationHandler = handler;
        }

        /// <summary>
        /// Gets or sets the search operation handler.
        /// </summary>
        /// <value>
        /// The search operation handler.
        /// </value>
        public ISearchOperationHandler SearchOperationHandler { get; set; }

        /// <summary>
        /// Handles the operation cancel.
        /// </summary>
        public virtual void HandleOperationCancel()
        {
            this.SearchOperationHandler = null;
        }

        /// <summary>
        /// Performs the operation.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool PerformOperation()
        {
            Task.Run(
                () =>
                {
                    var resultSet = this.query.Execute();
                    var dataDictionary = new Dictionary<string, object> { { "resultSet", resultSet } };

                    this.ProcessResult(dataDictionary);
                });
            return true;
        }

        /// <summary>
        /// Processes the result.
        /// </summary>
        /// <param name="dataDictionary">
        /// The data dictionary.
        /// </param>
        private void ProcessResult(Dictionary<string, object> dataDictionary)
        {
            if (this.Canceled)
            {
                return;
            }

            var resultSet = dataDictionary.ValueOrDefault("resultSet") as DatabaseRecordSet;
            this.containerMetaInfo.BuildResultMetaInfoResult(this.query, resultSet);
            var result = new UPCRMResult(this.containerMetaInfo, resultSet);

            result.Log();

            // Inform the handler about success
            this.SearchOperationHandler.SearchOperationDidFinishWithResult(this, result);
            this.FinishProcessing();
        }
    }
}
