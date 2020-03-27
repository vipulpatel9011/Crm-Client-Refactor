// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupBasedPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;
    using Core.Configuration;
    using Core.CRM;
    using Core.CRM.DataModel;
    using Core.CRM.UIModel;
    using Core.Extensions;
    using Core.Logging;
    using Delegates;
    using Group;
    using GalaSoft.MvvmLight.Ioc;
    using Organizer;
    using UIModel;
    using UIModel.Fields;
    using UIModel.Identifiers;
    using UIModel.Pages;
    using UIModel.Status;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The group added to page hint
        /// </summary>
        public const string GroupAddedToPageHint = "GroupAddedHint";

        /// <summary>
        /// The group page change hint
        /// </summary>
        public const string GroupPageChangeHint = "PageChangedHint";
    }

    /// <summary>
    /// Group based page model controller
    /// </summary>UpdatedElementForPage
    /// <seealso cref="UPPageModelController" />
    /// <seealso cref="IGroupModelControllerDelegate" />
    public abstract class GroupBasedPageModelController : UPPageModelController, IGroupModelControllerDelegate
    {
        private bool drawn;
        private List<UPGroupModelController> pendingGroupControllerCemetery;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBasedPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected GroupBasedPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.GroupModelControllerArray = new List<UPGroupModelController>();
            this.ValueDictionary = new Dictionary<string, object>();
            var recordIdentification = viewReference.ContextValueForKey("RecordId");

            if (!string.IsNullOrEmpty(recordIdentification))
            {
                this.ValueDictionary.Add("$Record", recordIdentification);
            }
        }

        /// <summary>
        /// Gets the value dictionary.
        /// </summary>
        /// <value>
        /// The value dictionary.
        /// </value>
        public Dictionary<string, object> ValueDictionary { get; }

        /// <summary>
        /// Gets or sets the group model controller array.
        /// </summary>
        /// <value>
        /// The group model controller array.
        /// </value>
        public List<UPGroupModelController> GroupModelControllerArray { get; set; }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public abstract void BuildPage();

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public abstract UPMElement UpdatedElementForPage(Page page);

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            var page = element as Page;
            return page != null ? this.UpdatedElementForPage(page) : element;
        }

        /// <summary>
        /// Applies the loading status on page.
        /// </summary>
        /// <param name="page">The page.</param>
        public virtual void ApplyLoadingStatusOnPage(Page page)
        {
            var stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));

            var statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"))
            {
                FieldValue = LocalizedString.TextLoadingData
            };

            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Performs the organizer action view reference.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        public virtual void PerformOrganizerAction(object sender, ViewReference viewReference)
        {
            this.PerformOrganizerAction(sender, viewReference, false);
        }

        /// <summary>
        /// Performs the organizer action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        public virtual void PerformOrganizerAction(object sender, ViewReference viewReference, bool onlineData)
        {
            var logProvider = SimpleIoc.Default.GetInstance<ILogger>();
            logProvider.LogInfo($"Perform PerformOrganizerAction -  {viewReference.ViewName}");
            var selectorName = string.Empty;
            if (viewReference.ViewName == "OrganizerAction")
            {
                selectorName = viewReference.ContextValueForKey(nameof(Action));
            }
            else if (viewReference.ViewName.StartsWith("Action:"))
            {
                selectorName = viewReference.ViewName.Substring(7);
            }

            if (!string.IsNullOrEmpty(selectorName))
            {
                if (selectorName == "switchOnRecord")
                {
                    this.ParentOrganizerModelController.SwitchOnRecord(viewReference);
                }
                else
                {
                    selectorName = selectorName.Replace(":", string.Empty);
                    selectorName = selectorName[0].ToString().ToUpper() + selectorName.Substring(1, selectorName.Length - 1);

                    this.ParentOrganizerModelController.FindAndInvokeMethod(selectorName, new object[] { viewReference });
                }

                return;
            }

            UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(viewReference);
            if (organizerModelController == null)
            {
                logProvider.LogWarn($"{viewReference.ViewName} - Not Supported");
            }
            else
            {
                organizerModelController.OnlineData = onlineData;
                this.ModelControllerDelegate?.TransitionToContentModelController(organizerModelController);
            }

        }

        /// <summary>
        /// Draws the whole page.
        /// </summary>
        /// <param name="page">The page.</param>
        public virtual void DrawWholePage(Page page)
        {
            this.TopLevelElement = page;
            if (this.GroupModelControllerArray == null)
            {
                return;
            }

            foreach (var groupModelController in this.GroupModelControllerArray)
            {
                if (groupModelController?.Group == null)
                {
                    continue;
                }

                groupModelController.Group.ConfiguredPostionOfGroup = groupModelController.RootTabIndex;
                page.AddGroup(groupModelController.Group);
            }

            page.Invalid = false;
        }

        /// <summary>
        /// The transition to content model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void TransitionToContentModelController(UPOrganizerModelController modelController)
        {
            this.ModelControllerDelegate.TransitionToContentModelController(modelController);
        }

        /// <summary>
        /// The exchange content view controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        public void ExchangeContentViewController(UPOrganizerModelController modelController)
        {
            this.ParentOrganizerModelController.ModelControllerDelegate.ExchangeContentViewController(modelController);
        }

        /// <summary>
        /// Drawables the page model controller.
        /// </summary>
        /// <param name="signalFinished">if set to <c>true</c> [signal finished].</param>
        /// <returns></returns>
        public virtual bool DrawablePageModelController(bool signalFinished) => true;

        /// <summary>
        /// Fills the page with result row request option.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="resultRow">The result row.</param>
        /// <param name="requestOption">The request option.</param>
        public virtual void FillPageWithResultRow(Page page, UPCRMResultRow resultRow, UPRequestOption requestOption)
        {
            // use localArray because the array could be changed from outside
            var localArray = new List<UPGroupModelController>(this.GroupModelControllerArray);
            var changeIdentifier = new List<IIdentifier>();
            try
            {
                foreach (var groupModelController in localArray)
                {
                    if (requestOption != UPRequestOption.Default)
                    {
                        if (requestOption != UPRequestOption.Offline || !groupModelController.OnlineOnly)
                        {
                            groupModelController.RequestOption = requestOption;
                        }
                    }

                    if (groupModelController.DelegateCleared)
                    {
                        continue;
                    }

                    groupModelController.ApplyResultRow(resultRow);
                    if (groupModelController.ControllerState != GroupModelControllerState.Finished ||
                        groupModelController.Group == null)
                    {
                        continue;
                    }

                    changeIdentifier.Add(groupModelController.Group.Identifier);
                    groupModelController.Group.ConfiguredPostionOfGroup = groupModelController.RootTabIndex;
                }
            }
            catch (Exception error)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(error);
            }

            if (this.DrawablePageModelController(true))
            {
                this.DrawWholePage(page);
                if (changeIdentifier.Count > 0)
                {
                    this.InformAboutDidChangeTopLevelElement(page, page, changeIdentifier, UPChangeHints.ChangeHintsWithHint(Constants.GroupAddedToPageHint));
                }
            }
        }

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public virtual void GroupModelControllerFinished(UPGroupModelController sender)
        {
            var newPage = this.InstantiatePage();
            var oldPage = this.Page;
            newPage.LabelText = oldPage.LabelText;
            this.DrawWholePage(newPage);
            this.TopLevelElement = newPage;
            this.PageLoadingFinished();

            if (sender?.Group != null)
            {
                sender.Group.ConfiguredPostionOfGroup = sender.RootTabIndex;

                //this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, UPChangeHints.ChangeHintsWithHint(Constants.GroupPageChangeHint));
                this.InformAboutDidChangeTopLevelElement(oldPage, newPage, new List<IIdentifier> { sender.Group.Identifier }, UPChangeHints.ChangeHintsWithHint(Constants.GroupAddedToPageHint));
            }
            else if(sender != null && sender.Group == null && oldPage.Groups.Count > sender.RootTabIndex)
            {
                var idenifier = oldPage.Groups[sender.RootTabIndex].Identifier;
                this.InformAboutDidChangeTopLevelElement(oldPage, newPage, new List<IIdentifier> { idenifier }, UPChangeHints.ChangeHintsWithHint(Constants.GroupAddedToPageHint));
            }
        }

        /// <summary>
        /// Groups the model controller for identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        public virtual UPGroupModelController GroupModelControllerForIdentifier(IIdentifier identifier)
        {
            foreach (var groupModelController in this.GroupModelControllerArray)
            {
                if (groupModelController.Group != null)
                {
                    if (identifier.Equals(groupModelController.Group.Identifier))
                    {
                        return groupModelController;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Pages the loading finished.
        /// </summary>
        public virtual void PageLoadingFinished()
        {
            // Do Nothing
        }

        /// <summary>
        /// Adds the group controller.
        /// </summary>
        /// <param name="groupModelController">The group model controller.</param>
        public virtual void AddGroupController(UPGroupModelController groupModelController)
        {
            this.GroupModelControllerArray.Add(groupModelController);
            if (string.IsNullOrEmpty(groupModelController.ValueName))
            {
                return;
            }

            var value = groupModelController.Value ?? string.Empty;

            this.ValueDictionary.SetObjectForKey(value, $"${groupModelController.ValueName}");
        }

        /// <summary>
        /// Adds the name of the value with value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueName">Name of the value.</param>
        public virtual void AddValueWithValueName(object value, string valueName)
        {
            this.ValueDictionary[$"${valueName}"] = value;
        }

        /// <summary>
        /// Clears the group controllers.
        /// </summary>
        public virtual void ClearGroupControllers()
        {
            foreach (var ctrl in this.GroupModelControllerArray)
            {
                ctrl.ClearDelegate();
                if (ctrl.ControllerState != GroupModelControllerState.Pending)
                {
                    continue;
                }

                if (this.pendingGroupControllerCemetery == null)
                {
                    this.pendingGroupControllerCemetery = new List<UPGroupModelController> { ctrl };
                }
                else
                {
                    this.pendingGroupControllerCemetery.Add(ctrl);
                }
            }

            this.GroupModelControllerArray.Clear();
        }

        /// <summary>
        /// Forces the redraw.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public virtual void ForceRedraw(object sender)
        {
            var oldPage = (Page)this.TopLevelElement;
            var newPage = this.InstantiatePage();
            this.drawn = false;
            this.DrawWholePage(newPage);
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        /// <summary>
        /// Groups the model controller value changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public virtual void GroupModelControllerValueChanged(object sender, object value)
        {
            var group = (UPGroupModelController)sender;
            var changedValueName = group.ValueName;
            if (string.IsNullOrEmpty(changedValueName))
            {
                return;
            }

            changedValueName = $"${changedValueName}";
            var changedValue = group.Value ?? string.Empty;

            List<UPGroupModelController> affectedGroupModelController = null;
            foreach (var g in this.GroupModelControllerArray)
            {
                if (!g.AffectedByKey(changedValueName))
                {
                    continue;
                }

                if (affectedGroupModelController == null)
                {
                    affectedGroupModelController = new List<UPGroupModelController> { g };
                }
                else
                {
                    affectedGroupModelController.Add(g);
                }
            }

            this.ValueDictionary.SetObjectForKey(changedValue, changedValueName);
            if (this.GroupModelControllerArray.Count == 0)
            {
                return;
            }

            var changedIdentfiers = new List<IIdentifier>();
            if (affectedGroupModelController != null)
            {
                foreach (var g in affectedGroupModelController)
                {
                    g.ApplyContext(this.ValueDictionary);
                    if (g.ControllerState != GroupModelControllerState.Finished)
                    {
                        continue;
                    }

                    if (g.Group != null)
                    {
                        changedIdentfiers.Add(g.Group.Identifier);
                    }
                }
            }

            var newPage = this.InstantiatePage();
            this.drawn = false;
            this.DrawWholePage(newPage);
            var oldPage = this.Page;
            this.TopLevelElement = newPage;
            if (this.DrawablePageModelController(false))
            {
                this.PageLoadingFinished();
                this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, UPChangeHints.ChangeHintsWithHint(Constants.GroupPageChangeHint));
                if (changedIdentfiers.Count > 0)
                {
                    this.InformAboutDidChangeTopLevelElement(oldPage, newPage, changedIdentfiers, UPChangeHints.ChangeHintsWithHint(Constants.GroupAddedToPageHint));
                }
            }
        }

        /// <summary>
        /// Groups the model controller value for key.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public virtual string GroupModelControllerValueForKey(object sender, string key)
        {
            return this.ValueDictionary?.ValueOrDefault(key) as string;
        }
    }
}
