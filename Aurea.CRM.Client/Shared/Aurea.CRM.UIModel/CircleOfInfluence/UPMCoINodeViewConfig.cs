// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoINodeViewConfig.cs" company="Aurea Software Gmbh">
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
//   The UPMCoINodeViewConfig
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// Complete implementation and refine constructor while doing CRM-5621

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// Node View Action
    /// </summary>
    public enum UPNodeViewAction
    {
        /// <summary>
        /// Do Nothing.
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Select node
        /// </summary>
        SelectNode,

        /// <summary>
        /// Relayout node
        /// </summary>
        RelayoutNode,

        /// <summary>
        /// Select node and expand collapse
        /// </summary>
        SelectNodeAndExpandCollapse,

        /// <summary>
        /// Custom via delegate
        /// </summary>
        CustomViaDelegate
    }

    /// <summary>
    /// UPMCoINodeViewConfig
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.CircleOfInfluence.UPMCoIConfigBase" />
    public class UPMCoINodeViewConfig : UPMCoIConfigBase
    {
        private List<UPMCoITextRowViewConfig> textRowConfig;
        private List<UPMCoITextRowViewConfig> selectedTextRowConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoINodeViewConfig"/> class.
        /// </summary>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="size">The size.</param>
        /// <param name="borderColor">Color of the border.</param>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="cornerRadius">The corner radius.</param>
        /// <param name="cornerRadiusInPercent">if set to <c>true</c> [corner radius in percent].</param>
        /// <param name="imageVisible">if set to <c>true</c> [image visible].</param>
        /// <param name="textVisible">if set to <c>true</c> [text visible].</param>
        /// <param name="childOffset">The child offset.</param>
        /// <param name="rootOffset">The root offset.</param>
        /// <param name="textAngle">The text angle.</param>
        /// <param name="maxTextSize">Maximum size of the text.</param>
        /// <param name="maxImageSize">Maximum size of the image.</param>
        /// <param name="imageOffset">The image offset.</param>
        /// <param name="textOffset">The text offset.</param>
        /// <param name="shadowRadius">The shadow radius.</param>
        /// <param name="shadowOffset">The shadow offset.</param>
        /// <param name="shadowOpacity">The shadow opacity.</param>
        /// <param name="shadowColor">Color of the shadow.</param>
        /// <param name="flatShadow">if set to <c>true</c> [flat shadow].</param>
        /// <param name="buttonsFrameSize">Size of the buttons frame.</param>
        /// <param name="buttonGap">The button gap.</param>
        /// <param name="buttonsStartGap">The buttons start gap.</param>
        /// <param name="centerButtons">if set to <c>true</c> [center buttons].</param>
        /// <param name="parentEdgeConfiguration">The parent edge configuration.</param>
        /// <param name="additionalEdgeConfiguration">The additional edge configuration.</param>
        /// <param name="maxChildren">The maximum children.</param>
        /// <param name="tapAction">The tap action.</param>
        /// <param name="longPressedAction">The long pressed action.</param>
        /// <param name="doubleTapAction">The double tap action.</param>
        /// <param name="progressIndicatorFillColor">Color of the progress indicator fill.</param>
        /// <param name="progressIndicatorStrokeColor">Color of the progress indicator stroke.</param>
        /// <param name="progressIndicatorLoopDuration">Duration of the progress indicator loop.</param>
        /// <param name="childrenStepCount">The children step count.</param>
        /// <param name="initialFontSize">Initial size of the font.</param>
        /// <param name="selectedBackgroundColor">Color of the selected background.</param>
        /// <param name="selectedSize">Size of the selected.</param>
        /// <param name="selectedBorderColor">Color of the selected border.</param>
        /// <param name="selectedBorderWidth">Width of the selected border.</param>
        /// <param name="selectedCornerRadius">The selected corner radius.</param>
        /// <param name="selectedCornerRadiusInPercent">The selected corner radius in percent.</param>
        /// <param name="selectedImageVisible">The selected image visible.</param>
        /// <param name="selectedTextVisible">The selected text visible.</param>
        /// <param name="selectedChildOffset">The selected child offset.</param>
        /// <param name="selectedRootOffset">The selected root offset.</param>
        /// <param name="selectedTextAngle">The selected text angle.</param>
        /// <param name="selectedMaxTextSize">Size of the selected maximum text.</param>
        /// <param name="selectedMaxImageSize">Size of the selected maximum image.</param>
        /// <param name="selectedImageOffset">The selected image offset.</param>
        /// <param name="selectedTextOffset">The selected text offset.</param>
        /// <param name="selectedShadowRadius">The selected shadow radius.</param>
        /// <param name="selectedShadowOffset">The selected shadow offset.</param>
        /// <param name="selectedShadowOpacity">The selected shadow opacity.</param>
        /// <param name="selectedShadowColor">Color of the selected shadow.</param>
        /// <param name="selectedButtonsFrameSize">Size of the selected buttons frame.</param>
        /// <param name="selectedButtonGap">The selected button gap.</param>
        /// <param name="selectedButtonsStartGap">The selected buttons start gap.</param>
        /// <param name="selectedCenterButtons">The selected center buttons.</param>
        /// <param name="selectedParentEdgeConfiguration">The selected parent edge configuration.</param>
        /// <param name="selectedAdditionalEdgeConfiguration">The selected additional edge configuration.</param>
        /// <param name="selectedMaxChildren">The selected maximum children.</param>
        /// <param name="selectedTapAction">The selected tap action.</param>
        /// <param name="selectedLongPressedAction">The selected long pressed action.</param>
        /// <param name="selectedDoubleTapAction">The selected double tap action.</param>
        /// <param name="selectedProgressIndicatorFillColor">Color of the selected progress indicator fill.</param>
        /// <param name="selectedProgressIndicatorStrokeColor">Color of the selected progress indicator stroke.</param>
        /// <param name="selectedProgressIndicatorLoopDuration">Duration of the selected progress indicator loop.</param>
        /// <param name="selectedChildrenStepCount">The selected children step count.</param>
        /// <param name="selectedInitialFontSize">Size of the selected initial font.</param>
        public UPMCoINodeViewConfig(AureaColor backgroundColor, Size size, AureaColor borderColor, float borderWidth, float cornerRadius,
            bool cornerRadiusInPercent, bool imageVisible, bool textVisible, Size childOffset, Size rootOffset, int textAngle,
            Size maxTextSize, Size maxImageSize, Size imageOffset, Size textOffset, float shadowRadius, Size shadowOffset,
            float shadowOpacity, AureaColor shadowColor, bool flatShadow, Size buttonsFrameSize, float buttonGap, float buttonsStartGap,
            bool centerButtons, UPMCoIEdgeViewConfig parentEdgeConfiguration, UPMCoIEdgeViewConfig additionalEdgeConfiguration, int maxChildren,
            UPNodeViewAction tapAction, UPNodeViewAction longPressedAction, UPNodeViewAction doubleTapAction, AureaColor progressIndicatorFillColor,
            AureaColor progressIndicatorStrokeColor, float progressIndicatorLoopDuration, int childrenStepCount, int initialFontSize,
            AureaColor selectedBackgroundColor, Size? selectedSize, AureaColor selectedBorderColor, float? selectedBorderWidth,
            float? selectedCornerRadius, bool? selectedCornerRadiusInPercent, bool? selectedImageVisible, bool? selectedTextVisible,
            Size? selectedChildOffset, Size? selectedRootOffset, int? selectedTextAngle, Size? selectedMaxTextSize,
            Size? selectedMaxImageSize, Size? selectedImageOffset, Size? selectedTextOffset, float? selectedShadowRadius,
            Size? selectedShadowOffset, float? selectedShadowOpacity, AureaColor selectedShadowColor, Size? selectedButtonsFrameSize,
            float? selectedButtonGap, float? selectedButtonsStartGap, bool? selectedCenterButtons,
            UPMCoIEdgeViewConfig selectedParentEdgeConfiguration, UPMCoIEdgeViewConfig selectedAdditionalEdgeConfiguration,
            int? selectedMaxChildren, UPNodeViewAction? selectedTapAction, UPNodeViewAction? selectedLongPressedAction, UPNodeViewAction? selectedDoubleTapAction,
            AureaColor selectedProgressIndicatorFillColor, AureaColor selectedProgressIndicatorStrokeColor, float? selectedProgressIndicatorLoopDuration,
            int? selectedChildrenStepCount, int? selectedInitialFontSize)
        {
            this.textRowConfig = new List<UPMCoITextRowViewConfig>();
            this.selectedTextRowConfig = new List<UPMCoITextRowViewConfig>();
            this.BackgroundColor = backgroundColor;
            this.Size = size;
            this.BorderColor = borderColor;
            this.BorderWidth = borderWidth;
            this.CornerRadius = cornerRadius;
            this.CornerRadiusInPercent = cornerRadiusInPercent;
            this.ImageVisible = imageVisible;
            this.TextVisible = textVisible;
            this.ChildOffset = childOffset;
            this.RootOffset = rootOffset;
            this.TextAngle = textAngle;
            this.MaxTextSize = maxTextSize;
            this.MaxImageSize = maxImageSize;
            this.ImageOffset = imageOffset;
            this.TextOffset = textOffset;
            this.ShadowColor = shadowColor;
            this.ShadowOffset = shadowOffset;
            this.ShadowOpacity = shadowOpacity;
            this.ShadowRadius = shadowRadius;
            this.FlatShadow = flatShadow;
            this.ButtonsFrameSize = buttonsFrameSize;
            this.ButtonGap = buttonGap;
            this.ButtonsStartGap = buttonsStartGap;
            this.CenterButtons = centerButtons;
            this.ParentEdgeConfiguration = parentEdgeConfiguration;
            this.AdditionalEdgeConfiguration = additionalEdgeConfiguration;
            this.MaxChildren = maxChildren < 1 ? int.MaxValue : maxChildren;
            this.TapAction = tapAction;
            this.DoubleTapAction = doubleTapAction;
            this.LongPressedAction = longPressedAction;
            this.ProgressIndicatorFillColor = progressIndicatorFillColor;
            this.ProgressIndicatorStrokeColor = progressIndicatorStrokeColor;
            this.ProgressIndicatorLoopDuration = progressIndicatorLoopDuration;
            this.ChildrenStepCount = childrenStepCount;
            this.InitialFontSize = initialFontSize;
            this.SelectedBackgroundColor = selectedBackgroundColor ?? backgroundColor;
            this.SelectedSize = selectedSize ?? size;
            this.SelectedBorderColor = selectedBorderColor ?? borderColor;
            this.SelectedBorderWidth = selectedBorderWidth ?? borderWidth;
            this.SelectedCornerRadius = selectedCornerRadius ?? cornerRadius;
            this.SelectedCornerRadiusInPercent = selectedCornerRadiusInPercent == true ? selectedCornerRadius != null && selectedCornerRadius != 0 : cornerRadiusInPercent;
            this.SelectedImageVisible = selectedImageVisible ?? imageVisible;
            this.SelectedTextVisible = selectedTextVisible ?? textVisible;
            this.SelectedChildOffset = selectedChildOffset ?? childOffset;
            this.SelectedRootOffset = selectedRootOffset ?? rootOffset;
            this.SelectedTextAngle = selectedTextAngle ?? textAngle;
            this.SelectedMaxTextSize = selectedMaxTextSize ?? maxTextSize;
            this.SelectedMaxImageSize = selectedMaxImageSize ?? maxImageSize;
            this.SelectedImageOffset = selectedImageOffset ?? imageOffset;
            this.SelectedTextOffset = selectedTextOffset ?? textOffset;
            this.SelectedShadowColor = selectedShadowColor ?? this.ShadowColor;
            this.SelectedShadowOffset = selectedShadowOffset ?? this.ShadowOffset;
            this.SelectedShadowOpacity = selectedShadowOpacity ?? this.ShadowOpacity;
            this.SelectedShadowRadius = selectedShadowRadius ?? this.ShadowRadius;
            this.SelectedButtonsFrameSize = selectedButtonsFrameSize ?? buttonsFrameSize;
            this.SelectedButtonGap = selectedButtonGap ?? buttonGap;
            this.SelectedButtonsStartGap = selectedButtonsStartGap ?? buttonsStartGap;
            this.SelectedCenterButtons = selectedCenterButtons ?? centerButtons;
            this.SelectedParentEdgeConfiguration = selectedParentEdgeConfiguration ?? parentEdgeConfiguration;
            this.SelectedAdditionalEdgeConfiguration = selectedAdditionalEdgeConfiguration ?? additionalEdgeConfiguration;
            this.SelectedMaxChildren = selectedMaxChildren ?? this.MaxChildren;
            this.SelectedTapAction = selectedTapAction ?? tapAction;
            this.SelectedDoubleTapAction = selectedDoubleTapAction ?? doubleTapAction;
            this.SelectedLongPressedAction = selectedLongPressedAction ?? longPressedAction;
            this.SelectedProgressIndicatorFillColor = selectedProgressIndicatorFillColor ?? progressIndicatorFillColor;
            this.SelectedProgressIndicatorStrokeColor = selectedProgressIndicatorStrokeColor ?? progressIndicatorStrokeColor;
            this.SelectedProgressIndicatorLoopDuration = selectedProgressIndicatorLoopDuration ?? progressIndicatorLoopDuration;
            this.SelectedChildrenStepCount = selectedChildrenStepCount ?? childrenStepCount;
            this.SelectedInitialFontSize = selectedInitialFontSize ?? initialFontSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoINodeViewConfig"/> class.
        /// </summary>
        public UPMCoINodeViewConfig()
            : this(AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), new Size(44, 44), AureaColor.RedGreenBlueAlpha(206.0 / 255.0, 82.0 / 255.0, 126.0 / 255.0, 1.0),
                  2.0f, 50.0f, true, true, true, new Size(75, 75), new Size(75, 75), int.MaxValue, new Size(150, 60), new Size(25, 40), default(Size),
                  default(Size), 0.0f, new Size(0, 1), 1.0f, AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), false,
                  new Size(150, 100), 60.0f, 55, true, new UPMCoIEdgeViewConfig(), null, 5,
                  UPNodeViewAction.SelectNodeAndExpandCollapse, UPNodeViewAction.SelectNode, UPNodeViewAction.CustomViaDelegate,
                  AureaColor.RedGreenBlueAlpha(187.0f / 255.0f, 187.0f / 255.0f, 187.0f / 255.0f, 1.0), AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0),
                  1.5f, 5, 14, AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, AureaColor.RedGreenBlue(255.0 / 255.0f, 90.0 / 255.0f, 16.0 / 255.0f),
                  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                  null, null, null, null, null, null, null, null, null, null)
        {
            UPMCoITextRowViewConfig rowConfig0 = this.TextRowConfigAtIndex(0);
            rowConfig0.Color = AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f);
            rowConfig0.Font = UIFont.FontWithNameSize("HelveticaNeue-Medium", 10);
            rowConfig0.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfig0.ShadowOffset = new Size(0, 1);
            rowConfig0.LineNumbers = 1;
            UPMCoITextRowViewConfig rowConfigS0 = this.SelectedTextRowConfigAtIndex(0);
            rowConfigS0.Color = AureaColor.RedGreenBlue(255.0 / 255.0f, 90.0 / 255.0f, 16.0 / 255.0f);
            rowConfigS0.Font = UIFont.FontWithNameSize("HelveticaNeue-Medium", 10);
            rowConfigS0.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfigS0.ShadowOffset = new Size(0, 1);
            rowConfigS0.LineNumbers = 1;
            UPMCoITextRowViewConfig rowConfig1 = this.TextRowConfigAtIndex(1);
            rowConfig1.Color = AureaColor.RedGreenBlue(102.0 / 255.0f, 102.0 / 255.0f, 102.0 / 255.0f);
            rowConfig1.Font = UIFont.FontWithNameSize("HelveticaNeue", 10);
            rowConfig1.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfig1.ShadowOffset = new Size(0, 1);
            rowConfig1.LineNumbers = 1;
            UPMCoITextRowViewConfig rowConfigS1 = this.SelectedTextRowConfigAtIndex(1);
            rowConfigS1.Color = AureaColor.RedGreenBlue(102.0 / 255.0f, 102.0 / 255.0f, 102.0 / 255.0f);
            rowConfigS1.Font = UIFont.FontWithNameSize("HelveticaNeue", 10);
            rowConfigS1.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfigS1.ShadowOffset = new Size(0, 1);
            rowConfigS1.LineNumbers = 1;
        }

        /// <summary>
        /// Gets or sets the initial size of the font.
        /// </summary>
        /// <value>
        /// The initial size of the font.
        /// </value>
        public int InitialFontSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public AureaColor BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        public AureaColor BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the border.
        /// </summary>
        /// <value>
        /// The width of the border.
        /// </value>
        public float BorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        public float CornerRadius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [corner radius in percent].
        /// Yes: cornerRadius is interpretated as percentage value.The cornerRadius will be calculated with the smallest size and the percentage value.
        /// No: cornerRadius is used as fix value
        /// </summary>
        /// <value>
        /// <c>true</c> if [corner radius in percent]; otherwise, <c>false</c>.
        /// </value>
        public bool CornerRadiusInPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [image visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [image visible]; otherwise, <c>false</c>.
        /// </value>
        public bool ImageVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [text visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [text visible]; otherwise, <c>false</c>.
        /// </value>
        public bool TextVisible { get; set; }

        /// <summary>
        /// Gets or sets the child offset.
        /// </summary>
        /// <value>
        /// The child offset.
        /// </value>
        public Size ChildOffset { get; set; }

        /// <summary>
        /// Gets or sets the root offset.
        /// </summary>
        /// <value>
        /// The root offset.
        /// </value>
        public Size RootOffset { get; set; }

        /// <summary>
        /// Gets or sets the text angle.
        /// textAngle: the angle used for the text to place the text around the node
        /// NSIntegerMin: The text is placed in the center of the node
        /// NSIntegerMax: In circle-View the angle from parent is used. In tree-view 180Â° is used.
        /// other values: fix angle to place the text around the node
        /// </summary>
        /// <value>
        /// The text angle.
        /// </value>
        public int TextAngle { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the text.
        /// </summary>
        /// <value>
        /// The maximum size of the text.
        /// </value>
        public Size MaxTextSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the image.
        /// </summary>
        /// <value>
        /// The maximum size of the image.
        /// </value>
        public Size MaxImageSize { get; set; }

        /// <summary>
        /// Gets or sets the image offset.
        /// </summary>
        /// <value>
        /// The image offset.
        /// </value>
        public Size ImageOffset { get; set; }

        /// <summary>
        /// Gets or sets the text offset.
        /// </summary>
        /// <value>
        /// The text offset.
        /// </value>
        public Size TextOffset { get; set; }

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
        /// Gets or sets the shadow opacity.
        /// </summary>
        /// <value>
        /// The shadow opacity.
        /// </value>
        public float ShadowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the color of the shadow.
        /// </summary>
        /// <value>
        /// The color of the shadow.
        /// </value>
        public AureaColor ShadowColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [flat shadow]. (for crmpad 20 design)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [flat shadow]; otherwise, <c>false</c>.
        /// </value>
        public bool FlatShadow { get; set; }

        /// <summary>
        /// Gets or sets the size of the buttons frame.
        /// </summary>
        /// <value>
        /// The size of the buttons frame.
        /// </value>
        public Size ButtonsFrameSize { get; set; }

        /// <summary>
        /// Gets or sets the button gap.
        /// </summary>
        /// <value>
        /// The button gap.
        /// </value>
        public float ButtonGap { get; set; }

        /// <summary>
        /// Gets or sets the buttons start gap.
        /// </summary>
        /// <value>
        /// The buttons start gap.
        /// </value>
        public float ButtonsStartGap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [center buttons].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [center buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool CenterButtons { get; set; }

        /// <summary>
        /// Gets or sets the parent edge configuration.
        /// </summary>
        /// <value>
        /// The parent edge configuration.
        /// </value>
        public UPMCoIEdgeViewConfig ParentEdgeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the additional edge configuration.
        /// </summary>
        /// <value>
        /// The additional edge configuration.
        /// </value>
        public UPMCoIEdgeViewConfig AdditionalEdgeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the maximum children.
        /// </summary>
        /// <value>
        /// The maximum children.
        /// </value>
        public int MaxChildren { get; set; }

        /// <summary>
        /// Gets or sets the tap action.
        /// </summary>
        /// <value>
        /// The tap action.
        /// </value>
        public UPNodeViewAction TapAction { get; set; }

        /// <summary>
        /// Gets or sets the double tap action.
        /// </summary>
        /// <value>
        /// The double tap action.
        /// </value>
        public UPNodeViewAction DoubleTapAction { get; set; }

        /// <summary>
        /// Gets or sets the long pressed action.
        /// </summary>
        /// <value>
        /// The long pressed action.
        /// </value>
        public UPNodeViewAction LongPressedAction { get; set; }

        /// <summary>
        /// Gets or sets the color of the progress indicator fill.
        /// </summary>
        /// <value>
        /// The color of the progress indicator fill.
        /// </value>
        public AureaColor ProgressIndicatorFillColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the progress indicator stroke.
        /// </summary>
        /// <value>
        /// The color of the progress indicator stroke.
        /// </value>
        public AureaColor ProgressIndicatorStrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the duration of the progress indicator loop.
        /// </summary>
        /// <value>
        /// The duration of the progress indicator loop.
        /// </value>
        public float ProgressIndicatorLoopDuration { get; set; }

        /// <summary>
        /// Gets or sets the children step count.
        /// </summary>
        /// <value>
        /// The children step count.
        /// </value>
        public int ChildrenStepCount { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected background.
        /// </summary>
        /// <value>
        /// The color of the selected background.
        /// </value>
        public AureaColor SelectedBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the size of the selected.
        /// </summary>
        /// <value>
        /// The size of the selected.
        /// </value>
        public Size SelectedSize { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected border.
        /// </summary>
        /// <value>
        /// The color of the selected border.
        /// </value>
        public AureaColor SelectedBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the selected border.
        /// </summary>
        /// <value>
        /// The width of the selected border.
        /// </value>
        public float SelectedBorderWidth { get; set; }

        /// <summary>
        /// Gets or sets the selected corner radius.
        /// </summary>
        /// <value>
        /// The selected corner radius.
        /// </value>
        public float SelectedCornerRadius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selected corner radius in percent].
        /// </summary>
        /// <value>
        /// <c>true</c> if [selected corner radius in percent]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectedCornerRadiusInPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selected image visible].
        /// </summary>
        /// <value>
        /// <c>true</c> if [selected image visible]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectedImageVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selected text visible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [selected text visible]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectedTextVisible { get; set; }

        /// <summary>
        /// Gets or sets the selected child offset.
        /// </summary>
        /// <value>
        /// The selected child offset.
        /// </value>
        public Size SelectedChildOffset { get; set; }

        /// <summary>
        /// Gets or sets the selected root offset.
        /// </summary>
        /// <value>
        /// The selected root offset.
        /// </value>
        public Size SelectedRootOffset { get; set; }

        /// <summary>
        /// Gets or sets the selected text angle.
        /// </summary>
        /// <value>
        /// The selected text angle.
        /// </value>
        public int SelectedTextAngle { get; set; }

        /// <summary>
        /// Gets or sets the size of the selected maximum text.
        /// </summary>
        /// <value>
        /// The size of the selected maximum text.
        /// </value>
        public Size SelectedMaxTextSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the selected maximum image.
        /// </summary>
        /// <value>
        /// The size of the selected maximum image.
        /// </value>
        public Size SelectedMaxImageSize { get; set; }

        /// <summary>
        /// Gets or sets the selected image offset.
        /// </summary>
        /// <value>
        /// The selected image offset.
        /// </value>
        public Size SelectedImageOffset { get; set; }

        /// <summary>
        /// Gets or sets the selected text offset.
        /// </summary>
        /// <value>
        /// The selected text offset.
        /// </value>
        public Size SelectedTextOffset { get; set; }

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
        /// Gets or sets the selected shadow opacity.
        /// </summary>
        /// <value>
        /// The selected shadow opacity.
        /// </value>
        public float SelectedShadowOpacity { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected shadow.
        /// </summary>
        /// <value>
        /// The color of the selected shadow.
        /// </value>
        public AureaColor SelectedShadowColor { get; set; }

        /// <summary>
        /// Gets or sets the size of the selected buttons frame.
        /// </summary>
        /// <value>
        /// The size of the selected buttons frame.
        /// </value>
        public Size SelectedButtonsFrameSize { get; set; }

        /// <summary>
        /// Gets or sets the selected button gap.
        /// </summary>
        /// <value>
        /// The selected button gap.
        /// </value>
        public float SelectedButtonGap { get; set; }

        /// <summary>
        /// Gets or sets the selected buttons start gap.
        /// </summary>
        /// <value>
        /// The selected buttons start gap.
        /// </value>
        public float SelectedButtonsStartGap { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [selected center buttons].
        /// </summary>
        /// <value>
        /// <c>true</c> if [selected center buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectedCenterButtons { get; set; }

        /// <summary>
        /// Gets or sets the selected parent edge configuration.
        /// </summary>
        /// <value>
        /// The selected parent edge configuration.
        /// </value>
        public UPMCoIEdgeViewConfig SelectedParentEdgeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the selected additional edge configuration.
        /// </summary>
        /// <value>
        /// The selected additional edge configuration.
        /// </value>
        public UPMCoIEdgeViewConfig SelectedAdditionalEdgeConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the selected maximum children.
        /// </summary>
        /// <value>
        /// The selected maximum children.
        /// </value>
        public int SelectedMaxChildren { get; set; }

        /// <summary>
        /// Gets or sets the selected tap action.
        /// </summary>
        /// <value>
        /// The selected tap action.
        /// </value>
        public UPNodeViewAction SelectedTapAction { get; set; }

        /// <summary>
        /// Gets or sets the selected double tap action.
        /// </summary>
        /// <value>
        /// The selected double tap action.
        /// </value>
        public UPNodeViewAction SelectedDoubleTapAction { get; set; }

        /// <summary>
        /// Gets or sets the selected long pressed action.
        /// </summary>
        /// <value>
        /// The selected long pressed action.
        /// </value>
        public UPNodeViewAction SelectedLongPressedAction { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected progress indicator fill.
        /// </summary>
        /// <value>
        /// The color of the selected progress indicator fill.
        /// </value>
        public AureaColor SelectedProgressIndicatorFillColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the selected progress indicator stroke.
        /// </summary>
        /// <value>
        /// The color of the selected progress indicator stroke.
        /// </value>
        public AureaColor SelectedProgressIndicatorStrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the duration of the selected progress indicator loop.
        /// </summary>
        /// <value>
        /// The duration of the selected progress indicator loop.
        /// </value>
        public float SelectedProgressIndicatorLoopDuration { get; set; }

        /// <summary>
        /// Gets or sets the selected children step count.
        /// </summary>
        /// <value>
        /// The selected children step count.
        /// </value>
        public int SelectedChildrenStepCount { get; set; }

        /// <summary>
        /// Gets or sets the size of the selected initial font.
        /// </summary>
        /// <value>
        /// The size of the selected initial font.
        /// </value>
        public int SelectedInitialFontSize { get; set; }

        /// <summary>
        /// Texts the index of the row configuration at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoITextRowViewConfig TextRowConfigAtIndex(int index)
        {
            while (index >= this.textRowConfig.Count)
            {
                this.textRowConfig.Add(UPMCoITextRowViewConfig.RowViewConfig());
            }

            return this.textRowConfig[index];
        }

        /// <summary>
        /// Selecteds the index of the text row configuration at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoITextRowViewConfig SelectedTextRowConfigAtIndex(int index)
        {
            while (index >= this.selectedTextRowConfig.Count)
            {
                this.selectedTextRowConfig.Add(UPMCoITextRowViewConfig.SelectedRowViewConfig());
            }

            return this.selectedTextRowConfig[index];
        }

        /// <summary>
        /// Sets the value for key.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void SetValueForKey(object value, string key)
        {
            if (key.StartsWith("field") || key.StartsWith("selectedfield"))
            {
                bool selected = key.StartsWith("selectedfield");
                string numberAsString = key.Substring(selected ? 13 : 5);
                if (numberAsString != null && value is Dictionary<string, object>)
                {
                    UPMCoITextRowViewConfig rowConfig = selected
                        ? this.SelectedTextRowConfigAtIndex(Convert.ToInt32(numberAsString))
                        : this.TextRowConfigAtIndex(Convert.ToInt32(numberAsString));

                    rowConfig.ApplyJsonDictionary((Dictionary<string, object>)value);
                }
            }
        }
    }
}
