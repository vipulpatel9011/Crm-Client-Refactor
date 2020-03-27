// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmRecordSyncResult.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <author>
//    Rashan Anushka
// </author>
// <summary>
//   CRM record sync Result
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Sync
{
    /// <summary>
    /// CRM record sync result
    /// </summary>
    public class UPCRMRecordSyncResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMRecordSyncResult"/> class.
        /// </summary>
        /// <param name="returnCode">
        /// The return code.
        /// </param>
        /// <param name="recordCount">
        /// The record count.
        /// </param>
        /// <param name="appendResult">
        /// The append result.
        /// </param>
        public UPCRMRecordSyncResult(int returnCode, int recordCount, UPCRMRecordSyncResult appendResult)
        {
            if (returnCode > 0)
            {
                this.ReturnCode = returnCode;
            }
            else if (appendResult != null)
            {
                this.ReturnCode = appendResult.ReturnCode;
            }
            else
            {
                this.ReturnCode = 0;
            }

            this.RecordCount = recordCount + (appendResult?.RecordCount ?? 0);
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <value>
        /// The record count.
        /// </value>
        public int RecordCount { get; private set; }

        /// <summary>
        /// Gets the return code.
        /// </summary>
        /// <value>
        /// The return code.
        /// </value>
        public int ReturnCode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="UPCRMRecordSyncResult"/> is successful.
        /// </summary>
        /// <value>
        /// <c>true</c> if successful; otherwise, <c>false</c>.
        /// </value>
        public bool Successful => this.ReturnCode == 0;

        /// <summary>
        /// Results the with error code.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public static UPCRMRecordSyncResult ResultWithErrorCode(int errorCode)
        {
            return new UPCRMRecordSyncResult(errorCode, 0, null);
        }

        /// <summary>
        /// Successes the result with record count.
        /// </summary>
        /// <param name="recordCount">
        /// The record count.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public static UPCRMRecordSyncResult SuccessResultWithRecordCount(int recordCount)
        {
            return new UPCRMRecordSyncResult(0, recordCount, null);
        }

        /// <summary>
        /// Results the by appending result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="UPCRMRecordSyncResult"/>.
        /// </returns>
        public UPCRMRecordSyncResult ResultByAppendingResult(UPCRMRecordSyncResult result)
        {
            return new UPCRMRecordSyncResult(this.ReturnCode, this.RecordCount, result);
        }
    }
}
