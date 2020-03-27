// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEColumnSortInfo.cs" company="Aurea Software Gmbh">
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
//   UPSEColumnSortInfo
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// UPSEColumnInfoCompareType
    /// </summary>
    public enum UPSEColumnInfoCompareType
    {
        /// <summary>
        /// Strings insensitive
        /// </summary>
        StringsInsensitive = 0,

        /// <summary>
        /// Number
        /// </summary>
        Number
    }

    /// <summary>
    /// ComparisonResult
    /// </summary>
    public enum ComparisonResult
    {
        /// <summary>
        /// Same
        /// </summary>
        Same = 0,

        /// <summary>
        /// Ascending
        /// </summary>
        Ascending = -1,

        /// <summary>
        /// Descending
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// UPSEColumnSortInfo
    /// </summary>
    public class UPSEColumnSortInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEColumnSortInfo"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="fieldConfig">The field configuration.</param>
        /// <exception cref="Exception">SortIndex is null</exception>
        public UPSEColumnSortInfo(UPSEColumn column, UPConfigFieldControlField fieldConfig)
        {
            string sortIndexString = fieldConfig.Attributes.ExtendedOptionForKey("SortIndex");
            if (string.IsNullOrEmpty(sortIndexString))
            {
                return;
               // throw new Exception("SortIndex is null");
            }

            this.SortIndex = Convert.ToInt32(sortIndexString);
            this.Column = column;
            this.Descending = fieldConfig.Attributes.ExtendedOptionIsSet("SortDescending");
            this.CompareType = fieldConfig.Field.IsNumericField ? UPSEColumnInfoCompareType.Number : UPSEColumnInfoCompareType.StringsInsensitive;
        }

        /// <summary>
        /// Gets the index of the sort.
        /// </summary>
        /// <value>
        /// The index of the sort.
        /// </value>
        public int SortIndex { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPSEColumnSortInfo"/> is descending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if descending; otherwise, <c>false</c>.
        /// </value>
        public bool Descending { get; private set; }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public UPSEColumn Column { get; private set; }

        /// <summary>
        /// Gets the type of the compare.
        /// </summary>
        /// <value>
        /// The type of the compare.
        /// </value>
        public UPSEColumnInfoCompareType CompareType { get; private set; }

        /// <summary>
        /// Determines whether [is empty string for row] [the specified row].
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        ///   <c>true</c> if [is empty string for row] [the specified row]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmptyStringForRow(UPSERow row)
        {
            string string1 = this.Column?.StringValueFromObject(row.ValueAtIndex(this.Column.Index));
            return string.IsNullOrEmpty(string1);
        }

        /// <summary>
        /// Determines whether [is empty number for row] [the specified row].
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        ///   <c>true</c> if [is empty number for row] [the specified row]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmptyNumberForRow(UPSERow row)
        {
            var number1 = this.Column.NumberFromValue(row.ValueAtIndex(this.Column.Index));
            return number1 == 0;
        }

        /// <summary>
        /// Determines whether [is empty for row] [the specified row].
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        ///   <c>true</c> if [is empty for row] [the specified row]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmptyForRow(UPSERow row)
        {
            switch (this.CompareType)
            {
                case UPSEColumnInfoCompareType.Number:
                    return this.IsEmptyNumberForRow(row);

                default:
                    return this.IsEmptyStringForRow(row);
            }
        }

        /// <summary>
        /// Compares the row with row.
        /// </summary>
        /// <param name="row1">The row1.</param>
        /// <param name="row2">The row2.</param>
        /// <returns></returns>
        public ComparisonResult CompareRowWithRow(UPSERow row1, UPSERow row2)
        {
            switch (this.CompareType)
            {
                case UPSEColumnInfoCompareType.Number:
                    return this.CompareRowNumberValuesWithRow(row1, row2);

                case UPSEColumnInfoCompareType.StringsInsensitive:
                    return this.CompareRowStringValuesWithRow(row1, row2);

                default:
                    return ComparisonResult.Same;
            }
        }

        /// <summary>
        /// Compares the row number values with row.
        /// </summary>
        /// <param name="row1">The row1.</param>
        /// <param name="row2">The row2.</param>
        /// <returns></returns>
        private ComparisonResult CompareRowNumberValuesWithRow(UPSERow row1, UPSERow row2)
        {
            decimal number1 = this.Column.NumberFromValue(row1.ValueAtIndex(this.Column.Index));
            decimal number2 = this.Column.NumberFromValue(row2.ValueAtIndex(this.Column.Index));

            if (number1 == 0)
            {
                return number2 == 0 ? ComparisonResult.Same : ComparisonResult.Descending;
            }

            if (number2 == 0)
            {
                return ComparisonResult.Ascending;
            }

            if (number1 < number2)
            {
                return this.Descending ? ComparisonResult.Descending : ComparisonResult.Ascending;
            }

            if (number1 > number2)
            {
                return this.Descending ? ComparisonResult.Ascending : ComparisonResult.Descending;
            }

            return ComparisonResult.Same;
        }

        /// <summary>
        /// Compares the row string values with row.
        /// </summary>
        /// <param name="row1">The row1.</param>
        /// <param name="row2">The row2.</param>
        /// <returns></returns>
        private ComparisonResult CompareRowStringValuesWithRow(UPSERow row1, UPSERow row2)
        {
            string string1 = this.Column.StringValueFromObject(row1.ValueAtIndex(this.Column.Index));
            string string2 = this.Column.StringValueFromObject(row2.ValueAtIndex(this.Column.Index));

            if (string.IsNullOrEmpty(string1))
            {
                return string.IsNullOrEmpty(string2) ? ComparisonResult.Same : ComparisonResult.Descending;
            }

            if (string.IsNullOrEmpty(string2))
            {
                return ComparisonResult.Ascending;
            }

            int res = string.Compare(string1, string2, StringComparison.OrdinalIgnoreCase);

            switch (res)
            {
                case 1:
                    return this.Descending ? ComparisonResult.Descending : ComparisonResult.Ascending;

                case -1:
                    return this.Descending ? ComparisonResult.Ascending : ComparisonResult.Descending;
            }

            return ComparisonResult.Same;
        }
    }
}
