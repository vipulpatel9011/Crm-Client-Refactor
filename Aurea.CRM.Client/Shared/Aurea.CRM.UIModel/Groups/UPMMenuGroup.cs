// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMMenuGroup.cs" company="Aurea Software Gmbh">
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
//   The Menu Group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Menu Group class
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMMenuGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMMenuGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMMenuGroup(IIdentifier identifier)
            : base(identifier)
        {
        }
    }
}
