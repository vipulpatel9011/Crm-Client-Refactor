// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildListGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Serdar
// </author>
// <summary>
//   The Child List Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// Child List Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.IGroupModelControllerDelegate" />
    public class ChildListGroupModelController : UPFieldControlBasedGroupModelController, IGroupModelControllerDelegate
    {
        private List<object> GroupActions;
        protected string ListStyle;
        protected bool DisablePaging;

        /// <summary>
        /// Gets the child controller.
        /// </summary>
        /// <value>
        /// The child controller.
        /// </value>
        public UPListResultGroupModelController ChildController { get; private set; }

        /// <summary>
        /// Gets the maximum results.
        /// </summary>
        /// <value>
        /// The maximum results.
        /// </value>
        public int MaxResults { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildListGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public ChildListGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
            string[] typeParts = this.TabConfig.Type.Split('_');
            string searchAndListConfigurationName;
            int linkId = -1;
            this.MaxResults = 0;
            bool swipeDetailRecords = ConfigurationUnitStore.DefaultStore.ConfigValueIsSetDefaultValue("View.RecordSwipeEnabledDefault", true);
            if (typeParts.Length > 1)
            {
                searchAndListConfigurationName = typeParts[1];
                string[] configParts = searchAndListConfigurationName.Split('#');
                if (configParts.Length > 1)
                {
                    searchAndListConfigurationName = configParts[0];
                    linkId = configParts[1].ToInt();
                }

                if (typeParts.Length > 2)
                {
                    this.MaxResults = typeParts[2].ToInt();
                }

                if (typeParts.Length > 3)
                {
                    if (swipeDetailRecords)
                    {
                        swipeDetailRecords = typeParts[3] != "NoSwipe";
                    }
                    else
                    {
                        swipeDetailRecords = typeParts[3] == "Swipe";
                    }
                }
            }
            else if (this.TabConfig.NumberOfFields > 0)
            {
                UPConfigFieldControlField field = this.TabConfig.FieldAtIndex(0);
                searchAndListConfigurationName = field.InfoAreaId;
                linkId = field.LinkId;
            }
            else
            {
                return;
            }

            FieldControlTab tabConfig = fieldControl.TabAtIndex(tabIndex);
            this.ListStyle = tabConfig.ValueForAttribute("listStyle");
            if (string.IsNullOrEmpty(this.ListStyle))
            {
                this.ListStyle = fieldControl.ValueForAttribute($"listStyle{tabIndex + 1}");
            }

            string disablePagingString = tabConfig.ValueForAttribute("disablePaging");
            if (string.IsNullOrEmpty(disablePagingString))
            {
                disablePagingString = fieldControl.ValueForAttribute($"disablePaging{tabIndex + 1}");
            }

            if (!string.IsNullOrEmpty(disablePagingString))
            {
                this.DisablePaging = disablePagingString.CompareWithOptions("true") || disablePagingString.CompareWithOptions("1");
            }
            else
            {
                this.DisablePaging = false;
            }

            this.ChildController = this.CreateChildControllerLinkIdSearchAndListConfigurationName(swipeDetailRecords, linkId, searchAndListConfigurationName);
            this.ChildController.ExplicitLabel = this.TabLabel;
            this.ChildController.RootTabIndex = this.RootTabIndex;
            if (this.MaxResults > 0)
            {
                this.ChildController.MaxResults = this.MaxResults;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="UPListResultGroupModelController" /> class
        /// </summary>
        /// <param name="swipeDetailRecords">if set to <c>true</c> [swipe detail records].</param>
        /// <param name="linkId">The link identifier.</param>
        /// <param name="searchAndListConfigurationName">Name of the search and list configuration.</param>
        /// <returns></returns>
        UPListResultGroupModelController CreateChildControllerLinkIdSearchAndListConfigurationName(bool swipeDetailRecords, int linkId, string searchAndListConfigurationName)
        {
            return new UPListResultGroupModelController(searchAndListConfigurationName, linkId, swipeDetailRecords, this.ListStyle, this.DisablePaging, this);
        }

        /// <summary>
        /// Applies link record identification and returns an instance of <see cref="UPMGroup" />
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPMGroup ApplyLinkRecordIdentification(string recordIdentification)
        {
            this.ControllerState = GroupModelControllerState.Finished;
            this.ChildController.ExplicitTabIdentifier = this.TabIdentifierForRecordIdentification(recordIdentification);
            return this.ChildController.ApplyLinkRecordIdentification(recordIdentification);
        }

        /// <summary>
        /// Gets a value indicating whether [online only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online only]; otherwise, <c>false</c>.
        /// </value>
        public override bool OnlineOnly => this.ChildController.OnlineOnly;

        /// <summary>
        /// Gets or sets the state of the controller.
        /// </summary>
        /// <value>
        /// The state of the controller.
        /// </value>
        public override GroupModelControllerState ControllerState
            =>
                base.ControllerState == GroupModelControllerState.Finished
                    ? this.ChildController.ControllerState
                    : base.ControllerState;

        /// <summary>
        /// Applies given result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            string recordIdentification = row.RootRecordIdentification;
            UPMGroup group = this.ApplyLinkRecordIdentification(recordIdentification);
            this.GroupActions = this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification);
            this.Group?.Actions.AddRange(this.GroupActions);

            return group;
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public override UPMGroup Group => this.ChildController.Group;


        /// <summary>
        /// Forces the redraw.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ForceRedraw(object sender)
        {
        }

        /// <summary>
        /// Finds the quick actions for row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public override void FindQuickActionsForRow(UPMListRow resultRow)
        {
            this.ChildController.FindQuickActionsForRow(resultRow);
        }

        /// <summary>
        /// Sets the request option.
        /// </summary>
        /// <param name="requestOption">The request option.</param>
        public void SetRequestOption(UPRequestOption requestOption)
        {
            this.RequestOption = requestOption;
            this.ChildController.RequestOption = requestOption;
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public override Exception Error => base.Error ?? this.ChildController.Error;

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void GroupModelControllerFinished(UPGroupModelController sender)
        {
            if (this.Group?.Actions?.Count == 0)
            {
                this.Group.Actions.AddRange(this.GroupActions);
            }

            this.Delegate?.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        public void PerformOrganizerAction(object sender, ViewReference viewReference)
        {
            this.Delegate?.PerformOrganizerAction(this, viewReference);
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        public void PerformOrganizerAction(object sender, ViewReference viewReference, bool onlineData)
        {
            this.Delegate.PerformOrganizerAction(this, viewReference, onlineData);
        }

        /// <summary>
        /// Groups the model controller value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public void GroupModelControllerValueChanged(object sender, object value)
        {
            this.Delegate?.GroupModelControllerValueChanged(sender, value);
        }

        /// <summary>
        /// The transition to content model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void TransitionToContentModelController(UPOrganizerModelController modelController)
        {
            this.Delegate?.TransitionToContentModelController(modelController);
        }

        /// <summary>
        /// The exchange content view controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void ExchangeContentViewController(UPOrganizerModelController modelController)
        {
            this.Delegate?.ExchangeContentViewController(modelController);
        }

        /// <summary>
        /// Groups the model controller value for key.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public string GroupModelControllerValueForKey(object sender, string key)
        {
            return null;
        }
    }
}
