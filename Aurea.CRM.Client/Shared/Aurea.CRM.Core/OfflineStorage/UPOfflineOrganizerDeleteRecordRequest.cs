// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineOrganizerDeleteRecordRequest.cs" company="Aurea Software Gmbh">
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
//   The Offline Organizer Delete Record Request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    /// <summary>
    /// Offline Organizer Delete Record Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineEditRecordRequest" />
    public class UPOfflineOrganizerDeleteRecordRequest : UPOfflineEditRecordRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineOrganizerDeleteRecordRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineOrganizerDeleteRecordRequest(int requestNr)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.DeleteRecord;

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
        public override string DefaultTitleLine => "Delete Record";
    }
}
