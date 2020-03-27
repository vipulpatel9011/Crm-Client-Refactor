// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebContentPageModelController.cs" company="Aurea Software Gmbh">
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
//   The UPWebContentPageModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using Aurea.CRM.UIModel.Web;

    /// <summary>
    /// UPWebContentPageModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.UPPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadataDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    public class UPWebContentPageModelController : UPPageModelController, UPCRMLinkReaderDelegate, UPWebContentMetadataDelegate, UPCopyFieldsDelegate
    {
        protected UPCRMLinkReader linkReader;
        protected bool pageBuilt;
        protected bool pageRequested;
        protected string xmlContentString;
        protected UPCopyFields copyFields;

        /// <summary>
        /// Gets the web content page.
        /// </summary>
        /// <value>
        /// The web content page.
        /// </value>
        public UPMWebContentPage WebContentPage => (UPMWebContentPage)this.Page;

        /// <summary>
        /// Gets or sets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; protected set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allows XML export].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows XML export]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowsXMLExport { get; set; }

        /// <summary>
        /// Gets the web content metadata.
        /// </summary>
        /// <value>
        /// The web content metadata.
        /// </value>
        public UPWebContentMetadata WebContentMetadata { get; private set; }

        /// <summary>
        /// Gets the prefixed field value dictionary.
        /// </summary>
        /// <value>
        /// The prefixed field value dictionary.
        /// </value>
        public Dictionary<string, object> PrefixedFieldValueDictionary { get; private set; }

        /// <summary>
        /// Gets the appearance j script string.
        /// </summary>
        /// <value>
        /// The appearance j script string.
        /// </value>
        public string AppearanceJScriptString { get; private set; }

        /// <summary>
        /// Gets the disappearance j script string.
        /// </summary>
        /// <value>
        /// The disappearance j script string.
        /// </value>
        public string DisappearanceJScriptString { get; private set; }

        /// <summary>
        /// Gets the organizer error.
        /// </summary>
        /// <value>
        /// The organizer error.
        /// </value>
        public Exception OrganizerError { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allows full screen].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows full screen]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowsFullScreen { get; set; }

        /// <summary>
        /// Gets the field value dictionary.
        /// </summary>
        /// <value>
        /// The field value dictionary.
        /// </value>
        public Dictionary<string, object> FieldValueDictionary { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send by email button is shown].
        /// </summary>
        /// <value>
        /// <c>true</c> if [send by email button is shown]; otherwise, <c>false</c>.
        /// </value>
        public bool SendByEmailButtonIsShown { get; set; }

        /// <summary>
        /// Gets the send by email actions.
        /// </summary>
        /// <value>
        /// The send by email actions.
        /// </value>
        public List<Menu> SendByEmailActions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPWebContentPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            this.SendByEmailActions = new List<Menu>();
            this.SendByEmailButtonIsShown = viewReference.ContextValueIsSet("SendByEmail");
            var actionStr = viewReference.ContextValueForKey("SendByEmailAction");
            var actions = !string.IsNullOrEmpty(viewReference.ContextValueForKey("SendByEmailAction")) ? actionStr.Split(',') : new string[0];
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            foreach (string action in actions)
            {
                Menu menu = configStore.MenuByName(action);
                if (menu != null)
                {
                    this.SendByEmailActions.Add(menu);
                }
            }

            this.WebContentMetadata = UPWebContentMetadata.WebContentMetaDataFromReportType(viewReference.ContextValueForKey("ReportType"), this);
            this.WebContentMetadata.UpdateMetadataWithViewReference(this.ViewReference);
            this.AllowsXMLExport = this.WebContentMetadata.AllowsXMLExport;
            this.AllowsFullScreen = this.WebContentMetadata.AllowsFullScreen;
            this.RecordIdentification = viewReference.ContextValueForKey("RecordId");
            IIdentifier identifier = this.BuildIdentifier();
            UPMWebContentPage page = new UPMWebContentPage(identifier)
            {
                Invalid = true,
                PrintEnabled = this.ViewReference.ContextValueForKey("ButtonPrint") == "true"
            };
            this.TopLevelElement = page;
            this.WebContentPage.ReportType = this.WebContentMetadata.ReportType;
            this.ApplyLoadingStatusOnPage(page);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentPageModelController"/> class.
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        public UPWebContentPageModelController(string htmlString)
            : base(null)
        {
            this.WebContentMetadata = new UPWebContentMetadataStaticHtml(null);
            UPMWebContentPage page = new UPMWebContentPage(StringIdentifier.IdentifierWithStringId("Web"))
            {
                Invalid = false,
                ReportType = this.WebContentMetadata.ReportType,
                WebContentHtml = htmlString
            };
            this.TopLevelElement = page;
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        protected virtual void BuildPage()
        {
            this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
            string linkIdStr = this.ViewReference.ContextValueForKey("LinkId");
            if (!string.IsNullOrEmpty(linkIdStr))
            {
                this.LinkId = Convert.ToInt32(linkIdStr);
            }
            else
            {
                this.LinkId = -1;
            }

            this.AppearanceJScriptString = this.ViewReference.ContextValueForKey("AppearanceJScript");
            this.DisappearanceJScriptString = this.ViewReference.ContextValueForKey("DisappearanceJScript");
            FieldControl copySourceFieldControl = null;
            string copySourceFieldGroupName = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
            if (!string.IsNullOrEmpty(copySourceFieldGroupName))
            {
                copySourceFieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", copySourceFieldGroupName);
            }

            if (copySourceFieldControl != null)
            {
                this.copyFields = new UPCopyFields(copySourceFieldControl);
                this.copyFields.CopyFieldValuesForRecordIdentification(this.RecordIdentification, false, this);
            }
            else
            {
                this.ContinueBuildPageWithParameters(null);
            }
        }

        private IIdentifier BuildIdentifier()
        {
            IIdentifier identifier;
            if (this.WebContentMetadata is UPWebContentMetadataClientReport)
            {
                UPWebContentMetadataClientReport clientReport = (UPWebContentMetadataClientReport)this.WebContentMetadata;
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                List<string> usedInfoAreas = new List<string>();

                foreach (UPWebContentClientReport contentClientReport in clientReport.ClientReports)
                {
                    SearchAndList searchAndList = configStore.SearchAndListByName(contentClientReport.ConfigName);
                    FieldControl fieldControl = configStore.FieldControlByNameFromGroup("List", searchAndList == null ? contentClientReport.ConfigName : searchAndList.FieldGroupName);

                    foreach (UPConfigFieldControlField field in fieldControl.Fields)
                    {
                        if (!usedInfoAreas.Contains(field.InfoAreaId))
                        {
                            usedInfoAreas.Add(field.InfoAreaId);
                        }
                    }
                }

                List<IIdentifier> usedIdentifiers = new List<IIdentifier>();
                usedIdentifiers.AddRange(usedInfoAreas.Select(infoArea => StringIdentifier.IdentifierWithStringId($"{infoArea}.*")));

                identifier = usedIdentifiers.Count > 0 ? new MultipleIdentifier(usedIdentifiers) : this.BuildStandartIdentifier();
            }
            else
            {
                identifier = this.BuildStandartIdentifier();
            }

            return identifier;
        }

        private IIdentifier BuildStandartIdentifier()
        {
            IIdentifier identifier;
            if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                identifier = new RecordIdentifier(this.RecordIdentification);
            }
            else
            {
                identifier = StringIdentifier.IdentifierWithStringId("Web");
            }

            return identifier;
        }

        private void ContinueBuildPageWithParameters(Dictionary<string, object> parameters)
        {
            this.copyFields = null;
            this.FieldValueDictionary = parameters;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, List<string>> sessionParameters = ServerSession.CurrentSession.SessionParameterReplacements;
            foreach (string key in sessionParameters.Keys)
            {
                List<string> val = sessionParameters[key];
                if (val.Count > 0)
                {
                    dict[key] = val[0];
                }
            }

            if (parameters?.Count > 0)
            {
                foreach (string key in parameters.Keys)
                {
                    dict[$"${key}"] = parameters[key];
                }
            }

            this.PrefixedFieldValueDictionary = dict;
            string parentLink = this.ViewReference.ContextValueForKey("ParentLink");
            if (!string.IsNullOrEmpty(parentLink))
            {
                this.linkReader = new UPCRMLinkReader(this.RecordIdentification, parentLink, UPRequestOption.FastestAvailable, this);
                this.linkReader.Start();
            }
            else
            {
                this.ContinueBuildPage();
            }
        }

        private void ContinueBuildPage()
        {
            this.WebContentMetadata.UpdateMetadataWithViewReference(this.ViewReference);
            this.pageBuilt = true;
            this.TopLevelElement = this.CreateUpdatedPageWithIdentifier(this.Page.Identifier);
        }

        /// <summary>
        /// Exports the XML.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ExportXML(object sender)
        {
            if (!(this.WebContentMetadata is UPWebContentMetadataClientReport))
            {
                return;
            }

            string xmlString = this.xmlContentString;
            byte[] xmlData = Encoding.UTF8.GetBytes(xmlString);
            //UPMail mail = new UPMail();
            //mail.Subject = "XML Export";
            //UPMailAttachment attachment = new UPMailAttachment(xmlData, "application/xml", "export.xml");
            //mail.AddAttachment(attachment);
            //this.ParentOrganizerModelController.ModelControllerDelegate.SendMailModal(mail, false);
        }

        /// <summary>
        /// Emails the report as PDF action.
        /// </summary>
        /// <param name="_viewTag">The view tag.</param>
        /// <param name="action">The action.</param>
        public virtual void EmailReportAsPdfAction(int _viewTag, Menu action)
        {
            string reportFileName = "report.pdf";
            //UPMail mail = UPMail.TheNew();
            //if (action != null)
            //{
            //    ViewReference sendEmailActionViewReference = action.ViewReference;
            //    sendEmailActionViewReference = sendEmailActionViewReference.ViewReferenceWith(this.RecordIdentification);
            //    string fieldGroup = sendEmailActionViewReference.ContextValueForKey("EmailFieldgroup");
            //    string recordId = sendEmailActionViewReference.ContextValueForKey("RecordId");
            //    mail.FillFromEmailTemplateFieldGroupForRecord(fieldGroup, recordId);
            //    byte[] pdfDataOut = this.ModelControllerDelegate.CreatePdfData(_viewTag);
            //    mail.AddAttachment(new UPMailAttachment(pdfDataOut, "application/pdf", reportFileName));
            //}
            //else
            //{
            //    byte[] pdfDataOut = this.ModelControllerDelegate.CreatePdfData(_viewTag);
            //    string title = this.ParentOrganizerModelController.Organizer.TitleText;
            //    mail.Subject = string.Format(LocalizationKeys.upTextOrderReportEmailSubject, title);
            //    mail.AddAttachment(new UPMailAttachment(pdfDataOut, "application/pdf", reportFileName));
            //}

            //this.ModelControllerDelegate.SendMailModal(mail, true);
        }

        /// <summary>
        /// Shows the full screen.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void ShowFullScreen(object sender)
        {
            //((UPWebContentModelControllerUIDelegate)this.ModelControllerDelegate).ShowFullScreen();
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            if (element is Page)
            {
                this.OrganizerError = this.Delegate?.PageModelControllerContextValueForKey(this, "organizerError") as Exception;
                return this.UpdatedElementForPage((UPMWebContentPage)element);
            }

            return null;
        }

        private void ApplyLoadingStatusOnPage(Page page)
        {
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextLoadingData;
            stillLoadingError.StatusMessageField = statusField;
            page.Status = stillLoadingError;
        }

        /// <summary>
        /// Builds the client report for page cached.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="cached">if set to <c>true</c> [cached].</param>
        /// <returns></returns>
        public virtual bool BuildClientReportForPage(UPMWebContentPage page, bool cached = false)
        {
            if (!(this.WebContentMetadata is UPWebContentMetadataClientReport))
            {
                return true;
            }

            UPWebContentMetadataClientReport clientReport = (UPWebContentMetadataClientReport)this.WebContentMetadata;
            if (cached)
            {
                page.Invalid = false;
                page.WebContentHtml = clientReport.CalcClientReportFromCachedResult();
                return true;
            }

            page.Invalid = true;
            this.ApplyLoadingStatusOnPage(page);
            clientReport.StartWithRecordIdentificationLinkIdParameters(this.RecordIdentification, this.LinkId, this.FieldValueDictionary);
            return false;
        }

        /// <summary>
        /// Forces the page update.
        /// </summary>
        /// <param name="changedIdentifiers">The changed identifiers.</param>
        public override void ForcePageUpdate(List<IIdentifier> changedIdentifiers)
        {
            this.WebContentPage.WebContentHtml = null;
            base.ForcePageUpdate(changedIdentifiers);
        }

        /// <summary>
        /// Specials the handling for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected virtual Page SpecialHandlingForPage(UPMWebContentPage page)
        {
            return null;
        }

        private Page UpdatedElementForPage(UPMWebContentPage originalPage)
        {
            if (!this.pageBuilt)
            {
                if (!this.pageRequested)
                {
                    this.pageRequested = true;
                    this.BuildPage();
                    return this.WebContentPage;
                }
            }

            if (!this.WebContentPage.Invalid)
            {
                return this.WebContentPage;
            }

            if (originalPage.Status is UPMErrorStatus)
            {
                UPMWebContentPage page = new UPMWebContentPage(originalPage.Identifier)
                {
                    Status = originalPage.Status,
                    Invalid = true
                };
                this.TopLevelElement = page;
                return page;
            }

            this.TopLevelElement = this.CreateUpdatedPageWithIdentifier(originalPage.Identifier);
            return this.TopLevelElement as Page;
        }

        private Page CreateUpdatedPageWithIdentifier(IIdentifier identifier)
        {
            UPMWebContentPage page = new UPMWebContentPage(identifier)
            {
                ReportType = this.WebContentPage.ReportType,
                PrintEnabled = this.WebContentPage.PrintEnabled
            };
            Page special = this.SpecialHandlingForPage(page);
            if (special != null)
            {
                return special;
            }

            if (this.WebContentMetadata is UPWebContentMetadataUrl)
            {
                UPWebContentMetadataUrl webContentMetaDataUrl = (UPWebContentMetadataUrl)this.WebContentMetadata;
                page.WebContentUrl = webContentMetaDataUrl.Url;
            }
            else if (this.WebContentMetadata is UPWebContentMetadataClientReport)
            {
                this.AllowsXMLExport = true;
                if (!this.BuildClientReportForPage(page))
                {
                    return page;
                }

                this.ApplyLoadingStatusOnPage(page);
            }
            else if (this.WebContentMetadata is UPWebContentMetadataServerReport)
            {
                page.WebContentUrl = ((UPWebContentMetadataServerReport)this.WebContentMetadata).CalcServiceURLWithRecordIdentification(this.RecordIdentification);
                string loadManuallySetting = this.ViewReference.ContextValueForKey("LoadManually");
                if (!string.IsNullOrEmpty(loadManuallySetting) && loadManuallySetting != "false")
                {
                    page.LoadContentManually = true;
                }
            }

            page.Invalid = false;
            page.Status = null;
            return page;
        }

        private string StringWithContextReplacements(string jscript)
        {
            Dictionary<string, object> dict = this.Delegate.ContextValueDictionaryForPageModelController(this);
            foreach (string key in dict.Keys)
            {
                string value = dict[key] as string;
                if (value != null)
                {
                    jscript = jscript.Replace($"{{${key}}}", value);
                }
            }

            return jscript;
        }

        /// <summary>
        /// Gets the appearance script.
        /// </summary>
        /// <value>
        /// The appearance script.
        /// </value>
        public string AppearanceScript => string.IsNullOrEmpty(this.AppearanceJScriptString)
                    ? null : this.StringWithContextReplacements(this.AppearanceJScriptString);

        /// <summary>
        /// Gets the disappearance script.
        /// </summary>
        /// <value>
        /// The disappearance script.
        /// </value>
        public string DisappearanceScript => string.IsNullOrEmpty(this.DisappearanceJScriptString) ? null : this.StringWithContextReplacements(this.DisappearanceJScriptString);

        /// <summary>
        /// Gets a value indicating whether [disable signing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable signing]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool DisableSigning => true;

        /// <summary>
        /// Gets a value indicating whether this instance is online.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is online; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnline => this.ParentOrganizerModelController.OnlineData;

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public virtual void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            if (!string.IsNullOrEmpty(_linkReader.DestinationRecordIdentification))
            {
                this.RecordIdentification = _linkReader.DestinationRecordIdentification;
                this.ContinueBuildPage();
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public virtual void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            this.ContinueBuildPage();
        }

        /// <summary>
        /// Webs the content meta data finished with XML string.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="xmlString">The XML string.</param>
        public void WebContentMetaDataFinishedWithXmlString(UPWebContentMetadata clientReport, string xmlString)
        {
            UPMWebContentPage page = new UPMWebContentPage(this.Page.Identifier);
            page.Invalid = false;
            this.TopLevelElement = page;
            this.xmlContentString = xmlString;
            page.ReportType = "ClientReport";
            ((UPMWebContentPage)this.Page).WebContentHtml = this.xmlContentString;
            this.InformAboutDidChangeTopLevelElement(null, null, null, null);
        }

        /// <summary>
        /// Webs the content meta data failed with error.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="error">The error.</param>
        public void WebContentMetaDataFailedWithError(UPWebContentMetadata clientReport, Exception error)
        {
            this.ContinueBuildPage();
        }

        /// <summary>
        /// Webs the content meta data finished with redirect URL.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="url">The URL.</param>
        public void WebContentMetaDataFinishedWithRedirectUrl(UPWebContentMetadata clientReport, Uri url)
        {
            UPMWebContentPage page = new UPMWebContentPage(this.Page.Identifier);
            page.Invalid = false;
            this.TopLevelElement = page;
            page.WebContentUrl = url;
            this.InformAboutDidChangeTopLevelElement(null, null, null, null);
        }

        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.ContinueBuildPageWithParameters(dictionary);
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.ContinueBuildPageWithParameters(null);
        }
    }
}
