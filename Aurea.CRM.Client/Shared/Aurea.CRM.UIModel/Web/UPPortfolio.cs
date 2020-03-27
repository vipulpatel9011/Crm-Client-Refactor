// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPPortfolio.cs" company="Aurea Software Gmbh">
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
//   UPPortfolio
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPPortfolio
    /// </summary>
    public class UPPortfolio
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPPortfolio"/> class.
        /// </summary>
        /// <param name="jsonDictionary">The json dictionary.</param>
        public UPPortfolio(Dictionary<string, object> jsonDictionary)
        {
            List<Dictionary<string, object>> chartInfoArray = jsonDictionary.ValueOrDefault("ChartInfo") as List<Dictionary<string, object>>;
            if (chartInfoArray?.Count > 0 && chartInfoArray[0] != null)
            {
                this.ChartInfo = this.LoadChartinfoFromDictionary(chartInfoArray[0]);
            }

            List<Dictionary<string, object>> portfolioArray = jsonDictionary.ValueOrDefault("Portfolio") as List<Dictionary<string, object>>;
            if (portfolioArray?.Count > 0 && portfolioArray[0] != null)
            {
                this.LoadPortfolioFromDictionary(portfolioArray[0]);
            }
        }

        /// <summary>
        /// Gets the portfolio text.
        /// </summary>
        /// <value>
        /// The portfolio text.
        /// </value>
        public string PortfolioText { get; private set; }

        /// <summary>
        /// Gets the portfolio x.
        /// </summary>
        /// <value>
        /// The portfolio x.
        /// </value>
        public double PortfolioX { get; private set; }

        /// <summary>
        /// Gets the portfolio y.
        /// </summary>
        /// <value>
        /// The portfolio y.
        /// </value>
        public double PortfolioY { get; private set; }

        /// <summary>
        /// Gets the chart information.
        /// </summary>
        /// <value>
        /// The chart information.
        /// </value>
        public UPChartInfo ChartInfo { get; private set; }

        private void LoadPortfolioFromDictionary(Dictionary<string, object> jsonDictionary)
        {
            if (jsonDictionary != null)
            {
                this.PortfolioText = jsonDictionary.ValueOrDefault("Text") as string;
                this.PortfolioX = Convert.ToDouble(jsonDictionary.ValueOrDefault("X"), System.Globalization.CultureInfo.InvariantCulture);
                this.PortfolioY = Convert.ToDouble(jsonDictionary.ValueOrDefault("Y"), System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        private UPChartInfo LoadChartinfoFromDictionary(Dictionary<string, object> jsonDictionary)
        {
            if (jsonDictionary != null)
            {
                return new UPChartInfo(jsonDictionary);
            }

            return null;
        }
    }
}
