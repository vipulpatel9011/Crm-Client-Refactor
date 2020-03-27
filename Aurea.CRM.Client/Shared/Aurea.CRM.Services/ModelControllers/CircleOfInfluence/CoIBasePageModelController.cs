// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoIBasePageModelController.cs" company="Aurea Software Gmbh">
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
//   The UPCoIBasePageModelController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.CircleOfInfluence
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.CircleOfInfluence;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// UPCoIBasePageModelController
    /// </summary>
    /// <seealso cref="UPPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="INodeLoaderDelegate" />
    public class UPCoIBasePageModelController : UPPageModelController, INodeLoaderDelegate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCoIBasePageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPCoIBasePageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.RunningDepthLoader = new List<IIdentifier>();
        }

        /// <summary>
        /// The root record identification
        /// </summary>
        protected string RootRecordIdentification;

        /// <summary>
        /// The request option
        /// </summary>
        protected UPRequestOption RequestOption;

        /// <summary>
        /// The maximum depth
        /// </summary>
        protected int MaxDepth;

        /// <summary>
        /// The expand settings
        /// </summary>
        protected UPConfigExpand ExpandSettings;

        /// <summary>
        /// The running depth loader
        /// </summary>
        protected List<IIdentifier> RunningDepthLoader;

        /// <summary>
        /// Builds the page.
        /// </summary>
        protected virtual void BuildPage()
        {
            string recordId = this.ViewReference.ContextValueForKey("recordId");
            if (string.IsNullOrEmpty(recordId))
            {
                recordId = this.ViewReference.ContextValueForKey("RecordId");
            }

            this.RootRecordIdentification = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(recordId);
            this.InfoAreaId = this.RootRecordIdentification.InfoAreaId();
            string requestOptionString = this.ViewReference.ContextValueForKey("RequestOption");
            this.RequestOption = UPCRMDataStore.RequestOptionFromString(requestOptionString, UPRequestOption.Offline);
            this.MaxDepth = Convert.ToInt32(this.ViewReference.ContextValueForKey("TreeMaxDepth"));
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.ExpandSettings = configStore.ExpandByName(this.ConfigName) ?? configStore.ExpandByName(this.InfoAreaId);
        }

        /// <summary>
        /// Applies the view configuration on page.
        /// </summary>
        /// <param name="page">The page.</param>
        protected void ApplyViewConfigOnPage(UPMCircleOfInfluencePage page)
        {
            string jsonNodesViewConfig = this.ViewReference.ContextValueForKey("nodesViewConfiguration");
            if (string.IsNullOrEmpty(jsonNodesViewConfig))
            {
                jsonNodesViewConfig = this.ViewReference.ContextValueForKey("NodesViewConfiguration");
            }

            page.ConfigProvider = new UPMCoIViewConfigProvider();
            page.ConfigProvider.ApplyJsonConfig(jsonNodesViewConfig);
        }

        /// <summary>
        /// Creates the page instance.
        /// </summary>
        /// <returns></returns>
        protected UPMCircleOfInfluencePage CreatePageInstance()
        {
            return new UPMCircleOfInfluencePage(new RecordIdentifier(this.RootRecordIdentification));
        }

        /// <summary>
        /// Gets the coi page.
        /// </summary>
        /// <value>
        /// The coi page.
        /// </value>
        public UPMCircleOfInfluencePage CoIPage => (UPMCircleOfInfluencePage)this.Page;

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            if (element is UPMCircleOfInfluencePage)
            {
                element.Invalid = true;
                return this.UpdatePage((UPMCircleOfInfluencePage)element);
            }

            return null;
        }

        /// <summary>
        /// Loads the next depth for node.
        /// </summary>
        /// <param name="node">The node.</param>
        protected virtual void LoadNextDepthForNode(UPMCoINode node)
        {
            // Override
        }

        /// <summary>
        /// Updates the page.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        protected virtual UPMCircleOfInfluencePage UpdatePage(UPMCircleOfInfluencePage element)
        {
            // Override
            return null;
        }

        /// <summary>
        /// Applies the loading status on page.
        /// </summary>
        /// <param name="page">The page.</param>
        protected void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextLoadingData;
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Switches to detail.
        /// </summary>
        /// <param name="nodeIdentifier">The node identifier.</param>
        public virtual void SwitchToDetail(IIdentifier nodeIdentifier)
        {
            bool switchToCoi = this.ViewReference.ContextValueForKey("SwitchToCoi") == "true";
            this.SwitchToRecord(((RecordIdentifier)nodeIdentifier).RecordIdentification, "SHOWRECORD", true, false, switchToCoi ? new SwitchToFirstCoiIndex() : null);
        }

        /// <summary>
        /// Switches to edit.
        /// </summary>
        /// <param name="edge">The edge.</param>
        public virtual void SwitchToEdit(UPMCoIEdge edge)
        {
            this.SwitchToRecord(((RecordIdentifier)edge.Identifier).RecordIdentification, "EDITRECORD", false, true, null);
        }

        /// <summary>
        /// Switches to record.
        /// </summary>
        /// <param name="recorddentification">The recorddentification.</param>
        /// <param name="menuByName">Name of the menu by.</param>
        /// <param name="shouldShowTabsForSingleTap">if set to <c>true</c> [should show tabs for single tap].</param>
        /// <param name="flip">if set to <c>true</c> [flip].</param>
        /// <param name="switchToIndex">Index of the switch to.</param>
        protected void SwitchToRecord(string recorddentification, string menuByName, bool shouldShowTabsForSingleTap, bool flip, UPOrganizerViewSwitchToIndex switchToIndex)
        {
            UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(ConfigurationUnitStore.DefaultStore.MenuByName(menuByName).ViewReference.ViewReferenceWith(recorddentification));
            organizerModelController.ShouldShowTabsForSingleTab = shouldShowTabsForSingleTap;
            if (switchToIndex != null)
            {
                organizerModelController.SwitchToTabAtIndexAfterTabsLoaded = switchToIndex;
            }

            this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }

        /// <summary>
        /// Nodes loaded succesfull.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        public virtual void NodesLoadedSuccesfull(CoIBaseNodeLoader nodeloader)
        {
            // Overrride
        }

        /// <summary>
        /// Nodes load failed error.
        /// </summary>
        /// <param name="nodeloader">The nodeloader.</param>
        /// <param name="error">The error.</param>
        public void NodesLoadFailedError(CoIBaseNodeLoader nodeloader, Exception error)
        {
            this.HandlePageErrorDetails(error.Message, error.StackTrace);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.HandlePageErrorDetails(error.Message, error.StackTrace);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            // Override
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public override void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public override void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
