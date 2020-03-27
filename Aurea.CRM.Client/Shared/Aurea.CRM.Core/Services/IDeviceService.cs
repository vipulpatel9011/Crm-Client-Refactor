// <copyright file="IDeviceService.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Ahmed Yasin Koculu
// </author>
// <summary>
//   This service provides shared device operations which are not device specific.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Services
{
    using System;

    /// <summary>
    /// This service provides shared device operations which are not device specific.
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Executes device default navigation event for given uri.
        /// </summary>
        /// <param name="uri">Uri address.</param>
        void OpenUri(Uri uri);
    }
}
