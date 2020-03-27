// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteServer.cs" company="Aurea Software Gmbh">
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
//   Defines the server authentication methods
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Session
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Aurea.CRM.Core.Extensions;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines the server authentication methods
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ServerAuthenticationType
    {
        /// <summary>
        /// The revolution.
        /// </summary>
        Revolution = 0, // Login via SaaS

        /// <summary>
        /// The sso.
        /// </summary>
        SSO, // Single Sign On, no parameters for server (e.g. VPN)

        /// <summary>
        /// The sso credential session cache.
        /// </summary>
        SSOCredentialSessionCache, // Single Sign On, windows user credentials in session have to be passed

        /// <summary>
        /// The username password.
        /// </summary>
        UsernamePassword, // Username and Password are sent as HTTP parameters

        /// <summary>
        /// The username password credentials.
        /// </summary>
        UsernamePasswordCredentials,

        // Username and Password are sent as HTTP parameters and windows user credentials are sent in session

        /// <summary>
        /// The sso credential no cache.
        /// </summary>
        SSOCredentialNoCache, // Single Sign On, windows user credentials in session have to be passed
    }

    /// <summary>
    /// Defines the password storing methods
    /// </summary>
    public enum ServerLoginMode
    {
        /// <summary>
        /// The default.
        /// </summary>
        Default = 0, // Do Nothing

        /// <summary>
        /// The store password.
        /// </summary>
        StorePassword, // Store Password locally

        /// <summary>
        /// The store password login.
        /// </summary>
        StorePasswordLogin, // Store Password locally and try auto login
    }

    /// <summary>
    /// Defines the remote server connection details
    /// Functionally this is simmilar to UPCRMServer of CRM.Pad
    /// </summary>
    public class RemoteServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServer" /> class.
        /// </summary>
        public RemoteServer()
        {
            this.DisableChangePassword = false;
        }

        /// <summary>
        /// Gets or sets the uri used to create the Remote server
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the type of the authentication.
        /// </summary>
        /// <value>
        /// The type of the authentication.
        /// </value>
        [JsonProperty("serverAuthenticationType")]
        public ServerAuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the default error report email address.
        /// </summary>
        /// <value>
        /// The default error report email address.
        /// </value>
        [JsonProperty("defaultErrorReportEmailAddress")]
        public string DefaultErrorReportEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [disable change password].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable change password]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("disableChangePassword")]
        public bool DisableChangePassword { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is site minder authentication required.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is site minder authentication required; otherwise, <c>false</c>.
        /// </value>
        public bool IsSiteMinderAuthenticationRequired => !string.IsNullOrWhiteSpace(this.SiteMinderUrl);

        /// <summary>
        /// Gets or sets the login mode.
        /// </summary>
        /// <value>
        /// The login mode.
        /// </value>
        [JsonProperty("loginMode")]
        public ServerLoginMode LoginMode { get; set; }

        /// <summary>
        /// Webservices the URL.
        /// </summary>
        /// <returns></returns>
        public Uri MobileWebserviceUrl => new Uri(this.ServerUrl, "mobile.axd");

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the network password.
        /// </summary>
        /// <value>
        /// The network password.
        /// </value>
        [JsonProperty("networkPassword")]
        public string NetworkPassword { get; set; }

        /// <summary>
        /// Gets or sets the network username.
        /// </summary>
        /// <value>
        /// The network username.
        /// </value>
        [JsonProperty("networkUsername")]
        public string NetworkUsername { get; set; }

        /// <summary>
        /// Gets or sets the original server URL.
        /// </summary>
        /// <value>
        /// The original server URL.
        /// </value>
        [JsonProperty("originalServerURL")]
        public Uri OriginalServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the server identification.
        /// </summary>
        /// <value>
        /// The server identification.
        /// </value>
        [JsonProperty("serverIdentification")]
        public string ServerIdentification { get; set; }

        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        [JsonProperty("serverURL")]
        public Uri ServerUrl { get; set; }

        /// <summary>
        /// Gets the server URL in string.
        /// </summary>
        /// <value>
        /// The server URL.
        /// </value>
        public string StringServerUrl => this.ServerUrl?.ToString();

        /// <summary>
        /// Gets or sets the site minder URL.
        /// </summary>
        /// <value>
        /// The site minder URL.
        /// </value>
        [JsonProperty("siteMinderURL")]
        public string SiteMinderUrl { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets the initial user name
        /// </summary>
        public string InitialUserName { get; private set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the AzureReturnUri.
        /// </summary>
        public string AzureReturnUri { get; set; }

        /// <summary>
        /// Gets or sets the AzureTenantId.
        /// </summary>
        public string AzureTenantId { get; set; }

        /// <summary>
        ///  Gets or sets the AzureApplictionId.
        /// </summary>
        public string AzureApplictionId { get; set; }

        /// <summary>
        /// Parses the login mode.
        /// </summary>
        /// <param name="loginMode">
        /// The login mode.
        /// </param>
        /// <returns>
        /// The <see cref="ServerLoginMode"/>.
        /// </returns>
        public static ServerLoginMode ParseLoginMode(string loginMode)
        {
            if (string.IsNullOrWhiteSpace(loginMode))
            {
                return ServerLoginMode.Default;
            }

            var lowered = loginMode.ToLower();

            if (lowered == "savepassword")
            {
                return ServerLoginMode.StorePassword;
            }

            return lowered == "savepasswordlogin" ? ServerLoginMode.StorePasswordLogin : ServerLoginMode.Default;
        }

        /// <summary>
        /// Parses the authentication type.
        /// </summary>
        /// <param name="authType">
        /// The auth type.
        /// </param>
        /// <returns>
        /// The <see cref="ServerAuthenticationType"/>.
        /// </returns>
        public static ServerAuthenticationType ParseAuthenticationType(string authType)
        {
            if (string.IsNullOrWhiteSpace(authType))
            {
                return ServerAuthenticationType.UsernamePassword;
            }

            var lowered = authType.ToLower();

            switch (lowered)
            {
                case "sso":
                    return ServerAuthenticationType.SSO;
                case "ssocredentials":
                    return ServerAuthenticationType.SSOCredentialSessionCache;
                case "ssocredentialsnocache":
                    return ServerAuthenticationType.SSOCredentialNoCache;
                case "revolution":
                    return ServerAuthenticationType.Revolution;
                case "username":
                    return ServerAuthenticationType.UsernamePassword;
                case "usernamecredentials":
                    return ServerAuthenticationType.UsernamePasswordCredentials;
                default:
                    return ServerAuthenticationType.UsernamePassword;
            }
        }

        /// <summary>
        /// Creates the remote server from URI data.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static RemoteServer CreateRemoteServerFromUriData(Uri uri)
        {
            var queryDict = uri.ExtractQueryString();
            var remoteServer = new RemoteServer
            {
                Uri = uri
            };
            var serverIdentifier = queryDict.ValueOrDefault("identification");
            var serverName = queryDict.ValueOrDefault("name");
            var serverUrlString = queryDict.ValueOrDefault("url");
            var authenticationType = queryDict.ValueOrDefault("authenticationType");
            var networkUsername = queryDict.ValueOrDefault("networkUsername");
            var networkPassword = queryDict.ValueOrDefault("networkPassword");
            var defaultErrorReportEmailAddress = queryDict.ValueOrDefault("defaultErrorReportEmailAddress");
            var siteMinderUrl = queryDict.ValueOrDefault("siteMinderURL");
            var userAgent = queryDict.ValueOrDefault("userAgent");
            var disableChangePasswordString = queryDict.ValueOrDefault("disableChangePassword");
            var initialUserName = queryDict.ValueOrDefault("username");
            var loginMode = queryDict.ValueOrDefault("loginMode");
            var azureTenantId = queryDict.ValueOrDefault("azureTenantId");
            var azureApplictionId = queryDict.ValueOrDefault("azureApplictionId");
            var azureReturnUri = queryDict.ValueOrDefault("azureReturnUri");

            if (!string.IsNullOrEmpty(serverIdentifier))
            {
                remoteServer.ServerIdentification = serverIdentifier;
            }

            if (!string.IsNullOrEmpty(serverName))
            {
                remoteServer.Name = serverName;
            }

            if (!string.IsNullOrEmpty(serverUrlString))
            {
                if (!serverUrlString.EndsWith(@"/", StringComparison.CurrentCulture))
                {
                    serverUrlString += @"/";
                }

                remoteServer.ServerUrl = new Uri(serverUrlString);
                remoteServer.OriginalServerUrl = new Uri(serverUrlString + @"\login");
            }

            if (!string.IsNullOrEmpty(authenticationType))
            {
                remoteServer.AuthenticationType = ParseAuthenticationType(authenticationType);
            }

            if (!string.IsNullOrEmpty(networkUsername) && !string.IsNullOrEmpty(networkPassword))
            {
                remoteServer.NetworkUsername = networkUsername;
                remoteServer.NetworkPassword = networkPassword;
            }

            if (!string.IsNullOrEmpty(siteMinderUrl))
            {
                remoteServer.SiteMinderUrl = siteMinderUrl;
            }

            if (!string.IsNullOrEmpty(defaultErrorReportEmailAddress))
            {
                remoteServer.DefaultErrorReportEmailAddress = defaultErrorReportEmailAddress;
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                remoteServer.UserAgent = userAgent;
            }

            if (!string.IsNullOrEmpty(loginMode))
            {
                remoteServer.LoginMode = ParseLoginMode(loginMode);
            }

            if (!string.IsNullOrEmpty(initialUserName))
            {
                remoteServer.InitialUserName = initialUserName;
            }

            if (!string.IsNullOrEmpty(disableChangePasswordString))
            {
                remoteServer.DisableChangePassword = disableChangePasswordString == "true";
            }

            if (!string.IsNullOrEmpty(azureApplictionId))
            {
                remoteServer.AzureApplictionId = azureApplictionId;
            }

            if (!string.IsNullOrEmpty(azureTenantId))
            {
                remoteServer.AzureTenantId = azureTenantId;
            }

            if (!string.IsNullOrEmpty(azureReturnUri))
            {
                remoteServer.AzureReturnUri = azureReturnUri;
            }

            return remoteServer;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.ServerIdentification}";
        }

        /// <summary>
        /// Compares to servers for equivalence
        /// </summary>
        /// <param name="other">The server to be compared to</param>
        /// <returns>True of the two servers are equaivalent</returns>
        public bool IsEquivalent(RemoteServer other)
        {
            return this.ServerUrl.Equals(other.ServerUrl) && this.Name == other.Name &&
                   this.ServerIdentification == other.ServerIdentification;
        }
    }
}
