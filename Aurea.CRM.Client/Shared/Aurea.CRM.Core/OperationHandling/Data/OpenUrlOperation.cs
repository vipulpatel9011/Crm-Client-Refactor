// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenUrlOperation.cs" company="Aurea Software Gmbh">
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
//   Data Synchronization Organizer Model Controller
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OperationHandling.Data
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;

    /// <summary>
    /// Open Url Operation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OperationHandling.LocalOperation" />
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class OpenUrlOperation : LocalOperation, ISearchOperationHandler
    {
        private FieldControl fieldControl;
        private UPConfigFilter filter;
        private Dictionary<string, UPConfigFieldControlField> fieldFunctions;
        private string replacedUrl;
        private Action<Uri, Exception> completion;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenUrlOperation"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="record">The record.</param>
        /// <param name="fieldGroup">The field group.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="theDelegate">The delegate.</param>
        public OpenUrlOperation(string url, string record, string fieldGroup, string encoding, UPOpenUrlOperationDelegate theDelegate)
        {
            this.Record = record;
            this.Url = url;
            this.FieldGroup = fieldGroup;
            this.Encoding = encoding;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenUrlOperation"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="recordId">The record identifier.</param>
        /// <param name="fieldGroup">The field group.</param>
        /// <param name="completion">The completion.</param>
        public OpenUrlOperation(string url, string recordId, string fieldGroup, Action<Uri, Exception> completion)
        {
            this.Record = recordId;
            this.Url = url;
            this.FieldGroup = fieldGroup;
            this.completion = completion;
        }

        /// <summary>
        /// Gets or sets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public string Record { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the field group.
        /// </summary>
        /// <value>
        /// The field group.
        /// </value>
        public string FieldGroup { get; set; }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>
        /// The encoding.
        /// </value>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPOpenUrlOperationDelegate TheDelegate { get; set; }

        /// <summary>
        /// Performs the operation.
        /// </summary>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        public override bool PerformOperation()
        {
            this.replacedUrl = this.ReplaceRepVariables(this.Url);
            this.LoadRecordForFieldGroup(this.FieldGroup);
            return true;
        }

        private string ReplaceRepVariables(string rawUrl)
        {
            string currentReplacedUrl = rawUrl;
            //int encodingConstant = this.EncodingForString(this.Encoding);
            //UPCRMDataStore dataStore = ServerSession.CurrentSession.CrmDataStore;
            //currentReplacedUrl = currentReplacedUrl.StringByReplacingOccurrencesOfStringWithString("$curRepId;", dataStore.Reps.CurrentRepId.StringByAddingPercentEscapesUsingEncoding(encodingConstant));
            //currentReplacedUrl = currentReplacedUrl.StringByReplacingOccurrencesOfStringWithString("$curRep;", dataStore.Reps.CurrentRep.RepName.StringByAddingPercentEscapesUsingEncoding(encodingConstant));
            //currentReplacedUrl = currentReplacedUrl.StringByReplacingOccurrencesOfStringWithString("$curTenantNo;", NSString.StringWithFormat("%ld", (long)UPCRMSession.CurrentSession().TenantNo()).StringByAddingPercentEscapesUsingEncoding(encodingConstant));
            //currentReplacedUrl = currentReplacedUrl.StringByReplacingOccurrencesOfStringWithString("$curLanguage;", ServerSession.CurrentSession.LanguageKey.StringByAddingPercentEscapesUsingEncoding(encodingConstant));
            return currentReplacedUrl;
        }

        private void LoadRecordForFieldGroup(string fieldGroup)
        {
            this.LoadConfigFieldControlForFieldGroup(fieldGroup);
            this.fieldFunctions = this.fieldControl.FunctionNames();
            if (this.fieldFunctions != null)
            {
                UPContainerMetaInfo container = this.LoadContainer();
                container.ReadRecord(this.Record, UPRequestOption.BestAvailable, this);
            }
            else
            {
                this.OpenUrl(this.replacedUrl);
            }
        }

        private void OpenUrl(string _url)
        {
            Uri urlToOpen = new Uri(_url);

            if (urlToOpen.Scheme == null)
            {
                //DDLogWarn("No scheme given for url %@, using 'http' .", urlToOpen.AbsoluteString());
                Logger.LogWarn($"No scheme given for url {urlToOpen.AbsoluteUri}, using 'http' .");
                urlToOpen = new Uri($"http://{_url}");
            }

            this.TheDelegate?.OpenUrlOperationDidFinishWithResult(this, urlToOpen);
        }

        private void LoadConfigFieldControlForFieldGroup(string fieldGroup)
        {
            IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
            SearchAndList searchAndListConfig = configStore.SearchAndListByName(fieldGroup);
            FieldControl configFieldControl;
            UPConfigFilter configFilter = null;
            if (searchAndListConfig != null)
            {
                configFieldControl = configStore.FieldControlByNameFromGroup("List", fieldGroup);
                if (!string.IsNullOrEmpty(searchAndListConfig.FilterName))
                {
                    configFilter = configStore.FilterByName(searchAndListConfig.FilterName);
                }
            }
            else
            {
                configFieldControl = configStore.FieldControlByNameFromGroup("List", fieldGroup);
            }

            if (configFieldControl != null)
            {
                this.fieldControl = configFieldControl;
            }

            if (configFilter != null)
            {
                this.filter = configFilter;
            }
        }

        private UPContainerMetaInfo LoadContainer()
        {
            if (this.fieldControl == null)
            {
                return null;
            }

            UPContainerMetaInfo metaInfo = new UPContainerMetaInfo(this.fieldControl);
            if (this.filter != null)
            {
                metaInfo.ApplyFilter(this.filter);
            }

            return metaInfo;
        }

        //NSStringEncoding EncodingForString(string _stringEncoding)
        //{
        //    NSStringEncoding usedEncoding = NSUTF8StringEncoding;
        //    if (_stringEncoding)
        //    {
        //        NSDictionary encodingDictionary = this.EncodingDictionary();
        //        NSNumber tempEncoding = (NSNumber)encodingDictionary.ObjectForKey(_stringEncoding.LowercaseString);
        //        if (tempEncoding) usedEncoding = tempEncoding.IntegerValue;

        //    }

        //    return usedEncoding;
        //}

        //NSDictionary EncodingDictionary()
        //{
        //    static NSMutableDictionary dictionary = null;
        //    if (!dictionary)
        //    {
        //        dictionary = new NSMutableDictionary();
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSASCIIStringEncoding), "ascii");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSNEXTSTEPStringEncoding), "nextstep");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSJapaneseEUCStringEncoding), "japanese");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF8StringEncoding), "utf8");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSISOLatin1StringEncoding), "isolation1");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSSymbolStringEncoding), "symbol");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSNonLossyASCIIStringEncoding), "nonlossyasci");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSShiftJISStringEncoding), "shiftjis");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSISOLatin2StringEncoding), "isolatin2");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUnicodeStringEncoding), "unicode");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSWindowsCP1251StringEncoding), "windowscp1251");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSWindowsCP1252StringEncoding), "windowscp1252");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSWindowsCP1253StringEncoding), "windowscp1253");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSWindowsCP1254StringEncoding), "windowscp1254");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSWindowsCP1250StringEncoding), "windowscp1250");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSISO2022JPStringEncoding), "iso2022jps");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSMacOSRomanStringEncoding), "macosroman");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF16StringEncoding), "utf16");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF16BigEndianStringEncoding), "utf16bigendian");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF16LittleEndianStringEncoding), "utf16littleendian");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF32StringEncoding), "utf32");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF32BigEndianStringEncoding), "utf32bigendian");
        //        dictionary.SetObjectForKey(NSNumber.NumberWithInt(NSUTF32LittleEndianStringEncoding), "utf32littleendian");
        //    }

        //    return dictionary;
        //}

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            //DDLogError("Could not load record %@ error %@.", this.Record, error);
            Logger.LogError($"Could not load record {this.Record} error{error?.Message}.");
        }

        /// <summary>
        /// Searches the operation did finish with results.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="results">The results.</param>
        public void SearchOperationDidFinishWithResults(Operation operation, List<UPCRMResult> results)
        {
            //DDLogError("Could not load record %@.", this.Record);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            //if (result != null && result.RowCount > 0)
            //{
            //    UPCRMResultRow resultRow = (UPCRMResultRow)result.ResultRowAtIndex(0);
            //    uint encodingConstant = this.EncodingForString(this.Encoding);
            //    uint recordIdCount = result.Metainfo.NumberOfResultInfoAreaMetaInfos;
            //    for (uint i = 0; i < recordIdCount; i++)
            //    {
            //        string variableString = string.Format("{recid{0}}", i);
            //        if (this.replacedUrl.RangeOfString(variableString).Location != NSNotFound)
            //        {
            //            string value = resultRow.RecordIdentificationAtIndex(i);
            //            string encodedValue = value.StringByAddingPercentEscapesUsingEncoding(encodingConstant);
            //            if (encodedValue)
            //            {
            //                DDLogInfo("Replacing %@ with %@ in url %@.", variableString, value, this.replacedUrl);
            //                this.replacedUrl = this.replacedUrl.StringByReplacingOccurrencesOfStringWithString(variableString, encodedValue);
            //            }
            //        }
            //    }

            //    foreach (object key in this.fieldFunctions.AllKeys())
            //    {
            //        UPConfigFieldControlField field = (UPConfigFieldControlField)this.fieldFunctions.ObjectForKey(key);
            //        string variableString = NSString.StringWithFormat("{$%@}", key);
            //        if (this.replacedUrl.RangeOfString(variableString).Location != NSNotFound)
            //        {
            //            string value = resultRow.RawValueAtIndex(field.TabIndependentFieldIndex);
            //            string encodedValue = value.StringByAddingPercentEscapesUsingEncoding(encodingConstant);
            //            if (encodedValue)
            //            {
            //                DDLogInfo("Replacing %@ with %@ in url %@.", variableString, value, this.replacedUrl);
            //                this.replacedUrl = this.replacedUrl.StringByReplacingOccurrencesOfStringWithString(variableString, encodedValue);
            //            }
            //        }
            //    }

            //    this.OpenUrl(this.replacedUrl);
            //}
            //else
            //{
            //    //DDLogError("Could not load record %@.", this.Record);
            //}
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
