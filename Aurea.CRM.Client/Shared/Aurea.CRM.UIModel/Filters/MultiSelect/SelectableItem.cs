// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectableItem.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Stefan Stanca
// </author>
// <summary>
//   Selectable item model class for multi select view
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Filters.MultiSelect
{
    using GalaSoft.MvvmLight;

    /// <summary>
    /// Selectable item class
    /// </summary>
    public class SelectableItem : ViewModelBase
    {
        /// <summary>
        /// The data.
        /// </summary>
        private object data;

        /// <summary>
        /// The is selected.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem"/> class.
        /// </summary>
        /// <param name="data">The data
        /// </param>
        public SelectableItem(object data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableItem"/> class.
        /// </summary>
        /// <param name="data">The Data
        /// </param>
        /// <param name="isSelected">If it is selected
        /// </param>
        public SelectableItem(object data, bool isSelected)
        {
            this.Data = data;
            this.IsSelected = isSelected;
        }

        /// <summary>
        /// Gets or sets the Data object
        /// </summary>
        public object Data
        {
            get
            {
                return this.data;
            }

            set
            {
                this.Set(() => this.Data, ref this.data, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is selected
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.Set(() => this.IsSelected, ref this.isSelected, value);
            }
        }

        /// <summary>
        /// Gets the value
        /// </summary>
        public string Value => this.Data.ToString();
    }
}
