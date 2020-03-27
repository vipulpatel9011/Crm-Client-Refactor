// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPWebContentMetadataUrl.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Max Menezes, Serdar Tepeyurt
// </author>
// <summary>
//   The Web Content Metadata Url class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.OperationHandling.Data;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// The Web Content Metadata Url class
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadata" />
    public class UPWebContentMetadataUrl : UPWebContentMetadata
    {
        private OpenUrlOperation urlOperation;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentMetadataUrl"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentMetadataUrl(UPWebContentMetadataDelegate theDelegate)
            : base(theDelegate)
        {
            this.Finished = true;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [local URL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [local URL]; otherwise, <c>false</c>.
        /// </value>
        public bool LocalUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPWebContentMetadataUrl"/> is finished.
        /// </summary>
        /// <value>
        ///   <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        public bool Finished { get; set; }

        /// <summary>
        /// Gets the type of the report.
        /// </summary>
        /// <value>
        /// The type of the report.
        /// </value>
        public override string ReportType => "Url";

        /// <summary>
        /// Updates the metadata with view reference.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public override void UpdateMetadataWithViewReference(ViewReference viewReference)
        {
            base.UpdateMetadataWithViewReference(viewReference);
            string urlAsString = viewReference.ContextValueForKey("Url");
            this.LocalUrl = viewReference.ContextValueForKey("localWebArchiv") == "true";
            if (this.LocalUrl)
            {
                this.Url = new Uri($"{ServerSession.CurrentSession.SessionSpecificCachesPath}{urlAsString}.htm");
            }
            else
            {
                if (!urlAsString.Contains("$cur"))
                {
                    ICRMDataStore dataStore = UPCRMDataStore.DefaultStore;
                    urlAsString = urlAsString.Replace("$curRepId;", dataStore.Reps.CurrentRepId);
                    urlAsString = urlAsString.Replace("$curRep;", dataStore.Reps.CurrentRep.RepName);
                    urlAsString = urlAsString.Replace("$curTenantNo;", ServerSession.CurrentSession.TenantNo.ToString());
                    urlAsString = urlAsString.Replace("$curLanguage;", ServerSession.CurrentSession.LanguageKey);
                }

                if (string.IsNullOrEmpty(viewReference.ContextValueForKey("FieldGroup"))
                    || string.IsNullOrEmpty(viewReference.ContextValueForKey("RecordId")))
                {
                    this.Url = new Uri(urlAsString);
                }
                else
                {
                    this.Finished = false;
                    this.urlOperation = new OpenUrlOperation(
                        urlAsString,
                        viewReference.ContextValueForKey("RecordId"),
                        viewReference.ContextValueForKey("FieldGroup"),
                        (Uri result, Exception error) =>
                        {
                            this.Url = result;
                            if (error != null)
                            {
                                this.TheDelegate.WebContentMetaDataFailedWithError(this, error);
                            }
                            else
                            {
                                this.TheDelegate.WebContentMetaDataFinishedWithRedirectUrl(this, this.Url);
                            }
                        });

                    this.urlOperation?.PerformOperation();
                }

                if (this.Url == null)
                {
                    this.Url = new Uri("about:blank");
                }
                else if (string.IsNullOrEmpty(this.Url.Scheme))
                {
                    this.Url = new Uri($"http://{urlAsString}");
                }
            }
        }
    }
}
