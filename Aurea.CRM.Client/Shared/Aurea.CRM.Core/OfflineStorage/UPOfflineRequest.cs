// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineRequest.cs" company="Aurea Software Gmbh">
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
//   The Offline Request class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Aurea.CRM.Core.Common;

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.DAL;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.Utilities;
    //using Microsoft.Practices.ServiceLocation;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The offline request blocked text
        /// </summary>
        public const string OFFLINEREQUEST_BLOCKED_TEXT = "blocked";

        /// <summary>
        /// The Crmpad client version
        /// </summary>
        public const string CRMPAD_CLIENTVERSION = "2.6.0";
    }

    /// <summary>
    /// Offline Request Mode
    /// </summary>
    public enum UPOfflineRequestMode
    {
        /// <summary>
        /// Online confirm
        /// </summary>
        OnlineConfirm = 0,

        /// <summary>
        /// Online no confirm
        /// </summary>
        OnlineNoConfirm = 1,

        /// <summary>
        /// Online only
        /// </summary>
        OnlineOnly = 2,

        /// <summary>
        /// Offline
        /// </summary>
        Offline = 3
    }

    /// <summary>
    /// Offline Request Base class
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializable" />
    public class UPOfflineRequest : UPSerializable
    {
        private string titleLine;
        private string detailsLine;
        private string imageName;
        private bool blockExecution;

        /// <summary>
        /// The request mode
        /// </summary>
        protected UPOfflineRequestMode requestMode;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public virtual OfflineRequestType RequestType => OfflineRequestType.Unknown;

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public virtual OfflineRequestProcess ProcessType => OfflineRequestProcess.Unknown;

        /// <summary>
        /// Gets a value indicating whether [needs wlan for synchronize].
        /// </summary>
        /// <value>
        /// <c>true</c> if [needs wlan for synchronize]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool NeedsWLANForSync => false;

        /// <summary>
        /// Gets a value indicating whether this instance has XML.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has XML; otherwise, <c>false</c>.
        /// </value>
        public virtual bool HasXml => false;

        /// <summary>
        /// Gets the dependent requests.
        /// </summary>
        /// <value>
        /// The dependent requests.
        /// </value>
        public List<UPOfflineRequest> DependentRequests
        {
            get
            {
                var requests = new List<UPOfflineRequest>();
                var offlineStorage = this.Storage;

                var dependentRequestNumbers = this.DependentRequestNumbers();
                if (dependentRequestNumbers == null)
                {
                    return requests;
                }

                foreach (var requestNumber in dependentRequestNumbers)
                {
                    UPOfflineRequest request = offlineStorage.RequestWithNr(requestNumber);
                    if (request != null)
                    {
                        request.LoadFromOfflineStorage();
                        requests.Add(request);
                    }
                }

                return requests;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [fixable by user].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixable by user]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool FixableByUser => false;

        /// <summary>
        /// Gets the identifying record identification.
        /// </summary>
        /// <value>
        /// The identifying record identification.
        /// </value>
        public virtual string IdentifyingRecordIdentification => null;

        /// <summary>
        /// Gets a value indicating whether [store before request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [store before request]; otherwise, <c>false</c>.
        /// </value>
        public bool StoreBeforeRequest => this.Storage.StoreBeforeRequest && !this.ConflictHandlingMode;

        /// <summary>
        /// Gets the request nr.
        /// </summary>
        /// <value>
        /// The request nr.
        /// </value>
        public int RequestNr { get; protected set; }

        /// <summary>
        /// Gets the json.
        /// </summary>
        /// <value>
        /// The json.
        /// </value>
        public string Json { get; private set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the error stack.
        /// </summary>
        /// <value>
        /// The error stack.
        /// </value>
        public string ErrorStack { get; private set; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public int Code { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPOfflineRequest"/> is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loaded; otherwise, <c>false</c>.
        /// </value>
        public bool Loaded { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPOfflineRequest"/> is synced.
        /// </summary>
        /// <value>
        ///   <c>true</c> if synced; otherwise, <c>false</c>.
        /// </value>
        public bool Synced { get; private set; }

        /// <summary>
        /// Gets the request mode.
        /// </summary>
        /// <value>
        /// The request mode.
        /// </value>
        public UPOfflineRequestMode RequestMode => this.requestMode;

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public string Response { get; private set; }

        /// <summary>
        /// Gets or sets the title line.
        /// </summary>
        /// <value>
        /// The title line.
        /// </value>
        public string TitleLine
        {
            get
            {
                return this.titleLine ?? this.DefaultTitleLine;
            }

            set
            {
                if (this.RequestNr < 0)
                {
                    this.titleLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the details line.
        /// </summary>
        /// <value>
        /// The details line.
        /// </value>
        public string DetailsLine
        {
            get
            {
                return this.detailsLine ?? this.DefaultDetailsLine;
            }

            set
            {
                if (this.RequestNr < 0)
                {
                    this.detailsLine = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName
        {
            get
            {
                return this.imageName ?? this.DefaultImageName;
            }

            set
            {
                if (this.RequestNr < 0)
                {
                    this.imageName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [block execution].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [block execution]; otherwise, <c>false</c>.
        /// </value>
        public bool BlockExecution
        {
            get
            {
                return this.blockExecution;
            }

            set
            {
                if (this.RequestNr > 0)
                {
                    IOfflineStorage storage = this.Storage;
                    IDatabase database = storage.Database;
                    DatabaseStatement statement = new DatabaseStatement(database);
                    if (statement.Prepare("UPDATE requests SET error = ?"))
                    {
                        storage.ClearCachedRequestNumbers();
                        if (value)
                        {
                            statement.Bind(0, Constants.OFFLINEREQUEST_BLOCKED_TEXT);
                        }
                        else
                        {
                            statement.Bind(0);
                        }

                        int ret = statement.Execute();
                        if (ret == 0)
                        {
                            this.blockExecution = value;
                        }
                    }
                }
                else
                {
                    this.blockExecution = value;
                }
            }
        }

        /// <summary>
        /// Gets the server request number.
        /// </summary>
        /// <value>
        /// The server request number.
        /// </value>
        public int ServerRequestNumber { get; private set; }

        /// <summary>
        /// Gets the server date time.
        /// </summary>
        /// <value>
        /// The server date time.
        /// </value>
        public string ServerDateTime { get; private set; }

        /// <summary>
        /// Gets the server session identifier.
        /// </summary>
        /// <value>
        /// The server session identifier.
        /// </value>
        public string ServerSessionId { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [application request].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [application request]; otherwise, <c>false</c>.
        /// </value>
        public bool ApplicationRequest { get; set; }

        /// <summary>
        /// Gets or sets the error translation key.
        /// </summary>
        /// <value>
        /// The error translation key.
        /// </value>
        public string ErrorTranslationKey { get; set; }

        /// <summary>
        /// Gets or sets the related information dictionary.
        /// </summary>
        /// <value>
        /// The related information dictionary.
        /// </value>
        public Dictionary<string, object> RelatedInfoDictionary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [conflict handling mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [conflict handling mode]; otherwise, <c>false</c>.
        /// </value>
        public bool ConflictHandlingMode { get; set; }

        /// <summary>
        /// Gets the base error code.
        /// </summary>
        /// <value>
        /// The base error code.
        /// </value>
        public int BaseErrorCode { get; private set; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public string ApplicationVersion { get; private set; }

        /// <summary>
        /// Gets the reserved request nr.
        /// </summary>
        /// <value>
        /// The reserved request nr.
        /// </value>
        public int ReservedRequestNr { get; private set; }

        /// <summary>
        /// Gets the default name of the image.
        /// </summary>
        /// <value>
        /// The default name of the image.
        /// </value>
        public virtual string DefaultImageName => string.Empty;

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public virtual string DefaultTitleLine => string.Empty;

        /// <summary>
        /// Gets the default details line.
        /// </summary>
        /// <value>
        /// The default details line.
        /// </value>
        public virtual string DefaultDetailsLine => string.Empty;

        /// <summary>
        /// Gets the IOfflineStorage that this request should reference
        /// </summary>
        public IOfflineStorage Storage => this.Session.OfflineStorage;

        /// <summary>
        /// Gets the <see cref="IConfigurationUnitStore"/> that this request should reference
        /// </summary>
        public IConfigurationUnitStore Configuration => this.Session.ConfigUnitStore;

        /// <summary>
        /// Gets the <see cref="ICRMDataStore"/> that this request should reference
        /// </summary>
        public ICRMDataStore DataStore => this.Session.CrmDataStore;

        /// <summary>
        /// Gets the <see cref="UPCRMTimeZone"/> that this request should reference
        /// </summary>
        public UPCRMTimeZone TimeZone => this.Session?.TimeZone;

        /// <summary>
        /// Gets the <see cref="IServerSession"/> that this request should reference
        /// </summary>
        public IServerSession Session { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request number</param>
        public UPOfflineRequest(int requestNr)
            : this()
        {
            this.RequestNr = requestNr;
            this.ServerRequestNumber = -1;
            if (requestNr >= 0)
            {
                this.requestMode = UPOfflineRequestMode.OnlineOnly;
            }
            else
            {
                this.requestMode = UPOfflineRequestMode.OnlineConfirm;
                this.ApplicationRequest = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRequest"/> class.
        /// </summary>
        /// <param name="_requestMode">The request mode.</param>
        /// <param name="jsonString">The json string.</param>
        public UPOfflineRequest(UPOfflineRequestMode _requestMode, string jsonString)
        {
            this.RequestNr = -1;
            this.requestMode = _requestMode;
            this.Json = jsonString;
            this.ServerRequestNumber = -1;
            this.ApplicationRequest = true;
            this.Session = ServerSession.CurrentSession;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRequest"/> class.
        /// </summary>
        /// <param name="_requestMode">The request mode.</param>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineRequest(UPOfflineRequestMode _requestMode, ViewReference viewReference)
            : this(_requestMode, viewReference.Serialized())
        {
            string errorTranslation = viewReference.ContextValueForKey("ErrorTranslation");
            if (!string.IsNullOrEmpty(errorTranslation))
            {
                this.ErrorTranslationKey = errorTranslation;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRequest"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineRequest(ViewReference viewReference)
            : this(UPOfflineRequestMode.OnlineConfirm, viewReference)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineRequest"/> class.
        /// </summary>
        public UPOfflineRequest()
         : this(UPOfflineRequestMode.OnlineConfirm, (string)null)
        {
        }

        /// <summary>
        /// Creates Requests for the given processtype.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        /// <param name="type">The type.</param>
        /// <param name="processtype">The processtype.</param>
        /// <returns></returns>
        public static UPOfflineRequest RequestWithIdTypeProcesstype(int requestNr, OfflineRequestType type, OfflineRequestProcess processtype)
        {
            if (type == OfflineRequestType.Records)
            {
                switch (processtype)
                {
                    case OfflineRequestProcess.EditRecord:
                        return new UPOfflineEditRecordRequest(requestNr);

                    case OfflineRequestProcess.SerialEntryOrder:
                        return new UPOfflineSerialEntryOrderRequest(requestNr);

                    case OfflineRequestProcess.SerialEntryPOS:
                        return new UPOfflineSerialEntryPOSRequest(requestNr);

                    case OfflineRequestProcess.SerialEntry:
                        return new UPOfflineSerialEntryRequest(requestNr);

                    case OfflineRequestProcess.Characteristics:
                        return new UPOfflineCharacteristicsRequest(requestNr);

                    case OfflineRequestProcess.Objectives:
                        return new UPOfflineObjectivesRequest(requestNr);

                    case OfflineRequestProcess.Questionnaire:
                        return new UPOfflineQuestionnaireRequest(requestNr);

                    case OfflineRequestProcess.CopyRecords:
                        return new UPOfflineCopyRecordsRequest(requestNr);

                    case OfflineRequestProcess.DeleteRecord:
                        return new UPOfflineOrganizerDeleteRecordRequest(requestNr);

                    case OfflineRequestProcess.ModifyRecord:
                        return new UPOfflineOrganizerModifyRecordRequest(requestNr);

                    default:
                        return new UPOfflineEditRecordRequest(requestNr);
                }
            }

            if (type == OfflineRequestType.DocumentUpload)
            {
                return new UPOfflineUploadDocumentRequest(requestNr);
            }

            if (type == OfflineRequestType.Settings)
            {
                return new UPOfflineSettingsRequest(requestNr);
            }

            return new UPOfflineGenericRequest(requestNr, type, processtype);
        }

        /// <summary>
        /// Loads from database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>True if success, else false</returns>
        public virtual bool LoadFromDatabase(IDatabase database)
        {
            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            string sql = @"SELECT json, error, errorcode, response, titleLine, detailsLine, imageName, serverRequestNumber, servertime, 
                           sessionid, translationkey, relatedInfo, baseerror, appversion, errorstack FROM requests WHERE requestnr = ?";

            recordSet.Query.Prepare(sql);
            recordSet.Query.Bind(1, this.RequestNr);
            int ret = recordSet.Execute();
            if (ret == 0 && recordSet.GetRowCount() > 0)
            {
                this.Loaded = true;
                DatabaseRow row = recordSet.GetRow(0);
                this.Json = row.GetColumn(0);
                this.blockExecution = false;

                if (!row.IsNull(1))
                {
                    string cError = row.GetColumn(1);
                    if (cError == Constants.OFFLINEREQUEST_BLOCKED_TEXT)
                    {
                        this.blockExecution = true;
                    }

                    this.Error = row.GetColumn(1);
                }

                this.Code = row.GetColumnInt(2, -1);
                if (!row.IsNull(3))
                {
                    this.Response = row.GetColumn(3);
                }

                if (!row.IsNull(4))
                {
                    this.titleLine = row.GetColumn(4);
                }

                if (!row.IsNull(5))
                {
                    this.detailsLine = row.GetColumn(5);
                }

                if (!row.IsNull(6))
                {
                    this.imageName = row.GetColumn(6);
                }

                if (!row.IsNull(7))
                {
                    this.ServerRequestNumber = row.GetColumnInt(7);
                }

                if (!row.IsNull(8))
                {
                    this.ServerDateTime = row.GetColumn(8);
                }

                if (!row.IsNull(9))
                {
                    this.ServerSessionId = row.GetColumn(9);
                }

                if (!row.IsNull(10))
                {
                    this.ErrorTranslationKey = row.GetColumn(10);
                }

                if (!row.IsNull(11))
                {
                    string _relatedInfo = row.GetColumn(11);
                    this.RelatedInfoDictionary = _relatedInfo.JsonDictionaryFromString();
                }

                if (!row.IsNull(12))
                {
                    this.BaseErrorCode = row.GetColumnInt(12);
                }

                if (!row.IsNull(13))
                {
                    this.ApplicationVersion = row.GetColumn(13);
                }

                if (!row.IsNull(14))
                {
                    this.ErrorStack = row.GetColumn(14);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads from offline storage.
        /// </summary>
        /// <returns>True if success, else false</returns>
        public bool LoadFromOfflineStorage()
        {
            var database = this.Storage?.Database;
            if (database == null)
            {
                return false;
            }

            database.BeginTransaction();
            bool ok = this.LoadFromDatabase(database);
            database.Commit();
            return ok;
        }

        /// <summary>
        /// Dependents the on request numbers.
        /// </summary>
        /// <returns></returns>
        public virtual List<object> DependentOnRequestNumbers()
        {
            return null;
        }

        /// <summary>
        /// Dependents the request numbers.
        /// </summary>
        /// <returns></returns>
        public virtual List<int> DependentRequestNumbers()
        {
            return null;
        }

        /// <summary>
        /// Dependents the requests deep.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, UPOfflineRequest> DependentRequestsDeep()
        {
            List<int> dependentRequestNumbers = this.DependentRequestNumbers();
            if (dependentRequestNumbers == null || dependentRequestNumbers.Count == 0)
            {
                return null;
            }

            Dictionary<int, UPOfflineRequest> dependentRequestNumberDictionary = new Dictionary<int, UPOfflineRequest>();
            IOfflineStorage offlineStorage = this.Storage;
            foreach (int cur in dependentRequestNumbers)
            {
                if (!dependentRequestNumberDictionary.ContainsKey(cur))
                {
                    UPOfflineRequest childRequest = offlineStorage.RequestWithNr(cur);
                    if (childRequest != null)
                    {
                        dependentRequestNumberDictionary.SetObjectForKey(childRequest, cur);
                        Dictionary<int, UPOfflineRequest> subdep = childRequest.DependentRequestsDeep();
                        foreach (int sub in subdep.Keys)
                        {
                            if (!dependentRequestNumberDictionary.ContainsKey(sub))
                            {
                                dependentRequestNumberDictionary.SetObjectForKey(subdep[sub], sub);
                            }
                        }
                    }
                }
            }

            return dependentRequestNumberDictionary;
        }

        /// <summary>
        /// Updates the child information.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        bool UpdateChildInformation(IDatabase database)
        {
            return this.DeleteRequestChildren(database) != 0 && this.StoreRequest(database) == 0;
        }

        /// <summary>
        /// Serializes the details.
        /// </summary>
        /// <param name="writer">The writer.</param>
        void SerializeDetails(UPSerializer writer)
        {
        }

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Serialize(UPSerializer writer)
        {
            if (!this.Loaded)
            {
                this.LoadFromOfflineStorage();
            }

            writer.WriteElementStart("Request");
            writer.WriteAttributeValue("clientVersion", Constants.CRMPAD_CLIENTVERSION);
            writer.WriteAttributeIntegerValue("id", this.RequestNr);
            writer.WriteAttributeValue("type", this.ProcessType.ToString());
            writer.WriteAttributeIntegerValue("mode", (int)this.RequestMode);
            writer.WriteAttributeIntegerValue("serverRequestNumber", this.ServerRequestNumber);
            writer.WriteElementValue("title", this.TitleLine);

            if (!string.IsNullOrEmpty(this.Error))
            {
                writer.WriteElementValue("ServerError", this.Error);
            }

            if (!string.IsNullOrEmpty(this.ServerDateTime))
            {
                writer.WriteElementValue("dateTime", this.ServerDateTime);
            }

            if (!string.IsNullOrEmpty(this.ServerSessionId))
            {
                writer.WriteElementValue("sessionId", this.ServerSessionId);
            }

            if (!string.IsNullOrEmpty(this.Response))
            {
                writer.WriteElementValue("ServerResponse", this.Response);
            }

            if (this.BaseErrorCode != 0)
            {
                writer.WriteElementValue("BaseErrorCode", this.BaseErrorCode.ToString());
            }

            if (!string.IsNullOrEmpty(this.ApplicationVersion))
            {
                writer.WriteElementValue("Version", this.ApplicationVersion);
            }

            string username = ServerSession.CurrentSession.UserName;
            if (!string.IsNullOrEmpty(username))
            {
                writer.WriteElementValue("username", username);
            }

            this.SerializeDetails(writer);
            if (this.RelatedInfoDictionary?.Count > 0)
            {
                writer.WriteElementStart("relatedInfo");
                foreach (string key in this.RelatedInfoDictionary.Keys)
                {
                    object v = this.RelatedInfoDictionary[key];
                    writer.WriteElementValue(key, v.ToString());
                }

                writer.WriteElementEnd();
            }

            var dependentRequests = this.DependentRequests;
            if (dependentRequests.Count > 0)
            {
                writer.WriteElementStart("DependentRequests");
                foreach (UPOfflineRequest dependentRequest in dependentRequests)
                {
                    dependentRequest.Serialize(writer);
                }

                writer.WriteElementEnd();
            }

            writer.WriteElementEnd();
        }

        /// <summary>
        /// XMLs this instance.
        /// </summary>
        /// <returns></returns>
        public string Xml()
        {
            UPXmlMemoryWriter xmlWriter = new UPXmlMemoryWriter();
            this.Serialize(xmlWriter);
            return xmlWriter.XmlContentString();
        }

        /// <summary>
        /// Saves to offline storage.
        /// </summary>
        /// <returns></returns>
        public bool SaveToOfflineStorage()
        {
            return this.SaveToOfflineStorage(null, -1);
        }

        /// <summary>
        /// Saves to offline storage.
        /// </summary>
        /// <param name="followUpRootRequestId">The follow up root request identifier.</param>
        /// <returns></returns>
        public int SaveToOfflineStorage(int followUpRootRequestId)
        {
            if (this.SaveToOfflineStorage(null, followUpRootRequestId))
            {
                return this.RequestNr;
            }

            return -1;
        }

        /// <summary>
        /// Saves to offline storage as follow up root.
        /// </summary>
        /// <returns></returns>
        public int SaveToOfflineStorageAsFollowUpRoot()
        {
            if (this.SaveToOfflineStorage(null, -1))
            {
                return this.RequestNr;
            }

            return -1;
        }

        /// <summary>
        /// Stores the request.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public virtual int StoreRequest(IDatabase database)
        {
            return 0;
        }

        /// <summary>
        /// Saves to offline storage.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="followUpRequestId">The follow up request identifier.</param>
        /// <returns></returns>
        bool SaveToOfflineStorage(string type, int followUpRequestId)
        {
            bool result;
            IDatabase database = this.Storage.Database;
            database.BeginTransaction();
            if (this.RequestNr < 0)
            {
                this.RequestNr = this.SaveRequest(database, type, followUpRequestId);
                if (this.RequestNr >= 0)
                {
                    result = this.StoreRequest(database) == 0;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = this.UpdateChildInformation(database);
            }

            database.Commit();
            return result;
        }

        /// <summary>
        /// Starts the synchronize.
        /// </summary>
        /// <param name="_delegate">The delegate.</param>
        /// <returns></returns>
        public bool StartSync(UPOfflineRequestDelegate _delegate)
        {
            return this.StartSync(_delegate, true);
        }

        /// <summary>
        /// Starts the synchronize.
        /// </summary>
        /// <param name="_delegate">The delegate.</param>
        /// <param name="alwaysPerform">if set to <c>true</c> [always perform].</param>
        /// <returns></returns>
        public virtual bool StartSync(UPOfflineRequestDelegate _delegate, bool alwaysPerform)
        {
            return false;
        }

        /// <summary>
        /// Starts the request.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public virtual bool StartRequest(UPOfflineRequestMode requestMode, UPOfflineRequestDelegate theDelegate)
        {
            return false;
        }

        /// <summary>
        /// Deletes the request children.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        protected virtual int DeleteRequestChildren(IDatabase database)
        {
            return 0;
        }

        /// <summary>
        /// Determines whether this instance can synchronize.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can synchronize; otherwise, <c>false</c>.
        /// </returns>
        public bool CanSync()
        {
            return string.IsNullOrEmpty(this.Error) && !(this.DependentOnRequestNumbers()?.Count > 0);
        }

        /// <summary>
        /// Offlines the records for request.
        /// </summary>
        /// <returns></returns>
        public List<string> OfflineRecordsForRequest()
        {
            List<string> deleteRecordArray = null;
            IDatabase database = this.Storage.Database;
            database.BeginTransaction();
            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);

            if (recordSet.Query.Prepare("SELECT infoAreaid, recordid FROM records WHERE requestnr = ? AND mode = 'NewOffline'"))
            {
                recordSet.Query.Bind(1, (int)this.RequestNr);
                int ret = recordSet.Execute();
                if (ret == 0)
                {
                    int count = recordSet.GetRowCount();
                    if (count > 0)
                    {
                        deleteRecordArray = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            DatabaseRow row = recordSet.GetRow(i);
                            deleteRecordArray.Add($"{row.GetColumn(0)}.{row.GetColumn(1)}");
                        }
                    }
                }
            }

            database.Commit();
            return deleteRecordArray;
        }

        /// <summary>
        /// Deletes the request.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        int DeleteRequest(IDatabase database)
        {
            DatabaseStatement statement = new DatabaseStatement(database);
            int ret = 0;
            if (statement.Prepare("DELETE FROM requests WHERE requestnr = ?"))
            {
                this.Storage.ClearCachedRequestNumbers();
                statement.Bind(0, this.RequestNr);
                ret = statement.Execute();
            }

            if (ret == 0)
            {
                ret = this.DeleteRequestChildren(database);
            }

            return ret;
        }

        /// <summary>
        /// Deletes the request.
        /// </summary>
        /// <param name="withDependentRequests">if set to <c>true</c> [with dependent requests].</param>
        /// <returns></returns>
        public int DeleteRequest(bool withDependentRequests)
        {
            int ret = 0;
            IDatabase database = this.Storage.Database;
            database.BeginTransaction();

            if (withDependentRequests)
            {
                Dictionary<int, UPOfflineRequest> dependentRequests = this.DependentRequestsDeep();
                if (dependentRequests != null)
                {
                    foreach (UPOfflineRequest subRequest in dependentRequests.Values)
                    {
                        ret = subRequest.DeleteRequest(database);
                        if (ret != 0)
                        {
                            break;
                        }
                    }
                }
            }

            if (ret == 0)
            {
                ret = this.DeleteRequest(database);
            }

            if (ret == 0)
            {
                database.Commit();
            }
            else
            {
                database.Rollback();
            }

            return ret;
        }

        /// <summary>
        /// Stores the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public int StoreError(Exception error)
        {
            var storage = UPOfflineStorage.DefaultStorage;
            var database = storage.Database;
            database.BeginTransaction();
            this.ServerRequestNumber = -1;
            DatabaseStatement statement = new DatabaseStatement(database);
            statement.Prepare(
                "UPDATE requests SET error = ?, errorstack = ? WHERE requestnr = ?");
            storage.ClearCachedRequestNumbers();
            string errorText = error.Message;
            string errorStack = error.StackTrace;
            if (error is ServerException)
            {
                errorStack = (error as ServerException).ServerStackTrace;
            }

            statement.Bind(1, errorText);
            statement.Bind(2, errorStack);
            statement.Bind(3, this.RequestNr);
            int ret = statement.Execute();
            database.Commit();
            return ret;
        }

        /// <summary>
        /// Clears the server request number.
        /// </summary>
        public void ClearServerRequestNumber()
        {
            this.ServerRequestNumber = -1;
        }

        /// <summary>
        /// Attaches the next server request number.
        /// </summary>
        public void AttachNextServerRequestNumber()
        {
            IDatabase database = this.Storage.Database;
            database.BeginTransaction();

            DatabaseRecordSet recordSet = new DatabaseRecordSet(database);
            if (recordSet.Query.Prepare("SELECT nextRequestNumber FROM requestcontrol"))
            {
                if (recordSet.Execute() == 0)
                {
                    if (recordSet.GetRowCount() == 1)
                    {
                        DatabaseRow row = recordSet.GetRow(0);
                        this.ServerRequestNumber = row.GetColumnInt(0);
                        DatabaseStatement updateStatement = new DatabaseStatement(database);
                        if (updateStatement.Prepare("UPDATE requestcontrol SET nextRequestNumber = ?"))
                        {
                            updateStatement.Bind(1, this.ServerRequestNumber + 1);
                            if (updateStatement.Execute() != 0)
                            {
                                this.ServerRequestNumber = -1;
                            }
                        }

                        if (this.RequestNr > 0)
                        {
                            updateStatement = new DatabaseStatement(database);
                            if (updateStatement.Prepare("UPDATE requests SET serverRequestNumber = ? WHERE requestnr = ?"))
                            {
                                updateStatement.Bind(1, this.ServerRequestNumber);
                                updateStatement.Bind(2, this.RequestNr);
                                updateStatement.Execute();
                            }
                        }
                    }
                    else
                    {
                        this.ServerRequestNumber = -1;
                    }
                }
            }

            database.Commit();
        }

        /// <summary>
        /// Reports the successful synchronize.
        /// </summary>
        /// <returns></returns>
        public virtual int ReportSuccessfulSync()
        {
            if (this.RequestNr >= 0)
            {
                int ret = this.DeleteRequest(false);
                if (ret == 0)
                {
                    this.RequestNr = -1;
                    this.Synced = true;
                }

                return ret;
            }

            this.Synced = true;
            return 0;
        }

        /// <summary>
        /// Reports the synchronize error.
        /// </summary>
        /// <param name="_error">The error.</param>
        /// <returns></returns>
        public virtual int ReportSyncError(Exception _error)
        {
            Logger?.LogDebug($"UPSync request {this.RequestNr} {this} returned sync error {_error}", LogFlag.LogUpSync);
            int ret = this.StoreError(_error);
            this.Error = _error.Message;
            return ret;
        }

        /// <summary>
        /// Saves the request root.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>o if success, else error number</returns>
        public int SaveRequestRoot(IDatabase database)
        {
            if (this.RequestNr < 0)
            {
                return -1;
            }

            return this.SaveRequest(database, null, -1);
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public bool CreateRequest(IDatabase database)
        {
            if (this.RequestNr >= 0)
            {
                return false;
            }

            int requestNr = this.SaveRequest(database, null, -1);
            if (requestNr > -1)
            {
                this.RequestNr = requestNr;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the request.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="type">The type.</param>
        /// <param name="followUpRequestId">The follow up request identifier.</param>
        /// <returns></returns>
        int SaveRequest(IDatabase database, string type, int followUpRequestId)
        {
            DatabaseStatement statement = new DatabaseStatement(database);
            IOfflineStorage storage = this.Storage;
            int requestId = this.RequestNr < 0 ? (this.ReservedRequestNr <= 0 ? storage.NextId : this.ReservedRequestNr) : this.RequestNr;

            string sql = @"INSERT INTO requests (requestnr, syncdate, requesttype, json, processtype, draft, titleLine, detailsLine, imageName, 
                           error, serverRequestNumber, followuproot, translationkey, relatedinfo, baseError, appversion) 
                           VALUES (?,datetime('now'),?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            if (statement.Prepare(sql))
            {
                storage.ClearCachedRequestNumbers();
                statement.Bind(1, requestId);
                statement.Bind(2, this.RequestType.ToString()); // TODO Verify, values are going correctly
                statement.Bind(3, this.Json);
                statement.Bind(4, this.ProcessType.ToString());
                statement.Bind(5, 0);
                statement.Bind(6, this.titleLine);
                statement.Bind(7, this.detailsLine);
                statement.Bind(8, this.imageName);

                if (!string.IsNullOrEmpty(type))
                {
                    statement.Bind(9, type);
                }
                else if (this.blockExecution)
                {
                    statement.Bind(9, Constants.OFFLINEREQUEST_BLOCKED_TEXT);
                }
                else
                {
                    statement.Bind(9, null);
                }

                statement.Bind(10, this.ServerRequestNumber);
                statement.Bind(11, followUpRequestId >= 0 ? (object)followUpRequestId : null);
                statement.Bind(12, !string.IsNullOrEmpty(this.ErrorTranslationKey) ? this.ErrorTranslationKey : null);

                statement.Bind(13, this.RelatedInfoDictionary != null ? StringExtensions.StringFromObject(this.RelatedInfoDictionary) : null);

                statement.Bind(14, this.BaseErrorCode != 0 ? (object)this.BaseErrorCode : null);

                this.ApplicationVersion = Constants.CRMPAD_CLIENTVERSION;
                statement.Bind(15, this.ApplicationVersion);

                if (statement.Execute() == 0)
                {
                    return requestId;
                }
            }

            return -1;
        }

        /// <summary>
        /// Requests the mode from string the default.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public static UPOfflineRequestMode RequestModeFromString(string requestMode, UPOfflineRequestMode mode)
        {
            switch (requestMode)
            {
                case "Offline":
                    return UPOfflineRequestMode.Offline;

                case "Online":
                    return UPOfflineRequestMode.OnlineOnly;

                case "Best":
                    return UPOfflineRequestMode.OnlineConfirm;

                case "Fastest":
                    return UPOfflineRequestMode.OnlineNoConfirm;
            }

            return mode;
        }

        /// <summary>
        /// Called when [pre start multi request].
        /// </summary>
        public virtual void OnPreStartMultiRequest()
        {
        }

        /// <summary>
        /// Redoes the record operations.
        /// </summary>
        public virtual void RedoRecordOperations()
        {
        }

        /// <summary>
        /// Reserves the request nr.
        /// </summary>
        /// <returns></returns>
        public int ReserveRequestNr()
        {
            if (this.RequestNr > 0)
            {
                return this.RequestNr;
            }

            if (this.ReservedRequestNr <= 0)
            {
                this.ReservedRequestNr = this.Storage.NextId;
            }

            return this.ReservedRequestNr;
        }
    }
}
