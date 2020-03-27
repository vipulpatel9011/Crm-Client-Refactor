// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigExpand.cs" company="Aurea Software Gmbh">
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
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Expands configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigExpand : ConfigUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpand"/> class.
        /// </summary>
        /// <param name="defarray">The defarray.</param>
        public UPConfigExpand(List<object> defarray)
        {
            if (defarray == null || defarray.Count < 6)
            {
                return;
            }

            this.UnitName = (string)defarray[0];
            this.InfoAreaId = (string)defarray[1];
            this.FieldGroupName = (string)defarray[2];
            this.HeaderGroupName = (string)defarray[3];
            this.MenuLabel = (string)defarray[4];
            this.TableCaptionName = (string)defarray[5];
            if (defarray.Count > 6 && defarray[6] != null)
            {
                var conditionArray = (defarray[6] as JArray)?.ToObject<List<object>>();
                List<object> currentConditions = null;
                List<UPConfigExpandAlternate> alternates = null;
                foreach (JArray conditionJDef in conditionArray)
                {
                    var conditionDef = conditionJDef?.ToObject<List<object>>();
                    if (conditionDef == null)
                    {
                        continue;
                    }

                    var alternateName = (string)conditionDef[4];
                    if (alternateName == ".")
                    {
                        if (currentConditions == null)
                        {
                            currentConditions = new List<object> { conditionDef };
                        }
                        else
                        {
                            currentConditions.Add(conditionDef);
                        }
                    }
                    else
                    {
                        UPConfigExpandAlternate alternateExpand = null;
                        if (currentConditions != null)
                        {
                            currentConditions.Add(conditionDef);
                            alternateExpand = new UPConfigExpandAlternate(alternateName, currentConditions, this);
                            currentConditions = null;
                        }
                        else
                        {
                            alternateExpand = new UPConfigExpandAlternate(alternateName,
                                new List<object> { conditionDef }, this);
                        }

                        if (alternates == null)
                        {
                            alternates = new List<UPConfigExpandAlternate> { alternateExpand };
                        }
                        else
                        {
                            alternates.Add(alternateExpand);
                        }
                    }
                }

                this.AlternateExpands = alternates;
            }

            if (defarray.Count > 8)
            {
                this.ColorKey = defarray[7] as string;
                this.ImageName = defarray[8] as string;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpand"/> class.
        /// </summary>
        /// <param name="expand">The expand.</param>
        /// <param name="crmQuery">The CRM query.</param>
        public UPConfigExpand(UPConfigExpand expand, UPContainerMetaInfo crmQuery)
        {
            this.InfoAreaId = expand.InfoAreaId;
            this.FieldGroupName = expand.FieldGroupName;
            this.HeaderGroupName = expand.HeaderGroupName;
            this.MenuLabel = expand.MenuLabel;
            this.TableCaptionName = expand.TableCaptionName;
            this.ColorKey = expand.ColorKey;
            this.ImageName = expand.ImageName;

            this.AlternateExpands = expand.AlternateExpands?
                .Select(alternate => new UPConfigExpandAlternate(alternate, crmQuery, this, new List<string> { expand.UnitName }))
                .ToList();
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the name of the field group.
        /// </summary>
        /// <value>
        /// The name of the field group.
        /// </value>
        public string FieldGroupName { get; private set; }

        /// <summary>
        /// Gets the name of the header group.
        /// </summary>
        /// <value>
        /// The name of the header group.
        /// </value>
        public string HeaderGroupName { get; private set; }

        /// <summary>
        /// Gets the menu label.
        /// </summary>
        /// <value>
        /// The menu label.
        /// </value>
        public string MenuLabel { get; private set; }

        /// <summary>
        /// Gets the name of the table caption.
        /// </summary>
        /// <value>
        /// The name of the table caption.
        /// </value>
        public string TableCaptionName { get; private set; }

        /// <summary>
        /// Gets the alternate expands.
        /// </summary>
        /// <value>
        /// The alternate expands.
        /// </value>
        public List<UPConfigExpandAlternate> AlternateExpands { get; private set; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the color key.
        /// </summary>
        /// <value>
        /// The color key.
        /// </value>
        public string ColorKey { get; private set; }

        /// <summary>
        /// Fieldses for alternate expands except.
        /// </summary>
        /// <param name="visited">The visited.</param>
        /// <returns>fields lookup</returns>
        public Dictionary<string, UPCRMField> FieldsForAlternateExpandsExcept(List<object> visited)
        {
            Dictionary<string, UPCRMField> fieldDictionary = null;

            if (this.AlternateExpands != null)
            {
                foreach (UPConfigExpandAlternate alternate in this.AlternateExpands)
                {
                    var neededFields = alternate.NeededFields();
                    if (fieldDictionary == null)
                    {
                        fieldDictionary = new Dictionary<string, UPCRMField>().Append(neededFields);
                    }
                    else
                    {
                        foreach (var key in neededFields.Keys)
                        {
                            if (fieldDictionary.ValueOrDefault(key) == null)
                            {
                                fieldDictionary.SetObjectForKey(neededFields.ValueOrDefault(key), key);
                            }
                        }
                    }
                }
            }

            if (visited != null)
            {
                visited.Add(this.UnitName);
                var configStore = ConfigurationUnitStore.DefaultStore;

                if (this.AlternateExpands != null)
                {
                    foreach (var alternate in this.AlternateExpands)
                    {
                        if (visited.Contains(alternate.AlternateName))
                        {
                            continue;
                        }

                        var alternateExpand = configStore.ExpandByName(alternate.AlternateName);
                        var addFields = alternateExpand?.FieldsForAlternateExpandsExcept(visited);
                        if (addFields?.Count > 0)
                        {
                            foreach (var key in addFields.Keys)
                            {
                                fieldDictionary.SetObjectForKey(addFields.ValueOrDefault(key), key);
                            }
                        }
                    }
                }
            }

            return fieldDictionary;
        }

        /// <summary>
        /// Fieldses for alternate expands.
        /// </summary>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        /// <returns>fields lookup</returns>
        public Dictionary<string, UPCRMField> FieldsForAlternateExpands(bool deep)
        {
            List<object> visited = null;
            if (deep)
            {
                visited = new List<object>();
            }

            return this.FieldsForAlternateExpandsExcept(visited);
        }

        /// <summary>
        /// Expands the checker for CRM query.
        /// </summary>
        /// <param name="crmQuery">The CRM query.</param>
        /// <returns>expand configuration instance</returns>
        public UPConfigExpand ExpandCheckerForCrmQuery(UPContainerMetaInfo crmQuery)
        {
            return new UPConfigExpand(this, crmQuery);
        }

        /// <summary>
        /// Expands for result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>expand configuration instance</returns>
        public UPConfigExpand ExpandForResultRow(UPCRMResultRow row)
        {
            UPConfigExpand alternateExpand = null;
            if (this.AlternateExpands != null)
            {
                foreach (var alternate in this.AlternateExpands)
                {
                    alternateExpand = alternate.ResultForRow(row);
                    if (alternateExpand != null)
                    {
                        break;
                    }
                }
            }

            return alternateExpand ?? this;
        }
    }

    /// <summary>
    /// Defines expand condition
    /// </summary>
    public class UPConfigExpandCondition
    {
        private bool numericEmptyCheck;
        private bool boolEmptyCheck;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpandCondition"/> class.
        /// </summary>
        /// <param name="def">The definition.</param>
        /// <param name="alternate">The alternate.</param>
        public UPConfigExpandCondition(List<object> def, UPConfigExpandAlternate alternate)
        {
            this.FieldId = def[2].ToInt();
            this.Compare = UPCRMField.ConditionOperatorFromString(def[1] as string);
            this.FieldValue = def[3] as string;
            this.Alternate = alternate;
            this.InitializeDependentFields();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpandCondition"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="alternate">The alternate.</param>
        public UPConfigExpandCondition(UPConfigExpandCondition condition, UPConfigExpandAlternate alternate)
        {
            this.FieldId = condition.FieldId;
            this.Compare = condition.Compare;
            this.FieldValue = condition.FieldValue;
            this.Alternate = alternate;
            this.InitializeDependentFields();
        }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public int FieldId { get; private set; }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        public string FieldValue { get; protected set; }

        /// <summary>
        /// Gets the compare.
        /// </summary>
        /// <value>
        /// The compare.
        /// </value>
        public UPConditionOperator Compare { get; private set; }

        /// <summary>
        /// Gets the alternate.
        /// </summary>
        /// <value>
        /// The alternate.
        /// </value>
        public UPConfigExpandAlternate Alternate { get; private set; }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public UPCRMField Field { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [empty field value].
        /// </summary>
        /// <value>
        /// <c>true</c> if [empty field value]; otherwise, <c>false</c>.
        /// </value>
        public bool EmptyFieldValue { get; private set; }

        /// <summary>
        /// Initializes the dependent fields.
        /// </summary>
        private void InitializeDependentFields()
        {
            this.Field = UPCRMField.FieldWithFieldIdInfoAreaId(this.FieldId, this.Alternate.ParentExpand.InfoAreaId);
            this.boolEmptyCheck = this.Field.FieldType == "B";
            this.numericEmptyCheck = this.Field.IsNumericField || this.Field.IsCatalogField;
            if (string.IsNullOrEmpty(this.FieldValue))
            {
                this.EmptyFieldValue = true;
            }
            else if (this.numericEmptyCheck && this.FieldValue == "0")
            {
                this.EmptyFieldValue = true;
            }
            else if (this.boolEmptyCheck && this.FieldValue == "false")
            {
                this.EmptyFieldValue = true;
            }
        }

        /// <summary>
        /// Likes the result for value pattern.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>true if value and field value are related; else false</returns>
        public bool LikeResultForValuePattern(string value, string fieldValue)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(fieldValue))
            {
                return false;
            }

            var pattern = fieldValue;
            var patternLength = pattern.Length;
            if (fieldValue.StartsWith("*"))
            {
                pattern = fieldValue.Substring(patternLength - 1);

                return value.Length >= patternLength - 1 &&
                       string.Compare(value.Substring(patternLength - 1), pattern, StringComparison.Ordinal) == 0;
            }

            return string.Compare(value, fieldValue, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Results for value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>the result value</returns>
        public bool ResultForValue(string value)
        {
            if (string.IsNullOrEmpty(this.FieldValue))
            {
                return false;
            }

            if (this.FieldValue.StartsWith("$cur") || this.FieldValue.StartsWith("$par"))
            {
                var replaceValues = UPConditionValueReplacement.SessionParameterReplacements().ValueOrDefault(this.FieldValue);
                if (replaceValues != null && replaceValues.Count > 0)
                {
                    return this.ResultForValuePattern(value, replaceValues[0]);
                }

                if (!this.FieldValue.StartsWith("$cur"))
                {
                    return this.ResultForValuePattern(value, this.FieldValue);
                }

                var replacedDate = this.FieldValue.ReplaceDateVariables();
                if (replacedDate != this.FieldValue)
                {
                    return this.ResultForValuePattern(value, replacedDate);
                }
            }
            else if (this.FieldValue.StartsWith("$"))
            {
                var replacedDate = this.FieldValue.ReplaceDateVariables();
                if (!replacedDate.StartsWith("$"))
                {
                    return this.ResultForValuePattern(value, replacedDate);
                }
            }

            return this.ResultForValuePattern(value, this.FieldValue);
        }

        /// <summary>
        /// Results for value pattern.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns>the result of the value pattern</returns>
        public bool ResultForValuePattern(string value, string fieldValue)
        {
            return UPCRMField.ResultForValue(value, this.Compare, fieldValue, null, this.numericEmptyCheck, this.boolEmptyCheck, this.EmptyFieldValue);
        }

        /// <summary>
        /// Results for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>the result for a row</returns>
        public virtual bool ResultForRow(UPCRMResultRow row)
        {
            var crmQuery = row?.Result?.MetaInfo;
            var resultPosition = crmQuery?.PositionForField(this.Field) ?? -1;
            return resultPosition >= 0 && this.ResultForValue(row?.RawValueAtIndex(resultPosition));
        }
    }

    /// <summary>
    /// expand condition for a query
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.UPConfigExpandCondition" />
    public class UPConfigExpandConditionForQuery : UPConfigExpandCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpandConditionForQuery"/> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="alternate">The alternate.</param>
        public UPConfigExpandConditionForQuery(UPConfigExpandCondition condition, UPContainerMetaInfo crmQuery, UPConfigExpandAlternate alternate)
            : base(condition, alternate)
        {
            this.ResultPosition = crmQuery?.PositionForField(this.Field) ?? 0;
            if (!this.FieldValue.StartsWith("$cur") && !this.FieldValue.StartsWith("$par"))
            {
                return;
            }

            var dateFieldValue = this.FieldValue.ReplaceDateVariables();
            if (dateFieldValue.StartsWith("$"))
            {
                var replaceValues = UPConditionValueReplacement.SessionParameterReplacements().ValueOrDefault(this.FieldValue);
                if (replaceValues != null && replaceValues.Count > 0)
                {
                    this.FieldValue = replaceValues[0];
                }
            }
            else
            {
                this.FieldValue = dateFieldValue;
            }
        }

        /// <summary>
        /// Gets the result position.
        /// </summary>
        /// <value>
        /// The result position.
        /// </value>
        public int ResultPosition { get; private set; }

        /// <summary>
        /// Results for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>
        /// the result for a row
        /// </returns>
        public override bool ResultForRow(UPCRMResultRow row)
        {
            return this.ResultPosition >= 0 && this.ResultForValuePattern(row?.RawValueAtIndex(this.ResultPosition), this.FieldValue);
        }
    }

    /// <summary>
    /// Alternate Expand config
    /// </summary>
    public class UPConfigExpandAlternate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpandAlternate"/> class.
        /// </summary>
        /// <param name="alternateName">Name of the alternate.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="parentExpand">The parent expand.</param>
        public UPConfigExpandAlternate(string alternateName, List<object> conditions, UPConfigExpand parentExpand)
        {
            this.ParentExpand = parentExpand;
            this.AlternateName = alternateName;
            if (conditions == null)
            {
                return;
            }

            var conditionArray = new List<UPConfigExpandCondition>(conditions.Count);
            conditionArray.AddRange(from List<object> conditionDef in conditions select new UPConfigExpandCondition(conditionDef, this));

            this.Conditions = conditionArray;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigExpandAlternate"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="crmQuery">The CRM query.</param>
        /// <param name="parentExpand">The parent expand.</param>
        /// <param name="parentList">The parent list.</param>
        public UPConfigExpandAlternate(UPConfigExpandAlternate source, UPContainerMetaInfo crmQuery, UPConfigExpand parentExpand, List<string> parentList)
        {
            this.ParentExpand = parentExpand;
            this.AlternateName = source.AlternateName;
            var conditionArray = new List<UPConfigExpandCondition>(source.Conditions.Count);
            foreach (var condition in source.Conditions)
            {
                conditionArray.Add(new UPConfigExpandConditionForQuery(condition, crmQuery, this));
            }

            this.Conditions = conditionArray;
            this.CurrentExpand = ConfigurationUnitStore.DefaultStore.ExpandByName(this.AlternateName);
            if (this.CurrentExpand != null && this.CurrentExpand.AlternateExpands?.Count > 0)
            {
                var subSet = new List<string>(parentList) { this.AlternateName };
                List<UPConfigExpandAlternate> _subAlternates = null;
                foreach (var subSource in this.CurrentExpand.AlternateExpands)
                {
                    if (subSet.Contains(subSource.AlternateName))
                    {
                        continue;
                    }

                    var subAlternate = new UPConfigExpandAlternate(subSource, crmQuery, this.CurrentExpand, subSet);
                    if (_subAlternates == null)
                    {
                        _subAlternates = new List<UPConfigExpandAlternate> { subAlternate };
                    }
                    else
                    {
                        _subAlternates.Add(subAlternate);
                    }
                }

                this.AlternateExpands = _subAlternates;
            }
        }

        /// <summary>
        /// Gets the conditions.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public List<UPConfigExpandCondition> Conditions { get; private set; }

        /// <summary>
        /// Gets the name of the alternate.
        /// </summary>
        /// <value>
        /// The name of the alternate.
        /// </value>
        public string AlternateName { get; private set; }

        /// <summary>
        /// Gets the parent expand.
        /// </summary>
        /// <value>
        /// The parent expand.
        /// </value>
        public UPConfigExpand ParentExpand { get; private set; }

        /// <summary>
        /// Gets the current expand.
        /// </summary>
        /// <value>
        /// The current expand.
        /// </value>
        public UPConfigExpand CurrentExpand { get; private set; }

        /// <summary>
        /// Gets the alternate expands.
        /// </summary>
        /// <value>
        /// The alternate expands.
        /// </value>
        public List<UPConfigExpandAlternate> AlternateExpands { get; private set; }

        /// <summary>
        /// Neededs the fields.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UPCRMField> NeededFields()
        {
            var addedFields = new Dictionary<string, UPCRMField>();
            foreach (var condition in this.Conditions)
            {
                var field = condition.Field;
                var fieldIdentification = field.FieldIdentification;
                if (addedFields.ValueOrDefault(fieldIdentification) == null)
                {
                    addedFields.SetObjectForKey(field, fieldIdentification);
                }
            }

            return addedFields;
        }

        /// <summary>
        /// Results for row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public UPConfigExpand ResultForRow(UPCRMResultRow row)
        {
            return this.ResultForRowIgnoreAlternates(row, new List<string>());
        }

        /// <summary>
        /// Results for row ignore alternates.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="ignoreAlternates">The ignore alternates.</param>
        /// <returns></returns>
        public UPConfigExpand ResultForRowIgnoreAlternates(UPCRMResultRow row, List<string> ignoreAlternates)
        {
            if (this.Conditions == null ||
                this.Conditions.Any(condition => !condition.ResultForRow(row)))
            {
                return null;
            }

            if (this.CurrentExpand == null)
            {
                var currentExpand = ConfigurationUnitStore.DefaultStore.ExpandByName(this.AlternateName);
                var _ignoreAlternates = new List<string>(ignoreAlternates) { this.AlternateName };

                if (currentExpand?.AlternateExpands != null)
                {
                    foreach (var alternate in currentExpand.AlternateExpands)
                    {
                        if (_ignoreAlternates.Contains(alternate.AlternateName))
                        {
                            continue;
                        }

                        var subExpand = alternate.ResultForRowIgnoreAlternates(row, _ignoreAlternates);
                        if (subExpand != null)
                        {
                            return subExpand;
                        }
                    }
                }

                return currentExpand;
            }

            if (this.AlternateExpands != null)
            {
                foreach (var alternate in this.AlternateExpands)
                {
                    var subExpand = alternate.ResultForRowIgnoreAlternates(row, ignoreAlternates);
                    if (subExpand != null)
                    {
                        return subExpand;
                    }
                }
            }

            return this.CurrentExpand;
        }
    }
}
