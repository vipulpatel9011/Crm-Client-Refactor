// <copyright file="GeoUPMResultRow.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.UIModel
{
    using Core.CRM.UIModel;
    using Core.Extensions;
    using Core.Structs;
    using Fields;

    /// <summary>
    /// Implementation of GeoUPMResultRow class.
    /// </summary>
    public class GeoUPMResultRow : UPMResultRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoUPMResultRow"/> class.
        /// </summary>
        /// <param name="resultRow">Result row</param>
        /// <param name="distance">Distance</param>
        public GeoUPMResultRow(UPMResultRow resultRow, double distance)
            : this(resultRow?.Identifier)
        {
            this.ResultRow = resultRow;
            this.Distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoUPMResultRow"/> class.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        public GeoUPMResultRow(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets the result row
        /// </summary>
        public UPMResultRow ResultRow { get; set; }

        /// <summary>
        /// Gets or sets the distance
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Gets the geolocation of record
        /// </summary>
        /// <param name="resultRow">Result row</param>
        /// <returns>Location for given result row</returns>
        public static Location Location(UPMResultRow resultRow)
        {
            UPMGpsXField gpsXField = null;
            UPMGpsYField gpsYField = null;
            foreach (UPMField field in resultRow.Fields)
            {
                if (field is UPMGpsXField)
                {
                    gpsXField = (UPMGpsXField)field;
                }
                else if (field is UPMGpsYField)
                {
                    gpsYField = (UPMGpsYField)field;
                }
            }

            return new Location(gpsXField?.StringValue?.ToDouble() ?? 0, gpsYField?.StringValue?.ToDouble() ?? 0);
        }
    }
}
