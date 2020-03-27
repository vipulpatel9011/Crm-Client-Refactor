// <copyright file="IAnalysisFunctionResultDelegate.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.Core.Analysis.Value
{
    /// <summary>
    /// Analytics function result delegate interface
    /// </summary>
    public interface IAnalysisFunctionResultDelegate
    {
        /// <summary>
        /// Gets a value indicating whether is text result
        /// </summary>
        bool IsTextResult { get; }

        /// <summary>
        /// Gets result
        /// </summary>
        double Result { get; }

        /// <summary>
        /// Gets count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets text result
        /// </summary>
        string TextResult { get; }
    }
}
