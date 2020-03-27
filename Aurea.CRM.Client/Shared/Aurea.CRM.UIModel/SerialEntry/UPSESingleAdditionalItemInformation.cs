// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSESingleAdditionalItemInformation.cs" company="Aurea Software Gmbh">
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
//   The UPSESingleAdditionalItemInformation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// UPSESingleAdditionalItemInformation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPSESingleAdditionalItemInformation : ISearchOperationHandler
    {
        private UPContainerMetaInfo crmQuery;
        private UPCRMResult result;
        private Dictionary<string, UPCRMResultRow> rowFromItemKey;
        private int keyColumnIndex;

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets the field control.
        /// </summary>
        /// <value>
        /// The field control.
        /// </value>
        public FieldControl FieldControl { get; private set; }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public UPConfigFilter Filter { get; private set; }

        /// <summary>
        /// Gets the name of the key column.
        /// </summary>
        /// <value>
        /// The name of the key column.
        /// </value>
        public string KeyColumnName { get; private set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPSESingleAdditionalItemInformationDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Creates the specified serial entry.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <param name="configName">Name of the configuration.</param>
        /// <param name="keyColumnName">Name of the key column.</param>
        /// <param name="filterParameters">The filter parameters.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public static UPSESingleAdditionalItemInformation Create(UPSerialEntry serialEntry, string configName, string keyColumnName,
            Dictionary<string, object> filterParameters, UPSESingleAdditionalItemInformationDelegate theDelegate)
        {
            try
            {
                return new UPSESingleAdditionalItemInformation(serialEntry, configName, keyColumnName, filterParameters, theDelegate);
            }
            catch
            {
                return null;
            }
        }

        private UPSESingleAdditionalItemInformation(UPSerialEntry _serialEntry, string _configName, string _keyColumnName,
            Dictionary<string, object> filterParameters, UPSESingleAdditionalItemInformationDelegate theDelegate)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndListConfiguration = configStore.SearchAndListByName(_configName);
            if (searchAndListConfiguration == null)
            {
                throw new Exception("Search & List Config is null");
            }

            this.FieldControl = configStore.FieldControlByNameFromGroup("List", searchAndListConfiguration.FieldGroupName);
            if (this.FieldControl == null)
            {
                throw new Exception("FieldControl is null");
            }

            if (searchAndListConfiguration.FilterName != null)
            {
                this.Filter = configStore.FilterByName(searchAndListConfiguration.FilterName);
                if (this.Filter != null && filterParameters != null)
                {
                    this.Filter = this.Filter.FilterByApplyingReplacements(UPConditionValueReplacement.ReplacementsFromValueParameterDictionary(filterParameters));
                }

                if (this.Filter == null)
                {
                    throw new Exception("Filter is null");
                }
            }

            UPConfigFieldControlField field = this.FieldControl.FieldWithFunction(_keyColumnName);
            if (field == null)
            {
                throw new Exception("Field is null");
            }

            this.keyColumnIndex = field.TabIndependentFieldIndex;
            this.SerialEntry = _serialEntry;
            this.KeyColumnName = _keyColumnName;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Loads the with request option.
        /// </summary>
        /// <param name="requestOption">The request option.</param>
        /// <returns></returns>
        public bool LoadWithRequestOption(UPRequestOption requestOption)
        {
            this.crmQuery = new UPContainerMetaInfo(this.FieldControl);
            if (this.Filter != null)
            {
                this.crmQuery.ApplyFilter(this.Filter);
            }

            this.crmQuery.Find(requestOption, this);
            return true;
        }

        /// <summary>
        /// Additionals the source fields with key column.
        /// </summary>
        /// <param name="keyColumn">The key column.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public List<UPSESourceAdditionalColumn> AdditionalSourceFieldsWithKeyColumn(UPSESourceColumn keyColumn, int startIndex)
        {
            List<UPSESourceAdditionalColumn> fieldArray = null;
            int positionInControl = 0;
            foreach (UPConfigFieldControlField field in this.FieldControl.FieldsOnFirstTab)
            {
                if (field.Function == this.KeyColumnName)
                {
                    positionInControl++;
                    continue;
                }

                UPSESourceAdditionalColumn addColumn = new UPSESourceAdditionalColumn(this, field, startIndex++, positionInControl++, keyColumn);
                if (fieldArray == null)
                {
                    fieldArray = new List<UPSESourceAdditionalColumn> { addColumn };
                }
                else
                {
                    fieldArray.Add(addColumn);
                }
            }

            return fieldArray;
        }

        /// <summary>
        /// Raws the value for item key result position.
        /// </summary>
        /// <param name="itemkey">The itemkey.</param>
        /// <param name="resultPosition">The result position.</param>
        /// <returns></returns>
        public string RawValueForItemKeyResultPosition(string itemkey, int resultPosition)
        {
            UPCRMResultRow row = this.rowFromItemKey.ValueOrDefault(itemkey);
            return row != null ? row.RawValueAtIndex(resultPosition) : string.Empty;
        }

        /// <summary>
        /// Values for item key result position.
        /// </summary>
        /// <param name="itemkey">The itemkey.</param>
        /// <param name="resultPosition">The result position.</param>
        /// <returns></returns>
        public string ValueForItemKeyResultPosition(string itemkey, int resultPosition)
        {
            UPCRMResultRow row = this.rowFromItemKey.ValueOrDefault(itemkey);
            return row != null ? row.ValueAtIndex(resultPosition) : string.Empty;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.TheDelegate?.AdditionalItemInformationDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="_result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult _result)
        {
            this.result = _result;
            int count = this.result.RowCount;
            if (count > 0)
            {
                this.rowFromItemKey = new Dictionary<string, UPCRMResultRow>();
                for (int i = 0; i < count; i++)
                {
                    UPCRMResultRow row = (UPCRMResultRow)this.result.ResultRowAtIndex(i);
                    string itemKey = row.RawValueAtIndex(this.keyColumnIndex);
                    this.rowFromItemKey[itemKey] = row;
                }
            }

            this.TheDelegate?.AdditionalItemInformationDidFinishWithResult(this, _result);
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with count.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="count">The count.</param>
        public void SearchOperationDidFinishWithCount(Operation operation, int count)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the operation did finish with counts.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="counts">The counts.</param>
        public void SearchOperationDidFinishWithCounts(Operation operation, List<int?> counts)
        {
            // Function part of interface implementation, not applicable here, will never be implemented.
            throw new NotImplementedException();
        }
    }
}
