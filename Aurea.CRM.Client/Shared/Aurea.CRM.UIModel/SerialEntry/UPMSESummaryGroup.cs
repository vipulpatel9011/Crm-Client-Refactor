// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSESummaryGroup.cs" company="Aurea Software Gmbh">
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
//   UPMSESummaryGroup
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// UPMSESummaryGroup
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.UPMContainer" />
    public class UPMSESummaryGroup : UPMContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSESummaryGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSESummaryGroup(IIdentifier identifier)
            : base(identifier)
        {
        }
    }
}
