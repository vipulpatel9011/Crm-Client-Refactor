// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMTimelineDetailsGroup.cs" company="Aurea Software Gmbh">
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
//   Timeline Details Group implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;

    /// <summary>
    /// Timeline Details Group implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMTimelineDetailsGroup : UPMGroup
    {
        /// <summary>
        /// Gets or sets the result context.
        /// </summary>
        /// <value>
        /// The result context.
        /// </value>
        public UPCoreMappingResultContext ResultContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMTimelineDetailsGroup(IIdentifier identifier)
            : base(identifier)
        {
        }
    }
}
