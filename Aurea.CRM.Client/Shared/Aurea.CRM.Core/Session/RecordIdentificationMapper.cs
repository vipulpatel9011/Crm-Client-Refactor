// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordIdentificationMapper.cs" company="Aurea Software Gmbh">
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
//   Implements a record identification mapper
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.Session
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Implements a record identification mapper
    /// </summary>
    public class UPRecordIdentificationMapper
    {
        /// <summary>
        /// The mapped record identifications.
        /// </summary>
        private readonly Dictionary<string, string> mappedRecordIdentifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="UPRecordIdentificationMapper"/> class.
        /// </summary>
        public UPRecordIdentificationMapper()
        {
            this.mappedRecordIdentifications = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds the new record identification old record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="oldRecordIdentification">
        /// The old record identification.
        /// </param>
        public void AddNewRecordIdentification(string recordIdentification, string oldRecordIdentification)
        {
            if (string.IsNullOrEmpty(oldRecordIdentification) || string.IsNullOrEmpty(recordIdentification))
            {
                return;
            }

            this.mappedRecordIdentifications[oldRecordIdentification] = recordIdentification;

            HistoryManager.DefaultHistoryManager.UpdateHistoryEntry(oldRecordIdentification, recordIdentification);
        }

        /// <summary>
        /// Mappeds the record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string MappedRecordIdentification(string recordIdentification)
        {
            if (recordIdentification == null)
            {
                return null;
            }

            var mapped = this.mappedRecordIdentifications.ValueOrDefault(recordIdentification);
            return mapped ?? recordIdentification;
        }
    }
}
