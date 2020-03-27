// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyFields.cs" company="Aurea Software Gmbh">
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
//   Copy Fields Delegate
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.Catalogs;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// Copy Fields implementation
    /// </summary>
    public class UPCopyFields
    {
        /// <summary>
        /// The requests
        /// </summary>
        protected List<UPCopyFieldsRequest> requests;

        /// <summary>
        /// Gets the field configs for function.
        /// </summary>
        /// <value>
        /// The field configs for function.
        /// </value>
        public Dictionary<string, UPConfigFieldControlField> FieldConfigsForFunction { get; private set; }

        /// <summary>
        /// Gets the field control configuration.
        /// </summary>
        /// <value>
        /// The field control configuration.
        /// </value>
        public FieldControl FieldControlConfiguration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCopyFields"/> class.
        /// </summary>
        /// <param name="fieldControlConfiguration">The field control configuration.</param>
        public UPCopyFields(FieldControl fieldControlConfiguration)
        {
            this.FieldControlConfiguration = fieldControlConfiguration;
            Dictionary<string, UPConfigFieldControlField> dictionary = new Dictionary<string, UPConfigFieldControlField>();

            foreach (FieldControlTab tab in fieldControlConfiguration.Tabs)
            {
                if (tab.Fields != null)
                {
                    foreach (UPConfigFieldControlField field in tab.Fields)
                    {
                        if (!string.IsNullOrEmpty(field.Function))
                        {
                            dictionary[field.Function] = field;
                        }
                    }
                }
            }

            this.FieldConfigsForFunction = dictionary;
        }

        /// <summary>
        /// Copies the field values for result.
        /// </summary>
        /// <param name="resultRow">The result row.</param>
        /// <returns></returns>
        public Dictionary<string, object> CopyFieldValuesForResult(UPCRMResultRow resultRow)
        {
            Dictionary<string, object> resultDictionary = new Dictionary<string, object>();
            foreach (string key in this.FieldConfigsForFunction.Keys)
            {
                UPConfigFieldControlField fieldConfig = this.FieldConfigsForFunction[key];
                string rawResult = resultRow.RawValueAtIndex(fieldConfig.TabIndependentFieldIndex);
                if (!string.IsNullOrEmpty(rawResult))
                {
                    resultDictionary[key] = rawResult;
                    if (fieldConfig.Field.FieldType == "K")
                    {
                        int catval = Convert.ToInt32(rawResult);
                        if (catval > 0)
                        {
                            UPCatalog catalog = fieldConfig.Field.Catalog;
                            UPCatalogValue catalogValue = catalog.ValueForCode(catval);

                            if (!string.IsNullOrEmpty(catalogValue?.ExtKey))
                            {
                                resultDictionary[$"{key}.extkey"] = catalogValue.ExtKey;
                            }

                            resultDictionary[$"{key}.text"] = catalogValue?.Text ?? string.Empty;
                        }
                    }
                }
                else
                {
                    resultDictionary[key] = string.Empty;
                }
            }

            return resultDictionary.Count > 0 ? resultDictionary : null;
        }

        /// <summary>
        /// Copies the field values for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <returns></returns>
        public Dictionary<string, object> CopyFieldValuesForRecordIdentification(string recordIdentification)
        {
            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(this.FieldControlConfiguration);
            crmQuery.SetLinkRecordIdentification(recordIdentification);
            UPCRMResult result = crmQuery.Find();

            return result.RowCount >= 1 ? this.CopyFieldValuesForResult((UPCRMResultRow)result.ResultRowAtIndex(0)) : null;
        }

        /// <summary>
        /// Copies the field values for record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="alwaysRemote">if set to <c>true</c> [always remote].</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation CopyFieldValuesForRecordIdentification(string recordIdentification, bool alwaysRemote, UPCopyFieldsDelegate theDelegate)
        {
            if (!alwaysRemote)
            {
                Dictionary<string, object> dict = this.CopyFieldValuesForRecordIdentification(recordIdentification);
                if (dict?.Count > 0)
                {
                    theDelegate?.CopyFieldsDidFinishWithValues(this, dict);
                    return null;
                }
            }

            UPContainerMetaInfo crmQuery = new UPContainerMetaInfo(this.FieldControlConfiguration);
            crmQuery.SetLinkRecordIdentification(recordIdentification);
            UPCopyFieldsRequest request = new UPCopyFieldsRequest(crmQuery, theDelegate, this);
            request.AlwaysRemote = true;

            if (this.requests == null)
            {
                this.requests = new List<UPCopyFieldsRequest>();
            }

            this.requests.Add(request);

            return request.Start();
        }

        /// <summary>
        /// Copies the field values for record.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="alwaysRemote">if set to <c>true</c> [always remote].</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <returns></returns>
        public Operation CopyFieldValuesForRecord(UPCRMRecord record, bool alwaysRemote, UPCopyFieldsDelegate theDelegate)
        {
            return this.CopyFieldValuesForRecordIdentification(record.RecordIdentification, alwaysRemote, theDelegate);
        }
    }
}
