// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoIEdgeViewConfig.cs" company="Aurea Software Gmbh">
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
//   The UPMCoIEdgeViewConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// Complete implementation and refine constructor while doing CRM-5621

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// UPMCoIEdgeViewConfig
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.CircleOfInfluence.UPMCoIConfigBase" />
    public class UPMCoIEdgeViewConfig : UPMCoIConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoIEdgeViewConfig"/> class.
        /// </summary>
        /// <param name="edgeTextFont">The edge text font.</param>
        /// <param name="edgeTextMinFontSize">Size of the edge text minimum font.</param>
        /// <param name="edgeTextColor">Color of the edge text.</param>
        /// <param name="edgeColor">Color of the edge.</param>
        /// <param name="edgeLineWidth">Width of the edge line.</param>
        /// <param name="edgeLineWidthForFactor">The edge line width for factor.</param>
        /// <param name="edgeLineJoin">The edge line join.</param>
        /// <param name="edgeDashPattern">The edge dash pattern.</param>
        /// <param name="edgeTextOpacity">The edge text opacity.</param>
        /// <param name="edgeOpacity">The edge opacity.</param>
        /// <param name="shadowOpacity">The shadow opacity.</param>
        /// <param name="shadowRadius">The shadow radius.</param>
        /// <param name="shadowOffset">The shadow offset.</param>
        /// <param name="shadowColor">Color of the shadow.</param>
        /// <param name="textShadowOpacity">The text shadow opacity.</param>
        /// <param name="textShadowRadius">The text shadow radius.</param>
        /// <param name="textShadowOffset">The text shadow offset.</param>
        /// <param name="textShadowColor">Color of the text shadow.</param>
        /// <param name="selectedEdgeTextFont">The selected edge text font.</param>
        /// <param name="selectedEdgeTextMinFontSize">Size of the selected edge text minimum font.</param>
        /// <param name="selectedEdgeTextColor">Color of the selected edge text.</param>
        /// <param name="selectedEdgeColor">Color of the selected edge.</param>
        /// <param name="selectedEdgeLineWidth">Width of the selected edge line.</param>
        /// <param name="selectedEdgeLineWidthForFactor">The selected edge line width for factor.</param>
        /// <param name="selectedEdgeLineJoin">The selected edge line join.</param>
        /// <param name="selectedEdgeDashPattern">The selected edge dash pattern.</param>
        /// <param name="selectedEdgeTextOpacity">The selected edge text opacity.</param>
        /// <param name="selectedEdgeOpacity">The selected edge opacity.</param>
        /// <param name="selectedShadowOpacity">The selected shadow opacity.</param>
        /// <param name="selectedShadowRadius">The selected shadow radius.</param>
        /// <param name="selectedShadowOffset">The selected shadow offset.</param>
        /// <param name="selectedShadowColor">Color of the selected shadow.</param>
        /// <param name="selectedTextShadowOpacity">The selected text shadow opacity.</param>
        /// <param name="selectedTextShadowRadius">The selected text shadow radius.</param>
        /// <param name="selectedTextShadowOffset">The selected text shadow offset.</param>
        /// <param name="selectedTextShadowColor">Color of the selected text shadow.</param>
        public UPMCoIEdgeViewConfig(UIFont edgeTextFont, float edgeTextMinFontSize, AureaColor edgeTextColor, AureaColor edgeColor, float edgeLineWidth,
            float edgeLineWidthForFactor, string edgeLineJoin, List<int> edgeDashPattern, float edgeTextOpacity, float edgeOpacity, float shadowOpacity,
            float shadowRadius, Size shadowOffset, AureaColor shadowColor, float textShadowOpacity, float textShadowRadius, Size textShadowOffset,
            AureaColor textShadowColor, UIFont selectedEdgeTextFont, float? selectedEdgeTextMinFontSize, AureaColor selectedEdgeTextColor,
            AureaColor selectedEdgeColor, float? selectedEdgeLineWidth, float? selectedEdgeLineWidthForFactor, string selectedEdgeLineJoin,
            List<int> selectedEdgeDashPattern, float? selectedEdgeTextOpacity, float? selectedEdgeOpacity, float? selectedShadowOpacity,
            float? selectedShadowRadius, Size? selectedShadowOffset, AureaColor selectedShadowColor, float? selectedTextShadowOpacity,
            float? selectedTextShadowRadius, Size? selectedTextShadowOffset, AureaColor selectedTextShadowColor)
        {
            this.EdgeTextFont = edgeTextFont;
            this.EdgeTextMinFontSize = edgeTextMinFontSize;
            this.EdgeTextColor = edgeTextColor;
            this.EdgeColor = edgeColor;
            this.EdgeLineWidth = edgeLineWidth;
            this.EdgeLineWidthForFactor = edgeLineWidthForFactor;
            this.EdgeLineJoin = edgeLineJoin;
            this.EdgeDashPattern = edgeDashPattern;
            this.EdgeTextOpacity = edgeTextOpacity;
            this.EdgeOpacity = edgeOpacity;
            this.ShadowOpacity = shadowOpacity;
            this.ShadowRadius = shadowRadius;
            this.ShadowOffset = shadowOffset;
            this.ShadowColor = shadowColor;
            this.TextShadowOpacity = textShadowOpacity;
            this.TextShadowRadius = textShadowRadius;
            this.TextShadowOffset = textShadowOffset;
            this.TextShadowColor = textShadowColor;
            this.SelectedEdgeTextFont = selectedEdgeTextFont ?? edgeTextFont;
            this.SelectedEdgeTextMinFontSize = selectedEdgeTextMinFontSize ?? edgeTextMinFontSize;
            this.SelectedEdgeTextColor = selectedEdgeTextColor ?? edgeTextColor;
            this.SelectedEdgeColor = selectedEdgeColor ?? edgeColor;
            this.SelectedEdgeLineWidth = selectedEdgeLineWidth ?? edgeLineWidth;
            this.SelectedEdgeLineWidthForFactor = selectedEdgeLineWidthForFactor ?? edgeLineWidthForFactor;
            this.SelectedEdgeLineJoin = selectedEdgeLineJoin ?? edgeLineJoin;
            this.SelectedEdgeDashPattern = selectedEdgeDashPattern ?? edgeDashPattern;
            this.SelectedEdgeTextOpacity = selectedEdgeTextOpacity ?? edgeTextOpacity;
            this.SelectedEdgeOpacity = selectedEdgeOpacity ?? edgeOpacity;
            this.SelectedShadowOpacity = selectedShadowOpacity ?? shadowOpacity;
            this.SelectedShadowRadius = selectedShadowRadius ?? shadowRadius;
            this.SelectedShadowOffset = selectedShadowOffset ?? shadowOffset;
            this.SelectedShadowColor = selectedShadowColor ?? shadowColor;
            this.SelectedTextShadowOpacity = selectedTextShadowOpacity ?? textShadowOpacity;
            this.SelectedTextShadowRadius = selectedTextShadowRadius ?? textShadowRadius;
            this.SelectedTextShadowOffset = selectedTextShadowOffset ?? textShadowOffset;
            this.SelectedTextShadowColor = selectedTextShadowColor ?? textShadowColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoIEdgeViewConfig"/> class.
        /// </summary>
        public UPMCoIEdgeViewConfig()
            : this(UIFont.SystemFontOfSize(12), 5.0f, AureaColor.RedGreenBlue(2.0 / 3.0, 2.0 / 3.0, 2.0 / 3.0),
                  AureaColor.RedGreenBlue(238.0f / 255.0f, 238.0f / 255.0f, 238.0f / 255.0f), 5.0f, 0.2f, /*kCALineJoinMiter*/ null, null, 0.0f, 1.0f, 0.4f, 0.0f,
                  new Size(0, 1), AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), 1.0f, 1, new Size(0, 1),
                  AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, 12.0f, AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f),
                  AureaColor.RedGreenBlue(255.0 / 255.0f, 90.0 / 255.0f, 16.0 / 255.0f), null, null, null, null, 1.0f, 1.0f,
                  null, null, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Gets or sets the edge text font.
        /// </summary>
        /// <value>
        /// The edge text font.
        /// </value>
        public UIFont EdgeTextFont { get; set; }

        /// <summary>
        /// Gets or sets the size of the edge text minimum font.
        /// </summary>
        /// <value>
        /// The size of the edge text minimum font.
        /// </value>
        public float EdgeTextMinFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the edge text.
        /// </summary>
        /// <value>
        /// The color of the edge text.
        /// </value>
        public AureaColor EdgeTextColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the edge.
        /// </summary>
        /// <value>
        /// The color of the edge.
        /// </value>
        public AureaColor EdgeColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the edge line.
        /// </summary>
        /// <value>
        /// The width of the edge line.
        /// </value>
        public float EdgeLineWidth { get; set; }

        /// <summary>
        /// Gets or sets the edge line width for factor.
        /// </summary>
        /// <value>
        /// The edge line width for factor.
        /// </value>
        public float EdgeLineWidthForFactor { get; set; }

        /// <summary>
        /// Gets or sets the edge line join.
        /// </summary>
        /// <value>
        /// The edge line join.
        /// </value>
        public string EdgeLineJoin { get; set; }

        /// <summary>
        /// Gets or sets the edge dash pattern.
        /// </summary>
        /// <value>
        /// The edge dash pattern.
        /// </value>
        public List<int> EdgeDashPattern { get; set; }

        /// <summary>
        /// Gets or sets the edge text opacity.
        /// </summary>
        /// <value>
        /// The edge text opacity.
        /// </value>
        public float EdgeTextOpacity { get; set; }

        /// <summary>
        /// Gets or sets the edge opacity.
        /// </summary>
        /// <value>
        /// The edge opacity.
        /// </value>
        public float EdgeOpacity { get; set; }

        /// <summary>
        /// Gets or sets the shadow opacity.
        /// </summary>
        /// <value>
        /// The shadow opacity.
        /// </value>
        public float ShadowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the shadow radius.
        /// </summary>
        /// <value>
        /// The shadow radius.
        /// </value>
        public float ShadowRadius { get; set; }

        /// <summary>
        /// Gets or sets the shadow offset.
        /// </summary>
        /// <value>
        /// The shadow offset.
        /// </value>
        public Size ShadowOffset { get; set; }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>
        /// The color of the shadow.
        /// </value>
        public AureaColor ShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the text shadow.
        /// </summary>
        /// <value>
        /// The color of the text shadow.
        /// </value>
        public AureaColor TextShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the text shadow offset.
        /// </summary>
        /// <value>
        /// The text shadow offset.
        /// </value>
        public Size TextShadowOffset { get; set; }

        /// <summary>
        /// Gets or sets the text shadow opacity.
        /// </summary>
        /// <value>
        /// The text shadow opacity.
        /// </value>
        public float TextShadowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the text shadow radius.
        /// </summary>
        /// <value>
        /// The text shadow radius.
        /// </value>
        public float TextShadowRadius { get; set; }

        /// <summary>
        /// Gets or sets the selected edge text font.
        /// </summary>
        /// <value>
        /// The selected edge text font.
        /// </value>
        public UIFont SelectedEdgeTextFont { get; set; }

        /// <summary>
        /// Gets or sets the size of the selected edge text minimum font.
        /// </summary>
        /// <value>
        /// The size of the selected edge text minimum font.
        /// </value>
        public float SelectedEdgeTextMinFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected edge text.
        /// </summary>
        /// <value>
        /// The color of the selected edge text.
        /// </value>
        public AureaColor SelectedEdgeTextColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected edge.
        /// </summary>
        /// <value>
        /// The color of the selected edge.
        /// </value>
        public AureaColor SelectedEdgeColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the selected edge line.
        /// </summary>
        /// <value>
        /// The width of the selected edge line.
        /// </value>
        public float SelectedEdgeLineWidth { get; set; }

        /// <summary>
        /// Gets or sets the selected edge line width for factor.
        /// </summary>
        /// <value>
        /// The selected edge line width for factor.
        /// </value>
        public float SelectedEdgeLineWidthForFactor { get; set; }

        /// <summary>
        /// Gets or sets the selected edge line join.
        /// </summary>
        /// <value>
        /// The selected edge line join.
        /// </value>
        public string SelectedEdgeLineJoin { get; set; }

        /// <summary>
        /// Gets or sets the selected edge dash pattern.
        /// </summary>
        /// <value>
        /// The selected edge dash pattern.
        /// </value>
        public List<int> SelectedEdgeDashPattern { get; set; }

        /// <summary>
        /// Gets or sets the selected edge opacity.
        /// </summary>
        /// <value>
        /// The selected edge opacity.
        /// </value>
        public float SelectedEdgeOpacity { get; set; }

        /// <summary>
        /// Gets or sets the selected edge text opacity.
        /// </summary>
        /// <value>
        /// The selected edge text opacity.
        /// </value>
        public float SelectedEdgeTextOpacity { get; set; }

        /// <summary>
        /// Gets or sets the selected shadow opacity.
        /// </summary>
        /// <value>
        /// The selected shadow opacity.
        /// </value>
        public float SelectedShadowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the selected shadow radius.
        /// </summary>
        /// <value>
        /// The selected shadow radius.
        /// </value>
        public float SelectedShadowRadius { get; set; }

        /// <summary>
        /// Gets or sets the selected shadow offset.
        /// </summary>
        /// <value>
        /// The selected shadow offset.
        /// </value>
        public Size SelectedShadowOffset { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected shadow.
        /// </summary>
        /// <value>
        /// The color of the selected shadow.
        /// </value>
        public AureaColor SelectedShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected text shadow.
        /// </summary>
        /// <value>
        /// The color of the selected text shadow.
        /// </value>
        public AureaColor SelectedTextShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the selected text shadow offset.
        /// </summary>
        /// <value>
        /// The selected text shadow offset.
        /// </value>
        public Size SelectedTextShadowOffset { get; set; }

        /// <summary>
        /// Gets or sets the selected text shadow opacity.
        /// </summary>
        /// <value>
        /// The selected text shadow opacity.
        /// </value>
        public float SelectedTextShadowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the selected text shadow radius.
        /// </summary>
        /// <value>
        /// The selected text shadow radius.
        /// </value>
        public float SelectedTextShadowRadius { get; set; }
    }
}
