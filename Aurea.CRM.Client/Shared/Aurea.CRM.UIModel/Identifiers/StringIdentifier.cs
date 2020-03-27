// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringIdentifier.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Identifier for a string field
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Identifiers
{
    /// <summary>
    /// Identifier for a string field
    /// </summary>
    /// <seealso cref="BaseIdentifier" />
    public class StringIdentifier : BaseIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIdentifier"/> class.
        /// </summary>
        /// <param name="stringId">
        /// The string identifier.
        /// </param>
        public StringIdentifier(string stringId)
        {
            this.identifierDescription = stringId;
            this.SetHasWildcardSuffix();
        }

        /// <summary>
        /// Identifiers the with string identifier.
        /// </summary>
        /// <param name="stringId">
        /// The string identifier.
        /// </param>
        /// <returns>
        /// The <see cref="StringIdentifier"/>.
        /// </returns>
        public static StringIdentifier IdentifierWithStringId(string stringId)
        {
            return new StringIdentifier(stringId);
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
            if (!(obj is StringIdentifier))
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
