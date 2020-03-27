// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MGpsEditField.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Ui control for editing a GPS value field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Fields.Edit
{
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;

    /// <summary>
    /// Ui control for editing a GPS value field
    /// </summary>
    /// <seealso cref="UPMEditField" />
    public class UPMGpsEditField : UPMEditField
    {
        /// <summary>
        /// The latitude.
        /// </summary>
        private double latitude;

        /// <summary>
        /// The longtitude.
        /// </summary>
        private double longtitude;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGpsEditField"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMGpsEditField(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the gui gps edit field.
        /// </summary>
        public IGUIGpsEditField GUIGpsEditField { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        public double Latitude
        {
            get
            {
                return this.latitude;
            }

            set
            {
                if (this.latitude - value <= float.Epsilon)
                {
                    return;
                }

                this.latitude = value;

                if (this.GUIGpsEditField != null)
                {
                    this.GUIGpsEditField.Latitude = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the longtitude.
        /// </summary>
        /// <value>
        /// The longtitude.
        /// </value>
        public double Longtitude
        {
            get
            {
                return this.longtitude;
            }

            set
            {
                if (this.longtitude == value)
                {
                    return;
                }

                this.longtitude = value;

                if (this.GUIGpsEditField != null)
                {
                    this.GUIGpsEditField.Longtitude = value;
                }
            }
        }
    }
}
