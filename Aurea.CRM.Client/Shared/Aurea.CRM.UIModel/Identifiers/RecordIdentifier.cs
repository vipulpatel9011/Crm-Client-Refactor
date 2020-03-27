// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordIdentifier.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Identifier for a record
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Identifiers
{
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// Identifier for a record
    /// </summary>
    /// <seealso cref="BaseIdentifier" />
    public class RecordIdentifier : BaseIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdentifier"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        public RecordIdentifier(string infoAreaId, string recordId)
        {
            this.InfoAreaId = infoAreaId;
            this.RecordId = recordId;
            this.identifierDescription = string.IsNullOrEmpty(this.RecordId)
                                             ? $"{this.InfoAreaId}.*"
                                             : $"{this.InfoAreaId}.{this.RecordId}.*";

            this.SetHasWildcardSuffix();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordIdentifier"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        public RecordIdentifier(string recordIdentification)
            : this(recordIdentification.InfoAreaId(), recordIdentification.RecordId())
        {
        }

        /// <summary>
        /// Gets the information area identifier.
        /// </summary>
        /// <value>
        /// The information area identifier.
        /// </value>
        public string InfoAreaId { get; private set; }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        public string RecordId { get; private set; }

        /// <summary>
        /// Gets the record identification.
        /// </summary>
        /// <value>
        /// The record identification.
        /// </value>
        public string RecordIdentification => this.InfoAreaId.InfoAreaIdRecordId(this.RecordId);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is RecordIdentifier))
            {
                return false;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Identifiers the with field identifier.
        /// </summary>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldIdentifier"/>.
        /// </returns>
        public FieldIdentifier IdentifierWithFieldId(string fieldId)
        {
            return new FieldIdentifier(this.InfoAreaId, this.RecordId, fieldId);
        }
    }
}
