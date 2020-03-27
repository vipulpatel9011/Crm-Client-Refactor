// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEListingOwner.cs" company="Aurea Software Gmbh">
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
//   UPSEListingOwner
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// UPSEListingOwner
    /// </summary>
    public class UPSEListingOwner
    {
        private Dictionary<string, UPSEListing> allListings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEListingOwner"/> class.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <param name="recordIndex">Index of the record.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="listingController">The listing controller.</param>
        public UPSEListingOwner(UPCRMResultRow resultRow, int recordIndex,
            Dictionary<string, UPConfigFieldControlField> mapping, UPSEListingController listingController)
        {
            this.RecordIdentification = resultRow.RecordIdentificationAtIndex(recordIndex);
            this.ListingController = listingController;
            this.ValueDictionary = mapping.ValuesFromResultRow(resultRow);
            Dictionary<string, string> displayValueDictionary = mapping.DisplayValuesFromResultRow(resultRow);
            if (!string.IsNullOrEmpty(this.ListingController.ListingLabelFormat))
            {
                this.Label = this.ListingController.ListingLabelFormat;
                foreach (string key in displayValueDictionary.Keys)
                {
                    this.Label = this.Label.Replace($"%%{key}", displayValueDictionary[key]);
                }
            }
            else
            {
                this.Label = displayValueDictionary.ValueOrDefault("Name");
            }

            if (!string.IsNullOrEmpty(this.Label))
            {
                this.Label = this.RecordIdentification;
            }
        }

        /// <summary>
        /// Gets or sets the related owners.
        /// </summary>
        /// <value>
        /// The related owners.
        /// </value>
        public List<UPSEListingOwner> RelatedOwners { get; set; }

        /// <summary>
        /// Gets a value indicating whether [listings loaded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [listings loaded]; otherwise, <c>false</c>.
        /// </value>
        public bool ListingsLoaded => this.Listings != null;

        /// <summary>
        /// Gets the listing controller.
        /// </summary>
        /// <value>
        /// The listing controller.
        /// </value>
        public UPSEListingController ListingController { get; private set; }

        /// <summary>
        /// Gets the listings.
        /// </summary>
        /// <value>
        /// The listings.
        /// </value>
        public List<UPSEListing> Listings { get; private set; }

        /// <summary>
        /// Gets the owner data.
        /// </summary>
        /// <value>
        /// The owner data.
        /// </value>
        public List<object> OwnerData { get; private set; }

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
        /// Gets all listings.
        /// </summary>
        /// <value>
        /// All listings.
        /// </value>
        public Dictionary<string, UPSEListing> AllListings
            => this.allListings ?? (this.allListings = this.AllListingsWithMaxDepth(10));

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Listingses from result.
        /// </summary>
        /// <param name="result">The result.</param>
        public void ListingsFromResult(UPCRMResult result)
        {
            List<UPSEListing> listingArray = new List<UPSEListing>();
            int count = result.RowCount;
            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                UPSEListing item = new UPSEListing(row, this.ListingController.ListingMapping, this);
                listingArray.Add(item);
            }

            this.Listings = listingArray;
        }

        /// <summary>
        /// Alls the listings with maximum depth.
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        public Dictionary<string, UPSEListing> AllListingsWithMaxDepth(int depth)
        {
            if (this.allListings != null)
            {
                return this.allListings;
            }

            Dictionary<string, UPSEListing> allListings = null;
            if (this.Listings.Count > 0)
            {
                allListings = new Dictionary<string, UPSEListing>(this.Listings.Count);
                foreach (UPSEListing listing in this.Listings)
                {
                    allListings[listing.RecordIdentification] = listing;
                }
            }

            if (this.RelatedOwners.Count == 0)
            {
                this.allListings = allListings.Count == 0 ? new Dictionary<string, UPSEListing>() : allListings;

                return this.allListings;
            }

            if (depth == 0)
            {
                // DDLogCError("listing computation stopped after reaching max depth of related listing owners - check if RelatedListingOwnersConfigName searchandlist is correct");
                Logger.LogError("Listing computation stopped after reaching max depth of related listing owners - check if RelatedListingOwnersConfigName searchandlist is correct");
                this.allListings = allListings.Count == 0 ? new Dictionary<string, UPSEListing>() : allListings;

                return this.allListings;
            }

            if (allListings == null)
            {
                allListings = new Dictionary<string, UPSEListing>();
            }

            foreach (UPSEListingOwner relatedOwner in this.RelatedOwners)
            {
                Dictionary<string, UPSEListing> relatedListings = relatedOwner.AllListingsWithMaxDepth(depth - 1);
                if (relatedListings.Count > 0)
                {
                    foreach (var entry in relatedListings)
                    {
                        allListings[entry.Key] = entry.Value;
                    }
                }
            }

            if (depth >= 5)
            {
                this.allListings = allListings;
            }

            return allListings;
        }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets a value indicating whether [transparent owner].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [transparent owner]; otherwise, <c>false</c>.
        /// </value>
        public bool TransparentOwner => this.Listings.Count == 0 && this.RelatedOwners.Count < 2;

        /// <summary>
        /// Gets the actual listing owner.
        /// </summary>
        /// <value>
        /// The actual listing owner.
        /// </value>
        public UPSEListingOwner ActualListingOwner
        {
            get
            {
                if (this.TransparentOwner)
                {
                    if (this.RelatedOwners.Count == 1)
                    {
                        return this.RelatedOwners[0].ActualListingOwner;
                    }
                }

                return this;
            }
        }
    }
}
