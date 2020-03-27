// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPObjectivesItem.cs" company="Aurea Software Gmbh">
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
//   UPObjectivesItem
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Objectives
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.CRM.Delegates;
    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.Query;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Logging;
    using Aurea.CRM.Core.OperationHandling;
    using Aurea.CRM.Core.Session;
    using GalaSoft.MvvmLight.Ioc;

    /// <summary>
    /// UPObjectivesItem
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.CRM.Delegates.ISearchOperationHandler" />
    public class UPObjectivesItem : ISearchOperationHandler
    {
        private FieldControl documentFieldControl;
        private List<UPConfigButton> buttonActions;
        private List<DocumentData> documentArray;
        private List<string> values;
        private bool completed;
        private bool originalCompleted;

        /// <summary>
        /// Gets logging interface
        /// </summary>
        public static ILogger Logger => SimpleIoc.Default.GetInstance<ILogger>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UPObjectivesItem"/> class.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <param name="date">The date.</param>
        /// <param name="group">The group.</param>
        /// <param name="canBeDeleted">if set to <c>true</c> [can be deleted].</param>
        /// <param name="documentDelegate">The document delegate.</param>
        public UPObjectivesItem(string keyValue, DateTime date, UPObjectivesGroup group, bool canBeDeleted, UPObjectivesDocumentDelegate documentDelegate)
        {
            this.KeyValue = keyValue;
            this.Date = date;
            this.DocumentDelegate = documentDelegate;
            this.Group = group;
            int count = this.AdditionalFields?.Count ?? 0;
            if (count > 0)
            {
                this.values = new List<string>(this.AdditionalFields.Count);
                for (int i = 0; i < count; i++)
                {
                    this.values.Add(string.Empty);
                }
            }

            this.Deleted = false;
            this.Changed = false;
            this.Created = false;
            this.CanBeDeleted = canBeDeleted;
        }

        /// <summary>
        /// Gets the additional fields.
        /// </summary>
        /// <value>
        /// The additional fields.
        /// </value>
        public List<UPConfigFieldControlField> AdditionalFields => this.Group?.Configuration.AdditionalFields;

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<string> Values => this.values;

        /// <summary>
        /// Gets the group key value.
        /// </summary>
        /// <value>
        /// The group key value.
        /// </value>
        public string GroupKeyValue => this.Group.GroupKey;

        /// <summary>
        /// Gets the button actions.
        /// </summary>
        /// <value>
        /// The button actions.
        /// </value>
        public List<UPConfigButton> ButtonActions => this.buttonActions;

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
        public List<DocumentData> Documents => this.documentArray;

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <value>
        /// The key value.
        /// </value>
        public string KeyValue { get; private set; }

        /// <summary>
        /// Gets the title field.
        /// </summary>
        /// <value>
        /// The title field.
        /// </value>
        public UPConfigFieldControlField TitleField { get; private set; }

        /// <summary>
        /// Gets the title field value.
        /// </summary>
        /// <value>
        /// The title field value.
        /// </value>
        public string TitleFieldValue { get; private set; }

        /// <summary>
        /// Gets the sub title field.
        /// </summary>
        /// <value>
        /// The sub title field.
        /// </value>
        public UPConfigFieldControlField SubTitleField { get; private set; }

        /// <summary>
        /// Gets the sub titel field value.
        /// </summary>
        /// <value>
        /// The sub titel field value.
        /// </value>
        public string SubTitelFieldValue { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public UPObjectivesGroup Group { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPObjectivesItem"/> is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if changed; otherwise, <c>false</c>.
        /// </value>
        public bool Changed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPObjectivesItem"/> is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPObjectivesItem"/> is created.
        /// </summary>
        /// <value>
        ///   <c>true</c> if created; otherwise, <c>false</c>.
        /// </value>
        public bool Created { get; set; }

        /// <summary>
        /// Gets the original values.
        /// </summary>
        /// <value>
        /// The original values.
        /// </value>
        public List<string> OriginalValues { get; private set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance can be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can be deleted; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeDeleted { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPObjectivesItem"/> is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if completed; otherwise, <c>false</c>.
        /// </value>
        public bool Completed
        {
            get
            {
                return this.completed;
            }

            set
            {
                if (value != this.originalCompleted)
                {
                    this.Changed = true;
                }

                this.completed = value;
            }
        }

        /// <summary>
        /// Gets the document delegate.
        /// </summary>
        /// <value>
        /// The document delegate.
        /// </value>
        public UPObjectivesDocumentDelegate DocumentDelegate { get; }

        /// <summary>
        /// Adds the button action.
        /// </summary>
        /// <param name="button">The button.</param>
        public void AddButtonAction(UPConfigButton button)
        {
            if (this.buttonActions == null)
            {
                this.buttonActions = new List<UPConfigButton> { button };
            }
            else
            {
                this.buttonActions.Add(button);
            }
        }

        /// <summary>
        /// Searches the operation did fail with error.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="error">The error.</param>
        public void SearchOperationDidFailWithError(Operation operation, Exception error)
        {
            this.DocumentDelegate?.ObjectivesDocumentFromSenderDidFailWithError(this, error);
        }

        /// <summary>
        /// Searches the operation did finish with result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="searchResult">The search result.</param>
        public void SearchOperationDidFinishWithResult(Operation operation, UPCRMResult searchResult)
        {
            if (searchResult != null && searchResult.RowCount > 0)
            {
                DocumentInfoAreaManager documentInfoAreaManager = new DocumentInfoAreaManager(this.documentFieldControl.InfoAreaId, this.documentFieldControl, null);
                this.documentArray = new List<DocumentData>();
                for (int i = 0; i < searchResult.RowCount; i++)
                {
                    UPCRMResultRow resultRow = (UPCRMResultRow)searchResult.ResultRowAtIndex(i);
                    DocumentData document = documentInfoAreaManager.DocumentDataForResultRow(resultRow);
                    this.documentArray.Add(document);
                }
            }

            this.DocumentDelegate?.ObjectivesDocumentDidFinishFromSender(this);
        }

        /// <summary>
        /// Sets from result row.
        /// </summary>
        /// <param name="row">The row.</param>
        public void SetFromResultRow(UPCRMResultRow row)
        {
            if (row != null)
            {
                this.Record = new UPCRMRecord(row.RootRecordIdentification);
                UPObjectivesConfiguration groupConfiguration = this.Group.Configuration;
                if (groupConfiguration != null)
                {
                    UPCRMListFormatter formatter = new UPCRMListFormatter(groupConfiguration.DestinationFieldControl);
                    this.TitleField = formatter.FirstFieldForPosition(0);
                    this.TitleFieldValue = formatter.StringFromRowForPosition(row, 0);
                    this.SubTitleField = formatter.FirstFieldForPosition(1);
                    this.SubTitelFieldValue = formatter.StringFromRowForPosition(row, 1);
                }

                if (this.AdditionalFields.Count > 0)
                {
                    this.values = new List<string>(this.AdditionalFields.Count);
                    foreach (UPConfigFieldControlField field in this.AdditionalFields)
                    {
                        this.values.Add(row.RawValueAtIndex(field.TabIndependentFieldIndex));
                    }
                }

                this.OriginalValues = new List<string>(this.values);
                this.completed = row.RawValueAtIndex(this.Group.Configuration.FieldForFunction(Constants.FieldCompletedFunction).TabIndependentFieldIndex).ToBoolWithDefaultValue(false);
                this.originalCompleted = this.completed;
            }
            else
            {
                this.Record = null;
                if (this.AdditionalFields.Count > 0)
                {
                    this.values = new List<string>(this.AdditionalFields.Count);
                    for (int i = 0; i < this.AdditionalFields.Count; i++)
                    {
                        this.values.Add(string.Empty);
                    }
                }

                this.OriginalValues = null;
                this.completed = false;
            }

            this.Created = false;
            this.Deleted = false;
            this.Changed = false;
        }

        /// <summary>
        /// Sets the value for additional field position.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="position">The position.</param>
        public void SetValueForAdditionalFieldPosition(string rawValue, int position)
        {
            if (this.values != null)
            {
                this.values[position] = rawValue;
                this.Changed = true;
                this.Deleted = false;
                if (this.Record == null)
                {
                    this.Created = true;
                }
            }
        }

        /// <summary>
        /// Changeds the records.
        /// </summary>
        /// <returns></returns>
        public List<UPCRMRecord> ChangedRecords()
        {
            UPCRMRecord changedRecord = null;
            if (this.Deleted && this.Record != null)
            {
                changedRecord = new UPCRMRecord(this.Record.RecordIdentification, "Delete");
            }
            else
            {
                if (this.Created && this.Record == null)
                {
                    UPObjectives objectives = this.Group.Objectives;
                    changedRecord = new UPCRMRecord(this.Group.Configuration.DestinationFieldControl.InfoAreaId);
                    changedRecord.AddLink(new UPCRMLink(objectives.RecordIdentification));
                    for (int i = 0; i < this.AdditionalFields.Count; i++)
                    {
                        string newValue = this.values[i];
                        if (!string.IsNullOrEmpty(newValue))
                        {
                            UPConfigFieldControlField field = this.AdditionalFields[i];
                            changedRecord.NewValueFieldId(newValue, field.FieldId);
                        }
                    }
                }
                else if (this.Changed && this.Record != null)
                {
                    changedRecord = new UPCRMRecord(this.Record.RecordIdentification);
                    changedRecord.AddLink(new UPCRMLink(this.Group.Objectives.RecordIdentification));
                    for (int i = 0; i < this.AdditionalFields.Count; i++)
                    {
                        string originalValue = this.OriginalValues[i];
                        string newValue = this.values[i];
                        if (originalValue != newValue)
                        {
                            UPConfigFieldControlField field = this.AdditionalFields[i];
                            changedRecord.NewValueFromValueFieldId(newValue, originalValue, field.FieldId);
                        }
                    }

                    if (this.completed != this.originalCompleted)
                    {
                        string sCompleted = StringExtensions.CrmValueFromBool(this.completed);
                        string sOriginalCompleted = StringExtensions.CrmValueFromBool(this.originalCompleted);
                        if (this.Group.Configuration.FieldForFunction(Constants.FieldCompletedFunction) != null)
                        {
                            changedRecord.NewValueFromValueFieldId(sCompleted, sOriginalCompleted, this.Group.Configuration.FieldForFunction(Constants.FieldCompletedFunction).FieldId);
                        }

                        if (this.completed)
                        {
                            if (this.Group.Configuration.FieldForFunction(Constants.FieldCompletedOnFunction) != null)
                            {
                                changedRecord.NewValueFromValueFieldId(StringExtensions.CrmValueFromDate(DateTime.UtcNow), string.Empty, this.Group.Configuration.FieldForFunction(Constants.FieldCompletedOnFunction).FieldId);
                            }

                            if (this.Group.Configuration.FieldForFunction(Constants.FieldCompletedByFunction) != null)
                            {
                                changedRecord.NewValueFieldId(ServerSession.CurrentSession.CurRep, this.Group.Configuration.FieldForFunction(Constants.FieldCompletedByFunction).FieldId);
                            }
                        }
                    }
                }
            }

            return changedRecord != null ? new List<UPCRMRecord> { changedRecord } : null;
        }

        /// <summary>
        /// Loads the documents with maximum results.
        /// </summary>
        /// <param name="maxResults">The maximum results.</param>
        public void LoadDocumentsWithMaxResults(int maxResults)
        {
            if (this.Record != null)
            {
                IConfigurationUnitStore configStore = ConfigurationUnitStore.DefaultStore;
                this.documentFieldControl = configStore.FieldControlByNameFromGroup("List", "D3DocData");
                UPContainerMetaInfo containerMetaInfo = new UPContainerMetaInfo(this.documentFieldControl);
                containerMetaInfo.SetLinkRecordIdentification(this.Record.OriginalRecordIdentification, 127);
                if (maxResults > 0)
                {
                    containerMetaInfo.MaxResults = maxResults;
                }

                Operation operation = containerMetaInfo.Find(this.Group.Objectives.RequestOption, this);
                if (operation == null)
                {
                    //DDLogError("Could not create operation for loading documents.{}");
                    Logger.LogError($"Could not create operation for loading documents. {this.Group.Objectives.RequestOption.ToString()}");
                }
            }
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
