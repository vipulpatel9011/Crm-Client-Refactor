// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryManager.cs" company="Aurea Software Gmbh">
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
//   HistoryManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Platform;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// History Manager
    /// </summary>
    public class HistoryManager
    {
        private List<object> filterSet;
        private static HistoryManager defaultManager;

        /// <summary>
        /// Gets the history entries.
        /// </summary>
        /// <value>
        /// The history entries.
        /// </value>
        public List<HistoryEntry> HistoryEntries { get; }

        /// <summary>
        /// Gets or sets the maximum entries.
        /// </summary>
        /// <value>
        /// The maximum entries.
        /// </value>
        public int MaxEntries { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="HistoryManager"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public bool Active { get; private set; }

        /// <summary>
        /// Gets the default history manager.
        /// </summary>
        /// <value>
        /// The default history manager.
        /// </value>
        public static HistoryManager DefaultHistoryManager => defaultManager ?? (defaultManager = new HistoryManager());

        /// <summary>
        /// Views the reference for record identifier.
        /// </summary>
        /// <param name="recordIdentifier">The record identifier.</param>
        /// <returns></returns>
        public ViewReference ViewReferenceForRecordIdentifier(string recordIdentifier)
        {
            HistoryEntry entry = this.EntryForRecordIdentifier(recordIdentifier);
            return entry.ViewReference;
        }

        /// <summary>
        /// Entries for record identifier.
        /// </summary>
        /// <param name="recordIdentifier">The record identifier.</param>
        /// <returns></returns>
        public HistoryEntry EntryForRecordIdentifier(string recordIdentifier)
        {
            return this.HistoryEntries.FirstOrDefault(entry => entry.RecordIdentification == recordIdentifier);
        }

        private HistoryManager()
        {
            this.HistoryEntries = new List<HistoryEntry>();
            this.MaxEntries = 10;
            this.filterSet = null;
            this.Active = false;
            IConfigurationUnitStore store = ConfigurationUnitStore.DefaultStore;
            WebConfigValue configValue = store.WebConfigValueByName(Constants.ConfigParamMenuName);
            string menuName = Constants.DefaultHistoryMenuName;
            if (configValue != null)
            {
                menuName = configValue.Value;
            }

            Menu menu = store.MenuByName(menuName);
            if (menu != null)
            {
                this.ConfigureWithViewReference(menu.ViewReference);
            }
        }

        private void ConfigureWithViewReference(ViewReference viewReference)
        {
            if (viewReference.ViewName == Constants.HistoryViewReferenceName)
            {
                string maxEntriesAsString = viewReference.ContextValueForKey("maxNumberHistoryRecords");
                if (!string.IsNullOrEmpty(maxEntriesAsString))
                {
                    this.MaxEntries = Convert.ToInt32(maxEntriesAsString);
                }

                string filterDefinitionJsonString = viewReference.ContextValueForKey("infoAreaFilter");
                if (string.IsNullOrEmpty(filterDefinitionJsonString))
                {
                    filterDefinitionJsonString = viewReference.ContextValueForKey("InfoAreaFilter");
                }

                if (!string.IsNullOrEmpty(filterDefinitionJsonString))
                {
                    List<object> _filterSet = new List<object>();
                    Dictionary<string, object> definition = filterDefinitionJsonString.JsonDictionaryFromString();
                    List<object> filterArray = definition["filter"] as List<object>;
                    _filterSet.AddRange(filterArray);
                    this.filterSet = _filterSet;
                }

                this.LoadHistory();
                this.Active = true;
            }
        }

        /// <summary>
        /// Updates the history entry.
        /// </summary>
        /// <param name="oldRecordIdentifier">The old record identifier.</param>
        /// <param name="newRecordIdentifier">The new record identifier.</param>
        public void UpdateHistoryEntry(string oldRecordIdentifier, string newRecordIdentifier)
        {
            HistoryEntry entry = this.EntryForRecordIdentifier(oldRecordIdentifier);
            if (entry != null)
            {
                entry.RecordIdentification = newRecordIdentifier;
                ViewReference newReferenz = new ViewReference(entry.ViewReference, oldRecordIdentifier, newRecordIdentifier, null);
                entry.ViewReference = newReferenz;
                this.SaveHistory();
            }
        }

        /// <summary>
        /// Adds the history.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        /// <param name="imageName">Name of the image.</param>
        public void AddHistory(string recordIdentification, ViewReference viewReference, bool onlineData, string imageName)
        {
            if (this.Active)
            {
                if (this.filterSet != null)
                {
                    string infoAreaId = recordIdentification.InfoAreaId();
                    if (!this.filterSet.Contains(infoAreaId))
                    {
                        return;     // InfoArea will not be added to History
                    }
                }

                HistoryEntry newEntry = new HistoryEntry(recordIdentification, viewReference, onlineData, imageName);
                this.HistoryEntries.RemoveAll(x => x.RecordIdentification == recordIdentification);
                this.HistoryEntries.Insert(0, newEntry);
                while (this.HistoryEntries.Count > 0 && this.HistoryEntries.Count > this.MaxEntries)
                {
                    this.HistoryEntries.RemoveAt(this.HistoryEntries.Count - 1);
                }

                this.SaveHistory();
            }
        }

        /// <summary>
        /// Releases the history manager.
        /// </summary>
        public static void ReleaseHistoryManager()
        {
            defaultManager = null;
        }

        /// <summary>
        /// Deletes the history.
        /// </summary>
        public void DeleteHistory()
        {
            Exception error;
            SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.TryDelete(this.FilePath, out error);
            this.HistoryEntries.Clear();
        }

        private string FilePath
        {
            get
            {
                IServerSession session = ServerSession.CurrentSession;
                string path = session.CrmAccount.AccountPath;
                return $"{path}\\history.crmpad";
            }
        }

        private async void LoadHistory()
        {
            string path = this.FilePath;

            if (SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.FileExists(path))
            {
                var entries = await SimpleIoc.Default.GetInstance<IStorageProvider>().LoadObject<List<HistoryEntrySerialized>>(path);

                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        this.HistoryEntries.Add(new HistoryEntry(entry));
                    }
                }
            }
        }

        private void SaveHistory()
        {
            SimpleIoc.Default.GetInstance<IStorageProvider>().SaveObject(this.HistoryEntries.Select(x => x.Serialized()), this.FilePath);
        }
    }
}
