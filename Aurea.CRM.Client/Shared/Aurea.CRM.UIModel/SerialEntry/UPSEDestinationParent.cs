// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEDestinationParent.cs" company="Aurea Software Gmbh">
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
//   UPSEDestinationParent
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// UPSEDestinationParent
    /// </summary>
    public class UPSEDestinationParent
    {
        private Dictionary<string, UPSERow> positions;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationParent"/> class.
        /// </summary>
        /// <param name="sourceRecordIdentification">The source record identification.</param>
        /// <param name="destinationRecordIdentification">The destination record identification.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSEDestinationParent(string sourceRecordIdentification, string destinationRecordIdentification, UPSerialEntry serialEntry)
        {
            this.SourceRecordIdentification = sourceRecordIdentification;
            if (!string.IsNullOrEmpty(destinationRecordIdentification))
            {
                this.DestinationRecord = new UPCRMRecord(destinationRecordIdentification);
            }

            this.SerialEntry = serialEntry;
            this.positions = new Dictionary<string, UPSERow>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSEDestinationParent"/> class.
        /// </summary>
        /// <param name="sourceRecordIdentification">The source record identification.</param>
        /// <param name="serialEntry">The serial entry.</param>
        public UPSEDestinationParent(string sourceRecordIdentification, UPSerialEntry serialEntry)
            : this(sourceRecordIdentification, null, serialEntry)
        {
        }

        /// <summary>
        /// Gets the source information area identifier.
        /// </summary>
        /// <value>
        /// The source information area identifier.
        /// </value>
        public string SourceInfoAreaId => this.SourceRecordIdentification.InfoAreaId();

        /// <summary>
        /// Gets the source record identifier.
        /// </summary>
        /// <value>
        /// The source record identifier.
        /// </value>
        public string SourceRecordId => this.SourceRecordIdentification.RecordId();

        /// <summary>
        /// Gets the serial entry.
        /// </summary>
        /// <value>
        /// The serial entry.
        /// </value>
        public UPSerialEntry SerialEntry { get; private set; }

        /// <summary>
        /// Gets the source record identification.
        /// </summary>
        /// <value>
        /// The source record identification.
        /// </value>
        public string SourceRecordIdentification { get; private set; }

        /// <summary>
        /// Gets the destination record.
        /// </summary>
        /// <value>
        /// The destination record.
        /// </value>
        public UPCRMRecord DestinationRecord { get; private set; }

        /// <summary>
        /// Adds the position.
        /// </summary>
        /// <param name="row">The row.</param>
        public void AddPosition(UPSERow row)
        {
            this.positions.SetObjectForKey(row, row.RowRecordId);
        }

        /// <summary>
        /// Removes the position.
        /// </summary>
        /// <param name="row">The row.</param>
        public void RemovePosition(UPSERow row)
        {
            this.positions.Remove(row.RowRecordId);
        }

        /// <summary>
        /// Changeds the record.
        /// </summary>
        /// <returns></returns>
        public UPCRMRecord ChangedRecord()
        {
            if (this.positions.Count > 0 && this.DestinationRecord == null)
            {
                this.DestinationRecord = UPCRMRecord.CreateNew(this.SerialEntry.DestParentInfoAreaId);
                if (this.SerialEntry.DestinationParentTemplateFilter != null)
                {
                    this.DestinationRecord.ApplyValuesFromTemplateFilter(this.SerialEntry.DestinationParentTemplateFilter);
                }

                if (this.SerialEntry.DestParentEditFieldControl != null)
                {
                    foreach (FieldControlTab tab in this.SerialEntry.DestParentEditFieldControl.Tabs)
                    {
                        foreach (UPConfigFieldControlField field in tab.Fields)
                        {
                            if (field.Attributes.Dontsave && field.Attributes.DontcacheOffline)
                            {
                                continue;
                            }

                            if (string.IsNullOrEmpty(field.Function))
                            {
                                continue;
                            }

                            string val = this.SerialEntry.InitialFieldValuesForDestination.ValueOrDefault(field.Function) as string;
                            if (!string.IsNullOrEmpty(val))
                            {
                                if (field.Attributes.Dontsave)
                                {
                                    this.DestinationRecord.NewValueFromValueFieldIdOnlyOffline(val, null, field.FieldId, true);
                                }
                                else
                                {
                                    this.DestinationRecord.NewValueFromValueFieldId(val, null, field.FieldId);
                                }
                            }
                        }
                    }
                }

                this.DestinationRecord.AddLink(new UPCRMLink(this.SerialEntry.Record, -1));
                this.DestinationRecord.AddLink(new UPCRMLink(this.SourceRecordIdentification, -1));
                return this.DestinationRecord;
            }

            return string.IsNullOrEmpty(this.DestinationRecord.RecordIdentification) ? this.DestinationRecord : null;
        }
    }
}
