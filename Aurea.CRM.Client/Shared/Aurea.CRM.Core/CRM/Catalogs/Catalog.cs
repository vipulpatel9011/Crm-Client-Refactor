// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Catalog.cs" company="Aurea Software Gmbh">
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
//   Catalog implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.CRM.Catalogs
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Catalog implementation
    /// </summary>
    public class UPCatalog
    {
        /// <summary>
        /// The enable explicit tenant check
        /// </summary>
        private static bool enableExplicitTenantCheck = false;

        /// <summary>
        /// The _fixed.
        /// </summary>
        private readonly bool _fixed;

        /// <summary>
        /// The cat no.
        /// </summary>
        private readonly int catNo;

        /// <summary>
        /// The crm database.
        /// </summary>
        private readonly CRMDatabase crmDatabase;

        /// <summary>
        /// The catalog info.
        /// </summary>
        private CatalogInfo catalogInfo;

        /// <summary>
        /// The cat initialized.
        /// </summary>
        private bool catInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalog"/> class.
        /// </summary>
        /// <param name="catNo">
        /// The cat no.
        /// </param>
        /// <param name="crmDatabase">
        /// The CRM database.
        /// </param>
        /// <param name="isFixed">
        /// if set to <c>true</c> [is fixed].
        /// </param>
        public UPCatalog(int catNo, CRMDatabase crmDatabase, bool isFixed)
        {
            this.catNo = catNo;
            this._fixed = isFixed;
            this.catInitialized = false;
            this.catalogInfo = null;
            this.crmDatabase = crmDatabase;
        }

        /// <summary>
        /// Gets the explicit key order.
        /// </summary>
        /// <value>
        /// The explicit key order.
        /// </value>
        public List<string> ExplicitKeyOrder => this.ExplicitKeyOrderEmptyValueIncludeHidden(true, false);

        /// <summary>
        /// Gets the explicit key order by code.
        /// </summary>
        /// <value>
        /// The explicit key order by code.
        /// </value>
        public List<string> ExplicitKeyOrderByCode => this.ExplicitKeyOrderByCodeEmptyValueIncludeHidden(true, false);

        /// <summary>
        /// Gets a value indicating whether this instance is dependent.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is dependent; otherwise, <c>false</c>.
        /// </value>
        public bool IsDependent
        {
            get
            {
                if (!this.catInitialized)
                {
                    this.CatInit();
                }

                return this.catalogInfo?.IsDependent ?? false;
            }
        }

        /// <summary>
        /// Gets the sorted values.
        /// </summary>
        /// <value>
        /// The sorted values.
        /// </value>
        public List<string> SortedValues => this.ExplicitKeyOrderEmptyValueIncludeHidden(false, false);

        /// <summary>
        /// Gets the text values.
        /// </summary>
        /// <value>
        /// The text values.
        /// </value>
        public Dictionary<string, string> TextValues => this.TextValuesForFieldValues(false);

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<UPCatalogValue> Values
        {
            get
            {
                if (!this.catInitialized)
                {
                    this.CatInit();
                }

                var valueSet = this.catalogInfo?.GetValueSet();
                return this.ValuesFromCatalogValueSetSortedIncludeHiddenCheckTenants(valueSet, false, true, false);
            }
        }

        /// <summary>
        /// Gets the values for codes.
        /// </summary>
        /// <value>
        /// The values for codes.
        /// </value>
        public Dictionary<string, UPCatalogValue> ValuesForCodes
        {
            get
            {
                if (!this.catInitialized)
                {
                    this.CatInit();
                }

                var valueSet = this.catalogInfo?.GetValueSet();
                var count = valueSet?.Count ?? 0;
                var valueDictionary = new Dictionary<string, UPCatalogValue>(count);
                for (var i = 0; i < count; i++)
                {
                    var value = valueSet?.GetCatalogValueFromIndex(i);
                    if (value == null)
                    {
                        continue;
                    }

                    valueDictionary[$"{value.Code}"] = new UPCatalogValue(value);
                }

                return valueDictionary;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [zero has value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [zero has value]; otherwise, <c>false</c>.
        /// </value>
        public bool ZeroHasValue => this._fixed && !string.IsNullOrEmpty(this.TextValueForCode(0));

        /// <summary>
        /// Explicits the key order for catalog values.
        /// </summary>
        /// <param name="catalogValues">
        /// The catalog values.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> ExplicitKeyOrderForCatalogValues(List<UPCatalogValue> catalogValues)
        {
            if (catalogValues == null || catalogValues.Any())
            {
                return new List<string>();
            }

            return catalogValues.Select(c => c.CodeKey).ToList();
        }

        /// <summary>
        /// Sets the enable explicit tenant check.
        /// </summary>
        /// <param name="enable">
        /// if set to <c>true</c> [enable].
        /// </param>
        public static void SetEnableExplicitTenantCheck(bool enable)
        {
            enableExplicitTenantCheck = enable;
        }

        /// <summary>
        /// Values the dictionary for catalog values.
        /// </summary>
        /// <param name="catalogValues">
        /// The catalog values.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, string> ValueDictionaryForCatalogValues(List<UPCatalogValue> catalogValues)
        {
            var dict = new Dictionary<string, string>();
            foreach (UPCatalogValue value in catalogValues)
            {
                dict.SetObjectForKey(value.Text, value.CodeKey);
            }

            return dict;
        }

        /// <summary>
        /// Explicits the key order by code empty value include hidden.
        /// </summary>
        /// <param name="emptyValue">
        /// if set to <c>true</c> [empty value].
        /// </param>
        /// <param name="includeHidden">
        /// if set to <c>true</c> [include hidden].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> ExplicitKeyOrderByCodeEmptyValueIncludeHidden(bool emptyValue, bool includeHidden)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            var valueSet = this.catalogInfo?.GetValueSet();
            int i, count = valueSet != null ? valueSet.Values?.Count ?? 0 : 0;
            var keys = new List<string>(count + (emptyValue ? 1 : 0));
            if (emptyValue)
            {
                keys.Add(this._fixed ? string.Empty : "0");
            }

            List<int> tenantArray = null;
            if (ServerSession.CurrentSession.TenantNo > 0)
            {
                tenantArray = ServerSession.CurrentSession.AllUserTenants;
            }

            var tenantCount = tenantArray == null ? 0 : tenantArray.Count;
            int[] tenants = null;
            if (tenantCount > 0)
            {
                tenants = new int[tenantCount];
                for (i = 0; i < tenantCount; i++)
                {
                    tenants[i] = tenantArray[i];
                }
            }

            for (i = 0; i < count; i++)
            {
                var value = valueSet.GetCatalogValueFromIndex(i);
                if (!includeHidden && value.Access > 0)
                {
                    continue;
                }

                if (enableExplicitTenantCheck && value.GetAccessForTenants(tenantCount, tenants) > 0)
                {
                    continue;
                }

                keys.Add($"{value.Code}");
            }

            return keys;
        }

        /// <summary>
        /// Explicits the key order empty value include hidden.
        /// </summary>
        /// <param name="emptyValue">
        /// if set to <c>true</c> [empty value].
        /// </param>
        /// <param name="includeHidden">
        /// if set to <c>true</c> [include hidden].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> ExplicitKeyOrderEmptyValueIncludeHidden(bool emptyValue, bool includeHidden)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            var valueSet = this.catalogInfo?.GetValueSet();
            int i, count = valueSet != null ? valueSet.Values?.Count ?? 0 : 0;
            var keys = new List<string>(count + (emptyValue ? 1 : 0));
            if (emptyValue)
            {
                if (!this._fixed)
                {
                    keys.Add("0");
                }
            }

            List<int> tenantArray = null;
            if (ServerSession.CurrentSession.TenantNo > 0)
            {
                tenantArray = ServerSession.CurrentSession.AllUserTenants;
            }

            var tenantCount = tenantArray?.Count ?? 0;
            int[] tenants = null;
            if (tenantCount > 0)
            {
                tenants = new int[tenantCount];
                for (i = 0; i < tenantCount; i++)
                {
                    tenants[i] = tenantArray[i];
                }
            }

            for (i = 0; i < count; i++)
            {
                var value = valueSet.GetSortedCatalogValueFromIndex(i);
                if (!includeHidden && value.Access > 0)
                {
                    continue;
                }

                if (enableExplicitTenantCheck && value.GetAccessForTenants(tenantCount, tenants) > 0)
                {
                    continue;
                }

                keys.Add($"{value.Code}");
            }

            return keys;
        }

        /// <summary>
        /// Sorteds the values for codes.
        /// </summary>
        /// <param name="codeArray">
        /// The code array.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> SortedValuesForCodes(List<string> codeArray)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            if (this.catalogInfo == null)
            {
                return null;
            }

            if (!this.catalogInfo.IsDependent)
            {
                var valueSet = this.catalogInfo?.GetValueSet();
                return this.SortedValuesForCodesFromValueSet(codeArray, valueSet);
            }

            var parents = new Dictionary<string, object>();
            var lastParentCode = -1;
            List<object> lastArray = null;
            foreach (var v in codeArray)
            {
                var code = int.Parse(v);
                var parentCode = code >> 16;
                if (lastParentCode != parentCode)
                {
                    var parentCodeString = $"{parentCode:D}";
                    lastArray = parents.ValueOrDefault(parentCodeString) as List<object>;
                    if (lastArray == null)
                    {
                        lastArray = new List<object>();
                        parents.SetObjectForKey(lastArray, parentCodeString);
                    }

                    lastParentCode = parentCode;
                }

                lastArray?.Add(v);
            }

            if (parents.Count == 1)
            {
                return this.SortedValuesForCodesForParent(codeArray, lastParentCode);
            }

            if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Disable.81601"))
            {
                return null;
            }

            List<string> resultArray = null;
            var parentCatalog =
                UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(
                    ((DependentCatalogInfo)this.catalogInfo).ParentCatalogNr);
            var allParents = parents.Keys.ToList();
            var sortedParentValues = allParents;
            if (parentCatalog != null)
            {
                sortedParentValues = parentCatalog.SortedValuesForCodes(sortedParentValues);
                if (sortedParentValues.Count != allParents.Count)
                {
                    var newSortParents = allParents.Where(p => !sortedParentValues.Contains(p)).ToList();

                    newSortParents.AddRange(sortedParentValues);
                    sortedParentValues = newSortParents;
                }
            }

            foreach (var parentCodeString in sortedParentValues)
            {
                var parentCode = int.Parse(parentCodeString);
                var sortedValues =
                    this.SortedValuesForCodesForParent(
                        parents.ValueOrDefault(parentCodeString) as List<string>,
                        parentCode);
                if (sortedValues == null || !sortedValues.Any())
                {
                    continue;
                }

                if (resultArray != null)
                {
                    resultArray.AddRange(sortedValues);
                }
                else
                {
                    resultArray = new List<string>(sortedValues);
                }
            }

            return resultArray;
        }

        /// <summary>
        /// Sorteds the values for codes for parent.
        /// </summary>
        /// <param name="codeArray">
        /// The code array.
        /// </param>
        /// <param name="parentValue">
        /// The parent value.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> SortedValuesForCodesForParent(List<string> codeArray, int parentValue)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            if (this.catalogInfo == null)
            {
                return null;
            }

            if (!this.catalogInfo.IsDependent)
            {
                return null;
            }

            var valueSet = this.catalogInfo.CreateValueSet(parentValue);
            var sortedValues = this.SortedValuesForCodesFromValueSet(codeArray, valueSet);
            return sortedValues;
        }

        /// <summary>
        /// Sorteds the values for codes from value set.
        /// </summary>
        /// <param name="codeArray">
        /// The code array.
        /// </param>
        /// <param name="catalogValueSet">
        /// The catalog value set.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> SortedValuesForCodesFromValueSet(List<string> codeArray, CatalogValueSet catalogValueSet)
        {
            var valueDict = new Dictionary<string, object>();

            // NSDictionary.DictionaryWithObjectsForKeys(codeArray, codeArray);
            int i, count = catalogValueSet?.Values?.Count ?? 0;
            var resultArray = new List<string>(codeArray.Count);
            for (i = 0; i < count; i++)
            {
                var v = catalogValueSet.GetSortedCatalogValueFromIndex(i);
                if (v == null)
                {
                    continue;
                }

                var n = $"{v.Code}";
                if (valueDict.ValueOrDefault(n) != null)
                {
                    resultArray.Add(n);
                }
            }

            return resultArray;
        }

        /// <summary>
        /// Sorteds the values for parent value include hidden.
        /// </summary>
        /// <param name="parentValue">
        /// The parent value.
        /// </param>
        /// <param name="includeHidden">
        /// if set to <c>true</c> [include hidden].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCatalogValue> SortedValuesForParentValueIncludeHidden(int parentValue, bool includeHidden)
        {
            return this.ValuesForParentValueIncludeHidden(parentValue, includeHidden);
        }

        /// <summary>
        /// Texts the value for code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TextValueForCode(int code)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            return this.catalogInfo?.GetCatalogText(code);
        }

        /// <summary>
        /// Texts the value for key.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string TextValueForKey(string code)
        {
            return this.TextValueForCode(code.ToInt());
        }

        /// <summary>
        /// Texts the values for field values.
        /// </summary>
        /// <param name="includeHidden">
        /// if set to <c>true</c> [include hidden].
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, string> TextValuesForFieldValues(bool includeHidden)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            var valueSet = this.catalogInfo?.GetValueSet();
            int i, count = valueSet != null ? valueSet.Values?.Count ?? 0 : 0;
            var valueDictionary = new Dictionary<string, string>(count + 1) { ["0"] = string.Empty };
            List<int> tenantArray = null;
            if (ServerSession.CurrentSession.TenantNo > 0)
            {
                tenantArray = ServerSession.CurrentSession.AllUserTenants;
            }

            var tenantCount = tenantArray?.Count ?? 0;
            int[] tenants;
            if (tenantCount > 0)
            {
                tenants = new int[tenantCount];
                for (i = 0; i < tenantCount; i++)
                {
                    tenants[i] = tenantArray[i];
                }
            }
            else
            {
                tenants = null;
            }

            for (i = 0; i < count; i++)
            {
                var value = valueSet.GetCatalogValueFromIndex(i);
                if (!includeHidden && value.Access > 0)
                {
                    continue;
                }

                if (enableExplicitTenantCheck && value.GetAccessForTenants(tenantCount, tenants) != 0)
                {
                    continue;
                }

                valueDictionary[$"{value.Code}"] = value.Text;
            }

            return valueDictionary;
        }

        /// <summary>
        /// Values for code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="UPCatalogValue"/>.
        /// </returns>
        public UPCatalogValue ValueForCode(int code)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            var value = this.catalogInfo?.GetCatalogValue(code);
            return value != null ? new UPCatalogValue(value) : null;
        }

        /// <summary>
        /// Valueses for parent value include hidden.
        /// </summary>
        /// <param name="parentValue">
        /// The parent value.
        /// </param>
        /// <param name="includeHidden">
        /// if set to <c>true</c> [include hidden].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCatalogValue> ValuesForParentValueIncludeHidden(int parentValue, bool includeHidden)
        {
            if (!this.catInitialized)
            {
                this.CatInit();
            }

            if (!this.catalogInfo.IsDependent)
            {
                return null;
            }

            var valueSet = this.catalogInfo?.CreateValueSet(parentValue);
            if (valueSet == null)
            {
                return null;
            }

            var values = this.ValuesFromCatalogValueSetSortedIncludeHiddenCheckTenants(
                valueSet,
                true,
                includeHidden,
                enableExplicitTenantCheck);
            return values;
        }

        /// <summary>
        /// Valueses from catalog value set sorted include hidden check tenants.
        /// </summary>
        /// <param name="valueSet">
        /// The value set.
        /// </param>
        /// <param name="sorted">
        /// if set to <c>true</c> [sorted].
        /// </param>
        /// <param name="includeHidden">
        /// if set to <c>true</c> [include hidden].
        /// </param>
        /// <param name="checkTenants">
        /// if set to <c>true</c> [check tenants].
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCatalogValue> ValuesFromCatalogValueSetSortedIncludeHiddenCheckTenants(
            CatalogValueSet valueSet,
            bool sorted,
            bool includeHidden,
            bool checkTenants)
        {
            int i, count = valueSet?.Values?.Count ?? 0;
            var valueArray = new List<UPCatalogValue>(count);
            var tenantCount = 0;
            int[] tenants = null;
            if (checkTenants)
            {
                List<int> tenantArray = null;
                if (ServerSession.CurrentSession.TenantNo > 0)
                {
                    tenantArray = ServerSession.CurrentSession.AllUserTenants;
                }

                tenantCount = tenantArray?.Count ?? 0;
                if (tenantCount > 0)
                {
                    tenants = new int[tenantCount];
                    for (i = 0; i < tenantCount; i++)
                    {
                        tenants[i] = tenantArray[i];
                    }
                }
            }

            for (i = 0; i < count; i++)
            {
                var value = sorted ? valueSet?.GetSortedCatalogValueFromIndex(i) : valueSet?.GetCatalogValueFromIndex(i);
                if (!includeHidden && value?.Access > 0)
                {
                    continue;
                }

                if (checkTenants && value?.GetAccessForTenants(tenantCount, tenants) != 0)
                {
                    continue;
                }

                valueArray.Add(new UPCatalogValue(value));
            }

            return valueArray;
        }

        /// <summary>
        /// Cats the initialize.
        /// </summary>
        private void CatInit()
        {
            var dataModel = this.crmDatabase.DataModel;
            this.catInitialized = true;
            if (this._fixed)
            {
                this.catalogInfo = dataModel.GetFixCat(this.catNo);
            }
            else
            {
                this.catalogInfo = dataModel.GetVarCat(this.catNo);
            }
        }
    }
}
