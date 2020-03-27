// <copyright file="RightsCheckerRequest.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Rights checker request
    /// </summary>
    /// <seealso cref="ISearchOperationHandler" />
    public class UPRightsCheckerRequest : ISearchOperationHandler
    {
        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPRightsCheckerDelegate Delegate { get; private set; }

        /// <summary>
        /// Gets the rights checker.
        /// </summary>
        /// <value>
        /// The rights checker.
        /// </value>
        public UPRightsChecker RightsChecker { get; private set; }

        /// <summary>
        /// Gets the CRM query.
        /// </summary>
        /// <value>
        /// The CRM query.
        /// </value>
        public UPContainerMetaInfo CrmQuery { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRightsCheckerRequest"/> class.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="rightsChecker">The rights checker.</param>
        public UPRightsCheckerRequest(string recordIdentification, UPContainerMetaInfo crmQuery, UPRightsCheckerDelegate theDelegate, UPRightsChecker rightsChecker)
        {
            this.RecordIdentification = recordIdentification;
            this.Delegate = theDelegate;
            this.RightsChecker = rightsChecker;
            this.CrmQuery = crmQuery;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns></returns>
        public Operation Start()
        {
            return this.CrmQuery?.Find(this);
        }

        /// <summary>
        /// Ups the search operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.Delegate?.RightsCheckerDidFinishWithError(this.RightsChecker, error);
        }

        /// <summary>
        /// Ups the search operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result != null && result.RowCount > 0)
            {
                this.Delegate?.RightsCheckerGrantsPermission(this.RightsChecker, this.RecordIdentification);
            }
            else
            {
                this.Delegate?.RightsCheckerRevokePermission(this.RightsChecker, this.RecordIdentification);
            }
        }

        /// <summary>
        /// Ups the search operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results) { }

        /// <summary>
        /// Ups the search operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count) { }

        /// <summary>
        /// Ups the search operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts) { }
    }
}
