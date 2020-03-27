// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineUploadDocumentRequest.cs" company="Aurea Software Gmbh">
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
//   The Offline Upload Document Request class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Sync;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Platform;
    using Aurea.CRM.Core.ResourceHandling;
    using Aurea.CRM.Core.Session;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// The Offline Upload Document Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRecordRequest" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Data.IUploadDocumentRequestDelegate" />
    public class UPOfflineUploadDocumentRequest : UPOfflineRecordRequest, IUploadDocumentRequestDelegate
    {
        /// <summary>
        /// The offline records
        /// </summary>
        private List<UPCRMRecord> offlineRecords;

        /// <summary>
        /// Gets the type of the MIME.
        /// </summary>
        /// <value>
        /// The type of the MIME.
        /// </value>
        public string MimeType { get; private set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Gets the local file URL.
        /// </summary>
        /// <value>
        /// The local file URL.
        /// </value>
        public string LocalFileUrl { get; private set; }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets the no d3 link.
        /// </summary>
        /// <value>
        /// The no d3 link.
        /// </value>
        public string NoD3Link { get; private set; }

        /// <summary>
        /// Gets the upload server operation.
        /// </summary>
        /// <value>
        /// The upload server operation.
        /// </value>
        public UploadDocumentServerOperation UploadServerOperation { get; private set; }

        /// <summary>
        /// Gets the offline request delegate.
        /// </summary>
        /// <value>
        /// The offline request delegate.
        /// </value>
        public UPOfflineRequestDelegate OfflineRequestDelegate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineUploadDocumentRequest"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="noD3Link">The no d3 link.</param>
        public UPOfflineUploadDocumentRequest(byte[] data, int requestNr, string fileName, string mimeType, string recordIdentification, int fieldId, string noD3Link = null)
        {
            this.Data = data;
            this.RequestNr = requestNr;
            this.FileName = fileName.ValidFileName();
            this.MimeType = mimeType;
            this.RecordIdentification = recordIdentification;
            this.FieldId = fieldId;
            this.NoD3Link = noD3Link;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineUploadDocumentRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineUploadDocumentRequest(int requestNr)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public override OfflineRequestType RequestType => OfflineRequestType.DocumentUpload;

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.DocumentUpload;

        /// <summary>
        /// Gets a value indicating whether [needs wlan for synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [needs wlan for synchronize]; otherwise, <c>false</c>.
        /// </value>
        public override bool NeedsWLANForSync
        {
            get
            {
                bool disable84817 = this.Configuration.ConfigValueIsSet("Disable.84817");
                if (disable84817)
                {
                    return true;
                }

                if (ServerSession.CurrentSession.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWWAN)
                {
                    string maxUploadFileSize = this.Configuration.ConfigValueDefaultValue("Sync.DocumentUploadMaxSizeForWan", null);
                    int configMaxSize = Convert.ToInt32(maxUploadFileSize) * 1024;
                    if (configMaxSize < 0 || this.Data.Length > configMaxSize)
                    {
                        return true;
                    }

                    string uploadMimeTypeString = this.Configuration.ConfigValueDefaultValue("Sync.DocumentUploadMimeTypesForWan", null);
                    Logger.LogDebug($"uploadMimeTypeString: {uploadMimeTypeString}", LogFlag.LogUpSync);

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public override string DefaultTitleLine => "DocumentUpload";

        /// <summary>
        /// Offlines the records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> OfflineRecords()
        {
            if (this.offlineRecords != null)
            {
                return this.offlineRecords;
            }

            UPCRMOfflineDocumentRecordCreator offlineRecordCreator = UPCRMOfflineDocumentRecordCreator.Create(this.NoD3Link);
            if (offlineRecordCreator == null)
            {
                return null;
            }

            Dictionary<string, object> parameterDictionary = offlineRecordCreator.ParametersForDocument(this.FileName, this.MimeType,
                StringExtensions.CrmValueFromDate(DateTime.UtcNow), this.Data.Length);
            UPCRMRecord rootRecord = new UPCRMRecord(this.RecordIdentification);
            this.offlineRecords = offlineRecordCreator.CrmRecordsForParametersRecordAddRoot(parameterDictionary, rootRecord, this.FieldId >= 0);
            return this.offlineRecords;
        }

        /// <summary>
        /// Creates the offline records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> CreateOfflineRecords()
        {
            this.offlineRecords = null;
            List<UPCRMRecord> records = this.OfflineRecords();
            this.ReserveRequestNr();
            int recordNr = 1;
            foreach (UPCRMRecord record in records)
            {
                record.CreateRecordIdentificationFromRequestNrRecordNr(this.ReservedRequestNr, recordNr++);
            }

            return this.offlineRecords;
        }

        /// <summary>
        /// Stores the request with offline records.
        /// </summary>
        /// <param name="_offlineRecords">The offline records.</param>
        /// <param name="rootRecord">The root record.</param>
        /// <returns>0, if successfull</returns>
        private int StoreRequestWithOfflineRecords(List<UPCRMRecord> _offlineRecords, UPCRMRecord rootRecord)
        {
            IOfflineStorage storage = this.Storage;
            storage.Database.BeginTransaction();

            if (this.RequestNr == -1)
            {
                if (!this.CreateRequest(storage.Database))
                {
                    storage.Database.Rollback();
                    return 1;
                }
            }

            int ret = storage.SaveDocumentUpload(this.Data, this.RequestNr, this.FileName, this.MimeType, this.RecordIdentification, this.FieldId, storage.Database);
            if (ret == 0 && _offlineRecords != null)
            {
                int nextRecordNr = 1;
                bool documentKeyFieldFilled = this.FieldId < 0;
                foreach (UPCRMRecord record in _offlineRecords)
                {
                    ret = storage.SaveRecord(record, this.RequestNr, nextRecordNr++, storage.Database);
                    if (ret != 0)
                    {
                        break;
                    }

                    if (rootRecord != null && !documentKeyFieldFilled && record.InfoAreaId == "D1")
                    {
                        documentKeyFieldFilled = true;
                        string documentKey = $"new:{record.RecordIdentification}";
                        rootRecord.AddValue(new UPCRMFieldValue(documentKey, rootRecord.InfoAreaId, this.FieldId, true));
                    }
                }
            }

            if (ret == 0)
            {
                storage.Database.Commit();
            }
            else
            {
                storage.Database.Rollback();
            }

            return ret;
        }

        /// <summary>
        /// Stores the request.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success</returns>
        public override int StoreRequest(IDatabase database)
        {
            List<UPCRMRecord> _offlineRecords = this.OfflineRecords();
            UPCRMRecord d3Record = null;
            UPCRMRecord d1Record = null;
            UPCRMRecord rootRecord = null;
            if (_offlineRecords.Count > 0)
            {
                foreach (UPCRMRecord rec in _offlineRecords)
                {
                    if (rec.InfoAreaId == "D3")
                    {
                        d3Record = rec;
                    }
                    else if (rec.InfoAreaId == "D1")
                    {
                        d1Record = rec;
                    }
                    else
                    {
                        rootRecord = rec;
                    }
                }
            }

            int ret = this.StoreRequestWithOfflineRecords(_offlineRecords, rootRecord);
            if (ret == 0)
            {
                this.RecordsToCacheWithRollback(_offlineRecords, UPCRMDataStore.DefaultStore, true);
                string documentRecordIdentification = d1Record.RecordIdentification != null ? d1Record.RecordIdentification : d3Record.RecordIdentification;
                if (documentRecordIdentification != null)
                {
                    ResourceManager resourceManager = SmartbookResourceManager.DefaultResourceManager;
                    Uri url = ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(documentRecordIdentification, this.FileName);
                    string localURL = resourceManager.LocalPathForResourceAtUrl(url, resourceManager.LocalPathForTransientResources, this.FileName);
                    SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.SaveFile(localURL, this.Data);
                }
            }

            return ret;
        }

        /// <summary>
        /// Starts the request the delegate.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="_theDelegate">The delegate.</param>
        /// <returns>True, if success</returns>
        public override bool StartRequest(UPOfflineRequestMode requestMode, UPOfflineRequestDelegate _theDelegate)
        {
            this.OfflineRequestDelegate = _theDelegate;
            string maxUploadFileSize = this.Configuration.ConfigValueDefaultValue("Sync.DocumentUploadMaxSize", null);
            bool canUploadDocument = true;
            if (!string.IsNullOrEmpty(maxUploadFileSize))
            {
                int configMaxSize = Convert.ToInt32(maxUploadFileSize) * 1024;
                if (configMaxSize < 0 || this.Data.Length > configMaxSize)
                {
                    canUploadDocument = false;
                }
            }

            if (canUploadDocument)
            {
                IDatabase database = this.Storage.Database;
                if (this.RequestMode == UPOfflineRequestMode.Offline)
                {
                    this.StoreRequest(database);
                    this.OfflineRequestDelegate?.OfflineRequestDidFinishWithResult(this, null, false, null, null);
                    return true;
                }

                if (ServerSession.CurrentSession.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWiFi
                    || (!this.NeedsWLANForSync && ServerSession.CurrentSession.ConnectionWatchDog.LastServerReachabilityStatus == ReachabilityStatus.ReachableViaWWAN))
                {
                    if (this.StoreBeforeRequest)
                    {
                        this.StoreRequest(database);
                    }

                    UploadDocument document = new UploadDataDocument(this.Data, this.FileName, this.MimeType);
                    this.UploadServerOperation = new UploadDocumentServerOperation(document, this.RecordIdentification, this.FieldId, this.NoD3Link, this);
                    ServerSession.CurrentSession.ExecuteRequest(this.UploadServerOperation);
                    return true;
                }

                if (requestMode == UPOfflineRequestMode.OnlineOnly)
                {
                    return false;
                }

                this.StoreRequest(database);
                this.OfflineRequestDelegate?.OfflineRequestDidFinishWithResult(this, null, false, null, null);
                return true;
            }

            Exception error = new Exception("Document can not be uploaded as the size exceeds the maximum limit");
            this.OfflineRequestDelegate?.OfflineRequestDidFailWithError(this, null, null, error);
            return false;
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            this.UploadServerOperation.Cancel();
            this.ReportSuccessfulSync();
        }

        /// <summary>
        /// Starts the synchronize always perform.
        /// </summary>
        /// <param name="_delegate">The delegate.</param>
        /// <param name="alwaysPerform">if set to <c>true</c> [always perform].</param>
        /// <returns>True, if success</returns>
        public override bool StartSync(UPOfflineRequestDelegate _delegate, bool alwaysPerform)
        {
            if (this.RequestNr < 0 || (!this.ApplicationRequest && !this.Loaded && !this.LoadFromOfflineStorage()))
            {
                return false;
            }

            this.OfflineRequestDelegate = _delegate;
            UploadDocument document = new UploadDataDocument(this.Data, this.FileName, this.MimeType);
            this.UploadServerOperation = new UploadDocumentServerOperation(document, this.RecordIdentification, this.FieldId, this);
            this.UploadServerOperation.AlwaysPerform = alwaysPerform;
            ServerSession.CurrentSession.ExecuteRequest(this.UploadServerOperation);
            return true;
        }

        /// <summary>
        /// Deletes the request children.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>0, if success</returns>
        protected override int DeleteRequestChildren(IDatabase database)
        {
            int ret = base.DeleteRequestChildren(database);
            if (ret == 0)
            {
                ret = database.Execute("DELETE FROM documentuploads WHERE requestnr = ?", this.RequestNr);
                if (this.LocalFileUrl != null)
                {
                    Logger.LogDebug($"Sync request {this.RequestNr}: document {this.LocalFileUrl} deleted", LogFlag.LogUpSync);
                    Exception error;
                    SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.TryDelete(this.LocalFileUrl, out error);
                }
            }

            return ret;
        }

        /// <summary>
        /// Loads from database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>
        /// True if success, else false
        /// </returns>
        public override bool LoadFromDatabase(IDatabase database)
        {
            if (!base.LoadFromDatabase(database))
            {
                return false;
            }

            int ret = 0;
            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            if (recordSet.Query.Prepare("SELECT data, filename, mimetype, infoareaid, recordid, fieldid FROM documentuploads WHERE requestnr = ?"))
            {
                recordSet.Query.Bind(1, this.RequestNr);
                ret = recordSet.Execute();
                if (ret == 0)
                {
                    int count = recordSet.GetRowCount();
                    if (count == 1)
                    {
                        DatabaseRow row = recordSet.GetRow(0);
                        string databaseValue = row.GetColumn(0);
                        if (databaseValue.StartsWith("file://"))
                        {
                            string lastPathComponent = databaseValue.StartsWith("file://localhost") ? Path.GetFileName(databaseValue) : databaseValue.Substring(7);

                            this.LocalFileUrl = this.Storage.GetOfflineStoragePath(lastPathComponent);

                            Task<byte[]> t = SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.FileContents(this.LocalFileUrl);
                            this.Data = t.Result;

                            if (SimpleIoc.Default.GetInstance<ILogSettings>().LogUpSync)
                            {
                                SimpleIoc.Default.GetInstance<ILogger>().LogDebug($"Sync request {this.RequestNr}: document {this.LocalFileUrl} loaded ({this.Data.Length} bytes)", LogFlag.LogUpSync);
                            }
                        }
                        else
                        {
                            this.LocalFileUrl = null;
                            this.Data = Convert.FromBase64String(databaseValue);
                        }

                        this.FileName = row.GetColumn(1);
                        this.MimeType = row.GetColumn(2);
                        string infoAreaId = row.GetColumn(3);
                        string recordId = row.GetColumn(4);
                        this.RecordIdentification = StringExtensions.InfoAreaIdRecordId(infoAreaId, recordId);
                        this.FieldId = row.GetColumnInt(5);
                    }
                }
            }

            return ret == 0 && this.Data != null;
        }

        /// <summary>
        /// Called when [pre start multi request].
        /// </summary>
        public override void OnPreStartMultiRequest()
        {
            if (this.Configuration.ConfigValueIsSet("Disable.86045"))
            {
                base.OnPreStartMultiRequest();
                return;
            }

            if (this.StoreBeforeRequest)
            {
                this.ApplicationRequest = true;
            }
        }

        /// <summary>
        /// Uploads the document request did finish with json.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="json">The json.</param>
        public void UploadDocumentRequestDidFinishWithJSON(UploadDocumentServerOperation request, Dictionary<string, object> json)
        {
            List<object> jsonArray = json.Values.ToList();
            string d1 = jsonArray[0] as string;
            List<object> syncInfos = jsonArray[2] as List<object>;
            UPCRMRecordSync.SyncRecordSetDefinitions(syncInfos);
            ResourceManager resourceManager = SmartbookResourceManager.DefaultResourceManager;
            Uri url = ServerSession.CurrentSession.DocumentRequestUrlForRecordIdentification(d1, request.FileName);
            string localUrl = resourceManager.LocalPathForResourceAtUrl(url, resourceManager.LocalPathForTransientResources, request.FileName);
            SimpleIoc.Default.GetInstance<IPlatformService>().StorageProvider.SaveFile(localUrl, request.Data);
            this.ReportSuccessfulSync();
            this.OfflineRequestDelegate?.OfflineRequestDidFinishWithResult(this, d1, true, null, json);
        }

        /// <summary>
        /// Uploads the document request did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        public void UploadDocumentRequestDidFailWithError(UploadDocumentServerOperation request, Exception error)
        {
            if ((this.ApplicationRequest || this.RequestNr < 0) && this.RequestMode != UPOfflineRequestMode.OnlineOnly && error.IsConnectionOfflineError())
            {
                if (this.RequestNr < 0)
                {
                    this.StoreRequest(this.Storage.Database);
                }

                if (!this.Configuration.ConfigValueIsSet("Disable.77642"))
                {
                    this.Storage.BlockOnlineRecordRequest = true;
                }

                this.OfflineRequestDelegate?.OfflineRequestDidFinishWithResult(this, null, false, null, null);
                return;
            }

            if (!error.IsConnectionOfflineError() && string.IsNullOrEmpty(this.Error) && this.RequestNr >= 0)
            {
                this.ReportSyncError(error);
                this.UndoRecordsInCache(this.Records, UPCRMDataStore.DefaultStore);
            }

            this.OfflineRequestDelegate?.OfflineRequestDidFailWithError(this, null, null, error);
        }
    }
}
