// <copyright file="IconResolverBase.cs" company="Aurea Software Gmbh">
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
namespace Aurea.CRM.Core.Utilities.GlyphIcons
{
    using System;
    using System.Collections.Generic;
    using Extensions;

    /// <summary>
    /// Icon resolver base implementation
    /// </summary>
    public abstract class IconResolverBase
    {
        private readonly Dictionary<string, string> glyphLookup = new Dictionary<string, string>();

        /// <summary>
        /// Gets font name for given <see cref="UPGlyphIcons"/>
        /// </summary>
        /// <param name="glyphicon">Glyphicon enum</param>
        /// <returns>Font name</returns>
        public static string FontNameForGlyphicons(UPGlyphIcons glyphicon)
        {
            string fontSetName;
            var totalStr = ((int)glyphicon).ToString("X8");
            var lastChar = totalStr[totalStr.Length - 1].ToInt();
            switch (lastChar)
            {
                case (int)UPGlyphIconsType.Icons:
                    fontSetName = "GLYPHICONS-Regular";
                    break;
                case (int)UPGlyphIconsType.Halflings:
                    fontSetName = "GLYPHICONSHalflingsSet-Regular";
                    break;
                case (int)UPGlyphIconsType.Social:
                    fontSetName = "GLYPHICONSSocial-Regular";
                    break;
                case (int)UPGlyphIconsType.Filetypes:
                    fontSetName = "GLYPHICONSFiletypes-Regular";
                    break;
                default:
                    fontSetName = "GLYPHICONS-Regular";
                    break;
            }

            return fontSetName;
        }

        /// <summary>
        /// Gets the <see cref="UPGlyphIcons" />, given a name
        /// </summary>
        /// <param name="iconName">name of the icon</param>
        /// <returns>UPGlyphIcons</returns>
        public static UPGlyphIcons GetIcon(string iconName)
        {
            foreach (var val in Enum.GetNames(typeof(UPGlyphIconsType)))
            {
                string enumValue = val;
                if (val == UPGlyphIconsType.Icons.ToString())
                {
                    enumValue = string.Empty;
                }
                if (Enum.TryParse(iconName?.Replace("Icon:", enumValue).Replace(" ", string.Empty), out UPGlyphIcons code))
                {
                    return code;
                }
            }
            return UPGlyphIcons.UnkownIcon;
        }

        /// <summary>
        /// Gets char for given icon name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Character</returns>
        public string GetCode(string name)
        {
            if (name.StartsWith("Icon:"))
            {
                name = name.Replace(" ", string.Empty);
            }

            string glyphString = this.glyphLookup.ValueOrDefault(name);
            if (glyphString != null)
            {
                return glyphString;
            }

            int location = name.IndexOf("\\");
            if (name?.Length > 1 && location != -1)
            {
                string hexString = name.Substring(location + 1);
                int code;
                bool success = int.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out code);
                if (success)
                {
                    string tmpString = char.ConvertFromUtf32(code);
                    this.glyphLookup.SetObjectForKey(tmpString, name);
                    return tmpString;
                }
                else
                {
                    return null;
                }
            }
            else if (name.StartsWith("#"))
            {
                string tmpString = name.Substring(1);
                this.glyphLookup.SetObjectForKey(tmpString, name);
                return tmpString;
            }
            else
            {
                var code = UPGlyphIcons.UnkownIcon;
                if (Enum.TryParse(name.Replace("Icon:", "Halflings"), out code))
                {
                    return this.GetCode(code);
                }
                else if (Enum.TryParse(name.Replace("Icon:", string.Empty), out code))
                {
                    return this.GetCode(code);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets char for given icon
        /// </summary>
        /// <param name="icon">Icon</param>
        /// <returns>Character</returns>
        public string GetCode(UPGlyphIcons icon)
        {
            string tmpString = char.ConvertFromUtf32((int)icon >> 8).ToString();
            this.glyphLookup.SetObjectForKey(tmpString, icon.ToString());
            return tmpString;
        }
    }
}
