// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizerActionHandler.cs" company="Aurea Software Gmbh">
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
//   Organizer Action Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Services.ModelControllers.Organizer;
    using Core.Platform;
    using Core.Structs;

    /// <summary>
    /// Organizer Action Handler
    /// </summary>
    public class OrganizerActionHandler
    {
        /// <summary>
        /// The follow up replace organizer
        /// </summary>
        protected bool followUpReplaceOrganizer;

        /// <summary>
        /// The follow up view reference
        /// </summary>
        protected ViewReference followUpViewReference;

        /// <summary>
        /// Gets the view reference.
        /// </summary>
        /// <value>
        /// The view reference.
        /// </value>
        public ViewReference ViewReference { get; private set; }

        /// <summary>
        /// Gets the model controller.
        /// </summary>
        /// <value>
        /// The model controller.
        /// </value>
        public UPOrganizerModelController ModelController { get; private set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public Exception Error { get; protected set; }

        /// <summary>
        /// Gets the follow up view reference.
        /// </summary>
        /// <value>
        /// The follow up view reference.
        /// </value>
        public virtual ViewReference FollowUpViewReference => this.followUpViewReference;

        /// <summary>
        /// Gets a value indicating whether [follow up replace organizer].
        /// </summary>
        /// <value>
        /// <c>true</c> if [follow up replace organizer]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool FollowUpReplaceOrganizer => this.followUpReplaceOrganizer;

        /// <summary>
        /// Gets or sets the changed records.
        /// </summary>
        /// <value>
        /// The changed records.
        /// </value>
        public List<UPCRMRecord> ChangedRecords { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizerActionHandler"/> class.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="viewReference">The view reference.</param>
        public OrganizerActionHandler(UPOrganizerModelController modelController, ViewReference viewReference)
        {
            this.ModelController = modelController;
            this.ViewReference = viewReference;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public virtual void Execute()
        {
            this.Finished();
        }

        /// <summary>
        /// Finisheds this instance.
        /// </summary>
        public void Finished()
        {
            this.ModelController?.HandleOrganizerAction(this);
        }

        /// <summary>
        /// Views the reference with.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="linkRecordIdentification">The link record identification.</param>
        /// <returns></returns>
        public ViewReference ViewReferenceWith(string parameterName, string recordIdentification, string linkRecordIdentification)
        {
            string menuName = this.ViewReference.ContextValueForKey(parameterName);
            if (!string.IsNullOrEmpty(menuName))
            {
                if (menuName == ".backToPrevious" || menuName == "Return")
                {
                    return ViewReference.BackViewReference();
                }

                if (menuName.StartsWith("Button:"))
                {
                    return ConfigurationUnitStore.DefaultStore.ButtonByName(menuName.Substring(7)).ViewReference.ViewReferenceWith(recordIdentification, linkRecordIdentification);
                }

                if (menuName.StartsWith("Menu:"))
                {
                    return ConfigurationUnitStore.DefaultStore.MenuByName(menuName.Substring(5)).ViewReference.ViewReferenceWith(recordIdentification, linkRecordIdentification);
                }

                return ConfigurationUnitStore.DefaultStore.MenuByName(menuName).ViewReference.ViewReferenceWith(recordIdentification, linkRecordIdentification);
            }

            return null;
        }
    }
}
