// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangedChildRecordsForRootRecordParentRecordProcessor.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Denis Latushkin
// </author>
// <summary>
//   ChangedChildRecordsForRootRecordParentRecordProcessor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;
    using Aurea.CRM.Core.Session;

    /// <summary>
    /// Changed child records handler
    /// </summary>
    public class ChangedChildRecordsForRootRecordParentRecordProcessor
    {
        private const string IdNew = "new";

        private List<UPCRMRecord> additionalDestinationTemplateFilterRecords;
        private int count;
        private UPCRMRecordWithParameter destinationRootRecord;
        private List<UPCRMRecordWithParameter> destinationChildRecords;
        private List<UPCRMRecordWithParameter> tentativeChildRecords;
        private int modifiedChildRecordCount;
        private int newChildRecordCount;
        private List<UPCRMRecord> changedRecords;
        private UPSEColumn column;
        private UPSEDestinationColumnBase destinationColumn;
        private UPSERowValue rowValue;

        /// <summary>
        /// Gets or sets serial entry
        /// </summary>
        public UPSerialEntry SerialEntry { get; set; }

        /// <summary>
        /// Gets or sets serial entry record id
        /// </summary>
        public string SerialEntryRecordId { get; set; }

        /// <summary>
        /// Gets or sets destination root record
        /// </summary>
        public UPCRMRecord DestinationRootRecord { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether create changed
        /// </summary>
        public bool CreateUnchanged { get; set; }

        /// <summary>
        /// Gets or sets listing record id
        /// </summary>
        public string RowRecordId { get; set; }

        /// <summary>
        /// Gets or sets listing record id
        /// </summary>
        public string ListingRecordId { get; set; }

        /// <summary>
        /// Gets or sets row values
        /// </summary>
        public IList<UPSERowValue> RowValues { get; set; }

        /// <summary>
        /// Gets or sets child record ids
        /// </summary>
        public IList<string> ChildRecordIds { get; set; }

        /// <summary>
        /// Gets or sets row quota
        /// </summary>
        public UPSERowQuota RowQuota { get; set; }

        private string RowRecordIdentification =>
            this.SerialEntry.SourceInfoAreaId.InfoAreaIdRecordId(this.RowRecordId);

        /// <summary>
        /// Sets properties according on changed child records
        /// </summary>
        /// <param name="rootRecord">The root record</param>
        /// <param name="parentRecord">The parent record</param>
        /// <returns>The list of UPCRMRecord</returns>
        public List<UPCRMRecord> ChangedChildRecordsForRootRecordParentRecord(
            UPCRMRecord rootRecord,
            UPCRMRecord parentRecord)
        {
            this.count = this.SerialEntry.Columns.Count;

            if (!string.IsNullOrWhiteSpace(this.SerialEntryRecordId) && this.SerialEntryRecordId != IdNew)
            {
                this.destinationRootRecord =
                    new UPCRMRecordWithParameter(this.SerialEntry.DestInfoAreaId, this.SerialEntryRecordId);
                this.DestinationRootRecord = this.destinationRootRecord;
            }
            else if (this.DestinationRootRecord == null)
            {
                this.ProcessDestinationRootRecordNull(rootRecord, parentRecord);
            }
            else if (this.DestinationRootRecord is UPCRMRecordWithParameter)
            {
                this.destinationRootRecord = (UPCRMRecordWithParameter)this.DestinationRootRecord;
                if (this.destinationRootRecord.IsNew && this.SerialEntry.DestinationTemplateFilter != null)
                {
                    this.additionalDestinationTemplateFilterRecords =
                        this.destinationRootRecord.ApplyValuesFromTemplateFilter(
                            this.SerialEntry.DestinationTemplateFilter,
                            false);
                }
            }

            if (this.SerialEntry.ChildrenCount != 0)
            {
                this.destinationChildRecords = new List<UPCRMRecordWithParameter>(this.SerialEntry.ChildrenCount);
                this.tentativeChildRecords = new List<UPCRMRecordWithParameter>(this.SerialEntry.ChildrenCount);
                for (var childIndex = 0; childIndex < this.SerialEntry.ChildrenCount; childIndex++)
                {
                    this.destinationChildRecords.Add(null);
                    this.tentativeChildRecords.Add(null);
                }
            }

            this.ProcessSerialEntryColumns(rootRecord);
            return this.ProcessDestinations();
        }

        private void ProcessDestinationRootRecordNull(UPCRMRecord rootRecord, UPCRMRecord parentRecord)
        {
            this.destinationRootRecord = new UPCRMRecordWithParameter(this.SerialEntry.DestInfoAreaId);
            this.DestinationRootRecord = this.destinationRootRecord;
            if (this.CreateUnchanged)
            {
                this.destinationRootRecord.HasValues = true;
            }

            if (this.SerialEntry.DestinationTemplateFilter != null)
            {
                this.additionalDestinationTemplateFilterRecords =
                    this.destinationRootRecord.ApplyValuesFromTemplateFilter(this.SerialEntry.DestinationTemplateFilter, false);
            }

            if (rootRecord != null)
            {
                this.destinationRootRecord.AddLink(new UPCRMLink(rootRecord, -1));
            }

            this.destinationRootRecord.AddLink(new UPCRMLink(this.RowRecordIdentification, -1));
            if (parentRecord != null)
            {
                this.destinationRootRecord.AddLink(new UPCRMLink(parentRecord, -1));
            }

            if (!string.IsNullOrWhiteSpace(this.ListingRecordId))
            {
                this.destinationRootRecord.AddLink(new UPCRMLink(
                    this.SerialEntry.ListingController.InfoAreaId,
                    this.ListingRecordId,
                    -1));
            }

            var sourceColumnWithLink = true;
            if (this.SerialEntry.SourceColumnsForFunction != null)
            {
                foreach (var key in this.SerialEntry.SourceColumnsForFunction.Keys)
                {
                    if (key.StartsWith("Copy"))
                    {
                        var sourceColumn = this.SerialEntry.SourceColumnsForFunction.ValueOrDefault(key);
                        var destColumn =
                            this.SerialEntry.DestColumnsForFunction.ValueOrDefault(key) as UPSEDestinationColumn;
                        if (destColumn != null &&
                            string.IsNullOrEmpty(destColumn.InitialValue) &&
                            destColumn.InfoAreaId == this.destinationRootRecord.InfoAreaId)
                        {
                            this.destinationRootRecord.NewValueFieldId(
                                (string)this.ValueAtIndex(sourceColumn.PositionInControl),
                                destColumn.FieldId);
                            if (key.Length == 4)
                            {
                                sourceColumnWithLink = false;
                            }
                        }
                    }
                }
            }

            this.ProcessSerialEntryColumns(sourceColumnWithLink);
        }

        private void ProcessSerialEntryColumns(bool sourceColumnWithLink)
        {
            foreach (var column in this.SerialEntry.Columns)
            {
                if (string.IsNullOrEmpty(column.InitialValue) || !(column is UPSEDestinationColumnBase))
                {
                    continue;
                }

                sourceColumnWithLink = false;
                var destinationColumn = (UPSEDestinationColumnBase)column;
                if (destinationColumn.InitialValue.StartsWith("Copy:"))
                {
                    var sourceColumn =
                        this.SerialEntry.ColumnForFunctionName(destinationColumn.InitialValue.Substring(5));
                    if (sourceColumn != null)
                    {
                        this.destinationRootRecord.NewValueFieldId(
                            (string)this.ValueAtIndex(sourceColumn.PositionInControl),
                            destinationColumn.FieldId);
                    }
                }
                else if (destinationColumn.InitialValue.StartsWith("$"))
                {
                    var fieldValue =
                        this.SerialEntry.InitialFieldValuesForDestination.ValueOrDefault(
                            destinationColumn.InitialValue
                            .Substring(1)) as string;
                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        var replacements =
                            ServerSession.CurrentSession.SessionParameterReplacements[destinationColumn.InitialValue];
                        if (replacements.Count >= 1)
                        {
                            fieldValue = replacements[0];
                        }
                    }

                    if (string.IsNullOrEmpty(fieldValue))
                    {
                        this.destinationRootRecord.NewValueFieldId(fieldValue, destinationColumn.FieldId);
                    }
                }
                else if (destinationColumn.InitialValue.StartsWith("Value:"))
                {
                    this.destinationRootRecord.NewValueFieldId(
                        destinationColumn.InitialValue.Substring(6),
                        destinationColumn.FieldId);
                }
            }

            if (sourceColumnWithLink)
            {
                this.DestinationRootRecord.AddLink(new UPCRMLink(this.SerialEntry.SourceInfoAreaId, this.RowRecordId, -1));
            }
        }

        private void ProcessSerialEntryColumns(UPCRMRecord rootRecord)
        {
            for (var columnIndex = 0; columnIndex < this.count; columnIndex++)
            {
                this.column = this.SerialEntry.Columns[columnIndex];
                this.destinationColumn = this.column as UPSEDestinationColumnBase;
                if (this.destinationColumn != null)
                {
                    if (this.destinationColumn.DontCacheOffline && this.destinationColumn.DontSave)
                    {
                        continue;
                    }

                    this.rowValue = this.RowValues[columnIndex];
                    var originalVal = this.destinationColumn.StringValueFromObject(this.rowValue.OriginalValue);
                    var stringValue = this.destinationColumn.StringValueFromObject(this.rowValue.Value);
                    var changedValue = this.column.IsValueDifferentThan(stringValue, originalVal);
                    if (!changedValue && !this.rowValue.SaveUnchanged)
                    {
                        continue;
                    }

                    if (!changedValue)
                    {
                        originalVal = null;
                    }

                    if (this.column.ColumnFrom == UPSEColumnFrom.DestChild)
                    {
                        this.ProcessUpseColumnFromDestChild(rootRecord, changedValue, stringValue, originalVal);
                    }
                    else
                    {
                        if (this.destinationColumn.DontSave)
                        {
                            this.destinationRootRecord.NewValueFromValueFieldIdOnlyOffline(
                                stringValue,
                                originalVal,
                                this.column.FieldId,
                                true);
                        }
                        else
                        {
                            this.destinationRootRecord.NewValueFromValueFieldId(stringValue, originalVal, this.column.FieldId);
                        }

                        if (changedValue)
                        {
                            this.destinationRootRecord.HasValues = true;
                        }
                    }
                }
            }
        }

        private void ProcessUpseColumnFromDestChild(
            UPCRMRecord rootRecord,
            bool changedValue,
            string stringValue,
            string origVal)
        {
            var childIndex = ((UPSEDestinationChildColumn)this.column).ChildIndex;
            object childRecordObject = this.tentativeChildRecords[childIndex];
            UPCRMRecordWithParameter childRecord;
            if (childRecordObject == null)
            {
                var childRecordId = string.Empty;
                if (this.ChildRecordIds != null)
                {
                    childRecordId = this.ChildRecordIds[childIndex];
                }

                if (string.IsNullOrWhiteSpace(childRecordId))
                {
                    childRecord = new UPCRMRecordWithParameter(this.SerialEntry.DestChildInfoAreaId, null);
                    if (this.SerialEntry.DestinationChildTemplateFilter != null)
                    {
                        childRecord.ApplyValuesFromTemplateFilter(
                            this.SerialEntry.DestinationChildTemplateFilter);
                    }

                    childRecord.AddLink(new UPCRMLink(this.DestinationRootRecord, -1));
                    var sourceChild = this.SerialEntry.SourceChildren[childIndex];
                    childRecord.AddLink(new UPCRMLink(sourceChild.Record, -1));
                    if (rootRecord != null)
                    {
                        childRecord.AddLink(new UPCRMLink(rootRecord, -1));
                    }
                }
                else
                {
                    childRecord =
                        new UPCRMRecordWithParameter(this.SerialEntry.DestChildInfoAreaId, childRecordId);
                }

                this.tentativeChildRecords[childIndex] = childRecord;
            }
            else
            {
                childRecord = (UPCRMRecordWithParameter)childRecordObject;
            }

            if (changedValue && this.destinationChildRecords[childIndex] == null)
            {
                if (childRecord.IsNew)
                {
                    ++this.newChildRecordCount;
                }
                else
                {
                    ++this.modifiedChildRecordCount;
                }

                this.destinationChildRecords[childIndex] = childRecord;
            }

            if (this.destinationColumn.DontSave)
            {
                childRecord.NewValueFromValueFieldIdOnlyOffline(stringValue, origVal, this.column.FieldId, true);
            }
            else
            {
                childRecord.NewValueFromValueFieldId(stringValue, origVal, this.column.FieldId);
            }

            if (changedValue && !string.IsNullOrEmpty(stringValue))
            {
                childRecord.HasValues = true;
            }
        }

        private List<UPCRMRecord> ProcessDestinations()
        {
            if (this.destinationRootRecord.HasValues ||
                this.additionalDestinationTemplateFilterRecords?.Count > 0 ||
                (this.newChildRecordCount != 0 && this.DestinationRootRecord.IsNew))
            {
                this.changedRecords = new List<UPCRMRecord>(this.newChildRecordCount + this.modifiedChildRecordCount + 1)
                {
                    this.DestinationRootRecord
                };
                if (this.additionalDestinationTemplateFilterRecords?.Count > 0)
                {
                    this.changedRecords.AddRange(this.additionalDestinationTemplateFilterRecords);
                }
            }
            else
            {
                this.changedRecords = new List<UPCRMRecord>(this.newChildRecordCount + this.modifiedChildRecordCount);
            }

            if (this.newChildRecordCount != 0 || this.modifiedChildRecordCount != 0)
            {
                foreach (var destinationChildRecordPointer in this.destinationChildRecords)
                {
                    if (destinationChildRecordPointer == null)
                    {
                        continue;
                    }

                    var destinationChildRecord = destinationChildRecordPointer;
                    if (!destinationChildRecord.HasValues)
                    {
                        if (!destinationChildRecord.IsNew)
                        {
                            destinationChildRecord.Deleted = true;
                            this.changedRecords.Add(destinationChildRecord);
                        }
                    }
                    else
                    {
                        this.changedRecords.Add(destinationChildRecord);
                    }
                }

                if (this.SerialEntry.SyncRowAfterChildren)
                {
                    this.changedRecords.Add(new UPCRMRecord(this.DestinationRootRecord, "Sync"));
                }
            }

            if (this.RowQuota != null)
            {
                this.RowQuota.CurrentCount = this.SumForDestinationColumns(this.SerialEntry.Quota.DestinationColumns);
            }

            return this.changedRecords;
        }

        private int SumForDestinationColumns(IEnumerable<UPSEColumn> columns) =>
            columns.Sum(column => this.IntegerValueFromColumn(column));

        private int IntegerValueFromColumn(UPSEColumn column)
        {
            if (column == null)
            {
                return 0;
            }

            var valueAtIndex = this.ValueAtIndex(column.Index);
            return Convert.ToInt32(valueAtIndex);
        }

        private object ValueAtIndex(int index) => this.RowValues[index].Value;
    }
}
