// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageModelController.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.UIControlInterfaces;
    using Aurea.CRM.Services.Delegates;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Aurea.CRM.UIModel;
    using Aurea.CRM.UIModel.Pages;
    using Aurea.CRM.UIModel.Status;

    /// <summary>
    /// Page model controller implementation
    /// </summary>
    /// <seealso cref="UPMModelController" />
    public class UPPageModelController : UPMModelController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPPageModelController"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPPageModelController(ViewReference viewReference)
        {
            this.ViewReference = viewReference;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public virtual Page Page => this.TopLevelElement as Page;

        /// <summary>
        /// Gets or sets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the configuration.
        /// </summary>
        /// <value>
        /// The name of the configuration.
        /// </value>
        public string ConfigName { get; protected set; }

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public IPageModelControllerDelegate Delegate { get; set; }

        /// <summary>
        /// Gets or sets the parent organizer model controller.
        /// </summary>
        /// <value>
        /// The parent organizer model controller.
        /// </value>
        public UPOrganizerModelController ParentOrganizerModelController { get; set; }

        /// <summary>
        /// Instantiates the page.
        /// </summary>
        /// <returns></returns>
        public virtual Page InstantiatePage()
        {
            return null;
        }

        /// <summary>
        /// Returns changed records.
        /// </summary>
        /// <returns></returns>
        public virtual List<UPCRMRecord> ChangedRecords()
        {
            // To be implemented in derived classes
            return null;
        }

        /// <summary>
        /// Views the will disappear.
        /// </summary>
        public virtual void ViewWillDisappear()
        {
            this.Delegate?.PageModelControllerViewWillDisappear(this);
        }

        /// <summary>
        /// Views the will appear.
        /// </summary>
        public virtual void ViewWillAppear()
        {
            this.Delegate?.PageModelControllerViewWillAppear(this);
        }

        /// <summary>
        /// Informs the delegate about failure.
        /// </summary>
        public virtual void InformDelegateAboutFailure()
        {
            this.InformAboutDidFailTopLevelElement(this.TopLevelElement);
        }

        /// <summary>
        /// Informs the delegate about success with data.
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void InformDelegateAboutSuccessWithData(Dictionary<string, object> data)
        {
            this.InformAboutDidChangeTopLevelElement(data["oldPage"] as ITopLevelElement, data["newPage"] as ITopLevelElement, data["changes"] as List<IIdentifier>, null);
        }

        /// <summary>
        /// Shows the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public virtual bool ShowStatus(UPMStatus status)
        {
            if (this.Page == null)
            {
                return false;
            }

            this.Page.Status = status;
            if (this.ModelControllerDelegate != null)
            {
                this.InformAboutDidFailTopLevelElement(this.Page);
                return true;
            }

            this.Page.Invalid = false;
            this.Page.Status = status;

            return false;
        }

        /// <summary>
        /// Handles the page message details.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        /// <returns></returns>
        public virtual bool HandlePageMessageDetails(string message, string details)
        {
            var messageStatus = UPMMessageStatus.MessageStatusWithMessageDetails(message, details);
            return this.ShowStatus(messageStatus);
        }

        /// <summary>
        /// Handles the page error details.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        /// <returns></returns>
        public virtual bool HandlePageErrorDetails(string message, string details)
        {
            var errorStatus = UPMErrorStatus.ErrorStatusWithMessageDetails(message, details);
            return this.ShowStatus(errorStatus);
        }

        /// <summary>
        /// Updates the element for changes in background with changes.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public virtual void UpdateElementForChangesInBackground(List<IIdentifier> changes)
        {
            var oldPage = (Page)this.TopLevelElement;
            var newPage = (Page)this.UpdatedElement(oldPage);

            if (oldPage == newPage && !(newPage.Status is UPMErrorStatus))
            {
                if (changes?.Count > 0)
                {
                    this.InformDelegateAboutSuccessWithData(new Dictionary<string, object>
                    {
                        { "oldPage", oldPage },
                        { "newPage", newPage },
                        { "changes", changes }
                    });
                }

                return;
            }

            this.TopLevelElement = newPage;
            var organizer = (UPMOrganizer)oldPage.Parent;
            organizer?.ReplacePageWithNewPage(oldPage, newPage);

            if (newPage.Status is UPMErrorStatus)
            {
                this.InformDelegateAboutFailure();
            }
            else
            {
                if (changes == null)
                {
                    changes = new List<IIdentifier>();
                }

                this.InformDelegateAboutSuccessWithData(new Dictionary<string, object>
                {
                    { "oldPage", oldPage },
                    { "newPage", newPage },
                    { "changes", changes }
                });
            }
        }

        /// <summary>
        /// Updates the element for current changes.
        /// </summary>
        /// <param name="changes">The changes.</param>
        public override void UpdateElementForCurrentChanges(List<IIdentifier> changes)
        {
            this.UpdateElementForChangesInBackground(changes);
        }

        /// <summary>
        /// Forces the page update.
        /// </summary>
        /// <param name="changedIdentifiers">The changed identifiers.</param>
        public virtual void ForcePageUpdate(List<IIdentifier> changedIdentifiers)
        {
            this.Page.Invalid = true;
            if (this.ModelControllerDelegate != null)
            {
                this.UpdateElementForCurrentChanges(changedIdentifiers);
            }
        }

        /// <summary>
        /// Reports the error continue operation.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="continueOperation">if set to <c>true</c> [continue operation].</param>
        public virtual void ReportError(Exception error, bool continueOperation)
        {
            if (continueOperation)
            {
                return;
            }
#if PORTING
            string response = error.UserInfo.ObjectForKey(kUPErrorUserInfoResponse);
            if (!string.IsNullOrEmpty(response))
            {
                var organizerModelController = new UPOrganizerModelController(response);
                if (NSThread.CurrentThread().IsMainThread())
                {
                    this.ModelControllerDelegate.TransitionToContentModelController(organizerModelController);
                }
                else
                {
                    this.ModelControllerDelegate.PerformSelectorOnMainThreadWithObjectWaitUntilDone(@selector(transitionToContentModelController:), organizerModelController, false);
                }
            }
#endif
        }

        /// <summary>
        /// Backgrounds the refresh in Start organizer.
        /// </summary>
        /// <returns></returns>
        public virtual bool BackgroundRefreshInStartOrganizer()
        {
            return false;
        }

        /// <summary>
        /// Handles the errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public override void HandleErrors(List<Exception> errors)
        {
#if PORTING
            if (errors.ContainsUnhandledErrors())
            {
                this.ParentOrganizerModelController.HandleErrors(errors);
            }
#endif
        }
    }
}
