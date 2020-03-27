// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmListFormatter.cs" company="Aurea Software Gmbh">
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
//   List formatter function data provider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// List formatter function data provider
    /// </summary>
    public interface UPCRMListFormatterFunctionDataProvider
    {
        /// <summary>
        /// The raw value for function name.
        /// </summary>
        /// <param name="functionName">
        /// The function name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string RawValueForFunctionName(string functionName);
    }

    /// <summary>
    /// List formatter
    /// </summary>
    public class UPCRMListFormatter
    {
        /// <summary>
        /// The _format options.
        /// </summary>
        private readonly UPFormatOption _formatOptions;

        /// <summary>
        /// The _positions.
        /// </summary>
        private readonly List<UPCRMListFormatterPosition> _positions;

        /// <summary>
        /// The _remove line breaks.
        /// </summary>
        private readonly bool _removeLineBreaks;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMListFormatter"/> class using FieldControl.
        /// </summary>
        /// <param name="fieldControl">
        /// The field Control.
        /// </param>
        public UPCRMListFormatter(FieldControl fieldControl)
            : this(fieldControl.TabAtIndex(0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMListFormatter"/> class  using FieldControl.
        /// </summary>
        /// <param name="fieldControl">
        /// The field Control.
        /// </param>
        /// <param name="fieldCount">
        /// The field count.
        /// </param>
        public UPCRMListFormatter(FieldControl fieldControl, int fieldCount)
            : this(fieldControl.TabAtIndex(0), fieldCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMListFormatter"/> class.
        /// </summary>
        /// <param name="fieldControlTab">
        /// The field control tab.
        /// </param>
        /// <param name="fieldCount">
        /// The field count.
        /// </param>
        /// <param name="removeLineBreaks">
        /// if set to <c>true</c> [_remove line breaks].
        /// </param>
        public UPCRMListFormatter(FieldControlTab fieldControlTab, int fieldCount, bool removeLineBreaks)
            : this(fieldControlTab, fieldCount)
        {
            this._removeLineBreaks = removeLineBreaks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMListFormatter"/> class.
        /// </summary>
        /// <param name="fieldControlTab">
        /// The field control tab.
        /// </param>
        /// <param name="removeLineBreaks">
        /// The remove Line Breaks.
        /// </param>
        public UPCRMListFormatter(FieldControlTab fieldControlTab, bool removeLineBreaks)
            : this(fieldControlTab, fieldControlTab?.Fields?.Count ?? 0, removeLineBreaks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMListFormatter"/> class.
        /// </summary>
        /// <param name="fieldControlTab">
        /// The field control tab.
        /// </param>
        public UPCRMListFormatter(FieldControlTab fieldControlTab)
            : this(fieldControlTab, fieldControlTab?.Fields?.Count ?? 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMListFormatter"/> class.
        /// </summary>
        /// <param name="fieldControlTab">
        /// The field control tab.
        /// </param>
        /// <param name="fieldCount">
        /// The field count.
        /// </param>
        public UPCRMListFormatter(FieldControlTab fieldControlTab, int fieldCount)
        {
            List<UPCRMListFormatterPosition> positionArray = null;
            var childFieldCount = 0;
            UPCRMListFormatterPosition currentPosition = null;
            this.DisplayNo = ServerSession.CurrentSession.UpTextForNO();

            foreach (var field in fieldControlTab.Fields ?? new List<UPConfigFieldControlField>())
            {
                if (fieldCount-- == 0)
                {
                    break;
                }

                if (childFieldCount > 0)
                {
                    currentPosition?.AddField(field);
                    --childFieldCount;

                    // PVCS 87654 Kombinierte Personendarstellung (3 Felder) in GRID View
                    // wenn das letzte Feld einen colSpan hat, so erhÃ¶ht sich der childFieldCount
                    if (childFieldCount == 0)
                    {
                        childFieldCount = field.Attributes.FieldCount - 1;
                    }
                }
                else
                {
                    if (field.TargetFieldNumber > 0 && field.TargetFieldNumber < 7)
                    {
                        if (positionArray == null)
                        {
                            positionArray = new List<UPCRMListFormatterPosition>();
                        }

                        int positionIndex = field.TargetFieldNumber - 1;
                        if (positionArray.Count <= positionIndex)
                        {
                            while (positionArray.Count < positionIndex)
                            {
                                positionArray.Add(null);
                            }

                            currentPosition = new UPCRMListFormatterPosition { ListFormatter = this };
                            positionArray.Add(currentPosition);
                        }
                        else if (positionArray[positionIndex] == null)
                        {
                            currentPosition = new UPCRMListFormatterPosition { ListFormatter = this };
                            positionArray[positionIndex] = currentPosition;
                        }
                        else
                        {
                            currentPosition = positionArray[positionIndex];
                        }
                    }
                    else
                    {
                        currentPosition = new UPCRMListFormatterPosition { ListFormatter = this };
                        if (positionArray == null)
                        {
                            positionArray = new List<UPCRMListFormatterPosition> { currentPosition };
                        }
                        else
                        {
                            positionArray.Add(currentPosition);
                        }
                    }

                    childFieldCount = field.Attributes.FieldCount - 1;
                    currentPosition.AddField(field);
                }
            }

            this._positions = positionArray;
            var configStore = ConfigurationUnitStore.DefaultStore;
            var show0 = configStore.ConfigValueIsSet("Format.Show0InLists");
            var show0Float =
                configStore.ConfigValueIsSet(
                    configStore.WebConfigValueByName("Format.Show0InListsForFloat") != null
                        ? "Format.Show0InListsForFloat"
                        : "Format:Show0InListsForFloat");

            if (show0)
            {
                this._formatOptions |= UPFormatOption.Show0;
            }

            if (show0Float)
            {
                this._formatOptions |= UPFormatOption.Show0Float;
            }

            if (this._removeLineBreaks)
            {
                this._formatOptions |= UPFormatOption.DontRemoveLineBreak;
            }
        }

        /// <summary>
        /// Gets the display no.
        /// </summary>
        /// <value>
        /// The display no.
        /// </value>
        public string DisplayNo { get; private set; }

        /// <summary>
        /// Gets the position count.
        /// </summary>
        /// <value>
        /// The position count.
        /// </value>
        public int PositionCount => this._positions?.Count ?? 0;

        /// <summary>
        /// Firsts the field for position.
        /// </summary>
        /// <param name="positionIndex">
        /// Index of the position.
        /// </param>
        /// <returns>
        /// The <see cref="UPConfigFieldControlField"/>.
        /// </returns>
        public UPConfigFieldControlField FirstFieldForPosition(int positionIndex)
        {
            if (this._positions.Count <= positionIndex)
            {
                return null;
            }

            var position = this._positions[positionIndex];
            return position?.FirstField;
        }

        /// <summary>
        /// Positions the strings from field array.
        /// </summary>
        /// <param name="fieldArray">
        /// The field array.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> PositionStringsFromFieldArray(List<string> fieldArray)
        {
            int i, count = this._positions.Count;
            var stringArray = new List<string>(count);
            for (i = 0; i < count; i++)
            {
                var rowString = this.StringFromArrayForPosition(fieldArray, i) ?? string.Empty;
                stringArray.Add(rowString);
            }

            return stringArray;
        }

        /// <summary>
        /// Strings from array for position.
        /// </summary>
        /// <param name="fieldArray">
        /// The field array.
        /// </param>
        /// <param name="positionIndex">
        /// Index of the position.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromArrayForPosition(List<string> fieldArray, int positionIndex)
        {
            if (positionIndex >= this._positions.Count)
            {
                return string.Empty;
            }

            var position = this._positions[positionIndex];
            return position?.StringFromValueArrayOptions(fieldArray, this._formatOptions) ?? string.Empty;
        }

        /// <summary>
        /// Strings from provider for position.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="positionIndex">
        /// Index of the position.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromProviderForPosition(
            UPCRMListFormatterFunctionDataProvider dataProvider,
            int positionIndex)
        {
            if (positionIndex >= this._positions.Count)
            {
                return string.Empty;
            }

            var position = this._positions[positionIndex];
            return position?.StringFromDataProviderOptions(dataProvider, this._formatOptions) ?? string.Empty;
        }

        /// <summary>
        /// Strings from row for position.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="positionIndex">
        /// Index of the position.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromRowForPosition(UPCRMResultRow row, int positionIndex)
        {
            if (positionIndex >= this._positions.Count)
            {
                return string.Empty;
            }

            var position = this._positions[positionIndex];
            return position?.StringFromRowOptions(row, this._formatOptions) ?? string.Empty;
        }
    }

    /// <summary>
    /// List formatter position
    /// </summary>
    public class UPCRMListFormatterPosition
    {
        /// <summary>
        /// The _fields.
        /// </summary>
        private List<UPConfigFieldControlField> _fields;

        /// <summary>
        /// Gets the first field.
        /// </summary>
        /// <value>
        /// The first field.
        /// </value>
        public UPConfigFieldControlField FirstField
            => this._fields != null && this._fields.Count > 0 ? this._fields[0] : null;

        /// <summary>
        /// Gets or sets the list formatter.
        /// </summary>
        /// <value>
        /// The list formatter.
        /// </value>
        public UPCRMListFormatter ListFormatter { get; set; }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        public void AddField(UPConfigFieldControlField field)
        {
            if (this._fields == null)
            {
                this._fields = new List<UPConfigFieldControlField> { field };
            }
            else
            {
                this._fields.Add(field);
            }
        }

        /// <summary>
        /// Strings from data provider options.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromDataProviderOptions(
            UPCRMListFormatterFunctionDataProvider dataProvider,
            UPFormatOption options)
        {
            return this.StringFromRowDataProviderValueArrayOptions(null, dataProvider, null, options);
        }

        /// <summary>
        /// Strings from row data provider value array options.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="valueArray">
        /// The value array.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromRowDataProviderValueArrayOptions(
            UPCRMResultRow row,
            UPCRMListFormatterFunctionDataProvider dataProvider,
            List<string> valueArray,
            UPFormatOption options)
        {
            string result = null;
            var colSpanFieldCount = 0;
            UPConfigFieldControlField colSpanField = null;
            List<string> colSpanFieldValues = null;
            string combineString = null;
            foreach (var field in this._fields)
            {
                var rawColumnValue = string.Empty;
                var columnValue = string.Empty;
                if (dataProvider != null)
                {
                    if (!string.IsNullOrEmpty(field?.Function))
                    {
                        rawColumnValue = dataProvider.RawValueForFunctionName(field.Function);
                        if (!string.IsNullOrEmpty(rawColumnValue))
                        {
                            columnValue = field.ValueFromRawValueOptions(rawColumnValue, options);
                        }
                    }
                }
                else if (valueArray != null)
                {
                    columnValue = valueArray.Count > field.TabIndependentFieldIndex
                                      ? valueArray[field.TabIndependentFieldIndex]
                                      : string.Empty;

                    rawColumnValue = columnValue;
                }
                else
                {
                    rawColumnValue = row.RawValueAtIndex(field.TabIndependentFieldIndex);
                    columnValue = row.ValueAtIndex(field.TabIndependentFieldIndex);
                }

                bool emptyColumnValue = false;

                if (field.Field.FieldType == "F")
                {
                    if (field.Attributes.ExtendedOptionIsSetToFalse("supportsDecimals") &&
                        !string.IsNullOrWhiteSpace(rawColumnValue) &&
                        decimal.TryParse(rawColumnValue, out var value))
                    {
                        const string noDecimalPlacesFormat = "F0";
                        columnValue = value.ToString(noDecimalPlacesFormat);
                    }

                    if (string.IsNullOrEmpty(rawColumnValue) || field.Field.IsEmptyValue(rawColumnValue))
                    {
                        columnValue = !options.HasFlag(UPFormatOption.Show0Float)
                                          ? string.Empty
                                          : StringExtensions.FloatDisplayTextFromFloat(0);

                        emptyColumnValue = true;
                    }
                }
                else if (field.Field.IsNumericField)
                {
                    if (string.IsNullOrEmpty(rawColumnValue) || field.Field.IsEmptyValue(rawColumnValue))
                    {
                        columnValue = !options.HasFlag(UPFormatOption.Show0)
                                          ? string.Empty
                                          : StringExtensions.IntegerDisplayTextFromInteger(0);

                        emptyColumnValue = true;
                    }
                }
                else if (string.IsNullOrEmpty(rawColumnValue) || field.Field.IsEmptyValue(rawColumnValue))
                {
                    if (field.Field.FieldType == "B")
                    {
                        if (string.IsNullOrEmpty(columnValue) || columnValue.Equals(this.ListFormatter.DisplayNo))
                        {
                            columnValue = string.Empty;
                            emptyColumnValue = true;
                        }
                    }
                    else
                    {
                        if (!options.HasFlag(UPFormatOption.Show0) || columnValue != "0")
                        {
                            columnValue = string.Empty;
                        }

                        emptyColumnValue = true;
                    }
                }

                var currentCombineString = field.Attributes.CombineString;
                var range = 0;
                if (!field.Attributes.NoPlaceHoldersInCombineString && !field.Attributes.CombineWithIndices
                    && !string.IsNullOrEmpty(currentCombineString))
                {
                    range = currentCombineString.IndexOf("v", StringComparison.Ordinal);
                    if (range > 0 && !string.IsNullOrEmpty(columnValue))
                    {
                        columnValue = currentCombineString.Replace("v", columnValue);
                    }
                    else if (range == -1)
                    {
                        range = currentCombineString.IndexOf("n", StringComparison.Ordinal);
                        if (range > 0)
                        {
                            columnValue = emptyColumnValue ? string.Empty : currentCombineString.Replace("n", columnValue);
                        }
                    }
                }

                if (colSpanFieldCount > 0)
                {
                    colSpanFieldValues.Add(!string.IsNullOrEmpty(columnValue) ? columnValue : string.Empty);

                    if (--colSpanFieldCount == 0)
                    {
                        columnValue = colSpanField?.Attributes?.FormatValues(colSpanFieldValues);
                    }
                }
                else if (field.Attributes.FieldCount > 1)
                {
                    colSpanFieldCount = field.Attributes.FieldCount - 1;
                    colSpanField = field;
                    colSpanFieldValues = new List<string> { columnValue };
                }

                if (colSpanFieldCount == 0 && !string.IsNullOrEmpty(columnValue))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = columnValue;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(combineString))
                        {
                            result += $"{combineString}{columnValue}";
                        }
                        else if (range > 0)
                        {
                            result += columnValue;
                        }
                        else
                        {
                            result += $" {columnValue}";
                        }
                    }

                    combineString = range <= 0 ? currentCombineString : string.Empty;
                }
            }

            return options.HasFlag(UPFormatOption.DontRemoveLineBreak) ? result : result?.SingleLineString();
        }

        /// <summary>
        /// Strings from row options.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromRowOptions(UPCRMResultRow row, UPFormatOption options)
        {
            return this.StringFromRowDataProviderValueArrayOptions(row, null, null, options);
        }

        /// <summary>
        /// Strings from value array options.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string StringFromValueArrayOptions(List<string> values, UPFormatOption options)
        {
            return this.StringFromRowDataProviderValueArrayOptions(null, null, values, options);
        }
    }
}
