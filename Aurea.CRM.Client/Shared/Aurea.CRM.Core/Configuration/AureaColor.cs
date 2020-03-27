// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AureaColor.cs" company="Aurea Software Gmbh">
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
//   Defines the color configurations and standard. Simmilar to UPColor in CRM.Pad
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Defines the color configurations and standard. Simmilar to UPColor in CRM.Pad
    /// </summary>
    public class AureaColor
    {
        /// <summary>
        /// The colors
        /// </summary>
        private static Dictionary<string, AureaColor> colors;

        /// <summary>
        /// The current system aurea color field
        /// </summary>
        private static AureaColor currentSystemAureaColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AureaColor"/> class.
        /// </summary>
        /// <param name="rgbString">
        /// The RGB string.
        /// </param>
        public AureaColor(string rgbString)
        {
            if (rgbString == null)
            {
                this.RgbString = null;
            }
            else if (rgbString.IndexOf("#", StringComparison.Ordinal) == -1
                     && rgbString.IndexOf(";", StringComparison.Ordinal) > -1)
            {
                this.RgbString = rgbString;
            }
            else
            {
                this.HexString = rgbString.StartsWith("#") ? rgbString : $"#{rgbString}";
                this.RgbString = RgabStringFromHexString(rgbString.StartsWith("#") ? rgbString : $"#{rgbString}");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AureaColor"/> class.
        /// </summary>
        /// <param name="red">
        /// The red.
        /// </param>
        /// <param name="green">
        /// The green.
        /// </param>
        /// <param name="blue">
        /// The blue.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        public AureaColor(double red, double green, double blue, double alpha)
            : this($"{red};{green};{blue};{alpha.ToString("#.####", CultureInfo.InvariantCulture)}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AureaColor"/> class.
        /// </summary>
        /// <param name="red">
        /// The red.
        /// </param>
        /// <param name="green">
        /// The green.
        /// </param>
        /// <param name="blue">
        /// The blue.
        /// </param>
        public AureaColor(double red, double green, double blue)
            : this(red, green, blue, 1.0)
        {
        }

        /// <summary>
        /// Gets the color dictionary.
        /// </summary>
        /// <value>
        /// The color dictionary.
        /// </value>
        public static Dictionary<string, AureaColor> ColorDictionary
        {
            get
            {
                if (colors != null)
                {
                    return colors;
                }

                colors = new Dictionary<string, AureaColor>
                             {
                                 { "red", RedGreenBlue(255, 0.0, 0.0) },
                                 { "blue", RedGreenBlue(0.0, 0.0, 255) },
                                 { "green", RedGreenBlue(0.0, 255, 0.0) },
                                 { "gray", RedGreenBlue(178, 178, 178) },
                                 { "yellow", RedGreenBlue(255, 255, 0.0) }
                             };

                return colors;
            }
        }

        /// <summary>
        /// Gets the RGB string.
        /// irgendwie wird die SystemColor aus UpdateUI im Model (COI-Provider) benÃ¶tigt.
        /// </summary>
        /// <value>
        /// The RGB string.
        /// </value>
        public string RgbString { get; private set; }

        /// <summary>
        /// Gets the Hex string.
        /// </summary>
        /// <value>
        /// The Hex string.
        /// </value>
        public string HexString { get; private set; }

        /// <summary>
        /// Colors the with string.
        /// </summary>
        /// <param name="colorString">
        /// The color string.
        /// </param>
        /// <returns>
        /// The <see cref="AureaColor"/>.
        /// </returns>
        public static AureaColor ColorWithString(string colorString)
        {
            var color = ColorDictionary.ValueOrDefault(colorString);
            if (color == null)
            {
                color = new AureaColor(colorString);
                colors[colorString] = color;
            }

            return color;
        }

        /// <summary>
        /// stolen from u8 web
        /// Delivers a new color that is contrasting to this one. This can be used e.g. for text color on varying background.
        /// For more information see http://en.wikipedia.org/wiki/YIQ and http://24ways.org/2010/calculating-color-contrast/
        /// </summary>
        /// <param name="hexColorString">
        /// The hex Color String.
        /// </param>
        /// <returns>
        /// White if the color is on the darker side of the spectrum, otherwise Black
        /// </returns>
        public static string GetContrastingColorForColor(string hexColorString)
        {
            var contrastingColor = "#FFFFFF";
            var rgbTuple = GetRgbTupleFromHexString(hexColorString);
            var yiq = (rgbTuple.R * 299 + rgbTuple.G * 587 + rgbTuple.B * 114) / 1000;
            if (yiq >= 128)
            {
                contrastingColor = "#000000";
            }

            return contrastingColor;
        }

        /// <summary>
        /// Reds the green blue.
        /// </summary>
        /// <param name="red">
        /// The red.
        /// </param>
        /// <param name="green">
        /// The green.
        /// </param>
        /// <param name="blue">
        /// The blue.
        /// </param>
        /// <returns>
        /// The <see cref="AureaColor"/>.
        /// </returns>
        public static AureaColor RedGreenBlue(double red, double green, double blue)
        {
            return new AureaColor(red, green, blue);
        }

        /// <summary>
        /// Reds the green blue alpha.
        /// </summary>
        /// <param name="red">
        /// The red.
        /// </param>
        /// <param name="green">
        /// The green.
        /// </param>
        /// <param name="blue">
        /// The blue.
        /// </param>
        /// <param name="alpha">
        /// The alpha.
        /// </param>
        /// <returns>
        /// The <see cref="AureaColor"/>.
        /// </returns>
        public static AureaColor RedGreenBlueAlpha(double red, double green, double blue, double alpha)
        {
            return new AureaColor(red, green, blue, alpha);
        }

        /// <summary>
        /// Rgabs the string from hexadecimal string.
        /// </summary>
        /// <param name="hexString">
        /// The hexadecimal string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RgabStringFromHexString(string hexString)
        {
            var colorStruct = GetRgbTupleFromHexString(hexString);

            return $"{colorStruct.R};{colorStruct.G};{colorStruct.B};{colorStruct.A.ToString("#.####", CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// Sets the color of the current system.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        public static void SetCurrentSystemColor(AureaColor color)
        {
            currentSystemAureaColor = color;
        }

        /// <summary>
        /// Ups the color of the current system.
        /// </summary>
        /// <returns>
        /// The <see cref="AureaColor"/>.
        /// </returns>
        public static AureaColor UpCurrentSystemColor()
        {
            return currentSystemAureaColor ?? (currentSystemAureaColor = new AureaColor(1.0, 1.0, 1.0));
        }

        /// <summary>
        /// Gets the RGB tuple from hexadecimal string.
        /// </summary>
        /// <param name="hexString">
        /// The hexadecimal string.
        /// </param>
        /// <returns>
        /// The <see cref="RGBAStruct"/>.
        /// </returns>
        private static RGBAStruct GetRgbTupleFromHexString(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            {
                return new RGBAStruct(0, 0, 0);
            }

            var hexCode = hexString.Replace("#", string.Empty);
            if (hexCode.Length < 6)
            {
                return new RGBAStruct(0, 0, 0);
            }

            Func<string, float> getColorComponent = s =>
                {
                    var intValue = 0;
                    int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out intValue);

                    if (intValue > 255)
                    {
                        intValue = intValue % 255;
                    }

                    return intValue;
                };

            return new RGBAStruct(
                getColorComponent(hexCode.Substring(0, 2)),
                getColorComponent(hexCode.Substring(2, 2)),
                getColorComponent(hexCode.Substring(4, 2)),
                hexCode.Length == 8 ? getColorComponent(hexCode.Substring(6, 2)) / 255.0F : 1.0F);
        }

        /// <summary>
        /// Internal struct to hold color details
        /// </summary>
        private struct RGBAStruct
        {
            /// <summary>
            /// the red color value
            /// </summary>
            public readonly float R;

            /// <summary>
            /// the green color value
            /// </summary>
            public readonly float G;

            /// <summary>
            /// the blue color value
            /// </summary>
            public readonly float B;

            /// <summary>
            /// the alpha value
            /// </summary>
            public readonly float A;

            /// <summary>
            /// Initializes a new instance of the <see cref="RGBAStruct"/> struct.
            /// </summary>
            /// <param name="r">
            /// The r.
            /// </param>
            /// <param name="g">
            /// The g.
            /// </param>
            /// <param name="b">
            /// The b.
            /// </param>
            /// <param name="a">
            /// a.
            /// </param>
            public RGBAStruct(float r, float g, float b, float a = 1.0F)
            {
                this.R = r;
                this.G = g;
                this.B = b;
                this.A = a > 1 ? 1 : a;
            }
        }
    }
}
