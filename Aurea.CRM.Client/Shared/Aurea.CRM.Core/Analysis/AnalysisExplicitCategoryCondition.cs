// <copyright file="AnalysisExplicitCategoryCondition.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Utilities;

    /// <summary>
    /// Implementation of analysis explicit category condition
    /// </summary>
    public class AnalysisExplicitCategoryCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisExplicitCategoryCondition"/> class.
        /// </summary>
        /// <param name="configCondition">Config condition</param>
        /// <param name="parameters">Parameters</param>
        public AnalysisExplicitCategoryCondition(UPConfigAnalysisCategoryCondition configCondition, List<object> parameters = null)
        {
            string value = configCondition.Value;
            string valueTo = configCondition.ValueTo;
            if (parameters != null)
            {
                foreach (List<object> param in parameters)
                {
                    var key = param[0] as string;
                    var val = param[1] as string;
                    value = value.Replace(key, val);
                    valueTo = valueTo.Replace(key, val);
                }
            }

            this.Checker = new ConditionChecker(configCondition.Type, value, valueTo);
        }

        /// <summary>
        /// Gets checker
        /// </summary>
        public ConditionChecker Checker { get; private set; }

        /// <summary>
        /// Checks if matches value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns true if matches</returns>
        public bool MatchesValue(string value)
        {
            return this.Checker.MatchesString(value);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Checker.ToString();
        }
    }
}
