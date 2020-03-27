// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldControlBasedGroupModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// --------------------------------------------------------------------------------------------------------------------s

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// Field control based group model controller for view mode
    /// </summary>
    /// <seealso cref="UPGroupModelController" />
    public class UPFieldControlBasedGroupModelController : UPGroupModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="formItem">The form item.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPFieldControlBasedGroupModelController(FormItem formItem, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this.FormItem = formItem;
            this.ControllerState = GroupModelControllerState.Empty;
            this.ChangedControllerState = true;
            this.RequestOption = UPRequestOption.FastestAvailable;
            this.RootTabIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPFieldControlBasedGroupModelController(IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPFieldControlBasedGroupModelController"/> class.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPFieldControlBasedGroupModelController(FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this.FieldControlConfig = fieldControl;
            this.TabIndex = tabIndex;
            this.TabConfig = this.FieldControlConfig.TabAtIndex(tabIndex);
        }

        /// <summary>
        /// Gets the field control configuration.
        /// </summary>
        /// <value>
        /// The field control configuration.
        /// </value>
        public FieldControl FieldControlConfig { get; private set; }

        /// <summary>
        /// Gets the index of the tab.
        /// </summary>
        /// <value>
        /// The index of the tab.
        /// </value>
        public int TabIndex { get; private set; }

        /// <summary>
        /// Gets or sets the tab configuration.
        /// </summary>
        /// <value>
        /// The tab configuration.
        /// </value>
        public FieldControlTab TabConfig { get; set; }

        /// <summary>
        /// Builds the additional actions for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public List<object> BuildAdditionalActionsForRecordIdentification(string recordIdentification)
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var buttonsArray = new List<object>();
            var buttonName = this.FieldControlConfig.ValueForAttribute($"action{this.TabIndex + 1}");
            var buttons = new List<string>();
            if (!string.IsNullOrEmpty(buttonName))
            {
                buttons.AddRange(buttonName.Split(';'));
            }

            buttonName = this.TabConfig.ValueForAttribute("action");
            if (!string.IsNullOrEmpty(buttonName))
            {
                buttons.AddRange(buttonName.Split(';'));
            }

            foreach (var bName in buttons)
            {
                var buttonDef = configStore.ButtonByName(bName);
                if (buttonDef == null || buttonDef.IsHidden)
                {
                    continue;
                }

                var iconName = string.Empty;
                if (!string.IsNullOrEmpty(buttonDef.ImageName))
                {
                    iconName = configStore.FileNameForResourceName(buttonDef.ImageName);
                }

                UPMOrganizerAction action = null;

                if (buttonDef.ViewReference != null)
                {
                    action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId($"action.{bName}"));
                    action.SetTargetAction(this, action.PerformAction);
                    action.ViewReference = buttonDef.ViewReference.ViewReferenceWith(recordIdentification);
                    if (!string.IsNullOrEmpty(iconName))
                    {
                        action.IconName = iconName;
                    }

                    if (action.Identifier.Equals(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                    {
                        action.IconName = "Icon:StarEmpty";
                        action.ActiveIconName = "Icon:Star";
                        action.LabelText = LocalizedString.TextProcessAddToFavorites;
                    }
                    else
                    {
                        action.LabelText = buttonDef.Label;
                    }
                }

                if (action != null)
                {
                    action.Invalid = true;
                    buttonsArray.Add(action);
                }
            }

            return buttonsArray;
        }

        /// <summary>
        /// Gets or sets the index of the root tab.
        /// </summary>
        /// <value>
        /// The index of the root tab.
        /// </value>
        public override int RootTabIndex
        {
            get
            {
                return base.RootTabIndex < 0 ? this.TabIndex : base.RootTabIndex;
            }

            set { base.RootTabIndex = value; }
        }

        /// <summary>
        /// Tabs the identifier for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public IIdentifier TabIdentifierForRecordIdentification(string recordIdentification)
        {
            return FieldIdentifier.IdentifierWithRecordIdentificationFieldId(recordIdentification, $"Tabheader {this.RootTabIndex}");
        }

        /// <summary>
        /// Tabs the label.
        /// </summary>
        /// <returns></returns>
        public override string TabLabel => !string.IsNullOrEmpty(this.ExplicitLabel) ? this.ExplicitLabel : this.TabConfig.Label;

        /// <summary>
        /// Detailses the group model controller for control tab index the delegate.
        /// </summary>
        /// <param name="fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPFieldControlBasedGroupModelController DetailsGroupModelControllerForFieldControl(
            FieldControl fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
        {
            var tab = fieldControl.TabAtIndex(tabIndex);
            var tabType = tab.Type;

            if (tabType.StartsWith("IGNORE"))
            {
                return new UPFieldControlBasedGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("PARTICIPANTS"))
            {
                return new UPParticipantsGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("CHILDREN"))
            {
                return new ChildListGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("MAP"))
            {
                return new UPMapGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("MULTIMAP"))
            {
                return new ChildMultiMapGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("PARENT"))
            {
                return new UPParentDetailsGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("DOCFIELDS"))
            {
                return new UPDocumentFieldGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("DOCSEARCH"))
            {
                return new UPDocumentGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("DOCUMENTS"))
            {
                if (fieldControl.TabAtIndex(tabIndex).NumberOfFields > 0)
                {
                    return new UPDocumentFieldGroupModelController(fieldControl, tabIndex, theDelegate);
                }

                return new UPDocumentGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("DOC"))
            {
                return new UPDocumentGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("CHARACTERISTICS"))
            {
                return new UPCharacteristicsGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("MENU"))
            {
                return new UPMenuGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("WEBCONTENT"))
            {
                return new UPWebContentGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("INSIGHTBOARDH"))
            {
                return new UPInsightBoardGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("INSIGHTBOARDV2"))
            {
                return new UPInsightBoardGroupModelController(fieldControl, tabIndex, theDelegate, false);
            }

            if (tabType.StartsWith("INSIGHTBOARDV"))
            {
                return new UPInsightBoardGroupModelController(fieldControl, tabIndex, theDelegate, false, true);
            }

            if (tabType.StartsWith("INSIGHTBOARD"))
            {
                return new UPInsightBoardGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("CONTACTTIMES"))
            {
                return new ContactTimesGroupModelController(fieldControl, tabIndex, theDelegate);
            }
#if PORTING
            if (tabType.StartsWith("QUERY"))
            {
                return new UPQueryListResultGroupModelController(fieldControl, tabIndex, theDelegate);
            }

            if (tabType.StartsWith("ANALYSIS"))
            {
                return new UPAnalysisListResultGroupModelController(fieldControl, tabIndex, theDelegate);
            }
#endif
            return new UPDetailsFieldGroupModelController(fieldControl, tabIndex, theDelegate);
        }

        /// <summary>
        /// Sets the attributes on field.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <param name="element">The element.</param>
        public static void SetAttributesOnField(FieldAttributes attributes, UPMField element)
        {
            element.SetAttributes(attributes);
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void PerformAction(Dictionary<string, object> actionDictionary)
        {
            var action = actionDictionary.ValueOrDefault("UPMOrganizerAction") as UPMOrganizerAction;
            this.Delegate.PerformOrganizerAction(this, action.ViewReference);
        }
    }
}
