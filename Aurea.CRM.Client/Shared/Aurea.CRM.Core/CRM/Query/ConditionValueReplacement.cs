// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionValueReplacement.cs" company="Aurea Software Gmbh">
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
//   Conditional value replacement
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Query
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Conditional value replacement
    /// </summary>
    public class UPConditionValueReplacement
    {
        /// <summary>
        /// The replace dictionary.
        /// </summary>
        private readonly Dictionary<string, List<string>> replaceDictionary;

        /// <summary>
        /// The replace parameters.
        /// </summary>
        private readonly Dictionary<string, object> replaceParameters;

        /// <summary>
        /// The unnamed parameters.
        /// </summary>
        private readonly List<List<string>> unnamedParameters;

        /// <summary>
        /// The next unnamed parameter.
        /// </summary>
        private int nextUnnamedParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConditionValueReplacement"/> class.
        /// </summary>
        public UPConditionValueReplacement()
        {
            this.replaceDictionary = new Dictionary<string, List<string>>(ServerSession.CurrentSession.SessionParameterReplacements);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConditionValueReplacement"/> class.
        /// </summary>
        /// <param name="_parameters">
        /// The _parameters.
        /// </param>
        public UPConditionValueReplacement(UPConfigFilterParameters _parameters)
        {
            var unnamedSource = _parameters.UnnamedParameters;
            var count = unnamedSource?.Count ?? 0;
            if (count > 0)
            {
                this.nextUnnamedParameter = 0;
                this.unnamedParameters = new List<List<string>>(count);
                for (var i = 0; i < count; i++)
                {
                    this.unnamedParameters.Add(unnamedSource[i].Values);
                }
            }

            count = _parameters.NumberOfNamedParameters;
            if (count <= 0)
            {
                return;
            }

            this.replaceDictionary = new Dictionary<string, List<string>>(count);
            this.replaceParameters = new Dictionary<string, object>(count);
            foreach (var parameter in _parameters.namedParameters.Select(parameters => parameters.Value[0]))
            {
                this.replaceDictionary[parameter.ParameterName] = parameter.Values;
                this.replaceParameters[parameter.ParameterName] = parameter;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConditionValueReplacement"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public UPConditionValueReplacement(Dictionary<string, object> dictionary)
            : this(FilterDictionaryFromSourceDictionary(dictionary))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConditionValueReplacement"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        public UPConditionValueReplacement(Dictionary<string, List<string>> dictionary)
        {
            this.replaceDictionary = dictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConditionValueReplacement"/> class.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public UPConditionValueReplacement(UPConfigFilterParameter parameter)
        {
            this.replaceDictionary = new Dictionary<string, List<string>> { { parameter.ParameterName, parameter.Values } };
            this.replaceParameters = new Dictionary<string, object> { { parameter.ParameterName, parameter } };
        }

        /// <summary>
        /// Gets the default parameters.
        /// </summary>
        /// <value>
        /// The default parameters.
        /// </value>
        public static UPConditionValueReplacement DefaultParameters => new UPConditionValueReplacement();

        /// <summary>
        /// Filters the dictionary from source dictionary.
        /// </summary>
        /// <param name="sourceDictionary">
        /// The source dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, List<string>> FilterDictionaryFromSourceDictionary(Dictionary<string, object> sourceDictionary)
        {
            var dict = new Dictionary<string, List<string>>();
            if (sourceDictionary != null)
            {
                foreach (var key in sourceDictionary.Keys)
                {
                    dict[$"$cur{key}"] = new List<string> { sourceDictionary[key] as string };
                    dict[$"$par{key}"] = new List<string> { sourceDictionary[key] as string };
                }
            }

            return dict;
        }

        /// <summary>
        /// Replacementses from value parameter dictionary.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <returns>
        /// The <see cref="UPConditionValueReplacement"/>.
        /// </returns>
        public static UPConditionValueReplacement ReplacementsFromValueParameterDictionary(Dictionary<string, object> valueDictionary)
        {
            return new UPConditionValueReplacement(valueDictionary);
        }

        /// <summary>
        /// Replacementses from value parameter dictionary the default.
        /// </summary>
        /// <param name="valueDictionary">
        /// The value dictionary.
        /// </param>
        /// <param name="theDefaults">
        /// if set to <c>true</c> [the defaults].
        /// </param>
        /// <returns>
        /// The <see cref="UPConditionValueReplacement"/>.
        /// </returns>
        public static UPConditionValueReplacement ReplacementsFromValueParameterDictionary(Dictionary<string, object> valueDictionary, bool theDefaults)
        {
            if (!theDefaults)
            {
                return ReplacementsFromValueParameterDictionary(valueDictionary);
            }

            var dict = SessionParameterReplacements();
            if (valueDictionary?.Count > 0)
            {
                var rawDictionary = FilterDictionaryFromSourceDictionary(valueDictionary);
                foreach (var item in rawDictionary)
                {
                    dict[item.Key] = item.Value.ToList();
                }
            }

            return new UPConditionValueReplacement(dict);
        }

        /// <summary>
        /// Sessions the parameter replacements.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public static Dictionary<string, List<string>> SessionParameterReplacements()
        {
            return ServerSession.CurrentSession.SessionParameterReplacements;
        }

        /// <summary>
        /// Nexts the unnamed value.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<string> NextUnnamedValue()
        {
            return this.nextUnnamedParameter < this.unnamedParameters.Count
                       ? this.unnamedParameters[this.nextUnnamedParameter++]
                       : null;
        }

        /// <summary>
        /// Removes the unbound parameters.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool RemoveUnboundParameters()
        {
            var strList = this.replaceDictionary.ValueOrDefault("$parRemoveUnboundParameters");
            if (strList == null || strList.Count == 0)
            {
                return false;
            }

            var item = strList[0];

            int objInt;
            if (int.TryParse(item, out objInt))
            {
                return objInt != 0;
            }

            return item == "true";
        }

        /// <summary>
        /// Replaces the field value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> ReplaceFieldValue(string value)
        {
            value = value.ReplaceDateVariables();
            if (!value.StartsWith("$"))
            {
                return new List<string> { value };
            }

            if (!value.StartsWith("$cur") && !value.StartsWith("$par"))
            {
                return new List<string> { value };
            }

            List<string> returnArray = null;
            if (this.replaceDictionary != null)
            {
                var foundLen = 0;
                foreach (string key in this.replaceDictionary.Keys.Where(x => value.StartsWith(x)))
                {
                    var keyLen = key.Length;
                    if (keyLen < foundLen)
                    {
                        continue;
                    }

                    foundLen = keyLen;
                    if (value.Length > key.Length)
                    {
                        var separatorRange = value.IndexOf(":");
                        if (separatorRange > 0)
                        {
                            var parameterPart = value.Substring(0, separatorRange);
                            var replacement = this.replaceDictionary[parameterPart];
                            if (replacement?.Count == 1)
                            {
                                var replacementValue = replacement[0];
                                var dateParamPart = value.Substring(separatorRange + 1);
                                string replacedValue;
                                if (dateParamPart.StartsWith("Time") || dateParamPart.StartsWith("Hour"))
                                {
                                    var timedat = replacementValue.TimeFromCrmValue();
                                    replacedValue = value.ReplaceTimePostfixWithDateReplacePrefixLength(
                                        timedat,
                                        separatorRange + 1);
                                }
                                else
                                {
                                    var dat = replacementValue.DateFromCrmValue();
                                    replacedValue = value.ReplaceDatePostfixWithDateReplacePrefixLength(
                                        dat.Value,
                                        separatorRange + 1);
                                }

                                if (!value.Equals(replacedValue))
                                {
                                    returnArray = new List<string> { replacedValue };
                                    break;
                                }
                            }
                        }
                    }

                    var values = this.replaceDictionary.ValueOrDefault(key);

                    var count = values?.Count ?? 0;
                    if (count == 1)
                    {
                        returnArray = new List<string> { $"{values[0]}{value.Substring(key.Length)}" };
                    }
                    else if (count > 0)
                    {
                        var postfix = value.Substring(key.Length);
                        var replacedValues = new List<string>(count);
                        for (var i = 0; i < count; i++)
                        {
                            replacedValues.Add($"{values[i]}{postfix}");
                        }

                        returnArray = replacedValues;
                    }
                }
            }

            return returnArray ?? new List<string> { value };
        }

        /// <summary>
        /// Replaces the field values.
        /// </summary>
        /// <param name="fieldValues">
        /// The field values.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<object> ReplaceFieldValues(List<object> fieldValues)
        {
            if (fieldValues == null)
            {
                return null;
            }

            List<object> replaceArray = null;
            int count = fieldValues.Count;
            var onlyFirst = false;
            for (int i = 0; i < count; i++)
            {
                var value = (string)fieldValues[i];
                if (value.StartsWith("$"))
                {
                    if (value == "$onlyFirst")
                    {
                        if (replaceArray == null)
                        {
                            replaceArray = new List<object>();
                            if (i > 0)
                            {
                                for (int j = 0; j < i; j++)
                                {
                                    replaceArray.Add(fieldValues[j]);
                                }
                            }
                        }

                        onlyFirst = true;
                        continue;
                    }

                    List<string> newValues = null;
                    if (this.unnamedParameters != null && value == "$parValue")
                    {
                        newValues = this.NextUnnamedValue();
                    }
                    else if (value.StartsWith("$"))
                    {
                        newValues = this.ReplaceFieldValue(value);
                        if (onlyFirst && newValues.Count == 1)
                        {
                            var newValue = newValues[0];
                            if (value == newValue || string.IsNullOrEmpty(newValue))
                            {
                                continue;
                            }
                        }
                    }

                    if (replaceArray != null || newValues?.Count != 1 || newValues[0] != value)
                    {
                        if (replaceArray == null)
                        {
                            replaceArray = new List<object>(fieldValues.Count);
                            if (i > 0)
                            {
                                var filterParameter = (UPConfigFilterParameter)this.replaceParameters.ValueOrDefault(value);
                                if (filterParameter != null && !filterParameter.RemoveFirstValuesFromValueSet)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        replaceArray.Add(fieldValues[j]);
                                    }
                                }
                            }
                        }

                        replaceArray.AddRange(newValues);
                    }
                }

                if (onlyFirst)
                {
                    break;
                }
            }

            return replaceArray != null ? (replaceArray.Count > 0 ? replaceArray : null) : fieldValues;
        }
    }
}
