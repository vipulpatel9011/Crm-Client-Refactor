// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCountOperationWithResult.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// A local count operation with results
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.LocalOperation" />
    public class UPLocalCountOperationWithResult : LocalOperation
    {
        /// <summary>
        /// The count.
        /// </summary>
        private readonly int count;

        /// <summary>
        /// The container meta info.
        /// </summary>
        private UPContainerMetaInfo containerMetaInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPLocalCountOperationWithResult"/> class.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="metaInfo">
        /// The meta information.
        /// </param>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public UPLocalCountOperationWithResult(int count, UPContainerMetaInfo metaInfo, ISearchOperationHandler handler)
        {
            this.count = count;
            this.containerMetaInfo = metaInfo;
            this.SearchOperationHandler = handler;
        }

        /// <summary>
        /// Gets or sets the search operation handler.
        /// </summary>
        /// <value>
        /// The search operation handler.
        /// </value>
        private ISearchOperationHandler SearchOperationHandler { get; set; }

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
            this.SearchOperationHandler?.SearchOperationDidFinishWithCount(this, this.count);
            this.FinishProcessing();
            return true;
        }
    }
}
