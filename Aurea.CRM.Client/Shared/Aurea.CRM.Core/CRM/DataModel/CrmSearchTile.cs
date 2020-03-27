// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmSearchTile.cs" company="Aurea Software Gmbh">
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
//   The Search Tile class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Delegates;
    using OperationHandling;
    using Query;

    /// <summary>
    /// Search Tile class
    /// </summary>
    /// <seealso cref="UpcrmTile" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPCRMSearchTile : UPCRMTile, ISearchOperationHandler
    {
        /// <summary>
        /// The CRM query
        /// </summary>
        protected UPContainerMetaInfo crmQuery;

        /// <summary>
        /// The title text
        /// </summary>
        protected string titleText;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMSearchTile"/> class.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="menuAction">The menu action.</param>
        public UPCRMSearchTile(UPCRMTiles tiles, ViewReference viewReference, Dictionary<string, object> parameters, Menu menuAction)
            : base(tiles, viewReference, parameters, menuAction)
        {
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public override void Load()
        {
            string searchAndListName = this.ViewReference.ContextValueForKey("name");
            string recordId = this.ViewReference.ContextValueForKey("link");
            string filterName = this.ViewReference.ContextValueForKey("filter");
            string linkIdString = this.ViewReference.ContextValueForKey("linkId");
            string infoAreaId = this.ViewReference.ContextValueForKey("infoAreaId");
            string tableCaptionName = this.ViewReference.ContextValueForKey("tableCaption");
            this.crmQuery = null;
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.titleText = string.Empty;

            if (!string.IsNullOrEmpty(searchAndListName))
            {
                this.crmQuery = new UPContainerMetaInfo(searchAndListName, this.Parameters, new List<object> { "List" });
            }

            if (this.crmQuery == null && !string.IsNullOrEmpty(tableCaptionName))
            {
                UPConfigTableCaption tableCaption = configStore.TableCaptionByName(tableCaptionName);
                if (tableCaption != null)
                {
                    this.crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), tableCaption.InfoAreaId);
                    this.titleText = tableCaption.FormatString.Replace("{1}", string.Empty).Replace(Environment.NewLine, string.Empty);
                }
            }

            if (this.crmQuery == null && infoAreaId != null)
            {
                this.crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), infoAreaId);
            }

            if (filterName != null)
            {
                UPConfigFilter filter = ConfigurationUnitStore.DefaultStore.FilterByName(filterName);
                filter = filter?.FilterByApplyingValueDictionaryDefaults(this.Parameters, true);
                if (filter != null)
                {
                    if (this.crmQuery == null)
                    {
                        this.crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), filter.InfoAreaId);
                    }

                    this.crmQuery.ApplyFilter(filter);
                }
            }

            if (this.crmQuery == null)
            {
                this.Tiles.TileFinishedWithError(this, new Exception("No query"));
            }

            if (string.IsNullOrEmpty(this.titleText))
            {
                this.titleText = this.MenuAction.DisplayName;
            }

            if (!string.IsNullOrEmpty(recordId))
            {
                this.crmQuery?.SetLinkRecordIdentification(recordId, Convert.ToInt32(linkIdString));
            }

            this.crmQuery?.Find(this.RequestOption, this);
        }

        /// <summary>
        /// Loads the with result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool LoadWithResult(UPCRMResult result)
        {
            this.Value = result.RowCount.ToString();
            this.Text = this.titleText;
            this.Tiles.TileFinished(this);
            return true;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.Tiles.TileFinishedWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.LoadWithResult(result);
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
}
