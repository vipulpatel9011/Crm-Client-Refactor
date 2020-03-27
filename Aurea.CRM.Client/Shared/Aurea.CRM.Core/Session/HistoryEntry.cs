// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistoryEntry.cs" company="Aurea Software Gmbh">
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
//   HistoryEntry
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The history view reference name
        /// </summary>
        public const string HistoryViewReferenceName = "HistoryListView";

        /// <summary>
        /// The default history menu name
        /// </summary>
        public const string DefaultHistoryMenuName = "HistoryView";

        /// <summary>
        /// The configuration parameter menu name
        /// </summary>
        public const string ConfigParamMenuName = "HistoryMenuName";
    }

    /// <summary>
    /// History Entry
    /// </summary>
    public class HistoryEntry
    {
        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; set; }

        /// <summary>
        /// Gets or sets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [onlin data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [onlin data]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlineData { get; set; }

        /// <summary>
        /// Gets or sets the last call.
        /// </summary>
        /// <value>
        /// The last call.
        /// </value>
        public DateTime LastCall { get; set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryEntry"/> class.
        /// </summary>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="onlineData">if set to <c>true</c> [online data].</param>
        /// <param name="imageName">Name of the image.</param>
        public HistoryEntry(string recordId, ViewReference viewReference, bool onlineData, string imageName)
        {
            this.RecordIdentification = recordId;
            this.ViewReference = viewReference;
            this.OnlineData = onlineData;
            this.LastCall = DateTime.UtcNow;
            this.ImageName = imageName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryEntry"/> class.
        /// </summary>
        /// <param name="serializedEntry">The serialized entry.</param>
        public HistoryEntry(HistoryEntrySerialized serializedEntry)
        {
            this.RecordIdentification = serializedEntry.RecordIdentification;
            this.LastCall = serializedEntry.LastCall;
            this.OnlineData = serializedEntry.OnlineData;
            this.ImageName = serializedEntry.ImageName;

            Dictionary<string, object> dictionary = serializedEntry.ViewReference.JsonDictionaryFromString();
            this.ViewReference = new ViewReference(dictionary, serializedEntry.ViewName);
        }

        /// <summary>
        /// Serializes this instance.
        /// </summary>
        /// <returns></returns>
        public HistoryEntrySerialized Serialized()
        {
            return new HistoryEntrySerialized
            {
                RecordIdentification = this.RecordIdentification,
                LastCall = this.LastCall,
                OnlineData = this.OnlineData,
                ImageName = this.ImageName,
                ViewName = this.ViewReference.ViewName,
                ViewReference = this.ViewReference.Serialized()
            };
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="theObject">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object theObject)
        {
            if (theObject is HistoryEntry)
            {
                HistoryEntry historyEntry = (HistoryEntry)theObject;
                return historyEntry.RecordIdentification == this.RecordIdentification;
            }

            return false;
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return this.RecordIdentification.GetHashCode();
        }
    }

    /// <summary>
    /// Class used to saved History Entry records in serialized form.
    /// </summary>
    public class HistoryEntrySerialized
    {
        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; set; }

        /// <summary>
        /// Gets or sets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public string ViewReference { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [onlin data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [onlin data]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlineData { get; set; }

        /// <summary>
        /// Gets or sets the last call.
        /// </summary>
        /// <value>
        /// The last call.
        /// </value>
        public DateTime LastCall { get; set; }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; set; }

        /// <summary>
        /// Gets or sets the name of the view.
        /// </summary>
        /// <value>
        /// The name of the view.
        /// </value>
        public string ViewName { get; set; }
    }
}
