// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogValueLoaderWithFixedParent.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Catalog value loader with fixed parent
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Catalogs.UPCatalogValueLoaderBase" />
    public class UPCatalogValueLoaderWithFixedParent : UPCatalogValueLoaderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogValueLoaderWithFixedParent"/> class.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        /// <param name="parentValue">
        /// The parent value.
        /// </param>
        public UPCatalogValueLoaderWithFixedParent(UPCRMFieldInfo fieldInfo, string parentValue)
            : base(fieldInfo)
        {
            if (fieldInfo.FieldType != "K" || fieldInfo.ParentCatalogFieldId < 0)
            {
                return;
            }

            this.ParentValue = parentValue;
        }

        /// <summary>
        /// Gets the parent value.
        /// </summary>
        /// <value>
        /// The parent value.
        /// </value>
        public string ParentValue { get; private set; }

        /// <summary>
        /// Possibles the values.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public override Dictionary<string, string> PossibleValues()
        {
            var catalog = UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(this.FieldInfo.CatNo);
            var childValues = catalog.SortedValuesForParentValueIncludeHidden(int.Parse(this.ParentValue), false);
            var dict = new Dictionary<string, string>(childValues.Count);
            foreach (var val in childValues)
            {
                dict[val.CodeKey] = val.Text;
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
            var catalog = UPCRMDataStore.DefaultStore.CatalogForVariableCatalogId(this.FieldInfo.CatNo);
            return catalog.SortedValuesForCodesForParent(codes, int.Parse(this.ParentValue));
        }
    }
}
