// <copyright file="MLocationField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.UIModel.Fields
{
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Location field model
    /// </summary>
    public class UPMLocationField : UPMField
    {
        private readonly UPMAction action;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMLocationField"/> class.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        /// <param name="action">Action</param>
        public UPMLocationField(IIdentifier identifier, UPMAction action)
            : base(identifier)
        {
            this.action = action;
        }

        /// <summary>
        /// Gets or sets latitude.
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Gets or sets longtitude.
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Gets or sets address title.
        /// </summary>
        public string AddressTitle { get; set; }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        public UPMAction Action => this.action;
    }
}
