// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordCopy.cs" company="Aurea Software Gmbh">
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
//   The UPRecordCopy
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.OperationHandling;

    /// <summary>
    /// UPRecordCopy
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPRecordCopy : ISearchOperationHandler
    {
        private UPConfigFilter replacedFilter;
        private List<UPRecordCopyStep> stepQueue;
        private List<UPCRMRecord> recordArray;
        private bool running;
        private Dictionary<string, object> parameters;
        private UPContainerMetaInfo crmQuery;
        private IConfigurationUnitStore configStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordCopy"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPRecordCopy(ViewReference viewReference, UPRecordCopyDelegate theDelegate)
            : this(viewReference.ContextValueForKey("TemplateFilter"), theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordCopy"/> class.
        /// </summary>
        /// <param name="templateFilterName">Name of the template filter.</param>
        /// <param name="theDelegate">The delegate.</param>
        /// <exception cref="System.Exception">Template Filter is null</exception>
        public UPRecordCopy(string templateFilterName, UPRecordCopyDelegate theDelegate)
        {
            this.configStore = ConfigurationUnitStore.DefaultStore;
            this.TemplateFilter = this.configStore.FilterByName(templateFilterName);
            if (this.TemplateFilter == null)
            {
                throw new Exception("Template Filter is null");
            }

            this.RequestOption = UPRequestOption.Offline;
            this.TheDelegate = theDelegate;
        }

        /// <summary>
        /// Gets the template filter.
        /// </summary>
        /// <value>
        /// The template filter.
        /// </value>
        public UPConfigFilter TemplateFilter { get; private set; }

        /// <summary>
        /// Gets or sets the request option.
        /// </summary>
        /// <value>
        /// The request option.
        /// </value>
        public UPRequestOption RequestOption { get; set; }

        /// <summary>
        /// Gets the delegate.
        /// </summary>
        /// <value>
        /// The delegate.
        /// </value>
        public UPRecordCopyDelegate TheDelegate { get; private set; }

        /// <summary>
        /// Starts the with source record identification.
        /// </summary>
        /// <param name="recordIdentification">The record identification.</param>
        /// <param name="_parameters">The parameters.</param>
        /// <returns></returns>
        public bool StartWithSourceRecordIdentification(string recordIdentification, Dictionary<string, object> _parameters)
        {
            if (this.running)
            {
                return false;
            }

            this.running = true;
            this.parameters = _parameters;
            this.replacedFilter = this.TemplateFilter.FilterByApplyingValueDictionaryDefaults(this.parameters, true);
            UPRecordCopyStep recordStep = new UPRecordCopyStep();
            this.recordArray = new List<UPCRMRecord>();
            recordStep.SourceRecordIdentification = recordIdentification;
            recordStep.QueryTable = this.replacedFilter.RootTable;
            this.ConfigForStepFromQueryTable(recordStep, recordStep.QueryTable);
            this.stepQueue = new List<UPRecordCopyStep> { recordStep };
            this.ExecuteNextStep();
            return true;
        }

        /// <summary>
        /// Starts the with source record identification.
        /// </summary>
        /// <param name="sourceRecordIdentification">The source record identification.</param>
        /// <param name="destRecordIdentification">The dest record identification.</param>
        /// <param name="_parameters">The parameters.</param>
        /// <returns></returns>
        public bool StartWithSourceRecordIdentification(string sourceRecordIdentification, string destRecordIdentification, Dictionary<string, object> _parameters)
        {
            if (this.running)
            {
                return false;
            }

            this.running = true;
            this.parameters = _parameters;
            this.replacedFilter = this.TemplateFilter.FilterByApplyingValueDictionaryDefaults(this.parameters, true);
            UPCRMRecord destinationRootRecord = new UPCRMRecord(destRecordIdentification);
            int count = this.replacedFilter.RootTable.NumberOfSubTables;
            this.recordArray = new List<UPCRMRecord>();
            this.stepQueue = new List<UPRecordCopyStep>();
            for (int i = 0; i < count; i++)
            {
                UPConfigQueryTable currentTable = this.replacedFilter.RootTable.SubTableAtIndex(i);
                UPRecordCopyStep recordStep = new UPRecordCopyStep();
                recordStep.SourceRecordIdentification = sourceRecordIdentification;
                recordStep.QueryTable = currentTable;
                recordStep.DestinationRecord = destinationRootRecord;
                this.ConfigForStepFromQueryTable(recordStep, currentTable);
                this.stepQueue.Add(recordStep);
            }

            this.ExecuteNextStep();
            return true;
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.FinishWithError(error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="result">The result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult result)
        {
            this.ProcessResult(result);
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

        private void ConfigForStepFromQueryTable(UPRecordCopyStep recordCopyStep, UPConfigQueryTable queryTable)
        {
            UPConfigQueryCondition sourceConfigNameCondition = queryTable.PropertyConditions.ValueOrDefault("SourceConfig");
            string sourceConfigName = null;
            if (sourceConfigNameCondition != null && sourceConfigNameCondition.FieldValues.Count > 0)
            {
                sourceConfigName = sourceConfigNameCondition.FieldValues[0] as string;
            }

            bool skipSearchAndList = false;
            if (string.IsNullOrEmpty(sourceConfigName))
            {
                sourceConfigName = queryTable.InfoAreaId;
                skipSearchAndList = true;
            }

            if (!skipSearchAndList)
            {
                recordCopyStep.SearchAndListConfiguration = this.configStore.SearchAndListByName(sourceConfigName);
            }

            if (recordCopyStep.SearchAndListConfiguration == null)
            {
                recordCopyStep.FieldControl = this.configStore.FieldControlByNameFromGroup("Edit", sourceConfigName) ??
                                              this.configStore.FieldControlByNameFromGroup("List", sourceConfigName);
            }
            else
            {
                recordCopyStep.FieldControl = this.configStore.FieldControlByNameFromGroup("List", recordCopyStep.SearchAndListConfiguration.FieldGroupName);
            }
        }

        private void FinishWithError(Exception error)
        {
            this.running = false;
            if (error != null)
            {
                this.TheDelegate.RecordCopyDidFailWithError(this, error);
            }
            else
            {
                this.TheDelegate.RecordCopyDidFinishWithResult(this, this.recordArray);
            }
        }

        private void ExecuteNextStep()
        {
            if (this.TheDelegate == null)
            {
                return;
            }
            else if (this.stepQueue.Count == 0)
            {
                this.FinishWithError(null);
                return;
            }

            UPRecordCopyStep currentStep = this.stepQueue[0];
            if (currentStep.SearchAndListConfiguration != null)
            {
                this.crmQuery = new UPContainerMetaInfo(currentStep.SearchAndListConfiguration, this.parameters);
            }
            else
            {
                this.crmQuery = new UPContainerMetaInfo(currentStep.FieldControl);
            }

            if (this.crmQuery == null)
            {
                this.stepQueue.RemoveAt(0);
                this.ExecuteNextStep();
                return;
            }

            this.crmQuery.SetLinkRecordIdentification(currentStep.SourceRecordIdentification, currentStep.QueryTable.LinkId);
            if (this.crmQuery.Find(this.RequestOption, this) == null)
            {
                this.FinishWithError(new Exception("RecordCopy: could not start search operation"));
                return;
            }
        }

        private void ProcessResult(UPCRMResult result)
        {
            UPRecordCopyStep currentStep = this.stepQueue[0];
            this.stepQueue.RemoveAt(0);
            UPConfigQueryTable queryTable = currentStep.QueryTable;
            int count = result.RowCount;
            int resultTableCount = result.NumberOfResultTables;
            UPContainerInfoAreaMetaInfo copyResultInfoArea = null;
            if (queryTable.InfoAreaId == currentStep.FieldControl.InfoAreaId)
            {
                copyResultInfoArea = result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(0);
            }

            for (int i = 0; i < count; i++)
            {
                UPCRMResultRow row = (UPCRMResultRow)result.ResultRowAtIndex(i);
                UPCRMRecord record = new UPCRMRecord(queryTable.InfoAreaId);
                if (currentStep.DestinationRecord != null)
                {
                    record.AddLink(new UPCRMLink(currentStep.DestinationRecord, queryTable.LinkId));
                }

                for (int j = 1; j < resultTableCount; j++)
                {
                    string linkRecordIdentification = row.RecordIdentificationAtIndex(j);
                    if (string.IsNullOrEmpty(linkRecordIdentification) && !result.IsServerResult)
                    {
                        UPContainerInfoAreaMetaInfo resultInfoArea = result.MetaInfo.ResultInfoAreaMetaInfoAtIndex(j);
                        UPCRMLinkReader linkReader = new UPCRMLinkReader(StringExtensions.InfoAreaIdRecordId(currentStep.FieldControl.InfoAreaId, row.RootRecordId),
                            $"{resultInfoArea.InfoAreaId}:{resultInfoArea.LinkId}", null);
                        linkRecordIdentification = linkReader.RequestLinkRecordOffline();
                    }

                    int linkId = -1;
                    if (linkRecordIdentification?.Length > 8)
                    {
                        if (currentStep.DestinationRecord == null || queryTable.LinkId != linkId
                            || linkRecordIdentification.InfoAreaId() != currentStep.DestinationRecord.InfoAreaId)
                        {
                            record.AddLink(new UPCRMLink(linkRecordIdentification, linkId));
                        }
                    }
                }

                Dictionary<string, object> fieldsWithFunctions = row.ValuesWithFunctions();
                UPConfigQueryTable replacedTable = queryTable.QueryTableByApplyingValueDictionary(fieldsWithFunctions);
                if (copyResultInfoArea != null)
                {
                    foreach (UPContainerFieldMetaInfo field in copyResultInfoArea.Fields)
                    {
                        string val = row.RawValueAtIndex(field.PositionInResult);
                        if (!string.IsNullOrEmpty(val))
                        {
                            record.AddValue(new UPCRMFieldValue(val, field.InfoAreaId, field.FieldId));
                        }
                    }
                }

                if (replacedTable != null)
                {
                    record.ApplyValuesFromTemplateFilter(replacedTable, true);
                }

                int numberOfSubTables = queryTable.NumberOfSubTables;
                if (numberOfSubTables > 0)
                {
                    for (int k = 0; k < numberOfSubTables; k++)
                    {
                        UPRecordCopyStep subStep = new UPRecordCopyStep();
                        subStep.QueryTable = queryTable.SubTableAtIndex(k);
                        subStep.SourceRecordIdentification = row.RootRecordIdentification;
                        subStep.DestinationRecord = record;
                        this.ConfigForStepFromQueryTable(subStep, subStep.QueryTable);
                        this.stepQueue.Add(subStep);
                    }
                }

                this.recordArray.Add(record);
            }

            this.ExecuteNextStep();
        }
    }
}
