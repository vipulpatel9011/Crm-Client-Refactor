// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEListing.cs" company="Aurea Software Gmbh">
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
//   UPSEListing
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSEListing
    /// </summary>
    public class UPSEListing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEListing"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="owner">The owner.</param>
        public UPSEListing(UPCRMResultRow resultRow, Dictionary<string, UPConfigFieldControlField> mapping, UPSEListingOwner owner)
        {
            this.ListingOwner = owner;
            this.RecordIdentification = resultRow.RootRecordIdentification;
            this.ValueDictionary = mapping.ValuesFromResultRow(resultRow);
        }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordId => this.RecordIdentification.RecordId();

        /// <summary>
        /// Gets the listing owner.
        /// </summary>
        /// <value>
        /// The listing owner.
        /// </value>
        public UPSEListingOwner ListingOwner { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the value dictionary.
        /// </summary>
        /// <value>
        /// The value dictionary.
        /// </value>
        public Dictionary<string, object> ValueDictionary { get; private set; }

        /// <summary>
        /// Values the maximum index of the dictionary match in hierarchy.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int ValueDictionaryMatchInHierarchyMaxIndex(Dictionary<string, string> values, List<UPSEListingFieldMatch> array, int index)
        {
            int maxIndex = index;
            if (maxIndex > array.Count || maxIndex == 0)
            {
                maxIndex = array.Count;
            }

            for (int i = 0; i < maxIndex; i++)
            {
                UPSEListingFieldMatch item = array[i];
                UPSEListingFieldMatchResult result = item.ListingMatchesValues(this, values);
                if (result == UPSEListingFieldMatchResult.Yes)
                {
                    return item.Index;
                }

                if (result == UPSEListingFieldMatchResult.No)
                {
                    return -1;
                }
            }

            return -1;
        }
    }
}
