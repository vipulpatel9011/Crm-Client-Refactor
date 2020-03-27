// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPricingBulkVolumeMatch.cs" company="Aurea Software Gmbh">
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
//   Pricing Bulk Volume Match
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Serial Entry Pricing Bulk Volume Match
    /// </summary>
    public class UPSEPricingBulkVolumeMatch
    {
        private Dictionary<string, UPSEPricingBulkVolume> bulkVolumeDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEPricingBulkVolumeMatch"/> class.
        /// </summary>
        /// <param name="columnString">The column string.</param>
        public UPSEPricingBulkVolumeMatch(string columnString)
        {
            this.MatchColumns = columnString.Split(',').ToList();
        }

        /// <summary>
        /// Gets the match columns.
        /// </summary>
        /// <value>
        /// The match columns.
        /// </value>
        public List<string> MatchColumns { get; private set; }

        /// <summary>
        /// Adds the bulk volume.
        /// </summary>
        /// <param name="bulkVolume">The bulk volume.</param>
        /// <returns></returns>
        public bool AddBulkVolume(UPSEPricingBulkVolume bulkVolume)
        {
            string key = this.StringIdentifierForDictionaryReturnOnEmpty(bulkVolume.Data, true);
            if (!string.IsNullOrEmpty(key))
            {
                if (this.bulkVolumeDictionary == null)
                {
                    this.bulkVolumeDictionary = new Dictionary<string, UPSEPricingBulkVolume>();
                }

                this.bulkVolumeDictionary[key] = bulkVolume;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Bulks the volume for dictionary.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <returns></returns>
        public UPSEPricingBulkVolume BulkVolumeForDictionary(Dictionary<string, object> dict)
        {
            string key = this.StringIdentifierForDictionaryReturnOnEmpty(dict, true);
            return !string.IsNullOrEmpty(key) ? this.bulkVolumeDictionary.ValueOrDefault(key) : null;
        }

        private string StringIdentifierForDictionaryReturnOnEmpty(Dictionary<string, object> dict, bool returnOnEmpty)
        {
            bool found = false;
            string identifierString = null;
            foreach (string matchColumn in this.MatchColumns)
            {
                string val = dict.ValueOrDefault(matchColumn) as string;
                if (!string.IsNullOrEmpty(val) && val != "0")
                {
                    found = true;
                }
                else
                {
                    if (returnOnEmpty)
                    {
                        return null;
                    }

                    val = string.Empty;
                }

                identifierString = identifierString == null ? val : $"{identifierString},{val}";
            }

            return found ? identifierString : null;
        }
    }
}
