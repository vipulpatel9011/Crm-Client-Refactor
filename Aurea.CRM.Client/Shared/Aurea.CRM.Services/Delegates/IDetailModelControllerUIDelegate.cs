// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDetailModelControllerUIDelegate.cs" company="Aurea Software Gmbh">
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
//   The detail model controller UI delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System.Collections.Generic;
    using ModelControllers.Inbox;

    /// <summary>
    /// The detail model controller UI delegate
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.Delegates.IModelControllerUIDelegate" />
    public interface IDetailModelControllerUIDelegate : IModelControllerUIDelegate
    {
        /// <summary>
        /// Uploads the photo with record identification.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="parameters">The parameters.</param>
        void UploadPhotoWithRecordIdentification(string config, string recordIdentification, Dictionary<string, object> parameters);

        /// <summary>
        /// Uploads the file with link record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="pageModelController">The page model controller.</param>
        void UploadFileWithLinkRecordIdentification(string recordIdentification, UPInBoxPageModelController pageModelController);
    }
}
