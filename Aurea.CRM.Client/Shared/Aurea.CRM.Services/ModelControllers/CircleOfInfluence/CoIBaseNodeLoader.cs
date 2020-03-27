// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoIBaseNodeLoader.cs" company="Aurea Software Gmbh">
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
//   The CoIBaseNodeLoader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.CircleOfInfluence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.CircleOfInfluence;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// CoINodeLoaderMode
    /// </summary>
    public enum CoINodeLoaderMode
    {
        /// <summary>
        /// Initial load
        /// </summary>
        InitialLoad,

        /// <summary>
        /// Sub node loader
        /// </summary>
        SubNodeLoader,

        /// <summary>
        /// Reload
        /// </summary>
        Reload
    }

    /// <summary>
    /// CoIBaseNodeLoader
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="INodeLoaderDelegate" />
    public class CoIBaseNodeLoader : ISearchOperationHandler, INodeLoaderDelegate
    {
        protected UPConfigExpand expandSettings;
        private Operation currentOperation;

        protected int depth;
        protected int maxDepth;
        protected UPRequestOption requestOption;
        protected Dictionary<IIdentifier, UPMCoINode> vistedNodes;
        protected ViewReference viewReference;
        protected List<CoIBaseNodeLoader> subNodeLoader;
        protected UPConfigExpand expandChecker;
        protected int pendingCount;
        protected string configName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoIBaseNodeLoader"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="maxDepth">The maximum depth.</param>
        /// <param name="vistedNodes">The visted nodes.</param>
        public CoIBaseNodeLoader(UPMCoINode node, ViewReference viewReference, int depth, int maxDepth, Dictionary<IIdentifier, UPMCoINode> vistedNodes)
        {
            this.RootNode = node;
            this.viewReference = viewReference;
            string requestOptionString = this.viewReference.ContextValueForKey("RequestOption");
            this.requestOption = UPCRMDataStore.RequestOptionFromString(requestOptionString, UPRequestOption.Offline);
            this.depth = depth;
            this.maxDepth = maxDepth;
            this.vistedNodes = vistedNodes;
            this.subNodeLoader = new List<CoIBaseNodeLoader>();
            this.ChangedIdentifiers = new List<IIdentifier>();
            this.pendingCount = 0;
            this.configName = this.viewReference.ContextValueForKey("ConfigName");
            this.ChangedIdentifiers.Add(this.RootNode.Identifier);
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.expandSettings = configStore.ExpandByName(this.configName);
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public CoINodeLoaderMode Mode { get; set; }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <value>
        /// The root node.
        /// </value>
        public UPMCoINode RootNode { get; private set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public INodeLoaderDelegate TheDelegate { get; set; }

        /// <summary>
        /// Gets the changed identifiers.
        /// </summary>
        /// <value>
        /// The changed identifiers.
        /// </value>
        public List<IIdentifier> ChangedIdentifiers { get; }

        /// <summary>
        /// Loads the node sub nodes.
        /// </summary>
        public virtual void LoadNodeSubNodes()
        {
            UPContainerMetaInfo query = this.CreateQuery(((RecordIdentifier)this.RootNode.Identifier).RecordIdentification);
            if (query != null)
            {
                query.Find(this.requestOption, this);
            }
            else
            {
                this.TheDelegate.NodesLoadFailedError(this, null);
            }
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="recordIdentifier">The record identifier.</param>
        /// <returns></returns>
        protected virtual UPContainerMetaInfo CreateQuery(string recordIdentifier)
        {
            // Override
            return null;
        }

        /// <summary>
        /// Creates the query link identifier search and list.
        /// </summary>
        /// <param name="recordIdentifier">The record identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="searchAndList">The search and list.</param>
        /// <returns></returns>
        protected UPContainerMetaInfo CreateQueryLinkIdSearchAndList(string recordIdentifier, int linkId, SearchAndList searchAndList)
        {
            if (searchAndList == null)
            {
                return null;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (this.expandSettings == null)
            {
                this.expandSettings = configStore.ExpandByName(searchAndList.InfoAreaId);
            }

            FieldControl fieldControlList = configStore.FieldControlByNameFromGroup("List", searchAndList.FieldGroupName);
            FieldControl fieldControlMini = configStore.FieldControlByNameFromGroup("MiniDetails", searchAndList.FieldGroupName);
            FieldControl fieldControlCombi = new FieldControl(new List<FieldControl> { fieldControlList, fieldControlMini });
            UPContainerMetaInfo query = new UPContainerMetaInfo(fieldControlCombi);
            query.SetLinkRecordIdentification(recordIdentifier, linkId);
            if (this.expandSettings != null)
            {
                Dictionary<string, UPCRMField> alternateExpandFields = this.expandSettings.FieldsForAlternateExpands(true);
                List<UPCRMField> additionalFields = alternateExpandFields.Values.Where(field => query.ContainsField(field) == null).ToList();

                if (additionalFields.Count > 0)
                {
                    query.AddCrmFields(additionalFields);
                }

                this.expandChecker = this.expandSettings.ExpandCheckerForCrmQuery(query);
            }

            return query;
        }

        /// <summary>
        /// Creates the node from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="virtualInfoArea">The virtual information area.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="currentDepth">The current depth.</param>
        /// <param name="rootNode">if set to <c>true</c> [root node].</param>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="infoAreaSuffix">The information area suffix.</param>
        /// <param name="infoAreaConfigId">The information area configuration identifier.</param>
        /// <returns></returns>
        protected static UPMCoINode CreateNodeFromResultRow(UPCRMResultRow row, string virtualInfoArea, string recordIdentification, int currentDepth, bool rootNode, UPConfigExpand expandChecker, string infoAreaSuffix, string infoAreaConfigId)
        {
            string nodeInfoAreaSuffix = infoAreaSuffix ?? string.Empty;
            string functionName = $"trgtNodeField{nodeInfoAreaSuffix}";
            string mainFieldFunctionName = rootNode ? "mainField" : $"mainField{nodeInfoAreaSuffix}";
            string subFieldFunctionName = rootNode ? "subField" : $"subField{nodeInfoAreaSuffix}";
            string initial1FunctionName = rootNode ? "initial1" : $"initial1{nodeInfoAreaSuffix}";
            string initial2FunctionName = rootNode ? "initial2" : $"initial2{nodeInfoAreaSuffix}";
            string initialFunctionName = rootNode ? "initialField" : $"initialField{nodeInfoAreaSuffix}";
            UPMCoINode node = new UPMCoINode(new RecordIdentifier(recordIdentification));
            AddImageToNode(node, infoAreaConfigId, virtualInfoArea, expandChecker, row);

            // Add Fields
            node.Fields = FieldsForResultRow(row, rootNode ? null : functionName, 0, !rootNode);
            List<UPMStringField> mainFields = FieldsForResultRow(row, mainFieldFunctionName, 1, true);
            List<UPMStringField> subFields = FieldsForResultRow(row, subFieldFunctionName, 1, true);
            List<UPMStringField> intial1Fields = FieldsForResultRow(row, initial1FunctionName, 0, true);
            UPMStringField initial1Field = null;
            if (intial1Fields.Count > 0)
            {
                initial1Field = intial1Fields[0];
            }

            List<UPMStringField> intial2Fields = FieldsForResultRow(row, initial2FunctionName, 0, true);
            UPMStringField initial2Field = null;
            if (intial2Fields.Count > 0)
            {
                initial2Field = intial2Fields[0];
            }

            List<UPMStringField> intialFields = FieldsForResultRow(row, initialFunctionName, 0, true);
            UPMStringField initialField = null;
            if (intialFields.Count > 0)
            {
                initialField = intialFields[0];
            }

            if (initialField != null)
            {
                node.InitialField = initialField;
            }
            else if (initial1Field != null && initial2Field != null)
            {
                string string1 = initial1Field.StringValue?.Length > 0 ? initial1Field.StringValue.Substring(0, 1) : string.Empty;
                string string2 = initial2Field.StringValue?.Length > 0 ? initial2Field.StringValue.Substring(0, 1) : string.Empty;
                initial1Field.StringValue = $"{string1}{string2}".ToUpper();
                node.InitialField = initial1Field;
            }
            else if (initial2Field != null)
            {
                initial2Field.StringValue = initial2Field.StringValue?.Length > 0 ? initial2Field.StringValue.Substring(0, 1) : string.Empty;
                node.InitialField = initial2Field;
            }
            else if (initial1Field != null)
            {
                initial1Field.StringValue = initial1Field.StringValue?.Length > 0 ? initial1Field.StringValue.Substring(0, 1) : string.Empty;
                node.InitialField = initial1Field;
            }

            List<UPMStringField> fields = new List<UPMStringField>(mainFields);
            fields.AddRange(subFields);
            if (fields.Count > 0)
            {
                node.InfoFields = fields;
            }

            return node;
        }

        /// <summary>
        /// Creates the edge from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="infoAreaSuffix">The information area suffix.</param>
        /// <param name="srcNode">The source node.</param>
        /// <param name="dstNode">The DST node.</param>
        /// <returns></returns>
        public static UPMCoIEdge CreateEdgeFromResultRow(UPCRMResultRow row, string infoAreaSuffix, UPMCoINode srcNode, UPMCoINode dstNode)
        {
            string nodeInfoAreaSuffix = infoAreaSuffix ?? string.Empty;
            string listFieldFunctionName = $"listField{nodeInfoAreaSuffix}";
            UPMCoIEdge edge = new UPMCoIEdge(new RecordIdentifier(row.RootRecordIdentification))
            {
                TargetNodeIdentifier = dstNode.Identifier,
                SrcNodeIdentifier = srcNode.Identifier
            };

            List<UPMStringField> listFields = FieldsForResultRow(row, listFieldFunctionName, 1, true);
            if (listFields.Count > 0)
            {
                edge.ListFields = listFields;
            }

            return edge;
        }

        /// <summary>
        /// Creates the edge from result row source node DST node.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="srcNode">The source node.</param>
        /// <param name="dstNode">The DST node.</param>
        /// <returns></returns>
        protected virtual UPMCoIEdge CreateEdgeFromResultRow(UPCRMResultRow row, UPMCoINode srcNode, UPMCoINode dstNode)
        {
            // Override
            return null;
        }

        /// <summary>
        /// Creates the node from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected virtual UPMCoINode CreateNodeFromResultRow(UPCRMResultRow row)
        {
            // Override
            return null;
        }

        private void AddNodesFromResult(UPCRMResult result)
        {
            for (int index = 0; index < result.RowCount; index++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(index);
                UPMCoINode relationNode = this.CreateNodeFromResultRow(row);
                if (relationNode != null)
                {
                    relationNode.OnlineData = !row.HasLocalCopy;
                    relationNode.Parent = this.RootNode;
                    UPMCoIEdge edge = this.CreateEdgeFromResultRow(row, this.RootNode, relationNode);
                    List<UPMStringField> relationNameList = FieldsForResultRow(row, "relationName", 0, true);
                    List<UPMStringField> relationIntensityList = FieldsForResultRow(row, "relationIntensity", 0, true);

                    // Only one relatioName ist supported
                    if (relationNameList.Count > 0)
                    {
                        edge.RelationName = relationNameList[0];
                    }

                    if (relationIntensityList.Count > 0)
                    {
                        UPMStringField field = relationIntensityList[0];
                        if (!string.IsNullOrEmpty(field.StringValue))
                        {
                            edge.RelationIntensity = Convert.ToInt32(field.StringValue);
                        }
                    }

                    // Node allready added?
                    UPMCoINode vistedNode = this.vistedNodes.ValueOrDefault(relationNode.Identifier);
                    bool additionalRealtion = false;
                    if (vistedNode != null)
                    {
                        this.RootNode.AddAllRelationToRelation(vistedNode, edge);

                        // handle double nodes
                        // Bedingung
                        if (relationNode.Parent.Identifier.MatchesIdentifier(vistedNode.Parent?.Identifier) 
                            || this.RootNode?.Parent?.Identifier?.MatchesIdentifier(vistedNode.Identifier) == true)
                        {
                            // Same Relation ignore
                            continue;
                        }

                        if (vistedNode.Parent == null)
                        {
                            // No Addtional Relations or Back relations to root node
                            continue;
                        }

                        if (relationNode.Parent.Identifier.MatchesIdentifier(relationNode.Identifier))
                        {
                            // Self relation ignore
                            continue;
                        }

                        additionalRealtion = true;

                        // Check Additional Relation allready Added?
                        for (int index2 = 0; index2 < this.RootNode.AdditionalRelationsCount; index2++)
                        {
                            UPMCoINode addRelationNode = this.RootNode.AdditionalRelationNodeAtIndex(index2);
                            if (addRelationNode.Identifier.MatchesIdentifier(vistedNode.Identifier))
                            {
                                additionalRealtion = false;
                            }
                        }

                        if (!additionalRealtion)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        this.RootNode.AddAllRelationToRelation(relationNode, edge);
                    }

                    if (additionalRealtion)
                    {
                        this.RootNode.AddAdditionalRelationToRelation(vistedNode, edge);
                        this.ChangedIdentifiers.Add(this.RootNode.Identifier);
                    }
                    else
                    {
                        this.CreateSubNodeLoadForNodeEdgeRow(relationNode, edge, row);
                    }
                }
            }

            if (this.subNodeLoader.Count == 0)
            {
                this.ChangedIdentifiers.Add(this.RootNode.Identifier);
                this.TheDelegate.NodesLoadedSuccesfull(this);
            }

            foreach (CoIBaseNodeLoader loaderItem in this.subNodeLoader)
            {
                loaderItem.LoadNodeSubNodes();
            }
        }

        /// <summary>
        /// Creates the sub node load for node edge row.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="edge">The edge.</param>
        /// <param name="row">The row.</param>
        protected virtual void CreateSubNodeLoadForNodeEdgeRow(UPMCoINode node, UPMCoIEdge edge, UPCRMResultRow row)
        {
            // Override
        }

        // functionName == nil => all Fields
        private static List<UPMStringField> FieldsForResultRow(UPCRMResultRow row, string functionName, int tabIndex, bool addHidden)
        {
            List<UPMStringField> fields = new List<UPMStringField>();
            FieldControlTab sourceFieldControl = row.Result.MetaInfo.SourceFieldControl.TabAtIndex(tabIndex);
            int offset = tabIndex == 0 ? 0 : row.Result.MetaInfo.SourceFieldControl.TabAtIndex(0).Fields.Count;
            int fieldCount = tabIndex == 0 ? sourceFieldControl.Fields.Count : row.NumberOfColumns;
            for (int rowFieldIndex = 0; rowFieldIndex < fieldCount;)
            {
                string fieldFunctionNames = sourceFieldControl.FieldAtIndex(rowFieldIndex)?.Function ?? string.Empty;
                var functionNames = fieldFunctionNames.Split(',').ToList();
                functionNames = functionNames.Count == 0 ? new List<string> { string.Empty } : functionNames;
                UPConfigFieldControlField configField = sourceFieldControl.FieldAtIndex(rowFieldIndex);
                FieldAttributes attributes = configField?.Attributes;
                bool found = false;
                foreach (string fieldFunctionName in functionNames)
                {
                    if (functionName == null || fieldFunctionName.StartsWith(functionName))
                    {
                        UPMStringField stringField = new UPMStringField(new FieldIdentifier(row.RecordIdentificationAtIndex(0), configField.Field.FieldIdentification));
                        stringField.Hidden = attributes.Hide;
                        if (attributes.FieldCount > 0)
                        {
                            List<string> combineFields = new List<string>();
                            for (int fieldIndex = 0; fieldIndex < attributes.FieldCount; fieldIndex++)
                            {
                                combineFields.Add(row.ValueAtIndex(rowFieldIndex + offset));
                                rowFieldIndex++;
                            }

                            stringField.StringValue = attributes.FormatValues(combineFields);
                        }
                        else
                        {
                            stringField.StringValue = row.ValueAtIndex(rowFieldIndex + offset);
                            rowFieldIndex++;
                        }

                        if (addHidden || !stringField.Hidden)
                        {
                            fields.Add(stringField);
                        }

                        found = true;
                    }
                }

                if (!found)
                {
                    rowFieldIndex++;
                }
            }

            return fields;
        }

        private static void AddImageToNode(UPMCoINode node, string infoArea, string virtualInfoArea, UPConfigExpand expandChecker, UPCRMResultRow row)
        {
            // Add Image
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigExpand expand = null;
            string imageName = null;
            if (!string.IsNullOrEmpty(virtualInfoArea))
            {
                expand = ConfigurationUnitStore.DefaultStore.ExpandByName(virtualInfoArea);
            }

            if (expand == null && expandChecker != null)
            {
                expand = expandChecker.ExpandForResultRow(row);
            }

            if (expand != null)
            {
                imageName = expand.ImageName;
            }

            if (string.IsNullOrEmpty(imageName))
            {
                InfoArea infoAreaConfig = configStore.InfoAreaConfigById(infoArea);
                imageName = infoAreaConfig.ImageName;
            }

            if (!string.IsNullOrEmpty(imageName))
            {
                //node.Icon = UIImage.UpImageWithFileName(imageName);   // CRM-5007
            }
        }

        /// <summary>
        /// Cancels the node loader.
        /// </summary>
        public void CancelNodeLoader()
        {
            this.currentOperation.Cancel();
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate.NodesLoadFailedError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.RootNode.ChildsLoaded = true;
            this.AddNodesFromResult(result);
        }

        /// <summary>
        /// Nodes loaded succesfull.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        public virtual void NodesLoadedSuccesfull(CoIBaseNodeLoader nodeloader)
        {
            this.pendingCount--;
            this.subNodeLoader.Remove(nodeloader);
            if (this.pendingCount == 0)
            {
                this.TheDelegate.NodesLoadedSuccesfull(this);
            }
        }

        /// <summary>
        /// Nodes load failed error.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        /// <param name="error">The error.</param>
        public virtual void NodesLoadFailedError(CoIBaseNodeLoader nodeloader, Exception error)
        {
            this.subNodeLoader.Remove(nodeloader);
            foreach (CoIBaseNodeLoader subnodeLoader in this.subNodeLoader)
            {
                subnodeLoader.CancelNodeLoader();
            }

            this.TheDelegate.NodesLoadFailedError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
