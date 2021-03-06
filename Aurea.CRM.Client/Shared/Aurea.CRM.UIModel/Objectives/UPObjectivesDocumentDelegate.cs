﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPObjectivesDocumentDelegate.cs" company="Aurea Software Gmbh">
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
//   UPObjectivesDocumentDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Objectives
{
    using System;

    /// <summary>
    /// UPObjectivesDocumentDelegate
    /// </summary>
    public interface UPObjectivesDocumentDelegate
    {
        /// <summary>
        /// Objectives document from sender did fail with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        void ObjectivesDocumentFromSenderDidFailWithError(UPObjectivesItem sender, Exception error);

        /// <summary>
        /// Objectives document did finish from sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        void ObjectivesDocumentDidFinishFromSender(UPObjectivesItem sender);
    }
}
