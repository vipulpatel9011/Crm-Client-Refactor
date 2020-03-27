// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCRMTilesDelegate.cs" company="Aurea Software Gmbh">
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
//   The CRM Tiles Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Delegates
{
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// CRM Tiles Delegate
    /// </summary>
    public interface UPCRMTilesDelegate
    {
        /// <summary>
        /// Tileses the did finish with success.
        /// </summary>
        /// <param name="crmTiles">The CRM tiles.</param>
        /// <param name="data">The data.</param>
        void TilesDidFinishWithSuccess(UPCRMTiles crmTiles, object data);
    }
}
