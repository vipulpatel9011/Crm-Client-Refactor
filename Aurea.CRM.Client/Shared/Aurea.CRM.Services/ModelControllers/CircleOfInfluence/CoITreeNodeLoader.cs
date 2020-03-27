// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoITreeNodeLoader.cs" company="Aurea Software Gmbh">
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
//   The CoITreeNodeLoader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.CircleOfInfluence
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.UIModel.CircleOfInfluence;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// CoITreeNodeLoader
    /// </summary>
    /// <seealso cref="CoIBaseNodeLoader" />
    public class CoITreeNodeLoader : CoIBaseNodeLoader
    {
        private UPConfigTreeViewTable childrenTreeConfig;
        private Dictionary<IIdentifier, UPConfigTreeViewTable> nodeConfigMapping;
        private List<CoITreeInfoAreaConfig> infoAreaConfigs;
        private Dictionary<string, CoITreeInfoAreaConfig> recordIdentifierInfoAreaConfigMapping;
        private UPMCoINode tmpGroupNode;

        // Add this node only with children
        // Also used by ModelControllers

        /// <summary>
        /// Initializes a new instance of the <see cref="CoITreeNodeLoader"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="_rootNodeTreeConfig">The root node tree configuration.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="vistedNodes">The visted nodes.</param>
        /// <param name="_nodeConfigMapping">The node configuration mapping.</param>
        /// <param name="_recordIdentifierInfoAreaConfigMapping">The record identifier information area configuration mapping.</param>
        public CoITreeNodeLoader(UPMCoINode node, UPConfigTreeViewTable _rootNodeTreeConfig, ViewReference viewReference, int depth,
            Dictionary<IIdentifier, UPMCoINode> vistedNodes, Dictionary<IIdentifier, UPConfigTreeViewTable> _nodeConfigMapping,
            Dictionary<string, CoITreeInfoAreaConfig> _recordIdentifierInfoAreaConfigMapping)
            : base(node, viewReference, depth, 1, vistedNodes)
        {
            this.childrenTreeConfig = _rootNodeTreeConfig;
            this.nodeConfigMapping = _nodeConfigMapping;
            this.infoAreaConfigs = new List<CoITreeInfoAreaConfig>();
            this.recordIdentifierInfoAreaConfigMapping = _recordIdentifierInfoAreaConfigMapping;

            if (!string.IsNullOrEmpty(this.childrenTreeConfig.SearchAndListName))
            {
                this.configName = this.childrenTreeConfig.SearchAndListName;
            }
            else if (this.childrenTreeConfig != null)
            {
                this.configName = this.childrenTreeConfig.ExpandName;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.expandSettings = configStore.ExpandByName(this.configName);
        }

        /// <summary>
        /// Loads the node sub nodes.
        /// </summary>
        public override void LoadNodeSubNodes()
        {
            this.tmpGroupNode = null;
            string recordIdentification = this.RootNode.GroupNode
                ? ((RecordIdentifier)this.RootNode.Parent.Identifier).RecordIdentification
                : ((RecordIdentifier)this.RootNode.Identifier).RecordIdentification;

            if ((this.childrenTreeConfig.Flags & UPConfigTreeViewTableFlags.HideGroupNode) > 0 || this.RootNode.GroupNode)
            {
                UPContainerMetaInfo query = this.CreateQueryWithRecordIdentifier(recordIdentification);
                if (query != null)
                {
                    query.Find(this.requestOption, this);
                }
                else
                {
                    SimpleIoc.Default.GetInstance<ILogger>().LogWarn("Nodes not loaded. SearchAndList missing?");
                    this.TheDelegate.NodesLoadFailedError(this, null);
                }
            }
            else
            {
                // Create static Group Nodes
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

                // Create Node
                // id ParentNodeId + SearchAndListName
                string idString = $"Node {this.childrenTreeConfig.SearchAndListName}, {recordIdentification}";
                this.tmpGroupNode = new UPMCoINode(StringIdentifier.IdentifierWithStringId(idString));
                this.tmpGroupNode.Parent = this.RootNode;
                UPMStringField titleField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"Title {this.childrenTreeConfig.SearchAndListName}"));
                titleField.StringValue = this.childrenTreeConfig.Label;
                List<UPMStringField> fields = new List<UPMStringField> { titleField };
                this.tmpGroupNode.Fields = fields;
                this.tmpGroupNode.InfoFields = fields;
                this.tmpGroupNode.GroupNode = true;
                //this.tmpGroupNode.Icon = UIImage.UpImageWithFileName(configStore.InfoAreaConfigById(this.childrenTreeConfig.InfoAreaId).ImageName);  // CRM-5007

                // load next Level directly to avoid Empty Static Nodes
                CoITreeNodeLoader loader = new CoITreeNodeLoader(this.tmpGroupNode, this.childrenTreeConfig, this.viewReference, 0, this.vistedNodes, this.nodeConfigMapping, this.recordIdentifierInfoAreaConfigMapping);
                loader.Mode = CoINodeLoaderMode.InitialLoad;
                loader.TheDelegate = this;
                this.subNodeLoader.Add(loader);
                this.pendingCount++;
                loader.LoadNodeSubNodes();
            }
        }

        private UPContainerMetaInfo CreateQueryWithRecordIdentifier(string recordIdentifier)
        {
            string childSearchAndListName = this.childrenTreeConfig.SearchAndListName;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(childSearchAndListName);
            UPContainerMetaInfo metainfo = this.CreateQueryLinkIdSearchAndList(recordIdentifier, this.childrenTreeConfig.LinkId, searchAndList);
            if (metainfo == null)
            {
                return null;
            }

            if (this.childrenTreeConfig.RecordCount <= 0)
            {
                metainfo.MaxResults = 0;
            }
            else
            {
                metainfo.MaxResults = this.childrenTreeConfig.RecordCount - 1;
            }

            // Add additionalConfig for RecordNodeChilds
            foreach (UPConfigTreeViewTable childTree in this.childrenTreeConfig.ChildNodes)
            {
                if (this.IsRecordNode(childTree))
                {
                    string recordCustomControl = childTree.RecordCustomControl;
                    Dictionary<string, object> customControlDict = recordCustomControl.JsonDictionaryFromString();
                    string nodeType = customControlDict["Type"] as string;
                    if (nodeType.StartsWith("Tree:"))
                    {
                        CoITreeInfoAreaConfig infoAreaConfig = new CoITreeInfoAreaConfig
                        {
                            InfoAreaId = childTree.InfoAreaId,
                            LinkId = childTree.LinkId
                        };
                        string jumpTreeConfigName = nodeType.Substring(5);
                        UPConfigTreeView treeConfig = configStore.TreeViewByName(jumpTreeConfigName);
                        infoAreaConfig.Config = treeConfig.RootNode;
                        infoAreaConfig.FunctionNameSuffix = childTree.RelationName;
                        infoAreaConfig.Definition = customControlDict;
                        this.infoAreaConfigs.Add(infoAreaConfig);
                    }
                    else if (nodeType.StartsWith("LinkNode"))
                    {
                        CoITreeInfoAreaConfig infoAreaConfig = new CoITreeInfoAreaConfig
                        {
                            InfoAreaId = childTree.InfoAreaId,
                            LinkId = childTree.LinkId,
                            Config = childTree,
                            FunctionNameSuffix = childTree.RelationName,
                            Definition = customControlDict
                        };
                        this.infoAreaConfigs.Add(infoAreaConfig);
                    }
                    else
                    {
                        SimpleIoc.Default.GetInstance<ILogger>().LogWarn($"UPCoITree cant handle nodeType: {nodeType}");
                    }
                }
            }

            return metainfo;
        }

        /// <summary>
        /// Nodes are loaded succesfull.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        public override void NodesLoadedSuccesfull(CoIBaseNodeLoader nodeloader)
        {
            if (this.tmpGroupNode != null && this.tmpGroupNode.ChildCount > 0)
            {
                this.vistedNodes.SetObjectForKey(this.tmpGroupNode, this.tmpGroupNode.Identifier);
                this.nodeConfigMapping.SetObjectForKey(this.childrenTreeConfig, this.tmpGroupNode.Identifier);

                // Create Edge
                UPMCoIEdge edge = new UPMCoIEdge(StringIdentifier.IdentifierWithStringId($"Edge {this.childrenTreeConfig.SearchAndListName}"));
                edge.ListFields = this.tmpGroupNode.Fields;
                this.RootNode.AddChildNodeChildRelation(this.tmpGroupNode, edge);
                this.RootNode.AddAllRelationToRelation(this.tmpGroupNode, edge);
                this.ChangedIdentifiers.Add(this.tmpGroupNode.Identifier);
                this.tmpGroupNode = null;
            }

            this.RootNode.ChildsLoaded = true;
            base.NodesLoadedSuccesfull(nodeloader);
        }

        /// <summary>
        /// Creates the sub node load for node edge row.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="edge">The edge.</param>
        /// <param name="row">The row.</param>
        protected override void CreateSubNodeLoadForNodeEdgeRow(UPMCoINode node, UPMCoIEdge edge, UPCRMResultRow row)
        {
            this.vistedNodes[node.Identifier] = node;
            this.nodeConfigMapping.SetObjectForKey(this.childrenTreeConfig, node.Identifier);
            this.RootNode.AddChildNodeChildRelation(node, edge);
            this.ChangedIdentifiers.Add(node.Identifier);
        }

        /// <summary>
        /// Creates the node from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override UPMCoINode CreateNodeFromResultRow(UPCRMResultRow row)
        {
            return CreateNodeFromResultRow(row, this.depth, this.expandChecker, this.infoAreaConfigs, this.recordIdentifierInfoAreaConfigMapping);
        }

        /// <summary>
        /// Creates the node from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="currentDepth">The current depth.</param>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="infoAreaConfigs">The information area configs.</param>
        /// <param name="infoAreaMapping">The information area mapping.</param>
        /// <returns></returns>
        public static UPMCoINode CreateNodeFromResultRow(UPCRMResultRow row, int currentDepth,
            UPConfigExpand expandChecker, List<CoITreeInfoAreaConfig> infoAreaConfigs, Dictionary<string, CoITreeInfoAreaConfig> infoAreaMapping)
        {
            string trgtNodeFieldSuffix = string.Empty;
            string recordIdentification = row.RootRecordIdentification;
            string rootInfoAreaId = recordIdentification.InfoAreaId();
            if (infoAreaConfigs != null)
            {
                foreach (CoITreeInfoAreaConfig infoAreaConfig in infoAreaConfigs)
                {
                    string linkRecordIdentifictaion = row.RecordIdentificationForLinkInfoAreaIdLinkId(infoAreaConfig.InfoAreaId, infoAreaConfig.LinkId);
                    if (!string.IsNullOrEmpty(linkRecordIdentifictaion.RecordId()))
                    {
                        trgtNodeFieldSuffix = infoAreaConfig.FunctionNameSuffix;
                        recordIdentification = linkRecordIdentifictaion;
                        infoAreaMapping[recordIdentification] = infoAreaConfig;
                        break;
                    }
                }
            }

            string virtualInfoAreaId = row.RootVirtualInfoAreaId == rootInfoAreaId ? null : row.RootVirtualInfoAreaId;
            UPMCoINode node = CreateNodeFromResultRow(row, virtualInfoAreaId, recordIdentification, currentDepth, infoAreaConfigs == null,
                expandChecker, trgtNodeFieldSuffix, recordIdentification.InfoAreaId());

            return node;
        }

        /// <summary>
        /// Creates the edge from result row source node DST node.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="srcNode">The source node.</param>
        /// <param name="dstNode">The DST node.</param>
        /// <returns></returns>
        protected override UPMCoIEdge CreateEdgeFromResultRow(UPCRMResultRow row, UPMCoINode srcNode, UPMCoINode dstNode)
        {
            string trgtNodeFieldSuffix = string.Empty;
            if (this.infoAreaConfigs != null)
            {
                foreach (CoITreeInfoAreaConfig infoAreaConfig in this.infoAreaConfigs)
                {
                    string linkRecordIdentifictaion = row.RecordIdentificationForLinkInfoAreaIdLinkId(infoAreaConfig.InfoAreaId, infoAreaConfig.LinkId);
                    if (!string.IsNullOrEmpty(linkRecordIdentifictaion.RecordId()))
                    {
                        trgtNodeFieldSuffix = infoAreaConfig.FunctionNameSuffix;
                        break;
                    }
                }
            }

            UPMCoIEdge edge = CreateEdgeFromResultRow(row, trgtNodeFieldSuffix, srcNode, dstNode);
            return edge;
        }

        private bool IsRecordNode(UPConfigTreeViewTable treeTable)
        {
            return !string.IsNullOrEmpty(treeTable.RecordCustomControl);
        }
    }
}
