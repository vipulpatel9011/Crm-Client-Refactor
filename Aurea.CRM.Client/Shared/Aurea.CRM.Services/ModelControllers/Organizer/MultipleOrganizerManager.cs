// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleOrganizerManager.cs" company="Aurea Software Gmbh">
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
//   The Multiple Organizer Manager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// The Multiple Organizer Manager
    /// </summary>
    public class UPMultipleOrganizerManager
    {
        static int navControllerId;
        static UPMultipleOrganizerManager defaultManager;

        private int currentNavControllerId;
        private Dictionary<int, UPOrganizerState> navControllerContextDictionary;
        private int maxNumberOfWorkingOrganizer;
        private List<int> multiOrganizerHistory;
        private bool backgroundRefreshOfStartOrganizer;

        /// <summary>
        /// The Organizer manager no nav controller
        /// </summary>
        public const int UPORGANIZER_MANAGER_NO_NAV_CONTROLLER = 0;

        /// <summary>
        /// Gets the start organizer nav controller identifier.
        /// </summary>
        /// <value>
        /// The start organizer nav controller identifier.
        /// </value>
        public int StartOrganizerNavControllerId { get; private set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IOrganizerManagerUIDelegate TheDelegate { get; set; }

        /// <summary>
        /// Gets or sets the organizer bar delegate.
        /// </summary>
        /// <value>
        /// The organizer bar delegate.
        /// </value>
        public IOrganizerManagerOrganizerBarDelegate OrganizerBarDelegate { get; set; }

        /// <summary>
        /// Gets or sets the application model controller.
        /// </summary>
        /// <value>
        /// The application model controller.
        /// </value>
        public ApplicationOrganizerModelController ApplicationModelController { get; set; }

        /// <summary>
        /// Gets the current nav controller identifier.
        /// </summary>
        /// <value>
        /// The current nav controller identifier.
        /// </value>
        public int CurrentNavControllerId => this.currentNavControllerId;

        /// <summary>
        /// Gets the model controller of current organizer.
        /// </summary>
        /// <value>
        /// The model controller of current organizer.
        /// </value>
        public UPOrganizerModelController ModelControllerOfCurrentOrganizer => this.TheDelegate.CurrentOrganizerModelController;

        /// <summary>
        /// Gets a value indicating whether this instance has editing nav controller.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has editing nav controller; otherwise, <c>false</c>.
        /// </value>
        public bool HasEditingNavController => this.EditingNavControllerId != UPORGANIZER_MANAGER_NO_NAV_CONTROLLER;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMultipleOrganizerManager"/> class.
        /// </summary>
        public UPMultipleOrganizerManager()
        {
            this.navControllerContextDictionary = new Dictionary<int, UPOrganizerState>();
            Messenger.Default.Register<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishFullSync, this.SyncManagerDidFinishFullSync);
            this.ApplicationModelController = new ApplicationOrganizerModelController();
            this.StartOrganizerNavControllerId = UPORGANIZER_MANAGER_NO_NAV_CONTROLLER;
            this.maxNumberOfWorkingOrganizer = 2;
            this.multiOrganizerHistory = new List<int>();
            this.backgroundRefreshOfStartOrganizer = ConfigurationUnitStore.DefaultStore?.ConfigValueIsSetDefaultValue("RefreshStartOrganizerInBackground", true) ?? false;

            if (this.backgroundRefreshOfStartOrganizer)
            {
                Messenger.Default.Register<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidPopViewController, this.DidPopViewController);
            }
        }

        /// <summary>
        /// Gets the current organizermanager.
        /// </summary>
        /// <value>
        /// The current organizermanager.
        /// </value>
        public static UPMultipleOrganizerManager CurrentOrganizerManager => defaultManager ?? (defaultManager = new UPMultipleOrganizerManager());

        /// <summary>
        /// Releases the instance.
        /// </summary>
        public static void ReleaseInstance()
        {
            navControllerId = 0;
            defaultManager = null;
        }

        /// <summary>
        /// Gets the nav controller context dictionary.
        /// </summary>
        /// <value>
        /// The nav controller context dictionary.
        /// </value>
        public Dictionary<int, UPOrganizerState> NavControllerContextDictionary => this.navControllerContextDictionary;

        void Dealloc()
        {
            Messenger.Default.Unregister<SyncManagerMessage>(this, SyncManagerMessageKey.DidFinishFullSync);
            if (this.backgroundRefreshOfStartOrganizer)
            {
                Messenger.Default.Unregister<CustomNavigationControllerMessage>(this, CustomNavigationControllerMessageKey.DidPopViewController);
            }
        }

        /// <summary>
        /// Adds the root nav controller.
        /// </summary>
        public void AddRootNavController()
        {
            this.StartOrganizerNavControllerId = navControllerId + 1;
            this.AddNavController();
        }

        private bool NeedsToCreateNewNavController
        {
            get
            {
                UPOrganizerState currentState = this.GetOrCreateNavControllerStateForKey(this.currentNavControllerId);
                return this.currentNavControllerId == this.StartOrganizerNavControllerId || currentState.Editing;
            }
        }

        /// <summary>
        /// Switches to working organizer with model controller.
        /// </summary>
        /// <param name="modelContoller">The model contoller.</param>
        /// <returns></returns>
        public int SwitchToWorkingOrganizerWithModelController(UPOrganizerModelController modelContoller)
        {
            int closeOrganizer = UPORGANIZER_MANAGER_NO_NAV_CONTROLLER;
            int workingOrganizerCount = 0;
            bool closeCurrent = this.CheckForAutoDestructOfCurrentOrganizer();
            int oldCurrentNavId = this.CurrentNavControllerId;
            foreach (int keyObject in this.navControllerContextDictionary.Keys)
            {
                int numberKey = keyObject;
                if (numberKey != this.StartOrganizerNavControllerId)
                {
                    UPOrganizerState state = this.navControllerContextDictionary[keyObject];
                    if (!state.Editing)
                    {
                        closeOrganizer = closeOrganizer == UPORGANIZER_MANAGER_NO_NAV_CONTROLLER ? numberKey : Math.Min(closeOrganizer, numberKey);

                        workingOrganizerCount++;
                    }
                }
            }

            if (workingOrganizerCount >= this.maxNumberOfWorkingOrganizer)
            {
                this.CloseNavController(closeOrganizer);
            }

            int newNavId = this.AddNavControllerForModelController(modelContoller);
            if (closeCurrent)
            {
                this.CloseNavController(oldCurrentNavId);
            }

            return newNavId;
        }

        private bool CheckForAutoDestructOfCurrentOrganizer()
        {
            if (this.currentNavControllerId != this.StartOrganizerNavControllerId)
            {
                UPOrganizerModelController organizerMC = this.ModelControllerOfCurrentOrganizer;
                return organizerMC.CloseOrganizerWhenLeaving;
            }

            return false;
        }

        private bool HasWorkingOrganizerWithId(int navControllerId)
        {
            return this.navControllerContextDictionary.Values.Any(state =>
                state.NavControllerId != this.StartOrganizerNavControllerId && state.NavControllerId == navControllerId);
        }

        /// <summary>
        /// Workings the organizer array.
        /// </summary>
        /// <returns></returns>
        public List<UPOrganizerState> WorkingOrganizerArray()
        {
            List<UPOrganizerState> result = new List<UPOrganizerState>();
            foreach (UPOrganizerState state in this.navControllerContextDictionary.Values)
            {
                if ((state.NavControllerId != this.StartOrganizerNavControllerId)
                    || (this.navControllerContextDictionary.Values.Count == 1 && state.NavControllerId == this.StartOrganizerNavControllerId))
                {
                    result.Add(state);
                }
            }

            return result;
        }

        /// <summary>
        /// Switches to start organizer.
        /// </summary>
        public void SwitchToStartOrganizer()
        {
            if (this.currentNavControllerId != this.StartOrganizerNavControllerId)
            {
                if (this.StartOrganizerNavControllerId == UPORGANIZER_MANAGER_NO_NAV_CONTROLLER)
                {
                    this.StartOrganizerNavControllerId = navControllerId + 1;
                    this.AddNavController();
                    this.SetDisplayTitleForNavControllerId("StartOrganizer", this.StartOrganizerNavControllerId);
                }
                else
                {
                    this.SwitchedToNavController(this.StartOrganizerNavControllerId);
                }
            }
        }

        /// <summary>
        /// Gets the editing nav controller identifier.
        /// </summary>
        /// <value>
        /// The editing nav controller identifier.
        /// </value>
        public int EditingNavControllerId
        {
            get
            {
                UPOrganizerState state = this.navControllerContextDictionary.Values.FirstOrDefault(x => x.Editing);
                return state?.NavControllerId ?? UPORGANIZER_MANAGER_NO_NAV_CONTROLLER;
            }
        }

        /// <summary>
        /// Determines whether [has nav controller with identifier] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [has nav controller with identifier] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasNavControllerWithId(int key)
        {
            return this.navControllerContextDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Switcheds to nav controller.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SwitchedToNavController(int key)
        {
            int oldId = this.currentNavControllerId;
            bool closeCurrent = this.CheckForAutoDestructOfCurrentOrganizer();
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillLeaveOrganizer, oldId));
            this.OrganizerBarDelegate.WillLeaveOrganizerWithNavControllerId(oldId);
            this.ResetOrganizerHistoryPosition(key);
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillEnterOrganizer, key));
            this.currentNavControllerId = key;
            this.TheDelegate.SwitchToNavControllerFromNavConroller(key, oldId);
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidEnterOrganizer, key));
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidLeaveOrganizer, oldId));

            if (closeCurrent)
            {
                this.CloseNavController(oldId);
            }
        }

        private int AddNavController()
        {
            int oldId = this.currentNavControllerId;
            if (oldId != UPORGANIZER_MANAGER_NO_NAV_CONTROLLER)
            {
                Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillLeaveOrganizer, oldId));
                this.OrganizerBarDelegate.WillLeaveOrganizerWithNavControllerId(oldId);
            }

            navControllerId++;
            this.ResetOrganizerHistoryPosition(navControllerId);
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillAddOrganizer, navControllerId));
            this.currentNavControllerId = navControllerId;
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(navControllerId);
            state.StartingNavControllerId = this.LastUsedWorkingOrganizer;
            this.TheDelegate.AddNewNavControllerForId(navControllerId);

            if (oldId != UPORGANIZER_MANAGER_NO_NAV_CONTROLLER)
            {
                Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidLeaveOrganizer, oldId));
            }

            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidAddOrganizer, navControllerId));
            return navControllerId;
        }

        /// <summary>
        /// Adds the nav controller for model controller.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <returns></returns>
        public int AddNavControllerForModelController(UPOrganizerModelController modelController)
        {
            int oldId = this.currentNavControllerId;
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillLeaveOrganizer, oldId));
            this.OrganizerBarDelegate.WillLeaveOrganizerWithNavControllerId(oldId);
            navControllerId++;
            this.ResetOrganizerHistoryPosition(navControllerId);
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillAddOrganizer, navControllerId));
            this.currentNavControllerId = navControllerId;
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(navControllerId);
            state.StartingNavControllerId = this.LastUsedWorkingOrganizer;
            this.TheDelegate.AddNewWorkingNavControllerForIdWithModelController(navControllerId, modelController);
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidLeaveOrganizer, oldId));
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidAddOrganizer, navControllerId));
            return navControllerId;
        }

        /// <summary>
        /// Closes the nav controller.
        /// </summary>
        /// <param name="key">The key.</param>
        public void CloseNavController(object key)
        {
            int ikey = (int)key;
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.WillCloseOrganizer, ikey));
            this.TheDelegate.CloseNavController(ikey);
            this.navControllerContextDictionary.Remove(ikey);
            this.multiOrganizerHistory.Remove(ikey);
            UPAppProcessContext.CurrentContext.RemoveAllForNavControlerId(ikey);
            Messenger.Default.Send(OrganizerManagerMessage.Create(OrganizerManagerMessageKey.DidCloseOrganizer, ikey));
        }

        private void CloseCurrentNavController()
        {
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(this.currentNavControllerId);
            if (state.StartingNavControllerId != UPORGANIZER_MANAGER_NO_NAV_CONTROLLER && this.HasNavControllerWithId(state.StartingNavControllerId))
            {
                this.SwitchedToNavController(state.StartingNavControllerId);
            }
            else
            {
                this.SwitchToStartOrganizer();
            }

            this.CloseNavController(state.NavControllerId);
        }

        private void SetDisplayTitleForNavControllerId(string text, int id)
        {
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(id);
            state.Title = text;
        }

        /// <summary>
        /// Sets the display sub title for nav controller identifier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="id">The identifier.</param>
        public void SetDisplaySubTitleForNavControllerId(string text, int id)
        {
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(id);
            state.Subtitle = text;
        }

        private string DisplayTitleForNavControllerId(int id)
        {
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(id);
            return state.Title;
        }

        private string DisplaySubTitleForNavControllerId(int id)
        {
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(id);
            return state.Subtitle;
        }

        /// <summary>
        /// Sets the editing for nav controller identifier.
        /// </summary>
        /// <param name="editing">if set to <c>true</c> [editing].</param>
        /// <param name="id">The identifier.</param>
        public void SetEditingForNavControllerId(bool editing, int id)
        {
            UPOrganizerState state = this.GetOrCreateNavControllerStateForKey(id);
            state.Editing = editing;
        }

        private UPOrganizerState GetOrCreateNavControllerStateForKey(int key)
        {
            UPOrganizerState state = this.navControllerContextDictionary.ValueOrDefault(key);
            if (state == null)
            {
                state = new UPOrganizerState();
                state.NavControllerId = key;
                this.navControllerContextDictionary[key] = state;
            }

            return state;
        }

        private void ResetOrganizerHistoryPosition(int navController)
        {
            this.multiOrganizerHistory.Remove(navControllerId);
            this.multiOrganizerHistory.Add(navControllerId);
        }

        private int LastUsedWorkingOrganizer
        {
            get
            {
                for (int index = this.multiOrganizerHistory.Count - 1; index >= 0; index--)
                {
                    int key = this.multiOrganizerHistory[index];
                    if (key != this.StartOrganizerNavControllerId && key != this.currentNavControllerId)
                    {
                        return key;
                    }
                }

                return UPORGANIZER_MANAGER_NO_NAV_CONTROLLER;
            }
        }

        /// <summary>
        /// Synchronizes the manager did finish full synchronize.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void SyncManagerDidFinishFullSync(object sender)
        {
            UPAppProcessContext.CurrentContext.RemoveAllForAllNavController();
            this.ApplicationModelController = new ApplicationOrganizerModelController();
            this.ApplicationModelController.Build();
            List<int> removeKeys = this.navControllerContextDictionary.Keys.Where(key => key != this.currentNavControllerId).ToList();

            foreach (int key in removeKeys)
            {
                this.navControllerContextDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Dids the pop view controller.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void DidPopViewController(object sender)
        {
            UPOrganizerModelController modelController = this.TheDelegate.CurrentOrganizerModelController;
            if (modelController != null)
            {
                List<IIdentifier> changes = UPChangeManager.CurrentChangeManager.ChangesToApplyForCurrentViewController(this.StartOrganizerNavControllerId);
                if (changes.Count > 0)
                {
                    modelController.ProcessChanges(changes);
                    foreach (UPPageModelController pageModelController in modelController.PageModelControllers)
                    {
                        if (pageModelController.BackgroundRefreshInStartOrganizer())
                        {
                            pageModelController.UpdateElementForCurrentChanges(changes);
                            pageModelController.RemovePendingChanges();
                            pageModelController.MarkForRedraw = true;
                        }
                    }
                }
            }
        }
    }
}
