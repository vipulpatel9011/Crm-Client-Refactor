// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEListingLoadRequest.cs" company="Aurea Software Gmbh">
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
//   UPSEListingLoadRequest
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// UPSEListingLoadRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPSEListingLoadRequest : ISearchOperationHandler
    {
        private UPContainerMetaInfo crmQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEListingLoadRequest"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="context">The context.</param>
        public UPSEListingLoadRequest(UPSEListingOwner owner, object context)
        {
            this.Owner = owner;
            this.Context = context;
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public UPSEListingOwner Owner { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context { get; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            Dictionary<string, object> serialEntryDict =
                this.Owner.ListingController.SerialEntry.InitialFieldValuesForDestination;
            Dictionary<string, object> dict = serialEntryDict.Count > 0
                ? new Dictionary<string, object>(serialEntryDict)
                : new Dictionary<string, object>();

            if (this.Owner.ValueDictionary.Count > 0)
            {
                foreach (var entry in this.Owner.ValueDictionary)
                {
                    dict[entry.Key] = entry.Value;
                }
            }

            this.crmQuery = new UPContainerMetaInfo(this.Owner.ListingController.ListingSearch, dict);
            this.crmQuery.SetLinkRecordIdentification(this.Owner.RecordIdentification);
            this.crmQuery.Find(this.Owner.ListingController.SerialEntry.RequestOption, this, false);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.Owner.ListingController.HandleListingLoadForOwnerContextDidFinishWithError(this.Owner, this.Context, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.Owner.ListingsFromResult(result);
            this.Owner.ListingController.HandleListingLoadedForOwnerContext(this.Owner, this.Context);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// DictionaryExtensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Valueses from result row.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static Dictionary<string, object> ValuesFromResultRow(this Dictionary<string, UPConfigFieldControlField> self, UPCRMResultRow row)
        {
            if (row == null)
            {
                return null;
            }

            Dictionary<string, object> valueDictionary = new Dictionary<string, object>();
            foreach (string key in self.Keys)
            {
                UPConfigFieldControlField field = self[key];
                valueDictionary[key] = row.RawValueAtIndex(field.TabIndependentFieldIndex);
            }

            return valueDictionary;
        }

        /// <summary>
        /// Displays the values from result row.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public static Dictionary<string, string> DisplayValuesFromResultRow(this Dictionary<string, UPConfigFieldControlField> self, UPCRMResultRow row)
        {
            if (row == null)
            {
                return null;
            }

            Dictionary<string, string> valueDictionary = new Dictionary<string, string>();
            foreach (string key in self.Keys)
            {
                UPConfigFieldControlField field = self[key];
                valueDictionary[key] = row.ValueAtIndex(field.TabIndependentFieldIndex);
            }

            return valueDictionary;
        }
    }
}
