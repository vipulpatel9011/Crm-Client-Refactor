// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCharacteristicsGroup.cs" company="Aurea Software Gmbh">
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
//   UPCharacteristicsGroup
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.UIModel.Characteristics;

    /// <summary>
    /// UPCharacteristicsGroup
    /// </summary>
    public class UPCharacteristicsGroup
    {
        private SortedSet<UPCharacteristicsItem> itemArray;
        private Dictionary<string, UPCharacteristicsItem> itemDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristicsGroup"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="catalogValue">The catalog value.</param>
        /// <param name="singleSelection">if set to <c>true</c> [single selection].</param>
        /// <param name="characteristics">The characteristics.</param>
        /// <param name="showExpanded">if set to <c>true</c> [show expanded].</param>
        public UPCharacteristicsGroup(string label, string catalogValue, bool singleSelection, UPCharacteristics characteristics, bool showExpanded)
        {
            this.Label = label;
            this.CatalogValue = catalogValue;
            this.Characteristics = characteristics;
            this.SingleSelection = singleSelection;
            this.itemDict = new Dictionary<string, UPCharacteristicsItem>();
            this.ShowExpanded = showExpanded;
            this.ShowAdditionalFields = false;

            this.itemArray = new SortedSet<UPCharacteristicsItem>(new CharacteristicsItemComparer());
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public SortedSet<UPCharacteristicsItem> Items => this.itemArray;

        /// <summary>
        /// Gets the item dictionary.
        /// </summary>
        /// <value>
        /// The item dictionary.
        /// </value>
        public Dictionary<string, UPCharacteristicsItem> ItemDictionary => this.itemDict;

        /// <summary>
        /// Gets the catalog value.
        /// </summary>
        /// <value>
        /// The catalog value.
        /// </value>
        public string CatalogValue { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the characteristics.
        /// </summary>
        /// <value>
        /// The characteristics.
        /// </value>
        public UPCharacteristics Characteristics { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [single selection].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [single selection]; otherwise, <c>false</c>.
        /// </value>
        public bool SingleSelection { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show expanded].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show expanded]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowExpanded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show additional fields].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show additional fields]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAdditionalFields { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has items.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has items; otherwise, <c>false</c>.
        /// </value>
        public bool HasItems => this.itemArray.Count > 0;

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool AddItem(UPCharacteristicsItem item)
        {
            if (this.itemDict.ContainsKey(item.CatalogValue))
            {
                return false;
            }

            this.itemArray.Add(item);
            this.itemDict[item.CatalogValue] = item;
            return true;
        }

        /// <summary>
        /// Removes the empty items.
        /// </summary>
        public void RemoveEmptyItems()
        {
            List<UPCharacteristicsItem> emptyItems = this.itemArray.Where(item => item.Record == null).ToList();

            foreach (UPCharacteristicsItem item in emptyItems)
            {
                this.itemArray.Remove(item);
                this.itemDict.Remove(item.CatalogValue);
            }
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords()
        {
            List<UPCRMRecord> array = null;
            foreach (UPCharacteristicsItem item in this.itemArray)
            {
                List<UPCRMRecord> changedItemRecords = item.ChangedRecords();
                if (changedItemRecords != null)
                {
                    if (array == null)
                    {
                        array = new List<UPCRMRecord>(changedItemRecords);
                    }
                    else
                    {
                        array.AddRange(changedItemRecords);
                    }
                }
            }

            return array;
        }
    }
}
