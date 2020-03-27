// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Synchronization.cs" company="Aurea Software Gmbh">
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
//   Handles sync data
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Sync;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling.Data;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles sync data
    /// </summary>
    public class UPSynchronization
    {
        /// <summary>
        /// Gets logging interface
        /// </summary>
        //  public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        public ILogger Logger = SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSynchronization"/> class.
        /// </summary>
        /// <param name="crmDataStore">
        /// The CRM data store.
        /// </param>
        /// <param name="configStore">
        /// The configuration store.
        /// </param>
        public UPSynchronization(ICRMDataStore crmDataStore, IConfigurationUnitStore configStore)
        {
            this.DataStore = crmDataStore;
            this.ConfigStore = configStore;
        }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Gets or sets the configuration store.
        /// </summary>
        /// <value>
        /// The configuration store.
        /// </value>
        private IConfigurationUnitStore ConfigStore { get; set; }

        /// <summary>
        /// Gets or sets the record count.
        /// </summary>
        /// <value>
        /// The record count.
        /// </value>
        private int RecordCount { get; set; }

        /// <summary>
        /// Synchronizes the with data dictionary.
        /// </summary>
        /// <param name="dataDict">
        /// The data dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SyncWithDataDictionary(Dictionary<string, object> dataDict)
        {
            var fullSync = false;
            var dataModelDef = dataDict.ValueOrDefault("dataModel") as JObject;
            if (dataModelDef != null)
            {
                // DDLogCRequest("Import DATAMODEL");
                this.Logger.LogDebug("Import DATAMODEL", LogFlag.LogRequests);
                var dataModelSync = new UPSyncDataModel(this.DataStore);

                dataModelSync.SyncWithDataModelDefinition(dataModelDef.ToObject<Dictionary<string, object>>());

                fullSync = true;
            }

            var catalogDef = dataDict.ValueOrDefault("fixedCatalogs") as JArray;
            UPSyncCatalogs catalogSync = null;
            if (catalogDef != null)
            {
                // DDLogCRequest("Import FIXED CATALOGS");
                this.Logger.LogDebug("Import FIXED CATALOGS", LogFlag.LogRequests);
                catalogSync = new UPSyncCatalogs(this.DataStore);
                catalogSync.SyncFixedCatalogs(catalogDef.ToObject<List<object>>());
            }

            catalogDef = dataDict.ValueOrDefault("variableCatalogs") as JArray;
            if (catalogDef != null)
            {
                // DDLogCRequest("Import VARIABLE CATALOGS");
                this.Logger.LogDebug("Import VARIABLE CATALOGS", LogFlag.LogRequests);
                if (catalogSync == null)
                {
                    catalogSync = new UPSyncCatalogs(this.DataStore);
                }

                catalogSync.SyncVariableCatalogs(catalogDef.ToObject<List<object>>());
            }

            var configurationDef = dataDict.ValueOrDefault("configuration") as JObject;
            if (configurationDef != null)
            {
                // DDLogCRequest("Import CONFIGURATION");
                this.Logger.LogDebug("Import CONFIGURATION", LogFlag.LogRequests);
                var configurationSync = new UPSyncConfiguration(this.ConfigStore);
                configurationSync.SyncWithConfigurationDefinition(configurationDef.ToObject<Dictionary<string, object>>());

                if (fullSync)
                {
                    var v = this.ConfigStore.ConfigValue("System.iOSServerTimeZone");
                    if (!string.IsNullOrEmpty(v))
                    {
                        this.DataStore.SetTimeZoneUtcDifference(v, 0);
                    }
                }
            }

            bool isEnterprise = false;
            var licenseDef = dataDict.ValueOrDefault("licenseInfo") as JObject;

            if (licenseDef != null)
            {
                // DDLogCRequest("Import LICENSEINFO");
                this.Logger.LogDebug("Import LICENSEINFO", LogFlag.LogRequests);
                Dictionary<string, object> licenseDefDictionary = licenseDef.ToObject<Dictionary<string, object>>();
                isEnterprise = bool.Parse(licenseDefDictionary.ValueOrDefault("IsEnterpriseVersion").ToString());
#if PORTING
                NSUserDefaults.StandardUserDefaults().SetBoolForKey(isEnterprise.BoolValue, "System.isEnterprise");
                NSUserDefaults.StandardUserDefaults().Synchronize();
#endif
            }

            var recordDef = dataDict.ValueOrDefault("records") as JArray;

            if (recordDef != null)
            {
                // DDLogCRequest("Import RECORDS");
                this.Logger.LogDebug("Import RECORDS", LogFlag.LogRequests);
                var recordSyncResult =
                    UPCRMRecordSync.SyncRecordSetDefinitionsCrmDataStore(recordDef.ToObject<List<object>>(), this.DataStore, isEnterprise);
                if (recordSyncResult.Successful)
                {
                    this.RecordCount += recordSyncResult.RecordCount;
                }
                else
                {
                    return recordSyncResult.ReturnCode;
                }
            }

            var queryDef = dataDict.ValueOrDefault("queries");
            if (queryDef != null)
            {
                var querySync = new UPSyncQuery(this.DataStore);
                querySync.SyncWithQueryDefinition((List<object>)queryDef);
            }

            return 0;
        }

        /// <summary>
        /// Synchronizes the with data dictionary.
        /// </summary>
        /// <param name="dataDict">
        /// The data dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SyncWithDataObject(DataModelSyncDeserializer dataDict)
        {
            bool isEnterprise = false;
            var licenseDef = dataDict.licenseInfo;

            if (licenseDef != null)
            {
                // DDLogCRequest("Import LICENSEINFO");
                this.Logger.LogDebug("Import LICENSEINFO", LogFlag.LogRequests);
                Dictionary<string, object> licenseDefDictionary = licenseDef.ToDictionary();
                isEnterprise = bool.Parse(licenseDefDictionary.ValueOrDefault("IsEnterpriseVersion").ToString());
#if PORTING
                NSUserDefaults.StandardUserDefaults().SetBoolForKey(isEnterprise.BoolValue, "System.isEnterprise");
                NSUserDefaults.StandardUserDefaults().Synchronize();
#endif
            }

            var recordDef = dataDict.records;

            if (recordDef != null)
            {
                // DDLogCRequest("Import RECORDS");
                this.Logger.LogDebug("Import RECORDS", LogFlag.LogRequests);
                var recordSyncResult =
                    UPCRMRecordSync.SyncRecordSetDefinitionsCrmDataStore(recordDef, this.DataStore, isEnterprise);
                if (recordSyncResult.Successful)
                {
                    this.RecordCount += recordSyncResult.RecordCount;
                }
                else
                {
                    return recordSyncResult.ReturnCode;
                }
            }
                       
            return 0;
        }
    }
}
