// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineCopyRecordsRequest.cs" company="Aurea Software Gmbh">
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
//   Offline Copy Records request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    /// <summary>
    /// Offline Copy Records Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRecordRequest" />
    public class UPOfflineCopyRecordsRequest : UPOfflineRecordRequest
    {
        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.CopyRecords;

        /// <summary>
        /// Gets a value indicating whether [fixable by user].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixable by user]; otherwise, <c>false</c>.
        /// </value>
        public override bool FixableByUser => false;

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public override string DefaultTitleLine => "Copy Records";

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineCopyRecordsRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineCopyRecordsRequest(int requestNr)
            : base(requestNr)
        {
        }
    }
}
