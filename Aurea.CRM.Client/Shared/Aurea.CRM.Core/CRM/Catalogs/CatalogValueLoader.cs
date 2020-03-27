// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogValueLoader.cs" company="Aurea Software Gmbh">
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
//   Catalog value provider interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Catalogs
{
    using System.Collections.Generic;
    using System.Text;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Catalogs.UPCatalogValueLoaderBase" />
    public class UPCatalogValueLoader : UPCatalogValueLoaderBase
    {
        /// <summary>
        /// The catalog.
        /// </summary>
        private readonly UPCatalog catalog;

        /// <summary>
        /// The dependent catalog.
        /// </summary>
        private readonly bool dependentCatalog;

        /// <summary>
        /// The include hidden.
        /// </summary>
        private readonly bool includeHidden;

        /// <summary>
        /// The variable catalog.
        /// </summary>
        private readonly bool variableCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogValueLoader"/> class.
        /// </summary>
        /// <param name="fieldInfo">
        /// The _field information.
        /// </param>
        public UPCatalogValueLoader(UPCRMFieldInfo fieldInfo)
            : base(fieldInfo)
        {
            if (fieldInfo.FieldType == "K")
            {
                this.variableCatalog = true;
                this.dependentCatalog = fieldInfo.ParentCatalogFieldId >= 0;
                this.catalog = UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(fieldInfo.CatNo);
            }
            else if (fieldInfo.FieldType != "X")
            {
                return;
            }
            else
            {
                this.catalog = UPCRMDataStore.DefaultStore.CatalogForFixedCatalogId(fieldInfo.CatNo);
                this.variableCatalog = false;
            }

            this.includeHidden = !ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Catalog.HideLockedInFilters");
            if (this.catalog == null)
            {
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCatalogValueProvider" /> is dependent.
        /// </summary>
        /// <value>
        /// <c>true</c> if dependent; otherwise, <c>false</c>.
        /// </value>
        public override bool Dependent => this.dependentCatalog;

        /// <summary>
        /// Gets the parent values.
        /// </summary>
        /// <value>
        /// The parent values.
        /// </value>
        public override Dictionary<string, string> ParentValues
        {
            get
            {
                if (!this.dependentCatalog || this.FieldInfo.ParentCatalogFieldId < 0)
                {
                    return null;
                }

                var crmStore = UPCRMDataStore.DefaultStore;
                var parentInfo = crmStore.FieldInfoForInfoAreaFieldId(this.FieldInfo.InfoAreaId, this.FieldInfo.ParentCatalogFieldId);
                if (parentInfo == null)
                {
                    return null;
                }

                var parentCatalog = crmStore.CatalogForVariableCatalogId(parentInfo.CatNo);
                return parentCatalog?.TextValuesForFieldValues(this.includeHidden);
            }
        }

        /// <summary>
        /// Gets the sorted parent value codes.
        /// </summary>
        /// <value>
        /// The sorted parent value codes.
        /// </value>
        public override List<string> SortedParentValueCodes
        {
            get
            {
                if (!this.dependentCatalog)
                {
                    return null;
                }

                var crmStore = UPCRMDataStore.DefaultStore;
                var parentInfo = crmStore.FieldInfoForInfoAreaFieldId(this.FieldInfo.InfoAreaId, this.FieldInfo.ParentCatalogFieldId);
                if (parentInfo == null)
                {
                    return null;
                }

                var parentCatalog = crmStore.CatalogForVariableCatalogId(parentInfo.CatNo);
                return parentCatalog?.SortedValues;
            }
        }

        /// <summary>
        /// Displays the string for codes.
        /// </summary>
        /// <param name="codesArray">
        /// The codes array.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string DisplayStringForCodes(List<string> codesArray)
        {
            if (codesArray == null || codesArray.Count == 0)
            {
                return string.Empty;
            }

            if (this.dependentCatalog)
            {
                var codesForParent = new Dictionary<string, object>();
                foreach (var code in codesArray)
                {
                    var parentCode = this.ParentCodeForCode(code);
                    var parentCodeString = $"{parentCode}";
                    var arr = codesForParent.ValueOrDefault(parentCodeString) as List<string>;
                    if (arr != null)
                    {
                        arr.Add(code);
                    }
                    else
                    {
                        codesForParent.SetObjectForKey(new List<string> { code }, parentCodeString);
                    }
                }

                var firstParent = true;
                var parentValues = this.ParentValues;
                var mutableString = new StringBuilder();
                foreach (var parentCode in codesForParent.Keys)
                {
                    var first = true;
                    var codes = codesForParent.ValueOrDefault(parentCode) as List<string>;
                    var codeDictionary = this.PossibleValuesForParentCode(parentCode);
                    var parentValue = parentValues.ValueOrDefault(parentCode);
                    if (parentValue == null || codes == null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(parentValue))
                    {
                        parentValue = LocalizedString.TextEmptyCatalog;
                    }

                    foreach (var code in codes)
                    {
                        var value = codeDictionary.ValueOrDefault(code);
                        if (value == null)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(value))
                        {
                            value = LocalizedString.TextEmptyCatalog;
                        }

                        if (firstParent)
                        {
                            mutableString.Append($"{parentValue} ({value}");
                            firstParent = false;
                        }
                        else if (first)
                        {
                            mutableString.Append($"), {parentValue} ({value}");
                            first = false;
                        }
                        else
                        {
                            mutableString.Append($",{value}");
                        }
                    }
                }

                if (!firstParent)
                {
                    mutableString.Append(")");
                }

                return mutableString.ToString();
            }

            return base.DisplayStringForCodes(codesArray);
        }

        /// <summary>
        /// Determines whether [is empty value] [the specified code].
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsEmptyValue(string code)
        {
            return this.FieldInfo.FieldType == "K" || string.IsNullOrEmpty(this.catalog.ValueForCode(0)?.Text);
        }

        /// <summary>
        /// Parents the code for code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int ParentCodeForCode(string code)
        {
            return int.Parse(code) / 65536;
        }

        /// <summary>
        /// Possibles the values.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public override Dictionary<string, string> PossibleValues()
        {
            return this.variableCatalog
                       ? (this.dependentCatalog ? null : this.catalog?.TextValuesForFieldValues(this.includeHidden))
                       : this.catalog?.TextValuesForFieldValues(this.includeHidden);
        }

        /// <summary>
        /// Possibles the values for parent code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public override Dictionary<string, string> PossibleValuesForParentCode(string code)
        {
            if (!this.dependentCatalog)
            {
                return null;
            }

            var valuesForParent = this.catalog.ValuesForParentValueIncludeHidden(int.Parse(code), this.includeHidden);
            if (valuesForParent.Count <= 0)
            {
                return null;
            }

            var dict = new Dictionary<string, string>(valuesForParent.Count);
            foreach (var catalogValue in valuesForParent)
            {
                dict.SetObjectForKey(catalogValue.Text, catalogValue.CodeKey);
            }

            return dict;
        }

        /// <summary>
        /// Sorts the order for codes.
        /// </summary>
        /// <param name="codes">
        /// The codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public override List<string> SortOrderForCodes(List<string> codes)
        {
            return this.catalog.SortedValuesForCodes(codes);
        }
    }
}
