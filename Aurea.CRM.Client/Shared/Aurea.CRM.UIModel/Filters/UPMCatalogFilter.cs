// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPMCatalogFilter.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The upm catalog filter value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Filters.MultiSelect;
    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// The upm catalog filter value.
    /// </summary>
    public class UPMCatalogFilterValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCatalogFilterValue"/> class.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public UPMCatalogFilterValue(string label, AureaColor color)
        {
            this.Label = label;
            this.Color = color;
        }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public AureaColor Color { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public object Image { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }
    }

    /// <summary>
    /// The upm catalog filter.
    /// </summary>
    public class UPMCatalogFilter : UPMFilter
    {
        /// <summary>
        /// The selectable items.
        /// </summary>
        private ObservableCollection<SelectableItem> selectableItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCatalogFilter"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        public UPMCatalogFilter(IIdentifier identifier)
            : base(identifier, UPMFilterType.Catalog)
        {
            this.SelectedCatalogCodes = new List<string>();
            this.Invalid = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCatalogFilter"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public UPMCatalogFilter(IIdentifier identifier, UPCatalogValueProvider provider)
            : base(identifier, provider.Dependent ? UPMFilterType.DependentCatalog : UPMFilterType.Catalog)
        {
            this.SelectedCatalogCodes = new List<string>();
            this.FilterValueProvider = provider;
            this.Invalid = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCatalogFilter"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="dependent">
        /// The dependent.
        /// </param>
        public UPMCatalogFilter(IIdentifier identifier, bool dependent)
            : base(identifier, dependent ? UPMFilterType.DependentCatalog : UPMFilterType.Catalog)
        {
            this.SelectedCatalogCodes = new List<string>();
            this.ExplicitCatalogValues = new Dictionary<string, string>();
            this.Invalid = false;
        }

        /// <summary>
        /// Gets the select all
        /// </summary>
        public string TextSelectAll => LocalizedString.TextSelectAll;

        /// <summary>
        /// Gets the Clear all
        /// </summary>
        public string TextClearAll => LocalizedString.TextClearAll;

        /// <summary>
        /// Gets the Select all command
        /// </summary>
        public RelayCommand SelectAllCommand => new RelayCommand(this.SelectAll);

        /// <summary>
        /// Gets the Item Tapped Command
        /// </summary>
        public RelayCommand<SelectableItem> ItemTappedCommand => new RelayCommand<SelectableItem>(this.ItemTapped);

        /// <summary>
        /// Gets the Clear all command
        /// </summary>
        public RelayCommand ClearAllCommand => new RelayCommand(this.ClearValue);

        /// <summary>
        /// Gets the catalog value dictionary.
        /// </summary>
        public Dictionary<string, string> CatalogValueDictionary =>
            this.ExplicitCatalogValues?.Count == 0
                ? this.FilterValueProvider.PossibleValues()
                : this.ExplicitCatalogValues ?? this.FilterValueProvider.PossibleValues();

        /// <summary>
        /// Gets or sets the explicit catalog values.
        /// </summary>
        public Dictionary<string, string> ExplicitCatalogValues { get; set; }

        /// <summary>
        /// Gets or sets the filter value provider.
        /// </summary>
        public UPCatalogValueProvider FilterValueProvider { get; set; }

        /// <inheritdoc/>
        public override bool HasParameters => !string.IsNullOrEmpty(this.ParameterName);

        /// <summary>
        /// Gets or sets the null value key.
        /// </summary>
        public string NullValueKey { get; set; }

        /// <summary>
        /// Gets or sets the parameter name.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets the raw values.
        /// </summary>
        public override List<string> RawValues => this.SelectedCatalogCodes;

        /// <summary>
        /// Gets an observable collection of selectable items for the Catalog Filter
        /// </summary>
        public ObservableCollection<SelectableItem> SelectableItems
        {
            get
            {
                if (this.selectableItems == null)
                {
                    this.selectableItems = new ObservableCollection<SelectableItem>(this.PrepareCatalogValues().AsEnumerable().Select(kvp => new SelectableItem(kvp)).ToList());
                }

                return this.selectableItems;
            }
        }

        /// <summary>
        /// Gets the selected catalog codes.
        /// </summary>
        public List<string> SelectedCatalogCodes { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether single select.
        /// </summary>
        public bool SingleSelect { get; set; }

        /// <inheritdoc/>
        public override void ClearValue()
        {
            this.SelectedCatalogCodes?.Clear();
            if (this.SelectableItems != null)
            {
                foreach (var item in this.SelectableItems)
                {
                    item.IsSelected = false;
                }
            }

            base.ClearValue();
        }

        /// <summary>
        /// The select all.
        /// </summary>
        public void SelectAll()
        {
            var allCodes = this.PrepareCatalogValues().Keys;
            this.SelectedCatalogCodes?.Clear();
            this.SelectedCatalogCodes?.AddRange(allCodes);
            foreach (var item in this.SelectableItems)
            {
                item.IsSelected = true;
            }

            this.UpdateSummary();
        }

        /// <summary>
        /// The count selected catalogs for parent.
        /// </summary>
        /// <param name="parentKey">
        /// The parent key.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CountSelectedCatalogsForParent(string parentKey)
        {
            List<string> availableKeys;
            if (this.FilterValueProvider != null)
            {
                availableKeys = this.FilterValueProvider.PossibleValuesForParentCode(parentKey).Keys.Select(x => x)
                    .ToList();
            }
            else
            {
                availableKeys = null; // this.ExplicitCatalogValues.ObjectForKey(parentKey).AllKeys();
            }

            return this.SelectedCatalogCodes.Count(code => availableKeys.Contains(code));
        }

        /// <summary>
        /// The selected catalogs display value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string SelectedCatalogsDisplayValue()
        {
            if (this.FilterValueProvider != null)
            {
                return this.FilterValueProvider.DisplayStringForCodes(this.SelectedCatalogCodes);
            }

            StringBuilder mutableString = new StringBuilder();
            if (this.FilterType == UPMFilterType.DependentCatalog)
            {
                foreach (string parent in this.ExplicitCatalogValues.Keys)
                {
                    string vDisplay = this.SelectedCatalogsDisplayValueForParent(parent);
                    if (!string.IsNullOrEmpty(vDisplay))
                    {
                        if (mutableString.Length > 0)
                        {
                            mutableString.Append(", ");
                        }

                        mutableString.Append(parent);
                        mutableString.Append(" (");
                        mutableString.Append(vDisplay);
                        mutableString.Append(")");
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.SelectedCatalogCodes.Count; i++)
                {
                    if (i > 0)
                    {
                        mutableString.Append(", ");
                    }

                    string code = this.SelectedCatalogCodes[i];
                    if (code == this.NullValueKey)
                    {
                        mutableString.Append("upText_EmptyCatalog");
                    }
                    else
                    {
                        mutableString.Append(this.ExplicitCatalogValues[code]);
                    }
                }
            }

            return mutableString.ToString();
        }

        /// <summary>
        /// The selected catalogs display value for parent.
        /// </summary>
        /// <param name="parentKey">
        /// The parent key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string SelectedCatalogsDisplayValueForParent(string parentKey)
        {
            StringBuilder mutableString = new StringBuilder();
            Dictionary<string, string> valuesForParent = null;
            bool first = true;
            int parentKeyInt = Convert.ToInt32(parentKey);
            for (int i = 0; i < this.SelectedCatalogCodes.Count; i++)
            {
                string code = this.SelectedCatalogCodes[i];
                if (this.FilterValueProvider != null
                    && this.FilterValueProvider.ParentCodeForCode(code) != parentKeyInt)
                {
                    continue;
                }

                string value;
                if (code == this.NullValueKey)
                {
                    value = "upText_EmptyCatalog";
                }
                else
                {
                    if (valuesForParent == null)
                    {
                        if (this.FilterValueProvider != null)
                        {
                            valuesForParent = this.FilterValueProvider.PossibleValuesForParentCode(parentKey);
                        }
                        else
                        {
                            valuesForParent = null; // this.ExplicitCatalogValues[parentKey];
                        }
                    }

                    value = valuesForParent[code];
                }

                if (value != null)
                {
                    if (!first)
                    {
                        mutableString.Append(", ");
                    }
                    else
                    {
                        first = false;
                    }

                    mutableString.Append(value);
                }
            }

            return mutableString.ToString();
        }

        /// <summary>
        /// Update Catalog values from SelectableItems list
        /// </summary>
        /// <param name="item">The Tapped item</param>
        public void ItemTapped(SelectableItem item)
        {
            item.IsSelected = !item.IsSelected;
            var selectedCodes = this.SelectableItems.Where(s => s.IsSelected)
                .Select(s => (s.Data as KeyValuePair<string, string>?))
                .Where(s => s.HasValue)
                .Select(s => s.Value.Key)
                .ToList();
            this.SelectedCatalogCodes = selectedCodes;
            this.UpdateSummary();
        }

        /// <summary>
        /// Prepares the catalog values for display
        /// </summary>
        /// <returns><see cref="KeyValuePair{TKey,TValue}"/></returns>
        protected Dictionary<string, string> PrepareCatalogValues()
        {
            var newDict = new Dictionary<string, string>();
            var myList = this.CatalogValueDictionary?.ToList();

            if (myList == null)
            {
                return newDict;
            }

            myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            foreach (var value in myList)
            {
                if (string.IsNullOrWhiteSpace(value.Value))
                {
                    newDict.Add(value.Key, LocalizedString.TextEmptyCatalog);
                }
                else
                {
                    newDict.Add(value.Key, value.Value);
                }
            }

            return newDict;
        }
    }
}
