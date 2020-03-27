// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HistorySearchPageModelController.cs" company="Aurea Software Gmbh">
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
//   The UPHistorySearchPageModelController
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// UPHistorySearchPageModelController
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Search.UPStandardSearchPageModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPHistorySearchPageModelController : UPStandardSearchPageModelController
    {
        private List<UPMElement> rows;
        private List<string> pendingRecords;
        private int resCount;
        private HistoryManager historyManager;
        private IConfigurationUnitStore configStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPStandardSearchPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public UPHistorySearchPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Creates page instance
        /// </summary>
        /// <returns>
        ///   <see cref="UPMSearchPage" />
        /// </returns>
        public override UPMSearchPage CreatePageInstance()
        {
            List<IIdentifier> identifiers = new List<IIdentifier>();
            List<HistoryEntry> historyEntries = HistoryManager.DefaultHistoryManager.HistoryEntries;
            foreach (HistoryEntry entry in historyEntries)
            {
                identifiers.Add(new RecordIdentifier(entry.RecordIdentification));
            }

            MultipleIdentifier multipleIdentifier = new MultipleIdentifier(identifiers);
            return new UPMSearchPage(multipleIdentifier);
        }

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            UPMSearchPage page = this.CreatePageInstance();
            page.Invalid = true;
            this.TopLevelElement = page;
            this.rows = new List<UPMElement>();
            this.pendingRecords = new List<string>();
            this.BuildPageDetails(page);
        }

        /// <summary>
        /// Updates the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The <see cref="UPMElement" />.
        /// </returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            var page = element as UPMSearchPage;
            return page != null ? this.UpdatePage(page) : null;
        }

        private void BuildPageDetails(UPMSearchPage searchPage)
        {
            searchPage.HideSearchBar = true;
            searchPage.ViewType = SearchPageViewType.List;
            searchPage.RowAction = new UPMAction(StringIdentifier.IdentifierWithStringId("UPHistoryListViewRowAction"));
            searchPage.RowAction.SetTargetAction(this, this.SwitchToDetail);
        }

        private UPMSearchPage UpdatePage(UPMSearchPage searchPage)
        {
            UPMSearchPage newPage = this.CreatePageInstance();
            this.BuildPageDetails(newPage);
            if (searchPage.Invalid)
            {
                this.pendingRecords.Clear();
                this.rows.Clear();
                this.historyManager = HistoryManager.DefaultHistoryManager;
                this.configStore = ConfigurationUnitStore.DefaultStore;
                if (this.historyManager.HistoryEntries.Count == 0)
                {
                    return newPage;
                }

                foreach (HistoryEntry entry in this.historyManager.HistoryEntries)
                {
                    string recordIdent = entry.RecordIdentification;
                    SearchAndList searchConfiguration = this.configStore.SearchAndListByName(recordIdent.InfoAreaId());
                    FieldControl listFieldControl = this.configStore.FieldControlByNameFromGroup("List", searchConfiguration.FieldGroupName);
                    UPContainerMetaInfo containerMetaInfo = new UPContainerMetaInfo(listFieldControl);
                    if (entry.OnlineData)
                    {
                        if (ServerSession.CurrentSession.ConnectedToServer)
                        {
                            this.pendingRecords.Add(recordIdent);
                            this.resCount++;
                            containerMetaInfo.ReadRecord(recordIdent, UPRequestOption.Online, this);
                        }
                        else
                        {
                            UPMResultRow row = new UPMResultRow(new RecordIdentifier(recordIdent));
                            UPMStringField stringField = new UPMStringField(StringIdentifier.IdentifierWithStringId(recordIdent));
                            stringField.FieldValue = LocalizedString.TextOfflineNotAvailable;
                            row.Fields = new List<UPMField> { stringField };
                            row.OnlineData = true;
                            //row.StatusIndicatorIcon = UIImage.UpXXImageNamed("crmpad-List-Cloud");    // CRM-5007
                            InfoArea infoAreaConfig = this.configStore.InfoAreaConfigById(recordIdent.InfoAreaId());
                            string imageName = infoAreaConfig.ImageName;
                            if (!string.IsNullOrEmpty(imageName))
                            {
                                //row.Icon = UIImage.UpImageWithFileName(imageName);    // CRM-5007
                            }

                            this.rows.Add(row);
                        }
                    }
                    else
                    {
                        this.resCount++;
                        this.pendingRecords.Add(recordIdent);
                        containerMetaInfo.ReadRecord(recordIdent, UPRequestOption.Offline, this);
                    }
                }

                newPage.Invalid = false;
                this.TopLevelElement = newPage;
                this.ApplyLoadingStatusOnPage(newPage);
                return newPage;
            }

            UPMResultSection section = (UPMResultSection)this.Page.Children[0];
            this.ResortRowListAddToPage(section.Children, newPage);
            this.TopLevelElement = newPage;
            return newPage;
        }

        /// <summary>
        /// Search operation did fail with error
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="error">Error</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ReportError(error, true);
        }

        /// <summary>
        /// Search operation did finish with result
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="result">Result</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(0);
            this.resCount--;
            if (resultRow != null)
            {
                string recordIdent = resultRow.RootRecordIdentification;
                RecordIdentifier recordIdentifier = new RecordIdentifier(recordIdent.InfoAreaId(), recordIdent.RecordId());
                UPMResultRow row = new UPMResultRow(new RecordIdentifier(recordIdent));
                SearchAndList searchConfiguration = this.configStore.SearchAndListByName(recordIdent.InfoAreaId());
                FieldControl listFieldControl = this.configStore.FieldControlByNameFromGroup("List", searchConfiguration.FieldGroupName);
                InfoArea configInfoArea = this.configStore.InfoAreaConfigById(recordIdent.InfoAreaId());
                row.RowColor = AureaColor.ColorWithString(configInfoArea.ColorKey);
                UPCRMListFormatter listFormatter = new UPCRMListFormatter(listFieldControl);
                int fieldCount = listFormatter.PositionCount;
                List<UPMField> listFields = new List<UPMField>(fieldCount);
                for (int i = 0; i < fieldCount; i++)
                {
                    UPConfigFieldControlField configField = listFormatter.FirstFieldForPosition(i);
                    if (configField == null)
                    {
                        continue;
                    }

                    FieldAttributes attributes = configField.Attributes;
                    if (attributes.Hide)
                    {
                        continue;
                    }

                    UPMStringField stringField = new UPMStringField(StringIdentifier.IdentifierWithStringId($"{recordIdent}-{i}"));
                    string stringValue = listFormatter.StringFromRowForPosition(resultRow, i);
                    if (attributes.Image)
                    {
                        DocumentManager documentManager = new DocumentManager();
                        string documentKey = stringValue;
                        DocumentData documentData = documentManager.DocumentForKey(documentKey);
                        if (documentData != null)
                        {
                            row.RecordImageDocument = new UPMDocument(documentData);
                        }
                        else
                        {
                            //row.RecordImageDocument = new UPMDocument(recordIdentifier, ServerSession.DocumentRequestUrlForDocumentKey(documentKey));
                        }

                        continue;
                    }

                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        if (attributes.NoLabel && !string.IsNullOrEmpty(configField.Label))
                        {
                            stringValue = $"{configField.Label} {stringValue}";
                        }

                        stringField.StringValue = stringValue;
                        stringField.SetAttributes(attributes);
                    }

                    listFields.Add(stringField);
                }

                row.OnlineData = resultRow.IsServerResponse;
                if (row.OnlineData)
                {
                    //row.StatusIndicatorIcon = UIImage.UpXXImageNamed("crmpad-List-Cloud");    // CRM-5007
                }

                foreach (HistoryEntry entry in this.historyManager.HistoryEntries)
                {
                    if (entry.RecordIdentification == recordIdent)
                    {
                        this.rows.Add(row);
                        if (this.pendingRecords.Contains(recordIdent))
                        {
                            this.pendingRecords.Remove(recordIdent);
                        }

                        break;
                    }
                }

                row.Fields = listFields;
            }

            if (this.resCount == 0)
            {
                this.DisplayPage(this.rows);
            }
        }

        /// <summary>
        /// Details the organizer for result row.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public override UPOrganizerModelController DetailOrganizerForResultRow(UPMResultRow resultRow)
        {
            RecordIdentifier identifier = (RecordIdentifier)resultRow.Identifier;
            ViewReference viewReference = HistoryManager.DefaultHistoryManager.ViewReferenceForRecordIdentifier(identifier.RecordIdentification);
            UPOrganizerModelController organizerController = UPOrganizerModelController.OrganizerFromViewReference(viewReference);
            if (resultRow.OnlineData)
            {
                organizerController.OnlineData = resultRow.OnlineData;
            }

            resultRow.DataValid = true;
            resultRow.Invalid = true;
            UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { resultRow.Identifier });
            return organizerController;
        }

        private void DisplayPage(List<UPMElement> _rows)
        {
            UPMSearchPage newPage = this.CreatePageInstance();
            this.BuildPageDetails(newPage);
            this.ResortRowListAddToPage(_rows, newPage);
            newPage.Invalid = false;
            Page oldPage = this.Page;
            this.TopLevelElement = newPage;
            this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
        }

        private void ResortRowListAddToPage(List<UPMElement> _rows, UPMSearchPage newPage)
        {
            var validRows = _rows.Where(x => x != null).ToList();

            validRows.Sort((object1, object2) =>
            {
                RecordIdentifier id1 = (RecordIdentifier)((UPMResultRow)object1).Identifier;
                RecordIdentifier id2 = (RecordIdentifier)((UPMResultRow)object2).Identifier;
                HistoryEntry entry1 = HistoryManager.DefaultHistoryManager.EntryForRecordIdentifier(id1.RecordIdentification);
                HistoryEntry entry2 = HistoryManager.DefaultHistoryManager.EntryForRecordIdentifier(id2.RecordIdentification);
                return DateTime.Compare(entry2.LastCall, entry1.LastCall);
            });

            UPMResultSection section = new UPMResultSection(StringIdentifier.IdentifierWithStringId("UPHistoryDefaultSection"));
            foreach (UPMElement element in validRows)
            {
                if (element != null)
                {
                    element.Invalid = false;
                    section.AddChild(element);
                }
            }

            newPage.AddResultSection(section);
        }
    }
}
