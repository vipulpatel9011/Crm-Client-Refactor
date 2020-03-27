// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordTile.cs" company="Aurea Software Gmbh">
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
//   The Record Tile class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Delegates;
    using Extensions;
    using OperationHandling;
    using Query;

    /// <summary>
    /// The Record Tile class
    /// </summary>
    /// <seealso cref="UpcrmTile" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPCRMRecordTile : UPCRMTile, ISearchOperationHandler
    {
        private UPConfigTableCaption tableCaption;
        private UPConfigFilter imageFilter;
        private UPContainerMetaInfo crmQuery;
        private UPConfigCatalogAttributes catalogAttributes;
        private string recordIdentification;
        private string tableCaptionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordTile"/> class.
        /// </summary>
        /// <param name="tiles">The tiles.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="menuAction">The menu action.</param>
        /// <exception cref="Exception">
        /// RecordIdentification is null
        /// or
        /// TableCaptionName is null
        /// </exception>
        public UPCRMRecordTile(UPCRMTiles tiles, ViewReference viewReference, Dictionary<string, object> parameters, Menu menuAction)
            : base(tiles, viewReference, parameters, menuAction)
        {
            this.tableCaptionName = this.ViewReference.ContextValueForKey("tableCaption");
            this.recordIdentification = this.ViewReference.ContextValueForKey("uid");
            string imageMapFilterName = this.ViewReference.ContextValueForKey("imageMapFilter");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;

            if (string.IsNullOrEmpty(this.recordIdentification))
            {
                throw new Exception("RecordIdentification is null");
            }

            if (!string.IsNullOrEmpty(this.tableCaptionName))
            {
                this.tableCaption = configStore.TableCaptionByName(this.tableCaptionName);
            }

            if (this.tableCaption == null)
            {
                if (string.IsNullOrEmpty(this.tableCaptionName))
                {
                    throw new Exception("TableCaptionName is null");
                }
            }

            if (!string.IsNullOrEmpty(imageMapFilterName))
            {
                this.imageFilter = configStore.FilterByName(imageMapFilterName);
                this.imageFilter = this.imageFilter?.FilterByApplyingValueDictionaryDefaults(this.Parameters, true);

                if (this.imageFilter != null)
                {
                    this.catalogAttributes = configStore.CatalogAttributesByFilter(this.imageFilter);
                    if (this.catalogAttributes != null)
                    {
                        this.crmQuery?.AddCrmFields(new List<UPCRMField> { this.catalogAttributes.CrmField });
                    }
                }
            }
        }

        /// <summary>
        /// Loads the with result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public override bool LoadWithResult(UPCRMResult result)
        {
            if (result.RowCount > 0)
            {
                return this.LoadWithResultRow((UPCRMResultRow)result.ResultRowAtIndex(0));
            }

            return false;
        }

        /// <summary>
        /// Loads the with result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public override bool LoadWithResultRow(UPCRMResultRow resultRow)
        {
            if (this.tableCaption != null)
            {
                string tc = this.tableCaption.TableCaptionForResultRow(resultRow, false);
                var parts = tc.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    this.Value = parts[0];
                    this.Text = parts[1];
                }
                else
                {
                    this.Value = tc;
                    this.Text = this.tableCaption.Fields[0].Label;
                }
            }
            else
            {
                this.Text = this.tableCaption.Fields[0].Label;
            }

            if (this.catalogAttributes != null)
            {
                string rawValue = resultRow.RawValueForFieldIdInfoAreaIdLinkId(this.catalogAttributes.CrmField.FieldId, this.catalogAttributes.CrmField.InfoAreaId, -1);
                if (!string.IsNullOrEmpty(rawValue))
                {
                    this.ImageName = this.catalogAttributes.ImageNameForRawValue(rawValue);
                }
            }

            this.Tiles.TileFinished(this);
            return true;
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public override void Load()
        {
            if (this.tableCaption == null)
            {
                this.crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), this.recordIdentification.InfoAreaId());
            }
            else
            {
                this.crmQuery = this.tableCaption.CrmQuery;
            }

            if (this.crmQuery == null)
            {
                this.Tiles.TileFinishedWithError(this, new Exception($"Invalid configuration - cannot create query for tableCaption {this.tableCaptionName}"));
                return;
            }

            this.crmQuery.SetLinkRecordIdentification(this.recordIdentification);
            this.crmQuery.Find(this.RequestOption, this);
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
            if (result.RowCount == 0)
            {
                this.Tiles.TileFinishedWithError(this, new Exception($"Record Not found: {this.recordIdentification}"));
            }
            else
            {
                this.LoadWithResultRow((UPCRMResultRow)result.ResultRowAtIndex(0));
            }
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
