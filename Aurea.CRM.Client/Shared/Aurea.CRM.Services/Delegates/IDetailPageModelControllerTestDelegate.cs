// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDetailPageModelControllerTestDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Test delegate interface for detal page model controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.Delegates
{
    using System;

    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Test delegate interface for detal page model controller
    /// </summary>
    public interface IDetailPageModelControllerTestDelegate
    {
        /// <summary>
        /// Models the controller did fail with error.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void ModelControllerDidFailWithError(object modelController, Exception error);

        /// <summary>
        /// Models the controller did finish with success.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="detailPage">
        /// The detail page.
        /// </param>
        void ModelControllerDidFinishWithSuccess(object modelController, MDetailPage detailPage);
    }
}
