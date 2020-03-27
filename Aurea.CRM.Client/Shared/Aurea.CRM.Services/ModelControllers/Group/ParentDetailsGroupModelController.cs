// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParentDetailsGroupModelController.cs" company="Aurea Software Gmbh">
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
//   Parent Details Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// Parent Details Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.IGroupModelControllerDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    public class UPParentDetailsGroupModelController : UPFieldControlBasedGroupModelController, ISearchOperationHandler, IGroupModelControllerDelegate, UPCRMLinkReaderDelegate
    {
        private UPContainerMetaInfo metaInfo;
        private bool enableDelegate;

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        /// <value>
        /// The parent controller.
        /// </value>
        public UPGroupModelController ParentController { get; private set; }

        /// <summary>
        /// Gets the parent field control.
        /// </summary>
        /// <value>
        /// The parent field control.
        /// </value>
        public FieldControl ParentFieldControl { get; private set; }

        /// <summary>
        /// Gets the link reader.
        /// </summary>
        /// <value>
        /// The link reader.
        /// </value>
        public UPCRMLinkReader LinkReader { get; private set; }

        /// <summary>
        /// Gets the link fields.
        /// </summary>
        /// <value>
        /// The link fields.
        /// </value>
        public List<UPConfigFieldControlField> LinkFields { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPParentDetailsGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPParentDetailsGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
            var typeParts = this.TabConfig.Type.Split('_');
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (typeParts.Length > 1)
            {
                string fieldGroupName;
                var configParts = ((string)typeParts[1]).Split('#');
                if (configParts.Length > 1)
                {
                    fieldGroupName = configParts[0];
                    this.LinkId = Convert.ToInt32(configParts[1]);
                }
                else
                {
                    fieldGroupName = (string)typeParts[1];
                    this.LinkId = -1;
                }

                int parentTabIndex;
                if (typeParts.Length > 2)
                {
                    parentTabIndex = Convert.ToInt32(typeParts[2]);
                    if (parentTabIndex >= 1)
                    {
                        parentTabIndex--;
                    }
                }
                else
                {
                    parentTabIndex = 0;
                }

                this.ParentFieldControl = configStore.FieldControlByNameFromGroup("Details", fieldGroupName);
                if (this.ParentFieldControl.NumberOfTabs <= parentTabIndex || parentTabIndex < 0)
                {
                    parentTabIndex = 0;
                }

                this.ParentFieldControl = this.ParentFieldControl.FieldControlWithSingleTab(parentTabIndex);
                this.ParentController = DetailsGroupModelController(this.ParentFieldControl, 0, this);
                this.ParentController.RootTabIndex = this.RootTabIndex;
                this.ParentController.ExplicitLabel = this.TabLabel;
            }
            else
            {
                this.LinkFields = fieldControl.TabAtIndex(tabIndex).Fields;
            }
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            UPMGroup group = this.ApplyLinkRecordIdentification(row.RootRecordIdentification);
            group?.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification));
            return group;
        }

        /// <summary>
        /// Gets the parent link string.
        /// </summary>
        /// <value>
        /// The parent link string.
        /// </value>
        public string ParentLinkString
        {
            get
            {
                string parentLinkString = null;
                foreach (UPConfigFieldControlField field in this.LinkFields)
                {
                    string linkFieldString = field.LinkId > 0 ? $"{field.InfoAreaId}:{field.LinkId}" : field.InfoAreaId;
                    parentLinkString = !string.IsNullOrEmpty(parentLinkString) ? $"{parentLinkString};{linkFieldString}" : linkFieldString;
                }

                return parentLinkString;
            }
        }

        /// <summary>
        /// Applies the link record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPMGroup ApplyLinkRecordIdentification(string recordIdentification)
        {
            this.enableDelegate = false;
            if (this.LinkFields != null)
            {
                this.LinkReader = new UPCRMLinkReader(recordIdentification, this.ParentLinkString, this.RequestOption, this);
                this.ControllerState = GroupModelControllerState.Pending;
                this.LinkReader.Start();
                this.enableDelegate = true;
                return this.Group;
            }

            this.metaInfo = new UPContainerMetaInfo(this.ParentFieldControl);
            this.metaInfo.SetLinkRecordIdentification(recordIdentification, this.LinkId);
            this.metaInfo.DisableVirtualLinks = true;
            this.ParentController.ExplicitTabIdentifier = this.TabIdentifierForRecordIdentification(recordIdentification);
            if (this.RequestOption == UPRequestOption.Offline || this.RequestOption == UPRequestOption.FastestAvailable)
            {
                UPCRMResult result = this.metaInfo.Find();
                if (result.RowCount > 0)
                {
                    this.ControllerState = GroupModelControllerState.Finished;
                    if (this.ParentController.ApplyResultRow((UPCRMResultRow)result.ResultRowAtIndex(0)) == null)
                    {
                        this.enableDelegate = true;
                    }

                    return this.Group;
                }
            }

            this.enableDelegate = true;
            if (this.RequestOption != UPRequestOption.Offline)
            {
                this.ControllerState = GroupModelControllerState.Pending;
                Operation remoteOperation = this.metaInfo.Find(this);
                if (remoteOperation == null)
                {
                    this.ControllerState = GroupModelControllerState.Error;
                }
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Empty;
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the state of the controller.
        /// </summary>
        /// <value>
        /// The state of the controller.
        /// </value>
        public override GroupModelControllerState ControllerState => base.ControllerState == GroupModelControllerState.Finished ? this.ParentController.ControllerState : base.ControllerState;

        /// <summary>
        /// Gets or sets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public override UPRequestOption RequestOption
        {
            get
            {
                return base.RequestOption;
            }

            set
            {
                base.RequestOption = value;
                if (this.ParentController != null)
                {
                    this.ParentController.RequestOption = value;
                }
            }
        }

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void GroupModelControllerFinished(UPGroupModelController sender)
        {
            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Groups the model controller value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public void GroupModelControllerValueChanged(object sender, object value)
        {
            this.Delegate.GroupModelControllerValueChanged(sender, value);
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        public void PerformOrganizerAction(object sender, ViewReference viewReference)
        {
            this.Delegate.PerformOrganizerAction(this, viewReference);
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        public void PerformOrganizerAction(object sender, ViewReference viewReference, bool onlineData)
        {
            this.Delegate.PerformOrganizerAction(sender, viewReference, onlineData);
        }

        /// <summary>
        /// The transition to content model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void TransitionToContentModelController(UPOrganizerModelController modelController)
        {
            this.Delegate.TransitionToContentModelController(modelController);
        }

        /// <summary>
        /// The exchange content view controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void ExchangeContentViewController(UPOrganizerModelController modelController)
        {
            this.Delegate.ExchangeContentViewController(modelController);
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public override UPMGroup Group
        {
            get { return this.ParentController.Group; }
            set { this.ParentController.Group = value; }
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public override Exception Error
        {
            get { return base.Error ?? this.ParentController.Error; }
            protected set { base.Error = value; }
        }

        /// <summary>
        /// Forces the redraw.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ForceRedraw(object sender)
        {
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

        /// <summary>
        /// Finds the quick actions for row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        public override void FindQuickActionsForRow(UPMListRow resultRow)
        {
            this.ParentController.FindQuickActionsForRow(resultRow);
        }

        /// <summary>
        /// The link reader did finish with result.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            if (!string.IsNullOrEmpty(linkReader.DestinationRecordIdentification))
            {
                if (this.CreateParentControllerFromLinkReader(linkReader))
                {
                    this.LinkFields = null;
                    if (this.ApplyLinkRecordIdentification(linkReader.DestinationRecordIdentification) != null)
                    {
                        this.ControllerState = GroupModelControllerState.Finished;
                        this.CallDelegate();
                    }
                }
                else
                {
                    this.ControllerState = GroupModelControllerState.Error;
                    this.CallDelegate();
                }
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Empty;
                this.CallDelegate();
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Error = error;
            this.CallDelegate();
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Error = error;
            this.CallDelegate();
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount > 0)
            {
                if (this.ParentController.ApplyResultRow((UPCRMResultRow)result.ResultRowAtIndex(0)) != null
                    || this.ParentController.ControllerState != GroupModelControllerState.Pending)
                {
                    this.ControllerState = GroupModelControllerState.Finished;
                    this.CallDelegate();
                }
            }
            else
            {
                this.ControllerState = GroupModelControllerState.Empty;
                this.CallDelegate();
            }
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

        private bool CreateParentControllerFromLinkReader(UPCRMLinkReader linkReader)
        {
            int tabIndex;
            UPConfigFieldControlField field = this.LinkFields[linkReader.DestinationPosition];
            string fieldGroupName = field.InfoAreaId;
            tabIndex = 0;
            string fieldStyle = field.Attributes.FieldStyle;
            if (!string.IsNullOrEmpty(fieldStyle))
            {
                var parts = fieldStyle.Split('_');
                if (parts.Length > 1)
                {
                    tabIndex = Convert.ToInt32(parts[1]);
                    fieldGroupName = parts[0];
                }
                else
                {
                    fieldGroupName = fieldStyle;
                }
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.ParentFieldControl = configStore.FieldControlByNameFromGroup("Details", fieldGroupName);
            if (this.ParentFieldControl == null && fieldGroupName == field.InfoAreaId)
            {
                this.ParentFieldControl = configStore.FieldControlByNameFromGroup("Details", field.InfoAreaId);
            }

            if (this.ParentFieldControl == null)
            {
                return false;
            }

            if (tabIndex >= this.ParentFieldControl.NumberOfTabs)
            {
                tabIndex = 0;
            }

            this.ParentController = UPGroupModelController.DetailsGroupModelController(this.ParentFieldControl, tabIndex, this);
            this.ParentController.ExplicitLabel = this.TabLabel;
            return true;
        }

        private void CallDelegate()
        {
            if (this.enableDelegate)
            {
                this.Delegate.GroupModelControllerFinished(this);
            }
        }
    }
}
