// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineSerialEntryRequest.cs" company="Aurea Software Gmbh">
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
//  Offline Serial Entry Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using System;
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;

    /// <summary>
    /// Offline Serial Entry Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRecordRequest" />
    public class UPOfflineSerialEntryRequest : UPOfflineRecordRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineSerialEntryRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineSerialEntryRequest(int requestNr)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineSerialEntryRequest"/> class.
        /// </summary>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineSerialEntryRequest(ViewReference viewReference)
            : base(viewReference)
        {
        }

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.SerialEntry;

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public override string DefaultTitleLine => "Serial Entry";

        /// <summary>
        /// Gets a value indicating whether [fixable by user].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixable by user]; otherwise, <c>false</c>.
        /// </value>
        public override bool FixableByUser => true;

        /// <summary>
        /// Applies the changes to serial entry.
        /// </summary>
        /// <param name="serialEntry">The serial entry.</param>
        /// <returns></returns>
        public UPOfflineSerialEntryApplyResult ApplyChangesToSerialEntry(/*UPSerialEntry*/ dynamic serialEntry)
        {
            UPOfflineSerialEntryApplyResult result = new UPOfflineSerialEntryApplyResult();
            bool ok = true;
            bool rowCreated;
            List<UPCRMRecordWithHierarchy> recordArray = new List<UPCRMRecordWithHierarchy>();

            foreach (UPCRMRecordWithHierarchy record in this.RecordStructure())
            {
                if (record.InfoAreaId == serialEntry.DestInfoAreaId)
                {
                    recordArray.Add(record);
                }

                if (record.InfoAreaId == serialEntry.Record.InfoAreaId)
                {
                    foreach (UPCRMRecordWithHierarchy subRecord in record.Children)
                    {
                        if (subRecord.InfoAreaId == serialEntry.DestInfoAreaId)
                        {
                            recordArray.Add(subRecord);
                        }
                    }
                }
            }

            foreach (UPCRMRecordWithHierarchy record in recordArray)
            {
                /*UPSERow*/
                dynamic row = serialEntry.ExistingRowForRecordIdentification(record.RecordIdentification);
                if (row == null)
                {
                    UPCRMLink link = record.LinkWithInfoAreaIdLinkId(serialEntry.SourceInfoAreaId, -1);
                    if (link == null)
                    {
                        continue;
                    }

                    row = serialEntry.CreateRowForSourceRecordIdentification(link.RecordIdentification);
                    rowCreated = true;
                }
                else
                {
                    rowCreated = false;
                }

                if (row != null)
                {
                    bool recordsApplied = row.ApplyRecordHierarchy(record);
                    if (!recordsApplied)
                    {
                        ok = false;
                    }
                    else if (rowCreated)
                    {
                        serialEntry.AddPosition(row);
                    }
                }
            }

            if (!ok)
            {
                result.Error = new Exception("serial entry conflict handling was not implemented yet");
            }

            return result;
        }
    }
}
