// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCopyFieldsDelegate.cs" company="Aurea Software Gmbh">
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
//   Copy Fields Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Delegates
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.Features;

    /// <summary>
    /// Copy Fields Delegate
    /// </summary>
    public interface UPCopyFieldsDelegate
    {
        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary);

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error);
    }
}
