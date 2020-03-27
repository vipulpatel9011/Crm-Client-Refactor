// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogValueLoaderBase.cs" company="Aurea Software Gmbh">
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
    /// Catalog value loader base implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Catalogs.UPCatalogValueProvider" />
    public class UPCatalogValueLoaderBase : UPCatalogValueProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCatalogValueLoaderBase"/> class.
        /// </summary>
        /// <param name="fieldInfo">
        /// The field information.
        /// </param>
        public UPCatalogValueLoaderBase(UPCRMFieldInfo fieldInfo)
        {
            this.FieldInfo = fieldInfo;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCatalogValueProvider" /> is dependent.
        /// </summary>
        /// <value>
        ///   <c>true</c> if dependent; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Dependent => false;

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public UPCRMFieldInfo FieldInfo { get; private set; }

        /// <summary>
        /// Gets the parent values.
        /// </summary>
        /// <value>
        /// The parent values.
        /// </value>
        public virtual Dictionary<string, string> ParentValues => null;

        /// <summary>
        /// Gets the sorted parent value codes.
        /// </summary>
        /// <value>
        /// The sorted parent value codes.
        /// </value>
        public virtual List<string> SortedParentValueCodes => null;

        /// <summary>
        /// Displays the string for codes.
        /// </summary>
        /// <param name="codes">
        /// The codes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string DisplayStringForCodes(List<string> codes)
        {
            var mutableString = new StringBuilder();
            var dict = this.PossibleValues();
            bool first = true;
            foreach (var code in codes)
            {
                var val = dict.ValueOrDefault(code) as string;
                if (val == null)
                {
                    continue;
                }

                if (!first)
                {
                    mutableString.Append(", ");
                }
                else
                {
                    first = false;
                }

                mutableString.Append(string.IsNullOrEmpty(val) ? LocalizedString.TextEmptyCatalog : val);
            }

            return mutableString.ToString();
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
        public virtual bool IsEmptyValue(string code)
        {
            return true;
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
        public virtual int ParentCodeForCode(string code)
        {
            return 0;
        }

        /// <summary>
        /// Possibles the values.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public virtual Dictionary<string, string> PossibleValues() => null;

        /// <summary>
        /// Possibles the values for parent code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public virtual Dictionary<string, string> PossibleValuesForParentCode(string code) => null;

        /// <summary>
        /// Sorts the order for codes.
        /// </summary>
        /// <param name="codes">
        /// The codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public virtual List<string> SortOrderForCodes(List<string> codes)
        {
            var dict = this.PossibleValues();
            return dict.SortedKeysFromKeyArray(codes);
        }
    }
}
