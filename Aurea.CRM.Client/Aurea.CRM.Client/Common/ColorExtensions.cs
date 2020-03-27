// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorExtensions.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Ioan Armenean (Nelutu)
// </author>
// <summary>
//   Extension methods for Color object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Client.UI.Common
{
    using System;
    using System.Globalization;
    using Aurea.CRM.Core.Logging;
    using Core.Configuration;
    using GalaSoft.MvvmLight.Ioc;
    using Xamarin.Forms;

    /// <summary>
    /// Extensions for Color class
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts an AureaColor object to a xamarin forms color object.
        /// </summary>
        /// <param name="aureaColor">An AureaColor object.</param>
        /// <returns>A xamarin forms Color object</returns>
        /// <exception cref="Exception">Color string was invalid.</exception>
        public static Color ToXamarinFormsColor(this AureaColor aureaColor)
        {
            if (aureaColor == null)
            {
                return Color.White;
            }

            var rgba = aureaColor.RgbString.Split(';');
            if (rgba.Length != 4)
            {
                throw new Exception("Color string was invalid.");
            }

            byte a = 255, r = 0, g = 0, b = 0;
            double dA, dR, dG, dB;

            double.TryParse(rgba[3], out dA);
            a = MapDoubleToByte(dA, 0, 1, 0, 255);

            if (rgba[0].Contains(".") || rgba[1].Contains(".") || rgba[2].Contains("."))
            {
                double.TryParse(rgba[3], out dA);
                double.TryParse(rgba[0], out dR);
                double.TryParse(rgba[1], out dG);
                double.TryParse(rgba[2], out dB);

                r = MapDoubleToByte(dR, 0, 1, 0, 255);
                g = MapDoubleToByte(dG, 0, 1, 0, 255);
                b = MapDoubleToByte(dB, 0, 1, 0, 255);
            }
            else
            {
                r = Convert.ToByte(rgba[0]);
                g = Convert.ToByte(rgba[1]);
                b = Convert.ToByte(rgba[2]);
            }

            return Color.FromRgba(r, g, b, a);
        }

        /// <summary>
        /// Convert aurea color to xamarin color for detail view
        /// </summary>
        /// <param name="aureaColor">color</param>
        /// <returns>xamarin color</returns>
        public static Color? ToDetailViewColor(this AureaColor aureaColor)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(aureaColor.HexString))
                {
                    return Color.FromHex(aureaColor.HexString);
                }
                else if (!string.IsNullOrWhiteSpace(aureaColor.RgbString))
                {
                    const char colorsSeparator = ';';

                    var rgbParts = aureaColor.RgbString.Split(colorsSeparator);

                    return Color.FromRgba(
                        (int)(255 * double.Parse(rgbParts[0], CultureInfo.InvariantCulture)),
                        (int)(255 * double.Parse(rgbParts[1], CultureInfo.InvariantCulture)),
                        (int)(255 * double.Parse(rgbParts[2], CultureInfo.InvariantCulture)),
                        (int)(255 * double.Parse(rgbParts[3], CultureInfo.InvariantCulture)));
                }
            }
            catch (Exception error)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(error);
            }

            return null;
        }

        private static byte MapDoubleToByte(this double value, double from1, double to1, byte from2, byte to2)
        {
            return (byte)(((value - from1) / (to1 - from1) * (to2 - from2)) + from2);
        }
    }
}
