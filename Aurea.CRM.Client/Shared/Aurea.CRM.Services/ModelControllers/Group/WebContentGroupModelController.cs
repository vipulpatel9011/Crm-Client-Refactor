// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebContentGroupModelController.cs" company="Aurea Software Gmbh">
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
//   The WebContent Group Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Group
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Web;

    /// <summary>
    /// UPWebContentGroupModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Group.UPFieldControlBasedGroupModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    /// <seealso cref="Aurea.CRM.UIModel.Web.UPWebContentMetadataDelegate" />
    public class UPWebContentGroupModelController : UPFieldControlBasedGroupModelController, UPCRMLinkReaderDelegate, UPWebContentMetadataDelegate
    {
        private FormItem formItem;
        private FieldControl fieldControl;
        private UPCRMLinkReader linkReader;
        private UPMWebContentGroup tmpGroup;
        private UPWebContentMetadata webContentMetadata;
        private ViewReference viewReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentGroupModelController"/> class.
        /// </summary>
        /// <param name="_formItem">The form item.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentGroupModelController(FormItem _formItem, IIdentifier identifier, IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
            this.ExplicitTabIdentifier = identifier;
            this.ExplicitLabel = _formItem.Label;
            this.formItem = _formItem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentGroupModelController"/> class.
        /// </summary>
        /// <param name="_fieldControl">The field control.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentGroupModelController(FieldControl _fieldControl, int tabIndex, IGroupModelControllerDelegate theDelegate)
            : base(_fieldControl, tabIndex, theDelegate)
        {
            this.fieldControl = _fieldControl;
            this.ExplicitLabel = this.fieldControl.TabAtIndex(tabIndex).Label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPWebContentGroupModelController"/> class.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public UPWebContentGroupModelController(IGroupModelControllerDelegate theDelegate)
            : base(theDelegate)
        {
        }

        /// <summary>
        /// Applies the link record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="context">The context.</param>
        /// <param name="_viewReference">The view reference.</param>
        /// <returns></returns>
        public UPMGroup ApplyLinkRecordIdentification(string recordIdentification, Dictionary<string, object> context, ViewReference _viewReference)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (this.formItem != null)
            {
                string reportType = this.formItem.ViewReference.ContextValueForKey("ReportType");
                if (reportType == "ClientReport")
                {
                    Menu configMenu = configStore.MenuByName(this.formItem.ViewReference.ContextValueForKey("ConfigName"));
                    this.viewReference = this.formItem.ViewReference.ViewReferenceWith(context);
                    if (configMenu != null)
                    {
                        this.viewReference = configMenu.ViewReference.ViewReferenceWith(this.viewReference.ContextValueForKey("RecordId"));
                    }
                }
                else
                {
                    this.viewReference = this.formItem.ViewReference;
                }
            }
            else
            {
                if (_viewReference == null)
                {
                    Menu configMenu = configStore.MenuByName(this.TabConfig.Type.Substring("WEBCONTENT_".Length));
                    this.viewReference = configMenu.ViewReference.ViewReferenceWith(recordIdentification);
                }
                else
                {
                    this.viewReference = _viewReference;
                }
            }

            this.webContentMetadata = UPWebContentMetadata.WebContentMetaDataFromViewReference(this.viewReference, this);
            this.RecordIdentification = this.viewReference.ContextValueForKey("RecordId");
            if (this.ExplicitTabIdentifier == null)
            {
                this.ExplicitTabIdentifier = this.TabIdentifierForRecordIdentification(
                    !string.IsNullOrEmpty(this.RecordIdentification) ? this.RecordIdentification : recordIdentification);
            }

            this.tmpGroup = new UPMWebContentGroup(this.ExplicitTabIdentifier);
            string linkIdStr = this.viewReference.ContextValueForKey("LinkId");
            if (!string.IsNullOrEmpty(linkIdStr))
            {
                this.LinkId = Convert.ToInt32(linkIdStr);
            }
            else
            {
                this.LinkId = -1;
            }

            string parentLink = this.viewReference.ContextValueForKey("ParentLink");
            if (!string.IsNullOrEmpty(parentLink))
            {
                this.linkReader = new UPCRMLinkReader(this.RecordIdentification, parentLink, UPRequestOption.FastestAvailable, this);
                this.linkReader.Start();
                this.ControllerState = GroupModelControllerState.Pending;
                return null;
            }

            this.ContinueBuildGroup();
            return this.Group;
        }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Applies the result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPMGroup ApplyResultRow(UPCRMResultRow row)
        {
            base.ApplyResultRow(row);
            UPMGroup group = this.ApplyLinkRecordIdentification(row.RootRecordIdentification, null, null);
            group.Actions.AddRange(this.BuildAdditionalActionsForRecordIdentification(row.RootRecordIdentification));
            return group;
        }

        /// <summary>
        /// Applies the context.
        /// </summary>
        /// <param name="contextDictionary">The context dictionary.</param>
        /// <returns></returns>
        public override UPMGroup ApplyContext(Dictionary<string, object> contextDictionary)
        {
            base.ApplyContext(contextDictionary);
            return this.ApplyLinkRecordIdentification(null, contextDictionary, null);
        }

        /// <summary>
        /// Webs the content meta data finished with XML string.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="xmlString">The XML string.</param>
        public void WebContentMetaDataFinishedWithXmlString(UPWebContentMetadata clientReport, string xmlString)
        {
            this.ControllerState = GroupModelControllerState.Pending;
            UPMWebContentGroup webcontentGroup = (UPMWebContentGroup)this.Group;
            if (webcontentGroup != null)
            {
                webcontentGroup.WebContentHtml = xmlString;
            }

            this.ControllerState = GroupModelControllerState.Finished;
            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Webs the content meta data failed with error.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="error">The error.</param>
        public void WebContentMetaDataFailedWithError(UPWebContentMetadata clientReport, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Webs the content meta data finished with redirect URL.
        /// </summary>
        /// <param name="clientReport">The client report.</param>
        /// <param name="url">The URL.</param>
        public void WebContentMetaDataFinishedWithRedirectUrl(UPWebContentMetadata clientReport, Uri url)
        {
            this.ControllerState = GroupModelControllerState.Finished;
            UPMWebContentGroup webcontentGroup = (UPMWebContentGroup)this.Group;
            webcontentGroup.WebContentUrl = url;
            this.ControllerState = GroupModelControllerState.Finished;
            this.Delegate.GroupModelControllerFinished(this);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is online.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is online; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnline => false;

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader _linkReader, object result)
        {
            if (!string.IsNullOrEmpty(_linkReader.DestinationRecordIdentification))
            {
                this.RecordIdentification = _linkReader.DestinationRecordIdentification;
                this.ContinueBuildGroup();
                if (this.ControllerState != GroupModelControllerState.Pending)
                {
                    this.Delegate.GroupModelControllerFinished(this);
                }
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            this.ControllerState = GroupModelControllerState.Error;
            this.Delegate.GroupModelControllerFinished(this);
        }

        private void ContinueBuildGroup()
        {
            this.webContentMetadata.UpdateMetadataWithViewReference(this.viewReference);
            int recommendedHeight;
            if (this.formItem != null)
            {
                recommendedHeight = this.formItem.CellAttributes?.ContainsKey("Height") == true ? Convert.ToInt32(this.formItem.CellAttributes["Height"]) : 500;
            }
            else
            {
                recommendedHeight = Convert.ToInt32(this.viewReference.ContextValueForKey("Height"));
            }

            if (this.webContentMetadata is UPWebContentMetadataClientReport)
            {
                this.ControllerState = GroupModelControllerState.Pending;
                this.BuildClientReportForGroup(this.tmpGroup);
                this.tmpGroup.ReportType = "ClientReport";
                this.tmpGroup.LabelText = this.ExplicitLabel;
                this.tmpGroup.RecommendedHeight = recommendedHeight > 0 ? recommendedHeight : 500;
                this.Group = this.tmpGroup;
            }
            else if (this.webContentMetadata is UPWebContentMetadataServerReport)
            {
                this.tmpGroup.ReportType = "CoreReport";
                this.tmpGroup.LabelText = this.ExplicitLabel;
                if (!ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Report.EnableCoreReportInBrowser"))
                {
                    this.tmpGroup.DisableOpenInUrl = true;
                }

                UPWebContentMetadataServerReport serverReport = (UPWebContentMetadataServerReport)this.webContentMetadata;
                this.tmpGroup.WebContentUrl = serverReport.CalcServiceURLWithRecordIdentification(this.RecordIdentification);
                this.tmpGroup.RecommendedHeight = recommendedHeight > 0 ? recommendedHeight : 500;
                this.Group = this.tmpGroup;
                this.ControllerState = GroupModelControllerState.Finished;
            }
            else if (this.webContentMetadata is UPWebContentQlikViewUrl)
            {
                this.tmpGroup.ReportType = "QlikView";
                this.tmpGroup.LabelText = this.ExplicitLabel;
                this.tmpGroup.RecommendedHeight = recommendedHeight > 0 ? recommendedHeight : 500;
                this.tmpGroup.OpenInModalFrame = true;
                this.ControllerState = GroupModelControllerState.Pending;
                this.Group = this.tmpGroup;
            }
            else if (this.webContentMetadata is UPWebContentPortfolio)
            {
                this.tmpGroup.ReportType = "Portfolio";
                this.tmpGroup.LabelText = this.ExplicitLabel;
                this.tmpGroup.RecommendedHeight = recommendedHeight > 0 ? recommendedHeight : 500;
                this.tmpGroup.OpenInModalFrame = true;
                this.ControllerState = GroupModelControllerState.Pending;
                this.Group = this.tmpGroup;
            }
            else if (this.webContentMetadata is UPWebContentMetadataUrl)
            {
                UPWebContentMetadataUrl webContentMetaDataUrl = (UPWebContentMetadataUrl)this.webContentMetadata;
                this.tmpGroup.ReportType = "Html";
                this.tmpGroup.LabelText = this.ExplicitLabel;
                this.tmpGroup.WebContentUrl = webContentMetaDataUrl.Url;
                this.tmpGroup.LocalUrlUsed = webContentMetaDataUrl.LocalUrl;
                this.tmpGroup.RecommendedHeight = recommendedHeight > 0 ? recommendedHeight : 500;
                this.ControllerState = webContentMetaDataUrl.Finished ? GroupModelControllerState.Finished : GroupModelControllerState.Pending;

                this.Group = this.tmpGroup;
            }
            else
            {
                this.Group = this.tmpGroup;
                this.ControllerState = GroupModelControllerState.Empty;
            }
        }

        private void BuildClientReportForGroup(UPMWebContentGroup webcontentGroup)
        {
            ((UPWebContentMetadataClientReport)this.webContentMetadata).StartWithRecordIdentificationLinkIdParameters(this.RecordIdentification, this.LinkId, null);
        }
    }
}
