// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentManager.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Document manager class implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.Features
{
    using System.Collections.Generic;
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Document manager class implementation.
    /// </summary>
    public class DocumentManager
    {
        /// <summary>
        /// The document info area dictionary.
        /// </summary>
        protected Dictionary<string, DocumentInfoAreaManager> documentInfoAreaDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentManager"/> class.
        /// Creates a new Instance of <see cref="DocumentManager"/>
        /// </summary>
        public DocumentManager()
        {
            this.documentInfoAreaDictionary = new Dictionary<string, DocumentInfoAreaManager>();
        }

        /// <summary>
        /// The document for key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentData"/>.
        /// </returns>
        public DocumentData DocumentForKey(string key)
        {
            var recordIdentification = key.DocumentKeyToRecordIdentification();
            if (!string.IsNullOrEmpty(recordIdentification))
            {
                return this.DocumentDataForRecordIdentification(recordIdentification);
            }

            return null;
        }

        /// <summary>
        /// The document data for record identification.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentData"/>.
        /// </returns>
        public DocumentData DocumentDataForRecordIdentification(string recordIdentification)
        {
            var infoAreaManager = this.DocumentManagerForInfoAreaId(recordIdentification.InfoAreaId());
            return infoAreaManager?.DocumentDataForRecordId(recordIdentification.RecordId());
        }

        /// <summary>
        /// The document manager for info area id.
        /// </summary>
        /// <param name="infoAreaId">
        /// The info area id.
        /// </param>
        /// <returns>
        /// The <see cref="DocumentInfoAreaManager"/>.
        /// </returns>
        private DocumentInfoAreaManager DocumentManagerForInfoAreaId(string infoAreaId)
        {
            DocumentInfoAreaManager infoAreaManager = this.documentInfoAreaDictionary.ValueOrDefault(infoAreaId);
            if (infoAreaManager == null)
            {
                infoAreaManager = new DocumentInfoAreaManager(infoAreaId, this);
                this.documentInfoAreaDictionary[infoAreaId] = infoAreaManager;
            }

            return infoAreaManager;
        }
    }
}
