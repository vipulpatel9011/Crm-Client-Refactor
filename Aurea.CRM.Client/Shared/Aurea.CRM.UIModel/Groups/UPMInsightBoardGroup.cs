// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMInsightBoardGroup.cs" company="Aurea Software Gmbh">
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
//   The UPMInsightBoardGroup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Enum UPMInsightBoardGroupLayout
    /// </summary>
    public enum UPMInsightBoardGroupLayout
    {
        /// <summary>
        /// Layout0
        /// </summary>
        Layout0,

        /// <summary>
        /// Layout1
        /// </summary>
        Layout1,

        /// <summary>
        /// Layout2
        /// </summary>
        Layout2,

        /// <summary>
        /// Layout3
        /// </summary>
        Layout3,

        /// <summary>
        /// Layout4
        /// </summary>
        Layout4
    }

    /// <summary>
    /// UPMInsightBoardGroup
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMInsightBoardGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMInsightBoardGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMInsightBoardGroup(IIdentifier identifier)
             : base(identifier)
        {
            this.Layout = UPMInsightBoardGroupLayout.Layout2;
            this.Height = 0;
            this.MaxVisibleItems = 4;
            this.SingleRow = true;
            this.Center = false;
        }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>
        /// The layout.
        /// </value>
        public UPMInsightBoardGroupLayout Layout { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the maximum visible items.
        /// </summary>
        /// <value>
        /// The maximum visible items.
        /// </value>
        public int MaxVisibleItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [single row].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [single row]; otherwise, <c>false</c>.
        /// </value>
        public bool SingleRow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPMInsightBoardGroup"/> is center.
        /// </summary>
        /// <value>
        ///   <c>true</c> if center; otherwise, <c>false</c>.
        /// </value>
        public bool Center { get; set; }
    }
}
