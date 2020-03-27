// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPSEPOS.cs" company="Aurea Software Gmbh">
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
//   Serial Entry POS
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel.SerialEntry
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Configuration;
    using Aurea.CRM.Core.CRM.DataModel;
    using Aurea.CRM.Core.OfflineStorage;

    /// <summary>
    /// Serial Entry POS
    /// </summary>
    /// <seealso cref="Aurea.CRM.UIModel.SerialEntry.UPSerialEntry" />
    public class UPSEPOS : UPSerialEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntry"/> class.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSEPOS(UPCRMRecord rootRecord, Dictionary<string, object> parameters, UPSerialEntryDelegate theDelegate)
            : base(rootRecord, parameters, theDelegate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPSerialEntry"/> class.
        /// </summary>
        /// <param name="rootRecord">The root record.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="offlineRequest">The offline request.</param>
        /// <param name="theDelegate">The delegate.</param>
        public UPSEPOS(UPCRMRecord rootRecord, Dictionary<string, object> parameters, UPOfflineSerialEntryRequest offlineRequest,
            UPSerialEntryDelegate theDelegate)
            : base(rootRecord, parameters, offlineRequest, theDelegate)
        {
        }

        /// <summary>
        /// Gets the offline request.
        /// </summary>
        /// <value>
        /// The offline request.
        /// </value>
        public override UPOfflineSerialEntryRequest OfflineRequest
        {
            get
            {
                if (this.offlineRequest == null)
                {
                    ViewReference viewReference = this.Parameters["viewReference"] as ViewReference;
                    this.offlineRequest = new UPOfflineSerialEntryPOSRequest(viewReference);
                }

                if (!this.ConflictHandling)
                {
                    this.offlineRequest.RelatedInfoDictionary = this.InitialFieldValuesForDestination;
                    UPOfflineStorage.DefaultStorage.BlockingRequest = this.offlineRequest;
                }

                return this.offlineRequest;
            }
        }

        /// <summary>
        /// Rows from source result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="listing">The listing.</param>
        /// <returns></returns>
        public override UPSERow RowFromSourceResultRow(UPCRMResultRow row, UPSEListing listing)
        {
            return new UPSEPOSRow(row, listing, this);
        }

        /// <summary>
        /// Rows from destination result row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="sourceFieldOffset">The source field offset.</param>
        /// <returns></returns>
        public override UPSERow RowFromDestinationResultRow(UPCRMResultRow row, int offset, int sourceFieldOffset)
        {
            return new UPSEPOSRow(row, offset, sourceFieldOffset, this);
        }

        /// <summary>
        /// Rows from source row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public override UPSERow RowFromSourceRow(UPSERow row)
        {
            return new UPSEPOSRow(row);
        }
    }
}
