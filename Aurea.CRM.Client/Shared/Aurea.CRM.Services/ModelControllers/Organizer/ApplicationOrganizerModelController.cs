// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Application Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// Application Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    public class ApplicationOrganizerModelController : UPOrganizerModelController
    {
        private const string ApplictionModelIdentifier = "ApplictionModel";
        private const string ExceptionNoStartOrganizer = "Fatal: No start Organizer!";
        private const string IdentifierPrefixDetailAction = "-DetailAction";
        private const string IdentifierPrefixGlobalAction = "-GlobalAction";
        private const string IdentifierSuffixTitle = "-title";
        private const string InfoAreaD1 = "D1";
        private const string KeyDocumentInbox = "DocumentInbox";
        private const string KeyGlobalSearch = "GlobalSearch";
        private const string KeyInfoArea = "InfoArea";
        private const string KeyModus = "Modus";
        private const string KeyMultiSearch = "MultiSearch";
        private const string KeyStartPage = "StartPage";
        private const string MenuAppSearch = "$AppSearchMenu";
        private const string MenuGlobalActions = "$GlobalActions";
        private const string MenuImageIconInnerCircle = "crmpad-DefaultIconInnerCircle";
        private const string MenuItemHistorySearch = "$HistorySearch";
        private const string MenuItemGlobalSearch = "$GlobalSearch";
        private const string MenuItemFavoriteSearch = "$FavoriteSearch";
        private const string ViewRecordListView = "RecordListView";
        private const string ViewCalendarView = "CalendarView";
        private const string ViewDocumentView = "DocumentView";
        private const string ViewDebugView = "DebugView";
        private const string ViewHistoryListView = "HistoryListView";

        /// <summary>
        /// The start menu
        /// </summary>
        private Menu startMenu;

        /// <summary>
        /// The view reference dictionary
        /// </summary>
        private Dictionary<IIdentifier, ViewReference> viewReferenceDict;

        /// <summary>
        /// Gets the start organizer model controller.
        /// </summary>
        /// <value>
        /// The start organizer model controller.
        /// </value>
        public UPOrganizerModelController StartOrganizerModelController { get; private set; }

        /// <summary>
        /// Gets the quick global search model controller.
        /// </summary>
        /// <value>
        /// The quick global search model controller.
        /// </value>
        public SearchPageModelController QuickGlobalSearchModelController { get; private set; }

        /// <summary>
        /// Gets the quick history search model controller.
        /// </summary>
        /// <value>
        /// The quick history search model controller.
        /// </value>
        public UPHistorySearchPageModelController QuickHistorySearchModelController { get; private set; }

        /// <summary>
        /// Gets the quick favorite search model controller.
        /// </summary>
        /// <value>
        /// The quick favorit search model controller.
        /// </value>
        public UPMultiSearchPageModelController QuickFavoriteSearchModelController { get; private set; }

        // public UPEditSettingsPageModelController EditSettingsPageModelController { get; private set; }

        /// <summary>
        /// Gets the data synchronize page model controller.
        /// </summary>
        /// <value>
        /// The data synchronize page model controller.
        /// </value>
        public UPDataSyncPageModelController DataSyncPageModelController { get; private set; }

        /// <summary>
        /// Gets the global actions.
        /// </summary>
        /// <value>
        /// The global actions.
        /// </value>
        public List<UPMGlobalAction> GlobalActions { get; private set; }

        // public UPDatabaseDebugPageModelController CrmDBPageModelController { get; private set; }

        // public UPDatabaseDebugPageModelController OfflineDBPageModelController { get; private set; }

        // public UPDatabaseDebugPageModelController ConfigDBPageModelController { get; private set; }

        public UPMAdvancedSearchOrganizer AdvancedOrganizer => (UPMAdvancedSearchOrganizer)this.Organizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationOrganizerModelController"/> class.
        /// </summary>
        public ApplicationOrganizerModelController()
            : base((ViewReference)null)
        {
            this.viewReferenceDict = new Dictionary<IIdentifier, ViewReference>();

            Messenger.Default.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidChangeConfiguration, this.DidChangeConfiguration);
            Messenger.Default.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishFullSync, this.DidFinishFullSync);
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public void Build()
        {
            var store = ConfigurationUnitStore.DefaultStore;
            var startMenuName = store.ConfigValue(KeyStartPage);

            if (!string.IsNullOrWhiteSpace(startMenuName))
            {
                var startMenuItem = store.MenuByName(startMenuName);
                if (startMenuItem != null)
                {
                    startMenu = startMenuItem;
                }
            }

            var organizer = new UPMAdvancedSearchOrganizer(StringIdentifier.IdentifierWithStringId(ApplictionModelIdentifier))
            {
                ExpandFound = true,
                DisplaysTitleText = false,
                LineCountAdditionalTitletext = 0,
                DisplaysImage = false,
                Invalid = false
            };

            TopLevelElement = organizer;
            DataSyncPageModelController = new UPDataSyncPageModelController();
            GlobalActions = new List<UPMGlobalAction>();
            var menu = ConfigurationUnitStore.DefaultStore.MenuByName(MenuAppSearch);

            if (menu != null)
            {
                PopulateSubMenusFromMenu(menu, organizer);
            }
            else
            {
                PopulateSubMenus(organizer, store, startMenuName);
            }

            PopulateGlobalMenuSubMenus();

            if (startMenu != null)
            {
                SetStartOrganizerModelController();
            }
            else
            {
                throw new InvalidOperationException(ExceptionNoStartOrganizer);
            }

            var crmStore = UPCRMDataStore.DefaultStore;
        }

        /// <summary>
        /// Performs the detail action.
        /// </summary>
        /// <param name="_action">The action.</param>
        public void PerformDetailAction(object _action)
        {
            var action = (UPMAction)_action;
            ViewReference detailViewReference = this.viewReferenceDict[action.Identifier];
            UPOrganizerModelController organizerModelController = OrganizerFromViewReference(detailViewReference);
            IModelControllerUIDelegate organizerDelegate = UPMultipleOrganizerManager.CurrentOrganizerManager.ModelControllerOfCurrentOrganizer.ModelControllerDelegate;
            organizerDelegate?.TransitionToContentModelController(organizerModelController, MultiOrganizerMode.AlwaysNewWorkingOrganizer);

            string name = detailViewReference.Name;
            if (name.StartsWith("Menu:"))
            {
                // UPGoogleAnalytics.TrackMenuSource(name, "Search");
            }
        }

        /// <summary>
        /// Performs the global action.
        /// </summary>
        /// <param name="_action">The action.</param>
        public void PerformGlobalAction(object _action)
        {
            var action = (UPMAction)_action;
            ViewReference detailViewReference = this.viewReferenceDict[action.Identifier];
            UPOrganizerModelController organizerModelController = OrganizerFromViewReference(detailViewReference);
            this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController, MultiOrganizerMode.AlwaysNewWorkingOrganizer);
        }

        /// <summary>
        /// Makes the system information group.
        /// </summary>
        /// <returns></returns>
        public UPMGroup MakeSystemInfoGroup()
        {
            return null;
#if PORTING
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            bool hideEmptyFields = configStore.ConfigValueIsSet("SettingsView.HideEmptyFields");
            UPMStandardGroup detailGroup = new UPMStandardGroup(StringIdentifier.IdentifierWithStringId("systeminfo_1"));

            UPSystemInfo systemInfo = new UPSystemInfo();
            NSDictionary infos = this.GetFunctionNameDictionaryForSystemInfo(systemInfo);
            FieldControl listControl = null;
            ViewReference systemViewRef = new ViewReference(new List<object> { "fieldgroup", "SYSTEMINFO" }, "SystemInfoView");
            string fieldGroup = systemViewRef.ContextValueForKey("fieldgroup");
            if (fieldGroup != null)
            {
                listControl = configStore.FieldControlByNameFromGroup("List", fieldGroup);
                int row = 0;
                foreach (UPConfigFieldControlField field in listControl.Fields)
                {
                    string functionName = field.Function;
                    string label = field.Label;
                    string functionNameLowerCase = functionName.ToLower();
                    if (infos.ObjectForKey(functionNameLowerCase) != null)
                    {
                        IIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId($"Id_{functionName}");
                        string fieldValue = infos[functionNameLowerCase];
                        if (!string.IsNullOrEmpty( fieldValue )|| !hideEmptyFields)
                        {
                            UPMStringField stringField = new UPMStringField(fieldIdentifier);
                            stringField.StringValue=fieldValue;
                            stringField.LabelText=label;
                            if (functionName == "webversion" && ServerSession.CurrentSession().IsEnterprise)
                            {
                                stringField.StringValue = $"{fieldValue} Enterprise";
                            }

                            detailGroup.AddField(stringField);
                        }
                    }

                    row++;
                }
            }

            if (systemInfo.WebconfigParameter())
            {
                foreach (UPConfigWebConfigLayoutField configField in systemInfo.WebconfigParameter().AllValues())
                {
                    IIdentifier fieldIdentifier = StringIdentifier.IdentifierWithStringId(NSString.StringWithFormat("Id_%@", configField.ValueName()));
                    string label = configField.ValueName();
                    if (listControl!= null)
                    {
                        UPConfigFieldControlField field = listControl.FieldWithFunction(configField.ValueName());
                        if (!string.IsNullOrEmpty(field?.Label))
                        {
                            label = field.Label;
                        }
                    }

                    UPMStringField field = new UPMStringField(fieldIdentifier);
                    field.StringValue = configField.Value();
                    field.LabelText = label;
                    detailGroup.AddField(field);
                }
            }

            return detailGroup;
#endif
        }

        void Dealloc()
        {
            Messenger.Default.Unregister(this);
        }

        // NSDictionary GetFunctionNameDictionaryForSystemInfo(UPSystemInfo systemInfo)
        // {
        //    NSMutableDictionary infos = new NSMutableDictionary();
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.DeviceModel(), "devicemodel");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.System(), "system");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.AppVersion(), "appversion");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.RasApplicationId(), "rasapplicationid");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.Roles(), "roles");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.Rights(), "rights");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.WebVersion(), "webversion");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.User(), "user");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.Configuration(), "configuration");
        //    this.AddFunctionToDictionaryValueForKey(infos, systemInfo.ServerUrl(), "serverurl");
        //    return infos;
        // }

        void AddFunctionToDictionaryValueForKey(Dictionary<string, string> dictionary, string value, string key)
        {
            if (value != null)
            {
                dictionary[key] = value;
            }
        }

        private void DidFinishFullSync(object notificaction)
        {
            this.Build();
            UPMultipleOrganizerManager manager = UPMultipleOrganizerManager.CurrentOrganizerManager;
            foreach (UPOrganizerState state in manager.NavControllerContextDictionary.Values)
            {
                manager.CloseNavController(state.NavControllerId);
            }

            manager.AddRootNavController();
        }

        private void DidChangeConfiguration(object notificaction)
        {
            this.Build();
        }

        /// <summary>
        /// Populates Sub menus of Global Menu
        /// </summary>
        private void PopulateGlobalMenuSubMenus()
        {
            var globalMenu = ConfigurationUnitStore.DefaultStore.MenuByName(MenuGlobalActions);
            for (var index = 0; index < globalMenu?.NumberOfSubMenus; index++)
            {
                var menuItem = globalMenu.SubMenuAtIndex(index);
                if (menuItem.ViewReference != null)
                {
                    var key = $"{IdentifierPrefixGlobalAction} {menuItem.UnitName}";
                    var stringIdentifier = StringIdentifier.IdentifierWithStringId(key);
                    var globalAction = new UPMGlobalAction(stringIdentifier)
                    {
                        IconName = !string.IsNullOrWhiteSpace(menuItem.ImageName) ? menuItem.ImageName : MenuImageIconInnerCircle
                    };
                    var chunks = menuItem.DisplayName.Split(';');
                    if (chunks.Length > 0)
                    {
                        globalAction.LabelText = chunks[0];
                    }

                    if (chunks.Length > 1)
                    {
                        globalAction.AdditionalLabelText = chunks[1];
                    }

                    globalAction.SetTargetAction(this, PerformGlobalAction);
                    GlobalActions.Add(globalAction);
                    viewReferenceDict[globalAction.Identifier] = menuItem.ViewReference;
                }
            }

            var inboxConfig = ConfigurationUnitStore.DefaultStore.ConfigValue(KeyDocumentInbox);
#if PORTING
             if (inboxConfig.UpInboxEnabled())
             {
                var globalAction = new UPMGlobalAction(StringIdentifier.IdentifierWithStringId("GlobalAction Inbox"));
                globalAction.IconName = NSString.StringWithFormat("Icon2:%@", UPGlyphFontSet.NameForGlyphicons(UPGlyphiconsInboxIn));
                globalAction.LabelText = upText_InboxTitle;
                globalAction.AdditionalLabelText = upText_InboxAdditionalTitle;
                globalAction.SetTargetAction(this, @selector(performGlobalAction:));
                GlobalActions.Add(globalAction);
                var inboxViewReference = new ViewReference(null, "DocumentInbox");
                viewReferenceDict.SetObjectForKey(inboxViewReference, globalAction.Identifier);
             }
#endif
        }

        /// <summary>
        /// Set ORganizer Model Controller
        /// </summary>
        private void SetStartOrganizerModelController()
        {
            var toRemove = new List<UPMGlobalAction>();
            foreach (var action in GlobalActions)
            {
                var detailViewReference = viewReferenceDict[action.Identifier];
                if (startMenu.ViewReference == detailViewReference)
                {
                    toRemove.Add(action);
                }
                else if (detailViewReference.ViewName == ViewDebugView)
                {
                    toRemove.Add(action);
                }
            }

            foreach (var item in toRemove)
            {
                GlobalActions.Remove(item);
            }

            StartOrganizerModelController = OrganizerFromViewReference(startMenu.ViewReference);
        }

        /// <summary>
        /// Populate Sub menus
        /// </summary>
        /// <param name="organizer">
        /// <see cref="UPMAdvancedSearchOrganizer"/> object
        /// </param>
        /// <param name="store">
        /// <see cref="IConfigurationUnitStore"/> config store
        /// </param>
        /// <param name="startMenuName">
        /// starting menu
        /// </param>
        private void PopulateSubMenus(UPMAdvancedSearchOrganizer organizer, IConfigurationUnitStore store, string startMenuName)
        {
            var searchTypes = new List<UPMDetailSearch>();
            var menu = Menu.MainMenu();
            for (var index = 0; index < menu.NumberOfSubMenus; index++)
            {
                var menuItem = menu.SubMenuAtIndex(index);
                if (menuItem?.ViewReference != null)
                {
                    if (startMenu == null)
                    {
                        startMenu = menuItem;
                    }

                    if (!string.IsNullOrWhiteSpace(startMenuName))
                    {
                        var startMenuItem = store.MenuByName(startMenuName);
                        if (startMenuItem != null && startMenuItem == menuItem)
                        {
                            startMenu = menuItem;
                        }
                    }

                    if (menuItem.ViewReference.ViewName == ViewHistoryListView)
                    {
                        var modelController = PageForViewReference(menuItem.ViewReference);
                        AddPageModelController(modelController);
                        organizer.AddPage(modelController.Page);
                        QuickHistorySearchModelController = (UPHistorySearchPageModelController)modelController;
                    }
                    else if (menuItem.ViewReference.ViewName == ViewRecordListView || menuItem.ViewReference.ViewName == ViewCalendarView
                        || menuItem.ViewReference.ViewName == ViewDocumentView)
                    {
                        if (menuItem.ViewReference.ContextValueForKey(KeyModus) == KeyGlobalSearch)
                        {
                            var modelController = PageForViewReference(menuItem.ViewReference);
                            AddPageModelController(modelController);
                            organizer.AddPage(modelController.Page);
                            QuickGlobalSearchModelController = (GlobalSearchPageModelController)modelController;
                        }
                        else if (menuItem.ViewReference.ContextValueForKey(KeyModus) == KeyMultiSearch)
                        {
                            var modelController = PageForViewReference(menuItem.ViewReference);
                            AddPageModelController(modelController);
                            organizer.AddPage(modelController.Page);
                            QuickFavoriteSearchModelController = (UPMultiSearchPageModelController)modelController;
                        }
                        else
                        {
                            var itemViewReference = menuItem.ViewReference;
                            var infoArea = itemViewReference.ContextValueForKey(KeyInfoArea);
                            if (menuItem.ViewReference.ViewName == ViewDocumentView && infoArea == null)
                            {
                                infoArea = InfoAreaD1;
                            }

                            var detailSearchType = new UPMDetailSearch(StringIdentifier.IdentifierWithStringId(menu.UnitName))
                            {
                                TileField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{menuItem.UnitName}{IdentifierSuffixTitle}"))
                                {
                                    StringValue = menuItem.DisplayName
                                },
                                Color = ColorForInfoAreaWithId(infoArea)
                            };
                            searchTypes.Add(detailSearchType);
                            var action = new UPMAction(StringIdentifier.IdentifierWithStringId($"{IdentifierPrefixDetailAction} {menuItem.UnitName}"));
                            action.SetTargetAction(this, PerformDetailAction);
                            detailSearchType.SwitchToDetailSearchAction = action;
                            viewReferenceDict[action.Identifier] = itemViewReference;
                        }
                    }
                }
            }

            organizer.DetailSearches = searchTypes;
        }

        /// <summary>
        /// Populate Sub menus from parent menu
        /// </summary>
        /// <param name="menu">
        /// <see cref="Menu"/>
        /// </param>
        /// <param name="organizer">
        /// <see cref="UPMAdvancedSearchOrganizer"/>
        /// </param>
        private void PopulateSubMenusFromMenu(Menu menu, UPMAdvancedSearchOrganizer organizer)
        {
            var searchTypes = new List<UPMDetailSearch>();
            for (var index = 0; index < menu.NumberOfSubMenus; index++)
            {
                var menuItem = menu.SubMenuAtIndex(index);
                if (menuItem?.ViewReference != null)
                {
                    if (menuItem.UnitName == MenuItemHistorySearch)
                    {
                        var modelController = PageForViewReference(menuItem.ViewReference);
                        AddPageModelController(modelController);
                        organizer.AddPage(modelController.Page);
                        QuickHistorySearchModelController = (UPHistorySearchPageModelController)modelController;
                    }
                    else if (menuItem.ViewReference.ViewName == ViewRecordListView || menuItem.ViewReference.ViewName == ViewCalendarView
                        || menuItem.ViewReference.ViewName == ViewDocumentView)
                    {
                        if (menuItem.UnitName == MenuItemGlobalSearch)
                        {
                            var modelController = PageForViewReference(menuItem.ViewReference);
                            AddPageModelController(modelController);
                            organizer.AddPage(modelController.Page);
                            QuickGlobalSearchModelController = (GlobalSearchPageModelController)modelController;
                        }
                        else if (menuItem.UnitName == MenuItemFavoriteSearch)
                        {
                            var modelController = PageForViewReference(menuItem.ViewReference);
                            AddPageModelController(modelController);
                            organizer.AddPage(modelController.Page);
                            QuickFavoriteSearchModelController = (UPMultiSearchPageModelController)modelController;
                        }
                        else
                        {
                            var itemViewReference = menuItem.ViewReference;
                            var infoArea = itemViewReference.ContextValueForKey(KeyInfoArea);
                            if (menuItem.ViewReference.ViewName == ViewDocumentView && infoArea == null)
                            {
                                infoArea = InfoAreaD1;
                            }

                            var identifier = StringIdentifier.IdentifierWithStringId(menu.UnitName);
                            var detailSearchType = new UPMDetailSearch(identifier)
                            {
                                TileField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{menuItem.UnitName}{IdentifierSuffixTitle}"))
                            };
                            detailSearchType.TileField.StringValue = menuItem.DisplayName;
                            detailSearchType.Color = ColorForInfoAreaWithId(infoArea);
                            searchTypes.Add(detailSearchType);
                            var action = new UPMAction(StringIdentifier.IdentifierWithStringId($"{IdentifierPrefixDetailAction} {menuItem.UnitName}"));
                            action.SetTargetAction(this, PerformDetailAction);
                            detailSearchType.SwitchToDetailSearchAction = action;
                            viewReferenceDict[action.Identifier] = itemViewReference;
                        }
                    }
                }
            }

            organizer.DetailSearches = searchTypes;
        }
    }
}
