// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPPortfolioCell.cs" company="Aurea Software Gmbh">
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
//   UPPortfolioCell
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Web
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPPortfolioCell
    /// </summary>
    public class UPPortfolioCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPPortfolioCell"/> class.
        /// </summary>
        /// <param name="jsonDictionary">The json dictionary.</param>
        public UPPortfolioCell(Dictionary<string, object> jsonDictionary)
        {
            if (jsonDictionary != null)
            {
                if (jsonDictionary.ContainsKey("Color"))
                {
                    this.Color = jsonDictionary["Color"] as string;
                }

                if (jsonDictionary.ContainsKey("Classification"))
                {
                    this.Classification = jsonDictionary["Classification"] as string;
                }

                if (jsonDictionary.ContainsKey("Name"))
                {
                    this.Name = jsonDictionary["Name"] as string;
                }

                if (jsonDictionary.ContainsKey("Result"))
                {
                    this.Result = jsonDictionary["Result"] as string;
                }

                if (jsonDictionary.ContainsKey("Cells"))
                {
                    List<object> cells = jsonDictionary.ValueOrDefault("Cells") as List<object>;
                    if (cells != null && cells.Count == 1)
                    {
                        Dictionary<string, string> cellDictionary = cells[0] as Dictionary<string, string>;
                        if (cellDictionary != null)
                        {
                            this.ColumnNumber = Convert.ToInt32(cellDictionary["ColumnNo"]);
                            this.RowNumber = Convert.ToInt32(cellDictionary["RowNo"]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        /// <value>
        /// The column number.
        /// </value>
        public int ColumnNumber { get; private set; }

        /// <summary>
        /// Gets the row number.
        /// </summary>
        /// <value>
        /// The row number.
        /// </value>
        public int RowNumber { get; private set; }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public string Color { get; private set; }

        /// <summary>
        /// Gets the classification.
        /// </summary>
        /// <value>
        /// The classification.
        /// </value>
        public string Classification { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; private set; }
    }
}
