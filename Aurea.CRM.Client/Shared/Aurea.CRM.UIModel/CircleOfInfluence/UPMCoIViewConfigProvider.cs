// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCoIViewConfigProvider.cs" company="Aurea Software Gmbh">
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
//   The UPMCoIViewConfigProvider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.UIModel.Structs;

    /// <summary>
    /// UPNodesViewType
    /// </summary>
    public enum UPNodesViewType
    {
        /// <summary>
        /// Circle
        /// </summary>
        Circle = 0,

        /// <summary>
        /// Tree
        /// </summary>
        Tree
    }

    /// <summary>
    /// UPEdgeViewType
    /// </summary>
    public enum UPEdgeViewType
    {
        /// <summary>
        /// Line
        /// </summary>
        Line = 0,

        /// <summary>
        /// Curve
        /// </summary>
        Curve,

        /// <summary>
        /// Orthogonal
        /// </summary>
        Orthogonal,
    }

    /// <summary>
    /// UPChildNodeMoveType
    /// </summary>
    public enum UPChildNodeMoveType
    {
        /// <summary>
        /// Always
        /// </summary>
        Always = 0,

        /// <summary>
        /// Only untouched
        /// </summary>
        OnlyUntouched,
    }

    /// <summary>
    /// UPMCoIViewConfigProvider
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.CircleOfInfluence.UPMCoIConfigBase" />
    public class UPMCoIViewConfigProvider : UPMCoIConfigBase
    {
        private const string HelveticaNeueBold = "HelveticaNeue-Bold";
        private const string HelveticaNeueMedium = "HelveticaNeue-Medium";
        private const string HelveticaNeue = "HelveticaNeue";

        private List<UPMCoIViewConfig> configurations;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCoIViewConfigProvider"/> class.
        /// </summary>
        public UPMCoIViewConfigProvider()
        {
            var systemColor = AureaColor.UpCurrentSystemColor();
            this.configurations = new List<UPMCoIViewConfig>();
            this.NodesViewType = UPNodesViewType.Circle;
            this.EdgeViewType = UPEdgeViewType.Line;
            this.ChildMoveType = UPChildNodeMoveType.Always;
            this.MinNodes = 3;
            this.PreAnimationDuration = 0.25f;
            this.PostAnimationDuration = 0.25f;
            this.AnimationDuration = 0.25f;
            this.FrameAndLabelAsBase = true;
            this.LessOrMoreChildrenAnimationDuration = 0.4f;

            var parentEdgeConfigurationLevel1 = new UPMCoIEdgeViewConfig(UIFont.SystemFontOfSize(12), 5.0f,
                AureaColor.RedGreenBlue(2.0 / 3.0, 2.0 / 3.0, 2.0 / 3.0), AureaColor.RedGreenBlue(238.0f / 255.0f, 238.0f / 255.0f, 238.0f / 255.0f),
                5.0f, 0.2f, /*kCALineJoinMiter*/ null, null, 0.0f, 1.0f, 0.4f, 0.0f, new Size(0, 1), AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f),
                1.0f, 1.0f, new Size(0, 1), AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, 12.0f, AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f),
                AureaColor.RedGreenBlue(255.0 / 255.0, 90.0 / 255.0, 16.0 / 255.0), null, null, null, null, 1.0f, 1.0f, null, null, null, null, null, null, null, null);

            var parentEdgeConfigurationLevel2 = new UPMCoIEdgeViewConfig(UIFont.SystemFontOfSize(12), 5.0f,
                AureaColor.RedGreenBlue(2.0 / 3.0, 2.0 / 3.0, 2.0 / 3.0), AureaColor.RedGreenBlue(238.0f / 255.0f, 238.0f / 255.0f, 238.0f / 255.0f),
                3.0f, 0.2f, /*kCALineJoinMiter*/ null, null, 0.0f, 1.0f, 0.4f, 0.0f, new Size(0, 1), AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f),
                1.0f, 1.0f, new Size(0, 1), AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, 12.0f, AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f),
                AureaColor.RedGreenBlue(255.0 / 255.0f, 90.0 / 255.0f, 16.0 / 255.0f), null, null, null, null, 1.0f, 1.0f, null, null, null, null, null, null, null, null);

            var additionalEdgeConfiguration = new UPMCoIEdgeViewConfig(UIFont.SystemFontOfSize(12), 5.0f,
                AureaColor.RedGreenBlue(2.0 / 3.0, 2.0 / 3.0, 2.0 / 3.0), AureaColor.RedGreenBlue(238.0f / 255.0f, 238.0f / 255.0f, 238.0f / 255.0f),
                3.0f, 0.2f, /*kCALineJoinMiter*/ null, new List<int> { 5, 5 }, 0.0f, 1.0f, 0.4f, 0.0f, new Size(0, 1),
                AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), 1.0f, 1, new Size(0, 1), AureaColor.RedGreenBlue(1.0, 1.0, 1.0),
                null, null, AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f), AureaColor.RedGreenBlue(255.0 / 255.0f, 90.0 / 255.0f, 16.0 / 255.0f),
                null, null, null, null, 1.0f, 1.0f, null, null, null, null, null, null, null, null);

            var config = new UPMCoIViewConfig();
            this.configurations.Add(config);

            Level0Config(systemColor, additionalEdgeConfiguration, config);
            Level1Config(systemColor, parentEdgeConfigurationLevel1, additionalEdgeConfiguration, config);
            Level2Config(systemColor, parentEdgeConfigurationLevel2, additionalEdgeConfiguration, config);
            Level3Config(systemColor, parentEdgeConfigurationLevel2, additionalEdgeConfiguration, config);
            Level4Config(systemColor, parentEdgeConfigurationLevel2, additionalEdgeConfiguration, config);
        }

        /// <summary>
        /// Gets or sets the start configuration.
        /// </summary>
        /// <value>
        /// The start configuration.
        /// </value>
        public int StartConfig { get; set; }

        /// <summary>
        /// Gets or sets the minimum nodes.
        /// </summary>
        /// <value>
        /// The minimum nodes.
        /// </value>
        public int MinNodes { get; set; }

        /// <summary>
        /// Gets or sets the type of the nodes view.
        /// </summary>
        /// <value>
        /// The type of the nodes view.
        /// </value>
        public UPNodesViewType NodesViewType { get; set; }

        /// <summary>
        /// Gets or sets the type of the edge view.
        /// </summary>
        /// <value>
        /// The type of the edge view.
        /// </value>
        public UPEdgeViewType EdgeViewType { get; set; }

        /// <summary>
        /// Gets or sets the duration of the pre animation.
        /// </summary>
        /// <value>
        /// The duration of the pre animation.
        /// </value>
        public float PreAnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration of the post animation.
        /// </summary>
        /// <value>
        /// The duration of the post animation.
        /// </value>
        public float PostAnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        /// <value>
        /// The duration of the animation.
        /// </value>
        public float AnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [frame and label as base].
        /// </summary>
        /// <value>
        /// <c>true</c> if [frame and label as base]; otherwise, <c>false</c>.
        /// </value>
        public bool FrameAndLabelAsBase { get; set; }

        /// <summary>
        /// Gets or sets the duration of the less or more children animation.
        /// </summary>
        /// <value>
        /// The duration of the less or more children animation.
        /// </value>
        public float LessOrMoreChildrenAnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets the type of the child move.
        /// </summary>
        /// <value>
        /// The type of the child move.
        /// </value>
        public UPChildNodeMoveType ChildMoveType { get; set; }

        /// <summary>
        /// Configurations at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public UPMCoIViewConfig ConfigAtIndex(int index)
        {
            while (index >= this.configurations.Count)
            {
                this.configurations.Add(new UPMCoIViewConfig());
            }

            return this.configurations[index];
        }

        /// <summary>
        /// Adds the configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void AddConfig(UPMCoIViewConfig config)
        {
            this.configurations.Add(new UPMCoIViewConfig());
        }

        /// <summary>
        /// Applies the json configuration.
        /// </summary>
        /// <param name="jsonNodesViewConfig">The json nodes view configuration.</param>
        public void ApplyJsonConfig(string jsonNodesViewConfig)
        {
            if (!string.IsNullOrEmpty(jsonNodesViewConfig))
            {
#if PORTING
                NSData data = jsonNodesViewConfig.DataUsingEncoding(NSUTF8StringEncoding);
                NSError error = null;
                object theObject = NSJSONSerialization.JSONObjectWithDataOptionsError(data, -1, error);
                if (!error)
                {
                    NSDictionary dict = (NSDictionary)theObject;
                    foreach (string key in dict.Keys)
                    {
                        if (key.StartsWith("#"))
                        {
                            int config = 0;
                            int level = 0;
                            int group = 0;
                            var components = key.Substring(1).Split('#');
                            foreach (string component in components)
                            {
                                switch (component[0])
                                {
                                    case 'c':
                                        config = Convert.ToInt32(component.Substring(1));
                                        break;
                                    case 'l':
                                        level = Convert.ToInt32(component.Substring(1));
                                        break;
                                    case 'g':
                                        group = Convert.ToInt32(component.Substring(1));
                                        break;
                                    default:
                                        break;
                                }
                            }

                            UPMCoINodeViewConfig nodeViewConfig = this.ConfigAtIndex(config).ConfigAtIndex(level).ConfigAtIndex(group);
                            nodeViewConfig.ApplyJsonDictionary(dict.ObjectForKey(key));
                        }
                        else
                        {
                            // Standard Property
                            this.SetValueForKey(dict.ObjectForKey(key), key);
                        }
                    }
                }
#endif
            }
        }

        private static void Level4Config(AureaColor systemColor, UPMCoIEdgeViewConfig parentEdgeConfigurationLevel2, UPMCoIEdgeViewConfig additionalEdgeConfiguration, UPMCoIViewConfig config)
        {
            var level = new UPMCoILevelViewConfig();

            var node = new UPMCoINodeViewConfig(AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), new Size(44, 44),
                AureaColor.RedGreenBlueAlpha(206.0 / 255.0, 82.0 / 255.0, 126.0 / 255.0, 1.0), 2.0f, 50.0f, true, true, true,
                new Size(75, 75), new Size(75, 75), int.MaxValue, new Size(150, 60), new Size(25, 40), new Size(0, 0),
                new Size(0, 0), 0.0f, new Size(0, 1), 1.0f, AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), false,
                new Size(150, 100), 60.0f, 55, true, parentEdgeConfigurationLevel2, additionalEdgeConfiguration, 5,
                UPNodeViewAction.SelectNodeAndExpandCollapse, UPNodeViewAction.SelectNode, UPNodeViewAction.CustomViaDelegate,
                AureaColor.RedGreenBlueAlpha(187.0f / 255.0f, 187.0f / 255.0f, 187.0f / 255.0f, 1.0), AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0),
                1.5f, 5, 17, AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, systemColor, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            UpdateRowConfig(
                systemColor,
                node,
                UIFont.FontWithNameSize(HelveticaNeueMedium, 10),
                UIFont.FontWithNameSize(HelveticaNeueMedium, 10),
                UIFont.FontWithNameSize(HelveticaNeue, 10),
                UIFont.FontWithNameSize(HelveticaNeue, 10));

            level.AddConfig(node);
            config.AddConfig(level);
        }

        private static void Level3Config(AureaColor systemColor, UPMCoIEdgeViewConfig parentEdgeConfigurationLevel2, UPMCoIEdgeViewConfig additionalEdgeConfiguration, UPMCoIViewConfig config)
        {
            var level = new UPMCoILevelViewConfig();

            var node = new UPMCoINodeViewConfig(AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), new Size(44, 44),
                AureaColor.RedGreenBlueAlpha(255.0 / 255.0, 183.0 / 255.0, 53.0 / 255.0, 1.0), 2.0f, 50.0f, true, true, true, new Size(75, 75),
                new Size(75, 75), int.MaxValue, new Size(150, 60), new Size(25, 40), new Size(0, 0), new Size(0, 0), 0.0f, new Size(0, 1), 1.0f,
                AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), false, new Size(150, 100), 60.0f, 55, true,
                parentEdgeConfigurationLevel2, additionalEdgeConfiguration, 5, UPNodeViewAction.SelectNodeAndExpandCollapse, UPNodeViewAction.SelectNode,
                UPNodeViewAction.CustomViaDelegate, AureaColor.RedGreenBlueAlpha(187.0f / 255.0f, 187.0f / 255.0f, 187.0f / 255.0f, 1.0),
                AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), 1.5f, 5, 17, AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, systemColor,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null);

            UpdateRowConfig(
              systemColor,
              node,
              UIFont.FontWithNameSize(HelveticaNeueMedium, 10),
              UIFont.FontWithNameSize(HelveticaNeueMedium, 10),
              UIFont.FontWithNameSize(HelveticaNeue, 10),
              UIFont.FontWithNameSize(HelveticaNeue, 10));

            level.AddConfig(node);
            config.AddConfig(level);
        }

        private static void Level2Config(AureaColor systemColor, UPMCoIEdgeViewConfig parentEdgeConfigurationLevel2, UPMCoIEdgeViewConfig additionalEdgeConfiguration, UPMCoIViewConfig config)
        {
            var level = new UPMCoILevelViewConfig();

            var node = new UPMCoINodeViewConfig(AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), new Size(44, 44),
                AureaColor.RedGreenBlueAlpha(130.0 / 255.0, 177.0 / 255.0, 214.0 / 255.0, 1.0), 2.0f, 50.0f, true, true, true, new Size(75, 75),
                new Size(75, 75), int.MaxValue, new Size(150, 60), new Size(25, 40), new Size(0, 0), new Size(0, 0), 0.0f, new Size(0, 1), 1.0f,
                AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), false, new Size(150, 100), 60.0f, 55, true,
                parentEdgeConfigurationLevel2, additionalEdgeConfiguration, 5, UPNodeViewAction.SelectNodeAndExpandCollapse, UPNodeViewAction.SelectNode,
                UPNodeViewAction.CustomViaDelegate, AureaColor.RedGreenBlueAlpha(187.0f / 255.0f, 187.0f / 255.0f, 187.0f / 255.0f, 1.0),
                AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), 1.5f, 5, 17, AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, systemColor,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null);

            UpdateRowConfig(
                systemColor,
                node,
                UIFont.FontWithNameSize(HelveticaNeueMedium, 10),
                UIFont.FontWithNameSize(HelveticaNeueMedium, 10),
                UIFont.FontWithNameSize(HelveticaNeue, 10),
                UIFont.FontWithNameSize(HelveticaNeue, 10));

            level.AddConfig(node);
            config.AddConfig(level);
        }

        private static void Level1Config(AureaColor systemColor, UPMCoIEdgeViewConfig parentEdgeConfigurationLevel1, UPMCoIEdgeViewConfig additionalEdgeConfiguration, UPMCoIViewConfig config)
        {
            var level = new UPMCoILevelViewConfig();

            var node = new UPMCoINodeViewConfig(AureaColor.RedGreenBlue(1.0, 1.0, 1.0), new Size(58, 58),
                AureaColor.RedGreenBlueAlpha(100.0 / 255.0, 162.0 / 255.0, 160.0 / 255.0, 1.0), 5.0f, 50.0f, true, true, true, new Size(75, 75),
                new Size(175, 175), int.MaxValue, new Size(150, 60), new Size(25, 40), new Size(0, 0), new Size(0, 0), 0.0f, new Size(0, 1), 1.0f,
                AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), false, new Size(150, 100), 60.0f, 55, true,
                parentEdgeConfigurationLevel1, additionalEdgeConfiguration, 5, UPNodeViewAction.SelectNodeAndExpandCollapse, UPNodeViewAction.SelectNode,
                UPNodeViewAction.CustomViaDelegate, AureaColor.RedGreenBlueAlpha(187.0f / 255.0f, 187.0f / 255.0f, 187.0f / 255.0f, 1.0),
                AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0), 1.5f, 5, 17, AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, systemColor,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null);

            level.AddConfig(node);

            UpdateRowConfig(
                systemColor,
                node,
                UIFont.FontWithNameSize(HelveticaNeue, 12),
                UIFont.FontWithNameSize(HelveticaNeue, 12),
                UIFont.FontWithNameSize(HelveticaNeue, 12),
                UIFont.FontWithNameSize(HelveticaNeue, 12));

            config.AddConfig(level);
        }

        private static void UpdateRowConfig(
            AureaColor systemColor,
            UPMCoINodeViewConfig node,
            UIFont textFont0,
            UIFont selectedTextFont0,
            UIFont textFont1,
            UIFont selectedTextFont1)
        {
            var rowConfig0 = node.TextRowConfigAtIndex(0);
            rowConfig0.Color = AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f);
            rowConfig0.Font = textFont0;
            rowConfig0.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfig0.ShadowOffset = new Size(0, 1);
            rowConfig0.LineNumbers = 1;

            var rowConfigS0 = node.SelectedTextRowConfigAtIndex(0);
            rowConfigS0.Color = systemColor;
            rowConfigS0.Font = selectedTextFont0;
            rowConfigS0.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfigS0.ShadowOffset = new Size(0, 1);
            rowConfigS0.LineNumbers = 1;

            var rowConfig1 = node.TextRowConfigAtIndex(1);
            rowConfig1.Color = AureaColor.RedGreenBlue(102.0 / 255.0f, 102.0 / 255.0f, 102.0 / 255.0f);
            rowConfig1.Font = textFont1;
            rowConfig1.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfig1.ShadowOffset = new Size(0, 1);
            rowConfig1.LineNumbers = 1;

            var rowConfigS1 = node.SelectedTextRowConfigAtIndex(1);
            rowConfigS1.Color = systemColor;
            rowConfigS1.Font = selectedTextFont1;
            rowConfigS1.ShadowColor = AureaColor.RedGreenBlue(1.0, 1.0, 1.0);
            rowConfigS1.ShadowOffset = new Size(0, 1);
            rowConfigS1.LineNumbers = 1;
        }

        private static void Level0Config(AureaColor systemColor, UPMCoIEdgeViewConfig additionalEdgeConfiguration, UPMCoIViewConfig config)
        {
            var level = new UPMCoILevelViewConfig();
            var node = new UPMCoINodeViewConfig(AureaColor.RedGreenBlue(1.0, 1.0, 1.0), new Size(117, 117),
                AureaColor.RedGreenBlueAlpha(179.0 / 255.0, 210.0 / 255.0, 103.0 / 255.0, 1.0), 8.0f, 50.0f, true, true, true, new Size(175, 175),
                new Size(100, 100), int.MinValue, new Size(75, 50), new Size(40, 40), new Size(0, -25), new Size(0, 10), 0.0f, new Size(0, 1), 1.0f,
                AureaColor.RedGreenBlue(219.0f / 255.0f, 219.0f / 255.0f, 219.0f / 255.0f), false, new Size(150, 100), 60.0f, 55, true, null,
                additionalEdgeConfiguration, 5, UPNodeViewAction.SelectNodeAndExpandCollapse, UPNodeViewAction.SelectNode, UPNodeViewAction.RelayoutNode,
                AureaColor.RedGreenBlueAlpha(187.0f / 255.0f, 187.0f / 255.0f, 187.0f / 255.0f, 1.0), AureaColor.RedGreenBlueAlpha(1.0, 1.0, 1.0, 1.0),
                1.5f, 5, 26, AureaColor.RedGreenBlue(1.0, 1.0, 1.0), null, systemColor, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            level.AddConfig(node);

            var rowConfig0 = node.TextRowConfigAtIndex(0);
            rowConfig0.Color = AureaColor.RedGreenBlue(51.0 / 255.0f, 51.0 / 255.0f, 51.0 / 255.0f);
            rowConfig0.Font = UIFont.FontWithNameSize(HelveticaNeueBold, 13);

            var rowConfigS0 = node.SelectedTextRowConfigAtIndex(0);
            rowConfigS0.Color = systemColor;
            rowConfigS0.Font = UIFont.FontWithNameSize(HelveticaNeueBold, 13);

            var rowConfig1 = node.TextRowConfigAtIndex(1);
            rowConfig1.Color = AureaColor.RedGreenBlue(102.0 / 255.0f, 102.0 / 255.0f, 102.0 / 255.0f);
            rowConfig1.Font = UIFont.FontWithNameSize(HelveticaNeueMedium, 12);

            var rowConfigS1 = node.SelectedTextRowConfigAtIndex(1);
            rowConfigS1.Color = systemColor;
            rowConfigS1.Font = UIFont.FontWithNameSize(HelveticaNeueMedium, 12);

            config.AddConfig(level);
        }
    }
}
