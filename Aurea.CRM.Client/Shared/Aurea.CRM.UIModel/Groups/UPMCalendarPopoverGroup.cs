// <copyright file="UPMCalendarPopoverGroup.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// The UPMCalendar popover group.
    /// </summary>
    public class UPMCalendarPopoverGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCalendarPopoverGroup"/> class. 
        /// </summary>
        /// <param name="identifier">
        /// Identifier object
        /// </param>
        public UPMCalendarPopoverGroup(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public ICalendarItem Context { get; set; }
    }
}
