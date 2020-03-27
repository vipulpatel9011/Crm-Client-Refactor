// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncCatalog.cs" company="Aurea Software Gmbh">
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
//   Handles catalog data synchronization
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Common
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Handles catalog data synchronization
    /// </summary>
    public class UPSyncCatalogs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSyncCatalogs"/> class.
        /// </summary>
        /// <param name="dataStore">
        /// The data store.
        /// </param>
        public UPSyncCatalogs(ICRMDataStore dataStore)
        {
            this.DataStore = dataStore;
        }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public ICRMDataStore DataStore { get; private set; }

        /// <summary>
        /// Catalogs the value set with dependent values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// a catalog value set
        /// </returns>
        public static CatalogValueSet CatalogValueSetWithDependentValues(List<object> values)
        {
            var valueSet = new CatalogValueSet();
            if (values == null)
            {
                return valueSet;
            }

            foreach (JArray catval in values)
            {
                var catvaldef = catval?.ToObject<List<object>>();
                var value = new UPCatalogValue(catvaldef);
                valueSet.AddWithOwnership(
                    new DependentCatalogValue(
                        value.Code,
                        value.ParentCode,
                        value.Text,
                        value.Tenant,
                        value.ExtKey,
                        value.Sortinfo,
                        value.Access,
                        true));
            }

            return valueSet;
        }

        /// <summary>
        /// Catalogs the value set with fixed values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// a catalog value set
        /// </returns>
        public static CatalogValueSet CatalogValueSetWithFixedValues(List<object> values)
        {
            var valueSet = new CatalogValueSet();
            if (values == null)
            {
                return valueSet;
            }

            foreach (JArray catval in values)
            {
                var catvaldef = catval?.ToObject<List<object>>();
                var value = new UPCatalogValue(catvaldef);
                valueSet.AddWithOwnership(new FixedCatalogValue(value.Code, value.Text, value.Sortinfo, value.Access, true));
            }

            return valueSet;
        }

        /// <summary>
        /// Catalogs the value set with variable values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// a catalog value set
        /// </returns>
        public static CatalogValueSet CatalogValueSetWithVariableValues(List<object> values)
        {
            var valueSet = new CatalogValueSet();
            if (values == null)
            {
                return valueSet;
            }

            foreach (JArray catval in values)
            {
                var catvaldef = catval?.ToObject<List<object>>();
                var value = new UPCatalogValue(catvaldef);
                valueSet.AddWithOwnership(
                    new VariableCatalogValue(
                        value.Code,
                        value.Text,
                        value.Tenant,
                        value.ExtKey,
                        value.Sortinfo,
                        value.Access,
                        true));
            }

            return valueSet;
        }

        /// <summary>
        /// Synchronizes the fixed catalogs.
        /// </summary>
        /// <param name="catalogDefs">
        /// The catalog defs.
        /// </param>
        /// <returns>
        /// 1 if failed, else 0
        /// </returns>
        public int SyncFixedCatalogs(List<object> catalogDefs)
        {
            if (catalogDefs == null)
            {
                return 0;
            }

            foreach (var catdef in catalogDefs)
            {
                var valDef = (catdef as JArray)?.ToObject<List<object>>();
                if (valDef == null)
                {
                    continue;
                }

                var ret = this.SyncFixedCatalogValues(
                    JObjectExtensions.ToInt(valDef[0]),
                    (valDef[1] as JArray)?.ToObject<List<object>>());
                if (ret == -1)
                {
                    ret = 0;
                }

                // ignore unknown catalogs
                if (ret > 0)
                {
                    continue;
                }
            }

            return 0;
        }

        /// <summary>
        /// Synchronizes the fixed catalog values.
        /// </summary>
        /// <param name="catNr">
        /// The cat nr.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// 1 if failed, else 0
        /// </returns>
        public int SyncFixedCatalogValues(int catNr, List<object> values)
        {
            var database = this.DataStore.DatabaseInstance;
            var datamodel = database.DataModel;
            var catInfo = datamodel.GetFixCat(catNr);
            if (catInfo == null)
            {
                return -1;
            }

            var catalogValueSet = CatalogValueSetWithFixedValues(values);
            catInfo.SetCatalogValueSetWithOwnership(catalogValueSet);
            return catInfo.Update();
        }

        /// <summary>
        /// Synchronizes the variable catalogs.
        /// </summary>
        /// <param name="catalogDefs">
        /// The catalog defs.
        /// </param>
        /// <returns>
        /// 1 if failed, else 0
        /// </returns>
        public int SyncVariableCatalogs(List<object> catalogDefs)
        {
            var ret = 0;
            var first = true;
            string timestamp = null;

            foreach (var catdef in catalogDefs)
            {
                var catdefArray = (catdef as JArray)?.ToObject<List<object>>();
                if (catdefArray == null)
                {
                    continue;
                }

                ret = this.SyncVariableCatalogValues(
                    JObjectExtensions.ToInt(catdefArray[0]),
                    (catdefArray[1] as JArray)?.ToObject<List<object>>());
                if (ret == -1)
                {
                    ret = 0;
                }

                // ignore unknown catalogs
                if (ret > 0)
                {
                    continue;
                }

                if (!first || catdefArray.Count <= 2)
                {
                    continue;
                }

                timestamp = catdefArray[2] as string;
                first = false;
            }

            if (!string.IsNullOrEmpty(timestamp))
            {
                this.DataStore.ReportSyncWithDatasetRecordCountTimestampFullSyncInfoAreaId(
                    "VariableCatalogs",
                    0,
                    timestamp,
                    false,
                    null);
            }

            return ret;
        }

        /// <summary>
        /// Synchronizes the variable catalog values.
        /// </summary>
        /// <param name="catNr">
        /// The cat nr.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// 1 if failed, else 0
        /// </returns>
        public int SyncVariableCatalogValues(int catNr, List<object> values)
        {
            var database = this.DataStore.DatabaseInstance;
            var datamodel = database.DataModel;
            var varcatInfo = datamodel.GetVarCat(catNr);
            if (varcatInfo == null)
            {
                return -1;
            }

            var catalogValueSet = varcatInfo.IsDependent
                                      ? CatalogValueSetWithDependentValues(values)
                                      : CatalogValueSetWithVariableValues(values);

            varcatInfo.SetCatalogValueSetWithOwnership(catalogValueSet);
            return varcatInfo.Update();
        }
    }
}
