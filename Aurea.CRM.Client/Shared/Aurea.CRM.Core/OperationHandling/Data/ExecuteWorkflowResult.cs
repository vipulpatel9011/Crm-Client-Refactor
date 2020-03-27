// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExecuteWorkflowResult.cs" company="Aurea Software Gmbh">
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
//   The UPExecuteWorkflowResult
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System.Collections.Generic;

    /// <summary>
    /// UPExecuteWorkflowResult
    /// </summary>
    public class UPExecuteWorkflowResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPExecuteWorkflowResult"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="messages">The messages.</param>
        public UPExecuteWorkflowResult(List<string> records, List<string> messages)
        {
            this.Messages = messages;
            this.ChangedRecords = records;
            this.Parameters = null;
        }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public List<string> Messages { get; private set; }

        /// <summary>
        /// Gets the changed records.
        /// </summary>
        /// <value>
        /// The changed records.
        /// </value>
        public List<string> ChangedRecords { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public object Parameters { get; private set; }
    }
}
