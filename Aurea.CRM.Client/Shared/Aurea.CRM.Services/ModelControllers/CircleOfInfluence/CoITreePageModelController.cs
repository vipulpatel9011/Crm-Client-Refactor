// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoITreePageModelController.cs" company="Aurea Software Gmbh">
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
//   The CoITreePageModelController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.CircleOfInfluence
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel.CircleOfInfluence;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// CoITreePageModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.CircleOfInfluence.UPCoIBasePageModelController" />
    public class CoITreePageModelController : UPCoIBasePageModelController
    {
        private UPConfigTreeView rootTreeNode;
        private FieldControl rootNodeFieldControl;
        private UPConfigExpand expandChecker;
        private Dictionary<IIdentifier, UPMCoINode> vistedNodes;
        private Dictionary<IIdentifier, UPConfigTreeViewTable> nodeIdConfigDict;
        private List<CoITreeNodeLoader> pendingNodeLoader;
        private Dictionary<string, CoITreeInfoAreaConfig> recordIdentifierInfoAreaConfigMapping;
        private int pendingNodeLoaderCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoITreePageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public CoITreePageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.pendingNodeLoader = new List<CoITreeNodeLoader>();
            this.recordIdentifierInfoAreaConfigMapping = new Dictionary<string, CoITreeInfoAreaConfig>();
            this.BuildPage();
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        protected override void BuildPage()
        {
            base.BuildPage();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string rootTreeNodeName = this.ViewReference.ContextValueForKey("ConfigName");
            if (!string.IsNullOrEmpty(rootTreeNodeName))
            {
                this.rootTreeNode = configStore.TreeViewByName(rootTreeNodeName);
            }

            UPMCircleOfInfluencePage page = this.CreatePageInstance();
            this.TopLevelElement = page;
            if (this.rootTreeNode == null)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError($"No treeView found for configName: {rootTreeNodeName}");
                return;
            }

            // Root FieldGroup
            string rootNodeExpandName = this.rootTreeNode.RootNode.ExpandName;
            this.ExpandSettings = configStore.ExpandByName(rootNodeExpandName) ??
                                  configStore.ExpandByName(this.InfoAreaId);

            List<FieldControl> fieldControler = new List<FieldControl>();
            FieldControl details = configStore.FieldControlByNameFromGroup("Details", this.ExpandSettings.FieldGroupName);
            if (details != null)
            {
                fieldControler.Add(details);
            }

            FieldControl miniDetails = configStore.FieldControlByNameFromGroup("MiniDetails", this.ExpandSettings.FieldGroupName);
            if (miniDetails != null)
            {
                fieldControler.Add(miniDetails);
            }

            if (fieldControler.Count > 0)
            {
                this.rootNodeFieldControl = new FieldControl(fieldControler);
            }

            this.ApplyViewConfigOnPage(page);
            page.Invalid = true;
            this.ApplyLoadingStatusOnPage(page);
            this.TopLevelElement = page;
        }

        /// <summary>
        /// Loads the next depth for node.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void LoadNextDepthForNode(UPMCoINode node)
        {
            if (this.RunningDepthLoader.Contains(node.Identifier))
            {
                return;
            }

            UPConfigTreeViewTable config = this.nodeIdConfigDict[node.Identifier];
            bool newLoader = false;
            if (!node.ChildsLoaded && config.ChildNodes.Count > 0)
            {
                this.RunningDepthLoader.Add(node.Identifier);
                foreach (UPConfigTreeViewTable childTree in config.ChildNodes)
                {
                    if (string.IsNullOrEmpty(childTree.RecordCustomControl))
                    {
                        CoITreeNodeLoader loader = new CoITreeNodeLoader(node, childTree, this.ViewReference, 0, this.vistedNodes, this.nodeIdConfigDict, this.recordIdentifierInfoAreaConfigMapping);
                        loader.Mode = CoINodeLoaderMode.Reload;
                        loader.TheDelegate = this;
                        this.pendingNodeLoader.Add(loader);
                        this.pendingNodeLoaderCount++;
                        newLoader = true;
                    }
                    else
                    {
                        // Lade die subnode fields der infoAreaConfig
                        CoITreeInfoAreaConfig infoAreaConfig = this.recordIdentifierInfoAreaConfigMapping[((RecordIdentifier)node.Identifier).RecordIdentification];
                        if (childTree.LinkId == infoAreaConfig.LinkId && childTree.InfoAreaId == infoAreaConfig.InfoAreaId)
                        {
                            // Bei zwei Kinder mit InfoAreaConfig darf nicht zweimal eine Query auf PB gemacht werden
                            // Es darf nur die ausgefÃ¼hrt werden zu der diese Node passt
                            foreach (UPConfigTreeViewTable _childTree in infoAreaConfig.Config.ChildNodes)
                            {
                                if (string.IsNullOrEmpty(_childTree.RecordCustomControl))
                                {
                                    CoITreeNodeLoader loader = new CoITreeNodeLoader(node, _childTree, this.ViewReference, 0, this.vistedNodes, this.nodeIdConfigDict, this.recordIdentifierInfoAreaConfigMapping);
                                    loader.Mode = CoINodeLoaderMode.Reload;
                                    loader.TheDelegate = this;
                                    this.pendingNodeLoader.Add(loader);
                                    this.pendingNodeLoaderCount++;
                                    newLoader = true;
                                }
                            }
                        }
                    }
                }

                foreach (CoITreeNodeLoader loader in this.pendingNodeLoader)
                {
                    loader.LoadNodeSubNodes();
                }

                if (!newLoader)
                {
                    node.ChildsLoaded = true;
                    this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, new List<IIdentifier> { node.Identifier }, null);
                }
            }
            else
            {
                node.ChildsLoaded = true;
                this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, new List<IIdentifier> { node.Identifier }, null);
            }
        }

        /// <summary>
        /// Updates the page.
        /// </summary>
        /// <param name="_page">The page.</param>
        /// <returns></returns>
        protected override UPMCircleOfInfluencePage UpdatePage(UPMCircleOfInfluencePage _page)
        {
            if (this.rootNodeFieldControl == null)
            {
                this.InformAboutDidFailTopLevelElement(_page);
                return _page;
            }

            // Load Root Node
            if (_page.Invalid || _page.RootNode.Invalid)
            {
                UPContainerMetaInfo query = new UPContainerMetaInfo(this.rootNodeFieldControl);
                if (this.ExpandSettings != null)
                {
                    Dictionary<string, UPCRMField> alternateExpandFields = this.ExpandSettings.FieldsForAlternateExpands(true);
                    List<UPCRMField> additionalFields = alternateExpandFields?.Values?.Where(field => query.ContainsField(field) == null).ToList();

                    if (additionalFields?.Count > 0)
                    {
                        query.AddCrmFields(additionalFields);
                    }

                    this.expandChecker = this.ExpandSettings.ExpandCheckerForCrmQuery(query);
                }

                Operation operation = query.ReadRecord(this.RootRecordIdentification, this.RequestOption, this);
                if (operation == null && this.RequestOption == UPRequestOption.Online)
                {
                    // Offline
                    UPMWatermarkStatus offlineStatus = UPMWatermarkStatus.WatermarkStatus();
                    _page.Status = offlineStatus;
                    this.InformAboutDidChangeTopLevelElement(_page, _page, null, null);
                }
            }

            return _page;
        }

        private void FillPageWithResult(UPCRMResult result)
        {
            this.vistedNodes = new Dictionary<IIdentifier, UPMCoINode>();
            this.nodeIdConfigDict = new Dictionary<IIdentifier, UPConfigTreeViewTable>();

            // Root Node Query
            if (result.RowCount == 1)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
                UPMCoINode rootNode = CoITreeNodeLoader.CreateNodeFromResultRow(row, 0, this.expandChecker, null, null);
                this.vistedNodes[rootNode.Identifier] = rootNode;
                this.nodeIdConfigDict[rootNode.Identifier] = this.rootTreeNode.RootNode;

                // Only root configured
                if (this.rootTreeNode.RootNode.ChildNodes.Count == 0)
                {
                    UPMCircleOfInfluencePage newPage = this.CreatePageInstance();
                    newPage.RootNode = rootNode;
                    newPage.Invalid = false;
                    newPage.ConfigProvider = ((UPMCircleOfInfluencePage)this.Page).ConfigProvider;
                    UPMCircleOfInfluencePage oldPage = (UPMCircleOfInfluencePage)this.Page;
                    this.TopLevelElement = newPage;
                    this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
                }

                foreach (UPConfigTreeViewTable childTree in this.rootTreeNode.RootNode.ChildNodes)
                {
                    CoITreeNodeLoader nodeLoader = new CoITreeNodeLoader(rootNode, childTree, this.ViewReference, 0, this.vistedNodes, this.nodeIdConfigDict, this.recordIdentifierInfoAreaConfigMapping);
                    nodeLoader.TheDelegate = this;
                    nodeLoader.Mode = CoINodeLoaderMode.InitialLoad;
                    this.pendingNodeLoader.Add(nodeLoader);
                    this.pendingNodeLoaderCount++;
                }
                // TODO: This is buggy and will be implemented in CRM-5621
                /*
                foreach (CoITreeNodeLoader _loader in this.pendingNodeLoader)
                {
                    _loader.LoadNodeSubNodes();
                }
                */
            }
            else
            {
                // Request Mode Offline and jump To Online Node
                if (result.RowCount == 0 && this.RequestOption == UPRequestOption.Offline)
                {
                    // Offline Configured and Online Record
                    UPMCircleOfInfluencePage newPage = this.CreatePageInstance();
                    newPage.Status = UPMMessageStatus.MessageStatusWithMessageDetails(string.Empty, LocalizedString.TextOfflineNotAvailable);
                    UPMCircleOfInfluencePage oldPage = (UPMCircleOfInfluencePage)this.Page;
                    this.TopLevelElement = newPage;
                    this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
                }
            }
        }

        /// <summary>
        /// Switches to detail.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public override void SwitchToDetail(IIdentifier identifier)
        {
            bool switchToCoi = this.ViewReference.ContextValueForKey("SwitchToCoi") == "true";
            if (identifier is RecordIdentifier)
            {
                string recordIdentification = ((RecordIdentifier)identifier).RecordIdentification;
                CoITreeInfoAreaConfig infoAreaConfig = this.recordIdentifierInfoAreaConfigMapping[recordIdentification];
                string editAction = infoAreaConfig.Definition.ValueOrDefault("NodeDetailAction") as string;
                editAction = !string.IsNullOrEmpty(editAction) ? editAction : "SHOWRECORD";
                this.SwitchToRecord(recordIdentification, editAction, true, false, switchToCoi ? new SwitchToFirstCoiIndex() : null);
            }
            else
            {
                // Switch to SearchAndList
                string searchAndListName = this.nodeIdConfigDict[identifier].SearchAndListName;
                if (!string.IsNullOrEmpty(searchAndListName))
                {
                    IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                    SearchAndList searchAndList = configStore.SearchAndListByName(searchAndListName);
                    if (searchAndList != null)
                    {
                        Dictionary<string, object> dictionary = new Dictionary<string, object>();
                        dictionary["ConfigName"] = searchAndListName;
                        dictionary["InfoArea"] = searchAndList.InfoAreaId;
                        UPMCoINode node = this.vistedNodes[identifier];
                        UPConfigTreeViewTable parentConfigTable = this.nodeIdConfigDict[node.Parent.Identifier];
                        if (!string.IsNullOrEmpty(node.Parent.RecordIdentification))
                        {
                            dictionary["LinkRecord"] = node.Parent.RecordIdentification;
                        }

                        if (parentConfigTable.LinkId > 0)
                        {
                            dictionary["LinkId"] = $"{parentConfigTable.LinkId}";
                        }

                        dictionary[Search.Constants.SwipeDetailRecordsConfigName] = "true";
                        string initialRquestOptionString = UPCRMDataStore.StringFromRequestOption(this.RequestOption);
                        if (!string.IsNullOrEmpty(initialRquestOptionString))
                        {
                            dictionary["InitialRequestOption"] = initialRquestOptionString;
                        }

                        UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(new ViewReference(dictionary, "RecordListView"));
                        this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
                    }
                }
            }
        }

        /// <summary>
        /// Switches to edit.
        /// </summary>
        /// <param name="edge">The edge.</param>
        public override void SwitchToEdit(UPMCoIEdge edge)
        {
            string edgeRecordIdentification = ((RecordIdentifier)edge.Identifier).RecordIdentification;
            string trgtNodeRecordIdentification = ((RecordIdentifier)edge.TargetNodeIdentifier).RecordIdentification;
            CoITreeInfoAreaConfig infoAreaConfig = this.recordIdentifierInfoAreaConfigMapping[trgtNodeRecordIdentification];
            string editAction = infoAreaConfig.Definition.ValueOrDefault("EdgeEditAction") as string;
            editAction = !string.IsNullOrEmpty(editAction) ? editAction : "EDITRECORD";
            this.SwitchToRecord(edgeRecordIdentification, editAction, false, true, null);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.FillPageWithResult(result);
        }

        /// <summary>
        /// Nodeses the loaded succesfull.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        public override void NodesLoadedSuccesfull(CoIBaseNodeLoader nodeloader)
        {
            this.pendingNodeLoaderCount--;
            if (this.pendingNodeLoaderCount == 0)
            {
                this.pendingNodeLoader.Clear();
                if (nodeloader.Mode == CoINodeLoaderMode.InitialLoad)
                {
                    UPMCircleOfInfluencePage newPage = this.CreatePageInstance();
                    newPage.RootNode = nodeloader.RootNode;
                    newPage.Invalid = false;
                    newPage.ConfigProvider = ((UPMCircleOfInfluencePage)this.Page).ConfigProvider;
                    UPMCircleOfInfluencePage oldPage = (UPMCircleOfInfluencePage)this.Page;
                    this.TopLevelElement = newPage;
                    if (!oldPage.Invalid && oldPage.RootNode.Invalid)
                    {
                        // return from edir
                        newPage.RefreshedNode = newPage.RootNode;
                        this.InformAboutDidChangeTopLevelElement(oldPage, newPage, new List<IIdentifier> { oldPage.RootNode.Identifier }, null);
                    }
                    else
                    {
                        this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
                    }
                }
                else if (nodeloader.Mode == CoINodeLoaderMode.Reload)
                {
                    this.RunningDepthLoader.Remove(nodeloader.RootNode.Identifier);
                    this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, nodeloader.ChangedIdentifiers, null);
                }
            }
        }
    }
}
