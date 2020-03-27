// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldIdentifier.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   The identifier for a field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Identifiers
{
    using Aurea.CRM.Core.Extensions;

    /// <summary>
    /// The identifier for a field
    /// </summary>
    /// <seealso cref="BaseIdentifier" />
    public class FieldIdentifier : BaseIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIdentifier"/> class.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        public FieldIdentifier(string infoAreaId, string recordId, string fieldId)
        {
            this.InfoAreaId = infoAreaId;
            this.RecordId = recordId;
            this.FieldId = fieldId;
            this.identifierDescription = $"{this.InfoAreaId}.{this.RecordId}.{this.FieldId}";
            this.SetHasWildcardSuffix();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldIdentifier"/> class.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        public FieldIdentifier(string recordIdentification, string fieldId)
            : this(recordIdentification.InfoAreaId(), recordIdentification.RecordId(), fieldId)
        {
        }

        /// <summary>
        /// Gets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier.
        /// </value>
        public string FieldId { get; private set; }

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
        /// Identifiers the with information area identifier record identifier field identifier.
        /// </summary>
        /// <param name="infoAreaId">
        /// The information area identifier.
        /// </param>
        /// <param name="recordId">
        /// The record identifier.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldIdentifier"/>.
        /// </returns>
        public static FieldIdentifier IdentifierWithInfoAreaIdRecordIdFieldId(
            string infoAreaId,
            string recordId,
            string fieldId)
        {
            return new FieldIdentifier(infoAreaId, recordId, fieldId);
        }

        /// <summary>
        /// Identifiers the with record identification field identifier.
        /// </summary>
        /// <param name="recordIdentification">
        /// The record identification.
        /// </param>
        /// <param name="fieldId">
        /// The field identifier.
        /// </param>
        /// <returns>
        /// The <see cref="FieldIdentifier"/>.
        /// </returns>
        public static FieldIdentifier IdentifierWithRecordIdentificationFieldId(
            string recordIdentification,
            string fieldId)
        {
            return new FieldIdentifier(recordIdentification, fieldId);
        }

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
            if (!(obj is FieldIdentifier))
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
    }
}
