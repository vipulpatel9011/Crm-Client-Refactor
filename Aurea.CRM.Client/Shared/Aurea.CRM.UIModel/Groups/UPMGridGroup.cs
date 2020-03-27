// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMGridGroup.cs" company="Aurea Software Gmbh">
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
//   The Grid Group.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Groups
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Column Data Type
    /// </summary>
    public enum UPMColumnDataType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// String
        /// </summary>
        String = 1,

        /// <summary>
        /// Numeric
        /// </summary>
        Numeric = 2,

        /// <summary>
        /// Date
        /// </summary>
        Date = 3
    }

    /// <summary>
    /// Grid Group
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.Groups.UPMListGroup" />
    public class UPMGridGroup : UPMListGroup
    {
        private Dictionary<int, UPMGridColumnInfo> columnInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridGroup"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="action">The action.</param>
        public UPMGridGroup(IIdentifier identifier, UPMAction action)
            : base(identifier, action)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sum row at end].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sum row at end]; otherwise, <c>false</c>.
        /// </value>
        public bool SumRowAtEnd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [fixed first column].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixed first column]; otherwise, <c>false</c>.
        /// </value>
        public bool FixedFirstColumn { get; set; }

        /// <summary>
        /// Gets or sets the goto page action.
        /// </summary>
        /// <value>
        /// The goto page action.
        /// </value>
        public UPMAction GotoPageAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is unsorted state allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is unsorted state allowed; otherwise, <c>false</c>.
        /// </value>
        public bool IsUnsortedStateAllowed { get; set; }

        /// <summary>
        /// Numerics the index of the column at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public bool NumericColumnAtIndex(int index)
        {
            if (this.columnInfos == null)
            {
                return false;
            }

            UPMGridColumnInfo columnInfo = this.columnInfos[index];
            return columnInfo.IsNumeric;
        }

        /// <summary>
        /// Specials the index of the column sort at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public bool SpecialColumnSortAtIndex(int index)
        {
            if (this.columnInfos == null)
            {
                return false;
            }

            UPMGridColumnInfo columnInfo = this.columnInfos[index];
            return columnInfo.SpecialSort;
        }

        /// <summary>
        /// Sets the index of the column information at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="specialSort">if set to <c>true</c> [special sort].</param>
        public void SetColumnInfoAtIndex(int index, UPMColumnDataType dataType, bool specialSort)
        {
            if (this.columnInfos == null)
            {
                this.columnInfos = new Dictionary<int, UPMGridColumnInfo>();
            }

            this.columnInfos[index] = new UPMGridColumnInfo(dataType, specialSort);
        }
    }

    /// <summary>
    /// Grid Column Info
    /// </summary>
    public class UPMGridColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridColumnInfo"/> class.
        /// </summary>
        public UPMGridColumnInfo()
        {
            this.DataType = UPMColumnDataType.Unknown;
            this.SpecialSort = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridColumnInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="specialSort">if set to <c>true</c> [special sort].</param>
        public UPMGridColumnInfo(UPMColumnDataType type, bool specialSort)
        {
            this.DataType = type;
            this.SpecialSort = specialSort;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [special sort].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [special sort]; otherwise, <c>false</c>.
        /// </value>
        public bool SpecialSort { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is numeric.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is numeric; otherwise, <c>false</c>.
        /// </value>
        public bool IsNumeric => this.DataType == UPMColumnDataType.Numeric;

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public UPMColumnDataType DataType { get; set; }
    }
}
