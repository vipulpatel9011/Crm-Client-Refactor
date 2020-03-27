// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSESingleAdditionalItemInformationDelegate.cs" company="Aurea Software Gmbh">
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
//   The UPSESingleAdditionalItemInformation Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;

    /// <summary>
    /// UPSESingleAdditionalItemInformationDelegate
    /// </summary>
    public interface UPSESingleAdditionalItemInformationDelegate
    {
        /// <summary>
        /// Additionals the item information did finish with result.
        /// </summary>
        /// <param name="addItem">The add item.</param>
        /// <param name="result">The result.</param>
        void AdditionalItemInformationDidFinishWithResult(UPSESingleAdditionalItemInformation addItem, object result);

        /// <summary>
        /// Additionals the item information did fail with error.
        /// </summary>
        /// <param name="addItem">The add item.</param>
        /// <param name="error">The error.</param>
        void AdditionalItemInformationDidFailWithError(UPSESingleAdditionalItemInformation addItem, Exception error);
    }
}
