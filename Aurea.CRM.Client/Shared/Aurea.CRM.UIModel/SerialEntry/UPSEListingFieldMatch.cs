// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEListingFieldMatch.cs" company="Aurea Software Gmbh">
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
//   UPSEListingFieldMatch
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Serial Entry Listing Field Match Result
    /// </summary>
    public enum UPSEListingFieldMatchResult
    {
        /// <summary>
        /// Yes
        /// </summary>
        Yes = 1,

        /// <summary>
        /// No
        /// </summary>
        No = 0,

        /// <summary>
        /// Open
        /// </summary>
        Open = -1
    }

    /// <summary>
    /// UPSEListingFieldMatch
    /// </summary>
    public class UPSEListingFieldMatch
    {
        /// <summary>
        /// Gets the function names.
        /// </summary>
        /// <value>
        /// The function names.
        /// </value>
        public List<string> FunctionNames { get; private set; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEListingFieldMatch"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="functionNames">The function names.</param>
        public UPSEListingFieldMatch(int index, List<string> functionNames)
        {
            this.FunctionNames = functionNames;
            this.Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEListingFieldMatch"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="functionNameString">The function name string.</param>
        public UPSEListingFieldMatch(int index, string functionNameString)
        {
            this.FunctionNames = !string.IsNullOrEmpty(functionNameString) ? functionNameString.Split(',').ToList() : new List<string> { "ItemNumber" };
            this.Index = index;
        }

        /// <summary>
        /// Listings the matches values.
        /// </summary>
        /// <param name="listing">The listing.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public UPSEListingFieldMatchResult ListingMatchesValues(UPSEListing listing, Dictionary<string, string> values)
        {
            UPSEListingFieldMatchResult result = UPSEListingFieldMatchResult.Yes;
            foreach (string functionName in this.FunctionNames)
            {
                string value = values.ValueOrDefault(functionName);
                string listingValue = listing.ValueDictionary.ValueOrDefault(functionName) as string;
                if (string.IsNullOrEmpty(value) || value == "0" || string.IsNullOrEmpty(listingValue) || listingValue == "0")
                {
                    result = UPSEListingFieldMatchResult.Open;
                }
                else if (value != listingValue)
                {
                    return UPSEListingFieldMatchResult.No;
                }
            }

            return result;
        }
    }
}
