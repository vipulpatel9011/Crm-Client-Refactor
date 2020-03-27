// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLinkReader.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The CRM link reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Delegates;
    using Extensions;
    using OperationHandling;
    using Query;

    /// <summary>
    /// The upcrm link reader.
    /// </summary>
    public class UPCRMLinkReader : ISearchOperationHandler
    {
        /// <summary>
        /// The crm query.
        /// </summary>
        protected UPContainerMetaInfo crmQuery;

        /// <summary>
        /// The link contexts.
        /// </summary>
        protected List<UPCRMLinkReaderLinkContext> linkContexts;

        /// <summary>
        /// The query with link record.
        /// </summary>
        protected bool queryWithLinkRecord;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkReader"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="parentLinkString">
        /// The parent link string.
        /// </param>
        /// <param name="requestOption">
        /// The request option.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPCRMLinkReader(
            string recordIdentification,
            string parentLinkString,
            UPRequestOption requestOption,
            UPCRMLinkReaderDelegate theDelegate)
        {
            if (recordIdentification == null)
            {
                return;
            }

            this.SourceRecordIdentification = recordIdentification;
            this.InfoAreaId = this.SourceRecordIdentification.InfoAreaId();
            this.ParentLinkString = parentLinkString;
            if (this.ParentLinkString == "KPFI")
            {
                this.ParentLinkString = "KP;FI";
            }

            this.RequestOption = requestOption;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLinkReader"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="parentLinkString">
        /// The parent link string.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPCRMLinkReader(
            string recordIdentification,
            string parentLinkString,
            UPCRMLinkReaderDelegate theDelegate)
            : this(recordIdentification, parentLinkString, UPRequestOption.BestAvailable, theDelegate)
        {
        }

        /// <summary>
        /// Gets the destination position.
        /// </summary>
        public int DestinationPosition { get; private set; }

        /// <summary>
        /// Gets the destination record identification.
        /// </summary>
        public string DestinationRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the info area id.
        /// </summary>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the parent link string.
        /// </summary>
        public string ParentLinkString { get; private set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets the source record identification.
        /// </summary>
        public string SourceRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the the delegate.
        /// </summary>
        public UPCRMLinkReaderDelegate TheDelegate { get; private set; }

        /// <summary>
        /// The client query.
        /// </summary>
        /// <returns>
        /// The <see cref="UPContainerMetaInfo"/>.
        /// </returns>
        public UPContainerMetaInfo ClientQuery()
        {
            List<UPCRMField> fieldArray = this.FieldArray();
            List<UPCRMLinkField> linkfieldArray = this.LinkFieldArrayWithStartIndex(fieldArray?.Count ?? 0);
            if (linkfieldArray != null)
            {
                if (fieldArray == null)
                {
                    fieldArray = linkfieldArray.Cast<UPCRMField>().ToList();
                }
                else
                {
                    List<UPCRMField> fa = new List<UPCRMField>(fieldArray);
                    fa.AddRange(linkfieldArray);
                    fieldArray = fa;
                }
            }

            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(fieldArray, this.InfoAreaId);
            crmQuery.SetLinkRecordIdentification(this.SourceRecordIdentification, -1);
            return crmQuery;
        }

        /// <summary>
        /// The field array.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMField> FieldArray()
        {
            List<UPCRMField> fieldArray = null;
            int fieldCount = 0;
            foreach (UPCRMLinkReaderLinkContext context in this.linkContexts)
            {
                if (context.FieldLinkFields?.Count > 0)
                {
                    context.StartingPosition = fieldCount;
                    if (fieldArray == null)
                    {
                        fieldArray = new List<UPCRMField>();
                    }

                    fieldArray.AddRange(context.FieldLinkFields);
                    fieldCount += context.FieldLinkFields.Count;
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// The handle result client.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        public void HandleResultClient(UPCRMResult result, bool client)
        {
            if (this.queryWithLinkRecord)
            {
                if (result.RowCount > 0)
                {
                    UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(0);
                    this.DestinationRecordIdentification = resultRow.RootRecordIdentification;
                }
            }
            else
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
                if (client)
                {
                    this.DestinationRecordIdentification = this.RecordIdentificationFromResultRow(row);
                }
                else
                {
                    int position = 0;
                    Dictionary<string, int> linkPositions = new Dictionary<string, int>();
                    foreach (UPCRMLinkReaderLinkContext context in this.linkContexts)
                    {
                        string linkKey = $"{context.LinkInfo.TargetInfoAreaId}_{(context.LinkInfo.LinkId < 0 ? 0 : context.LinkInfo.LinkId)}";
                        if (linkPositions.ValueOrDefault(linkKey) == 0)
                        {
                            continue;
                        }

                        this.DestinationRecordIdentification = row.RecordIdentificationAtIndex(++position);
                        if (!string.IsNullOrEmpty(this.DestinationRecordIdentification))
                        {
                            break;
                        }

                        linkPositions[linkKey] = position - 1;
                    }
                }
            }

            this.TheDelegate?.LinkReaderDidFinishWithResult(this, this.DestinationRecordIdentification);
        }

        /// <summary>
        /// The link field array with start index.
        /// </summary>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMLinkField> LinkFieldArrayWithStartIndex(int startIndex)
        {
            List<UPCRMLinkField> fieldArray = null;
            foreach (UPCRMLinkReaderLinkContext context in this.linkContexts)
            {
                if (context.LinkField != null)
                {
                    context.StartingPosition = startIndex++;
                    if (fieldArray == null)
                    {
                        fieldArray = new List<UPCRMLinkField>();
                    }

                    fieldArray.Add(context.LinkField);
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// The link info from parent link string.
        /// </summary>
        /// <param name="singleParentLink">
        /// The single parent link.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMLinkInfo"/>.
        /// </returns>
        public UPCRMLinkInfo LinkInfoFromParentLinkString(string singleParentLink)
        {
            UPCRMTableInfo tableInfo = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(this.InfoAreaId);
            string[] stringParts = singleParentLink.Split(':');

            return stringParts.Length > 1
                       ? tableInfo.LinkInfoForTargetInfoAreaIdLinkId(stringParts[0], stringParts[1].ToInt())
                       : tableInfo.LinkInfoForTargetInfoAreaIdLinkId(singleParentLink, -1);
        }

        /// <summary>
        /// The link reader contexts for parent link string.
        /// </summary>
        /// <param name="parentLinkString">
        /// The parent link string.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UPCRMLinkReaderLinkContext> LinkReaderContextsForParentLinkString(string parentLinkString)
        {
            List<UPCRMLinkReaderLinkContext> linkReaderContext = new List<UPCRMLinkReaderLinkContext>();

            if (parentLinkString != null)
            {
                string[] infoAreaIds = parentLinkString.Split(';');
                foreach (string linkString in infoAreaIds)
                {
                    UPCRMLinkInfo linkInfo = this.LinkInfoFromParentLinkString(linkString);
                    if (linkInfo != null)
                    {
                        UPCRMLinkReaderLinkContext context = new UPCRMLinkReaderLinkContext(linkInfo, this);
                        linkReaderContext.Add(context);
                    }
                }
            }

            return linkReaderContext.Count > 0 ? linkReaderContext : null;
        }

        /// <summary>
        /// The query for link link record.
        /// </summary>
        /// <param name="linkInfo">
        /// The link info.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="UPContainerMetaInfo"/>.
        /// </returns>
        public UPContainerMetaInfo QueryForLinkLinkRecord(UPCRMLinkInfo linkInfo, string recordIdentification)
        {
            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(null, linkInfo.TargetInfoAreaId);
            crmQuery.SetLinkRecordIdentification(recordIdentification, linkInfo.LinkId);
            return crmQuery;
        }

        /// <summary>
        /// The record identification from result row.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RecordIdentificationFromResultRow(UPCRMResultRow row)
        {
            foreach (UPCRMLinkReaderLinkContext context in this.linkContexts)
            {
                if (context.LinkInfo.LinkId <= 0 && context.LinkInfo.TargetInfoAreaId.Equals(this.InfoAreaId))
                {
                    return this.SourceRecordIdentification;
                }

                if (context.LinkField != null)
                {
                    string val = row.RawValueAtIndex(context.StartingPosition);
                    if (!string.IsNullOrEmpty(val))
                    {
                        return context.LinkInfo.TargetInfoAreaId.InfoAreaIdRecordId(val);
                    }
                }
                else if (context.FieldLinkFields.Count == 2 && context.LinkInfo.LinkFieldArray.Count == context.FieldLinkFields.Count)
                {
                    string statNoValue = row.RawValueAtIndex(context.StartingPosition);
                    string lNrValue = row.RawValueAtIndex(context.StartingPosition + 1);
                    if (!string.IsNullOrEmpty(lNrValue) && lNrValue != "0")
                    {
                        return StringExtensions.InfoAreaIdRecordId(
                            context.LinkInfo.TargetInfoAreaId,
                            StringExtensions.RecordIdFromStatNoStringRecordNoString(statNoValue, lNrValue));
                    }
                }
                else
                {
                    string linkRecord = context.ReadLinkRecordIdentificationFromRow(row);
                    if (!string.IsNullOrEmpty(linkRecord))
                    {
                        return linkRecord;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The request link record offline.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string RequestLinkRecordOffline()
        {
            this.linkContexts = this.LinkReaderContextsForParentLinkString(this.ParentLinkString);
            if (this.linkContexts == null || this.linkContexts.Count == 0)
            {
                return null;
            }

            if (this.linkContexts.Count == 1)
            {
                UPCRMLinkReaderLinkContext context = this.linkContexts[0];
                if (context.LinkInfo.IsGeneric)
                {
                    UPContainerMetaInfo crmQuery = this.QueryForLinkLinkRecord(context.LinkInfo, this.SourceRecordIdentification);
                    UPCRMResult result = crmQuery.Find();
                    if (result.RowCount > 0)
                    {
                        UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(0);
                        return row.RootRecordIdentification;
                    }

                    return null;
                }
            }

            UPContainerMetaInfo crmQuery2 = this.ClientQuery();
            UPCRMResult result2 = crmQuery2.Find();
            if (result2.RowCount == 0)
            {
                return null;
            }

            UPCRMResultRow row2 = (UPCRMResultRow)result2.ResultRowAtIndex(0);
            return this.RecordIdentificationFromResultRow(row2);
        }

        /// <summary>
        /// The search operation did fail with error.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (error.IsConnectionOfflineError() && this.RequestOption == UPRequestOption.BestAvailable)
            {
                if (!this.queryWithLinkRecord)
                {
                    this.crmQuery = this.ClientQuery();
                }

                UPCRMResult result = this.crmQuery.Find();
                if (result.RowCount > 0)
                {
                    this.HandleResultClient(result, true);
                    this.crmQuery = null;
                    return;
                }
            }

            this.crmQuery = null;
            this.TheDelegate?.LinkReaderDidFinishWithResult(this, error);
        }

        /// <summary>
        /// The search operation did finish with count.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The search operation did finish with counts.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="counts">
        /// The counts.
        /// </param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The search operation did finish with result.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount > 0 || this.RequestOption != UPRequestOption.BestAvailable)
            {
                this.HandleResultClient(result, false);
                this.crmQuery = null;
                return;
            }

            if (!this.queryWithLinkRecord)
            {
                this.crmQuery = this.ClientQuery();
            }

            result = this.crmQuery.Find();
            this.HandleResultClient(result, true);
        }

        /// <summary>
        /// The search operation did finish with results.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="results">
        /// The results.
        /// </param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The server query.
        /// </summary>
        /// <returns>
        /// The <see cref="UPContainerMetaInfo"/>.
        /// </returns>
        public UPContainerMetaInfo ServerQuery()
        {
            List<UPCRMField> serverLinkFields = new List<UPCRMField>();
            foreach (UPCRMLinkReaderLinkContext context in this.linkContexts)
            {
                serverLinkFields.Add(
                    UPCRMField.FieldWithFieldIdInfoAreaIdLinkId(
                        0,
                        context.LinkInfo.TargetInfoAreaId,
                        context.LinkInfo.LinkId));
            }

            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(serverLinkFields, this.InfoAreaId);
            crmQuery.SetLinkRecordIdentification(this.SourceRecordIdentification, -1);
            return crmQuery;
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Start()
        {
            this.linkContexts = this.LinkReaderContextsForParentLinkString(this.ParentLinkString);
            this.queryWithLinkRecord = false;
            if (this.linkContexts.Count == 1)
            {
                UPCRMLinkReaderLinkContext context = this.linkContexts[0];
                if (context.LinkInfo.IsGeneric)
                {
                    this.crmQuery = this.QueryForLinkLinkRecord(context.LinkInfo, this.SourceRecordIdentification);
                    this.queryWithLinkRecord = true;
                }
            }

            if (!this.queryWithLinkRecord)
            {
                foreach (UPCRMLinkReaderLinkContext context in this.linkContexts)
                {
                    if (context.LinkInfo.LinkId <= 0 && context.LinkInfo.TargetInfoAreaId.Equals(this.InfoAreaId))
                    {
                        this.DestinationRecordIdentification = this.SourceRecordIdentification;
                        this.TheDelegate?.LinkReaderDidFinishWithResult(this, this.SourceRecordIdentification);

                        return true;
                    }
                }

                this.crmQuery = this.ClientQuery();
            }

            if (this.RequestOption == UPRequestOption.FastestAvailable || this.RequestOption == UPRequestOption.Offline)
            {
                UPCRMResult result = this.crmQuery.Find();
                if (result.RowCount > 0)
                {
                    UPContainerMetaInfo crmQuery = this.crmQuery;
                    this.JustCall(crmQuery);
                    this.crmQuery = null;
                    this.HandleResultClient(result, true);
                    return true;
                }
            }

            if (this.RequestOption != UPRequestOption.Offline)
            {
                if (!this.queryWithLinkRecord)
                {
                    this.crmQuery = this.ServerQuery();
                }

                this.crmQuery.Find(this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The just call.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        void JustCall(UPContainerMetaInfo query)
        {
        }
    }
}
