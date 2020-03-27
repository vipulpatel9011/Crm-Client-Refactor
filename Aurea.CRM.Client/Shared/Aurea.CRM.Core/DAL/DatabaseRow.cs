// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseRow.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   Defines a database result row information
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.Core.DAL
{
    using Aurea.CRM.Core.Extensions;

    using SQLite;

    /// <summary>
    /// Defines a database result row information
    /// </summary>
    public class DatabaseRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRow"/> class.
        /// </summary>
        public DatabaseRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRow"/> class
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="columnCount">columnCount</param>
        /// <param name="offsets">offsets</param>
        public DatabaseRow(string buffer, int columnCount, int[] offsets)
        {
            //this.ColumnCount = columnCount;
            this.Buffer = buffer;
            this.Offsets = offsets;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRow"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public DatabaseRow(ResultRow result)
        {            
            this.Result = result;
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        public int ColumnCount => this.Result?.Count ?? 0;

        /// <summary>
        /// Gets the offsets.
        /// </summary>
        /// <value>
        /// The offsets.
        /// </value>
        public int[] Offsets { get; set; }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        /// <value>
        /// The buffer.
        /// </value>
        public string Buffer { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        private ResultRow Result { get; set; }

        /// <summary>
        /// Gets the <see cref="string"/> with the specified col number.
        /// </summary>
        /// <value>
        /// The <see cref="string"/>.
        /// </value>
        /// <param name="colNum">
        /// The col number.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[int colNum] => this.Result?.ValueOrDefault(colNum)?.ToString();

        // colNum < ColumnCount ? Buffer + Offsets[colNum] : null;

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="colNum">
        /// The col number.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetColumn(int colNum) => this[colNum];

        /// <summary>
        /// Determines whether the specified col number is null.
        /// </summary>
        /// <param name="colNum">
        /// The col number.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsNull(int colNum) => this.Result?.ValueOrDefault(colNum) == null;

        /// <summary>
        /// Gets the column int.
        /// </summary>
        /// <param name="colNum">
        /// The col number.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetColumnInt(int colNum) => this.GetColumnInt(colNum, 0);

        /// <summary>
        /// Gets the column int.
        /// </summary>
        /// <param name="colNum">
        /// The col number.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetColumnInt(int colNum, int defaultValue)
        {
            int result;
            return int.TryParse(this[colNum], out result) ? result : defaultValue;
        }
    }
}
