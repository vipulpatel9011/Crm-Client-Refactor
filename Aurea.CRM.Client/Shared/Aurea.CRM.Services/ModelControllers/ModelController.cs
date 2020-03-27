// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelController.cs" company="Aurea Software Gmbh">
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
//   Model controller reset reasons
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Core.CRM;
    using Core.CRM.DataModel;
    using Core.CRM.Delegates;
    using Core.CRM.Query;
    using Core.Logging;
    using Core.Messages;
    using Core.OperationHandling;
    using GalaSoft.MvvmLight.Messaging;
    using GalaSoft.MvvmLight.Ioc;
    using Resources;

    /// <summary>
    /// Model controller reset reasons
    /// </summary>
    public enum ModelControllerResetReason
    {
        /// <summary>
        /// The memory warning.
        /// </summary>
        MemoryWarning = 1,

        /// <summary>
        /// The multi organizer switch.
        /// </summary>
        MultiOrganizerSwitch,

        /// <summary>
        /// The quick search switch.
        /// </summary>
        QuickSearchSwitch,

        /// <summary>
        /// Quick search disappeared switch.
        /// </summary>
        SearchCacheInvalidated
    }

    /// <summary>
    /// Model controller implementation
    /// </summary>
    public class UPMModelController : ISearchOperationHandler
    {
        /// <summary>
        /// The model controller delegate.
        /// </summary>
        protected IModelControllerDelegate modelControllerDelegate;

        /// <summary>
        /// The pending changes.
        /// </summary>
        private readonly List<IIdentifier> pendingChanges;

        /// <summary>
        /// The view reference
        /// </summary>
        private ViewReference viewReference;

        /// <summary>
        /// The replace organizer flag
        /// </summary>
        private bool replaceOrganizer;

#if PORTING

    // reset the state - should release as much memory as possible
        private OpenUrlOperation openUrlOperationRequest;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMModelController"/> class.
        /// </summary>
        public UPMModelController()
        {
            this.pendingChanges = new List<IIdentifier>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMModelController"/> class.
        /// </summary>
        /// <param name="topLevelElement">
        /// The top level element.
        /// </param>
        public UPMModelController(ITopLevelElement topLevelElement)
        {
            if (topLevelElement == null)
            {
                return;
            }

            this.TopLevelElement = topLevelElement;
            this.pendingChanges = new List<IIdentifier>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [mark for redraw].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mark for redraw]; otherwise, <c>false</c>.
        /// </value>
        public bool MarkForRedraw { get; set; }

        /// <summary>
        /// Gets or sets the top level element.
        /// </summary>
        /// <value>
        /// The top level element.
        /// </value>
        public ITopLevelElement TopLevelElement { get; set; }

        /// <summary>
        /// Gets or sets the model controller delegate.
        /// </summary>
        /// <value>
        /// The model controller delegate.
        /// </value>
        public IModelControllerDelegate ModelControllerDelegate
        {
            get => this.modelControllerDelegate;

            set
            {
                this.modelControllerDelegate = value;
                if (value != null && (this.TopLevelElement?.Invalid ?? false || this.pendingChanges.Count > 0))
                {
                    this.UpdateElementForCurrentChanges(new List<IIdentifier>(this.pendingChanges));
                    this.pendingChanges.Clear();
                }
                else if (value != null && this.MarkForRedraw)
                {
                    this.InformAboutDidChangeTopLevelElement(
                        this.TopLevelElement, this.TopLevelElement, null, UPChangeHints.ChangeHintsWithHint("RedrawAfterBackgroundChanges"));
                }
            }
        }

        /// <summary>
        /// Resets the with reason.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        public virtual void ResetWithReason(ModelControllerResetReason reason)
        {
        }

        /// <summary>
        /// Updateds the element.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="UPMElement"/>.
        /// </returns>
        public virtual UPMElement UpdatedElement(UPMElement element)
        {
            return null;
        }

        /// <summary>
        /// Updates the element for current changes.
        /// </summary>
        /// <param name="changes">
        /// The changes.
        /// </param>
        public virtual void UpdateElementForCurrentChanges(List<IIdentifier> changes)
        {
        }

        /// <summary>
        /// Removes the pending changes.
        /// </summary>
        public virtual void RemovePendingChanges()
        {
            this.pendingChanges.Clear();
        }

        /// <summary>
        /// Processes the changes applied identifiers.
        /// </summary>
        /// <param name="listOfIdentifiers">
        /// The list of identifiers.
        /// </param>
        /// <param name="appliedIdentifiers">
        /// The applied identifiers.
        /// </param>
        public virtual void ProcessChangesAppliedIdentifiers(List<IIdentifier> listOfIdentifiers, List<IIdentifier> appliedIdentifiers)
        {
            this.TopLevelElement.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
            if ((this.TopLevelElement.Invalid || appliedIdentifiers.Count > 0) && this.ModelControllerDelegate != null)
            {
                this.UpdateElementForCurrentChanges(appliedIdentifiers);
            }
            else
            {
                this.pendingChanges.AddRange(appliedIdentifiers.Where(identifier => !this.pendingChanges.Contains(identifier)));
            }
        }

        /// <summary>
        /// Processes the changes.
        /// </summary>
        /// <param name="listOfIdentifiers">
        /// The list of identifiers.
        /// </param>
        public virtual void ProcessChanges(List<IIdentifier> listOfIdentifiers)
        {
            if (listOfIdentifiers == null || listOfIdentifiers.Count == 0)
            {
                return;
            }

            var appliedIdentifiers = new List<IIdentifier>();
            this.ProcessChangesAppliedIdentifiers(listOfIdentifiers, appliedIdentifiers);
        }

        /// <summary>
        /// Determines whether this instance [can execute button] the specified button.
        /// </summary>
        /// <param name="button">
        /// The button.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool CanExecuteButton(UPConfigButton button)
            => !(button?.Label?.IndexOf('{') > -1) && this.CanExecuteAction(button.ViewReference);

        /// <summary>
        /// Handles the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public virtual void HandleErrors(List<Exception> errors)
        {
        }

        /// <summary>
        /// Informs about change of top level element.
        /// </summary>
        /// <param name="oldTopLevelElement">The old top level element.</param>
        /// <param name="newTopLevelElement">The new top level element.</param>
        /// <param name="changedIdentifiers">The changed identifiers.</param>
        /// <param name="changeHints">The change hints.</param>
        public virtual void InformAboutDidChangeTopLevelElement(ITopLevelElement oldTopLevelElement, ITopLevelElement newTopLevelElement, List<IIdentifier> changedIdentifiers, UPChangeHints changeHints)
        {
            this.ModelControllerDelegate?.ModelControllerDidChange(this, oldTopLevelElement, newTopLevelElement, changedIdentifiers, changeHints);
        }

        /// <summary>
        /// Informs the about did fail top level element.
        /// </summary>
        /// <param name="failedTopLevelElement">The failed top level element.</param>
        public virtual void InformAboutDidFailTopLevelElement(ITopLevelElement failedTopLevelElement)
        {
            this.ModelControllerDelegate?.ModelControllerDidFail(this, failedTopLevelElement);
        }

        /// <summary>
        /// Informs the about did update list of errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public void InformAboutDidUpdateListOfErrors(List<Exception> errors)
        {
            this.ModelControllerDelegate?.ModelControllerDidUpdate(this, errors);
        }

        /// <summary>
        /// Performs the action with addional parameters.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        /// <param name="addionalParameters">The addional parameters.</param>
        public void PerformAction(Dictionary<string, object> actionDictionary, Dictionary<string, object> addionalParameters)
        {
            try
            {
                this.viewReference = GetViewRefernce(actionDictionary);

                this.replaceOrganizer = Convert.ToInt32(actionDictionary.ValueOrDefault("replaceOrganizer")) != 0;
                if (this.viewReference == null)
                {
                    return;
                }

                var selectorName = this.GetSelectorName();

                if (addionalParameters?.Count > 0)
                {
                    var additionalParamString = this.viewReference.ContextValueForKey("AdditionalParameters");
                    if (!string.IsNullOrEmpty(additionalParamString))
                    {
                        foreach (var key in addionalParameters.Keys)
                        {
                            additionalParamString =
                                additionalParamString.Replace(key, addionalParameters[key] as string);
                        }

                        this.viewReference = new ViewReference(this.viewReference, null, additionalParamString, "AdditionalParameters");
                    }
                }

                if (!string.IsNullOrEmpty(selectorName))
                {
                    this.ProcessSelectorName(selectorName);
                    return;
                }

                if (this.viewReference.ContextValueForKey("Mode") == "NewOrEdit" && this.viewReference.ContextValueForKey("NewOrEditViewQuery") != null)
                {
                    var record = this.ReadRecord();
                }
                else
                {
                    this.ContinueBuildingEditOrNewPage();
                }
            }
            catch (Exception ex)
            {
                SimpleIoc.Default.GetInstance<ILogger>().LogError(ex);
                SimpleIoc.Default.GetInstance<IMessenger>().Send(new ToastrMessage
                {
                    MessageText = ex.Message,
                    DetailedMessage = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Search operation did fail with error handler.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public virtual void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.ContinueBuildingEditOrNewPage();
        }

        /// <summary>
        /// Search operation did finish with result handler.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public virtual void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result.RowCount > 0)
            {
                var editRecord = result.ResultRowAtIndex(0);
                this.viewReference = new ViewReference(this.viewReference, null, editRecord.RootRecordIdentification, "EditOrNewRecordId");
            }

            this.ContinueBuildingEditOrNewPage();
        }

        /// <summary>
        /// Search operation did finish with results handler.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public virtual void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            this.ContinueBuildingEditOrNewPage();
        }

        /// <summary>
        /// Search operation did finish with count handler.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="NotImplementedException">Not implemented yet as it's not needed for now</exception>
        public virtual void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search operation did finish with counts handler.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        /// <exception cref="NotImplementedException">Not implemented yet as it's not needed for now</exception>
        public virtual void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Switches to edit.
        /// </summary>
        /// <param name="actionDictionary">The action dictionary.</param>
        protected virtual void SwitchToEdit(object actionDictionary)
        {
            this.FlipToEdit((ViewReference)actionDictionary);
        }

        /// <summary>
        /// Uploads the photo.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void UploadPhoto(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Modifies the record.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void ModifyRecord(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void DeleteRecord(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Syncs the record to local database.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void SyncRecord(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Toggles the favorite.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void ToggleFavorite(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Flips to edit.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void FlipToEdit(ViewReference viewReference)
        {
        }

        /// <summary>
        /// Flips to edit serial entry.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        protected virtual void FlipToEditSerialEntry(ViewReference viewReference)
        {
        }

        [Conditional("PORTING")]
        private static void Porting(string selectorName)
        {
            throw new NotImplementedException($"Possible Issue: Need to implement selector{selectorName}?");
        }

        private void ContinueBuildingEditOrNewPage()
        {
            UPOrganizerModelController actionOrganizerModelController = UPOrganizerModelController.OrganizerFromViewReference(this.viewReference);
            if (actionOrganizerModelController != null)
            {
                if (this.replaceOrganizer)
                {
                    this.ModelControllerDelegate.PopToNewOrganizerModelController(actionOrganizerModelController);
                }
                else
                {
                    this.ModelControllerDelegate.TransitionToContentModelController(actionOrganizerModelController);
                }
            }
            else if (this.replaceOrganizer)
            {
                this.ModelControllerDelegate.PopToPreviousContentViewController();
            }
        }

#if PORTING
        public void OpenUrl(ViewReference _viewReference)
        {
            string recordId = _viewReference.ContextValueForKey("RecordId");
            string url = _viewReference.ContextValueForKey("url");
            string fieldGroup = _viewReference.ContextValueForKey("FieldGroup");
            string encoding = _viewReference.ContextValueForKey("encoding");
            openUrlOperationRequest = new OpenUrlOperation(url, recordId, fieldGroup, encoding, this);
            openUrlOperationRequest.PerformOperation();
        }

        void UpOpenUrlOperationDidFailWithError(OpenUrlOperation operation, NSError error)
        {
        }

        void UpOpenUrlOperationDidFinishWithResult(OpenUrlOperation operation, NSURL urlToOpen)
        {
            if (UIApplication.SharedApplication().CanOpenURL(urlToOpen))
            {
                UIApplication.SharedApplication().OpenURL(urlToOpen);
            }
            else
            {
                DDLogError("Could not open url %@.", urlToOpen.AbsoluteString());
            }

        }
#endif

        /// <summary>
        /// Determines whether this instance [can execute action] using the specified view reference.
        /// </summary>
        /// <param name="viewReferenceParam">
        /// The view reference.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanExecuteAction(ViewReference viewReferenceParam)
        {
            if (string.IsNullOrEmpty(viewReferenceParam?.ViewName))
            {
                return false;
            }

            var selectorName = string.Empty;
            if (viewReferenceParam.ViewName == "OrganizerAction" || viewReferenceParam.ViewName == "PhotoUploadAction")
            {
                selectorName = viewReferenceParam.ContextValueForKey("Action");
            }
            else if (viewReferenceParam.ViewName.StartsWith("Action:"))
            {
                selectorName = viewReferenceParam.ViewName.Substring(7);
            }

            if (!string.IsNullOrEmpty(selectorName))
            {
                selectorName = selectorName.Replace(":", string.Empty);
                selectorName = selectorName[0].ToString().ToUpper() + selectorName.Substring(1, selectorName.Length - 1);
                return this.HasMethod(selectorName);
            }

            return UPOrganizerModelController.OrganizerFromViewReference(viewReferenceParam) != null;
        }

        private void ProcessSelectorName(string selectorName)
        {
            if (this.replaceOrganizer)
            {
                this.viewReference = this.viewReference.ViewReferenceWithBackToPreviousFollowUpAction();
            }

            selectorName = selectorName.Replace(":", string.Empty);
            selectorName = selectorName[0].ToString().ToUpper() +
                           selectorName.Substring(1, selectorName.Length - 1);

            // Directly handle some known selector names (helps in tracking references)
            switch (selectorName)
            {
                case "ShowInTwoRows":
                    // NSNotificationCenter.DefaultCenter().PostNotificationNameTheObject("kChangeRowsInOderViewNotification", null);
                    return;

                case "UploadPhoto":
                    this.UploadPhoto(this.viewReference);
                    return;

                case "SwitchToEdit":
                    this.SwitchToEdit(this.viewReference);
                    return;

                case "ModifyRecord":
                    this.ModifyRecord(this.viewReference);
                    return;

                case "DeleteRecord":
                    (this as DetailOrganizerModelController)?.DeleteRecord(this.viewReference);
                    return;

                case "ToggleFavorite":
                    this.ToggleFavorite(this.viewReference);
                    return;

                case "SyncRecord":
                    this.SyncRecord(this.viewReference);
                    return;
                default:
                    if (this.HasMethod(selectorName))
                    {
                        this.FindAndInvokeMethod(selectorName, new object[] { this.viewReference });
                    }
                    else
                    {
                        this.LogError(selectorName);
                    }

                    break;
            }

            Porting(selectorName);
        }

        private string GetSelectorName()
        {
            string selectorName = null;
            if (this.viewReference.ViewName == "OrganizerAction" ||
                this.viewReference.ViewName == "PhotoUploadAction" ||
                this.viewReference.ViewName == "FileUploadAction")
            {
                selectorName = this.viewReference.ContextValueForKey("Action");
            }
            else if (this.viewReference.ViewName.StartsWith("Action:"))
            {
                selectorName = this.viewReference.ViewName.Substring(7);
            }

            return selectorName;
        }

        private static ViewReference GetViewRefernce(Dictionary<string, object> actionDictionary)
        {
            var action =
                actionDictionary.ValueOrDefault(Core.Constants.OrganizerAction) as UPMOrganizerAction;
            return action != null
                ? action.ViewReference
                : actionDictionary.ValueOrDefault("viewReference") as ViewReference;
        }

        private void LogError(string selectorName)
        {
            var messageText = LocalizedString.Localize(
                LocalizationKeys.TextGroupClientUIText,
                LocalizationKeys.KeyClientUINonSupportedActionTitle,
                AppResources.KeyClientUINonSupportedActionTitle);

            var detailedMessage =
                string.Format(
                    LocalizedString.Localize(
                        LocalizationKeys.TextGroupClientUIText,
                        LocalizationKeys.KeyClientUINonSupportedActionMessage,
                        AppResources.KeyClientUINonSupportedActionMessage),
                    this.viewReference?.ViewName,
                    selectorName);

            SimpleIoc.Default.GetInstance<IMessenger>().Send(new ToastrMessage
            {
                MessageText = messageText,
                DetailedMessage = detailedMessage
            });

            SimpleIoc.Default.GetInstance<ILogger>()
                .LogError(string.Concat(messageText, " - ", detailedMessage));
        }

        private UPCRMResult ReadRecord()
        {
            var queryName = this.viewReference.ContextValueForKey("NewOrEditViewQuery");
            var linkRecord = this.viewReference.ContextValueForKey("LinkRecord");
            var query = ConfigurationUnitStore.DefaultStore.QueryByName(queryName);
            var crmQuery = new UPContainerMetaInfo(query);
            crmQuery.SetLinkRecordIdentification(linkRecord);
            crmQuery.Find(UPRequestOption.BestAvailable, this, false);
            return crmQuery.ReadRecord(linkRecord);
        }
    }
}
