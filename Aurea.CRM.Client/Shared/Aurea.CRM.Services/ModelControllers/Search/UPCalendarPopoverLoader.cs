// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCalendarPopoverLoader.cs" company="Aurea Software Gmbh">
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
//   The Calendar Popover Loader
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Services.ModelControllers.Search
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Services.ModelControllers.Group;
    using Aurea.CRM.UIModel.Groups;
    using Aurea.CRM.UIModel.Identifiers;
    using Aurea.CRM.UIModel.Pages;

    /// <summary>
    /// Calendar Popover Loader
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPCalendarPopoverLoader : ISearchOperationHandler
    {
        /// <summary>
        /// The search page model controller
        /// </summary>
        protected SearchPageModelController searchPageModelController;

        /// <summary>
        /// The source field control
        /// </summary>
        protected FieldControl sourceFieldControl;

        /// <summary>
        /// The original detail group
        /// </summary>
        protected UPMCalendarPopoverGroup origDetailGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCalendarPopoverLoader"/> class.
        /// </summary>
        /// <param name="_searchPageModelController">The search page model controller.</param>
        public UPCalendarPopoverLoader(SearchPageModelController _searchPageModelController)
        {
            this.searchPageModelController = _searchPageModelController;
        }

        /// <summary>
        /// Loads the element for calendar group field control.
        /// </summary>
        /// <param name="_origDetailGroup">The original detail group.</param>
        /// <param name="_sourceFieldControl">The source field control.</param>
        public void LoadElementForCalendarGroupFieldControl(UPMCalendarPopoverGroup _origDetailGroup, FieldControl _sourceFieldControl)
        {
            this.sourceFieldControl = _sourceFieldControl;
            FieldControlTab tabConfig = _sourceFieldControl.TabAtIndex(0);
            this.origDetailGroup = _origDetailGroup;
            this.origDetailGroup.LabelText = tabConfig.Label;

            if (this.sourceFieldControl != null)
            {
                this.LoadSourceRecordRecordId(this.sourceFieldControl, ((RecordIdentifier)this.origDetailGroup.Identifier).RecordIdentification);
            }
        }

        /// <summary>
        /// Loads the source record record identifier.
        /// </summary>
        /// <param name="_fieldControl">The field control.</param>
        /// <param name="recordId">The record identifier.</param>
        public void LoadSourceRecordRecordId(FieldControl _fieldControl, string recordId)
        {
            UPContainerMetaInfo query = new UPContainerMetaInfo(_fieldControl);
            UPMSearchPage searchPage = this.searchPageModelController.SearchPage;
            query.ReadRecord(recordId, searchPage.SearchType == SearchPageSearchType.OfflineSearch
                    ? UPRequestOption.Offline
                    : UPRequestOption.Online, this);
        }


        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            UPDetailsFieldGroupModelController groupModelController = new UPDetailsFieldGroupModelController(this.sourceFieldControl, 0, null);
            if (result.RowCount > 0)
            {
                groupModelController.GroupFromRowGroup((UPCRMResultRow)result.ResultRowAtIndex(0), this.origDetailGroup);
                var changedIdentifiers = new List<IIdentifier> { this.origDetailGroup.Identifier };
                this.origDetailGroup.Invalid = false;
                this.searchPageModelController.InformAboutDidUpdateListOfErrors(new List<Exception>());
                this.searchPageModelController.InformAboutDidChangeTopLevelElement(this.searchPageModelController.TopLevelElement, this.searchPageModelController.TopLevelElement, changedIdentifiers, null);
            }
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.searchPageModelController.InformAboutDidUpdateListOfErrors(new List<Exception> { error });
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
    }
}
