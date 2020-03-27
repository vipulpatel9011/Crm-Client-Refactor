// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetailPageModelController.cs" company="Aurea Software Gmbh">
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
//   The page model controller for details view
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.Networking;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// The page model controller for details view
    /// </summary>
    /// <seealso cref="GroupBasedPageModelController" />
    /// <seealso cref="ISearchOperationHandler" />
    /// <seealso cref="IAlternateExpandCheckerDelegate" />
    public class DetailPageModelController : GroupBasedPageModelController, IAlternateExpandCheckerDelegate,
        IRemoteTableCaptionDelegate
    {
        /// <summary>
        /// The record id.
        /// </summary>
        protected string recordId;

        /// <summary>
        /// The action identifier to view reference dictionary.
        /// </summary>
        protected Dictionary<string, object> actionIdentifierToViewReferenceDictionary;

        /// <summary>
        /// The details control.
        /// </summary>
        protected FieldControl detailsControl;

        protected UPCRMAlternateExpandChecker alternateExpandChecker;

        /// <summary>
        /// The _send update message.
        /// </summary>
        protected bool _sendUpdateMessage;

        /// <summary>
        /// The refresh organizer.
        /// </summary>
        protected bool refreshOrganizer;

        /// <summary>
        /// The config name.
        /// </summary>
        private string configName;

        /// <summary>
        /// The info area id.
        /// </summary>
        private string infoAreaId;

        private string title;

        private string detail;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        public DetailPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">
        /// The view reference.
        /// </param>
        /// <param name="testDelegate">
        /// The test delegate.
        /// </param>
        public DetailPageModelController(
            ViewReference viewReference,
            IDetailPageModelControllerTestDelegate testDelegate)
            : this(viewReference)
        {
            this.TestDelegate = testDelegate;
        }

        /// <summary>
        /// Gets the link record identification.
        /// i know these are the same for DetailPage and SearchPage, but not for ALL pages!
        /// </summary>
        /// <value>
        /// The link record identification.
        /// </value>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets the expand configuration.
        /// </summary>
        /// <value>
        /// The expand configuration.
        /// </value>
        public UPConfigExpand ExpandConfig { get; private set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets the name of the expand.
        /// </summary>
        /// <value>
        /// The name of the expand.
        /// </value>
        public string ExpandName { get; private set; }

        /// <summary>
        /// Gets the test delegate.
        /// </summary>
        /// <value>
        /// The test delegate.
        /// </value>
        public IDetailPageModelControllerTestDelegate TestDelegate { get; private set; }

        /// <summary>
        /// Initialize instance
        /// </summary>
        public void Init()
        {
            // For the current release the default behavior disregard RequestOption value
            // is UPRequestOption.BestAvailable.
            // this.RequestOption =
            // UPCRMDataStore.RequestOptionFromString(
            // this.ViewReference.ContextValueForKey("RequestOption"),
            // UPRequestOption.FastestAvailable);

            // To restore the original functionality uncomment the code above and remove the following lines until END CHANGES:
            var isOnline = !ServerSession.CurrentSession.UserChoiceOffline;
            if (isOnline && SimpleIoc.Default.GetInstance<IConnectionWatchdog>() != null)
            {
                isOnline = SimpleIoc.Default.GetInstance<IConnectionWatchdog>().HasInternetConnection;
            }

            if (isOnline)
            {
                this.RequestOption = UPRequestOption.BestAvailable;
            }
            else
            {
                this.RequestOption = UPRequestOption.Offline;
            }

            // END CHANGES
            this.BuildPage();
        }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        public override Page InstantiatePage()
        {
            return this.Page != null
                ? new MDetailPage(this.Page.Identifier)
                : new MDetailPage(
                    FieldIdentifier.IdentifierWithRecordIdentificationFieldId(
                        this.infoAreaId.InfoAreaIdRecordId(this.recordId),
                        "Page0"));
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            this.recordId = this.ViewReference.ContextValueForKey("RecordId");
            this.recordId = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(this.recordId);
            this.infoAreaId = this.ViewReference.ContextValueForKey("VirtualInfoAreaId");
            if (string.IsNullOrWhiteSpace(this.infoAreaId))
            {
                this.infoAreaId = this.ViewReference.ContextValueForKey("InfoAreaId");
            }

            this.infoAreaId = GetInfoArea(this.infoAreaId, this.recordId);
            this.configName = this.ViewReference.ContextValueForKey("ConfigName");
            this.ExpandName = this.ViewReference.ContextValueForKey("CurrentExpandName");
            this.LinkRecordIdentification = this.ViewReference.ContextValueForKey("LinkRecordId");

            var linkIdObj = this.ViewReference.ContextValueForKey("LinkId");
            this.LinkId = linkIdObj != null ? int.Parse(linkIdObj) : -1;

            if (this.recordId.IsRecordIdentification())
            {
                if (string.IsNullOrWhiteSpace(this.infoAreaId))
                {
                    this.infoAreaId = this.recordId.InfoAreaId();
                }

                this.recordId = this.recordId.RecordId();
            }

            if (string.IsNullOrWhiteSpace(this.configName))
            {
                this.configName = this.infoAreaId;
            }

            var page = (MDetailPage)this.InstantiatePage();
            page.LabelText = LocalizedString.TextTabOverview;
            page.Invalid = true;
            this.TopLevelElement = page;
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="oldDetailPage">
        /// The old detail page.
        /// </param>
        /// <returns>
        /// The <see cref="UPMElement"/>.
        /// </returns>
        public override UPMElement UpdatedElementForPage(Page oldDetailPage)
        {
            var detailPage = (MDetailPage)this.InstantiatePage();
            var configStore = ConfigurationUnitStore.DefaultStore;
            detailPage.Invalid = true;
            detailPage.LabelText = oldDetailPage.LabelText;
            this.TopLevelElement = detailPage;
            this.ApplyLoadingStatusOnPage(detailPage);
            if (!string.IsNullOrWhiteSpace(this.ExpandName))
            {
                this.ExpandConfig = configStore.ExpandByName(this.ExpandName);
                if (this.TestDelegate == null)
                {
                    this.InformAboutDidChangeTopLevelElement(oldDetailPage, detailPage, null, null);
                }

                this.ContinueWithExpandConfig();
                this.PageLoadingFinished();
                return this.TopLevelElement as Page;
            }

            this.ExpandConfig = configStore.ExpandByName(this.configName);
            this.alternateExpandChecker =
                new UPCRMAlternateExpandChecker(this.infoAreaId.RecordId(), this.ExpandConfig, this);
            this._sendUpdateMessage = false;
            this.alternateExpandChecker.Start(this.RequestOption);
            this._sendUpdateMessage = true;

            return this.TopLevelElement as Page;
        }

        /// <summary>
        /// Continues the with expand configuration.
        /// </summary>
        private void ContinueWithExpandConfig()
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            this.ProcessDetailsControl(configStore);
            if (this.detailsControl == null)
            {
                return;
            }

            var result = this.GetResultFromDetailsControl(configStore);

            if (this.Page.Invalid)
            {
                return;
            }

            this.Page.Status = null;
            if (this._sendUpdateMessage)
            {
                this.SignalModelControllerDelegate();
            }
        }

        private void ApplySubTitleFromResultRow(UPCRMResultRow resultRow)
        {
            Dictionary<string, UPConfigFieldControlField> fieldMapping = this.detailsControl.FunctionNames();
            UPConfigFieldControlField fieldControl = fieldMapping.ValueOrDefault("OrganizerHeaderSubLabel");
            if (fieldControl != null)
            {
                ((DetailOrganizerModelController)this.ParentOrganizerModelController)
                    .SetRecordTitleForOrganizerHeader(
                        resultRow.FormattedFieldValueAtIndex(fieldControl.TabIndependentFieldIndex, null,
                            this.detailsControl));
            }
        }

        /// <summary>
        /// Applies the image from page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        public void ApplyImageFromPage(MDetailPage page)
        {
            if (this.ParentOrganizerModelController?.Organizer.ImageDocument != null)
            {
                return;
            }

            var vCount = page?.Groups?.Count ?? 0;
            if (vCount <= 0)
            {
                return;
            }

            for (var i = 0; i < vCount; i++)
            {
                var group = page?.Groups?[i] as UPMStandardGroup;

                if (group?.ImageDocument != null)
                {
                    (this.ParentOrganizerModelController as DetailOrganizerModelController)
                        ?.SetImageForOrganizerHeader(group.ImageDocument);
                    return;
                }
            }
        }

        /// <summary>
        /// Pages the loading finished.
        /// </summary>
        public override void PageLoadingFinished()
        {
            if (this.TestDelegate != null)
            {
                this.TestDelegate.ModelControllerDidFinishWithSuccess(this, (MDetailPage)this.TopLevelElement);
                return;
            }

            base.PageLoadingFinished();

            this.ApplyImageFromPage((MDetailPage)this.TopLevelElement);
        }

        /// <summary>
        /// Fills the page with result row.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="resultRow">
        /// The result row.
        /// </param>
        /// <param name="requestOption">
        /// The request option.
        /// </param>
        public override void FillPageWithResultRow(Page page, UPCRMResultRow resultRow, UPRequestOption requestOption)
        {
            this.Page.Status = null;

            base.FillPageWithResultRow(page, resultRow, requestOption);
        }

        /// <summary>
        /// Signals the model controller delegate.
        /// </summary>
        public void SignalModelControllerDelegate()
        {
            this.InformAboutDidChangeTopLevelElement(this.Page, this.Page, null, null);
        }

        /// <summary>
        /// Updates the page with result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public void UpdatePageWithResult(UPCRMResult result)
        {
            if (result == null)
            {
                return;
            }

            var oldPage = this.Page;
            var newPage = (MDetailPage)this.InstantiatePage();
            newPage.LabelText = oldPage.LabelText;
            newPage.Invalid = false;
            newPage.Status = null;
            if (result.RowCount == 0)
            {
                if (this.RequestOption == UPRequestOption.BestAvailable)
                {
                    var containerMetaInfo = new UPContainerMetaInfo(this.detailsControl)
                    {
                        ReplaceCaseSensitiveCharacters =
                            ConfigurationUnitStore.DefaultStore.ConfigValueIsSet(
                                "Search.ReplaceCaseSensitiveCharacters")
                    };

                    if (string.IsNullOrWhiteSpace(this.LinkRecordIdentification))
                    {
                        result = containerMetaInfo.ReadRecord(this.infoAreaId.InfoAreaIdRecordId(this.recordId));
                    }
                    else
                    {
                        containerMetaInfo.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
                        result = containerMetaInfo.Find();
                    }

                    if (result.RowCount == 0)
                    {
                        var errorStatus = UPMErrorStatus.ErrorStatusWithMessageDetails("Loading error", null);
                        newPage.Status = errorStatus;
                        newPage.Invalid = true;
                        this.TopLevelElement = newPage;
                        this.InformAboutDidFailTopLevelElement(this.Page);
                        return;
                    }
                }
                else
                {
                    var errorStatus = UPMErrorStatus.ErrorStatusWithMessageDetails("Loading error", null);
                    newPage.Status = errorStatus;
                    newPage.Invalid = true;
                    this.TopLevelElement = newPage;
                    this.InformAboutDidFailTopLevelElement(this.Page);
                    return;
                }
            }

            var resultRow = result.ResultRowAtIndex(0) as UPCRMResultRow;
            this.FillPageWithResultRow(
                newPage,
                resultRow,
                resultRow.IsServerResponse ? UPRequestOption.Online : UPRequestOption.Offline);
            this.ApplySubTitleFromResultRow(resultRow);
            this.TopLevelElement = newPage;
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.UpdatePageWithResult(result);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="results">
        /// The results.
        /// </param>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.RequestOption == UPRequestOption.BestAvailable && error.IsConnectionOfflineError())
            {
                var containerMetaInfo = new UPContainerMetaInfo(this.detailsControl);
                var result = containerMetaInfo.ReadRecord(recordId);

                if (result.RowCount > 0)
                {
                    this.UpdatePageWithResult(result);
                }
                else
                {
                    this.ShowError(error);
                }
            }
            else
            {
                this.ShowReadError(error);
            }
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public override void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="counts">
        /// The counts.
        /// </param>
        public override void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a test detail page.
        /// </summary>
        public void TestCreateDetailPage()
        {
            this.UpdatedElementForPage(null);
        }

        /// <summary>
        /// Handles the page error details.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        /// <returns></returns>
        public override bool HandlePageErrorDetails(string message, string details)
        {
            if (this.TestDelegate != null)
            {
                this.TestDelegate.ModelControllerDidFailWithError(this, new Exception(message));
                return true;
            }

            return base.HandlePageErrorDetails(message, details);
        }

        /// <summary>
        /// Informs the about did fail top level element.
        /// </summary>
        /// <param name="failedTopLevelElement">The failed top level element.</param>
        public override void InformAboutDidFailTopLevelElement(ITopLevelElement failedTopLevelElement)
        {
            if (this.TestDelegate != null)
            {
                this.TestDelegate.ModelControllerDidFailWithError(this, new Exception("error"));
            }
            else
            {
                base.InformAboutDidFailTopLevelElement(failedTopLevelElement);
            }
        }

        /// <summary>
        /// Informs about change of top level element.
        /// </summary>
        /// <param name="oldTopLevelElement">The old top level element.</param>
        /// <param name="newTopLevelElement">The new top level element.</param>
        /// <param name="changedIdentifiers">The changed identifiers.</param>
        /// <param name="changeHints">The change hints.</param>
        public override void InformAboutDidChangeTopLevelElement(ITopLevelElement oldTopLevelElement,
            ITopLevelElement newTopLevelElement, List<IIdentifier> changedIdentifiers, UPChangeHints changeHints)
        {
            if (this.TestDelegate != null)
            {
                this.TestDelegate.ModelControllerDidFinishWithSuccess(this, (MDetailPage)newTopLevelElement);
            }
            else
            {
                base.InformAboutDidChangeTopLevelElement(oldTopLevelElement, newTopLevelElement, changedIdentifiers,
                    changeHints);
            }
        }

        /// <summary>
        /// The alternate expand checker did finish with result.
        /// </summary>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="expand">The expand.</param>
        public void AlternateExpandCheckerDidFinishWithResult(UPCRMAlternateExpandChecker expandChecker,
            UPConfigExpand expand)
        {
            this.ExpandConfig = expand;
            this.ContinueWithExpandConfig();
        }

        /// <summary>
        /// The alternate expand checker did fail with error.
        /// </summary>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="error">The error.</param>
        public void AlternateExpandCheckerDidFailWithError(UPCRMAlternateExpandChecker expandChecker, Exception error)
        {
            this.HandlePageErrorDetails(error.Message, error.StackTrace);
        }

        /// <summary>
        /// The table caption did fail with error.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="error">The error.</param>
        public void TableCaptionDidFailWithError(UPConfigTableCaption tableCaption, Exception error)
        {
            Messenger.Default.Send(HeaderBarMessage.HeaderBarTitleUpdate(string.Empty));
        }

        /// <summary>
        /// The table caption did finish with result.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="tableCaptionString">The table caption string.</param>
        public void TableCaptionDidFinishWithResult(UPConfigTableCaption tableCaption, string tableCaptionString)
        {
            Messenger.Default.Send(HeaderBarMessage.HeaderBarTitleUpdate(tableCaptionString));
        }

        private static string GetInfoArea(string infoAreaId, string recordId)
        {
            if (recordId == null)
            {
                return infoAreaId;
            }

            var recordInfoArea = recordId?.Split('.')[0];
            return recordInfoArea != infoAreaId ? recordInfoArea : infoAreaId;
        }

        private void ShowError(Exception exception)
        {
            this.HandlePageErrorDetails(exception.Message, exception.ToString());

            if (this.Page.Status != null)
            {
                this.Page.Status = null;
                this.InformAboutDidChangeTopLevelElement(null, null, null, null);
            }

            Messenger.Default.Send(new ToastrMessage
            {
                MessageText = exception.Message,
                DetailedMessage = $"{exception.InnerException.Message}"
            });
        }

        private void ShowReadError(Exception error)
        {
            var details = error.Message;
            if (details.Contains(@"NOTFOUND"))
            {
                this.ShowError(
                    new KeyNotFoundException(
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupErrors,
                            LocalizationKeys.KeyErrorsRecordNotFound), error));
            }
            else if (error.IsConnectionOfflineError())
            {
                this.ShowError(
                    new InvalidOperationException(
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupErrors,
                            LocalizationKeys.KeyBasicOfflineNotAvailable), error));
            }
            else
            {
                this.ShowError(
                    new Exception(
                        LocalizedString.Localize(
                            LocalizationKeys.TextGroupErrors,
                            LocalizationKeys.KeyErrorsGeneralServerError), error));
            }
        }

        private UPCRMResult GetResultFromDetailsControl(IConfigurationUnitStore configStore)
        {
            UPCRMResult result = null;
            if (this.detailsControl.InfoAreaId == this.infoAreaId)
            {
                result = this.GetResultFromContainerMetaInfo(configStore);
            }
            else
            {
                // control show children
                result = new UPCRMResult(StringExtensions.InfoAreaIdRecordId(this.infoAreaId, this.recordId));
                var resultRow = result.ResultRowAtIndex(0) as UPCRMResultRow;
                this.ApplySubTitleFromResultRow(resultRow);

                this.FillPageWithResultRow(
                    this.Page,
                    resultRow,
                    (resultRow?.IsServerResponse ?? true) ? UPRequestOption.BestAvailable : UPRequestOption.Offline);
            }

            return result;
        }

        private UPCRMResult GetResultFromContainerMetaInfo(IConfigurationUnitStore configStore)
        {
            UPCRMResult result = null;

            var containerMetaInfo = new UPContainerMetaInfo(this.detailsControl)
            {
                ReplaceCaseSensitiveCharacters =
                        configStore.ConfigValueIsSet("Search.ReplaceCaseSensitiveCharacters")
            };

            // read record synchronously offline
            if (this.RequestOption == UPRequestOption.Default
                || this.RequestOption == UPRequestOption.FastestAvailable
                || this.RequestOption == UPRequestOption.Offline)
            {
                if (string.IsNullOrWhiteSpace(this.LinkRecordIdentification))
                {
                    result = containerMetaInfo.ReadRecord(
                        StringExtensions.InfoAreaIdRecordId(this.infoAreaId, this.recordId));
                }
                else
                {
                    containerMetaInfo.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
                    containerMetaInfo.DisableVirtualLinks = true;
                    result = containerMetaInfo.Find();
                    if (result.RowCount >= 1)
                    {
                        this.recordId = result.ResultRowAtIndex(0).RootRecordId;
                    }
                }
            }

            if (result == null || result.RowCount < 1)
            {
                this.UpdatePage(containerMetaInfo);
            }
            else
            {
                var resultRow = result.ResultRowAtIndex(0) as UPCRMResultRow;
                this.ApplySubTitleFromResultRow(resultRow);
                this.FillPageWithResultRow(
                    this.Page,
                    resultRow,
                    (resultRow?.IsServerResponse ?? true) ? UPRequestOption.Online : UPRequestOption.Offline);
            }

            return result;
        }

        private void UpdatePage(UPContainerMetaInfo containerMetaInfo)
        {
            if (this.RequestOption != UPRequestOption.Offline)
            {
                // read record asynchronously online
                Operation operation;
                if (string.IsNullOrWhiteSpace(this.LinkRecordIdentification))
                {
                    operation = containerMetaInfo.ReadRecord(this.recordId, UPRequestOption.Online, this);
                }
                else
                {
                    containerMetaInfo.DisableVirtualLinks = true;
                    containerMetaInfo.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
                    operation = containerMetaInfo.Find(UPRequestOption.Online, this);
                }

                if (operation == null)
                {
                    this.UpdatePageWithResult(null);
                }
            }
            else
            {
                this.UpdatePageWithResult(null);
            }
        }

        private void ProcessDetailsControl(IConfigurationUnitStore configStore)
        {
            this.detailsControl = configStore.FieldControlByNameFromGroup("Details", this.ExpandConfig?.FieldGroupName);

            if (this.ExpandConfig == null)
            {
                this.ExpandConfig = configStore.ExpandByName(this.configName);
            }

            int tabCount = this.detailsControl?.NumberOfTabs ?? 0;
            this.ClearGroupControllers();

            for (int i = 0; i < tabCount; i++)
            {
                var groupModelController =
                    UPGroupModelController.DetailsGroupModelController(this.detailsControl, i, this);
                if (groupModelController != null)
                {
                    this.GroupModelControllerArray.Add(groupModelController);
                }
            }
        }
    }
}
