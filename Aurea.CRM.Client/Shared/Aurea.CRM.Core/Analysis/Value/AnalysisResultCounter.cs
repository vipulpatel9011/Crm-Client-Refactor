// <copyright file="AnalysisResultCounter.cs" company="Aurea Software Gmbh">
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
    using System.Collections.Generic;
    using Extensions;

    /// <summary>
    /// Implementation of analysis result counter class
    /// </summary>
    public class AnalysisResultCounter : IAnalysisFunctionResultDelegate
    {
        private int count;
        private Dictionary<string, object> dict;
        private double value;

        /// <summary>
        /// Gets count
        /// </summary>
        public int Count => this.count;

        /// <summary>
        /// Gets a value indicating whether is text result
        /// </summary>
        public bool IsTextResult => false;

        /// <summary>
        /// Gets result
        /// </summary>
        public double Result => this.value;

        /// <summary>
        /// Gets text result
        /// </summary>
        public string TextResult => null;

        /// <summary>
        /// Gets value
        /// </summary>
        public double Value => this.value;

        /// <summary>
        /// Adds value
        /// </summary>
        /// <param name="value">Value.AnalysisValueFunction</param>
        public void AddValue(double value)
        {
            this.value += value;
            ++this.count;
        }

        /// <summary>
        /// Gets object for key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Returns object for key</returns>
        public object ObjectForKey(string key)
        {
            if (this.dict.ContainsKey(key))
            {
                return this.dict[key];
            }

            return null;
        }

        /// <summary>
        /// Sets object for key
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="key">Key</param>
        public void SetObjectForKey(object obj, string key)
        {
            if (this.dict == null)
            {
                this.dict = new Dictionary<string, object> { { key, obj } };
            }
            else
            {
                this.dict.SetObjectForKey(obj, key);
            }
        }

        private void Increment()
        {
            ++this.count;
        }
    }
}
