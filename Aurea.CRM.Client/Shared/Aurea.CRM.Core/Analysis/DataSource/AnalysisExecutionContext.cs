// <copyright file="AnalysisExecutionContext.cs" company="Aurea Software Gmbh">
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

    /// <summary>
    /// Implementation of analysis execution context
    /// </summary>
    public class AnalysisExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExecutionContext"/> class.
        /// </summary>
        /// <param name="analysisConfiguration">Analysis configuration</param>
        /// <param name="filterParameters">Filter parameters</param>
        /// <param name="recordIdentification">Record identification</param>
        /// <param name="linkId">Link id</param>
        public AnalysisExecutionContext(UPConfigAnalysis analysisConfiguration, Dictionary<string, object> filterParameters, string recordIdentification = null, int linkId = 0)
        {
            this.AnalysisConfiguration = analysisConfiguration;
            this.FilterParameters = filterParameters;
            this.LinkRecordIdentification = recordIdentification;
            this.LinkId = linkId;
        }

        /// <summary>
        /// Gets analysis conditions
        /// </summary>
        public List<object> AnalysisConditions { get; private set; }

        /// <summary>
        /// Gets analysis configuration
        /// </summary>
        public UPConfigAnalysis AnalysisConfiguration { get; private set; }

        /// <summary>
        /// Gets or sets context delegate
        /// </summary>
        public IAnalysisExecutionContextDelegate ContextDelegate { get; set; }

        /// <summary>
        /// Gets filter parameters
        /// </summary>
        public Dictionary<string, object> FilterParameters { get; private set; }

        /// <summary>
        /// Gets link id
        /// </summary>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets link record identification
        /// </summary>
        public string LinkRecordIdentification { get; private set; }

        /// <summary>
        /// Gets meta info
        /// </summary>
        public virtual ICrmDataSourceMetaInfo MetaInfo => null;

        /// <summary>
        /// Execution context for settings
        /// </summary>
        /// <returns>Returns execution context for settings</returns>
        public virtual AnalysisExecutionContext ExecutionContext()
        {
            return this;
        }

        /// <summary>
        /// Query result
        /// </summary>
        /// <returns>Returns query result</returns>
        public virtual UPCRMResult QueryResult()
        {
            return null;
        }

        /// <summary>
        /// Query result with request option and delegate
        /// </summary>
        /// <param name="requestOption">Request option</param>
        /// <param name="contextDelegate">Context delegate</param>
        public virtual void QueryResultWithRequestOptionDelegate(UPRequestOption requestOption, IAnalysisExecutionContextDelegate contextDelegate)
        {
            contextDelegate.ExecutionContextDidFailWithError(this, new Exception("cannot request result for UPAnalysisExecutionContext base class", null));
        }
    }
}
