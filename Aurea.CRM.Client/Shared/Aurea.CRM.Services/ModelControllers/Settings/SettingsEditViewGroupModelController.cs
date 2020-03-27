// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsEditViewGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The Settings Edit View Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Settings
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Settings Edit View Group Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Settings.SettingsViewGroupModelController" />
    public class SettingsEditViewGroupModelController : SettingsViewGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsEditViewGroupModelController"/> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        public SettingsEditViewGroupModelController(WebConfigLayout layout, int tabIndex)
            : base(layout, tabIndex)
        {
        }

        /// <summary>
        /// Gets the edit field contexts.
        /// </summary>
        /// <value>
        /// The edit field contexts.
        /// </value>
        public List<UPEditFieldContext> EditFieldContexts { get; private set; }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPMStandardGroup detailGroup = null;
            int fieldCount = this.LayoutTab.FieldCount;
            List<UPEditFieldContext> editFieldContextArray = new List<UPEditFieldContext>(fieldCount);
            for (int j = 0; j < fieldCount; j++)
            {
                WebConfigLayoutField fieldDef = this.LayoutTab.FieldAtIndex(j);
                IIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId(fieldDef.ValueName);
                string fieldValue = configStore.ConfigValue(fieldDef.ValueName);
                UPEditFieldContext editFieldContext = UPEditFieldContext.FieldContextForWebConfigParameterFieldIdentifierValue(fieldDef, fieldIdentifier, fieldValue);
                if (editFieldContext == null)
                {
                    continue;
                }

                editFieldContextArray.Add(editFieldContext);
                if (detailGroup == null)
                {
                    detailGroup = new UPMStandardGroup(StringIdentifier.IdentifierWithStringId($"{this.Layout.UnitName}_{this.TabIndex}"));
                    detailGroup.LabelText = this.TabLabel;
                }

                foreach (UPMEditField editField in editFieldContext.EditFields)
                {
                    detailGroup.AddField(editField);
                }
            }

            this.EditFieldContexts = editFieldContextArray;
            this.Group = detailGroup;
            this.ControllerState = (detailGroup == null) ? GroupModelControllerState.Empty : GroupModelControllerState.Finished;
            return detailGroup;
        }
    }
}
