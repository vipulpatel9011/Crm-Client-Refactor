// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCRMLinkReaderDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The PCRMLinkReaderDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;

    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// The PCRMLinkReaderDelegate interface.
    /// </summary>
    public interface UPCRMLinkReaderDelegate
    {
        /// <summary>
        /// The link reader did finish with error.
        /// </summary>
        /// <param name="linkReader">
        /// The link reader.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception ex);

        /// <summary>
        /// The link reader did finish with result.
        /// </summary>
        /// <param name="linkReader">
        /// The link reader.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result);
    }
}
