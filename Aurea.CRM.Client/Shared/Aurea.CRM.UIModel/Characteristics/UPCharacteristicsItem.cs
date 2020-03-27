// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPCharacteristicsItem.cs" company="Aurea Software Gmbh">
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
//   UPCharacteristicsItem
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.Characteristics
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.UIModel.Groups;

    /// <summary>
    /// UPCharacteristicsItem
    /// </summary>
    public class UPCharacteristicsItem
    {
        private List<string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCharacteristicsItem"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="catalogValue">The catalog value.</param>
        /// <param name="group">The group.</param>
        /// <param name="additionalFields">The additional fields.</param>
        public UPCharacteristicsItem(string label, string catalogValue, UPCharacteristicsGroup group, List<UPConfigFieldControlField> additionalFields)
        {
            this.Label = label;
            this.CatalogValue = catalogValue;
            this.AdditionalFields = additionalFields;
            int count = this.AdditionalFields?.Count ?? 0;
            if (count > 0)
            {
                this.values = new List<string>(additionalFields.Count);
                for (int i = 0; i < count; i++)
                {
                    this.values.Add(string.Empty);
                }
            }

            this.Group = group;
            this.Deleted = false;
            this.Changed = false;
            this.Created = false;
            this.ShowAdditionalFields = this.AdditionalFields?.Count > 0;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public List<string> Values => this.values;

        /// <summary>
        /// Gets the group catalog value.
        /// </summary>
        /// <value>
        /// The group catalog value.
        /// </value>
        public string GroupCatalogValue => this.Group.CatalogValue;

        /// <summary>
        /// Gets the additional fields.
        /// </summary>
        /// <value>
        /// The additional fields.
        /// </value>
        public List<UPConfigFieldControlField> AdditionalFields { get; private set; }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Gets the committed text value.
        /// </summary>
        /// <value>
        /// The committed text value.
        /// </value>
        public string CommittedTextValue { get; private set; }

        /// <summary>
        /// Gets the text value.
        /// </summary>
        /// <value>
        /// The text value.
        /// </value>
        public string TextValue { get; private set; }

        /// <summary>
        /// Gets the catalog value.
        /// </summary>
        /// <value>
        /// The catalog value.
        /// </value>
        public string CatalogValue { get; private set; }

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>
        /// The label.
        /// </value>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public UPCharacteristicsGroup Group { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCharacteristicsItem" /> is changed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if changed; otherwise, <c>false</c>.
        /// </value>
        public bool Changed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPCharacteristicsItem"/> is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if deleted; otherwise, <c>false</c>.
        /// </value>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UPCharacteristicsItem"/> is created.
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
        /// Gets a value indicating whether [show additional fields].
        /// </summary>
        /// <value>
        /// <c>true</c> if [show additional fields]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAdditionalFields { get; private set; }

        /// <summary>
        /// Sets from record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void SetFromRecord(UPCRMRecord record)
        {
            if (record != null)
            {
                this.Record = record;
                if (this.AdditionalFields?.Count > 0)
                {
                    this.values = new List<string>(this.AdditionalFields.Count);
                    if (this.Group.Characteristics.EditMode == false)
                    {
                        foreach (UPConfigFieldControlField field in this.AdditionalFields)
                        {
                            string value = record.StringFieldValueForFieldIndex(field.TabIndependentFieldIndex);
                            this.values.Add(field.IsEmptyValue(value) ? string.Empty : record.StringFieldValueForFieldIndex(field.TabIndependentFieldIndex));
                        }
                    }
                    else
                    {
                        foreach (UPConfigFieldControlField field in this.AdditionalFields)
                        {
                            bool found = false;
                            foreach (UPCRMFieldValue fieldValue in record.FieldValues)
                            {
                                if (fieldValue.InfoAreaId == field.InfoAreaId && fieldValue.FieldId == field.FieldId)
                                {
                                    if (fieldValue.Value != null)
                                    {
                                        found = true;
                                        this.values.Add(fieldValue.Value);
                                        break;
                                    }
                                }
                            }

                            if (!found)
                            {
                                this.values.Add(string.Empty);
                            }
                        }
                    }
                }

                this.OriginalValues = new List<string>(this.values);
            }
            else
            {
                this.Record = null;
                if (this.AdditionalFields?.Count > 0)
                {
                    this.values = new List<string>(this.AdditionalFields.Count);
                    for (int i = 0; i < this.AdditionalFields.Count; i++)
                    {
                        this.values.Add(string.Empty);
                    }
                }

                this.OriginalValues = null;
            }

            this.Created = this.Record.IsNew;
            this.Deleted = this.Record.Deleted;
            this.Changed = false;
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
                this.values = new List<string>();

                if (this.AdditionalFields?.Count > 0)
                {
                    if (this.Group.Characteristics.EditMode == false)
                    {
                        foreach (UPConfigFieldControlField field in this.AdditionalFields)
                        {
                            string value = row.RawValueAtIndex(field.TabIndependentFieldIndex);
                            this.values.Add(field.IsEmptyValue(value) ? string.Empty : row.ValueAtIndex(field.TabIndependentFieldIndex));
                        }
                    }
                    else
                    {
                        foreach (UPConfigFieldControlField field in this.AdditionalFields)
                        {
                            this.values.Add(row.RawValueAtIndex(field.TabIndependentFieldIndex));
                        }
                    }
                }

                this.OriginalValues = new List<string>(this.values);
            }
            else
            {
                this.Record = null;
                if (this.AdditionalFields?.Count > 0)
                {
                    this.values = new List<string>();
                    for (int i = 0; i < this.AdditionalFields.Count; i++)
                    {
                        this.values.Add(string.Empty);
                    }
                }

                this.OriginalValues = null;
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
                if (rawValue == null)
                {
                    rawValue = string.Empty;
                }

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
        /// Marks the item as set.
        /// </summary>
        public void MarkItemAsSet()
        {
            if (this.Record == null)
            {
                this.Created = true;
            }

            this.Deleted = false;
        }

        /// <summary>
        /// Marks the item as unset.
        /// </summary>
        public void MarkItemAsUnset()
        {
            if (this.Record != null)
            {
                this.Deleted = true;
            }
            else
            {
                this.Created = false;
                this.Changed = false;
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
                if (this.Deleted && this.Created)
                {
                    // This happens during conflict handling. Saved offline with new RecordId and in conflict handling remove again.
                    // Remove from crmdata only.
                    this.Record.Deleted = true;
                    changedRecord = this.Record;
                }
                else
                {
                    changedRecord = new UPCRMRecord(this.Record.RecordIdentification, "Delete", null);
                }
            }
            else
            {
                if (this.Created)
                {
                    changedRecord = this.Record == null ? UPCRMRecord.CreateNew(this.Group.Characteristics.DestinationFieldControl.InfoAreaId)
                        : new UPCRMRecord(this.Record.RecordIdentification);

                    UPCharacteristics characteristics = this.Group.Characteristics;
                    changedRecord.AddLink(new UPCRMLink(characteristics.RecordIdentification));
                    changedRecord.NewValueFieldId(this.Group.CatalogValue, characteristics.DestinationGroupField.FieldId);
                    changedRecord.NewValueFieldId(this.CatalogValue, characteristics.DestinationItemField.FieldId);

                    for (int i = 0; i < this.AdditionalFields?.Count; i++)
                    {
                        string newValue = this.values[i];
                        if (!string.IsNullOrEmpty(newValue))
                        {
                            UPConfigFieldControlField field = this.AdditionalFields[i];
                            changedRecord.NewValueFieldId(newValue, field.FieldId);
                        }
                    }
                }
                else if ((this.Changed && this.Record != null) || (this.Created && this.Record != null))
                {
                    changedRecord = new UPCRMRecord(this.Record.RecordIdentification);
                    for (int i = 0; i < this.AdditionalFields?.Count; i++)
                    {
                        string originalValue = this.OriginalValues[i];
                        string newValue = this.values[i];
                        if (originalValue != newValue)
                        {
                            UPConfigFieldControlField field = this.AdditionalFields[i];
                            changedRecord.NewValueFromValueFieldId(newValue, originalValue, field.FieldId);
                        }
                    }
                }
            }

            return changedRecord != null ? new List<UPCRMRecord> { changedRecord } : null;
        }
    }
}
