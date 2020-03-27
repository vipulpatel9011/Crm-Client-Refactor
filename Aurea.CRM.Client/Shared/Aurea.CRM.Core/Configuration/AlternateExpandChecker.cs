// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlternateExpandChecker.cs" company="Aurea Software Gmbh">
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
//   The upcrm alternate expand checker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// The upcrm alternate expand checker.
    /// </summary>
    public class UPCRMAlternateExpandChecker : ISearchOperationHandler
    {
        /// <summary>
        /// The record identification.
        /// </summary>
        private readonly string recordIdentification;

        /// <summary>
        /// The root expand.
        /// </summary>
        private readonly UPConfigExpand rootExpand;

        /// <summary>
        /// The the delegate.
        /// </summary>
        private readonly IAlternateExpandCheckerDelegate theDelegate;

        /// <summary>
        /// The target expand.
        /// </summary>
        private UPConfigExpand targetExpand;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMAlternateExpandChecker"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="rootExpand">
        /// The root expand.
        /// </param>
        /// <param name="theDelegate">
        /// The the delegate.
        /// </param>
        public UPCRMAlternateExpandChecker(
            string recordIdentification,
            UPConfigExpand rootExpand,
            IAlternateExpandCheckerDelegate theDelegate)
        {
            this.recordIdentification = recordIdentification;
            this.rootExpand = rootExpand;
            this.theDelegate = theDelegate;
        }

        /// <summary>
        /// The search operation did fail with error.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.theDelegate?.AlternateExpandCheckerDidFailWithError(this, error);
        }

        /// <summary>
        /// The search operation did finish with count.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The search operation did finish with counts.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="counts">
        /// The counts.
        /// </param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The search operation did finish with result.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (result?.RowCount > 0)
            {
                UPCRMResultRow row = result.ResultRowAtIndex(0) as UPCRMResultRow;
                this.targetExpand = this.rootExpand.ExpandForResultRow(row);
            }
            else
            {
                this.targetExpand = this.rootExpand;
            }

            this.theDelegate?.AlternateExpandCheckerDidFinishWithResult(this, this.targetExpand);
        }

        /// <summary>
        /// The search operation did finish with results.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="results">
        /// The results.
        /// </param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="requestOption">
        /// The request option.
        /// </param>
        public void Start(UPRequestOption requestOption)
        {
            Dictionary<string, UPCRMField> fields = this.rootExpand?.FieldsForAlternateExpands(true);

            if (fields == null || fields?.Count == 0)
            {
                this.SearchOperationDidFinishWithResult(null, null);
                return;
            }

            var fieldValues = fields.Values.Select(x => x).ToList();
            var crmQuery = new UPContainerMetaInfo(fieldValues, this.rootExpand.InfoAreaId);
            crmQuery.SetLinkRecordIdentification(this.recordIdentification);
            crmQuery.Find(requestOption, this);
        }
    }
}
