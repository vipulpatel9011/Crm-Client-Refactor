// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncConfiguration.cs" company="Aurea Software Gmbh">
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
// <summary>
//   Interface the sync configurations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Logging;
    using GalaSoft.MvvmLight.Messaging;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Interface the sync configurations
    /// </summary>
    public class UPSyncConfiguration
    {
        /// <summary>
        /// Gets the log provider settings
        /// </summary>
        /// public static ILogSettings Logsettings => SimpleIoc.Default.GetInstance<ILogger>()?.LogSettings;


        public static ILogSettings Logsettings => SimpleIoc.Default.GetInstance<ILogger>()?.LogSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncConfiguration"/> class.
        /// </summary>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        public UPSyncConfiguration(IConfigurationUnitStore configStore)
        {
            this.ConfigStore = configStore;
        }

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        public IConfigurationUnitStore ConfigStore { get; private set; }

        /// <summary>
        /// Synchronizes the type of the elements of unit.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="unitTypeName">Name of the unit type.</param>
        /// <param name="emptyTable">if set to <c>true</c> [empty table].</param>
        /// <returns></returns>
        public int SyncElementsOfUnitType(JArray elements, string unitTypeName, bool emptyTable)
        {
            return this.ConfigStore.SyncElementsOfUnitTypeEmptyTable(elements, unitTypeName, emptyTable);
        }

        /// <summary>
        /// Synchronizes the with configuration definition.
        /// </summary>
        /// <param name="configurationDef">
        /// The configuration definition.
        /// </param>
        /// <returns>
        /// 1 if failed, else 0
        /// </returns>
        public int SyncWithConfigurationDefinition(Dictionary<string, object> configurationDef)
        {
            if (configurationDef == null)
            {
                return 0;
            }

            var ret = 0;
            var keyenum = configurationDef.GetEnumerator();
            while (ret == 0 && keyenum.MoveNext())
            {
                var entry = keyenum.Current;
                ret = this.ConfigStore?.SyncElementsOfUnitTypeEmptyTable(entry.Value as JArray, entry.Key, true) ?? 0;
            }

            this.ConfigStore?.Reset();
            if (ret == 0)
            {
                Messenger.Default.Send(this.ConfigStore);
            }

            Logsettings.UpdateSettingsFromSession(this.ConfigStore);
            return ret;
        }
    }
}
