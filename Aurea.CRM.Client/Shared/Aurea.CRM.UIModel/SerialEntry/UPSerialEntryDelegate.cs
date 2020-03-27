// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryDelegate.cs" company="Aurea Software Gmbh">
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
//   UPSerialEntryDelegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPSerialEntryDelegate
    /// </summary>
    public interface UPSerialEntryDelegate
    {
        /// <summary>
        /// Serials the entry build did finish with success.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="result">The result.</param>
        void SerialEntryBuildDidFinishWithSuccess(UPSerialEntry serialEntry, object result);

        /// <summary>
        /// Serials the entry positions for filter did finish with success.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="position">The position.</param>
        void SerialEntryPositionsForFilterDidFinishWithSuccess(UPSerialEntry serialEntry, List<UPSERow> position);

        /// <summary>
        /// Serials the entry did fail with error.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="error">The error.</param>
        void SerialEntryDidFailWithError(UPSerialEntry serialEntry, Exception error);

        /// <summary>
        /// Serials the entry row changed with success context.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        void SerialEntryRowChangedWithSuccessContext(UPSerialEntry serialEntry, UPSERow row, object context);

        /// <summary>
        /// Serials the entry row photo uploaded context.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        void SerialEntryRowPhotoUploadedContext(UPSerialEntry serialEntry, UPSERow row, object context);

        /// <summary>
        /// Serials the entry no changes in row context.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        void SerialEntryNoChangesInRowContext(UPSerialEntry serialEntry, UPSERow row, object context);

        /// <summary>
        /// Serials the entry row deleted with success.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="dependentRows">The dependent rows.</param>
        /// <param name="context">The context.</param>
        void SerialEntryRowDeletedWithSuccess(UPSerialEntry serialEntry, UPSERow row, List<UPSERow> dependentRows, object context);

        /// <summary>
        /// Serials the entry row error context.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="row">The row.</param>
        /// <param name="error">The error.</param>
        /// <param name="context">The context.</param>
        void SerialEntryRowErrorContext(UPSerialEntry serialEntry, UPSERow row, Exception error, object context);

        /// <summary>
        /// Serials the entry field values for query.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="fieldValues">The field values.</param>
        void SerialEntryFieldValuesForQuery(UPSerialEntry serialEntry, List<object> fieldValues);

        /// <summary>
        /// Serials the entry signal activate initial filters.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="filters">The filters.</param>
        void SerialEntrySignalActivateInitialFilters(UPSerialEntry serialEntry, List<UPConfigFilter> filters);
    }
}
