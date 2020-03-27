// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticateServerOperation.cs" company="Aurea Software Gmbh">
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
//   implements the server authentication operation
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.OperationHandling.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// implements the server authentication operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.JsonResponseServerOperation" />
    public class AuthenticateServerOperation : JsonResponseServerOperation
    {
        /// <summary>
        /// The clientversion
        /// </summary>
        public const string Clientversion = "2.6";

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateServerOperation"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="requestServerInformation">
        /// if set to <c>true</c> [request server information].
        /// </param>
        /// <param name="requestSessionInformation">
        /// if set to <c>true</c> [request session information].
        /// </param>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public AuthenticateServerOperation(
            string username,
            string password,
            bool requestServerInformation,
            bool requestSessionInformation,
            string languageKey,
            IAuthenticateServerOperationDelegate theDelegate)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     { "Service", "Authenticate" },
                                     { "Method", "Logon" },
                                     { "ClientVersion", Clientversion }
                                 };

            if (!string.IsNullOrWhiteSpace(username))
            {
                parameters["UserName"] = username;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                parameters["Password"] = password;
            }

            parameters["Language"] = string.IsNullOrWhiteSpace(languageKey) ? string.Empty : languageKey;
            parameters["ServerInfo"] = requestServerInformation ? "1" : "0";
            parameters["SessionInfo"] = requestSessionInformation ? "1" : "0";

            this.ParameterDictionary = parameters;

            this.Delegate = theDelegate;
            this.AlwaysPerform = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticateServerOperation"/> class.
        /// </summary>
        protected AuthenticateServerOperation()
        {
        }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IAuthenticateServerOperationDelegate Delegate { get; set; }

        /// <summary>
        /// Gets or sets the parameter dictionary.
        /// </summary>
        /// <value>
        /// The parameter dictionary.
        /// </value>
        public Dictionary<string, string> ParameterDictionary { get; protected set; }

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        /// <value>
        /// The request parameters.
        /// </value>
        public override Dictionary<string, string> RequestParameters => this.ParameterDictionary;

        /// <summary>
        /// Changes the password to.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        public void ChangePasswordTo(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                this.ParameterDictionary["NewPassword"] = string.Empty;
                this.ParameterDictionary["NewPasswordEmpty"] = "true";
            }
            else
            {
                this.ParameterDictionary["NewPassword"] = password;
                this.ParameterDictionary["NewPasswordEmpty"] = string.Empty;
            }
        }

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.Delegate?.AuthenticateServerOperationDidFail(this, error);
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessJsonResponse(Dictionary<string, object> json, RemoteData remoteData)
        {
            this.Delegate?.AuthenticateServerOperationDidFinish(this, json);
        }

        /// <summary>
        /// Processes the json response.
        /// </summary>
        /// <param name="json">
        /// The json.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessJsonSyncObject(DataModelSyncDeserializer json, RemoteData remoteData)
        {
            //this.Delegate?.OnFinishWithObjectResponse(this, json);
        }


        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return
                $"AuthenticateRequest for user: {this.RequestParameters["UserName"]}, language: {this.RequestParameters["Language"]}.";
        }
    }

    /// <summary>
    /// Server operation to perform Revolution cookie validation step
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.ServerOperation" />
    public class RevolutionCookieServerOperation : ServerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevolutionCookieServerOperation"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="theDelegate">
        /// The delegate.
        /// </param>
        public RevolutionCookieServerOperation(
            string username,
            string password,
            IRevolutionCookieServerOperationDelegate theDelegate)
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(username))
            {
                parameters["UserName"] = username;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                parameters["Password"] = password;
            }

            this.ParameterDictionary = parameters;

            this.Delegate = theDelegate;
        }

        /// <summary>
        /// Gets a value indicating whether [always perform].
        /// </summary>
        /// <value>
        /// <c>true</c> if [always perform]; otherwise, <c>false</c>.
        /// </value>
        public override bool AlwaysPerform => true;

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IRevolutionCookieServerOperationDelegate Delegate { get; set; }

        /// <summary>
        /// Gets the parameter dictionary.
        /// </summary>
        /// <value>
        /// The parameter dictionary.
        /// </value>
        public Dictionary<string, string> ParameterDictionary { get; }

        /// <summary>
        /// Gets the request parameters.
        /// </summary>
        /// <value>
        /// The request parameters.
        /// </value>
        public override Dictionary<string, string> RequestParameters => this.ParameterDictionary;

        /// <summary>
        /// Cookieses the specified cookie text.
        /// </summary>
        /// <param name="cookieText">
        /// The cookie text.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Cookie> Cookies(string cookieText, Uri request)
        {
            // Set-Cookie header
            var cookies = new List<Cookie>();
            foreach (var singleCookie in cookieText.Split(','))
            {
                var match = Regex.Match(singleCookie, "(.+?)=(.+?);");
                if (match.Captures.Count == 0)
                {
                    continue;
                }

                cookies.Add(
                    new Cookie(
                        match.Groups[1].ToString().Trim(),
                        match.Groups[2].ToString(),
                        "/",
                        request.Host.Split(':')[0]));
            }

            return cookies;
        }

        /// <summary>
        /// Processes the error with remote data.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessErrorWithRemoteData(Exception error, RemoteData remoteData)
        {
            this.Delegate?.AuthenticateRevolutionCookieServerOperationDidFail(this, error);
        }

        /// <summary>
        /// Processes the remote data.
        /// </summary>
        /// <param name="remoteData">
        /// The remote data.
        /// </param>
        public override void ProcessRemoteData(RemoteData remoteData)
        {
            if (remoteData == null)
            {
                return;
            }

            if (remoteData.ResponseHeaders == null)
            {
                this.Delegate?.AuthenticateRevolutionCookieServerOperationDidFail(this, null);
            }
            else
            {
                Logger.LogDebug(remoteData.ResponseHeaders?.ValueOrDefault("Ras-Code"), LogFlag.LogNetwork);

                // TODO: HTTPCookie.CookiesWithResponseHeaderFieldsForURL(headerParameters, remoteData.Url);
                var cookieArray = new List<Cookie>();
                var cookies = Cookies(remoteData.ResponseHeaders["Set-Cookie"], remoteData.Url);
                var authenticated = false;
                foreach (Cookie cookie in cookies)
                {
                    if (cookie.Name == ".updateAuth")
                    {
                        cookieArray.Add(cookie);
                        authenticated = true;
                    }
                    else if (cookie.Name.IndexOf("UpdateRAS", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        cookieArray.Add(cookie);
                    }
                }

                if (!authenticated)
                {
                    this.Delegate?.AuthenticateRevolutionCookieServerOperationDidFail(
                        this,
                        new Exception("invalidLoginError"));
                    return;
                }

                Uri serverUrl = null;
                if (remoteData.ResponseHeaders.ContainsKey("AppBaseUrl"))
                {
                    serverUrl = new Uri(remoteData.ResponseHeaders["AppBaseUrl"]);
                }
                else
                {
                    var relativeServerUrlString = Encoding.UTF8.GetString(remoteData.Data, 0, remoteData.Data.Length);
                    if (relativeServerUrlString.StartsWith("http:") || relativeServerUrlString.StartsWith("https:"))
                    {
                        serverUrl = new Uri(relativeServerUrlString);
                    }
                    else
                    {
                        var baseServerURL = remoteData.Url; // .URLByDeletingLastPathComponent();

                        // baseServerURL = baseServerURL;//.URLByDeletingLastPathComponent();
                        serverUrl = new Uri(baseServerURL, relativeServerUrlString);
                    }
                }

                this.Delegate?.AuthenticateRevolutionCookieServerOperationDidFinish(
                    this,
                    cookieArray,
                    serverUrl,
                    remoteData.ResponseHeaders);
            }
        }
    }

    /// <summary>
    /// The revolution authentication server operation.
    /// </summary>
    public class RevolutionAuthenticationServerOperation : AuthenticateServerOperation
    {
        /// <summary>
        /// The cookies.
        /// </summary>
        protected List<Cookie> cookies;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevolutionAuthenticationServerOperation"/> class.
        /// </summary>
        /// <param name="cookies">
        /// The cookies.
        /// </param>
        /// <param name="requestServerInformation">
        /// The request server information.
        /// </param>
        /// <param name="requestSessionInformation">
        /// The request session information.
        /// </param>
        /// <param name="languageKey">
        /// The language key.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public RevolutionAuthenticationServerOperation(
            List<Cookie> cookies,
            bool requestServerInformation,
            bool requestSessionInformation,
            string languageKey,
            IAuthenticateServerOperationDelegate theDelegate)
        {
            this.cookies = cookies;

            var parameters = new Dictionary<string, string>
                                 {
                                     { "Service", "Authenticate" },
                                     { "Method", "RASLogon" },
                                     { "ClientVersion", Clientversion }
                                 };

            parameters["Language"] = string.IsNullOrWhiteSpace(languageKey) ? string.Empty : languageKey;
            parameters["ServerInfo"] = requestServerInformation ? "1" : "0";
            parameters["SessionInfo"] = requestSessionInformation ? "1" : "0";

            this.ParameterDictionary = parameters;
            this.Delegate = theDelegate;
            this.AlwaysPerform = true;
        }

        /// <summary>
        /// The http header fields.
        /// </summary>
        public override Dictionary<string, string> HttpHeaderFields => null;

        // NSHTTPCookie.RequestHeaderFieldsWithCookies(Cookies)
    }
}
