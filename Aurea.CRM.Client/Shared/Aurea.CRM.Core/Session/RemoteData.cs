// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteData.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Interface for a remote data tracker
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Services;

    // using Microsoft.Practices.ServiceLocation;

    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Interface for a remote data tracker
    /// </summary>
    public interface IRemoteDataTracking
    {
        /// <summary>
        /// On request cancelation.
        /// </summary>
        void Canceled();

        /// <summary>
        /// On Data receive finished.
        /// </summary>
        void DataFinished();

        /// <summary>
        /// On Data received.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        void DataReceived(byte[] data);

        /// <summary>
        /// On an Error received.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        void ErrorReceived(Exception error);

        /// <summary>
        /// The Request loaded event.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        void RequestLoaded(HttpWebRequest request);

        /// <summary>
        /// The Responses received event.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        void ResponseReceived(HttpWebResponse response);
    }

    /// <summary>
    /// Remote data loaded error code
    /// </summary>
    public enum RemoteDataLoadErrorCodes
    {
        /// <summary>
        /// The no data returned.
        /// </summary>
        NoDataReturned = 1,

        /// <summary>
        /// The not valid json.
        /// </summary>
        NotValidJson,

        /// <summary>
        /// The no internet connection.
        /// </summary>
        NoInternetConnection,

        /// <summary>
        /// The general.
        /// </summary>
        General
    }

    /// <summary>
    /// Remote data request handling
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class RemoteData
    {
        private readonly CancellationTokenSource tokenSource;
        private bool canceled;
        private int missingServiceNameRetryCount;
        private Dictionary<string, object> pendingDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteData"/> class.
        /// </summary>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <param name="remoteSession">
        /// The remote session.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public RemoteData(Uri url, RemoteSession remoteSession, IRemoteDataDelegate theDelegate)
        {
            this.Url = url;
            this.Delegate = theDelegate;
            this.CookieStorage = remoteSession?.CookieStorage;
            this.tokenSource = new CancellationTokenSource();
            this.CacheResponseOnDisk = false;
            this.Method = "GET";
            this.RequestTimeout = 60.0;
            var session = ServerSession.CurrentSession;
            if (session != null)
            {
                var requestTimeout = session.ClientRequestTimeout;
                if (requestTimeout > 0.0)
                {
                    this.RequestTimeout = requestTimeout;
                }

                this.tokenSource.CancelAfter(new TimeSpan(0, 0, (int)this.RequestTimeout));
                var userAgent = session.CrmServer.UserAgent;
                if (!string.IsNullOrWhiteSpace(userAgent))
                {
                    this.UserAgent = userAgent;
                }

                this.AsyncRetryTime = session.AsynchronousRetryTime;
                this.AsyncWaitTime = session.AsynchronousWaitTime;
                this.AuthenticationType = session.CrmServer.AuthenticationType;
            }

            this.NetworkLog = Logger.LogSettings.LogNetwork;
            this.RetryCount = 3;
        }

        public ServerAuthenticationType AuthenticationType { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets or sets the type of the accept.
        /// </summary>
        /// <value>
        /// The type of the accept.
        /// </value>
        public string AcceptType { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous retry time.
        /// </summary>
        /// <value>
        /// The asynchronous retry time.
        /// </value>
        public int AsyncRetryTime { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous wait time.
        /// </summary>
        /// <value>
        /// The asynchronous wait time.
        /// </value>
        public int AsyncWaitTime { get; set; }

        /// <summary>
        /// Gets or sets the cache policy.
        /// </summary>
        /// <value>
        /// The cache policy.
        /// </value>
        public RequestCachePolicy CachePolicy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [cache response on disk].
        /// </summary>
        /// <value>
        /// <c>true</c> if [cache response on disk]; otherwise, <c>false</c>.
        /// </value>
        public bool CacheResponseOnDisk { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the cookie storage.
        /// </summary>
        /// <value>
        /// The cookie storage.
        /// </value>
        public CookieStorage CookieStorage { get; set; }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>
        /// The credentials.
        /// </value>
        public NetworkCredential Credentials { get; set; }

        /// <summary>
        /// Gets or sets the custom HTTP header fields.
        /// </summary>
        /// <value>
        /// The custom HTTP header fields.
        /// </value>
        public Dictionary<string, string> CustomHttpHeaderFields { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public byte[] Data { get; set; }

        /// <summary>
        /// The delegate.
        /// </summary>
        public IRemoteDataDelegate Delegate { get; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [network log].
        /// </summary>
        /// <value>
        /// <c>true</c> if [network log]; otherwise, <c>false</c>.
        /// </value>
        public bool NetworkLog { get; set; }

        /// <summary>
        /// Gets or sets the request body data.
        /// </summary>
        /// <value>
        /// The request body data.
        /// </value>
        public byte[] RequestBodyData { get; set; }

        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        /// <value>
        /// The response headers.
        /// </value>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>
        /// The retry count.
        /// </value>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public double RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets the tracking delegate.
        /// </summary>
        /// <value>
        /// The tracking delegate.
        /// </value>
        public IRemoteDataTracking TrackingDelegate { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public string UserAgent { get; set; }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            this.TrackingDelegate.Canceled();
            if (this.NetworkLog)
            {
                Logger.LogDebug("request canceled", LogFlag.LogNetwork);
            }

            this.canceled = true;
            this.tokenSource.Cancel();
        }

        /// <summary>
        /// Handles the pending.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public async void HandlePending(Dictionary<string, object> response)
        {
            this.missingServiceNameRetryCount = 0;

            await Task.Delay(this.AsyncRetryTime / 1000);
            this.PendingRetryRequest(response);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public async void Load()
        {
            try
            {
                Logger.LogDebug($"Server operation {this.Method}", LogFlag.LogRequests);
                await SimpleIoc.Default.GetInstance<IMobileWebService>().LoadRemoteData(this, this.tokenSource.Token);

                this.Delegate?.RemoteDataDidFinishLoading(this);
            }
            catch (TaskCanceledException cancelledException)
            {
                Logger.LogError($"Server operation: {this.Url.AbsoluteUri} cancelled after: {this.RequestTimeout} Reason: {cancelledException.Message}");
                this.Delegate?.RemoteDataDidFailLoadingWithError(this, new TaskCanceledException($"{this.RequestTimeout}s"));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Server operation: {this.Url.AbsoluteUri} failed with message: {ex.Message}");
                this.Delegate?.RemoteDataDidFailLoadingWithError(this, ex);
            }
        }

        /// <summary>
        /// Pendings the retry request.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public async void PendingRetryRequest(Dictionary<string, object> dictionary)
        {
            if (this.canceled)
            {
                return;
            }

            var request = WebRequest.CreateHttp(this.Url);
            request.Method = this.Method;

            var requestParameterValues = new List<string>();

            foreach (var key in dictionary.Keys)
            {
                var value = (string)dictionary[key];
                var parameter = $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
                requestParameterValues.Add(parameter);
            }

            requestParameterValues.Add("Service=PendingRequest");

            var bodyData = new UTF8Encoding().GetBytes(string.Join("&", requestParameterValues));

            if (bodyData != null)
            {
                request.Headers["Content-Length"] = $"{bodyData.Length}";

                // Set request body
                var newStream = await request.GetRequestStreamAsync();
                newStream.Write(bodyData, 0, bodyData.Length);
            }

            request.ContentType = this.ContentType;
            request.Accept = this.AcceptType;
            if (!string.IsNullOrWhiteSpace(this.UserAgent))
            {
                request.Headers["User-Agent"] = this.UserAgent;
            }

            if (this.CustomHttpHeaderFields != null)
            {
                foreach (var key in this.CustomHttpHeaderFields.Keys)
                {
                    request.Headers[key] = this.CustomHttpHeaderFields[key];
                }
            }

            if (this.CookieStorage != null)
            {
                // request.HTTPShouldHandleCookies = false;
                this.CookieStorage.AddCookiesToRequest(request);
            }
        }

        /// <summary>
        /// Retries the once.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RetryOnce()
        {
            if (this.missingServiceNameRetryCount == 0)
            {
                this.missingServiceNameRetryCount = 1;
                if (this.pendingDictionary != null)
                {
                    this.PendingRetryRequest(this.pendingDictionary);
                }
                else
                {
                    this.Load();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the request body.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public void SetRequestBody(Dictionary<string, string> dictionary)
        {
            if (dictionary == null || !dictionary.Keys.Any())
            {
                return;
            }

            var requestParameterValues = new List<string>();

            foreach (var key in dictionary.Keys)
            {
                var value = dictionary[key];
                var parameter = $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
                requestParameterValues.Add(parameter);
            }

            if (this.AsyncWaitTime > 0)
            {
                requestParameterValues.Add($"AsyncWaitMS={(long)this.AsyncWaitTime}");
            }

            this.RequestBodyData = new UTF8Encoding().GetBytes(string.Join("&", requestParameterValues));

            this.ContentType = "application/x-www-form-urlencoded";
            this.Method = "POST";
        }

        /// <summary>
        /// Sets the request body.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="paramName">
        /// Name of the parameter.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="contentType">
        /// Type of the content.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public void SetRequestBody(
            byte[] data,
            string paramName,
            string fileName,
            string contentType,
            string encoding,
            Dictionary<string, string> dictionary)
        {
            if (data == null || dictionary == null)
            {
                return;
            }

            const string mainBoundary = "0xKhTmLbOuNdArY---This_Is_ThE_BoUnDaRyy---pqo";
            var postData = new List<byte>();
            var postBody = new StringBuilder();
            foreach (var key in dictionary.Keys)
            {
                var value = dictionary[key];
                postData.AddRange(Encoding.UTF8.GetBytes($"--{mainBoundary}\r\n"));
                postData.AddRange(Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{key}\"\r\n"));
                postData.AddRange(Encoding.UTF8.GetBytes($"\r\n{value}\r\n"));
            }

            postData.AddRange(Encoding.UTF8.GetBytes($"--{mainBoundary}\r\n"));
            postData.AddRange(Encoding.UTF8.GetBytes($"Content-Disposition: form-data; name=\"{paramName}\"; filename=\"{fileName}\"\r\n"));
            postData.AddRange(Encoding.UTF8.GetBytes($"Content-Type: {contentType}\r\n"));
            postData.AddRange(Encoding.UTF8.GetBytes($"Content-Transfer-Encoding: {encoding}\r\n\r\n"));

            // ohne das klappt es im falle binary nicht
            postData.AddRange(data);

            postData.AddRange(Encoding.UTF8.GetBytes($"\r\n--{mainBoundary}--\r\n"));

            // Achtung: vor dem Boundary muss ein Zeilenumbruch stehen
            this.RequestBodyData = postData.ToArray();
            this.ContentType = $"multipart/form-data; boundary={mainBoundary}";
            this.Method = "POST";
        }
    }
}
