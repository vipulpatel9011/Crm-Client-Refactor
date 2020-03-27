// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DetailOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The page model controller for details view
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Documents;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using Inbox;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// The detail Organizer model controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.IRemoteTableCaptionDelegate" />
    /// <seealso cref="IFavoriteModelControllerDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.IAlternateExpandCheckerDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMTilesDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    public class DetailOrganizerModelController : UPOrganizerModelController, IRemoteTableCaptionDelegate, IFavoriteModelControllerDelegate,
        IAlternateExpandCheckerDelegate, UPCRMLinkReaderDelegate, UPCRMTilesDelegate, UPCopyFieldsDelegate
    {
        private const string ActionSyncRecord = "action.syncRecord";
        private const string ButtonSyncRecordText = "Button:syncRecord";
        private const string HeaderNameExpand = "Expand";
        private const string HintAlternateExpandFound = "AlternateExpandFound";
        private const string TileIdentifier = "Tile";
        private const string TileTextIdentifier = "TileText";
        private const string ValueTextIdentifier = "ValueText";

        /// <summary>
        /// The favorite model controller
        /// </summary>
        protected UPFavoriteModelController FavoriteModelController;

        /// <summary>
        /// The alternate expand checker
        /// </summary>
        protected UPCRMAlternateExpandChecker AlternateExpandChecker;

        /// <summary>
        /// The link reader
        /// </summary>
        protected UPCRMLinkReader LinkReader;

        /// <summary>
        /// The tiles
        /// </summary>
        protected UPCRMTiles Tiles;

        /// <summary>
        /// The tiles name
        /// </summary>
        protected string TilesName;

        /// <summary>
        /// The copy fields
        /// </summary>
        protected UPCopyFields CopyFields;

        /// <summary>
        /// The refresh organizer
        /// </summary>
        protected bool RefreshOrganizer;

        /// <summary>
        /// Gets or sets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; set; }

        /// <summary>
        /// Gets or sets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; set; }

        /// <summary>
        /// Gets or sets the name of the configuration.
        /// </summary>
        /// <value>
        /// The name of the configuration.
        /// </value>
        public string ConfigName { get; set; }

        /// <summary>
        /// Gets the expand configuration.
        /// </summary>
        /// <value>
        /// The expand configuration.
        /// </value>
        public UPConfigExpand ExpandConfig { get; private set; }

        /// <summary>
        /// Gets or sets the name of the alternate configuration.
        /// </summary>
        /// <value>
        /// The name of the alternate configuration.
        /// </value>
        public string AlternateConfigName { get; set; }

        /// <summary>
        /// Gets or sets the virtual information area identifier.
        /// </summary>
        /// <value>
        /// The virtual information area identifier.
        /// </value>
        public string VirtualInfoAreaId { get; set; }

        /// <summary>
        /// Gets or sets the parameter dictionary.
        /// </summary>
        /// <value>
        /// The parameter dictionary.
        /// </value>
        public Dictionary<string, object> ParameterDictionary { get; set; }

        /// <summary>
        /// Gets the detail model controller delegate.
        /// </summary>
        /// <value>
        /// The detail model controller delegate.
        /// </value>
        public IDetailModelControllerUIDelegate DetailModelControllerDelegate => (IDetailModelControllerUIDelegate)this.ModelControllerDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="options">The options.</param>
        public DetailOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions options = null)
            : base(viewReference, options)
        {
        }

        /// <summary>
        /// Initialize instance
        /// </summary>
        public void Init()
        {
            this.FavoriteModelController = new UPFavoriteModelController();
            this.FavoriteModelController.TheDelegate = this;

            // For the current release the default behavior disregard OrganizerRefresh value is
            // RefreshOrganizer set to true, to restore the original functionality uncoment the
            // following line and remove this.RefreshOrganizer = true;
            // this.RefreshOrganizer = this.ViewReference.ContextValueIsSet("OrganizerRefresh");
            this.RefreshOrganizer = true;
            this.BuildDetailOrganizerPages();
        }

        private void RefreshTableCaption()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            var crmStore = UPCRMDataStore.DefaultStore;
            string tableCaptionName = this.ExpandConfig.TableCaptionName;
            if (string.IsNullOrEmpty(tableCaptionName))
            {
                tableCaptionName = this.InfoAreaId;
            }

            UPConfigTableCaption tableCaption = configStore.TableCaptionByName(tableCaptionName);
            string recordTableCaption = tableCaption?.TableCaptionForRecordIdentification(this.RecordIdentification);

            if (string.IsNullOrEmpty(recordTableCaption) && !crmStore.RecordExistsOffline(this.RecordIdentification))
            {
                this.Organizer.TitleText = configStore.BasicTextByIndex(4);
                tableCaption?.RequestTableCaptionForRecordIdentification(this.RecordIdentification, this);
            }
            else
            {
                this.Organizer.TitleText = recordTableCaption;
            }

            string subTitleText = null;
            if (!string.IsNullOrEmpty(this.ExpandConfig.HeaderGroupName))
            {
                UPConfigHeader header = configStore.HeaderByNameFromGroup("Expand", this.ExpandConfig.HeaderGroupName);
                subTitleText = header?.Label;
            }

            if (string.IsNullOrEmpty(subTitleText))
            {
                InfoArea infoAreaConfig = configStore.InfoAreaConfigById(this.InfoAreaId);
                subTitleText = infoAreaConfig.SingularName;
                if (string.IsNullOrEmpty(subTitleText))
                {
                    subTitleText = crmStore.TableInfoForInfoArea(this.InfoAreaId).Label;
                }
            }

            this.Organizer.SubtitleText = subTitleText;
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("TableCaptionChanged"));
        }

        /// <summary>
        /// Builds the detail organizer pages.
        /// </summary>
        protected virtual void BuildDetailOrganizerPages()
        {
            string recordId = this.ViewReference.ContextValueForKey("RecordId");
            recordId = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(recordId);
            this.InfoAreaId = this.ViewReference.ContextValueForKey("InfoAreaId");

            UPMOrganizer detailOrganizer;
            if (recordId.IsRecordIdentification() && this.RefreshOrganizer)
            {
                detailOrganizer = new UPMOrganizer(new RecordIdentifier(recordId));
            }
            else
            {
                detailOrganizer = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Details"));
            }

            this.TopLevelElement = detailOrganizer;
            detailOrganizer.DisplaysTitleText = true;
            detailOrganizer.ExpandFound = false;
            string linkIdString = this.ViewReference.ContextValueForKey("LinkId");
            if (!string.IsNullOrEmpty(this.InfoAreaId))
            {
                var parts = this.InfoAreaId.Split(':');
                if (parts.Length > 1)
                {
                    this.InfoAreaId = parts[0];
                    linkIdString = parts[1];
                }
            }

            if (recordId.IsRecordIdentification())
            {
                this.RecordIdentification = recordId;
                if (string.IsNullOrEmpty(this.InfoAreaId))
                {
                    this.InfoAreaId = recordId.InfoAreaId();
                }
            }
            else
            {
                this.RecordIdentification = this.InfoAreaId.InfoAreaIdRecordId(recordId);
            }

            string linkRecordId = this.ViewReference.ContextValueForKey("LinkRecordId");
            if (!string.IsNullOrEmpty(linkRecordId))
            {
                string parentLinkString = this.InfoAreaId;
                if (!string.IsNullOrEmpty(linkIdString))
                {
                    parentLinkString = $"{parentLinkString}:{linkIdString}";
                }

                this.LinkReader = new UPCRMLinkReader(linkRecordId, parentLinkString, UPRequestOption.FastestAvailable, this);
                this.LinkReader.Start();
            }
            else
            {
                this.ContinueBuildDetailOrganizerPages0();
            }
        }

        private void ContinueBuildDetailOrganizerPages0()
        {
            this.ConfigName = this.ViewReference.ContextValueForKey("ConfigName");
            bool configNameHasWildCards = !string.IsNullOrEmpty(this.ConfigName) && this.ConfigName.Contains("##");

            if (string.IsNullOrEmpty(this.ConfigName) || configNameHasWildCards)
            {
                this.VirtualInfoAreaId = UPCRMDataStore.DefaultStore.VirtualInfoAreaIdForRecordIdentification(this.RecordIdentification);
                if (!string.IsNullOrEmpty(this.VirtualInfoAreaId))
                {
                    if (!configNameHasWildCards)
                    {
                        this.ConfigName = this.VirtualInfoAreaId;
                        if (this.VirtualInfoAreaId != this.InfoAreaId)
                        {
                            this.AlternateConfigName = this.InfoAreaId;
                        }
                    }
                }
            }
            else
            {
                this.VirtualInfoAreaId = this.InfoAreaId;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (configNameHasWildCards)
            {
                this.ExpandConfig = configStore.ExpandByName(this.ConfigName.Replace("##", this.VirtualInfoAreaId));
                if (this.ExpandConfig == null && this.VirtualInfoAreaId != this.InfoAreaId)
                {
                    this.ExpandConfig = configStore.ExpandByName(this.ConfigName.Replace("##", this.InfoAreaId));
                }

                if (this.ExpandConfig == null)
                {
                    this.ExpandConfig = configStore.ExpandByName(this.VirtualInfoAreaId);
                }

                if (this.ExpandConfig == null)
                {
                    this.ExpandConfig = configStore.ExpandByName(this.InfoAreaId);
                }
            }
            else
            {
                this.ExpandConfig = configStore.ExpandByName(this.ConfigName);
                if (this.ExpandConfig == null && !string.IsNullOrEmpty(this.AlternateConfigName))
                {
                    this.ExpandConfig = configStore.ExpandByName(this.AlternateConfigName);
                }
            }

            if (this.ExpandConfig == null)
            {
                SimpleIoc.Default.GetInstance<ILogger>()?.
                    LogError($"Detail Organizer: Detail expand configuration is not present for the Info Area {this.InfoAreaId}");
                return;
            }

            this.Organizer.OrganizerColor = this.ColorForInfoAreaWithId(this.InfoAreaId);
            if (this.ExpandConfig.AlternateExpands != null)
            {
                this.AlternateExpandChecker = new UPCRMAlternateExpandChecker(this.RecordIdentification, this.ExpandConfig, this);
                this.AlternateExpandChecker.Start(UPRequestOption.FastestAvailable);
                this.ApplyLoadingStatusOnOrganizer(this.Organizer);
            }
            else
            {
                this.AlternateExpandChecker = null;
                this.ContinueBuildDetailsOrganizerPage();
                this.AddToHistory(this.RecordIdentification, this.ViewReference, this.ExpandConfig.ImageName);
            }
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
            if (element is UPMOrganizer)
            {
                this.AlternateExpandChecker = null;
                this.LinkReader = null;
                this.Tiles = null;
                this.TilesName = null;
                this.CopyFields = null;
                this.PageModelControllers.Clear();
                List<UPMElement> newActions = null;

                foreach (UPMOrganizerAction action in this.LeftNavigationBarItems)
                {
                    if (action.Identifier.IdentifierAsString == "CloseOrganizerAction")
                    {
                        newActions = new List<UPMElement> { action };
                    }
                }

                if (newActions != null)
                {
                    this.LeftNavigationBarItems = newActions;
                }
                else
                {
                    this.LeftNavigationBarItems.Clear();
                }

                this.RightNaviagtionBarItems.Clear();
                this.OrganizerHeaderQuickActionItems?.Clear();
                this.OrganizerHeaderActionItems.Clear();
                this.SaveActionItems.Clear();
                this.BuildDetailOrganizerPages();
            }

            return null;
        }

        private void ApplyLoadingStatusOnOrganizer(UPMOrganizer organizer)
        {
            UPMProgressStatus progressStatus = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("loadingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextLoadingData;
            progressStatus.StatusMessageField = statusField;
            organizer.Status = progressStatus;
        }

        private void ContinueBuildDetailsOrganizerPage()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            FieldControl fieldControl = configStore.FieldControlByNameFromGroup("Details", this.ExpandConfig.FieldGroupName);
            this.TilesName = fieldControl.ValueForAttribute("tiles");
            bool hasCopyFields = false;

            foreach (FieldControlTab tab in fieldControl.Tabs)
            {
                if (tab.Fields != null)
                {
                    if (tab.Fields.Any(field => !string.IsNullOrEmpty(field.Function) && field.Function != "="))
                    {
                        hasCopyFields = true;
                    }
                }

                if (hasCopyFields)
                {
                    break;
                }
            }

            if (!hasCopyFields)
            {
                this.ContinueWithCopyFieldsLoaded(null);
            }
            else
            {
                this.CopyFields = new UPCopyFields(fieldControl);
                this.CopyFields.CopyFieldValuesForRecordIdentification(this.RecordIdentification, false, this);
            }
        }

        private void ContinueWithCopyFieldsLoaded(Dictionary<string, object> parameters)
        {
            this.ParameterDictionary = parameters;
            this.Tiles = !string.IsNullOrEmpty(this.TilesName) ? new UPCRMTiles(this.TilesName, this.RecordIdentification, parameters, this) : null;

            if (this.Tiles != null)
            {
                this.Tiles.Load();
                return;
            }

            this.ContinueBuildDetailsOrganizerPageTilesLoaded();
        }

        private int LinecountOrganizerHeaderSubLabelConfigured()
        {
            FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("Details", this.ExpandConfig.FieldGroupName);
            if (fieldControl != null)
            {
                var fieldMapping = fieldControl.FunctionNames();

                if (fieldMapping.ContainsKey("OrganizerHeaderSubLabel"))
                {
                    return fieldMapping["OrganizerHeaderSubLabel"].Attributes.LineCount;
                }
            }

            return 0;
        }

        private bool IsOrganizerHeaderImageConfigured()
        {
            FieldControl fieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("Details", this.ExpandConfig.FieldGroupName);
            if (fieldControl != null)
            {
                int numberOfFields = fieldControl.NumberOfFields;
                for (int index = 0; index < numberOfFields; index++)
                {
                    UPConfigFieldControlField field = fieldControl.FieldAtIndex(index);
                    FieldAttribute attribute = field.Attributes.AttributForId((int)FieldAttr.Image);
                    if (attribute != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Informs the delegate about changed records.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public override void InformDelegateAboutChangedRecords(List<IIdentifier> changes)
        {
            if (this.RefreshOrganizer)
            {
                foreach (IIdentifier identifier in changes)
                {
                    var recordIdentifier = identifier as RecordIdentifier;
                    if (recordIdentifier != null && recordIdentifier.RecordIdentification == this.RecordIdentification)
                    {
                        this.PageModelControllers.Clear();
                        this.UpdatedElement(this.Organizer);
                        this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("Reset"));
                        return;
                    }
                }
            }

            base.InformDelegateAboutChangedRecords(changes);
        }

        /// <summary>
        /// Updates the action items status.
        /// </summary>
        public override void UpdateActionItemsStatus()
        {
            if (this.ActionItem(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite)) != null)
            {
                this.FavoriteModelController.IsFavorite(this.RecordIdentification);
            }
        }

        /// <summary>
        /// The table caption did fail with error.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="error">The error.</param>
        public void TableCaptionDidFailWithError(UPConfigTableCaption tableCaption, Exception error)
        {
            this.Organizer.TitleText = LocalizedString.Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsCouldNotLoadTableCaption);
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("RecordDataChanged"));
        }

        /// <summary>
        /// The table caption did finish with result.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="tableCaptionString">The table caption string.</param>
        public void TableCaptionDidFinishWithResult(UPConfigTableCaption tableCaption, string tableCaptionString)
        {
            this.Organizer.TitleText = tableCaptionString;
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("RecordDataChanged"));
        }

        /// <summary>
        /// Processes the changes.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        public override void ProcessChanges(List<IIdentifier> listOfIdentifiers)
        {
            base.ProcessChanges(listOfIdentifiers);
            if (this.Organizer.Invalid)
            {
                this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("Reset"));
                this.UpdatedElement(this.Organizer);
            }
            else
            {
                if (listOfIdentifiers.Count > 0)
                {
                    this.RefreshTableCaption();
                }

                this.Tiles?.Load();
            }
        }

        /// <summary>
        /// Sets the image for organizer header.
        /// </summary>
        /// <param name="imageDocument">The image document.</param>
        public void SetImageForOrganizerHeader(UPMDocument imageDocument)
        {
            this.Organizer.ImageDocument = imageDocument;
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("RecordDataChanged"));
        }

        /// <summary>
        /// Sets the record title for organizer header.
        /// </summary>
        /// <param name="recordTitle">The record title.</param>
        public void SetRecordTitleForOrganizerHeader(string recordTitle)
        {
            this.Organizer.AdditionalTitleText = recordTitle;
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("RecordDataChanged"));
        }

        /// <summary>
        /// Uploads the photo.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected override void UploadPhoto(ViewReference viewReference)
        {
            Dictionary<string, object> dict = this.ParameterDictionary != null
                                                ? new Dictionary<string, object>(this.ParameterDictionary)
                                                : new Dictionary<string, object>();

            string curDay = DateTime.Today.CrmValueFromDate();
            string curRep = UPCRMDataStore.DefaultStore.Reps.CurrentRepId;

            if (curDay == null)
            {
                curDay = string.Empty;
            }

            if (curRep == null)
            {
                curRep = string.Empty;
            }

            dict["curDay"] = curDay;
            dict["curRep"] = curRep;
            if (dict.ContainsKey("="))
            {
                dict.Remove("=");
            }

            this.DetailModelControllerDelegate.UploadPhotoWithRecordIdentification(viewReference.ContextValueForKey("Config"), this.RecordIdentification, dict);
        }

        /// <summary>
        /// Refreshes the after document uploaded.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        public void RefreshAfterDocumentUploaded(string recordIdentification)
        {
            List<IIdentifier> changedIdentifiers = null;
            if (recordIdentification != null)
            {
                changedIdentifiers = new List<IIdentifier> { new RecordIdentifier(recordIdentification) };
            }

            if (!string.IsNullOrEmpty(this.RecordIdentification))
            {
                RecordIdentifier ri = new RecordIdentifier(this.RecordIdentification);
                if (changedIdentifiers != null)
                {
                    changedIdentifiers.Add(ri);
                }
                else
                {
                    changedIdentifiers = new List<IIdentifier> { ri };
                }
            }

            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                if (modelController is DetailPageModelController || modelController is DocumentPageModelController)
                {
                    modelController.Page.Invalid = true;
                    modelController.ProcessChanges(changedIdentifiers);
                }
            }

            UPChangeManager.CurrentChangeManager.RegisterChanges(changedIdentifiers);
        }

        /// <summary>
        /// Toggles the favorite.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected override void ToggleFavorite(ViewReference viewReference)
        {
            if (!string.IsNullOrEmpty(this.Organizer.FavoriteRecordIdentification))
            {
                this.FavoriteModelController.ChangeFavoriteValue(this.Organizer.FavoriteRecordIdentification, false);
            }
            else
            {
                this.FavoriteModelController.ChangeFavoriteValue(this.RecordIdentification, true);
            }

            RecordIdentifier identifier = new RecordIdentifier(this.RecordIdentification);
            UPChangeManager.CurrentChangeManager.RegisterChanges(new List<IIdentifier> { identifier });
        }

        /// <summary>
        /// The favorite model controller did change favorite.
        /// </summary>
        /// <param name="favoriteModelController">The favorite model controller.</param>
        /// <param name="favoriteRecordIdentification">The favorite record identification.</param>
        public void FavoriteModelControllerDidChangeFavorite(UPFavoriteModelController favoriteModelController, string favoriteRecordIdentification)
        {
            ((UPMOrganizer)this.TopLevelElement).FavoriteRecordIdentification = favoriteRecordIdentification;
            UPMOrganizerAction action = this.ActionItem(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite));
            if (action != null)
            {
                action.Aktive = !string.IsNullOrEmpty(favoriteRecordIdentification);
                action.LabelText = action.Aktive ? LocalizedString.TextProcessDeleteFromFavorites : LocalizedString.TextProcessAddToFavorites;
                this.ModelControllerDelegate?.ModelControllerDidChange(this, this.TopLevelElement, this.TopLevelElement,
                    new List<IIdentifier> { action.Identifier }, UPChangeHints.ChangeHintsWithHint("ActionItemStatusChanged"));
            }
        }

        /// <summary>
        /// The favorite model controller favorite record identification.
        /// </summary>
        /// <param name="favoriteModelController">The favorite model controller.</param>
        /// <param name="favoriteRecordIdentification">The favorite record identification.</param>
        public void FavoriteModelControllerFavoriteRecordIdentification(UPFavoriteModelController favoriteModelController, string favoriteRecordIdentification)
        {
            ((UPMOrganizer)this.TopLevelElement).FavoriteRecordIdentification = favoriteRecordIdentification;
            UPMOrganizerAction action = this.ActionItem(StringIdentifier.IdentifierWithStringId(Core.Constants.ActionIdToggleFavorite));
            if (action != null)
            {
                action.Aktive = string.IsNullOrEmpty(favoriteRecordIdentification);
                action.LabelText = !action.Aktive ? LocalizedString.TextProcessDeleteFromFavorites : LocalizedString.TextProcessAddToFavorites;
                this.ModelControllerDelegate?.ModelControllerDidChange(this, this.TopLevelElement, this.TopLevelElement,
                    new List<IIdentifier> { action.Identifier }, UPChangeHints.ChangeHintsWithHint("ActionItemStatusChanged"));
            }
        }

        /// <summary>
        /// The favorite model controller did fail with error.
        /// </summary>
        /// <param name="favoriteModelController">The favorite model controller.</param>
        /// <param name="error">The error.</param>
        public void FavoriteModelControllerDidFailWithError(UPFavoriteModelController favoriteModelController, Exception error)
        {
            //UPStatusHandler.ShowErrorMessage(upTextErrorCouldNotBeSaved);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public void UploadFile(ViewReference viewReference)
        {
            UPInBoxPageModelController inboxPageModelController = new UPInBoxPageModelController(viewReference)
            {
                ParentOrganizerModelController = this
            };
            this.DetailModelControllerDelegate.UploadFileWithLinkRecordIdentification(this.RecordIdentification, inboxPageModelController);
        }

        /// <summary>
        /// Adds the name of to history view reference image.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="imageName">Name of the image.</param>
        private void AddToHistory(string recordIdentification, ViewReference viewReference, string imageName)
        {
            HistoryManager.DefaultHistoryManager.AddHistory(recordIdentification, viewReference, this.OnlineData, imageName);
        }

        /// <summary>
        /// The alternate expand checker did finish with result.
        /// </summary>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="expand">The expand.</param>
        public void AlternateExpandCheckerDidFinishWithResult(UPCRMAlternateExpandChecker expandChecker, UPConfigExpand expand)
        {
            if (expand != null)
            {
                this.ExpandConfig = expand;
            }

            this.ContinueBuildDetailsOrganizerPage();
            this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("AlternateExpandFound"));
            this.AddToHistory(this.RecordIdentification, this.ViewReference, this.ExpandConfig.ImageName);
        }

        /// <summary>
        /// The alternate expand checker did fail with error.
        /// </summary>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="error">The error.</param>
        public void AlternateExpandCheckerDidFailWithError(UPCRMAlternateExpandChecker expandChecker, Exception error)
        {
            this.ContinueBuildDetailsOrganizerPage();
        }

        /// <summary>
        /// Links the reader did finish with result.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            this.LinkReader = null;
            if (!string.IsNullOrEmpty(linkReader.DestinationRecordIdentification))
            {
                this.RecordIdentification = linkReader.DestinationRecordIdentification;
                this.ContinueBuildDetailOrganizerPages0();
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader linkReader, Exception error)
        {
            this.LinkReader = null;
            this.ContinueBuildDetailOrganizerPages0();
        }

        /// <summary>
        /// Tileses the did finish with success.
        /// </summary>
        /// <param name="crmTiles">The CRM tiles.</param>
        /// <param name="data">The data.</param>
        public void TilesDidFinishWithSuccess(UPCRMTiles crmTiles, object data)
        {
            this.Tiles = crmTiles;
            UPMOrganizer detailOrganizer = (UPMOrganizer)this.TopLevelElement;
            if (detailOrganizer.ExpandFound)
            {
                List<UPMTile> recordTiles = new List<UPMTile>();
                foreach (UPCRMTile tile in this.Tiles.Tiles)
                {
                    if (string.IsNullOrEmpty(tile.Value) && string.IsNullOrEmpty(tile.Text))
                    {
                        continue;
                    }

                    UPMTile tileElement = new UPMTile(StringIdentifier.IdentifierWithStringId("Tile"))
                    {
                        TextField = new UPMStringField(StringIdentifier.IdentifierWithStringId("TileText"))
                        {
                            StringValue = tile.Text
                        },
                        ValueField = new UPMStringField(StringIdentifier.IdentifierWithStringId("ValueText"))
                        {
                            StringValue = tile.Value
                        },
                        ImageName = tile.ImageName
                    };
                    recordTiles.Add(tileElement);
                }

                detailOrganizer.Tiles = recordTiles;
                this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("RecordDataChanged"));
            }
            else
            {
                this.ContinueBuildDetailsOrganizerPageTilesLoaded();
            }
        }

        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            this.CopyFields = null;
            this.ContinueWithCopyFieldsLoaded(dictionary);
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            this.CopyFields = null;
            this.ContinueWithCopyFieldsLoaded(null);
        }

        /// <summary>
        /// Switch on record
        /// </summary>
        /// <param name="viewReference">View reference</param>
        public override void SwitchOnRecord(ViewReference viewReference)
        {
            if (this.ActionHandler != null)
            {
                return; // don't do anything if other action is active
            }

            this.ActionHandler = new RecordSwitchActionHandler(this, viewReference);
            this.ActionHandler.Execute();
        }

        /// <summary>
        /// Method Loads tiles and subviews into Details Organizer Page
        /// </summary>
        private void ContinueBuildDetailsOrganizerPageTilesLoaded()
        {
            var configStore = ConfigurationUnitStore.DefaultStore;
            var detailOrganizer = (UPMOrganizer)TopLevelElement;
            var recordTiles = GetUPMTiles();

            detailOrganizer.Tiles = recordTiles;
            Organizer.OrganizerColor = ColorForInfoAreaWithId(InfoAreaId);
            Organizer.DisplaysTitleText = true;
            Organizer.ExpandFound = true;
            RefreshTableCaption();

            if (string.IsNullOrWhiteSpace(VirtualInfoAreaId) || string.IsNullOrWhiteSpace(ExpandConfig.UnitName))
            {
                return;
            }

            var additionalParameters = new Dictionary<string, object>
            {
                { nameof(VirtualInfoAreaId), VirtualInfoAreaId },
                { "CurrentExpandName", ExpandConfig.UnitName }
            };

            var detailPageModelController = new DetailPageModelController(ViewReference.ViewReferenceWith(additionalParameters));
            detailPageModelController.Init();
            var overviewPage = detailPageModelController.Page;
            overviewPage.Parent = detailOrganizer;
            AddPageModelController(detailPageModelController);
            detailOrganizer.AddPage(overviewPage);
            var expandHeader = configStore.HeaderByNameFromGroup(HeaderNameExpand, ExpandConfig.HeaderGroupName);

            if (expandHeader != null)
            {
                UPMOrganizerAddPages(expandHeader, detailOrganizer, overviewPage);
                AddActionButtonsFromHeaderRecordIdentification(expandHeader, RecordIdentification);
                var offline = UPCRMDataStore.DefaultStore.RecordExistsOffline(RecordIdentification);
                if (!offline)
                {
                    OnlineData = true;
                    SyncRecordAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId(ActionSyncRecord));
                    SyncRecordAction.SetTargetAction(this, PerformAction);
                    var parameter = new List<string> { "RecordId", "Value", "uid" };
                    var parameters = new List<object> { parameter };
                    var definition = new List<object> { string.Empty, "Action:syncRecord", parameters };
                    var syncRecordViewReference = new ViewReference(definition, ButtonSyncRecordText);
                    SyncRecordAction.ViewReference = syncRecordViewReference.ViewReferenceWith(RecordIdentification);
                }
                else
                {
                    OnlineData = false;
                    SyncRecordAction = null;
                }
            }

            Organizer.DisplaysImage = IsOrganizerHeaderImageConfigured();
            Organizer.LineCountAdditionalTitletext = LinecountOrganizerHeaderSubLabelConfigured();
            UpdateActionItemsStatus();
            InformAboutDidChangeTopLevelElement(Organizer, Organizer, null, UPChangeHints.ChangeHintsWithHint(HintAlternateExpandFound));
        }

        /// <summary>
        /// Method generates a list of UPMTiles
        /// </summary>
        /// <returns>
        /// List <see cref="UPMTile"/>
        /// </returns>
        private List<UPMTile> GetUPMTiles()
        {
            var recordTiles = new List<UPMTile>();
            if (Tiles?.Tiles != null)
            {
                foreach (var tile in Tiles.Tiles)
                {
                    if (string.IsNullOrWhiteSpace(tile.Value) && string.IsNullOrWhiteSpace(tile.Text))
                    {
                        continue;
                    }

                    var tileElement = new UPMTile(StringIdentifier.IdentifierWithStringId(TileIdentifier))
                    {
                        TextField = new UPMStringField(StringIdentifier.IdentifierWithStringId(TileTextIdentifier))
                        {
                            StringValue = tile.Text
                        },
                        ValueField = new UPMStringField(StringIdentifier.IdentifierWithStringId(ValueTextIdentifier))
                        {
                            StringValue = tile.Value
                        },
                        ImageName = tile.ImageName
                    };

                    recordTiles.Add(tileElement);
                }
            }

            return recordTiles;
        }

        /// <summary>
        /// Adds Subview pages to UPMOrganizer
        /// </summary>
        /// <param name="expandHeader">
        /// <see cref="UPConfigHeader"/> configuration object
        /// </param>
        /// <param name="detailOrganizer">
        /// <see cref="UPMOrganizer"/> to add pages to
        /// </param>
        /// <param name="overviewPage">
        /// <see cref="Page"/> object to update LableText
        /// </param>
        private void UPMOrganizerAddPages(UPConfigHeader expandHeader, UPMOrganizer detailOrganizer, Page overviewPage)
        {
            var additionalParameters = new Dictionary<string, object> { { nameof(VirtualInfoAreaId), VirtualInfoAreaId } };
            if (expandHeader.SubViews != null)
            {
                foreach (UPConfigHeaderSubView subView in expandHeader.SubViews)
                {
                    if (subView.Options == "#")
                    {
                        overviewPage.LabelText = subView.Label;
                        continue;
                    }

                    UPPageModelController pageModelController = PageForViewReference(
                        subView.ViewReference?.ViewReferenceWith(additionalParameters).ViewReferenceWith(RecordIdentification));

                    if (pageModelController != null)
                    {
                        pageModelController.Page.Parent = detailOrganizer;
                        pageModelController.Page.Invalid = true;
                        pageModelController.Page.LabelText = subView.Label;
                        AddPageModelController(pageModelController);
                        if (pageModelController is UPWebContentPageModelController)
                        {
                            var webContentPageModelController = (UPWebContentPageModelController)pageModelController;
                            if (webContentPageModelController.AllowsXMLExport)
                            {
                                var exportXMLAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.exportXML"))
                                {
                                    LabelText = "Export XML"
                                };
                                AddOrganizerHeaderActionItem(exportXMLAction);
                            }
                        }

                        detailOrganizer.AddPage(pageModelController.Page);
                    }
                }
            }
        }
    }
}
