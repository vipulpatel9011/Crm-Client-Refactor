// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewGroupModelController.cs" company="Aurea Software Gmbh">
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
//   Settings View Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Settings View Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPGroupModelController" />
    public class SettingsViewGroupModelController : UPGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewGroupModelController"/> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <exception cref="Exception">Layout Tab is null</exception>
        public SettingsViewGroupModelController(WebConfigLayout layout, int tabIndex)
            : base(null)
        {
            this.Layout = layout;
            this.TabIndex = tabIndex;
            this.LayoutTab = this.Layout.TabAtIndex(tabIndex);

            if (this.LayoutTab == null)
            {
                throw new Exception("Layout Tab is null");
            }
        }

        /// <summary>
        /// Gets the layout.
        /// </summary>
        /// <value>
        /// The layout.
        /// </value>
        public WebConfigLayout Layout { get; private set; }

        /// <summary>
        /// Gets the index of the tab.
        /// </summary>
        /// <value>
        /// The index of the tab.
        /// </value>
        public int TabIndex { get; private set; }

        /// <summary>
        /// Gets the layout tab.
        /// </summary>
        /// <value>
        /// The layout tab.
        /// </value>
        public WebConfigLayoutTab LayoutTab { get; private set; }

        /// <summary>
        /// Gets the tab label.
        /// </summary>
        /// <value>
        /// The tab label.
        /// </value>
        public override string TabLabel => this.LayoutTab.Label;

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            bool hideEmptyFields = configStore.ConfigValueIsSet("SettingsView.HideEmptyFields");
            UPMStandardGroup detailGroup = null;
            int fieldCount = this.LayoutTab.FieldCount;
            for (int j = 0; j < fieldCount; j++)
            {
                WebConfigLayoutField fieldDef = this.LayoutTab.FieldAtIndex(j);
                IIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId(fieldDef.ValueName);
                string fieldValue = configStore.ConfigValue(fieldDef.ValueName);
                bool hasFieldValue = !string.IsNullOrEmpty(fieldValue);

                if (!hasFieldValue && hideEmptyFields)
                {
                    continue;
                }

                var field = new UPMStringField(fieldIdentifier)
                {
                    StringValue = fieldDef.DisplayValueForValue(fieldValue),
                    LabelText = fieldDef.Label
                };
                if (detailGroup == null)
                {
                    detailGroup = new UPMStandardGroup(StringIdentifier.IdentifierWithStringId($"{this.Layout.UnitName}_{this.TabIndex}"));
                    detailGroup.LabelText = this.TabLabel;
                }

                detailGroup.AddField(field);
            }

            this.Group = detailGroup;
            this.ControllerState = (detailGroup == null) ? GroupModelControllerState.Empty : GroupModelControllerState.Finished;
            return detailGroup;
        }
    }
}
