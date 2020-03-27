// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSearchPageModelControllerTestDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The PSearchPageModelControllerTestDelegate interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.Delegates
{
    using System;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The PSearchPageModelControllerTestDelegate interface.
    /// </summary>
    public interface UPSearchPageModelControllerTestDelegate
    {
        /// <summary>
        /// The model controller returned error.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        void ModelControllerReturnedError(object modelController, Exception error);

        /// <summary>
        /// The model controller returned page.
        /// </summary>
        /// <param name="modelController">
        /// The model controller.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        void ModelControllerReturnedPage(object modelController, IPage page);
    }
}
