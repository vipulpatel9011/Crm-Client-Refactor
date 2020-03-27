// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MDocument.cs" company="Aurea Software Gmbh">
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
//   UI control for showing a document field value
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.UIModel
{
    using System;

    using Aurea.CRM.Core.CRM.Features;
    using Aurea.CRM.Core.CRM.UIModel;
    using Aurea.CRM.UIModel.Fields;
    using Aurea.CRM.UIModel.Identifiers;

    /// <summary>
    /// UI control for showing a document field value
    /// </summary>
    /// <seealso cref="UPMField" />
    public class UPMDocument : UPMField, IUPMDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDocument"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="dateText">
        /// The date text.
        /// </param>
        /// <param name="sizeText">
        /// The size text.
        /// </param>
        /// <param name="iconName">
        /// Name of the icon.
        /// </param>
        /// <param name="url">
        /// The URL.
        /// </param>
        /// <param name="localFileName">
        /// Name of the local file.
        /// </param>
        /// <param name="modificationDate">
        /// The modification date.
        /// </param>
        /// <param name="displayText">
        /// The display text.
        /// </param>
        /// <param name="d1Url">
        /// The d1 URL.
        /// </param>
        public UPMDocument(
            IIdentifier identifier,
            string label,
            string dateText,
            string sizeText,
            string iconName,
            Uri url,
            string localFileName,
            DateTime? modificationDate,
            string displayText,
            Uri d1Url)
            : base(identifier)
        {
            this.LabelText = label;
            this.DisplayText = displayText;
            this.SizeText = sizeText;
            this.DateText = dateText;
            this.IconName = iconName;
            this.Url = url;
            this.LocalFileName = localFileName;
            this.ModificationDate = modificationDate;
            this.D1Url = d1Url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDocument"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="url">
        /// The URL.
        /// </param>
        public UPMDocument(IIdentifier identifier, Uri url)
            : this(identifier, null, null, null, null, url, null, DateTime.MinValue, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDocument"/> class.
        /// </summary>
        /// <param name="document">
        /// The document.
        /// </param>
        public UPMDocument(DocumentData document)
            : this(
                new RecordIdentifier(document.RecordIdentification),
                document.Title,
                document.DateString,
                document.SizeString,
                document.IconName,
                document.Url,
                document.Title,
                document.ServerUpdateDate,
                document.DisplayText,
                document.UrlForD1RecordId())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPMDocument"/> class.
        /// </summary>
        /// <param name="addPhotoDirectButtonName">
        /// Name of the add photo direct button.
        /// </param>
        /// <param name="documentsIndex">
        /// Index of the documents.
        /// </param>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        public UPMDocument(string addPhotoDirectButtonName, int documentsIndex, string recordIdentification)
            : base(StringIdentifier.IdentifierWithStringId("AddPhoto"))
        {
            this.AddPhotoDirectButtonName = addPhotoDirectButtonName;
            this.DocumentsIndex = documentsIndex;
            this.RecordIdentification = recordIdentification;
        }

        /// <summary>
        /// Gets a value indicating whether [add photo button].
        /// Dieses Document ist ein add-Button, wenn der ButtonName gesetzt ist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [add photo button]; otherwise, <c>false</c>.
        /// </value>
        public bool AddPhotoButton => !string.IsNullOrEmpty(this.AddPhotoDirectButtonName);

        /// <summary>
        /// Gets the name of the add photo direct button.
        /// </summary>
        /// <value>
        /// The name of the add photo direct button.
        /// </value>
        public string AddPhotoDirectButtonName { get; private set; }

        /// <summary>
        /// Gets or sets the d1 URL.
        /// </summary>
        /// <value>
        /// The d1 URL.
        /// </value>
        public Uri D1Url { get; set; }

        /// <summary>
        /// Gets or sets the date text.
        /// </summary>
        /// <value>
        /// The date text.
        /// </value>
        public string DateText { get; set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets the index of the documents.
        /// </summary>
        /// <value>
        /// The index of the documents.
        /// </value>
        public int DocumentsIndex { get; private set; }

        /// <summary>
        /// Gets or sets the email fieldgroup.
        /// </summary>
        /// <value>
        /// The email fieldgroup.
        /// </value>
        public string EmailFieldgroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the icon.
        /// </summary>
        /// <value>
        /// The name of the icon.
        /// </value>
        public string IconName { get; set; }

        /// <summary>
        /// Gets or sets the linked record identifier.
        /// </summary>
        /// <value>
        /// The linked record identifier.
        /// </value>
        public string LinkedRecordId { get; set; }

        /// <summary>
        /// Gets or sets the name of the local file.
        /// </summary>
        /// <value>
        /// The name of the local file.
        /// </value>
        public string LocalFileName { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        public DateTime? ModificationDate { get; set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification { get; private set; }

        /// <summary>
        /// Gets or sets the size text.
        /// </summary>
        /// <value>
        /// The size text.
        /// </value>
        public string SizeText { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
            => $"[{base.ToString()}, url: {this.Url}, localFileName: {this.LocalFileName}]";
    }
}
