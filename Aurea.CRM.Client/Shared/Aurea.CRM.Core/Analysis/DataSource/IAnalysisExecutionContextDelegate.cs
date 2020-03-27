// <copyright file="IAnalysisExecutionContextDelegate.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;

    /// <summary>
    /// Execution context delegate interface
    /// </summary>
    public interface IAnalysisExecutionContextDelegate
    {
        /// <summary>
        /// Execution context did finish with result
        /// </summary>
        /// <param name="executionContext">Execution context</param>
        /// <param name="result">Result</param>
        void ExecutionContextDidFinishWithResult(AnalysisExecutionContext executionContext, ICrmDataSource result);

        /// <summary>
        /// Execution context did fail with error
        /// </summary>
        /// <param name="executionContext">Execution context</param>
        /// <param name="error">Error</param>
        void ExecutionContextDidFailWithError(AnalysisExecutionContext executionContext, Exception error);
    }
}
