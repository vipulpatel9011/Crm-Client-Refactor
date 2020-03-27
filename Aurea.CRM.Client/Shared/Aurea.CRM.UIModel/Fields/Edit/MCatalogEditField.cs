// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MCatalogEditField.cs" company="Aurea Software Gmbh">
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
//   The MCatalogEditField.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Fields.Edit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.Core.Extensions;
    using Core.Configuration;

    /// <summary>
    /// Types of catalog element views
    /// </summary>
    public enum CatalogElementViewType
    {
        /// <summary>
        /// Popover
        /// </summary>
        PopOver = 0,

        /// <summary>
        /// Table
        /// </summary>
        Table = 1,

        /// <summary>
        /// Segmented
        /// </summary>
        Segmented = 2
    }

    /// <summary>
    /// Edit UI field for catalog value
    /// </summary>
    /// <seealso cref="UPMStringEditField" />
    public class UPMCatalogEditField : UPMStringEditField
    {
        private Dictionary<string, UPMCatalogPossibleValue> possibleValues;
        private List<object> fieldValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMCatalogEditField"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="multiSelectMode">if set to <c>true</c> [multi select mode].</param>
        public UPMCatalogEditField(IIdentifier identifier, bool multiSelectMode = false)
            : base(identifier)
        {
            this.CatalogElementViewType = CatalogElementViewType.PopOver;
            this.MultiSelectMode = multiSelectMode;
            if (this.MultiSelectMode)
            {
                this.fieldValues = new List<object>();
            }

            this.ListBoxDataContext = new ObservableCollection<UPMCatalogPossibleValue>();
        }

        /// <summary>
        /// Gets or sets the ListBox data context.
        /// </summary>
        /// <value>
        /// The ListBox data context.
        /// </value>
        public ObservableCollection<UPMCatalogPossibleValue> ListBoxDataContext { get; set; }

        /// <summary>
        /// Gets all keys from possible values.
        /// </summary>
        /// <value>
        /// All keys from possible values.
        /// </value>
        public List<string> AllKeysFromPossibleValues => this.possibleValues?.Keys.ToList();

        /// <summary>
        /// Gets all possible values.
        /// </summary>
        /// <value>
        /// All possible values.
        /// </value>
        public List<UPMCatalogPossibleValue> AllPossibleValues => this.possibleValues?.Values.ToList();

        /// <summary>
        /// Gets or sets the null value key.
        /// </summary>
        /// <value>
        /// The null value key.
        /// </value>
        public string NullValueKey { get; set; }

        /// <summary>
        /// Gets or sets the null value text.
        /// </summary>
        /// <value>
        /// The null value text.
        /// </value>
        public string NullValueText { get; set; }

        /// <summary>
        /// Gets or sets the type of the catalog element view.
        /// </summary>
        /// <value>
        /// The type of the catalog element view.
        /// </value>
        public CatalogElementViewType CatalogElementViewType { get; set; }

        /// <summary>
        /// Gets the field values.
        /// </summary>
        /// <value>
        /// The field values.
        /// </value>
        public List<object> FieldValues
        {
            get
            {
                if (this.MultiSelectMode)
                {
                    return this.fieldValues;
                }

                var curFieldValue = this.FieldValue;
                return curFieldValue != null ? new List<object> { curFieldValue } : null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [multi select mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multi select mode]; otherwise, <c>false</c>.
        /// </value>
        public bool MultiSelectMode { get; set; }

        /// <summary>
        /// Gets or sets the multi select maximum count.
        /// </summary>
        /// <value>
        /// The multi select maximum count.
        /// </value>
        public int MultiSelectMaxCount { get; set; }

        /// <summary>
        /// Gets or sets the explicit key order.
        /// </summary>
        /// <value>
        /// The explicit key order.
        /// </value>
        public List<string> ExplicitKeyOrder { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPMCatalogEditField"/> is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if empty; otherwise, <c>false</c>.
        /// </value>
        public override bool Empty => !this.MultiSelectMode
           ? string.IsNullOrEmpty(this.StringValue) || this.FieldValue.Equals(this.NullValueKey)
           : this.FieldValues.Count == 0;

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <value>
        /// The string display value.
        /// </value>
        public override string StringDisplayValue => this.GetStringDisplayValue();

        /// <summary>
        /// Gets the string display value.
        /// </summary>
        /// <returns></returns>
        private string GetStringDisplayValue()
        {
            if (!this.MultiSelectMode)
            {
                if (this.Empty)
                {
                    return this.NullValueText;
                }

                var possibleValue = this.possibleValues?.ValueOrDefault(this.StringValue);
                return possibleValue?.TitleLabelField?.StringValue;
            }

            var displayValue = string.Empty;
            var sortedfieldValues = this.SortedKeysForKeys(this.fieldValues);
            foreach (var value in sortedfieldValues)
            {
                var possibleValue = this.possibleValues.ValueOrDefault(value);
                displayValue = $"{displayValue}, {possibleValue.TitleLabelField.StringValue}";
            }

            return displayValue.Trim(' ', ',');
        }

        /// <summary>
        /// Adds the possible value.
        /// </summary>
        /// <param name="possibleValue">The possible value.</param>
        /// <param name="key">The key.</param>
        public void AddPossibleValue(UPMCatalogPossibleValue possibleValue, string key = null)
        {
            if (possibleValue == null)
            {
                return;
            }

            if (this.possibleValues == null)
            {
                this.possibleValues = new Dictionary<string, UPMCatalogPossibleValue>();
            }

            this.possibleValues.SetObjectForKey(possibleValue, possibleValue.Key);

            if (string.IsNullOrEmpty(possibleValue.TitleLabelFieldStringValue))
            {
                possibleValue.TitleLabelField.StringValue = LocalizedString.TextEmptyCatalog;
            }

            this.ListBoxDataContext.Add(possibleValue);
        }

        /// <summary>
        /// Sorteds the keys for keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        private List<string> SortedKeysForKeys(List<object> keys)
        {
            var tempKeys = new List<string>();
            List<string> sortedKeys;

            // Wenn explicitKeyOrder gesetzt ist, muss die gesetzte Reihenfolge bleiben
            if (this.ExplicitKeyOrder != null)
            {
                if (keys != null)
                {
                    tempKeys.AddRange(this.ExplicitKeyOrder.Where(keys.Contains));
                    sortedKeys = new List<string>(tempKeys);
                }
                else
                {
                    sortedKeys = new List<string>(this.ExplicitKeyOrder);
                }
            }
            else
            {
                if (keys != null)
                {
                    tempKeys.AddRange(this.AllKeysFromPossibleValues.Where(keys.Contains));

                    sortedKeys = tempKeys;
                    sortedKeys = this.SortKeys(sortedKeys);
                }
                else
                {
                    sortedKeys = this.AllKeysFromPossibleValues;
                    sortedKeys = this.SortKeys(sortedKeys);
                }
            }

            return sortedKeys;
        }

        /// <summary>
        /// Sorts the keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public List<string> SortKeys(List<string> keys)
        {
            keys?.Sort((code1, code2) =>
           {
               if (string.IsNullOrEmpty(code1) || code1.Equals(this.NullValueKey))
               {
                   return -1;
               }

               if (string.IsNullOrEmpty(code2) || code2.Equals(this.NullValueKey))
               {
                   return 1;
               }

               var possibleValue1 = this.PossibleValueForKey(code1);
               var value1 = possibleValue1.TitleLabelField.StringValue;

               var possibleValue2 = this.PossibleValueForKey(code2);
               var value2 = possibleValue2.TitleLabelField.StringValue;

               return string.Compare(value1, value2, StringComparison.Ordinal);
           });

            return keys;
        }

        /// <summary>
        /// Possibles the value for key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UPMCatalogPossibleValue PossibleValueForKey(string key)
        {
            return key != null ? this.possibleValues?.ValueOrDefault(key) : null;
        }

        /// <summary>
        /// Resets the possible values.
        /// </summary>
        public void ResetPossibleValues()
        {
            this.possibleValues = null;
            this.ListBoxDataContext.Clear();
        }

        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        public void SetFieldValue(object fieldValue)
        {
            if (!this.MultiSelectMode)
            {
                var stringValue = fieldValue as string;

                if (this.AllKeysFromPossibleValues == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(stringValue) &&
                    !this.AllKeysFromPossibleValues.Contains(stringValue))
                {
                    return;
                }

                this.FieldValue = fieldValue;
            }
            else
            {
                this.RemoveAllFieldValues();
                if (fieldValue == null)
                {
                    return;
                }

                if (fieldValue is IEnumerable)
                {
                    foreach (var el in fieldValue as IEnumerable)
                    {
                        this.AddFieldValue(el);
                    }
                }

                this.AddFieldValue(fieldValue);
            }
        }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public override object FieldValue
        {
            get
            {
                return !this.MultiSelectMode ? base.FieldValue : this.fieldValues;
            }

            set
            {
                base.FieldValue = value;
            }
        }

        /// <summary>
        /// Adds the field value.
        /// </summary>
        /// <param name="theObject">The object.</param>
        public void AddFieldValue(object theObject)
        {
            if (this.MultiSelectMode &&
                this.fieldValues != null &&
                !this.fieldValues.Contains(theObject) &&
                this.AllKeysFromPossibleValues != null &&
                this.AllKeysFromPossibleValues.Contains(theObject))
            {
                this.fieldValues.Add(theObject as string);
            }
        }

        /// <summary>
        /// Removes all field values.
        /// </summary>
        public void RemoveAllFieldValues()
        {
            if (this.MultiSelectMode)
            {
                this.fieldValues.Clear();
            }
        }

        /// <summary>
        /// Removes the field value.
        /// </summary>
        /// <param name="theObject">The object.</param>
        public void RemoveFieldValue(object theObject)
        {
            if (this.MultiSelectMode)
            {
                this.fieldValues.Remove(theObject);
            }
        }
    }
}
