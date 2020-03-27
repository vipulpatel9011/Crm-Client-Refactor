// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuickSearch.cs" company="Aurea Software Gmbh">
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
//   defines quick search configurations
//   corresponds to UPConfigQuickSearch in CRM.Pad
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System.Collections.Generic;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// defines quick search configurations
    /// corresponds to UPConfigQuickSearch in CRM.Pad
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class QuickSearch : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickSearch"/> class.
        /// </summary>
        /// <param name="defArray">
        /// The definition array.
        /// </param>
        public QuickSearch(List<object> defArray)
        {
            this.UnitName = (string)defArray[0];
            var entryArray = defArray[1] as JArray;
            if (entryArray != null)
            {
                var count = entryArray.Count;
                var arr = new List<QuickSearchEntry>(count);
                for (var i = 0; i < count; i++)
                {
                    var internalArray = entryArray[i] as JArray;
                    if (internalArray != null)
                    {
                        arr.Add(new QuickSearchEntry(internalArray.ToObject<List<object>>()));
                    }
                }

                this.Entries = arr;
            }

            if (this.Entries == null)
            {
                return;
            }

            var arri = new List<object>();
            var dict = new Dictionary<string, List<QuickSearchEntry>>();
            foreach (var entry in this.Entries)
            {
                var infoAreaId = entry.InfoAreaId;
                var infoAreaArray = dict.ValueOrDefault(infoAreaId);
                if (infoAreaArray == null)
                {
                    infoAreaArray = new List<QuickSearchEntry> { entry };
                    dict[infoAreaId] = infoAreaArray;
                    arri.Add(infoAreaId);
                }
                else
                {
                    infoAreaArray.Add(entry);
                }
            }

            this.InfoAreaIds = arri;
            this.EntryPerInfoAreaId = dict;
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public List<QuickSearchEntry> Entries { get; }

        /// <summary>
        /// Gets the entry per information area identifier.
        /// </summary>
        /// <value>
        /// The entry per information area identifier.
        /// </value>
        public Dictionary<string, List<QuickSearchEntry>> EntryPerInfoAreaId { get; }

        /// <summary>
        /// Gets the information area ids.
        /// </summary>
        /// <value>
        /// The information area ids.
        /// </value>
        public List<object> InfoAreaIds { get; }

        /// <summary>
        /// Gets the number of entries.
        /// </summary>
        /// <value>
        /// The number of entries.
        /// </value>
        public int NumberOfEntries => this.Entries.Count;

        /// <summary>
        /// Gets or sets the number of information areas.
        /// </summary>
        /// <value>
        /// The number of information areas.
        /// </value>
        public int NumberOfInfoAreas => this.InfoAreaIds.Count;

        /// <summary>
        /// Entrieses for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<QuickSearchEntry> EntriesForInfoAreaId(string infoAreaId)
        {
            return this.EntryPerInfoAreaId[infoAreaId];
        }

        /// <summary>
        /// Entries at index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="QuickSearchEntry"/>.
        /// </returns>
        public QuickSearchEntry EntryAtIndex(int index)
        {
            return this.Entries[index];
        }

        /// <summary>
        /// Entries at index information area identifier.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="QuickSearchEntry"/>.
        /// </returns>
        public QuickSearchEntry EntryAtIndexInfoAreaId(int index, string infoAreaId)
        {
            var entries = this.EntriesForInfoAreaId(infoAreaId);
            return entries?[index];
        }

        /// <summary>
        /// Informations the index of the area identifier at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string InfoAreaIdAtIndex(int index)
        {
            return (string)this.InfoAreaIds[index];
        }

        /// <summary>
        /// Numbers the of entries for information area identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int NumberOfEntriesForInfoAreaId(string infoAreaId)
        {
            var entries = this.EntryPerInfoAreaId[infoAreaId];
            return entries?.Count ?? 0;
        }
    }
}
