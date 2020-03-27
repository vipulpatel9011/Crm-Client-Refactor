// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalGlobalSearchOperation.cs" company="Aurea Software Gmbh">
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
    using System.Linq;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// Search operation that will perform globally
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.LocalOperation" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Data.ISearchOperation" />
    public class LocalGlobalSearchOperation : LocalOperation, ISearchOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalGlobalSearchOperation"/> class.
        /// </summary>
        /// <param name="handler">
        /// The handler.
        /// </param>
        public LocalGlobalSearchOperation(ISearchOperationHandler handler)
        {
            this.SearchOperationHandler = handler;
            this.ContainerMetaInfos = new List<UPContainerMetaInfo>();
        }

        /// <summary>
        /// Gets or sets the container meta infos.
        /// </summary>
        /// <value>
        /// The container meta infos.
        /// </value>
        protected List<UPContainerMetaInfo> ContainerMetaInfos { get; set; }

        /// <summary>
        /// Gets or sets the search operation handler.
        /// </summary>
        /// <value>
        /// The search operation handler.
        /// </value>
        protected ISearchOperationHandler SearchOperationHandler { get; set; }

        /// <summary>
        /// Adds the container meta information.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        public void AddContainerMetaInfo(UPContainerMetaInfo containerMetaInfo)
        {
            if (containerMetaInfo != null)
            {
                this.ContainerMetaInfos.Add(containerMetaInfo);
            }
        }

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
            var results = this.ContainerMetaInfos.Select(container => container.Find()).ToList();

            this.SearchOperationHandler?.SearchOperationDidFinishWithResults(this, results);
            this.FinishProcessing();
            return true;
        }
    }
}
