// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPObjectivesGroup.cs" company="Aurea Software Gmbh">
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
//   UPObjectivesGroup
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Objectives
{
    using System.Collections.Generic;

    /// <summary>
    /// UPObjectivesGroup
    /// </summary>
    public class UPObjectivesGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPObjectivesGroup"/> class.
        /// </summary>
        /// <param name="groupKey">The group key.</param>
        /// <param name="objectives">The objectives.</param>
        /// <param name="configuration">The configuration.</param>
        public UPObjectivesGroup(string groupKey, UPObjectives objectives, UPObjectivesConfiguration configuration)
        {
            this.GroupKey = groupKey;
            this.Objectives = objectives;
            this.Configuration = configuration;
            this.Items = new List<UPObjectivesItem>();
            this.ItemDictionary = new Dictionary<string, UPObjectivesItem>();
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<UPObjectivesItem> Items { get; }

        /// <summary>
        /// Gets the item dictionary.
        /// </summary>
        /// <value>
        /// The item dictionary.
        /// </value>
        public Dictionary<string, UPObjectivesItem> ItemDictionary { get; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label => this.Configuration.SectionHeaderLabel;

        /// <summary>
        /// Gets the group key.
        /// </summary>
        /// <value>
        /// The group key.
        /// </value>
        public string GroupKey { get; private set; }

        /// <summary>
        /// Gets the objectives.
        /// </summary>
        /// <value>
        /// The objectives.
        /// </value>
        public UPObjectives Objectives { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public UPObjectivesConfiguration Configuration { get; private set; }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(UPObjectivesItem item)
        {
            this.Items.Add(item);
            this.ItemDictionary[item.KeyValue] = item;
        }
    }
}
