// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearchOperation.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// Defines the search operations common functionality
    /// </summary>
    public interface ISearchOperation
    {
        /// <summary>
        /// Adds the container meta information.
        /// </summary>
        /// <param name="containerMetaInfo">
        /// The container meta information.
        /// </param>
        void AddContainerMetaInfo(UPContainerMetaInfo containerMetaInfo);

        /// <summary>
        /// Handles the operation cancel.
        /// </summary>
        void HandleOperationCancel();
    }
}
