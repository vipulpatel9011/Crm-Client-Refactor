// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPExecuteWorkflowRequestDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes
// </author>
// <summary>
//  UPExecuteWorkflowRequestDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;

    /// <summary>
    /// UPExecuteWorkflowRequestDelegate
    /// </summary>
    public interface UPExecuteWorkflowRequestDelegate
    {
        /// <summary>
        /// Executes the workflow request did finish with result.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="result">The result.</param>
        void ExecuteWorkflowRequestDidFinishWithResult(UPExecuteWorkflowServerOperation sender, UPExecuteWorkflowResult result);

        /// <summary>
        /// Executes the workflow request did fail with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        void ExecuteWorkflowRequestDidFailWithError(UPExecuteWorkflowServerOperation sender, Exception error);
    }
}
