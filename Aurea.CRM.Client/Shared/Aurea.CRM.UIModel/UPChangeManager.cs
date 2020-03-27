// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChangeManager.cs" company="Aurea Software Gmbh">
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
//   The Change Manager class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.UIModel.Identifiers;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// The Change Manager class
    /// </summary>
    public class UPChangeManager
    {
        /// <summary>
        /// The change organizer
        /// </summary>
        private Dictionary<int, List<List<IIdentifier>>> changeOrganizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPChangeManager"/> class.
        /// </summary>
        public UPChangeManager()
        {
            this.changeOrganizer = new Dictionary<int, List<List<IIdentifier>>>();
            this.RegisterForViewNavigationNotifications();

            Messenger.Default.Register<SyncManagerMessage>(this, Core.Session.Constants.KUPSyncManagerModifiedRecordIdentifications, this.SyncManagerDidUpdateRecords);
        }

        void Dealloc()
        {
            Messenger.Default.Unregister<SyncManagerMessage>(this, Core.Session.Constants.KUPSyncManagerModifiedRecordIdentifications);
            this.RemoveViewNavigationNotifications();
        }

        private List<List<IIdentifier>> CurrentChangeLevel => null;

        private List<List<IIdentifier>> ChangeLevelForNavId(int navId)
        {
            return null;
        }

        /// <summary>
        /// Changes to apply for current view controller.
        /// </summary>
        /// <param name="navId">The nav identifier.</param>
        /// <returns></returns>
        public List<IIdentifier> ChangesToApplyForCurrentViewController(int navId)
        {
            List<IIdentifier> changesToApply;
            lock (this)
            {
                var changes = this.ChangeLevelForNavId(navId);
                List<IIdentifier> changesForCurrentLevel = changes?[changes.Count - 1];
                changesToApply = new List<IIdentifier>(changesForCurrentLevel);
                changesForCurrentLevel?.Clear();
            }

            return changesToApply;
        }

        /// <summary>
        /// Changeses to apply for current view controller.
        /// </summary>
        /// <returns></returns>
        public List<IIdentifier> ChangesToApplyForCurrentViewController()
        {
            List<IIdentifier> changesToApply;
            lock (this)
            {
                var changes = this.CurrentChangeLevel;
                List<IIdentifier> changesForCurrentLevel = changes?[changes.Count - 1];
                changesToApply = new List<IIdentifier>(changesForCurrentLevel);
                changesForCurrentLevel?.Clear();
            }

            return changesToApply;
        }

        /// <summary>
        /// Registers the changes.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        public void RegisterChanges(List<IIdentifier> listOfIdentifiers)
        {
            lock (this)
            {
                foreach (List<List<IIdentifier>> changeLevels in this.changeOrganizer.Values)
                {
                    foreach (List<IIdentifier> changeArray in changeLevels)
                    {
                        foreach (IIdentifier identifier in listOfIdentifiers)
                        {
                            if (!changeArray.Contains(identifier))
                            {
                                changeArray.Add(identifier);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all changes.
        /// </summary>
        public void RemoveAllChanges()
        {
            lock (this)
            {
                this.CurrentChangeLevel.Clear();
            }
        }

        private static UPChangeManager currentChangeManager;

        /// <summary>
        /// Gets the current change manager.
        /// </summary>
        /// <value>
        /// The current change manager.
        /// </value>
        public static UPChangeManager CurrentChangeManager => currentChangeManager ?? (currentChangeManager = new UPChangeManager());

        /// <summary>
        /// Registers for view navigation notifications.
        /// </summary>
        public void RegisterForViewNavigationNotifications()
        {
        }

        /// <summary>
        /// Removes the view navigation notifications.
        /// </summary>
        public void RemoveViewNavigationNotifications()
        {
        }

        /// <summary>
        /// Synchronizes the manager did update records.
        /// </summary>
        /// <param name="notification">The notification.</param>
        public void SyncManagerDidUpdateRecords(SyncManagerMessage notification)
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

                this.RegisterChanges(recordIdentifiers);
            }
        }
    }
}
