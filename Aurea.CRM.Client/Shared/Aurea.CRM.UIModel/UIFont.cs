// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIFont.cs" company="Aurea Software Gmbh">
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
//   Font Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    /// <summary>
    /// Font Implementation
    /// </summary>
    public class UIFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIFont"/> class.
        /// </summary>
        public UIFont()
        {
            this.FontName = string.Empty;
            this.Size = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIFont"/> class.
        /// </summary>
        /// <param name="fontName">name of UIFont</param>
        /// <param name="size">size of UIFont</param>
        private UIFont(string fontName, int size)
        {
            this.FontName = fontName;
            this.Size = size;
        }

        /// <summary>
        /// Gets the name of UIFont
        /// </summary>
        public string FontName { get; private set; }

        /// <summary>
        /// Gets the size of UIFont
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Fonts the size of the with name.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="size">The size.</param>
        /// <returns>Returns new UIFont instance</returns>
        public static UIFont FontWithNameSize(string fontName, int size)
        {
            return new UIFont(fontName, size);
        }

        /// <summary>
        /// Systems the size of the font of.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Returns new UIFont instance</returns>
        public static UIFont SystemFontOfSize(int size)
        {
            return new UIFont(string.Empty, size);
        }
    }
}
