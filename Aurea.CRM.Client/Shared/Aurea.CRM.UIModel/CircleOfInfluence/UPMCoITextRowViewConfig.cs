// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoITextRowViewConfig.cs" company="Aurea Software Gmbh">
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
//   The UPMCoITextRowViewConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// UPMCoITextRowViewConfig
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.CircleOfInfluence.UPMCoIConfigBase" />
    public class UPMCoITextRowViewConfig : UPMCoIConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoITextRowViewConfig"/> class.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="minFontSize">Minimum size of the font.</param>
        public UPMCoITextRowViewConfig(UIFont font, AureaColor color, float minFontSize)
        {
            this.Color = color;
            this.Font = font;
            this.MinFontSize = minFontSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoITextRowViewConfig"/> class.
        /// </summary>
        public UPMCoITextRowViewConfig()
            : this(null, null, 8.0f)
        {
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public AureaColor Color { get; set; }

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public UIFont Font { get; set; }

        /// <summary>
        /// Gets or sets the minimum size of the font.
        /// </summary>
        /// <value>
        /// The minimum size of the font.
        /// </value>
        public float MinFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>
        /// The color of the shadow.
        /// </value>
        public AureaColor ShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the shadow offset.
        /// </summary>
        /// <value>
        /// The shadow offset.
        /// </value>
        public Size ShadowOffset { get; set; }

        /// <summary>
        /// Gets or sets the line numbers.
        /// </summary>
        /// <value>
        /// The line numbers.
        /// </value>
        public int LineNumbers { get; set; }

        /// <summary>
        /// Selecteds the row view configuration.
        /// </summary>
        /// <returns></returns>
        public static UPMCoITextRowViewConfig SelectedRowViewConfig()
        {
            return new UPMCoITextRowViewConfig
            {
                Color = AureaColor.UpCurrentSystemColor(),
                Font = UIFont.FontWithNameSize("HelveticaNeue", 10),
                ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0),
                ShadowOffset = new Size(0, 1)
            };
        }

        /// <summary>
        /// Rows the view configuration.
        /// </summary>
        /// <returns></returns>
        public static UPMCoITextRowViewConfig RowViewConfig()
        {
            return new UPMCoITextRowViewConfig
            {
                Color = AureaColor.RedGreenBlue(102.0 / 255.0f, 102.0 / 255.0f, 102.0 / 255.0f),
                Font = UIFont.FontWithNameSize("HelveticaNeue", 10),
                ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0),
                ShadowOffset = new Size(0, 1)
            };
        }
    }
}
