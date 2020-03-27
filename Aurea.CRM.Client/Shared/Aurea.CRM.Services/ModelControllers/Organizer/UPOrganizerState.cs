// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOrganizerState.cs" company="Aurea Software Gmbh">
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
//   Organizer State
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    /// <summary>
    /// Organizer State
    /// </summary>
    public class UPOrganizerState
    {
        /// <summary>
        /// Gets or sets the nav controller identifier.
        /// </summary>
        /// <value>
        /// The nav controller identifier.
        /// </value>
        public int NavControllerId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        /// <value>
        /// The subtitle.
        /// </value>
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPOrganizerState"/> is editing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if editing; otherwise, <c>false</c>.
        /// </value>
        public bool Editing { get; set; }

        /// <summary>
        /// Gets or sets the starting nav controller identifier.
        /// </summary>
        /// <value>
        /// The starting nav controller identifier.
        /// </value>
        public int StartingNavControllerId { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"UPOrganizerState id{this.NavControllerId}, editing: {this.Editing} title: {this.Title} subtitle: {this.Subtitle}";
        }
    }
}
