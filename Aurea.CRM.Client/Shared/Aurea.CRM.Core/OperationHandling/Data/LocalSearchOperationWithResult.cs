// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalSearchOperationWithResult.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// A local search operation with results returned
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.LocalOperation" />
    public class UPLocalSearchOperationWithResult : LocalOperation
    {
        /// <summary>
        /// The crm result.
        /// </summary>
        private readonly UPCRMResult crmResult;

        /// <summary>
        /// The crm query.
        /// </summary>
        private UPContainerMetaInfo crmQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPLocalSearchOperationWithResult"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public UPLocalSearchOperationWithResult(
            UPCRMResult result,
            UPContainerMetaInfo query,
            ISearchOperationHandler handler)
        {
            this.crmResult = result;
            this.crmQuery = query;
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
        public void HandleOperationCancel()
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
            this.SearchOperationHandler?.SearchOperationDidFinishWithResult(this, this.crmResult);
            this.FinishProcessing();
            return true;
        }
    }
}
