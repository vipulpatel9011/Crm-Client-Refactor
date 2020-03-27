// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Menu Group Model Controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Menu Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    public class UPMenuGroupModelController : UPFieldControlBasedGroupModelController
    {
        private Dictionary<string, ViewReference> viewReferenceDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPMenuGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
            : base(formItem, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPMenuGroupModelController(IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPMenuGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(fieldControl, tabIndex, theDelegate)
        {
        }

        /// <summary>
        /// Gets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; private set; }

        /// <summary>
        /// Performs the menu action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PerformMenuAction(object sender)
        {
            UPMLinkRecordField linkRecordField = (UPMLinkRecordField)sender;
            ViewReference viewReference = this.viewReferenceDictionary[linkRecordField.Identifier.IdentifierAsString];
            this.Delegate.PerformOrganizerAction(sender, viewReference);
        }

        /// <summary>
        /// Applies the link record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPMGroup ApplyLinkRecordIdentification(string recordIdentification)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string menuName = this.TabConfig.Type.Substring(5);
            this.Menu = configStore.MenuByName(menuName);
            if (this.Menu == null || this.Menu.Items.Count == 0)
            {
                this.ControllerState = GroupModelControllerState.Error;
                base.Error = new Exception("ConfigurationError");
                return null;
            }

            UPMMenuGroup menuGroup = null;
            uint menuItemNr = 0;
            this.viewReferenceDictionary = new Dictionary<string, ViewReference>();
            foreach (string menuItemName in this.Menu.Items)
            {
                Menu menuItem = configStore.MenuByName(menuItemName);
                if (menuItem.ViewReference == null)
                {
                    continue;
                }

                if (menuGroup == null)
                {
                    menuGroup = new UPMMenuGroup(this.TabIdentifierForRecordIdentification(recordIdentification));
                    menuGroup.LabelText = this.TabLabel;
                }

                string identifierString = $"menu_{menuItemNr++}_{ menuItem.UnitName}";
                IIdentifier identifier = StringIdentifier.IdentifierWithStringId(identifierString);
                UPMAction menuAction = new UPMAction(identifier);
                ViewReference linkRecordViewReference = menuItem.ViewReference.ViewReferenceWith(recordIdentification);
                this.viewReferenceDictionary[identifierString] = linkRecordViewReference;
                menuAction.SetTargetAction(this, this.PerformMenuAction);
                if (menuItem.ImageName != null)
                {
                    menuAction.IconName = menuItem.ImageName;
                }

                UPMLinkRecordField field = new UPMLinkRecordField(identifier, menuAction);
                field.LabelText = menuItem.UnitName;
                field.StringValue = menuItem.DisplayName;
                menuGroup.AddField(field);
            }

            this.ControllerState = menuGroup != null ? GroupModelControllerState.Finished : GroupModelControllerState.Empty;
            this.Group = menuGroup;
            return menuGroup;
        }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            base.ApplyResultRow(row);
            UPMGroup group = this.ApplyLinkRecordIdentification(row.RootRecordIdentification);
            group.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification));
            return group;
        }
    }
}
