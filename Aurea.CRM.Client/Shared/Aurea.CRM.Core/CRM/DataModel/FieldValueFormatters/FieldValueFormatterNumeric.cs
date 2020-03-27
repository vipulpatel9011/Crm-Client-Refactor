// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldValueFormatterNumeric.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//   Jakub Malczak
// </author>
// <summary>
//   Field value formatter numeric
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel.FieldValueFormatters
{
    using System.Globalization;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.DAL;

    /// <summary>
    /// Field value formatter numeric
    /// </summary>
    public class FieldValueFormatterNumeric
    {
        /// <summary>
        /// Convert method
        /// </summary>
        /// <param name="rawValue">value</param>
        /// <param name="fieldType">field type</param>
        /// <param name="options">options</param>
        /// <param name="configField">config field</param>
        /// <param name="field">field</param>
        /// <param name="fieldInfo">field info</param>
        /// <returns>string</returns>
        public static string Convert(
            string rawValue,
            char fieldType,
            UPFormatOption options,
            UPConfigFieldControlField configField,
            UPCRMFieldInfo field,
            FieldInfo fieldInfo)
        {
            var convertedValue = string.Empty;
            // CLIENT-207 ( Value coming from database always in en culture so first convert to decimal with en)
            if (decimal.TryParse(rawValue,NumberStyles.Number,new CultureInfo("en"), out var numericValue))
            {
                if (IsPercentageField(field, configField))
                {
                    numericValue *= 100;
                }

                switch (fieldType)
                {
                    case 'L':
                    case 'S':
                        {
                            if (!string.IsNullOrWhiteSpace(field.RepType))
                            {
                                int intValue;
                                var repName = UPCRMDataStore.DefaultStore.Reps.NameOfRepIdString(rawValue);
                                if (!string.IsNullOrWhiteSpace(repName))
                                {
                                    return repName;
                                }
                                else if (int.TryParse(rawValue, out intValue) && intValue == 0)
                                {
                                    return string.Empty;
                                }
                                else
                                {
                                    return rawValue;
                                }
                            }

                            if (string.IsNullOrWhiteSpace(rawValue) && (options == UPFormatOption.Show0))
                            {
                                convertedValue = "0";
                            }
                            else if (!field.ShowZero && field.IsEmptyValue(rawValue))
                            {
                                convertedValue = string.Empty;
                            }
                            else
                            {
                                convertedValue = numericValue.ToString("N0");
                            }

                            break;
                        }

                    case 'F':
                        {
                            if (!field.ShowZero && numericValue == 0)
                            {
                                convertedValue = string.Empty;
                            }
                            else if (field.OneDecimalDigit)
                            {
                                convertedValue = $"{numericValue:N1}";
                            }
                            else if (field.NoDecimalDigits)
                            {
                                convertedValue = $"{numericValue:N0}";
                            }
                            else if (field.ThreeDecimalDigits)
                            {
                                convertedValue = $"{numericValue:N3}";
                            }
                            else if (field.FourDecimalDigits)
                            {
                                convertedValue = $"{numericValue:N4}";
                            }
                            else if (int.TryParse(configField?.Attributes.RenderHooksForKey("DecimalDigits"), out var digits))
                            {
                                convertedValue = numericValue.ToString($"N{digits}");
                            }
                            else
                            {
                                convertedValue = $"{numericValue:N}";
                            }

                            if (options != UPFormatOption.Report && fieldInfo.IsAmount)
                            {
                                convertedValue = $"{numericValue:N},-";
                            }

                            break;
                        }
                }

                if (configField?.Attributes.RenderHooksForKey("GroupingSeparator") == false.ToString().ToLower())
                {
                    convertedValue = convertedValue.Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator, string.Empty);
                }

                if (IsPercentageField(field, configField) && !string.IsNullOrWhiteSpace(convertedValue))
                {
                    if (options != UPFormatOption.Report)
                    {
                        convertedValue = $"{convertedValue}%";
                    }
                }
            }

            return convertedValue;
        }

        private static bool IsPercentageField(UPCRMFieldInfo field, UPConfigFieldControlField configField)
        {
            return field.PercentField || configField?.Attributes.RenderHooksForKey(nameof(field.PercentField)) ==
                   true.ToString().ToLower();
        }
    }
}
