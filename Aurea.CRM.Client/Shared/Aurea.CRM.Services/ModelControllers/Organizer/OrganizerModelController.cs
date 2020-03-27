// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The OrganizerModelController.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using Analysis;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Characteristics;
    using Aurea.CRM.Services.ModelControllers.CircleOfInfluence;
    using Aurea.CRM.Services.ModelControllers.Documents;
    using Aurea.CRM.Services.ModelControllers.Questionnaire;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.Services.ModelControllers.SerialEntry;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Organizer View Switch to Index
    /// </summary>
    public interface UPOrganizerViewSwitchToIndex
    {
        /// <summary>
        /// Index the of view to switch.
        /// </summary>
        /// <param name="pageModelControllers">The page model controllers.</param>
        /// <returns>Index of the view</returns>
        int IndexOfViewToSwitch(List<UPPageModelController> pageModelControllers);
    }

    /// <summary>
    /// Organizer Action Test Result
    /// </summary>
    public class UPOrganizerActionTestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerActionTestResult"/> class.
        /// </summary>
        /// <param name="changedRecords">The changed records.</param>
        /// <param name="followViewReference">The follow view reference.</param>
        public UPOrganizerActionTestResult(List<UPCRMRecord> changedRecords, ViewReference followViewReference)
        {
            this.ChangedRecords = changedRecords;
            this.FollowViewReference = followViewReference;
        }

        /// <summary>
        /// Gets the changed records.
        /// </summary>
        /// <value>
        /// The changed records.
        /// </value>
        public List<UPCRMRecord> ChangedRecords { get; private set; }

        /// <summary>
        /// Gets the follow view reference.
        /// </summary>
        /// <value>
        /// The follow view reference.
        /// </value>
        public ViewReference FollowViewReference { get; private set; }
    }

    /// <summary>
    /// Organizer Init Options
    /// </summary>
    public class UPOrganizerInitOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to should show tabs for single tab.
        /// </summary>
        /// <value>
        /// <c>true</c> if to should show tabs for single tab; otherwise, <c>false</c>.
        /// </value>
        public bool bShouldShowTabsForSingleTab { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [b no automatic build].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [b no automatic build]; otherwise, <c>false</c>.
        /// </value>
        public bool bNoAutoBuild { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerInitOptions"/> class.
        /// </summary>
        /// <param name="noAutoBuild">if set to <c>true</c> [no automatic build].</param>
        /// <param name="tabForSingleTab">if set to <c>true</c> [tab for single tab].</param>
        public UPOrganizerInitOptions(bool noAutoBuild, bool tabForSingleTab)
        {
            this.bNoAutoBuild = noAutoBuild;
            this.bShouldShowTabsForSingleTab = tabForSingleTab;
        }

        /// <summary>
        /// Adds the no automatic build to options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Organizer init options</returns>
        public static UPOrganizerInitOptions AddNoAutoBuildToOptions(UPOrganizerInitOptions options)
        {
            return new UPOrganizerInitOptions(true, options?.bShouldShowTabsForSingleTab ?? false);
        }

        /// <summary>
        /// Shoulds the show tabs for single tab.
        /// </summary>
        /// <returns>Organizer init options</returns>
        public static UPOrganizerInitOptions ShouldShowTabsForSingleTab()
        {
            return new UPOrganizerInitOptions(false, true);
        }
    }

    /// <summary>
    /// Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPMModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPRightsCheckerDelegate" />
    /// <seealso cref="Aurea.CRM.Services.Delegates.IPageModelControllerDelegate" />
    public class UPOrganizerModelController : UPMModelController,
             UPOfflineRequestDelegate,
             UPRightsCheckerDelegate,
             IPageModelControllerDelegate

    // , UPExecuteWorkflowRequestDelegate
    {
        private const string ActionGroupAddIdentifier = "actiongroup.Add";
        private const string ActionIdentifier = "action.";
        private const string ColorThemeLight = "@light";
        private const string ColorThemeDark = "@dark";
        private const string GroupStartLiteral = "GroupStart";
        private const string GroupEndLiteral = "GroupEnd";
        private const string IconStarEmpty = "Icon:StarEmpty";
        private const string IconStar = "Icon:Star";
        private const string IdentifierPrefixSearch = "Search_";
        private const string KeyConfigName = "ConfigName";
        private const string KeyFullTextSearch = "FullTextSearch";
        private const string KeyGeoSearch = "GeoSearch";
        private const string KeyGlobalSearch = "GlobalSearch";
        private const string KeyInfoArea = "InfoArea";
        private const string KeyLinkRecord = "LinkRecord";
        private const string KeyModus = "Modus";
        private const string KeyMultiSearch = "MultiSearch";
        private const string KeySearch = "Search";
        private const string SubViewOptionHash = "#";

        /// <summary>
        /// The left navigation bar items
        /// </summary>
        protected List<UPMElement> LeftNavigationBarItems;

        /// <summary>
        /// The right naviagtion bar items
        /// </summary>
        protected List<UPMElement> RightNaviagtionBarItems;

        /// <summary>
        /// The save action items
        /// </summary>
        protected List<UPMElement> SaveActionItems;

        /// <summary>
        /// The offline request
        /// </summary>
        protected UPOfflineRequest OfflineRequest;

        /// <summary>
        /// The stay on page after offline request
        /// </summary>
        protected bool stayOnPageAfterOfflineRequest;

        /// <summary>
        /// The online data
        /// </summary>
        protected bool onlineData;

        /// <summary>
        /// The action handler
        /// </summary>
        protected OrganizerActionHandler ActionHandler;

        /// <summary>
        /// The page context dictionary
        /// </summary>
        protected Dictionary<string, object> pageContextDictionary;

        /// <summary>
        /// The rights checker
        /// </summary>
        protected UPRightsChecker rightsChecker;

        /// <summary>
        /// The delete record view reference
        /// </summary>
        protected ViewReference deleteRecordViewReference;

        /// <summary>
        /// The page model controllers
        /// </summary>
        public List<UPPageModelController> PageModelControllers { get; }

        /// <summary>
        /// Gets or sets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; protected set; }

        /// <summary>
        /// Gets or sets the nav controller identifier.
        /// </summary>
        /// <value>
        /// The nav controller identifier.
        /// </value>
        public int NavControllerId { get; set; }

        /// <summary>
        /// Gets or sets the close organizer delegate.
        /// </summary>
        /// <value>
        /// The close organizer delegate.
        /// </value>
        public UPMCloseOrganizerDelegate CloseOrganizerDelegate { get; set; }

        /// <summary>
        /// Gets the organizer.
        /// </summary>
        /// <value>
        /// The organizer.
        /// </value>
        public UPMOrganizer Organizer => (UPMOrganizer)this.TopLevelElement;

        /// <summary>
        /// Gets or sets the organizer header action items.
        /// </summary>
        /// <value>
        /// The organizer header action items.
        /// </value>
        public List<UPMElement> OrganizerHeaderActionItems { get; set; }

        /// <summary>
        /// Gets the organizer header quick action items.
        /// </summary>
        /// <value>
        /// The organizer header quick action items.
        /// </value>
        public List<UPMElement> OrganizerHeaderQuickActionItems { get; private set; }

        /// <summary>
        /// Gets the HTML string.
        /// </summary>
        /// <value>
        /// The HTML string.
        /// </value>
        public string HtmlString { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [should show tabs for single tab].
        /// </summary>
        /// <value>
        /// <c>true</c> if [should show tabs for single tab]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldShowTabsForSingleTab { get; set; }

        /// <summary>
        /// Gets or sets the parent swipe page record controller.
        /// </summary>
        /// <value>
        /// The parent swipe page record controller.
        /// </value>
        public ISwipePageRecordController ParentSwipePageRecordController { get; set; }

        /// <summary>
        /// Gets the organizer status.
        /// </summary>
        /// <value>
        /// The organizer status.
        /// </value>
        public UPMStatus OrganizerStatus { get; private set; }

        /// <summary>
        /// Gets or sets the switch to tab at index after tabs loaded.
        /// </summary>
        /// <value>
        /// The switch to tab at index after tabs loaded.
        /// </value>
        public UPOrganizerViewSwitchToIndex SwitchToTabAtIndexAfterTabsLoaded { get; set; }

        /// <summary>
        /// Gets or sets the synchronize record action.
        /// </summary>
        /// <value>
        /// The synchronize record action.
        /// </value>
        public UPMOrganizerAction SyncRecordAction { get; set; }

        /// <summary>
        /// Gets the organizer test delegate.
        /// </summary>
        /// <value>
        /// The organizer test delegate.
        /// </value>
        public UPOrganizerModelControllerTestDelegate OrganizerTestDelegate { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is edit organizer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is edit organizer; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsEditOrganizer => false;

        /// <summary>
        /// Gets a value indicating whether [close organizer when leaving].
        /// </summary>
        /// <value>
        /// <c>true</c> if the multipleOrganizerManager will close this organizer when switchted to another.
        /// </value>
        public virtual bool CloseOrganizerWhenLeaving => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public UPOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
        {
            this.ViewReference = viewReference;
            this.PageModelControllers = new List<UPPageModelController>();
            this.LeftNavigationBarItems = new List<UPMElement>();
            this.RightNaviagtionBarItems = new List<UPMElement>();
            this.OrganizerHeaderActionItems = new List<UPMElement>();
            this.SaveActionItems = new List<UPMElement>();
            this.ShouldShowTabsForSingleTab = options?.bShouldShowTabsForSingleTab ?? false;

            if (!(options?.bNoAutoBuild ?? false))
            {
                this.BuildPagesFromViewReference();
            }

            Messenger.Default.Register<OrganizerManagerMessage>(this, OrganizerManagerMessageKey.DidLeaveOrganizer, this.DidLeftNavController);
        }

        /// <summary>
        /// Gets previous organizer model controller
        /// </summary>
        public UPOrganizerModelController PreviousOrganizerModelController { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerModelController"/> class.
        /// </summary>
        /// <param name="organizerTestDelegate">The organizer test delegate.</param>
        public UPOrganizerModelController(UPOrganizerModelControllerTestDelegate organizerTestDelegate)
        {
            this.OrganizerTestDelegate = organizerTestDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOrganizerModelController"/> class.
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        public UPOrganizerModelController(string htmlString)
        {
            this.HtmlString = htmlString;
            this.PageModelControllers = new List<UPPageModelController>();
            this.BuildPagesFromViewReference();

            Messenger.Default.Register<OrganizerManagerMessage>(this, OrganizerManagerMessageKey.DidLeaveOrganizer, this.DidLeftNavController);
        }

        /// <summary>
        /// Adds the close multi organizer action.
        /// </summary>
        public void AddCloseMultiOrganizerAction()
        {
            UPMOrganizerAction action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId(@"CloseOrganizerAction"));
            action.LabelText = LocalizedString.TextClose;
            action.SetTargetAction(UPMultipleOrganizerManager.CurrentOrganizerManager, UPMultipleOrganizerManager.CurrentOrganizerManager.CloseNavController);
            this.LeftNavigationBarItems.Add(action);
        }

        /// <summary>
        /// Adds the left navigation bar action item.
        /// </summary>
        /// <param name="actionItem">The action item.</param>
        public void AddLeftNavigationBarActionItem(UPMElement actionItem)
        {
            this.LeftNavigationBarItems.Add(actionItem);
        }

        /// <summary>
        /// Adds the right navigation bar action item.
        /// </summary>
        /// <param name="actionItem">The action item.</param>
        public void AddRightNavigationBarActionItem(UPMElement actionItem)
        {
            this.RightNaviagtionBarItems.Add(actionItem);
        }

        /// <summary>
        /// Adds the organizer header action item.
        /// </summary>
        /// <param name="actionItem">The action item.</param>
        public void AddOrganizerHeaderActionItem(UPMElement actionItem)
        {
            this.OrganizerHeaderActionItems.Add(actionItem);
        }

        /// <summary>
        /// Adds the quick organizer header action item.
        /// </summary>
        /// <param name="actionItem">The action item.</param>
        public void AddQuickOrganizerHeaderActionItem(UPMElement actionItem)
        {
            if (this.OrganizerHeaderQuickActionItems == null)
            {
                this.OrganizerHeaderQuickActionItems = new List<UPMElement>();
            }

            this.OrganizerHeaderQuickActionItems.Add(actionItem);
        }

        /// <summary>
        /// Organizers the index of the header action at group separator for.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns></returns>
        public bool OrganizerHeaderActionAtGroupSeparatorForIndex(int itemIndex)
        {
            int index = 0;
            bool changeSection = true;
            foreach (UPMElement element in this.OrganizerHeaderActionItems)
            {
                if (element is UPMOrganizerActionGroup)
                {
                    UPMOrganizerActionGroup group = (UPMOrganizerActionGroup)element;
                    if (itemIndex < index + group.Children.Count)
                    {
                        return itemIndex - index == 0;
                    }

                    index += group.Children.Count;
                    changeSection = true;
                }
                else if (element is UPMOrganizerAction)
                {
                    if (index == itemIndex)
                    {
                        return changeSection;
                    }

                    changeSection = false;
                    index++;
                }

                if (index > itemIndex)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Organizers the index of the header action at group separator text for.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns></returns>
        public string OrganizerHeaderActionAtGroupSeparatorTextForIndex(int itemIndex)
        {
            int index = 0;
            string groupTitle = null;

            foreach (UPMElement element in this.OrganizerHeaderActionItems)
            {
                if (element is UPMOrganizerActionGroup)
                {
                    UPMOrganizerActionGroup group = element as UPMOrganizerActionGroup;
                    if (itemIndex < (index + group.Children.Count))
                    {
                        groupTitle = group.Title;
                        return groupTitle;
                    }

                    index += group.Children.Count;
                }
                else if (element is UPMOrganizerAction)
                {
                    if (index == itemIndex)
                    {
                        return groupTitle;
                    }

                    index++;
                }

                if (index > itemIndex)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Organizers the header group independent item count.
        /// </summary>
        /// <returns></returns>
        public int OrganizerHeaderGroupIndependentItemCount()
        {
            int count = 0;
            foreach (UPMElement element in this.OrganizerHeaderActionItems)
            {
                if (element is UPMOrganizerActionGroup)
                {
                    count += ((UPMOrganizerActionGroup)element).Children.Count;
                }
                else if (element is UPMOrganizerAction)
                {
                    count++;
                }
            }

            count += this.OrganizerHeaderQuickActionItems.Count;
            return count;
        }

        /// <summary>
        /// Adds the action buttons from header record identification.
        /// </summary>
        /// <param name="headerConfig">The header configuration.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void AddActionButtonsFromHeaderRecordIdentification(UPConfigHeader headerConfig, string recordIdentification)
        {
            UPMOrganizerActionGroup currentGroup = null;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            UPConfigHeader quickActionConfig = configStore.HeaderByName($"{headerConfig?.UnitName}.QuickActions[crmpad]") ??
                                               configStore.HeaderByName($"{headerConfig?.UnitName}.QuickActions");

            AddQuickOrganizerHeaderButtons(quickActionConfig, configStore, currentGroup, recordIdentification);
            currentGroup = AddOrganizerHeaderButtons(headerConfig,configStore,currentGroup,recordIdentification);

            if (currentGroup != null && currentGroup.Children.Any())
            {
                AddOrganizerHeaderActionItem(currentGroup);
            }
        }

        /// <summary>
        /// Disables all action items.
        /// </summary>
        /// <param name="disable">if set to <c>true</c> [disable].</param>
        public void DisableAllActionItems(bool disable)
        {
            // First disable all actions
            this.Disable(disable, this.LeftNavigationBarItems);
            this.Disable(disable, this.RightNaviagtionBarItems);
            this.Disable(disable, this.OrganizerHeaderActionItems);

            List<IIdentifier> actionIdentifiers = this.LeftNavigationBarItems.Select(actionElement => actionElement.Identifier).ToList();
            actionIdentifiers.AddRange(this.RightNaviagtionBarItems.Select(actionElement => actionElement.Identifier));
            actionIdentifiers.AddRange(this.OrganizerHeaderActionItems.Select(actionElement => actionElement.Identifier));

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, actionIdentifiers, null);
        }

        /// <summary>
        /// Disables the save action items.
        /// </summary>
        /// <param name="disable">if set to <c>true</c> [disable].</param>
        public void DisableSaveActionItems(bool disable)
        {
            if (this.SaveActionItems.Count == 0)
            {
                return;
            }

            this.Disable(disable, this.SaveActionItems);
            List<IIdentifier> actionIdentifiers = this.SaveActionItems.Select(actionElement => actionElement.Identifier).ToList();

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, actionIdentifiers, null);
        }

        /// <summary>
        /// Enables the action items disable action items.
        /// </summary>
        /// <param name="enabledActionItems">The enabled action items.</param>
        /// <param name="disabledActionItems">The disabled action items.</param>
        protected void EnableActionItemsDisableActionItems(List<UPMElement> enabledActionItems, List<UPMElement> disabledActionItems)
        {
            // First disable all actions
            this.Disable(false, enabledActionItems);
            this.Disable(true, disabledActionItems);

            List<IIdentifier> actionIdentifiers = enabledActionItems.Select(actionElement => actionElement.Identifier).ToList();
            actionIdentifiers.AddRange(disabledActionItems.Select(actionElement => actionElement.Identifier));

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, actionIdentifiers, null);
        }

        /// <summary>
        /// Actions the item.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns></returns>
        protected UPMOrganizerAction ActionItem(StringIdentifier identifier)
        {
            foreach (UPMElement actionElement in this.OrganizerHeaderActionItems)
            {
                if (actionElement is UPMOrganizerAction)
                {
                    if (actionElement.Identifier.MatchesIdentifier(identifier))
                    {
                        return (UPMOrganizerAction)actionElement;
                    }
                }
                else if (actionElement is UPMOrganizerActionGroup)
                {
                    foreach (UPMElement action in ((UPMOrganizerActionGroup)actionElement).Children)
                    {
                        if (action.Identifier.MatchesIdentifier(identifier))
                        {
                            return (UPMOrganizerAction)action;
                        }
                    }
                }
            }

            foreach (UPMElement actionElement in this.LeftNavigationBarItems)
            {
                if (actionElement is UPMOrganizerAction)
                {
                    if (actionElement.Identifier.MatchesIdentifier(identifier))
                    {
                        return (UPMOrganizerAction)actionElement;
                    }
                }
                else if (actionElement is UPMOrganizerActionGroup)
                {
                    foreach (UPMElement action in ((UPMOrganizerActionGroup)actionElement).Children)
                    {
                        if (action.Identifier.MatchesIdentifier(identifier))
                        {
                            return (UPMOrganizerAction)action;
                        }
                    }
                }
            }

            foreach (UPMElement actionElement in this.RightNaviagtionBarItems)
            {
                if (actionElement is UPMOrganizerAction)
                {
                    if (actionElement.Identifier.MatchesIdentifier(identifier))
                    {
                        return (UPMOrganizerAction)actionElement;
                    }
                }
                else if (actionElement is UPMOrganizerActionGroup)
                {
                    foreach (UPMElement action in ((UPMOrganizerActionGroup)actionElement).Children)
                    {
                        if (action.Identifier.MatchesIdentifier(identifier))
                        {
                            return (UPMOrganizerAction)action;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Closes the organizer allowed with delegate.
        /// </summary>
        /// <param name="closeOrganizerDelegate">The close organizer delegate.</param>
        /// <returns></returns>
        public virtual bool CloseOrganizerAllowedWithDelegate(UPMCloseOrganizerDelegate closeOrganizerDelegate)
        {
            return true;
        }

        /// <summary>
        /// Organizers the will appear.
        /// </summary>
        public void OrganizerWillAppear()
        {
            Messenger.Default.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidUpdateRecords, this.SyncManagerDidUpdateRecords);
            Messenger.Default.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishSingleRecordSync, this.DidFinishSingleRecordSyncNotification);

            Messenger.Default.Register<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidPushViewController, this.PushCompleted);
            Messenger.Default.Register<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidPopViewController, this.PopCompleted);
            Messenger.Default.Register<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidSwitchViewController, this.SwitchCompleted);

            UPMultipleOrganizerManager.CurrentOrganizerManager.SetEditingForNavControllerId(this.IsEditOrganizer, this.NavControllerId);
        }

        /// <summary>
        /// Organizers the will disappear.
        /// </summary>
        public void OrganizerWillDisappear()
        {
            Messenger.Default.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidUpdateRecords);
            Messenger.Default.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishSingleRecordSync);

            Messenger.Default.Unregister<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidPushViewController);
            Messenger.Default.Unregister<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidSwitchViewController);
            Messenger.Default.Unregister<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidPopViewController);
        }

        /// <summary>
        /// Pushes the completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PushCompleted(object sender)
        {
            if (this.Organizer.Status is UPMProgressStatus)
            {
                this.Organizer.Status = null;
            }

            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, null);
        }

        /// <summary>
        /// Pops the completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void PopCompleted(object sender)
        {
            if (this.Organizer.Status is UPMProgressStatus)
            {
                this.Organizer.Status = null;
            }

            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, null);
        }

        /// <summary>
        /// Switches the completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void SwitchCompleted(object sender)
        {
            if (this.Organizer.Status is UPMProgressStatus)
            {
                this.Organizer.Status = null;
            }

            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, null);
        }

        /// <summary>
        /// Organizers the index of the header action at group independent.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns></returns>
        public UPMOrganizerAction OrganizerHeaderActionAtGroupIndependentIndex(int itemIndex)
        {
            if (this.OrganizerHeaderQuickActionItems.Count > itemIndex)
            {
                return this.OrganizerHeaderQuickActionItems[itemIndex] as UPMOrganizerAction;
            }

            int index = this.OrganizerHeaderQuickActionItems.Count;

            foreach (UPMElement element in this.OrganizerHeaderActionItems)
            {
                if (element is UPMOrganizerActionGroup)
                {
                    UPMOrganizerActionGroup group = element as UPMOrganizerActionGroup;
                    if (itemIndex < index + group.Children.Count)
                    {
                        return group.Children[itemIndex - index] as UPMOrganizerAction;
                    }

                    index += group.Children.Count;
                }
                else if (element is UPMOrganizerAction)
                {
                    if (index == itemIndex)
                    {
                        return element as UPMOrganizerAction;
                    }

                    index++;
                }

                if (index > itemIndex)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Disables the specified disable.
        /// </summary>
        /// <param name="disable">if set to <c>true</c> [disable].</param>
        /// <param name="array">The array.</param>
        public void Disable(bool disable, List<UPMElement> array)
        {
            foreach (UPMElement actionElement in array)
            {
                if (actionElement is UPMOrganizerAction)
                {
                    ((UPMOrganizerAction)actionElement).Enabled = !disable;
                }
                else if (actionElement is UPMOrganizerActionGroup)
                {
                    ((UPMOrganizerActionGroup)actionElement).Enabled = !disable;
                }
            }
        }

        /// <summary>
        /// Colors for information area with identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <returns></returns>
        public AureaColor ColorForInfoAreaWithId(string infoAreaId)
        {
            var infoAreaConfig = ConfigurationUnitStore.DefaultStore.InfoAreaConfigById(infoAreaId);

            string colorKey = infoAreaConfig?.ColorKey;
            return !string.IsNullOrEmpty(colorKey) ? AureaColor.ColorWithString(colorKey) : null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [online data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online data]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool OnlineData
        {
            get
            {
                return this.onlineData;
            }

            set
            {
                this.onlineData = value;
                // this.Organizer.StatusIndicatorIcon = this.onlineData ? UIImage.upXXImageNamed(@"crmpad-OrganizerHeader-Cloud") : null;     // CRM-5007
            }
        }

        /// <summary>
        /// Resets the with reason.
        /// </summary>
        /// <param name="_reason">The reason.</param>
        public override void ResetWithReason(ModelControllerResetReason _reason)
        {
            base.ResetWithReason(_reason);

            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                pageModelController.ResetWithReason(_reason);
            }
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            return null;
        }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        /// <param name="appliedIdentifiers">The applied identifiers.</param>
        public override void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
            base.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);

            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                pageModelController.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            }
        }

        /// <summary>
        /// Deallocs this instance.
        /// </summary>
        public virtual void Dealloc()
        {
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// Did left nav controller.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void DidLeftNavController(OrganizerManagerMessage notification)
        {
            int navControllerId = notification.ControllerId;

            if (navControllerId > 0 && navControllerId == this.NavControllerId)
            {
                this.ResetWithReason(ModelControllerResetReason.MultiOrganizerSwitch);
            }
        }

        private void SyncManagerDidUpdateRecords(SyncManagerMessage notification)
        {
            List<string> recordIdentifications = notification.State.ValueOrDefault(Core.Session.Constants.KUPSyncManagerModifiedRecordIdentifications) as List<string>;

            if (recordIdentifications != null)
            {
                List<IIdentifier> recordIdentifiers = new List<IIdentifier>();

                foreach (string identification in recordIdentifications)
                {
                    RecordIdentifier recordIdentifier = new RecordIdentifier(identification);
                    recordIdentifiers.Add(recordIdentifier);
                }

                this.ProcessChanges(recordIdentifiers);
            }
        }

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public virtual void BuildPagesFromViewReference()
        {
            if (this.HtmlString != null)
            {
                this.BuildHtmlDataPage();
            }
            else
            {
                switch (this.ViewReference?.ViewName)
                {
                    case @"RecordListView":
                    case @"CalendarView":
                        this.BuildSearchOrganizerPages();
                        break;
                    //case @"CalendarView":
                    //    this.BuildOrganizerPages();
                    //    break;

                    case @"WebContentView":
                    case @"ConfirmWebContentView":
                        this.BuildWebContentOrganizerPages();
                        break;

                    case @"Analysis":
                    case @"Query":
                        this.BuildAnalysisPages();
                        break;

                    default:
                        this.BuildOrganizerPages();
                        break;
                }
            }
        }

        /// <summary>
        /// Builds the HTML data page.
        /// </summary>
        public void BuildHtmlDataPage()
        {
            UPWebContentPageModelController pageModelController = new UPWebContentPageModelController(this.HtmlString);
            UPMOrganizer webOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId(@"Report"))
            {
                SubtitleText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicHTMLPageDefaultOrganizerTitle),
                DisplaysTitleText = false,
                LineCountAdditionalTitletext = 0,
                DisplaysImage = false,
                ExpandFound = true
            };

            Page page = pageModelController.Page;
            page.LabelText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicHTMLPageDefaultOrganizerTitle);
            this.AddPageModelController(pageModelController);
            webOrganizer.AddPage(page);
            this.TopLevelElement = webOrganizer;
        }

        /// <summary>
        /// Builds the web content organizer pages.
        /// </summary>
        public void BuildWebContentOrganizerPages()
        {
            UPWebContentPageModelController webContentPageModelController = this.ViewReference.ViewName == @"ConfirmWebContentView"
                                           ? new SerialEntryWebContentModelController(this.ViewReference)
                                           : new UPWebContentPageModelController(this.ViewReference);

            UPMOrganizer webOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId(@"Report"))
            {
                SubtitleText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicWebReportDefaultOrganizerTitle),
                DisplaysTitleText = false,
                LineCountAdditionalTitletext = 0,
                DisplaysImage = false,
                ExpandFound = true
            };

            string headerName = this.ViewReference.ContextValueForKey(@"HeaderName");
            IConfigurationUnitStore store = ConfigurationUnitStore.DefaultStore;
            UPConfigHeader header = store.HeaderByName(headerName);

            if (!string.IsNullOrEmpty(header?.Label))
            {
                webOrganizer.TitleText = header.Label;
            }

            Page page = webContentPageModelController.Page;
            page.LabelText = LocalizedString.Localize(LocalizationKeys.TextGroupBasic, LocalizationKeys.KeyBasicWebReportDefaultOrganizerTitle);
            this.AddPageModelController(webContentPageModelController);
            webOrganizer.AddPage(page);
            this.TopLevelElement = webOrganizer;

            if (webContentPageModelController.AllowsXMLExport)
            {
                UPMOrganizerAction exportXMLAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId(@"action.exportXML"));
                exportXMLAction.SetTargetAction(webContentPageModelController, webContentPageModelController.ExportXML);
                exportXMLAction.LabelText = @"Export XML";
                this.AddOrganizerHeaderActionItem(exportXMLAction);
            }

            int count = header?.NumberOfSubViews ?? 0;
            for (int i = 0; i < count; i++)
            {
                UPConfigHeaderSubView subViewDef = header.SubViewAtIndex(i);

                if (i == 0 && subViewDef.Options == @"#")
                {
                    page.LabelText = subViewDef.Label;
                    continue;
                }

                ViewReference subViewViewReference = subViewDef.ViewReference;
                UPPageModelController pageModelController = this.PageForViewReference(subViewViewReference);

                if (pageModelController != null)
                {
                    pageModelController.Page.LabelText = subViewDef.Label;
                    this.AddPageModelController(pageModelController);
                }
            }

            this.AddActionButtonsFromHeaderRecordIdentification(header, string.Empty);
        }

        /// <summary>
        /// Builds the analysis pages.
        /// </summary>
        public void BuildAnalysisPages()
        {
            UPMOrganizer analysisOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId(@"Analysis"));

            bool isQuery = this.ViewReference.ViewName == @"Query";
            analysisOrganizer.TitleText = isQuery ? LocalizedString.TextAnalysesQuery : LocalizedString.TextAnalysesAnalysis;

            analysisOrganizer.SubtitleText = string.Format(isQuery ? LocalizedString.TextAnalysesQueryWithName : LocalizedString.TextAnalysesAnalysisWithName,
                                            this.ViewReference.ContextValueForKey(isQuery ? @"Query" : @"Analysis"));

            AnalysisPageModelController analysisPageController = new AnalysisPageModelController(this.ViewReference);
            Page page = analysisPageController.Page;
            analysisOrganizer.AddPage(page);
            this.AddPageModelController(analysisPageController);
            page.LabelText = isQuery ? LocalizedString.TextAnalysesQuery : LocalizedString.TextAnalysesAnalysis;
            this.TopLevelElement = analysisOrganizer;
            this.Organizer.DisplaysTitleText = false;
            this.Organizer.LineCountAdditionalTitletext = 0;
            this.Organizer.DisplaysImage = false;
            this.Organizer.ExpandFound = true;
        }

        /// <summary>
        /// Builds the search organizer pages.
        /// </summary>
        public void BuildSearchOrganizerPages()
        {
            var infoAreaId = ViewReference.ContextValueForKey(KeyInfoArea);
            var searchOrganizer = (UPMOrganizer)null;

            var configStore = ConfigurationUnitStore.DefaultStore;
            if (string.IsNullOrWhiteSpace(infoAreaId) && ViewReference.ContextValueForKey(KeyModus) == KeyGlobalSearch)
            {
                CreateGlobalSearchPageModelController(searchOrganizer);
            }
            else if (string.IsNullOrWhiteSpace(infoAreaId) && ViewReference.ContextValueForKey(KeyModus) == KeyGeoSearch)
            {
                CreateGeoSearchPageModelController(searchOrganizer);
            }
            else if (string.IsNullOrWhiteSpace(infoAreaId) && ViewReference.ContextValueForKey(KeyModus) == KeyMultiSearch)
            {
                CreateMultiSearchPageModelController(searchOrganizer);
            }
            else
            {
                CreateDefaultSearchPageModelController(searchOrganizer, configStore, infoAreaId);
            }

            Organizer.DisplaysTitleText = false;
            Organizer.LineCountAdditionalTitletext = 0;
            Organizer.DisplaysImage = false;
            Organizer.ExpandFound = true;
            Organizer.OrganizerColor = ColorForInfoAreaWithId(ViewReference.ContextValueForKey(KeyInfoArea));
        }

        /// <summary>
        /// Builds the organizer pages.
        /// </summary>
        public void BuildOrganizerPages()
        {
            var infoAreaId = ViewReference?.ContextValueForKey("InfoArea");
            var configName = ViewReference?.ContextValueForKey("ConfigName");
            var recordIdentification = ViewReference?.ContextValueForKey("RecordId");

            if (string.IsNullOrWhiteSpace(infoAreaId) && string.IsNullOrWhiteSpace(recordIdentification))
            {
                infoAreaId = recordIdentification.InfoAreaId();
            }

            StringIdentifier identifier;
            if (!string.IsNullOrWhiteSpace(configName))
            {
                identifier = StringIdentifier.IdentifierWithStringId($@"Organizer{configName}");
            }
            else if (!string.IsNullOrWhiteSpace(infoAreaId))
            {
                identifier = StringIdentifier.IdentifierWithStringId($@"Organizer{infoAreaId}");
            }
            else
            {
                identifier = StringIdentifier.IdentifierWithStringId(@"OrganizerDefault");
            }

            var organizer = new UPMOrganizer(identifier);
            TopLevelElement = organizer;

            if (string.IsNullOrWhiteSpace(configName) && !string.IsNullOrWhiteSpace(infoAreaId))
            {
                configName = infoAreaId;
            }

            if (!string.IsNullOrWhiteSpace(configName))
            {
                BuildOrganizerPagesUsingConfigName(organizer, configName, infoAreaId, recordIdentification);
            }
            else
            {
                var pageModelController = PageForViewReference(ViewReference);
                if (pageModelController != null)
                {
                    var page = pageModelController.Page;
                    page.LabelText = LocalizedString.TextTabOverview;

                    AddPageModelController(pageModelController);
                    organizer.AddPage(page);
                }
            }

            Organizer.DisplaysTitleText = false;
            Organizer.LineCountAdditionalTitletext = 0;
            Organizer.DisplaysImage = false;
            Organizer.ExpandFound = true;
            Organizer.OrganizerColor = ViewReference != null
                ? ColorForInfoAreaWithId(ViewReference.ContextValueForKey("InfoArea"))
                : null;
        }

        private void BuildOrganizerPagesUsingConfigName(UPMOrganizer organizer, string configName, string infoAreaId, string recordIdentification)
        {
            string organizerTitle = null;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigHeader header = configStore.HeaderByNameFromGroup(@"Expand", configName);

            if (header == null && configName != infoAreaId)
            {
                header = configStore.HeaderByNameFromGroup(@"Expand", infoAreaId);
            }

            if (header != null)
            {
                organizerTitle = header.Label;
            }

            if (string.IsNullOrEmpty(organizerTitle))
            {
                organizer.SubtitleText = organizerTitle;
            }
            else
            {
                InfoArea infoAreaConfig = configStore.InfoAreaConfigById(infoAreaId);
                organizer.SubtitleText = infoAreaConfig != null
                    ? infoAreaConfig.SingularName
                    : UPCRMDataStore.DefaultStore.TableInfoForInfoArea(infoAreaId).Label;
            }

            UPPageModelController pageModelController = this.PageForViewReference(this.ViewReference);

            Page page = pageModelController.Page;
            page.LabelText = LocalizedString.TextTabOverview;

            this.AddPageModelController(pageModelController);
            organizer.AddPage(page);

            int count = header?.NumberOfSubViews ?? 0;
            for (int i = 0; i < count; i++)
            {
                UPConfigHeaderSubView subViewDef = header.SubViewAtIndex(i);
                if (i == 0 && subViewDef.Options == @"#")
                {
                    page.LabelText = subViewDef.Label;
                    continue;
                }

                ViewReference subViewViewReference = subViewDef.ViewReference;

                if (subViewViewReference == null)
                {
                    continue;
                }

                UPPageModelController subSearchPageController = this.PageForViewReference(subViewViewReference);
                Page subSearchPage = subSearchPageController.Page;

                if (subSearchPage == null)
                {
                    continue;
                }

                subSearchPage.LabelText = subViewDef.Label;
                organizer.AddPage(subSearchPage);
                this.AddPageModelController(subSearchPageController);
            }

            if (header != null)
            {
                this.AddActionButtonsFromHeaderRecordIdentification(header, recordIdentification);
            }
        }

        /// <summary>
        /// Organizers from offline request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static UPOrganizerModelController OrganizerFromOfflineRequest(UPOfflineRequest request)
        {
            request.LoadFromOfflineStorage();
            if (request.RequestType == OfflineRequestType.Records)
            {
                if (request.ProcessType == OfflineRequestProcess.EditRecord)
                {
                    return new EditOrganizerModelController((UPOfflineEditRecordRequest)request);
                }

                if (request.ProcessType == OfflineRequestProcess.SerialEntryOrder ||
                                   request.ProcessType == OfflineRequestProcess.SerialEntryPOS ||
                                   request.ProcessType == OfflineRequestProcess.SerialEntry)
                {
                    return new SerialEntryOrganizerWithoutRootModelController((UPOfflineSerialEntryRequest)request);
                }

                if (request.ProcessType == OfflineRequestProcess.Characteristics)
                {
                    return UPCharacteristicsEditOrganizerModelController.Create((UPOfflineCharacteristicsRequest)request);
                }
            }

            return null;
        }

        /// <summary>
        /// Organizers from default action of record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public static UPOrganizerModelController OrganizerFromDefaultActionOfRecordIdentification(string recordIdentification)
        {
            string infoAreaId = recordIdentification.InfoAreaId();
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            InfoArea infoAreaConfig = configStore.InfoAreaConfigById(infoAreaId);
            Menu menu = null;

            if (!string.IsNullOrEmpty(infoAreaConfig?.DefaultAction))
            {
                menu = configStore.MenuByName(infoAreaConfig.DefaultAction);
            }

            if (menu == null)
            {
                menu = configStore.MenuByName(@"SHOWRECORD");
            }

            ViewReference viewReference = menu?.ViewReference.ViewReferenceWith(recordIdentification);

            return viewReference != null ? OrganizerFromViewReference(viewReference) : null;
        }

        /// <summary>
        /// Organizers from view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <returns></returns>
        public static UPOrganizerModelController OrganizerFromViewReference(ViewReference viewReference)
        {
            // Returns the filename of the associated Page UPOrganizerModelController
            UPOrganizerModelController result = null;
            UPOrganizerInitOptions options = null;

            switch (viewReference.ViewName)
            {
                case @"RecordListView":
                    result = viewReference.ContextValueForKey(@"Modus") == @"GeoSearch"
                    ? new UPOrganizerModelController(viewReference)
                    : new UPOrganizerModelController(viewReference, UPOrganizerInitOptions.ShouldShowTabsForSingleTab());
                    break;

                case @"CalendarView":
                    result = new UPOrganizerModelController(viewReference, UPOrganizerInitOptions.ShouldShowTabsForSingleTab());
                    break;

                case @"HistoryListView":
                    result = new HistoryOrganizerModelController(viewReference, UPOrganizerInitOptions.ShouldShowTabsForSingleTab());
                    break;

                case @"RecordView":
                    options = UPOrganizerInitOptions.ShouldShowTabsForSingleTab();
                    var organizerModelController = new DetailOrganizerModelController(viewReference, UPOrganizerInitOptions.AddNoAutoBuildToOptions(options));
                    organizerModelController.Init();
                    result = organizerModelController;
                    break;

                case @"EditView":
                    result = new EditOrganizerModelController(viewReference);
                    break;

                case @"SerialEntry":
                    result = new SerialEntryOrganizerWithoutRootModelController(viewReference);
                    break;

                case @"EditViewSerialEntry":
                    result = new SerialEntryOrganizerModelController(viewReference);
                    break;

                case @"DashboardView":
                    result = new UPDashboardOrganizerModelController(viewReference, null);
                    break;

                case @"WebContentView":
                case @"ConfirmWebContentView":
                    result = new UPOrganizerModelController(viewReference);
                    break;

                case @"SettingsView":
                    // result = new UPSettingsViewOrganizerModelController(_viewReference, null);
                    break;

                case @"SettingsEditView":
                    // result = new UPSettingsEditViewOrganizerModelController(_viewReference, null);
                    break;

                case @"CharacteristicsEditView":
                    result = new UPCharacteristicsEditOrganizerModelController(viewReference);
                    break;

                case @"ObjectivesView":
                    result = new ObjectivesEditOrganizerModelController(viewReference);
                    break;

                case @"DocumentView":
                    options = UPOrganizerInitOptions.ShouldShowTabsForSingleTab();
                    result = new DocumentOrganizerModelController(viewReference, UPOrganizerInitOptions.AddNoAutoBuildToOptions(options));
                    break;

                case @"DataSynchronization":
                    result = new UPDataSynchronizationOrganizerModelController(viewReference, null);
                    break;

                case @"SystemInfo":
                    // result = new UPSystemInfoOrganizerModelController(_viewReference, UPOrganizerInitOptions.ShouldShowTabsForSingleTab();
                    break;

                case @"FirstConflict":
                    // DEMO FUNCTIONALITY - create conflict organizer
                    IOfflineStorage offlineStore = UPOfflineStorage.DefaultStorage;
                    List<UPOfflineRequest> conflictRequests = offlineStore.ConflictRequests;
                    if (conflictRequests == null || conflictRequests.Count == 0)
                    {
                        return null;
                    }

                    result = OrganizerFromOfflineRequest(conflictRequests[0]);
                    break;

                case @"QuestionnaireView":
                    result = new QuestionnaireEditOrganizerModelController(viewReference, null);
                    break;

                case @"QuestionnaireEditView":
                    result = new QuestionnaireEditOrganizerModelController(viewReference, null);
                    break;

                case @"Analysis":
                case @"Query":
                    result = new UPOrganizerModelController(viewReference);
                    break;

                case @"ContactTimesEditView":
                    result = new ContactTimes.ContactTimesOrganizerModelController(viewReference, null);
                    break;

                case @"CircleOfInfluenceTreeView":
                case @"CircleOfInfluenceView":
                case @"DocumentInbox":
                    result = new UPOrganizerModelController(viewReference);
                    break;

                default:
                    return null;
            }

            if (result != null)
            {
                result.NavControllerId = UPMultipleOrganizerManager.CurrentOrganizerManager.CurrentNavControllerId;
            }

            return result;
        }

        /// <summary>
        /// Synchronizes the record.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected override void SyncRecord(ViewReference viewReference)
        {
            ServerSession.CurrentSession.SyncManager.PerformSyncForRecord(viewReference.ContextValueForKey(@"RecordId"));
        }

        /// <summary>
        /// Did finish single record synchronize notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        private async void DidFinishSingleRecordSyncNotification(SyncManagerMessage notification)
        {
            string recordId = this.ViewReference.ContextValueForKey(@"RecordId");

            if (recordId == null || recordId == notification.State.ValueOrDefault(Core.Session.Constants.KUPSyncManagerSingleRecordIdentification) as string)
            {
                if (recordId != null)
                {
                    this.OnlineData = false;
                }

                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(LocalizedString.TextProcessRecordDataWasSync, string.Empty);

#if PORTING
                UPStatusHandler.showErrorMessage(upTextProcessRecordDataWasSync);
#endif
            }
        }

        /// <summary>
        /// Sets the offline request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="stayOnPage">if set to <c>true</c> [stay on page].</param>
        /// <returns></returns>
        public bool SetOfflineRequest(UPOfflineRequest request, bool stayOnPage)
        {
            if (request != null)
            {
                this.OfflineRequest = null;
                this.stayOnPageAfterOfflineRequest = false;
                return true;
            }

            if (this.OfflineRequest != null)
            {
                return false;
            }

            this.OfflineRequest = request;
            this.stayOnPageAfterOfflineRequest = stayOnPage;
            return true;
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected override void DeleteRecord(ViewReference viewReference)
        {
            string recordIdentification = viewReference.ContextValueForKey(@"RecordId");

            if (string.IsNullOrEmpty(recordIdentification))
            {
                this.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, LocalizedString.TextErrorParameterEmpty.Replace("%@", "RecordId"), true);
                return;
            }

            string templateFilterName = viewReference.ContextValueForKey(@"RightsFilter");

            if (!string.IsNullOrEmpty(templateFilterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(templateFilterName);
                if (filter != null)
                {
                    this.rightsChecker = new UPRightsChecker(filter)
                    {
                        Context = viewReference,
                        Selector = this.DeleteRecordWithoutRightsCheck
                    };
                    this.rightsChecker.CheckPermission(recordIdentification, false, this);
                    return;
                }
            }

            this.DeleteRecordWithoutRightsCheck(viewReference);
        }

        /// <summary>
        /// Deletes the record without rights check.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        private async void DeleteRecordWithoutRightsCheck(ViewReference viewReference)
        {
            string recordIdentification = viewReference.ContextValueForKey(@"RecordId");
            if (string.IsNullOrEmpty(recordIdentification))
            {
                this.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, LocalizedString.TextErrorParameterEmpty.Replace("%@", "RecordId"), true);
                return;
            }

            this.deleteRecordViewReference = viewReference;

            if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSet(@"Delete.Ask"))
            {
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                    LocalizedString.TextDeleteRecordMessage,
                    LocalizedString.TextDeleteRecordTitle,
                    LocalizedString.TextYes,
                    LocalizedString.TextNo,
                    c =>
                    {
                        if (c)
                        {
                            this.ReallyDeleteRecord();
                        }
                    });
            }
            else
            {
                this.ReallyDeleteRecord();
            }
        }

        private void ReallyDeleteRecord()
        {
            ViewReference viewReference = this.deleteRecordViewReference;
            this.deleteRecordViewReference = null;
            string recordIdentification = viewReference.ContextValueForKey(@"RecordId");
            UPCRMRecord record = new UPCRMRecord(recordIdentification, @"Delete", null);
            UPOfflineEditRecordRequest request = new UPOfflineOrganizerDeleteRecordRequest(0);

            if (this.SetOfflineRequest(request, viewReference.ContextValueForKey(@".fromPopup") != null))
            {
                string requestModeString = viewReference.ContextValueForKey(@"RequestMode");
                UPOfflineRequestMode requestMode = UPOfflineRequest.RequestModeFromString(requestModeString, UPOfflineRequestMode.OnlineConfirm);

                if (!request.StartRequest(requestMode, new List<UPCRMRecord> { record }, this))
                {
                    this.OfflineRequest = null;
                }
            }
            else
            {
                this.HandleOrganizerActionError(LocalizedString.TextErrorActionNotPossible, LocalizedString.TextErrorActionPending, true);
            }
        }

        /// <summary>
        /// Sends the admin information.
        /// </summary>
        /// <param name="_viewReference">The view reference.</param>
        public void SendAdminInfo(ViewReference _viewReference)
        {
#if PORTING
            // UPMail mail = [[UPMail alloc] init];

            // string eMailAddress = _viewReference.ContextValueForKey(@"eMailAddress");

            // if (eMailAddress.length > 0)
            //        {
            //            [mail addRecipientFromString: eMailAddress forType: ToRecipientType];
            //        }

            // [mail setSubject:[string stringWithFormat:@"CRMpad_Admin_%@_%@", [UPCRMSession currentSession].userName, [string crmValueFromDateTime:[NSDate new]]]];

            // string syncTrace = _viewReference.ContextValueForKey(@"syncStats"];

            // if (syncTrace.length > 0)
            //        {
            //            string syncTraceAttachmentText = [[UPOfflineStorage defaultStorage] syncTraceWithSetting:syncTrace];

            // if (syncTraceAttachmentText.length)
            //            {
            //                NSData data = [syncTraceAttachmentText dataUsingEncoding: NSUTF8StringEncoding];
            // UPMailAttachment attachment = [[UPMailAttachment alloc] initAttachmentData:data mimeType:@"text/plain" fileName:[string stringWithFormat:@"SyncTrace_%@.txt", [UPCRMSession currentSession].userName]];
            //                mail.AddAttachment(attachment);
            //            }
            //        }

            // [this.modelControllerDelegate sendMail:mail modal:true];
#endif
        }

        /// <summary>
        /// Executes the trigger.
        /// </summary>
        /// <param name="_viewReference">The view reference.</param>
        public void ExecuteTrigger(ViewReference _viewReference)
        {
            string recordIdentification = _viewReference.ContextValueForKey(@"RecordId");
            string triggerName = _viewReference.ContextValueForKey(@"TriggerName");

#if PORTING
            // if (string.IsNullOrEmpty(recordIdentification))
            // {
            //    this.handleOrganizerActionError(upTextErrorConfiguration, string.Format(upTextErrorParameterEmpty, @"RecordId"), true);
            //    return;
            // }

            //// create request for server....
            // UPExecuteTriggerServerOperation request = new UPExecuteTriggerServerOperation(triggerName, recordIdentification, this);
            // UPCRMSession.currentSession.executeRequest(request);
#endif
        }

#if PORTING
        // public void ExecuteTriggerRequestDidFailWithError(UPExecuteTriggerServerOperation sender, Exception error)
        // {
        //    if (this.modelControllerDelegate != null)
        //    {
        //        this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
        //    }
        // }

        // public void ExecuteTriggerRequestDidFinishWithResult(UPExecuteTriggerServerOperation sender, object result)
        // {
        //    Dictionary<string, object> resultDictionary = (Dictionary<string, object>)result;

        // object changedFields = resultDictionary[@"ChangedFields"];
        //    object changedRecords = resultDictionary[@"ChangedRecords"];
        //    object messages = resultDictionary[@"Messages"];

        // if (messages != null)
        //    {
        //        // when we have implemented trigger messages ... push messages to organizer
        //    }

        // if (changedFields != null)
        //    {
        //        // when we have implemented interactive trigger ... push changed fields to organizer
        //    }

        // if (changedRecords is List<object>)
        //    {
        //        List<object> changedRecordData = resultDictionary[@"ChangedRecordsData"];
        //        UPCRMRecordSync.SyncRecordSetDefinitions(changedRecordData);

        // List<RecordIdentifier> changedRecordIdentifier = new List<RecordIdentifier>();

        // foreach (NSDictionary record in changedRecords)
        //        {
        //            RecordIdentifier identifier = new RecordIdentifier(record[@"InfoAreaId"], record[@"RecordId"]);
        //            changedRecordIdentifier.Add(identifier);
        //        }

        // //this.performSelectorOnMainThread: @selector(informDelegateAboutChangedRecords:) withObject: changedRecordIdentifier waitUntilDone: true];
        //        UPChangeManager.currentChangeManager.registerChanges(changedRecordIdentifier);
        //    }
        // }
#endif

        /// <summary>
        /// Executes the workflow.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void ExecuteWorkflow(ViewReference viewReference)
        {
            string recordIdentification = viewReference.ContextValueForKey(@"RecordId");
            string workflowName = viewReference.ContextValueForKey(@"WorkflowName");
            string parameters = viewReference.ContextValueForKey(@"Parameters");
            string executionFlags = viewReference.ContextValueForKey(@"ExecutionFlags");

            if (string.IsNullOrEmpty(recordIdentification))
            {
                this.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorParameterEmpty, @"RecordId"), true);
                return;
            }

            // create request for server....
            // UPExecuteWorkflowServerOperation request = new UPExecuteWorkflowServerOperation(workflowName, recordIdentification, parameters, executionFlags, this);

            // ServerSession.CurrentSession().ExecuteRequest(request);
        }

#if PORTING
        // public void ExecuteWorkflowRequestDidFailWithError(UPExecuteWorkflowServerOperation sender, Exception error)
        // {
        //    if (this.modelControllerDelegate != null)
        //    {
        //        this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
        //    }
        // }

        // public void ExecuteWorkflowRequestDidFinishWithResult(UPExecuteWorkflowServerOperation sender, UPExecuteWorkflowResult result)
        // {
        //    if (result.changedRecords.count > 0)
        //    {
        //        List<RecordIdentifier> changedRecordIdentifier = new List<RecordIdentifier>();

        // foreach (string _recordIdentification in result.changedRecords)
        //        {
        //            RecordIdentifier identifier = new RecordIdentifier(_recordIdentification);
        //            changedRecordIdentifier.Add(identifier);
        //        }

        // //this.performSelectorOnMainThread: @selector(informDelegateAboutChangedRecords:) withObject: changedRecordIdentifier waitUntilDone: true];
        //        UPChangeManager.currentChangeManager.registerChanges(changedRecordIdentifier);
        //    }
        // }
#endif

        /// <summary>
        /// Updates the organizer.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void UpdateOrganizer(ViewReference viewReference)
        {
            List<IIdentifier> changedIdentifiers = new List<IIdentifier>();

            string _recordIdentification = viewReference.ContextValueForKey(@"RecordId");

            if (!string.IsNullOrEmpty(_recordIdentification))
            {
                changedIdentifiers.Add(new RecordIdentifier(_recordIdentification));
            }
            // else if (this.respondsToSelector:@selector(_recordIdentification))
            // {
            //    string rid = this.performSelector:@selector(_recordIdentification);
            //    if (!string.IsNullOrEmpty(rid))
            //    {
            //        changedIdentifiers.Add(new RecordIdentifier(rid));
            //    }
            // }

            if (changedIdentifiers.Count > 0)
            {
                this.InformDelegateAboutChangedRecords(changedIdentifiers);
            }
        }

        /// <summary>
        /// Modifies the record.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected override void ModifyRecord(ViewReference viewReference)
        {
            string recordIdentification = viewReference.ContextValueForKey(@"RecordId");
            string rightsFilterName = viewReference.ContextValueForKey(@"RightsFilter");
            string requestModeString = viewReference.ContextValueForKey(@"RequestMode");

            UPOfflineRequestMode requestMode = UPOfflineRequest.RequestModeFromString(requestModeString, UPOfflineRequestMode.OnlineOnly);

            if (!string.IsNullOrEmpty(rightsFilterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(rightsFilterName);
                if (filter != null)
                {
                    this.rightsChecker = new UPRightsChecker(filter)
                    {
                        Context = viewReference,
                        Selector = this.ModifyRecordWithoutRightsCheck
                    };
                    this.rightsChecker.CheckPermission(recordIdentification, requestMode == UPOfflineRequestMode.OnlineOnly, this);
                    return;
                }
            }

            this.ModifyRecordWithoutRightsCheck(viewReference);
        }

        /// <summary>
        /// Modifies the record without rights check.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        private void ModifyRecordWithoutRightsCheck(ViewReference viewReference)
        {
            this.ActionHandler = new ModifyRecordActionHandler(this, viewReference);
            this.ActionHandler.Execute();
        }

        /// <summary>
        /// Switches the tab. Called by the Page Call SwitchTabIndexAction.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void SwitchTab(ViewReference viewReference)
        {
            int tabIndex = Convert.ToInt32(viewReference.ContextValueForKey(@"TabIndex"));
            if (tabIndex > 0 && tabIndex <= this.PageModelControllers.Count)
            {
                this.modelControllerDelegate.SwitchToPageAtIndex(tabIndex - 1);
            }
        }

        /// <summary>
        /// Clients the email.
        /// </summary>
        /// <param name="emailViewReference">The email view reference.</param>
        public void ClientEmail(ViewReference emailViewReference)
        {
            string emailFieldGroup = emailViewReference.ContextValueForKey(@"EmailFieldgroup");
            string emailRecordId = emailViewReference.ContextValueForKey(@"RecordId");

            // this.mail = [UPMail new];
            // [this.mail fetchFromEmailTemplateFieldGroup:emailFieldGroup forRecord:emailRecordId delegate:self];
        }

        // public void MailFetcherFinished(UPMail mail, Exception error)
        // {
        //    if (this.mail == mail && !error)
        //    {
        //        if (!error)
        //        {
        //            this.modelControllerDelegate.sendMail(mail, false);
        //        }
        //        this.mail = null;
        //    }
        // }

        /// <summary>
        /// Alerts the specified view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public async void Alert(ViewReference viewReference)
        {
            string titleTextgroupName = viewReference.ContextValueForKey(@"TitleTextGroupName");
            string titleTextNr = viewReference.ContextValueForKey(@"TitleTextNr");
            string titleDefaultText = viewReference.ContextValueForKey(@"TitleDefaultText");

            string textgroupName = viewReference.ContextValueForKey(@"TextGroupName");
            string textNr = viewReference.ContextValueForKey(@"TextNr");
            string defaultText = viewReference.ContextValueForKey(@"DefaultText");

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            string titleText = !string.IsNullOrEmpty(titleTextgroupName)
                ? configStore.TextByGroupIndexDefaultText(titleTextgroupName, Convert.ToInt32(titleTextNr), titleDefaultText)
                : titleDefaultText;

            if (string.IsNullOrEmpty(titleText))
            {
                titleText = string.Empty;
            }

            string alertText = !string.IsNullOrEmpty(textgroupName)
                 ? configStore.TextByGroupIndexDefaultText(textgroupName, Convert.ToInt32(textNr), defaultText)
                 : defaultText;

            if (!string.IsNullOrEmpty(alertText))
            {
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(alertText, titleText);
            }
        }

        /// <summary>
        /// Backs the specified view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void Back(ViewReference viewReference)
        {
            this.modelControllerDelegate.PopToPreviousContentViewController();
        }

        /// <summary>
        /// News the in background.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void NewInBackground(ViewReference viewReference)
        {
            string recordIdentification = viewReference.ContextValueForKey(@"LinkRecordId");
            string rightsFilterName = viewReference.ContextValueForKey(@"RightsFilter");

            string requestModeString = viewReference.ContextValueForKey(@"RequestMode");
            // UPOfflineRequestMode requestMode = new UPOfflineRequest(requestModeString, UPOfflineRequestMode.OnlineOnly);

            if (!string.IsNullOrEmpty(rightsFilterName))
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(rightsFilterName);
                if (filter != null)
                {
                    this.rightsChecker = new UPRightsChecker(filter);
                    this.rightsChecker.Context = viewReference;
                    this.rightsChecker.Selector = this.NewInBackgroundWithoutRightsCheck;
                    // this.rightsChecker.CheckPermission(recordIdentification, (requestMode == UPOfflineRequestModeOnlineOnly), this);
                    return;
                }
            }

            this.NewInBackgroundWithoutRightsCheck(viewReference);
        }

        /// <summary>
        /// News the in background without rights check.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        private void NewInBackgroundWithoutRightsCheck(ViewReference viewReference)
        {
            this.ActionHandler = new NewInBackgroundActionHandler(this, viewReference);
            this.ActionHandler.Execute();
        }

        /// <summary>
        /// Handles the organizer action.
        /// </summary>
        /// <param name="_actionHandler">The action handler.</param>
        public void HandleOrganizerAction(OrganizerActionHandler _actionHandler)
        {
            if (_actionHandler != this.ActionHandler)
            {
                return; // don't do anything with wrong handler
            }

            if (_actionHandler.Error != null)
            {
                if (this.OrganizerTestDelegate != null)
                {
                    this.OrganizerTestDelegate.OrganizerModelControllerDidFinishWithError(this, _actionHandler.Error);
                }
                else
                {
                    this.HandleOrganizerActionError(this.ActionHandler.Error.Message, _actionHandler.Error.StackTrace, true);
                }

                this.ActionHandler = null; // clear action
                return;
            }

            List<IIdentifier> changedIdentifiers = null;

            if (this.OrganizerTestDelegate != null)
            {
                UPOrganizerActionTestResult testResult;

                if (_actionHandler.FollowUpViewReference != null)
                {
                    testResult = new UPOrganizerActionTestResult(_actionHandler.ChangedRecords, _actionHandler.FollowUpViewReference);
                }
                else if (_actionHandler is RecordSwitchActionHandler)
                {
                    RecordSwitchActionHandler recordSwitchHandler = (RecordSwitchActionHandler)_actionHandler;
                    testResult = new UPOrganizerActionTestResult(_actionHandler.ChangedRecords, recordSwitchHandler.ResultViewReference());
                }
                else
                {
                    testResult = new UPOrganizerActionTestResult(_actionHandler.ChangedRecords, null);
                }

                this.OrganizerTestDelegate.OrganizerModelControllerDidFinishAction(this, testResult);

                return;
            }

            if (_actionHandler.ChangedRecords?.Count > 0)
            {
                changedIdentifiers = new List<IIdentifier>();
                changedIdentifiers.AddRange(_actionHandler.ChangedRecords.Select(record => new RecordIdentifier(record.RecordIdentification)));
                UPChangeManager.CurrentChangeManager.RegisterChanges(changedIdentifiers);
            }

            if (_actionHandler.FollowUpViewReference != null)
            {
                var viewReference = _actionHandler.FollowUpViewReference;
                this.ActionHandler = null;

                if (_actionHandler.FollowUpReplaceOrganizer)
                {
                    var dict = new Dictionary<string, object>
                        {
                            { @"viewReference", viewReference },
                            { @"replaceOrganizer", 1 }
                        };

                    this.PerformAction(dict, null);
                }
                else
                {
                    var dict = new Dictionary<string, object>
                        {
                            { @"viewReference", viewReference }
                        };

                    this.PerformAction(dict, null);
                }

                return;
            }

            if (_actionHandler is RecordSwitchActionHandler)
            {
                RecordSwitchActionHandler recordSwitchHandler = (RecordSwitchActionHandler)_actionHandler;
                var viewReference = recordSwitchHandler.ResultViewReference();
                this.ActionHandler = null; // clear action

                if (viewReference != null)
                {
                    if (_actionHandler.FollowUpReplaceOrganizer)
                    {
                        var dict = new Dictionary<string, object>
                            {
                                { @"viewReference", viewReference },
                                { @"replaceOrganizer", 1 }
                            };

                        this.PerformAction(dict, null);
                    }
                    else
                    {
                        var dict = new Dictionary<string, object>
                            {
                                { @"viewReference", viewReference }
                            };

                        this.PerformAction(dict, null);
                    }
                }
            }
            else if (changedIdentifiers?.Count > 0)
            {
                this.InformDelegateAboutChangedRecords(changedIdentifiers);
            }

            this.ActionHandler = null; // clear action
        }

        /// <summary>
        /// Switches the on record.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public virtual void SwitchOnRecord(ViewReference viewReference)
        {
            if (this.ActionHandler != null)
            {
                return; // don't do anything if other action is active
            }

            this.ActionHandler = new RecordSwitchActionHandler(this, viewReference);
            this.ActionHandler.Execute();
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void PerformAction(object actionDictionary)
        {
            base.PerformAction(
                (Dictionary<string, object>)actionDictionary,
                this.pageContextDictionary.ValueOrDefault("AdditionalParametersOverride") as Dictionary<string, object>);
        }

        /// <summary>
        /// Performs the action with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="dict">The dictionary.</param>
        public void PerformActionWithViewReference(ViewReference viewReference, Dictionary<string, object> dict = null)
        {
            if (dict == null)
            {
                if (viewReference != null)
                {
                    var mdict = new Dictionary<string, object>
                        {
                            { @"viewReference", viewReference }
                        };

                    this.PerformAction(mdict);
                }
                else
                {
                    this.PerformAction(new Dictionary<string, object>());
                }
            }
            else if (viewReference == null)
            {
                this.PerformAction(dict);
            }
            else
            {
                var mdict = new Dictionary<string, object>(dict);
                mdict.Add(@"viewReference", viewReference);

                this.PerformAction(dict);
            }
        }

        /// <summary>
        /// Informs the delegate about changed records.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public virtual void InformDelegateAboutChangedRecords(List<IIdentifier> changes)
        {
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                modelController.Page.Invalid = true;
                modelController.ProcessChanges(changes);
            }
        }

        /// <summary>
        /// Pages for view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <returns></returns>
        public virtual UPPageModelController PageForViewReference(ViewReference viewReference)
        {
            switch (viewReference?.ViewName)
            {
                case @"RecordListView":
                    switch (viewReference.ContextValueForKey(@"Modus"))
                    {
                        case @"GlobalSearch":
                            return new GlobalSearchPageModelController(viewReference);

                        case @"MultiSearch":
                            return new UPMultiSearchPageModelController(viewReference);

                        case @"GeoSearch":
                            return new GeoSearchPageModelController(viewReference);

                        default:
                            return new UPStandardSearchPageModelController(viewReference);
                    }

                case @"RecordView":
                    var detailPageModelController = new DetailPageModelController(viewReference);
                    detailPageModelController.Init();
                    return detailPageModelController;

                case @"DashboardView":
                    return new DashboardPageModelController(viewReference);

                case @"WebContentView":
                    return new UPWebContentPageModelController(viewReference);

                case @"ConfirmWebContentView":
                    return new SerialEntryWebContentModelController(viewReference);

                case @"SettingsView":
                    // return new UPSettingsViewPageModelController(_viewReference);
                    break;

                case @"SettingsEditView":
                    // return new UPEditSettingsPageModelController(_viewReference);
                    break;

                case @"SerialEntry":
                    return new SerialEntryPageModelController(viewReference);

                case @"CharacteristicsView":
                case @"CharacteristicsEditView":
                    return new UPCharacteristicsEditPageModelController(viewReference);

                case @"ObjectivesView":
                    return new UPObjectivesPageModelController(viewReference);

                case @"DocumentView":
                    return new DocumentPageModelController(viewReference);

                case @"HistoryListView":
                    return new UPHistorySearchPageModelController(viewReference);

                case @"CircleOfInfluenceView":
                    return new CoIPageModelController(viewReference);

                case @"CircleOfInfluenceTreeView":
                    return new CoITreePageModelController(viewReference);

                case @"CalendarView":
                    return new UPCalendarPageModelController(viewReference);
                    
                case @"DataSynchronization":
                    return new UPDataSyncPageModelController(null);

                case @"SystemInfoView":
                    // return new UPSystemInfoPageModelController(viewReference);
                    break;

                case @"ConflictsView":
                    return new SyncConflictsPageModelController(viewReference);

                case @"TimelineView":
                    return new TimelineCalendarPageModelController(viewReference);

                case @"QuestionnaireView":
                    return new QuestionnaireEditPageModelController(viewReference);

                case @"QuestionnaireEditView":
                    return new QuestionnaireEditPageModelController(viewReference);

                case @"Analysis":
                case @"Query":
                    return new AnalysisPageModelController(viewReference);
                    break;

                case @"DocumentInbox":
                    // return new UPInBoxPageModelController(viewReference);
                    break;

                default:
                    // NSLog(@"view reference: %@", _viewReference);
                    break;
            }

            return null;
        }

        /// <summary>
        /// Adds the page model controller.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        public void AddPageModelController(UPPageModelController pageModelController)
        {
            pageModelController.ParentOrganizerModelController = this;
            this.PageModelControllers.Add(pageModelController);
        }

        /// <summary>
        /// Views the reference from parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="originalViewReference">The original view reference.</param>
        /// <returns></returns>
        private ViewReference ViewReferenceFromParameters(ViewReference parameters, ViewReference originalViewReference)
        {
            string contextMenuName = parameters.ContextValueForKey(@"ContextMenuAction");
            if (!string.IsNullOrEmpty(contextMenuName))
            {
                return originalViewReference;
            }

            Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(contextMenuName);
            ViewReference viewReference = menu?.ViewReference;

            if (viewReference == null)
            {
                return originalViewReference;
            }

            return viewReference.ViewReferenceWith(originalViewReference.ContextValueForKey(@"RecordId"));
        }

        /// <summary>
        /// Flips to edit.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        protected override void FlipToEdit(ViewReference parameters)
        {
            ViewReference viewReference = this.ViewReferenceFromParameters(parameters, this.ViewReference);

            // PVCS #76528: Wenn Detail über ParentRecordView aufgerufen wird, wird RecordId nicht an EditPageModelController übergeben.
            if (viewReference.ContextValueForKey(@"RecordId") == null && (this as DetailOrganizerModelController)?.RecordIdentification != null)
            {
                viewReference = viewReference.ViewReferenceWith(((DetailOrganizerModelController)this).RecordIdentification, @"RecordId");
            }

            EditOrganizerModelController organizerModelController = new EditOrganizerModelController(viewReference)
            {
                NavControllerId = this.NavControllerId,
                onlineData = this.onlineData
            };

            this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }

        /// <summary>
        /// Flips to edit serial entry.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        protected override void FlipToEditSerialEntry(ViewReference parameters)
        {
            var organizerModelController = new SerialEntryOrganizerModelController(this.ViewReference, UPOrganizerInitOptions.ShouldShowTabsForSingleTab());
            organizerModelController.onlineData = this.onlineData;
            this.modelControllerDelegate.TransitionToContentModelController(organizerModelController);
        }

        /// <summary>
        /// Switches to edit.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        protected override void SwitchToEdit(object actionDictionary)
        {
            this.FlipToEdit((ViewReference)actionDictionary);
        }

        /// <summary>
        /// Switches to edit serial entry.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void SwitchToEditSerialEntry(object actionDictionary)
        {
            this.FlipToEditSerialEntry((ViewReference)actionDictionary);
        }

        /// <summary>
        /// Shows the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public bool ShowStatus(UPMStatus status)
        {
            if (this.Organizer != null)
            {
                this.Organizer.Status = status;
                if (this.ModelControllerDelegate != null)
                {
                    this.InformAboutDidFailTopLevelElement(this.Organizer);
                    return true;
                }

                this.OrganizerStatus = status;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the organizer action error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        /// <param name="stayOnPage">if set to <c>true</c> [stay on page].</param>
        public async void HandleOrganizerActionError(string message, string details, bool stayOnPage)
        {
            if (stayOnPage)
            {
                if (this.modelControllerDelegate != null)
                {
                    await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(details, message);
                }
            }
            else
            {
                UPMErrorStatus messageStatus = UPMErrorStatus.ErrorStatusWithMessageDetails(message, details);
                this.ShowStatus(messageStatus);
            }
        }

        /// <summary>
        /// Pages the context value for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object PageContextValueForKey(string key)
        {
            return this.pageContextDictionary[key];
        }

        /// <summary>
        /// Pages the context valuefor key.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void PageContextValueforKey(object value, string key)
        {
            if (value == null)
            {
                this.pageContextDictionary.Remove(key);
            }
            else if (this.pageContextDictionary == null)
            {
                this.pageContextDictionary = new Dictionary<string, object>();
            }

            this.pageContextDictionary[key] = value;
        }

        /// <summary>
        /// Updates the action items status.
        /// </summary>
        public virtual void UpdateActionItemsStatus()
        {
        }

        /// <summary>
        /// Signals the test failure.
        /// </summary>
        /// <param name="failureText">The failure text.</param>
        /// <returns></returns>
        public bool SignalTestFailure(string failureText)
        {
            if (this.OrganizerTestDelegate != null)
            {
                this.OrganizerTestDelegate.OrganizerModelControllerDidFinishWithError(this, new Exception(failureText));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the focus view visible.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        public void SetFocusViewVisible(bool visible)
        {
            UPMultipleOrganizerManager.CurrentOrganizerManager.TheDelegate.SetFocusViewVisible(visible);
        }

        /// <summary>
        /// Handles the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public override void HandleErrors(List<Exception> errors)
        {
            this.modelControllerDelegate.HandleErrors(errors);
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public virtual void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            if (this.modelControllerDelegate != null)
            {
                this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
            }

            this.OfflineRequest = null;
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public virtual void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online,
            object context, Dictionary<string, object> result)
        {
            if (request is UPOfflineRecordRequest)
            {
                var listRecords = (List<UPCRMRecord>)data;
                List<IIdentifier> changes = listRecords.Select(record => new RecordIdentifier(record.RecordIdentification)).Cast<IIdentifier>().ToList();

                if (changes.Count > 0)
                {
                    UPChangeManager.CurrentChangeManager.RegisterChanges(changes);
                }

                if (!this.stayOnPageAfterOfflineRequest)
                {
                    this.modelControllerDelegate.PopToPreviousContentViewController();
                }
                else if (changes.Count > 0)
                {
                    this.SetFocusViewVisible(false);
                    this.ProcessChanges(changes);
                }
            }

            this.OfflineRequest = null;
        }

        /// <summary>
        /// Offlines the request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Rights the checker grants permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void RightsCheckerGrantsPermission(UPRightsChecker sender, string recordIdentification)
        {
            if(sender!= null && sender.Context != null)
            {
                sender.Selector?.Invoke(((ViewReference)sender.Context));
            }
        }

        /// <summary>
        /// Rights the checker revoke permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void RightsCheckerRevokePermission(UPRightsChecker sender, string recordIdentification)
        {
            this.HandleOrganizerActionError(sender.ForbiddenMessage, null, true);
        }

        /// <summary>
        /// Rights the checker did finish with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void RightsCheckerDidFinishWithError(UPRightsChecker sender, Exception error)
        {
            this.HandleOrganizerActionError(error.Message, error.Source, true);
        }

        /// <summary>
        /// Performed when the model controller view will disappear.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        public virtual void PageModelControllerViewWillDisappear(UPPageModelController pageModelController)
        {
            // do nothing in default implementation (implemented in SerialEntry-Organizer)
        }

        /// <summary>
        /// Performed when the model controller view will appear.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        public virtual void PageModelControllerViewWillAppear(UPPageModelController pageModelController)
        {
            // do nothing in default implementation (implemented in SerialEntry-Organizer)
        }

        /// <summary>
        /// Performed when the model controller set context value for key.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <param name="value">The value.</param>
        /// <param name="key">The key.</param>
        public void PageModelControllerSetContextValueForKey(UPPageModelController pageModelController, object value, string key)
        {
            // this.PageContextValue(value, key);
        }

        /// <summary>
        /// Provides context value for key.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object PageModelControllerContextValueForKey(UPPageModelController pageModelController, string key)
        {
            return this.PageContextValueForKey(key);
        }

        /// <summary>
        /// Provides contexts value dictionary for page model controller.
        /// </summary>
        /// <param name="pageModelController">The page model controller.</param>
        /// <returns></returns>
        public Dictionary<string, object> ContextValueDictionaryForPageModelController(UPPageModelController pageModelController)
        {
            return this.pageContextDictionary;
        }

        /// <summary>
        /// Method Adds Quick Organizer Header Buttons
        /// </summary>
        /// <param name="quickActionConfig">
        /// Config Header to add buttons to
        /// </param>
        /// <param name="configStore">
        /// Configuration Store <see cref="IConfigurationUnitStore"/>
        /// </param>
        /// <param name="currentGroup">
        /// Organizer Action Group <see cref="UPMOrganizerActionGroup"/>
        /// </param>
        /// <param name="recordIdentification">
        /// Record Id
        /// </param>
        private void AddQuickOrganizerHeaderButtons(
            UPConfigHeader quickActionConfig,
            IConfigurationUnitStore configStore,
            UPMOrganizerActionGroup currentGroup,
            string recordIdentification)
        {
            if (quickActionConfig != null && quickActionConfig.ButtonNames.Any())
            {
                foreach (var buttonName in quickActionConfig.ButtonNames)
                {
                    var button = configStore.ButtonByName(buttonName);

                    if (!CanExecuteButton(button) || button.IsHidden)
                    {
                        continue;
                    }

                    var action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId(ActionIdentifier + buttonName));
                    action.SetTargetAction(this, PerformAction);
                    action.ViewReference = button.ViewReference.ViewReferenceWith(recordIdentification);

                    var iconName = string.Empty;
                    if (!string.IsNullOrWhiteSpace(button.ImageName))
                    {
                        iconName = configStore.FileNameForResourceName(button.ImageName) + (currentGroup == null ? ColorThemeLight : ColorThemeDark);
                    }

                    if (!string.IsNullOrWhiteSpace(iconName))
                    {
                        action.IconName = iconName;
                    }

                    if (action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                    {
                        action.IconName = IconStarEmpty;
                        action.ActiveIconName = IconStar;
                        action.LabelText = LocalizedString.TextProcessAddToFavorites;
                    }
                    else
                    {
                        action.LabelText = button.Label;
                    }

                    AddQuickOrganizerHeaderActionItem(action);
                }
            }
        }

        /// <summary>
        /// Method Adds Organizer Header Buttons
        /// </summary>
        /// <param name="headerConfig">
        /// Config Header to add buttons to
        /// </param>
        /// <param name="configStore">
        /// /// Configuration Store <see cref="IConfigurationUnitStore"/>
        /// </param>
        /// <param name="currentGroup">
        /// Organizer Action Group <see cref="UPMOrganizerActionGroup"/>
        /// </param>
        /// <param name="recordIdentification">
        /// Record Id
        /// </param>
        /// <returns>
        /// Resulting Organizer Action Group <see cref="UPMOrganizerActionGroup"/>
        /// </returns>
        private UPMOrganizerActionGroup AddOrganizerHeaderButtons(
            UPConfigHeader headerConfig,
            IConfigurationUnitStore configStore,
            UPMOrganizerActionGroup currentGroup,
            string recordIdentification)
        {
            if (headerConfig?.ButtonNames != null)
            {
                foreach (var buttonName in headerConfig.ButtonNames)
                {
                    var action = (UPMOrganizerAction)null;
                    var buttonDef = configStore.ButtonByName(buttonName);
                    var iconName = string.Empty;

                    if (!string.IsNullOrWhiteSpace(buttonDef.ImageName))
                    {
                        var configImageName = configStore.FileNameForResourceName(buttonDef.ImageName);
                        if (configImageName != null)
                        {
                            var themeColor = currentGroup == null ? ColorThemeLight : ColorThemeDark;
                            iconName = configStore.FileNameForResourceName(buttonDef.ImageName) + themeColor;
                        }
                    }

                    if (buttonName.StartsWith(GroupStartLiteral))
                    {
                        currentGroup = new UPMOrganizerActionGroup(StringIdentifier.IdentifierWithStringId(ActionGroupAddIdentifier));
                        if (!string.IsNullOrWhiteSpace(iconName))
                        {
                            currentGroup.IconName = iconName;
                        }

                        if (!string.IsNullOrWhiteSpace(buttonDef.Label))
                        {
                            currentGroup.Title = buttonDef.Label;
                        }

                        continue;
                    }

                    if (buttonName.StartsWith(GroupEndLiteral))
                    {
                        if (Convert.ToBoolean(currentGroup?.Children.Any()))
                        {
                            AddOrganizerHeaderActionItem(currentGroup);
                        }

                        currentGroup = null;
                        continue;
                    }

                    if (buttonDef.ViewReference != null)
                    {
                        if (buttonDef.IsHidden)
                        {
                            continue;
                        }

                        action = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId(ActionIdentifier + buttonName));
                        action.SetTargetAction(this, PerformAction);
                        action.ViewReference = buttonDef.ViewReference.ViewReferenceWith(recordIdentification);

                        if (!string.IsNullOrWhiteSpace(iconName))
                        {
                            action.IconName = iconName;
                        }

                        if (action.Identifier.MatchesIdentifier(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)))
                        {
                            action.IconName = IconStarEmpty;
                            action.ActiveIconName = IconStar;
                            action.LabelText = LocalizedString.TextProcessAddToFavorites;
                        }
                        else
                        {
                            action.LabelText = buttonDef.Label;
                        }
                    }

                    if (action != null)
                    {
                        if (currentGroup != null)
                        {
                            currentGroup.AddAction(action);
                        }
                        else
                        {
                            AddOrganizerHeaderActionItem(action);
                        }
                    }
                }
            }

            return currentGroup;
        }

        /// <summary>
        /// Create Default controller object
        /// </summary>
        /// <param name="searchOrganizer">
        /// organizer
        /// </param>
        /// <param name="configStore">
        /// config store
        /// </param>
        /// <param name="infoAreaId">
        /// info Area Id string
        /// </param>
        private void CreateDefaultSearchPageModelController(UPMOrganizer searchOrganizer, IConfigurationUnitStore configStore, string infoAreaId)
        {
            var searchHeader = (UPConfigHeader)null;
            searchOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId($"{IdentifierPrefixSearch}{infoAreaId}"));
            TopLevelElement = searchOrganizer;

            var configName = ViewReference.ContextValueForKey(KeyConfigName);

            if (string.IsNullOrWhiteSpace(configName))
            {
                configName = infoAreaId;
            }

            var search = configStore.SearchAndListByName(configName);
            var organizerTitle = (string)null;

            if (search != null)
            {
                searchHeader = configStore.HeaderByNameFromGroup(KeySearch, search.HeaderGroupName);

                if (searchHeader == null && search.HeaderGroupName != infoAreaId)
                {
                    searchHeader = configStore.HeaderByNameFromGroup(KeySearch, infoAreaId);
                }

                if (searchHeader != null)
                {
                    organizerTitle = searchHeader.Label;
                }
            }

            if (!string.IsNullOrWhiteSpace(organizerTitle))
            {
                searchOrganizer.SubtitleText = organizerTitle;
            }
            else
            {
                var infoAreaConfig = configStore.InfoAreaConfigById(infoAreaId);
                var label = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(infoAreaId).Label;
                searchOrganizer.SubtitleText = infoAreaConfig != null
                    ? infoAreaConfig.PluralName
                    : label;
            }

            var linkRecordIdentification = ViewReference.ContextValueForKey(KeyLinkRecord);

            SetSearchOrganizerSubTitleText(configStore, searchOrganizer, linkRecordIdentification);

            TopLevelElement = searchOrganizer;

            var searchPageController = (UPStandardSearchPageModelController)PageForViewReference(ViewReference);

            if (searchPageController == null)
            {
                return;
            }

            var searchPage = (UPMSearchPage)searchPageController.Page;
            searchPage.LabelText = LocalizedString.TextTabAll;

            AddPageModelController(searchPageController);
            searchOrganizer.AddPage(searchPage);

            AddSubView(searchHeader, searchPage, searchOrganizer, linkRecordIdentification);

            if (searchHeader != null)
            {
                AddActionButtonsFromHeaderRecordIdentification(searchHeader, linkRecordIdentification);
            }
        }

        /// <summary>
        /// Creates <see cref="UPMultiSearchPageModelController"/> object.
        /// </summary>
        /// <param name="searchOrganizer">
        /// organizer object
        /// </param>
        private void CreateMultiSearchPageModelController(UPMOrganizer searchOrganizer)
        {
            searchOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId(KeyMultiSearch))
            {
                SubtitleText = LocalizedString.TextFilter
            };
            var searchPageController = new UPMultiSearchPageModelController(ViewReference);
            var searchPage = (UPMSearchPage)searchPageController.Page;

            searchOrganizer.AddPage(searchPage);
            AddPageModelController(searchPageController);
            searchPage.LabelText = LocalizedString.TextTabAll;
            TopLevelElement = searchOrganizer;
        }

        /// <summary>
        /// Creates <see cref="GeoSearchPageModelController"/> object.
        /// </summary>
        /// <param name="searchOrganizer">
        /// organizer object
        /// </param>
        private void CreateGeoSearchPageModelController(UPMOrganizer searchOrganizer)
        {
            searchOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId(KeyGeoSearch))
            {
                SubtitleText = LocalizedString.TextSearchWithRadius
            };
            TopLevelElement = searchOrganizer;
        }

        /// <summary>
        /// Creates <see cref="GlobalSearchPageModelController"/> object.
        /// </summary>
        /// <param name="searchOrganizer">
        /// organizer object
        /// </param>
        private void CreateGlobalSearchPageModelController(UPMOrganizer searchOrganizer)
        {
            searchOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId(KeyGlobalSearch))
            {
                SubtitleText = LocalizedString.TextHeadlineGlobalSearch
            };

            var searchPageController = new GlobalSearchPageModelController(ViewReference);
            var searchPage = (UPMSearchPage)searchPageController.Page;

            searchOrganizer.AddPage(searchPage);
            AddPageModelController(searchPageController);
            searchPage.LabelText = LocalizedString.TextTabAll;
            TopLevelElement = searchOrganizer;
        }

        /// <summary>
        /// Sets Organizer's SubTitle Text
        /// </summary>
        /// <param name="configStore">
        /// <see cref="IConfigurationUnitStore"/> object.
        /// </param>
        /// <param name="searchOrganizer">
        /// organizer
        /// </param>
        /// <param name="linkRecordIdentification">
        /// link record identification string
        /// </param>
        private void SetSearchOrganizerSubTitleText(IConfigurationUnitStore configStore, UPMOrganizer searchOrganizer, string linkRecordIdentification)
        {
            if (!string.IsNullOrWhiteSpace(linkRecordIdentification))
            {
                var linkInfoAreaId = linkRecordIdentification.InfoAreaId();
                var tableCaption = configStore.TableCaptionByName(linkInfoAreaId);

                if (tableCaption != null)
                {
                    var recordTableCaption = tableCaption.TableCaptionForRecordIdentification(linkRecordIdentification);
                    searchOrganizer.SubtitleText = recordTableCaption;
                }
            }
        }

        /// <summary>
        /// Add Sub Views to the object
        /// </summary>
        /// <param name="searchHeader">
        /// <see cref="UPConfigHeader"/> object
        /// </param>
        /// <param name="searchPage">
        /// <see cref="UPMSearchPage"/> object.
        /// </param>
        /// <param name="searchOrganizer">
        /// <see cref="UPMOrganizer"/> organizer object
        /// </param>
        /// <param name="linkRecordIdentification">
        /// link record identification
        /// </param>
        private void AddSubView(UPConfigHeader searchHeader, UPMSearchPage searchPage, UPMOrganizer searchOrganizer, string linkRecordIdentification)
        {
            var subViewsCount = searchHeader?.NumberOfSubViews ?? 0;
            for (var index = 0; index < subViewsCount; index++)
            {
                UPConfigHeaderSubView subViewDef = searchHeader.SubViewAtIndex(index);

                if (index == 0 && subViewDef.Options == SubViewOptionHash)
                {
                    searchPage.LabelText = subViewDef.Label;
                    continue;
                }

                ViewReference subViewViewReference = subViewDef.ViewReference;

                if (!string.IsNullOrWhiteSpace(linkRecordIdentification))
                {
                    subViewViewReference = subViewViewReference?.ViewReferenceWith(linkRecordIdentification);
                }

                if (subViewViewReference == null)
                {
                    continue;
                }

                var fullTextSearchString = ViewReference.ContextValueForKey(KeyFullTextSearch);

                if (!string.IsNullOrWhiteSpace(fullTextSearchString))
                {
                    Dictionary<string, object> addParam = new Dictionary<string, object>
                    {
                        { KeyFullTextSearch, fullTextSearchString }
                    };
                    subViewViewReference = subViewViewReference.ViewReferenceWith(addParam);
                }

                UPPageModelController subSearchPageController = PageForViewReference(subViewViewReference);
                Page subSearchPage = subSearchPageController.Page;

                if (subSearchPage == null)
                {
                    continue;
                }

                subSearchPage.LabelText = subViewDef.Label;
                searchOrganizer.AddPage(subSearchPage);
                AddPageModelController(subSearchPageController);
            }
        }
    }
}
