// <copyright file="InfoAreaConditionLeaf.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>

namespace Aurea.CRM.Core.CRM.Query
{
    using System.Collections.Generic;
    using System.Text;
    using Aurea.CRM.Core.DAL;

    public partial class Constants
    {
        /// <summary>
        /// AND operator
        /// </summary>
        public const string AndOperation = "AND";

        /// <summary>
        /// Equality sign
        /// </summary>
        public const string EqualOperation = "=";

        /// <summary>
        /// InEquality sign
        /// </summary>
        public const string InequalOperation = "<>";

        /// <summary>
        /// OR operator
        /// </summary>
        public const string OrOperation = "OR";

        /// <summary>
        /// Question mark sign
        /// </summary>
        public const string QuestionMarkLiteral = "?";

        /// <summary>
        /// Char K
        /// </summary>
        public const char FieldInfoFieldTypeK = 'K';
    }

    /// <summary>
    /// Info area condition leaf
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Query.UPInfoAreaCondition" />
    public class UPInfoAreaConditionLeaf : UPInfoAreaCondition
    {
        private readonly string _infoAreaId;
        private readonly List<object> _fieldValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaConditionLeaf"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="fieldValue">The field value.</param>
        public UPInfoAreaConditionLeaf(string infoAreaId, int fieldId, string fieldValue)
            : this(infoAreaId, fieldId, Constants.EqualOperation, fieldValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaConditionLeaf"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="compareOperator">The compare operator.</param>
        /// <param name="fieldValue">The field value.</param>
        public UPInfoAreaConditionLeaf(string infoAreaId, int fieldId, string compareOperator, string fieldValue)
        {
            this.FieldIndex = fieldId;
            this.FirstValue = fieldValue;
            this.CompareOperator = compareOperator;
            this._infoAreaId = infoAreaId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPInfoAreaConditionLeaf"/> class.
        /// </summary>
        /// <param name="infoAreaId">The information area identifier.</param>
        /// <param name="fieldId">The field identifier.</param>
        /// <param name="compareOperator">The compare operator.</param>
        /// <param name="fieldValues">The field values.</param>
        public UPInfoAreaConditionLeaf(string infoAreaId, int fieldId, string compareOperator, List<object> fieldValues)
        {
            this.FieldIndex = fieldId;
            this.CompareOperator = compareOperator;
            this._infoAreaId = infoAreaId;

            if (fieldValues != null)
            {
                if (fieldValues.Count == 1)
                {
                    this.FirstValue = (string)fieldValues[0];
                }
                else
                {
                    this._fieldValues = fieldValues;
                }
            }
        }

        /// <summary>
        /// Creates the query condition options.
        /// </summary>
        /// <param name="crmDatabase">The CRM database.</param>
        /// <param name="options">The options.</param>
        /// <returns><see cref="TreeItemCondition"/></returns>
        public override TreeItemCondition CreateQueryConditionOptions(CRMDatabase crmDatabase, int options)
        {
            var fieldInfo = (FieldInfo)null;
            var multiFieldArray = PopulateMultiFieldArray(crmDatabase, fieldInfo);
            var multiFieldCount = multiFieldArray?.Length;
            var fieldValueArray = SetFieldValueArray(options, out var emptyFieldValue);

            var rootCondition = (TreeItemConditionRelation)null;
            var compare = string.Empty;
            if (multiFieldArray != null ||
                fieldValueArray?.Count > 1)
            {
                if (fieldInfo != null &&
                    fieldInfo.FieldType == Constants.FieldInfoFieldTypeK &&
                    fieldValueArray?.Count == 1 &&
                    fieldValueArray[0].Equals("0"))
                {
                    emptyFieldValue = true;
                }

                if (CompareOperator == Constants.InequalOperation)
                {
                    rootCondition = new TreeItemConditionRelation(emptyFieldValue ? Constants.OrOperation : Constants.AndOperation);
                    compare = Constants.InequalOperation;
                }
                else
                {
                    rootCondition = new TreeItemConditionRelation(emptyFieldValue ? Constants.AndOperation : Constants.OrOperation);
                    compare = Constants.EqualOperation;
                }
            }
            else
            {
                compare = !string.IsNullOrWhiteSpace(CompareOperator) ? CompareOperator : Constants.EqualOperation;

                return emptyFieldValue
                    ? new TreeItemConditionFieldValue((FieldIdType)FieldIndex, string.Empty, compare)
                    : new TreeItemConditionFieldValue((FieldIdType)FieldIndex, fieldValueArray[0].ToString(), compare);
            }

            AddTreeItemSubConditions(multiFieldArray, rootCondition, fieldValueArray, emptyFieldValue, compare);

            return rootCondition;
        }

        /// <summary>
        /// Conditions to object.
        /// </summary>
        /// <returns></returns>
        public override object ConditionToObject()
        {
            return this._fieldValues != null ?
                new List<object> { "LEAF", this.FieldIndex, this.CompareOperator, this._fieldValues } :
                new List<object> { "LEAF", this.FieldIndex, this.CompareOperator, new List<object> { this.FirstValue } };
        }

        /// <summary>
        /// Dates the time condition for information area parent.
        /// </summary>
        /// <param name="infoArea">The information area.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public override UPInfoAreaDateTimeCondition DateTimeConditionForInfoAreaParent(UPContainerInfoAreaMetaInfo infoArea, UPInfoAreaConditionTree parent)
        {
            if (!this.IsSingle || this.FirstValue?.Length < 2)
            {
                return null;
            }

            var fieldInfo = UPCRMDataStore.DefaultStore.FieldInfoForInfoAreaFieldId(this._infoAreaId, this.FieldIndex);
            if (fieldInfo.IsDateField)
            {
                var timeFieldId = fieldInfo.TimeFieldId;
                if (timeFieldId >= 0)
                {
                    return new UPInfoAreaSingleDateTimeCondition(infoArea, this.FieldIndex, timeFieldId, this, parent);
                }
            }
            else if (fieldInfo.IsTimeField)
            {
                var dateFieldId = fieldInfo.DateFieldId;
                if (dateFieldId >= 0)
                {
                    return new UPInfoAreaSingleDateTimeCondition(infoArea, dateFieldId, this.FieldIndex, this, parent);
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether this instance is single.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSingle => this._fieldValues?.Count == 1;

        /// <summary>
        /// Gets the first value.
        /// </summary>
        /// <value>
        /// The first value.
        /// </value>
        public string FirstValue { get; private set; }

        /// <summary>
        /// Replaces the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ReplaceValue(string value)
        {
            this.FirstValue = value;
        }

        /// <summary>
        /// Gets the index of the field.
        /// </summary>
        /// <value>
        /// The index of the field.
        /// </value>
        public int FieldIndex { get; }

        /// <summary>
        /// Gets the compare operator.
        /// </summary>
        /// <value>
        /// The compare operator.
        /// </value>
        public string CompareOperator { get; }

        /// <summary>
        /// Method Populates Multi Field Array
        /// </summary>
        /// <param name="crmDatabase">
        /// Database object
        /// </param>
        /// <param name="fieldInfo">
        /// FieldInfo object</param>
        /// <returns>
        /// MultiField Array of integers
        /// </returns>
        private int[] PopulateMultiFieldArray(CRMDatabase crmDatabase, FieldInfo fieldInfo)
        {
            var multiFieldCount = 0;
            var multiFieldArray = (int[])null;
            if (crmDatabase != null)
            {
                var tableInfo = crmDatabase.DataModel.InternalGetTableInfo(_infoAreaId);
                fieldInfo = tableInfo?.GetFieldInfo(FieldIndex);
                if (fieldInfo != null && fieldInfo.ArrayFieldCount > 1)
                {
                    var count = fieldInfo.ArrayFieldCount;
                    multiFieldArray = new int[count];
                    for (var i = 0; i < count; i++)
                    {
                        multiFieldArray[multiFieldCount] = fieldInfo.ArrayFieldIndices[i];
                        if (tableInfo.GetFieldInfoByIndex(multiFieldArray[multiFieldCount]) != null)
                        {
                            ++multiFieldCount;
                        }
                    }

                    if (multiFieldCount < 2)
                    {
                        multiFieldArray = null;
                        multiFieldCount = 0;
                    }
                }
            }

            return multiFieldArray;
        }

        /// <summary>
        /// Method return list of field values
        /// </summary>
        /// <param name="options">
        /// option value
        /// </param>
        /// <param name="emptyFieldValue">
        /// Is Field Value empty
        /// </param>
        /// <returns>
        /// List of Field Value objects
        /// </returns>
        private List<object> SetFieldValueArray(int options, out bool emptyFieldValue)
        {
            var fieldValueArray = (List<object>) null;
            emptyFieldValue = false;
            if (_fieldValues == null)
            {
                if (string.IsNullOrWhiteSpace(FirstValue))
                {
                    emptyFieldValue = true;
                }
                else
                {
                    fieldValueArray = new List<object> { FirstValue };
                }
            }
            else
            {
                fieldValueArray = _fieldValues;
                if (_fieldValues.Count == 0 || (_fieldValues.Count == 1 && string.IsNullOrWhiteSpace(_fieldValues[0] as string)))
                {
                    emptyFieldValue = true;
                }
            }

            if (options == 1 && fieldValueArray != null)
            {
                var convertedFieldValueArray = new List<object>();
                foreach (string s in fieldValueArray)
                {
                    var convertedString = new StringBuilder();
                    var lastFound = -1;
                    var len = s.Length;
                    for (var i = 0; i < len; i++)
                    {
                        var c = s[i];
                        if (c < 32 || c > 127)
                        {
                            if (lastFound < 0)
                            {
                                if (i > 0)
                                {
                                    convertedString.Append(s.Substring(0, i))
                                        .Append(Constants.QuestionMarkLiteral);
                                }
                                else
                                {
                                    convertedString.Append(Constants.QuestionMarkLiteral);
                                }

                                lastFound = i;
                            }
                            else
                            {
                                var location = lastFound + 1;
                                var length = i - lastFound - 1;
                                if (length > 0)
                                {
                                    convertedString.Append(s.Substring(location, length));
                                }

                                convertedString.Append(Constants.QuestionMarkLiteral);
                                lastFound = i;
                            }
                        }
                    }

                    if (lastFound < 0)
                    {
                        convertedFieldValueArray.Add(s);
                    }
                    else
                    {
                        if (lastFound < len - 1)
                        {
                            convertedString.Append(s.Substring(lastFound + 1));
                        }

                        convertedFieldValueArray.Add(convertedString.ToString());
                    }
                }

                fieldValueArray = convertedFieldValueArray;
            }

            return fieldValueArray;
        }

        /// <summary>
        /// Method Adds Sub conditions to <see cref="TreeItemConditionRelation"/> root object.
        /// </summary>
        /// <param name="multiFieldArray">
        /// Array of multi Field
        /// </param>
        /// <param name="rootCondition">
        /// <see cref="TreeItemConditionRelation"/> root object.
        /// </param>
        /// <param name="fieldValueArray">
        /// Array of field Values
        /// </param>
        /// <param name="emptyFieldValue">
        /// is FieldValue empty
        /// </param>
        /// <param name="compare">
        /// comparison string
        /// </param>
        private void AddTreeItemSubConditions(
            int[] multiFieldArray,
            TreeItemConditionRelation rootCondition,
            List<object> fieldValueArray,
            bool emptyFieldValue,
            string compare)
        {
            if (multiFieldArray != null)
            {
                foreach (var fieldIndex in multiFieldArray)
                {
                    if (emptyFieldValue)
                    {
                        rootCondition.AddSubCondition(new TreeItemConditionFieldValue((FieldIdType)fieldIndex, string.Empty, compare));
                    }
                    else
                    {
                        foreach (string fieldValue in fieldValueArray)
                        {
                            rootCondition.AddSubCondition(new TreeItemConditionFieldValue((FieldIdType)fieldIndex, fieldValue, compare));
                        }
                    }
                }
            }
            else
            {
                foreach (string fieldValue in fieldValueArray)
                {
                    rootCondition.AddSubCondition(new TreeItemConditionFieldValue((FieldIdType)FieldIndex, fieldValue, compare));
                }
            }
        }
    }
}
