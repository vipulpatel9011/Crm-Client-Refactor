// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerSessionSyncHandlerDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Server session sync handler delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Common;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Server session sync handler delegate interface
    /// </summary>
    public interface IServerSessionSyncHandlerDelegate
    {
        /// <summary>
        /// Servers the session synchronize handler did finish.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="changedRecordIdentifications">
        /// The changed record identifications.
        /// </param>
        /// <param name="documentDownloadUrLs">
        /// The document download ur ls.
        /// </param>
        void ServerSessionSyncHandlerDidFinish(
            ServerSessionSyncHandler syncHandler,
            List<string> changedRecordIdentifications,
            List<SyncDocument> syncDocuments);

        /// <summary>
        /// Servers the session synchronize handler did finish with error.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void ServerSessionSyncHandlerDidFinishWithError(ServerSessionSyncHandler syncHandler, Exception error);

        /// <summary>
        /// Servers the session synchronize handler did proceed to step number of steps.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="currentStepNumber">
        /// The current step number.
        /// </param>
        /// <param name="totalStepNumber">
        /// The total step number.
        /// </param>
        void ServerSessionSyncHandlerDidProceedToStepNumberOfSteps(
            ServerSessionSyncHandler syncHandler,
            int currentStepNumber,
            int totalStepNumber);

        /// <summary>
        /// Servers the session synchronize handler status hint.
        /// </summary>
        /// <param name="syncHandler">
        /// The synchronize handler.
        /// </param>
        /// <param name="statusHint">
        /// The status hint.
        /// </param>
        void ServerSessionSyncHandlerStatusHint(ServerSessionSyncHandler syncHandler, string statusHint);
    }
}
