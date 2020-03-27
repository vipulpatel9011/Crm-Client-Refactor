// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoIPageModelController.cs" company="Aurea Software Gmbh">
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
//   The CoIPageModelController.
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
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.UIModel.CircleOfInfluence;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// CoIPageModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.CircleOfInfluence.UPCoIBasePageModelController" />
    public class CoIPageModelController : UPCoIBasePageModelController
    {
        private string fiFilterName;
        private string kpFilterName;
        private Dictionary<IIdentifier, UPMCoINode> vistedNodes;
        private CoINodeLoader currentNodeLoader;
        private FieldControl rootNodeFieldControl;
        private UPConfigExpand expandChecker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoIPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public CoIPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.MaxDepth = 2;
            this.vistedNodes = new Dictionary<IIdentifier, UPMCoINode>();
            if(viewReference != null)
            this.BuildPage();
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        protected override void BuildPage()
        {
            base.BuildPage();
            this.ConfigName = this.ViewReference.ContextValueForKey("RootNodeConfigName");
            UPMCircleOfInfluencePage page = this.CreatePageInstance();
            string rootNodeFieldGroup = null;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (!string.IsNullOrEmpty(this.ConfigName))
            {
                SearchAndList searchAndList = configStore.SearchAndListByName(this.ConfigName);
                rootNodeFieldGroup = searchAndList.FieldGroupName;
            }

            if (string.IsNullOrEmpty(rootNodeFieldGroup))
            {
                rootNodeFieldGroup = this.ViewReference.ContextValueForKey("RootNodeFieldGroup");
            }

            FieldControl details = configStore.FieldControlByNameFromGroup("Details", rootNodeFieldGroup);
            FieldControl miniDetails = configStore.FieldControlByNameFromGroup("MiniDetails", rootNodeFieldGroup);
            this.rootNodeFieldControl = new FieldControl(new List<FieldControl> { details, miniDetails });
            this.ApplyViewConfigOnPage(page);
            this.ApplyLoadingStatusOnPage(page);
            page.Invalid = true;
            this.TopLevelElement = page;
        }

        /// <summary>
        /// Updates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected override UPMCircleOfInfluencePage UpdatePage(UPMCircleOfInfluencePage page)
        {
            if (this.rootNodeFieldControl == null)
            {
                this.InformAboutDidFailTopLevelElement(page);
                return page;
            }

            // Load Root Node
            if (page.Invalid || page.RootNode.Invalid)
            {
                UPContainerMetaInfo query = new UPContainerMetaInfo(this.rootNodeFieldControl);
                if (this.ExpandSettings != null)
                {
                    Dictionary<string, UPCRMField> alternateExpandFields = this.ExpandSettings.FieldsForAlternateExpands(true);
                    List<UPCRMField> additionalFields = alternateExpandFields?.Values.Where(field => query.ContainsField(field) == null).ToList();

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
                    page.Status = offlineStatus;
                    this.InformAboutDidChangeTopLevelElement(page, page, null, null);
                }
            }

            return page;
        }

        private void FillPageWithResult(UPCRMResult result)
        {
            this.vistedNodes = new Dictionary<IIdentifier, UPMCoINode>();
            if (result.RowCount == 1)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
                UPMCoINode rootNode = CoINodeLoader.CreateRootNodeFromResultRow(row, this.RootRecordIdentification, 0, this.expandChecker);
                this.vistedNodes[rootNode.Identifier] = rootNode;
                if (this.MaxDepth > 0)
                {
                    this.currentNodeLoader = new CoINodeLoader(rootNode, this.ViewReference, 0, this.MaxDepth, this.vistedNodes);
                    this.currentNodeLoader.TheDelegate = this;
                    this.currentNodeLoader.Mode = CoINodeLoaderMode.InitialLoad;
                    this.currentNodeLoader.LoadNodeSubNodes();
                }
                else
                {
                    // Finished loading
                    UPMCircleOfInfluencePage newPage = this.CreatePageInstance();
                    newPage.RootNode = rootNode;
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
        /// Loads the next depth for node.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void LoadNextDepthForNode(UPMCoINode node)
        {
            this.currentNodeLoader = new CoINodeLoader(node, this.ViewReference, 0, 0, this.vistedNodes);
            this.currentNodeLoader.TheDelegate = this;
            this.currentNodeLoader.Mode = CoINodeLoaderMode.Reload;
            this.currentNodeLoader.LoadNodeSubNodes();
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
            if (nodeloader.Mode == CoINodeLoaderMode.InitialLoad)
            {
                UPMCircleOfInfluencePage newPage = this.CreatePageInstance();
                newPage.RootNode = this.currentNodeLoader.RootNode;
                newPage.Invalid = false;
                newPage.ConfigProvider = ((UPMCircleOfInfluencePage)this.Page).ConfigProvider;
                UPMCircleOfInfluencePage oldPage = (UPMCircleOfInfluencePage)this.Page;
                this.TopLevelElement = newPage;
                if (!oldPage.Invalid && oldPage.RootNode.Invalid)
                {
                    // return from edit
                    newPage.RefreshedNode = newPage.RootNode;
                    this.InformAboutDidChangeTopLevelElement(oldPage, newPage, new List<IIdentifier> { oldPage.RootNode.Identifier }, null);
                }
                else
                {
                    this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
                }
            }
            else if (this.currentNodeLoader.Mode == CoINodeLoaderMode.Reload)
            {
                this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, this.currentNodeLoader.ChangedIdentifiers, null);
            }

            this.currentNodeLoader = null;
        }
    }
}
