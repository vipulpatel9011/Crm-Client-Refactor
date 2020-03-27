// <copyright file="RightsChecker.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Rights checker
    /// </summary>
    public class UPRightsChecker
    {
        /// <summary>
        /// The requests
        /// </summary>
        private List<UPRightsCheckerRequest> requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRightsChecker"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public UPRightsChecker(UPConfigFilter filter)
        {
            this.Filter = filter;
            this.ForbiddenMessage = this.Filter?.DisplayName;
            if (string.IsNullOrEmpty(this.ForbiddenMessage))
            {
                this.ForbiddenMessage = "ErrorRightsFilter";
            }
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public UPConfigFilter Filter { get; private set; }

        /// <summary>
        /// Gets the forbidden message.
        /// </summary>
        /// <value>
        /// The forbidden message.
        /// </value>
        public string ForbiddenMessage { get; private set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public object Context { get; set; }

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>
        /// The selector.
        /// </value>
        public Action<ViewReference> Selector { get; set; }

        /// <summary>
        /// CRMs the query for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public UPContainerMetaInfo CrmQueryForRecordIdentification(string recordIdentification)
        {
            var crmQuery = new UPContainerMetaInfo(new List<UPCRMField>(), recordIdentification.InfoAreaId());
            if (this.Filter != null)
            {
                crmQuery.ApplyFilter(this.Filter);
            }

            crmQuery.SetLinkRecordIdentification(recordIdentification);
            return crmQuery;
        }

        /// <summary>
        /// Checks the local permission.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public bool CheckLocalPermission(string recordIdentification)
        {
            var crmQuery = this.CrmQueryForRecordIdentification(recordIdentification);
            var result = crmQuery.Find();
            return result?.RowCount == 1;
        }

        /// <summary>
        /// Checks the permission.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="alwaysRemote">if set to <c>true</c> [always remote].</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation CheckPermission(string recordIdentification, bool alwaysRemote, UPRightsCheckerDelegate theDelegate)
        {
            if (!alwaysRemote &&
                UPCRMDataStore.DefaultStore.RecordExistsOffline(recordIdentification))
            {
                var hasPermission = this.CheckLocalPermission(recordIdentification);
                if (hasPermission)
                {
                    theDelegate?.RightsCheckerGrantsPermission(this, recordIdentification);
                }
                else
                {
                    theDelegate?.RightsCheckerRevokePermission(this, recordIdentification);
                }

                return null;
            }

            var crmQuery = this.CrmQueryForRecordIdentification(recordIdentification);
            var request = new UPRightsCheckerRequest(recordIdentification, crmQuery, theDelegate, this);
            if (this.requests == null)
            {
                this.requests = new List<UPRightsCheckerRequest> { request };
            }
            else
            {
                this.requests.Add(request);
            }

            return request.Start();
        }
    }
}
