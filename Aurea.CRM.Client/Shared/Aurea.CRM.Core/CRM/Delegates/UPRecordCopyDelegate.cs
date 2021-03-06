﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPRecordCopyDelegate.cs" company="Aurea Software Gmbh">
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
//   The UPRecordCopyDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;

    /// <summary>
    /// UPRecordCopyDelegate
    /// </summary>
    public interface UPRecordCopyDelegate
    {
        /// <summary>
        /// Records the copy did finish with result.
        /// </summary>
        /// <param name="recordCopy">The record copy.</param>
        /// <param name="records">The records.</param>
        void RecordCopyDidFinishWithResult(UPRecordCopy recordCopy, List<UPCRMRecord> records);

        /// <summary>
        /// Records the copy did fail with error.
        /// </summary>
        /// <param name="recordCopy">The record copy.</param>
        /// <param name="error">The error.</param>
        void RecordCopyDidFailWithError(UPRecordCopy recordCopy, Exception error);
    }
}
