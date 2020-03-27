// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditPageModelController.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Services.ModelControllers.Edit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OfflineStorage;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.Services.ModelControllers.Search;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Contexts;
    using Aurea.CRM.UIModel.Fields.Edit;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// Model controller for a edit page
    /// </summary>
    /// <seealso cref="GroupBasedPageModelController" />
    /// <seealso cref="ISearchOperationHandler" />
    /// <seealso cref="UPRightsCheckerDelegate" />
    public class EditPageModelController : GroupBasedPageModelController, ISearchOperationHandler, UPRightsCheckerDelegate, IEditTableViewDataProvider
    {
        private const string ModeRequestForChange = "RequestForChange";
        private const string CONTROLS = "CONTROLS";
        private const string KeyboardWithCannedSuggestions = "keyboardWithScannedSuggestions";
        private const string INFOAREAID = nameof(InfoAreaId);
        private const string CONFIGNAME = nameof(ConfigName);

        protected string recordId;
        protected UPEditPageContext editPageContext;

        private bool isConfigEditControl;
        private UPRightsChecker rightsChecker;
        private Dictionary<string, UPCRMLink> changedLinks;
        private UPCRMResult rootRecordResult;
        private UPCRMResultRow rootRecordResultRow;
        private UPContainerMetaInfo containerMetaInfo;
        private Dictionary<string, object> vCardDownloads;
        private List<object> observedDownloads;

        private List<UPEditConstraintViolation> lastSignaledViolations;
        private Dictionary<string, object> initialValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="isNew">if set to <c>true</c> [is new].</param>
        /// <param name="_initialValues">The _initial values.</param>
        /// <param name="_offlineRequest">The _offline request.</param>
        /// <param name="_testDelegate">The _test delegate.</param>
        public EditPageModelController(ViewReference viewReference, bool isNew, Dictionary<string, object> _initialValues,
            UPOfflineEditRecordRequest _offlineRequest, IEditOrganizerModelControllerTestDelegate _testDelegate = null)
            : base(viewReference)
        {
            this.initialValues = _initialValues;
            this.IsNew = isNew;
            this.OfflineRequest = _offlineRequest;
            this.TestDelegate = _testDelegate;

            if (this.OfflineRequest != null && this.OfflineRequest.RequestNr >= 0)
            {
                this.RequestOption = UPRequestOption.Online;
            }
            else
            {
                var requestModeString = viewReference.ContextValueForKey("RequestMode");
                if (!string.IsNullOrEmpty(requestModeString))
                {
                    this.RequestOption = UPCRMDataStore.RequestOptionFromString(requestModeString, UPRequestOption.BestAvailable);
                }
                else if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Edit.BestAvailable"))
                {
                    this.RequestOption = UPRequestOption.BestAvailable;
                }
                else
                {
                    this.RequestOption = UPRequestOption.FastestAvailable;
                }
            }

            this.changedLinks = null;
            this.vCardDownloads = new Dictionary<string, object>();
            this.observedDownloads = new List<object>();
            this.BuildPage();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public EditPageModelController(ViewReference viewReference)
            : base(viewReference)
        {
            var requestModeString = viewReference.ContextValueForKey("RequestMode");
            if (string.IsNullOrEmpty(requestModeString))
            {
                // PVCS 84491 Fallback for ContactTimesEditView RequestOption
                requestModeString = viewReference.ContextValueForKey("RequestOption");
            }

            if (!string.IsNullOrEmpty(requestModeString))
            {
                this.RequestOption = UPCRMDataStore.RequestOptionFromString(requestModeString, UPRequestOption.BestAvailable);
            }
            else if (ConfigurationUnitStore.DefaultStore.ConfigValueIsSet("Edit.BestAvailable"))
            {
                this.RequestOption = UPRequestOption.BestAvailable;
            }
            else
            {
                this.RequestOption = UPRequestOption.FastestAvailable;
            }
        }

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Gets the edit page.
        /// </summary>
        /// <value>
        /// The edit page.
        /// </value>
        public EditPage EditPage => (EditPage)this.Page;

        /// <summary>
        /// Gets a value indicating whether this instance is process changes.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process changes; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsProcessChanges => false;

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        public bool IsNew { get; private set; }

        /// <summary>
        /// Gets or sets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; protected set; }

        /// <summary>
        /// Gets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [disable right action items].
        /// </summary>
        /// <value>
        /// <c>true</c> if [disable right action items]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableRightActionItems { get; private set; }

        /// <summary>
        /// Gets the client check filter.
        /// </summary>
        /// <value>
        /// The client check filter.
        /// </value>
        public UPConfigFilter ClientCheckFilter { get; private set; }

        /// <summary>
        /// Gets the test delegate.
        /// </summary>
        /// <value>
        /// The test delegate.
        /// </value>
        public IEditOrganizerModelControllerTestDelegate TestDelegate { get; private set; }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public UPOfflineEditRecordRequest OfflineRequest { get; private set; }

#if PORTING
        public UPCRMEditTrigger EditTrigger { get; private set; }
#else
        // TODO: Replace these with above properties after porting them
        public object EditTrigger { get; private set; }
#endif

        /// <summary>
        /// Gets the group count.
        /// </summary>
        /// <value>
        /// The group count.
        /// </value>
        public int GroupCount => this.Page?.Groups?.Count ?? 0;

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">The list of identifiers.</param>
        /// <param name="appliedIdentifiers">The applied identifiers.</param>
        public override void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
            if (this.IsProcessChanges)
            {
                base.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            }
        }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public override Page InstantiatePage()
        {
            return this.Page != null ?
                new EditPage(this.Page.Identifier) :
                new EditPage(FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(this.InfoAreaId, this.recordId, "Page0"));
        }

        #region RightsCheckerDelegate
        /// <summary>
        /// Rightses the checker grants permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void RightsCheckerGrantsPermission(UPRightsChecker sender, string recordIdentification)
        {
            var tabCount = this.FieldControl.NumberOfTabs;
            this.editPageContext = new UPEditPageContext(this.InfoAreaId.InfoAreaIdRecordId(this.recordId), this.IsNew, this.initialValues, this.OfflineRequest, this.ViewReference)
            {
                EditTrigger = this.EditTrigger
            };

            for (var i = 0; i < tabCount; i++)
            {
                var groupModelController = UPFieldControlBasedEditGroupModelController.EditGroupModelControllerFor(this.FieldControl, i, this.editPageContext, this);
                if (groupModelController == null)
                {
                    continue;
                }

                // no idea why request option parameter is ignored here, but a change would potentially be a breaking change of logic for existing customers
                groupModelController.RequestOption = this.TestDelegate != null ? this.RequestOption : UPRequestOption.BestAvailable;

                this.GroupModelControllerArray.Add(groupModelController);
            }

            // wenn es keinen rightsChecker gibt, so wird diese Methode trotzdem aufgerufen.
            // Aber es kommt durch die fehlende AsyncronitÃ¤t nicht zum Fehler aus pvcs #86709 und die Seite muss nicht erneut upgedated werden
            if (sender != null && this.rootRecordResult != null)
            {
                this.UpdatePageWithResult(this.rootRecordResult);
            }
            else if (this.TestDelegate != null)
            {
                this.UpdatedElementForPage(this.Page);
            }
        }

        /// <summary>
        /// Rightses the checker revoke permission.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void RightsCheckerRevokePermission(UPRightsChecker sender, string recordIdentification)
        {
            this.DisableRightActionItems = true;
            this.HandlePageMessageDetails("ErrorActionNotAllowed", sender?.ForbiddenMessage);
        }

        /// <summary>
        /// Rightses the checker did finish with error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void RightsCheckerDidFinishWithError(UPRightsChecker sender, Exception error)
        {
            this.RightsCheckerRevokePermission(sender, null);
        }
        #endregion

        /// <summary>
        /// Builds the page.
        /// </summary>
        public override void BuildPage()
        {
            var infoAreadIdContextName = INFOAREAID;
            var configNameContextName = CONFIGNAME;
            var expandConfigurationContextName = "CurrentExpandName";
            var mode = this.ViewReference?.ContextValueForKey("Mode");
            var recordIdContextName = "RecordId";
            SetValuesForRequestForChange(
                mode,
                ref recordIdContextName,
                ref infoAreadIdContextName,
                ref configNameContextName,
                ref expandConfigurationContextName);
            this.SetRecordId(recordIdContextName, infoAreadIdContextName, configNameContextName);

            var qrCodeEnabled = this.ViewReference?.ContextValueIsSet(KeyboardWithCannedSuggestions) ?? false;

            var qrCodeControlsEnabled = false;
            var qrCodeBeepEnabled = true;
            this.GetQrCodeValues(ref qrCodeControlsEnabled, ref qrCodeBeepEnabled);

            if (this.recordId.IsRecordIdentification())
            {
                if (string.IsNullOrWhiteSpace(this.InfoAreaId))
                {
                    this.InfoAreaId = this.recordId.InfoAreaId();
                }

                this.recordId = this.recordId.RecordId();
            }

            if (string.IsNullOrWhiteSpace(this.ConfigName))
            {
                this.ConfigName = this.InfoAreaId;
            }

            var configStore = ConfigurationUnitStore.DefaultStore;

            var page = this.GetEditPage(qrCodeEnabled, qrCodeBeepEnabled, qrCodeControlsEnabled, configStore);

            this.TopLevelElement = page;

            var expandConfig = this.GetConfigExpand(configStore, expandConfigurationContextName);
            if (expandConfig == null)
            {
                return;
            }

            this.GetFieldControl(expandConfig, configStore);
            if (this.FieldControl == null)
            {
                this.TestDelegate?.PageModelControllerDidFailWithError(this, new Exception("No field control"));
                return;
            }

            this.SetClientCheckFilter(configStore);
            if (!this.GetRightsChecker(configStore, page))
            {
                return;
            }

            this.RightsCheckerGrantsPermission(null, this.recordId);
        }

        /// <summary>
        /// Drawables the page model controller.
        /// </summary>
        /// <param name="signalFinished">if set to <c>true</c> [signal finished].</param>
        /// <returns></returns>
        public override bool DrawablePageModelController(bool signalFinished)
        {
            if (this.TestDelegate == null)
            {
                return true;
            }

            if (!signalFinished)
            {
                return false;
            }

            this.TestDelegate.PageModelControllerFinished(this);
            return false;
        }

        /// <summary>
        /// Updateds the element for page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElementForPage(Page page)
        {
            var oldPage = (EditPage)page;
            var editPage = (EditPage)this.InstantiatePage();
            editPage.QrCodeEnabled = oldPage.QrCodeEnabled;
            editPage.QrCodeBeepEnabled = oldPage.QrCodeBeepEnabled;
            editPage.QrCodeControlsEnabled = oldPage.QrCodeControlsEnabled;
            this.containerMetaInfo = new UPContainerMetaInfo(this.FieldControl);
            UPCRMResult result = null;
            if (!string.IsNullOrEmpty(this.recordId))
            {
                if (this.RequestOption == UPRequestOption.FastestAvailable || this.RequestOption == UPRequestOption.Offline)
                {
                    result = this.containerMetaInfo.ReadRecord(this.recordId);
                    if (result?.RowCount == 0)
                    {
                        result = null;
                    }
                }

                if (result == null && this.RequestOption != UPRequestOption.Offline)
                {
                    var remoteOperation = this.containerMetaInfo.ReadRecord(this.recordId, this);
                    if (remoteOperation == null)
                    {
                        return null; // should not happen
                    }

                    return oldPage;
                }
            }
            else
            {
                var linkRecordIdentification = this.ViewReference?.ContextValueForKey("LinkRecordId");
                if (!string.IsNullOrEmpty(linkRecordIdentification))
                {
                    var linkIdStr = this.ViewReference?.ContextValueForKey("LinkId");
                    if (string.IsNullOrEmpty(linkIdStr))
                    {
                        linkIdStr = "-1";
                    }

                    result = this.containerMetaInfo.NewRecordWithLinkLinkId(linkRecordIdentification, linkIdStr.ToInt());
                }
                else
                {
                    result = this.containerMetaInfo.NewRecord();
                }
            }

            if (result?.RowCount > 0)
            {
                this.TopLevelElement = editPage;
                this.rootRecordResultRow = result.ResultRowAtIndex(0) as UPCRMResultRow;
                this.rootRecordResult = result;
                this.TopLevelElement = editPage;
                this.FillPageWithResultRow(editPage, this.rootRecordResultRow, this.rootRecordResultRow != null
                    && this.rootRecordResultRow.IsServerResponse ? UPRequestOption.Online : UPRequestOption.Offline);
                this.editPageContext?.HandleDependentFields();
                return this.Page;
            }

            this.HandlePageErrorDetails("ErrorActionNotPossible", "ErrorRecordDoesNotExist");
            return null;
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public override UPMElement UpdatedElement(UPMElement element)
        {
            return element is EditPage ? this.UpdatedElementForPage(this.EditPage) : element;
        }

        /// <summary>
        /// Users the did change record selector.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="callDepth">The call depth.</param>
        /// <returns></returns>
        public List<IIdentifier> UserDidChangeRecordSelector(UPMRecordSelectorEditField field, int callDepth)
        {
            var selector = field.CurrentSelector;
            var copyValues = field.ResultRows != null ?
                field.ResultRows.FunctionValues :
                selector.ValuesFromResultRow(null);

            var crmLinkInfo = selector.LinkInfo;
            if (crmLinkInfo != null)
            {
                var link = !string.IsNullOrEmpty(field.ResultRows?.RootRecordIdentification) ?
                    new UPCRMLink(field.ResultRows.RootRecordIdentification, crmLinkInfo.LinkId) :
                    new UPCRMLink(crmLinkInfo.TargetInfoAreaId, crmLinkInfo.LinkId, false);

                if (this.changedLinks != null)
                {
                    this.changedLinks.SetObjectForKey(link, crmLinkInfo.Key);
                }
                else
                {
                    this.changedLinks = new Dictionary<string, UPCRMLink> { { crmLinkInfo.Key, link } };
                }
            }

            var changedIdentifiers = this.ApplyCopyValues(copyValues, field, callDepth);
            var changedChildren = ((UPEditFieldContext)field.EditFieldContext).NotifyDependentFields();

            if (changedChildren == null || changedChildren.Count == 0)
            {
                return changedIdentifiers;
            }

            if (changedIdentifiers == null || changedIdentifiers.Count == 0)
            {
                return changedChildren;
            }

            var arr = new List<IIdentifier>(changedIdentifiers);
            arr.AddRange(changedChildren);
            return arr;
        }

        /// <summary>
        /// Users the did change field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="callDepth">The call depth.</param>
        /// <returns></returns>
        public List<IIdentifier> UserDidChangeField(UPMEditField field, UPSelector selector, string fieldValue, int callDepth)
        {
            var copyValues = selector.ValueDictionaryForOptionName(fieldValue);
            var copyDisplayValues = selector.DisplayValueDictionaryForOptionName(fieldValue);
            var changedFields = this.ApplyCopyValues(copyValues, copyDisplayValues, callDepth);
            var sourceValueDictionary = this.ViewReference.ContextValueForKey("copyFields")?.JsonDictionaryFromString();
            var templateParameterValueDictionary = sourceValueDictionary ?? new Dictionary<string, object>();
            if (copyValues?.Count > 0)
            {
                templateParameterValueDictionary.Append(copyValues);
            }

            var templateFilter = selector.TemplateFilterFor(fieldValue, templateParameterValueDictionary);
            if (templateFilter != null)
            {
                this.ApplyTemplateFilter(templateFilter);
                return null; // refresh everything
            }

            return changedFields;
        }

        /// <summary>
        /// Applies the copy values.
        /// </summary>
        /// <param name="copyValues">The copy values.</param>
        /// <param name="copyDisplayValues">The copy display values.</param>
        /// <param name="callDepth">The call depth.</param>
        /// <returns></returns>
        public List<IIdentifier> ApplyCopyValues(Dictionary<string, object> copyValues, Dictionary<string, string> copyDisplayValues, int callDepth)
        {
            var changedIdentifiers = new List<IIdentifier>();
            var changedContexts = new List<UPEditFieldContext>();
            if (copyValues != null && this.editPageContext?.EditFields != null)
            {
                foreach (var fieldContext in this.editPageContext.EditFields.Values)
                {
                    if (string.IsNullOrEmpty(fieldContext?.FieldFunction))
                    {
                        continue;
                    }

                    var value = copyDisplayValues.ValueOrDefault(fieldContext.FieldFunction);

                    if (value == null)
                    {
                        continue;
                    }

                    fieldContext.SetOfflineChangeValue(value);
                    changedIdentifiers.Add(fieldContext.FieldIdentifier);
                    changedContexts.Add(fieldContext);
                }
            }

            foreach (var efc in changedContexts)
            {
                var ci = this.UserDidChangeField(efc, null, callDepth + 1);
                if (ci != null && ci.Any())
                {
                    changedIdentifiers.AddRange(ci);
                }
            }

            return changedIdentifiers;
        }

        /// <summary>
        /// Applies the copy values.
        /// </summary>
        /// <param name="copyValues">The copy values.</param>
        /// <param name="field">The field.</param>
        /// <param name="callDepth">The call depth.</param>
        /// <returns></returns>
        public List<IIdentifier> ApplyCopyValues(Dictionary<string, object> copyValues, UPMRecordSelectorEditField field, int callDepth)
        {
            var changedIdentifiers = new List<IIdentifier>();
            var parentIdentifiers = new List<IIdentifier>();
            var changedEditFieldContexts = new List<UPEditFieldContext>();
            if (copyValues != null && field.EditFieldsContext != null)
            {
                var fieldEditPageContext = field.EditFieldsContext as UPEditPageContext;

                if (fieldEditPageContext != null)
                {
                    var editFieldArray = fieldEditPageContext.OrderedEditFieldContexts;
                    foreach (var fieldContext in editFieldArray)
                    {
                        if (string.IsNullOrEmpty(fieldContext.FieldFunction))
                        {
                            continue;
                        }

                        var value = copyValues.ValueOrDefault(fieldContext.FieldFunction) as string;
                        if (value == null)
                        {
                            continue;
                        }

                        fieldContext.SetValue(value);
                        fieldContext.SetChanged(true);
                        if (fieldContext.HasDependentFields)
                        {
                            var ci = fieldContext.NotifyDependentFields();
                            if (ci.Count > 0)
                            {
                                changedIdentifiers.AddRange(ci);
                            }
                        }

                        changedEditFieldContexts.Add(fieldContext);
                        if (fieldContext.FieldIdentifier == null)
                        {
                            if (fieldContext.ParentFieldIdentifier != null)
                            {
                                parentIdentifiers.Add(fieldContext.ParentFieldIdentifier);
                            }
                        }
                        else
                        {
                            changedIdentifiers.Add(fieldContext.FieldIdentifier);
                        }
                    }
                }
                else if (field.EditFieldsContext is UPChildEditContext)
                {
                    var childEditContext = (UPChildEditContext)field.EditFieldsContext;
                    foreach (var fieldContext in childEditContext.EditFieldContext.Values)
                    {
                        if (string.IsNullOrEmpty(fieldContext.FieldFunction))
                        {
                            continue;
                        }

                        var value = copyValues.ValueOrDefault(fieldContext.FieldFunction) as string;
                        if (value == null)
                        {
                            continue;
                        }

                        fieldContext.SetValue(value);
                        fieldContext.SetChanged(true);
                        changedIdentifiers.Add(fieldContext.FieldIdentifier);
                        if (fieldContext.EditField != field)
                        {
                            changedEditFieldContexts.Add(fieldContext);
                        }
                    }
                }
            }

            if (parentIdentifiers.Count > 0)
            {
                foreach (var parentIdentifier in parentIdentifiers)
                {
                    if (!changedIdentifiers.Contains(parentIdentifier))
                    {
                        changedIdentifiers.Add(parentIdentifier);
                    }
                }
            }

            foreach (var editFieldContext in changedEditFieldContexts)
            {
                var ci = this.UserDidChangeField(editFieldContext, null, callDepth + 1);
                if (ci?.Count > 0)
                {
                    changedIdentifiers.AddRange(ci);
                }
            }

            return changedIdentifiers;
        }

        /// <summary>
        /// Applies the template filter.
        /// </summary>
        /// <param name="templateFilter">The template filter.</param>
        /// <returns></returns>
        public List<IIdentifier> ApplyTemplateFilter(UPConfigFilter templateFilter)
        {
            Dictionary<string, object> copyValues = templateFilter.FieldsWithConditions(true);
            List<IIdentifier> changedIdentifiers = null;

            foreach (var fieldContext in this.editPageContext.EditFields.Values)
            {
                var fieldKey = $"{this.InfoAreaId}.{fieldContext.FieldId}";
                var fieldValue = copyValues.ValueOrDefault(fieldKey);
                if (fieldValue == null)
                {
                    continue;
                }

                fieldContext.SetValue(fieldValue as string);
                fieldContext.SetChanged(true);
                if (changedIdentifiers != null)
                {
                    changedIdentifiers.Add(fieldContext.FieldIdentifier);
                }
                else
                {
                    changedIdentifiers = new List<IIdentifier> { fieldContext.FieldIdentifier };
                }
            }

            foreach (string copyValueKey in copyValues.Keys)
            {
                if (!copyValueKey.StartsWith("."))
                {
                    continue;
                }

                var parts = copyValueKey.Split('.');
                var linkId = -1;
                if (parts.Length > 2)
                {
                    linkId = Convert.ToInt32(parts[2]);
                }

                string groupModelControllerKey = UPEditChildrenGroupModelControllerBase.ChildGroupKeyForInfoAreaIdLinkId(parts[1], linkId);
                foreach (UPGroupModelController group in this.GroupModelControllerArray)
                {
                    UPEditChildrenGroupModelControllerBase childrenGroup = group as UPEditChildrenGroupModelControllerBase;
                    if (childrenGroup != null)
                    {
                        if (groupModelControllerKey == childrenGroup.ChildGroupKey)
                        {
                            // TODO: childrenGroup.AddGroupsWithInitialValues(copyValues[copyValueKey]);
                            break;
                        }
                    }
                }
            }

            return changedIdentifiers;
        }

        /// <summary>
        /// Removes the pending changes.
        /// </summary>
        public override void RemovePendingChanges()
        {
            base.RemovePendingChanges();
            if (this.editPageContext?.EditFields?.Values == null)
            {
                return;
            }

            foreach (var fieldContext in this.editPageContext.EditFields.Values)
            {
                fieldContext?.SetChanged(false);
                var editField = fieldContext?.Field as UPMEditField;
                if (editField != null)
                {
                    editField.Changed = false;
                }
            }

            this.changedLinks = null;
        }

        /// <summary>
        /// Shows the qr code scanner.
        /// </summary>
        public void ShowQrCodeScanner()
        {
            this.InformAboutDidChangeTopLevelElement(this.EditPage, this.EditPage, null, UPChangeHints.ChangeHintsWithHint("showQrCodeScanner"));
        }

#if PORTING
        void ObserveValueForKeyPathOfObjectChangeContext(string keyPath, object theObject, NSDictionary change, ref void context)
        {
            if (keyPath.IsEqualToString("finished"))
            {
                UPResourceDownload download = (UPResourceDownload)theObject;
                NSData data = new NSData(download.LocalURL);
                UPImageDownload imageDownload = vCardDownloads.ObjectForKey(download.DownloadURL.AbsoluteString);
                imageDownload.EditField.Image = UIImage.ImageWithData(data);
                imageDownload.EditField.Changed = true;
                vCardDownloads.RemoveObjectForKey(imageDownload.Url.AbsoluteString);
                observedDownloads.Remove(download);
                theObject.RemoveObserverForKeyPath(this, "finished");
                this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, @[imageDownload.EditField.Identifier], null);
            }

        }
#endif
        /// <summary>
        /// Users the did change standard field call depth.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="callDepth">The call depth.</param>
        /// <returns></returns>
        public List<IIdentifier> UserDidChangeStandardFieldCallDepth(UPMEditField field, int callDepth)
        {
            var fieldContext = (UPEditFieldContext)field?.EditFieldContext;
            return fieldContext?.NotifyDependentFields();
        }

        /// <summary>
        /// Users the did change field.
        /// </summary>
        /// <param name="field">The field.</param>
        public virtual void UserDidChangeField(UPMEditField field)
        {
            this.UserDidChangeField((UPEditFieldContext)field.EditFieldContext, field, 0);
        }

        /// <summary>
        /// Groups the model controller finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public override void GroupModelControllerFinished(UPGroupModelController sender)
        {
            if (this.TestDelegate != null)
            {
                this.TestDelegate.PageModelControllerFinishedGroupController(this, sender);
            }
            else
            {
                base.GroupModelControllerFinished(sender);
            }
        }

        /// <summary>
        /// Users the did change field.
        /// </summary>
        /// <param name="fieldContext">The field context.</param>
        /// <param name="editField">The _edit field.</param>
        /// <param name="callDepth">The call depth.</param>
        /// <returns></returns>
        public List<IIdentifier> UserDidChangeField(UPEditFieldContext fieldContext, UPMEditField editField, int callDepth)
        {
            if (fieldContext == null)
            {
                throw new ArgumentNullException(nameof(fieldContext));
            }

            var refreshEverything = false;
            var field = editField ?? fieldContext.Field;
            if (callDepth > 0)
            {
                if (callDepth > 3)
                {
                    return null; // ignore - possible dependency loop!
                }

                if (field is UPMRecordSelectorEditField)
                {
                    return null; // no cascading handling of record selectors -> use ClearValues
                }
            }

            IList<IIdentifier> changedIdentifiers;

            if (field is UPMEditField)
            {
                changedIdentifiers = this.UserDidChangeField(callDepth, out refreshEverything, field as UPMEditField);
            }
            else
            {
                changedIdentifiers = fieldContext.NotifyDependentFields();
            }

            if (callDepth == 0 && (changedIdentifiers?.Count > 0 || refreshEverything))
            {
                this.InformAboutDidChangeTopLevelElement(this.EditPage, this.EditPage, changedIdentifiers as List<IIdentifier>, null);
                return null;
            }

            return changedIdentifiers as List<IIdentifier>;
        }

        /// <summary>
        /// Edits the field contexts.
        /// </summary>
        /// <param name="result">The Result.</param>
        /// <param name="allEditFields">All edit fields.</param>
        /// <returns></returns>
        public List<UPEditFieldContext> EditFieldContexts(UPConfigFilterCheckResult result, Dictionary<string, UPEditFieldContext> allEditFields)
        {
            var fieldKeys = result?.FieldKeys;
            if (fieldKeys == null)
            {
                return null;
            }

            List<UPEditFieldContext> allEditFieldContexts = null;
            foreach (var fieldKey in fieldKeys)
            {
                var editFieldContext = allEditFields.ValueOrDefault(fieldKey);
                if (editFieldContext == null)
                {
                    continue;
                }

                if (allEditFieldContexts?.Count > 0)
                {
                    allEditFieldContexts.Add(editFieldContext);
                }
                else
                {
                    allEditFieldContexts = new List<UPEditFieldContext> { editFieldContext };
                }
            }

            return allEditFieldContexts;
        }

        /// <summary>
        /// Constraints the violations.
        /// </summary>
        /// <returns></returns>
        public List<UPEditConstraintViolation> ConstraintViolations()
        {
            Dictionary<string, object> valueByFieldId = this.GetValueByFieldId();
            List<UPEditConstraintViolation> violationArray = this.GetViolationArray();
            violationArray = this.ProcessValueByFieldId(valueByFieldId, violationArray);

            return this.ProcessGroupModelController(violationArray);
        }

        /// <summary>
        /// Changeds the fields.
        /// </summary>
        /// <param name="userChangesOnly">if set to <c>true</c> [user changes only].</param>
        /// <returns></returns>
        public Dictionary<string, UPEditFieldContext> ChangedFields(bool userChangesOnly = false)
        {
            Dictionary<string, UPEditFieldContext> dict = null;
            if (this.editPageContext != null)
            {
                foreach (var fieldContext in this.editPageContext.EditFields.Values)
                {
                    if (fieldContext.WasChanged(userChangesOnly))
                    {
                        if (fieldContext.FieldConfig.LinkId > 0 || fieldContext.FieldConfig.InfoAreaId != this.InfoAreaId)
                        {
                            continue;
                        }

                        if (dict != null)
                        {
                            dict.SetObjectForKey(fieldContext, fieldContext.Key);
                        }
                        else
                        {
                            dict = new Dictionary<string, UPEditFieldContext>
                            {
                                { fieldContext.Key, fieldContext }
                            };
                        }
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// Changed the links.
        /// </summary>
        /// <param name="userChangedOnly">if set to <c>true</c> [user changed only].</param>
        /// <returns></returns>
        public Dictionary<string, UPCRMLink> ChangedLinks(bool userChangedOnly)
        {
            Dictionary<string, UPCRMLink> changedLinksFromChildren = null;

            foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
            {
                UPEditChildrenGroupModelControllerBase editChildrenGroup = groupModelController as UPEditChildrenGroupModelControllerBase;
                Dictionary<string, UPCRMLink> _changedLinksByGroup = editChildrenGroup?.ChangedLinksForInfoAreaId(this.InfoAreaId, userChangedOnly);

                if (_changedLinksByGroup?.Count > 0)
                {
                    if (changedLinksFromChildren != null)
                    {
                        foreach (var item in _changedLinksByGroup)
                        {
                            changedLinksFromChildren[item.Key] = item.Value;
                        }
                    }
                    else
                    {
                        changedLinksFromChildren = new Dictionary<string, UPCRMLink>(_changedLinksByGroup);
                    }
                }
            }

            if (changedLinksFromChildren?.Count > 0)
            {
                if (this.changedLinks.Count > 0)
                {
                    foreach (var item in this.changedLinks)
                    {
                        changedLinksFromChildren[item.Key] = item.Value;
                    }
                }

                return changedLinksFromChildren;
            }

            return this.changedLinks;
        }

        /// <summary>
        /// Changeds the child records.
        /// </summary>
        /// <param name="parentRecord">The parent record.</param>
        /// <param name="userChangesOnly">if set to <c>true</c> [user changes only].</param>
        /// <returns></returns>
        public virtual List<UPCRMRecord> ChangedChildRecordsForParentRecord(UPCRMRecord parentRecord, bool userChangesOnly)
        {
            List<UPCRMRecord> changedRecords = null;
            foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
            {
                List<UPCRMRecord> changedRecordsOfGroup = null;

                var childrenGroupModelController = groupModelController as UPEditChildrenGroupModelControllerBase;
                if (childrenGroupModelController != null)
                {
                    changedRecordsOfGroup = childrenGroupModelController.ChangedChildRecordsForParentRecord(parentRecord, userChangesOnly);
                }

                if (changedRecordsOfGroup != null)
                {
                    if (changedRecords != null)
                    {
                        changedRecords.AddRange(changedRecordsOfGroup);
                    }
                    else
                    {
                        changedRecords = new List<UPCRMRecord>(changedRecordsOfGroup);
                    }
                }
            }

            return changedRecords;
        }

        /// <summary>
        /// Filters the by removing displayed sub tables.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public UPConfigFilter FilterByRemovingDisplayedSubTables(UPConfigFilter filter)
        {
            if (filter == null)
            {
                return null;
            }

            foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
            {
                UPEditChildrenGroupModelControllerBase childrenController = groupModelController as UPEditChildrenGroupModelControllerBase;
                if (childrenController != null)
                {
                    filter = filter.FilterByRemovingSubTablesWithInfoAreaIdLinkId(childrenController.TargetInfoAreaId, childrenController.TargetLinkId);
                }
            }

            return filter;
        }

        /// <summary>
        /// Links the record identification for information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="linkId">The link identifier.</param>
        /// <returns></returns>
        public string LinkRecordIdentificationForInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            return this.rootRecordResultRow?.RecordIdentificationForLinkInfoAreaIdLinkId(infoAreaId, linkId);
        }

        /// <summary>
        /// Updates the page with Result.
        /// </summary>
        /// <param name="result">The Result.</param>
        public void UpdatePageWithResult(UPCRMResult result)
        {
            if (result == null || result.RowCount == 0)
            {
                this.ShowError(LocalizedString.TextErrorRecordNotFound);
                return;
            }

            var newPage = (EditPage)this.InstantiatePage();
            newPage.Invalid = false;
            this.rootRecordResultRow = result.ResultRowAtIndex(0) as UPCRMResultRow;
            this.rootRecordResult = result;

            this.FillPageWithResultRow(
                newPage, this.rootRecordResultRow, this.rootRecordResultRow != null && this.rootRecordResultRow.IsServerResponse ? UPRequestOption.Online : UPRequestOption.Offline);

            this.editPageContext?.HandleDependentFields();
            Page oldPage = this.Page;
            this.TopLevelElement = newPage;

            if (this.TestDelegate != null)
            {
                this.TestDelegate.PageModelControllerFinished(this);
            }
            else
            {
                this.InformAboutDidChangeTopLevelElement(oldPage, newPage, null, null);
            }
        }

        /// <summary>
        /// Updates the page with violations.
        /// </summary>
        /// <returns></returns>
        public bool UpdatePageWithViolations()
        {
            var constraintViolations = this.ConstraintViolations();
            if (constraintViolations == null || constraintViolations.Count == 0)
            {
                return false;
            }

            var changedIdentifiers = new List<IIdentifier>();
            if (this.lastSignaledViolations != null)
            {
                foreach (var violation in this.lastSignaledViolations)
                {
                    var field = violation?.EditFieldContext?.EditField;
                    if (field != null)
                    {
                        field.HasError = false;
                        changedIdentifiers.Add(field.Identifier);
                    }
                }
            }

            this.lastSignaledViolations = constraintViolations;
            foreach (var violation in constraintViolations)
            {
                if (violation == null)
                {
                    continue;
                }

                var field = violation.EditFieldContext?.EditField;
                if (field != null)
                {
                    field.HasError = true;
                    changedIdentifiers.Add(field.Identifier);
                }

                if (violation.AdditionalEditFieldContexts == null)
                {
                    continue;
                }

                foreach (var additionalContext in violation.AdditionalEditFieldContexts)
                {
                    field = additionalContext?.EditField;
                    if (field != null)
                    {
                        field.HasError = true;
                        changedIdentifiers.Add(field.Identifier);
                    }
                }
            }

            this.InformAboutDidChangeTopLevelElement(this.TopLevelElement, this.TopLevelElement, changedIdentifiers, null);

            var _errors = new List<Exception>();
            foreach (var violation in constraintViolations)
            {
                Exception error = null;
                var localizedDescription = violation.LocalizedDescription;
                if (!string.IsNullOrEmpty(violation.ViolationKey))
                {
                    var errorLabel = ConfigurationUnitStore.DefaultStore.TextFromFieldControlFunctionIdentifier(violation.ViolationKey);
                    if (!string.IsNullOrEmpty(errorLabel))
                    {
                        error = new Exception($"Constraint {errorLabel}: {localizedDescription}");
                    }
                }

                if (error == null)
                {
                    if (violation.ViolationType == EditConstraintViolationType.MustField)
                    {
                        error = new Exception(violation.EditFieldContext.EditField?.LabelText + ": " + (!string.IsNullOrEmpty(localizedDescription)
                            ? localizedDescription
                            : LocalizedString.Localize(LocalizationKeys.TextGroupErrors, LocalizationKeys.KeyErrorsEditMandatoryFieldNotSet)));
                    }
                    else if (violation.ViolationType == EditConstraintViolationType.ClientConstraint)
                    {
                        error = new Exception(violation.EditFieldContext.EditField?.LabelText + ": " + (!string.IsNullOrEmpty(localizedDescription)
                            ? localizedDescription
                            : LocalizedString.TextErrorClientConstraintError));
                    }
                }

                if (error != null)
                {
                    // error.ResponsibleElementIdentifier = violation.EditFieldContext.EditField.HelpIdentifier;
                    _errors.Add(error);
                }
            }

            this.InformAboutDidUpdateListOfErrors(_errors);
            return true;
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public void ShowError(string errorMessage)
        {
            var error = new Exception(errorMessage);
            if (this.TestDelegate != null)
            {
                this.TestDelegate.PageModelControllerDidFailWithError(this, error);
                return;
            }

            this.InformAboutDidUpdateListOfErrors(new List<Exception> { error });

            if (this.Page?.Status != null)
            {
                this.Page.Status = null;
                this.InformAboutDidChangeTopLevelElement(null, null, null, null);
            }
        }

        /// <summary>
        /// Shows the read error.
        /// </summary>
        /// <param name="error">The error.</param>
        protected void ShowReadError(Exception error)
        {
            if (error.Message.Contains("NOTFOUND"))
            {
                this.ShowError(LocalizedString.TextErrorRecordNotFound);
            }
            else if (error.IsConnectionOfflineError())
            {
                this.ShowError(LocalizedString.TextOfflineNotAvailable);
            }
            else
            {
                this.ShowError(LocalizedString.TextErrorGeneralServerError);
            }
        }

        #region ISearchOperationHandler
        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results) { }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count) { }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts) { }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public virtual void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.RequestOption == UPRequestOption.BestAvailable && error.IsConnectionOfflineError())
            {
                this.containerMetaInfo = new UPContainerMetaInfo(this.FieldControl);
                var result = this.containerMetaInfo.ReadRecord(this.recordId);
                if (result.RowCount > 0)
                {
                    this.UpdatePageWithResult(result);
                }
                else
                {
                    this.ShowError(LocalizedString.TextErrorRecordNotFound);
                }
            }
            else
            {
                this.ShowReadError(error);
            }
        }

        /// <summary>
        /// Searches the operation did finish with Result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The Result.</param>
        public virtual void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.UpdatePageWithResult(result);
        }
        #endregion

        /// <summary>
        /// Creates and returns an instance of <see cref="UPRecordSelectorPageModelController"/> based on given <see cref="UPMRecordSelectorEditField"/> object.
        /// </summary>
        /// <param name="editField">Edit field</param>
        /// <returns><see cref="UPRecordSelectorPageModelController"/></returns>
        public UPRecordSelectorPageModelController RecordSelectorPageModelControllerForField(UPMRecordSelectorEditField editField)
        {
            var viewRef = editField.CurrentSelector.SearchViewReference;
            if (viewRef != null)
            {
                viewRef = viewRef.ViewReferenceWith(editField.ContextRecord);
                viewRef.CurrentRecordValues = this.CurrentRecordValues();
            }

            var searchPageController = new UPRecordSelectorPageModelController(viewRef);
            return searchPageController;
        }
        
        /// <summary>
        /// Groups at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><see cref="UPMGroup"/></returns>
        public UPMGroup GroupAtIndex(int index)
        {
            return this.Page?.GroupAtIndex(index);
        }

        /// <summary>
        /// Handles adding new item to repeatable edit group
        /// </summary>
        /// <param name="repeatableEditGroup">Repeatable edit group for adding new item</param>
        public void UserWillInsertNewGroupInRepeatableEditGroup(UPMRepeatableEditGroup repeatableEditGroup)
        {
            foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
            {
                if (groupModelController.Group == repeatableEditGroup)
                {
                    ((UPEditChildrenGroupModelControllerBase)groupModelController).UserWillInsertNewGroupInRepeatableEditGroup(repeatableEditGroup);
                }
            }
        }

        /// <summary>
        /// Handles removing an item from repeatable edit group
        /// </summary>
        /// <param name="group">Group to remove</param>
        /// <param name="repeatableEditGroup">Repeatable edit group to remove from</param>
        public void UserWillDeleteGroupInRepeatableEditGroup(UPMGroup group, UPMRepeatableEditGroup repeatableEditGroup)
        {
            foreach (UPGroupModelController groupModelController in this.GroupModelControllerArray)
            {
                if (groupModelController.Group == repeatableEditGroup)
                {
                    ((UPEditChildrenGroupModelControllerBase)groupModelController).UserWillDeleteGroupInRepeatableEditGroup(group, repeatableEditGroup);
                }
            }
        }

        private static void SetValuesForRequestForChange(
            string mode,
            ref string recordIdContextName,
            ref string infoAreadIdContextName,
            ref string configNameContextName,
            ref string expandConfigurationContextName)
        {
            var isRequestForChangeMode = ModeRequestForChange.Equals(mode, StringComparison.OrdinalIgnoreCase);
            if (isRequestForChangeMode)
            {
                recordIdContextName = "RfcRecordId";
                infoAreadIdContextName = "RfcInfoAreaId";
                configNameContextName = "RfcConfigName";
                expandConfigurationContextName = "RfcexpandConfiguration";
            }
        }

        /// <summary>
        /// Creates and returns current record values
        /// </summary>
        /// <returns>Current record values</returns>
        private Dictionary<string, object> CurrentRecordValues()
        {
            var currentRecordValues = new Dictionary<string, object>();
            foreach (string fieldKey in this.editPageContext.EditFields.Keys)
            {
                var editFieldContext = this.editPageContext.EditFields[fieldKey];

                if (!string.IsNullOrEmpty(editFieldContext?.FieldFunction))
                {
                    object value;

                    if (!string.IsNullOrWhiteSpace(editFieldContext.Value))
                    {
                        value = editFieldContext.Value;
                    }
                    else if (editFieldContext.EditField?.FieldValue != null)
                    {
                        value = editFieldContext.EditField.FieldValue;
                    }
                    else
                    {
                        value = editFieldContext.InitialEditFieldValue;
                    }

                    var addField = !string.IsNullOrEmpty(value as string) || (value as ICollection)?.Count > 0;

                    if (addField)
                    {
                        var key = editFieldContext.FieldFunction;
                        currentRecordValues.SetObjectForKey(value, key);
                    }
                }
            }

            return currentRecordValues;
        }

        private bool TimeConstraintsViolationCheck()
        {
            bool check = false;
            UPEditFieldContext startDate = null, endDate = null, startTime = null, endTime = null;

            foreach (UPEditFieldContext fieldContext in this.editPageContext.EditFields.Values)
            {
                if (fieldContext.FieldFunction == "Startdate")
                {
                    startDate = fieldContext;
                }

                if (fieldContext.FieldFunction == "Enddate")
                {
                    endDate = fieldContext;
                }

                if (fieldContext.FieldFunction == "Starttime")
                {
                    startTime = fieldContext;
                }

                if (fieldContext.FieldFunction == "Endtime")
                {
                    endTime = fieldContext;
                }
            }

            if (startTime != null && startDate != null && endTime != null && endDate != null)
            {
#if PORTING
                if (startDate.Value == endDate.Value)
                {
                    if (startTime.Value.IntValue() > endTime.Value.IntValue())
                    {
                        UIAlertView alert = new UIAlertView(upText_Error, upTextError_InvalidTime, null, upText_OK, null);
                        alert.Show();
                        check = true;
                    }
                }
                else if (startDate.Value.IntValue() > endDate.Value.IntValue())
                {
                    UIAlertView alert = new UIAlertView(upText_Error, upTextError_InvalidDate, null, upText_OK, null);
                    alert.Show();
                    check = true;
                }
#endif
            }

            return check;
        }

        private Dictionary<string, object> GetValueByFieldId()
        {
            Dictionary<string, object> valueByFieldId = null;
            if (this.ClientCheckFilter != null)
            {
                Dictionary<string, object> valueDictionary = null;
                if (this.ClientCheckFilter.RootTable?.NumberOfSubTables > 0)
                {
                    valueByFieldId = new Dictionary<string, object>();
                }

                foreach (var fieldContext in this.editPageContext.EditFields.Values)
                {
                    if (fieldContext == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(fieldContext.FieldConfig?.Function) && fieldContext.Value != null)
                    {
                        if (valueDictionary == null)
                        {
                            var copyFields = this.ViewReference.ContextValueForKey("copyFields")?.JsonDictionaryFromString();
                            valueDictionary = copyFields != null ? new Dictionary<string, object>(copyFields) : new Dictionary<string, object>();
                        }

                        valueDictionary[$"New{fieldContext.FieldConfig.Function}"] = fieldContext.Value;
                    }

                    if (valueByFieldId != null && !string.IsNullOrEmpty(fieldContext.Value))
                    {
                        valueByFieldId.SetObjectForKey(fieldContext.Value, fieldContext.FieldConfig?.Field.FieldIdentification);
                    }
                }

                if (valueDictionary == null)
                {
                    if (this.editPageContext.ClientCheckFilter == null)
                    {
                        var copyFields = this.ViewReference.ContextValueForKey("copyFields").JsonDictionaryFromString();
                        this.editPageContext.ClientCheckFilter = this.ClientCheckFilter.FilterByApplyingValueDictionaryDefaults(copyFields, true);
                    }
                }
                else
                {
                    this.editPageContext.ClientCheckFilter = this.ClientCheckFilter.FilterByApplyingValueDictionaryDefaults(valueDictionary, true);
                }
            }

            return valueByFieldId;
        }

        private List<UPEditConstraintViolation> GetViolationArray()
        {
            List<UPEditConstraintViolation> violationArray = null;
            if (this.editPageContext != null)
            {
                foreach (var fieldContext in this.editPageContext.EditFields.Values)
                {
                    var constraintViolations = fieldContext.ConstraintViolationsWithPageContext(this.editPageContext);
                    if (constraintViolations == null)
                    {
                        continue;
                    }

                    if (violationArray == null)
                    {
                        violationArray = new List<UPEditConstraintViolation>(constraintViolations);
                    }
                    else
                    {
                        violationArray.AddRange(constraintViolations);
                    }
                }
            }

            return violationArray;
        }

        private List<UPEditConstraintViolation> ProcessValueByFieldId(
            Dictionary<string, object> valueByFieldId,
            List<UPEditConstraintViolation> violationArray)
        {
            if (valueByFieldId?.Count > 0)
            {
                var count = this.editPageContext?.ClientCheckFilter?.RootTable?.NumberOfSubTables ?? 0;
                for (var i = 0; i < count; i++)
                {
                    var subTable = this.editPageContext.ClientCheckFilter.RootTable.SubTableAtIndex(i);
                    if (subTable.InfoAreaId != this.InfoAreaId || subTable.LinkId > 0)
                    {
                        continue;
                    }

                    var checkResult = subTable.CheckWithValues(valueByFieldId);
                    if (checkResult == null)
                    {
                        continue;
                    }

                    var violation = new UPEditConstraintViolation(
                        this.EditFieldContexts(checkResult, this.editPageContext.EditFields),
                        EditConstraintViolationType.ClientConstraint,
                        checkResult.ErrorKey);

                    if (violationArray == null)
                    {
                        violationArray = new List<UPEditConstraintViolation> { violation };
                    }
                    else
                    {
                        violationArray.Add(violation);
                    }
                }
            }

            return violationArray;
        }

        private List<UPEditConstraintViolation> ProcessGroupModelController(List<UPEditConstraintViolation> violationArray)
        {
            foreach (var groupModelController in this.GroupModelControllerArray)
            {
                var controllerBase = groupModelController as UPEditChildrenGroupModelControllerBase;

                var constraintViolations = controllerBase?.ConstraintViolationsWithPageContext(this.editPageContext);
                if (constraintViolations != null)
                {
                    if (violationArray == null)
                    {
                        violationArray = new List<UPEditConstraintViolation>(constraintViolations);
                    }
                    else
                    {
                        violationArray.AddRange(constraintViolations);
                    }
                }
            }

            return violationArray;
        }

        private IList<IIdentifier> UserDidChangeField(int callDepth, out bool refreshEverything, UPMEditField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            refreshEverything = false;
            IList<IIdentifier> changedIdentifiers;
            var editField = field;

            changedIdentifiers = field.FindAndInvokeMethod(nameof(UserDidChangeField), new object[] { this }) as List<IIdentifier>;

            if (changedIdentifiers == null)
            {
                changedIdentifiers = UserDidChangeField(callDepth, out refreshEverything, field, editField);
            }

            editField.HasError = false;
            UPEditFieldContext editFieldContext = null;
            if (this.EditTrigger != null &&
                editField.EditFieldsContext is UPEditPageContext)
            {
                var pageContext = (UPEditPageContext)editField.EditFieldsContext;
                foreach (UPEditFieldContext fiedlContext in pageContext.EditFields.Values)
                {
                    if (fiedlContext.Field == field)
                    {
                        editFieldContext = fiedlContext;
                        break;
                    }
                }
            }

            if (editFieldContext != null)
            {
                changedIdentifiers = this.UserDidChangeField(changedIdentifiers, editFieldContext);
            }

            return changedIdentifiers;
        }

        private IList<IIdentifier> UserDidChangeField(int callDepth, out bool refreshEverything, UPMEditField field, UPMEditField editField)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            if (editField == null)
            {
                throw new ArgumentNullException(nameof(editField));
            }

            refreshEverything = false;
            IList<IIdentifier> changedIdentifiers;
            if (callDepth == 0 && field is UPMRecordSelectorEditField)
            {
                var recordSelectorField = (UPMRecordSelectorEditField)field;
                var childEditContext = recordSelectorField.EditFieldsContext as UPChildEditContext;
                if (childEditContext != null)
                {
                    childEditContext.UserDidChangeRecordSelectorEditFieldCallDepth(recordSelectorField, callDepth);
                    var selector = recordSelectorField.CurrentSelector;

                    var copyValues = recordSelectorField.ResultRows != null ?
                        recordSelectorField.ResultRows.FunctionValues :
                        selector.ValuesFromResultRow(null);

                    changedIdentifiers = this.ApplyCopyValues(copyValues, recordSelectorField, callDepth);
                }
                else
                {
                    changedIdentifiers = this.UserDidChangeRecordSelector((UPMRecordSelectorEditField)field, callDepth);
                }
            }
            else if (callDepth == 0 && field is UPMRepEditField)
            {
                var repEditField = (UPMRepEditField)field;
                changedIdentifiers = new List<IIdentifier>() { repEditField.Identifier };
            }
            else if (callDepth == 0 && field is UPMImageEditField)
            {
                var imgEditField = (UPMImageEditField)field;
                changedIdentifiers = new List<IIdentifier>() { imgEditField.Identifier };
            }
            else
            {
                UPSelector selector = null;

                if (!(field.FieldValue is Enumerable))
                {
                    var fieldValue = $"{field.FieldValue}";
                    if (selector != null && !string.IsNullOrWhiteSpace(fieldValue))
                    {
                        changedIdentifiers = this.UserDidChangeField(editField, selector, fieldValue, callDepth);
                        refreshEverything = true;
                    }
                    else
                    {
                        changedIdentifiers = this.UserDidChangeStandardFieldCallDepth(editField, callDepth);
                    }
                }
                else
                {
                    changedIdentifiers = this.UserDidChangeStandardFieldCallDepth(editField, callDepth);
                }
            }

            return changedIdentifiers;
        }

        private IList<IIdentifier> UserDidChangeField(IList<IIdentifier> changedIdentifiers, UPEditFieldContext editFieldContext)
        {
            if (changedIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(changedIdentifiers));
            }

            if (editFieldContext == null)
            {
                throw new ArgumentNullException(nameof(editFieldContext));
            }

            var changedFields = new Dictionary<string, object>();
            List<IIdentifier> triggerChangedIdentifiers = null;
            if (!string.IsNullOrWhiteSpace(editFieldContext.FieldFunction))
            {
                this.UserDidChangeField(editFieldContext.MayHaveExtKey);
            }

            foreach (var key in changedFields.Keys)
            {
                var val = changedFields.ValueOrDefault(key);
                var contexts = this.editPageContext.ContextsForCrmField.ValueOrDefault(key);
                foreach (var ctx in contexts)
                {
                    if (ctx.Value.Equals(val))
                    {
                        continue;
                    }

                    ctx.SetValue(val as string);
                    ctx.SetChanged(true);
                    if (ctx.Field == null || ctx == editFieldContext)
                    {
                        continue;
                    }

                    if (triggerChangedIdentifiers == null)
                    {
                        triggerChangedIdentifiers = new List<IIdentifier> { ctx.Field.Identifier };
                    }
                    else
                    {
                        triggerChangedIdentifiers.Add(ctx.Field.Identifier);
                    }
                }
            }

            if (triggerChangedIdentifiers.Count > 0)
            {
                if (changedIdentifiers.Count > 0)
                {
                    var ci = new List<IIdentifier>(changedIdentifiers);
                    ci.AddRange(triggerChangedIdentifiers);
                    changedIdentifiers = ci;
                }
                else
                {
                    changedIdentifiers = triggerChangedIdentifiers;
                }
            }

            return changedIdentifiers;
        }

        private void UserDidChangeField(bool editFieldContextMayHaveExtKey)
        {
            var rules = new List<object>();

            if (editFieldContextMayHaveExtKey)
            {
                var extKeyRules = new List<object>();
                if (extKeyRules != null)
                {
                    if (rules != null)
                    {
                        var sumRules = new List<object>(rules);
                        foreach (var rule in extKeyRules)
                        {
                            if (!sumRules.Contains(rule))
                            {
                                sumRules.Add(rule);
                            }
                        }

                        rules = sumRules;
                    }
                    else
                    {
                        rules = extKeyRules;
                    }
                }
            }

            if (rules != null && this.editPageContext?.ContextForFunctionName != null)
            {
                var fieldValueDictionary = new Dictionary<string, string>();
                foreach (var functionName in this.editPageContext.ContextForFunctionName.Keys)
                {
                    var currentEditFieldContext = this.editPageContext.ContextForFunctionName.ValueOrDefault(functionName);
                    if (functionName.StartsWith(".extkey"))
                    {
                        var extkey = currentEditFieldContext.Extkey;
                        if (extkey != null)
                        {
                            fieldValueDictionary.SetObjectForKey(extkey, functionName);
                        }
                    }
                    else
                    {
                        var val = currentEditFieldContext.Value;
                        if (val != null)
                        {
                            fieldValueDictionary.SetObjectForKey(val, functionName);
                        }
                    }
                }
            }
        }

        private void SetRecordId(string recordIdContextName, string infoAreadIdContextName, string configNameContextName)
        {
            if (this.OfflineRequest?.Records != null && this.OfflineRequest.Records.Count > 0)
            {
                var rootRecord = this.OfflineRequest.FirstRecord;
                if (!rootRecord.IsNew && rootRecord.Mode != "New")
                {
                    this.recordId = this.OfflineRequest.FirstRecord.RecordIdentification;
                }
            }
            else
            {
                this.recordId = this.ViewReference?.ContextValueForKey(recordIdContextName);
                this.recordId = UPCRMDataStore.DefaultStore.ReplaceRecordIdentification(this.recordId);
            }

            this.InfoAreaId = this.ViewReference?.ContextValueForKey(infoAreadIdContextName);
            this.ConfigName = this.ViewReference?.ContextValueForKey(configNameContextName);
        }

        private void GetQrCodeValues(ref bool qrCodeControlsEnabled, ref bool qrCodeBeepEnabled)
        {
            var keyboardWithScannedSuggestions = this.ViewReference?.ContextValueForKey(KeyboardWithCannedSuggestions);
            var qrCodeEnabled = this.ViewReference?.ContextValueIsSet(KeyboardWithCannedSuggestions) ?? false;
            if (bool.TrueString.Equals(keyboardWithScannedSuggestions, StringComparison.OrdinalIgnoreCase))
            {
                qrCodeBeepEnabled = true;
                qrCodeControlsEnabled = false;
            }
            else if (CONTROLS.Equals(keyboardWithScannedSuggestions, StringComparison.OrdinalIgnoreCase))
            {
                qrCodeBeepEnabled = false;
                qrCodeControlsEnabled = true;
            }
            else if (!string.IsNullOrWhiteSpace(keyboardWithScannedSuggestions))
            {
                var dictionary = keyboardWithScannedSuggestions.JsonDictionaryFromString();
                qrCodeBeepEnabled = (dictionary?.ValueOrDefault("beep") as string)?
                    .Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
                qrCodeControlsEnabled = (dictionary?.ValueOrDefault("controls") as string)?
                    .Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
            }
        }

        private EditPage GetEditPage(
            bool qrCodeEnabled,
            bool qrCodeBeepEnabled,
            bool qrCodeControlsEnabled,
            IConfigurationUnitStore configStore)
        {
            return new EditPage(FieldIdentifier.IdentifierWithInfoAreaIdRecordIdFieldId(this.InfoAreaId, this.recordId, "Page0"))
            {
                Invalid = true,
                QrCodeEnabled = qrCodeEnabled,
                QrCodeBeepEnabled = qrCodeBeepEnabled,
                QrCodeControlsEnabled = qrCodeControlsEnabled,
                LabelText = configStore.BasicTextByIndexDefaultText(0, "Overview")
            };
        }

        private UPConfigExpand GetConfigExpand(IConfigurationUnitStore configStore, string expandConfigurationContextName)
        {
            var expandConfig = !string.IsNullOrWhiteSpace(expandConfigurationContextName) ?
                configStore.ExpandByName(this.ViewReference?.ContextValueForKey(expandConfigurationContextName)) :
                null;

            if (expandConfig == null)
            {
                expandConfig = configStore.ExpandByName(this.ConfigName);
            }

            if (expandConfig == null)
            {
                string configParam = !string.IsNullOrWhiteSpace(expandConfigurationContextName) ? expandConfigurationContextName : this.ConfigName;
                this.Logger.LogError(
                    $"EditPageModelController: cannot find expand configuration parameter {configParam}");
            }

            return expandConfig;
        }

        private void GetFieldControl(UPConfigExpand expandConfig, IConfigurationUnitStore configStore)
        {
            this.InfoAreaId = expandConfig.InfoAreaId;
            this.FieldControl = configStore.FieldControlByNameFromGroup(nameof(Edit), expandConfig.FieldGroupName);
            if (this.FieldControl == null)
            {
                this.isConfigEditControl = false;
                this.FieldControl = configStore.FieldControlByNameFromGroup("Details", expandConfig.FieldGroupName);
            }
            else
            {
                this.isConfigEditControl = true;
                if (this.FieldControl?.AllCRMFields != null)
                {
                    foreach (var field in this.FieldControl.AllCRMFields)
                    {
                        if (field.FieldType != "X" || this.initialValues == null)
                        {
                            continue;
                        }

                        if (this.initialValues.ValueOrDefault(field.FieldIdentification) == null)
                        {
                            this.initialValues[field.FieldIdentification] = "0";
                        }
                    }
                }
            }
        }

        private void SetClientCheckFilter(IConfigurationUnitStore configStore)
        {
            this.FieldControl = this.FieldControl?.FieldControlWithMode(this.IsNew ? FieldDetailsMode.New : FieldDetailsMode.Update);
            var clientFilterName = this.ViewReference?.ContextValueForKey("ClientRightsFilterName");
            if (string.IsNullOrWhiteSpace(clientFilterName))
            {
                clientFilterName = this.FieldControl?.ValueForAttribute(this.IsNew ? "ClientFilterForNew" : "ClientFilterForUpdate");

                if (string.IsNullOrWhiteSpace(clientFilterName))
                {
                    clientFilterName = this.FieldControl?.ValueForAttribute("ClientFilter");
                }
            }

            var editTriggerName = this.ViewReference?.ContextValueForKey("EditTriggerFilter");
            if (string.IsNullOrWhiteSpace(editTriggerName))
            {
                editTriggerName = this.FieldControl?.ValueForAttribute(this.IsNew ? "EditTriggerFilterForNew" : "EditTriggerFilterForUpdate");

                if (string.IsNullOrWhiteSpace(editTriggerName))
                {
                    editTriggerName = this.FieldControl?.ValueForAttribute("EditTriggerFilter");
                }
            }

            if (!string.IsNullOrWhiteSpace(clientFilterName))
            {
                this.ClientCheckFilter = configStore.FilterByName(clientFilterName);
            }
        }

        private bool GetRightsChecker(IConfigurationUnitStore configStore, EditPage page)
        {
            var rightsFilterName = this.ViewReference?.ContextValueForKey("RightsFilterName");
            this.ApplyLoadingStatusOnPage(page);
            UPConfigFilter rightsFilter = null;
            if (!string.IsNullOrWhiteSpace(rightsFilterName))
            {
                rightsFilter = configStore.FilterByName(rightsFilterName);
            }

            if (rightsFilter != null)
            {
                var parameters = this.ViewReference?.ContextValueForKey("copyFields")?.JsonDictionaryFromString();
                if (parameters != null)
                {
                    var replacement = new UPConditionValueReplacement(parameters);
                    rightsFilter = rightsFilter.FilterByApplyingReplacements(replacement);
                }

                this.rightsChecker = new UPRightsChecker(rightsFilter);
            }
            else
            {
                this.rightsChecker = new UPRightsChecker(null);
            }

            if (this.rightsChecker != null)
            {
                var identifiction = this.IsNew
                    ? this.ViewReference.ContextValueForKey("LinkRecordId")
                    : this.InfoAreaId?.InfoAreaIdRecordId(this.recordId);

                if (!string.IsNullOrWhiteSpace(identifiction))
                {
                    this.rightsChecker.CheckPermission(identifiction, false, this);
                    return false;
                }
            }

            return true;
        }
    }
}
