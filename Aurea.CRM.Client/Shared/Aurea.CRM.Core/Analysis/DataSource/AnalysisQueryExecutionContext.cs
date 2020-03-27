// <copyright file="AnalysisQueryExecutionContext.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Analysis.DataSource
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using CRM;
    using CRM.DataModel;
    using CRM.Delegates;
    using CRM.Query;
    using OperationHandling;

    /// <summary>
    /// Implementation of analysis query execution context
    /// </summary>
    public class AnalysisQueryExecutionContext : AnalysisExecutionContext, ISearchOperationHandler
    {
        private UPContainerMetaInfo currentQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisQueryExecutionContext"/> class.
        /// </summary>
        /// <param name="analysisConfiguration">Analysis configuration</param>
        /// <param name="filterParameters">Filter parameters</param>
        /// <param name="recordIdentification">Record identification</param>
        /// <param name="linkId">Link id</param>
        public AnalysisQueryExecutionContext(UPConfigAnalysis analysisConfiguration, Dictionary<string, object> filterParameters, string recordIdentification = "", int linkId = 0)
            : base(analysisConfiguration, filterParameters, recordIdentification, linkId)
        {
            this.QueryConfiguration = ConfigurationUnitStore.DefaultStore.QueryByName(analysisConfiguration.QueryName);
        }

        /// <summary>
        /// Gets metainfo
        /// </summary>
        public override ICrmDataSourceMetaInfo MetaInfo => this.Query();

        /// <summary>
        /// Gets query configuration
        /// </summary>
        public UPConfigQuery QueryConfiguration { get; private set; }

        /// <inheritdoc />
        public override AnalysisExecutionContext ExecutionContext()
        {
            return this;
        }

        /// <inheritdoc />
        public override UPCRMResult QueryResult()
        {
            return this.Query().Find();
        }

        /// <inheritdoc />
        public override void QueryResultWithRequestOptionDelegate(UPRequestOption requestOption, IAnalysisExecutionContextDelegate contextDelegate)
        {
            this.currentQuery = this.Query();
            this.ContextDelegate = contextDelegate;
            this.currentQuery.Find(requestOption, this);
        }

        /// <inheritdoc />
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            var contextDelegate = this.ContextDelegate;
            this.ContextDelegate = null;
            contextDelegate.ExecutionContextDidFailWithError(this, error);
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            var contextDelegate = this.ContextDelegate;
            this.ContextDelegate = null;
            contextDelegate.ExecutionContextDidFinishWithResult(this, result);
        }

        /// <inheritdoc />
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
        }

        private UPContainerMetaInfo Query()
        {
            if (this.QueryConfiguration == null)
            {
                return null;
            }

            UPContainerMetaInfo query = new UPContainerMetaInfo(this.QueryConfiguration, this.FilterParameters);
            if (query == null)
            {
                return null;
            }

            query.DisableVirtualLinks = true;
            if (this.LinkRecordIdentification?.Length > 0)
            {
                query.SetLinkRecordIdentification(this.LinkRecordIdentification, this.LinkId);
            }

            if (this.AnalysisConditions?.Count > 0)
            {
                // currently not implemented (analysis conditions are not set, and so do not influence the query)
            }

            return query;
        }
    }
}
