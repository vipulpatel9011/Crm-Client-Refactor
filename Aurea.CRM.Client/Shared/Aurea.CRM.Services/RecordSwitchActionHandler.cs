// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordSwitchActionHandler.cs" company="Aurea Software Gmbh">
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
//   Record Switch Action Handler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.ModelControllers.Organizer;

    /// <summary>
    /// Record Switch Action Handler
    /// </summary>
    /// <seealso cref="OrganizerActionHandler" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class RecordSwitchActionHandler : OrganizerActionHandler, ISearchOperationHandler
    {
        /// <summary>
        /// Gets the CRM query.
        /// </summary>
        /// <value>
        /// The CRM query.
        /// </value>
        public UPContainerMetaInfo CrmQuery { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public UPCRMResult Result { get; private set; }

        /// <summary>
        /// Gets the decision filter.
        /// </summary>
        /// <value>
        /// The decision filter.
        /// </value>
        public UPConfigFilter DecisionFilter { get; private set; }

        /// <summary>
        /// Gets the decision handler.
        /// </summary>
        /// <value>
        /// The decision handler.
        /// </value>
        public UPCRMFilterBasedDecision DecisionHandler { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSwitchActionHandler"/> class.
        /// </summary>
        /// <param name="modelController">The model controller.</param>
        /// <param name="viewReference">The view reference.</param>
        public RecordSwitchActionHandler(UPOrganizerModelController modelController, ViewReference viewReference)
            : base(modelController, viewReference)
        {
            this.RecordIdentification = viewReference.ContextValueForKey("RecordId");
            string link = viewReference.ContextValueForKey("Link");
            string decisionFilterName = viewReference.ContextValueForKey("DecisionFilterName");
            if (decisionFilterName != null)
            {
                this.DecisionFilter = ConfigurationUnitStore.DefaultStore.FilterByName(decisionFilterName);
                if (this.DecisionFilter != null)
                {
                    this.DecisionFilter = this.DecisionFilter.FilterByApplyingDefaultReplacements();
                }

                if (this.DecisionFilter != null)
                {
                    this.DecisionHandler = new UPCRMFilterBasedDecision(this.DecisionFilter);
                }
            }

            string infoAreaId = null;
            int linkId = -1;
            if (!string.IsNullOrEmpty(link))
            {
                var linkParts = link.Split('#');
                infoAreaId = linkParts[0];
                if (linkParts.Length > 1)
                {
                    linkId = Convert.ToInt32(linkParts[1]);
                }
            }

            if (this.DecisionHandler != null && !string.IsNullOrEmpty(this.RecordIdentification))
            {
                this.CrmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), this.DecisionFilter.InfoAreaId);
                this.CrmQuery.SetLinkRecordIdentification(this.RecordIdentification, linkId);
                this.CrmQuery.AddCrmFields(this.DecisionHandler.FieldDictionary.Values.ToList());
            }
            else if (!string.IsNullOrEmpty(infoAreaId) && !string.IsNullOrEmpty(this.RecordIdentification))
            {
                this.CrmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), infoAreaId);
                this.CrmQuery.SetLinkRecordIdentification(this.RecordIdentification, linkId);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            UPRequestOption requestOption = UPCRMDataStore.RequestOptionFromString(this.ViewReference.ContextValueForKey("RequestOption"), UPRequestOption.FastestAvailable);
            this.DecisionHandler?.UseCrmQuery(this.CrmQuery);

            this.CrmQuery.Find(requestOption, this);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.Error = error;
            this.Finished();
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.Result = result;
            this.Finished();
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Results the view reference.
        /// </summary>
        /// <returns></returns>
        public ViewReference ResultViewReference()
        {
            if (this.DecisionHandler != null && this.Result.RowCount > 0)
            {
                UPCRMResultRow resultRow = (UPCRMResultRow)this.Result.ResultRowAtIndex(0);
                UPConfigQueryTable resultQueryTable = this.DecisionHandler.QueryTableForResultRow(resultRow);
                if (resultQueryTable != null)
                {
                    UPConfigQueryCondition propertyCondition = resultQueryTable.PropertyConditions["Action"];
                    if (!string.IsNullOrEmpty(propertyCondition.FirstValue))
                    {
                        Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(propertyCondition.FirstValue);
                        ViewReference returnViewReference = menu.ViewReference.ViewReferenceWith(this.RecordIdentification, resultRow.RootRecordIdentification);
                        if (this.ViewReference.HasBackToPreviousFollowUpAction())
                        {
                            this.followUpReplaceOrganizer = true;
                            return returnViewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                        }

                        return returnViewReference;
                    }

                    propertyCondition = resultQueryTable.PropertyConditions["ButtonAction"];
                    if (!string.IsNullOrEmpty(propertyCondition.FirstValue))
                    {
                        UPConfigButton button = ConfigurationUnitStore.DefaultStore.ButtonByName(propertyCondition.FirstValue);
                        ViewReference returnViewReference = button.ViewReference.ViewReferenceWith(this.RecordIdentification, resultRow.RootRecordIdentification);
                        if (this.ViewReference.HasBackToPreviousFollowUpAction())
                        {
                            this.followUpReplaceOrganizer = true;
                            return returnViewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                        }

                        return returnViewReference;
                    }

                    propertyCondition = resultQueryTable.PropertyConditions["DefaultAction"];
                    if (!string.IsNullOrEmpty(propertyCondition.FirstValue))
                    {
                        Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(propertyCondition.FirstValue);
                        ViewReference returnViewReference = menu.ViewReference.ViewReferenceWith(this.RecordIdentification, resultRow.RootRecordIdentification);
                        if (this.ViewReference.HasBackToPreviousFollowUpAction())
                        {
                            this.followUpReplaceOrganizer = true;
                            return returnViewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                        }

                        return returnViewReference;
                    }

                    propertyCondition = resultQueryTable.PropertyConditions["DefaultButtonAction"];
                    if (!string.IsNullOrEmpty(propertyCondition.FirstValue))
                    {
                        UPConfigButton button = ConfigurationUnitStore.DefaultStore.ButtonByName(propertyCondition.FirstValue);
                        ViewReference returnViewReference = button.ViewReference.ViewReferenceWith(this.RecordIdentification, resultRow.RootRecordIdentification);
                        if (this.ViewReference.HasBackToPreviousFollowUpAction())
                        {
                            this.followUpReplaceOrganizer = true;
                            return returnViewReference.ViewReferenceWithBackToPreviousFollowUpAction();
                        }

                        return returnViewReference;
                    }
                }
            }

            if (this.Result.RowCount > 0)
            {
                Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(this.ViewReference.ContextValueForKey("ExistsAction"));
                if (menu != null)
                {
                    ViewReference returnViewReference = menu.ViewReference.ViewReferenceWith(
                        ((UPCRMResultRow)this.Result.ResultRowAtIndex(0)).RootRecordIdentification,
                        this.ViewReference.ContextValueForKey("RecordId"));

                    return this.ViewReference.HasBackToPreviousFollowUpAction()
                        ? returnViewReference.ViewReferenceWithBackToPreviousFollowUpAction()
                        : returnViewReference;
                }

                return null;
            }
            else
            {
                Menu menu = ConfigurationUnitStore.DefaultStore.MenuByName(this.ViewReference.ContextValueForKey("NotExistsAction"));
                if (menu != null)
                {
                    ViewReference returnViewReference = menu.ViewReference.ViewReferenceWith(this.ViewReference.ContextValueForKey("RecordId"));
                    return this.ViewReference.HasBackToPreviousFollowUpAction()
                        ? returnViewReference.ViewReferenceWithBackToPreviousFollowUpAction()
                        : returnViewReference;
                }

                return null;
            }
        }
    }
}
