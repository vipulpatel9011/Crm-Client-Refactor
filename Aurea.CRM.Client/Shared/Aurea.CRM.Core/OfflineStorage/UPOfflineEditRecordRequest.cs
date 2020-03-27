// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UPOfflineEditRecordRequest.cs" company="Aurea Software Gmbh">
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
//   Offline edit record request
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.OfflineStorage
{
    using Aurea.CRM.Core.Configuration;

    /// <summary>
    /// Offline Edit Record Request
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.OfflineStorage.UPOfflineRecordRequest" />
    public class UPOfflineEditRecordRequest : UPOfflineRecordRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineEditRecordRequest"/> class.
        /// </summary>
        /// <param name="requestNr">The request nr.</param>
        public UPOfflineEditRecordRequest(int requestNr)
            : base(requestNr)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPOfflineEditRecordRequest"/> class.
        /// </summary>
        /// <param name="requestMode">The request mode.</param>
        /// <param name="viewReference">The view reference.</param>
        public UPOfflineEditRecordRequest(UPOfflineRequestMode requestMode, ViewReference viewReference)
            : base(requestMode, viewReference)
        {
        }

        /// <summary>
        /// Gets the type of the process.
        /// </summary>
        /// <value>
        /// The type of the process.
        /// </value>
        public override OfflineRequestProcess ProcessType => OfflineRequestProcess.EditRecord;

        /// <summary>
        /// Gets a value indicating whether [fixable by user].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [fixable by user]; otherwise, <c>false</c>.
        /// </value>
        public override bool FixableByUser => true;

        /// <summary>
        /// Gets the default title line.
        /// </summary>
        /// <value>
        /// The default title line.
        /// </value>
        public override string DefaultTitleLine => "New/Edit Record";
    }
}
