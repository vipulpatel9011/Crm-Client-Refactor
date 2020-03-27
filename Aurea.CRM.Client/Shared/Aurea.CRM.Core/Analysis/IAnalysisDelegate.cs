// <copyright file="IAnalysisDelegate.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Serdar Tepeyurt
// </author>

namespace Aurea.CRM.Core.Analysis
{
    using System;
    using Aurea.CRM.Core.Analysis.Result;

    /// <summary>
    /// Analysis delegate interface
    /// </summary>
    public interface IAnalysisDelegate
    {
        /// <summary>
        /// Analysis did finish with result
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="result">Result</param>
        void AnalysisDidFinishWithResult(Analysis analysis, AnalysisResult result);

        /// <summary>
        /// Analysis did fail with error
        /// </summary>
        /// <param name="analysis">Analysis</param>
        /// <param name="error">Error</param>
        void AnalysisDidFailWithError(Analysis analysis, Exception error);
    }
}
