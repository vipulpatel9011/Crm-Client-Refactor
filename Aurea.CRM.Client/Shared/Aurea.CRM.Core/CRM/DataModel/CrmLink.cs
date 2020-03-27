// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmLink.cs" company="Aurea Software Gmbh">
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
//   CRM Link implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Aurea.CRM.Core.CRM.DataModel
{
    using System.Collections.Generic;

    using Extensions;
    using Session;
    using Utilities;

    /// <summary>
    /// CRM Link implementation
    /// </summary>
    /// <seealso cref="Aurea.CRM.Core.Utilities.UPSerializable" />
    public class UPCRMLink : UPSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        public UPCRMLink(UPCRMRecord record, int linkId, bool onlyOffline)
        {
            this.Record = record;
            this.LinkId = linkId;
            this.OnlyOffline = onlyOffline;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public UPCRMLink(UPCRMRecord record, int linkId)
            : this(record, linkId, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        public UPCRMLink(string recordIdentification)
            : this(recordIdentification, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public UPCRMLink(string recordIdentification, int linkId)
            : this(new UPCRMRecord(recordIdentification), linkId, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        public UPCRMLink(string recordIdentification, int linkId, bool onlyOffline)
            : this(new UPCRMRecord(recordIdentification), linkId, onlyOffline)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        public UPCRMLink(string infoAreaId, string recordId)
            : this(infoAreaId, recordId, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        public UPCRMLink(string infoAreaId, string recordId, int linkId)
            : this(new UPCRMRecord(infoAreaId, recordId), linkId, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UPCRMLink"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <param name="onlyOffline">
        /// if set to <c>true</c> [only offline].
        /// </param>
        public UPCRMLink(string infoAreaId, string recordId, int linkId, bool onlyOffline)
            : this(new UPCRMRecord(infoAreaId.InfoAreaIdRecordId(recordId)), linkId, onlyOffline)
        {
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId => this.Record?.InfoAreaId;

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key => this.LinkId > 0 ? $"{this.InfoAreaId}#{this.LinkId}" : this.InfoAreaId;

        /// <summary>
        /// Gets the link identifier.
        /// </summary>
        /// <value>
        /// The link identifier.
        /// </value>
        public int LinkId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [only offline].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [only offline]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlyOffline { get; private set; }

        /// <summary>
        /// Gets the physical information area identifier.
        /// </summary>
        /// <value>
        /// The physical information area identifier.
        /// </value>
        public string PhysicalInfoAreaId => this.Record?.PhysicalInfoAreaId;

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>
        /// The record.
        /// </value>
        public UPCRMRecord Record { get; private set; }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordId => this.Record?.RecordId;

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification => this.Record?.RecordIdentification;

        /// <summary>
        /// Applies the mapped record identifications.
        /// </summary>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        public void ApplyMappedRecordIdentifications(UPRecordIdentificationMapper mapper)
        {
            this.Record.ApplyMappedRecordIdentifications(mapper, true);
        }

        /// <summary>
        /// Jsons this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Json()
        {
            if (this.OnlyOffline)
            {
                return null;
            }

            return new List<object> { this.RecordIdentification, this.LinkId };
        }

        /// <summary>
        /// Links the name of the field.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string LinkFieldName()
        {
            var temp = this.LinkId < 0 ? 0 : this.LinkId;
            return $"LINK_{this.Record.PhysicalInfoAreaId}_{temp}";
        }

        /// <summary>
        /// Matcheses the information area identifier link identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="linkId">
        /// The link identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool MatchesInfoAreaIdLinkId(string infoAreaId, int linkId)
        {
            if (this.InfoAreaId.Equals(infoAreaId))
            {
                return (this.LinkId == linkId || (this.LinkId <= 0 && linkId <= 0)) ? true : false;
            }

            return false;
        }

        /// <summary>
        /// Serializes the specified writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public void Serialize(UPSerializer writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteElementStart("Link");
            writer.WriteAttributeValue("infoAreaId", this.InfoAreaId);
            writer.WriteAttributeIntegerValue("linkId", this.LinkId);
            writer.WriteElementStringValue(this.RecordId);
            writer.WriteElementEnd();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{this.Record.InfoAreaId}:{this.LinkId}: {this.Record.RecordIdentification}";
        }
    }
}
