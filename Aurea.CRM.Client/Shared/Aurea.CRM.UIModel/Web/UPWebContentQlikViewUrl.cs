// <copyright file="UPWebContentQlikViewUrl.cs" company="Aurea Software Gmbh">
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
//   UPWebContentQlikViewUrl
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Web
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// UPWebContentQlikViewUrl
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadataUrl" />
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.Data.UPQlikViewAuthenticateServerOperationDelegate" />
    public class UPWebContentQlikViewUrl : UPWebContentMetadataUrl, UPQlikViewAuthenticateServerOperationDelegate
    {
        /// <summary>
        /// The redirected URL
        /// </summary>
        protected Uri redirectedUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentQlikViewUrl"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentQlikViewUrl(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType => "Url";

        /// <summary>
        /// Gets a value indicating whether [allows full screen].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows full screen]; otherwise, <c>false</c>.
        /// </value>
        public override bool AllowsFullScreen => true;

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public override void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
            string documentName = viewReference.ContextValueForKey("DocumentName");
            string serverName = viewReference.ContextValueForKey("ServerName");
            string userLogin = viewReference.ContextValueForKey("UserLogin");
            string scheme = viewReference.ContextValueForKey("ConnectionType");
            string loginMethod = viewReference.ContextValueForKey("LoginMethod");
            if (loginMethod != null && loginMethod == "AD")
            {
                this.Logger.LogInfo("Using Windows authentication for QklikView.");
                //DDLogInfo("Using Windows authentication for QklikView.");
            }
            else
            {
                userLogin = $"{userLogin}{ServerSession.CurrentSession.UserName}";
                loginMethod = string.Empty;
                this.Logger.LogInfo($"Using username authentication for QklikView - username: {userLogin}");
                //DDLogInfo("Using username authentication for QklikView - username: %@.", userLogin);
            }

            ServiceInfo serviceInfo = ServerSession.CurrentSession.ServiceInfoForServiceName("Authenticate");
            if (serviceInfo.IsAtLeastVersion("1.2"))
            {
                UPQlikViewAuthenticateServerOperation operation;
                if (scheme != null || loginMethod != null)
                {
                    operation = new UPQlikViewAuthenticateServerOperation(serverName, userLogin, documentName, scheme, loginMethod, this);
                }
                else
                {
                    operation = new UPQlikViewAuthenticateServerOperation(serverName, userLogin, documentName, this);
                }

                ServerSession.CurrentSession.ExecuteRequest(operation);
            }
            else
            {
                this.Logger.LogError("Server does not support QlikView authentication.");
                //DDLogError("Server does not support QlikView authentication.");
            }
        }

        /// <summary>
        /// Qliks the view authenticate server operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void QlikViewAuthenticateServerOperationDidFailWithError(UPQlikViewAuthenticateServerOperation operation, Exception error)
        {
            //Console.WriteLine("Could not authenticate against QlikView server. Error %@", error);
        }

        /// <summary>
        /// Qliks the view server operation did finish did finish with json response.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="json">The json.</param>
        public void QlikViewServerOperationDidFinishDidFinishWithJSONResponse(UPQlikViewAuthenticateServerOperation operation, Dictionary<string, object> json)
        {
            //string redirectUrlString = json.ObjectForKey("redirectUrl");
            //if (redirectUrlString && redirectUrlString.Length() > 0)
            //{
            //    NSURL redirectUrl = new NSURL(redirectUrlString);
            //    if (redirectUrl == null)
            //    {
            //        DDLogInfo("Could not create url object for url %@ ... trying to escape it.", redirectUrlString);
            //        redirectUrlString = redirectUrlString.StringByAddingPercentEscapesUsingEncoding(NSUTF8StringEncoding);
            //        redirectUrl = new NSURL(redirectUrlString);
            //        DDLogInfo("Escaped url string is %@", redirectUrlString);
            //    }

            //    if (redirectUrl == null)
            //    {
            //        DDLogError("Could not create url object for string %@.", redirectUrlString);
            //        return;
            //    }

            //    this.SetUrl(redirectUrl);
            //    NSRange tryRange = redirectUrlString.RangeOfString("try=");
            //    if (tryRange.Length > 0)
            //    {
            //        string tryUrl = redirectUrlString.SubstringFromIndex((tryRange.Location + 4));
            //        if (tryUrl && tryUrl.Length() > 0)
            //        {
            //            redirectedUrl = new NSURL(tryUrl);
            //        }
            //    }
            //    else
            //    {
            //        DDLogError("Could not found try url in %@", redirectUrlString);
            //        return;
            //    }

            //    NSMutableURLRequest request = new NSMutableURLRequest(redirectUrl);
            //    NSURLConnection connection = new NSURLConnection(request, this);
            //    if (!connection)
            //    {
            //        DDLogError("Could not create connection object for qlikview authentication.");
            //    }
            //}
        }

        //void ConnectionDidFailWithError(NSURLConnection connection, Exception error)
        //{
        //    DDLogError("Server did not send a valid redirect url for qlikview authentication.");
        //}

        //void ConnectionDidFinishLoading(NSURLConnection connection)
        //{
        //    this.TheDelegate.WebContentMetaDataFinishedWithRedirectUrl(this, this.redirectedUrl);
        //}
    }
}
