// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPGeoLocation.cs" company="Aurea Software Gmbh">
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
//   Geo Location
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Logging;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Geo Location
    /// </summary>
    public class UPGeoLocation
    {
        /// <summary>
        /// Gets the GPS x string.
        /// </summary>
        /// <value>
        /// The GPS x string.
        /// </value>
        public string GpsXString { get; }

        /// <summary>
        /// Gets the GPS y string.
        /// </summary>
        /// <value>
        /// The GPS y string.
        /// </value>
        public string GpsYString { get; }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; }

        /// <summary>
        /// Gets the address title.
        /// </summary>
        /// <value>
        /// The address title.
        /// </value>
        public string AddressTitle { get; }

        /// <summary>
        /// Gets a value indicating whether [valid GPS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [valid GPS]; otherwise, <c>false</c>.
        /// </value>
        public bool ValidGPS { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPGeoLocation"/> class.
        /// </summary>
        /// <param name="gpsXString">The GPS x string.</param>
        /// <param name="gpsYString">The GPS y string.</param>
        /// <param name="addressTitle">The address title.</param>
        /// <param name="address">The address.</param>
        public UPGeoLocation(string gpsXString, string gpsYString, string addressTitle, string address)
        {
            this.GpsXString = gpsXString;
            this.GpsYString = gpsYString;
            this.ValidGPS = ValidGpsXstringGpsYstring(this.GpsXString, this.GpsYString);
            this.Address = address;
            this.AddressTitle = addressTitle;
        }

        /// <summary>
        /// Creates the specified row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="tabConfig">The tab configuration.</param>
        /// <returns></returns>
        public static UPGeoLocation Create(UPCRMResultRow row, FieldControlTab tabConfig)
        {
            try
            {
                string gpsXString = null, gpsYString = null, addressTitle = null;
                var currentFieldIndex = 0;
                var found = false;
                var numberOfFields = tabConfig.NumberOfFields;
                var address = new StringBuilder();

                while (currentFieldIndex + 1 < numberOfFields)
                {
                    gpsXString = row.RawValueAtIndex(tabConfig.FieldAtIndex(currentFieldIndex++).TabIndependentFieldIndex);
                    gpsYString = row.RawValueAtIndex(tabConfig.FieldAtIndex(currentFieldIndex++).TabIndependentFieldIndex);

                    if (string.IsNullOrEmpty(gpsXString) || Convert.ToDecimal(gpsXString, System.Globalization.CultureInfo.InvariantCulture) == 0m || string.IsNullOrEmpty(gpsYString) || Convert.ToDecimal(gpsYString) == 0m)
                    {
                        gpsXString = gpsYString = string.Empty;
                    }
                    else
                    {
                        found = true;
                    }

                    if (currentFieldIndex < numberOfFields)
                    {
                        UPConfigFieldControlField addressNameField = tabConfig.FieldAtIndex(currentFieldIndex++);
                        int fieldCount = addressNameField.Attributes.FieldCount;
                        if (fieldCount <= 1)
                        {
                            addressTitle = row.ValueAtIndex(addressNameField.TabIndependentFieldIndex);
                        }
                        else
                        {
                            addressTitle = FormattedValueForResultRowFieldControlTabFieldIndex(row, tabConfig, currentFieldIndex + 1);
                            currentFieldIndex += fieldCount - 1;
                        }
                    }

                    if (currentFieldIndex < numberOfFields)
                    {
                        UPConfigFieldControlField addressField = tabConfig.FieldAtIndex(currentFieldIndex++);
                        int fieldCount = addressField.Attributes.FieldCount;
                        if (fieldCount <= 1)
                        {
                            address.Append(row.ValueAtIndex(addressField.TabIndependentFieldIndex));
                        }
                        else
                        {
                            address.Append(FormattedValueForResultRowFieldControlTabFieldIndex(row, tabConfig, currentFieldIndex - 1));
                            currentFieldIndex += fieldCount - 1;
                        }

                        if (address.Length > 1)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    while (currentFieldIndex + 1 < numberOfFields)
                    {
                        if (currentFieldIndex < numberOfFields)
                        {
                            UPConfigFieldControlField addressField = tabConfig.FieldAtIndex(currentFieldIndex++);
                            int fieldCount = addressField.Attributes.FieldCount;
                            if (fieldCount <= 1)
                            {
                                address.Append($" , {row.ValueAtIndex(addressField.TabIndependentFieldIndex)}");
                            }
                            else
                            {
                                address.Append($" , {FormattedValueForResultRowFieldControlTabFieldIndex(row, tabConfig, currentFieldIndex - 1)}");
                                currentFieldIndex += fieldCount - 1;
                            }
                        }
                    }

                    return new UPGeoLocation(gpsXString, gpsYString, addressTitle, address.ToString());
                }
            }
            catch (Exception error)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(error);
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ValidGPS
                ? $"GEO: {this.AddressTitle} -> {this.GpsXString}/{this.GpsYString} ({this.Address})"
                : $"GEO: {this.AddressTitle} -> {this.Address}";
        }

        private static string FormattedValueForResultRowFieldControlTabFieldIndex(UPCRMResultRow row, FieldControlTab tabConfig, int fieldIndex)
        {
            bool found = false;
            FieldAttributes attributes = tabConfig.FieldAtIndex(fieldIndex).Attributes;
            List<string> fieldValueArray = new List<string>(attributes.FieldCount);
            int lastIndex = fieldIndex + attributes.FieldCount;
            for (int i = fieldIndex; i < lastIndex; i++)
            {
                UPConfigFieldControlField field = tabConfig.FieldAtIndex(i);
                if (field != null)
                {
                    string v = row.ValueAtIndex(field.TabIndependentFieldIndex);
                    if (v != null)
                    {
                        fieldValueArray.Add(v);
                        if (v.Length > 1)
                        {
                            found = true;
                        }
                    }
                    else
                    {
                        fieldValueArray.Add(string.Empty);
                    }
                }
            }

            return found ? attributes.FormatValues(fieldValueArray) : string.Empty;
        }

        private static bool ValidGpsXstringGpsYstring(string gpsX, string gpsY)
        {
            if (string.IsNullOrEmpty(gpsX) || string.IsNullOrEmpty(gpsY))
            {
                return false;
            }

            double gpsXFloat = Convert.ToDouble(gpsX, System.Globalization.CultureInfo.InvariantCulture);
            if (gpsXFloat < -180 || gpsXFloat > 180)
            {
                return false;
            }

            double gpsYFloat = Convert.ToDouble(gpsY, System.Globalization.CultureInfo.InvariantCulture);
            if (gpsYFloat < -90 || gpsYFloat > 90)
            {
                return false;
            }

            return gpsXFloat != 0 || gpsYFloat != 0;
        }
    }
}
