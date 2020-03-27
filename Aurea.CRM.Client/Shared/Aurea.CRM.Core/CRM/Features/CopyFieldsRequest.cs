// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyFieldsRequest.cs" company="Aurea Software Gmbh">
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
//   Copy Fields Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Copy Fields Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPCopyFieldsRequest : ISearchOperationHandler
    {
        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPCopyFieldsDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public UPCopyFields Parent { get; private set; }

        /// <summary>
        /// Gets the CRM query.
        /// </summary>
        /// <value>
        /// The CRM query.
        /// </value>
        public UPContainerMetaInfo CrmQuery { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [always remote].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [always remote]; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysRemote { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCopyFieldsRequest"/> class.
        /// </summary>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <param name="copyFields">The copy fields.</param>
        public UPCopyFieldsRequest(UPContainerMetaInfo crmQuery, UPCopyFieldsDelegate theDelegate, UPCopyFields copyFields)
        {
            this.CrmQuery = crmQuery;
            this.Parent = copyFields;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns></returns>
        public Operation Start()
        {
            return this.CrmQuery.Find(this);
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            if (this.TheDelegate != null)
            {
                if (this.AlwaysRemote || !error.IsConnectionOfflineError())
                {
                    this.TheDelegate.CopyFieldsDidFailWithError(this.Parent, error);
                }
                else
                {
                    this.TheDelegate.CopyFieldsDidFinishWithValues(this.Parent, new Dictionary<string, object>());
                }
            }
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            if (this.TheDelegate != null)
            {
                if (result.RowCount == 0)
                {
                    this.TheDelegate.CopyFieldsDidFinishWithValues(this.Parent, null);
                }
                else
                {
                    this.TheDelegate.CopyFieldsDidFinishWithValues(this.Parent, this.Parent.CopyFieldValuesForResult(result.ResultRowAtIndex(0) as UPCRMResultRow));
                }
            }
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
