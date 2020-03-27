// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigTableCaption.cs" company="Aurea Software Gmbh">
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
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// table caption search handler
    /// </summary>
    /// <seealso cref="ISearchOperationHandler" />
    public class UPTableCaptionSearchDelegate : ISearchOperationHandler
    {
        /// <summary>
        /// The table caption
        /// </summary>
        private readonly UPConfigTableCaption tableCaption;

        /// <summary>
        /// The delegate
        /// </summary>
        private readonly IRemoteTableCaptionDelegate Delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPTableCaptionSearchDelegate" /> class.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPTableCaptionSearchDelegate(UPConfigTableCaption caption, IRemoteTableCaptionDelegate theDelegate)
        {
            this.tableCaption = caption;
            this.Delegate = theDelegate;
        }

        #region ISearchOperationHandler

        /// <summary>
        /// Ups the search operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.tableCaption.RemoveLocalDelegate(this);
            this.Delegate?.TableCaptionDidFailWithError(this.tableCaption, error);
        }

        /// <summary>
        /// Ups the search operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            string tableCaptionString;
            if (result?.RowCount < 1)
            {
                tableCaptionString = string.Empty;
            }
            else
            {
                var resultRow = result?.ResultRowAtIndex(0) as UPCRMResultRow;
                tableCaptionString = this.tableCaption.TableCaptionForOrderedResultRow(resultRow);
                if (!string.IsNullOrEmpty(tableCaptionString))
                {
                    this.tableCaption.AddTableCaptionToCache(tableCaptionString,
                        resultRow?.RootRecordIdentification);
                }
            }

            this.tableCaption.RemoveLocalDelegate(this);
            this.Delegate?.TableCaptionDidFinishWithResult(this.tableCaption, tableCaptionString);
        }

        /// <summary>
        /// Ups the search operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
        }

        /// <summary>
        /// Ups the search operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
        }

        /// <summary>
        /// Ups the search operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
        }

        #endregion
    }

    /// <summary>
    /// table caption configurations
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Configuration.ConfigUnit" />
    public class UPConfigTableCaption : ConfigUnit
    {
        private readonly bool hasFieldMap;
        private readonly List<int> fieldMap;
        private List<UPTableCaptionSearchDelegate> localDelegates;
        private Dictionary<string, string> recordCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPConfigTableCaption"/> class.
        /// </summary>
        /// <param name="defArray">The definition array.</param>
        public UPConfigTableCaption(List<object> defArray)
        {
            if (defArray == null || defArray.Count < 6)
            {
                return;
            }

            this.UnitName = defArray[0] as string;
            this.hasFieldMap = false;
            this.InfoAreaId = defArray[1] as string;
            this.PrefixString = defArray[2] as string;
            this.FormatString = defArray[3] as string;
            this.ImageName = defArray[4] as string;

            var fielddefarray = (defArray[5] as JArray)?.ToObject<List<object>>();
            if (fielddefarray != null)
            {
                var count = fielddefarray.Count;
                var arr = new List<UPCRMField>(count);
                var currentArrayIndex = 1;
                for (var i = 0; i < count; i++)
                {
                    var fieldObject = (fielddefarray[i] as JArray)?.ToObject<List<object>>();
                    if (fieldObject == null)
                    {
                        continue;
                    }

                    var nr = JObjectExtensions.ToInt(fieldObject[0]);
                    var fieldId = JObjectExtensions.ToInt(fieldObject[1]);
                    var fieldInfoAreaId = fieldObject[2] as string;
                    var linkId = JObjectExtensions.ToInt(fieldObject[3]);
                    if (this.hasFieldMap)
                    {
                        this.fieldMap.Add(nr);
                    }
                    else if (currentArrayIndex < nr)
                    {
                        this.fieldMap = new List<int>(count);
                        for (var j = 1; j < currentArrayIndex; j++) this.fieldMap.Add(j);

                        this.hasFieldMap = true;
                        this.fieldMap.Add(nr);
                    }
                    else
                    {
                        ++currentArrayIndex;
                    }

                    arr.Add(UPCRMField.FieldWithFieldIdInfoAreaIdLinkId(fieldId, fieldInfoAreaId, linkId));
                }

                this.Fields = arr;
            }

            var specialdefs = defArray.Count > 6 ? (defArray[6] as JArray)?.ToObject<List<object>>() : null;
            if (specialdefs == null)
            {
                return;
            }

            var defCount = specialdefs.Count;
            var ar = new List<UPConfigTableCaptionSpecialDefs>(defCount);
            for (var i = 0; i < defCount; i++)
            {
                ar.Add(new UPConfigTableCaptionSpecialDefs((specialdefs[i] as JArray)?.ToObject<List<object>>()));
            }

            this.SpecialDefArray = ar;
        }

        /// <summary>
        /// Gets the format string.
        /// </summary>
        /// <value>
        /// The format string.
        /// </value>
        public string FormatString { get; }

        /// <summary>
        /// Gets the prefix string.
        /// </summary>
        /// <value>
        /// The prefix string.
        /// </value>
        public string PrefixString { get; private set; }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public List<UPCRMField> Fields { get; }

        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName { get; private set; }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; }

        /// <summary>
        /// Gets the special definition array.
        /// </summary>
        /// <value>
        /// The special definition array.
        /// </value>
        public List<UPConfigTableCaptionSpecialDefs> SpecialDefArray { get; }

        /// <summary>
        /// Gets the CRM query.
        /// </summary>
        /// <value>
        /// The CRM query.
        /// </value>
        public UPContainerMetaInfo CrmQuery =>
            this.Fields != null ? new UPContainerMetaInfo(this.Fields, this.InfoAreaId) : null;

        /* make sure to pass the return value of this function to tableCaptionForResultRow! */
        public List<UPContainerFieldMetaInfo> AddTableCaptionFieldsToCrmQuery(UPContainerMetaInfo crmQuery)
            => crmQuery?.AddCrmFields(this.Fields);

        /// <summary>
        /// Tables the caption for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns>table caption</returns>
        public string TableCaptionForRecordIdentification(string recordIdentification)
        {
            var result = this.CrmQuery?.ReadRecord(recordIdentification);
            if (result != null && result.RowCount >= 1)
            {
                return this.TableCaptionForOrderedResultRow(result.ResultRowAtIndex(0) as UPCRMResultRow);
            }

            var cacheValue = this.recordCache?.ValueOrDefault(recordIdentification);
            return cacheValue;
        }

        /// <summary>
        /// Requests the table caption for record identification the delegate.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="theDelegate">The delegate.</param>
        public void RequestTableCaptionForRecordIdentification(string recordIdentification,
            IRemoteTableCaptionDelegate theDelegate)
        {
            var localDelegate = new UPTableCaptionSearchDelegate(this, theDelegate);
            this.CrmQuery?.ReadRecord(recordIdentification, localDelegate);
            if (this.localDelegates == null)
            {
                this.localDelegates = new List<UPTableCaptionSearchDelegate>();
            }

            this.localDelegates.Add(localDelegate);
        }

        /// <summary>
        /// Removes the local delegate.
        /// </summary>
        /// <param name="theDelegate">The delegate.</param>
        public void RemoveLocalDelegate(UPTableCaptionSearchDelegate theDelegate)
        {
            this.localDelegates?.Remove(theDelegate);
        }

        /// <summary>
        /// Tables the caption for ordered result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>table caption</returns>
        public string TableCaptionForOrderedResultRow(UPCRMResultRow row)
        {
            return this.TableCaptionForResultRow(row, null, true);
        }

        /// <summary>
        /// Tables the caption for result row result field map.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="resultFieldMap">The result field map.</param>
        /// <returns>table caption</returns>
        public string TableCaptionForResultRow(UPCRMResultRow row, List<UPContainerFieldMetaInfo> resultFieldMap)
        {
            return this.TableCaptionForResultRow(row, resultFieldMap, true);
        }

        /// <summary>
        /// Tables the caption for result row result field map trim.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="resultFieldMap">The result field map.</param>
        /// <param name="trim">if set to <c>true</c> [trim].</param>
        /// <returns>table caption</returns>
        public string TableCaptionForResultRow(UPCRMResultRow row, List<UPContainerFieldMetaInfo> resultFieldMap,
            bool trim)
        {
            int count;
            var fieldValues = new List<string>();
            var needsRawValues = this.FormatString.IndexOf("%r", StringComparison.Ordinal) > -1;
            var needsLabels = this.FormatString.IndexOf("{label:", StringComparison.Ordinal) > -1;
            if (this.SpecialDefArray != null)
            {
                count = this.SpecialDefArray.Count;
                for (var i = 0; !needsRawValues && i < count; i++)
                {
                    var specialDef = this.SpecialDefArray[i];
                    needsRawValues = specialDef?.FormatString?.IndexOf("%r", StringComparison.Ordinal) > -1;
                    needsLabels = specialDef?.FormatString?.IndexOf("{label:", StringComparison.Ordinal) > -1;
                }
            }

            count = this.Fields.Count;
            var rawFieldValues = needsRawValues ? new List<string>() : null;
            var fieldLabels = needsLabels ? new List<string>() : null;
            this.ProcessFields(rawFieldValues, fieldLabels, fieldValues, count, needsRawValues, needsLabels,
                resultFieldMap, row);

            return this.GetTableCaption(fieldValues, needsLabels, fieldLabels, needsRawValues, rawFieldValues, trim,
                count);
        }

        /// <summary>
        /// Results the field map from meta information.
        /// </summary>
        /// <param name="metaInfo">The meta information.</param>
        /// <returns>table caption</returns>
        public List<UPContainerFieldMetaInfo> ResultFieldMapFromMetaInfo(UPContainerMetaInfo metaInfo)
        {
            var resultFieldPosition = new List<UPContainerFieldMetaInfo>();
            var foundCount = 0;
            if (this.Fields != null)
            {
                foreach (var field in this.Fields)
                {
                    UPContainerFieldMetaInfo foundField = null;
                    if (metaInfo?.OutputFields != null)
                    {
                        foreach (var fieldMetaInfo in metaInfo.OutputFields)
                        {
                            if (!fieldMetaInfo.CrmField.IsEqualToField(field))
                            {
                                continue;
                            }

                            foundField = fieldMetaInfo;
                            break;
                        }
                    }

                    if (foundField != null)
                    {
                        ++foundCount;
                        resultFieldPosition.Add(foundField);
                    }
                    else
                    {
                        resultFieldPosition.Add(null);
                    }
                }
            }

            return foundCount > 0 ? resultFieldPosition : null;
        }

        /// <summary>
        /// Tables the caption for result row trim.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="trim">if set to <c>true</c> [trim].</param>
        /// <returns>table caption</returns>
        public string TableCaptionForResultRow(UPCRMResultRow row, bool trim = true)
        {
            var resultFieldMap = this.ResultFieldMapFromMetaInfo(row?.Result?.MetaInfo);
            return resultFieldMap != null ? this.TableCaptionForResultRow(row, resultFieldMap, trim) : null;
        }

        /// <summary>
        /// Adds the table caption to cache.
        /// </summary>
        /// <param name="tableCaptionString">The table caption string.</param>
        /// <param name="recordIdentification">The record identification.</param>
        public void AddTableCaptionToCache(string tableCaptionString, string recordIdentification)
        {
            if (this.recordCache == null)
            {
                this.recordCache = new Dictionary<string, string>();
            }

            this.recordCache[recordIdentification ?? string.Empty] = tableCaptionString;
        }

        /// <summary>
        /// Tables the caption for record identification default text.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns></returns>
        public static string TableCaptionForRecordIdentification(string recordIdentification, string defaultText)
        {
            var infoAreaId = recordIdentification.InfoAreaId();
            var configStore = ConfigurationUnitStore.DefaultStore;
            var defaultTableCaptionForInfoAreaId = configStore.TableCaptionByName(infoAreaId);
            if (defaultTableCaptionForInfoAreaId != null)
            {
                var tableCaption =
                    defaultTableCaptionForInfoAreaId.TableCaptionForRecordIdentification(recordIdentification);

                if (!string.IsNullOrEmpty(tableCaption))
                {
                    return tableCaption;
                }
            }

            var tableInfo = UPCRMDataStore.DefaultStore.TableInfoForInfoArea(infoAreaId);
            if (tableInfo == null)
            {
                return defaultText;
            }

            if (!infoAreaId.Equals(tableInfo.RootInfoAreaId()))
            {
                defaultTableCaptionForInfoAreaId = configStore.TableCaptionByName(tableInfo.RootInfoAreaId());
                var tableCaption =
                    defaultTableCaptionForInfoAreaId.TableCaptionForRecordIdentification(recordIdentification);

                if (!string.IsNullOrEmpty(tableCaption))
                {
                    return tableCaption;
                }
            }
            else if (tableInfo.HasVirtualInfoAreas)
            {
                var count = tableInfo.NumberOfVirtualInfoAreas;
                for (var i = 0; i < count; i++)
                {
                    defaultTableCaptionForInfoAreaId =
                        configStore.TableCaptionByName(tableInfo.VirtualInfoAreaAtIndex(i));
                    var tableCaption =
                        defaultTableCaptionForInfoAreaId.TableCaptionForRecordIdentification(recordIdentification);

                    if (!string.IsNullOrEmpty(tableCaption))
                    {
                        return tableCaption;
                    }
                }
            }

            return defaultText;
        }

        private void ProcessFields(
            ICollection<string> rawFieldValues,
            ICollection<string> fieldLabels,
            ICollection<string> fieldValues,
            int count,
            bool needsRawValues,
            bool needsLabels,
            IReadOnlyList<UPContainerFieldMetaInfo> resultFieldMap,
            UPCRMResultRow row)
        {
            var nextIndex = 1;
            for (var i = 0; i < count; i++)
            {
                var fieldIndexInTableCaption = this.hasFieldMap
                    ? this.fieldMap[i].ToInt()
                    : i + 1;

                while (nextIndex < fieldIndexInTableCaption)
                {
                    fieldValues.Add(string.Empty);
                    if (needsRawValues)
                    {
                        rawFieldValues.Add(string.Empty);
                    }

                    ++nextIndex;
                }

                string val, rawValue = null;
                if (resultFieldMap != null)
                {
                    var fieldMetaInfo = resultFieldMap[i];
                    if (fieldMetaInfo != null)
                    {
                        val = row.ValueForField(fieldMetaInfo);
                        if (needsRawValues)
                        {
                            rawValue = row.RawValueForField(fieldMetaInfo);
                        }

                        if (needsLabels)
                        {
                            var fieldLabel = fieldMetaInfo.CrmField?.Label;
                            fieldLabels.Add(fieldLabel ?? string.Empty);
                        }
                    }
                    else
                    {
                        val = string.Empty;
                        rawValue = string.Empty;
                    }
                }
                else
                {
                    val = row.ValueAtIndex(i);
                    if (needsRawValues)
                    {
                        rawValue = row.RawValueAtIndex(i);
                    }
                }

                fieldValues.Add(val == null ? string.Empty : val.SingleLineString());

                if (needsRawValues)
                {
                    rawFieldValues.Add(rawValue ?? string.Empty);
                }

                ++nextIndex;
            }
        }

        private string GetTableCaption(
            List<string> fieldValues,
            bool needsLabels,
            IReadOnlyList<string> fieldLabels,
            bool needsRawValues,
            IReadOnlyList<string> rawFieldValues,
            bool trim,
            int count)
        {
            var tableCaption = this.FormatString;
            if (this.SpecialDefArray != null)
            {
                count = this.SpecialDefArray.Count;
                for (var i = 0; i < count; i++)
                {
                    var specialDef = this.SpecialDefArray[i];
                    if (specialDef != null && specialDef.AllArrayFieldsAreEmpty(fieldValues))
                    {
                        tableCaption = specialDef.FormatString;
                        break;
                    }
                }
            }

            if (needsLabels)
            {
                for (var i = 0; i < count; i++)
                {
                    var pattern = "{label:" + $"{i + 1}" + "}";
                    tableCaption = tableCaption.Replace(pattern, fieldLabels[i]);
                }
            }

            count = fieldValues.Count;
            for (var i = 0; i < count; i++)
            {
                tableCaption = tableCaption?.StringByReplacingOccurrencesOfParameterWithIndexWithString(i,
                    fieldValues[i]);
            }

            if (needsRawValues)
            {
                count = rawFieldValues.Count;
                for (var i = 0; i < count; i++)
                {
                    tableCaption = tableCaption.StringByReplacingOccurrencesOfParameterWithRawIndexWithString(i,
                        rawFieldValues[i]);
                }
            }

            tableCaption = tableCaption?.Trim();
            return trim ? tableCaption?.Trim(Environment.NewLine.ToCharArray()) : tableCaption;
        }
    }
}