// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSerialEntryRequest.cs" company="Aurea Software Gmbh">
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
//   Serial Entry Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// UPSerialEntryRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    public class UPSerialEntryRequest : UPOfflineRequestDelegate
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context { get; private set; }

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSerialEntryRequest(object context, UPSerialEntry serialEntry)
        {
            this.Context = context;
            this.SerialEntry = serialEntry;
        }

        /// <summary>
        /// Finisheds this instance.
        /// </summary>
        protected void Finished()
        {
            this.SerialEntry.StartNextRequest();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void Start()
        {
            this.OfflineRequestDidFailWithError(null, null, null, new Exception("cannot start abstract serial entry request"));
        }

        /// <summary>
        /// Submits the change record request.
        /// </summary>
        /// <param name="changedRecords">The changed records.</param>
        protected void SubmitChangeRecordRequest(List<UPCRMRecord> changedRecords)
        {
            UPOfflineSerialEntryRequest offlineRequest = this.SerialEntry.OfflineRequest;
            offlineRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, changedRecords, this);
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public virtual void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.SerialEntry.HandleRowErrorContext(null, error, this.Context);
            this.Finished();
        }

        /// <summary>
        /// Offlines the request did finish multi request.
        /// </summary>
        /// <param name="request">The request.</param>
        public void OfflineRequestDidFinishMultiRequest(UPOfflineRequest request)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public virtual void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            this.Finished();
        }
    }

    /// <summary>
    /// UPSerialEntryRowRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryRequest" />
    public class UPSerialEntryRowRequest : UPSerialEntryRequest
    {
        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public UPSERow Row { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryRowRequest"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public UPSerialEntryRowRequest(UPSERow row, object context)
            : base(context, row.SerialEntry)
        {
            this.Row = row;
        }

        /// <summary>
        /// Offlines the request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="_context">The context.</param>
        /// <param name="error">The error.</param>
        public override void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object _context, Exception error)
        {
            this.Row.Error = error;
            this.SerialEntry.HandleRowErrorContext(this.Row, error, this.Context);
            this.Finished();
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            this.Row.Error = null;
            base.OfflineRequestDidFinishWithResult(request, data, online, context, result);
        }
    }

    /// <summary>
    /// UPSerialEntrySaveRowRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryRowRequest" />
    public class UPSerialEntrySaveRowRequest : UPSerialEntryRowRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntrySaveRowRequest"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public UPSerialEntrySaveRowRequest(UPSERow row, object context)
            : base(row, context)
        {
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            List<UPCRMRecord> changedRecords = this.SerialEntry.ChangedChildRecordsForRow(this.Row);
            if (changedRecords?.Count > 0)
            {
                this.SubmitChangeRecordRequest(changedRecords);
            }
            else
            {
                this.SerialEntry.HandleRowUnchangedContext(this.Row, this.Context);
                this.Finished();
            }
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="_context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object _context, Dictionary<string, object> result)
        {
            this.Row.Error = null;
            List<UPSERow> dependentPositions = this.Row.DependentRows(this.SerialEntry.Positions);
            if (dependentPositions != null)
            {
                //List<UPSERow> bundlePositions = new List<UPSERow>(dependentPositions);
                //bundlePositions.Add(this.Row);

                foreach (UPSERow row in dependentPositions)
                {
                    //row.RowPricing.UpdateCurrentConditionsWithPositions(bundlePositions);
                    //row.ClearDiscountInfo();
                    row.ComputeRowWithConditionsWithDependent(false);
                }
            }
            this.SerialEntry.HandleRowChangesResultContextSaveAll(new List<UPSERow> { this.Row }, data, this.Context, false);
            this.Finished();
        }
    }

    /// <summary>
    /// UPSerialEntryUploadPhotoRowRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryRowRequest" />
    public class UPSerialEntryUploadPhotoRowRequest : UPSerialEntryRowRequest
    {
        private UPOfflineUploadDocumentRequest uploadDocumentRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryUploadPhotoRowRequest"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public UPSerialEntryUploadPhotoRowRequest(UPSERow row, object context)
            : base(row, context)
        {
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            UPSERow row = this.Row;
            if (row.PhotoData != null)
            {
                string recordIdentification = row.SerialEntryRecordIdentification;
                if (!string.IsNullOrEmpty(recordIdentification))
                {
                    string fileName = row.FileNamePattern.StringByReplacingOccurrencesOfParameterWithIndexWithString(0, recordIdentification);
                    fileName = fileName.StringByReplacingOccurrencesOfParameterWithIndexWithString(1, DateTime.UtcNow.ToString("yyyyMMdd"));

                    this.uploadDocumentRequest = new UPOfflineUploadDocumentRequest(row.PhotoData, -1, fileName, "image/jpeg", recordIdentification, -1);
                    this.uploadDocumentRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, this);
                }
                else
                {
                    this.Finished();
                }
            }
            else
            {
                this.Finished();
            }
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="_context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object _context, Dictionary<string, object> result)
        {
            this.uploadDocumentRequest = null;
            this.Row.Error = null;
            this.SerialEntry.HandlePhotoUploadedContext(this.Row, this.Context);
            this.Finished();
        }

        /// <summary>
        /// Offlines the request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="_context">The context.</param>
        /// <param name="error">The error.</param>
        public override void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object _context, Exception error)
        {
            this.uploadDocumentRequest = null;
            base.OfflineRequestDidFailWithError(request, data, _context, error);
        }
    }

    /// <summary>
    /// UPSerialEntryDeleteRowRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryRowRequest" />
    public class UPSerialEntryDeleteRowRequest : UPSerialEntryRowRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntryDeleteRowRequest"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="context">The context.</param>
        public UPSerialEntryDeleteRowRequest(UPSERow row, object context)
            : base(row, context)
        {
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            List<UPCRMRecord> changedRecords = this.SerialEntry.ChangedChildRecordsForDeleteRow(this.Row);
            if (changedRecords?.Count > 0)
            {
                this.SubmitChangeRecordRequest(changedRecords);
            }
            else
            {
                this.SerialEntry.HandleRowDeleteUnchangedContext(this.Row, this.Context);
                this.Finished();
            }
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="_context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object _context, Dictionary<string, object> result)
        {
            this.Row.Error = null;
            this.SerialEntry.HandleRowDeleteResultContext(this.Row, data, this.Context);
            this.Finished();
        }
    }

    /// <summary>
    /// UPSerialEntrySaveAllRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryRequest" />
    public class UPSerialEntrySaveAllRequest : UPSerialEntryRequest
    {
        private List<UPSERow> changedRows;
        private bool ignoreChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntrySaveAllRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ignoreChanges">if set to <c>true</c> [ignore changes].</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSerialEntrySaveAllRequest(object context, bool ignoreChanges, UPSerialEntry serialEntry)
            : base(context, serialEntry)
        {
            this.ignoreChanges = ignoreChanges;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntrySaveAllRequest"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSerialEntrySaveAllRequest(object context, UPSerialEntry serialEntry)
            : base(context, serialEntry)
        {
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            List<UPSERow> changedRowArray = new List<UPSERow>();
            List<UPCRMRecord> changedRecords = this.ignoreChanges
                ? this.SerialEntry.ChangedRecordsForEndingSerialEntry()
                : this.SerialEntry.ChangedChildRecordsWithChangedRows(changedRowArray);

            this.changedRows = changedRowArray;
            if (changedRecords?.Count > 0)
            {
                this.SubmitChangeRecordRequest(changedRecords);
            }
            else
            {
                this.SerialEntry.UnblockOfflineRequestWithUpSync(true);
                this.SerialEntry.HandleRowUnchangedContext(null, this.Context);
                this.SerialEntry.StartNextRequest();
            }
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="_context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object _context, Dictionary<string, object> result)
        {
            foreach (UPSERow row in this.changedRows)
            {
                row.Error = null;
            }

            this.SerialEntry.Error = null;
            this.SerialEntry.SaveAllExecuted = true;
            this.SerialEntry.UnblockOfflineRequestWithUpSync(online ? online : ServerSession.CurrentSession.ConnectedToServer);

            this.SerialEntry.HandleRowChangesResultContextSaveAll(this.changedRows, data, this.Context, true);
            this.SerialEntry.StartNextRequest();
        }

        /// <summary>
        /// Offlines the request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="_context">The context.</param>
        /// <param name="error">The error.</param>
        public override void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object _context, Exception error)
        {
            this.SerialEntry.Error = error;
            if (this.changedRows.Count == 1)
            {
                UPSERow row = this.changedRows[0];
                row.Error = error;
            }
            else if (this.changedRows.Count > 0)
            {
                Exception rowError = new Exception("SerialEntrySave", error);
                foreach (UPSERow row in this.changedRows)
                {
                    if (row.Error == null)
                    {
                        row.Error = rowError;
                    }
                }
            }

            this.SerialEntry.UnblockOfflineRequestWithUpSync(!error.IsConnectionOfflineError());
            this.SerialEntry.HandleRowErrorContext(null, error, _context);
            this.Finished();
        }
    }

    /// <summary>
    /// UPSerialEntrySaveRowsRequest
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntryRequest" />
    public class UPSerialEntrySaveRowsRequest : UPSerialEntryRequest
    {
        private List<UPSERow> rows;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntrySaveRowsRequest"/> class.
        /// </summary>
        /// <param name="_rows">The rows.</param>
        /// <param name="context">The context.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSerialEntrySaveRowsRequest(List<UPSERow> _rows, object context, UPSerialEntry serialEntry)
        : base(context, serialEntry)
        {
            this.rows = _rows;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            List<UPCRMRecord> changedRecords = null;
            foreach (UPSERow row in this.rows)
            {
                List<UPCRMRecord> changedRecordsForRow = this.SerialEntry.ChangedChildRecordsForRow(row);
                if (changedRecordsForRow.Count > 0)
                {
                    if (changedRecords == null)
                    {
                        changedRecords = new List<UPCRMRecord>(changedRecordsForRow);
                    }
                    else
                    {
                        changedRecords.AddRange(changedRecordsForRow);
                    }
                }
            }

            if (changedRecords?.Count > 0)
            {
                this.SubmitChangeRecordRequest(changedRecords);
            }
            else
            {
                this.SerialEntry.HandleRowUnchangedContext(null, this.Context);
                this.Finished();
            }
        }

        /// <summary>
        /// Offlines the request did finish with result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="_context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object _context, Dictionary<string, object> result)
        {
            List<object> contextArray = (List<object>)this.Context;

            int rowIndex = 0;
            foreach (UPSERow row in this.rows)
            {
                object context = null;
                if (contextArray.Count > rowIndex)
                {
                    context = contextArray[rowIndex];
                }

                ++rowIndex;
                this.SerialEntry.HandleRowChangesResultContextSaveAll(new List<UPSERow> { row }, null, context, false);
            }

            this.Finished();
        }
    }
}
