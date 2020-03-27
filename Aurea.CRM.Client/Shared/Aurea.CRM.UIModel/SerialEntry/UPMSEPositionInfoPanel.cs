// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMSEPositionInfoPanel.cs" company="Aurea Software Gmbh">
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
//   UPMSEPositionInfoPanel
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// UPInfoPanelCellTextAlignment
    /// </summary>
    public enum UPInfoPanelCellTextAlignment
    {
        /// <summary>
        /// Left
        /// </summary>
        Left = 0,

        /// <summary>
        /// Center
        /// </summary>
        Center = 1,

        /// <summary>
        /// Right
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// UPInfoPanelTableTyp
    /// </summary>
    public enum UPInfoPanelTableTyp
    {
        /// <summary>
        /// Vertikal
        /// </summary>
        Vertikal = 0,

        /// <summary>
        /// Horizontal
        /// </summary>
        Horizontal = 1
    }

    /// <summary>
    /// UPMSEPositionInfoPanel
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMGroup" />
    public class UPMSEPositionInfoPanel : UPMGroup
    {
        private Dictionary<int, float> columnWidthsInPercent;
        private Dictionary<string, UPInfoPanelCellTextAlignment> textAlignmentForCells;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMSEPositionInfoPanel"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public UPMSEPositionInfoPanel(IIdentifier identifier)
            : base(identifier)
        {
            this.TableTyp = UPInfoPanelTableTyp.Vertikal;
        }

        /// <summary>
        /// Gets or sets the title field.
        /// </summary>
        /// <value>
        /// The title field.
        /// </value>
        public UPMStringField TitleField { get; set; }

        /// <summary>
        /// Gets or sets the column names.
        /// </summary>
        /// <value>
        /// The column names.
        /// </value>
        public List<UPMStringField> ColumnNames { get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public List<UPMContainer> Rows { get; set; }

        /// <summary>
        /// Gets or sets the table typ.
        /// </summary>
        /// <value>
        /// The table typ.
        /// </value>
        public UPInfoPanelTableTyp TableTyp { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPMSEPositionInfoPanel"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public bool Empty => this.ColumnCount == 0 || this.RowCount == 0;

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        public int ColumnCount => this.ColumnNames.Count;

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <value>
        /// The row count.
        /// </value>
        public int RowCount => this.Rows.Count;

        /// <summary>
        /// Columns the name.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public UPMStringField ColumnName(int column)
        {
            return column < this.ColumnNames.Count ? this.ColumnNames[column] : null;
        }

        /// <summary>
        /// Values the column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public UPMStringField ValueColumn(int row, int column)
        {
            if (row < this.Rows.Count)
            {
                UPMContainer currentRow = this.Rows[row];
                if (column < currentRow.Children.Count)
                {
                    return currentRow.Children[column] as UPMStringField;
                }
            }

            return null;
        }

        /// <summary>
        /// Texts the alignment for row column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public UPInfoPanelCellTextAlignment TextAlignmentForRowColumn(int row, int column)
        {
            if (this.textAlignmentForCells != null)
            {
                return this.textAlignmentForCells[$"{row}#{column}"];
            }

            return UPInfoPanelCellTextAlignment.Left;
        }

        /// <summary>
        /// Updates the text alignment for cell row column.
        /// </summary>
        /// <param name="textAlignment">The text alignment.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        public void UpdateTextAlignmentForCellRowColumn(UPInfoPanelCellTextAlignment textAlignment, int row, int column)
        {
            if (this.textAlignmentForCells == null)
            {
                this.textAlignmentForCells = new Dictionary<string, UPInfoPanelCellTextAlignment>();
            }

            this.textAlignmentForCells[$"{row}#{column}"] = textAlignment;
        }

        /// <summary>
        /// Columns the width in percent.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public float ColumnWidthInPercent(int column)
        {
            if (this.columnWidthsInPercent != null && this.columnWidthsInPercent.ContainsKey(column))
            {
                return this.columnWidthsInPercent.ValueOrDefault(column);
            }

            return this.ColumnCount > 0 ? 100.0f / this.ColumnCount : 100.0f;
        }

        /// <summary>
        /// Removes the empty rows.
        /// </summary>
        public void RemoveEmptyRows()
        {
            List<UPMContainer> rowsToDelete = new List<UPMContainer>();
            for (int i = 0; i < this.Rows.Count; i++)
            {
                UPMContainer row = this.Rows[i];
                bool leer = true;
                for (int j = 0; j < row.Children.Count; j++)
                {
                    if (((UPMStringField)row.Children[j]).StringValue.Length != 0)
                    {
                        leer = false;
                        break;
                    }
                }

                if (leer)
                {
                    rowsToDelete.Add(row);
                }
            }

            List<UPMContainer> rowsNew = new List<UPMContainer>(this.Rows);
            foreach (UPMContainer row in rowsToDelete)
            {
                rowsNew.Remove(row);
            }

            this.Rows = rowsNew;
        }

        /// <summary>
        /// Updates the column width in percent column.
        /// </summary>
        /// <param name="widthInPercent">The width in percent.</param>
        /// <param name="column">The column.</param>
        public void UpdateColumnWidthInPercentColumn(float widthInPercent, int column)
        {
            if (this.columnWidthsInPercent == null)
            {
                this.columnWidthsInPercent = new Dictionary<int, float>();
            }

            this.columnWidthsInPercent[column] = widthInPercent;
        }
    }
}
