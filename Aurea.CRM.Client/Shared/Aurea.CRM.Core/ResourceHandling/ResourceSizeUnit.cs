// <copyright file="ResourceSizeUnit.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.ResourceHandling
{
    /// <summary>
    /// The up resource size unit.
    /// </summary>
    public enum ResourceSizeUnit
    {
        /// <summary>
        /// The bytes.
        /// </summary>
        Bytes = 0,

        /// <summary>
        /// The kilo bytes.
        /// </summary>
        KiloBytes,

        /// <summary>
        /// The mega bytes.
        /// </summary>
        MegaBytes,

        /// <summary>
        /// The giga bytes.
        /// </summary>
        GigaBytes
    }
}