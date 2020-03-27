// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPChartInfo.cs" company="Aurea Software Gmbh">
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
//   The UPChartInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPChartInfo
    /// </summary>
    public class UPChartInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPChartInfo"/> class.
        /// </summary>
        /// <param name="jsonDictionary">The json dictionary.</param>
        public UPChartInfo(Dictionary<string, object> jsonDictionary)
        {
            if (jsonDictionary != null)
            {
                if (jsonDictionary.ContainsKey("Sectors"))
                {
                    List<Dictionary<string, object>> sectors = jsonDictionary.ValueOrDefault("Sectors") as List<Dictionary<string, object>>;
                    if (sectors != null && sectors.Count > 0)
                    {
                        List<UPPortfolioCell> sectorArray = new List<UPPortfolioCell>(sectors.Count);
                        foreach (Dictionary<string, object> bla in sectors)
                        {
                            UPPortfolioCell cell = new UPPortfolioCell(bla);
                            sectorArray.Add(cell);
                        }

                        this.Cells = sectorArray;
                    }
                }

                this.LabelXAxis = jsonDictionary.ValueOrDefault("LabelXAxis") as string;
                this.LabelYAxis = jsonDictionary.ValueOrDefault("LabelYAxis") as string;
                if (jsonDictionary.ContainsKey("PercentagesColumns"))
                {
                    this.PercentageColumns = jsonDictionary["PercentagesColumns"] as List<object>;
                }

                if (jsonDictionary.ContainsKey("PercentagesRows"))
                {
                    this.PercentageRows = jsonDictionary["PercentagesRows"] as List<object>;
                }
            }
        }

        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public List<UPPortfolioCell> Cells { get; private set; }

        /// <summary>
        /// Gets the label x axis.
        /// </summary>
        /// <value>
        /// The label x axis.
        /// </value>
        public string LabelXAxis { get; private set; }

        /// <summary>
        /// Gets the label y axis.
        /// </summary>
        /// <value>
        /// The label y axis.
        /// </value>
        public string LabelYAxis { get; private set; }

        /// <summary>
        /// Gets the percentage columns.
        /// </summary>
        /// <value>
        /// The percentage columns.
        /// </value>
        public List<object> PercentageColumns { get; private set; }

        /// <summary>
        /// Gets the percentage rows.
        /// </summary>
        /// <value>
        /// The percentage rows.
        /// </value>
        public List<object> PercentageRows { get; private set; }

        private UPPortfolioCell FindSectorForRowColumns(int rowNumber, int columnNumber)
        {
            UPPortfolioCell foundCell = null;
            if (this.Cells != null)
            {
                foreach (UPPortfolioCell cell in this.Cells)
                {
                    if (cell.RowNumber == rowNumber && cell.ColumnNumber == columnNumber)
                    {
                        foundCell = cell;
                        break;
                    }
                }
            }

            return foundCell;
        }
    }
}
