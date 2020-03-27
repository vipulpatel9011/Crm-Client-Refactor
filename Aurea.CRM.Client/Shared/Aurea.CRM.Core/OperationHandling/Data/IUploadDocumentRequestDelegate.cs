// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUploadDocumentRequestDelegate.cs" company="Aurea Software Gmbh">
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
//   The Upload Document Request Delegate interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Upload Document Request Delegate interface
    /// </summary>
    public interface IUploadDocumentRequestDelegate
    {
        /// <summary>
        /// Uploads the document request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        void UploadDocumentRequestDidFailWithError(UploadDocumentServerOperation request, Exception error);

        /// <summary>
        /// Uploads the document request did finish with json.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="json">The json.</param>
        void UploadDocumentRequestDidFinishWithJSON(UploadDocumentServerOperation request, Dictionary<string, object> json);
    }
}
