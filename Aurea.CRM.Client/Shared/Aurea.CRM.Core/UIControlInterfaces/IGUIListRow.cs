﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGUIListRow.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The GUIListRow interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.UIControlInterfaces
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// The GUIListRow interface.
    /// </summary>
    public interface IGUIListRow
    {
        /// <summary>
        /// Sets the row color.
        /// </summary>
        AureaColor RowColor { set; }
    }
}
