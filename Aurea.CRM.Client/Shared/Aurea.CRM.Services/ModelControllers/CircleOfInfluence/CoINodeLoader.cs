// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoINodeLoader.cs" company="Aurea Software Gmbh">
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
//   The CoINodeLoader.
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
    using Aurea.CRM.UIModel.CircleOfInfluence;

    /// <summary>
    /// CoINodeLoader
    /// </summary>
    /// <seealso cref="CoIBaseNodeLoader" />
    public class CoINodeLoader : CoIBaseNodeLoader
    {
        private string fiFilterName;
        private string kpFilterName;
        private int fiResultColumnIndex;
        private int kpResultColumnIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoINodeLoader"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <param name="vistedNodes">The visted nodes.</param>
        public CoINodeLoader(UPMCoINode node, ViewReference viewReference, int depth, int maxDepth, Dictionary<IIdentifier, UPMCoINode> vistedNodes)
            : base(node, viewReference, depth, maxDepth, vistedNodes)
        {
            this.fiFilterName = this.viewReference.ContextValueForKey("AdditionalFIFilter");
            this.kpFilterName = this.viewReference.ContextValueForKey("AdditionalKPFilter");
            this.fiResultColumnIndex = -1;
            this.kpResultColumnIndex = -1;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="recordIdentifier">The record identifier.</param>
        /// <returns></returns>
        protected override UPContainerMetaInfo CreateQuery(string recordIdentifier)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndList = configStore.SearchAndListByName(this.configName);
            if (searchAndList == null)
            {
                return null;
            }

            UPContainerMetaInfo query = this.CreateQueryLinkIdSearchAndList(recordIdentifier, -1, searchAndList);
            if (recordIdentifier.InfoAreaId() == "FI" && !string.IsNullOrEmpty(this.fiFilterName))
            {
                query.ApplyFilter(configStore.FilterByName(this.fiFilterName));
            }
            else if (recordIdentifier.InfoAreaId() == "KP" && !string.IsNullOrEmpty(this.kpFilterName))
            {
                query.ApplyFilter(configStore.FilterByName(this.kpFilterName));
            }

            return query;
        }

        /// <summary>
        /// Creates the node from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override UPMCoINode CreateNodeFromResultRow(UPCRMResultRow row)
        {
            if (this.fiResultColumnIndex == -1)
            {
                this.fiResultColumnIndex = row.Result.MetaInfo.IndexOfResultInfoAreaIdLinkId("FI", 1);
            }

            if (this.kpResultColumnIndex == -1)
            {
                this.kpResultColumnIndex = row.Result.MetaInfo.IndexOfResultInfoAreaIdLinkId("KP", 1);
            }

            bool kpRow = false;
            string recordIdentification = null;
            string virtualInfoArea = null;
            string physicalInfoArea = null;
            if (this.kpResultColumnIndex != -1)
            {
                recordIdentification = row.RecordIdentificationAtIndex(this.kpResultColumnIndex);
                if (recordIdentification?.Length > 3)
                {
                    kpRow = true;
                    virtualInfoArea = row.VirtualInfoAreaIdAtIndex(this.kpResultColumnIndex);
                    physicalInfoArea = row.PhysicalInfoAreaIdAtIndex(this.kpResultColumnIndex);
                }
            }

            if (!kpRow && this.fiResultColumnIndex != -1)
            {
                recordIdentification = row.RecordIdentificationAtIndex(this.fiResultColumnIndex);
                virtualInfoArea = row.VirtualInfoAreaIdAtIndex(this.fiResultColumnIndex);
                physicalInfoArea = row.PhysicalInfoAreaIdAtIndex(this.fiResultColumnIndex);
            }

            if (recordIdentification?.Length > 3)
            {
                virtualInfoArea = virtualInfoArea == physicalInfoArea ? null : virtualInfoArea;
                return CreateNodeFromResultRow(row, virtualInfoArea, recordIdentification, this.depth, false, this.expandChecker, physicalInfoArea, physicalInfoArea);
            }

            return null;
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
            UPMCoIEdge edge = CreateEdgeFromResultRow(row, this.PhysicalInfoArea(row), srcNode, dstNode);
            return edge;
        }

        /// <summary>
        /// Creates the root node from result row. Called from PageModeController
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="currentDepth">The current depth.</param>
        /// <param name="expandChecker">The expand checker.</param>
        /// <returns></returns>
        public static UPMCoINode CreateRootNodeFromResultRow(UPCRMResultRow row, string recordIdentification, int currentDepth, UPConfigExpand expandChecker)
        {
            string virtualInfoArea = row.RootVirtualInfoAreaId;
            string infoAreaId = row.RootRecordIdentification.InfoAreaId();
            virtualInfoArea = infoAreaId == virtualInfoArea ? null : infoAreaId;
            return CreateNodeFromResultRow(row, virtualInfoArea, recordIdentification, 0, true, expandChecker, null, infoAreaId);
        }

        protected override void CreateSubNodeLoadForNodeEdgeRow(UPMCoINode node, UPMCoIEdge edge, UPCRMResultRow row)
        {
            this.vistedNodes.SetObjectForKey(node, node.Identifier);
            this.RootNode.AddChildNodeChildRelation(node, edge);
            this.ChangedIdentifiers.Add(node.Identifier);
            if (this.depth < this.maxDepth)
            {
                this.pendingCount++;
                CoINodeLoader loader = new CoINodeLoader(node, this.viewReference, this.depth + 1, -1, this.vistedNodes);
                loader.Mode = CoINodeLoaderMode.SubNodeLoader;
                loader.fiResultColumnIndex = this.fiResultColumnIndex;
                loader.kpResultColumnIndex = this.kpResultColumnIndex;
                this.subNodeLoader.Add(loader);
                loader.TheDelegate = this;
            }
        }

        private string PhysicalInfoArea(UPCRMResultRow row)
        {
            if (this.fiResultColumnIndex == -1)
            {
                this.fiResultColumnIndex = row.Result.MetaInfo.IndexOfResultInfoAreaIdLinkId("FI", 1);
            }

            if (this.kpResultColumnIndex == -1)
            {
                this.kpResultColumnIndex = row.Result.MetaInfo.IndexOfResultInfoAreaIdLinkId("KP", 1);
            }

            bool kpRow = false;
            string physicalInfoArea = null;
            if (this.kpResultColumnIndex != -1)
            {
                string recordIdentification = row.RecordIdentificationAtIndex(this.kpResultColumnIndex);
                if (recordIdentification?.Length > 3)
                {
                    kpRow = true;
                    physicalInfoArea = row.PhysicalInfoAreaIdAtIndex(this.kpResultColumnIndex);
                }
            }

            if (!kpRow && this.fiResultColumnIndex != -1)
            {
                physicalInfoArea = row.PhysicalInfoAreaIdAtIndex(this.fiResultColumnIndex);
            }

            return physicalInfoArea;
        }
    }
}
