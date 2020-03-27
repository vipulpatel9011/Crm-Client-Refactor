// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleIdentifier.cs" company="Aurea Software Gmbh">
//   Copyright © 1988-2018 Aurea Software Gmbh. All Rights Reserved.
//   Unless otherwise noted, all materials contained in this Site are copyrights,
//   trademarks, trade dress and/or other intellectual properties, owned,
//   controlled or licensed by update Software AG and may not be used without
//   written consent except as provided in these terms and conditions or in the
//   copyright notice (documents and software) or other proprietary notices
//   provided with the relevant materials.
// </copyright>
// <summary>
//   Identifier for a multiple fields
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Aurea.CRM.UIModel.Identifiers
{
    using System.Collections.Generic;
    using System.Linq;

    using Aurea.CRM.Core.CRM.UIModel;

    /// <summary>
    /// Identifier for a multiple fields
    /// </summary>
    /// <seealso cref="BaseIdentifier" />
    public class MultipleIdentifier : BaseIdentifier
    {
        /// <summary>
        /// The identifiers.
        /// </summary>
        protected List<IIdentifier> identifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleIdentifier"/> class.
        /// </summary>
        /// <param name="identifiers">
        /// The identifiers.
        /// </param>
        public MultipleIdentifier(List<IIdentifier> identifiers)
        {
            this.identifiers = identifiers;
            this.identifierDescription = string.Empty;
            foreach (var identifier in this.identifiers)
            {
                this.identifierDescription = $"{this.identifierDescription},{identifier.IdentifierAsString}";
            }
        }

        /// <summary>
        /// Gets the number of identifiers.
        /// </summary>
        /// <value>
        /// The number of identifiers.
        /// </value>
        public int NumberOfIdentifiers => this.identifiers?.Count ?? 0;

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
            if (!(obj is MultipleIdentifier))
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
        /// Matcheses the identifier.
        /// </summary>
        /// <param name="otherIdentifier">
        /// The other identifier.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool MatchesIdentifier(IIdentifier otherIdentifier)
        {
            return this.identifiers.Aggregate(
                false,
                (current, identifier) => current || identifier.MatchesIdentifier(otherIdentifier));
        }
    }
}
