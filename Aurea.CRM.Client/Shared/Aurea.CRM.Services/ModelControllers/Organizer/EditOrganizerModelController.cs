// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditOrganizerModelController.cs" company="Aurea Software Gmbh">
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
//   The Edit Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Aurea.CRM.Services.ModelControllers.Organizer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.Messages;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Edit;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Views;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Constants
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The information area identifier configuration name
        /// </summary>
        public const string InfoAreaIdConfigurationName = "InfoAreaId";

        /// <summary>
        /// The edit view configuration name
        /// </summary>
        public const string EditViewConfigurationName = "EditView";

        /// <summary>
        /// The RFC root information area identifier configuration name
        /// </summary>
        public const string RfcRootInfoAreaIdConfigurationName = "R4CRootInfoAreaId";

        /// <summary>
        /// The RFC information area identifier configuration name
        /// </summary>
        public const string RfcInfoAreaIdConfigurationName = "R4CInfoAreaId";

        /// <summary>
        /// The RFC source field group configuration name
        /// </summary>
        public const string RfcSourceFieldGroupConfigurationName = "SourceCopyFieldGroup";
    }

    /// <summary>
    /// Edit Organizer Model Controller
    /// </summary>
    /// <seealso cref="Aurea.CRM.Services.ModelControllers.Organizer.UPOrganizerModelController" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPOfflineRequestDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCopyFieldsDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.IRemoteTableCaptionDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.UPCRMLinkReaderDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.IAlternateExpandCheckerDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Features.UPSelectorContextDelegate" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class EditOrganizerModelController : UPOrganizerModelController,
        UPCopyFieldsDelegate,
        IRemoteTableCaptionDelegate,
        UPCRMLinkReaderDelegate,
        IAlternateExpandCheckerDelegate,
        UPSelectorContextDelegate
    {
        private const string KeyPopToPrevious = "PopToPrevious";
        private const string RefreshFilterTokensLiteral = "RefreshFilterTokens";

        protected string LinkRecordIdentification;
        protected string OriginalLinkRecordIdentification;
        protected string CopyRecordIdentification;
        protected int LinkId;
        protected string LinkRecordIdentification2;
        protected int LinkId2;
        protected string ConfigName;
        protected string AlternateConfigName;
        protected string InfoAreaId;
        protected Dictionary<string, object> InitialValueDictionary;
        protected UPConfigFilter TemplateFilter;
        protected UPConfigFilter SubRecordTemplateFilter;
        protected Dictionary<string, object> SubRecordTemplateFilterParameters;
        protected Dictionary<string, object> CopyFieldDictionary;
        protected Dictionary<string, object> AdditionalParametersDictionary;
        protected bool IsNew;
        protected UPConfigExpand ExpandConfig;
        protected UPCopyFields CopyFields;
        protected UPOfflineEditRecordRequest EditRecordRequest;
        protected List<UPCRMRecord> EditRecordRequestData;
        protected UPOfflineUploadDocumentRequest UploadDocumentRequest;
        protected UPCRMLinkReader LinkReader;
        protected UPCRMAlternateExpandChecker AlternateExpandChecker;
        protected string RfcRootInfoAreaId;
        protected string RfcInfoAreaId;
        protected FieldControl FieldControl;
        protected string RfcRecordIdentification;
        protected ViewReference SaveViewReference;
        protected bool PopToPrevious;
        protected bool PopToRootOnSave;
        protected List<UPOfflineUploadDocumentRequest> UploadDocumentRequests;
        protected bool isClosed;
        private int changedRecordsCount;

        /// <summary>
        /// Gets the first <see cref="EditPageModelController">EditPageModelController</see> from the list
        /// </summary>
        public EditPageModelController FirstPageModelController { get; set; }

        /// <summary>
        /// Gets current record ID
        /// </summary>
        public string RecordIdentification { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [online data].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [online data]; otherwise, <c>false</c>.
        /// </value>
        public override bool OnlineData
        {
            get
            {
                return this.onlineData;
            }

            set
            {
                this.onlineData = value;
                if (this.onlineData)
                {
                    //this.Organizer.StatusIndicatorIcon = UIImage.UpXXImageNamed("crmpad-OrganizerHeader-Cloud");         // CRM-5007
                }
            }
        }

        /// <summary>
        /// Gets the virtual information area identifier.
        /// </summary>
        /// <value>
        /// The virtual information area identifier.
        /// </value>
        public string VirtualInfoAreaId { get; private set; }

        /// <summary>
        /// Gets the request mode.
        /// </summary>
        /// <value>
        /// The request mode.
        /// </value>
        public UPOfflineRequestMode RequestMode { get; private set; }

        /// <summary>
        /// Gets the test delegate.
        /// </summary>
        /// <value>
        /// The test delegate.
        /// </value>
        public IEditOrganizerModelControllerTestDelegate TestDelegate { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="initOptions">The initialize options.</param>
        public EditOrganizerModelController(ViewReference viewReference, UPOrganizerInitOptions initOptions = null)
            : base(viewReference, initOptions)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string configValue = this.ViewReference.ContextValueForKey("RequestMode");
            var defaultRequestOption = UPOfflineRequestMode.OnlineConfirm;

            if (this.ViewReference.ContextValueForKey("Mode") == "New" || this.ViewReference.ContextValueForKey("Mode") == "NewOrEdit")
            {
                string offlineCheckInfoArea = this.InfoAreaId;
                if (string.IsNullOrEmpty(offlineCheckInfoArea))
                {
                    string configName = viewReference.ContextValueForKey("ConfigName");
                    FieldControl fieldControl = configStore.FieldControlByNameFromGroup("Edit", configName) ??
                                                 configStore.FieldControlByNameFromGroup("Details", this.ConfigName);

                    offlineCheckInfoArea = fieldControl.InfoAreaId ?? this.ViewReference.ContextValueForKey("InfoAreaId");
                }

                if (!string.IsNullOrEmpty(offlineCheckInfoArea) && !UPCRMDataStore.DefaultStore.HasOfflineData(offlineCheckInfoArea))
                {
                    defaultRequestOption = UPOfflineRequestMode.OnlineOnly;
                }
            }

            if (defaultRequestOption == UPOfflineRequestMode.OnlineOnly)
            {
                if (string.IsNullOrEmpty(configValue))
                {
                    configValue = configStore.ConfigValue("Edit.DefaultOfflineNewRequestMode");
                }
            }
            else if (string.IsNullOrEmpty(configValue))
            {
                configValue = configStore.ConfigValue("Edit.DefaultRequestMode");
            }

            this.RequestMode = UPOfflineRequest.RequestModeFromString(configValue, defaultRequestOption);
            this.EditRecordRequest = new UPOfflineEditRecordRequest(this.RequestMode, viewReference);
            this.EditRecordRequest.RelatedInfoDictionary = this.CopyFieldDictionary;
            string optionsString = viewReference.ContextValueForKey("Options");

            if (!string.IsNullOrEmpty(optionsString))
            {
                Dictionary<string, object> options = optionsString.JsonDictionaryFromString();
                if (Convert.ToInt32(options.ValueOrDefault("ComputeLinks")) != 0)
                {
                    this.EditRecordRequest.AlwaysSetImplicitLinks = true;
                }
            }

            InfoArea infoAreaConfig = configStore.InfoAreaConfigById(this.InfoAreaId);
            if (!string.IsNullOrEmpty(infoAreaConfig?.ImageName))
            {
                UPConfigResource resource = configStore.ResourceByName(infoAreaConfig.ImageName);
                if (resource != null)
                {
                    this.EditRecordRequest.ImageName = resource.FileName;
                }
            }

            this.ShouldShowTabsForSingleTab = initOptions?.bShouldShowTabsForSingleTab ?? false;
            this.PopToPrevious = false;
            string savedActionName = viewReference.ContextValueForKey("SavedAction");
            if (!string.IsNullOrEmpty(savedActionName))
            {
                if (savedActionName.StartsWith("Button:"))
                {
                    UPConfigButton button = configStore.ButtonByName(savedActionName.Substring(7));
                    this.SaveViewReference = button.ViewReference;
                }
                else if (savedActionName.StartsWith("Menu:"))
                {
                    Menu menu = configStore.MenuByName(savedActionName.Substring(5));
                    this.SaveViewReference = menu.ViewReference;
                }
                else if (savedActionName == "Return")
                {
                    this.PopToPrevious = true;
                }
                else
                {
                    Menu menu = configStore.MenuByName(savedActionName);
                    this.SaveViewReference = menu?.ViewReference;
                }

                var stringValue = viewReference.ContextValueForKey(KeyPopToPrevious);

                bool isPopToPrevious;
                if (bool.TryParse(stringValue, out isPopToPrevious) && isPopToPrevious)
                {
                    this.PopToPrevious = true;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditOrganizerModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="testDelegate">The test delegate.</param>
        public EditOrganizerModelController(ViewReference viewReference, IEditOrganizerModelControllerTestDelegate testDelegate)
            : this(viewReference, new UPOrganizerInitOptions(true, false))
        {
            this.TestDelegate = testDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditOrganizerModelController"/> class.
        /// </summary>
        /// <param name="offlineRequest">The offline request.</param>
        public EditOrganizerModelController(UPOfflineEditRecordRequest offlineRequest)
            : base( new ViewReference(offlineRequest.Json, Constants.EditViewConfigurationName), UPOrganizerInitOptions.AddNoAutoBuildToOptions(null))
        {
            this.EditRecordRequest = offlineRequest;
            this.BuildEditOrganizerPages();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is edit organizer.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is edit organizer; otherwise, <c>false</c>.
        /// </value>
        public override bool IsEditOrganizer => true;

        /// <summary>
        /// Gets a value indicating whether [hide back button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide back button]; otherwise, <c>false</c>.
        /// </value>
        public bool HideBackButton => true;

        /// <summary>
        /// Builds the pages from view reference.
        /// </summary>
        public override void BuildPagesFromViewReference()
        {
            if (this.ViewReference.ViewName == "RecordListView")
            {
            }
            else if (this.ViewReference.ViewName == "RecordView")
            {
                this.BuildEditOrganizerPages();
            }
            else if (this.ViewReference.ViewName.StartsWith(Constants.EditViewConfigurationName))
            {
                var recordId = this.ViewReference.ContextValueForKey("EditOrNewRecordId");
                var isEditOrNew = this.ViewReference.ContextValueForKey("Mode") == "NewOrEdit";
                if (isEditOrNew)
                {
                    this.ViewReference = new ViewReference(this.ViewReference, null, recordId, "RecordId");
                }

                this.BuildEditOrganizerPages();
            }
            else if (this.ViewReference.ViewName.StartsWith("SerialEntry"))
            {
                this.BuildEditOrganizerPages();
            }

            UPSelector.StaticSelectorContextDelegate = this;
        }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        /// <param name="appliedIdentifiers">The applied identifiers.</param>
        public override void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                pageModelController.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            }
        }

        /// <summary>
        /// Ups the table caption did fail with error.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="error">The error.</param>
        public void TableCaptionDidFailWithError(UPConfigTableCaption tableCaption, Exception error)
        {
            this.Organizer.TitleText = LocalizedString.Localize(LocalizationKeys.TextGroupErrors,
                                        LocalizationKeys.KeyErrorsCouldNotLoadTableCaption);
        }

        /// <summary>
        /// Ups the table caption did finish with result.
        /// </summary>
        /// <param name="tableCaption">The table caption.</param>
        /// <param name="tableCaptionString">The table caption string.</param>
        public void TableCaptionDidFinishWithResult(UPConfigTableCaption tableCaption, string tableCaptionString)
        {
            this.Organizer.TitleText = tableCaptionString;
        }

        /// <summary>
        /// Builds the edit organizer pages.
        /// </summary>
        public void BuildEditOrganizerPages()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            var configNameHasWildCards = this.HasConfigNameHasWildCards(configStore);
            this.SetLinkRecordIdentification();
            this.SetLinkId();
            this.Organizer.OrganizerColor = this.ColorForInfoAreaWithId(this.InfoAreaId);
            this.SetExpandConfig(configNameHasWildCards, configStore);

            this.IsNew = this.ViewReference.ContextValueForKey("Mode") == "New" || (this.ViewReference.ContextValueForKey("Mode") == "NewOrEdit" && this.RecordIdentification == null);
            if (this.IsNew)
            {
                this.ContinueBuildEditOrganizerPages();
            }
            else
            {
                if (UPCRMDataStore.DefaultStore.RecordExistsOffline(this.RecordIdentification))
                {
                    if (this.ExpandConfig?.AlternateExpands == null)
                    {
                        this.ContinueBuildEditOrganizerPages();
                    }
                    else
                    {
                        this.AlternateExpandChecker = new UPCRMAlternateExpandChecker(this.RecordIdentification, this.ExpandConfig, this);
                        this.AlternateExpandChecker.Start(UPRequestOption.FastestAvailable);
                    }
                }
                else
                {
                    string sourceCopyFieldGroupName = this.ViewReference.ContextValueForKey(Constants.RfcSourceFieldGroupConfigurationName);
                    FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Edit", sourceCopyFieldGroupName);
                    UPContainerMetaInfo containerMetaInfo = new UPContainerMetaInfo(sourceFieldControl);
                    Operation remoteOperation = containerMetaInfo.ReadRecord(this.RecordIdentification, this);
                }
            }
        }

        private ViewReference UpdateViewReferenceForRequestForChange(ViewReference sourceViewReference)
        {
            Dictionary<string, object> newOrUpdateValues = new Dictionary<string, object>();
            string configExpandName = this.ViewReference.ContextValueForKey("ExpandName");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.RfcRootInfoAreaId = this.ViewReference.ContextValueForKey(Constants.RfcRootInfoAreaIdConfigurationName);
            this.RfcInfoAreaId = this.ViewReference.ContextValueForKey(Constants.RfcInfoAreaIdConfigurationName);
            UPConfigExpand configExpand = configStore.ExpandByName(configExpandName);
            newOrUpdateValues["RfcConfigName"] = configExpandName;
            this.FieldControl = configStore.FieldControlByNameFromGroup("Edit", configExpand.FieldGroupName);
            this.RfcRecordIdentification = this.LoadRecordIdentificationForRequestForChangeInfoArea(this.FieldControl, this.RfcInfoAreaId);
            newOrUpdateValues["RfcRecordId"] = this.RfcRecordIdentification;
            newOrUpdateValues["RfcInfoAreaId"] = this.RfcInfoAreaId;
            ViewReference newViewReference = sourceViewReference.ViewReferenceWith(newOrUpdateValues);
            newViewReference = newViewReference.ViewReferenceWith(new Dictionary<string, object> { { "RfcexpandConfiguration", configExpand.UnitName } });

            if (UPCRMDataStore.DefaultStore.RecordExistsOffline(this.RecordIdentification))
            {
                this.InitialValueDictionary = null;
                string sourceCopyFieldGroupName = this.ViewReference.ContextValueForKey(Constants.RfcSourceFieldGroupConfigurationName);
                FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Edit", sourceCopyFieldGroupName);
                UPCRMResult sourceResult = this.LoadSourceRecordForRequestForChangeRecordId(sourceFieldControl, this.RecordIdentification);
                UPCRMResultRow sourceResultRow = sourceResult.RowCount > 0 ? (UPCRMResultRow)sourceResult.ResultRowAtIndex(0) : null;
                if (this.IsNew)
                {
                    this.InitialValueDictionary = this.FillDictionaryFromSourceFieldGroup(sourceFieldControl,
                        this.FieldControl, sourceResultRow?.RawValues());
                }
            }

            if (this.IsNew)
            {
                if (this.EditRecordRequest != null)
                {
                    this.EditRecordRequest.RelatedInfoDictionary = this.InitialValueDictionary;
                }

                string templateFilterName = this.ViewReference.ContextValueForKey("TemplateFilterName");
                if (!string.IsNullOrEmpty(templateFilterName))
                {
                    UPConfigFilter templateFilterTemp = configStore.FilterByName(templateFilterName);
                    if (templateFilterTemp != null)
                    {
                        this.TemplateFilter = templateFilterTemp.FilterByApplyingReplacements(UPConditionValueReplacement.DefaultParameters);
                    }
                }
            }

            return newViewReference;
        }

        private string LoadRecordIdentificationForRequestForChangeInfoArea(FieldControl fieldControl, string infoAreaId)
        {
            string rfcRecordId = null;
            if (fieldControl.InfoAreaId != this.RecordIdentification.InfoAreaId())
            {
                this.IsNew = true;
                return null;
            }

            UPContainerMetaInfo query = new UPContainerMetaInfo(fieldControl);
            query.SetLinkRecordIdentification(this.RecordIdentification);
            UPCRMResult crmResult = query.Find();
            if (crmResult.RowCount > 0)
            {
                rfcRecordId = crmResult.ResultRowAtIndex(0).RootRecordId;
            }
            else
            {
                this.IsNew = true;
            }

            if (!string.IsNullOrEmpty(rfcRecordId))
            {
                rfcRecordId = $"{infoAreaId}.{rfcRecordId}";
            }

            return rfcRecordId;
        }

        private UPCRMResult LoadSourceRecordForRequestForChangeRecordId(FieldControl fieldControl, string recordId)
        {
            UPContainerMetaInfo query = new UPContainerMetaInfo(fieldControl);
            UPCRMResult crmResult = query.ReadRecord(recordId);
            return crmResult?.RowCount > 0 ? crmResult : null;
        }

        private void ContinueBuildEditOrganizerPages()
        {
            if (this.ExpandConfig == null)
            {
                this.AddOrganizerActions();
                this.EnableActionItemsDisableActionItems(this.LeftNavigationBarItems, this.RightNaviagtionBarItems);
                if (this.TestDelegate == null)
                {
                    this.HandleOrganizerActionError(LocalizedString.TextErrorConfiguration, string.Format(LocalizedString.TextErrorExpandMissing, this.ConfigName), false);
                }
                else
                {
                    this.TestDelegate.OrganizerModelControllerDidFailWithError(this, new Exception(LocalizedString.TextErrorExpandMissing));
                }

                return;
            }

            if (string.IsNullOrEmpty(this.InfoAreaId))
            {
                this.InfoAreaId = this.ExpandConfig.InfoAreaId;
            }

            if (this.IsNew && !IsRequestForChangeMode(this.ViewReference))
            {
                this.BuildEditOrganizerPagesForNew();
            }
            else
            {
                this.BuildEditOrganizerPagesForEdit();
            }
        }

        private Dictionary<string, object> ParametersToCopyFieldDictionary(Dictionary<string, object> parameters)
        {
            string configValue = this.ViewReference.ContextValueForKey("AdditionalParameters");
            if (!string.IsNullOrEmpty(configValue))
            {
                this.AdditionalParametersDictionary = configValue.JsonDictionaryFromString();
            }

            if (this.AdditionalParametersDictionary != null)
            {
                //CLIENT-200 : Parameter always come as null so removed the below condition 
                //Dictionary<string, object> dict = parameters.Count > 0 ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();

                Dictionary<string, object> dict = new Dictionary<string, object>();

                foreach (string key in this.AdditionalParametersDictionary.Keys)
                {
                    if (dict.ValueOrDefault(key) == null)
                    {
                        object value = this.AdditionalParametersDictionary[key];
                        if (!(value is string) || !((string)value).StartsWith("$"))
                        {
                            dict[key] = value;
                        }
                    }
                }

                return dict;
            }

            return parameters;
        }

        private void ContinueTemplateFilterGeoCheckedWithInformUI(bool inform)
        {
            FieldControl copyFieldControl = null;
            if (!string.IsNullOrEmpty(this.CopyRecordIdentification))
            {
                string copySourceFieldGroup = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
                if (copySourceFieldGroup != null)
                {
                    copyFieldControl = ConfigurationUnitStore.DefaultStore.FieldControlByNameFromGroup("List", copySourceFieldGroup);
                }
            }

            if (copyFieldControl != null)
            {
                this.CopyFields = new UPCopyFields(copyFieldControl);
                this.CopyFields.CopyFieldValuesForRecordIdentification(this.CopyRecordIdentification, false, this);
            }
            else
            {
                this.ContinueBuildEditOrganizerPagesForNewWithParameters(null);
                if (inform)
                {
                    this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("AlternateExpandFound"));
                }
            }
        }

        private void ContinueBuildEditOrganizerPagesForNewWithParameters(Dictionary<string, object> parameters)
        {
            this.CopyFieldDictionary = this.ParametersToCopyFieldDictionary(parameters);

            if (this.EditRecordRequest != null)
            {
                this.EditRecordRequest.RelatedInfoDictionary = this.CopyFieldDictionary;
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string templateFilterName = this.ViewReference.ContextValueForKey("TemplateFilterName");
            string subRecordTemplateFilterName = this.ViewReference.ContextValueForKey("SubRecordTemplateFilterName");
            if (!string.IsNullOrEmpty(templateFilterName) || !string.IsNullOrEmpty(subRecordTemplateFilterName))
            {
                UPConfigFilter templateFilterTemp = !string.IsNullOrEmpty(templateFilterName) ? configStore.FilterByName(templateFilterName) : null;
                this.SubRecordTemplateFilter = !string.IsNullOrEmpty(subRecordTemplateFilterName) ? configStore.FilterByName(subRecordTemplateFilterName) : null;
                if (templateFilterTemp != null || this.SubRecordTemplateFilter != null)
                {
                    Dictionary<string, object> allCopyFieldParameters = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(this.RecordIdentification))
                    {
                        allCopyFieldParameters.SetObjectForKey(this.RecordIdentification, "Record");
                    }

                    if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
                    {
                        allCopyFieldParameters.SetObjectForKey(this.LinkRecordIdentification, "LinkRecord");
                    }

                    if (!string.IsNullOrEmpty(this.LinkRecordIdentification2))
                    {
                        allCopyFieldParameters.SetObjectForKey(this.LinkRecordIdentification2, "LinkRecord2");
                    }

                    if (this.CopyFieldDictionary != null)
                    {
                        foreach (var key in this.CopyFieldDictionary.Keys)
                        {
                            allCopyFieldParameters[key] = this.CopyFieldDictionary[key];
                        }
                    }

                    if (templateFilterTemp != null)
                    {
                        this.TemplateFilter = templateFilterTemp.FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(allCopyFieldParameters, true));
                    }

                    this.SubRecordTemplateFilterParameters = allCopyFieldParameters;
                    if (!IsRequestForChangeMode(this.ViewReference))
                    {
                        this.InitialValueDictionary = this.TemplateFilter.FieldsWithConditions(true);
                    }
                }
            }

            this.AddOrganizerActions();
            this.AddPageModelControllers();
            this.Organizer.ExpandFound = true;
            this.TestDelegate?.OrganizerModelControllerFinishedWithPageModelController(this, this.FirstPageModelController);
        }

        /// <summary>
        /// Copies the fields did finish with values.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="dictionary">The dictionary.</param>
        public void CopyFieldsDidFinishWithValues(UPCopyFields copyFields, Dictionary<string, object> dictionary)
        {
            if (this.IsNew && !IsRequestForChangeMode(this.ViewReference))
            {
                this.ContinueBuildEditOrganizerPagesForNewWithParameters(dictionary);
            }
            else
            {
                this.ContinueBuildEditOrganizerPagesForEdit(dictionary);
            }

            if (this.TestDelegate == null)
            {
                this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("AlternateExpandFound"));
            }
        }

        /// <summary>
        /// Copies the fields did fail with error.
        /// </summary>
        /// <param name="copyFields">The copy fields.</param>
        /// <param name="error">The error.</param>
        public void CopyFieldsDidFailWithError(UPCopyFields copyFields, Exception error)
        {
            if (this.IsNew)
            {
                this.ContinueBuildEditOrganizerPagesForNewWithParameters(null);
            }
            else
            {
                this.ContinueBuildEditOrganizerPagesForEdit(null);
            }

            if (this.TestDelegate == null)
            {
                this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("AlternateExpandFound"));
            }
        }

        /// <summary>
        /// Builds the edit organizer pages for new.
        /// </summary>
        public void BuildEditOrganizerPagesForNew()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            string titleText = null;
            if (!string.IsNullOrEmpty(this.ExpandConfig.HeaderGroupName))
            {
                UPConfigHeader headerNew = configStore.HeaderByNameFromGroup("New", this.ExpandConfig.HeaderGroupName);
                titleText = headerNew?.Label;
            }

            if (string.IsNullOrEmpty(titleText))
            {
                InfoArea infoAreaDefinition = configStore.InfoAreaConfigById(this.InfoAreaId);
                titleText = LocalizedString.TextNewInfoArea.Replace("%@", infoAreaDefinition != null ? infoAreaDefinition.SingularName : this.InfoAreaId);
            }

            this.Organizer.SubtitleText = titleText;
            this.CopyRecordIdentification = this.ViewReference.ContextValueForKey("CopyRecordId");
            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                if (string.IsNullOrEmpty(this.CopyRecordIdentification))
                {
                    this.CopyRecordIdentification = this.LinkRecordIdentification;
                }

                string parentLink = this.ViewReference.ContextValueForKey("ParentLink");
                if (!string.IsNullOrEmpty(parentLink))
                {
                    this.OriginalLinkRecordIdentification = this.LinkRecordIdentification;
                    this.LinkReader = new UPCRMLinkReader(this.LinkRecordIdentification, parentLink, UPRequestOption.FastestAvailable, this);
                    this.LinkReader.Start();
                }
                else
                {
                    this.ContinueBuildOrganizerPagesForNew();
                }
            }
            else
            {
                this.ContinueBuildOrganizerPagesForNew();
            }
        }

        private void ContinueBuildOrganizerPagesForNew()
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            if (!string.IsNullOrEmpty(this.LinkRecordIdentification))
            {
                UPConfigTableCaption tableCaption = configStore.TableCaptionByName(this.LinkRecordIdentification.InfoAreaId());
                if (tableCaption != null)
                {
                    string recordTableCaption = tableCaption.TableCaptionForRecordIdentification(this.LinkRecordIdentification);
                    this.Organizer.SubtitleText += string.Concat(" - ", recordTableCaption);
                }
                else
                {
                    this.Organizer.SubtitleText += string.Concat(" - ", this.LinkRecordIdentification);
                }
            }

            string templateFilterName = this.ViewReference.ContextValueForKey("TemplateFilterName");
            if (!string.IsNullOrEmpty(templateFilterName))
            {
                UPConfigFilter templateFilterTemp = configStore.FilterByName(templateFilterName);
                //if (templateFilterTemp.NeedsLocation)
                //{
                //    //UPLocationProvider.Current().RequestLocationForObject(this);
                //    return;
                //}
            }

            this.ContinueTemplateFilterGeoCheckedWithInformUI(false);
        }

        /// <summary>
        /// Builds the edit organizer pages for edit.
        /// </summary>
        public void BuildEditOrganizerPagesForEdit()
        {
            string tableCaptionName;
            if (!IsRequestForChangeMode(this.ViewReference) || this.InfoAreaId == this.RecordIdentification.InfoAreaId())
            {
                tableCaptionName = this.ExpandConfig.TableCaptionName;
                if (string.IsNullOrEmpty(tableCaptionName))
                {
                    tableCaptionName = this.InfoAreaId;
                }
            }
            else
            {
                tableCaptionName = this.RecordIdentification.InfoAreaId();
            }

            if (IsRequestForChangeMode(this.ViewReference))
            {
                tableCaptionName = this.ViewReference.ContextValueForKey("TableCaption");
                if (string.IsNullOrEmpty(tableCaptionName))
                {
                    tableCaptionName = this.InfoAreaId;
                }
            }

            if (IsRequestForChangeMode(this.ViewReference))
            {
                this.ViewReference = this.UpdateViewReferenceForRequestForChange(this.ViewReference);
            }

            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            UPConfigTableCaption tableCaption = configStore.TableCaptionByName(tableCaptionName);
            string recordTableCaption = tableCaption?.TableCaptionForRecordIdentification(this.RecordIdentification);

            if (string.IsNullOrEmpty(recordTableCaption))
            {
                recordTableCaption = this.RecordIdentification;
            }

            this.Organizer.TitleText = recordTableCaption;
            string subTitleText = null;
            if (!string.IsNullOrEmpty(this.ExpandConfig.HeaderGroupName))
            {
                UPConfigHeader header = configStore.HeaderByNameFromGroup("Edit", this.ExpandConfig.HeaderGroupName);
                subTitleText = header?.Label;
            }

            if (string.IsNullOrEmpty(subTitleText))
            {
                InfoArea infoAreaConfig = ConfigurationUnitStore.DefaultStore.InfoAreaConfigById(this.InfoAreaId);
                subTitleText = infoAreaConfig?.SingularName;
                if (string.IsNullOrEmpty(subTitleText))
                {
                    subTitleText = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(this.InfoAreaId).Label;
                }
            }

            this.Organizer.SubtitleText = subTitleText;
            this.CopyRecordIdentification = this.ViewReference.ContextValueForKey("CopyRecordId");
            if (string.IsNullOrEmpty(this.CopyRecordIdentification))
            {
                this.CopyRecordIdentification = this.RecordIdentification;
            }

            FieldControl copyFieldControl = null;
            if (!string.IsNullOrEmpty(this.CopyRecordIdentification))
            {
                string copySourceFieldGroup = this.ViewReference.ContextValueForKey("CopySourceFieldGroupName");
                if (copySourceFieldGroup != null)
                {
                    copyFieldControl = configStore.FieldControlByNameFromGroup("List", copySourceFieldGroup);
                }
            }

            if (copyFieldControl != null)
            {
                this.CopyFields = new UPCopyFields(copyFieldControl);
                this.CopyFields.CopyFieldValuesForRecordIdentification(this.CopyRecordIdentification, false, this);
            }
            else
            {
                this.ContinueBuildEditOrganizerPagesForEdit(null);
            }
        }

        private void ContinueBuildEditOrganizerPagesForEdit(Dictionary<string, object> parameters)
        {
            this.CopyFieldDictionary = this.ParametersToCopyFieldDictionary(parameters);
            if (this.EditRecordRequest != null)
            {
                this.EditRecordRequest.RelatedInfoDictionary = this.CopyFieldDictionary;
            }

            if (IsRequestForChangeMode(this.ViewReference))
            {
                this.TemplateFilter = this.TemplateFilter.FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(this.CopyFieldDictionary));
            }

            this.AddOrganizerActions();
            this.AddPageModelControllers();
            this.Organizer.ExpandFound = true;
            this.TestDelegate?.OrganizerModelControllerFinishedWithPageModelController(this, this.FirstPageModelController);
        }

        /// <summary>
        /// Adds the page model controllers.
        /// </summary>
        public virtual void AddPageModelControllers()
        {
            Dictionary<string, object> additionalValues;
            if (this.CopyFieldDictionary != null)
            {
                additionalValues = new Dictionary<string, object>
                {
                    { "VirtualInfoAreaId", this.VirtualInfoAreaId },
                    { "CurrentExpandName", this.ExpandConfig.UnitName },
                    { "copyFields", this.CopyFieldDictionary }
                };
            }
            else
            {
                additionalValues = new Dictionary<string, object>
                {
                    { "VirtualInfoAreaId", this.VirtualInfoAreaId },
                    { "CurrentExpandName", this.ExpandConfig.UnitName }
                };
            }

            ViewReference pageViewReference = this.ViewReference.ViewReferenceWith(additionalValues);
            EditPageModelController editPageModelController = new EditPageModelController(pageViewReference, this.IsNew, this.InitialValueDictionary, this.EditRecordRequest, this.TestDelegate);
            this.FirstPageModelController = editPageModelController;
            Page overviewPage = editPageModelController.Page;
            if (editPageModelController.DisableRightActionItems || !(overviewPage.Status is UPMProgressStatus))
            {
                this.EnableActionItemsDisableActionItems(this.LeftNavigationBarItems, this.RightNaviagtionBarItems);
            }

            this.AddPageModelController(editPageModelController);
            this.Organizer.AddPage(overviewPage);
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            var editHeader = configStore.HeaderByNameFromGroup(this.IsNew ? "New" : "Update", this.ExpandConfig.HeaderGroupName) ??
                             configStore.HeaderByNameFromGroup("Edit", this.ExpandConfig.HeaderGroupName);

            if (editHeader?.SubViews != null && this.EditRecordRequest.RequestNr <= 0)
            {
                foreach (UPConfigHeaderSubView subView in editHeader.SubViews)
                {
                    if (this.CopyFieldDictionary != null)
                    {
                        pageViewReference = subView.ViewReference.ViewReferenceWith(new Dictionary<string, object>
                            {
                                { "copyFields", this.CopyFieldDictionary }
                            });
                    }
                    else
                    {
                        pageViewReference = subView.ViewReference;
                    }

                    var pageModelController = pageViewReference.ViewName == "RecordView"
                        ? new EditPageModelController(pageViewReference.ViewReferenceWith(this.RecordIdentification))
                        : this.PageForViewReference(pageViewReference.ViewReferenceWith(this.RecordIdentification));

                    if (pageModelController != null)
                    {
                        pageModelController.Page.Invalid = true;
                        pageModelController.Page.LabelText = subView.Label;
                        this.AddPageModelController(pageModelController);
                        this.Organizer.AddPage(pageModelController.Page);
                    }
                }
            }

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, new List<IIdentifier>{ new StringIdentifier("EditDataLoaded")}, null);
        }

        /// <summary>
        /// Adds the organizer actions.
        /// </summary>
        public virtual void AddOrganizerActions()
        {
            UPMOrganizerAction cancelAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.cancel"));
            cancelAction.SetTargetAction(this, this.Cancel);
            cancelAction.LabelText = LocalizedString.TextCancel;
            this.AddLeftNavigationBarActionItem(cancelAction);
            UPMOrganizerAction saveAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.save"));
            saveAction.SetTargetAction(this, this.Save);
            saveAction.LabelText = LocalizedString.TextSave;
            saveAction.MainAction = true;
            this.AddRightNavigationBarActionItem(saveAction);
            this.SaveActionItems.Add(saveAction);
            bool qrCodeEnabled = this.ViewReference.ContextValueIsSet("keyboardWithScannedSuggestions");
            if (qrCodeEnabled)
            {
                UPMOrganizerAction qrAction = new UPMOrganizerAction(StringIdentifier.IdentifierWithStringId("action.qrcode"));
                qrAction.SetTargetAction(this, this.ShowQrCode);
                qrAction.LabelText = LocalizedString.TextScanQrCode;
                qrAction.IconName = "Icon:Qrcode";
                this.AddOrganizerHeaderActionItem(qrAction);
            }
        }

        /// <summary>
        /// Shows the qr code.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public void ShowQrCode(object actionDictionary)
        {
            foreach (UPPageModelController pageModelController in this.PageModelControllers)
            {
                var editPageModelController = pageModelController as EditPageModelController;
                if (editPageModelController != null)
                {
                    editPageModelController.ShowQrCodeScanner();
                    break;
                }
            }
        }

        /// <summary>
        /// Pops to root content view controller.
        /// </summary>
        public void PopToRootContentViewController()
        {
            this.ModelControllerDelegate.PopToRootContentViewController();
        }

        /// <summary>
        /// Pops to previous content view controller.
        /// </summary>
        public virtual void PopToPreviousContentViewController()
        {
            if (this.PreviousOrganizerModelController?.ViewReference?.ViewName == "RecordView")
            {
                this.PreviousOrganizerModelController.UpdateOrganizer(this.PreviousOrganizerModelController.ViewReference);
            }

            this.ModelControllerDelegate?.PopToPreviousContentViewController();
        }

        /// <summary>
        /// Changeds the records for root.
        /// </summary>
        /// <param name="userChangesOnly">if set to <c>true</c> [user changes only].</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecordsForRoot(bool userChangesOnly)
        {
            Dictionary<string, UPEditFieldContext> changedFields = null;
            Dictionary<string, UPCRMLink> changedLinks = null;
            var infoAreaId = this.InfoAreaId;
            if (IsRequestForChangeMode(this.ViewReference))
            {
                infoAreaId = this.RfcInfoAreaId;
            }

            if (!userChangesOnly && this.InitialValueDictionary != null)
            {
                changedFields = this.ChangedRecordsForRootIdentificationFields(infoAreaId, changedFields);
            }

            var removeDisplayedSubTables = true;
            var applyChangedFields = false;
            UPConfigFilter templateFilter;
            if (this.SubRecordTemplateFilter != null)
            {
                templateFilter = this.SubRecordTemplateFilter;
                removeDisplayedSubTables = false;
                applyChangedFields = true;
            }
            else
            {
                templateFilter = this.TemplateFilter;
            }

            foreach (var modelController in PageModelControllers)
            {
                if (!(modelController is EditPageModelController))
                {
                    continue;
                }

                var editModelController = (EditPageModelController)modelController;
                if (removeDisplayedSubTables)
                {
                    templateFilter = editModelController.FilterByRemovingDisplayedSubTables(templateFilter);
                }

                var changedFieldsOnPage = GetChangedFields(userChangesOnly, editModelController, ref changedFields);

                if (applyChangedFields)
                {
                    templateFilter = ApplyChangedFields(changedFieldsOnPage, templateFilter);
                }

                changedLinks = GetChangedLinks(userChangesOnly, editModelController, changedLinks);
            }

            UPCRMRecord parentRecord = null;
            List<UPCRMRecord> additionalRecords = null;
            var record = GetUpcrmRecord(templateFilter, applyChangedFields, ref parentRecord, ref additionalRecords);

            if (changedFields != null)
            {
                ChangedRecordsForRootChangedFields(changedFields, record);
            }

            ChangedRecordsForRootChangedLinks(changedLinks, record);

            return GetRecordsForRootResult(userChangesOnly, changedFields, changedLinks, parentRecord, record, additionalRecords);
        }

        private List<UPCRMRecord> GetRecordsForRootResult(
            bool userChangesOnly,
            ICollection changedFields,
            ICollection changedLinks,
            UPCRMRecord parentRecord,
            UPCRMRecord record,
            IReadOnlyCollection<UPCRMRecord> additionalRecords)
        {
            if (IsNew && !userChangesOnly || changedFields?.Count > 0 || changedLinks?.Count > 0)
            {
                var returnArray = parentRecord != null
                    ? new List<UPCRMRecord> { parentRecord, record }
                    : new List<UPCRMRecord> { record };

                if (additionalRecords?.Count > 0)
                {
                    var fullArray = new List<UPCRMRecord>();
                    fullArray.AddRange(returnArray);
                    fullArray.AddRange(additionalRecords);
                    return fullArray;
                }

                return returnArray;
            }
            return new List<UPCRMRecord>();
        }

        private UPCRMRecord GetUpcrmRecord(
            UPConfigFilter templateFilter,
            bool applyChangedFields,
            ref UPCRMRecord parentRecord,
            ref List<UPCRMRecord> additionalRecords)
        {
            UPCRMRecord record;
            if (this.IsNew)
            {
                record = ChangedRecordsForRootNewRecords(
                    templateFilter,
                    applyChangedFields,
                    ref parentRecord,
                    ref additionalRecords);
            }
            else if (!string.IsNullOrWhiteSpace(RfcRecordIdentification))
            {
                record = new UPCRMRecord(RfcRecordIdentification);
            }
            else
            {
                record = new UPCRMRecord(RecordIdentification);
            }

            return record;
        }

        private static Dictionary<string, UPEditFieldContext> GetChangedFields(
            bool userChangesOnly,
            EditPageModelController editModelController,
            ref Dictionary<string, UPEditFieldContext> changedFields)
        {
            var changedFieldsOnPage = editModelController.ChangedFields(userChangesOnly);
            if (changedFieldsOnPage != null)
            {
                if (changedFields == null)
                {
                    changedFields = changedFieldsOnPage;
                }
                else
                {
                    foreach (var context in changedFieldsOnPage.Values)
                    {
                        changedFields.SetObjectForKey(context, context.Key);
                    }
                }
            }
            return changedFieldsOnPage;
        }

        private UPConfigFilter ApplyChangedFields(
            Dictionary<string, UPEditFieldContext> changedFieldsOnPage,
            UPConfigFilter templateFilter)
        {
            var changedValuesWithFunctionNames = new Dictionary<string, object>(SubRecordTemplateFilterParameters);
            foreach (var context in changedFieldsOnPage.Values)
            {
                if (!string.IsNullOrWhiteSpace(context.FieldConfig.Function))
                {
                    var contextValue = context.Value;
                    if (contextValue != null)
                    {
                        changedValuesWithFunctionNames.SetObjectForKey(contextValue, context.FieldConfig.Function);
                    }
                }
            }
            templateFilter = templateFilter.FilterByApplyingValueDictionaryDefaults(changedValuesWithFunctionNames, true);
            return templateFilter;
        }

        private static Dictionary<string, UPCRMLink> GetChangedLinks(
            bool userChangesOnly,
            EditPageModelController editModelController,
            Dictionary<string, UPCRMLink> changedLinks)
        {
            var changedLinksOnPage = editModelController.ChangedLinks(userChangesOnly);
            if (changedLinksOnPage != null)
            {
                if (changedLinks == null)
                {
                    changedLinks = new Dictionary<string, UPCRMLink>(changedLinksOnPage);
                }
                else
                {
                    foreach (var linkKey in changedLinksOnPage.Keys)
                    {
                        changedLinks.SetObjectForKey(changedLinksOnPage[linkKey], linkKey);
                    }
                }
            }

            return changedLinks;
        }

        private static void ChangedRecordsForRootChangedLinks(Dictionary<string, UPCRMLink> changedLinks, UPCRMRecord record)
        {
            if (changedLinks == null)
            {
                return;
            }

            foreach (var linkName in changedLinks.Keys)
            {
                var link = changedLinks[linkName];
                if (!string.IsNullOrWhiteSpace(link.RecordIdentification.RecordId()))
                {
                    record.AddLink(link);
                }
            }
        }

        private Dictionary<string, UPEditFieldContext> ChangedRecordsForRootIdentificationFields(
            string infoAreaId,
            Dictionary<string, UPEditFieldContext> changedFields)
        {
            if (InitialValueDictionary != null)
            {
                foreach (var fieldIdentification in InitialValueDictionary.Keys)
                {
                    if (fieldIdentification.StartsWith("."))
                    {
                        continue;
                    }

                    var fieldId = fieldIdentification.FieldIdFromStringWithInfoAreaId(infoAreaId);
                    if (fieldId >= 0)
                    {
                        var initialValueEditField =
                            new UPEditFieldContext(fieldId, InitialValueDictionary[fieldIdentification] as string);
                        if (changedFields == null)
                        {
                            changedFields = new Dictionary<string, UPEditFieldContext>
                                { { initialValueEditField.Key, initialValueEditField } };
                        }
                        else
                        {
                            changedFields[initialValueEditField.Key] = initialValueEditField;
                        }
                    }
                }
            }

            return changedFields;
        }

        private UPCRMRecord ChangedRecordsForRootNewRecords(
            UPConfigFilter templateFilter,
            bool applyChangedFields,
            ref UPCRMRecord parentRecord,
            ref List<UPCRMRecord> additionalRecords)
        {
            UPCRMRecord record;
            if (IsRequestForChangeMode(ViewReference))
            {
                parentRecord = UPCRMRecord.CreateNew(RfcRootInfoAreaId);
                if (TemplateFilter != null)
                {
                    parentRecord.ApplyValuesFromTemplateFilter(TemplateFilter);
                }

                parentRecord.AddLink(new UPCRMLink(RecordIdentification));
                record = UPCRMRecord.CreateNew(RfcInfoAreaId);
                record.AddLink(new UPCRMLink(parentRecord, -1));
                record.AddLink(new UPCRMLink(RecordIdentification));
            }
            else
            {
                if (EditRecordRequest?.Records?.Count > 0)
                {
                    record = new UPCRMRecord(EditRecordRequest.FirstRecord.RecordIdentification, "New", null);
                }
                else
                {
                    record = UPCRMRecord.CreateNew(InfoAreaId);
                    if (TemplateFilter != null)
                    {
                        record.ApplyValuesFromTemplateFilter(TemplateFilter);
                    }
                }

                if (!string.IsNullOrWhiteSpace(LinkRecordIdentification))
                {
                    record.AddLink(new UPCRMLink(LinkRecordIdentification, LinkId));
                }

                if (!string.IsNullOrWhiteSpace(LinkRecordIdentification2)
                    && (LinkId != LinkId2 || LinkRecordIdentification2 != LinkRecordIdentification))
                {
                    record.AddLink(new UPCRMLink(LinkRecordIdentification2, LinkId2));
                }

                UPConfigFilter createFilter = null;
                if (templateFilter?.RootTable.NumberOfSubTables > 0)
                {
                    createFilter = !applyChangedFields
                        ? templateFilter.FilterByRemovingMandatorySubTables()
                        : templateFilter;
                    templateFilter = templateFilter.FilterByRemovingRootConditions();
                }

                if (templateFilter?.RootTable.NumberOfSubTables == 0)
                {
                    createFilter = ConfigurationUnitStore.DefaultStore.FilterByName(
                        $"{record.InfoAreaId}.ChildCreateTemplate");
                    createFilter = createFilter?.FilterByApplyingDefaultReplacements();
                }

                additionalRecords = record.ApplyValuesFromTemplateFilter(createFilter, false);
            }

            return record;
        }

        private void ChangedRecordsForRootChangedFields(
            Dictionary<string, UPEditFieldContext> changedFields,
            UPCRMRecord record)
        {
            foreach (var changedField in changedFields.Values)
            {
                UPCRMFieldValue fieldValue;
                if (IsNew)
                {
                    fieldValue = record.NewValueFieldId(changedField.Value, changedField.FieldId);
                }
                else
                {
                    var multilineField = changedField.EditField as UPMMultilineEditField;
                    if (multilineField != null)
                    {
                        fieldValue = record.NewValueFromValueFieldId(
                            changedField.Value,
                            multilineField.Html
                                ? changedField.OriginalValue.StripHtml()
                                : changedField.OriginalValue,
                            changedField.FieldId);
                    }
                    else
                    {
                        fieldValue = record.NewValueFromValueFieldId(
                            changedField.Value,
                            changedField.OriginalValue,
                            changedField.FieldId);
                    }
                }

                if (!string.IsNullOrWhiteSpace(changedField.DateOriginalValue))
                {
                    fieldValue.DateOriginalValue = changedField.DateOriginalValue;
                }
                else if (!string.IsNullOrWhiteSpace(changedField.TimeOriginalValue))
                {
                    fieldValue.TimeOriginalValue = changedField.TimeOriginalValue;
                }
            }
        }

        private UPCRMRecord ChangedRootRecord(bool userChangesOnly)
        {
            List<UPCRMRecord> changedRecordsForRoot = this.ChangedRecordsForRoot(userChangesOnly);
            return changedRecordsForRoot.Count > 0 ? changedRecordsForRoot[0] : null;
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <param name="userChangesOnly">if set to <c>true</c> [user changes only].</param>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords(bool userChangesOnly)
        {
            List<UPCRMRecord> changedRecords = new List<UPCRMRecord>(this.ChangedRecordsForRoot(userChangesOnly));
            UPCRMRecord record;
            bool noChanges;
            if (changedRecords.Count > 0)
            {
                noChanges = false;
                record = changedRecords[0];
            }
            else
            {
                record = new UPCRMRecord(!string.IsNullOrEmpty(this.RfcRecordIdentification) ? this.RfcRecordIdentification : this.RecordIdentification);
                noChanges = true;
            }

            // Apply children without childEditGroup
            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                EditPageModelController editModelController = modelController as EditPageModelController;

                var changedChildRecordsOfPage = editModelController?.ChangedChildRecordsForParentRecord(record, userChangesOnly);

                if (changedChildRecordsOfPage?.Count > 0)
                {
                    noChanges = false;
                    changedRecords.AddRange(changedChildRecordsOfPage);
                }
            }

            if (noChanges)
            {
                return null;
            }

            string syncParentInfoAreaIdString = this.ViewReference.ContextValueForKey("SyncParentInfoAreaIds");
            if (!string.IsNullOrEmpty(syncParentInfoAreaIdString))
            {
                var syncParentInfoAreaIds = syncParentInfoAreaIdString.Split(',');
                foreach (string syncParentInfoAreaId in syncParentInfoAreaIds)
                {
                    var infoAreaIdParts = syncParentInfoAreaId.Split(':');
                    if (infoAreaIdParts.Length == 1)
                    {
                        changedRecords.Add(new UPCRMRecord(syncParentInfoAreaId, new UPCRMLink(record, -1)));
                    }
                    else if (infoAreaIdParts.Length > 1)
                    {
                        changedRecords.Add(new UPCRMRecord(infoAreaIdParts[0], new UPCRMLink(record, Convert.ToInt32(infoAreaIdParts[1]))));
                    }
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// Executes the saved action with record identification is new.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        public void ExecuteSavedActionWithRecordIdentification(string recordIdentification, bool isNew)
        {
            this.RemovePendingChanges();
            ViewReference viewReference = null;
            string passLinkRecordIdentification = this.LinkRecordIdentification;
            if (string.IsNullOrEmpty(passLinkRecordIdentification))
            {
                passLinkRecordIdentification = this.CopyRecordIdentification;
            }

            if (this.SaveViewReference != null)
            {
                viewReference = this.SaveViewReference.ViewReferenceWith(recordIdentification, passLinkRecordIdentification);
            }

            if (viewReference == null && isNew)
            {
                string createdInfoAreaId = recordIdentification.InfoAreaId();
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                InfoArea infoArea = configStore.InfoAreaConfigById(createdInfoAreaId);
                Menu menu = null;
                if (!string.IsNullOrEmpty(infoArea?.DefaultAction))
                {
                    menu = configStore.MenuByName(infoArea.DefaultAction);
                }

                if (menu == null)
                {
                    menu = configStore.MenuByName("SHOWRECORD");
                }

                viewReference = menu.ViewReference.ViewReferenceWith(recordIdentification, passLinkRecordIdentification);
            }
            else
            {
                if (viewReference != null &&
                    (viewReference.ViewName == "OrganizerAction" || viewReference.ViewName == "PhotoUploadAction" || viewReference.ViewName.StartsWith("Action:")))
                {
                    viewReference = viewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                    UPMultipleOrganizerManager.CurrentOrganizerManager.SetEditingForNavControllerId(false, this.NavControllerId);
                    this.PerformActionWithViewReference(viewReference);
                    return;
                }
            }

            if (this.PopToPrevious == false && viewReference != null)
            {
                UPOrganizerModelController organizerModelController = UPOrganizerModelController.OrganizerFromViewReference(viewReference);
                if (organizerModelController != null)
                {
                    if (this.ModelControllerDelegate.PopToNewOrganizerModelController(organizerModelController))
                    {
                        return;
                    }
                }
            }

            Messenger.Default.Send(SyncManagerMessage.Create(SyncManagerMessageKey.DidOfflineRequestsNumberChanged));
            if (this.PopToRootOnSave)
            {
                this.PopToRootContentViewController();
            }
            else
            {
                this.PopToPreviousContentViewController();
            }
        }

        /// <summary>
        /// Saves the specified action dictionary.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        public virtual void Save(object actionDictionary)
        {
            bool hasViolations = false;
            this.ModelControllerDelegate?.StopAllEditing();

            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                EditPageModelController editModelController = modelController as EditPageModelController;
                if (editModelController == null)
                {
                    continue;
                }

                if (editModelController.UpdatePageWithViolations())
                {
                    hasViolations = true;
                }
            }

            if (hasViolations)
            {
                return;
            }

            this.DisableAllActionItems(true);
            List<UPCRMRecord> changedRecords = this.ChangedRecords(false);
            if (changedRecords == null)
            {
                this.ExecuteSavedActionWithRecordIdentification(this.RecordIdentification, false);
                return;
            }

            changedRecords = UPCRMRecord.ArrayRemovingYieldRecordsFromArray(changedRecords);
            UPMProgressStatus stillLoadingError = new UPMProgressStatus(StringIdentifier.IdentifierWithStringId("savingIdentifier"));
            UPMStringField statusField = new UPMStringField(StringIdentifier.IdentifierWithStringId("statusFieldIdentifier"));
            statusField.FieldValue = LocalizedString.TextWaitForChanges;
            stillLoadingError.StatusMessageField = statusField;
            this.Organizer.Status = stillLoadingError;
            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, null, null);

            if (this.EditRecordRequest.RequestNr >= 0 && !this.EditRecordRequest.ApplicationRequest)
            {
                this.EditRecordRequest.StartRequest(UPOfflineRequestMode.OnlineOnly, changedRecords, this);
            }
            else
            {
                this.EditRecordRequest.TitleLine = this.Organizer.TitleText;
                this.EditRecordRequest.DetailsLine = this.Organizer.SubtitleText;
                this.EditRecordRequest.StartRequest(this.RequestMode, changedRecords, this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool HasChanges => this.ChangedRecords(true) != null;

        /// <summary>
        /// Cancels edit page
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        protected virtual async void Cancel(object actionDictionary)
        {
            if (!this.HasChanges)
            {
                this.PopToPreviousContentViewController();
            }
            else
            {
                await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                     LocalizedString.TextReallyAbortAndLoseChanges,
                     LocalizedString.TextYouMadeChanges,
                     LocalizedString.TextYes,
                     LocalizedString.TextNo,
                     c =>
                     {
                         if (c)
                         {
                             this.RemovePendingChanges();
                             if (this.CloseOrganizerDelegate != null)
                             {
                                 this.CloseOrganizerDelegate.UpOrganizerModelControllerAllowedClosingOrganizer(this);
                                 this.CloseOrganizerDelegate = null;
                             }
                             else
                             {
                                 this.ModelControllerDelegate.StopAllEditing();
                                 this.PopToPreviousContentViewController();
                             }
                         }
                     });
            }
        }

        /// <inheritdoc />
        public override void RemovePendingChanges()
        {
            base.RemovePendingChanges();
            if (this.PageModelControllers != null)
            {
                foreach (var pageController in this.PageModelControllers)
                {
                    if (pageController is EditPageModelController editPageModelController)
                    {
                        editPageModelController.RemovePendingChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Closes the organizer allowed with delegate.
        /// </summary>
        /// <param name="closeOrganizerDelegate">The close organizer delegate.</param>
        /// <returns></returns>
        public override bool CloseOrganizerAllowedWithDelegate(UPMCloseOrganizerDelegate closeOrganizerDelegate)
        {
            if (this.HasChanges)
            {
                this.CloseOrganizerDelegate = closeOrganizerDelegate;
                //UIAlertView alertview = new UIAlertView(upTextYouMadeChanges, upTextReallyAbortAndLoseChanges, this, upText_NO, upText_YES, null);
                //alertview.Show();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cancel the current changes
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> ConfirmCloseOrganizerAsync(Action<bool> callback = null)
        {
            var newChangedRecordCount = this.ChangedRecords(true)?.Count ?? 0;
            if (this.changedRecordsCount != newChangedRecordCount)
            {
                this.changedRecordsCount = newChangedRecordCount;

                // Reset isClosed if the user has again changed something new
                this.isClosed = false;
            }

            if (this.changedRecordsCount == 0 || this.isClosed)
            {
                return true;
            }

            var confirmCloseOrganizerAsync = await SimpleIoc.Default.GetInstance<IDialogService>().ShowMessage(
                LocalizedString.TextReallyAbortAndLoseChanges,
                LocalizedString.TextYouMadeChanges,
                LocalizedString.TextYes,
                LocalizedString.TextNo,
                callback);

            if (confirmCloseOrganizerAsync)
            {
                this.isClosed = true;
            }

            return confirmCloseOrganizerAsync;
        }

        /// <summary>
        /// The alternate expand checker did fail with error.
        /// </summary>
        /// <param name="expandChecker">The expand checker.</param>
        /// <param name="error">The error.</param>
        public void AlternateExpandCheckerDidFailWithError(UPCRMAlternateExpandChecker expandChecker, Exception error)
        {
            this.ContinueBuildEditOrganizerPages();
        }

        /// <summary>
        /// The alternate expand checker did finish with result.
        /// </summary>
        /// <param name="expandChecker">
        /// The expand checker.
        /// </param>
        /// <param name="expand">
        /// The expand.
        /// </param>
        public void AlternateExpandCheckerDidFinishWithResult(UPCRMAlternateExpandChecker expandChecker, UPConfigExpand expand)
        {
            this.ExpandConfig = expand;
            this.ContinueBuildEditOrganizerPages();
            if (this.TestDelegate == null)
            {
                this.InformAboutDidChangeTopLevelElement(this.Organizer, this.Organizer, null, UPChangeHints.ChangeHintsWithHint("AlternateExpandFound"));
            }
        }

#if PORTING
        void AlertViewDidDismissWithButtonIndex(UIAlertView alertView, int buttonIndex)
        {
            if (buttonIndex != 0)
            {
                if (this.CloseOrganizerDelegate != null)
                {
                    this.CloseOrganizerDelegate.UpOrganizerModelControllerAllowedClosingOrganizer(this);
                    this.CloseOrganizerDelegate = null;
                }
                else
                {
                    this.ModelControllerDelegate.StopAllEditing();
                    this.PopToPreviousContentViewController();
                }
            }
        }
#endif

        /// <summary>
        /// Offlines the request data online context did finish with result.
        /// </summary>
        /// <param name="request">The <see cref="UPOfflineRequest"/> request.</param>
        /// <param name="data">The data.</param>
        /// <param name="online">if set to <c>true</c> [online].</param>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OfflineRequestDidFinishWithResult(UPOfflineRequest request, object data, bool online, object context, Dictionary<string, object> result)
        {
            PopulateUploadDocumentRequests(request, data);

            if ((request == EditRecordRequest || request == UploadDocumentRequest) && UploadDocumentRequests.Any())
            {
                UploadDocumentRequest = UploadDocumentRequests[0];
                UploadDocumentRequests.Remove(UploadDocumentRequest);
                var requestAccepted = UploadDocumentRequest.StartRequest(UPOfflineRequestMode.OnlineConfirm, this);
                //if (requestAccepted)
                //{
                //    return;
                //}
            }

            var records = EditRecordRequestData;
            var count = records?.Count ?? 0;
            var changes = new List<IIdentifier>(count);
            var firstRecordId = EditRecordRequestData?[0].RecordIdentification;
            var originalRecordChanged = SettingValueTransferContext(records, changes, firstRecordId);

            if (!originalRecordChanged)
            {
                changes.Add(new RecordIdentifier(InfoAreaId, RecordIdentification.RecordId()));
                var record = records[0];

                if (record?.Links != null)
                {
                    changes.AddRange(record.Links.Select(link => new RecordIdentifier(link.RecordIdentification)));
                }
            }

            if (!string.IsNullOrWhiteSpace(LinkRecordIdentification))
            {
                changes.Add(new RecordIdentifier(LinkRecordIdentification));
            }

            if (!string.IsNullOrWhiteSpace(OriginalLinkRecordIdentification) && OriginalLinkRecordIdentification != LinkRecordIdentification)
            {
                changes.Add(new RecordIdentifier(OriginalLinkRecordIdentification));
            }

            if (ViewReference.ContextValueIsSet(RefreshFilterTokensLiteral))
            {
                ServerSession.CurrentSession.ResetSessionVariables();
            }

            UPChangeManager.CurrentChangeManager.RegisterChanges(changes);
            if (IsNew && !IsRequestForChangeMode(ViewReference))
            {
                ExecuteSavedActionWithRecordIdentification(firstRecordId, true);
            }
            else
            {
                ExecuteSavedActionWithRecordIdentification(RecordIdentification, false);
            }
        }

        /// <summary>
        /// Offlines the request data context did fail with error.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public override void OfflineRequestDidFailWithError(UPOfflineRequest request, object data, object context, Exception error)
        {
            this.DisableAllActionItems(false);
            this.Organizer.Status = null;
            this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
        }

        private static bool IsRequestForChangeMode(ViewReference viewReference)
        {
            return viewReference.ContextValueForKey("Mode") == "RequestForChange";
        }

        private Dictionary<string, object> FillDictionaryFromSourceFieldGroup(FieldControl sourceFieldGroup, FieldControl destinationFieldGroup, List<string> sourceResultValues)
        {
            Dictionary<string, object> filledDictionary = new Dictionary<string, object>();
            Dictionary<string, UPConfigFieldControlField> sourceFunctionDictionary = sourceFieldGroup.FunctionNames();
            Dictionary<string, UPConfigFieldControlField> destinationFunctionDictionary = destinationFieldGroup.FunctionNames();
            if (destinationFunctionDictionary != null)
            {
                if (sourceFunctionDictionary != null)
                {
                    foreach (string sourceFieldFunction in sourceFunctionDictionary.Keys)
                    {
                        UPConfigFieldControlField sourceField = sourceFunctionDictionary.ValueOrDefault(sourceFieldFunction);
                        UPConfigFieldControlField destinationField = destinationFunctionDictionary.ValueOrDefault(sourceFieldFunction);
                        if (destinationField != null)
                        {
                            filledDictionary[destinationField.Field.FieldIdentification] = sourceResultValues[sourceField.TabIndependentFieldIndex];
                        }

                        destinationField = destinationFunctionDictionary.ValueOrDefault($"Source:{sourceFieldFunction}");
                        if (destinationField != null)
                        {
                            filledDictionary[destinationField.Field.FieldIdentification] = sourceResultValues[sourceField.TabIndependentFieldIndex];
                        }
                    }
                }
            }

            return filledDictionary;
        }

        /// <summary>
        /// The link reader did finish with result.
        /// </summary>
        /// <param name="linkReader">The link reader.</param>
        /// <param name="result">The result.</param>
        public void LinkReaderDidFinishWithResult(UPCRMLinkReader linkReader, object result)
        {
            if (!string.IsNullOrEmpty(linkReader.DestinationRecordIdentification))
            {
                this.LinkRecordIdentification = linkReader.DestinationRecordIdentification;
                this.ContinueBuildOrganizerPagesForNew();
            }
        }

        /// <summary>
        /// Links the reader did finish with error.
        /// </summary>
        /// <param name="_linkReader">The link reader.</param>
        /// <param name="error">The error.</param>
        public void LinkReaderDidFinishWithError(UPCRMLinkReader _linkReader, Exception error)
        {
            SimpleIoc.Default.GetInstance<ILogger>().LogError($"New Record: linkReader {_linkReader.ParentLinkString} for record {_linkReader.SourceRecordIdentification} returned error {error.Message} -> ignore & continue");
            this.ContinueBuildOrganizerPagesForNew();
        }

        /// <summary>
        /// Links the only online available.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="linkTargetInfoAreaId">
        /// The link target information area identifier.
        /// </param>
        /// <param name="linklinkId">
        /// The linklink identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool LinkOnlyOnlineAvailable(object context, string linkTargetInfoAreaId, int linklinkId)
        {
            if (string.IsNullOrEmpty(this.SenderLinkForInfoAreaIdLinkId(this, linkTargetInfoAreaId, linklinkId)))
            {
                string parentLinkString = linklinkId > 0 ? $"{linkTargetInfoAreaId}:{linklinkId}" : linkTargetInfoAreaId;
                UPCRMLinkReader linkreader = new UPCRMLinkReader(this.RecordIdentification, parentLinkString, UPRequestOption.Offline, null);
                if (!string.IsNullOrEmpty(linkreader.RequestLinkRecordOffline()))
                {
                    return !UPCRMDataStore.DefaultStore.RecordExistsOffline(linkreader.RequestLinkRecordOffline());
                }
            }

            return false;
        }

        /// <summary>
        /// Senders the link for information area identifier link identifier.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <returns></returns>
        public string SenderLinkForInfoAreaIdLinkId(object sender, string infoAreaId, int linkId)
        {
            UPCRMRecord changedRecord = this.ChangedRootRecord(false);
            UPCRMLink link = changedRecord?.LinkWithInfoAreaIdLinkId(infoAreaId, linkId);
            if (link != null)
            {
                return link.RecordIdentification;
            }

            foreach (UPPageModelController modelController in this.PageModelControllers)
            {
                var editPageModelController = modelController as EditPageModelController;
                if (editPageModelController != null)
                {
                    string linkRecordIdentification = ((EditPageModelController)modelController).LinkRecordIdentificationForInfoAreaIdLinkId(infoAreaId, linkId);
                    if (!string.IsNullOrEmpty(linkRecordIdentification))
                    {
                        return linkRecordIdentification;
                    }
                }
            }

            return null;
        }

#if PORTING
        void LocationProviderDidProvideLocation(UPLocationProvider locationProvider, CLLocation location)
        {
            this.ContinueTemplateFilterGeoCheckedWithInformUI(true);
        }

        void LocationProviderDidFinishWithError(UPLocationProvider locationProvider, Exception error)
        {
            DDLogCError("error from geo location service: %@", error);
            this.ContinueTemplateFilterGeoCheckedWithInformUI(true);
        }
#endif

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public override void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.AlternateExpandChecker = new UPCRMAlternateExpandChecker(this.RecordIdentification, this.ExpandConfig, this);
            this.AlternateExpandChecker.Start(UPRequestOption.FastestAvailable);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="sourceResult">The source result.</param>
        public override void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult sourceResult)
        {
            this.InitialValueDictionary = null;
            string configExpandName = this.ViewReference.ContextValueForKey("ExpandName");
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            this.RfcRootInfoAreaId = this.ViewReference.ContextValueForKey(Constants.RfcRootInfoAreaIdConfigurationName);
            this.RfcInfoAreaId = this.ViewReference.ContextValueForKey(Constants.RfcInfoAreaIdConfigurationName);
            UPConfigExpand configExpand = configStore.ExpandByName(configExpandName);
            this.FieldControl = configStore.FieldControlByNameFromGroup("Edit", configExpand.FieldGroupName);
            this.RfcRecordIdentification = this.LoadRecordIdentificationForRequestForChangeInfoArea(this.FieldControl, this.RfcInfoAreaId);
            UPCRMResultRow sourceResultRow = sourceResult.RowCount > 0 ? (UPCRMResultRow)sourceResult.ResultRowAtIndex(0) : null;

            if (this.IsNew)
            {
                string sourceCopyFieldGroupName = this.ViewReference.ContextValueForKey(Constants.RfcSourceFieldGroupConfigurationName);
                FieldControl sourceFieldControl = configStore.FieldControlByNameFromGroup("Edit", sourceCopyFieldGroupName);
                this.InitialValueDictionary = this.FillDictionaryFromSourceFieldGroup(sourceFieldControl, this.FieldControl, sourceResultRow?.RawValues());
                this.EditRecordRequest.RelatedInfoDictionary = this.InitialValueDictionary;
            }

            this.AlternateExpandChecker = new UPCRMAlternateExpandChecker(this.RecordIdentification, this.ExpandConfig, this);
            this.AlternateExpandChecker.Start(UPRequestOption.FastestAvailable);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public override void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public override void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public override void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates List of <see cref="UPOfflineUploadDocumentRequest"/> from data for EditRecordRequest
        /// </summary>
        /// <param name="request">
        /// The <see cref="UPOfflineRequest"/> request.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        private void PopulateUploadDocumentRequests(UPOfflineRequest request, object data)
        {
            if (request == EditRecordRequest)
            {
                EditRecordRequestData = data as List<UPCRMRecord>;
                var record = EditRecordRequestData?[0];
                var firstRecordIdentification = record?.RecordIdentification;
                UploadDocumentRequests = new List<UPOfflineUploadDocumentRequest>();

                foreach (var modelController in PageModelControllers)
                {
                    var editPageModelController = modelController as EditPageModelController;
                    if (editPageModelController != null)
                    {
                        var editPage = (EditPage)editPageModelController.Page;
                        foreach (var editGroup in editPage.Groups)
                        {
                            if (editGroup is UPMContactTimesGroup)
                            {
                                continue;
                            }

                            foreach (var editField in editGroup.Fields)
                            {
                                if (editField is UPMImageEditField)
                                {
                                    var imageEditField = (UPMImageEditField)editField;
                                    if (imageEditField != null && imageEditField.Changed)
                                    {
                                        var fileName = !string.IsNullOrWhiteSpace(imageEditField.ExplicitFileName)
                                            ? imageEditField.ExplicitFileName
                                            : "photo.jpg";

                                        UploadDocumentRequest = new UPOfflineUploadDocumentRequest(
                                            imageEditField.Image,
                                            -1,
                                            fileName,
                                            "image/jpeg",
                                            firstRecordIdentification,
                                            ((FieldIdentifier)imageEditField.Identifier).FieldId.FieldIdFromFieldIdentification(),
                                            "true");

                                        UploadDocumentRequests.Add(UploadDocumentRequest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method sets Value Transfer in <see cref="UPAppProcessContext"/>
        /// </summary>
        /// <param name="records">
        /// List of <see cref="UPCRMRecord"/>
        /// </param>
        /// <param name="changes">
        /// List of <see cref="IIdentifier"/>
        /// </param>
        /// <param name="firstRecordId">
        /// First Record Id
        /// </param>
        /// <returns>
        /// Is original record changed
        /// </returns>
        private bool SettingValueTransferContext(List<UPCRMRecord> records, List<IIdentifier> changes, string firstRecordId)
        {
            var count = records?.Count ?? 0;
            var originalRecordChanged = false;
            var valueTransferRequestDict = UPAppProcessContext.CurrentContext.ContextValueForKey(
                UPAppProcessContext.AppContextEditFieldValueTransferRequest,
                NavControllerId) as Dictionary<string, string>;
            var valueTransferResponseDict = new Dictionary<string, object>();

            for (var i = 0; i < count; i++)
            {
                var record = records[i];
                changes.Add(new RecordIdentifier(record.InfoAreaId, record.RecordId));
                if (!string.IsNullOrWhiteSpace(record.OriginalRecordIdentification) && record.RecordIdentification != record.OriginalRecordIdentification)
                {
                    changes.Add(new RecordIdentifier(record.OriginalRecordIdentification.InfoAreaId(), record.OriginalRecordIdentification.RecordId()));
                }

                if (record.RecordIdentification == RecordIdentification)
                {
                    originalRecordChanged = true;
                }

                if (i == 0)
                {
                    firstRecordId = record.RecordIdentification;
                }

                if (valueTransferRequestDict != null)
                {
                    foreach (var transferKey in valueTransferRequestDict.Keys)
                    {
                        var fieldIdentifier = valueTransferRequestDict[transferKey];
                        var fieldInfoAreaId = fieldIdentifier.InfoAreaId();
                        var fieldId = fieldIdentifier.FieldIdFromFieldIdentification();
                        if (record.InfoAreaId == fieldInfoAreaId)
                        {
                            var value = record.StringFieldValueForFieldIndex(fieldId);
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                valueTransferResponseDict.SetObjectForKey(value, transferKey);
                            }
                        }
                    }
                }
            }

            if (valueTransferResponseDict.Any())
            {
                UPAppProcessContext.CurrentContext.SetContextValueForKey(
                    valueTransferResponseDict,
                    UPAppProcessContext.AppContextEditFieldValueTransferResponse,
                    NavControllerId);
            }

            return originalRecordChanged;
        }

        private bool HasConfigNameHasWildCards(IConfigurationUnitStore configStore)
        {
            this.TopLevelElement = new UPMOrganizer(StringIdentifier.IdentifierWithStringId("Edit"));
            if (this.EditRecordRequest?.Records?.Count > 0)
            {
                this.RecordIdentification = this.EditRecordRequest.FirstRecord.RecordIdentification;
            }
            else
            {
                this.RecordIdentification = this.ViewReference.ContextValueForKey("RecordId");
                this.RecordIdentification = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(this.RecordIdentification);
            }

            this.ConfigName = this.ViewReference.ContextValueForKey("ExpandName") ??
                              this.ViewReference.ContextValueForKey("ConfigName");

            var configNameHasWildCards = false;
            if (!string.IsNullOrWhiteSpace(this.RecordIdentification))
            {
                this.InfoAreaId = this.RecordIdentification.InfoAreaId();
                configNameHasWildCards = this.ConfigName?.IndexOf("##") >= 0;
                if (string.IsNullOrWhiteSpace(this.ConfigName) || configNameHasWildCards)
                {
                    this.VirtualInfoAreaId = UPCRMDataStore.DefaultStore.VirtualInfoAreaIdForRecordIdentification(this.RecordIdentification);
                    if (!string.IsNullOrWhiteSpace(this.VirtualInfoAreaId) && !configNameHasWildCards)
                    {
                        this.ConfigName = this.VirtualInfoAreaId;
                        if (this.VirtualInfoAreaId != this.InfoAreaId)
                        {
                            this.AlternateConfigName = this.InfoAreaId;
                        }
                    }
                }
                else
                {
                    this.VirtualInfoAreaId = this.InfoAreaId;
                }
            }
            else
            {
                this.InfoAreaId = this.ViewReference.ContextValueForKey("InfoAreaId");
                if (string.IsNullOrWhiteSpace(this.InfoAreaId))
                {
                    UPConfigExpand expand = configStore.ExpandByName(this.ConfigName);
                    this.InfoAreaId = expand != null ? expand.InfoAreaId : this.ConfigName;
                }

                this.VirtualInfoAreaId = this.InfoAreaId;
                if (string.IsNullOrWhiteSpace(this.ConfigName))
                {
                    this.ConfigName = this.InfoAreaId;
                }
            }

            return configNameHasWildCards;
        }

        private void SetLinkRecordIdentification()
        {
            if (this.EditRecordRequest?.Records?.Count > 0)
            {
                UPCRMRecord record = this.EditRecordRequest.FirstRecord;
                if (record.IsNew && record.Links?.Count > 0)
                {
                    this.LinkRecordIdentification = record.Links[0].RecordIdentification;
                }
            }
            else
            {
                this.LinkRecordIdentification = this.ViewReference.ContextValueForKey("LinkRecordId") ?? this.ViewReference.ContextValueForKey("LinkRecord");
            }
        }

        private void SetLinkId()
        {
            string linkIdStr = this.ViewReference.ContextValueForKey("LinkId");
            this.LinkId = !string.IsNullOrWhiteSpace(linkIdStr) ? Convert.ToInt32(linkIdStr) : this.LinkId = -1;

            this.LinkRecordIdentification2 = this.ViewReference.ContextValueForKey("LinkRecordId2");
            if (!string.IsNullOrWhiteSpace(this.LinkRecordIdentification2))
            {
                linkIdStr = this.ViewReference.ContextValueForKey("LinkId2");
                this.LinkId2 = !string.IsNullOrWhiteSpace(linkIdStr) ? Convert.ToInt32(linkIdStr) : -1;
            }
        }

        private void SetExpandConfig(bool configNameHasWildCards, IConfigurationUnitStore configStore)
        {
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
                if (this.ExpandConfig == null && !string.IsNullOrWhiteSpace(this.AlternateConfigName))
                {
                    this.ExpandConfig = configStore.ExpandByName(this.AlternateConfigName);
                }
            }
        }
    }
}
