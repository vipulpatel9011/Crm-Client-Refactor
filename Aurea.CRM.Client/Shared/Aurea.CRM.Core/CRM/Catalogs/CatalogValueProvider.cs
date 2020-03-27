// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogValueProvider.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Catalog value provider interface
    /// </summary>
    public interface UPCatalogValueProvider
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCatalogValueProvider"/> is dependent.
        /// </summary>
        /// <value>
        /// <c>true</c> if dependent; otherwise, <c>false</c>.
        /// </value>
        bool Dependent { get; }

        /// <summary>
        /// Gets the parent values.
        /// </summary>
        /// <value>
        /// The parent values.
        /// </value>
        Dictionary<string, string> ParentValues { get; }

        /// <summary>
        /// Gets the sorted parent value codes.
        /// </summary>
        /// <value>
        /// The sorted parent value codes.
        /// </value>
        List<string> SortedParentValueCodes { get; }

        /// <summary>
        /// Displays the string for codes.
        /// </summary>
        /// <param name="codes">
        /// The codes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string DisplayStringForCodes(List<string> codes);

        /// <summary>
        /// Determines whether [is empty value] [the specified code].
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsEmptyValue(string code);

        /// <summary>
        /// Parents the code for code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int ParentCodeForCode(string code);

        /// <summary>
        /// Possibles the values.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        Dictionary<string, string> PossibleValues();

        /// <summary>
        /// Possibles the values for parent code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        Dictionary<string, string> PossibleValuesForParentCode(string code);

        /// <summary>
        /// Sorts the order for codes.
        /// </summary>
        /// <param name="codes">
        /// The codes.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<string> SortOrderForCodes(List<string> codes);
    }
}
