// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSESortInfo.cs" company="Aurea Software Gmbh">
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
//   Sort Info
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;

    /// <summary>
    /// Sort Info
    /// </summary>
    public class UPSESortInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSESortInfo"/> class.
        /// </summary>
        /// <param name="columnSortInfoArray">The column sort information array.</param>
        public UPSESortInfo(List<UPSEColumnSortInfo> columnSortInfoArray)
        {
            List<UPSEColumnSortInfo> sortedArray = new List<UPSEColumnSortInfo>(columnSortInfoArray.Count);
            foreach (UPSEColumnSortInfo current in columnSortInfoArray)
            {
                int currentIndex = 0;
                foreach (UPSEColumnSortInfo existing in sortedArray)
                {
                    if (existing.SortIndex >= current.SortIndex)
                    {
                        break;
                    }

                    ++currentIndex;
                }

                if (currentIndex == sortedArray.Count)
                {
                    sortedArray.Add(current);
                }
                else
                {
                    sortedArray.Insert(currentIndex, current);
                }
            }

            this.ColumnSortInfoArray = sortedArray;
        }

        /// <summary>
        /// Gets the column sort information array.
        /// </summary>
        /// <value>
        /// The column sort information array.
        /// </value>
        public List<UPSEColumnSortInfo> ColumnSortInfoArray { get; private set; }

        /// <summary>
        /// Sorts the rows.
        /// </summary>
        /// <param name="rowArray">The row array.</param>
        /// <returns></returns>
        public List<UPSERow> SortRows(List<UPSERow> rowArray)
        {
            List<UPSERow> sortableColumns = null;
            List<UPSERow> unsortableColumns = null;

            foreach (UPSERow row in rowArray)
            {
                bool isEmpty = true;
                row.EnsureLoaded();
                foreach (UPSEColumnSortInfo columnSortInfo in this.ColumnSortInfoArray)
                {
                    if (!columnSortInfo.IsEmptyForRow(row))
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                {
                    if (unsortableColumns == null)
                    {
                        unsortableColumns = new List<UPSERow> { row };
                    }
                    else
                    {
                        unsortableColumns.Add(row);
                    }
                }
                else
                {
                    if (sortableColumns == null)
                    {
                        sortableColumns = new List<UPSERow> { row };
                    }
                    else
                    {
                        sortableColumns.Add(row);
                    }
                }
            }

            if (sortableColumns == null)
            {
                return rowArray;
            }

            //List<UPSERow> sortedArray = sortableColumns.SortedArrayUsingComparator(delegate (UPSERow obj1, UPSERow obj2)
            //{
            //    return this.CompareRowWithRow(obj1, obj2);
            //});

            List<UPSERow> sortedArray = sortableColumns;

            if (unsortableColumns?.Count > 0)
            {
                sortedArray.AddRange(unsortableColumns);
            }

            return sortedArray;
        }

        /// <summary>
        /// Compares the row with row.
        /// </summary>
        /// <param name="row1">The row1.</param>
        /// <param name="row2">The row2.</param>
        /// <returns></returns>
        private ComparisonResult CompareRowWithRow(UPSERow row1, UPSERow row2)
        {
            foreach (UPSEColumnSortInfo columnSortInfo in this.ColumnSortInfoArray)
            {
                var compareResult = columnSortInfo.CompareRowWithRow(row1, row2);
                if (compareResult != ComparisonResult.Same)
                {
                    return compareResult;
                }
            }

            return ComparisonResult.Same;
        }
    }
}
