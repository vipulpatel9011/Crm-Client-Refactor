// <copyright file="UPMGridPage.cs" company="Aurea Software Gmbh">
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

namespace Aurea.CRM.UIModel.Pages
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// Implementation of grid page class
    /// </summary>
    public class UPMGridPage : UPMSearchPage
    {
        private List<UPMGridCategory> categories;
        private Dictionary<int, UPMGridColumnInfo> columnInfos;
        private List<UPMGridHeadOption> headOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMGridPage"/> class.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        public UPMGridPage(IIdentifier identifier)
            : base(identifier)
        {
        }

        /// <summary>
        /// Gets or sets empty category text
        /// </summary>
        public string EmptyCategoryText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fixed first column
        /// </summary>
        public bool FixedFirstColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fixed sum row
        /// </summary>
        public bool FixedSumRow { get; set; }

        /// <summary>
        /// Gets or sets grid categories
        /// </summary>
        public List<UPMGridCategory> GridCategories
        {
            get
            {
                return this.categories;
            }

            set
            {
                this.categories = value;
            }
        }

        /// <summary>
        /// Gets or sets grid header options
        /// </summary>
        public List<UPMGridHeadOption> GridHeadOptions
        {
            get
            {
                return this.headOptions;
            }

            set
            {
                this.headOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets initial sort column
        /// </summary>
        public int InitialSortColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is unsorted state allowed
        /// </summary>
        public bool IsUnsortedStateAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether reset
        /// </summary>
        public bool Reset { get; set; }

        /// <summary>
        /// Gets or sets reset head option
        /// </summary>
        public UPMGridHeadOption ResetHeadOption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show menu
        /// </summary>
        public bool ShowMenu { get; set; }

        /// <summary>
        /// Gets or sets sum result row
        /// </summary>
        public UPMResultRow SumResultRow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sum row at end
        /// </summary>
        public bool SumRowAtEnd { get; set; }

        /// <summary>
        /// Adds category
        /// </summary>
        /// <param name="gridCategory">Grid category</param>
        public void AddCategory(UPMGridCategory gridCategory)
        {
            if (this.categories == null)
            {
                this.categories = new List<UPMGridCategory> { gridCategory };
            }
            else
            {
                this.categories.Add(gridCategory);
            }
        }

        /// <summary>
        /// Adds head option
        /// </summary>
        /// <param name="headOption">Head option</param>
        public void AddHeadOption(UPMGridHeadOption headOption)
        {
            if (this.headOptions == null)
            {
                this.headOptions = new List<UPMGridHeadOption> { headOption };
            }
            else
            {
                this.headOptions.Add(headOption);
            }
        }

        /// <summary>
        /// Sets column info at index data
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="dataType">Data type</param>
        /// <param name="specialSort">Special sort</param>
        public void SetColumnInfoAtIndexDataTypeSpecialSort(int index, UPMColumnDataType dataType, bool specialSort)
        {
            if (this.columnInfos == null)
            {
                this.columnInfos = new Dictionary<int, UPMGridColumnInfo>();
            }

            this.columnInfos.SetObjectForKey(new UPMGridColumnInfo(dataType, specialSort), index);
        }

        private bool NumericColumnAtIndex(int index)
        {
            if (this.columnInfos == null)
            {
                return false;
            }

            UPMGridColumnInfo columnInfo = this.columnInfos[index];
            return columnInfo != null ? columnInfo.IsNumeric : false;
        }

        private bool SpecialColumnSortAtIndex(int index)
        {
            if (this.columnInfos == null)
            {
                return false;
            }

            UPMGridColumnInfo columnInfo = this.columnInfos[index];
            return columnInfo != null ? columnInfo.SpecialSort : false;
        }
    }
}
