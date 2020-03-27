// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMMapGroup.cs" company="Aurea Software Gmbh">
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
//   Map Group implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.UIModel.Fields;

    /// <summary>
    /// Class for containing map group and user interface data.
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMMapGroup : UPMGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMMapGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMMapGroup(IIdentifier identifier)
            : base(identifier)
        {
            this.RecommendedHeight = 200;
            this.UserInteractionOnlyEnabledInFullscreenMode = false;
        }

        /// <summary>
        /// Gets or sets the GUI map group.
        /// </summary>
        /// <value>
        /// The GUI map group.
        /// </value>
        public IGUIMapGroup GuiMapGroup { get; set; }

        /// <summary>
        /// Gets or sets the height of the recommended.
        /// </summary>
        /// <value>
        /// The height of the recommended.
        /// </value>
        public double RecommendedHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [user interaction only enabled in fullscreen mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [user interaction only enabled in fullscreen mode]; otherwise, <c>false</c>.
        /// </value>
        public bool UserInteractionOnlyEnabledInFullscreenMode { get; set; }

        /// <summary>
        /// Gets the number of locations.
        /// </summary>
        /// <value>
        /// The number of locations.
        /// </value>
        public int NumberOfLocations => this.Fields.Count;

        /// <summary>
        /// Locations the index of the field at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The Location field</returns>
        public UPMLocationField LocationFieldAtIndex(int index)
        {
            return this.Fields[index] as UPMLocationField;
        }

        /// <summary>
        /// Adds a location to location list.
        /// </summary>
        /// <param name="locationField"></param>
        public void AddLocation(UPMLocationField locationField)
        {
            this.AddChild(locationField);
        }

        /// <summary>
        /// Gets locations list.
        /// </summary>
        public List<UPMLocationField> Locations => this.Children.Cast<UPMLocationField>().ToList();
    }
}
