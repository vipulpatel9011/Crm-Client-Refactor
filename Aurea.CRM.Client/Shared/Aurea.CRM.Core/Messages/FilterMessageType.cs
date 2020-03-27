// <copyright file="FilterMessageType.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.Messages
{
    /// <summary>
    /// Filter Message type
    /// </summary>
    public enum FilterMessageType
    {
        /// <summary>
        /// Filter has been activated
        /// </summary>
        Activated,

        /// <summary>
        /// Filter has been expanded
        /// </summary>
        Expanded,

        /// <summary>
        /// Filter has been collapsed
        /// </summary>
        Collapsed,

        /// <summary>
        /// Filter has been deactivated
        /// </summary>
        Deactivated,
    }
}
