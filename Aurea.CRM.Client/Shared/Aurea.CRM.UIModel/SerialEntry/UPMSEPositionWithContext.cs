// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSEPositionWithContext.cs" company="Aurea Software Gmbh">
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
//   UPMSEPositionWithContext
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UPMSEPositionWithContext
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPMSEPosition" />
    public class UPMSEPositionWithContext : UPMSEPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSEPositionWithContext"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSEPositionWithContext(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public UPSERow Row { get; set; }
    }
}
